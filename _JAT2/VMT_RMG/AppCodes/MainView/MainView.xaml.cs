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
using System.ComponentModel;

//20190108
using Common.Interface;
using System.IO;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public enum BtnJobSetType
        {
            BtnJobSetType_Unknown,
            BtnJobSetType_Normal,
            BtnJobSetType_Processing,
            BtnJobSetType_Lock
        }
        private bool _cntrLockMode = false;
        public bool cntrLockMode
        {
            get
            {
                return _cntrLockMode;
            }
            set
            {
                _cntrLockMode = value;
                if (value == true)
                {
                    this.Image_Lock.Source = UIThemeMgr.Day.IconLock;
                }
                else
                {
                    this.Image_Lock.Source = UIThemeMgr.Day.IconUnlock;
                }
            }
        }
        public string currentBlock;
        public string notice = "";
        public string preNotice = "";
        public Boolean jobDone = false;
        public Boolean selectedJobList = false;
        public Boolean deselectJobList = false;
        public Boolean selectedJobList1Time = false;
        public Boolean blckAllSwitch = false;
        public static bool contItmSelected = false;
        public VMT_Data_JAT2.Marshalling.Geometry.sPosition prevPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();

        public VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain plcdomain = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain();

        public VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain plcDomainTwistLock = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain();
        public VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain plcDomainTwistLockPrv = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain();

        private readonly BitmapImage _jobSetDefaultDayImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative));
        private readonly BitmapImage _jobSetDefaultNightImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative));

        private readonly BitmapImage _jobSetEnableDayImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative));
        private readonly BitmapImage _jobSetEnableNightImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative));

        private readonly BitmapImage _jobSetLockDayImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Lock.png", UriKind.Relative));
        private readonly BitmapImage _jobSetLockNightImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Lock.png", UriKind.Relative));

        private BtnJobSetType _BtnJobSetType = BtnJobSetType.BtnJobSetType_Unknown;
        //public BtnJobSetType SetBtnJobSetType(BtnJobSetType bType)
        //{
        //    _BtnJobSetType = bType;

        //    Image imgCheck = this.Btn_JobSet.Template.FindName("Image_Check", this.Btn_JobSet) as Image;
        //    Image imgUncheck = this.Btn_JobSet.Template.FindName("Image_Uncheck", this.Btn_JobSet) as Image;
        //    Image imgDisable = this.Btn_JobSet.Template.FindName("Image_Disable", this.Btn_JobSet) as Image;

        //    if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Normal)
        //    {
        //        if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
        //        {
        //            if (imgCheck != null) imgCheck.Source = this._jobSetDefaultDayImg;
        //            if (imgUncheck != null) imgUncheck.Source = this._jobSetDefaultDayImg;
        //            if (imgDisable != null) imgDisable.Source = this._jobSetDefaultDayImg;
        //        }
        //        else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
        //        {
        //            if (imgCheck != null) imgCheck.Source = this._jobSetDefaultNightImg;
        //            if (imgUncheck != null) imgUncheck.Source = this._jobSetDefaultNightImg;
        //            if (imgDisable != null) imgDisable.Source = this._jobSetDefaultNightImg;
        //        }
        //    }
        //    else if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Processing)
        //    {
        //        if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
        //        {
        //            if (imgCheck != null) imgCheck.Source = this._jobSetEnableDayImg;
        //            if (imgUncheck != null) imgUncheck.Source = this._jobSetEnableDayImg;
        //            if (imgDisable != null) imgDisable.Source = this._jobSetEnableDayImg;
        //        }
        //        else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
        //        {
        //            if (imgCheck != null) imgCheck.Source = this._jobSetEnableNightImg;
        //            if (imgUncheck != null) imgUncheck.Source = this._jobSetEnableNightImg;
        //            if (imgDisable != null) imgDisable.Source = this._jobSetEnableNightImg;
        //        }
        //    }
        //    else if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Lock)
        //    {
        //        if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
        //        {
        //            if (imgCheck != null) imgCheck.Source = this._jobSetLockDayImg;
        //            if (imgUncheck != null) imgUncheck.Source = this._jobSetLockDayImg;
        //            if (imgDisable != null) imgDisable.Source = this._jobSetLockDayImg;
        //        }
        //        else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
        //        {
        //            if (imgCheck != null) imgCheck.Source = this._jobSetLockNightImg;
        //            if (imgUncheck != null) imgUncheck.Source = this._jobSetLockNightImg;
        //            if (imgDisable != null) imgDisable.Source = this._jobSetLockNightImg;
        //        }
        //    }

        //    return _BtnJobSetType;
        //}

        public BtnJobSetType SetBtnJobSetType(BtnJobSetType bType)
        {
            _BtnJobSetType = bType;

            if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Normal)
            {
                this.Label_JobSet.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#444444"));
                this.Label_JobSet.Foreground = new SolidColorBrush(Colors.White);
            }
            else if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Processing)
            {
                this.Label_JobSet.Background = new SolidColorBrush(Colors.Red);
                this.Label_JobSet.Foreground = new SolidColorBrush(Colors.White);
            }
            else if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Lock)
            {
                this.Label_JobSet.Background = new SolidColorBrush(Colors.LightGray);
                this.Label_JobSet.Foreground = new SolidColorBrush(Colors.White);
            }

            return _BtnJobSetType;
        }

        public MainView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
                new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
        }

        ~MainView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            //this.Grid_Block_All.MouseDown += new MouseButtonEventHandler(Grid_Block_All_Click);
            //this.CheckBox_Block_All.Click += new RoutedEventHandler(Grid_Block_All_Click);
            this.Lb_Block.MouseDown += new MouseButtonEventHandler(Lb_Block_MouseDown);
            this.Lb_All.MouseDown += new MouseButtonEventHandler(Lb_All_MouseDown);

            this.Btn_Block.Click += new RoutedEventHandler(Btn_Block_Click);
            this.Btn_BlockText.Click += new RoutedEventHandler(Btn_Block_Click);
            this.Btn_All.Click += new RoutedEventHandler(Btn_All_Click);
            this.Btn_JobSet.Click += new RoutedEventHandler(Btn_JobSet_Click);

            this.Btn_Available.Click += new RoutedEventHandler(Btn_Available_Click);
            this.Btn_Notice.Click += new RoutedEventHandler(Btn_Notice_Click);
            //this.Btn_Navigator.Click += new RoutedEventHandler(Btn_Navigator_Click);
            this.Btn_Chg_Loc.Click += new RoutedEventHandler(Btn_Chg_Loc_Click);
            this.Label_JobSet.MouseLeftButtonUp += new MouseButtonEventHandler(Btn_JobSet_Click);
            this.Btn_JobDone.Click += new RoutedEventHandler(Btn_JobDone_Click);
            this.Btn_Search.Click += new RoutedEventHandler(Btn_Search_Click);

            this.Btn_Left.Click += new RoutedEventHandler(Btn_Left_Click);
            this.Btn_Right.Click += new RoutedEventHandler(Btn_Right_Click);

            this.TbCntrNo.GotFocus += new RoutedEventHandler(TbCntrNo_GotFocus);
            this.TbCntrNo.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TbCntrNo_PreviewMouseLeftButtonDown);

            //if (MainWindow.TEST_MODE == true)
            //    this.Btn_Chg_Loc.IsEnabled = true;
            this.Btn_Chg_Loc.IsEnabled = false;

            Btn_JobSet.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0123", LanguageService.LABEL_CUSTOMIZE);
            Btn_Search.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0019", LanguageService.LABEL_MAINWINDOW);
            Btn_Available.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0231", LanguageService.LABEL_CUSTOMIZE);
            Btn_Notice.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0219", LanguageService.LABEL_CUSTOMIZE);
            Btn_Chg_Loc.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0061", LanguageService.LABEL_MAINWINDOW);
            Label_JobSet.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0062", LanguageService.LABEL_MAINWINDOW);
            Btn_JobDone.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0124", LanguageService.LABEL_CUSTOMIZE);

            Image_Lock.MouseLeftButtonDown += new MouseButtonEventHandler(Image_Lock_MouseLeftButtonDown);
            Lbl_Navigator_On.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0074", LanguageService.LABEL_MAINWINDOW);
            Lbl_On.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0075", LanguageService.LABEL_MAINWINDOW);
            Lbl_Navigator_Off.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0074", LanguageService.LABEL_MAINWINDOW);
            Lbl_Off.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0076", LanguageService.LABEL_MAINWINDOW);
            Lb_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0077", LanguageService.LABEL_MAINWINDOW);
            Lb_All.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0078", LanguageService.LABEL_MAINWINDOW);

            Button_TwoButton_Left.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0027", LanguageService.LABEL_POPUP);
            Button_TwoButton_Right.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_POPUPOUT);
            TextBlock_popup_title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0040", LanguageService.LABEL_POPUP);
            TextBlock_popup_message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0049", LanguageService.MESSAGE_GROUP);
        }

        public void RMG_Member_PropertyChanged_TargetJobKey(object sender, PropertyChangedEventArgs e)
        {
            this.CheckupButtonStatus();
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerSearch);
        }

        public void CheckupButtonStatus()
        {
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (!String.IsNullOrEmpty(jobKey))
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                if (jobOrder != null)
                {
                    Btn_JobDone.IsEnabled = bBtnDoneEnableManual &&
                        (jobOrder.type.jobStatus == "P") &&
                        ((PresentationMgr.Singleton.IsTwistLock != true))
                         ? true : false;

                    // 2016-01-27 Inactive Jobset 할 수 있도록
                    Btn_JobSet.IsEnabled =
                        //jobOrder.type.jobStatus == "P" || String.IsNullOrEmpty(PresentationMgr.Singleton.ProcessingJobKey);
                        (jobOrder.type.jobStatus == "P" && jobOrder.workingMchn.mchnId.Equals(VMT_Data_JAT2.Objects.UserInfo.gMchnID))
                        || (jobOrder.type.jobStatus != "P" && String.IsNullOrEmpty(PresentationMgr.Singleton.ProcessingJobKey));

                    Btn_JobSet.IsChecked = jobOrder.type.jobStatus == "P" ? true : false;

                    if (jobOrder.type.jobStatus == "P" &&
                        PresentationMgr.Singleton.IsTwistLock == true)
                    {
                        this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Lock);
                    }
                    else if (Btn_JobSet.IsChecked == true)
                    {
                        this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Processing);
                    }
                    else// if (Btn_JobSet.IsChecked == false)
                    {
                        this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Normal);
                    }

                    switch (jobOrder.type.jobTp)
                    {
                        case "GI":
                        case "MI":
                        case "DS":
                        case "RH":
                        case "AH":
                        case "LC":
                        case "GC":
                            Btn_JobDone.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0124", LanguageService.LABEL_CUSTOMIZE);
                            break;
                        case "GO":
                        case "MO":
                        case "LD":
                            Btn_JobDone.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0123", LanguageService.LABEL_CUSTOMIZE);
                            break;
                    }

                    return;
                }
            }
            // job item NOT selected
            this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Normal);
            Btn_JobSet.IsEnabled = false;
            Btn_JobDone.IsEnabled = false;
        }

        private void SelectFirstJob()
        {
            if (this.UC_JobList.ListBox_Job.Items.Count > 0)
            {
                JobListItem item = this.UC_JobList.ListBox_Job.Items.GetItemAt(0) as JobListItem;
                if (item != null)
                {
                    var job = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                    if (job != null)
                    {
                        UC_JobList.ListBox_Job.SelectedIndex = 0;
                        if (UC_JobList.ListBox_Job.SelectedItem != null)
                        {
                            (UC_JobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = job.jobKey;
                        }
                    }
                }
            }
        }

        private void Grid_Block_All_Click(object sender, RoutedEventArgs e)
        {
            var blckFilter = Convert.ToString(UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
            if (String.IsNullOrEmpty(blckFilter))
            {
                //CheckBox_Block_All.IsChecked = false;
                return;
            }
            //this.CheckBox_Block_All.IsChecked = (this.CheckBox_Block_All.IsChecked == true) ? false : true;
            //this.UC_JobList.CurrentFilter.FilterJobBlock = (this.CheckBox_Block_All.IsChecked == true) ? true : false;

            PresentationMgr.Singleton.NeedJobAutoSelection = true;
            if (this.UC_JobList.CurrentFilter.FilterJobBlock)
            {
                this.currentBlock = blckFilter;
                VMT_Data_JAT2.Objects.Common.BlckVal = blckFilter;
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, blckFilter, false, true);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            }
            else
            {
                //PresentationMgr.Singleton.JL_Refresh(this.UC_JobList);
                VMT_Data_JAT2.Objects.Common.BlckVal = "";
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, false);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            }
        }
        private void Lb_All_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.UC_JobList.CurrentFilter.FilterJobBlock)
                return;
            SetAllMode();
        }
        private void Lb_Block_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.UC_JobList.CurrentFilter.FilterJobBlock)
                return;
            SetBlockMode();
        }
        public void SetAllMode()
        {
            blckAllSwitch = true;
            this.UC_JobList.CurrentFilter.FilterJobBlock = false;
            this.Lb_All.Background = Brushes.Black;
            this.Lb_All.Foreground = Brushes.White;
            this.Lb_Block.Background = Brushes.LightGray;
            this.Lb_Block.Foreground = Brushes.Black;

            PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = String.Empty;
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
            PresentationMgr.Singleton.CorrectionSource.Clear();
            PresentationMgr.MainView.UC_JobList.CurrentPageIndex = 0;
            VMT_Data_JAT2.Objects.Common.BlckVal = "";
            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
        }
        public void SetAllModeWithoutStartPolling()
        {
            blckAllSwitch = true;
            this.UC_JobList.CurrentFilter.FilterJobBlock = false;
            this.Lb_All.Background = Brushes.Black;
            this.Lb_All.Foreground = Brushes.White;
            this.Lb_Block.Background = Brushes.LightGray;
            this.Lb_Block.Foreground = Brushes.Black;

            PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = String.Empty;
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
            PresentationMgr.Singleton.CorrectionSource.Clear();
            PresentationMgr.MainView.UC_JobList.CurrentPageIndex = 0;
            VMT_Data_JAT2.Objects.Common.BlckVal = "";           
        }
        public void SetBlockMode(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job = null)
        {
            blckAllSwitch = true;
            this.UC_JobList.CurrentFilter.FilterJobBlock = true;
            this.Lb_All.Background = Brushes.LightGray;
            this.Lb_All.Foreground = Brushes.Black;
            this.Lb_Block.Background = Brushes.Black;
            this.Lb_Block.Foreground = Brushes.White;

            PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = (job != null ? job.jobKey : String.Empty);
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = (job != null ? job.jobKey : String.Empty);
            PresentationMgr.Singleton.CorrectionSource.Clear();
            PresentationMgr.MainView.UC_JobList.CurrentPageIndex = 0;
            var blckFilter = Convert.ToString(UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
            this.currentBlock = blckFilter;
            VMT_Data_JAT2.Objects.Common.BlckVal = blckFilter;
            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
        }
        private void Btn_All_Click(object sender, RoutedEventArgs e)
        {
            Btn_BlockText.Visibility = Visibility.Hidden;
            Btn_Block.Visibility = Visibility.Visible;
            this.UC_JobList.CurrentFilter.FilterJobBlock = false;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_All,
                UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_All,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);
            }
            VMT_Data_JAT2.Objects.Common.BlckVal = "";
            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, true);
            VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
        }

        private void Btn_Block_Click(object sender, RoutedEventArgs e)
        {
            UC_BlockPopupView.ShowPopup(new BlockPopupView.Callback_BlockPopup(CallbackClosePopup));
        }

        public void CallbackClosePopup(BlockPopupView.UC_BlockViewRetType selected)
        {
            switch (selected)
            {
                case BlockPopupView.UC_BlockViewRetType.UC_PopupViewRetType_ClickButtonLeft:
                    break;
                case BlockPopupView.UC_BlockViewRetType.UC_PopupViewRetType_ClickButtonRight:
                    {
                        if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                        {
                            PresentationMgr.SetSkinButton(this.Btn_All,
                            UIThemeMgr.Day.LoginView_ButtonNightDefaultImage, UIThemeMgr.Day.LoginView_ButtonNightDefaultImage, UIThemeMgr.Day.LoginView_ButtonNightDefaultImage);

                        }
                        else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                        {
                            PresentationMgr.SetSkinButton(this.Btn_All,
                                UIThemeMgr.Night.LoginView_ButtonNightDefaultImage, UIThemeMgr.Night.LoginView_ButtonNightDefaultImage, UIThemeMgr.Night.LoginView_ButtonNightDefaultImage);
                        }
                        Btn_Block.Visibility = Visibility.Hidden;
                        Btn_BlockText.Visibility = Visibility.Visible;
                        //this.UC_JobList.CurrentFilter.FilterJobBlock = true;
                        this.currentBlock = this.UC_BlockPopupView.TextBox_Block.Text;
                        Btn_BlockText.Content = this.UC_BlockPopupView.TextBox_Block.Text;
                        //PresentationMgr.Singleton.JL_Refresh(this.UC_JobList);
                        VMT_Data_JAT2.Objects.Common.BlckVal = this.UC_BlockPopupView.TextBox_Block.Text;
                        VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                        //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, this.UC_BlockPopupView.TextBox_Block.Text, false, false);
                        VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Btn_JobSet_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Btn_JobSet.IsEnabled)
                return;
            //throw new NotImplementedException();
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (String.IsNullOrEmpty(jobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null || jobOrder.type == null)
                return;

            if (jobOrder.type.jobStatus == "P")
            {
                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "SetJobStatus_Ask"), jobOrder.jobKey + ", false");

                //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.jobKey, false);
                //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                //    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.type.ycTwinKey, false);

                var jobKeyList = new System.Collections.ArrayList();    // List<string>();
                jobKeyList.Add(jobOrder.jobKey);
                //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                //{
                //    var twinJob = PresentationMgr.Singleton.JOB_Get(jobOrder.type.ycTwinKey);
                //    if (twinJob != null && twinJob.type != null && twinJob.type.jobStatus == "P")
                //        jobKeyList.Add(jobOrder.type.ycTwinKey);
                //}
                //Aug 17 Add to List to Sort after JobSet
                // Aug 19 Rollback this logic
                //if (PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Find(x => jobKeyList.Contains(x.JobKey)) == null)
                //    PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Clear();
                //foreach (String jobSetJobKey in jobKeyList)
                //{                  
                //    if (PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Find(x => x.JobKey == jobSetJobKey) == null)
                //    {
                //        PresentationMgr.JobKeyObjToCheckAfterJobSet jobSetCheckJob = new PresentationMgr.JobKeyObjToCheckAfterJobSet();
                //        jobSetCheckJob.type = PresentationMgr.JobSetCheckType.JobSetType;
                //        jobSetCheckJob.JobKey = jobSetJobKey;
                //        PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Add(jobSetCheckJob);
                //    }
                //}
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, false);

                PresentationMgr.AppWin.ShowProgressBar(0);

                PresentationMgr.Singleton.RefreshJobInventory(jobOrder, false);
            }
            else if (jobOrder.type.jobStatus != "C")//if (jobOrder.type.jobStatus == "Q" || jobOrder.type.jobStatus == "A")
            {
                //SaveLog("Btn_JobSet_Click");
                // 2017-02-02 : 제거요청
                //if ((jobOrder.type.jobTp == "DS" //|| jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "MI"       // 하차 작업시
                //    || jobOrder.type.jobTp == "LC" || jobOrder.type.jobTp == "GC") &&
                //    (jobOrder.type.jobFlagInfo == "F" || jobOrder.type.jobFlagInfo == "A"))
                //{
                //    // show vehicle view                    
                //    //PresentationMgr.MainView.UC_VehiclePositionView.CurrentJobOrder = jobOrder;
                //    PresentationMgr.MainView.UC_VehiclePositionView.CurrentJobKey = jobOrder.jobKey;
                //    PresentationMgr.MainView.UC_VehiclePositionView.Visibility = Visibility.Visible;
                //}
                //else
                {
                    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "SetJobStatus_Ask"), jobOrder.jobKey + ", true");

                    //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.jobKey, true);
                    //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.type.ycTwinKey, true);

                    var jobKeyList = new System.Collections.ArrayList();    // List<string>();
                    jobKeyList.Add(jobOrder.jobKey);
                    if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                        jobKeyList.Add(jobOrder.type.ycTwinKey);
                    //Aug 17 Add to List to Sort after JobSet
                    // Aug 19 Rollback this logic
                    //if (PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Find(x => jobKeyList.Contains(x.JobKey)) == null)
                    //    PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Clear();
                    //foreach (String jobSetJobKey in jobKeyList)
                    //{
                    //    if (PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Find(x => x.JobKey == jobSetJobKey) == null)
                    //    {
                    //        PresentationMgr.JobKeyObjToCheckAfterJobSet jobSetCheckJob = new PresentationMgr.JobKeyObjToCheckAfterJobSet();
                    //        jobSetCheckJob.type = PresentationMgr.JobSetCheckType.JobSetType;
                    //        jobSetCheckJob.JobKey = jobSetJobKey;
                    //        PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Add(jobSetCheckJob);
                    //    }                      
                    //}
                    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, true);

                    PresentationMgr.AppWin.ShowProgressBar(0);

                    PresentationMgr.Singleton.RefreshJobInventory(jobOrder, true);
                }
            }
        }

        public void SaveLog(string sJob)  // nDataType 0 EEv2JobOrder, 
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

                string logFilePath = @sDirPath + "/RTG_LOG_" + System.DateTime.Now.Hour + ".txt";

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

        //public void Callback
        private void Image_Lock_MouseLeftButtonDown(object senser, MouseButtonEventArgs e)
        {
            cntrLockMode = !cntrLockMode;
        }

        private void Btn_Navigator_Click(object sender, RoutedEventArgs e)
        {
            if (this.Btn_Navigator.IsChecked == true)
            {
                this.UC_NavigatorView.Visibility = System.Windows.Visibility.Visible;
                this.UC_NavigatorView.IsWaitingToHide = false;
            }
            else
            {
                this.UC_NavigatorView.Visibility = System.Windows.Visibility.Hidden;
                this.UC_NavigatorView.IsWaitingToHide = true;
                PresentationMgr.Singleton.SetInventoryData(null);
            }
        }

        private void Grid_Navigator_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Grid_Navigator_Off.Visibility == System.Windows.Visibility.Visible)
            {
                Grid_Navigator_Off.Visibility = System.Windows.Visibility.Hidden;
                Grid_Navigator_On.Visibility = System.Windows.Visibility.Visible;
                this.UC_NavigatorView.Visibility = System.Windows.Visibility.Visible;
                this.UC_NavigatorView.IsWaitingToHide = false;
            }
            else
            {
                Grid_Navigator_Off.Visibility = System.Windows.Visibility.Visible;
                Grid_Navigator_On.Visibility = System.Windows.Visibility.Hidden;
                this.UC_NavigatorView.Visibility = System.Windows.Visibility.Hidden;
                this.UC_NavigatorView.IsWaitingToHide = true;
                PresentationMgr.Singleton.SetInventoryData(null);
            }
        }

        private void Btn_Available_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Btn_Available.Content.Equals(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0231", LanguageService.LABEL_CUSTOMIZE)))
            {
                if (PresentationMgr.MainView.UC_AvailableView.Wrap_AvailableView.Children.Count <= 0)
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStopCodeList_Ask();

                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.AvailableView);
            }
            else
            {
                //PresentationMgr.MainView.UC_BreakTimeView.SetEndDateTime();
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BreakTimeView);
            }
        }
        public void SetBtnNotice()
        {
            if (!String.IsNullOrEmpty(this.notice) && this.notice != this.preNotice)
            {
                this.Btn_Notice.Foreground = Brushes.Red;
                this.Btn_Notice.IsEnabled = true;
            }
            else if (String.IsNullOrEmpty(this.notice))
            {
                this.Btn_Notice.IsEnabled = false;
            }
            else
            {
                this.Btn_Notice.IsEnabled = true;
            }
        }
        private void Btn_Notice_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0038", LanguageService.LABEL_POPUP), this.notice, "OK", null, 0);
            this.Btn_Notice.Foreground = Brushes.Black;
            this.preNotice = this.notice;
        }

        void Btn_Chg_Loc_Click(object sender, RoutedEventArgs e)
        {

            //PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.CorrectionView);
            var block = Convert.ToString(this.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
            var bay = Convert.ToString(this.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text);
            if (!String.IsNullOrEmpty(block) && !String.IsNullOrEmpty(bay))
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetChangePosition_ask(block, bay);
        }

        public void Btn_JobDone_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            {
                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                if (target != null && target.type != null)
                {
                    PresentationMgr.AppWin.wrkCdPLC = "2";
                    this.bBtnDoneEnableManual = false;
                    this.Btn_JobDone.IsEnabled = false;
                    new PresentationMgr.SingleShot(1000, BtnDoneEnableManual);

                    //if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                    //{
                    //    var twin = PresentationMgr.Singleton.JOB_Get(target.type.ycTwinKey);
                    //    if (null != twin && twin.type != null && twin.type.jobStatus == "P")
                    //    {
                    //        var targetLoc = target.locWorking;
                    //        var twinLoc = twin.locWorking;
                    //        if (!string.IsNullOrEmpty(targetLoc.bay) && !string.IsNullOrEmpty(twinLoc.bay))
                    //        {
                    //            // 2017-01-12 : 정호진 수석 요청
                    //            // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                    //            if ((target.type.jobFlagInfo.Equals("F") && Convert.ToInt32(targetLoc.bay) < Convert.ToInt32(twinLoc.bay)) ||
                    //                (target.type.jobFlagInfo.Equals("A") && Convert.ToInt32(targetLoc.bay) > Convert.ToInt32(twinLoc.bay)))
                    //            {
                    //                target.locWorking = twinLoc;
                    //                twin.locWorking = targetLoc;
                    //            }
                    //        }

                    //        PresentationMgr.Singleton.SendJobDoneAsk(twin);
                    //    }
                    //}
                    PresentationMgr.Singleton.SendJobDoneAsk(target);
                }
            }
        }
        public void SetJobDoneToDiffLocation(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos)
        {
            if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            {
                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                if (target != null && target.type != null)
                {
                    PresentationMgr.AppWin.wrkCdPLC = "2";
                    this.bBtnDoneEnableManual = false;
                    this.Btn_JobDone.IsEnabled = false;
                    new PresentationMgr.SingleShot(1000, BtnDoneEnableManual);

                    //if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                    //{
                    //    var twin = PresentationMgr.Singleton.JOB_Get(target.type.ycTwinKey);
                    //    if (null != twin && twin.type != null && twin.type.jobStatus == "P")
                    //    {
                    //        var targetLoc = target.locWorking;
                    //        var twinLoc = twin.locWorking;
                    //        if (!string.IsNullOrEmpty(targetLoc.bay) && !string.IsNullOrEmpty(twinLoc.bay))
                    //        {
                    //            // 2017-01-12 : 정호진 수석 요청
                    //            // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                    //            if ((target.type.jobFlagInfo.Equals("F") && Convert.ToInt32(targetLoc.bay) < Convert.ToInt32(twinLoc.bay)) ||
                    //                (target.type.jobFlagInfo.Equals("A") && Convert.ToInt32(targetLoc.bay) > Convert.ToInt32(twinLoc.bay)))
                    //            {
                    //                target.locWorking = twinLoc;
                    //                twin.locWorking = targetLoc;
                    //            }
                    //        }
                    //        PresentationMgr.Singleton.SendJobDoneAsk(twin);
                    //    }
                    //}
                    PresentationMgr.Singleton.SendJobDoneToDiffLocation(target, pos);
                }
            }
         
        }

        private Boolean bBtnDoneEnableManual = true;
        private void BtnDoneEnableManual()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            this.bBtnDoneEnableManual = true;
                            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                            if (!String.IsNullOrEmpty(jobKey))
                            {
                                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                                if (jobOrder != null)
                                {
                                    //var isSiemensConnected = PresentationMgr.AppWin.UC_IndicatorView.CheckBox_Siemens.IsChecked;
                                }
                            }
                        }));
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                //PresentationMgr.SetSkinCheckBox(this.CheckBox_Block_All,
                //    UIThemeMgr.Day.ButtonBlockOnImage, UIThemeMgr.Day.ButtonBlockOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Block_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Block_Off.png", UriKind.Relative))
                //);
                PresentationMgr.SetSkinButton(this.Btn_Block,
                   UIThemeMgr.Day.LoginView_ButtonNightDefaultImage, UIThemeMgr.Day.LoginView_ButtonNightDefaultImage, UIThemeMgr.Day.LoginView_ButtonNightDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                   UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_All,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

                this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Normal);

                //PresentationMgr.SetSkinCheckBox(this.Btn_JobSet,
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative))
                //    );

                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator,
                    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Chg_Loc,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_JobDone,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Available,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Notice,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Left,
                     UIThemeMgr.Day.ButtonLeftDefaultImage, UIThemeMgr.Day.ButtonLeftPressImage, UIThemeMgr.Day.ButtonLeftDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Left_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Right,
                   UIThemeMgr.Day.ButtonRightDefaultImage, UIThemeMgr.Day.ButtonRightPressImage, UIThemeMgr.Day.ButtonRightDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Right_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Button_TwoButton_Left,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Button_TwoButton_Right,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);

                Lbl_Off.Background = Brushes.DarkGray;
                Lbl_Off.Foreground = Brushes.White;
                Lbl_Navigator_Off.Background = Brushes.WhiteSmoke;
                Lbl_Navigator_Off.Foreground = Brushes.Black;
                Lbl_Navigator_On.Background = Brushes.WhiteSmoke;
                Lbl_Navigator_On.Foreground = Brushes.Black;
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                //PresentationMgr.SetSkinCheckBox(this.CheckBox_Block_All,
                //    UIThemeMgr.Night.ButtonBlockOnImage, UIThemeMgr.Night.ButtonBlockOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Block_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Block_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Block,
                    UIThemeMgr.Night.LoginView_ButtonNightDefaultImage, UIThemeMgr.Night.LoginView_ButtonNightDefaultImage, UIThemeMgr.Night.LoginView_ButtonNightDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_All,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);

                this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Normal);

                //PresentationMgr.SetSkinCheckBox(this.Btn_JobSet,
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative))
                //    );

                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator,
                    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Chg_Loc,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Available,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Notice,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_JobDone,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Left,
                    UIThemeMgr.Night.ButtonLeftDefaultImage, UIThemeMgr.Night.ButtonLeftPressImage, UIThemeMgr.Night.ButtonLeftDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Right,
                    UIThemeMgr.Night.ButtonRightDefaultImage, UIThemeMgr.Night.ButtonRightPressImage, UIThemeMgr.Night.ButtonRightDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Button_TwoButton_Left,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Button_TwoButton_Right,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);

                Lbl_Off.Background = Brushes.DimGray;
                Lbl_Off.Foreground = Brushes.DarkGray;
                Lbl_Navigator_Off.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#262828"));
                Lbl_Navigator_Off.Foreground = Brushes.White;
                Lbl_Navigator_On.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#262828"));
                Lbl_Navigator_On.Foreground = Brushes.White;
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void Button_TwoButton_Left_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.TbCntrNo.Text)) return;
            inputPopup.Visibility = System.Windows.Visibility.Hidden;
            plcdomain.cntrNo = this.TbCntrNo.Text;
            VMT_Data_JAT2.VMT_DataMgr_RMG.ProcessPLC_Ask(plcdomain);
            this.TbCntrNo.Text = String.Empty;
            KeypadDone();
        }

        private void Button_TwoButton_Right_Click(object sender, RoutedEventArgs e)
        {
            inputPopup.Visibility = System.Windows.Visibility.Hidden;
            VMT_Data_JAT2.VMT_DataMgr_RMG.CancelPLC_Ask(plcdomain);

            PresentationMgr.AppWin.MainWin.wrkCd = "";
            this.TbCntrNo.Text = String.Empty;
            plcdomain = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain();
            KeypadDone();
        }
        private void TbCntrNo_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.TbCntrNo);
            this.TbCntrNo.Focus();
        }
        private void TbCntrNo_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.TbCntrNo);
            this.TbCntrNo.Focus();
        }
        private void KeypadDone()
        {
            PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
        }

        public void Btn_Left_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.MainView.UC_NavigatorView.Btn_Left_Click(null, null);
            PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
            PresentationMgr.Singleton.SetInventoryData(null);
        }

        public void Btn_Right_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.MainView.UC_NavigatorView.Btn_Right_Click(null, null);
            PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
            PresentationMgr.Singleton.SetInventoryData(null);
        }
    }
}