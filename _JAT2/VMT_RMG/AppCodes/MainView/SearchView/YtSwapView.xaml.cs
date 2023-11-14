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
using Common.Interface;
using VMT_Data_JAT2;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for YtSwapView.xaml
    /// </summary>
    public partial class YtSwapView : UserControl
    {      
        //private readonly int _showMachineMaxCount = 4;
        //private int _currentFirstIndex = 0;

        private List<MachineSearchControl> _machineControlItems = null;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();
        private List<Border> _BorderList = new List<Border>();

        //private string _currentITVNumber = string.Empty;
        //private Boolean _isAvailableForDetwin = false;
        private Boolean _validate4LoadingSwap = false;
        public bool JobTpLDorMO = false;
        public String jobKeyChangeYtNo = String.Empty;
        public String oldYtNo = String.Empty;
        public String newYtNo = String.Empty;
        public String currentPositionOnChassis = String.Empty;
        public bool enableItvSwap = false;

        public string ChssPsn
        {
            get
            {
                return this.Btn_Position_Fore.IsChecked == true ? "F" :
                    (this.Btn_Position_After.IsChecked == true ? "A" :
                    (this.Btn_Position_Center.IsChecked == true ? "C" : String.Empty));
            }
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _currentJob = null;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder CurrentJob
        {
            set
            {
                this._currentJob = value;
              
                var job = this._currentJob;
                if (job == null)
                    job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);

                if (job == null || job.type == null)
                {
                    this._validate4LoadingSwap = false;

                    this.Btn_Position_Fore.IsChecked = false;
                    this.Btn_Position_After.IsChecked = false;
                    this.Btn_Position_Center.IsChecked = false;
                } else
                {
                    this._validate4LoadingSwap = //this.Grid_PositionOnChassis.IsEnabled =                         
                job != null && job.type.jobStatus.Equals("P") &&
                (job.type.jobTp.Equals("LD") || job.type.jobTp.Equals("MO"));

                    this.Btn_Position_Fore.IsChecked = job.type.jobFlagInfo.Equals("F");
                    this.Btn_Position_After.IsChecked = job.type.jobFlagInfo.Equals("A");
                    this.Btn_Position_Center.IsChecked = job.type.jobFlagInfo.Equals("C");

                    if (job.cntr.cntrIso.StartsWith("2"))
                    {
                        // Aug 25 Only check Aft / Fore / Center position selection, don't disable OK btn
                        this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = enableItvSwap ? true : false;
                        this.Btn_Position_Center.IsEnabled = false;
                    }
                    else
                    {
                        this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = false;
                        // Aug 25 Only check Aft / Fore / Center position selection, don't disable OK btn
                        this.Btn_Position_Center.IsEnabled = enableItvSwap ? true : false;
                        this.Btn_Position_Center.IsChecked = true;
                    }
                }
                this.EnablePositionOnChassis(this._validate4LoadingSwap);
            }
        }
        
        public YtSwapView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this._machineControlItems = new List<MachineSearchControl>();

            this.UC_KeyPad.ShowTruckKey(true);

            AppendTextBlockList();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.UC_KeyPad.Button_keypad_done_1.Visibility = Visibility.Hidden;
            this.UC_KeyPad.Button_keypad_done_2.Visibility = Visibility.Hidden;
            //this.UC_KeyPad.Button_keypad_abc.Visibility = Visibility.Hidden;
            this.UC_KeyPad.Grid_DigitalPad.Visibility = System.Windows.Visibility.Visible;
            this.UC_KeyPad.Grid_TextPad.Visibility = System.Windows.Visibility.Hidden;
            this.UC_KeyPad.ShowKeyPad(this.Tb_Change);

            // Init Event Handler         
            this.Btn_Position_Fore.Checked += new RoutedEventHandler(Btn_Position_Fore_Checked);
            this.Btn_Position_After.Checked += new RoutedEventHandler(Btn_Position_After_Checked);
            this.Btn_Position_Center.Checked += new RoutedEventHandler(Btn_Position_Center_Checked);
            this.Grid_CANCEL.MouseLeftButtonDown += new MouseButtonEventHandler(Btn_Cancel_Click);
            this.Grid_OK.MouseLeftButtonDown += new MouseButtonEventHandler(Btn_Done_Click);
            this.IsVisibleChanged += YtSwapView_IsVisibleChanged;

            LoadLanguage();         
        }

        private void YtSwapView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if(this.IsVisible == false )
            //    if (PresentationMgr.AppWin.MainWin.wrkCd == "2")
            //        PresentationMgr.AppWin.MainWin.wrkCd = "";
        }

        private void LoadLanguage()
        {
            Tbl_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_YTSWAP);
            Tbl_Current.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_YTSWAP);
            Tbl_Change.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_YTSWAP);
            Tbl_JobTp.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_YTSWAP);
            Tbl_Location.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0005", LanguageService.LABEL_YTSWAP);
            Tbl_Size.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_YTSWAP);
            Tbl_FM.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_YTSWAP);
            Tbl_QCNo.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0008", LanguageService.LABEL_YTSWAP);
            Tbl_Reefer.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0009", LanguageService.LABEL_YTSWAP);
            Tbl_Weight.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0010", LanguageService.LABEL_YTSWAP);
            Tbl_CntrNo.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0011", LanguageService.LABEL_YTSWAP);
            Tbl_POD.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0012", LanguageService.LABEL_YTSWAP);
            Btn_Position_Fore.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0013", LanguageService.LABEL_YTSWAP);
            Btn_Position_After.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0014", LanguageService.LABEL_YTSWAP);
            Btn_Position_Center.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0015", LanguageService.LABEL_YTSWAP);
            Tbl_OK.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0016", LanguageService.LABEL_YTSWAP);
            Tbl_Cancel.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0017", LanguageService.LABEL_YTSWAP);
        }
        private SolidColorBrush _enabledTextBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x48, 0x48));
        private SolidColorBrush _disabledTextBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        private SolidColorBrush enabledBackgorundColor = new SolidColorBrush(Color.FromArgb(255, 62, 62, 62));
        private SolidColorBrush disabledBackgroundColor = new SolidColorBrush(Color.FromArgb(255, 234, 234, 234));
        public void EnablePositionOnChassis(Boolean enable)
        {
            if (this.Btn_Position_Fore.IsChecked == true)
            {
                this.Btn_Position_Fore.Foreground = _disabledTextBrush;
                this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            }
            else if (this.Btn_Position_After.IsChecked == true)
            {
                this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                this.Btn_Position_After.Foreground = _disabledTextBrush;
                this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            }
            else if (this.Btn_Position_Center.IsChecked == true)
            {
                this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                this.Btn_Position_Center.Foreground = _disabledTextBrush;
            }         
        }

        private void Btn_Position_Fore_Checked(object sender, RoutedEventArgs e)
        {
            this.Btn_Position_Fore.Foreground = _disabledTextBrush;
            this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
        }

        private void Btn_Position_After_Checked(object sender, RoutedEventArgs e)
        {
            this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_After.Foreground = _disabledTextBrush;
            this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
        }

        private void Btn_Position_Center_Checked(object sender, RoutedEventArgs e)
        {
            this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_Center.Foreground = _disabledTextBrush;
        }

        public void CheckOkayBtnStatus()
        {
            if (enableItvSwap)
            {
                Grid_OK.IsEnabled = true;
                Grid_OK.Background = enabledBackgorundColor;
            }
            else
            {
                Grid_OK.IsEnabled = false;
                Grid_OK.Background = disabledBackgroundColor;
            }
        }

        private void Btn_Done_Click(object sender, MouseButtonEventArgs e)
        {
            if ( String.IsNullOrEmpty(Tb_Current.Text) && String.IsNullOrEmpty(Tb_Change.Text) )
            {
                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                  , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0126", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0053", LanguageService.MESSAGE_GROUP)
                                  , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                return;
            }
            
            VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
        }

        public void KeypadDone()
        {
            newYtNo = Tb_Change.Text;
            var machineList = DataMgr.Singleton.List_MachineofPool.Machine.ToList();
            Boolean existMachine = false;

            //if (!String.IsNullOrEmpty(machineID))
            {
                //newYtNo = "T" + newYtNo; //20201005 remove + "T"

                foreach (var machine in machineList)
                {
                    if (newYtNo.ToUpper().Equals(machine.mchnId.ToUpper()))
                    {
                        existMachine = true;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(this._currentJob.jobKey) )
                    return;

                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = this._currentJob.jobKey;

                VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send send = new VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send();
                send.cntrNo = this._currentJob.cntr.cntrNo;
                send.partnerMachineID = this._currentJob.partnerMchn.mchnId;
                send.jobid = this._currentJob.jobKey;
                send.positionOnChassis = this.ChssPsn;
                send.externalId = this._currentJob.workingMchn.mchnId;
                send.newYTNo = this._currentJob.partnerMchn.mchnId;
                oldYtNo = this._currentJob.partnerMchn.mchnId;

                if (existMachine)
                {
                    
                    
                    //SAME YT NO (CHANGE AND CURRENT)
                    //if (machineID.Equals(this._currentJob.partnerMchn.mchnId)) 
                    //    return;

                    
                    send.newYTNo = newYtNo;
                    
                }
                else
                {
                    if (JobTpLDorMO && String.IsNullOrEmpty(this._currentJob.partnerMchn.mchnId))
                    {
                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                          , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0126", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0058", LanguageService.MESSAGE_GROUP)
                                          , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                        return;
                    }
                    
                    //String error = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0051", LanguageService.MESSAGE_GROUP);
                    //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Swap failed", error, "OK", null, 0);
                    //this.Tb_Change.Text = String.Empty;
                }

                
                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "DoSwap4Manual_Ask"), send);

                jobKeyChangeYtNo = send.jobid;
                newYtNo = send.newYTNo;
                currentPositionOnChassis = send.positionOnChassis;

                VMT_Data_JAT2.VMT_DataMgr_Common.DoSwap4Manual_Ask(send);
                //PresentationMgr.AppWin.ShowProgressBar(0);
                this.Visibility = System.Windows.Visibility.Hidden;

            }
            //else // YT CHANGE = ""
            //{

            //    if (PresentationMgr.MainView.jobDone)
            //    {
            //        PresentationMgr.MainView.jobDone = false;
            //        var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
            //        PresentationMgr.Singleton.SendJobDoneAsk(target);
            //    }

            //    if (PresentationMgr.AppWin.MainWin.wrkCd == "2")
            //    {
            //        VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
            //    }

            //    this.Visibility = System.Windows.Visibility.Hidden;
            //}
        }     

        public void Btn_Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
            {   
                if (PresentationMgr.AppWin.MainWin.wrkCd == "2")
                {
                    PresentationMgr.AppWin.MainWin.wrkCd = "";
                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common,
                                  "Fail", PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0050a", LanguageService.MESSAGE_GROUP), "OK", null, 0);
                    
                    VMT_DataMgr_RMG.CancelPLC_Ask(PresentationMgr.MainView.plcdomain);
                }
                
                if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                {
                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;
                    if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                        PresentationMgr.Singleton.ThreadTimerStart(false);
                }

                this.Visibility = System.Windows.Visibility.Hidden;

                //if (PresentationMgr.MainView.jobDone)
                //{
                //    PresentationMgr.MainView.jobDone = false;
                //    var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                //    PresentationMgr.Singleton.SendJobDoneAsk(target);
                //}

            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinRadioButton(this.Btn_Position_Fore,
               UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinRadioButton(this.Btn_Position_After,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinRadioButton(this.Btn_Position_Center,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);                           
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinRadioButton(this.Btn_Position_Fore,
               UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinRadioButton(this.Btn_Position_After,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinRadioButton(this.Btn_Position_Center,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonDisableImage);
            }
            RefreshJobListItem();
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();
            _BorderList.Clear();

            IEnumerable<TextBlock> collection = LayoutRoot.Children.OfType<TextBlock>();
            foreach(TextBlock tb in collection)
            {
               // _TextBlockList.Add(tb);
            }

            _TextBlockList.Add(Tbl_Current);
            _TextBlockList.Add(Tbl_Change);
            _TextBlockList.Add(Tbl_JobTp);
            _TextBlockList.Add(Tbl_Location);
            _TextBlockList.Add(Tbl_Size);
            _TextBlockList.Add(Tbl_FM);
            _TextBlockList.Add(Tbl_QCNo);
            _TextBlockList.Add(Tbl_Reefer);
            _TextBlockList.Add(Tbl_Weight);
            _TextBlockList.Add(Tbl_CntrNo);
            _TextBlockList.Add(Tbl_POD);

            _TextBlockList.Add(Tb_Current);
            _TextBlockList.Add(Tb_JobTp);
            _TextBlockList.Add(Tb_Location);
            _TextBlockList.Add(Tb_Size);
            _TextBlockList.Add(Tb_FM);
            _TextBlockList.Add(Tb_QCNo);
            _TextBlockList.Add(Tb_Reefer);
            _TextBlockList.Add(Tb_Weight);
            _TextBlockList.Add(Tb_CntrNo);
            _TextBlockList.Add(Tb_POD);

            _BorderList.Add(bdCenter);
            _BorderList.Add(bd1);
            _BorderList.Add(bd2);
            _BorderList.Add(bd3);
            _BorderList.Add(bd4);
            _BorderList.Add(bd5);
            _BorderList.Add(bd6);
            _BorderList.Add(bd7);
            _BorderList.Add(bd8);
            _BorderList.Add(bd9);
            _BorderList.Add(bd10);

        }

        private void RefreshJobListItem()
        {
            foreach (TextBlock tBlock in _TextBlockList)
            {
                ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                var strRec = rec["TextBox_Foreground_3"];

                if (strRec is SolidColorBrush)
                    tBlock.Foreground = strRec as SolidColorBrush;
            }

            foreach (Border bd in _BorderList)
            {
                ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                var strRec = rec["Gird_Background_2"];

                if (strRec is SolidColorBrush)
                    bd.BorderBrush = strRec as SolidColorBrush;
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
        public void SetJobInfo(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job)
        {
            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            this.Tb_Current.Text = job.partnerMchn.mchnId;
            this.Tb_JobTp.Text = lang.Contains("Korea") ? job.jobTpKor : job.type.jobTp;
            if (PresentationMgr.UseFromLocationForRehandling == true && (job.type.jobTp == "RH" || job.type.jobTp == "AH") && job.type.jobStatus != "P")
                this.Tb_Location.Text = job.locFrom.location;
            else if (job.type.jobTp == "MO" || job.type.jobTp == "LD" || job.type.jobTp == "GO")
                this.Tb_Location.Text = string.IsNullOrEmpty(job.locFrom.location) ? job.locWorking.location : job.locFrom.location;
            else
                this.Tb_Location.Text = job.locWorking.location;
            if (job.type.jobTp == "LD" || job.type.jobTp == "MO")
                JobTpLDorMO = true;
            else
                JobTpLDorMO = false;
            this.Tb_Location.Text = this.Tb_Location.Text.Replace("-", "");
            this.Tb_Size.Text = job.cntr.cntrIso;
            String fm = job.cntr.fullMty;
            this.Tb_FM.Text = fm + (fm.Equals("F") ? "/" + job.cntr.cntrWgt : fm.Equals("E") ? "/" + job.cntr.opr : String.Empty);
            this.Tb_QCNo.Text = job.type.qcId;
            this.Tb_Reefer.Text = job.cntr.cntrCgoTp.Equals("R") ? "Y" : "N";
            this.Tb_Weight.Text = job.cntr.cntrWgt;
            this.Tb_CntrNo.Text = job.cntr.cntrNo;
            this.Tb_POD.Text = job.podCd;
            this.Tb_Change.Text = String.Empty;
            this.Tb_Change.Focus();
        }
    }
}
