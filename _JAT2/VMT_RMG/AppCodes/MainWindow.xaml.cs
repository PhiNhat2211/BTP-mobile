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

//using ExternalAPI;
using System.Runtime.InteropServices;
using System.Collections;
using Common.Util;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using System.Reflection;
using WPSocketComm.Socket_TCP;

//20190108
using Common.Interface;
using System.Windows.Threading;
using System.Windows.Interop;
using static VMT_Data_JAT2.Objects.Common;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO;
using Microsoft.Win32;
using static Common.Util.Registry64;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //----------------------------------------------
        //- Build Relation Configuration

        //----------------------------------------------
        //- RMG for NCT  
        static public String SERVICE_COMPANY = "BTP";
        //static public String Project = @"BTP-ALPHA";
        //static public String Project = @"BTP-BRAVO";
        static public String Project = @"BTP-PROD";
        //static public String Project = @"BTP-DEV";
        //static public String Project = @"BTP-QAS";
        static public String KeyCLTVMT_RMG = @"SOFTWARE\CyberLogitec\VMT-RTG for " + Project;
        static public String DefUpdateServerAddr = "172.19.51.17:56000";  // Default Update Server URL
        public const string VMT_EngineDLL = "EE_Sensor_KAP.dll";
        public const string VMT_DataMgrDLL = "VMT_Data_JAT2.Common.dll";

        public const String SIEMENS_PROCESSNAME = "SIEMENSInterface";

        #region [variables]
        private double _DesignWidth = 1024;
        private double _DesignHeight = 768;

        public const int MESSAGE_TO_EEVMT_LOGIN = 0x10001;
        public const int MESSAGE_TO_EEVMT_CONNECT = 0x10002;
        private Int32 PORT_SIEMENS_INTERFACE = 58001;
        private Int32 PERIOD_SIEMENS_CONNECTION_CHECK = 30;

        public Boolean chkPLC = false;
        public Boolean autoSelectP = false;
        public String wrkCd = "", wrkCdPLC = String.Empty, cntrReleasePLC = String.Empty;
        public int endPollingPLCwrkCd12 = 0; // stop polling getMachineJobByKey
        private int checkWrkCdPLCCount = 0;

        public Boolean gIsDay = true;
        public VMT_Data_JAT2.Objects.Common.VD_Common_Yard_Location loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Yard_Location();
        public VMT_Data_JAT2.Objects.Common.VD_Common_Yard_Location locChg = new VMT_Data_JAT2.Objects.Common.VD_Common_Yard_Location();

        static public System.Media.SoundPlayer gSoundPlayer_Windows_Background;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_Foreground;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_Notify_System_Generic;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_User_Account_Control;
        static public System.Media.SoundPlayer gSoundPlayer_Windows_Exclamation;

        //public String sMchnID = "";
        //public String sUserID = "";
        //public String sUserPW = "";

        public static LogWindow LogWin = null;
        static public bool ShowAppUI = true;
        public Boolean gIsServerConnected = false;

        public socketServerTCP TcpSock4Siemens = null;

        private System.Threading.Timer _siemensProcessTimer = null;

        static public Boolean IsMachineLogOn = false;

        //private bool setTimer = false;

        public static string locChgFlg = "NA";
        private string autoFlg = "";
        public static bool firstLoad = true;
        List<string> lstLDMOJobYTAssigned = new List<string>();
        List<string> lstOldJob = new List<string>();
        List<string> lstYTAssigned = new List<string>();
        public bool needToCheckYtNo = false;
        public int ytNoCheckCount = 0;

        private string timer = "";
        private Boolean needSetTimeStart = false;
        private Boolean needSetLoginLong = false;
        DateTime date = new DateTime();
        DateTime dateTimeStart = new DateTime();

        public static DispatcherTimer dtClockTime = new DispatcherTimer();
        DispatcherTimer Timer = new DispatcherTimer();

        //public DispatcherTimer PLCTimer = new DispatcherTimer();
        //private int plcCnt = 0;
        //RMG.VD_RMG_VmtDomain newPLCDomain =  new RMG.VD_RMG_VmtDomain();

        public DispatcherTimer PLCTimer = new DispatcherTimer();// timer check wrkCd
        private int plcCnt = 0; // count time check wrkCd
        private int plcPollingCntTimer = 0; // count time Polling ProcessPLC
        private int plcReceiveCnt = 0; // check Polling ProcessPLC receive
        public String getConfigValueKey = String.Empty; //Oct 29 2021 this variable is used to call getConfigValue api by sequence

        #endregion [variables]

        #region [Application Data]
        //=======================================================================
        //=
        //= Application Data 
        //=
        //=======================================================================


        #endregion [Appliation Data]


        //=======================================================================
        //=
        //= MainWindow Class
        //= Description Application Main Window
        //=
        //=======================================================================

        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 1); ;//ticks every 1 second
            Timer.Tick += new EventHandler(Timer_Click); ;
            //Timer.Start();

            PLCTimer.Interval = new TimeSpan(0, 0, 1); ;//ticks every 1 second
            PLCTimer.Tick += new EventHandler(PLCTimer_Click);

            // Initialize Logger
            //DateTime time = DateTime.Now;             // Use current time
            //string formatFile = "yyyy-MM-dd_HH";      // Use this format
            //string filetag = time.ToString(formatFile);   // Write to console
            //string logFile = string.Format(@"Log\HessianLog_{0}.txt", filetag);

            // Initialize Machine
            int size = 0;
            RMG.RMG_User.GetMachineID(ref RMG.RMG_User.gMchnID, ref size);
            RMG.RMG_User.GetMachineType(ref RMG.RMG_User.gMchnTp, ref size);
            RMG.RMG_User.GetJobTypeSortOrder(ref RMG.RMG_User.gJobTypeSortOrder);
            //PresentationMgr.Singleton.MachineID = RMG.RMG_User.gMchnID;

            // Loading Application Configuration File
            this.LoadAppCfg();

            // Loading Application Setting File
            //this.LoadAppSetting();

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

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            // Init Geometry (POW of Terminal Yard)
            // VMT_Data_JAT2.Common.InitGeometry();

            // Init Shortcut Input
            InitKeyInput();

            if (!ShowAppUI)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }

            MainWindow.LogWin = new LogWindow();
            if (App.TEST_MODE)
            {
                LogWin.Show();
            }

            gSoundPlayer_Windows_Background = new System.Media.SoundPlayer(Properties.Resources.Windows_Background_soundup);
            gSoundPlayer_Windows_Foreground = new System.Media.SoundPlayer(Properties.Resources.Windows_Foreground_soundup);
            gSoundPlayer_Windows_Notify_System_Generic = new System.Media.SoundPlayer(Properties.Resources.Windows_Notify_System_Generic_soundup);
            gSoundPlayer_Windows_User_Account_Control = new System.Media.SoundPlayer(Properties.Resources.Windows_User_Account_Control_soundup);
            gSoundPlayer_Windows_Exclamation = new System.Media.SoundPlayer(Properties.Resources.Windows_Exclamation);

            // Init Delegate Method (Callback Functions)
            InitAppCallbackFunctions();

            // Init Data Manager
            PresentationMgr.Singleton.InitDataMgr();

            //InitAvailable();

            //this.DataContext = ViewModel.Singleton;
            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~MainWindow()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {

            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {

            }
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
                strCfgPath = AppCfgMgr.GetAppDirectory() + "JAT2_VMT_RMG.cfg.xml";

            // Loading Application Config XML Document.
            AppCfgMgr.Singleton.LoadFile(strCfgPath);

            string strTestMode;
            strTestMode = AppCfgMgr.Singleton.GetValueByKey("IsTestMode");
            if (strTestMode == "1") App.TEST_MODE = true; // true - test mode, false - real mode
            else App.TEST_MODE = false; // true - test mode, false - real mode

            string strTestWriteMode;
            strTestWriteMode = AppCfgMgr.Singleton.GetValueByKey("IsTestWriteMode");
            if (strTestWriteMode == "1") App.TEST_WRITE_MODE = true;
            else App.TEST_WRITE_MODE = false;

            string strStandAlone;
            strStandAlone = AppCfgMgr.Singleton.GetValueByKey("IsStandAlone");
            if (strStandAlone == "1") App.STANDALONE_MODE = true; // true - stand alone mode, false - real mode
            else App.STANDALONE_MODE = false; // true - stand alone mode, false - real mode

            string strMessageCapture;
            strMessageCapture = AppCfgMgr.Singleton.GetValueByKey("IsMessageCapture");
            if (strMessageCapture == "1") App.MESSAGE_CAPTURE_MODE = true; // true - stand alone mode, false - real mode
            else App.MESSAGE_CAPTURE_MODE = false; // true - stand alone mode, false - real mode

            string cfgValue = "";
            cfgValue = AppCfgMgr.Singleton.GetValueByKey("ShowAppUI");
            if (cfgValue == "0")
                MainWindow.ShowAppUI = false;

            cfgValue = AppCfgMgr.Singleton.GetValueByKey("IsStartUp");
            String RegAddr = @"Software\Microsoft\Windows\CurrentVersion\Run";

            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, RegAddr);
            if (uIntPtr.ToUInt32() > 0)
            {
                String regKey = KeyCLTVMT_RMG.Substring(KeyCLTVMT_RMG.LastIndexOf('\\') + 1);
                String regValue = GetRegValue(uIntPtr, regKey);
                if (!String.IsNullOrEmpty(regValue))
                {
                    TryDeleteRegValue(uIntPtr, regKey);
                }
            }
            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegAddr, true);
            //if (keyDir != null)
            //{
            //    String regKey = KeyCLTVMT_RMG.Substring(KeyCLTVMT_RMG.LastIndexOf('\\') + 1); // productName
            //    String regValue = (String)keyDir.GetValue(regKey, ""); // ProductPath
               
            //    //if (cfgValue.Equals("0"))
            //    //{
            //    if (!String.IsNullOrEmpty(regValue))
            //    {
            //        keyDir.DeleteValue(regKey, false);
            //    }
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

            string strHessianServerIP = AppCfgMgr.Singleton.GetValueByKey("HessianServerIP");
            VMT_DataMgr.gHessianServerIP = strHessianServerIP;
            string strHessianServerPort = AppCfgMgr.Singleton.GetValueByKey("HessianServerPort");
            VMT_DataMgr.gHessianServerPort = strHessianServerPort;

            string strHessianServerMgtIP = AppCfgMgr.Singleton.GetValueByKey("HessianServerMgtIP");
            VMT_DataMgr.gHessianServerMgtIP = strHessianServerMgtIP;
            string strHessianServerMgtPort = AppCfgMgr.Singleton.GetValueByKey("HessianServerMgtPort");
            VMT_DataMgr.gHessianServerMgtPort = strHessianServerMgtPort;

            string strSiemensInterfacePort = AppCfgMgr.Singleton.GetValueByKey("SiemensInterfacePort");
            this.PORT_SIEMENS_INTERFACE = Convert.ToInt32(strSiemensInterfacePort);
            string strSiemensConnectionCheckPeriod = AppCfgMgr.Singleton.GetValueByKey("SiemensConnectionCheckPeriod");
            this.PERIOD_SIEMENS_CONNECTION_CHECK = Convert.ToInt32(strSiemensConnectionCheckPeriod);

            this.SetWindowSizeNPosition();
        }


        public void SaveAppCfg()
        {
            //AppCfgMgr.Singleton.SetValueByKey("Top", this.Top.ToString());
            //AppCfgMgr.Singleton.SetValueByKey("Left", this.Left.ToString());

            AppCfgMgr.Singleton.SaveFile();
        }

        public void SetWindowSizeNPosition()
        {
            double n_sWidth = (double)_DesignWidth;
            double n_sHeight = (double)_DesignHeight;
            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            //if (screen.Width < 1024 || screen.Height < 768)
            //{
            //    n_sWidth = 1280;
            //    n_sHeight = 800;
            //}

            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);

            string uiSize = ini.IniReadValue("MACHINE", "UISIZE");
            if (uiSize == "1024" || uiSize == "")
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
        #endregion [Application Configuration Methods]


        private bool InitRMGConnetivity()
        {
            if (VMT_DataMgr.CreateVMTClient(App.STANDALONE_MODE))
            {
                PresentationMgr.AppWin.UC_IndicatorView.TextBlock_MachineID.Text = RMG.RMG_User.gMchnID;
                PresentationMgr.AppWin.UC_IndicatorView.TextBox_JobCount.Text = "0";

                //String ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //String[] vn = ver.Split('.');
                //String strVersion = vn[0] + "." + vn[1] + "." + vn[2] + "." + vn[3];

                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "GetMachineStopCodeList_Ask"), UserInfo.gMchnTp);

                //VMT_DataMgr_Common.GetMachineStopCodeList_Ask(); move to after login
            }
            else
            {
            }

            PresentationMgr.StartNetworkCheck();
            //this.TcpSock4Siemens = new socketServerTCP();
            //this.TcpSock4Siemens.socketServerInit(PORT_SIEMENS_INTERFACE);
            //this.TcpSock4Siemens.tcpMsgEvt += new socketServerTCP.TcpSrvMsgEventHandler(SiemensInterface_TcpSrvMsgEventHandler);
            //this.TcpSock4Siemens.tcpRcvMsgEvt += new socketServerTCP.TcpSrvRcvMsgEventHandler(SiemensInterface_TcpSrvRcvMsgEventHandler);
            //this.TcpSock4Siemens.startLisner();

            return true;
        }

        public void BeefPlay_Windows_Background()
        {
            gSoundPlayer_Windows_Background.Stop();
            gSoundPlayer_Windows_Background.Play();
        }
        public void BeefPlay_Windows_Foreground()
        {
            gSoundPlayer_Windows_Foreground.Stop();
            gSoundPlayer_Windows_Foreground.Play();
        }
        public void BeefPlay_Windows_Notify_System_Generic(int seconds = 2)
        {
            gSoundPlayer_Windows_Notify_System_Generic.Stop();
            gSoundPlayer_Windows_Notify_System_Generic.Play();

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

        public void BeefPlay_Windows_Exclamation(int seconds = 1)
        {
            gSoundPlayer_Windows_Exclamation.Stop();
            gSoundPlayer_Windows_Exclamation.Play();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                gSoundPlayer_Windows_Exclamation.Stop();
                gSoundPlayer_Windows_Exclamation.Play();

                timer.Stop();
            };
        }

        private void PopupProgressView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            //  PopupProgressView.Init(this);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.LogWin.Close();
            VMT_DataMgr.DestroyVMTClient();
            Environment.Exit(0);
        }

        public void ShowProgressBar(int message, Boolean isShow = false, String text = "")
        {
            PopupProgressView.Visibility = System.Windows.Visibility.Visible;
            PopupProgressView.ShowProgressText(isShow, text);
            PopupProgressView.StartAnimation(message);
        }

        private void HideProgressBarInvoke()
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                        }));
        }

        public void HideProgressBar(int message)
        {
            PopupProgressView.Visibility = System.Windows.Visibility.Collapsed;
            PopupProgressView.StopAnimation(message);
        }

        private void InitAppCallbackFunctions()
        {
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyHandleLogApi(new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyHandleLogApi(NotifyHandleLogApi));

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGPSStatus = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyGPSStatus(NotifyGPSStatus);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGPSStatus(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGPSStatus);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyWIFIStatus(NotifyWIFIStatus);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyWIFIStatus(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyKeepAlive(NotifyKeepAlive);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyKeepAlive(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineNotice = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.CallBack_NotifyMachineNotice(NotifyMessage);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineNotice(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineNotice);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetupDone = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.CallBack_NotifySetupDone(NotifySetupDone);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetupDone(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetupDone);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyAccessRole = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyAccessRole(NotifyAccessRole);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyAccessRole(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyAccessRole);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyLogin4Machine(NotifyLogin4Machine);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyLogin4Machine(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine);

            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogIn = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyLogIn(NotifyLogIn);
            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyLoginInfo(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogIn);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyMachineStatusChanged(NotifyMachineStatusChanged);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineStatusChanged(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged);

            /*NotifyJobOrder???�래*/

            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobOrder = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobOrder(NotifyJobOrder);
            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobOrder(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobOrder);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobChange = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobChange(NotifyJobChange);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobChange(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobChange);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDelete = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobDelete(NotifyJobDelete);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobDelete(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDelete);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDeleteAll = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobDeleteAll(NotifyJobDeleteAll);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobDeleteAll(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDeleteAll);

            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDone = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyJobDone(NotifyJobDone);
            //VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyJobDone(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyJobDone);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStopCodeList = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyMachineStopCodeList(NotifyMachineStopCodeList);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineStopCodeList(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineStopCodeList);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineAccessAction = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyMachineAccessAction(NotifyMachineAccessAction);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyMachineAccessAction(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyMachineAccessAction);

            VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop = new VMT_DataMgr_Common_Callback.Callback_NotifyGetMachineStop(NotifyGetMachineStop);
            VMT_DataMgr_Common_Callback.SetCallBack_NotifyAvailable(VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop);

            VMT_DataMgr_RMG_Callback.static_NotifySetMachineStop = new VMT_DataMgr_RMG_Callback.Callback_NotifySetMachineStop(NotifySetMachineStop);
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySetMachineStop(VMT_DataMgr_RMG_Callback.static_NotifySetMachineStop);

            VMT_DataMgr_Common_Callback.static_NotifyErrorCode = new VMT_DataMgr_Common_Callback.Callback_NotifyErrorCode(NotifyErrorCode);
            VMT_DataMgr_Common_Callback.SetCallBack_NotifyErrorCode(VMT_DataMgr_Common_Callback.static_NotifyErrorCode);

            //VMT_DataMgr_Common_Callback.static_NotifyCFGEKF = new VMT_DataMgr_Common_Callback.Callback_NotifyCFGEKF(NotifyCFGEKF);
            //VMT_DataMgr_Common_Callback.SetCallBack_NotifyCFGEKF(VMT_DataMgr_Common_Callback.static_NotifyCFGEKF);

            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockList(NotifyBlockList));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockListForBlockMap(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockListForBlockMap(NotifyBlockListForBlockMap));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyGetVmtAutoStartConfig(new VMT_DataMgr_Common_Callback.Callback_NotifyGetVmtAutoStartConfig(NotifyGetVmtAutoStartConfig));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapList(NotifyBlockMapList));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapList1(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapList(NotifyBlockMapList1));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapList2(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapList(NotifyBlockMapList2));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapSwapList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapSwapList(NotifyBlockMapSwapList));

            VMT_DataMgr_Common_Callback.SetCallback_NotifyDoSwap4Manual(new VMT_DataMgr_Common_Callback.Callback_NotifyDoSwap4Manual(NotifyDoSwap4Manual));

            VMT_DataMgr_Common_Callback.SetCallback_NotifyException(new VMT_DataMgr_Common_Callback.Callback_NotifyException(NotifyException));

            VMT_DataMgr_Common_Callback.SetCallBack_NotifyConfig(new VMT_DataMgr_Common_Callback.Callback_NotifyConfig(NotifyConfig));

            //--------------------------------------------------
            //- Set RMG Callback
            VMT_DataMgr_RMG_Callback.static_RMG_PDS_Periodic_Payload = new VMT_DataMgr_RMG_Callback.Callback_RMG_PDS_Periodic_Payload(RMG_Periodic);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_PDS_Periodic_Payload(VMT_DataMgr_RMG_Callback.static_RMG_PDS_Periodic_Payload);

            VMT_DataMgr_RMG_Callback.static_RMG_CpsAlign_Payload = new VMT_DataMgr_RMG_Callback.Callback_RMG_CpsAlign_Payload(RMG_CpsAlign);
            VMT_DataMgr_RMG_Callback.SetCallBack_RMG_CpsAlign_Payload(VMT_DataMgr_RMG_Callback.static_RMG_CpsAlign_Payload);

            VMT_DataMgr_RMG_Callback.static_RMG_PickDrop_Payload = new VMT_DataMgr_RMG_Callback.Callback_RMG_PickDrop_Payload(RMG_PickDrop);
            VMT_DataMgr_RMG_Callback.SetCallBack_RMG_PickDrop_Payload(VMT_DataMgr_RMG_Callback.static_RMG_PickDrop_Payload);

            VMT_DataMgr_RMG_Callback.static_RMG_RFID_Payload = new VMT_DataMgr_RMG_Callback.Callback_RMG_RFID_Payload(RMG_RFID);
            VMT_DataMgr_RMG_Callback.SetCallBack_RMG_Rfid_Payload(VMT_DataMgr_RMG_Callback.static_RMG_RFID_Payload);

            VMT_DataMgr_RMG_Callback.static_NotifyJobOrderRMG = new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobOrderRMG(NotifyJobOrderRMG);
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyJobOrderRMG(VMT_DataMgr_RMG_Callback.static_NotifyJobOrderRMG);

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyJobOrderList(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobOrderList(NotifyJobOrderList));

            VMT_DataMgr_RMG_Callback.static_NotifyChangedMachineLocation = new VMT_DataMgr_RMG_Callback.CallBack_NotifyChangedMachineLocation(NotifyChangedMachineLocation);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyChangedMachineLocation(VMT_DataMgr_RMG_Callback.static_NotifyChangedMachineLocation);

            // Get Driver Job History
            VMT_DataMgr_RMG_Callback.static_NotifyGetDriverJobHistory = new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetDriverJobHistory(NotifyGetDriverJobHistory);
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetDriverJobHistory(VMT_DataMgr_RMG_Callback.static_NotifyGetDriverJobHistory);

            VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetMachineStatusChanged = new VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.Callback_NotifyGetMachineStatusChanged(NotifyGetMachineStatusChanged);
            VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetMachineStatusChanged(VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetMachineStatusChanged);

            VMT_DataMgr_RMG_Callback.static_NotifySwapListRMG = new VMT_DataMgr_RMG_Callback.CallBack_NotifySwapListRMG(NotifySwapListRMG);
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySwapListRMG(VMT_DataMgr_RMG_Callback.static_NotifySwapListRMG);

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySwapListRTG(new VMT_DataMgr_RMG_Callback.CallBack_NotifySwapListRTG(NotifySwapListRTG));

            VMT_DataMgr_RMG_Callback.static_NotifySetSwapRMG = new VMT_DataMgr_RMG_Callback.CallBack_NotifySetSwapRMG(NotifySetSwapRMG);
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySetSwapRMG(VMT_DataMgr_RMG_Callback.static_NotifySetSwapRMG);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyPOWInfo = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyPOWInfo(RMG_POWInfo);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyPOWInfo(VMT_DataMgr_RMG_Callback.static_RMG_NotifyPOWInfo);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyBlockEnteranceITV = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyBlockEnteranceITV(RMG_BlockEnteranceITV);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyBlockEnteranceITV(VMT_DataMgr_RMG_Callback.static_RMG_NotifyBlockEnteranceITV);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyManualReadyITV = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyManualReadyITV(RMG_ManualReadyITV);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyManualReadyITV(VMT_DataMgr_RMG_Callback.static_RMG_NotifyManualReadyITV);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyBlockBayInfo = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyBlockBayInfo(RMG_BlockBayInfo);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyBlockBayInfo(VMT_DataMgr_RMG_Callback.static_RMG_NotifyBlockBayInfo);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyBlockBayInfoSimple = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyBlockBayInfoSimple(RMG_BlockBayInfoSimple);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyBlockBayInfoSimple(VMT_DataMgr_RMG_Callback.static_RMG_NotifyBlockBayInfoSimple);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyCorrection = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyCorrection(RMG_Correction);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyCorrection(VMT_DataMgr_RMG_Callback.static_RMG_NotifyCorrection);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifySetCurrentJob = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifySetCurrentJob(RMG_SetCurrentJob);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifySetCurrentJob(VMT_DataMgr_RMG_Callback.static_RMG_NotifySetCurrentJob);

            VMT_DataMgr_RMG_Callback.static_RMG_TargetJob = new VMT_DataMgr_RMG_Callback.Callback_RMG_TargetJob(RMG_TargetJob);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_TargetJob(VMT_DataMgr_RMG_Callback.static_RMG_TargetJob);

            VMT_DataMgr_RMG_Callback.static_RMG_NotifyMarring = new VMT_DataMgr_RMG_Callback.Callback_RMG_NotifyMarring(RMG_Marring);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMarring(VMT_DataMgr_RMG_Callback.static_RMG_NotifyMarring);

            VMT_DataMgr_RMG_Callback.static_RMG_SwapResult = new VMT_DataMgr_RMG_Callback.Callback_RMG_SwapResult(RMG_SwapResult);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_SwapResult(VMT_DataMgr_RMG_Callback.static_RMG_SwapResult);

            VMT_DataMgr_RMG_Callback.static_RMG_ReturnWarning = new VMT_DataMgr_RMG_Callback.Callback_RMG_ReturnWarning(RMG_ReturnWarning);
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_ReturnWarning(VMT_DataMgr_RMG_Callback.static_RMG_ReturnWarning);

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyInventoryListRMG(new VMT_DataMgr_RMG_Callback.CallBack_NotifyInventoryListRMG(NotifyGetInventoryList));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyInventoryListRMG1(new VMT_DataMgr_RMG_Callback.CallBack_NotifyInventoryListRMG(NotifyGetInventoryList1));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyInventoryListRMG2(new VMT_DataMgr_RMG_Callback.CallBack_NotifyInventoryListRMG(NotifyGetInventoryList2));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyInventorySwapListRMG(new VMT_DataMgr_RMG_Callback.CallBack_NotifyInventorySwapListRMG(NotifyGetInventorySwapList));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMachineList(new VMT_DataMgr_RMG_Callback.CallBack_NotifyMachineList(NotifyGetMahchineList));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMachineListofPool(new VMT_DataMgr_RMG_Callback.CallBack_NotifyMachineListofPool(NotifyGetMahchineListofPool));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMachineLogoutCheck(new VMT_DataMgr_RMG_Callback.CallBack_NotifyMachineLogoutCheck(NotifyMahchineLogoutCheck));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyJobOrderByContainer(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobOrderByContainer(NotifyGetJobOrderByContainer));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyDetwinJob(new VMT_DataMgr_RMG_Callback.CallBack_NotifyDetwinJob(NotifySetDetwinJob));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifySetJobStatus(new VMT_DataMgr_RMG_Callback.CallBack_NotifySetJobStatus(NotifySetJobStatus));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyGetContainerInfo(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetContainerInfo(NotifyGetContainerInfo));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyGetTwinContainerInfo(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetContainerInfo(NotifyGetTwinContainerInfo));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyGetTwinContainerInfo(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetContainerInfo(NotifyGetTwinContainerInfo));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetMaxRow(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetMaxRow(NotifyGetMaxRow));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkArea(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkArea));
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkArea1(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkArea1));
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkArea2(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkArea2));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkTier(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkTier));
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkTier1(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkTier1));
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkTier2(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkTier2));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyChangePosition(new VMT_DataMgr_RMG_Callback.CallBack_NotifyChangePosition(NotifyChangePosition));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyJobDone(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobDone(NotifyJobDone));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySetJobReOnChassis(new VMT_DataMgr_RMG_Callback.CallBack_NotifySetJobReOnChassis(NotifyJobReOnChassis));
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyPickedContainer(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobDone(NotifyPickedContainer));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyCheckYcDeTwin(new VMT_DataMgr_RMG_Callback.CallBack_NotifyCheckYcDeTwin(NotifyCheckYcDeTwin));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyValidate4LoadingSwapping(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobDone(NotifyValidate4LoadingSwapping));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyCheckPLCData(new VMT_DataMgr_RMG_Callback.CallBack_NotifyCheckPLCData(NotifyCheckPLCData));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyCheckPLCTwistLock(new VMT_DataMgr_RMG_Callback.CallBack_NotifyCheckPLCTwistLock(NotifyCheckPLCTwistLock));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyInitPLCMessage(new VMT_DataMgr_RMG_Callback.CallBack_NotifyInitPLCMessage(NotifyInitPLCMessage));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyProcessPLC(new VMT_DataMgr_RMG_Callback.CallBack_NotifyProcessPLC(NotifyProcessPLC));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyCancelPLC(new VMT_DataMgr_RMG_Callback.CallBack_NotifyCancelPLC(NotifyCancelPLC));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyReleasePLCLock(new VMT_DataMgr_RMG_Callback.CallBack_NotifyReleasePLCLock(NotifyReleasePLCLock));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyEmptySwappingTargetList(new VMT_DataMgr_RMG_Callback.CallBack_NotifyEmptySwappingTargetList(NotifyEmptySwappingTargetList));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyDoEmptySwap(new VMT_DataMgr_RMG_Callback.CallBack_NotifyDoEmptySwap(NotifyDoEmptySwap));

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyConfig = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyConfig(NotifyLoginConfig);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyConfig(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyConfig);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogout = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyLogout(NotifyLogout);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyLogout(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyLogout);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallback_NotifyGetConfigValue(new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifyGetConfigValue(GetConfigValue));
        }

        #region [Callback Delegate Functions - RMG]
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
                                if (App.TEST_MODE)
                                {
                                    if (typeToLog.ToUpper().Contains("KEEPALIVE"))
                                    {
                                        LogWin.WriteLog("");
                                        LogWin.WriteLog("MAC: " + GetSystemMacStr() + " | IP: " + GetDeviceIpStr() + " | PING TO " + ipToPing + ": " + GetPingTimeAverageStr(ipToPing, 4));
                                        LogWin.WriteLog("");
                                    }
                                    LogWin.WriteLog("SEND: " + typeToLog + " | " + jsonObj, true);
                                }
                                if (App.TEST_WRITE_MODE)
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
                                if (App.TEST_MODE)
                                {
                                    LogWin.WriteLog("RECEIVE: " + typeToLog + (!String.IsNullOrEmpty(receiveTimeMsStr) ? " | " + receiveTimeMsStr + " ms" : "") + " | " + jsonObj);
                                }
                                if (App.TEST_WRITE_MODE)
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

        #region [NotifyLogout]
        public void NotifyLogout(Boolean value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            UserInfo.gUserID = String.Empty;

                            PresentationMgr.Singleton.CorrectionSource.Clear();

                            PresentationMgr.AppWin.UC_IndicatorView.TextBox_UserID.Text = "";
                            PresentationMgr.AppWin.UC_IndicatorView.TextBox_JobCount.Text = "0";

                            //PresentationMgr.AppWin.UC_IndicatorView.Image_Setting.Visibility = Visibility.Visible;
                            PresentationMgr.Singleton.showSetting = false; //20201008 hide setting after logout
                            PresentationMgr.Singleton.showHideButtonsAccessAction();

                            PresentationMgr.AppWin.UC_IndicatorView.Image_WS.Visibility = Visibility.Hidden;

                            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetChangedMachineLocation); //20201030
                            PresentationMgr.Singleton.machineLocationPrevious = null;
                        }));
        }
        #endregion [NotifyLogout]

        #region [GetConfigValue]
        public void GetConfigValue(String value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (getConfigValueKey == "ENABLE_RTG_VMT_BAY_VIEW_SELECTION_YN")
                            {
                                if (value == "Y")
                                {
                                    getConfigValueKey = "ENABLE_RTG_VMT_BAY_VIEW_SELECTION_CODE";
                                    VMT_Data_JAT2.VMT_DataMgr_Common.GetConfigValue_Ask(getConfigValueKey);
                                }
                                else
                                {
                                    PresentationMgr.Singleton.enableBayViewSelection = false;
                                }
                            }
                            else if (getConfigValueKey == "ENABLE_RTG_VMT_BAY_VIEW_SELECTION_CODE")
                            {
                                PresentationMgr.Singleton.enableBayViewSelection = false;
                                if (value.Contains(UserInfo.gMchnID))
                                {
                                    PresentationMgr.Singleton.enableBayViewSelection = true;
                                }
                                else
                                {
                                    String mchnLetterStr = String.Empty;
                                    String mchnNumberStr = String.Empty;
                                    foreach (char c in UserInfo.gMchnID)
                                    {
                                        if (Char.IsLetter(c))
                                            mchnLetterStr += c;
                                        else if (Char.IsNumber(c))
                                            mchnNumberStr += c;
                                    }
                                    Boolean mchnNumberParseIntBool = Int32.TryParse(mchnNumberStr, out int mchnNumberInt);
                                    if (!String.IsNullOrEmpty(value))
                                    {
                                        String[] mchnIdGrpArr = value.Split(',');
                                        foreach (String mchIdGrp in mchnIdGrpArr)
                                        {
                                            String startLetterStr = String.Empty;
                                            String startNumberStr = String.Empty;
                                            String endLetterStr = String.Empty;
                                            String endNumberStr = String.Empty;
                                            Boolean mark = false;
                                            foreach (char c in mchIdGrp)
                                            {
                                                if (!mark && c == '-')
                                                    mark = true;
                                                if (!mark)
                                                {
                                                    if (Char.IsLetter(c))
                                                        startLetterStr += c;
                                                    else if (Char.IsNumber(c))
                                                        startNumberStr += c;
                                                }
                                                else
                                                {
                                                    if (Char.IsLetter(c))
                                                        endLetterStr += c;
                                                    else if (Char.IsNumber(c))
                                                        endNumberStr += c;
                                                }
                                            }
                                            Boolean startNumberParseIntBool = Int32.TryParse(startNumberStr, out int startNumberInt);
                                            Boolean endNumberParseIntBool = Int32.TryParse(endNumberStr, out int endNumberInt);
                                            if (mchnLetterStr == startLetterStr && mchnLetterStr == endLetterStr &&
                                                mchnNumberParseIntBool && startNumberParseIntBool && endNumberParseIntBool &&
                                                mchnNumberInt >= startNumberInt && mchnNumberInt <= endNumberInt)
                                            {
                                                PresentationMgr.Singleton.enableBayViewSelection = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }));
        }
        #endregion [GetConfigValue]

        #region [NotifyGPSStatus]
        public void NotifyGPSStatus(int value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //if (value == 0)
                            //    this.UC_IndicatorView.CheckBox_Siemens.IsChecked = false;
                            //else
                            //    this.UC_IndicatorView.CheckBox_Siemens.IsChecked = true;

                        }));

        }
        #endregion [NotifyGPSStatus]

        #region [NotifyWIFIStatus]
        public void NotifyWIFIStatus(int value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                          new Action(delegate
                          {
                              if (value == 0)
                              {
                                  this.UC_IndicatorView.CheckBox_Wifi.IsChecked = false;
                                  PresentationMgr.AppWin.gIsServerConnected = false;
                              }
                              else
                              {
                                  this.UC_IndicatorView.CheckBox_Wifi.IsChecked = true;
                                  PresentationMgr.AppWin.gIsServerConnected = true;
                              }
                          }));

        }
        #endregion [NotifyWIFIStatus]

        #region [NotifyKeepAlive]
        public void NotifyKeepAlive(String value)
        {
            //LogWin.WriteLog("[FNC]NotifyKeepAlive => value :" + value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                        new Action(delegate
                        {
                            if (String.IsNullOrEmpty(value))
                            {
                                this.UC_IndicatorView.CheckBox_Power.IsChecked = false;
                                this.UC_IndicatorView.CheckBox_Wifi.IsChecked = false;
                                PresentationMgr.AppWin.gIsServerConnected = false;
                                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.LogInView &&
                                    PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MachineSettingView)
                                {
                                    if (NetworkInterface.GetIsNetworkAvailable()) //Dec 19 2019 Check Network Service
                                    {
                                        PresentationMgr.AppWin.UC_DisconnectPopupView.ShowPopup
                                        (VMT_RMG.UC_DisconnectPopupView.UC_PopupViewType.PopupViewType_NetworkDisconnect
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0125", LanguageService.LABEL_CUSTOMIZE)
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0023", LanguageService.LABEL_POPUP), String.Empty, null, 0); //Program service is stopped
                                    }
                                    else
                                    {
                                        PresentationMgr.AppWin.UC_DisconnectPopupView.ShowPopup
                                        (VMT_RMG.UC_DisconnectPopupView.UC_PopupViewType.PopupViewType_NetworkDisconnect
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0125", LanguageService.LABEL_CUSTOMIZE)
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0025", LanguageService.LABEL_POPUP), String.Empty, null, 0);//Network Disconnected
                                    }
                                }
                                Timer.Stop(); // Dec 18 2019 Stop Timer KeepAlive failed
                            }
                            else
                            {
                                PresentationMgr.AppWin.UC_DisconnectPopupView.HidePopup();
                                if (!PresentationMgr.AppWin.gIsServerConnected)
                                    PresentationMgr.Singleton.ThreadResetEvent.Set();
                                timer = value;
                                if (!"".Equals(timer))
                                {
                                    date = DateTime.Parse(timer, System.Globalization.CultureInfo.CurrentCulture);
                                    if (needSetTimeStart)
                                    {
                                        dateTimeStart = date;
                                        PresentationMgr.MainView.UC_InfomationView.TextBlock_loginTime.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0070", LanguageService.LABEL_MAINWINDOW) + " " + date.ToString("HH:mm");
                                        needSetTimeStart = false;
                                        needSetLoginLong = true;
                                    }
                                    if (needSetLoginLong)
                                    {
                                        PresentationMgr.MainView.UC_InfomationView.TextBlock_loginLong.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0072", LanguageService.LABEL_MAINWINDOW)  //Start
                                           + " " + ((int)(date - dateTimeStart).TotalMinutes).ToString() + " "
                                           + PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0073", LanguageService.LABEL_MAINWINDOW); //Min
                                    }
                                }                                              
                                this.UC_IndicatorView.CheckBox_Power.IsChecked = true;
                                this.UC_IndicatorView.CheckBox_Wifi.IsChecked = true;
                                PresentationMgr.AppWin.gIsServerConnected = true;
                                if (!Timer.IsEnabled)
                                    Timer.Start();
                            }
                        }));
        }

        private void Timer_Click(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                       new Action(delegate
                       {
                           if(endPollingPLCwrkCd12 > 0)
                               endPollingPLCwrkCd12++;
                           if (endPollingPLCwrkCd12 == 3)
                           {
                               VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                               endPollingPLCwrkCd12 = 0;
                           }

                           date = date + new TimeSpan(0, 0, 1);

                           //update label
                           UC_IndicatorView.Lbl_time.Content = date.ToString("HH:mm:ss");

                           if (!PresentationMgr.MainView.UC_JobList.IsAvailableBreaking)
                           {
                               PresentationMgr.MainView.UC_BreakTimeView.TextBlock_Break_Start_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");
                           }
                           else
                           {
                               PresentationMgr.MainView.UC_BreakTimeView.TextBlock_Break_End_Date.Text = date.ToString("yyyy/MM/dd") + " " + date.ToString("HH:mm:ss");
                           }
                       }));         
        }

        private void PLCTimer_Click(object sender, EventArgs e)
        {
            if (PresentationMgr.MainView.UC_ListJobPopupView.Visibility == Visibility.Visible || this.UC_PopupView.Visibility == Visibility.Visible ||
                PresentationMgr.MainView.UC_ListJobEmptyContainerView.Visibility == Visibility.Visible || PresentationMgr.MainView.UC_YtSwapView.Visibility == Visibility.Visible)
                return;

            plcPollingCntTimer++;
            Console.WriteLine("plcPollingCnt: " + plcPollingCntTimer.ToString());
            if (plcPollingCntTimer - plcReceiveCnt == 30)
            {
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
            }

            plcCnt++;
            Console.WriteLine("plcCnt: " +plcCnt.ToString());
            if (plcCnt == 30)
            {
                Console.WriteLine("InitPLCMessage_Ask: ");
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
                wrkCd = "";
                VMT_Data_JAT2.VMT_DataMgr_RMG.InitPLCMessage_Ask();
            }
        }

        private void NetworkDisconnectCallback(VMT_RMG.UC_DisconnectPopupView.UC_PopupViewRetType seleted)
        {
            if (seleted == VMT_RMG.UC_DisconnectPopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickOneButton)
            {
                PresentationMgr.APP_CloseApp();
            }
        }
        #endregion [NotifyKeepAlive]

        #region [NotifyMessage]
        public void NotifyMessage(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive value)
        {
            //LogWin.WriteLog("VMT Debug ==> " + "NotifyMessage Sussess ");

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive>(value);

            TimeSpan timeout = new TimeSpan(0, 0, 3); // 3 sec
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, timeout,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            PresentationMgr.MainView.notice = clone.m_strMessage2;
                            PresentationMgr.MainView.SetBtnNotice();
                            if (!String.IsNullOrEmpty(clone.m_strMessage))
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0126", LanguageService.LABEL_CUSTOMIZE), clone.m_strMessage
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                            }
                            //PopupView.ShowPopup(1, "Message", clone.m_strMessage, "", "OK", "", null, 3);
                            //BeefPlay();
                        }));
        }
        #endregion [NotifyMessage]

        #region [NotifySetupDone]
        public void NotifySetupDone(int value)
        {
            //LogMessage("NotifySetupDone => value : " + value.ToString());

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {

                        }));
        }
        #endregion [NotifySetupDone]

        #region [NotifyAccessRole]
        public void NotifyAccessRole(ref VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive>(value);

            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.GroupListSeperator));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            //if (!String.IsNullOrEmpty(clone.shift))
                            //{
                                this.UC_LogInView.userAccessRole = true;
                                clone.Notice = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0007", LanguageService.MESSAGE_GROUP);
                                this.UC_LogInView.ProcessByGetAccessRoleCallback(clone);
                            //}
                            //else
                            //{
                            //    String mess = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("F31", LanguageService.MESSAGE_LOGIN_GROUP);
                            //    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                            //        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0126", LanguageService.LABEL_CUSTOMIZE), mess
                            //        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                            //    this.UC_LogInView.ReFlash();
                            //}
                        }));
        }
        #endregion [NotifyAccessRole]

        #region [NotifyLogin4Machine]
        public void NotifyLogin4Machine(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value)
        {
            //LogMessage("NotifyLogin4Machine => value : " + value.ToString());

            VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            this.loc.blck = clone.loc.blck;
                            this.loc.bay = clone.loc.bay;
                            PresentationMgr.MainView.cntrLockMode = true;

                            if (PresentationMgr.Singleton.showViewINV)
                            { //20201030 if VIEW_INV = FALSE, call getChangedMachineLocation every 4 secs. else [keep current process]
                                VMT_DataMgr_RMG.GetChangedMachineLocation_Ask(loc.blck, loc.bay);
                            } 
                            else
                            {
                                //VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetChangedMachineLocation);
                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetChangedMachineLocation);
                            }
                            
                            this.UC_LogInView.ProcessByLogin4MachineCallback(clone);

                            needSetTimeStart = true;
                            VMT_DataMgr_RMG.GetMaxRow_Ask();
                            VMT_DataMgr_Common.GetMachineStopCodeList_Ask();

                            // Set default AllMode, don't need to StartPolling because it will be processed after that
                            if (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobBlock)
                            {
                                PresentationMgr.MainView.SetAllModeWithoutStartPolling();
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
                            wrkCd = String.Empty;
                            wrkCdPLC = String.Empty;

                            if (!"CLT".Equals(RMG.RMG_User.gUserID))
                            {
                                getConfigValueKey = "ENABLE_RTG_VMT_BAY_VIEW_SELECTION_YN";
                                VMT_Data_JAT2.VMT_DataMgr_Common.GetConfigValue_Ask(getConfigValueKey);

                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.SetVMTMachineStatus);
                            }
                            else
                            {
                                PresentationMgr.Singleton.enableBayViewSelection = true;
                            }
                        }));
        }
        #endregion [NotifyLogin4Machine]

        #region [NotifyChangedMachineLocation]
        public void NotifyChangedMachineLocation(VMT_Data_JAT2.Objects.Common.VmtMachine value)
        {
            //LogMessage("NotifyLogin4Machine => value : " + value.ToString());

            VMT_Data_JAT2.Objects.Common.VmtMachine clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VmtMachine>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VmtMachine>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);

                            /* //20201030
                             * Only import and reflect bay data when the block is changed
                                BF / 01 -> call API getChangedMachineLocation -> AE / 03 -> change inventory view AE / 03
                                BF / 01 -> call API getChangedMachineLocation -> BF / 03 -> do not change inventory view
                             * */
                            if (!PresentationMgr.Singleton.showViewINV && ((!PresentationMgr.Singleton.CurrentBlock.Equals(String.Empty) && !PresentationMgr.Singleton.CurrentBlock.Equals(clone.blck))
                            && (PresentationMgr.Singleton.machineLocationPrevious == null || !(PresentationMgr.Singleton.machineLocationPrevious.blck.Equals(value.blck) && PresentationMgr.Singleton.machineLocationPrevious.bay.Equals(value.bay)))))
                            {
                                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                                {
                                    m_cBlock = clone.blck,
                                    m_cBay = clone.bay,
                                    m_cRow = String.Empty,
                                    m_cTier = String.Empty
                                };

                                PresentationMgr.Singleton.CurrentPostion = pos;

                                PresentationMgr.Singleton.machineLocationPrevious = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VmtMachine>(value); //20201102 if value same -> no reload bayview
                                //VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(clone.blck);

                                return;
                            }

                            if (!firstLoad) return;
                            
                            string currBay = clone.bay;
                            if (currBay != "" && currBay != "00")
                            {
                                int bay = int.Parse(PresentationMgr.BayRemoveChars(currBay));
                                if (bay % 2 == 0) bay -= 1;
                                currBay = (bay < 10 ? "0" : "") + bay.ToString();
                            }
                            else if (currBay == "00")
                                return;

                            clone.bay = currBay;

                            this.locChg.blck = clone.blck;
                            this.locChg.bay = clone.bay;

                            if (firstLoad == true && 
                                    (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(clone.blck)
                                            || DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[clone.blck].DicBay == null
                                            || (!String.IsNullOrEmpty(clone.bay) &&  !DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[clone.blck].DicBay.ContainsKey(clone.bay) 
                                                )
                                    )
                            )
                            {
                                return;
                            }

                            if ("NA".Equals(locChgFlg))
                            {
                                if (this.loc.blck == "" && this.loc.bay == "")
                                {
                                    this.loc.blck = clone.blck;
                                    this.loc.bay = clone.bay;
                                    setCurrentPositionByFlag();
                                }
                                //dtClockTime.Interval = new TimeSpan(0, 0, 0, 5);
                                //dtClockTime.Tick += new EventHandler(TimerCallGetChangedMachineLocation);
                                //dtClockTime.Start();
                            }

                            if (wrkCd == "3")
                            {
                                //locChgFlg = clone.locChgFlg;
                                autoFlg = "Y";
                                locChgFlg = "Y";
                                PresentationMgr.Singleton.highlightRow = "";
                                setCurrentPositionByFlag();
                            }
                            else if (wrkCd == "4")
                            {
                                locChgFlg = "Y";
                                autoFlg = "Y";
                                PresentationMgr.Singleton.highlightRow = clone.row;
                                PresentationMgr.Singleton.SetInventoryData(null);
                                setCurrentPositionByFlag();
                            }
                            else
                                PresentationMgr.Singleton.highlightRow = "";

                        }));
        }

        private void setCurrentPositionByFlag()
        {
            if (this.autoFlg == "Y" && locChgFlg == "Y")
            {
                locChgFlg = "";
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = this.locChg.blck,
                    m_cBay = this.locChg.bay,
                    m_cRow = "",
                    m_cTier = ""
                };
                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        private void TimerCallGetChangedMachineLocation(object sender, EventArgs e)
        {
            //VMT_DataMgr_RMG.GetChangedMachineLocation_Ask(locChg.blck, locChg.bay);
        }
        #endregion [NotifyChangedMachineLocation]

        #region [NotifyMachineStatusChanged]
        public void NotifyMachineStatusChanged(ref VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive>(value);

            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, Convert.ToString(value.m_iResult)));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            this.UC_LogInView.ProcessByMachineStatusChangeCallback(clone);
                        }));
        }
        #endregion [NotifyMachineStatusChanged]

        #region [NotifyJobOrderRMG]
        public void NotifyJobOrderRMG(RMG.VD_RMG_JobOrderList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_JobOrderList>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString())); 

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_JobOrderList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                          var pJob = clone.JobOrder.Find(x => x.type.jobStatus.Equals("P"));
                          //if (pJob != null)
                          //    PresentationMgr.MainView.SaveLog("NotifyJobOrderRMG_processing");
                          //else
                          //    PresentationMgr.MainView.SaveLog("NotifyJobOrderRMG_NonpProcessing");

                          //20200221 error display joblist when wrkCd == 1 or 2 after ProcessPLC

                          checkWrkCdPLCCount += 1;
                          if (needToCheckYtNo)
                          {
                              if (ytNoCheckCount > 2)
                              {
                                  ytNoCheckCount = 0;
                                  needToCheckYtNo = false;
                              }
                              else
                              {
                                  ytNoCheckCount += 1;
                              }
                          }
                          
                          if (checkWrkCdPLCCount > 3)
                          {
                              wrkCdPLC = String.Empty;
                          }

                          if (endPollingPLCwrkCd12 > 0)
                          {
                              return;
                          }

                          if (!String.IsNullOrEmpty(wrkCdPLC))
                          {
                              var jobItem = clone.JobOrder.Find(x => x.type.jobStatus.Equals("P"));
                              if (wrkCdPLC.Equals("1")) {
                                  if (jobItem == null)
                                      return;
                              }
                              else // wrkCd == 2
                              {
                                  if (jobItem != null)
                                      return;
                              }
                              wrkCdPLC = String.Empty;
                          }

                          checkWrkCdPLCCount = 0;

                          PresentationMgr.Singleton.JOB_Clear();

                          PresentationMgr.Singleton.ProcessingJobKey = String.Empty;

                          //int i = 10;
                          // Add JobOrder Data Structure
                          int ytNoFixedCount = 0;
                          foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in clone.JobOrder)
                          {
                              if (jobOrder.spndFlg == "Y")
                              {
                                  continue;
                              }

                              if (String.IsNullOrEmpty(jobOrder.jobKey))
                                  continue;//jobOrder.jobKey = System.Guid.NewGuid().ToString();                    

                              //Check YtNo Changed
                              
                              if (needToCheckYtNo && PresentationMgr.MainView.UC_YtSwapView.newYtNo != PresentationMgr.MainView.UC_YtSwapView.oldYtNo)
                              {
                                  if (jobOrder.jobKey.Equals(PresentationMgr.MainView.UC_YtSwapView.jobKeyChangeYtNo))
                                  {
                                      if (!jobOrder.partnerMchn.mchnId.Equals(PresentationMgr.MainView.UC_YtSwapView.newYtNo))
                                      {
                                          jobOrder.partnerMchn.mchnId = PresentationMgr.MainView.UC_YtSwapView.newYtNo;
                                          ytNoFixedCount += 1;
                                      }
                                  }
                                  else if (jobOrder.partnerMchn.mchnId.Equals(PresentationMgr.MainView.UC_YtSwapView.newYtNo) &&
                                            jobOrder.type != null && jobOrder.type.jobFlagInfo.Equals(PresentationMgr.MainView.UC_YtSwapView.currentPositionOnChassis))
                                  {
                                      jobOrder.partnerMchn.mchnId = PresentationMgr.MainView.UC_YtSwapView.oldYtNo;
                                      ytNoFixedCount += 1;
                                  }
                              }                             
                              PresentationMgr.Singleton.JOB_Add(jobOrder);
                              //i++;
                          }

                          if (needToCheckYtNo && ytNoFixedCount > 1)
                          {
                              needToCheckYtNo = false;
                          }

                          //TEST DATA
                          //if (DataMgr.Singleton.List_JobOrder.Count >= 1)
                          //{
                          //    //DataMgr.Singleton.List_JobOrder[0].type.jobStatus = "P";
                          //    //DataMgr.Singleton.List_JobOrder[0].vbsDate = "2020012615230000";
                          //    DataMgr.Singleton.List_JobOrder[0].locFrom.location = "67MT02C3";
                          //    DataMgr.Singleton.List_JobOrder[0].locWorking.location = "67MT02C3";
                          //}

                          PresentationMgr.Singleton.JOB_Sort();

                          JobList jobList = PresentationMgr.MainView.UC_JobList;
                        
                          //20201008 show/hide mainview info BP#475
                          if (PresentationMgr.Singleton.showViewINV)
                          {
                              PresentationMgr.MainView.Grid_Block_All.Visibility = Visibility.Visible;
                              PresentationMgr.MainView.Btn_JobSet.Visibility = Visibility.Visible;
                              PresentationMgr.MainView.Label_JobSet.Visibility = Visibility.Visible;

                              PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Visibility = Visibility.Visible;
                              PresentationMgr.MainView.Grid_Right_Bottom_Button.Visibility = Visibility.Visible;

                              if (DataMgr.Singleton.List_JobOrder.Count > 0 && PresentationMgr.MainView.UC_BayView.Visibility == Visibility.Visible)
                              {
                                  PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;
                              }
                          }
                          else
                          {
                              PresentationMgr.MainView.Grid_Block_All.Visibility = Visibility.Collapsed;            
                              if (DataMgr.Singleton.List_JobOrder.Count < 1)
                              {
                                  if (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobBlock) //if all have data and block not exists data
                                  {
                                      PresentationMgr.MainView.SetAllMode();
                                      return;
                                  }
                                  PresentationMgr.MainView.Btn_JobSet.Visibility = Visibility.Collapsed;
                                  PresentationMgr.MainView.Label_JobSet.Visibility = Visibility.Collapsed;
                                  PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Collapsed;
                                  PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Collapsed;
                                  PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Visibility = Visibility.Collapsed;
                                  PresentationMgr.MainView.Grid_Right_Bottom_Button.Visibility = Visibility.Collapsed;
                              }
                              else
                              {
                                  PresentationMgr.MainView.Btn_JobSet.Visibility = Visibility.Visible;
                                  PresentationMgr.MainView.Label_JobSet.Visibility = Visibility.Visible;

                                  PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Visibility = Visibility.Visible;
                                  PresentationMgr.MainView.Grid_Right_Bottom_Button.Visibility = Visibility.Visible;
                              }
                          }

                          //Notify the new LD MO job (S)
                          bool chkLDMOJobYTAssigned = false;
                          bool chkNewJob = false;
                          foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
                          {
                              //CHECK LD/MO JOB WITH YT ASSIGNED
                              if (firstLoad && ("LD".Equals(jobOrder.type.jobTp) || "MO".Equals(jobOrder.type.jobTp))  // first load => Add LD MO job exists ytNo
                              && !String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId)
                              && !lstLDMOJobYTAssigned.Contains(jobOrder.cntr.cntrNo))
                              {
                                  lstLDMOJobYTAssigned.Add(jobOrder.cntr.cntrNo);
                                  lstYTAssigned.Add(jobOrder.partnerMchn.mchnId);
                              }
                              else if (!firstLoad && ("LD".Equals(jobOrder.type.jobTp) || "MO".Equals(jobOrder.type.jobTp))
                                          && !String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId)
                                          && !lstLDMOJobYTAssigned.Contains(jobOrder.cntr.cntrNo)
                                        ) //ADD NEW LD MO JOB YT ASSIGNED 
                              {
                                  lstLDMOJobYTAssigned.Add(jobOrder.cntr.cntrNo);
                                  lstYTAssigned.Add(jobOrder.partnerMchn.mchnId);

                                  if (!lstOldJob.Contains(jobOrder.cntr.cntrNo))
                                      chkNewJob = true;
                                  else
                                      chkLDMOJobYTAssigned = true;
                              }
                              else if (!firstLoad && ("LD".Equals(jobOrder.type.jobTp) || "MO".Equals(jobOrder.type.jobTp)) //COMPARE YT ASSIGNED OF LD MO JOB
                                            && lstLDMOJobYTAssigned.Contains(jobOrder.cntr.cntrNo)
                                        )
                              {
                                  if (lstYTAssigned[lstLDMOJobYTAssigned.IndexOf(jobOrder.cntr.cntrNo)] != jobOrder.partnerMchn.mchnId)
                                  {
                                      lstYTAssigned[lstLDMOJobYTAssigned.IndexOf(jobOrder.cntr.cntrNo)] = jobOrder.partnerMchn.mchnId;
                                      if (!String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId))
                                      {
                                          chkLDMOJobYTAssigned = true;
                                      }
                                  }
                              }

                              //CHECK NEW JOB
                              if (firstLoad)
                              {
                                  lstOldJob.Add(jobOrder.cntr.cntrNo);
                              }
                              else if (!lstOldJob.Contains(jobOrder.cntr.cntrNo))
                              {
                                  lstOldJob.Add(jobOrder.cntr.cntrNo);
                                  chkNewJob = true;
                              }
                          }

                          if(chkLDMOJobYTAssigned)
                              alertLDMOJobYTAssigned();

                          if (chkNewJob)
                              alertNewJob();
                          //Notify the new LD MO job (E)

                          //Set Start location for bayview after login from location SetLogin4Machine
                          if (firstLoad)
                          {
                              string blck = this.loc.blck;
                              string bay = this.loc.bay;
                              if (String.IsNullOrEmpty(blck))
                              {
                                  foreach (var item in DataMgr.Singleton.List_JobOrder)
                                  {
                                      if (!String.IsNullOrEmpty(item.locWorking.blck) && !String.IsNullOrEmpty(item.locWorking.bay))
                                      {
                                          blck = item.locWorking.blck;
                                          bay = item.locWorking.bay;
                                          break;
                                      }
                                      else if (!String.IsNullOrEmpty(item.locFrom.blck) && !String.IsNullOrEmpty(item.locFrom.bay))
                                      {
                                          blck = item.locFrom.blck;
                                          bay = item.locFrom.bay;
                                          break;
                                      }
                                  }
                              }

                              var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                              {
                                  m_cBlock = blck,
                                  m_cBay = bay,
                                  m_cRow = "",
                                  m_cTier = ""
                              };
                              PresentationMgr.Singleton.CurrentPostion = pos;
                              jobList.CurrentPageIndex = 0;
                          }

                          firstLoad = false;
                          
                          //AUTO SELECT THE JOB
                          if (MainWin.autoSelectP)
                          {
                              String cntrNo = PresentationMgr.MainView.plcDomainTwistLock.cntrNo;
                              var jobLst = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);
                              if (jobLst.Count > 0)
                              {
                                  var jobOrder = jobLst.Find(x => x.type.jobStatus == "P");
                                  if (jobOrder == null)
                                      jobOrder = jobLst.First();
                                  if (jobOrder.jobKey != VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey)
                                  {
                                      VMT_Data_JAT2.Marshalling.Geometry.sPosition ContainerPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                      var loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location();

                                      if (jobOrder.type == null)
                                      {
                                          loc = jobOrder.locWorking;
                                      } else
                                      {
                                          if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "GO")
                                              loc = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                                          else if (jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "MI" || jobOrder.type.jobTp == "GI"
                                              || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH" || jobOrder.type.jobTp == "LC" || jobOrder.type.jobTp == "GC")
                                              loc = jobOrder.locWorking;
                                      }
                                      if(loc != null)
                                      {
                                          ContainerPos.m_cBlock = loc.blck;
                                          ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(loc.bay);
                                          ContainerPos.m_cRow = loc.row;
                                          ContainerPos.m_cTier = loc.tier;
                                          PresentationMgr.Singleton.CorrectionSource.SetPos(ContainerPos);
                                          PresentationMgr.Singleton.CorrectionSource.CntrNo = cntrNo;
                                          PresentationMgr.Singleton.CorrectionSource.CntrIso = jobOrder.cntr.cntrIso;
                                      }
                                      VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobOrder.jobKey;
                                  }
                              }
                              MainWin.autoSelectP = false;
                          }

                          // CHECK SELECTED JOB LOCATION CHANGE BY PNO - AUG 31 2020 - QUAN LE
                          var jobKeyCheckLoc = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                          var jobOrderCheckLoc = PresentationMgr.Singleton.JOB_Get(jobKeyCheckLoc);
                          if (jobOrderCheckLoc != null)
                          {
                              var loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location();
                              if (jobOrderCheckLoc.type == null)
                              {
                                  loc = jobOrderCheckLoc.locWorking;
                              }
                              else
                              {
                                  if (jobOrderCheckLoc.type.jobTp == "LD" || jobOrderCheckLoc.type.jobTp == "MO" || jobOrderCheckLoc.type.jobTp == "GO")
                                      loc = string.IsNullOrEmpty(jobOrderCheckLoc.locFrom.location) ? jobOrderCheckLoc.locWorking : jobOrderCheckLoc.locFrom;
                                  else if (jobOrderCheckLoc.type.jobTp == "DS" || jobOrderCheckLoc.type.jobTp == "MI" || jobOrderCheckLoc.type.jobTp == "GI"
                                      || jobOrderCheckLoc.type.jobTp == "RH" || jobOrderCheckLoc.type.jobTp == "AH" || jobOrderCheckLoc.type.jobTp == "LC" || jobOrderCheckLoc.type.jobTp == "GC")
                                      loc = jobOrderCheckLoc.locWorking;
                              }
                              if (loc != null)
                              {
                                  if (PresentationMgr.Singleton.CorrectionSource.Pos.m_cBlock != loc.blck || PresentationMgr.Singleton.CorrectionSource.Pos.m_cBay != PresentationMgr.GetFrontOddBay(loc.bay) ||
                                        PresentationMgr.Singleton.CorrectionSource.Pos.m_cRow != loc.row || PresentationMgr.Singleton.CorrectionSource.Pos.m_cTier != loc.tier)
                                  {
                                      if (PresentationMgr.Singleton.CorrectionSource.Pos.m_cBlock != loc.blck || PresentationMgr.Singleton.CorrectionSource.Pos.m_cBay != PresentationMgr.GetFrontOddBay(loc.bay))
                                      {
                                          PresentationMgr.Singleton.NeedJobAutoSelection = true;
                                          PresentationMgr.Singleton.SendGetInventoryAsk(loc.blck, loc.bay);
                                      }
                                      VMT_Data_JAT2.Marshalling.Geometry.sPosition ContainerPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                      ContainerPos.m_cBlock = loc.blck;
                                      ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(loc.bay);
                                      ContainerPos.m_cRow = loc.row;
                                      ContainerPos.m_cTier = loc.tier;
                                      PresentationMgr.Singleton.CorrectionSource.SetPos(ContainerPos);
                                  }
                              }
                          }

                          //PresentationMgr.Singleton.JL_SearchNRefresh(jobList, PresentationMgr.Singleton.SelectBlckName, jobList.CurrentPageIndex);

                          PresentationMgr.Singleton.JL_Refresh(jobList, jobList.CurrentPageIndex, true);
                          if (PresentationMgr.MainView.blckAllSwitch)
                          {
                              PresentationMgr.Singleton.SetInventoryData(null);
                              PresentationMgr.MainView.blckAllSwitch = false;
                          }
                          //PresentationMgr.AppWin.HideProgressBar(0);
                          //PresentationMgr.Singleton.JBN_AutoTargetJob();
                          clone = null;
                      }));
        }
        public void NotifyJobOrderList(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> listJobOrder)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          this.UC_MainView.UC_OtherMachineJobListView.SetListOtherMachineJobCallBack(listJobOrder);
                      }));
        }
        public void alertLDMOJobYTAssigned(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = null)
        {
            //var mess = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0217", LanguageService.LABEL_CUSTOMIZE), jobOrder.partnerMchn.mchnId, jobOrder.cntr.cntrNo);
            //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0038", LanguageService.LABEL_POPUP), mess, "OK", null, 0);

            BeefPlay_Windows_Background();
        }

        public void alertNewJob(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = null)
        {
            //var mess = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0217", LanguageService.LABEL_CUSTOMIZE), jobOrder.partnerMchn.mchnId, jobOrder.cntr.cntrNo);
            //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0038", LanguageService.LABEL_POPUP), mess, "OK", null, 0);

            BeefPlay_Windows_Notify_System_Generic();
        }
        #endregion [NotifyJobOrderRMG]

        #region [NotifyGetDriverJobHistory]
        public void NotifyGetDriverJobHistory(RMG.VD_RMG_JobOrderList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_JobOrderList>(value);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          UC_MainView.UC_DriverWorkingHistory.SetListDriverWorkingCallBack(clone.JobOrder);

                          clone = null;
                      }));
        }
        #endregion [NotifyGetDriverJobHistory]

        #region [NotifyGetMachineStatusChanged]
        public void NotifyGetMachineStatusChanged(VMT_Data_JAT2.Objects.Common.VmtMachine machine)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate
                    {
                        //PresentationMgr.AppWin.UC_IndicatorView.TextBox_WSOnOff.Visibility = Visibility.Visible; //20200611 no need show W/S
                        //PresentationMgr.AppWin.UC_IndicatorView.Label_WS.Visibility = Visibility.Visible;
                        AccessText wgtSysStsCd = new AccessText()
                        {
                            Text = machine.wgtSysStsCd,
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center,
                        };
                        PresentationMgr.AppWin.UC_IndicatorView.TextBox_WSOnOff.Content = wgtSysStsCd;

                        String wgtSysStsCdText = wgtSysStsCd.Text.ToUpper();
                        String imgName = "IndicatorView_WS_" + (wgtSysStsCdText.Equals("WORKING") ? "green" : wgtSysStsCdText.Equals("BREAK DOWN") ? "red" : wgtSysStsCdText.Equals("NOT EQUIPPED") ? "yellow" : "default") + ".png";
                        this.UC_IndicatorView.Image_WS.Source = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/" + imgName, UriKind.Relative));

                        if (machine.mchnMd == "Y")
                        {
                            this.UC_IndicatorView.CheckBox_AutoFlg.IsChecked = true;
                            //PresentationMgr.AppWin.UC_MainView.UC_InfomationView.UC_TargetJobInfo.Label_OnOff.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0075", LanguageService.LABEL_MAINWINDOW);
                            //PresentationMgr.AppWin.UC_MainView.UC_InfomationView.UC_TargetJobInfo.Label_OnOff_Color.Background = new SolidColorBrush(Colors.LimeGreen);
                        }
                        else
                        {
                            this.UC_IndicatorView.CheckBox_AutoFlg.IsChecked = false;
                            //PresentationMgr.AppWin.UC_MainView.UC_InfomationView.UC_TargetJobInfo.Label_OnOff.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0076", LanguageService.LABEL_MAINWINDOW);
                            //PresentationMgr.AppWin.UC_MainView.UC_InfomationView.UC_TargetJobInfo.Label_OnOff_Color.Background = new SolidColorBrush(Colors.Red);
                        }
                    }));
            if (machine.loginUsrLst != null &&
                !machine.loginUsrLst.Contains(RMG.RMG_User.gUserID) &&
                (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.LogInView &&
                PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MachineSettingView))
            {

                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate
                    {
                        if ("CLT".Equals(UserInfo.gUserID) && "ACCESS".Equals(UserInfo.gUserPW))
                            return;
                        PresentationMgr.AppWin.UC_LogInView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Logout,
                            PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0036", LanguageService.MESSAGE_GROUP), //Log Off
                            PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0057", LanguageService.MESSAGE_GROUP), //Force Logoff been processed.
                            PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 3); //OK

                        if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
                        {
                            PresentationMgr.AppWin.PLCTimer.Stop();
                        }
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                    }));
            }

            /* 20190806 machineNotice process
            if (!String.IsNullOrEmpty(machine.noticeMsg))
            {
                Logger.Log("Received Notice Message : " + machine.noticeMsg);
                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, machine.noticeMsg);

                //VMT_DataMgr_Common.SetMachineNotice(); //20190806

                TimeSpan timeout = new TimeSpan(0, 0, 3); // 3 sec
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, timeout,
                            new Action(delegate
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Message", machine.noticeMsg, "OK", null, 0);
                            }));
            }*/
        }
        #endregion [NotifyGetMachineStatusChanged]

        #region [NotifySwapListRMG]
        public void NotifySwapListRMG(RMG.VD_RMG_SwapList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_SwapList>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString())); 

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_SwapList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                          // Add JobOrder Data Structure
                          PresentationMgr.MainView.UC_SwapView.UC_JobListSwap.JobList_Clear();
                          DataMgr.Singleton.List_swap = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();
                          foreach (VMT_Data_JAT2.Objects.Common.VmtSwap swapJob in clone.vmtSwap)
                          {
                              PresentationMgr.MainView.UC_SwapView.UC_JobListSwap.JobList_Add(swapJob);
                              DataMgr.Singleton.List_swap.Add(swapJob);
                          }

                          if (clone.vmtSwap.Count > 0)
                          {
                              PresentationMgr.MainView.UC_SwapView.Btn_Swap.IsEnabled = true;
                          }
                          else
                          {
                              PresentationMgr.MainView.UC_SwapView.Btn_Swap.IsEnabled = false;
                          }


                          JobListSwap jobList = PresentationMgr.MainView.UC_SwapView.UC_JobListSwap;
                          //PresentationMgr.Singleton.JL_SearchNRefresh(jobList, PresentationMgr.Singleton.SelectBlckName, jobList.CurrentPageIndex);
                          clone = null;
                          PresentationMgr.Singleton.JLS_Refresh(jobList, jobList.CurrentPageIndex, true);

                          //PresentationMgr.AppWin.HideProgressBar(0);
                          //PresentationMgr.Singleton.JBN_AutoTargetJob();

                          PresentationMgr.MainView.UC_SwapView.UC_JobListSwap.JobList_SelectFirstItem();

                      }));
        }
        #endregion [NotifySwapListRMG]

        #region [NotifySwapListRTG]
        public void NotifySwapListRTG(RMG.VD_RMG_SwapList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_SwapList>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_SwapList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                          
                          PresentationMgr.Singleton.swapListRTG.Clear();

                          foreach (VMT_Data_JAT2.Objects.Common.VmtSwap swapJob in clone.vmtSwap)
                          {
                              PresentationMgr.Singleton.swapListRTG.Add(swapJob);
                          }

                          if (PresentationMgr.Singleton.swapListRTG.Count > 0 && !String.IsNullOrEmpty(RMG.RMG_Member.Singleton.TargetJobKey))
                          {
                              PresentationMgr.Singleton.SetInventoryData(null);
                          }
                          else
                          {
                              PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, Convert.ToString(PresentationMgr.Singleton.CurrentBay), true, true, false);
                          }
                          clone = null;
                      }));
        }
        #endregion [NotifySwapListRTG]

        #region [NotifySetSwapRMG]
        public void NotifySetSwapRMG(RMG.VD_RMG_VmtResult value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_VmtResult>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString())); 

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_VmtResult>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                          if ("S1".Equals(clone.resultObj))
                          {
                              PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                "SUCCESS", PresentationMgr.GetEmptySwapErrorMessage(clone.resultObj), "OK", new UC_PopupView.Callback_Popup(CallbackClosePopup), 0);
                          }
                          else
                          {
                              PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                "Fail", PresentationMgr.GetEmptySwapErrorMessage(clone.resultObj), "OK", null, 0);
                          }
                          clone = null;
                      }));
        }
        #endregion [NotifySetSwapRMG]

        #region [CallbackClosePopup]
        public void CallbackClosePopup(UC_PopupView.UC_PopupViewRetType selected)
        {
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
        }
        #endregion [CallbackClosePopup]

        #region [NotifyJobDeleteAll]
        public void NotifyJobDeleteAll(int value)
        {
            //LogMessage("NotifyJobDeleteAll => value : " + value.ToString());

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);

                            PresentationMgr.Singleton.JOB_Clear();
                        }));
        }
        #endregion [NotifyJobDeleteAll]

        #region [NotifyJobChange]
        public void NotifyJobChange(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder value)
        {
            // Debug.WriteLine("received NotifyJobChange");

            //MainWindow mw = PresentationMgr.AppWin;
            //       this.gMchnID = value.MchnID;

            int i = Marshal.SizeOf(value);

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(value);


            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.JOB_Add(clone);

                            PresentationMgr.Singleton.JBN_AutoTargetJob();

                            //this.HideProgressBar(0);
                            //this.IndicatorView.StopAnimationTruck();
                            //this.MainView.ProcessByJobChangeCallback(value);
                            //this.IndicatorView.StartJobTimer(0);
                        }));
        }
        #endregion [NotifyJobChange]

        #region [NotifyJobDelete]
        public void NotifyJobDelete(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobKey value)
        {
            //Debug.WriteLine("received NotifyJobDelete");

            VMT_Data_JAT2.Objects.Common.VD_Common_JobKey clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobKey>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_JobKey>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            //PresentationMgr.Singleton.JOB_Remove(clone); // TODO
                            //PresentationMgr.Singleton.JBN_AutoTargetJob();
                        }));


        }
        #endregion [NotifyJobDelete]

        #region [NotifyJobDone]
        public void NotifyJobDone(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(value);

            // wrkCd == 2
            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            endPollingPLCwrkCd12 = 1;

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {

                            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            {
                                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                            }

                            if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;

                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(
                                System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);

                            if (clone.resultTP.Equals("SUCCESS"))
                            {
                                if (PresentationMgr.MainView.UC_VBlockView.Visibility != System.Windows.Visibility.Visible) //20200612 fix clear bayviewdata when click virtual at VBlock screen
                                {
                                    PresentationMgr.Singleton.CorrectionSource.Clear();
                                    //PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                                }

                                System.Threading.Thread.Sleep(500);
                                //PresentationMgr.Singleton.ThreadTimerStart(true);
                                PresentationMgr.Singleton.SendGetInventoryList4Multi_Sync_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);

                                {//SET LAYOUT DEFAULT FOR YT BUTTON
                                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content = String.Empty;
                                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = String.Empty;

                                    if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                                    {
                                        PresentationMgr.SetSkinButton(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber,
                                            UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                                    }
                                    else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                                    {
                                        PresentationMgr.SetSkinButton(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber,
                                            UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                                    }
                                }

                                var jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                                //PresentationMgr.RemoveLoadingSwapInfo(jobKey);

                                //var jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                                //if (jobOrder != null && jobOrder.type != null && !string.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                                //    PresentationMgr.RemoveLoadingSwapInfo(jobOrder.type.ycTwinKey);

                                if (!String.IsNullOrEmpty(jobKey))
                                {                                  
                                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
                                    {
                                        if (jobOrder.jobKey == jobKey)
                                        {
                                            DataMgr.Singleton.List_JobOrder.Remove(jobOrder);
                                            break;
                                        }
                                    }
                                    PresentationMgr.Singleton.JOB_Sort();
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                                    PresentationMgr.Singleton.ProcessingJobKey = String.Empty;
                                    PresentationMgr.MainView.deselectJobList = true;
                                    PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(500);
                                    VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                                }
                            }
                            else
                            {
                                if (clone.resultObj.Equals("F5"))
                                {
                                    BeefPlay_Windows_Exclamation();

                                    String message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP);

                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), message,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                }
                                else if (clone.resultObj.Equals("F16"))
                                {
                                    String message = String.Empty;
                                    if (UserInfo.gMchnTp == "TC")
                                        message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP);
                                    else
                                        message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("F16a", LanguageService.MESSAGE_JOBSETERROR_GROUP);

                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0221", LanguageService.LABEL_CUSTOMIZE), message,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                }
                                else
                                {
                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                   PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                   , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                }
                            }
                            //MainView.stopLoad = false;
                            clone = null;
                        }));
        }
        #endregion [NotifyJobDone]

        #region [NotifyPickedContainer]
        public void NotifyPickedContainer(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();
                            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            {
                                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            }
                            else
                                VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, true);

                            if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;
                            //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                            PresentationMgr.Singleton.ThreadTimerStart(true);

                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(
                                System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);

                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                if (ContainerSelectionView.OldProcessingJobKeys != null)
                                    ContainerSelectionView.OldProcessingJobKeys.Clear();

                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                            }
                            else
                            {
                                if (ContainerSelectionView.OldProcessingJobKeys != null)
                                {
                                    if (ContainerSelectionView.OldProcessingJobKeys.Count >= 2)
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobReOnChassis_Ask(ContainerSelectionView.OldProcessingJobKeys[0], ContainerSelectionView.OldProcessingJobKeys[1]);
                                    else if (ContainerSelectionView.OldProcessingJobKeys.Count == 1)
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobReOnChassis_Ask(ContainerSelectionView.OldProcessingJobKeys[0]);
                                    ContainerSelectionView.OldProcessingJobKeys.Clear();
                                }

                                PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
                                PresentationMgr.Singleton.NeedJobAutoSelection = true;
                            }

                            clone = null;
                        }));
        }
        #endregion [NotifyPickedContainer]

        #region [NotifyJobReOnChassis]
        public void NotifyJobReOnChassis(Boolean value)
        {
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));
            InterfaceMessageLoader.instance().WriteInterfaceMessage<bool?>(System.Reflection.MethodBase.GetCurrentMethod().Name, (bool?)value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();
                            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            {
                                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            }
                            else
                                VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, true);
                            PresentationMgr.Singleton.ThreadTimerStart(true);
                            if (!value)
                            {
                                //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "SetJobReOnChassis", "Failed!", "OK", null, 0);
                            }
                            PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
                            PresentationMgr.Singleton.NeedJobAutoSelection = true;
                        }));
        }
        #endregion [NotifyJobReOnChassis]

        #region [NotifyJobReOnChassis]
        public void NotifyCheckYcDeTwin(Boolean value)
        {
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));
            InterfaceMessageLoader.instance().WriteInterfaceMessage<bool?>(System.Reflection.MethodBase.GetCurrentMethod().Name, (bool?)value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.MainView.UC_MachineSearchView.EnablePositionOnChassis(value);
                            this.HideProgressBar(0);
                        }));
        }
        #endregion [NotifyJobReOnChassis]        

        #region [NotifyValidate4LoadingSwapping]
        public void NotifyValidate4LoadingSwapping(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            {
                                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            }
                            else
                                VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, true);

                            if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;

                            PresentationMgr.Singleton.ThreadTimerStart(true);

                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(
                                System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);

                            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0128", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.GetLoadingSwapMessage(clone.resultObj)
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);

                            // 성공/실패 여부 관계없이 변경된 정보로 표시
                            //if (clone.resultTp.Equals("SUCCESS"))
                            {
                                var jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                                PresentationMgr.AddLoadingSwapInfo(
                                    jobKey,
                                    new PresentationMgr.LoadingSwapInfo()
                                    {
                                        MachineID = PresentationMgr.MainView.UC_MachineSearchView.GetSelectedMachine(),
                                        ChssPsn = PresentationMgr.MainView.UC_MachineSearchView.ChssPsn
                                    });
                                var jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                                if (jobOrder != null && jobOrder.type != null && !string.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                                {
                                    PresentationMgr.AddLoadingSwapInfo(
                                        jobOrder.type.ycTwinKey,
                                        new PresentationMgr.LoadingSwapInfo()
                                        {
                                            MachineID = PresentationMgr.MainView.UC_MachineSearchView.GetSelectedMachine(),
                                            ChssPsn = PresentationMgr.MainView.UC_MachineSearchView.ChssPsn.Equals("F") ? "A" : "F"
                                        });
                                }

                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.RefreshTargetJobInfo();
                            }

                            clone = null;
                        }));
        }
        #endregion [NotifyValidate4LoadingSwapping]

        #region [NotifyMachineStopCodeList]
        public void NotifyMachineStopCodeList(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.m_pData.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            PresentationMgr.MainView.UC_AvailableView.MakeupAvailableItems(clone);
                            //PresentationMgr.AppWin.UC_MainView.UC_AvailableView.ProcessByMachineStopCodeList(clone); // TODO
                        }));
        }
        #endregion [NotifyMachineStopCodeList]

        #region [NotifyMachineAccessAction]
        public void NotifyMachineAccessAction(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineAccessAction_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineAccessAction_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.showSetting.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.Singleton.showSetting = clone.showSetting;
                            PresentationMgr.Singleton.showCHGLOC = clone.showCHGLOC;
                            PresentationMgr.Singleton.showViewINV = clone.showViewINV;
                            PresentationMgr.Singleton.showVBlock = false;
                            PresentationMgr.Singleton.cntrSelected = String.Empty;

                            PresentationMgr.Singleton.viewBLockList = clone.viewBLockList; //20201020
                            PresentationMgr.Singleton.callFirst1Time = false;

                            PresentationMgr.Singleton.machineLocationPrevious = null; //20201102

                            PresentationMgr.Singleton.showHideButtonsAccessAction();

                            // Jul 30 add config to check YT SwapView OK Btn
                            PresentationMgr.MainView.UC_YtSwapView.enableItvSwap = clone.enableItvSwap;
                            // Aug 25 Only check Aft / Fore / Center position selection, don't disable OK btn
                            //PresentationMgr.MainView.UC_YtSwapView.CheckOkayBtnStatus();
                        }));
        }
        #endregion [NotifyMachineAccessAction]

        #region [NotifyGetMachineStop]
        public void NotifyGetMachineStop(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value)
        {
            //LogMessage("NotifyGetMachineStop => value : " + value.ToString());

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value == null? String.Empty : value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (clone == null &&
                                    PresentationMgr.MainView.UC_BreakTimeView.Visibility == Visibility.Visible
                                    && PresentationMgr.MainView.UC_BreakTimeView.setMachineFlg == false)
                            {
                                PresentationMgr.MainView.UC_BreakTimeView.Waiting = BreakTimeView.WaitingType.UNSET;
                            }

                            if (PresentationMgr.MainView.UC_BreakTimeView.Waiting != BreakTimeView.WaitingType.NONE ||
                                    (PresentationMgr.MainView.UC_BreakTimeView.Visibility == Visibility.Hidden &&
                                    PresentationMgr.MainView.UC_AvailableView.Visibility == Visibility.Hidden)
                                )
                            {
                                if (clone != null)
                                    PresentationMgr.ApplyGetMachineStop(clone);
                                else
                                    PresentationMgr.ApplySetMachineStop(String.Empty);
                            }
                            // Oct 17 2019 Add Remark Field
                            if (clone != null && clone.remark == "PNO")
                                PresentationMgr.MainView.UC_BreakTimeView.Lbl_BayControl.Visibility = Visibility.Visible;
                            else
                                PresentationMgr.MainView.UC_BreakTimeView.Lbl_BayControl.Visibility = Visibility.Hidden;

                            if (PresentationMgr.MainView.UC_BreakTimeView.Waiting != BreakTimeView.WaitingType.NONE)
                            {
                                if (clone != null &&
                                    PresentationMgr.MainView.UC_BreakTimeView.Waiting == BreakTimeView.WaitingType.SET)
                                {
                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0038", LanguageService.LABEL_POPUP)
                                        , String.Format("{0} " + PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0129", LanguageService.LABEL_CUSTOMIZE), clone.Data.ReasonNm)
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                }
                                else if (clone == null &&
                                    PresentationMgr.MainView.UC_BreakTimeView.Waiting == BreakTimeView.WaitingType.UNSET)
                                {
                                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
                                }
                                else
                                {
                                    VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send sendValue = new VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send();
                                    sendValue.m_iBreakStatus = PresentationMgr.MainView.UC_BreakTimeView.Waiting == BreakTimeView.WaitingType.SET ? 1 : 0;
                                    sendValue.Data.ReasonCd = PresentationMgr.MainView.UC_BreakTimeView.ReasonCd;
                                    sendValue.Data.ReasonNm = PresentationMgr.MainView.UC_BreakTimeView.ReasonNm;
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "SetMachineStop_Ask"), sendValue);

                                    VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineStop_Ask(ref sendValue);
                                    if (PresentationMgr.MainView.UC_BreakTimeView.Waiting != BreakTimeView.WaitingType.SET)
                                    {
                                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
                                        PresentationMgr.MainView.UC_BreakTimeView.Btn_Cancel.IsEnabled = true;
                                        PresentationMgr.MainView.UC_BreakTimeView.Bd_Cancel.Visibility = Visibility.Hidden;
                                    }
                                    else
                                    {
                                        PresentationMgr.AppWin.MainWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Hidden;
                                        PresentationMgr.MainView.UC_BreakTimeView.Btn_Cancel.IsEnabled = false;
                                        PresentationMgr.MainView.UC_BreakTimeView.Bd_Cancel.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                            PresentationMgr.MainView.UC_BreakTimeView.Waiting = BreakTimeView.WaitingType.NONE;
                        }));
        }
        #endregion [NotifyGetMachineStop]

        #region [NotifyGetInventoryList]
        public void NotifyGetInventoryList(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            //System.Diagnostics.Trace.WriteLine("[Timestamp] NotifyGetInventoryList : " + DateTime.Now.ToString("[HH:mm:ss:fff]"));

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (PresentationMgr.MainView.UC_VBlockView.Visibility == System.Windows.Visibility.Visible)
                            {
                                this.HideProgressBar(0);
                                PresentationMgr.MainView.UC_VBlockView.GetInventoryListByVirtualBlock_New_CallBack(clone);
                            }
                            else
                            {
                                switch (clone.enPurposeType)
                                {
                                    case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND:
                                        PresentationMgr.Singleton.SetInventoryData(clone);
                                        break;
                                    case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND_Ex:
                                        PresentationMgr.Singleton.SetInventoryDataEx(clone, true, true);
                                        break;
                                    case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND:
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                        PresentationMgr.Singleton.SetInventoryData(clone);
                                        this.HideProgressBar(0);
                                        break;
                                    case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND_Ex:
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                        PresentationMgr.Singleton.SetInventoryDataEx(clone, true, false);
                                        break;
                                    case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA:
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                        PresentationMgr.Singleton.SetInventoryData(clone, false);
                                        this.HideProgressBar(0);
                                        PresentationMgr.Singleton.Siemens_JobSetForRH();
                                        break;
                                    case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA_Ex:
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                        PresentationMgr.Singleton.SetInventoryDataEx(clone, false);
                                        PresentationMgr.Singleton.Siemens_GetCurrentInventoryData();
                                        break;
                                }
                            }

                            clone = null;
                        }));

        }
        #endregion [NotifyGetInventoryList]

        #region [NotifyGetInventoryList1]
        public void NotifyGetInventoryList1(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (value != null)
                            {
                                List<VD_Common_Def_Inventory> lstCntr = new List<VD_Common_Def_Inventory>();
                                foreach (var blockInfo in value.DicBlock)
                                {
                                    foreach (var bayInfo in blockInfo.Value.DicBay)
                                    {
                                        foreach (var cntrInfo in bayInfo.Value.invenList)
                                        {
                                            lstCntr.Add(cntrInfo);
                                        }
                                    }

                                }
                                PresentationMgr.GetPLCJobListEmptyContainer(lstCntr);
                                this.HideProgressBar(0);
                            }

                            clone = null;
                        }));

        }
        #endregion [NotifyGetInventoryList1]

        #region [NotifyGetInventoryList2]
        public void NotifyGetInventoryList2(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            //System.Diagnostics.Trace.WriteLine("[Timestamp] NotifyGetInventoryList : " + DateTime.Now.ToString("[HH:mm:ss:fff]"));

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            switch (clone.enPurposeType)
                            {
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND:
                                    PresentationMgr.Singleton.SetInventoryDataBay2(clone);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND_Ex:
                                    PresentationMgr.Singleton.SetInventoryDataEx(clone, true, true);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataBay2(clone);
                                    this.HideProgressBar(0);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND_Ex:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataEx(clone, true, false);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataBay2(clone, false);
                                    this.HideProgressBar(0);
                                    PresentationMgr.Singleton.Siemens_JobSetForRH();
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA_Ex:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataEx(clone, false);
                                    PresentationMgr.Singleton.Siemens_GetCurrentInventoryData();
                                    break;
                            }

                            clone = null;
                        }));

        }
        #endregion [NotifyGetInventoryList2]

        #region [NotifyGetInventorySwapList]
        public void NotifyGetInventorySwapList(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            //System.Diagnostics.Trace.WriteLine("[Timestamp] NotifyGetInventoryList : " + DateTime.Now.ToString("[HH:mm:ss:fff]"));

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            switch (clone.enPurposeType)
                            {
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND:
                                    PresentationMgr.Singleton.SetInventoryDataSwap(clone);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND_Ex:
                                    PresentationMgr.Singleton.SetInventoryDataEx(clone, true, true);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataSwap(clone);
                                    this.HideProgressBar(0);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND_Ex:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataEx(clone, true, false);
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataSwap(clone, false);
                                    this.HideProgressBar(0);
                                    PresentationMgr.Singleton.Siemens_JobSetForRH();
                                    break;
                                case RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA_Ex:
                                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                                    PresentationMgr.Singleton.SetInventoryDataEx(clone, false);
                                    PresentationMgr.Singleton.Siemens_GetCurrentInventoryData();
                                    break;
                            }

                            clone = null;
                        }));

        }
        #endregion [NotifyGetInventoryList]

        #region [NotifySetMachineStop]
        public void NotifySetMachineStop(String value)
        {
            //LogMessage("NotifySetMachineStop => value : " + value.ToString());

            //VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive>(value);
            String clone = Util.DeepCopy<String>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.ApplySetMachineStop(clone);
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStop_Ask();
                            PresentationMgr.MainView.UC_BreakTimeView.setMachineFlg = false;
                        }));
        }
        #endregion [NotifySetMachineStop]

        #region [NotifyErrorCode]
        public void NotifyErrorCode(String value)
        {
            //LogMessage("NotifyErrorCode => value : " + value.ToString());

            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (value.Length > 0)
                            {
                                String[] errString = value.Split('@');
                                if (!String.IsNullOrEmpty(errString[0]))
                                {
                                    this.HideProgressBar(0);
                                    this.UC_LogInView.userAccessRole = false;
                                    this.UC_LogInView.ReFlash();
                                    String mess = PresentationMgr.Singleton.LanguageSer.GetResourceRMG(errString[0], LanguageService.MESSAGE_LOGIN_GROUP);
                                    if (errString.Count() > 1 && errString[1].Length > 0)
                                        mess = mess + " (" + errString[1] + ")";
                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0130", LanguageService.LABEL_CUSTOMIZE), mess
                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                }
                            }
                        }));
        }
        #endregion [NotifyErrorCode]

        #region [NotifyBlockList]
        public void NotifyBlockList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value == null ? "null" : value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            DataMgr.Singleton.AddBlockBayInfo(clone);

                            //20201020 keep blockbayinfo data
                            if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList && DataMgr.Singleton._SimpleBlockBayInfoKeep == null && DataMgr.Singleton._SimpleBlockBayInfo.Count > 0)
                            {
                                DataMgr.Singleton._SimpleBlockBayInfoKeep = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(DataMgr.Singleton._SimpleBlockBayInfo);
                            }

                            PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                            PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                            if (PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.blockView)
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.blockView = false;
                                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BlockSelectionView);
                                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                                {
                                    PresentationMgr.MainView.UC_BlockSelectionView.SelectedBlockName = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    if(PresentationMgr.MainView.UC_ContainerArea.Visibility != Visibility.Visible)
                                        PresentationMgr.MainView.UC_BlockSelectionView.Wrap_BlockSelectionView_MouseLeftButtonUp(null, null);
                                    PresentationMgr.MainView.UC_BaySelectionView.SelectedBlockName = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    PresentationMgr.MainView.UC_BlockSelectionView.MakeupBlockItemsWithTabSelection();
                                }
                                else
                                {
                                    PresentationMgr.MainView.UC_SwapView.UC_BlockSelectionView.SelectedBlockName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    PresentationMgr.MainView.UC_SwapView.UC_BaySelectionView.SelectedBlockName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    PresentationMgr.MainView.UC_SwapView.UC_BlockSelectionView.MakeupBlockItemsWithTabSelection();
                                }
                            }

                            if (PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode == 1)
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode = 0;
                                PresentationMgr.MainView.UC_VBlockView.AddBlockBayVrtlInfoCallBack(clone);
                            }
                            else if (PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode == 2)
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode = 0;
                                PresentationMgr.MainView.UC_VBlockChangeView.AddBlockBayVrtlInfoCallBack(clone);
                            }

                            if (PresentationMgr.MainView.UC_VBlockView.Visibility == Visibility.Visible || PresentationMgr.MainView.UC_VBlockChangeView.Visibility == Visibility.Visible)
                                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(PresentationMgr.Singleton.CurrentBlock);

                            clone = null;

                            //20201020
                            if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList)
                            {
                                if (!PresentationMgr.Singleton.CurrentBlock.Equals(String.Empty) && !PresentationMgr.Singleton.callFirst1Time)
                                {
                                    PresentationMgr.Singleton.callFirst1Time = true;
                                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, PresentationMgr.Singleton.CurrentBlock);
                                }
                            }
                        }));
        }
        #endregion [NotifyBlockList]

        #region [NotifyBlockListForBlockMap]
        public void NotifyBlockListForBlockMap(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList)
            {
                if (DataMgr.Singleton._SimpleBlockBayInfoKeep != null)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive valueFromRoot = new VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive();
                    foreach (var info in value.DicBlock)
                    {
                        valueFromRoot.DicBlock.Add(info.Key, DataMgr.Singleton._SimpleBlockBayInfoKeep.DicBlock[info.Key]);
                    }
                    value = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(valueFromRoot);

                    if (value.Count < 1 && DataMgr.Singleton._SimpleBlockBayInfoKeep.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock))
                    {
                        value.DicBlock.Add(PresentationMgr.Singleton.CurrentBlock, DataMgr.Singleton._SimpleBlockBayInfoKeep.DicBlock[PresentationMgr.Singleton.CurrentBlock]);
                    }
                }
            }

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);

            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value == null ? "null" : value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            //20201020 reset blockbayinfo when call api
                            if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList)
                            {
                                DataMgr.Singleton._SimpleBlockBayInfo = new VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive();
                            }

                            DataMgr.Singleton.AddBlockBayInfo(clone);
                            PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                            PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                            if (PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.blockView)
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.blockView = false;
                                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BlockSelectionView);
                                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                                {
                                    PresentationMgr.MainView.UC_BlockSelectionView.SelectedBlockName = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    if (PresentationMgr.MainView.UC_ContainerArea.Visibility != Visibility.Visible)
                                        PresentationMgr.MainView.UC_BlockSelectionView.Wrap_BlockSelectionView_MouseLeftButtonUp(null, null);
                                    PresentationMgr.MainView.UC_BaySelectionView.SelectedBlockName = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    PresentationMgr.MainView.UC_BlockSelectionView.MakeupBlockItemsWithTabSelection();
                                }
                                else
                                {
                                    PresentationMgr.MainView.UC_SwapView.UC_BlockSelectionView.SelectedBlockName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    PresentationMgr.MainView.UC_SwapView.UC_BaySelectionView.SelectedBlockName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                                    PresentationMgr.MainView.UC_SwapView.UC_BlockSelectionView.MakeupBlockItemsWithTabSelection();
                                }
                            }

                            if (PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode == 1)
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode = 0;
                                PresentationMgr.MainView.UC_VBlockView.AddBlockBayVrtlInfoCallBack(clone);
                            }
                            else if (PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode == 2)
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.virtualBlockMode = 0;
                                PresentationMgr.MainView.UC_VBlockChangeView.AddBlockBayVrtlInfoCallBack(clone);
                            }

                            if ((PresentationMgr.MainView.UC_VBlockView.Visibility == Visibility.Visible || PresentationMgr.MainView.UC_VBlockChangeView.Visibility == Visibility.Visible)
                            || (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList))
                            {
                                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(PresentationMgr.Singleton.CurrentBlock);
                            }
                                                            
                            clone = null; 
                        }));
        }
        #endregion [NotifyBlockListForBlockMap]

        #region [NotifyGetVmtAutoStartConfig]
        public void NotifyGetVmtAutoStartConfig(string retStr)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            String RegAddr = @"Software\Microsoft\Windows\CurrentVersion\Run";
                            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
                            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, RegAddr);
                            if (uIntPtr.ToUInt32() > 0)
                            {
                                String regKey = KeyCLTVMT_RMG.Substring(KeyCLTVMT_RMG.LastIndexOf('\\') + 1);
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
                            //    String regKey = KeyCLTVMT_RMG.Substring(KeyCLTVMT_RMG.LastIndexOf('\\') + 1); // productName
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

        #region [NotifyBlockMapList]
        public void NotifyBlockMapList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.AddBlockBayInfo(clone);

                            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            {
                                this.HideProgressBar(0);

                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;

                                if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) &&
                                    clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay != null)//.Count > 0)
                                {
                                    //if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual != true)
                                    //{
                                    //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                    //    , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock);
                                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                                    //}

                                    if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
                                    {
                                        if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                                            PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, String.Empty);
                                        else
                                        {
                                            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                                            {
                                                m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                                                m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                                                m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                                                m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                                            };
                                            var bayList = clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay.Values.ToList();
                                            if (bayList.Count > 0)
                                            {
                                                bayList.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
                                                pos.m_cBay = bayList.First().BayName;
                                                PresentationMgr.Singleton.CurrentPostion = pos;
                                            }
                                            else
                                            {
                                                PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, String.Empty);
                                            }                                        
                                        }
                                    }
                                    else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                                    {
                                        //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        //, "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock + "" + PresentationMgr.Singleton.CurrentBay);
                                        //VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);

                                        PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
                                    }

                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.CheckBlockLocation();
                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.CheckBayLocation();
                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                                    if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0)  PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                                    PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;
                                }
                                else if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) &&
                                    clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay == null)//.Count = 0)
                                {
                                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                                    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                                    if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Visible;
                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = false;
                                    PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, string.Empty);
                                }

                                if (PresentationMgr.AppWin.UC_PopupOutView.Visibility == Visibility)
                                {
                                    PresentationMgr.AppWin.UC_PopupOutView.CheckBlockLocation();
                                    PresentationMgr.AppWin.UC_PopupOutView.CheckBayLocation();
                                }

                                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                                {
                                    PresentationMgr.MainView.UC_BaySelectionView.MakeupBayItems();
                                    PresentationMgr.MainView.UC_BlockSelectionView.MakeupBayItems();
                                }
                            }

                            clone = null;
                        }));
        }
        #endregion [NotifyBlockMapList]

        #region [NotifyBlockMapList1]
        public void NotifyBlockMapList1(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.AddBlockBayInfo(clone);

                            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            {
                                this.HideProgressBar(0);

                                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_BayText.IsEnabled = true;

                                if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock1) &&
                                    clone.DicBlock[PresentationMgr.Singleton.CurrentBlock1].DicBay != null)//.Count > 0)
                                {
                                    if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock1].IsVirtual != true)
                                    {
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock1);
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea1_Ask(PresentationMgr.Singleton.CurrentBlock1);
                                    }

                                    if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay1))
                                    {
                                        if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock1].IsVirtual)
                                            PresentationMgr.Singleton.SendGetInventory1Ask(PresentationMgr.Singleton.CurrentBlock1, String.Empty);
                                        else
                                        {
                                            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                                            {
                                                m_cBlock = PresentationMgr.Singleton.MovingPosition1.m_cBlock,
                                                m_cBay = PresentationMgr.Singleton.MovingPosition1.m_cBay,
                                                m_cRow = PresentationMgr.Singleton.MovingPosition1.m_cRow,
                                                m_cTier = PresentationMgr.Singleton.MovingPosition1.m_cTier
                                            };
                                            var bayList = clone.DicBlock[PresentationMgr.Singleton.CurrentBlock1].DicBay.Values.ToList();
                                            bayList.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
                                            pos.m_cBay = bayList.First().BayName;
                                            PresentationMgr.Singleton.MovingPosition1 = pos;
                                        }
                                    }
                                    else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock1].IsVirtual)
                                    {
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock1 + "" + PresentationMgr.Singleton.CurrentBay1);
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier1_Ask(PresentationMgr.Singleton.CurrentBlock1, PresentationMgr.Singleton.CurrentBay1, PresentationMgr.Singleton.CurrentBay1);

                                        PresentationMgr.Singleton.SendGetInventory1Ask(PresentationMgr.Singleton.CurrentBlock1, PresentationMgr.Singleton.CurrentBay1);
                                    }

                                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.CheckBlockLocation();
                                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.CheckBayLocation();
                                }

                                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock1))
                                    PresentationMgr.MainView.UC_BaySelectionView1.MakeupBayItems();
                            }

                            clone = null;
                        }));
        }
        #endregion [NotifyBlockMapList1]

        #region [NotifyBlockMapList2]
        public void NotifyBlockMapList2(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.AddBlockBayInfo(clone);

                            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            {
                                this.HideProgressBar(0);

                                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_BayText.IsEnabled = true;

                                if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock2) &&
                                    clone.DicBlock[PresentationMgr.Singleton.CurrentBlock2].DicBay != null)//.Count > 0)
                                {
                                    if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock2].IsVirtual != true)
                                    {
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock2);
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea2_Ask(PresentationMgr.Singleton.CurrentBlock2);
                                    }

                                    if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay2))
                                    {
                                        if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock2].IsVirtual)
                                            PresentationMgr.Singleton.SendGetInventory2Ask(PresentationMgr.Singleton.CurrentBlock2, String.Empty);
                                        else
                                        {
                                            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                                            {
                                                m_cBlock = PresentationMgr.Singleton.MovingPosition2.m_cBlock,
                                                m_cBay = PresentationMgr.Singleton.MovingPosition2.m_cBay,
                                                m_cRow = PresentationMgr.Singleton.MovingPosition2.m_cRow,
                                                m_cTier = PresentationMgr.Singleton.MovingPosition2.m_cTier
                                            };
                                            var bayList = clone.DicBlock[PresentationMgr.Singleton.CurrentBlock2].DicBay.Values.ToList();
                                            bayList.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
                                            pos.m_cBay = bayList.First().BayName;
                                            PresentationMgr.Singleton.MovingPosition2 = pos;
                                        }
                                    }
                                    else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock2].IsVirtual)
                                    {
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock2 + "" + PresentationMgr.Singleton.CurrentBay2);
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier2_Ask(PresentationMgr.Singleton.CurrentBlock2, PresentationMgr.Singleton.CurrentBay2, PresentationMgr.Singleton.CurrentBay2);

                                        PresentationMgr.Singleton.SendGetInventory2Ask(PresentationMgr.Singleton.CurrentBlock2, PresentationMgr.Singleton.CurrentBay2);
                                    }

                                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.CheckBlockLocation();
                                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.CheckBayLocation();
                                }

                                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                                    PresentationMgr.MainView.UC_BaySelectionView2.MakeupBayItems();
                            }

                            clone = null;
                        }));
        }
        #endregion [NotifyBlockMapList2]

        #region [NotifyBlockMapSwapList]
        public void NotifyBlockMapSwapList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.AddBlockBayInfo(clone);

                            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            {
                                this.HideProgressBar(0);

                                PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;

                                if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) &&
                                    clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay != null)//.Count > 0)
                                {
                                    //if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual != true)
                                    //{
                                    //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                    //    , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock);
                                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                                    //}

                                    if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
                                    {
                                        if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                                            PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, String.Empty);
                                        else
                                        {
                                            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                                            {
                                                m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                                                m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                                                m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                                                m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                                            };
                                            var bayList = clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay.Values.ToList();
                                            bayList.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
                                            pos.m_cBay = bayList.First().BayName;
                                            PresentationMgr.Singleton.CurrentPostion = pos;
                                        }
                                    }
                                    else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                                    {
                                        //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        //, "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock + "" + PresentationMgr.Singleton.CurrentBay);
                                        //VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);

                                        PresentationMgr.Singleton.SendGetInventorySwapAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
                                    }

                                    PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.CheckBlockLocation();
                                    PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.CheckBayLocation();
                                }

                                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                                    PresentationMgr.MainView.UC_SwapView.UC_BaySelectionView.MakeupBayItems();
                            }
                            PresentationMgr.MainView.UC_SwapView.UC_JobListSwap.ListBoxSeletion();

                            clone = null;
                        }));
        }
        #endregion [NotifyBlockMapSwapList]

        #region [NotifyGetMahchineList]
        public void NotifyGetMahchineList(ref RMG.VD_RMG_PartnerMachineList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_PartnerMachineList>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            DataMgr.Singleton.List_MachineofPool = clone;
                            PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();

                            if (PresentationMgr.MainView.UC_OtherMachineJobListView.IsVisible)
                                PresentationMgr.MainView.UC_OtherMachineJobListView.SetMachineList();

                            //PresentationMgr.MainView.UC_YtSwapView.SetMachineList();
                            this.HideProgressBar(0);
                            if (PresentationMgr.MainView.UC_YtSwapView.IsVisible)
                            {
                                PresentationMgr.MainView.UC_YtSwapView.KeypadDone();
                            }
                        }));
        }
        #endregion [NotifyGetMahchineList]

        #region [NotifyGetMahchineListofPool]
        public void NotifyGetMahchineListofPool(ref RMG.VD_RMG_PartnerMachineList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_PartnerMachineList>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.List_MachineofPool = clone;
                            PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
                            this.HideProgressBar(0);
                        }));
        }
        #endregion [NotifyGetMahchineListofPool]

        #region [NotifyMahchineLogoutCheck]
        public void NotifyMahchineLogoutCheck(RMG.VD_RMG_MachineList_Receive value)
        {
            if ("CLT".Equals(UserInfo.gUserID) && "ACCESS".Equals(UserInfo.gUserPW))
                return;
            foreach (var machine in value.MachineList)
            {
                if (RMG.RMG_User.gMchnID.Equals(machine.mchnId) && RMG.RMG_User.gMchnTp.Equals(machine.mchnTp))
                {
                    this.autoFlg = machine.autoFlg;
                    setCurrentPositionByFlag();
                    if (machine.loginUsrLst != null &&
                        !machine.loginUsrLst.Contains(RMG.RMG_User.gUserID) &&
                        //if (machine.isLogOn == false &&
                        (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.LogInView &&
                        PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MachineSettingView))
                    {
                        //Logger.Log("Received Force Logout : " +
                        //    "mchnId =" + machine.mchnId + ", mchnTp=" + machine.mchnTp +
                        //    ", isLogOn=" + machine.isLogOn.ToString() + ", isOn=" + machine.isOn.ToString() +
                        //    ", mchnSts=" + machine.mchnSts + ", vrtlFlg=" + machine.vrtlFlg);

                        //InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_MachineList_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, value);

                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate
                            {
                                //PresentationMgr.AppWin.UC_LogInView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                                //VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                                //VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);

                                //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Logout, "Log Off", "Force Logoff been processed.", "OK", null, 3);
                            }
                                ));
                    }
                    else if (!String.IsNullOrEmpty(machine.noticeMsg))
                    {
                        //Logger.Log("Received Notice Message : " + machine.noticeMsg);
                        //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, machine.noticeMsg);

                        //VMT_DataMgr_Common.SetMachineNotice();

                        //TimeSpan timeout = new TimeSpan(0, 0, 3); // 3 sec
                        //this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, timeout,
                        //            new Action(delegate
                        //            {
                        //                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Message", machine.noticeMsg, "OK", null, 0);
                        //            }));
                    }

                    break;
                }
            }
        }
        #endregion [NotifyMahchineLogoutCheck]

        #region [NotifyGetJobOrderByContainer]
        public void NotifyGetJobOrderByContainer(ref RMG.VD_RMG_JobOrderList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_JobOrderList>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            if (PresentationMgr.Singleton.showViewINV)
                            {
                                PresentationMgr.MainView.UC_ContainerSearchView.SetSearchedJobList(clone);
                            }
                            else // Filtered searchList by location
                            {
                                RMG.VD_RMG_JobOrderList locFilterJobList = new RMG.VD_RMG_JobOrderList();
                                foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in clone.JobOrder)
                                {
                                    if (jobOrder.locFrom.blck == PresentationMgr.Singleton.CurrentBlock || jobOrder.locWorking.blck == PresentationMgr.Singleton.CurrentBlock)
                                    {
                                        locFilterJobList.JobOrder.Add(jobOrder);
                                    }
                                }
                                PresentationMgr.MainView.UC_ContainerSearchView.SetSearchedJobList(locFilterJobList);
                            }                               
                            this.HideProgressBar(0);
                        }));
        }
        #endregion [NotifyGetJobOrderByContainer]

        #region [NotifySetDetwinJob]
        public void NotifySetDetwinJob(Boolean value)
        {
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));
            InterfaceMessageLoader.instance().WriteInterfaceMessage<bool?>(System.Reflection.MethodBase.GetCurrentMethod().Name, (bool?)value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);

                            //VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();
                            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            {
                                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            }
                            else
                                VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, true);

                            this.HideProgressBar(0);
                            if (!value)
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "SetDetwinJob", "Detwin Failed!", "OK", null, 0);
                            }
                        }));
        }
        #endregion [NotifySetDetwinJob]

        #region [NotifySetJobStatus]
        public void NotifySetJobStatus(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobSet_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobSet_Receive>(value);
            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //PresentationMgr.MainView.SaveLog("NotifySetJobStatus");
                            //VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();
                            //if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            //{
                            //    VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                            //    PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                            //    System.Threading.Thread.Sleep(500);
                            //    VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                            //    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            //    //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                            //    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            //}
                            //else
                            //{
                            //}
                            System.Threading.Thread.Sleep(1000);
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                            Thread thread = new Thread(RestartPolling);
                            thread.Start();
                            PresentationMgr.MainView.Btn_JobSet.IsEnabled = true;
                            if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;
                            //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                            PresentationMgr.Singleton.ThreadTimerStart(true);

                            this.HideProgressBar(0);

                            if (clone.resultTP.Equals("FAIL"))
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.GetJobSetErrorMessage(clone.resultObj)
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                // Aug 18 Clear ListJobKeyObjToCheckAfterJobSet when JobSetFail to avoid changing sort
                                // Aug 19 Rollback this logic
                                //PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Clear();
                            }
                            else //if (clone.resultTp.Equals("SUCCESS"))  // AH 생성, processing/inactive 상태변경시 최상단 job 선택
                            {
                                // processing을 해제한 경우만 적용될 것으로 가정
                                var jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;

                                PresentationMgr.RemoveLoadingSwapInfo(jobKey);

                                var jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                                if (jobOrder != null && jobOrder.type != null && !string.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                                    PresentationMgr.RemoveLoadingSwapInfo(jobOrder.type.ycTwinKey);

                                //PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
                                PresentationMgr.Singleton.NeedJobAutoSelection = true;
                                PresentationMgr.Singleton.NeedMoveToTargetJobPage = true;
                                PresentationMgr.Singleton.JobSet = true;
                                // Aug 19 Rollback this logic
                                //if (PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Find(x => x.type == PresentationMgr.JobSetCheckType.GeneratedType) == null) // Not contains generated AH Job
                                PresentationMgr.Singleton.CheckAHJobAfterJobSetCount = 2;
                            }
                            //MainView.stopLoad = false;
                            clone = null;
                        }));
        }
        void wait(double x)
        {
            DateTime t = DateTime.Now;
            DateTime tf = DateTime.Now.AddSeconds(x);

            while (t < tf)
            {
                t = DateTime.Now;
            }
        }

        private void RestartPolling()
        {
            Thread.Sleep(100);
            wait(2);
            VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
        }
        #endregion [NotifySetJobStatus]

        #region [NotifyGetContainerInfo]
        public void NotifyGetContainerInfo(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_ContainerInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_ContainerInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.MainView.UC_ContainerDetailView.SetTargetJobContainerInfo(clone);
                            PresentationMgr.AppWin.HideProgressBar(0);
                        }));
        }
        #endregion [NotifyGetContainerInfo]

        #region [NotifyGetTwinContainerInfo]
        public void NotifyGetTwinContainerInfo(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_ContainerInfo_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_ContainerInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.MainView.UC_ContainerDetailView.SetTwinJobContainerInfo(clone);
                        }));
        }
        #endregion [NotifyGetTwinContainerInfo]

        #region [NotifyDoSwap4Manual]
        public void NotifyDoSwap4Manual(String value)
        {
            var clone = Util.DeepCopy<String>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            if (!String.IsNullOrEmpty(value))  //SWAP FAIL
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Swap failed", value, "OK", null, 0);
                                if (wrkCd == "2")
                                {
                                    wrkCd = "";
                                    plcCnt = 31;
                                }
                            }
                            else
                            {
                                if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerSearch)
                                    PresentationMgr.MainView.UC_ContainerSearchView.RefreshSearchResult();

                                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                                if (target != null && target.type != null)
                                {
                                    //string newYtNo = !PresentationMgr.MainView.UC_YtSwapView.Tb_Change.Text.Trim().Equals("") ? "T" + PresentationMgr.MainView.UC_YtSwapView.Tb_Change.Text
                                    //                                                                                            : target.partnerMchn.mchnId; 
                                    string newYtNo = !PresentationMgr.MainView.UC_YtSwapView.Tb_Change.Text.Trim().Equals("") ? PresentationMgr.MainView.UC_YtSwapView.Tb_Change.Text
                                                                                                                                : target.partnerMchn.mchnId; //20201005 remove + "T"; maxlength = 8, upper, char + num

                                    if (target != null && target.cntr != null
                                    && !lstLDMOJobYTAssigned.Contains(target.cntr.cntrNo)
                                    && ("LD".Equals(target.type.jobTp) || "MO".Equals(target.type.jobTp)))
                                    {
                                        lstLDMOJobYTAssigned.Add(target.cntr.cntrNo);
                                        lstYTAssigned.Add(newYtNo);
                                    }
                                    else if (lstLDMOJobYTAssigned.Contains(target.cntr.cntrNo))
                                    {
                                        lstYTAssigned[lstLDMOJobYTAssigned.IndexOf(target.cntr.cntrNo)] = newYtNo;
                                    }

                                    if (PresentationMgr.MainView.jobDone)
                                    {
                                        PresentationMgr.MainView.jobDone = false;
                                        if(wrkCd != "2")
                                            PresentationMgr.Singleton.SendJobDoneAsk(target, newYtNo, PresentationMgr.MainView.UC_YtSwapView.ChssPsn);
                                    }
                                }

                                if (wrkCd == "2")
                                {
                                    VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
                                }

                                if (!PresentationMgr.MainView.jobDone && wrkCd != "2")
                                {
                                    needToCheckYtNo = true;
                                    ytNoCheckCount = 0;
                                    System.Threading.Thread.Sleep(500);

                                    PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);

                                    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                }
                            }

                        }));
        }
        #endregion [NotifyDoSwap4Manual]

        #region [NotifyGetMaxRow]
        public void NotifyGetMaxRow(String value)
        {
            var maxRow = Util.DeepCopy<String>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, maxRow);
                            PresentationMgr.MainView.UC_NavigatorView.ChangeMaxColDefCount(maxRow);
                        }));
        }
        #endregion [NotifyGetMaxRow]

        #region [NotifyGetNoWorkArea]
        public void NotifyGetNoWorkArea(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_NoWorkArea_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.SetNoWorkArea(clone);
                        }));
        }
        #endregion [NotifyGetNoWorkArea]

        #region [NotifyGetNoWorkArea1]
        public void NotifyGetNoWorkArea1(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_NoWorkArea_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.SetNoWorkArea1(clone);
                        }));
        }
        #endregion [NotifyGetNoWorkArea1]

        #region [NotifyGetNoWorkArea2]
        public void NotifyGetNoWorkArea2(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_NoWorkArea_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.SetNoWorkArea2(clone);
                        }));
        }
        #endregion [NotifyGetNoWorkArea2]

        #region [NotifyGetNoWorkTier]
        public void NotifyGetNoWorkTier(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_NoWorkArea_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.SetNoWorkTier(clone);
                        }));
        }
        #endregion [NotifyGetNoWorkTier]    

        #region [NotifyGetNoWorkTier1]
        public void NotifyGetNoWorkTier1(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_NoWorkArea_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.SetNoWorkTier1(clone);
                        }));
        }
        #endregion [NotifyGetNoWorkTier1]

        #region [NotifyGetNoWorkTier2]
        public void NotifyGetNoWorkTier2(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_NoWorkArea_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.Singleton.SetNoWorkTier2(clone);
                        }));
        }
        #endregion [NotifyGetNoWorkTier2]

        #region [NotifyChangePosition]
        public void NotifyChangePosition(Object value)
        {
        }
        #endregion [NotifyChangePosition]

        #region [NotifyException]
        public void NotifyException(String value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.HideProgressBar(0);
                            if (value.Contains("GetInventoryListBackgroundMulti_New") || value.Contains("GetInventoryListMulti_New"))
                            {
                                PresentationMgr.Singleton.ThreadTimerStart(true); // exception => re-call getInventory data
                            }
                           
                            //if (wrkCd == "1" || wrkCd == "2")
                            wrkCd = "";
                            plcCnt = 31;

                            //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Hessian Exception", value, "OK", null, 0);
                        }));
        }
        #endregion [NotifyException]

        #region [NotifyConfig]
        public void NotifyConfig(ref VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive value)
        {
            PresentationMgr.ShowUnplugReeferOnly = value.ShowUnplugReeferOnly;
            value = null;
        }
        #endregion [NotifyConfig]

        #region [NotifyCheckPLCData]
        public void NotifyCheckPLCData(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain value)
        {
            //TEST DATA
            //value = new RMG.VD_RMG_VmtDomain();
            //value.cntrNo = "CAIU8489190";
            //value.containerWeight = "11100";
            //value.currentBay = "12";
            //value.currentBlock = "6A";
            //value.currentCell = "D";
            //value.currentRow = "D";
            //value.currentTier = "2";
            //value.jbFlg = "Y";
            //value.mchnId = "A51";
            //value.wrkCd = "1";
            if (plcReceiveCnt == 10000)
                plcReceiveCnt = 0;
            plcReceiveCnt++;
            Console.WriteLine("receivePLCCnt: " + plcReceiveCnt.ToString());

            plcPollingCntTimer = plcReceiveCnt;

            if (wrkCd != "" || value == null
                || (value != null && //CHECK NEW PLC DATA DUPLICATE
                        value.cntrNo == PresentationMgr.MainView.plcdomain.cntrNo
                        && value.currentBlock == PresentationMgr.MainView.plcdomain.currentBlock
                        && value.currentBay == PresentationMgr.MainView.plcdomain.currentBay
                        && value.currentCell == PresentationMgr.MainView.plcdomain.currentCell
                        && value.currentTier == PresentationMgr.MainView.plcdomain.currentTier
                        && value.wrkCd == PresentationMgr.MainView.plcdomain.wrkCd
                        && value.msgSeq == PresentationMgr.MainView.plcdomain.msgSeq
                    ) 
                )
                return;
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain>(value);

            //SET DATA PLCDOMAIN
            PresentationMgr.MainView.plcdomain = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain>(clone);

            var workCd = clone.wrkCd;
            if (workCd == "1" || workCd == "2")
            {
                wrkCdPLC = workCd;
            }

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (PresentationMgr.MainView.UC_YtSwapView.Visibility == Visibility.Visible && clone.wrkCd == "2")
                            {
                                PresentationMgr.MainView.UC_YtSwapView.Btn_Cancel_Click(null, null);
                            }

                            var jobSelected = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);

                            String mess = String.Empty;

                            switch (workCd)
                            {
                                case "1":
                                    if (!String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo))
                                    {
                                        mess = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0055", LanguageService.MESSAGE_GROUP), PresentationMgr.MainView.plcDomainTwistLock.cntrNo); // The crane already has processing job. Please finish this by manual
                                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), mess
                                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), new UC_PopupView.Callback_Popup(CallbackProcessPLCPopup), 0);
                                    }
                                    else
                                    {
                                        wrkCd = "1";
                                        plcCnt = 0;

                                        if ( !String.IsNullOrEmpty(clone.cntrNo) )
                                            //(jobSelected != null &&
                                            //                ((jobSelected.locWorking.blck == clone.currentBlock && jobSelected.locWorking.bay == clone.currentBay)
                                            //                || (jobSelected.locFrom.blck == clone.currentBlock && jobSelected.locFrom.bay == clone.currentBay))
                                            //            )
                                        {
                                            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> lstJob = DataMgr.Singleton.List_JobOrder.FindAll(x => x.cntr.cntrNo == clone.cntrNo);
                                            if (lstJob.Count >= 2)
                                            {
                                                PresentationMgr.MainView.UC_MultiJobPopupView.Visibility = Visibility.Visible;
                                                PresentationMgr.MainView.UC_MultiJobPopupView.listJob = lstJob;
                                                PresentationMgr.MainView.UC_MultiJobPopupView.SetGridData();
                                            }
                                            else
                                            {
                                                VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
                                            }
                                        }
                                        else if ("YT".Equals(clone.currentCell) || "OT".Equals(clone.currentCell))
                                        {
                                            if (jobSelected != null && jobSelected.type != null && (jobSelected.type.jobTp == "GI" || jobSelected.type.jobTp == "MI" || jobSelected.type.jobTp == "DS" || jobSelected.type.jobTp == "LC" || jobSelected.type.jobTp == "GC"))
                                            {
                                                PresentationMgr.MainView.plcdomain.cntrNo = jobSelected.cntr.cntrNo;
                                                PresentationMgr.MainView.plcdomain.rsnDesc = jobSelected.type.jobTp;
                                                VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
                                            }
                                            else
                                            {
                                                PresentationMgr.MainView.UC_ListJobPopupView.Visibility = Visibility.Visible;
                                                PresentationMgr.MainView.UC_ListJobPopupView.listContainer = PresentationMgr.GetPLCJobList(); //GI/MI/DS/LC/GC
                                                PresentationMgr.MainView.UC_ListJobPopupView.SetGridData();
                                            }
                                        }
                                        else
                                        {
                                            int row = 0;
                                            if (Int32.TryParse(clone.currentRow, out row))
                                                PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow = row;
                                            else
                                                PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow = PresentationMgr.parseCharToNum(clone.currentRow);

                                            int tier = 0;
                                            if (Int32.TryParse(clone.currentTier, out tier))
                                                PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentTier = tier;
                                            else
                                                PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentTier = PresentationMgr.parseCharToNum(clone.currentTier);

                                            //clone = new RMG.VD_RMG_VmtDomain();
                                            //clone.currentBlock = "6D";
                                            //clone.currentBay = "02";
                                            if (!String.IsNullOrEmpty(clone.currentBay) && Convert.ToInt32(clone.currentBay) % 2 == 0)
                                            {
                                                clone.currentBay = (Convert.ToInt32(clone.currentBay) - 1).ToString();
                                                if (clone.currentBay.Length == 1) clone.currentBay = "0" + clone.currentBay;
                                            }
                                            PresentationMgr.Singleton.SendGetInventory1Ask(clone.currentBlock, clone.currentBay);
                                        }
                                    }
                                    break;

                                case "2":
                                    if (String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo))
                                    {
                                        mess = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0056", LanguageService.MESSAGE_GROUP); // There is no Twistlocked container. Please finish job by manual

                                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), mess
                                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), new UC_PopupView.Callback_Popup(CallbackProcessPLCPopup), 0);
                                    }
                                    else
                                    {
                                        wrkCd = "2";
                                        plcCnt = 0;
                                        if ("YT".Equals(clone.currentCell) || "OT".Equals(clone.currentCell))
                                        {
                                            var jobOrder = PresentationMgr.Singleton.JOB_GetByCntrNo(clone.cntrNo);
                                            if (jobOrder == null)
                                            {
                                                mess = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050", LanguageService.MESSAGE_GROUP); // Error occurred during processing PLC message. Please complete job manually
                                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                                            , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), mess
                                                            , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), new UC_PopupView.Callback_Popup(CallbackProcessPLCPopup), 0);
                                                return;
                                            }

                                            var job = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(jobOrder);
                                            if (job.type.jobTp == "GO")
                                            {
                                                VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
                                            }
                                            else
                                            {
                                                PresentationMgr.ShowYtSwapPopup(job);
                                            }
                                        }
                                        else
                                        {
                                            VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
                                        }
                                    }
                                    break;

                                case "3":
                                    if (clone.mchnId != UserInfo.gMchnID)
                                    {
                                        wrkCd = "";
                                        plcCnt = 31;
                                        break;
                                    }
                                    else
                                    {
                                        wrkCd = "3";
                                        plcCnt = 0;
                                        string currBay = PresentationMgr.MainView.plcdomain.currentBay;
                                        if (currBay != "" && currBay != "00")
                                        {
                                            int bay = int.Parse(currBay);
                                            if (bay % 2 == 0) bay -= 1;
                                            currBay = (bay < 10 ? "0" : "") + bay.ToString();
                                        }

                                        VmtMachine mchn = new VmtMachine();
                                        mchn.blck = PresentationMgr.MainView.plcdomain.currentBlock;
                                        mchn.bay = PresentationMgr.MainView.plcdomain.currentBay;

                                        if (PresentationMgr.Singleton.CurrentPostion.m_cBlock.Equals(mchn.blck)
                                        && PresentationMgr.Singleton.CurrentPostion.m_cBay.Equals(currBay))
                                        {
                                            PresentationMgr.MainView.plcdomain.rsnDesc = "S";
                                        }
                                        else
                                            NotifyChangedMachineLocation(mchn);

                                        VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);

                                        break;
                                    }

                                case "4":
                                    if (clone.mchnId != UserInfo.gMchnID)
                                    {
                                        wrkCd = "";
                                        plcCnt = 31;
                                        break;
                                    }
                                    else
                                    {
                                        wrkCd = "4";
                                        plcCnt = 0;
                                        VmtMachine mchn1 = new VmtMachine();
                                        mchn1.blck = PresentationMgr.MainView.plcdomain.currentBlock;
                                        mchn1.bay = PresentationMgr.MainView.plcdomain.currentBay;
                                        mchn1.row = PresentationMgr.MainView.plcdomain.currentCell;
                                        NotifyChangedMachineLocation(mchn1);
                                        VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);

                                        break;
                                    }

                                case "XX":
                                    //mess = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050", LanguageService.MESSAGE_GROUP); // Error occurred during processing PLC message. Please complete job manually
                                    //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                    //            , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), mess
                                    //            , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), new UC_PopupView.Callback_Popup(CallbackProcessPLCPopup), 0);

                                    VMT_DataMgr_RMG.CancelPLC_Ask(PresentationMgr.MainView.plcdomain);
                                    break;
                            }
                            clone = null;
                        }));
        }

        #endregion [NotifyCheckPLCData]

        #region [NotifyCheckPLCTwistLock]
        public void NotifyCheckPLCTwistLock(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //clone = new RMG.VD_RMG_VmtDomain();
                            //clone.cntrNo = "BMOU2108444"; //UACU3835891
                            //clone.currentBlock = "6A";
                            //clone.currentBay = "2";
                            //clone.currentRow = "A";
                            //clone.currentTier = "2";

                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.MainView.plcDomainTwistLockPrv = PresentationMgr.MainView.plcDomainTwistLock;
                            PresentationMgr.MainView.plcDomainTwistLock = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain>(clone);
                            if (!String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo))
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = true;
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 1;
                            }
                            else
                            {
                                if (PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem is JobListItem)
                                {
                                    JobListItem item = PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem as JobListItem;
                                    var job = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                                    if (job != null && job.type != null && job.type.jobStatus == "P")
                                    {
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = true;
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 1;
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.jobKeyProcessing = job.jobKey;
                                    }
                                    else
                                    {
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = false;
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 0.5;
                                    }
                                }
                                else
                                {
                                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = false;
                                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 0.5;
                                }
                            }
                           
                            if (!String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo)
                                && (PresentationMgr.MainView.plcDomainTwistLock.cntrNo != PresentationMgr.MainView.plcDomainTwistLockPrv.cntrNo
                                    || PresentationMgr.MainView.plcDomainTwistLock.currentBlock != PresentationMgr.MainView.plcDomainTwistLockPrv.currentBlock
                                    || PresentationMgr.MainView.plcDomainTwistLock.currentBay != PresentationMgr.MainView.plcDomainTwistLockPrv.currentBay
                                    || PresentationMgr.MainView.plcDomainTwistLock.currentRow != PresentationMgr.MainView.plcDomainTwistLockPrv.currentRow
                                    || PresentationMgr.MainView.plcDomainTwistLock.currentTier != PresentationMgr.MainView.plcDomainTwistLockPrv.currentTier)
                                )
                            {
                                //RELOAD JOBLIST WITH NEW BLOCK/BAY IF BLOCK MODE IS SELECTED
                                String cntrNo = PresentationMgr.MainView.plcDomainTwistLock.cntrNo;
                                var jobLst = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);
                                if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.Common.BlckVal) && jobLst.Count > 0)
                                {
                                    //PresentationMgr.Singleton.NeedJobAutoSelection = true;
                                    PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal = PresentationMgr.Singleton.CurrentPostion.m_cBlock;

                                    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                }

                                PresentationMgr.Singleton.CurrentPostion.m_cBay = String.Empty;
                                PresentationMgr.Singleton.NeedMoveToTargetJobPage = true;

                                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                                {
                                    m_cBlock = clone.currentBlock,
                                    m_cBay = clone.currentBay,
                                    m_cRow = "",
                                    m_cTier = "",
                                };

                                PresentationMgr.Singleton.CurrentPostion = pos;
                                autoSelectP = true;
                            }

                            clone = null;
                        }));
        }
        public void CallbackProcessPLCPopup(UC_PopupView.UC_PopupViewRetType selected)
        {
            VMT_DataMgr_RMG.CancelPLC_Ask(PresentationMgr.MainView.plcdomain);
        }
        #endregion [NotifyCheckPLCTwistLock]

        #region [NotifyInitPLCMessage]
        public void NotifyInitPLCMessage()
        {
            VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
        }
        #endregion [NotifyInitPLCMessage]

        #region [NotifyProcessPLC]

        public void NotifyProcessPLC(RMG.VD_RMG_VmtResult value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_VmtResult>(value);
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                            {
                                string block = (!String.IsNullOrEmpty(PresentationMgr.MainView.plcdomain.currentBlock)) ? PresentationMgr.MainView.plcdomain.currentBlock : Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                                VMT_Data_JAT2.Objects.Common.BlckVal = block;
                                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                            }

                            if (wrkCd == "1" || wrkCd == "2")
                            {
                                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                endPollingPLCwrkCd12 = 1;
                            }
                        }));
            

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_VmtResult>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            if (clone.resultObj != null && clone.resultTp != null && ("S1".Equals(clone.resultObj) || clone.resultTp.Contains("S"))) //wrk = 3 & 4 will set empty in callback this process
                            {
                                String cntrNo = PresentationMgr.MainView.plcDomainTwistLock.cntrNo;
                                var jobLst = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);

                                //SET STATUS BUTTONS FOR WRK_CD 2
                                if (wrkCd == "2")
                                {
                                    if (jobLst.Count > 0)
                                    {
                                        PresentationMgr.Singleton.CorrectionSource.Clear();
                                        PresentationMgr.Singleton.NeedJobAutoSelection = true;

                                        //SET LAYOUT DEFAULT FOR YT BUTTON
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content = String.Empty;
                                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = String.Empty;

                                        if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                                        {
                                            PresentationMgr.SetSkinButton(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber,
                                                UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                                        }
                                        else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                                        {
                                            PresentationMgr.SetSkinButton(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber,
                                                UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                                        }
                                    }
                                }

                                //LOAD TWISTLOCK CONTAINER INVENTORY
                                if (wrkCd == "1" || wrkCd == "2")
                                {
                                    string currBay = PresentationMgr.MainView.plcdomain.currentBay;
                                    if (!String.IsNullOrEmpty(currBay) && currBay != "00")
                                    {
                                        int bay = int.Parse(currBay);
                                        if (bay % 2 == 0) bay -= 1;
                                        currBay = (bay < 10 ? "0" : "") + bay.ToString();
                                    }

                                    if (wrkCd == "1" && PresentationMgr.MainView.plcdomain.currentBlock.Equals(PresentationMgr.Singleton.CurrentBlock) && currBay.Equals(PresentationMgr.Singleton.CurrentBay))
                                    {
                                        PresentationMgr.MainView.plcDomainTwistLock.cntrNo = PresentationMgr.MainView.plcdomain.cntrNo;
                                        PresentationMgr.MainView.UC_BayView.SetBayItemInfo_Target(PresentationMgr.MainView.plcdomain.currentRow, Convert.ToInt32(PresentationMgr.MainView.plcdomain.currentTier));
                                    }
                                    else if (wrkCd == "2")
                                    {
                                        PresentationMgr.MainView.plcDomainTwistLock.cntrNo = String.Empty;
                                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                        new Action(delegate
                                        {
                                            System.Threading.Thread.Sleep(200);
                                            PresentationMgr.Singleton.SendGetInventoryList4Multi_Sync_Ask(PresentationMgr.Singleton.CurrentBlock, Convert.ToString(PresentationMgr.Singleton.CurrentBay));
                                        }));
                                    }
                                    else
                                    {
                                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                        new Action(delegate
                                        {
                                            System.Threading.Thread.Sleep(200);
                                            PresentationMgr.Singleton.SendGetInventoryList4Multi_Sync_Ask(PresentationMgr.Singleton.CurrentBlock, Convert.ToString(PresentationMgr.Singleton.CurrentBay));
                                        }));
                                    }
                                    
                                }

                                //LOAD JOB LIST
                                if(wrkCd == "2")
                                {
                                    if (jobLst.Count > 0)
                                    {
                                        var jobKey = jobLst.First().jobKey;
                                        foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
                                        {
                                            if (jobOrder.jobKey == jobKey)
                                            {
                                                DataMgr.Singleton.List_JobOrder.Remove(jobOrder);
                                                break;
                                            }
                                        }
                                        PresentationMgr.Singleton.JOB_Sort();
                                        PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = string.Empty;
                                        VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                                        PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
                                    }

                                }
                                else if (wrkCd == "1")
                                {
                                    String cntrNoWrkCd1 = PresentationMgr.MainView.plcdomain.cntrNo;
                                    var jobLstWrkCd1 = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNoWrkCd1);
                                    if (jobLstWrkCd1.Count > 0)
                                    {
                                        foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
                                        {
                                            if(jobOrder.jobKey == jobLstWrkCd1.First().jobKey)
                                            {
                                                jobOrder.type.jobStatus = "P";
                                                PresentationMgr.Singleton.JOB_Sort();
                                                VMT_Data_JAT2.Marshalling.Geometry.sPosition ContainerPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                                var loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location();

                                                if (jobOrder.type == null)
                                                {
                                                    loc = jobOrder.locWorking;
                                                }
                                                else
                                                {
                                                    if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "GO")
                                                        loc = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                                                    else if (jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "MI" || jobOrder.type.jobTp == "GI"
                                                        || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH" || jobOrder.type.jobTp == "LC" || jobOrder.type.jobTp == "GC")
                                                        loc = jobOrder.locWorking;
                                                }
                                                if (loc != null)
                                                {
                                                    ContainerPos.m_cBlock = loc.blck;
                                                    ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(loc.bay);
                                                    ContainerPos.m_cRow = loc.row;
                                                    ContainerPos.m_cTier = loc.tier;
                                                    PresentationMgr.Singleton.CorrectionSource.SetPos(ContainerPos);
                                                    PresentationMgr.Singleton.CorrectionSource.CntrNo = cntrNoWrkCd1;
                                                    PresentationMgr.Singleton.CorrectionSource.CntrIso = jobOrder.cntr.cntrIso;
                                                }
                                                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobOrder.jobKey;
                                                PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                        new Action(delegate
                                        {
                                            System.Threading.Thread.Sleep(1000);
                                            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                                        }));
                                }

                                //RESTART POLLING
                                if (wrkCd == "3" || wrkCd == "4")
                                {
                                    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                                }

                                wrkCd = "";
                                plcCnt = 31;
                            }
                            else
                            {
                                if (wrkCd == "2") // 20200127 message same as jobdone
                                {
                                    wrkCd = "";
                                    plcCnt = 31;
                                    if (clone.resultObj.Equals("F16"))
                                    {
                                        String message = String.Empty;
                                        if (UserInfo.gMchnTp == "TC")
                                            message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP);
                                        else
                                            message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("F16a", LanguageService.MESSAGE_JOBSETERROR_GROUP);

                                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0221", LanguageService.LABEL_CUSTOMIZE), message,
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                    }
                                    else
                                    {
                                        if (clone.resultObj != null)
                                        {
                                            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                       PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                       , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                        }
                                        else
                                        {
                                            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                      PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE)
                                      , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050", LanguageService.MESSAGE_GROUP),
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                        }

                                    }
                                }
                                else if (wrkCd == "1") // 20200213 mess code F5
                                {
                                    wrkCd = "";
                                    plcCnt = 11;
                                    if (!String.IsNullOrEmpty(clone.resultObj) && clone.resultObj.Equals("F5"))
                                    {
                                        BeefPlay_Windows_Exclamation();

                                        String message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP);
                                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), message,
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                    }
                                    else
                                    {
                                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE),
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050", LanguageService.MESSAGE_GROUP),
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                    }
                                }
                                else
                                {
                                    wrkCd = "";
                                    plcCnt = 31;
                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                      PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE)
                                      , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050", LanguageService.MESSAGE_GROUP),
                                        PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                }
                            }
                            clone = null;
                        }));
        }
        #endregion [NotifyProcessPLC]

        #region [NotifyCancelPLC]
        public void NotifyCancelPLC(RMG.VD_RMG_VmtResult value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_VmtResult>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_VmtResult>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            ////CLEAR TWISTLOCK CONTAINER
                            //string currBay = PresentationMgr.MainView.plcDomainTwistLock.currentBay;
                            //if (!String.IsNullOrEmpty(currBay) && currBay != "00")
                            //{
                            //    int bay = int.Parse(currBay);
                            //    if (bay % 2 == 0) bay -= 1;
                            //    currBay = (bay < 10 ? "0" : "") + bay.ToString();
                            //}

                            //if (!String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo) && wrkCd == ""
                            //    && PresentationMgr.MainView.plcDomainTwistLock.currentBlock.Equals(PresentationMgr.Singleton.CurrentBlock)
                            //    && currBay.Equals(PresentationMgr.Singleton.CurrentBay))
                            //{
                            //    PresentationMgr.MainView.plcDomainTwistLock.cntrNo = String.Empty;
                            //    PresentationMgr.MainView.UC_BayView.SetBayItemInfo_Target(PresentationMgr.MainView.plcDomainTwistLock.currentCell, Convert.ToInt32(PresentationMgr.MainView.plcdomain.currentTier));
                            //}

                            if (wrkCd == "1" || wrkCd == "2" || wrkCd == "XX")
                            {
                                wrkCd = "";
                                plcCnt = 31;
                            }

                            if ("S1".Equals(clone.resultObj) || clone.resultTp.Contains("S")) //wrk = 3 & 4 will set empty in callback this process
                            {
                                //PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content = String.Empty;
                                //PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = String.Empty;
                                //PresentationMgr.Singleton.CorrectionSource.Clear();
                                //PresentationMgr.Singleton.NeedJobAutoSelection = true;
                                //VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;

                            }
                            else
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                  "Fail", PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050", LanguageService.MESSAGE_GROUP), "OK", null, 0);
                            }
                            clone = null;
                        }));
        }
        #endregion [NotifyCancelPLC]

        #region [NotifyReleasePLCLock]
        public void NotifyReleasePLCLock(RMG.VD_RMG_VmtResult value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_VmtResult>(value);

            // wrkCd == 2
            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            endPollingPLCwrkCd12 = 1;

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_VmtResult>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            if (clone.resultTp.Equals("SUCCESS"))
                            {
                                PresentationMgr.Singleton.CorrectionSource.Clear();
                                PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                                PresentationMgr.MainView.plcDomainTwistLock.cntrNo = String.Empty;

                                System.Threading.Thread.Sleep(500);
                                PresentationMgr.Singleton.SendGetInventoryList4Multi_Sync_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);

                                {//SET LAYOUT DEFAULT FOR YT BUTTON
                                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content = String.Empty;
                                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = String.Empty;

                                    if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                                    {
                                        PresentationMgr.SetSkinButton(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber,
                                            UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                                    }
                                    else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                                    {
                                        PresentationMgr.SetSkinButton(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber,
                                            UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                                    }
                                }

                                var jobLst = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrReleasePLC);

                                if (jobLst.Count > 0)
                                {
                                    var jobKey = jobLst.First().jobKey;
                                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
                                    {
                                        if (jobOrder.jobKey == jobKey)
                                        {
                                            jobOrder.type.jobStatus = "A";
                                            //DataMgr.Singleton.List_JobOrder.Remove(jobOrder);
                                            break;
                                        }
                                    }
                                    PresentationMgr.Singleton.JOB_Sort();
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                                    PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(500);
                                    VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();
                                }

                                //RESTART POLLING
                                {
                                    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
                                    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
                                    wrkCd = "";
                                }
                            }
                            else
                            {
                               //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                               //PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                               //, PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);

                            }
                            cntrReleasePLC = String.Empty;
                            clone = null;
                        }));
        }
        #endregion [NotifyReleasePLCLock]

        #region [NotifyEmptySwappingTargetList]
        public void NotifyEmptySwappingTargetList(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapOutVO value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapOutVO>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.Singleton.swapList.Clear();
                            PresentationMgr.Singleton.reservedList.Clear();

                            foreach (VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO swapJob in clone.swappingList)
                            {
                                PresentationMgr.Singleton.swapList.Add(swapJob);
                            }
                            foreach (VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO reservedJob in clone.reservedList)
                            {
                                PresentationMgr.Singleton.reservedList.Add(reservedJob);
                            }

                            PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, Convert.ToString(PresentationMgr.Singleton.CurrentBay), true, true, false);
                            clone = null;
                        }));
        }
        #endregion [NotifyEmptySwappingTargetList]

        #region [NotifyDoEmptySwap]
        public void NotifyDoEmptySwap(RMG.VD_RMG_VmtResult value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_VmtResult>(value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.Singleton.swapList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                            PresentationMgr.Singleton.reservedList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                            PresentationMgr.Singleton.swapListRTG = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();

                            if (!clone.resultTp.ToUpper().Equals("SUCCESS"))
                            {
                                String message = !String.IsNullOrEmpty(clone.resultMsg) ? clone.resultMsg
                                : !String.IsNullOrEmpty(clone.resultObj) ? PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                                : String.Empty;

                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0127", LanguageService.LABEL_CUSTOMIZE), message
                                    , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                PresentationMgr.Singleton.CorrectionSource.Clear();
                            }
                            else
                            {
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content = String.Empty;
                                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = String.Empty;

                                Thread.Sleep(500);
                                PresentationMgr.Singleton.CorrectionSource.Clear();
                                PresentationMgr.Singleton.SendGetInventoryList4Multi_Sync_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
                                Thread.Sleep(500);
                                VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineJobByKeys_Sync_Ask();

                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0225", LanguageService.LABEL_CUSTOMIZE),
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("S1", LanguageService.MESSAGE_JOBDONE_GROUP),
                                    PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), 
                                    new UC_PopupView.Callback_Popup(CallbackClosePopup), 0);
                            }
                            clone = null;

                            // Process like setEmptySwap
                            //if (!clone.resultTp.ToUpper().Equals("SUCCESS"))
                            //{
                            //    String message = !String.IsNullOrEmpty(clone.resultMsg) ? clone.resultMsg
                            //    : !String.IsNullOrEmpty(clone.resultObj) ? PresentationMgr.Singleton.LanguageSer.GetResourceRMG(clone.resultObj, LanguageService.MESSAGE_JOBSETERROR_GROUP)
                            //    : String.Empty;

                            //    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                            //     "Fail", message, "OK", null, 0);
                            //}
                            //else
                            //{                             
                            //    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                            //      "Success", "Completed", "OK", new UC_PopupView.Callback_Popup(CallbackClosePopup), 0);
                            //}
                            //clone = null;
                        }));
        }
        #endregion [NotifyDoEmptySwap]

        #endregion [Callback Delegate Functions - RMG]


        #region [NotifyLoginConfig]
        public void NotifyLoginConfig(ref VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive value)
        {

            PresentationMgr.jobItemColor = value.jobItemColor;
            value = null;
        }
        #endregion [NotifyLoginConfig]

        #region [Callback Delegate Functions - RMG]

        #region [RMG_Periodic]
        public void RMG_Periodic(ref RMG.VD_RMG_PDS_Periodic_Payload value)
        {
        }
        #endregion [RMG_Periodic]

        #region [RMG_CpsAlign]
        public void RMG_CpsAlign(ref RMG.VD_RMG_CPS_Alignment_Payload value)
        {
        }
        #endregion [RMG_CpsAlign]

        #region [RMG_PickDrop]
        public void RMG_PickDrop(ref RMG.VD_RMG_PDS_PickDrop_Payload value)
        {
        }
        #endregion [RMG_PickDrop]

        #region [RMG_RFID]
        public void RMG_RFID(ref RMG.VD_RMG_PDS_RFID_Payload value)
        {
        }
        #endregion [RMG_RFID]

        #region [RMG_POWInfo]
        public void RMG_POWInfo(ref RMG.VD_RMG_POWInfo_Receive value)
        {
        }
        #endregion [RMG_POWInfo]

        #region [RMG_BlockEnteranceITV]
        public void RMG_BlockEnteranceITV(ref RMG.VD_RMG_BlockEnteranceITV_Receive value)
        { }
        #endregion [RMG_BlockEnteranceITV]

        #region [RMG_ManualReadyITV]
        public void RMG_ManualReadyITV(ref RMG.VD_RMG_ManualReadyITV_Receive value)
        {
        }
        #endregion [RMG_ManualReadyITV]

        #region [RMG_BlockBayInfo]
        public void RMG_BlockBayInfo(ref RMG.VD_RMG_InventoryInfo_Receive value)
        {
        }
        #endregion [RMG_BlockBayInfo]

        #region [RMG_BlockBayInfoSimple]
        public void RMG_BlockBayInfoSimple(ref RMG.VD_RMG_BlockBayInfoSimple_Receive value)
        {
        }
        #endregion [RMG_BlockBayInfoSimple]

        #region [RMG_Correction]
        public void RMG_Correction(ref RMG.VD_RMG_InventoryInfo_Receive value)
        {
        }
        #endregion [RMG_Correction]

        #region [RMG_SetCurrentJob]
        public void RMG_SetCurrentJob(ref RMG.VD_RMG_SetCurrentJob_Receive value)
        {
        }
        #endregion [RMG_SetCurrentJob]

        #region [RMG_TargetJob]
        public void RMG_TargetJob(ref RMG.VD_RMG_TargetJob_Receive value)
        {
        }
        #endregion [RMG_TargetJob]

        #region [RMG_Marring]
        public void RMG_Marring(ref RMG.VD_RMG_RMGMarrying_Receive value)
        {
        }
        #endregion [RMG_Marring]

        #region [RMG_SwapResult]
        public void RMG_SwapResult(ref RMG.VD_RMG_SwapResult_Receive value)
        {
        }
        #endregion [RMG_SwapResult]

        #region [RMG_ReturnWarning]
        public void RMG_ReturnWarning(ref RMG.VD_RMG_ReturnWarning_Receive value)
        {
        }
        #endregion [RMG_ReturnWarning]


        //public void RMG_Periodic(ref VMT_Data_JAT2.Common.sRMG_PDS_Periodic_Payload_Recv2 value)
        //{
        //    if (PresentationMgr.Singleton.IsUIInit == false)
        //        return;

        //    VMT_Data_JAT2.Common.sRMG_PDS_Periodic_Payload_Recv2 clone = Util.DeepCopy<VMT_Data_JAT2.Common.sRMG_PDS_Periodic_Payload_Recv2>(value);

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Common.sRMG_PDS_Periodic_Payload_Recv2>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

        //                    //-------------------------------------------------------
        //                    //-  Parsing Periodic Information
        //                    string timeInstance = VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cTimeInstance);
        //                    bool bGantryMoveOnOff;
        //                    if (clone.m_cGantryMoveOnOff == '0')
        //                        bGantryMoveOnOff = false;
        //                    else
        //                        bGantryMoveOnOff = true;
        //                    double driverDirectionDegree = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cDriveDirectionDegree));
        //                    bool bAntiCollisionDetectionSignal;
        //                    if (clone.m_cAntiCollisionDetectionSignal == 0)
        //                        bAntiCollisionDetectionSignal = true;
        //                    else
        //                        bAntiCollisionDetectionSignal = false;
        //                    double position_la = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cLatitude));
        //                    double position_lo = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cLongitude));
        //                    double trollyPosition = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cTrolleyPosition));
        //                    double hoistPosition = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cHoistPosition));
        //                    string RFIDStatus;
        //                    if (clone.m_cRFIDStatus == '1')
        //                        RFIDStatus = "run";
        //                    else
        //                        RFIDStatus = "Break Down";
        //                    string fuelGage;
        //                    if (clone.m_cFuelGage == '1')
        //                        fuelGage = "run";
        //                    else
        //                        fuelGage = "Break Down";
        //                    string tirePressurRMGeck;
        //                    if (clone.m_cTirePressurRMGeck == '1')
        //                        tirePressurRMGeck = "run";
        //                    else
        //                        tirePressurRMGeck = "Break Down";

        //                    string strPriodicInfo = string.Format("LA:{0} / LO:{1}\r\n", position_la, position_lo);
        //                    YardView yardView = PresentationMgr.Singleton.GetUIComponent("VMT_RMG.YardView") as YardView;
        //                    yardView.TextBlock_RMG_Position.Text = strPriodicInfo;


        //                    //-----------------------------------------------
        //                    //- Set Tooltip 
        //                    string sInfo = "";

        //                    sInfo += string.Format("TimeInstance : {0} \r\n", timeInstance);
        //                    sInfo += string.Format("GantryMove On/Off : {0} \r\n", bGantryMoveOnOff);
        //                    sInfo += string.Format("DriverDirectionDegree : {0} \r\n", driverDirectionDegree);
        //                    sInfo += string.Format("AntiCollisionDetectionSignal : {0} \r\n", bAntiCollisionDetectionSignal);
        //                    sInfo += string.Format("Latitude : {0} \r\n", position_la);
        //                    sInfo += string.Format("Longitude : {0} \r\n", position_lo);
        //                    sInfo += string.Format("TrollyPosition : {0}mm \r\n", trollyPosition);
        //                    sInfo += string.Format("HoistPosition : {0}mm \r\n", hoistPosition);
        //                    sInfo += string.Format("RFID Status : {0} \r\n", RFIDStatus);
        //                    sInfo += string.Format("FuelGage : {0} \r\n", fuelGage);
        //                    sInfo += string.Format("TirePressurRMGeck : {0} \r\n", tirePressurRMGeck);

        //                    yardView.TextBlock_RMG_Message.Text = "RMG_Periodic";
        //                    yardView.TextBlock_RMG_Text.Text = sInfo;

        //                    // Output Log to LogWin
        //                    MainWindow.LogWin.WriteLog("---------------------------------------");
        //                    MainWindow.LogWin.WriteLog("RMG_Periodic");
        //                    MainWindow.LogWin.WriteLog("--------------------------------------");
        //                    MainWindow.LogWin.WriteLog(sInfo);


        //                    //-----------------------------------------------
        //                    //- Update RMG GPS Position
        //                    VMT_Data_JAT2.Common.sGeoPoint pt = PresentationMgr.Singleton.CurrentGeoPoint;
        //                    pt.lo = position_lo;
        //                    pt.la = position_la;
        //                    PresentationMgr.Singleton.CurrentGeoPoint = pt;

        //                    // Update Trolley Position
        //                    PresentationMgr.Singleton.TrolleyPos = trollyPosition;

        //                    // Update Hoist Position
        //                    PresentationMgr.Singleton.HoistPosition = hoistPosition;

        //                    // Switching StopView ( When Gantry is not moving )
        //                    if (!bGantryMoveOnOff)
        //                    {
        //                        PresentationMgr.Singleton.STV_TriggerBayView();
        //                    }
        //                    else
        //                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.Moving);

        //                }));
        //}

        //public void RMG_PickDrop(ref VMT_Data_JAT2.Common.sRMG_PDS_PickDrop_Payload_Recv value)
        //{
        //    if (PresentationMgr.Singleton.IsUIInit == false)
        //        return;

        //    VMT_Data_JAT2.Common.sRMG_PDS_PickDrop_Payload_Recv clone = Util.DeepCopy<VMT_Data_JAT2.Common.sRMG_PDS_PickDrop_Payload_Recv>(value);

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Common.sRMG_PDS_PickDrop_Payload_Recv>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

        //                    // TimeStamp
        //                    string timeInstance = VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cTimeInstance);
        //                    // Trolley Position
        //                    double trollyPosition = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cTrolleyPosition));
        //                    // Hoist Position
        //                    double hoistPosition = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cHoistPosition));
        //                    // Operation Type
        //                    string operationType = "";
        //                    if (clone.m_cOperationType == '0')
        //                        operationType = "Null";
        //                    else if (clone.m_cOperationType == '1')
        //                        operationType = "20ft";
        //                    else if (clone.m_cOperationType == '2')
        //                        operationType = "20ft_Twin";
        //                    else if (clone.m_cOperationType == '3')
        //                        operationType = "40ft";
        //                    else if (clone.m_cOperationType == '4')
        //                        operationType = "45ft";
        //                    else if (clone.m_cOperationType == '5')
        //                        operationType = "48ft";
        //                    else if (clone.m_cOperationType == '8')
        //                        operationType = "None containerized operation";
        //                    else if (clone.m_cOperationType == 'E')
        //                        operationType = "Error";

        //                    // Block
        //                    string block = VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cBlock);
        //                    // Bay
        //                    string bay = VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cBay);
        //                    // Row
        //                    string row = String.Format("{0}", clone.m_cRow);
        //                    // Tier
        //                    int tier = (int)clone.m_cTier;
        //                    // Twist Lock/Unlock
        //                    String twistLcokUnlock = "";

        //                    if( clone.m_cTwistLockUnlock == '1')
        //                        twistLcokUnlock = "Lock";
        //                    else if( clone.m_cTwistLockUnlock == '2')
        //                        twistLcokUnlock = "Unlock";
        //                    else if( clone.m_cTwistLockUnlock == 'E')
        //                        twistLcokUnlock = "Error";

        //                    // Container Weight
        //                    double containerWeight = double.Parse(VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cContainerWeight));


        //                    //yardView.TextBlock_RMG_Periodic.Text = sInfo;

        //                    //-----------------------------------------------
        //                    //- Set Tooltip 
        //                    string sInfo = "";

        //                    sInfo += string.Format("TimeInstance : {0} \r\n", timeInstance);
        //                    sInfo += string.Format("TrollyPosition : {0}mm \r\n", trollyPosition);
        //                    sInfo += string.Format("HoistPosition : {0}mm \r\n", hoistPosition);
        //                    sInfo += string.Format("Operation Type : {0} \r\n", operationType);
        //                    sInfo += string.Format("Block : {0} \r\n", block);
        //                    sInfo += string.Format("Bay : {0} \r\n", bay);
        //                    sInfo += string.Format("Tier : {0} \r\n", tier.ToString());
        //                    sInfo += string.Format("TwistLockUnlock : {0} \r\n", twistLcokUnlock);

        //                    sInfo += string.Format("Container Weight : {0} \r\n", containerWeight.ToString());

        //                    // Output Log to LogWin
        //                    MainWindow.LogWin.WriteLog("--------------------------------------");
        //                    MainWindow.LogWin.WriteLog("RMG_PickDrop");
        //                    MainWindow.LogWin.WriteLog("--------------------------------------");
        //                    MainWindow.LogWin.WriteLog(sInfo);

        //                    // Update Trolley Position
        //                    PresentationMgr.Singleton.TrolleyPos = trollyPosition;

        //                    // Update Hoist Position
        //                    PresentationMgr.Singleton.HoistPosition = hoistPosition;

        //                    // Update Hoist Lock/Unlock
        //                    PresentationMgr.Singleton.TwistLockStatus = twistLcokUnlock;



        //                }));
        //}

        //public void RMG_TireFuel(ref VMT_Data_JAT2.Common.sRMG_PDS_TireFuel_Payload_Recv value)
        //{
        //    VMT_Data_JAT2.Common.sRMG_PDS_TireFuel_Payload_Recv clone = Util.DeepCopy<VMT_Data_JAT2.Common.sRMG_PDS_TireFuel_Payload_Recv>(value);

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Common.sRMG_PDS_TireFuel_Payload_Recv>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
        //                }));
        //}

        //public void RMG_RFID(ref VMT_Data_JAT2.Common.sRMG_PDS_RFID_Payload_Recv value)
        //{
        //    VMT_Data_JAT2.Common.sRMG_PDS_RFID_Payload_Recv clone = Util.DeepCopy<VMT_Data_JAT2.Common.sRMG_PDS_RFID_Payload_Recv>(value);

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Common.sRMG_PDS_RFID_Payload_Recv>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
        //                }));
        //}

        //public void RMG_CpsAlign(ref VMT_Data_JAT2.Common.sRMG_CPS_Alignment_Payload_Recv value)
        //{
        //    if (PresentationMgr.Singleton.IsUIInit == false)
        //        return;

        //    VMT_Data_JAT2.Common.sRMG_CPS_Alignment_Payload_Recv clone = Util.DeepCopy<VMT_Data_JAT2.Common.sRMG_CPS_Alignment_Payload_Recv>(value);

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Common.sRMG_CPS_Alignment_Payload_Recv>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

        //                    // TimeStamp
        //                    string timeInstance = VMT_Data_JAT2.Common.TranscodeByteArrayToString(clone.m_cTimeInstance);
        //                    // Direction
        //                    string direction = "";
        //                    if (clone.m_cDirection == '0')
        //                        direction = "Empty";
        //                    else if (clone.m_cDirection == '1')
        //                        direction = "Normal Direction";
        //                    else if (clone.m_cDirection == '2')
        //                        direction = "Reverse Direction";
        //                    else if (clone.m_cDirection == '3')
        //                        direction = "Full";
        //                    else if (clone.m_cDirection == 'E')
        //                        direction = "Error";
        //                    // Alignment Result Type
        //                    string alignmentResult = "";
        //                    if (clone.m_cAlignmentResult == '0')
        //                        alignmentResult = "Empty";
        //                    else if (clone.m_cAlignmentResult == '1')
        //                        alignmentResult = "Completed";
        //                    else if (clone.m_cAlignmentResult == '2')
        //                        alignmentResult = "Processing";
        //                    else if (clone.m_cAlignmentResult == '3')
        //                        alignmentResult = "Detected";
        //                    else if (clone.m_cAlignmentResult == '4')
        //                        alignmentResult = "Passed";
        //                    else if (clone.m_cAlignmentResult == 'E')
        //                        alignmentResult = "Error";

        //                    // ForeAfter
        //                    string foreAfter = "";
        //                    if (clone.m_cForeAfter == '0')
        //                        foreAfter = "Null";
        //                    else if (clone.m_cForeAfter == '1')
        //                        foreAfter = "Fore";
        //                    else if (clone.m_cForeAfter == '2')
        //                        foreAfter = "After";
        //                    else if (clone.m_cForeAfter == 'E')
        //                        foreAfter = "Error";

        //                    // JobType
        //                    string jobType = "";
        //                    if (clone.m_cJobType == 'D')
        //                        jobType = "Yard Out";
        //                    else if (clone.m_cJobType == 'L')
        //                        jobType = "Yard In";
        //                    else
        //                        jobType = "Error";


        //                    //-----------------------------------------------
        //                    //- Set Tooltip 
        //                    string sInfo = "";

        //                    sInfo += string.Format("TimeInstance : {0} \r\n", timeInstance);
        //                    sInfo += string.Format("Direction : {0}mm \r\n", direction);
        //                    sInfo += string.Format("AlignmentResult : {0}mm \r\n", alignmentResult);

        //                    sInfo += string.Format("ForeAfter : {0} \r\n", foreAfter);
        //                    sInfo += string.Format("JobType : {0} \r\n", jobType);

        //                    // Output Log to LogWin
        //                    MainWindow.LogWin.WriteLog("--------------------------------------");
        //                    MainWindow.LogWin.WriteLog("RMG_CpsAlign");
        //                    MainWindow.LogWin.WriteLog("--------------------------------------");
        //                    MainWindow.LogWin.WriteLog(sInfo);

        //                }));
        //}

        //public void NotifyBlockInfo(IntPtr value)
        //{
        //    try
        //    {
        //        VMT_Data_JAT2.Common.sBlockBayInfo clone = (VMT_Data_JAT2.Common.sBlockBayInfo)Marshal.PtrToStructure(value, typeof(VMT_Data_JAT2.Common.sBlockBayInfo));

        //        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Common.sBlockBayInfo>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
        //                }));

        //        DataMgr.Singleton.Dic_BlockBayInfo[PresentationMgr.Singleton.RequestBayInventory] = clone;

        //        PresentationMgr.Singleton.CompleteResetEvent.Set();
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}
        #endregion [Callback Delegate Functions - RMG]

        private String[] mAvailable_default_message ={"Day/Night",
                                                        "Change Chassis",
                                                        "Change Driver",
                                                        "Oiling",
                                                        "Toilet",
                                                        "Punk",
                                                        "Accident",
                                                        "OOG",
                                                        "RMG Breakdown",
                                                        "GC Breakdown",
                                                        "YC Breakdown"
                                                          };
        private ArrayList mAvaliableList;
        private void InitAvailable()
        {
            mAvaliableList = new ArrayList();

            for (Int32 i = 0; i < mAvailable_default_message.Length; i++)
            {
                mAvaliableList.Add(mAvailable_default_message[i]);
            }

            // Add Available Grid UI
            for (Int32 i = 0; i < mAvaliableList.Count; i++)
            {
                AvailableControl aControl = new AvailableControl();
                aControl.ReasonCd = (String)mAvaliableList[i];
                aControl.ReasonNm = (String)mAvaliableList[i];

                if (aControl.ReasonNm.Length > 0)
                    aControl.Visibility = System.Windows.Visibility.Visible;
                else
                    aControl.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //public MainView UC_MainView = null;
        //public PopupProgressView PopupProgressView = new PopupProgressView();
        //public MachineSettingView UC_MachineSettingView = new MachineSettingView();
        //public Keypad UC_KeypadView = new Keypad();
        //public UC_PopupView UC_PopupView = new UC_PopupView();
        private void MainWin_InitView()
        {
            //<local:MainView x:Name="UC_MainView" Grid.Row="1" Visibility="Hidden"/>
            //this.UC_MainView = new MainView();
            //this.UC_MainView.Visibility = Visibility.Hidden;
            //Grid.SetRow(this.UC_MainView, 1);
            //this.LayoutRoot.Children.Insert(2, this.UC_MainView);
            ////this.LayoutRoot.Children.Add(this.UC_MainView);            

            ////<local:PopupProgressView x:Name='PopupProgressView' Loaded='PopupProgressView_Loaded' HorizontalAlignment='Left' VerticalAlignment='Top' Height='768' Grid.RowSpan='2' Width='1024' Visibility='Hidden'/>
            ////this.PopupProgressView = new PopupProgressView();
            // Init MainWindow to PopupProgressViw
            this.PopupProgressView.Init(this);
            //this.PopupProgressView.Loaded += PopupProgressView_Loaded;
            //this.PopupProgressView.HorizontalAlignment = HorizontalAlignment.Left;
            //this.PopupProgressView.VerticalAlignment = VerticalAlignment.Top;
            //this.PopupProgressView.Width = 1024;
            //this.PopupProgressView.Height = 768;
            //this.PopupProgressView.Visibility = Visibility.Hidden;
            //Grid.SetRowSpan(this.PopupProgressView, 2);                
            //this.LayoutRoot.Children.Add(this.PopupProgressView);            

            ////<local:MachineSettingView x:Name='UC_MachineSettingView' Margin='0' d:LayoutOverrides='Width, Height' Grid.RowSpan='2' Visibility='Hidden'/>
            ////this.UC_MachineSettingView = new MachineSettingView();            
            //Grid.SetRowSpan(this.UC_MachineSettingView, 2);
            //this.UC_MachineSettingView.Visibility = Visibility.Hidden;
            //this.LayoutRoot.Children.Add(this.UC_MachineSettingView);

            ////<local:Keypad x:Name='UC_KeypadView' Height='240' Margin='0' VerticalAlignment='Bottom' d:LayoutOverrides='VerticalAlignment' Grid.Row='1' Visibility='Hidden'/>
            ////this.UC_KeypadView = new Keypad();
            //this.UC_KeypadView.VerticalAlignment = VerticalAlignment.Bottom;
            //this.UC_KeypadView.Height = 240;            
            //this.UC_KeypadView.Visibility = Visibility.Hidden;            
            //Grid.SetRow(this.UC_KeypadView, 1);
            //this.LayoutRoot.Children.Add(this.UC_KeypadView);

            ////<local:UC_PopupView x:Name='UC_PopupView' Grid.Row='1' d:LayoutOverrides='Height' Visibility='Hidden'/>
            ////this.UC_PopupView = new UC_PopupView();
            //this.UC_PopupView.Visibility = Visibility.Hidden;
            //Grid.SetRow(this.UC_PopupView, 1);
            //this.LayoutRoot.Children.Add(this.UC_PopupView);

            this.UC_LogInView.TextBox_IDNumber.IsEnabled = true;

            this.LayoutUpdated += this.MainWin_LayoutUpdated;

            this.LayoutRoot.UpdateLayout();
        }

        private void MainWin_LayoutUpdated(object sender, EventArgs e)
        {
            this.LayoutUpdated -= this.MainWin_LayoutUpdated;

            //if (!App.TEST_MODE && !App.STANDALONE_MODE)
            //{
            //    this.ShowProgressBar(0, true, "Checking update version...");
            //    new PresentationMgr.SingleShot(15000, this.HideProgressBarInvoke); // 15 sec
            //}
            this.UC_LogInView.Focus();
        }

        private void MainWin_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ////---------------------------------------------
            ////- Init Current GPS Position
            VMT_Data_JAT2.Marshalling.Geometry.sGeoPoint pt = new VMT_Data_JAT2.Marshalling.Geometry.sGeoPoint();
            pt.lo = 39.085391;
            pt.la = 22.527045;
            PresentationMgr.Singleton.CurrentGeoPoint = pt;
            PresentationMgr.Singleton.TrolleyPos = PresentationMgr.const_RangeTrolley.Max;
            PresentationMgr.Singleton.HoistPosition = PresentationMgr.const_RangeHoist.Max;


            // Allow External Periodic Signal
            PresentationMgr.Singleton.IsUIInit = true;
            PresentationMgr.Singleton.UIInit();

            // Run LiveUpdate
            PresentationMgr.FileTouchEvent_RunUpdate();

            new System.Threading.Timer(new System.Threading.TimerCallback(
                delegate
                {
                    if (!Dispatcher.CheckAccess())
                    {
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(delegate
                                {
                                    SplashForm.HideSplash();

                                    this.MainWin_InitView();
                                    this.InitRMGConnetivity();   // Init RMG Connectivity                                        
                                }));
                    }
                    else
                    {
                        SplashForm.HideSplash();

                        this.MainWin_InitView();
                        this.InitRMGConnetivity();   // Init RMG Connectivity  
                    }
                })
                , null, 0, System.Threading.Timeout.Infinite);
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
                    PresentationMgr.AppWin.UC_IndicatorView.SetDownloadProgress((int)wParam, (int)lParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }
    }
}