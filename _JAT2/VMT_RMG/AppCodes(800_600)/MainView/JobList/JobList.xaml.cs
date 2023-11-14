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

namespace VMT_RMG_800by600
{
    public class JobFilter
    {
        public const Int32 TYPE_ALL = 0x01;
        public const Int32 TYPE_GI = 0x02;
        public const Int32 TYPE_GO = 0x04;
        public const Int32 TYPE_MI = 0x08;
        public const Int32 TYPE_MO = 0x10;
        public const Int32 TYPE_DS = 0x20;
        public const Int32 TYPE_LD = 0x40;
        public const Int32 TYPE_RH = 0x80;

        public Int32 FilterJobType = TYPE_ALL;
        public Boolean FilterJobActive = false;
        public Boolean FilterJobBlock = false;
    }

    /// <summary>
    /// Interaction logic for MovingViewJobList.xaml
    /// </summary>
    public partial class JobList : UserControl
    {
        public List<JobListItem> ListItems = null;
        public JobFilter CurrentFilter = new JobFilter();
        //public Int32 JobFilter = JobFilter_Type.TYPE_ALL;
        public Int32 pageItemCount = 8;
        private Int32 _currentPageIndex = 0;
        private Int32 _totalPageCount = 0;
        private String _preSelectedTargetJobKey = String.Empty;

        private Boolean _isAvailableBreaking = false;
        public Boolean IsAvailableBreaking
        {
            get
            {
                return _isAvailableBreaking;
            }
            set
            {
                _isAvailableBreaking = value;
                if (_isAvailableBreaking)
                {
                    if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                    {
                        PresentationMgr.SetSkinButton(this.Btn_Available,
                            UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    }
                    else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                    {
                        PresentationMgr.SetSkinButton(this.Btn_Available,
                            UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    }
                }
                else
                {
                    if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                    {
                        PresentationMgr.SetSkinButton(this.Btn_Available,
                            UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    }
                    else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                    {
                        PresentationMgr.SetSkinButton(this.Btn_Available,
                            UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    }
                }
            }
        }

        public Int32 CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set { _currentPageIndex = value; }
        }

        public Int32 TotalPageCount
        {
            get { return _totalPageCount; }
            set { _totalPageCount = value; }
        }

        public Boolean IsRefreshReserved { get; set; }

        public JobList()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this.ListBox_Job.SelectionChanged += new SelectionChangedEventHandler(RMG_JobListSelectionChanged);

            this.ListItems = new List<JobListItem>(this.pageItemCount);
            for (int i = 0; i < pageItemCount; i++)
                this.ListItems.Add(new JobListItem());
        }

        ~JobList()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        public void RMG_JobListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (JobListItem jobListItem in this.ListBox_Job.Items)
            //{
            //    jobListItem.Selected = false;
            //}

            //if (this.ListBox_Job.SelectedItem is JobListItem)
            //{                
            //    JobListItem item = this.ListBox_Job.SelectedItem as JobListItem;
            //    item.Selected = true;

            //    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);

            //    if (_preSelectedTargetJobKey != jobOrder.jobKey)
            //    {
            //        _preSelectedTargetJobKey = jobOrder.jobKey;
            //        VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobOrder.jobKey;
            //    }
            //}            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.CheckBox_All.Click += new RoutedEventHandler(CheckBox_All_Click);
            this.Btn_Available.Click += new RoutedEventHandler(Btn_Available_Click);
            this.CheckBox_Status_Active.Click += new RoutedEventHandler(CheckBox_Status_Active_Click);
            this.Btn_Search.Click += new RoutedEventHandler(Btn_Search_Click);
            this.CheckBox_Refresh.Click += new RoutedEventHandler(CheckBox_Refresh_Click);

            this.CheckBox_Type_GI.Click += new RoutedEventHandler(CheckBox_Type_GI_Click);
            this.CheckBox_Type_GO.Click += new RoutedEventHandler(CheckBox_Type_GO_Click);
            this.CheckBox_Type_MI.Click += new RoutedEventHandler(CheckBox_Type_MI_Click);
            this.CheckBox_Type_MO.Click += new RoutedEventHandler(CheckBox_Type_MO_Click);
            this.CheckBox_Type_DS.Click += new RoutedEventHandler(CheckBox_Type_DS_Click);
            this.CheckBox_Type_LD.Click += new RoutedEventHandler(CheckBox_Type_LD_Click);
            this.CheckBox_Type_RH.Click += new RoutedEventHandler(CheckBox_Type_RH_Click);

            this.Btn_PageDown.Click += new RoutedEventHandler(Btn_PageDown_Click);
            this.Btn_PageUp.Click += new RoutedEventHandler(Btn_PageUp_Click);

            this.ListBox_Job.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListBox_Job_PreviewMouseLeftButtonUp);

            if (!VMT_RMG.App.STANDALONE_MODE)
                this.ListBox_Job.Items.Clear();

            this.IsRefreshReserved = Convert.ToBoolean(this.CheckBox_Refresh.IsChecked);
        }

        private void CheckBox_All_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_Type_GI.IsChecked = false;
            this.CheckBox_Type_GO.IsChecked = false;
            this.CheckBox_Type_MI.IsChecked = false;
            this.CheckBox_Type_MO.IsChecked = false;
            this.CheckBox_Type_DS.IsChecked = false;
            this.CheckBox_Type_LD.IsChecked = false;
            this.CheckBox_Type_RH.IsChecked = false;
            //this.CheckBox_Status_Active.IsChecked = false;

            this.CurrentFilter.FilterJobType = JobFilter.TYPE_ALL;

            if (this.CheckBox_All.IsChecked == false)
                this.CheckBox_All.IsChecked = true;
            else
                PresentationMgr.Singleton.JL_Refresh(this);
        }

        // Button Event
        private void Btn_Available_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Btn_Available.Content.Equals("Available"))
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.AvailableView);
            else
            {
                PresentationMgr.MainView.UC_BreakTimeView.SetEndDateTime();
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.BreakTimeView);
            }
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.ContainerSearch);
        }

        private void CheckBox_Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.IsRefreshReserved = Convert.ToBoolean(this.CheckBox_Refresh.IsChecked);
            if (this.CheckBox_Refresh.IsChecked == true)
            {
                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                {
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                    if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);
                }

                //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                    PresentationMgr.Singleton.ThreadTimerStart(true);
            }
            
            //PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
        }

        private void CheckBox_Status_Active_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();   
            //this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobActive = true == this.CheckBox_Status_Active.IsChecked ? true : false;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void CheckBox_Status_All_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_RH_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = (true == this.CheckBox_Type_RH.IsChecked) ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_RH) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_RH;

            PresentationMgr.Singleton.JL_Refresh(this);

        }

        private void CheckBox_Type_LD_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();            
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = true == this.CheckBox_Type_LD.IsChecked ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_LD) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_LD;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void CheckBox_Type_DS_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = true == this.CheckBox_Type_DS.IsChecked ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_DS) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_DS;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void CheckBox_Type_MO_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = true == this.CheckBox_Type_MO.IsChecked ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_MO) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_MO;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void CheckBox_Type_MI_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = true == this.CheckBox_Type_MI.IsChecked ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_MI) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_MI;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void CheckBox_Type_GO_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = true == this.CheckBox_Type_GO.IsChecked ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_GO) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_GO;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void CheckBox_Type_GI_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            this.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            this.CurrentFilter.FilterJobType = true == this.CheckBox_Type_GI.IsChecked ?
                (this.CurrentFilter.FilterJobType | JobFilter.TYPE_GI) : this.CurrentFilter.FilterJobType & ~JobFilter.TYPE_GI;

            PresentationMgr.Singleton.JL_Refresh(this);
        }

        private void Grid_SeparateType_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (PresentationMgr.MainView.UC_JobTypeFilterView.Visibility != System.Windows.Visibility.Visible)
                PresentationMgr.MainView.UC_JobTypeFilterView.Visibility = System.Windows.Visibility.Visible;
        }

        public void ListBox_setJobSelection(String jobKey)
        {
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobKey;
            this._preSelectedTargetJobKey = jobKey;
            if (!String.IsNullOrEmpty(jobKey))
            {
                PresentationMgr.Singleton.NeedMoveToTargetJobPage = true;
                this.CheckBox_All.IsChecked = true;
                this.CheckBox_All_Click(null, null);
            }
            else if (this.ListBox_Job.SelectedItem != null)
            {
                JobListItem item = this.ListBox_Job.SelectedItem as JobListItem;
                item.Selected = false;
                this.ListBox_Job.SelectedItem = null;
                this.CurrentPageIndex = 0;
            }
            else
                this.CurrentPageIndex = 0;
        }

        void ListBox_Job_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MainView &&
                PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerDetailView)
                return;

            if (this.ListBox_Job.SelectedIndex < 0)
                return;

            //PresentationMgr.Singleton.NeedJobAutoSelection = true;

            foreach (JobListItem jobListItem in this.ListBox_Job.Items)
            {
                jobListItem.Selected = false;
            }

            if (this.ListBox_Job.SelectedItem is JobListItem)
            {
                JobListItem item = this.ListBox_Job.SelectedItem as JobListItem;
                item.Selected = true;

                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);

                if (_preSelectedTargetJobKey != jobOrder.jobKey)
                {
                    //this.CheckBox_Refresh.IsChecked = this.IsRefreshReserved = false;

                    _preSelectedTargetJobKey = jobOrder.jobKey;
                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobOrder.jobKey;                    
                }
                else
                    PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
            }

            //if (this.ListBox_Job.SelectedItem is JobListItem)
            //{
            //    JobListItem jobListItem = this.ListBox_Job.SelectedItem as JobListItem;                

            //    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobListItem.JobKey);

            //    if (_preSelectedTargetJobKey == jobOrder.jobKey)
            //        PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
            //}
        }

        void Btn_PageDown_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex < TotalPageCount - 1)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex + 1);
                PresentationMgr.Singleton.JL_Refresh(this, CurrentPageIndex + 1, true);
            }
        }

        void Btn_PageUp_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex > 0)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex - 1);
                PresentationMgr.Singleton.JL_Refresh(this, CurrentPageIndex - 1, true);
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                if (this._isAvailableBreaking)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
                }
                else
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                }

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Refresh,
                //    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.SelectionView_ButtonUpDefaultImage, UIThemeMgr.Day.SelectionView_ButtonUpPressImage, UIThemeMgr.Day.SelectionView_ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.SelectionView_ButtonDownDefaultImage, UIThemeMgr.Day.SelectionView_ButtonDownPressImage, UIThemeMgr.Day.SelectionView_ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Down_Disable.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                if (this._isAvailableBreaking)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
                }
                else
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                }

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Refresh,
                //    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);               

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Night.SelectionView_ButtonUpDefaultImage, UIThemeMgr.Night.SelectionView_ButtonUpPressImage, UIThemeMgr.Night.SelectionView_ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Night.SelectionView_ButtonDownDefaultImage, UIThemeMgr.Night.SelectionView_ButtonDownPressImage, UIThemeMgr.Night.SelectionView_ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Down_Disable.png", UriKind.Relative))
                //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public void JobList_Add(String JobKey)
        {
            JobList_Del(JobKey);

            JobListItem jItem = new JobListItem();
            jItem.SetJobInfo(JobKey, ListBox_Job.Items.Count);
            this.ListBox_Job.Items.Add(jItem);
        }

        public void JobList_Del(String JobKey)
        {
            foreach (JobListItem jItem in this.ListBox_Job.Items)
            {
                if (jItem.JobKey.Equals(JobKey))
                {
                    this.ListBox_Job.Items.Remove(jItem);
                    break;
                }
            }
        }

        public void JobList_Clear()
        {
            this._preSelectedTargetJobKey = String.Empty;
            this.CheckBox_Refresh.IsChecked = this.IsRefreshReserved = true;
            this.ListBox_Job.Items.Clear();
        }
    }
}