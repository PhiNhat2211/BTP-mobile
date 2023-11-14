using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;

using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using System.Runtime.InteropServices;
using Common.Util;
using System.Reflection;
using System.Text.RegularExpressions;

//20190108
using Common.Interface;
using System.Windows.Interop;
using System.Net.NetworkInformation;
using System.IO;
using Microsoft.Win32;
using static Common.Util.Registry64;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        #region [Build Configuration]
        //----------------------------------------------
        //- Build Relation Configuration
        // Common for All Site
        static public String KeyCLTAgent = @"SOFTWARE\CyberLogitec\CLT Agent for Windows";
        static public Boolean TEST_MODE = false; // true - test mode, false - real mode
        static public Boolean TEST_WRITE_MODE = false; // TRUE - Write log to File
        static public Boolean STANDALONE_MODE = false; // true - stand alone mode, false - real mode
        static public Boolean MESSAGE_CAPTURE_MODE = false; // true - stand alone mode, false - real mode

        //----------------------------------------------
        //- ITV for JAT2
        static public String SERVICE_COMPANY = "BTP";  // NCT:NCT   JAT2:JAT2
        //static public String Project = @"BTP-ALPHA";
        //static public String Project = @"BTP-BRAVO";
        static public String Project = @"BTP-PROD";
        //static public String Project = @"BTP-DEV";
        //static public String Project = @"BTP-QAS";
        static public String KeyCLTVMT_ITV = @"SOFTWARE\CyberLogitec\VMT-ITV for " + Project; // NCT:NCT   JAT2:JAT2
        static public String DefUpdateServerAddr = "172.19.51.17:56000";  // Default Update Server URL
        public const String VMT_EngineDLL = "EE_Sensor_JA.dll";

        //----------------------------------------------
        //- ITV for NCT
        //static public String SERVICE_COMPANY = "NCT";  // NCT:NCT   JAT2:JAT2
        //static public String KeyCLTVMT_ITV = @"SOFTWARE\CyberLogitec\VMT-ITV for NCT"; // NCT:NCT   JAT2:JAT2
        ////static public String DefUpdateServerAddr = "218.153.116.97:59000";  // Default Update Server URL
        //static public String DefUpdateServerAddr = "10.200.205.81:59000";  // Default Update Server URL
        //public const string VMT_EngineDLL = "EE_Sensor_KAP.dll";

        public const String VMT_DataMgrDLL = "VMT_DataMgr.dll";
        #endregion [Build Configuration]

        #region [variables]
        public const int MESSAGE_TO_EEVMT_LOGIN = 0x10001;
        public const int MESSAGE_TO_EEVMT_CONNECT = 0x10002;

        public Boolean gIsDay = true;
        public String preChassisCd = "";

        public static LogWindow LogWin = null;

        //ITV.sITV_PDS_Periodic_Payload_Recv Value;
        ITV.VD_ITV_PDS_Periodic_Payload Value;

        static public System.Media.SoundPlayer gSoundPlayer;
        static public System.Media.SoundPlayer gSoundPlayer_dingdong;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_Background;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_Foreground;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_Notify_System_Generic;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_User_Account_Control;
        static public MediaPlayer gMediaPlayer_Windows_Notify_System_Generic;
        static public int MessagePopupDelayTime = 0;
        static public bool ShowAppUI = true;
        static public string DGPSDirectionPinPolarity = "";
        static public int nHideTextBoxMilliseconds = 20000; // 20 sec
        static public bool ButtonEnable = true;
        //static public bool logOff = false;
        static public bool restart = false;
        static public bool login = false;
        static public bool changeChssNo = false;
        //private bool setTimer = false;
        private string timer = "";
        DateTime date = new DateTime();
        public Boolean isLoggedWhenDiscon = true;

        public Boolean gIsServerConnected = false;
        public int countKeepAliveFail = 0;
        DispatcherTimer Timer = new DispatcherTimer();
        #endregion [variables]

        public MainWindow()
        {
            InitializeComponent();

            ITV.ITV_User.IsUseUDP = true;
            Timer.Interval = new TimeSpan(0, 0, 1); ;//ticks every 1 second
            Timer.Tick += new EventHandler(Timer_Click); ;
            Timer.Start();
            // Initialize Logger
            //DateTime time = DateTime.Now;             // Use current time
            //string formatFile = "yyyy-MM-dd_HH";      // Use this format
            //string filetag = time.ToString(formatFile);   // Write to console
            //string logFile = string.Format(@"Log\HessianLog_{0}.txt", filetag);

            // Initialize Machine
            int size = 0;
            ITV.ITV_User.GetMachineID(ref ITV.ITV_User.gMchnID, ref size);
            ITV.ITV_User.GetMachineType(ref ITV.ITV_User.gMchnTp, ref size);

            // Loading Application Configuration File
            this.LoadAppCfg();

            // Loading Application Setting File
            this.LoadAppSetting();

            string strMaxSizeRollBackups = AppCfgMgr.Singleton.GetValueByKey("MaxSizeRollBackups");
            int nMaxSizeRollBackups = 0;
            if (int.TryParse(strMaxSizeRollBackups, out nMaxSizeRollBackups)) { }
            else
                nMaxSizeRollBackups = 7;

            string strMaximumFileSize = AppCfgMgr.Singleton.GetValueByKey("MaximumFileSize");

            Logger.SetLogPath("HessianLog");
            Logger.SetLogFilters(Logger.LogFilters.Debug);
            Logger.SetLogPolicy(nMaxSizeRollBackups, strMaximumFileSize);

            // event
            this.Closed += new EventHandler(MainWindow_Closed);

            // Init Shortcut Input
            InitKeyInput();

            if (!ShowAppUI)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }

            // Create LogWindow
            MainWindow.LogWin = new LogWindow();
            if (TEST_MODE)
            {
                LogWin.Show();
            }

            gSoundPlayer = new System.Media.SoundPlayer(Properties.Resources._2014050108012211);
            gSoundPlayer_dingdong = new System.Media.SoundPlayer(Properties.Resources.dingdong_3);
            gSoundPlayer_Windows_Background = new System.Media.SoundPlayer(Properties.Resources.Windows_Background_soundup);
            gSoundPlayer_Windows_Foreground = new System.Media.SoundPlayer(Properties.Resources.Windows_Foreground_soundup);
            gSoundPlayer_Windows_Notify_System_Generic = new System.Media.SoundPlayer(Properties.Resources.Windows_Notify_System_Generic_soundup);
            gSoundPlayer_Windows_User_Account_Control = new System.Media.SoundPlayer(Properties.Resources.Windows_User_Account_Control_soundup);
            gMediaPlayer_Windows_Notify_System_Generic = new MediaPlayer();
            //gMediaPlayer_Windows_Notify_System_Generic.Open(new Uri("pack://siteoforigin:,,,/sound/Windows_Notify_System_Generic.wav") );

            if (VMT_DataMgr.CreateVMTClient(STANDALONE_MODE))
            {
                IndicatorView.Init(this);
                LoginView.Init(this);
                MainView.Init(this);
                MainView.FlowString.Init(this);
                PopupView.Init(this);
                PinningStationPopup.Init(this);
                ByPassPopup.Init(this);
                RestartPopup.Init(this);
                PopupProgressView.Init(this);
                KeypadView.Init(this);
                WifiPopup.Init(this);
                CalibrationInfoView.Init(this);
                CalibrationInitPopup.Init(this);
                ChassisNumberView.Init(this);

                FlowText.instance().Init(this);

                InitAppCallbackFunctions();

                VMT_Data_JAT2.Objects.Common.VD_Common_Swinfo_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_Swinfo_Send();

                String ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                String[] vn = ver.Split('.');
                value.m_VMTUI_Version = vn[0] + "." + vn[1] + "." + vn[2] + "." + vn[3];
                value.m_szMchnID = ITV.ITV_User.gMchnID;
                value.m_szMchnTp = ITV.ITV_User.gMchnTp;

                VMT_DataMgr_Common.SoftwareInfo_Ask(ref value);
                VMT_DataMgr_Common.ConnectionStatus_Ask();
            }
            else
            {
                IndicatorView.Init(this);
                LoginView.Init(this);
                MainView.Init(this);
                MainView.FlowString.Init(this);
                PopupView.Init(this);
                PinningStationPopup.Init(this);
                ByPassPopup.Init(this);
                RestartPopup.Init(this);
                PopupProgressView.Init(this);
                KeypadView.Init(this);
                ChassisNumberView.Init(this);
                CalibrationInitPopup.Init(this);
                WifiPopup.Init(this);

                FlowText.instance().Init(this);
            }
            //   btn_test1.Visibility = System.Windows.Visibility.Visible;
            //VMT_DataMgr_Common.GetMachineStopCodeList_Ask(); move to after login
            IndicatorView.InitGeoFence();

            // test // TODO 
            // TeamSelectPopup.Init(this);
            // TeamSelectPopup.Visibility = System.Windows.Visibility.Visible;
            // TeamSelectPopup.AddTeamList();

            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, KeyCLTVMT_ITV);
            if (uIntPtr.ToUInt32() > 0)
            {
                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                String regValue = GetRegValue(uIntPtr, "IsFirstRun");
                if (String.IsNullOrEmpty(regValue) || !regValue.Equals(ver.Revision.ToString()))
                {
                    TrySetRegValue(uIntPtr, "IsFirstRun", ver.ToString());
                }
            }

            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(KeyCLTVMT_ITV, true);
            //if (keyDir != null)
            //{
            //    String regValue = (String)keyDir.GetValue("IsFirstRun", "");
            //    Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            //    if (String.IsNullOrEmpty(regValue) || !regValue.Equals(ver.Revision.ToString()))
            //    {
            //        keyDir.SetValue("IsFirstRun", ver.Revision.ToString(), Microsoft.Win32.RegistryValueKind.String);

            //        // 1 Sec
            //        //new PresentationMgr.SingleShot(1000, SystemRestart);
            //    }
            //}

            SetWindowSizeNPosition();
        }



        private double _DesignWidth = 800;
        private double _DesignHeight = 600;
        public void SetWindowSizeNPosition()
        {
            double n_sWidth = (double)_DesignWidth;
            double n_sHeight = (double)_DesignHeight;

            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);

            string uiSize = ini.IniReadValue("MACHINE", "UISIZE");
            if (uiSize == "1024")
            {
                n_sWidth = 1024;
                n_sHeight = 768;
            }
            else
            {
                n_sWidth = 1280;
                n_sHeight = 800;
            }

            this.Width = n_sWidth;
            this.Height = n_sHeight;

            this.Grid_Scale.CenterX = .5;
            this.Grid_Scale.CenterY = .5;
            this.Grid_Scale.ScaleX = n_sWidth / _DesignWidth;
            this.Grid_Scale.ScaleY = n_sHeight / _DesignHeight;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // private DispatcherTimer TimerSystemRestart = null;
        private void SystemRestart()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.UC_SystemRestartPopup._nSystemRestart_Sec = 20;

                            //if (TimerSystemRestart == null)
                            //{
                            //    TimerSystemRestart = new DispatcherTimer();
                            //    TimerSystemRestart.Interval = new TimeSpan(0, 0, 1);
                            //    TimerSystemRestart.Tick += new EventHandler(SystemRestart_Handler);                
                            //    TimerSystemRestart.Start();
                            //}

                            SystemRestart_Handler(null, null);
                        }));
        }

        private void SystemRestart_Handler(object sender, EventArgs e)
        {
            this.UC_SystemRestartPopup.Visibility = System.Windows.Visibility.Visible;
            this.UC_SystemRestartPopup.decreaseOneSec();
        }

        public void HideSystemRestartPopup()
        {
            //if (TimerSystemRestart != null)
            //    TimerSystemRestart.Stop();

            //TimerSystemRestart = null;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            VMT_DataMgr.DestroyVMTClient();

            Environment.Exit(0);
        }

        ~MainWindow()
        {
            // VMT_DataMgr.DestroyVMTClient();
        }

        void InitKeyInput()
        {
            this.PreviewKeyDown += new KeyEventHandler(MainWindow_PreviewKeyDown);
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //EventMgr.Singleton.Process_KeyDown(sender, e); //20191031 don't need use hotkeys
        }

        #region [Application Configuration Methods]

        public void LoadAppCfg()
        {
            string strCfgPath = "";

            if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
                strCfgPath = AppCfgMgr.GetAppDirectory() + "JAT2_VMT_ITV.cfg.xml";

            // Loading Application Config XML Document.
            AppCfgMgr.Singleton.LoadFile(strCfgPath);

            string strTestMode;
            strTestMode = AppCfgMgr.Singleton.GetValueByKey("IsTestMode");
            if (strTestMode == "1") TEST_MODE = true; // true - test mode, false - real mode
            else TEST_MODE = false; // true - test mode, false - real mode

            string strTestWriteMode;
            strTestWriteMode = AppCfgMgr.Singleton.GetValueByKey("IsTestWriteMode");
            if (strTestWriteMode == "1") TEST_WRITE_MODE = true;
            else TEST_WRITE_MODE = false;

            string strStandAlone;
            strStandAlone = AppCfgMgr.Singleton.GetValueByKey("IsStandAlone");
            if (strStandAlone == "1") STANDALONE_MODE = true; // true - stand alone mode, false - real mode
            else STANDALONE_MODE = false; // true - stand alone mode, false - real mode

            string strMessageCapture;
            strMessageCapture = AppCfgMgr.Singleton.GetValueByKey("IsMessageCapture");
            if (strMessageCapture == "1") MESSAGE_CAPTURE_MODE = true; // true - stand alone mode, false - real mode
            else MESSAGE_CAPTURE_MODE = false; // true - stand alone mode, false - real mode

            // Temp Value
            string cfgValue = "";
            cfgValue = AppCfgMgr.Singleton.GetValueByKey("MessagePopUpDelay");
            MainWindow.MessagePopupDelayTime = int.Parse(cfgValue);

            cfgValue = AppCfgMgr.Singleton.GetValueByKey("ShowAppUI");
            if (cfgValue == "0")
                MainWindow.ShowAppUI = false;

            cfgValue = AppCfgMgr.Singleton.GetValueByKey("IsStartUp");
            String RegAddr = @"Software\Microsoft\Windows\CurrentVersion\Run";
            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, RegAddr);
            if (uIntPtr.ToUInt32() > 0)
            {
                String regKey = KeyCLTVMT_ITV.Substring(KeyCLTVMT_ITV.LastIndexOf('\\') + 1);
                String regValue = GetRegValue(uIntPtr, regKey);
                if (!String.IsNullOrEmpty(regValue))
                {
                    TryDeleteRegValue(uIntPtr, regKey);
                }
            }
            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegAddr, true);
            //if (keyDir != null)
            //{
            //    String regKey = KeyCLTVMT_ITV.Substring(KeyCLTVMT_ITV.LastIndexOf('\\') + 1); // productName
            //    String regValue = (String)keyDir.GetValue(regKey, ""); // ProductPath

            //    //if (cfgValue.Equals("0"))
            //    //{
            //        if (!String.IsNullOrEmpty(regValue))
            //        {
            //            keyDir.DeleteValue(regKey, false);
            //        }
            //    //}
            //    //else
            //    //{
            //    //    if (String.IsNullOrEmpty(regValue))
            //    //    {
            //    //        keyDir.SetValue(regKey, "\"" + AppCfgMgr.GetApplicationPath(true) + "\"", Microsoft.Win32.RegistryValueKind.String);
            //    //    }
            //    //}
            //}

            VMT_Data_JAT2.VMT_DataMgr_Common.GetVmtAutoStartConfig_Ask();

            cfgValue = AppCfgMgr.Singleton.GetValueByKey("ButtonEnable");
            if (cfgValue.Equals("0"))
            {
                ButtonEnable = false;
            }
            else
            {
                ButtonEnable = true;
            }

            cfgValue = AppCfgMgr.Singleton.GetValueByKey("PinningStationExposureTime");
            MainWindow.nHideTextBoxMilliseconds = int.Parse(cfgValue);

            string strHessianServerIP = AppCfgMgr.Singleton.GetValueByKey("HessianServerIP");
            VMT_DataMgr.gHessianServerIP = strHessianServerIP;
            string strHessianServerPort = AppCfgMgr.Singleton.GetValueByKey("HessianServerPort");
            VMT_DataMgr.gHessianServerPort = strHessianServerPort;

            string strMchnID = ITV.ITV_User.gMchnID;
            //strMchnID = strMchnID.Replace("T", "");
            strMchnID = Regex.Match(strMchnID, @"(\d+)$").Value;
            var portNum = String.IsNullOrEmpty(strMchnID) ? 0 : Convert.ToInt32(strMchnID);

            string strBaseClientPort = AppCfgMgr.Singleton.GetValueByKey("BaseClientPort");
            VMT_DataMgr.gUDPClientPort = (portNum +
                (String.IsNullOrEmpty(strBaseClientPort) ? 0 : Convert.ToInt32(strBaseClientPort))
                ).ToString();
            string strBaseServerPort = AppCfgMgr.Singleton.GetValueByKey("BaseServerPort");
            VMT_DataMgr.gUDPServerPort = (portNum +
                (String.IsNullOrEmpty(strBaseServerPort) ? 0 : Convert.ToInt32(strBaseServerPort))
                ).ToString();

            //string strTCPBypassPort = AppCfgMgr.Singleton.GetValueByKey("TCPBypassPort");
            //VMT_DataMgr_Common.gTCPBypassPort = Convert.ToInt32(strTCPBypassPort).ToString();

            string strGeoFence_LT_Latitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_LT_Latitude");
            double dGeoFence_LT_Latitude = Convert.ToSingle(strGeoFence_LT_Latitude);
            string strGeoFence_LT_Longitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_LT_Longitude");
            double dGeoFence_LT_Longitude = Convert.ToSingle(strGeoFence_LT_Longitude);

            string strGeoFence_RT_Latitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_RT_Latitude");
            double dGeoFence_RT_Latitude = Convert.ToSingle(strGeoFence_RT_Latitude);
            string strGeoFence_RT_Longitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_RT_Longitude");
            double dGeoFence_RT_Longitude = Convert.ToSingle(strGeoFence_RT_Longitude);

            string strGeoFence_RB_Latitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_RB_Latitude");
            double dGeoFence_RB_Latitude = Convert.ToSingle(strGeoFence_RB_Latitude);
            string strGeoFence_RB_Longitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_RB_Longitude");
            double dGeoFence_RB_Longitude = Convert.ToSingle(strGeoFence_RB_Longitude);

            string strGeoFence_LB_Latitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_LB_Latitude");
            double dGeoFence_LB_Latitude = Convert.ToSingle(strGeoFence_LB_Latitude);
            string strGeoFence_LB_Longitude = AppCfgMgr.Singleton.GetValueByKey("GeoFence_LB_Longitude");
            double dGeoFence_LB_Longitude = Convert.ToSingle(strGeoFence_LB_Longitude);

            IndicatorView.GeoFence_LT = new Point(dGeoFence_LT_Latitude, dGeoFence_LT_Longitude);
            IndicatorView.GeoFence_RT = new Point(dGeoFence_RT_Latitude, dGeoFence_RT_Longitude);
            IndicatorView.GeoFence_RB = new Point(dGeoFence_RB_Latitude, dGeoFence_RB_Longitude);
            IndicatorView.GeoFence_LB = new Point(dGeoFence_LB_Latitude, dGeoFence_LB_Longitude);

            // Set Window Position
            //this.Top = int.Parse(AppCfgMgr.Singleton.GetValueByKey("Top"));
            //this.Left = int.Parse(AppCfgMgr.Singleton.GetValueByKey("Left"));
        }

        public void LoadAppSetting()
        {
            String strSettingsKey = String.Empty;
            strSettingsKey = AppCfgMgr.GetAppDirectory() + @"appSetting.config";

            AppSettings.Local.SettingsKey = strSettingsKey;
            AppSettings.Local.Reload();
        }

        public void SaveAppCfg()
        {
            AppCfgMgr.Singleton.SetValueByKey("MessagePopUpDelay", MessagePopupDelayTime.ToString());

            AppCfgMgr.Singleton.SaveFile();
        }

        #endregion [Application Configuration Methods]

        private void InitAppCallbackFunctions()
        {
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyHandleLogApi(new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyHandleLogApi(NotifyHandleLogApi));

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGPSStatus = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyGPSStatus(NotifyGPSStatus);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGPSStatus(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGPSStatus);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGPS = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyGPS(NotifyGPS);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGPS(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGPS);

            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyWIFIStatus(NotifyWIFIStatus);
            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyWIFIStatus(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyKeepAlive(NotifyKeepAlive);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyKeepAlive(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineNotice = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.CallBack_NotifyMachineNotice(NotifyMessage);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineNotice(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineNotice);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyAccessRole = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyAccessRole(NotifyAccessRole);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyAccessRole(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyAccessRole);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyLogin4Machine(NotifyLogin4Machine);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyLogin4Machine(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyChangeDriverCheck(new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyChangeDriverCheck(NotifyChangeDriverCheck));

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyConfig = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyConfig(NotifyConfig);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyConfig(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyConfig);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyGetVmtAutoStartConfig(new VMT_DataMgr_Common_Callback.Callback_NotifyGetVmtAutoStartConfig(NotifyGetVmtAutoStartConfig));

            //VMT_DataMgr_Common_Callback.static_NotifyLogIn = new VMT_DataMgr_Common_Callback.Callback_NotifyLogIn(NotifyLogIn);
            //VMT_DataMgr_Common_Callback.SetCallBack_NotifyLoginInfo(VMT_DataMgr_Common_Callback.static_NotifyLogIn);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyMachineStatusChanged(NotifyMachineStatusChanged);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineStatusChanged(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged);

            /*NotifyJobOrder는 아래*/

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobChange = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobChange(NotifyJobChange);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobChange(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobChange);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDelete = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobDelete(NotifyJobDelete);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobDelete(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDelete);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDeleteAll = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobDeleteAll(NotifyJobDeleteAll);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobDeleteAll(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDeleteAll);

            VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobDone = new VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.CallBack_NotifyJobDone(NotifyJobDone);
            VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.SetCallBack_NotifyJobDone(VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobDone);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStopCodeList = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyMachineStopCodeList(NotifyMachineStopCodeList);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineStopCodeList(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStopCodeList);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyGetMachineStop(NotifyGetMachineStop);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyAvailable(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineStop = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySetMachineStop(NotifySetMachineStop);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachineStop(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineStop);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStopConfirm = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyMachineStopConfirm(NotifyMachineStopConfirm);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineStopConfirm(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStopConfirm);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyErrorCode = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyErrorCode(NotifyErrorCode);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyErrorCode(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyErrorCode);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyCFGEKF = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyCFGEKF(NotifyCFGEKF);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyCFGEKF(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyCFGEKF);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyAlarm = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyAlarm(NotifyAlarm);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyAlarm(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyAlarm);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_PDS_Periodic_Payload = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_ITV_PDS_Periodic_Payload(NotifyITV_Periodic);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_ITV_PDS_Periodic_Payload(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_PDS_Periodic_Payload);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_PDS_Event_Payload = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_ITV_PDS_Event_Payload(NotifyITV_Event);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_ITV_PDS_Event_Payload(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_PDS_Event_Payload);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyChassis_Attach = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_ITV_NotifyChassis_Attach(NotifyChassis_Attach);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_ITV_NotifyChassis_Attach(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyChassis_Attach);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyJobOrderITV = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyJobOrderITV(NotifyJobOrderITV);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyJobOrderITV(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyJobOrderITV);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyGetMachineStatusChanged = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyGetMachineStatusChanged(NotifyGetMachineStatusChanged);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyGetMachineStatusChanged(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyGetMachineStatusChanged);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifySetMachineArrival(NotifySetMachineArrival);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifySetMachineArrival(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineReady = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifySetMachineReady(NotifySetMachineReady);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifySetMachineReady(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineReady);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetItvDone = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifySetItvDone(NotifySetItvDone);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifySetItvDone(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetItvDone);

            // Set Job Done For QC
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetQCJobReleaseByYt = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifySetQCJobDoneByYt(NotifySetQCJobDoneByYt);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifySetQCJobDoneByYt(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetQCJobReleaseByYt);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyChangeChassisNo = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyChangeChassisNo(NotifyChangeChassisNo);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyChangeChassisNo(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyChangeChassisNo);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyChangeDriver(new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyChangeDriver(NotifyChangeDriver));

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyGetChssUsingData = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyGetChssUsingData(NotifyGetChssUsingData);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyGetChssUsingData(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyGetChssUsingData);

            VMT_DataMgr_Common_Callback.SetCallback_NotifyException(new VMT_DataMgr_Common_Callback.Callback_NotifyException(NotifyException));

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyBlockEnterance = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_BlockEnterance(NotifyBlockEnterance);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_BlockEnterance(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyBlockEnterance);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyCPSAlign = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_CPSAlign(NotifyCPSAlign);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_NotifyCPSAlign(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyCPSAlign);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyConfirmJobByScanner = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyConfirmJobByScanner(NotifyConfirmJobByScanner);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_NotifyConfirmJobByScanner(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyConfirmJobByScanner);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifySTSLDSeq = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifySTSLDSeq(NotifySTSLDSeq);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_NotifySTSLDSeq(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifySTSLDSeq);

            if (MainWindow.SERVICE_COMPANY.Equals("JAT2") ||
                MainWindow.SERVICE_COMPANY.Equals("JAT3"))
            {
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyPinningStation = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyPinningStation(NotifyPinningStation);
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyPinningStation(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyPinningStation);
            }

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyArrvdMchnAtPow = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyArrvdMchnAtPow(NotifyArrvdMchnAtPow);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyArrvdMchnAtPow(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyArrvdMchnAtPow);

            //VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyPowOut = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyPowOut(NotifyPowOut);
            //VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallBack_NotifyPowOut(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyPowOut);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyEngineTemp = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyEngineTemp(NotifyEngineTemp);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyEngineTemp(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyEngineTemp);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyFuelGage = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyFuelGage(NotifyFuelGage);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyFuelGage(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyFuelGage);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySpeedKm = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySpeedKm(NotifySpeedKm);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySpeedKm(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySpeedKm);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyCalibration = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyCalibration(NotifyCalibration);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyCalibration(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyCalibration);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyValidChassisInfos = new VMT_DataMgr_ITV_Callback.Callback_NotifyValidChassisInfos(NotifyValidChassisInfos);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_NotifyValidChassisInfos(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyValidChassisInfos);

            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockList(NotifyBlockList));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapList(NotifyBlockMapList));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapListForYt(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapListForYt(NotifyBlockMapListForYt));

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogout = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyLogout(NotifyLogout);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyLogout(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogout);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyChassisOrderComplete = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyChassisOrderCompletes(ChassisOrderCompletes);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_NotifyChassisOrderCompletes(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyChassisOrderComplete);

            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyReleaseYtFromJob = new VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.Callback_NotifyReleaseYtFromJob(ReleaseYtFromJob);
            VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.SetCallback_NotifyReleaseYtFromJob(VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyReleaseYtFromJob);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetConfigValue = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyGetConfigValue(GetConfigValue);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyGetConfigValue(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetConfigValue);
        }

        private void MainWin_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.LogWin.Owner = this;

            if (!ShowAppUI)
            {
                //this.Visibility = System.Windows.Visibility.Hidden;
                //this.WindowState = System.Windows.WindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
            {
                // Run LiveUpdate
                PresentationMgr.FileTouchEvent_RunUpdate();

                if (TEST_MODE || STANDALONE_MODE)
                    return;

                //this.ShowProgressBar(MESSAGE_TO_EEVMT_CONNECT, "Checking update version...");
                //new PresentationMgr.SingleShot(15000, HideUpdateCheckProgress); // 15 sec
            }
        }

        private void HideUpdateCheckProgress()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(MESSAGE_TO_EEVMT_CONNECT);
                        }));
        }

        private void MainWin_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void uc_IndicatorView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // IndicatorView.Init(this);
        }

        private void LogInView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // LoginView.Init(this);
        }

        private void uc_MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // MainView.Init(this);
        }

        private void PopupView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // PopupView.Init(this);
        }

        private void PopupProgressView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // PopupProgressView.Init(this);
        }

        public void gotoLogInView()
        {
            MainView.Visibility = System.Windows.Visibility.Collapsed;
            LoginView.Visibility = System.Windows.Visibility.Visible;
        }

        public void gotoMainView()
        {
            MainView.Visibility = System.Windows.Visibility.Visible;
            LoginView.Visibility = System.Windows.Visibility.Collapsed;
            ChassisNumberView.Visibility = System.Windows.Visibility.Collapsed;
            IndicatorView.ChangeMainView();
            
        }

        public void LogOut(UserControl curWin)
        {
            // LoginView.logInSessionInfo.logInStatus = LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_Logout; // Exceptional Version 3.0.0.36

            LoginView.mIsLogin = false;
            //MainView Collapsed.
            curWin.Visibility = System.Windows.Visibility.Collapsed;
            //LoginView.Visibility = System.Windows.Visibility.Visible;
            LoginView.ReFlash();
            //IndicatorView.ReFlash();
            //LoginView.Init();
        }

        /*
         *  @"Images/MainView/day/Container/Active";
            @"Images/MainView/day/Container/Blank";
            @"Images/MainView/day/Container/Full";
            @"Images/MainView/day/Container/Processing";
         */
        // utils
        public BitmapImage getContainerImageByDayOrNight(String path, ITV.ITV_Container_Type containerTp)
        {
            String imagePath;
            BitmapImage bitmap = null;

            if (gIsDay)
            {
                if (containerTp == ITV.ITV_Container_Type.Blank)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/Container/Blank/" + path;
                }
                else if (containerTp == ITV.ITV_Container_Type.Active)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/Container/Active/" + path;
                }
                else if (containerTp == ITV.ITV_Container_Type.Processing)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/Container/Processing/" + path;
                }
                else if (containerTp == ITV.ITV_Container_Type.Full)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/Container/Full/" + path;
                }
                else
                {
                    // Error
                    imagePath = "";
                }
            }
            else
            {
                if (containerTp == ITV.ITV_Container_Type.Blank)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/Container/Blank/" + path;
                }
                else if (containerTp == ITV.ITV_Container_Type.Active)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/Container/Active/" + path;
                }
                else if (containerTp == ITV.ITV_Container_Type.Processing)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/Container/Processing/" + path;
                }
                else if (containerTp == ITV.ITV_Container_Type.Full)
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/Container/Full/" + path;
                }
                else
                {
                    // Error
                    imagePath = "";
                }
            }

            try
            {
                bitmap = new BitmapImage(new Uri(imagePath));
            }
            catch (Exception ex)
            {
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/" + path;
                bitmap = new BitmapImage(new Uri(imagePath));
            }

            return bitmap;
        }

        public BitmapImage getImageByDayOrNight(String path)
        {
            String imagePath;
            BitmapImage bitmap = null;

            if (path.IndexOf("wifi") < 0 && path.IndexOf("gps") < 0)
            {
                if (gIsDay)
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/" + path;
                else
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/(night)_" + path;
                }
            }
            else
            {
                if (gIsDay)
                {
                    if (MainView.Visibility == System.Windows.Visibility.Visible)
                    {
                        imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/" + path;
                    }
                    else
                    {
                        imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/(night)_" + path;
                    }
                }
                else
                {
                    imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/night/(night)_" + path;
                }
            }

            try
            {
                bitmap = new BitmapImage(new Uri(imagePath));
            }
            catch (Exception ex)
            {
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/MainView/day/" + path;
                bitmap = new BitmapImage(new Uri(imagePath));
            }

            return bitmap;
        }

        public BitmapImage getImageByDayOrNightForLogIn(String path)
        {
            String imagePath;
            BitmapImage bitmap = null;
            if (gIsDay)
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/LogInView/day/" + path;
            else
            {
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/LogInView/night/(night)_" + path;

            }

            try
            {
                bitmap = new BitmapImage(new Uri(imagePath));
            }
            catch (Exception ex)
            {
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/LogInView/day/" + path;
                bitmap = new BitmapImage(new Uri(imagePath));
            }

            return bitmap;
        }

        public BitmapImage getImageByDayOrNightForProgress(String path)
        {
            String imagePath;
            BitmapImage bitmap = null;
            if (gIsDay)
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/PopupProgress/day/" + path;
            else
            {
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/PopupProgress/night/(night)_" + path;

            }

            try
            {
                bitmap = new BitmapImage(new Uri(imagePath));
            }
            catch (Exception ex)
            {
                imagePath = @"pack://application:,,,/VMT_ITV;component/Images/PopupProgress/day/" + path;
                bitmap = new BitmapImage(new Uri(imagePath));
            }

            return bitmap;
        }

        public BitmapImage getImageByDayOrNightForPopup(String path)
        {
            String imagePath;
            BitmapImage bitmap = null;

            imagePath = @"pack://application:,,,/VMT_ITV;component/Images/Popup/" + path;

            bitmap = new BitmapImage(new Uri(imagePath));

            return bitmap;
        }
        public void changeDayOrNight(Boolean day)
        {
            gIsDay = day;
        }

        public void ShowProgressBar(int message, string text = "")
        {
            PopupProgressView.Visibility = System.Windows.Visibility.Visible;
            PopupProgressView.StartAnimation(message, text);
        }

        public void HideProgressBar(int message)
        {
            PopupProgressView.Visibility = System.Windows.Visibility.Collapsed;
            PopupProgressView.StopAnimation(message);
        }

        public void ShowProgressBarTimer(int sec)
        {
            PopupProgressView.Visibility = System.Windows.Visibility.Visible;
            PopupProgressView.StartProgressBar_Timer(sec);
        }

        public void HideProgressBarTimer()
        {
            PopupProgressView.Visibility = System.Windows.Visibility.Collapsed;
            PopupProgressView.StopProgrssBar_Timer();
            CalibrationInfoView.End_Device_Init();
        }

        public void HidePopup()
        {
            PopupView.Visibility = System.Windows.Visibility.Collapsed;
            PopupView.TextBlock_popup_message.Text = "";
        }


        #region [Notify]

        #region [NotifyHandleLogApi]
        public String typeToCheck = String.Empty;
        public DateTime timeToCheck = DateTime.Now;
        public void NotifyHandleLogApi(bool isSend, HessianComm.HessianCommType type, Object obj)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            String typeToLog = Convert.ToString(type).Replace("_New", "").Replace("Background", "").Replace("Data", "").Replace("Multi", "4Multi").Replace("44Multi", "4Multi");
                            String jsonObj = JsonConvert.SerializeObject(obj);
                            String receiveTimeMsStr = String.Empty;
                            String ipToPing = VMT_DataMgr.gHessianServerIP;
                            // Aug 11 2023 Log to LogWin or File
                            if (isSend)
                            {
                                typeToCheck = typeToLog;
                                timeToCheck = DateTime.Now;
                                if (TEST_MODE)
                                {
                                    if (typeToLog.ToUpper().Contains("KEEPALIVE"))
                                    {
                                        LogWin.WriteLog("");
                                        LogWin.WriteLog("MAC: " + GetSystemMacStr() + " | IP: " + GetDeviceIpStr() + " | PING TO " + ipToPing + ": " + GetPingTimeAverageStr(ipToPing, 4));
                                        LogWin.WriteLog("");
                                    }
                                    LogWin.WriteLog("SEND: " + typeToLog + " | " + jsonObj, true);
                                }
                                if (TEST_WRITE_MODE)
                                {
                                    if (typeToLog.ToUpper().Contains("KEEPALIVE"))
                                    {
                                        PresentationMgr.MainView.SaveLog("MAC: " + GetSystemMacStr() + " | IP: " + GetDeviceIpStr() + " | PING TO " + ipToPing + ": " + GetPingTimeAverageStr(ipToPing, 4));
                                    }
                                    PresentationMgr.MainView.SaveLog("SEND: " + typeToLog + " | " + jsonObj);
                                }
                            }
                            else
                            {
                                if (typeToCheck.Equals(typeToLog))
                                {
                                    receiveTimeMsStr = Convert.ToString((DateTime.Now - timeToCheck).TotalMilliseconds);
                                }
                                if (TEST_MODE)
                                {
                                    LogWin.WriteLog("RECEIVE: " + typeToLog + (!String.IsNullOrEmpty(receiveTimeMsStr) ? " | " + receiveTimeMsStr + " ms" : "") + " | " + jsonObj);
                                }
                                if (TEST_WRITE_MODE)
                                {
                                    PresentationMgr.MainView.SaveLog("RECEIVE: " + typeToLog + (!String.IsNullOrEmpty(receiveTimeMsStr) ? " | " + receiveTimeMsStr + " ms" : "") + " | " + jsonObj);
                                }
                            }
                        }));
        }
        public string GetSystemMacStr()
        {
            String returnStr = String.Empty;
            try
            {
                String firstMacAddress = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                returnStr = firstMacAddress;
                if (String.IsNullOrEmpty(returnStr))
                    returnStr = "EMPTY";
            }
            catch (Exception e)
            {
                returnStr = e.Message;
            }
            return returnStr;
        }
        public String GetDeviceIpStr()
        {
            String returnStr = String.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        returnStr += Convert.ToString(ip) + " | ";
                    }
                }
                if (String.IsNullOrEmpty(returnStr))
                    returnStr = "EMPTY";
            }
            catch (Exception e)
            {
                returnStr = e.Message;
            }
            return returnStr;
        }
        public String GetPingTimeAverageStr(string host, int echoNum)
        {
            String returnStr = String.Empty;
            try
            {
                long totalTime = 0;
                int timeout = 120;
                Ping pingSender = new Ping();

                for (int i = 0; i < echoNum; i++)
                {
                    PingReply reply = pingSender.Send(host, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        totalTime += reply.RoundtripTime;
                    }
                }
                double pingTimeAverage = totalTime / echoNum;
                if (pingTimeAverage <= 0)
                    returnStr = "FAILED";
                else
                    returnStr = Convert.ToString(pingTimeAverage);
            }
            catch (Exception e)
            {
                returnStr = e.Message;
            }
            return returnStr;
        }
        #endregion [NotifyHandleLogApi]

        #region [NotifyGPSStatus]
        public void NotifyGPSStatus(int value)
        {
            // Debug.WriteLine("VMT Debug ==> " + "Received NotifyGPSStatus :" + value);

            // MainWindow mw = PresentationMgr.AppWin;
            // this.gMchnID = value.MchnID;

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.IndicatorView.setGPSData(value);

                        }));
        }
        #endregion [NotifyGPSStatus]

        #region [NotifyGPSStatus]
        public void NotifyGPS(ref VMT_Data_JAT2.Objects.Common.VD_Common_GPS value)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_GPS clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_GPS>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.IndicatorView.SetGeoFence(ref clone);
                        }));
        }
        #endregion [NotifyGPSStatus]

        #region [NotifyWIFIStatus]
        public void NotifyWIFIStatus(int value)
        {
            // Debug.WriteLine("VMT Debug ==> " + "Received NotifyWIFIStatus :" + value);
            // MainWindow mw = PresentationMgr.AppWin;
            // this.gMchnID = value.MchnID;

            //LogWin.WriteLog("[FNC]NotifyWIFIStatus => value :" + value.ToString());

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (value == 0)
                            {
                                this.IndicatorView.setWifiData(0);
                                PresentationMgr.AppWin.gIsServerConnected = false;
                            }
                            else
                            {
                                this.IndicatorView.setWifiData(1);
                                PresentationMgr.AppWin.gIsServerConnected = true;
                            }
                        }));
        }
        #endregion [NotifyWIFIStatus]

        #region [NotifyKeepAlive]
        public void NotifyKeepAlive(String value)
        {
            //LogWin.WriteLog("[FNC]NotifyKeepAlive => value :" + value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (String.IsNullOrEmpty(value))
                            {
                                countKeepAliveFail++;
                                isLoggedWhenDiscon = false;
                                if (NetworkInterface.GetIsNetworkAvailable()) //Dec 19 2019 Check Network Service
                                {
                                    this.WifiPopup.TextBlock_popup_message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0023", LanguageService.LABEL_POPUP);
                                    LogWin.WriteLog("Program Service Stopped");
                                    this.WifiPopup.TextBlock_popup_message.FontSize = 40;
                                }
                                else
                                {
                                    this.WifiPopup.TextBlock_popup_message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0025", LanguageService.LABEL_POPUP);
                                    LogWin.WriteLog("Network disconnected");
                                    this.WifiPopup.TextBlock_popup_message.FontSize = 48;
                                }
                                this.IndicatorView.setWifiData(0);
                                PresentationMgr.AppWin.gIsServerConnected = false;
                                Timer.Stop(); // Dec 18 2019 Stop Timer KeepAlive failed
                            }
                            else
                            {
                                countKeepAliveFail = 0;
                                this.WifiPopup.Visibility = System.Windows.Visibility.Hidden;
                                this.IndicatorView.setWifiData(1);
                                PresentationMgr.AppWin.gIsServerConnected = true;

                                timer = value.Split(' ')[1];
                                if (!"".Equals(timer))
                                    date = DateTime.Parse(timer, System.Globalization.CultureInfo.CurrentCulture);
                                if (!Timer.IsEnabled)
                                    Timer.Start();
                                if(isLoggedWhenDiscon == false)
                                {
                                    LogWin.WriteLog("Program Service Re-Started");
                                    isLoggedWhenDiscon = true;
                                }
                            }

                            ITV.ITV_User.gServerTime = value;
                        }));
        }
        private void Timer_Click(object sender, EventArgs e)
        {
            date = date + new TimeSpan(0, 0, 1);
            //update label
            IndicatorView.Lbl_time.Content = date.ToString("HH:mm:ss");

            if (MainView.WaitingBreakRequest == MainView.BreakWaitingType.None || MainView.WaitingBreakRequest == MainView.BreakWaitingType.Unset)
            {
                if (MainView.BreakPopupView.TextBlock_Break_Start_Time.Visibility != Visibility.Visible)
                {
                    MainView.BreakPopupView.TextBlock_Break_Start_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");
                    MainView.BreakPopupView.TextBlock_Break_Start_Time.Text = date.ToString("HH:mm:ss");
                }               

                MainView.BreakPopupView.TextBlock_Break_End_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");
                MainView.BreakPopupView.TextBlock_Break_End_Time.Text = date.ToString("HH:mm:ss");

                if (MainView.ChangeDriverPopupView.Visibility != Visibility.Visible)
                    MainView.ChangeDriverPopupView.TextBlock_Break_Start_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");

                MainView.ChangeDriverPopupView.TextBlock_Break_End_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");
                //MainView.BreakPopupView.btn_Cancel.IsEnabled = true;
                //MainView.BreakPopupView.btn_Cancel.Foreground = Brushes.White;
            }
            else
            { 
                MainView.BreakPopupView.TextBlock_Break_End_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");
                MainView.BreakPopupView.TextBlock_Break_End_Time.Text = date.ToString("HH:mm:ss");

                MainView.ChangeDriverPopupView.TextBlock_Break_End_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");

                //MainView.BreakPopupView.btn_Cancel.IsEnabled = false;
                // MainView.BreakPopupView.btn_Cancel.Foreground = Brushes.Gray;
            }
        }
        #endregion [NotifyKeepAlive]


        #region [NotifyEngineTemp]
        // Cone Checker
        public void NotifyEngineTemp(int value)
        {
            // Debug.WriteLine("VMT Debug ==> " + "Received NotifyEngineTemp :" + value);

            //LogWin.WriteLog("[FNC]NotifyEngineTemp => value :" + value.ToString());

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessByEngineTempCallback(value);
                        }));
        }
        #endregion [NotifyEngineTemp]

        #region [NotifyFuelGage]
        public void NotifyFuelGage(int value)
        {
            // Debug.WriteLine("VMT Debug ==> " + "Received NotifyFuelGage :" + value);

            //LogWin.WriteLog("[FNC]NotifyFuelGage => value :" + value.ToString());

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessByFuelGageCallback(value);
                        }));
        }
        #endregion [NotifyFuelGage]

        #region [NotifySpeedKm]
        public void NotifySpeedKm(float value)
        {
            // Debug.WriteLine("VMT Debug ==> " + "Received NotifySpeedKm :" + value);

            //LogWin.WriteLog("[FNC]NotifySpeedKm => value :" + value.ToString());

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessBySpeedKmCallback((int)value);
                        }));
        }
        #endregion [NotifySpeedKm]

        #region [NotifyAccessRole]
        public void NotifyAccessRole(ref VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyDriverInfo Sussess !!! GroupListSeperator:" + value.GroupListSeperator + " Notice:" + value.Notice);

            VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //if (!String.IsNullOrEmpty(clone.shift))
                            //{
                                this.LoginView.userAccessRole = true;
                                clone.Notice = PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0007", LanguageService.MESSAGE_GROUP);
                                this.LoginView.ProcessByGetAccessRoleCallback(clone);
                            //}
                            //else
                            //{
                            //    String mess = PresentationMgr.Singleton.LanguageSer.GetResourceITV("F31", LanguageService.MESSAGE_LOGIN_GROUP);
                            //    PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0121", LanguageService.LABEL_CUSTOMIZE), mess, ""
                            //          , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            //    this.LoginView.ReFlash();
                            //}
                        }));
        }
        #endregion [NotifyAccessRole]

        #region [NotifyLogin4Machine]
        public void NotifyLogin4Machine(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyLogin4Machine Sussess !!! UserName:" + value.UserName + " TeamName:" + value.Notice);

            VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (this.MainView.ChangeDriverPopupView.SetLoginFromChangeDriver)
                            {
                                ITV.ITV_User.gUserID = this.MainView.ChangeDriverPopupView.Tb_Id.Text;
                                ITV.ITV_User.gUserNm = clone.UserName;
                                this.IndicatorView.Lbl_User_Val.Content = clone.UserName;
                                this.IndicatorView.Image_Setting.Visibility = Visibility.Hidden;
                                this.MainView.ChangeDriverPopupView.ProcessByLogin4MachineCallback();
                            }
                            else
                            {
                                this.HideProgressBar(0);
                                this.LoginView.ProcessByLogin4MachineCallback(clone);
                                IndicatorView.Lbl_YtNoChssNo.Content = (String.IsNullOrEmpty(clone.chssNo)) ? UserInfo.gMchnID : UserInfo.gMchnID + "-" + clone.chssNo;
                            }
                            // Log folder Dir
                            string sRootPath = AppCfgMgr.GetAppDirectory();
                            string sDirPath = sRootPath + @"{0}\Log\";

                            //CLEAR OLD FILES
                            if (Directory.Exists(sDirPath) == true)
                            {
                                if (Directory.GetFiles(sDirPath).Count() > 0)
                                    Array.ForEach(Directory.GetFiles(sDirPath), File.Delete);

                                DirectoryInfo dir = new DirectoryInfo(sDirPath);
                                var folders = dir.GetDirectories();
                                var sorted = folders.OrderBy(f => f.CreationTime).ToList();

                                if (sorted.Count > 5)
                                {
                                    for (int i = 0; i < sorted.Count - 5; i++)
                                    {
                                        sorted[i].Delete(true);
                                    }
                                }
                            }                               
                            VMT_DataMgr_Common.GetMachineStopCodeList_Ask();

                            if (!"CLT".Equals(RMG.RMG_User.gUserID))
                            {
                                VMT_Data_JAT2.VMT_DataMgr_Common.GetConfigValue_Ask("VMT_DONE_FLAG");

                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.SetVMTMachineStatus);
                            }
                        }));
        }
        #endregion [NotifyLogin4Machine]

        #region [NotifyLogin4Machine]
        public void NotifyConfig(ref VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyConfig Sussess !!! Arrival:" + value.Arrival + " Ready:" + value.Ready + " Done:" + value.Done);

            VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            this.MainView.ProcessByConfigCallback(clone);
                            IndicatorView.Lbl_YtNoChssNo.Content = (String.IsNullOrEmpty(clone.chssNo)) ?  UserInfo.gMchnID : UserInfo.gMchnID + "-" + clone.chssNo;

                        }));
        }
        #endregion [NotifyLogin4Machine]

        #region [NotifyChangeDriverCheck]
        public void NotifyChangeDriverCheck(String rsnCd) //rsnCd == "S" in callback
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ChangeDriverPopupView.ProcessChangeDriverCheck();
                        }));
        }
        #endregion [NotifyChangeDriverCheck]

        #region [NotifyGetVmtAutoStartConfig]
        public void NotifyGetVmtAutoStartConfig(String retStr)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            String RegAddr = @"Software\Microsoft\Windows\CurrentVersion\Run";
                            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
                            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, RegAddr);
                            if (uIntPtr.ToUInt32() > 0)
                            {
                                String regKey = KeyCLTVMT_ITV.Substring(KeyCLTVMT_ITV.LastIndexOf('\\') + 1);
                                String regValue = GetRegValue(uIntPtr, regKey);

                                if (!retStr.Equals("Y"))
                                {
                                    if (!String.IsNullOrEmpty(regValue))
                                    {
                                        TryDeleteRegValue(uIntPtr, regKey);
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(regValue))
                                    {
                                        TrySetRegValue(uIntPtr, regKey, "\"" + AppCfgMgr.GetApplicationPath(true) + "\"");
                                    }
                                }                              
                            }
                            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegAddr, true);
                            //if (keyDir != null)
                            //{
                            //    String regKey = KeyCLTVMT_ITV.Substring(KeyCLTVMT_ITV.LastIndexOf('\\') + 1); // productName
                            //    String regValue = (String)keyDir.GetValue(regKey, ""); // ProductPath

                            //    if (!retStr.Equals("Y"))
                            //    {
                            //        if (!String.IsNullOrEmpty(regValue))
                            //        {
                            //            keyDir.DeleteValue(regKey, false);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (String.IsNullOrEmpty(regValue))
                            //        {
                            //            keyDir.SetValue(regKey, "\"" + AppCfgMgr.GetApplicationPath(true) + "\"", Microsoft.Win32.RegistryValueKind.String);
                            //        }
                            //    }
                            //}
                        }));
        }
        #endregion [NotifyGetVmtAutoStartConfig]

        //#region [NotifyLogIn]
        //public void NotifyLogIn(ref VMT_Data_JAT2.Common.SVMT_LogInRp value)
        //{
        //    Debug.WriteLine("VMT Debug ==> " + "NotifyLogIn Sussess !!! bLogin:" + value.bLogin + " MchnID:" + value.MchnID);

        //    // MainWindow mw = PresentationMgr.AppWin;
        //    // this.gMchnID = value.MchnID;

        //    VMT_Data_JAT2.Common.SVMT_LogInRp clone = Util.DeepCopy<VMT_Data_JAT2.Common.SVMT_LogInRp>(value);

        //    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    this.HideProgressBar(0);
        //                    this.LoginView.ProcessByLogInCallback(clone);
        //                }));
        //}
        //#endregion [NotifyLogIn]

        #region [NotifyMachineStatusChanged]
        public void NotifyMachineStatusChanged(ref VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyMachineStatusChanged");

            VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            
                        }));
        }
        #endregion [NotifyMachineStatusChanged]

        #region [NotifyJobOrderITV]
        public void NotifyJobOrderITV(ref ITV.VD_ITV_JobOrderList value)
        {
            Debug.WriteLine("VMT Debug ==> " + "EEv2JobOrderForITV");
            if (LoginView.mIsLogin == false) // DLL 수정 해야 되지만 일단 UI에서 수정
                return;

            //int i = Marshal.SizeOf(value);

            ITV.VD_ITV_JobOrderList clone = Util.DeepCopy<ITV.VD_ITV_JobOrderList>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          this.HideProgressBar(0);

                          this.IndicatorView.StartJobTimer(0, 300);

                          this.MainView.ProcessByJobOrderITVCallback(clone);
                      }));
        }
        #endregion [NotifyJobOrderITV]

        #region [NotifyGetMachineStatusChanged]
        public void NotifyGetMachineStatusChanged(VMT_Data_JAT2.Objects.Common.VmtMachine machine)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate
                            {
                                this.MainView.getMachineStatusChangedHatchJobCallback(machine);
                            }));

            if (machine.loginUsrLst != null &&
                !machine.loginUsrLst.Contains(ITV.ITV_User.gUserID))
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate
                    {
                        if ("CLT".Equals(UserInfo.gUserID) && "ACCESS".Equals(UserInfo.gUserPW))
                            return;
                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Logout,
                            PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0036", LanguageService.MESSAGE_GROUP), //Log Off
                            PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0053", LanguageService.MESSAGE_GROUP), //Force Logoff been processed.
                            PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), null, 3); //OK
                        this.MainView.CallBack_Popup_LogOff(2);
                    }));
            }
            if (!String.IsNullOrEmpty(machine.noticeMsg))
            {
                Logger.Log("Received Notice Message : " + machine.noticeMsg);
                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, machine.noticeMsg);

                VMT_DataMgr_Common.SetMachineNotice();

                TimeSpan timeout = new TimeSpan(0, 0, 3); // 3 sec
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, timeout,
                            new Action(delegate
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Message", machine.noticeMsg, "OK", null, 0);
                            }));
            }
        }
        #endregion [NotifyGetMachineStatusChanged]

        #region [NotifyJobDeleteAll]
        public void NotifyJobDeleteAll(int value)
        {
            Debug.WriteLine("VMT Debug ==> " + "received NotifyJobDeleteAll");

            // MainWindow mw = PresentationMgr.AppWin;
            // this.gMchnID = value.MchnID;
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //       this.HideProgressBar(0);
                            //    this.IndicatorView.StopAnimationTruck();
                            this.MainView.ProcessByJobDeleteAllCallback(value);
                            this.IndicatorView.StopJobTimer();
                        }));
        }
        #endregion [NotifyJobDeleteAll]

        #region [NotifyJobChange]
        public void NotifyJobChange(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder value)
        {
            Debug.WriteLine("VMT Debug ==> " + "received NotifyJobChange");

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            // this.HideProgressBar(0);
                            // this.IndicatorView.StopAnimationTruck();
                            this.IndicatorView.StopJobTimer();
                            this.MainView.ProcessByJobChangeCallback(clone);
                            this.IndicatorView.StartJobTimer(0, 300);
                        }));
        }
        #endregion [NotifyJobChange]

        #region [Test]
        //public void NotifyJobOrder(VMT_DataMgr.EEv2JobOrder value)
        //{
        //    Debug.WriteLine("VMT Debug ==> " + "NotifyJobOrder");
        //    int i = Marshal.SizeOf(value);

        //    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //              new Action(delegate
        //              {
        //                  this.HideProgressBar(0);
        //                  this.MainView.ProcessByJobOrderCallback(value);
        //                  this.IndicatorView.StartJobTimer(0);
        //              }));

        //}
        //public void NotifyJobChange(VMT_DataMgr.EEv2JobOrder value)
        //{
        //    Debug.WriteLine("VMT Debug ==> " + "received NotifyJobChange");

        //    //MainWindow mw = PresentationMgr.AppWin;
        //    //       this.gMchnID = value.MchnID;



        //    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //             //       this.HideProgressBar(0);
        //                    this.IndicatorView.StopJobTimer();
        //                    this.MainView.ProcessByJobChangeCallback(value);
        //                    this.IndicatorView.StartJobTimer(0); 
        //                }));


        //}
        #endregion [Test]

        #region [NotifyJobDelete]
        public void NotifyJobDelete(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobKey value)
        {
            Debug.WriteLine("VMT Debug ==> " + "received NotifyJobDelete");

            // MainWindow mw = PresentationMgr.AppWin;
            // this.gMchnID = value.MchnID;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobKey clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobKey>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            // this.HideProgressBar(0);
                            // this.IndicatorView.StopAnimationTruck();
                            this.MainView.ProcessByJobDeleteCallback(clone);
                            this.IndicatorView.StopJobTimer();
                        }));
        }
        #endregion [NotifyJobDelete]

        #region [NotifyJobDone]
        public void NotifyJobDone(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "received NotifyJobDone");

            // MainWindow mw = PresentationMgr.AppWin;
            // this.gMchnID = value.MchnID;

            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                if (clone.resultObj.Equals("F16"))
                                {
                                    String message = String.Empty;
                                    if (UserInfo.gMchnTp == "TC")
                                        message = PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP);
                                    else
                                        message = PresentationMgr.Singleton.LanguageSer.GetResourceITV("F16a", LanguageService.MESSAGE_JOBSETERROR_GROUP);

                                    PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0152", LanguageService.LABEL_CUSTOMIZE), message
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                                }
                                else
                                {
                                    PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                                }                                
                            }
                            else
                            { // Oct 4 2019 Don't need Popup when SetJobDoneComplete
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            clone = null;
                        }));
        }
        #endregion [NotifyJobDone]

        #region [NotifyLogout]
        public void NotifyLogout(Boolean value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            UserInfo.gUserID = String.Empty;                       
                            this.IndicatorView.Lbl_User_Val.Content = String.Empty;
                            this.IndicatorView.Lbl_YtNoChssNo.Content = UserInfo.gMchnID;
                            this.IndicatorView.Image_Setting.Visibility = Visibility.Visible;

                            if (this.MainView.ChangeDriverPopupView.SetLogoutFromChangeDriver)
                            {                      
                                if (value == true)
                                {
                                    this.MainView.ChangeDriverPopupView.ProcessSetLogout4MachineCallBack();
                                    this.MainView.ChangeDriverPopupView.SetLogoutFromChangeDriver = false;
                                }                   
                                else
                                {
                                   String mess = PresentationMgr.Singleton.LanguageSer.GetResourceITV("FX", LanguageService.MESSAGE_LOGOUT_GROUP);
                                   PresentationMgr.AppWin.PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), mess
                                   , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                                }
                            }
                            else
                            {
                                login = false;
                                if (restart)
                                {
                                    PresentationMgr.App_RestartApp();
                                    PresentationMgr.APP_CloseApp();
                                }
                                //if (logOff)
                                //    PresentationMgr.APP_CloseApp();
                            }
                        }));
        }
        #endregion [NotifyLogout]

        #region [ChassisOrderCompletes]
        public void ChassisOrderCompletes(VMT_Data_JAT2.Objects.ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceITV("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            clone = null;
                        }));
        }
        #endregion [ChassisOrderCompletes]

        #region [ReleaseYtFromJob]
        public void ReleaseYtFromJob(VMT_Data_JAT2.Objects.ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), (!string.IsNullOrEmpty(clone.resultObj) ? clone.resultObj.ToString() : "")
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else //Oct 3 2019 Don't need Popup when Complete
                            {
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            clone = null;
                        }));
        }
        #endregion [ReleaseYtFromJob]

        #region [GetConfigValue]
        public void GetConfigValue(String value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            MainView.ProcessConfigValue(value);
                        }));
        }
        #endregion [GetConfigValue]

        #region [NotifyArrival]
        public void NotifySetMachineArrival(ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else
                            {
                                
                                if (MainView.arrivalCancel.Equals("N"))
                                {
                                    //MainView.arrivalCancel = "Y";
                                }
                                else
                                {
                                    MainView.arrivalCancel = "N";
                                }
                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeysITV_Ask();
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            Thread.Sleep(1000);
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                            clone = null;
                        }));
        }
        #endregion [NotifyArrival]

        #region [NotifyReady]
        public void NotifySetMachineReady(ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else
                            {
                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeysITV_Ask();
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            Thread.Sleep(1000);
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                            clone = null;
                        }));
        }
        #endregion [NotifyReady]

        #region [NotifyDone]
        public void NotifySetItvDone(ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else
                            {
                                // Oct 4 2019 Don't need Popup when SetJobDoneComplete
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            Thread.Sleep(1000);
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                            clone = null;
                        }));
        }
        #endregion [NotifyDone]

        #region [Notify Job Done For QC]
        public void NotifySetQCJobDoneByYt(ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else
                            { // Oct 4 2019 Don't need Popup when SetJobDoneComplete
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);

                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeysITV_Ask();
                            }
                            Thread.Sleep(1000);
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                            clone = null;
                        }));
        }
        #endregion [Notify Job Done For QC]

        #region [NotifyChangeChassisNo]
        public void NotifyChangeChassisNo(ITV.VD_ITV_Result value)
        {
            VMT_Data_JAT2.Objects.ITV.VD_ITV_Result clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_ITV_Result>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            //this.MainView.ProcessByJobDoneCallback(clone);
                            //this.IndicatorView.StopJobTimer();
                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0146", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            else
                            {
                                IndicatorView.Lbl_YtNoChssNo.Content = (String.IsNullOrEmpty(clone.resultTP)) ? UserInfo.gMchnID : UserInfo.gMchnID + "-" + MainView.ChassisChangeView.Tb_After.Text;
                                this.preChassisCd = MainView.ChassisChangeView.Tb_After.Text;
                                //PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                //    PresentationMgr.Singleton.LanguageSer.GetResourceITV("S", LanguageService.MESSAGE_SERVER_GROUP), PresentationMgr.Singleton.LanguageSer.GetResourceSC("S1", LanguageService.MESSAGE_JOBDONE_GROUP)
                                //    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            clone = null;
                        }));
        }
        #endregion [NotifyChangeChassisNo]

        #region [NotifyChangeDriver]
        public void NotifyChangeDriver(Boolean value)
        {
            Boolean clone = Util.DeepCopy<Boolean>(value);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (clone) //SUCCESS
                            {
                                PresentationMgr.MainView.ChangeDriverPopupView.ChangeDriverClicked = true;
                                VMT_DataMgr_Common.GetMachineStop_Ask();
                            }                         
                            clone = false;
                        }));
        }
        #endregion [NotifyChangeDriver]

        #region [NotifyGetChssUsingData]
        public void NotifyGetChssUsingData(String value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            if (!changeChssNo)
                                LoginView.ProcessGetChssUsingDataCallback(value);
                            else
                                MainView.ChassisChangeView.ProcessChangeChassisNoCallback(value);
                        }));
        }
        #endregion [NotifyChangeChassisNo]

        #region [NotifyException]
        public void NotifyException(String value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            if(value.ToUpper().Contains("GETUSERACCESSROLE"))
                            {
                                this.LoginView.userAccessRole = false;
                                this.LoginView.ReFlash();
                                //String mess = PresentationMgr.Singleton.LanguageSer.GetResourceITV("F9", LanguageService.MESSAGE_LOGIN_GROUP);
                                //PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("F8", LanguageService.MESSAGE_SERVER_GROUP), mess, ""
                                //    , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                            }
                            //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Hessian Exception", value, "OK", null, 0);
                        }));
        }
        #endregion [NotifyException]

        #region [NotifyBlockEnterance]
        public void NotifyBlockEnterance(ref ITV.VD_ITV_NotifyBlockEnter_Receive value)
        {
            //LogMessage("NotifyBlockEnterance => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifyBlockEnterance Sussess");

            ITV.VD_ITV_NotifyBlockEnter_Receive clone = Util.DeepCopy<ITV.VD_ITV_NotifyBlockEnter_Receive>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<ITV.VD_ITV_NotifyBlockEnter_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            //this.MainView.ProcessByBlockEnteranceCallback(clone);
                        }));
        }
        #endregion [NotifyBlockEnterance]

        #region [NotifyCPSAlign]
        public void NotifyCPSAlign(ref ITV.VD_ITV_NotifyCPSAlign_Receive value)
        {
            //LogMessage("NotifyCPSAlign => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifyCPSAlign Sussess");

            ITV.VD_ITV_NotifyCPSAlign_Receive clone = Util.DeepCopy<ITV.VD_ITV_NotifyCPSAlign_Receive>(value);
            // TimeSpan timeout = new TimeSpan(0, 0, 5);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<ITV.VD_ITV_NotifyCPSAlign_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            //this.MainView.ProcessByCPSAlignCallback(clone);
                        }));
        }
        #endregion [NotifyCPSAlign]

        #region [NotifyMessage]
        public void NotifyMessage(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyMessage Sussess ");

            VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive>(value);
            TimeSpan timeout = new TimeSpan(0, 0, MainWindow.MessagePopupDelayTime);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, timeout,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            if (!String.IsNullOrEmpty(clone.m_strMessage))
                            {
                                PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0121", LanguageService.LABEL_CUSTOMIZE), clone.m_strMessage, ""
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0); //20190806
                            }
                            //SET NOTICE MESSAGE
                            if (String.IsNullOrEmpty(clone.m_strMessage2))
                            {
                                MainView.TextBlock_Message.Text = String.Empty;
                                MainView.Tbl_MessPage.Text = "0/0";
                            }
                            else if (clone.m_strMessage2.Replace("\r\n", "@") == MainView.currentMessage)
                            {
                                return;
                            }
                            else
                            {
                                MainView.getMessageList(clone.m_strMessage2);
                                MainView.SetMessageGrid();
                            }
                            //BeefPlay();
                        }));
        }
        #endregion [NotifyMessage]

        #region [NotifyMachineStopCodeList]
        public void NotifyMachineStopCodeList(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyMachineStopCodeList Sussess ");

            VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.MainView.ProcessByMachineStopCodeList(clone);
                        }));
        }
        #endregion [NotifyMachineStopCodeList]

        #region [NotifyGetMachineStop]
        public void NotifyGetMachineStop(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value)
        {
            //LogMessage("NotifyGetMachineStop => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifyGetMachineStop Sussess ");

            VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (this.MainView.ChangeDriverPopupView.ChangeDriverClicked)
                            {
                                if (clone != null)
                                {
                                    this.MainView.ChangeDriverPopupView.ProcessByGetMachineStopCallback(clone);
                                    this.MainView.ChangeDriverPopupView.ShowPopup();
                                }
                                this.MainView.ChangeDriverPopupView.ChangeDriverClicked = false;
                            }
                            else
                            {
                                if (this.MainView.WaitingBreakRequest != MainView.BreakWaitingType.None ||
                                     (MainView.AvailablePopupView.Visibility != Visibility.Visible &&
                                     MainView.BreakPopupView.Visibility != Visibility.Visible) || 
                                     (MainView.BreakPopupView.Visibility == Visibility.Visible && clone == null) ||
                                     (MainView.ChangeDriverPopupView.Visibility == Visibility.Visible && clone == null)
                                 )
                                {
                                    if (clone != null)
                                    {
                                        if (this.MainView.ChangeDriverPopupView.Visibility != Visibility.Visible)
                                            this.MainView.ProcessByGetMachineStopCallback(clone);
                                    }
                                    else
                                    {
                                        this.MainView.MainView_ResetAvaliable();
                                    }
                                }

                                // Oct 17 2019 Add Remark Field
                                if (clone != null && clone.remark == "PNO")
                                {
                                    PresentationMgr.MainView.BreakPopupView.Lbl_ByControl.Visibility = Visibility.Visible;
                                    if (MainView.isFinishAvailable == false)
                                        this.MainView.WaitingBreakRequest = MainView.BreakWaitingType.None;
                                }
                                else
                                    PresentationMgr.MainView.BreakPopupView.Lbl_ByControl.Visibility = Visibility.Hidden;

                                if (this.MainView.WaitingBreakRequest != MainView.BreakWaitingType.None)
                                {
                                    if (clone != null &&
                                        this.MainView.WaitingBreakRequest == MainView.BreakWaitingType.Set)
                                    {

                                        if (this.MainView.BreakPopupView.Visibility == System.Windows.Visibility.Hidden)
                                        {
                                            PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0121", LanguageService.LABEL_POPUP)
                                            , String.Format("{0} " + PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0145", LanguageService.LABEL_CUSTOMIZE), clone.Data.ReasonNm), ""
                                            , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                                            this.MainView.ShowBreak(clone.Data);
                                        }
                                    }
                                    else if (clone == null &&
                                        this.MainView.WaitingBreakRequest == MainView.BreakWaitingType.Unset)
                                    {
                                        this.MainView.MainView_SetMachineStop(true);
                                    }
                                    else
                                    {
                                        if (clone != null && clone.remark == "PNO" && MainView.isFinishAvailable == false)
                                        {
                                            this.MainView.MainView_SetMachineStop(false, true);
                                            this.MainView.ProcessBySetMachineStopCallback();
                                            if (this.MainView.mCurrentBreak.ToString().ToLower() != "none") this.MainView.ShowBreak(clone.Data);
                                        }
                                        else
                                        {
                                            if (MainView.isFinishAvailable == true)
                                                this.MainView.MainView_SetMachineStop();
                                            MainView.isFinishAvailable = false;

                                        }
                                    }
                                }
                            }                          
                        }));
        }
        #endregion [NotifyGetMachineStop]

        #region [NotifySetMachineStop]
        public void NotifySetMachineStop(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive value)
        {
            //LogMessage("NotifySetMachineStop => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifySetMachineStop Sussess ");

            VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (this.MainView.ChangeDriverPopupView.SetLoginFromChangeDriver)
                            {
                                this.MainView.ChangeDriverPopupView.HidePopup();
                                this.MainView.ChangeDriverPopupView.SetLoginFromChangeDriver = false;
                            }
                            else
                            {
                                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                this.MainView.ProcessBySetMachineStopCallback(clone);
                                if (this.MainView.mCurrentBreak.ToString().ToLower() != "none") this.MainView.ShowBreak(clone.Data); // 20190703 show Break UI after Start
                                VMT_DataMgr_Common.GetMachineStop_Ask();
                            }                            
                        }));
        }
        #endregion [NotifySetMachineStop]

        #region [NotifyMachineStopConfirm]
        public void NotifyMachineStopConfirm(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopConfirm_Receive value)
        {
            //LogMessage("NotifyMachineStopConfirm => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifyMachineStopConfirm Sussess ");

            VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopConfirm_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopConfirm_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //this.MainView.ProcessByMachineStopConfirm(clone);

                        }));
        }
        #endregion [NotifyMachineStopConfirm]

        #region [NotifyErrorCode]
        public void NotifyErrorCode(String value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyErrorCode Sussess ");

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (value.Length > 0)
                            {
                                String[] errString = value.Split('@');
                                if (!String.IsNullOrEmpty(errString[0]))
                                {
                                    this.HideProgressBar(0);
                                    String mess = PresentationMgr.Singleton.LanguageSer.GetResourceITV(errString[0], LanguageService.MESSAGE_LOGIN_GROUP);
                                    if (errString.Count() > 1 && errString[1].Length > 0)
                                        mess = mess + " (" + errString[1] + ")";
                                    PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0121", LanguageService.LABEL_CUSTOMIZE), mess, ""
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                                    this.LoginView.ReFlash();
                                }                             
                            }
                        }));
        }
        #endregion [NotifyErrorCode]

        #region [NotifyCalibration]
        public void NotifyCalibration(ref VMT_Data_JAT2.Objects.Common.VD_Common_Calibration value)
        {
            //LogMessage("NotifyCalibration => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifyAvailable NotifyErrorCode ");

            VMT_Data_JAT2.Objects.Common.VD_Common_Calibration clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_Calibration>(value);

            // Print Log Window
            //string strLog = "";
            //strLog += "---------------------------------------------\r\n";
            //strLog += "Message Name : NotifyCalibration\r\n"; 
            //strLog += string.Format("Speed Pulse : {0}, GyroScaleFator : {1}, GyroBias : {2}\r\n",
            //    new String(clone.SpeedPulse, 1),
            //    new String(clone.GyroSF, 1),
            //    new String(clone.GyroBias, 1));
            //strLog += string.Format("Value Pulse : {0}, Value Gyro : {1}, Value Temp : {2}\r\n",
            //    clone.Pulse, clone.Gyro, clone.Temp.ToString());
            //strLog += "---------------------------------------------\r\n";
            //MainWindow.LogWin.WriteLog(strLog);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            string strSpeedPulse = new String(clone.SpeedPulse, 1);
                            string strGyroSF = new String(clone.GyroSF, 1);
                            string strGyroBias = new String(clone.GyroBias, 1);

                            int nSpeedPulse; Int32.TryParse(strSpeedPulse, out nSpeedPulse);
                            int nGyroSF; Int32.TryParse(strGyroSF, out nGyroSF);
                            int nGyroBias; Int32.TryParse(strGyroBias, out nGyroBias);

                            this.CalibrationInfoView.TextBox_SpeedPulse.Text = strSpeedPulse;
                            this.CalibrationInfoView.TextBox_GyroScaleFactor.Text = strGyroSF;
                            this.CalibrationInfoView.TextBox_GyroBias.Text = strGyroBias;

                            this.CalibrationInfoView.TextBox_ValuePulse.Text = clone.Pulse;
                            this.CalibrationInfoView.TextBox_ValueGyro.Text = clone.Gyro;
                            this.CalibrationInfoView.TextBox_ValueTemp.Text = clone.Temp.ToString();

                            // TODO
                            // Calibration value 3가지 값의 합이 4 이하이면 경고창
                            //if (nSpeedPulse + nGyroSF + nGyroBias <= 4)
                            //{
                            //    if (this.PopupView.Visibility != System.Windows.Visibility.Visible)
                            //        this.PopupView.ShowPopup(0, "Warning", "Calibration Warning", "", "", "", Callback_Calibration, 0);
                            //}
                            //else
                            //{
                            //    if (this.PopupView.Visibility == System.Windows.Visibility.Visible)
                            //        this.PopupView.HidePopup();
                            //}
                        }));
        }
        #endregion [NotifyCalibration]

        #region [NotifyMachineStop]
        public void NotifyMachineStop(ref Object value)
        {
            Debug.WriteLine("VMT Debug ==> " + "NotifyMachineStop");

            Object clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessByMachineStopCallback(clone);
                        }));
        }
        #endregion [NotifyMachineStop]

        #region [NotifyITV_Periodic]
        public void NotifyITV_Periodic(ref ITV.VD_ITV_PDS_Periodic_Payload value)
        {
            //LogMessage("NotifyITV_Periodic => value : " + value.ToString());

            ITV.VD_ITV_PDS_Periodic_Payload clone = Util.DeepCopy<ITV.VD_ITV_PDS_Periodic_Payload>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_NCT.Objects.Common.VD_ITV_PDS_Periodic_Payload>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            //String speed_s = Encoding.ASCII.GetString(clone.m_cSpeedOver, 0, clone.m_cSpeedOver.Length);
                            String speed_s = clone.m_fSpeedOver.ToString();
                            String[] speed_int_s = speed_s.Split('.');
                            int speed = Convert.ToInt32(speed_int_s[0]);
                            this.MainView.ProcessBySpeedKmCallback(speed);
                        }));
        }
        #endregion [NotifyITV_Periodic]

        #region [NotifyITV_Event]
        public void NotifyITV_Event(ref ITV.VD_ITV_PDS_Event_Payload value)
        {
            //LogMessage("NotifyITV_Event => value : " + value.ToString());

            ITV.VD_ITV_PDS_Event_Payload clone = Util.DeepCopy<ITV.VD_ITV_PDS_Event_Payload>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //InterfaceMessageLoader.instance().WriteInterfaceMessage<ITV.VD_ITV_PDS_Event_Payload>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            Int32 nFuelGage = Convert.ToInt32(clone.m_ucFuelGage);
                            Int32 nConeCheck = Convert.ToInt32(clone.m_BConCheck);

                            this.MainView.ProcessByFuelGageCallback(nFuelGage);
                            this.MainView.ProcessByEngineTempCallback(nConeCheck);
                        }));
        }
        #endregion [NotifyITV_Event]

        #region [NotifyChassis_Attach]
        public void NotifyChassis_Attach(ref ITV.VD_ITV_ChassisAttachInfo_Receive value)
        {
            //LogMessage("NotifyChassis_Attach => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "NotifyChassis_Attach Sussess ");

            ITV.VD_ITV_ChassisAttachInfo_Receive clone = Util.DeepCopy<ITV.VD_ITV_ChassisAttachInfo_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<ITV.VD_ITV_ChassisAttachInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                        }));
        }
        #endregion [NotifyChassis_Attach]

        //#region [NotifyPowOut]
        //public void NotifyPowOut(int value)
        //{
        //    Debug.WriteLine("VMT Debug ==> " + "received NotifyPowOut");

        //    // MainWindow mw = PresentationMgr.AppWin;
        //    // this.gMchnID = value.MchnID;

        //    // TimeSpan timeout = new TimeSpan(0, 0, 5);
        //    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    this.MainView.ProcessByPowOutCallback(value);
        //                }));
        //}
        //#endregion [NotifyPowOut]

        #region [NotifyCFGEKF]
        public void NotifyCFGEKF(ref VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive value)
        {
            //LogMessage("NotifyCFGEKF => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "received NotifyCFGEKF");

            VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.CalibrationInfoView.ProcessByCFGEKFCallback(clone);
                        }));
        }
        #endregion [NotifyCFGEKF]

        #region [NotifyAlarm]
        public void NotifyAlarm(ref VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive value)
        {
            //LogMessage("NotifyAlarm => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "received NotifyAlarm");

            VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                        }));
        }
        #endregion [NotifyAlarm]

        #region [NotifySTSLDSeq]
        public void NotifySTSLDSeq(ref ITV.VD_ITV_STS_LDPlan value)
        {
            //LogMessage("NotifyAlarm => value : " + value.ToString());
            Debug.WriteLine("VMT Debug ==> " + "received NotifySTSLDSeq");

            ITV.VD_ITV_STS_LDPlan clone = Util.DeepCopy<ITV.VD_ITV_STS_LDPlan>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<ITV.VD_ITV_STS_LDPlan>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.MainView.ProcessBySTSLDSeqCallback(clone);
                        }));
        }
        #endregion [NotifySTSLDSeq]

        #region [NotifyPinningStation]
        public void NotifyPinningStation(ref Object value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessByPinningStationCallback();
                        }));
        }
        #endregion [NotifyPinningStation]

        #region [NotifyArrvdMchnAtPow]
        public void NotifyArrvdMchnAtPow(String value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessByArrvdMchnAtPow(value);
                        }));
        }
        #endregion [NotifyArrvdMchnAtPow]


        #region [NotifyConfirmJobByScanner]
        public void NotifyConfirmJobByScanner(Boolean value)
        {
            //LogMessage("NotifyConfirmJobByScanner => value : " + value.ToString());

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.MainView.ProcessByConfirmJobByScanner(value);
                        }));
        }
        #endregion [NotifyConfirmJobByScanner]

        #region [NotifyValidChassisInfos]
        public void NotifyValidChassisInfos(VMT_Data_JAT2.Objects.ITV.VD_Common_ChassisInventory value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.ITV.VD_Common_ChassisInventory>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, "null"));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.LinkPopupView.AddLinkInfo(clone);

                            clone = null;
                            //this.HideProgressBar(0);

                        }));
        }
        #endregion [NotifyValidChassisInfos]

        #region [NotifyBlockList]
        public void NotifyBlockList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value == null ? "null" : value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.UnLinkPopupView.AddBlockInfo(clone);

                            clone.Dispose();
                            //this.HideProgressBar(0);

                        }));
        }
        #endregion [NotifyBlockList]

        #region [NotifyBlockMapListForYt]
        public void NotifyBlockMapListForYt(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            this.UnLinkPopupView.AddUnlinkBlockInfo(clone);
                            //DataMgr.Singleton.AddBlockBayInfo(clone);

                            //if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            //{
                            //    this.HideProgressBar(0);

                            //    if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) &&
                            //        clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay != null)//.Count > 0)
                            //    {
                            //        if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual != true)
                            //        {
                            //            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            //            , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock);
                            //            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                            //        }

                            //        if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
                            //        {
                            //            if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                            //                PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, String.Empty);
                            //            else
                            //            {
                            //                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                            //                {
                            //                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                            //                    m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                            //                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                            //                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                            //                };
                            //                var bayList = clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay.Values.ToList();
                            //                pos.m_cBay = bayList.First().BayName;
                            //                PresentationMgr.Singleton.CurrentPostion = pos;
                            //            }
                            //        }
                            //        else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                            //        {
                            //            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            //            , "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock + "" + PresentationMgr.Singleton.CurrentBay);
                            //            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);

                            //            PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
                            //        }
                            //    }
                            //}

                            clone.Dispose();
                        }));
        }
        #endregion [NotifyBlockMapListForYt]
        
        #region [NotifyBlockMapList]
        public void NotifyBlockMapList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            this.UnLinkPopupView.AddUnlinkBlockInfo(clone);
                            //DataMgr.Singleton.AddBlockBayInfo(clone);

                            //if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            //{
                            //    this.HideProgressBar(0);

                            //    if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) &&
                            //        clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay != null)//.Count > 0)
                            //    {
                            //        if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual != true)
                            //        {
                            //            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            //            , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock);
                            //            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                            //        }

                            //        if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
                            //        {
                            //            if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                            //                PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, String.Empty);
                            //            else
                            //            {
                            //                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                            //                {
                            //                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                            //                    m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                            //                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                            //                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                            //                };
                            //                var bayList = clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay.Values.ToList();
                            //                pos.m_cBay = bayList.First().BayName;
                            //                PresentationMgr.Singleton.CurrentPostion = pos;
                            //            }
                            //        }
                            //        else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                            //        {
                            //            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            //            , "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock + "" + PresentationMgr.Singleton.CurrentBay);
                            //            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);

                            //            PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
                            //        }
                            //    }
                            //}

                            clone.Dispose();
                        }));
        }
        #endregion [NotifyBlockMapList]

        #endregion [Notify]

        public void HideAllChild(DependencyObject parent)
        {
            List<UIElement> uiList = new List<UIElement>();

            PresentationMgr.FindChildByType<UIElement>((DependencyObject)parent, uiList);

            foreach (UIElement ui in uiList)
                ui.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ShowMachineView()
        {
            MachineInfoView.Visibility = System.Windows.Visibility.Visible;
            KeypadView.Visibility = Visibility.Hidden;
        }

        public void HideMachineView()
        {
            MachineInfoView.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ShowCalibrationView()
        {
            CalibrationInfoView.SetDGPSDirectionPinPolarity();
            CalibrationInfoView.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideCalibrationView()
        {
            CalibrationInfoView.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ShowChassisNumberDlg()
        {
            ChassisNumberView.refreshView();
            ChassisNumberView.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideChassisNumberDlg()
        {
            ChassisNumberView.Visibility = System.Windows.Visibility.Hidden;
            KeypadView.HideKeyPad();
            this.Focus();
        }

        public void ShowCalibrationInitPopup()
        {
            CalibrationInitPopup.refreshView();
            CalibrationInitPopup.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideCalibrationInitPopup()
        {
            CalibrationInitPopup.Visibility = System.Windows.Visibility.Hidden;
            KeypadView.HideKeyPad();
            this.Focus();
        }

        public void BeefPlay()
        {
            gSoundPlayer.Stop();
            gSoundPlayer.Play();
        }
        public void BeefPlay_dingdong()
        {
            gSoundPlayer_dingdong.Stop();
            gSoundPlayer_dingdong.Play();
        }
        public void BeefPlay_Windows_Background()
        {
            gSoundPlayer_Windows_Background.Stop();
            gSoundPlayer_Windows_Foreground.Play();
        }
        public void BeefPlay_Windows_Foreground()
        {
            gSoundPlayer_Windows_Foreground.Stop();
            gSoundPlayer_Windows_Foreground.Play();
        }
        public void BeefPlay_Windows_Notify_System_Generic(int seconds=2)
        {
            //gSoundPlayer_Windows_Notify_System_Generic.Stop();
            //gSoundPlayer_Windows_Notify_System_Generic.Play();

            //var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
            //timer.Start();
            //timer.Tick += (sender, args) =>
            //{
            //    gSoundPlayer_Windows_Notify_System_Generic.Stop();
            //    gSoundPlayer_Windows_Notify_System_Generic.Play();

            //    timer.Stop();
            //};
        }
        public void BeefPlay_Windows_User_Account_Control()
        {
            gSoundPlayer_Windows_User_Account_Control.Stop();
            gSoundPlayer_Windows_User_Account_Control.Play();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BeefPlay();

            ITV.VD_ITV_SetManualArrival_Receive value = new ITV.VD_ITV_SetManualArrival_Receive();
            this.MainView.ProcessByArrivalCallback(value);
        }

        private void btn_test2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotifyFuelGage(10);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProcHook);
        }
        public const int UM_UPDATE_PRG_POS = 0x0400 + 8000;
        static IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case UM_UPDATE_PRG_POS:                 
                    PresentationMgr.AppWin.IndicatorView.SetDownloadProgress((int)wParam, (int)lParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }     
    }
}