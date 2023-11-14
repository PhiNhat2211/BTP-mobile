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
using VMT_RMG;

namespace VMT_RMG_800by600
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
        static public String SERVICE_COMPANY = "JAT2";  // NCT:NCT   JAT2:JAT2
        static public String KeyCLTVMT_RMG = @"SOFTWARE\CyberLogitec\VMT-RMG for JAT2"; // NCT:NCT   JAT2:JAT2
        static public String DefUpdateServerAddr = "172.19.51.17:56000";  // Default Update Server URL
        public const string VMT_EngineDLL = "EE_Sensor_KAP.dll";
        public const string VMT_DataMgrDLL = "VMT_Data_JAT2.Common.dll";

        private const String SIEMENS_MUTEXTNAME = "SIEMENS_Interface_VMT_RMG";
        public const String SIEMENS_PROCESSNAME = "SIEMENSInterface";

        #region [variables]
        private double _DesignWidth = 1024;
        private double _DesignHeight = 768;

        public const int MESSAGE_TO_EEVMT_LOGIN = 0x10001;
        public const int MESSAGE_TO_EEVMT_CONNECT = 0x10002;
        private Int32 PORT_SIEMENS_INTERFACE = 58001;
        private Int32 PERIOD_SIEMENS_CONNECTION_CHECK = 30;
        //private Int32 PERIOD_SIEMENS_RECEIVE_CHECK = 30;

        public Boolean gIsDay = true;

        //public String sMchnID = "";
        //public String sUserID = "";
        //public String sUserPW = "";

        public static LogWindow LogWin = null;
        static public bool ShowAppUI = true;
        public Boolean gIsServerConnected = false;

        public socketServerTCP TcpSock4Siemens = null;
        private System.Threading.Timer _siemensProcessTimer = null;
        //private System.Timers.Timer _siemensReceiveTimer = null;

        static public Boolean IsMachineLogOn = false;

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
            
            // Initialize Logger
            DateTime time = DateTime.Now;             // Use current time
            string formatFile = "yyyy-MM-dd_HH";      // Use this format
            string filetag = time.ToString(formatFile);   // Write to console
            string logFile = string.Format(@"Log\HessianLog_{0}.txt", filetag);
            Logger.SetLogPath(logFile);
            Logger.SetLogFilters(Logger.LogFilters.Debug);

            // Initialize Machine
            int size = 0;
            RMG.RMG_User.GetMachineID(ref RMG.RMG_User.gMchnID, ref size);
            RMG.RMG_User.GetMachineType(ref RMG.RMG_User.gMchnTp, ref size);
            //PresentationMgr.Singleton.MachineID = RMG.RMG_User.gMchnID;

            // Loading Application Configuration File
            this.LoadAppCfg();

            // Loading Application Setting File
            //this.LoadAppSetting();

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
            EventMgr.Singleton.Process_KeyDown(sender, e);
        }

        #region [Application Configuration Methods]

        public void LoadAppCfg()
        {
            string strCfgPath = "";

            if (MainWindow.SERVICE_COMPANY.Equals("JAT2"))
                strCfgPath = AppCfgMgr.GetAppDirectory() + "JAT2_VMT_RMG.cfg.xml";

            // Loading Application Config XML Document.
            AppCfgMgr.Singleton.LoadFile(strCfgPath);

            string strTestMode;
            strTestMode = AppCfgMgr.Singleton.GetValueByKey("IsTestMode");
            if (strTestMode == "1") App.TEST_MODE = true; // true - test mode, false - real mode
            else App.TEST_MODE = false; // true - test mode, false - real mode

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

            string strHessianServerIP = AppCfgMgr.Singleton.GetValueByKey("HessianServerIP");
            VMT_DataMgr.gHessianServerIP = strHessianServerIP;
            string strHessianServerPort = AppCfgMgr.Singleton.GetValueByKey("HessianServerPort");
            VMT_DataMgr.gHessianServerPort = strHessianServerPort;

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
            //int n_sWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //int n_sHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            int n_sWidth = 800;
            int n_sHeight = 600;

            //int n_sWidth = (int)_DesignWidth;
            //int n_sHeight = (int)_DesignHeight;

            this.Width = n_sWidth;
            this.Height = n_sHeight;

            this.Grid_Scale.CenterX = .5;
            this.Grid_Scale.CenterY = .5;
            this.Grid_Scale.ScaleX = n_sWidth / _DesignWidth;
            this.Grid_Scale.ScaleY = n_sHeight / _DesignHeight;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            //this.Location = new Point(
            //    n_sWidth / 2 - this.Size.Width / 2,
            //    n_sHeight / 2 - this.Size.Height / 2
            //); 
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

                VMT_DataMgr_Common.GetMachineStopCodeList_Ask();
            }
            else
            {
            }

            PresentationMgr.StartNetworkCheck();
            this.TcpSock4Siemens = new socketServerTCP();
            this.TcpSock4Siemens.socketServerInit(PORT_SIEMENS_INTERFACE);
            this.TcpSock4Siemens.tcpMsgEvt += new socketServerTCP.TcpSrvMsgEventHandler(SiemensInterface_TcpSrvMsgEventHandler);
            this.TcpSock4Siemens.tcpRcvMsgEvt += new socketServerTCP.TcpSrvRcvMsgEventHandler(SiemensInterface_TcpSrvRcvMsgEventHandler);
            this.TcpSock4Siemens.startLisner();
            _siemensProcessTimer = new System.Threading.Timer(new System.Threading.TimerCallback(SiemensProcess_TimerCallback),
                null, 1000, PERIOD_SIEMENS_CONNECTION_CHECK * 1000);

            //_siemensReceiveTimer = new System.Timers.Timer(PERIOD_SIEMENS_RECEIVE_CHECK * 1000);
            //_siemensReceiveTimer.Elapsed += new System.Timers.ElapsedEventHandler(_siemensReceiveTimer_Elapsed);
            //_siemensReceiveTimer.AutoReset = true;
            //_siemensReceiveTimer.Start();

            return true;
        }

        private void SiemensProcess_TimerCallback(object state)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.UC_IndicatorView.CheckBox_Siemens.IsChecked = PresentationMgr.Singleton.IsSiemensConnectionChecked;
                            PresentationMgr.Singleton.IsSiemensConnectionChecked = false;

                            if (!System.Diagnostics.Process.GetProcessesByName(SIEMENS_PROCESSNAME).Any())
                            {
                                var dir = System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
                                var siemensProcesss = System.IO.Path.Combine(System.IO.Path.Combine(dir.FullName, "SIEMENS_Interface"), (SIEMENS_PROCESSNAME + ".exe"));
                                if (System.IO.File.Exists(siemensProcesss))
                                    System.Diagnostics.Process.Start(siemensProcesss);
                            }
                        }));
        }

        //void _siemensReceiveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    PresentationMgr.Singleton.IsSiemensConnectionChecked = false;

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                    new Action(delegate
        //                    {
        //                        this.UC_IndicatorView.CheckBox_Siemens.IsChecked = PresentationMgr.Singleton.IsSiemensConnectionChecked;
        //                    }));
        //}

        private void SiemensInterface_TcpSrvMsgEventHandler(object sender, String rcvMsg, Boolean sMsgFlag)
        {
        }

        private void SiemensInterface_TcpSrvRcvMsgEventHandler(object sender, byte[] rcvMsg)
        {
            try
            {
                if (rcvMsg.Length == 1 && rcvMsg[0] == 0)
                {
                    // KeepAlive
                }
                else
                {
                    PresentationMgr.Singleton.IsSiemensConnectionChecked = true;                    
                    this._siemensProcessTimer.Change(0, PERIOD_SIEMENS_CONNECTION_CHECK * 1000);
                    //this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    //        new Action(delegate
                    //        {
                    //            this.UC_IndicatorView.CheckBox_Siemens.IsChecked = PresentationMgr.Singleton.IsSiemensConnectionChecked;
                    //        }));

                    //if (_siemensReceiveTimer != null)
                    //{
                    //    _siemensReceiveTimer.Stop();
                    //    _siemensReceiveTimer.Start();
                    //}

                    if (!IsMachineLogOn)
                        return;

                    var packet = new SIEMENS.Packet();
                    packet.Parse(rcvMsg);

                    if (packet.header.MethodNo == SIEMENS.MethodNo_jobAcceptedReport)
                    {
                        var body = packet.body as SIEMENS.Packet.BodyRmgStatus;
                        if (body != null)
                        {
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(delegate
                                    {
                                        PresentationMgr.Singleton.ProcessSiemensData(body);
                                    }));
                        }
                    }
                }

                if (this.TcpSock4Siemens != null)
                {
                }
            }
            catch (Exception e)
            {
            }
        }

        private void PopupProgressView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            //  PopupProgressView.Init(this);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.LogWin.Close();
            VMT_DataMgr.DestroyVMTClient();
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

            VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop = new VMT_DataMgr_Common_Callback.Callback_NotifyGetMachineStop(NotifyGetMachineStop);
            VMT_DataMgr_Common_Callback.SetCallBack_NotifyAvailable(VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop);

            VMT_DataMgr_RMG_Callback.static_NotifySetMachineStop = new VMT_DataMgr_RMG_Callback.Callback_NotifySetMachineStop(NotifySetMachineStop);
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySetMachineStop(VMT_DataMgr_RMG_Callback.static_NotifySetMachineStop);

            VMT_DataMgr_Common_Callback.static_NotifyErrorCode = new VMT_DataMgr_Common_Callback.Callback_NotifyErrorCode(NotifyErrorCode);
            VMT_DataMgr_Common_Callback.SetCallBack_NotifyErrorCode(VMT_DataMgr_Common_Callback.static_NotifyErrorCode);

            //VMT_DataMgr_Common_Callback.static_NotifyCFGEKF = new VMT_DataMgr_Common_Callback.Callback_NotifyCFGEKF(NotifyCFGEKF);
            //VMT_DataMgr_Common_Callback.SetCallBack_NotifyCFGEKF(VMT_DataMgr_Common_Callback.static_NotifyCFGEKF);

            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockList(NotifyBlockList));
            VMT_DataMgr_Common_Callback.SetCallback_NotifyBlockMapList(new VMT_DataMgr_Common_Callback.Callback_NotifyBlockMapList(NotifyBlockMapList));

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

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMachineList(new VMT_DataMgr_RMG_Callback.CallBack_NotifyMachineList(NotifyGetMahchineList));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMachineListofPool(new VMT_DataMgr_RMG_Callback.CallBack_NotifyMachineListofPool(NotifyGetMahchineListofPool));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyMachineLogoutCheck(new VMT_DataMgr_RMG_Callback.CallBack_NotifyMachineLogoutCheck(NotifyMahchineLogoutCheck));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyJobOrderByContainer(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobOrderByContainer(NotifyGetJobOrderByContainer));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyDetwinJob(new VMT_DataMgr_RMG_Callback.CallBack_NotifyDetwinJob(NotifySetDetwinJob));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifySetJobStatus(new VMT_DataMgr_RMG_Callback.CallBack_NotifySetJobStatus(NotifySetJobStatus));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyGetContainerInfo(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetContainerInfo(NotifyGetContainerInfo));
            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyGetTwinContainerInfo(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetContainerInfo(NotifyGetTwinContainerInfo));

            VMT_DataMgr_RMG_Callback.SetCallback_RMG_NotifyGetTwinContainerInfo(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetContainerInfo(NotifyGetTwinContainerInfo));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkArea(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkArea));
            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyGetNoWorkTier(new VMT_DataMgr_RMG_Callback.CallBack_NotifyGetNoWorkArea(NotifyGetNoWorkTier));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyChangePosition(new VMT_DataMgr_RMG_Callback.CallBack_NotifyChangePosition(NotifyChangePosition));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifyJobDone(new VMT_DataMgr_RMG_Callback.CallBack_NotifyJobDone(NotifyJobDone));

            VMT_DataMgr_RMG_Callback.SetCallBack_NotifySetJobReOnChassis(new VMT_DataMgr_RMG_Callback.CallBack_NotifySetJobReOnChassis(NotifyJobReOnChassis));
        }

        #region [Callback Delegate Functions - RMG]

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
        #endregion [NotifyWIFIStatus]

        #region [NotifyGPSStatus]
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

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (String.IsNullOrEmpty(value))
                            {
                                this.UC_IndicatorView.CheckBox_Power.IsChecked = false;
                                PresentationMgr.AppWin.gIsServerConnected = false;

                                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.LogInView &&
                                    PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MachineSettingView)
                                {
                                    PresentationMgr.AppWin.UC_DisconnectPopupView.ShowPopup
                                        (VMT_RMG_800by600.UC_DisconnectPopupView.UC_PopupViewType.PopupViewType_NetworkDisconnect, "Warning", "Network Disconnected.", "Terminate application", NetworkDisconnectCallback, 0);
                                }
                            }
                            else
                            {
                                this.UC_IndicatorView.CheckBox_Power.IsChecked = true;
                                PresentationMgr.AppWin.gIsServerConnected = true;

                                PresentationMgr.AppWin.UC_DisconnectPopupView.HidePopup();
                            }
                        }));
        }

        private void NetworkDisconnectCallback(VMT_RMG_800by600.UC_DisconnectPopupView.UC_PopupViewRetType seleted)
        {
            if (seleted == VMT_RMG_800by600.UC_DisconnectPopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickOneButton)
            {
                PresentationMgr.APP_CloseApp();
            }
        }
        #endregion [NotifyKeepAlive]

        #region [NotifyMessage]
        public void NotifyMessage(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive value)
        {
            LogWin.WriteLog("VMT Debug ==> " + "NotifyMessage Sussess ");

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive>(value);

            TimeSpan timeout = new TimeSpan(0, 0, 3); // 3 sec
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, timeout,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Message", clone.m_strMessage, "OK", null, 0);
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

            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.GroupListSeperator));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);
                            this.UC_LogInView.ProcessByGetAccessRoleCallback(clone);

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
                            this.UC_LogInView.ProcessByLogin4MachineCallback(clone);

                        }));
        }
        #endregion [NotifyLogin4Machine]

        //#region [NotifyLogIn]
        //public void NotifyLogIn(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value)
        //{
        //    // Debug.WriteLine("NotifyLogIn Sussess !!! bLogin:" + value.bLogin + " MchnID:" + value.MchnID);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive>(value);

        //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(delegate
        //                {
        //                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

        //                    this.HideProgressBar(0);
        //                    this.UC_LogInView.ProcessByLogInCallback(clone);
        //                }));
        //}
        //#endregion [NotifyLogIn]

        #region [NotifyMachineStatusChanged]
        public void NotifyMachineStatusChanged(ref VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive>(value);

            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, Convert.ToString(value.m_iResult)));

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

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(delegate
                      {
                          InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_JobOrderList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                          PresentationMgr.Singleton.JOB_Clear();

                          PresentationMgr.Singleton.ProcessingJobKey = String.Empty;
                          // Add JobOrder Data Structure
                          foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in clone.JobOrder)
                          {
                              if (String.IsNullOrEmpty(jobOrder.jobKey))
                                  continue;//jobOrder.jobKey = System.Guid.NewGuid().ToString();

                              if (jobOrder.type.jobStatus.Equals("P"))
                                  PresentationMgr.Singleton.ProcessingJobKey = jobOrder.jobKey;
                              PresentationMgr.Singleton.JOB_Add(jobOrder);
                          }
                          PresentationMgr.Singleton.JOB_Sort();

                          JobList jobList = PresentationMgr.MainView.UC_JobList;
                          //PresentationMgr.Singleton.JL_SearchNRefresh(jobList, PresentationMgr.Singleton.SelectBlckName, jobList.CurrentPageIndex);
                          PresentationMgr.Singleton.JL_Refresh(jobList, jobList.CurrentPageIndex, true);

                          //PresentationMgr.AppWin.HideProgressBar(0);
                          //PresentationMgr.Singleton.JBN_AutoTargetJob();
                          clone = null;
                      }));
        }
        #endregion [NotifyJobOrderRMG]


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

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();

                            if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                                PresentationMgr.MainView.UC_JobList.CheckBox_Refresh.IsChecked = true;
                            //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                                PresentationMgr.Singleton.ThreadTimerStart(true);

                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive>(
                                System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            this.HideProgressBar(0);

                            if (!clone.resultTP.Equals("SUCCESS"))
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    "Fail", PresentationMgr.GetJobDoneErrorMessage(clone.resultObj), "OK", null, 0);
                            }
                            clone = null;
                        }));
        }
        #endregion [NotifyJobDone]

        #region [NotifyJobReOnChassis]
        public void NotifyJobReOnChassis(Boolean value)
        {
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));
            InterfaceMessageLoader.instance().WriteInterfaceMessage<bool?>(System.Reflection.MethodBase.GetCurrentMethod().Name, (bool?)value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();
                            PresentationMgr.Singleton.ThreadTimerStart(true);
                            if (!value)
                            {
                                //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "SetJobReOnChassis", "Failed!", "OK", null, 0);
                            }
                        }));
        }
        #endregion [NotifyJobReOnChassis]

        #region [NotifyMachineStopCodeList]
        public void NotifyMachineStopCodeList(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.m_pData.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);
                            PresentationMgr.MainView.UC_AvailableView.MakeupAvailableItems(clone);
                            //PresentationMgr.AppWin.UC_MainView.UC_AvailableView.ProcessByMachineStopCodeList(clone); // TODO
                        }));
        }
        #endregion [NotifyMachineStopCodeList]

        #region [NotifyGetMachineStop]
        public void NotifyGetMachineStop(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value)
        {
            //LogMessage("NotifyGetMachineStop => value : " + value.ToString());

            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive>(value);
            //LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value == null? String.Empty : value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (clone != null && (
                                PresentationMgr.MainView.UC_BreakTimeView.Waiting != BreakTimeView.WaitingType.NONE || 
                                PresentationMgr.MainView.UC_BreakTimeView.Visibility == Visibility.Hidden)
                                )
                                PresentationMgr.ApplyGetMachineStop(clone);

                            if (PresentationMgr.MainView.UC_BreakTimeView.Waiting != BreakTimeView.WaitingType.NONE)
                            {
                                if (clone != null &&
                                    PresentationMgr.MainView.UC_BreakTimeView.Waiting == BreakTimeView.WaitingType.SET)
                                {
                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                        "Notice", String.Format("{0} has been set already", clone.Data.ReasonNm), "OK", null, 0);
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

                                    PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);
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

                            if (clone != null)
                                clone.Dispose();
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
                                this.HideProgressBar(0);
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Log-in Fail", value, "OK", null, 0);
                            }
                        }));
        }
        #endregion [NotifyErrorCode]

        #region [NotifyBlockList]
        public void NotifyBlockList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value == null ? "null" : value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            //InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.AddBlockBayInfo(clone);
                            PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;

                            clone.Dispose();
                            //this.HideProgressBar(0);

                        }));
        }
        #endregion [NotifyBlockList]

        #region [NotifyBlockMapList]
        public void NotifyBlockMapList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            var clone = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.AddBlockBayInfo(clone);

                            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerSearch)
                            {
                                this.HideProgressBar(0);

                                PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = true;

                                if (clone.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) &&
                                    clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay != null)//.Count > 0)
                                {
                                    if (clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual != true)
                                    {
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "GetNoWorkArea_Ask"), PresentationMgr.Singleton.CurrentBlock);
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                                    }

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
                                            pos.m_cBay = bayList.First().BayName;
                                            PresentationMgr.Singleton.CurrentPostion = pos;
                                        }
                                    }
                                    else if (!clone.DicBlock[PresentationMgr.Singleton.CurrentBlock].IsVirtual)
                                    {
                                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                        , "GetNoWorkTier_Ask"), PresentationMgr.Singleton.CurrentBlock + "" + PresentationMgr.Singleton.CurrentBay);
                                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);

                                        PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
                                    }

                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.CheckBlockLocation();
                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.CheckBayLocation();
                                }

                                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                                    PresentationMgr.MainView.UC_BaySelectionView.MakeupBayItems();
                            }

                            clone.Dispose();
                        }));
        }
        #endregion [NotifyBlockMapList]

        #region [NotifyGetMahchineList]
        public void NotifyGetMahchineList(ref RMG.VD_RMG_PartnerMachineList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_PartnerMachineList>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            DataMgr.Singleton.List_MachineofPool = clone;
                            PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
                            this.HideProgressBar(0);
                        }));
        }
        #endregion [NotifyGetMahchineList]

        #region [NotifyGetMahchineListofPool]
        public void NotifyGetMahchineListofPool(ref RMG.VD_RMG_PartnerMachineList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_PartnerMachineList>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

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
            foreach (var machine in value.MachineList)
            {
                if (RMG.RMG_User.gMchnID.Equals(machine.mchnId) && RMG.RMG_User.gMchnTp.Equals(machine.mchnTp))
                {
                    if (machine.loginUsrLst != null &&
                        !machine.loginUsrLst.Contains(RMG.RMG_User.gUserID) &&
                        //if (machine.isLogOn == false &&
                        (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.LogInView &&
                        PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MachineSettingView))
                    {
                        Logger.Log("Received Force Logout : " +
                            "mchnId =" + machine.mchnId + ", mchnTp=" + machine.mchnTp +
                            ", isLogOn=" + machine.isLogOn.ToString() + ", isOn=" + machine.isOn.ToString() +
                            ", mchnSts=" + machine.mchnSts + ", vrtlFlg=" + machine.vrtlFlg);

                        InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_MachineList_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, value);

                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate
                            {
                                PresentationMgr.AppWin.UC_LogInView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                                VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                                VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);

                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Logout, "Log Off", "Force Logoff been processed.", "OK", null, 3);
                            }
                                ));
                    }
                    else if (!String.IsNullOrEmpty(machine.noticeMsg))
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

                    break;
                }
            }
        }
        #endregion [NotifyMahchineLogoutCheck]

        #region [NotifyGetJobOrderByContainer]
        public void NotifyGetJobOrderByContainer(ref RMG.VD_RMG_JobOrderList value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_JobOrderList>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.Count.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            PresentationMgr.MainView.UC_ContainerSearchView.SetSearchedJobList(clone);
                            this.HideProgressBar(0);
                        }));
        }
        #endregion [NotifyGetJobOrderByContainer]

        #region [NotifySetDetwinJob]
        public void NotifySetDetwinJob(Boolean value)
        {
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));
            InterfaceMessageLoader.instance().WriteInterfaceMessage<bool?>(System.Reflection.MethodBase.GetCurrentMethod().Name, (bool?)value);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);

                            VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();

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

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();                            

                            if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                                PresentationMgr.MainView.UC_JobList.CheckBox_Refresh.IsChecked = true;
                            //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                                PresentationMgr.Singleton.ThreadTimerStart(true);

                            this.HideProgressBar(0);

                            if (clone.resultTP.Equals("FAIL"))
                            {
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                    "Fail", PresentationMgr.GetJobSetErrorMessage(clone.resultObj), "OK", null, 0);
                            }
                            else //if (clone.resultTP.Equals("SUCCESS"))  // AH 생성, processing/inactive 상태변경시 최상단 job 선택
                            {
                                PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
                                PresentationMgr.Singleton.NeedJobAutoSelection = true;
                            }                            

                            clone = null;
                        }));
        }
        #endregion [NotifySetJobStatus]

        #region [NotifyGetContainerInfo]
        public void NotifyGetContainerInfo(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive value)
        {
            var clone = Util.DeepCopy<RMG.VD_RMG_ContainerInfo_Receive>(value);
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

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
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

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
            LogWin.WriteLog(String.Format("[{0}] => {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, value.ToString()));

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone);

                            VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderList_Ask();

                            this.HideProgressBar(0);
                            if (!String.IsNullOrEmpty(value))
                                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Swap failed", value, "OK", null, 0);
                            else
                            {
                                if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerSearch)
                                    PresentationMgr.MainView.UC_ContainerSearchView.RefreshSearchResult();
                            }
                        }));
        }
        #endregion [NotifyDoSwap4Manual]

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

        #endregion [Callback Delegate Functions - RMG]


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
        //                    YardView yardView = PresentationMgr.Singleton.GetUIComponent("VMT_RMG_800by600.YardView") as YardView;
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
        //                        PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.Moving);

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

    }
}