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
using VMT_RMG;

namespace VMT_RMG_800by600
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

        private readonly BitmapImage _jobSetDefaultDayImg = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative));
        private readonly BitmapImage _jobSetDefaultNightImg = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative));

        private readonly BitmapImage _jobSetEnableDayImg = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative));
        private readonly BitmapImage _jobSetEnableNightImg = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative));

        private readonly BitmapImage _jobSetLockDayImg = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_JobSet_Lock.png", UriKind.Relative));
        private readonly BitmapImage _jobSetLockNightImg = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_JobSet_Lock.png", UriKind.Relative));

        private BtnJobSetType _BtnJobSetType = BtnJobSetType.BtnJobSetType_Unknown;
        public BtnJobSetType SetBtnJobSetType(BtnJobSetType bType)
        {
            _BtnJobSetType = bType;

            Image imgCheck = this.Btn_JobSet.Template.FindName("Image_Check", this.Btn_JobSet) as Image;
            Image imgUncheck = this.Btn_JobSet.Template.FindName("Image_Uncheck", this.Btn_JobSet) as Image;
            Image imgDisable = this.Btn_JobSet.Template.FindName("Image_Disable", this.Btn_JobSet) as Image;

            if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Normal)
            {
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    imgCheck.Source = this._jobSetDefaultDayImg;
                    imgUncheck.Source = this._jobSetDefaultDayImg;
                    imgDisable.Source = this._jobSetDefaultDayImg;
                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    imgCheck.Source = this._jobSetDefaultNightImg;
                    imgUncheck.Source = this._jobSetDefaultNightImg;
                    imgDisable.Source = this._jobSetDefaultNightImg;
                }
            }
            else if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Processing)
            {
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    imgCheck.Source = this._jobSetEnableDayImg;
                    imgUncheck.Source = this._jobSetEnableDayImg;
                    imgDisable.Source = this._jobSetEnableDayImg;
                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    imgCheck.Source = this._jobSetEnableNightImg;
                    imgUncheck.Source = this._jobSetEnableNightImg;
                    imgDisable.Source = this._jobSetEnableNightImg;
                }
            }
            else if (_BtnJobSetType == BtnJobSetType.BtnJobSetType_Lock)
            {
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    imgCheck.Source = this._jobSetLockDayImg;
                    imgUncheck.Source = this._jobSetLockDayImg;
                    imgDisable.Source = this._jobSetLockDayImg;
                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    imgCheck.Source = this._jobSetLockNightImg;
                    imgUncheck.Source = this._jobSetLockNightImg;
                    imgDisable.Source = this._jobSetLockNightImg;
                }
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
            this.CheckBox_Block_All.Click += new RoutedEventHandler(CheckBox_Block_All_Click);
            this.Btn_JobSet.Click += new RoutedEventHandler(Btn_JobSet_Click);

            this.Btn_Navigator.Click += new RoutedEventHandler(Btn_Navigator_Click);
            this.Btn_Chg_Loc.Click += new RoutedEventHandler(Btn_Chg_Loc_Click);
            this.Btn_JobDone.Click += new RoutedEventHandler(Btn_JobDone_Click);

            //if (App.TEST_MODE == true)
            //    this.Btn_Chg_Loc.IsEnabled = true;
            this.Btn_Chg_Loc.IsEnabled = false;
        }

        public void RMG_Member_PropertyChanged_TargetJobKey(object sender, PropertyChangedEventArgs e)
        {
            this.CheckupButtonStatus();
        }

        public void CheckupButtonStatus()
        {
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (!String.IsNullOrEmpty(jobKey))
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                if (jobOrder != null)
                {
                    var isSiemensConnected = PresentationMgr.AppWin.UC_IndicatorView.CheckBox_Siemens.IsChecked;

                    Btn_JobDone.IsEnabled = bBtnDoneEnableManual &&
                        (jobOrder.type.jobStatus == "P") &&
                        ((isSiemensConnected != true) || (PresentationMgr.Singleton.IsTwistLock != true))
                         ? true : false;

                    // 2016-01-27 Inactive Jobset 할 수 있도록
                    Btn_JobSet.IsEnabled = //true;//(jobOrder.type.jobStatus != "Q") &&
                        jobOrder.type.jobStatus == "P" || String.IsNullOrEmpty(PresentationMgr.Singleton.ProcessingJobKey);
                    //((isSiemensConnected != true) || (PresentationMgr.Singleton.IsTwistLock != true))
                    //? true : false;

                    Btn_JobSet.IsChecked = jobOrder.type.jobStatus == "P" ? true : false;

                    if (jobOrder.type.jobStatus == "P" && isSiemensConnected == true &&
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
                            Btn_JobDone.Content = "Set Down";
                            break;
                        case "GO":
                        case "MO":
                        case "LD":
                            Btn_JobDone.Content = "On Chassis";
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

        private void CheckBox_Block_All_Click(object sender, RoutedEventArgs e)
        {
            //if (String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            //{
            //    this.SelectFirstJob();
            //}

            this.UC_JobList.CurrentFilter.FilterJobBlock = true == this.CheckBox_Block_All.IsChecked ? true : false;

            PresentationMgr.Singleton.JL_Refresh(this.UC_JobList);
        }

        private void Btn_JobSet_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (String.IsNullOrEmpty(jobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null)
                return;

            this.UC_JobList.CheckBox_Refresh.IsChecked = false;

            if (jobOrder.type.jobStatus == "P")
            {
                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "SetJobStatus_Ask"), jobOrder.jobKey + ", false");

                //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.jobKey, false);
                //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                //    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.type.ycTwinKey, false);

                var jobKeyList = new List<String>();
                jobKeyList.Add(jobOrder.jobKey);
                if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                    jobKeyList.Add(jobOrder.type.ycTwinKey);
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, false);

                PresentationMgr.AppWin.ShowProgressBar(0);

                PresentationMgr.Singleton.RefreshJobInventory(jobOrder, false);
            }
            else if (jobOrder.type.jobStatus != "C")//if (jobOrder.type.jobStatus == "Q" || jobOrder.type.jobStatus == "A")
            {
                if ((jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "MI"       // 하차 작업시
                    || jobOrder.type.jobTp == "LC" || jobOrder.type.jobTp == "GC") &&
                    (jobOrder.type.jobFlagInfo == "F" || jobOrder.type.jobFlagInfo == "A"))
                {
                    // show vehicle view                    
                    //PresentationMgr.MainView.UC_VehiclePositionView.CurrentJobOrder = jobOrder;
                    PresentationMgr.MainView.UC_VehiclePositionView.CurrentJobKey = jobOrder.jobKey;
                    PresentationMgr.MainView.UC_VehiclePositionView.Visibility = Visibility.Visible;
                }
                else
                {
                    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "SetJobStatus_Ask"), jobOrder.jobKey + ", true");

                    //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.jobKey, true);
                    //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.type.ycTwinKey, true);

                    var jobKeyList = new List<String>();
                    jobKeyList.Add(jobOrder.jobKey);
                    if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                        jobKeyList.Add(jobOrder.type.ycTwinKey);
                    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, true);

                    PresentationMgr.AppWin.ShowProgressBar(0);

                    PresentationMgr.Singleton.RefreshJobInventory(jobOrder, true);
                }
            }
        }

        //public void Callback

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

        void Btn_Chg_Loc_Click(object sender, RoutedEventArgs e)
        {

            //PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.CorrectionView);
            var block = Convert.ToString(this.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Content);
            var bay = Convert.ToString(this.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Content);
            if (!String.IsNullOrEmpty(block) && !String.IsNullOrEmpty(bay))
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetChangePosition_ask(block, bay);
        }

        private void Btn_JobDone_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            {
                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                if (target != null)
                {
                    this.UC_JobList.CheckBox_Refresh.IsChecked = false;

                    this.bBtnDoneEnableManual = false;
                    this.Btn_JobDone.IsEnabled = false;
                    new PresentationMgr.SingleShot(1000, BtnDoneEnableManual);

                    PresentationMgr.Singleton.SendJobDoneAsk(target);
                    if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                    {
                        var twin = PresentationMgr.Singleton.JOB_Get(target.type.ycTwinKey);
                        if (null != twin)
                            PresentationMgr.Singleton.SendJobDoneAsk(twin);
                    }
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
                                    var isSiemensConnected = PresentationMgr.AppWin.UC_IndicatorView.CheckBox_Siemens.IsChecked;

                                    this.Btn_JobDone.IsEnabled = (jobOrder.type.jobStatus == "P") &&
                                        ((isSiemensConnected != true) || (PresentationMgr.Singleton.IsTwistLock != true))
                                        ? true : false;
                                }
                            }
                        }));
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinCheckBox(this.CheckBox_Block_All,
                    UIThemeMgr.Day.ButtonBlockOnImage, UIThemeMgr.Day.ButtonBlockOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Block_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Block_Off.png", UriKind.Relative))
                //);

                this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Normal);

                //PresentationMgr.SetSkinCheckBox(this.Btn_JobSet,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative))
                //    );

                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator,
                    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Chg_Loc,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_JobDone,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinCheckBox(this.CheckBox_Block_All,
                    UIThemeMgr.Night.ButtonBlockOnImage, UIThemeMgr.Night.ButtonBlockOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Block_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Block_Off.png", UriKind.Relative))
                //);

                this.SetBtnJobSetType(BtnJobSetType.BtnJobSetType_Normal);

                //PresentationMgr.SetSkinCheckBox(this.Btn_JobSet,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative))
                //    );

                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator,
                    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Chg_Loc,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_JobDone,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}