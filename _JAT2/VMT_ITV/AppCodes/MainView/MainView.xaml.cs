using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using System.Reflection;
using System.Net;

//20190108
using Common.Interface;
using HessianComm;
using System.Linq;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for MainWnd.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        #region [variables]
        // public VMT_DataMgr.EEv2JobOrder[] gJobOrderList;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder gTestOderJob;

        public ArrayList gJobOrderList;
        public String gBeforeMchnTp = "";

        private int miFuelGage = 0;
        private ArrayList mAvaliableList;
        private ArrayList mAvaliableUIList;

        private MainWindow mMainWindow = null;

        private List<StringBuilder> unLinkList = new List<StringBuilder>(), unLinkPopupList = new List<StringBuilder>(), linkList = new List<StringBuilder>();
        public VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder = new VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder();

        public bool mIsDay = true;

        public static string arrivalCancel = "N"; // Y: Cancel - N: Active
        public bool isFinishAvailable = false;
        
        private String[][] gAniContainerImages = new String[2][];
        private Image[] gContainerImage = new Image[2];
        private TextBlock[] gContainerIDTextBlock = new TextBlock[2];
        private ImageSource[] mContainerImagePathforJobDone = new ImageSource[2]; // job done 시 상차일때 바꿀 컨테이너 이미지를 미리 저장한다.
        private Image[] mContainerImageforJobDone = new Image[2];
        private int countDoneTwinJob = 0;
        private bool isTwinJob = false;
        private bool btnDoneBeep = false;
        private bool btnLDCmplBeep = false;
        public String ycNoFForLogging = "";
        public String ycNoAForLogging = "";
        public String cntrANoForLogging = "";
        public String cntrFNoForLogging = "";
        public static LogWindow LogWin = null;
        public Boolean isDiffType;
        public Boolean popupShowed = false;

        VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive receivedAvailable = null;

        //private const int COUNT_AVAILABLE_BY_UI = 3;
        private const int COUNT_AVAILABLE_BY_UI = 2; // Chassis Select Delete
        private String[] mAvailable_default_message ={PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0106", LanguageService.LABEL_CUSTOMIZE),
                                                        //"Change Chassis",
                                                        PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0107", LanguageService.LABEL_CUSTOMIZE),
                                                        //"Oiling",
                                                        //"Toilet",
                                                        //"Punk",
                                                        //"Accident",
                                                        //"OOG",
                                                        //"ITV Breakdown",
                                                        //"GC Breakdown",
                                                        //"YC Breakdown"
                                                        };
        private long AvaliableStartTime = 0;
        private const String SCREENMODE_DESCRIPTION_DAY = "This mode is optimized Day driving";
        private const String SCREENMODE_DESCRIPTION_NIGHT = "This mode is optimized Night driving";

        public enum BreakModeType
        {
            None,
            WaitingConfirm,
            BreakDown,
        }
        public BreakModeType mCurrentBreak = BreakModeType.None;

        public enum BreakWaitingType
        {
            None = 0,
            Set,
            Unset,
        }
        public BreakWaitingType WaitingBreakRequest = BreakWaitingType.None;

        private LinkedList<ITV.VD_ITV_PlanSeq> SVMT_PlanSeqList = new LinkedList<ITV.VD_ITV_PlanSeq>();
        private bool isPrecedingYtListPolling = false;
        private string armgReadFlg = "";

        private List<String> _movingVehicleList = new List<String> { "TC", // RMGC
                                                                     "FL", // Folk Lift
                                                                     "RS", // Reach Stacker
                                                                     //"GC", // STS Crane
                                                                     "SC", // Staddle Carrier
                                                                     //"YT", // Yard Tractor
                                                                     "HC", // Harbour Crane
                                                                     "VC", // Virtual Crane
                                                                     "TH", // Top Handler
                                                                     "RR", // Rail Crane
                                                                     //"QC"  // Quay Crane
                                                                    };

        ITV.VD_ITV_JobOrderList JobOrderForITV_Current = new ITV.VD_ITV_JobOrderList();

        ITV.VD_ITV_JobOrderList JobOrderForITV_Prev = new ITV.VD_ITV_JobOrderList();

        VMT_Data_JAT2.Objects.Common.VmtMachine machineStatusChanged_Current = new VMT_Data_JAT2.Objects.Common.VmtMachine();
        VMT_Data_JAT2.Objects.Common.VmtMachine machineStatusChanged_PRV = new VMT_Data_JAT2.Objects.Common.VmtMachine();

        ITV.VD_ITV_JobOrderList standAlone_job = new ITV.VD_ITV_JobOrderList();

        VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _selectedContainer = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder(); //20190807


        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder selectedContainer
        {
            get { return _selectedContainer; }
            set
            {
                _selectedContainer = value;
                String jobKeySelected = (value == null ? "" : value.jobKey);
                if (jobKeySelected != VMT_Data_JAT2.Objects.ITV.ITV_User.jobKey)
                {
                    VMT_DataMgr_Common.EndPolling_Ask(HessianCommType.GetMachineJobByKeys_New);
                    VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineJobByKeys_New);

                    VMT_Data_JAT2.Objects.ITV.ITV_User.jobKey = jobKeySelected;
                    VMT_DataMgr_Common.EndPolling_Ask(HessianCommType.GetMachineStatusChanged);
                    VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineStatusChanged);

                }
            }
        }

        public System.Windows.Media.SolidColorBrush colorDisable = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA6A4A4")); // Color button disable
        public System.Windows.Media.SolidColorBrush colorEnable = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF333333")); // Color button enable
        private System.Windows.Media.SolidColorBrush color_lightYellow = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF7F")); 
        private System.Windows.Media.SolidColorBrush color_gray = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDFDFDF")); 

        private int previous2MOJobs = 0, previousMOMIJobs = 0, previous2LDUncompleteJobs = 0, previous1LDCompletedJobs = 0, tenTimesBeepLDCMPL = 0;

        #endregion [variables]

        public MainView()
        {
            this.InitializeComponent();
            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);
            // Not CallBack ProcessByAvailable
            if (MainWindow.TEST_MODE)
            {
                miFuelGage = 80; // 320 * 0.25
            }


            AvailablePopupView.Init(this);
            BreakPopupView.Init(this);
            DayNightPopupView.Init(this);
            ChassisChangeView.Init(this);
            MainWindow.LogWin = new LogWindow();
        }

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;

            if (MainWindow.SERVICE_COMPANY.Equals("BTP")) { }

            gJobOrderList = new ArrayList();
            SetNoJob();
            SetAvailListUI();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DisableButton();
            InitEvent();
            LoadLanguage();

            CheckContainerToBeefTimer();
        }
        private void CheckContainerToBeefTimer()
        {
            DispatcherTimer playBeefTimer = new DispatcherTimer();
            playBeefTimer.Tick += CheckContainerToPlayBeef;
            playBeefTimer.Interval = new TimeSpan(0, 0, 0, 0, 10000);
            playBeefTimer.Start();
        }
      
        //CheckContainerToPlayDingDong
        private void CheckContainerToPlayBeef(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible && JobOrderForITV_Current.Count > 0)
                if (((JobOrderForITV_Current.FirstJob.cntr.cntrSpTp == "OOG" && JobOrderForITV_Current.FirstJob.workingMchn.mchnSts == "L") ||
                    (JobOrderForITV_Current.SecondJob.cntr.cntrSpTp == "OOG" && JobOrderForITV_Current.SecondJob.workingMchn.mchnSts == "L"))
                    && !popupShowed)
                {
                    mMainWindow.BeefPlay_dingdong();
                    String message = PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0052", LanguageService.MESSAGE_GROUP);
                    String lang = mMainWindow.MachineInfoView.langString;
                    if (lang.Contains("Korea"))
                        message = message.Split('.')[0] + "." + Environment.NewLine + message.Split('.')[1] + "." + Environment.NewLine + message.Split('.')[2] + ".";
                    else
                        message = message.Split('.')[0] + "." + Environment.NewLine + message.Split('.')[1] + ".";
                    mMainWindow.PopupView.ShowPopup(1,
                                   PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0136", LanguageService.LABEL_CUSTOMIZE), message
                                   , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 2, 32);
                }
        }
        private void DisableButton()
        {
            TextBlock_Arrival.IsEnabled = false;
            TextBlock_Arrival.Background = colorDisable;            
            TextBlock_Done.IsEnabled = false;
            TextBlock_Done.Background = colorDisable;
        }
        private void DisableField(string pos)
        {
            if (pos.Equals("F"))
            {
                txt_cntrNoF.Background = Brushes.LightGray;
                txt_fmLocationF.Background = Brushes.LightGray;
                txt_toLocationF.Background = Brushes.LightGray;
                txt_cntrWgtF.Background = Brushes.LightGray;
                Lbl_HoldValF.Background = Brushes.LightGray;
                Lbl_LaneValF.Background = Brushes.LightGray;
                txt_ycNoF.Background = Brushes.LightGray;
                txt_vslHoldDeckF.Background = Brushes.LightGray;
                txt_cntrLen_cntrTp_F.Background = Brushes.LightGray;
            }else if (pos.Equals("A"))
            {
                txt_cntrNoA.Background = Brushes.LightGray;
                txt_fmLocationA.Background = Brushes.LightGray;
                txt_toLocationA.Background = Brushes.LightGray;
                txt_cntrWgtA.Background = Brushes.LightGray;
                Lbl_HoldValA.Background = Brushes.LightGray;
                Lbl_LaneValA.Background = Brushes.LightGray;
                txt_ycNoA.Background = Brushes.LightGray;
                txt_vslHoldDeckA.Background = Brushes.LightGray;
                txt_cntrLen_cntrTp_A.Background = Brushes.LightGray;
            }

        }

        private void EnableField()
        {
            //if (pos.Equals("F"))
            {
                txt_cntrNoF.Background = Brushes.White;
                txt_fmLocationF.Background = Brushes.White;
                txt_toLocationF.Background = Brushes.White;
                txt_cntrWgtF.Background = Brushes.White;
                Lbl_HoldValF.Background = Brushes.White;
                Lbl_LaneValF.Background = Brushes.White;
                txt_ycNoF.Background = Brushes.White;
                txt_vslHoldDeckF.Background = Brushes.White;
                txt_cntrLen_cntrTp_F.Background = Brushes.White;
            }
            //else if (pos.Equals("A"))
            {
                txt_cntrNoA.Background = Brushes.White;
                txt_fmLocationA.Background = Brushes.White;
                txt_toLocationA.Background = Brushes.White;
                txt_cntrWgtA.Background = Brushes.White;
                Lbl_HoldValA.Background = Brushes.White;
                Lbl_LaneValA.Background = Brushes.White;
                txt_ycNoA.Background = Brushes.White;
                txt_vslHoldDeckA.Background = Brushes.White;
                txt_cntrLen_cntrTp_A.Background = Brushes.White;
            }
        }

        private void InitEvent()
        {
            this.Grid_Fore.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Fore_MouseLeftButtonDown);
            this.Grid_After.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_After_MouseLeftButtonDown);
        }
        private void LoadLanguage()
        {
            this.lbl_Hatch_Job.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0024", LanguageService.LABEL_MAINWINDOW);
            this.TextBlock_Arrival.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_MAINWINDOW);
            this.TextBlock_Done.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0029", LanguageService.LABEL_MAINWINDOW);
            this.TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
            this.TextBlock_ChangeDriver.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_CHANGEDRIVER);
            this.lbl_positionF.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0016", LanguageService.LABEL_MAINWINDOW);
            this.lbl_positionA.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0017", LanguageService.LABEL_MAINWINDOW);
            this.Lbl_CurJob.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0074", LanguageService.LABEL_MAINWINDOW);
            this.Lbl_NextJob.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0075", LanguageService.LABEL_MAINWINDOW);
            this.Lbl_Weight.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0076", LanguageService.LABEL_MAINWINDOW);
        }
        private void SetAvailListUI()
        {
            mAvaliableList = new ArrayList();
            mAvaliableUIList = new ArrayList();

            for (int i = 0; i < mAvailable_default_message.Length; i++)
            {
                mAvaliableList.Add(mAvailable_default_message[i]);

                AvailablePopupView availableItem = new AvailablePopupView();
                availableItem.SetJobInfo(mAvailable_default_message[i], mAvailable_default_message[i]);
                mAvaliableUIList.Add(availableItem);
            }
        }

        private String GetDestination(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob)
        {
            String retValue = String.Empty;
            VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location loc = currentJob.locWorking;
            if (!string.IsNullOrEmpty(currentJob.locWorking.location))
            {
                if (currentJob.partnerMchn.mchnTp == "QC")
                    retValue = (loc.location.Split('-').Length >= 3 && loc.location.Split('-')[1] != "" && loc.location.Split('-')[2] != "") ? loc.location : loc.location.Split('-')[0];
                else
                    retValue = loc.blck + (string.IsNullOrEmpty(loc.bay) ? "" : "-" + loc.bay + "-" + loc.row) ;

                return retValue;
            }
            else
                return "";

            ////COMMENT BELOW CONTENT
            if (currentJob.partnerMchn.mchnTp.Equals("QC")) // STS
            {
                {
                    Int32 num = 0;
                    if (Int32.TryParse(currentJob.locWorking.location, out num) == true)
                        retValue = currentJob.partnerMchn.mchnId + "-" + currentJob.locWorking.location;
                    else
                        retValue = currentJob.partnerMchn.mchnId;
                }
            }
            else
            {
                if (currentJob.locWorking.locTp.Length > 0)
                {
                    if (currentJob.locWorking.bay.Equals(""))
                        retValue = currentJob.locWorking.blck;
                    else
                    {
                        if (!string.IsNullOrEmpty(currentJob.workingMchn.aprchLn))
                            retValue = currentJob.locWorking.blck + "-" + currentJob.locWorking.bay + "-" + PresentationMgr.GetMatchedAprchLn(currentJob.workingMchn.aprchLn);
                        else
                            retValue = currentJob.locWorking.blck + "-" + currentJob.locWorking.bay;
                    }
                }
                else
                {
                    retValue = currentJob.locWorking.blck + "-" + currentJob.locWorking.bay;
                }

            }

            return retValue;
        }

        private void ProgressJob()
        {
            ReFlash();

            if (gJobOrderList.Count < 1)
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[0];
        }

        public void CallBack_Popup_LogOff(int seleted)
        {
            switch (seleted)
            {
                case 0: // left cancel
                    TextBlock_ChangeDriver.Background = colorEnable;
                    break;
                case 1: // center system off
                    {
                    }
                    break;
                case 2: // right log out
                    {
                        TextBlock_ID.Text = "";
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, PresentationMgr.AppWin.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? PresentationMgr.AppWin.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "");
                        mMainWindow.IndicatorView.Image_close.Visibility = Visibility.Visible;
                        mMainWindow.IndicatorView.Image_logout.Visibility = Visibility.Hidden;
                        //mMainWindow.IndicatorView.sliderColor.Visibility = Visibility.Visible;
                        mMainWindow.LoginView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                        mMainWindow.LoginView.ReFlash();
                        mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content = UserInfo.gMchnID;
                        mMainWindow.IndicatorView.Lbl_User_Val.Content = String.Empty;
                        mMainWindow.IndicatorView.Image_Setting.Visibility = Visibility.Visible;

                        ReFlash();

                        TextBlock_ChangeDriver.Background = colorEnable;

                        mMainWindow.LogOut(this);
                    }
                    break;
            }
        }

        public void SaveLog(string sJob)
        {
            try
            {
                string sRootPath = AppCfgMgr.GetAppDirectory();
                string sDirPath = sRootPath + @"{0}\Log\"
                    + System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day;

                if (Directory.Exists(sDirPath) == false)
                {
                    Directory.CreateDirectory(sDirPath);
                }

                string logFilePath = @sDirPath + "/ITV_LOG_" + System.DateTime.Now.Hour + ".txt";

                FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine("//===========================================================================");
                sw.WriteLine("[" + System.DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "] " + sJob);
                sw.WriteLine("//===========================================================================\r\n");
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SetNoJob(string pos = "")
        {
            if(pos == "")
            {
                lbl_SingleFlgF.Content = "";
                lbl_SingleFlgF.Visibility = Visibility.Hidden;
                lbl_SingleFlgA.Content = "";
                lbl_SingleFlgA.Visibility = Visibility.Hidden;

                this.txt_cntrNoF.Content = "";
                this.txt_toLocationF.Content = "";
                this.txt_fmLocationF.Content = "";
                this.txt_cntrWgtF.Content = "";
                this.txt_ycNoF.Content = "";
                this.txt_vslHoldDeckF.Content = "";
                this.txt_cntrLen_cntrTp_F.Content = "";
                this.txt_fmLocationF.Background = Brushes.White;
                this.txt_ycNoF.Background = Brushes.White;
                this.txt_toLocationF.Background = Brushes.White;

                this.txt_cntrNoA.Content = "";
                this.txt_toLocationA.Content = "";
                this.txt_fmLocationA.Content = "";
                this.txt_cntrWgtA.Content = "";
                this.txt_ycNoA.Content = "";
                this.txt_vslHoldDeckA.Content = "";
                this.txt_cntrLen_cntrTp_A.Content = "";
                this.txt_fmLocationA.Background = Brushes.White;
                this.txt_ycNoA.Background = Brushes.White;
                this.txt_toLocationA.Background = Brushes.White;

                //this.txt_RfidNo.Content = "";
                //txt_RfidChk.Content = "";
                //txt_RfidChk.Background = new SolidColorBrush(Colors.Red);

                this.Lbl_HoldValF.Content = "";
                this.Lbl_LaneValF.Content = "";
                this.Lbl_HoldValA.Content = "";
                this.Lbl_LaneValA.Content = "";
                //PresentationMgr.AppWin.IndicatorView.Lbl_User_Val.Content = "";
                //PresentationMgr.AppWin.IndicatorView.Lbl_User.Content = "";

                ResetSelectedContainer();
                mMainWindow.IndicatorView.ReFlash();
            }
            else if(pos == "A")
            {
                lbl_SingleFlgA.Content = "";
                lbl_SingleFlgA.Visibility = Visibility.Hidden;
                this.txt_cntrNoA.Content = "";
                this.txt_toLocationA.Content = "";
                this.txt_toLocationA.Background = Brushes.White;
                this.txt_fmLocationA.Content = "";
                this.txt_fmLocationA.Background = Brushes.White;
                this.txt_cntrWgtA.Content = "";
                this.txt_ycNoA.Content = "";
                this.txt_ycNoA.Background = Brushes.White;
                this.txt_vslHoldDeckA.Content = "";
                this.txt_cntrLen_cntrTp_A.Content = "";
                this.Lbl_HoldValA.Content = "";
                this.Lbl_LaneValA.Content = "";

                //ResetSelectedContainer();
            }
            else if (pos == "F")
            {
                lbl_SingleFlgF.Content = "";
                lbl_SingleFlgF.Visibility = Visibility.Hidden;
                this.txt_cntrNoF.Content = "";
                this.txt_toLocationF.Content = "";
                this.txt_toLocationF.Background = Brushes.White;
                this.txt_fmLocationF.Content = "";
                this.txt_fmLocationF.Background = Brushes.White;
                this.txt_cntrWgtF.Content = "";
                this.txt_ycNoF.Content = "";
                this.txt_ycNoF.Background = Brushes.White;
                this.txt_vslHoldDeckF.Content = "";
                this.txt_cntrLen_cntrTp_F.Content = "";
                this.Lbl_HoldValF.Content = "";
                this.Lbl_LaneValF.Content = "";

                //ResetSelectedContainer();
            }

        }

        public void ReFlash()
        {
            SetNoJob();
        }

        public void SetUserID(String id)
        {
            TextBlock_ID.Text = id;
        }

        public VMT_Data_JAT2.Objects.Common.VD_Common_Available _availableSelectData = new VMT_Data_JAT2.Objects.Common.VD_Common_Available();
        public void ShowBreak(VMT_Data_JAT2.Objects.Common.VD_Common_Available availableSelectData)
        {
            _availableSelectData = availableSelectData;

            BreakPopupView.btn_Cancel.IsEnabled = true;
            BreakPopupView.btn_Cancel.Foreground = Brushes.White;

            AvailablePopupView.Hide_Popup();
            BreakPopupView.Show_Popup();

            // VMT_Data_JAT2.Common.SVMT_AvailableSend available = new VMT_Data_JAT2.Common.SVMT_AvailableSend();
            if (mCurrentBreak == BreakModeType.None)
            {
                BreakPopupView.TextBlock_Season_Name.Text = availableSelectData.ReasonNm;

                BreakPopupView.Grid_break_Start_Time.Visibility = System.Windows.Visibility.Visible;
                BreakPopupView.Grid_break_End_Time.Visibility = System.Windows.Visibility.Hidden;

                DateTime currentServerDateTime = DateTime.Now;
                if (!String.IsNullOrEmpty(ITV.ITV_User.gServerTime))
                {
                    String currentServerTime = ITV.ITV_User.gServerTime;

                    DateTime.TryParse(currentServerTime, out currentServerDateTime);
                }

                String currentDate = currentServerDateTime.ToString("yyyy/MM/dd"); // 시간을 구합니다
                String currentTime = currentServerDateTime.ToString("hh:mm:ss"); // 시간을 구합니다

                //BreakPopupView.TextBlock_Break_Start_Date.Text = currentDate;
                //BreakPopupView.TextBlock_Break_Start_Time.Text = currentTime; ///
                BreakPopupView.btn_Complete.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0101", LanguageService.LABEL_CUSTOMIZE);
                BreakPopupView.DisableButton();

                //if (index - COUNT_AVAILABLE_BY_UI - 1 < receivedAvailable.iAvailableCount)
                //{
                //    available.Data = receivedAvailable.AData[index - COUNT_AVAILABLE_BY_UI - 1];
                //    TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                //    available.StartTime = Convert.ToInt64(ts.TotalSeconds);
                //    AvaliableStartTime = available.StartTime;

                //    VMT_Data_JAT2.Common.SendAvailalbe(ref available);
                //    TextBlock_Avaliable.Text = (String)mAvaliableList[index - COUNT_AVAILABLE_BY_UI - 1];
                //    Image_Avaliable.Source = mMainWindow.getImageByDayOrNight(@"g_left_btn_available_press.png");
                //}
            }
            else if (mCurrentBreak == BreakModeType.WaitingConfirm)
            {
                //
            }
            else if (mCurrentBreak == BreakModeType.BreakDown)
            {
                BreakPopupView.Grid_break_End_Time.Visibility = System.Windows.Visibility.Visible;

                DateTime currentServerDateTime = DateTime.Now;
                if (!String.IsNullOrEmpty(ITV.ITV_User.gServerTime))
                {
                    String currentServerTime = ITV.ITV_User.gServerTime;
                    //currentServerDateTime = Convert.ToDateTime(currentServerTime);
                    DateTime.TryParse(currentServerTime, out currentServerDateTime);
                }

                String currentDate = currentServerDateTime.ToString("yyyy/MM/dd"); // 시간을 구합니다
                String currentTime = currentServerDateTime.ToString("hh:mm:ss"); // 시간을 구합니다

                //BreakPopupView.TextBlock_Break_End_Date.Text = currentDate;
                //BreakPopupView.TextBlock_Break_End_Time.Text = currentTime; ///
                BreakPopupView.btn_Complete.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0102", LanguageService.LABEL_CUSTOMIZE);
                BreakPopupView.btn_Cancel.IsEnabled = false;
                BreakPopupView.btn_Cancel.Foreground = Brushes.Gray;

                //if (index - COUNT_AVAILABLE_BY_UI - 1 < receivedAvailable.iAvailableCount)
                //{
                //    available.Data = receivedAvailable.AData[index - COUNT_AVAILABLE_BY_UI - 1];
                //    TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                //    available.StartTime = AvaliableStartTime;
                //    available.FinishTime = Convert.ToInt64(ts.TotalSeconds);
                //    AvaliableStartTime = 0;

                //    VMT_Data_JAT2.Common.SendAvailalbe(ref available);
                //    //TextBlock_Avaliable.Text = "Avaliable";
                //    //Image_Avaliable.Source = mMainWindow.getImageByDayOrNight(@"g_left_btn_available_default.png");
                //}
            }
            if (JobOrderForITV_Current.Count > 0)
            {
                if (JobOrderForITV_Current.FirstJob.cntr.cntrNo.Length > 0 && BreakPopupView.btn_Complete.Content.ToString() == PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0102", LanguageService.LABEL_CUSTOMIZE))
                {
                    BreakPopupView.CheckButton(JobOrderForITV_Current.FirstJob);
                }
                else if (JobOrderForITV_Current.Count > 1 && JobOrderForITV_Current.SecondJob.cntr.cntrNo.Length > 0 && BreakPopupView.btn_Complete.Content.ToString() == PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0102", LanguageService.LABEL_CUSTOMIZE))
                {
                    BreakPopupView.CheckButton(JobOrderForITV_Current.SecondJob);
                }
            }
        }

        public void HideBreakView()
        {
            AvailablePopupView.Show_Popup();
            BreakPopupView.Hide_Popup();
        }

        public void ShowScreenMode()
        {
            AvailablePopupView.Hide_Popup();
            DayNightPopupView.Show_Popup();

            if (mMainWindow.gIsDay)
            {
                DayNightPopupView.Image_dayView.Source = mMainWindow.getImageByDayOrNight(@"main_available_img_screen_day.png");
                DayNightPopupView.TextBlock_Day.Background = Brushes.Red;
                DayNightPopupView.TextBlock_Night.Background = TextBlock_StopPage.Background = colorEnable;
                //TextBlock_Description.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0123", LanguageService.LABEL_CUSTOMIZE);
            }
            else
            {
                DayNightPopupView.Image_dayView.Source = mMainWindow.getImageByDayOrNight(@"main_available_img_screen_night.png");
                DayNightPopupView.TextBlock_Night.Background = Brushes.Red;
                DayNightPopupView.TextBlock_Day.Background = TextBlock_StopPage.Background = colorEnable;
                //TextBlock_Description.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0124", LanguageService.LABEL_CUSTOMIZE);
            }
        }

        public void HideScreenMode()
        {
            AvailablePopupView.Show_Popup();
            DayNightPopupView.Hide_Popup();
        }

        public void HideAvaliableView()
        {
            Grid_Main.Visibility = System.Windows.Visibility.Visible;
            AvailablePopupView.Visibility = System.Windows.Visibility.Collapsed;
        }

        #region [Process Callback]
        ///////////////////////////////////////////
        //------------------Process Callback
        ///////////////////////////////////////////

        #region [Config Callback]
        public void ProcessByConfigCallback(VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive value)
        {
            //this.TextBlock_LdCmpl.IsEnabled = value.Arrival;
            //this.TextBlock_Ready.IsEnabled = value.Ready;
            //this.TextBlock_Done.IsEnabled = value.Done;
        }
        public void ProcessConfigValue(String configValue)
        {
            if ("Y".Equals(configValue))
            {
                Grid_Button_ColDef_03.Width = new GridLength(3, GridUnitType.Star);
            }
            else
            {
                Grid_Button_ColDef_03.Width = new GridLength(0, GridUnitType.Star);
            }
        }
        #endregion [Config Callback]

        #region [Available Callback]
        public void ProcessByMachineStopCodeList(VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive value)
        {
            int i;
            //mfLimitSpeed = value.fLimitSpeed;
            //miFuelGage = value.iFuelGage;

            mAvaliableList.Clear();
            mAvaliableUIList.Clear();
            //Wrap_Available.Children.Clear();

            receivedAvailable = value;

            //// ADD Default
            //for (i = 0; i < mAvailable_default_message.Length; i++)
            //{
            //    AvailablePopupView availableItem = new AvailablePopupView();
            //    availableItem.SetJobInfo(mAvailable_default_message[i], mAvailable_default_message[i]);
            //    mAvaliableList.Add(availableItem);
            //}

            // ADD Receive Value
            for (i = 0; i < value.m_iAvailableCount; i++)
            {
                AvailablePopupView availableItem = new AvailablePopupView();
                availableItem.SetJobInfo(value.m_pData[i].ReasonNm, value.m_pData[i].ReasonCd);
                mAvaliableList.Add(availableItem);
            }
            AvailablePopupView.setStopCodeList(mAvaliableList);
        }

        public void ProcessByGetMachineStopCallback(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value)
        {
            String ReasonCd = value.Data.ReasonCd;
            String ReasonNm = value.Data.ReasonNm;
            String MchnStopDt = value.StartTime.ToString();

            if (receivedAvailable.m_pData != null)
            {
                for (int i = 0; i < receivedAvailable.m_pData.Count; i++)
                {
                    if (receivedAvailable.m_pData[i].ReasonCd.Equals(ReasonCd))
                    {
                        _availableSelectData.ReasonCd = ReasonCd;
                        _availableSelectData.ReasonNm = ReasonNm;
                        break;
                    }
                }
            }

            AvailablePopupView.Hide_Popup();
            mCurrentBreak = BreakModeType.BreakDown;
            TextBlock_StopPage.Text = ReasonNm;
            TextBlock_StopPage.Background = Brushes.Red;

            // 20190703 if had start -> show Break UI
            VMT_Data_JAT2.Objects.Common.VD_Common_Available availableSelectData = new VMT_Data_JAT2.Objects.Common.VD_Common_Available();
            availableSelectData.ReasonNm = ReasonNm;
            availableSelectData.ReasonCd = ReasonCd;

            this.ShowBreak(availableSelectData);
            this.TextBlock_StopPage.Text = ReasonNm;
            BreakPopupView.TextBlock_Season_Name.Text = ReasonNm;

            BreakPopupView.TextBlock_Break_Start_Date.Text =
                   DateTime.ParseExact(Convert.ToString(value.StartTime), "yyyyMMddHHmmss", null).ToString("yyyy/MM/dd HH:mm:ss");
            //

            DateTime dtAvailable = DateTime.Now;
            if (MchnStopDt.Length == 14)
            {
                String strYear = MchnStopDt.Substring(0, 4);
                String strMonth = MchnStopDt.Substring(4, 2);
                String strDay = MchnStopDt.Substring(6, 2);

                String strHour = MchnStopDt.Substring(8, 2);
                String strMin = MchnStopDt.Substring(10, 2);
                String strSec = MchnStopDt.Substring(12, 2);

                dtAvailable = new DateTime(Convert.ToInt32(strYear),
                                Convert.ToInt32(strMonth),
                                Convert.ToInt32(strDay),
                                Convert.ToInt32(strHour),
                                Convert.ToInt32(strMin),
                                Convert.ToInt32(strSec));

            }

            String currentDate = dtAvailable.ToString("yyyy/MM/dd");
            String currentTime = dtAvailable.ToString("hh:mm");
            
            //BreakPopupView.TextBlock_Break_Start_Date.Text = currentDate;
            //BreakPopupView.TextBlock_Break_Start_Time.Text = currentTime; ///
        }

        public void ProcessBySetMachineStopCallback(Object value = null)
        {
            if (mCurrentBreak == BreakModeType.None)
            {
                //Grid_break_End_Time.Visibility = System.Windows.Visibility.Visible;
                //string currentDate = DateTime.Now.ToString("yyyy/MM/dd"); // 시간을 구합니다
                //string currentTime = DateTime.Now.ToString("hh:mm"); // 시간을 구합니다

                //TextBlock_Break_End_Date.Text = currentDate;
                //TextBlock_Break_End_Time.Text = currentTime;
            }
            else if (mCurrentBreak == BreakModeType.WaitingConfirm)
            {
                // 
            }
            else if (mCurrentBreak == BreakModeType.BreakDown)
            {
                BreakPopupView.TextBlock_Season_Name.Text = _availableSelectData.ReasonNm;
                //Grid_break_Start_Time.Visibility = System.Windows.Visibility.Visible;
                //Grid_break_End_Time.Visibility = System.Windows.Visibility.Hidden;
                //string currentDate = DateTime.Now.ToString("yyyy/MM/dd"); // 시간을 구합니다
                //string currentTime = DateTime.Now.ToString("hh:mm"); // 시간을 구합니다

                //TextBlock_Break_Start_Date.Text = currentDate;
                //TextBlock_Break_Start_Time.Text = currentTime;
            }
        }

        private DispatcherTimer TimerMachineStopConfirm = null;
        public void ProcessBySetMachineStopConfirm()
        {
            if (AppSettings.Local.ReasonCd == null ||
                AppSettings.Local.ReasonCd == String.Empty)
                return;

            mCurrentBreak = BreakModeType.WaitingConfirm;

            _availableSelectData.ReasonCd = AppSettings.Local.ReasonCd;
            _availableSelectData.ReasonNm = AppSettings.Local.ReasonNm;
            long.TryParse(AppSettings.Local.StartTime, out AvaliableStartTime);

            {
                // 2015-07-10 김희준 수석 요청
                //VMT_Data_JAT2.Common.SVMT_AvailableSend available = new VMT_Data_JAT2.Common.SVMT_AvailableSend();
                //available.Data.ReasonCd = ReasonCd;
                //available.Data.ReasonNm = ReasonNm;
                //available.StartTime = Convert.ToInt64(MchnStopDt);
                //_availableSelectData = available.Data;
                //VMT_Data_JAT2.Common.SendAvailalbe(ref available);
            }

            TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0109", LanguageService.LABEL_CUSTOMIZE);
            TextBlock_StopPage.Background = Brushes.Red;

            if (UserInfo.IsUseTCP)
            {
                if (TimerMachineStopConfirm == null)
                {
                    this.TimerMachineStopConfirm = new DispatcherTimer();
                    this.TimerMachineStopConfirm.Interval = new TimeSpan(0, 2, 0); // 2 min                
                    this.TimerMachineStopConfirm.Tick += new EventHandler(TimerMachineStopConfirm_Handler);
                }
                TimerMachineStopConfirm.Start();
            }
        }

        private void TimerMachineStopConfirm_Handler(object sender, EventArgs e)
        {
            if (this.Grid_BreakConfirmCancel.Visibility != System.Windows.Visibility.Visible)
                this.Grid_BreakConfirmCancel.Visibility = System.Windows.Visibility.Visible;

            if (TimerMachineStopConfirm != null)
                TimerMachineStopConfirm.Stop();
        }

        #endregion [Available Callback]

        private static string GetIniDirectory()
        {
            string path = null;

            DirectoryInfo dInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            string strParentPath = dInfo.Parent.FullName;
            string strCommonPath = strParentPath + @"\Common";

            if (!Directory.Exists(strCommonPath))
            {
                Directory.CreateDirectory(strCommonPath);
            }

            path = strCommonPath + @"\";

            return path;
        }


        private string getLocStar(string firstLoc, string secondLoc = "")
        {
            if (!String.IsNullOrEmpty(secondLoc))
            {
                if (firstLoc.Split('-').Length >= 2 && secondLoc.Split('-').Length >= 2
                    && firstLoc.Split('-')[0] == secondLoc.Split('-')[0] 
                    && firstLoc.Split('-')[1] == secondLoc.Split('-')[1])
                {
                    return firstLoc;
                }
            }

            string val = string.Empty;

            // Show ***** logic
            //val = (firstLoc.Split('-').Length >= 2 && firstLoc.Split('-')[1] != "") ? "*****" : (firstLoc.Split('-')[0] != "" ? firstLoc.Split('-')[0] : "");

            // Show full text logic
            val = (firstLoc.Split('-').Length >= 2 && firstLoc.Split('-')[1] != "") ? firstLoc : (firstLoc.Split('-')[0] != "" ? firstLoc.Split('-')[0] : "");

            return val;
        }

        public void ProcessByJobOrderITVCallback(ITV.VD_ITV_JobOrderList value)
        {
            try
            {
                JobOrderForITV_Prev = JobOrderForITV_Current;
                JobOrderForITV_Current = value;
                EnableField();

                //Different JobTp || Job Count >= 3 || job count > 1 & have pos C
                if (
                        (JobOrderForITV_Current.Count >= 2 && JobOrderForITV_Current.FirstJob.type.jobTp != JobOrderForITV_Current.SecondJob.type.jobTp
                            && ((!JobOrderForITV_Current.FirstJob.type.jobTp.Equals("MO") && !JobOrderForITV_Current.FirstJob.type.jobTp.Equals("MI"))
                                || (!JobOrderForITV_Current.SecondJob.type.jobTp.Equals("MI") && !JobOrderForITV_Current.SecondJob.type.jobTp.Equals("MO")))
                        )
                    ||
                        (
                            JobOrderForITV_Current.FirstJob != null && !String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.jobCount) && Convert.ToInt32(JobOrderForITV_Current.FirstJob.jobCount) >= 3
                        )

                    || 
                        (
                            (JobOrderForITV_Current.FirstJob != null && !String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.jobCount) && Convert.ToInt32(JobOrderForITV_Current.FirstJob.jobCount) > 1 
                            && !JobOrderForITV_Current.FirstJob.cntr.cntrIso.StartsWith("2"))
                            ||
                            (JobOrderForITV_Current.SecondJob != null && !String.IsNullOrEmpty(JobOrderForITV_Current.SecondJob.jobCount) && Convert.ToInt32(JobOrderForITV_Current.SecondJob.jobCount) > 1 
                            && !JobOrderForITV_Current.SecondJob.cntr.cntrIso.StartsWith("2"))
                        )
                   )
                {
                    if (JobOrderForITV_Current.FirstJob.cntr.cntrNo != JobOrderForITV_Prev.FirstJob.cntr.cntrNo || JobOrderForITV_Current.SecondJob.cntr.cntrNo != JobOrderForITV_Prev.SecondJob.cntr.cntrNo)
                    {
                        PresentationMgr.AppWin.HidePopup();
                        popupShowed = false;
                    }
                    if (!popupShowed)
                    {
                        //job count > 1 & have pos C
                        if (
                                (JobOrderForITV_Current.FirstJob != null && !String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.jobCount) && Convert.ToInt32(JobOrderForITV_Current.FirstJob.jobCount) > 1
                                && !JobOrderForITV_Current.FirstJob.cntr.cntrIso.StartsWith("2"))
                                ||
                                (JobOrderForITV_Current.SecondJob != null && !String.IsNullOrEmpty(JobOrderForITV_Current.SecondJob.jobCount) && Convert.ToInt32(JobOrderForITV_Current.SecondJob.jobCount) > 1
                                && !JobOrderForITV_Current.SecondJob.cntr.cntrIso.StartsWith("2"))
                            )
                        {
                            PresentationMgr.AppWin.PopupView.ShowPopup(0,
                            PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0154", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0045", LanguageService.LABEL_POPUP)
                            , "", "", "", null, 0);

                            popupShowed = true;
                        }
                        else if (JobOrderForITV_Current.FirstJob != null && !String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.jobCount) && Convert.ToInt32(JobOrderForITV_Current.FirstJob.jobCount) >= 3) //Job Count >= 3
                        {                          
                            PresentationMgr.AppWin.PopupView.ShowPopup(0,
                            PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0154", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0044", LanguageService.LABEL_POPUP)
                            , "", "", "", null, 0);

                            popupShowed = true;
                        }
                        else //Different JobTp
                        {                           
                            PresentationMgr.AppWin.PopupView.ShowPopup(0,
                            PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0154", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0043", LanguageService.LABEL_POPUP)
                            , "", "", "", null, 0);

                            popupShowed = true;
                        }
                    }                  
                }
                else if (popupShowed)
                {
                    PresentationMgr.AppWin.HidePopup();
                    popupShowed = false;
                }

                //if (JobOrderForITV_Current.FirstJob.type.jobTp != JobOrderForITV_Current.SecondJob.type.jobTp)

                if (JobOrderForITV_Prev.FirstJob.cntr.cntrNo != JobOrderForITV_Current.FirstJob.cntr.cntrNo
                    //CNTR_NO
                    ||
                    JobOrderForITV_Prev.FirstJob.ytJbSts != JobOrderForITV_Current.FirstJob.ytJbSts
                    //YTJBSTS
                    ||
                    JobOrderForITV_Prev.FirstJob.locFrom.location != JobOrderForITV_Current.FirstJob.locFrom.location
                    //LOC FROM
                    ||
                    JobOrderForITV_Prev.FirstJob.locWorking.location != JobOrderForITV_Current.FirstJob.locWorking.location
                    // LOC TO
                    ||
                    JobOrderForITV_Prev.FirstJob.type.jobStatus != JobOrderForITV_Current.FirstJob.type.jobStatus
                    // JOB STS
                    ||
                    JobOrderForITV_Prev.FirstJob.type.jobFlagInfo != JobOrderForITV_Current.FirstJob.type.jobFlagInfo
                    // POSSITION ON CHASSIS
                    ||
                    JobOrderForITV_Prev.FirstJob.vslHoldDeck != JobOrderForITV_Current.FirstJob.vslHoldDeck
                    // vslHoldDeck
                    ||
                    JobOrderForITV_Prev.FirstJob.qcLn != JobOrderForITV_Current.FirstJob.qcLn
                    // QC LANE
                    )
                {
                    if (JobOrderForITV_Prev.FirstJob.workingMchn.mchnSts == "U" && JobOrderForITV_Current.FirstJob.workingMchn.mchnSts == "L")
                    {
                        if (JobOrderForITV_Current.FirstJob.type.jobTp == "DS")
                        {
                            mMainWindow.BeefPlay_Windows_Foreground();
                            btnDoneBeep = false;
                            btnLDCmplBeep = false;
                        }
                    }
                    else
                    {
                        mMainWindow.BeefPlay_Windows_Foreground();
                        btnDoneBeep = false;
                        btnLDCmplBeep = false;
                    }
                }

                if (
                    JobOrderForITV_Prev.SecondJob.cntr.cntrNo != JobOrderForITV_Current.SecondJob.cntr.cntrNo //CNTR_NO

                    ||
                     JobOrderForITV_Prev.SecondJob.ytJbSts != JobOrderForITV_Current.SecondJob.ytJbSts //YTJBSTS
                    ||
                     JobOrderForITV_Prev.SecondJob.locFrom.location != JobOrderForITV_Current.SecondJob.locFrom.location //LOC FROM
                    ||
                     JobOrderForITV_Prev.SecondJob.locWorking.location != JobOrderForITV_Current.SecondJob.locWorking.location // LOC TO
                    ||
                     JobOrderForITV_Prev.SecondJob.type.jobStatus != JobOrderForITV_Current.SecondJob.type.jobStatus // JOB STS
                    ||
                     JobOrderForITV_Prev.SecondJob.type.jobFlagInfo != JobOrderForITV_Current.SecondJob.type.jobFlagInfo // POSSITION ON CHASSIS
                    ||
                     JobOrderForITV_Prev.SecondJob.vslHoldDeck != JobOrderForITV_Current.SecondJob.vslHoldDeck // vslHoldDeck
                    ||
                     JobOrderForITV_Prev.SecondJob.qcLn != JobOrderForITV_Current.SecondJob.qcLn // QC LANE
                    )
                {
                    if (JobOrderForITV_Prev.SecondJob.workingMchn.mchnSts == "U" && JobOrderForITV_Current.SecondJob.workingMchn.mchnSts == "L")
                    {
                        if (JobOrderForITV_Current.SecondJob.type.jobTp == "DS")
                        {
                            mMainWindow.BeefPlay_Windows_Background();
                            btnDoneBeep = false;
                            btnLDCmplBeep = false;
                        }
                    }
                    else
                    {
                        mMainWindow.BeefPlay_Windows_Background();
                        btnDoneBeep = false;
                        btnLDCmplBeep = false;
                    }
                }

                //if(isDifferent)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();

                    if (selectedContainer != null)
                    {
                        job = selectedContainer;
                    }
                    else
                    {
                        if (value.FirstJob.cntr.cntrNo.Length > 0)
                            job = value.FirstJob;
                        else if (value.SecondJob.cntr.cntrNo.Length > 0)
                            job = value.SecondJob;
                        else
                            job = null;
                    }
                }

                #region [SaveLog]
                // for Debug Log save
                //if (value.FirstJob != null && value.FirstJob.cntr != null && !String.IsNullOrEmpty(value.FirstJob.cntr.cntrNo) )
                //{
                //    SaveLog("ProcessByJobOrderITVCallback First UI",
                //        "WorkingMchn"
                //        + "[firstJobStatus]" + value.FirstJobStatus
                //        + " [mchnId]" + value.FirstJob.workingMchn.mchnId + " [mchnTp]" + value.FirstJob.workingMchn.mchnTp
                //        + " [mchnSts]" + value.FirstJob.workingMchn.mchnSts 
                //        + "\nPartnerMchn"
                //        + " [mchnId]" + value.FirstJob.partnerMchn.mchnId + " [mchnTp]" + value.FirstJob.partnerMchn.mchnTp
                //        + " [mchnSts]" + value.FirstJob.partnerMchn.mchnSts 
                //        + "\nCntr"
                //        + " [cntrNo]" + value.FirstJob.cntr.cntrNo
                //        + "\nType"
                //        + " [jobTp]" + value.FirstJob.type.jobTp + " [jobStatus]" + value.FirstJob.type.jobStatus
                //        + " [ytJbSts]" + value.FirstJob.ytJbSts
                //    );
                //}

                //if (value.SecondJob != null && value.SecondJob.cntr != null && !String.IsNullOrEmpty(value.SecondJob.cntr.cntrNo) )
                //{
                //    SaveLog("ProcessByJobOrderCallback Second UI",
                //       "WorkingMchn"
                //       + "[secondJobStatus]" + value.SecondJobStatus
                //       + " [mchnId]" + value.SecondJob.workingMchn.mchnId + " [mchnTp]" + value.SecondJob.workingMchn.mchnTp
                //       + " [mchnSts]" + value.SecondJob.workingMchn.mchnSts
                //       + "\nPartnerMchn"
                //       + " [mchnId]" + value.SecondJob.partnerMchn.mchnId + " [mchnTp]" + value.SecondJob.partnerMchn.mchnTp
                //       + " [mchnSts]" + value.SecondJob.partnerMchn.mchnSts
                //       + "\nCntr"
                //       + " [cntrNo]" + value.SecondJob.cntr.cntrNo
                //       + "\nType"
                //       + " [jobTp]" + value.SecondJob.type.jobTp + " [jobStatus]" + value.SecondJob.type.jobStatus
                //       + " [ytJbSts]" + value.SecondJob.ytJbSts
                //   );
                //}
                // for Debug XMl Log save
                InterfaceMessageLoader.instance().WriteInterfaceMessage<ITV.VD_ITV_JobOrderList>("ProcessByJobOrderITVCallback", value);
                #endregion [SaveLog]

                countDoneTwinJob = 0; // twin job done 일때
                isTwinJob = false;
                gJobOrderList.Clear();

                //20190807
                bool followFirst = false;
                if (value.FirstJob.cntr.cntrNo.Length == 0 && value.SecondJob.cntr.cntrNo.Length == 0) //20190807
                {
                    followFirst = true;
                    ReFlash();
                    selectedContainer = null;
                }
                else
                {
                    //20190807
                    if (value.FirstJob.cntr.cntrNo.Length > 0)
                    {
                        followFirst = true;
                        gJobOrderList.Add(value.FirstJob);
                    }
                    if (value.SecondJob.cntr.cntrNo.Length > 0)
                        gJobOrderList.Add(value.SecondJob);

                    ITV.ITV_User.gJobOrderList = gJobOrderList;
                    ITV.ITV_User.gFirstJobStatus = value.FirstJobStatus;
                    ITV.ITV_User.gSecondJobStatus = value.SecondJobStatus;

                    

                    ProgressJob(); // mJobState = JOB_STATE.STATE_HAVEJOB;

                    if ((followFirst && value.FirstJobStatus.Equals("A")) || value.SecondJobStatus.Equals("A"))
                    {
                        ITV.VD_ITV_SetManualArrival_Receive arrivalvalue = new ITV.VD_ITV_SetManualArrival_Receive();
                        arrivalvalue.PartnerMchnID = "jobOrder";
                        arrivalvalue.m_bPOWIN = true;
                        ProcessByArrivalCallback(arrivalvalue);
                    }
                    else if ((followFirst && value.FirstJobStatus.Equals("R")) || value.SecondJobStatus.Equals("R"))
                    {
                        ITV.VD_ITV_SetManualReady_Receive readyvalue = new ITV.VD_ITV_SetManualReady_Receive();
                        readyvalue.PartnerMchnID = "jobOrder";
                        readyvalue.iReadyResult = 0;
                        ProcessByReadyCallback(readyvalue);
                    }

                    // jobStatus ByPass
                    if (value.FirstJob.type.jobStatus.Equals("B") || value.SecondJob.type.jobStatus.Equals("B"))
                    {
                        if (mMainWindow.ByPassPopup.Visibility != System.Windows.Visibility.Visible)
                            mMainWindow.ByPassPopup.ShowPopup(0, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0110", LanguageService.LABEL_CUSTOMIZE)
                                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0114", LanguageService.LABEL_CUSTOMIZE), "", "", "", null, 0);
                    }
                    else
                    {
                        if (mMainWindow.ByPassPopup.Visibility == System.Windows.Visibility.Visible)
                            mMainWindow.ByPassPopup.HidePopup();
                    }

                    if ((followFirst && value.FirstJob.isLink) || value.SecondJob.isLink)
                    {
                        chassOrder = followFirst ? value.FirstJob.chassisOrder : value.SecondJob.chassisOrder;
                        mMainWindow.UnLinkPopupView.chassOrder = null;
                    }
                    else
                    {
                        chassOrder = followFirst ? value.FirstJob.chassisOrder : value.SecondJob.chassisOrder;
                        mMainWindow.LinkPopupView.chassOrder = null;
                    }

                    //INDICATOR
                    //PresentationMgr.AppWin.IndicatorView.Lbl_User.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0058", LanguageService.LABEL_MAINWINDOW);
                    //PresentationMgr.AppWin.IndicatorView.Lbl_User_Val.Content = UserInfo.gUserNm;

                    //FORE CONTAINER
                    String lang = mMainWindow.MachineInfoView.langString;
                    String jobTpOptF = lang.Contains("Korea") ? value.FirstJob.jobTpKor : value.FirstJob.type.jobTp;
                    txt_cntrNoF.Content = String.IsNullOrEmpty(value.FirstJob.cntr.cntrNo) ? "" : (value.FirstJob.cntr.cntrNo + " (" + jobTpOptF + ")");

                    cntrFNoForLogging = value.FirstJob.cntr.cntrNo;

                    String holdValF = value.FirstJob.hold;
                    Lbl_HoldValF.FontSize = holdValF.Length >= 8 ? 17 : 22;
                    if (String.IsNullOrEmpty(holdValF) || "N".Equals(holdValF))
                    {
                        Lbl_HoldValF.Content = "N";
                        Lbl_HoldValF.Foreground = Brushes.Black;
                        Lbl_HoldValF.Background = Brushes.White;
                    }
                    else
                    {
                        Lbl_HoldValF.Content = holdValF;
                        Lbl_HoldValF.Foreground = Brushes.White;
                        Lbl_HoldValF.Background = Brushes.Red;
                    }

                    Lbl_LaneValF.Content = value.FirstJob.qcLn;

                    //*****START LOCATION*****
                    string toLocCntrF = GetDestination(value.FirstJob);
                    string fmLocCntrF = value.FirstJob.locFrom.location;

                    string toLocCntrA = GetDestination(value.SecondJob);
                    string fmLocCntrA = value.SecondJob.locFrom.location;

                    String lastLoc = "";
                    if (value.Count >= 2 && !value.FirstJob.spndFlg.Equals("Y") && !value.SecondJob.spndFlg.Equals("Y") 
                        && (value.FirstJob.workingMchn.ycTp == "TC" || value.SecondJob.workingMchn.ycTp == "TC"))
                    {
                        if (value.FirstJob.type.jobTp == "LD" && !value.SecondJob.workingMchn.mchnSts.Equals("L"))
                        {
                            lastLoc = fmLocCntrA.Split('-')[0];
                            fmLocCntrF = String.IsNullOrEmpty(value.FirstJob.locWorking.location) ? "" : getLocStar(value.FirstJob.locFrom.location, fmLocCntrA);
                        }
                        else if (value.SecondJob.type.jobTp == "DS")
                        {
                            lastLoc = toLocCntrF.Split('-')[0];
                            toLocCntrA = String.IsNullOrEmpty(value.SecondJob.locFrom.location) ? "" : getLocStar(value.SecondJob.locWorking.location, toLocCntrF);
                        }
                        else if (value.SecondJob.type.jobTp == "MI")
                        {
                            lastLoc = value.SecondJob.locWorking.blck;

                            if (value.FirstJob.type.jobTp == "MI" || value.FirstJob.type.jobTp == "MO")
                                toLocCntrA = String.IsNullOrEmpty(value.SecondJob.locFrom.location) ? "" : getLocStar(value.SecondJob.locWorking.location, toLocCntrF);
                        }
                        else if (value.FirstJob.type.jobTp == "RH")
                        {
                            lastLoc = value.SecondJob.locWorking.blck;
                        }
                        else if (value.FirstJob.type.jobTp == "MO")
                        {
                            if (value.SecondJob.type.jobTp == "MO" && !value.SecondJob.workingMchn.mchnSts.Equals("L"))
                                fmLocCntrF = String.IsNullOrEmpty(value.FirstJob.locWorking.location) ? "" : getLocStar(value.FirstJob.locFrom.location, fmLocCntrA);

                            if (value.FirstJob.workingMchn.mchnSts.Equals("L"))
                            {
                                lastLoc = value.FirstJob.locWorking.blck;
                            }
                            else
                            {
                                lastLoc = value.FirstJob.locFrom.blck;
                            }
                        }
                    }
                    else if (value.Count >= 2 && (value.FirstJob.spndFlg.Equals("Y") || value.SecondJob.spndFlg.Equals("Y")) 
                            && (value.FirstJob.workingMchn.ycTp == "TC" || value.SecondJob.workingMchn.ycTp == "TC"))
                    {
                        if (value.FirstJob.type.jobTp == "LD" && !value.SecondJob.workingMchn.mchnSts.Equals("L"))
                        {
                            lastLoc = fmLocCntrA.Split('-')[0];
                            fmLocCntrF = String.IsNullOrEmpty(value.FirstJob.locWorking.location) ? "" : value.FirstJob.spndFlg.Equals("Y") ? getLocStar(value.FirstJob.locFrom.location, fmLocCntrA) : fmLocCntrF;
                        }
                        else if (value.SecondJob.type.jobTp == "DS")
                        {
                            lastLoc = toLocCntrF.Split('-')[0];
                            toLocCntrA = String.IsNullOrEmpty(value.SecondJob.locFrom.location) ? "" : value.SecondJob.spndFlg.Equals("Y") ? getLocStar(value.SecondJob.locWorking.location, toLocCntrF) : toLocCntrA;
                        }
                        else if (value.SecondJob.type.jobTp == "MI")
                        {
                            lastLoc = value.SecondJob.locWorking.blck;

                            if (value.FirstJob.type.jobTp == "MI" || value.FirstJob.type.jobTp == "MO")
                                toLocCntrA = String.IsNullOrEmpty(value.SecondJob.locFrom.location) ? "" : value.SecondJob.spndFlg.Equals("Y") ? getLocStar(value.SecondJob.locWorking.location, toLocCntrF) : toLocCntrA;
                        }
                        else if (value.FirstJob.type.jobTp == "RH")
                        {
                            lastLoc = value.SecondJob.locWorking.blck;
                        }
                        else if (value.FirstJob.type.jobTp == "MO")
                        {
                            if (value.SecondJob.type.jobTp == "MO" && !value.SecondJob.workingMchn.mchnSts.Equals("L"))
                                fmLocCntrF = String.IsNullOrEmpty(value.FirstJob.locWorking.location) ? "" : value.FirstJob.spndFlg.Equals("Y") ? getLocStar(value.FirstJob.locFrom.location, fmLocCntrA) : fmLocCntrF;

                            if (value.FirstJob.workingMchn.mchnSts.Equals("L"))
                            {
                                lastLoc = value.FirstJob.locWorking.blck;
                            }
                            else
                            {
                                lastLoc = value.FirstJob.locFrom.blck;
                            }
                        }
                    }
                    else if (value.Count == 1) // 20190807
                    {
                        if ((followFirst && value.FirstJob.type.jobTp == "LD") || value.SecondJob.type.jobTp == "LD")
                        {
                            if ((followFirst && value.FirstJob.workingMchn.mchnSts.Equals("L")) || value.SecondJob.workingMchn.mchnSts.Equals("L"))
                                lastLoc = followFirst ? toLocCntrF.Split('-')[0] : toLocCntrA.Split('-')[0];
                            else
                                lastLoc = followFirst ? fmLocCntrF.Split('-')[0] : fmLocCntrA.Split('-')[0];
                        }
                        else if ((followFirst && value.FirstJob.type.jobTp == "DS") || value.SecondJob.type.jobTp == "DS")
                        {
                            lastLoc = followFirst ? toLocCntrF.Split('-')[0] : toLocCntrA.Split('-')[0];
                        }
                        else if ((followFirst && (value.FirstJob.type.jobTp == "MI" || value.FirstJob.type.jobTp == "RH")) || (value.SecondJob.type.jobTp == "MI" || value.SecondJob.type.jobTp == "RH"))
                        {
                            lastLoc = followFirst ? value.FirstJob.locWorking.blck : value.SecondJob.locWorking.blck;
                        }
                        else if ((followFirst && value.FirstJob.type.jobTp == "MO") || value.SecondJob.type.jobTp == "MO")
                        {
                            if ((followFirst && value.FirstJob.workingMchn.mchnSts.Equals("L")) || value.SecondJob.workingMchn.mchnSts.Equals("L"))
                            {
                                lastLoc = followFirst ? value.FirstJob.locWorking.blck : value.SecondJob.locWorking.blck;
                            }
                            else
                            {
                                lastLoc = followFirst ? value.FirstJob.locFrom.blck : value.SecondJob.locFrom.blck;
                            }
                        }
                    }

                    Ini.IniFile ini = new Ini.IniFile(GetIniDirectory() + @"MachineInfo.ini");
                    ini.IniWriteValue("MACHINE", "LastLocation", lastLoc);
                    mMainWindow.LoginView.TextBox_LastLoc.Text = lastLoc;

                    //*****END LOCATION*****

                    //txt_toLocationF.Content = toLocCntrF == "-" ? "" : toLocCntrF;
                    txt_toLocationF.Content = new AccessText() { Text = (toLocCntrF == "-" ? "" : toLocCntrF), TextWrapping = TextWrapping.Wrap };

                    txt_fmLocationF.Content = fmLocCntrF;
                    txt_fmLocationF.Content = new AccessText() { Text = fmLocCntrF, TextWrapping = TextWrapping.Wrap };

                    if ((value.FirstJob.workingMchn.mchnSts.Equals("U") && (value.FirstJob.type.jobTp.Equals("LD")) || value.FirstJob.type.jobTp.Equals("MO")))
                    {
                        txt_fmLocationF.Background = color_lightYellow;
                        if (value.FirstJob.type.jobTp.Equals("LD"))
                            txt_ycNoF.Background = color_lightYellow;
                        else
                            txt_ycNoF.Background = Brushes.White; ;
                    }
                    else if (value.FirstJob.workingMchn.mchnSts.Equals("L"))
                    {
                        txt_fmLocationF.Background = color_gray;

                        if (value.FirstJob.type.jobTp.Equals("LD"))
                            txt_ycNoF.Background = color_gray;
                        else
                            txt_ycNoF.Background = Brushes.White;
                    }
                    else
                    {
                        txt_fmLocationF.Background = Brushes.White;
                        txt_ycNoF.Background = Brushes.White;
                    }

                    if (value.FirstJob.type.jobTp.Equals("MI") || value.FirstJob.workingMchn.mchnSts.Equals("L"))
                        txt_toLocationF.Background = color_lightYellow;
                    else
                        txt_toLocationF.Background = Brushes.White;

                    txt_cntrWgtF.Content = value.FirstJob.cntr.cntrWgt;
                    txt_ycNoF.Content = value.FirstJob.ycNo;
                    ycNoFForLogging = value.FirstJob.ycNo;
                    txt_vslHoldDeckF.Content = value.FirstJob.vslHoldDeck == "H" ? "HOLD" : (value.FirstJob.vslHoldDeck == "D" ? "DECK" : "");
                    string tpRfFore = "R".Equals(value.FirstJob.cntr.cntrCgoTp) ? "Y" : "N";
                    String doorOptF = lang.Contains("Korea") ? value.FirstJob.doorKor : value.FirstJob.doorDir;
                    txt_cntrLen_cntrTp_F.Content = (string.IsNullOrEmpty(value.FirstJob.cntr.cntrNo)) ? "" : (value.FirstJob.cntr.cntrLen + value.FirstJob.cntr.cntrTp + Environment.NewLine + "/" + tpRfFore + "/" + doorOptF);

                    lbl_SingleFlgF.BorderBrush = Brushes.Red;
                    lbl_SingleFlgA.BorderBrush = Brushes.Red;

                    if (!String.IsNullOrEmpty(value.FirstJob.cntr.cntrNo) && !String.IsNullOrEmpty(value.SecondJob.cntr.cntrNo)) // 2 CNTR 20ft
                    {
                        lbl_SingleFlgF.Visibility = Visibility.Visible;
                        lbl_SingleFlgA.Visibility = Visibility.Hidden;

                        if ((followFirst && (value.FirstJob.type.twinTandemFlg.Equals("M")) || value.SecondJob.type.twinTandemFlg.Equals("M")))
                        {
                            lbl_SingleFlgF.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0071", LanguageService.LABEL_MAINWINDOW); // M => W
                        }
                        else if ((followFirst && (value.FirstJob.type.twinTandemFlg.Equals("W")) || value.SecondJob.type.twinTandemFlg.Equals("W")))
                        {
                            lbl_SingleFlgF.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0070", LanguageService.LABEL_MAINWINDOW); // W => T
                        }
                        else
                        {
                            lbl_SingleFlgF.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0072", LanguageService.LABEL_MAINWINDOW); // "S"
                            lbl_SingleFlgA.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0072", LanguageService.LABEL_MAINWINDOW); // "S"
                            lbl_SingleFlgA.Visibility = Visibility.Visible;
                        }
                    }
                    else if (!String.IsNullOrEmpty(value.FirstJob.cntr.cntrNo) || !String.IsNullOrEmpty(value.SecondJob.cntr.cntrNo)) // 1 CNTR 20ft/40ft
                    {
                        if ("C".Equals(value.FirstJob.type.jobFlagInfo))
                        {
                            lbl_SingleFlgF.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0073", LanguageService.LABEL_MAINWINDOW); // "C"
                            lbl_SingleFlgA.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0073", LanguageService.LABEL_MAINWINDOW); // "C"
                        }
                        else
                        {
                            lbl_SingleFlgF.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0072", LanguageService.LABEL_MAINWINDOW); // "S"
                            lbl_SingleFlgA.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0072", LanguageService.LABEL_MAINWINDOW); // "S"
                        }

                        if (String.IsNullOrEmpty(value.FirstJob.cntr.cntrNo))
                        {
                            lbl_SingleFlgF.Visibility = Visibility.Hidden;
                            lbl_SingleFlgA.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            lbl_SingleFlgF.Visibility = Visibility.Visible;
                            lbl_SingleFlgA.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        lbl_SingleFlgF.Content = "";
                        lbl_SingleFlgF.Visibility = Visibility.Hidden;
                        lbl_SingleFlgA.Content = "";
                        lbl_SingleFlgA.Visibility = Visibility.Hidden;
                    }

                    //AFTER CONTAINER
                    //txt_toLocationA.Content = toLocCntrA == "-" ? "" : toLocCntrA;
                    txt_toLocationA.Content = new AccessText() { Text = (toLocCntrA == "-" ? "" : toLocCntrA), TextWrapping = TextWrapping.Wrap };

                    //txt_fmLocationA.Content = fmLocCntrA;
                    txt_fmLocationA.Content = new AccessText() { Text = fmLocCntrA, TextWrapping = TextWrapping.Wrap };

                    String jobTpOptA = lang.Contains("Korea") ? value.SecondJob.jobTpKor : value.SecondJob.type.jobTp;
                    txt_cntrNoA.Content = String.IsNullOrEmpty(value.SecondJob.cntr.cntrNo) ? "" : (value.SecondJob.cntr.cntrNo + " (" + jobTpOptA + ")");

                    cntrANoForLogging = value.SecondJob.cntr.cntrNo;

                    String holdValA = value.SecondJob.hold;
                    Lbl_HoldValA.FontSize = holdValA.Length >= 8 ? 17 : 22;
                    if (String.IsNullOrEmpty(holdValA) || "N".Equals(holdValA))
                    {
                        Lbl_HoldValA.Content = "N";
                        Lbl_HoldValA.Foreground = Brushes.Black;
                        Lbl_HoldValA.Background = Brushes.White;
                    }
                    else
                    {
                        Lbl_HoldValA.Content = holdValA;
                        Lbl_HoldValA.Foreground = Brushes.White;
                        Lbl_HoldValA.Background = Brushes.Red;
                    }

                    Lbl_LaneValA.Content = value.SecondJob.qcLn;

                    if ((value.SecondJob.workingMchn.mchnSts.Equals("U") && (value.SecondJob.type.jobTp.Equals("LD")) || value.SecondJob.type.jobTp.Equals("MO")))
                    {
                        txt_fmLocationA.Background = color_lightYellow;

                        if (value.SecondJob.type.jobTp.Equals("LD"))
                            txt_ycNoA.Background = color_lightYellow;
                        else
                            txt_ycNoA.Background = Brushes.White;
                    }
                    else if (value.SecondJob.workingMchn.mchnSts.Equals("L"))
                    {
                        txt_fmLocationA.Background = color_gray;

                        if (value.SecondJob.type.jobTp.Equals("LD"))
                            txt_ycNoA.Background = color_gray;
                        else
                            txt_ycNoA.Background = Brushes.White;
                    }
                    else
                    {
                        txt_fmLocationA.Background = Brushes.White;
                        txt_ycNoA.Background = Brushes.White;
                    }

                    if (value.SecondJob.type.jobTp.Equals("MI") || value.SecondJob.workingMchn.mchnSts.Equals("L"))
                        txt_toLocationA.Background = color_lightYellow;
                    else
                        txt_toLocationA.Background = Brushes.White;

                    txt_cntrWgtA.Content = value.SecondJob.cntr.cntrWgt;
                    txt_ycNoA.Content = value.SecondJob.ycNo;
                    ycNoAForLogging = value.SecondJob.ycNo;
                    txt_vslHoldDeckA.Content = value.SecondJob.vslHoldDeck == "H" ? "HOLD" : (value.SecondJob.vslHoldDeck == "D" ? "DECK" : "");
                    string tpRfAfter = "R".Equals(value.SecondJob.cntr.cntrCgoTp) ? "Y" : "N";
                    String doorOptA = lang.Contains("Korea") ? value.SecondJob.doorKor : value.SecondJob.doorDir;
                    txt_cntrLen_cntrTp_A.Content = (string.IsNullOrEmpty(value.SecondJob.cntr.cntrNo)) ? "" : (value.SecondJob.cntr.cntrLen + value.SecondJob.cntr.cntrTp + Environment.NewLine + "/" + tpRfAfter + "/" + doorOptA);

                    //CHECK THE YTSTS AND CHASSIS NO
                    if (checkLadenChss000())
                        if (!checkSameJobCurAndPrv()
                            || (checkSameJobCurAndPrv() && ((JobOrderForITV_Prev.FirstJob.workingMchn.mchnSts.Equals("U") && JobOrderForITV_Current.FirstJob.workingMchn.mchnSts.Equals("L"))
                                                            || (JobOrderForITV_Prev.SecondJob.workingMchn.mchnSts.Equals("U") && JobOrderForITV_Current.SecondJob.workingMchn.mchnSts.Equals("L")))
                                )
                            )
                        {
                            PresentationMgr.AppWin.PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0042", LanguageService.LABEL_POPUP), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0041", LanguageService.LABEL_POPUP), ""
                                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                        }                    

                    //RFID
                    //txt_RfidNo.Content = followFirst ? value.FirstJob.rfidNo : value.SecondJob.rfidNo;
                    //if ("Y".Equals(followFirst ? value.FirstJob.rfidChk : value.SecondJob.rfidChk))
                    //{
                    //    txt_RfidChk.Content = "Read";
                    //    txt_RfidChk.Background = new SolidColorBrush(Colors.Green);
                    //}
                    //else
                    //{
                    //    txt_RfidChk.Content = "Non";
                    //    txt_RfidChk.Background = new SolidColorBrush(Colors.Red);
                    //}

                }

                isPinningStationUserClick = false;
                if ((followFirst && value.FirstJob.type.jobTp.Equals("LD") && value.FirstJob.partnerMchn.mchnTp.Equals("QC")) || (value.SecondJob.type.jobTp.Equals("LD") && value.SecondJob.partnerMchn.mchnTp.Equals("QC")))
                {
                    if (isPrecedingYtListPolling == false)
                    {
                        isPrecedingYtListPolling = true;
                        VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetPrecedingYtList);
                    }
                }
                else if (isPrecedingYtListPolling)
                {
                    isPrecedingYtListPolling = false;
                    VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetPrecedingYtList);
                    FlowText.instance().ClearLDSeqMachine();
                }

                //Show animation for cone signal
                if (value.Count > 0 &&
                    (
                    (!"".Equals(value.FirstJob.jobKey) &&
                    ("DS".Equals(value.FirstJob.type.jobTp) || "LD".Equals(value.FirstJob.type.jobTp))
                    && (value.FirstJob.type.conePlan != null && !"N".Equals(value.FirstJob.type.conePlan)) && "L".Equals(value.FirstJob.workingMchn.mchnSts))

                    ||

                    (!"".Equals(value.SecondJob.jobKey) &&
                    ("DS".Equals(value.SecondJob.type.jobTp) || "LD".Equals(value.SecondJob.type.jobTp))
                    && (value.SecondJob.type.conePlan != null && !"N".Equals(value.SecondJob.type.conePlan)) && "L".Equals(value.SecondJob.workingMchn.mchnSts))
                    )
                    )
                {
                    isExistConeChecker = true;
                }
                else
                {
                    isExistConeChecker = false;
                }
                // BreakPopupView
                if (value.FirstJob.cntr.cntrNo.Length > 0 && BreakPopupView.btn_Complete.Content.ToString() == PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0102", LanguageService.LABEL_CUSTOMIZE))
                {
                    BreakPopupView.CheckButton(value.FirstJob);
                }
                else if (value.SecondJob.cntr.cntrNo.Length > 0 && BreakPopupView.btn_Complete.Content.ToString() == PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0102", LanguageService.LABEL_CUSTOMIZE))
                {
                    BreakPopupView.CheckButton(value.SecondJob);
                }
                else
                    BreakPopupView.DisableButton();

                //20190807 SELECT JOB DEFAULT
                if (value.JobOrder.Count > 0)
                {
                    int selectJob = 0;
                    if (selectedContainer == null || selectedContainer.cntr.cntrNo.Length == 0 || (!selectedContainer.cntr.cntrNo.Equals(value.FirstJob.cntr.cntrNo) && !selectedContainer.cntr.cntrNo.Equals(value.SecondJob.cntr.cntrNo))) // after auto refresh, don't need select job default
                    {
                        selectJob = 1;

                        if (value.Count < 2)
                        {
                            if (value.FirstJob.cntr.cntrNo.Length == 0)
                            {
                                selectJob = 2;
                            }
                        }
                        else
                        {
                            string jobType = value.FirstJob.type.jobTp;
                            if (jobType.Equals("LD") || jobType.Equals("MO"))
                            {
                                if (value.FirstJob.locFrom.blck == value.SecondJob.locFrom.blck && value.FirstJob.locFrom.bay == value.SecondJob.locFrom.bay)
                                {
                                    if ("MO".Equals(jobType) || ("LD".Equals(jobType) && (!"C".Equals(value.FirstJob.type.jobStatus) || "C".Equals(value.SecondJob.type.jobStatus))))
                                        selectJob = 1;
                                }
                                else if ( (value.SecondJob.workingMchn.mchnSts != "L")
                                    || (jobType.Equals("LD") && "C".Equals(value.FirstJob.type.jobStatus))
                                    )
                                {
                                    selectJob = 2;
                                }
                            }
                        }
                    }
                    else if (selectedContainer.cntr.cntrNo.Equals(value.FirstJob.cntr.cntrNo)) // && !selectedContainer.type.jobFlagInfo.Equals(value.FirstJob.type.jobFlagInfo))
                    {
                        selectJob = 1;
                    }
                    else if (selectedContainer.cntr.cntrNo.Equals(value.SecondJob.cntr.cntrNo)) // && !selectedContainer.type.jobFlagInfo.Equals(value.SecondJob.type.jobFlagInfo))
                    {
                        selectJob = 2;
                    }

                    //20200102 MO, MI SET JOB SELECTED WITH JOBTYPE CHANGE MO/MI
                    if (value.Count > 1 && ("MO".Equals(value.FirstJob.type.jobTp) || "MI".Equals(value.FirstJob.type.jobTp) )
                        )
                    {
                        if (value.FirstJob.type.jobTp.Equals("MO") && value.SecondJob.type.jobTp.Equals("MO"))
                        {
                            previous2MOJobs = 1;
                        }

                        if (previous2MOJobs == 1)
                        {
                            if (value.FirstJob.type.jobTp.Equals("MI") && value.SecondJob.type.jobTp.Equals("MO"))
                            {
                                selectJob = 2;
                                previous2MOJobs = 0;
                                previousMOMIJobs = 1;
                            }
                            else if (value.FirstJob.type.jobTp.Equals("MO") && value.SecondJob.type.jobTp.Equals("MI"))
                            {
                                selectJob = 1;
                                previous2MOJobs = 0;
                                previousMOMIJobs = 1;
                            }
                        }

                        if (value.FirstJob.type.jobTp.Equals("MI") && value.SecondJob.type.jobTp.Equals("MI") && selectJob == 2 && previousMOMIJobs == 1)
                        {
                            selectJob = 1;
                            previousMOMIJobs = 0;
                        }
                    }//LD SET JOB SELECTED WITH JOB STATUS
                    else if (value.Count > 1 && "LD".Equals(value.FirstJob.type.jobTp))
                    {
                        if (!"C".Equals(value.FirstJob.type.jobStatus) && !"C".Equals(value.SecondJob.type.jobStatus))
                        {
                            previous2LDUncompleteJobs = 1;
                        }

                        if (previous2LDUncompleteJobs == 1)
                        {
                            if ("C".Equals(value.FirstJob.type.jobStatus) && !"C".Equals(value.SecondJob.type.jobStatus) )
                            {
                                selectJob = 2;
                                previous2LDUncompleteJobs = 0;
                                previous1LDCompletedJobs = 1;
                            }
                            else if (!"C".Equals(value.FirstJob.type.jobStatus) && "C".Equals(value.SecondJob.type.jobStatus))
                            {
                                selectJob = 1;
                                previous2LDUncompleteJobs = 0;
                                previous1LDCompletedJobs = 1;
                            }
                        }

                        if (  "C".Equals(value.FirstJob.type.jobTp) && "C".Equals(value.SecondJob.type.jobTp) && previous1LDCompletedJobs == 1)
                        {
                            selectJob = 1;
                            previous1LDCompletedJobs = 0;
                        }
                    }

                    if (selectJob == 2)
                    {
                        Grid_After_MouseLeftButtonDown(null, null);
                    }
                    else if (selectJob == 1)
                    {
                        Grid_Fore_MouseLeftButtonDown(null, null);
                    }
                    else
                    {
                        selectedContainer = null;
                    }
                }

                //----- 20191212 spndFlg - SUSPEND FLAG
                if(selectedContainer != null && selectedContainer.spndFlg.Equals("Y"))
                    DisableButton();
                
                // FORE
                if (value.FirstJob.spndFlg.Equals("Y") )
                {
                    if (value.FirstJob.workingMchn.mchnSts == "L")
                    {
                        DisableField("F");
                    }
                    else 
                    {
                        SetNoJob("F");
                    }
                }

                //AFTER
                if (value.SecondJob.spndFlg.Equals("Y"))
                {
                    if (value.SecondJob.workingMchn.mchnSts == "L")
                    {
                        
                        DisableField("A");
                    }
                    else 
                    {
                        SetNoJob("A");
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog("[Exception getMachineJobByKeys UI]" + " | " + "[Exception message]: " + ex.Message + "[InnerException]: " + ex.InnerException + Environment.NewLine
                    + "[Source]:" + ex.Source + Environment.NewLine +"[StackTrace]:" + ex.StackTrace);
            }
        }

        #region [getMachineStatusChanged callback]
        int hatchJobIndex = 0;
        List<String> listHatchJob;

        private bool checkArmgReadFlg()
        {
            if (armgReadFlg.ToUpper().Equals("Y"))
                return true;
            return false;
        }

        private bool checkLadenChss000()
        {
            if ((JobOrderForITV_Current.FirstJob.workingMchn.mchnSts.Equals("L") || JobOrderForITV_Current.SecondJob.workingMchn.mchnSts.Equals("L"))
                    && mMainWindow.preChassisCd.Equals("000"))
            {
                return true;
            }
            return false;
        }

        private bool checkSameJobCurAndPrv()
        {
            if (JobOrderForITV_Current.FirstJob.cntr.cntrNo != JobOrderForITV_Prev.FirstJob.cntr.cntrNo
                || JobOrderForITV_Current.SecondJob.cntr.cntrNo != JobOrderForITV_Prev.SecondJob.cntr.cntrNo)
            {
                return false;
            }
            return true;
        }

        public void getMachineStatusChangedHatchJobCallback(VMT_Data_JAT2.Objects.Common.VmtMachine mchn)
        {
            try
            {
                machineStatusChanged_PRV = machineStatusChanged_Current;
                machineStatusChanged_Current = mchn;

                if ((machineStatusChanged_PRV.rfidBlck != machineStatusChanged_Current.rfidBlck && machineStatusChanged_PRV.rfidBlck == "")
                    || (machineStatusChanged_PRV.armgReadFlg != machineStatusChanged_Current.armgReadFlg && machineStatusChanged_PRV.armgReadFlg == "")) // ONLY SOUND ONCE WHEN EMPTY => VALUE
                {
                    mMainWindow.BeefPlay_dingdong();
                }


                armgReadFlg = mchn.armgReadFlg;

                var cntrNo = String.Empty;
                if (selectedContainer != null && !string.IsNullOrEmpty(selectedContainer.cntr.cntrNo))
                {
                    cntrNo = selectedContainer.cntr.cntrNo;
                }

                //SaveLog("getMachineStatusChangedCallback UI",
                //            "[rfidBlck]" + mchn.rfidBlck
                //            + " [armgReadFlg]" + mchn.armgReadFlg + " [cntrNo]" + cntrNo
                //            );              

                listHatchJob = new List<string>();
                foreach (String s in mchn.hatchQcList)
                {
                    listHatchJob.Add(s);
                }              
                showHatchJobItem();
                if (cntrANoForLogging != "")
                {
                    MainWindow.LogWin.WriteLog("ARMGC Ready -" + ycNoAForLogging + "/" + cntrANoForLogging);
                }
                if (cntrFNoForLogging != "")
                {
                    MainWindow.LogWin.WriteLog("ARMGC Ready -" + ycNoFForLogging + "/" + cntrFNoForLogging);
                }
            }
            catch(Exception ex)
            {
                SaveLog("[Exception GetMachineStatusChanged UI]" + " | " + "[Exception message]: " + ex.Message + "[InnerException]: " + ex.InnerException + Environment.NewLine
                    + "[Source]:" + ex.Source + Environment.NewLine + "[StackTrace]:" + ex.StackTrace);
            }   
        }

        private void showHatchJobItem()
        {
            //HATCH JOB
            txt_HatchJob.Content = "";
            int i, c = hatchJobIndex + 3;
            for (i = hatchJobIndex; i < c; i++)
            {
                if (i < listHatchJob.Count && i != c)
                {
                    txt_HatchJob.Content += listHatchJob[i] + "\r\n";
                }
            }

            if (listHatchJob.Count % 3 == 0)
            {
                TextBlock_PageNum.Text = (hatchJobIndex / 3 + 1).ToString() + "/" + (listHatchJob.Count / 3).ToString();
            }
            else
            {
                TextBlock_PageNum.Text = (hatchJobIndex / 3 + 1).ToString() + "/" + (listHatchJob.Count / 3 + 1).ToString();
            }

            if (listHatchJob.Count > 3)
                Btn_HatchJob_Down.IsEnabled = true;
        }


        private void Btn_HatchJob_Up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (hatchJobIndex > 0)
            {
                hatchJobIndex -= 3;
                showHatchJobItem();
            }
        }

        private void Btn_HatchJob_Down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (hatchJobIndex < listHatchJob.Count - 3)
            {
                hatchJobIndex += 3;
                showHatchJobItem();
            }

            if (hatchJobIndex > 0)
                Btn_HatchJob_Up.IsEnabled = true;
        }
        #endregion [getMachineStatusChanged callback]

        #region [MessageProcess]
        public String currentMessage = String.Empty;
        public String[] arrayMessage;
        private int messPage = 1;
        private int messCount = 0;
        public void getMessageList(String message)
        {
            currentMessage = message.Replace("\r\n", "@");
            arrayMessage = currentMessage.Split('@');
            messCount = arrayMessage.Length;
            messPage = 1;
        }
        public void SetMessageGrid()
        {
            this.TextBlock_Message.Text = String.Empty;
            this.Tbl_MessPage.Text = messPage + "/" + (messCount / 3 + (messCount % 3 > 0 ? 1 : 0));
            for (int i = 3 * messPage - 2; i <= 3 * messPage; i++)
            {
                if (i <= messCount)
                {
                    this.TextBlock_Message.Text += arrayMessage[i - 1];
                    if (i <= 3 * messPage - 1)
                        this.TextBlock_Message.Text += "\r\n";
                }
            }
        }
        private void Btn_MessUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (messPage > 1)
            {
                messPage -= 1;
                SetMessageGrid();
            }
        }
        private void Btn_MessDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (messPage < (messCount / 3 + (messCount % 3 > 0 ? 1 : 0)))
            {
                messPage += 1;
                SetMessageGrid();
            }
        }
        #endregion [MessageProcess]

        #region [Arrival Callback]
        public void ProcessByArrivalCallback(ITV.VD_ITV_SetManualArrival_Receive value)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob;

            if (value.m_bPOWIN == false)
                return;

            if (gJobOrderList.Count < 1)
                return;

            currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[0];

            mMainWindow.IndicatorView.StopJobTimer(); // Arrival 이후에는 ETA 보여주지 않음.
        }
        #endregion [Arrival Callback]

        #region [Ready Callback]
        public void ProcessByReadyCallback(ITV.VD_ITV_SetManualReady_Receive value)
        {
            if (gJobOrderList.Count < 1)
                return;

            mMainWindow.IndicatorView.StopJobTimer(); // Arrival 이후에는 ETA 보여주지 않음.           
        }
        #endregion [Ready Callback]

        #region [JoneDone Callback]
        public void ProcessByJobDoneCallback(VMT_Data_JAT2.Objects.Common.VD_Common_JobDone value)
        {
            //    if (mJobState == JOB_STATE.STATE_READY)
            {
                

                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob;
                
                for (int i = 0; i < gJobOrderList.Count; i++)
                {
                    if (MainWindow.STANDALONE_MODE)
                    {
                        value = new VMT_Data_JAT2.Objects.Common.VD_Common_JobDone();
                        value.jobKey = ((VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[0]).jobKey;
                    }

                    currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[i];
                    if (value.jobKey.Equals(currentJob.jobKey))
                    {
                        gBeforeMchnTp = currentJob.type.jobTp + currentJob.partnerMchn.mchnTp;
                        
                        if (isTwinJob)
                        {
                            countDoneTwinJob++;
                        }
                        else
                        {
                            countDoneTwinJob = 1;
                        }
                        gJobOrderList.RemoveAt(i);
                        break;
                    }
                }

                if (gJobOrderList.Count > 0)
                {
                    ProgressJob();
                    return;
                }

                if (gBeforeMchnTp.Equals("DSQC") || gBeforeMchnTp.Equals("LCQC") || gBeforeMchnTp.Equals("LDTC") || gBeforeMchnTp.Equals("MOTC"))
                {
                    for (int index = 0; index < countDoneTwinJob; index++)
                    {
                        mContainerImageforJobDone[index].Source = mContainerImagePathforJobDone[index];
                        gContainerIDTextBlock[index].Foreground = new SolidColorBrush(Colors.White);
                    }
                }
                else
                {
                    SetNoJob();
                }

                mMainWindow.IndicatorView.StopJobTimer();              
            }
        }
        #endregion [JoneDone Callback]

        #region [JobDelete Callback]
        public void ProcessByJobDeleteCallback(VMT_Data_JAT2.Objects.Common.VD_Common_JobKey value)
        {
            
            int i = 0;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob;
            //StopAnimationTruck();
            //StopAnimationContainer();

            //   TextBox_OderUp.Text = "JOB DONE";
            for (i = 0; i < gJobOrderList.Count; i++)
            {
                currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[i];
                if (value.jobKey.Equals(currentJob.jobKey))
                {
                    //       * 
                    //*   * 1. DS+QC = 빈차
                    //* 2. DS+TC = 상차
                    //* 3. LD+QC = 상차
                    //* 4. LD+TC = 빈차
                    //* 5. MO+TC = 빈차
                    //* 6. MI+TC = 상차

                    gJobOrderList.RemoveAt(i);
                    break;
                }
            }
            if (i == 0)
            {
                if (gJobOrderList.Count > 0)
                {
                    ProgressJob();
                }
                else
                {
                    ReFlash();
                }
            }
            else
            {
            }
            mMainWindow.PopupView.ShowPopup(1, "Cancel Job", "Job is canceled.", "", "OK", "", null, 3);
        }
        #endregion [JobDelete Callback]

        #region [JobDeleteAll Callback]
        public void ProcessByJobDeleteAllCallback(int value)
        {
            // for Debug XMl Log save
            InterfaceMessageLoader.instance().WriteInterfaceMessage<int>("ProcessByJobDeleteAllCallback", value);

            gJobOrderList.Clear();

            mMainWindow.PopupView.ShowPopup(1, "Message", "Processing", "", "OK", "", null, 3);
            ReFlash();

            if (mMainWindow.ByPassPopup.Visibility == System.Windows.Visibility.Visible)
                mMainWindow.ByPassPopup.HidePopup();

        }
        #endregion [JobDeleteAll Callback]

        #region [ProcessByJobChange Callback]
        public void ProcessByJobChangeCallback(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder value)
        {

        }
        #endregion [ProcessByJobChange Callback]
        ///////////////////////////////////////////

        ///////////////////////////////////////////
        //--Periodic Process Callback
        #region [EngineTemp Callback]
        private bool isExistConeChecker = false;
        public void ProcessByEngineTempCallback(int value)
        {
            if (MainWindow.TEST_MODE)
            {
                // Print Log Window
                string strLog = "";
                strLog += "---------------------------------------------\r\n";
                strLog += "Message Name : ProcessByEngineTempCallback\r\n";
                strLog += string.Format("EngineTemp Callback Value : {0}\r\n", value.ToString());
                strLog += "---------------------------------------------\r\n";
                MainWindow.LogWin.WriteLog(strLog);
            }

            if (value == 1) // Temperature High
            {
                isExistConeChecker = true;
                //StartAnimationEngineIcon();
            }
            else
            {
                isExistConeChecker = false;
                //StopAnimationEngineIcon();
            }

        }
        #endregion [EngineTemp Callback]

        #region [FuelGage Callback]
        public void ProcessByFuelGageCallback(int value)
        {
            if (MainWindow.TEST_MODE)
            {
                // Print Log Window
                string strLog = "";
                strLog += "---------------------------------------------\r\n";
                strLog += "Message Name : ProcessByFuelGageCallback\r\n";
                strLog += string.Format("FuelGage(Min) Gage : {0}\r\n", miFuelGage.ToString());
                strLog += string.Format("FuelGage Callback Value : {0}\r\n", value.ToString());
                strLog += "---------------------------------------------\r\n";
                MainWindow.LogWin.WriteLog(strLog);
            }

            if (value <= miFuelGage)
            {
                //StartAnimationDieselIcons();

                if (gJobOrderList.Count == 0)
                {
                    //TextBox_OderDown.Text = "Go to Gas Station";
                    //Image_next_gas.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else if (value >= miFuelGage + (miFuelGage * 0.2))
            {
                //Image_next_gas.Visibility = System.Windows.Visibility.Hidden;
                //StopAnimationDieselIcon();
            }
        }
        #endregion [FuelGage Callback]

        #region [SpeedKm Callback]
        public void ProcessBySpeedKmCallback(int value)
        {
            // Print Log Window
            //if (TextBlock_speed.Text != value.ToString() &&
            //    MainWindow.TEST_MODE)
            //{
            //    string strLog = "";
            //    strLog += "---------------------------------------------\r\n";
            //    strLog += "Message Name : ProcessBySpeedKmCallback\r\n";
            //    strLog += string.Format("SpeedKm Callback Value : {0}\r\n", value.ToString());
            //    strLog += "---------------------------------------------\r\n";
            //    MainWindow.LogWin.WriteLog(strLog);
            //}

            //TextBlock_speed.Text = value.ToString();
            //if (value > 0 && gJobOrderList.Count > 0 && mJobState == JOB_STATE.STATE_HAVEJOB)
            //{
            //    StartAnimationTruck();
            //}
            //else
            //{
            //    StopAnimationTruck();
            //}

            //if (value >= mfLimitSpeed)
            //{
            //    StartAnimationSpeedIcon();
            //}
            //else
            //{
            //    StopAnimationSpeedIcon();
            //}
        }
        #endregion [SpeedKm Callback]
        ///////////////////////////////////////////

        #region [STSLDSeq Callback]
        public void ProcessBySTSLDSeqCallback(ITV.VD_ITV_STS_LDPlan value)
        {
            //-----------------------------------------------------
            //- reOrdering with planSeq
            int nCount = value.nCount;
            SVMT_PlanSeqList.Clear();
            for (int i = 0; i < nCount; i++)
            {
                LinkedListNode<ITV.VD_ITV_PlanSeq> node = SVMT_PlanSeqList.First;

                if (node == null)
                {
                    SVMT_PlanSeqList.AddFirst(value.MchnPlan[i]);
                    continue;
                }

                int planSeq = value.MchnPlan[i].planSeq;

                while (node != null)
                {
                    if (planSeq < node.Value.planSeq)
                    {
                        SVMT_PlanSeqList.AddBefore(node, value.MchnPlan[i]);
                        break;
                    }
                    else
                    {
                        if (node.Next == null)
                        {
                            SVMT_PlanSeqList.AddAfter(node, value.MchnPlan[i]);
                            break;
                        }
                    }

                    node = node.Next;
                }
            }
            //-----------------------------------------------------

            refreshSTSLDSeqFlowText();
        }

        private void refreshSTSLDSeqFlowText()
        {
            FlowText.instance().ClearLDSeqMachine();

            LinkedListNode<ITV.VD_ITV_PlanSeq> node = SVMT_PlanSeqList.First;
            List<ITV.VD_ITV_PlanSeq> strMchnIDList = new List<ITV.VD_ITV_PlanSeq>();
            strMchnIDList.Clear();
            while (node != null)
            {
                strMchnIDList.Add(node.Value);
                node = node.Next;
            }

            // Bottom Display
            // FlowText.instance().AddLDSeqMachine(strMchnIDList);

            Int32 nMoveCount = 0;
            Int32 nMyPosition = 0;
            for (Int32 currentIndex = 0; currentIndex < strMchnIDList.Count; currentIndex++)
            {
                if (strMchnIDList[currentIndex].MchnID.Equals(ITV.ITV_User.gMchnID))
                {
                    nMyPosition = currentIndex;
                    nMoveCount = currentIndex + 1;
                    break;
                }
            }

            if (nMyPosition > 0)
            {
                //TextBlock_truck_prev_sts.Text = strMchnIDList[nMyPosition - 1].MchnID;
                //TextBlock_current_sts.Text = nMoveCount.ToString();
            }
            else
            {
                //TextBlock_truck_prev_sts.Text = "";

                if (nMoveCount > 0)
                {
                    //TextBlock_current_sts.Text = nMoveCount.ToString();
                }
                else
                {
                    //TextBlock_current_sts.Text = "";
                }
            }
        }
        #endregion [STSLDSeq Callback]

        #region [MachineStop Callback]
        public void ProcessByMachineStopCallback(Object value)
        {
            if (mCurrentBreak == BreakModeType.None)
            {
                //Grid_break_End_Time.Visibility = System.Windows.Visibility.Visible;
                //string currentDate = DateTime.Now.ToString("yyyy/MM/dd"); // 시간을 구합니다
                //string currentTime = DateTime.Now.ToString("hh:mm"); // 시간을 구합니다

                //TextBlock_Break_End_Date.Text = currentDate;
                //TextBlock_Break_End_Time.Text = currentTime;
            }
            else if (mCurrentBreak == BreakModeType.WaitingConfirm)
            {
                // 
            }
            else if (mCurrentBreak == BreakModeType.BreakDown)
            {
                //Grid_break_Start_Time.Visibility = System.Windows.Visibility.Visible;
                //Grid_break_End_Time.Visibility = System.Windows.Visibility.Hidden;
                //string currentDate = DateTime.Now.ToString("yyyy/MM/dd"); // 시간을 구합니다
                //string currentTime = DateTime.Now.ToString("hh:mm"); // 시간을 구합니다

                //TextBlock_Break_Start_Date.Text = currentDate;
                //TextBlock_Break_Start_Time.Text = currentTime;
            }
        }
        #endregion [MachineStop Callback]

        //#region [ControlMessage Callback]
        //public void ProcessByControlMessageCallback(ITV.SVMT_ControlMessage value)
        //{
        //    TextBox_OderUp.Text = value.topMessage;
        //    TextBox_OderDown.Text = value.bottomMessage;
        //}
        //#endregion [ControlMessage Callback]

        #region [GeoFence Callback]
        private bool isPinningStationUserClick = false;

        private void Image_Occupied_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (gJobOrderList.Count > 0)
                mMainWindow.PopupView.ShowPopup(2, "ALLOCATE", "Are you sure wish to allocate", "Cancel", "", "OK", CallBack_Allocate, 0);
        }

        private void CallBack_Allocate(int selected)
        {
            switch (selected)
            {
                case 0: // left cancel
                    break;
                case 1: // center system off
                    {
                    }
                    break;
                case 2: // right log out
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[0];
                        VMT_Data_JAT2.VMT_DataMgr_ITV.SetRealLocation(currentJob.jobKey);
                    }
                    break;
            }
        }

        public void ProcessByPinningStationCallback()
        {
            // GeoFence 여부 Check Function 추가
            // bool isGeoFenceIn = PresentationMgr.AppWin.IndicatorView.isGeoFenceIn();

            if (isPinningStationUserClick == true ||
                gJobOrderList.Count == 0)
                return;

            bool isShow = false;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[0];
            if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
            {
                if (currentJob.type.jobTp.Equals("DS") &&
                    currentJob.workingMchn.mchnSts.Equals("L") &&
                    isExistConeChecker == true)
                {
                    isShow = true;
                }
                else if (currentJob.type.jobTp.Equals("LD") &&
                        currentJob.workingMchn.mchnSts.Equals("L") &&
                        currentJob.type.conePlan.Equals("Y") &&
                        isExistConeChecker == false)
                {
                    isShow = true;
                }
            }

            if (isShow == true)
            {
                if (mMainWindow.PinningStationPopup.Visibility != System.Windows.Visibility.Visible)
                    mMainWindow.PinningStationPopup.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0136", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0140", LanguageService.LABEL_CUSTOMIZE), "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", CallBack_PinningStation, 0);
            }
            else
            {
                if (mMainWindow.PinningStationPopup.Visibility == System.Windows.Visibility.Visible)
                    mMainWindow.PinningStationPopup.HidePopup();
            }
        }

        private void CallBack_PinningStation(int selected)
        {
            isPinningStationUserClick = true;
        }
        #endregion [GeoFence Callback]

        #region [ArrvdMchnAtPow Callback]
        public void ProcessByArrvdMchnAtPow(String value)
        {
            if (gJobOrderList.Count == 0)
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)gJobOrderList[0];

            if (currentJob.partnerMchn.mchnTp.Equals("TC"))
            {
                String[] arrvdMchnAtPowArray = value.Split(',');

                String arrvdMchnAtPow = "";
                if (arrvdMchnAtPowArray.Length > 0)
                    arrvdMchnAtPow = arrvdMchnAtPowArray[0];

                if (!String.IsNullOrEmpty(arrvdMchnAtPow))
                {
                    if (!arrvdMchnAtPow.Equals(ITV.ITV_User.gMchnID)) // different
                    {
                        //----------------------- Visible
                        //TextBlock_truck_prev_rtg.Text = arrvdMchnAtPow;
                    }
                    else // same
                    {
                        //----------------------- unVisible
                        //TextBlock_truck_prev_rtg.Text = "";
                    }
                }
                else
                {
                    //----------------------- unVisible
                    //TextBlock_truck_prev_rtg.Text = "";
                }
            }
        }

        #endregion [ArrvdMchnAtPow Callback]

        #region [ConfirmJobByScanner Callback]
        public void ProcessByConfirmJobByScanner(Boolean value)
        {

        }
        #endregion [ConfirmJobByScanner Callback]


        ///////////////////////////////////////////
        #endregion [Process Callback]

        #region [Textblock Event]
        private void TextBlock_LdCmpl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> tasks = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count >= 1)
                tasks.Add((VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0]);
            if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count >= 2)
                tasks.Add((VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[1]);

            if (tasks.Count > 0)
                VMT_Data_JAT2.VMT_DataMgr_ITV.SetQCJobReleaseByYt_Ask(tasks);
        }


        private void TextBlock_Arrival_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedContainer != null)
            {
                selectedContainer.cancelYn = arrivalCancel;
                VMT_Data_JAT2.VMT_DataMgr_ITV.SetMachineArrival_Ask(selectedContainer);
            }
        }
        
        private void TextBlock_Ready_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedContainer != null)
            {
                VMT_Data_JAT2.VMT_DataMgr_ITV.SetMachineReady_Ask(selectedContainer);
            }
        }

        private void TextBlock_Done_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedContainer != null)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder unSelectedContainer = null;
                if (JobOrderForITV_Current.Count > 1)
                {
                    if (JobOrderForITV_Current.FirstJob.jobKey == selectedContainer.jobKey)
                        unSelectedContainer = JobOrderForITV_Current.SecondJob;
                    else
                        unSelectedContainer = JobOrderForITV_Current.FirstJob;
                }
                
                VMT_Data_JAT2.VMT_DataMgr_ITV.SetItvDone_Ask(selectedContainer, unSelectedContainer);
            }
        }

        private void TextBlock_Chss_Chg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ChassisChangeView.Visibility = Visibility.Visible;
            ChassisChangeView.Tb_Before.Text = mMainWindow.preChassisCd;
            ChassisChangeView.Tb_After.Text = "";
            ChassisChangeView.CheckChangeBtn();
        }

        #endregion [Textblock Event]

        #region [Textblock available click]

        public void DisableAll()
        {
            selectedContainer = null;

            txt_cntrNoF.Background = Brushes.White;
            txt_cntrNoA.Background = Brushes.White;

            Grid_Fore.IsEnabled = false;
            Grid_After.IsEnabled = false;

            TextBlock_Arrival.IsEnabled = false;
            TextBlock_Arrival.Background = colorDisable;
            TextBlock_Done.IsEnabled = false;
            TextBlock_Done.Background = colorDisable;
        }
        
        private void TextBlock_Available_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WaitingBreakRequest = BreakWaitingType.None; // Jan 10 2020

            if (mCurrentBreak == BreakModeType.None)
            {
                if (BreakPopupView.Visibility == System.Windows.Visibility.Visible)
                    return;

                if (AvailablePopupView.Visibility == System.Windows.Visibility.Visible)
                {
                    TextBlock_StopPage.Background = colorEnable;
                    AvailablePopupView.Hide_Popup();
                }
                else
                {
                    TextBlock_StopPage.Background = Brushes.Red;
                    AvailablePopupView.Show_Popup();

                    if (this.receivedAvailable == null || this.receivedAvailable.m_pData == null || this.receivedAvailable.m_pData.Count <= 0)
                    {
                        VMT_DataMgr_Common.GetMachineStopCodeList_Ask();
                    }
                }
            }
            else if (mCurrentBreak == BreakModeType.WaitingConfirm)
            {
                if (this.Grid_BreakConfirmCancel.Visibility != System.Windows.Visibility.Visible)
                {
                    this.Grid_BreakConfirmCancel.Visibility = System.Windows.Visibility.Visible;

                    if (TimerMachineStopConfirm != null)
                        TimerMachineStopConfirm.Stop();
                    //this.StartBreakConfirmCancelAnimation();
                }
                else
                {
                    this.Grid_BreakConfirmCancel.Visibility = System.Windows.Visibility.Hidden;
                    //this.StopBreakConfirmCancelAnimation();
                }
            }
            else if (mCurrentBreak == BreakModeType.BreakDown)
            {
                TextBlock_StopPage.Background = Brushes.Red;

                if (BreakPopupView.Visibility != System.Windows.Visibility.Visible)
                {
                    ShowBreak(_availableSelectData);
                }
            }
        }
        #endregion [Textblock available click]

        #region [event break popup view]

        public void Cancel_Break_Event()
        {
            HideBreakView();
            HideAvaliableView();
            if (mCurrentBreak == BreakModeType.None)
            {
                TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
                TextBlock_StopPage.Background = colorEnable;
            }
            else if (mCurrentBreak == BreakModeType.WaitingConfirm)
            {
                TextBlock_StopPage.Background = Brushes.Red;
            }
            else if (mCurrentBreak == BreakModeType.BreakDown)
            {
                TextBlock_StopPage.Background = Brushes.Red;
            }
        }

        public void Complete_Break_Event()
        {
            HideBreakView();
            HideAvaliableView();
            this.WaitingBreakRequest = mCurrentBreak == BreakModeType.None ? BreakWaitingType.Set : BreakWaitingType.Unset;
            isFinishAvailable = true;
            VMT_DataMgr_Common.GetMachineStop_Ask();
        }
        #endregion [event break popup view]

        #region [event change day/night]
        public void Change_Day_Night()
        {
            mMainWindow.gIsDay = mIsDay;
            PresentationMgr.Singleton.ChangeDayMode(PresentationMgr.AppWin.gIsDay);
            HideScreenMode();
            HideAvaliableView();
            TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
            TextBlock_StopPage.Background = colorEnable;
        }
        #endregion [event change day/night]

        public void MainView_ResetAvaliable()
        {
            //if (mCurrentBreak == BreakModeType.BreakDown)
            //{
                mCurrentBreak = BreakModeType.None;
                TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
                TextBlock_StopPage.Background = colorEnable;
                AvailablePopupView.Hide_Popup();
                BreakPopupView.Hide_Popup();
                ChangeDriverPopupView.HidePopup();
            //}
        }

        public void MainView_SetMachineStop(Boolean onlyViewChange = false, bool StopByTOS = false)
        {
            if (onlyViewChange)
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send available = new VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send();
            if (mCurrentBreak == BreakModeType.BreakDown)
            {
                mCurrentBreak = BreakModeType.None;
                {
                    // available.Data = receivedAvailable.AData[availableSelectIndex - COUNT_AVAILABLE_BY_UI - 1];
                    available.Data = _availableSelectData;

                    TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                    if (!String.IsNullOrEmpty(ITV.ITV_User.gServerTime))
                    {
                        String currentServerTime = ITV.ITV_User.gServerTime;
                        //DateTime currentServerDateTime = Convert.ToDateTime(currentServerTime);
                        DateTime currentServerDateTime = default(DateTime);
                        DateTime.TryParse(currentServerTime, out currentServerDateTime);

                        ts = (currentServerDateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0));
                    }

                    available.m_StartTime = AvaliableStartTime;
                    available.m_FinishTime = Convert.ToInt64(ts.TotalSeconds);
                    available.m_iBreakStatus = 0;

                    AvaliableStartTime = 0;

                    if(StopByTOS == false)
                        VMT_DataMgr_Common.SetMachineStop_Ask(ref available);
                }

                TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
                TextBlock_StopPage.Background = colorEnable;

                // 20190703 if not Start -> hide Break UI
                this.MainView_ResetAvaliable();
                HideBreakView();
                HideAvaliableView();
            }
            //else if (mCurrentBreak == BreakModeType.WaitingConfirm)
            //{ 
            //}
            else
            {
                if (UserInfo.IsUseHessian)
                {
                    mCurrentBreak = BreakModeType.BreakDown;
                    TextBlock_StopPage.Text = _availableSelectData.ReasonNm;
                }

                if (UserInfo.IsUseTCP)
                {
                    mCurrentBreak = BreakModeType.WaitingConfirm; // Eagle Eay Mode
                    TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0109", LanguageService.LABEL_CUSTOMIZE);
                }

                {
                    // available.Data = receivedAvailable.AData[availableSelectIndex - COUNT_AVAILABLE_BY_UI - 1];
                    available.Data = _availableSelectData;

                    TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                    if (!String.IsNullOrEmpty(ITV.ITV_User.gServerTime))
                    {
                        String currentServerTime = ITV.ITV_User.gServerTime;
                        //DateTime currentServerDateTime = Convert.ToDateTime(currentServerTime);
                        DateTime currentServerDateTime;
                        DateTime.TryParse(currentServerTime, out currentServerDateTime);
                        ts = (currentServerDateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0));
                    }

                    available.m_StartTime = Convert.ToInt64(ts.TotalSeconds);
                    available.m_FinishTime = 0;
                    available.m_iBreakStatus = 1;

                    AvaliableStartTime = available.m_StartTime;

                    if (StopByTOS == false)
                        VMT_DataMgr_Common.SetMachineStop_Ask(ref available);

                    if (UserInfo.IsUseTCP)
                    {
                        AppSettings.Local.ReasonCd = available.Data.ReasonCd;
                        AppSettings.Local.ReasonNm = available.Data.ReasonNm;
                        AppSettings.Local.StartTime = available.m_StartTime.ToString();
                        AppSettings.Local.Save();
                    }
                }

                TextBlock_StopPage.Background = Brushes.Red;


                if (UserInfo.IsUseTCP)
                {
                    if (TimerMachineStopConfirm == null)
                    {
                        this.TimerMachineStopConfirm = new DispatcherTimer();
                        this.TimerMachineStopConfirm.Interval = new TimeSpan(0, 2, 0); // 2 min                
                        this.TimerMachineStopConfirm.Tick += new EventHandler(TimerMachineStopConfirm_Handler);
                    }
                    TimerMachineStopConfirm.Start();
                }
            }
        }

        #region [button Break Confirm Cancel]
        private void Btn_BreakConfirmCancel_No_Click(object sender, RoutedEventArgs e)
        {
            if (this.Grid_BreakConfirmCancel.Visibility == System.Windows.Visibility.Visible)
            {
                this.Grid_BreakConfirmCancel.Visibility = System.Windows.Visibility.Hidden;
                //this.StopBreakConfirmCancelAnimation();
            }
        }

        private void Btn_BreakConfirmCancel_Yes_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Local.ReasonCd = null;
            AppSettings.Local.ReasonNm = null;
            AppSettings.Local.StartTime = null;
            AppSettings.Local.Save();

            if (this.Grid_BreakConfirmCancel.Visibility == System.Windows.Visibility.Visible)
            {
                this.Grid_BreakConfirmCancel.Visibility = System.Windows.Visibility.Hidden;
                mCurrentBreak = BreakModeType.None;
                TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
                TextBlock_StopPage.Background = colorEnable;
            }
        }
        #endregion [button Break Confirm Cancel]

        #region [Select Container]
        private void ResetSelectedContainer()
        {
            if (this.txt_cntrNoF.Background != Brushes.LightGray)
                this.txt_cntrNoF.Background = Brushes.White;

            if (this.txt_cntrNoA.Background != Brushes.LightGray)
                this.txt_cntrNoA.Background = Brushes.White;
            //selectedContainer = null;
            DisableButton();
        }

        private void Grid_Fore_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (JobOrderForITV_Current.FirstJob.cntr.cntrNo.Length > 0 && JobOrderForITV_Current.FirstJob.spndFlg != "Y")
            {
                ResetSelectedContainer();
                if (this.txt_cntrNoF.Background != Brushes.Yellow)
                {
                    selectedContainer = JobOrderForITV_Current.FirstJob;
                    this.txt_cntrNoF.Background = Brushes.Yellow;
                    EnableMainButtons();
                }
            }
        }
        private void Grid_After_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (JobOrderForITV_Current.SecondJob.cntr.cntrNo.Length > 0 && JobOrderForITV_Current.SecondJob.spndFlg != "Y")
            {
                ResetSelectedContainer();                
                if (this.txt_cntrNoA.Background != Brushes.Yellow)
                {
                    selectedContainer = JobOrderForITV_Current.SecondJob;
                    this.txt_cntrNoA.Background = Brushes.Yellow;
                    EnableMainButtons();
                }
            }
        }

        private void EnableMainButtons()
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder select = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
            if (selectedContainer != null && !string.IsNullOrEmpty(selectedContainer.cntr.cntrNo))
            {
                select = selectedContainer;
            }

            //LD CMPL BUTTON
            if ((!String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.cntr.cntrNo) && !String.IsNullOrEmpty(JobOrderForITV_Current.SecondJob.cntr.cntrNo)
                && JobOrderForITV_Current.FirstJob.pinChkFlg.Equals("Y") && JobOrderForITV_Current.SecondJob.pinChkFlg.Equals("Y")) //2 cntr

                ||

                (!String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.cntr.cntrNo) && String.IsNullOrEmpty(JobOrderForITV_Current.SecondJob.cntr.cntrNo)
                && JobOrderForITV_Current.FirstJob.pinChkFlg.Equals("Y"))

                ||

                (String.IsNullOrEmpty(JobOrderForITV_Current.FirstJob.cntr.cntrNo) && !String.IsNullOrEmpty(JobOrderForITV_Current.SecondJob.cntr.cntrNo)
                && JobOrderForITV_Current.SecondJob.pinChkFlg.Equals("Y"))
                )
            {

                /*if (previousDisableLDCMPL == 1)
                {
                    mMainWindow.BeefPlay_dingdong();
                    previousDisableLDCMPL = 0;
                }*/

                //if(!JobOrderForITV_Current.FirstJob.pinChkFlg.Equals(JobOrderForITV_Prev.FirstJob.pinChkFlg))
                if (btnLDCmplBeep == false)
                {
                    btnLDCmplBeep = true;
                    mMainWindow.BeefPlay_Windows_Notify_System_Generic(2);
                }
            }
            else
            {
                btnLDCmplBeep = false;
                tenTimesBeepLDCMPL = 0;
            }

            //ARRIVAL BUTTON
            if (
                 (
                   (select.type.jobTp.Equals("DS") && select.type.jobStatus.Equals("Q") && (String.IsNullOrEmpty(select.ytJbSts) || select.workingMchn.mchnSts.Equals("L"))) ||
                   ((select.type.jobTp.Equals("LD") || select.type.jobTp.Equals("MI") || select.type.jobTp.Equals("MO") || select.type.jobTp.Equals("LC")) && select.type.jobStatus.Equals("Q")) ||
                   //(select.type.jobTp.Equals("LD") && select.type.jobStatus.Equals("C") && select.ytJbSts.Equals("F")) ||
                   //(select.type.jobTp.Equals("LD") && select.ytJbSts.Equals("B")) ||
                   (select.type.jobTp.Equals("LD") && select.type.jobStatus.Equals("C") && select.ytJbSts.Equals("F") && select.workingMchn.mchnSts.Equals("L"))
                 ) &&
                 (
                   (txt_cntrNoF.Background == Brushes.Yellow && !txt_fmLocationF.Content.ToString().Contains("****") && !txt_toLocationF.Content.ToString().Contains("****")) ||
                   (txt_cntrNoA.Background == Brushes.Yellow && !txt_fmLocationA.Content.ToString().Contains("****") && !txt_toLocationA.Content.ToString().Contains("****"))
                 )
               )
            {
                arrivalCancel = "N";
                TextBlock_Arrival.IsEnabled = true;
                TextBlock_Arrival.Background = colorEnable;
            }
            else
            {
                if (select.type.jobStatus.Equals("A") ||
                    //(select.type.jobTp.Equals("LD") && select.ytJbSts.Equals("B")) ||
                    (select.type.jobTp.Equals("LD") && select.type.jobStatus.Equals("C") && select.ytJbSts.Equals("A") && select.workingMchn.mchnSts.Equals("L")) ||
                    (select.type.jobTp.Equals("DS") && select.type.jobStatus.Equals("Q") && select.ytJbSts.Equals("B"))
                   )
                {
                    // May 31 2023 BLUE Enable and arrivalCancel Y
                    arrivalCancel = "Y";
                    TextBlock_Arrival.IsEnabled = true;
                    TextBlock_Arrival.Background = Brushes.Blue;
                }
                else
                {
                    TextBlock_Arrival.IsEnabled = false;
                    TextBlock_Arrival.Background = colorDisable;
                }
            }

            //DONE BUTTON
            if (
                (!select.type.jobTp.Equals("LD")
                && !select.type.jobTp.Equals("MO")
                && ((select.autoType.Equals("O") && !select.type.jobStatus.Equals("C") && select.is1stPart == true) ||
                    (select.autoType.Equals("F") && (select.type.jobStatus.Equals("A") || select.type.jobStatus.Equals("P") || select.is1stPart == true))
                   )
                )   
               || (select.type.jobTp.Equals("LD") && select.partnerMchn.mchnSts.Equals("C") && select.ytJbSts.Equals("A"))
               )
            {
                TextBlock_Done.IsEnabled = true;
                TextBlock_Done.Background = colorEnable;

                if (btnDoneBeep == false)
                {
                    btnDoneBeep = true;
                    mMainWindow.BeefPlay_Windows_User_Account_Control();
                }
            }
            else
            {
                btnDoneBeep = false;
                TextBlock_Done.IsEnabled = false;
                TextBlock_Done.Background = colorDisable;
            }
        }

        #endregion [Select Container]

        private void TextBlock_ChangeDriver_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mMainWindow.PopupView.ShowPopup(2, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0020", LanguageService.LABEL_CHANGEDRIVER)
               , PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0028", LanguageService.LABEL_CHANGEDRIVER)
               , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0030", LanguageService.MESSAGE_GROUP), ""
               , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), ChangeDriver, 0);
        }

        private void TextBlock_Power_Off_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mMainWindow.PopupView.ShowPopup(2, PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0031", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0033", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0030", LanguageService.MESSAGE_GROUP), ""
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), System_Off, 0);
        }

        public void LogOut()
        {
            mMainWindow.PopupView.ShowPopup(2, PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0036", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0035", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0030", LanguageService.MESSAGE_GROUP), ""
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), CallBack_Popup_LogOff, 0);
        }
        public void ChangeDriver(int selected)
        {
            switch (selected)
            {
                case 0: // left cancel
                    break;
                case 1:                   
                    break;
                case 2: // right Change Driver
                    {
                        VMT_Data_JAT2.VMT_DataMgr_ITV.changeDriver_Ask();
                    }
                    break;
            }
        }
        public void System_Off(int seleted)
        {
            switch (seleted)
            {
                case 0: // left cancel
                    break;
                case 1:
                    {
                    }
                    break;
                case 2: // right system off
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, PresentationMgr.AppWin.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? PresentationMgr.AppWin.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "");
                        System.Diagnostics.Process.Start("shutdown.exe", "-s -f");
                    }
                    break;
            }
        }

        public void Complete_Job_Event()
        {
            int jobCount = VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count;
            if (jobCount > 0)
            {
                for (int i = 0; i < jobCount; i++)
                {
                    VMT_Data_JAT2.VMT_DataMgr_ITV.SetItvDone_Ask((VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[i], null);
                }
            }
        }

        public void Replease_Job_Event()
        {
            String ytNo = UserInfo.gMchnID;
            VMT_Data_JAT2.VMT_DataMgr_Common.ReleaseYtFromJob_Ask(ytNo);
        }
    }
}

