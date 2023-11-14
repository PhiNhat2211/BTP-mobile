using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

namespace VMT_RMG
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
        public Int32 pageItemCount = 7;
        private Int32 _currentPageIndex = 0;
        private Int32 _totalPageCount = 0;
        private String _preSelectedTargetJobKey = String.Empty;

        private Boolean _isAvailableBreaking = false;
        public int isSelected = 0; //0 - manual click / 1 - refresh
        public String selectedJobKeyPriority = String.Empty; //jobKey selectedItem envent SelectionChanged

        string jobKey = "";
        public Boolean needToGetEmptySwapWithSelectedLocation = false;

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
        public string PreSelectedTargetJobKey
        {
            get { return _preSelectedTargetJobKey; }
            set { _preSelectedTargetJobKey = value; }
        }

        public JobList()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this.ListItems = new List<JobListItem>(this.pageItemCount);
            for (int i = 0; i < pageItemCount; i++)
                this.ListItems.Add(new JobListItem());
        }

        ~JobList()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
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
            this.Btn_GC.Click += new RoutedEventHandler(Btn_GC_Click);
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

            //this.ListBox_Job.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ListBox_Job_PreviewMouseLeftButtonUp);
            this.ListBox_Job.PreviewMouseDown += ListBox_Job_PreviewMouseDown;
            this.ListBox_Job.SelectionChanged += ListBox_Job_SelectionChanged;
            this.ListBox_Job.MouseUp += ListBox_Job_MouseUp;
            //this.ListBox_Job.PreviewMouseUp += ListBox_Job_PreviewMouseUp;

            if (!App.STANDALONE_MODE)
                this.ListBox_Job.Items.Clear();

            this.IsRefreshReserved = Convert.ToBoolean(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked);

            CheckBox_All.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0118", LanguageService.LABEL_CUSTOMIZE);
            CheckBox_Status_Active.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0119", LanguageService.LABEL_CUSTOMIZE);
            Btn_Available.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0120", LanguageService.LABEL_CUSTOMIZE);
            Btn_Search.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0121", LanguageService.LABEL_CUSTOMIZE);
            Btn_GC.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0133", LanguageService.LABEL_CUSTOMIZE);
            CheckBox_Refresh.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0122", LanguageService.LABEL_CUSTOMIZE);
        }

        void Btn_PageDown_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex < TotalPageCount - 1)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex + 1);
                PresentationMgr.Singleton.NeedMoveToTargetJobPage = false;
                PresentationMgr.Singleton.JL_RefreshChangePageClick(this, CurrentPageIndex + 1, true);
            }
        }

        void Btn_PageUp_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex > 0)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex - 1);
                PresentationMgr.Singleton.NeedMoveToTargetJobPage = false;
                PresentationMgr.Singleton.JL_RefreshChangePageClick(this, CurrentPageIndex - 1, true);
            }
        }

        private void ListBox_Job_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isSelected = 0;
        }
        public void ListBox_Job_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (isSelected == 1)
                {
                    isSelected = 0;
                    return;
                }

                foreach (JobListItem jobListItem in this.ListBox_Job.Items)
                {
                    if (jobListItem != null && jobListItem.LayoutRoot != null)
                    {
                        if (((SolidColorBrush)jobListItem.LayoutRoot.Background).Color != ((SolidColorBrush)new BrushConverter().ConvertFrom("#FFF75348")).Color //PROCESSING JOB DON'T CLEAR COLOR
                            && ((SolidColorBrush)jobListItem.LayoutRoot.Background).Color != ((SolidColorBrush)new BrushConverter().ConvertFrom("#47C83E")).Color) //GreenJob with vbsDate too
                        {
                            ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                            var strRec = rec["Gird_Background_8"];
                            if (strRec is SolidColorBrush)
                                jobListItem.LayoutRoot.Background = strRec as SolidColorBrush;
                        }
                    }
                }

                if (this.ListBox_Job.SelectedItem is JobListItem)
                {
                    JobListItem item = this.ListBox_Job.SelectedItem as JobListItem;

                    if (item == null) return;

                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                    if (jobOrder == null || jobOrder.type == null)
                        return;
                    if (jobOrder.type.jobStatus != "P")
                    {
                        ChangeBackGroundColor(item);
                        selectedJobKeyPriority = jobOrder.jobKey;

                        //MAKE CORRECTION AND BLINK FOR BAYVIEW
                        string bay = "";

                        var loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location();
                        if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "GO")
                            loc = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                        else if (jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "MI" || jobOrder.type.jobTp == "GI"
                            || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH" || jobOrder.type.jobTp == "LC" || jobOrder.type.jobTp == "GC")
                            loc = jobOrder.locWorking;

                        if (loc != null)
                        {
                            if (!String.IsNullOrEmpty(loc.bay))
                            {
                                if (Convert.ToInt32(PresentationMgr.BayRemoveChars(loc.bay)) % 2 == 0)
                                {
                                    bay = (Convert.ToInt32(PresentationMgr.BayRemoveChars(loc.bay)) - 1) < 10 ? "0" + (Convert.ToInt32(PresentationMgr.BayRemoveChars(loc.bay)) - 1).ToString() : (Convert.ToInt32(PresentationMgr.BayRemoveChars(loc.bay)) - 1).ToString();
                                }
                                else
                                {
                                    bay = loc.bay;
                                }
                            }
                            if (bay == PresentationMgr.Singleton.CurrentBay && loc.blck == PresentationMgr.Singleton.CurrentBlock)
                            {
                                if (!String.IsNullOrEmpty(loc.row) && !String.IsNullOrEmpty(loc.tier))
                                {
                                    //Thread thread = new Thread(() => {
                                    if (PresentationMgr.MainView.UC_NavigatorView.ChangePageBayView(loc.row))
                                    {
                                        Thread.Sleep(50);
                                        PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                                        PresentationMgr.Singleton.SetInventoryData(null);
                                    }

                                    //Thread.Sleep(50);
                                    PresentationMgr.MainView.UC_BayView.SetBlinkSelect_Target(loc.row, Convert.ToInt32(loc.tier), item.TextBlock_JobType.Text);
                                    
                                    //});
                                    //thread.Start();
                                }
                            }
                        }
                        if (String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo))
                        {
                            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = false;
                            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 0.5;
                        }                      
                    }
                    else
                    {
                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = true;
                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 1;
                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.jobKeyProcessing = jobOrder.jobKey;
                    }

                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.RefreshTargetJobInfo(item.JobKey);
                }
                else
                {
                    if (String.IsNullOrEmpty(PresentationMgr.MainView.plcDomainTwistLock.cntrNo))
                    {
                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.IsEnabled = false;
                        PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Unlock.Opacity = 0.5;
                    }
                }
            }
            catch
            {

            } 
        }

        public void ListBox_Job_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (isChange)
            //{
            //    isChange = false;
            //    return;
            //}

            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MainView &&
                PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerDetailView)
                return;

            if (this.ListBox_Job.SelectedIndex < 0)
                return;
            //PresentationMgr.Singleton.NeedJobAutoSelection = true;

            foreach (JobListItem jobListItem in this.ListBox_Job.Items)
            {
                if (jobListItem != null && jobListItem.Selected)
                {
                    jobListItem.Selected = false;
                }
            }

            if (this.ListBox_Job.SelectedItem is JobListItem)
            {
                JobListItem item = this.ListBox_Job.SelectedItem as JobListItem;
                if (item != null)
                {
                    //VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                    //if (jobOrder.type.jobStatus != "P")
                    //{
                    //    //this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 240, 165, 15));
                    //    this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;
                    //}

                    //20210406 getSwapList blockName-bayName   
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                    if (jobOrder != null && jobOrder.type != null && "GO".Equals(jobOrder.type.jobTp) && (PreSelectedTargetJobKey != jobOrder.jobKey))
                    {
                        var location = String.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                        if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(location.blck))
                        {
                            needToGetEmptySwapWithSelectedLocation = true;
                        }
                    }
                    
                    item.Selected = true;
                    isSelected = 0;
                    jobKey = item.JobKey;

                    if (!string.IsNullOrEmpty(jobKey))
                    {
                        Thread thread = new Thread(selectedJobProcess);
                        thread.Start();
                    }
                }
            }
        }

        private void selectedJobProcess()
        {
            Thread.Sleep(100);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(this.jobKey);

                if (jobOrder == null) return;
                MainView.contItmSelected = false;
                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetVirtualContNo = String.Empty;
                PresentationMgr.MainView.selectedJobList = true;
                if (jobOrder.isCmtJob)
                {
                    Btn_GC.IsEnabled = true;
                    Btn_GC_dis.Visibility = Visibility.Hidden;
                }
                else
                {
                    Btn_GC.IsEnabled = false;
                    Btn_GC_dis.Visibility = Visibility.Visible;
                }

                PresentationMgr.Singleton.CorrectionSource.Clear();

                if (PreSelectedTargetJobKey != jobOrder.jobKey) //SELECT CHANGED
                {
                    var oldJob = PresentationMgr.Singleton.JOB_Get(this.PreSelectedTargetJobKey);
                    if(oldJob != null && (oldJob.type.jobTp == "RH" || oldJob.type.jobTp == "AH")
                    && oldJob.locFrom.blck == jobOrder.locFrom.blck && PresentationMgr.GetFrontOddBay(oldJob.locFrom.bay) == PresentationMgr.GetFrontOddBay(jobOrder.locFrom.bay))
                    {
                        PresentationMgr.Singleton.PreSelectedAHRHJobKey = PreSelectedTargetJobKey;
                    }
                    else
                    {
                        PresentationMgr.Singleton.PreSelectedAHRHJobKey = String.Empty;
                    }

                    //PresentationMgr.MainView.selectedJobList1Time = true;
                    PreSelectedTargetJobKey = jobOrder.jobKey;
                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobOrder.jobKey;

                    /* 20210406 -> call getswapslit in joblistItem(auto refresh)
                    // Call getSwapList
                    if (jobOrder.type.jobTp.Equals("GO"))
                    {
                        //VMT_DataMgr_RMG.GetEmptySwappingTargetList_Ask(job.cntr.cntrNo, job.partnerMchn.mchnId);
                        var location = String.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                        if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(location.blck))
                        {
                            if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[location.blck].isBolBlck && !String.IsNullOrEmpty(location.bay))
                            {
                                VMT_DataMgr_RMG.getSwapListRTG_Ask(jobOrder.jobKey, location.blck + "-" + location.bay, true);
                            }
                            else
                            {
                                VMT_DataMgr_RMG.getSwapListRTG_Ask(jobOrder.jobKey, location.blck, true);
                            }
                        }
                    }
                    else
                    {
                        PresentationMgr.Singleton.swapList.Clear();
                        PresentationMgr.Singleton.reservedList.Clear();
                        PresentationMgr.Singleton.swapListRTG.Clear();
                    }*/
                }
                else //DE-SELECT
                {
                    PresentationMgr.Singleton.PreSelectedAHRHJobKey = String.Empty;
                    
                    PresentationMgr.MainView.deselectJobList = true;
                    if (PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem is JobListItem)
                    {
                        PreSelectedTargetJobKey = String.Empty;
                        ListBox_setJobSelection(String.Empty);
                        PresentationMgr.Singleton.NeedJobAutoSelection = false;
                    }
                    for (int i = 0; i < PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children.Count; i++)
                    {
                        if (PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children[i] is ContainerItem)
                        {
                            var itm = PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children[i] as ContainerItem;
                            if (itm.Selected)
                            {
                                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = "";
                                itm.Image_ContainerType.Source = itm.GetBGImageByContainerType("Exist");
                                itm.Image_ContainerType1.Source = itm.GetBGImageByContainerType("Exist");
                            }
                        }
                    }
                    //VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                    PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
                }
                PresentationMgr.Singleton.NeedJobAutoSelection = false;

            }));
        }

        public void ChangeBackGroundColor(JobListItem item)
        {
            Thread thread = new Thread(() =>
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(() => item.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground));
            });
            thread.Start();
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
            {
                if (PresentationMgr.MainView.UC_AvailableView.Wrap_AvailableView.Children.Count <= 0)
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStopCodeList_Ask();

                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.AvailableView);
            }
            else
            {
                PresentationMgr.MainView.UC_BreakTimeView.SetEndDateTime();
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BreakTimeView);
            }
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerSearch);
        }

        private void Btn_GC_Click(object sender, RoutedEventArgs e)
        {
            var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
            if (target == null) return;
            target.isGcBtn = true;
            PresentationMgr.Singleton.SendJobDoneAsk(target);
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
            this.PreSelectedTargetJobKey = jobKey;

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
                if (!PresentationMgr.MainView.deselectJobList)
                    this.CurrentPageIndex = 0;
            }
            else if (!PresentationMgr.MainView.deselectJobList)
                this.CurrentPageIndex = 0;
        }

        public void ListBox_Job_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.MainView &&
                PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerDetailView)
                return;

            if (this.ListBox_Job.SelectedIndex < 0)
                return;

            //PresentationMgr.Singleton.NeedJobAutoSelection = true;

            foreach (JobListItem jobListItem in this.ListBox_Job.Items)
            {
                if (jobListItem.Selected)
                    jobListItem.Selected = false;
            }

            if (this.ListBox_Job.SelectedItem is JobListItem)
            {
                JobListItem item = this.ListBox_Job.SelectedItem as JobListItem;
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);

                if (jobOrder != null && jobOrder.type != null)
                {
                    if (jobOrder.type.jobStatus != "P")
                    {
                        item.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;
                    }
                    item.Selected = true;
                    PresentationMgr.MainView.selectedJobList = true;
                    if (jobOrder.isCmtJob)
                    {
                        Btn_GC.IsEnabled = true;
                        Btn_GC_dis.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Btn_GC.IsEnabled = false;
                        Btn_GC_dis.Visibility = Visibility.Visible;
                    }


                    if (PreSelectedTargetJobKey != jobOrder.jobKey)
                    {
                        //this.CheckBox_Refresh.IsChecked = this.IsRefreshReserved = false;
                        PreSelectedTargetJobKey = jobOrder.jobKey;
                        VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobOrder.jobKey;
                    }
                    else
                    {
                        for (int i = 0; i < PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children.Count; i++)
                        {
                            if (PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children[i] is ContainerItem)
                            {
                                var itm = PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children[i] as ContainerItem;
                                if (itm.Selected)
                                {
                                    PresentationMgr.MainView.deselectJobList = true;
                                    itm.Image_ContainerType.Source = itm.GetBGImageByContainerType(String.Empty);
                                    itm.Image_ContainerType1.Source = itm.GetBGImageByContainerType(String.Empty);
                                }
                            }
                        }

                        //VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                        PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
                    }
                }
            }

            //if (this.ListBox_Job.SelectedItem is JobListItem)
            //{
            //    JobListItem jobListItem = this.ListBox_Job.SelectedItem as JobListItem;                

            //    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobListItem.JobKey);

            //    if (_preSelectedTargetJobKey == jobOrder.jobKey)
            //        PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
            //}
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                if (this._isAvailableBreaking)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
                }
                else
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                }

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_GC,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

                //PresentationMgr.SetSkinButton(this.Btn_Refresh,
                //    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
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
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
                }
                else
                {
                    PresentationMgr.SetSkinButton(this.Btn_Available,
                        UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                }

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_GC,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);

                //PresentationMgr.SetSkinButton(this.Btn_Refresh,
                //    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
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
            this.PreSelectedTargetJobKey = String.Empty;
            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = this.IsRefreshReserved = true;
            this.IsRefreshReserved = true;
            this.ListBox_Job.Items.Clear();
            Btn_GC_dis.Visibility = Visibility.Hidden;
        }
    }
}