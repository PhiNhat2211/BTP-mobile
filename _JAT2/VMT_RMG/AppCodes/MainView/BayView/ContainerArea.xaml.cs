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

//20190108
using Common.Interface;

namespace VMT_RMG
{

    /// <summary>
    /// Interaction logic for MovingViewJobList.xaml
    /// </summary>
    public partial class ContainerArea : UserControl
    {
        public List<JobListItem> ListItems = null;
        public JobFilter CurrentFilter = new JobFilter();
        //public Int32 JobFilter = JobFilter_Type.TYPE_ALL;
        public Int32 pageItemCount = 7;
        private Int32 _currentPageIndex = 0;
        private Int32 _totalPageCount = 0;
        private String _preSelectedTargetContNo = String.Empty;


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

        public String cntrNoCorrectSelection = string.Empty, blockCorrectSelection = string.Empty, bayCorrectSelection = string.Empty;

        public ContainerArea()
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

        ~ContainerArea()
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
            this.CurrentFilter.FilterJobType = JobFilter.TYPE_ALL;
            // Init Event Handler
            this.Btn_PageDown.Click += new RoutedEventHandler(Btn_PageDown_Click);
            this.Btn_PageUp.Click += new RoutedEventHandler(Btn_PageUp_Click);

            this.ListBox_Job.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListBox_Job_PreviewMouseLeftButtonUp);

            if (!App.STANDALONE_MODE)
                this.ListBox_Job.Items.Clear();
        }

        // Button Event

        public void ListBox_Job_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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

                VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven = PresentationMgr.Singleton.CONTVirtual_Get(item.TextBlock_CntrNo.Text);
                VMT_Data_JAT2.Marshalling.Geometry.sPosition containerPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();

                containerPos.m_cBlock = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text;

                setTargetContainerNumber(containerPos, inven.cntr.cntrNo /*+ " " + inven.cntr.opr + " " + inven.cntr.cntrLen + " " + (inven.cntr.chkHold == true ? "H" : "S")*/ /*+ " " + this._inventory.cntr.imdgCd*/);

                if (!PresentationMgr.Singleton.CorrectionSource.CntrNo.Equals(inven.cntr.cntrNo))
                    PresentationMgr.Singleton.SetCorrectionSelectArea(containerPos, inven.cntr.cntrNo, inven.cntr.cntrIso);

                if (_preSelectedTargetContNo != inven.cntr.cntrNo)
                {
                    _preSelectedTargetContNo = inven.cntr.cntrNo;
                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetVirtualContNo = inven.cntr.cntrNo;
                }
                else
                {
                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetVirtualContNo = String.Empty;
                    _preSelectedTargetContNo = String.Empty;
                    item.Selected = false;
                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                    PresentationMgr.Singleton.CorrectionSource.Clear();
                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = "";
                }
            }
        }

        private void setTargetContainerNumber(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos, string contNumberText)
        {
            MainView.contItmSelected = true;
            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = true;
            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = contNumberText;

            JobListItem item = PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem as JobListItem;
            if (item != null)
                item.Selected = false;
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
        }

        void Btn_PageDown_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex < TotalPageCount - 1)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex + 1);
                PresentationMgr.Singleton.JLArea_Refresh(this, CurrentPageIndex + 1, true);
            }
        }

        void Btn_PageUp_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex > 0)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex - 1);
                PresentationMgr.Singleton.JLArea_Refresh(this, CurrentPageIndex - 1, true);
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
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
            this._preSelectedTargetContNo = String.Empty;
            this.ListBox_Job.Items.Clear();
        }        
    }
}