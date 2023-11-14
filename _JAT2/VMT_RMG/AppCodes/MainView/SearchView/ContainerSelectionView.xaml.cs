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

//20190108
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// ContainerSelectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContainerSelectionView : UserControl
    {
        //private Int32 pageItemCount = 7;
        private List<ContainerSearchControl> _containerControlItems = null;
        private String _SelectKey = String.Empty;
        private Boolean _isTwin = false;

        static public List<string> OldProcessingJobKeys = new List<string>();

        public ContainerSelectionView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();            

            // Init Event Handler
            this.Btn_PageUp.Click += new RoutedEventHandler(Btn_PageUp_Click);
            this.Btn_PageDown.Click += new RoutedEventHandler(Btn_PageDown_Click);
            this.Btn_Done.Click += new RoutedEventHandler(Btn_Done_Click);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(ContainerSearchView_IsVisibleChanged);
            this.TextBlock_ResultCount.Text = String.Empty;
            this.ListBox_SearchJobOrder.Children.Clear();

            this.ListBox_SearchJobOrder.MouseLeftButtonUp += new MouseButtonEventHandler(ListBox_SearchJobOrder_MouseLeftButtonUp);

            this._containerControlItems = new List<ContainerSearchControl>();

            /*tbContainer.Text = PresentationMgr.Singleton.LanguageSer.GetResourceECH("LA0065", LanguageService.LABEL_CONTAINERDETAIL);
            tbISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceECH("LA0041", LanguageService.LABEL_CONTAINERDETAIL);
            tbTruckNo.Text = PresentationMgr.Singleton.LanguageSer.GetResourceECH("LA0066", LanguageService.LABEL_CONTAINERDETAIL);
            tbWaitJob.Text = PresentationMgr.Singleton.LanguageSer.GetResourceECH("LA0067", LanguageService.LABEL_CONTAINERDETAIL);
            tbLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceECH("LA0049", LanguageService.LABEL_CONTAINERDETAIL);
             */
            TextBlock_Result.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0131", LanguageService.LABEL_CUSTOMIZE) + " ";
            Btn_Done.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0110", LanguageService.LABEL_CUSTOMIZE);
        }

        private void ContainerSearchView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {            
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;
            
            this.Btn_Done.IsEnabled = false;
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            { 
                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;    // new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpDisableImage);                    

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownDisableImage);                    

                PresentationMgr.SetSkinButton(this.Btn_Done,
                    UIThemeMgr.Day.SelectionView_ButtonDefaultImage, UIThemeMgr.Day.SelectionView_ButtonPressImage, UIThemeMgr.Day.SelectionView_ButtonDefaultImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {    
                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;  //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Night.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpDisableImage);                    

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Night.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownDisableImage);                    

                PresentationMgr.SetSkinButton(this.Btn_Done,
                    UIThemeMgr.Night.SelectionView_ButtonDefaultImage, UIThemeMgr.Night.SelectionView_ButtonPressImage, UIThemeMgr.Night.SelectionView_ButtonDefaultImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public static int ComparePowIn(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder x, VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder y)
        {
            try
            {
                if (x.type.isArrivedItv && y.type.isArrivedItv)
                {
                    var typeX = x.type.jobTp;
                    var typeY = y.type.jobTp;
                    
                    if (!(typeX == "GI" || typeX == "GO" || typeX == "GC") && (typeY == "GI" || typeY == "GO" || typeY == "GC"))
                        return -1;
                    else if ((typeX == "GI" || typeX == "GO" || typeX == "GC") && !(typeY == "GI" || typeY == "GO" || typeY == "GC"))
                        return 1;                    
                    else
                        return 0;
                }
                else if (x.type.isArrivedItv && !y.type.isArrivedItv)
                    return -1;
                else if (!x.type.isArrivedItv && y.type.isArrivedItv)
                    return 1;                                
            }
            catch
            {
            }
            return 0;
        }

        public void SetSearchedJobList(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList, Boolean isTwin = false)
        {
            this._isTwin = isTwin;
            this.Btn_Done.IsEnabled = false;
            
            this.ListBox_SearchJobOrder.Children.Clear();

            var processingJobKey = PresentationMgr.Singleton.ProcessingJobKey;
            //var processingJob = PresentationMgr.Singleton.JOB_Get(processingJobKey);

            jobOrderList.Sort(ContainerSelectionView.ComparePowIn);
            Int32 controlCount = 0;
            foreach (var jobOrder in jobOrderList)
            {
                 // 2017/02/17 : 이미 프로세싱된 잡은 제외하고 조회 (twin processing job은 제외하지 않음)
                if (jobOrder.jobKey.Equals(processingJobKey) && string.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                    continue;                

                ContainerSearchControl control = null;
                if (this._containerControlItems.Count <= controlCount)
                {
                    control = new ContainerSearchControl();
                    _containerControlItems.Add(control);
                }
                else                
                    control = _containerControlItems[controlCount];                    
                control.ClearJobInfo();

                control.SetJobInfo(jobOrder);
                control.SetPowInInfo(jobOrder);

                this.ListBox_SearchJobOrder.Children.Add(control);

                controlCount++;
            }

            this.TextBlock_ResultCount.Text = Convert.ToString(this.ListBox_SearchJobOrder.Children.Count);
            this.Scroll_SearchJobOrder.ScrollToVerticalOffset(0);
        }      

        private void Scroll_SearchJobOrder_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer))
                return;

            ScrollViewer scroller = sender as ScrollViewer;

            if (scroller.ContentVerticalOffset == 0)
            {
                this.Btn_PageUp.IsEnabled = false;
            }
            else
            {
                this.Btn_PageUp.IsEnabled = true;
            }

            if (scroller.ScrollableHeight == scroller.ContentVerticalOffset)
            {
                this.Btn_PageDown.IsEnabled = false;
            }
            else
            {
                this.Btn_PageDown.IsEnabled = true;
            }
        }

        private double scrollMoveOffSet = 69*5; // ContainerSearchControl DesignHeight
        private void Btn_PageUp_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_SearchJobOrder.ContentVerticalOffset;

            if (currentOffset - scrollMoveOffSet > 0)
                this.Scroll_SearchJobOrder.ScrollToVerticalOffset(currentOffset - scrollMoveOffSet);
            else
                this.Scroll_SearchJobOrder.ScrollToVerticalOffset(0);
        }

        private void Btn_PageDown_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_SearchJobOrder.ContentVerticalOffset;
            if (currentOffset + scrollMoveOffSet < this.Scroll_SearchJobOrder.ScrollableHeight)
                this.Scroll_SearchJobOrder.ScrollToVerticalOffset(currentOffset + scrollMoveOffSet);
            else
                this.Scroll_SearchJobOrder.ScrollToVerticalOffset(this.Scroll_SearchJobOrder.ScrollableHeight);
        }

        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            ContainerSelectionView.OldProcessingJobKeys.Clear();
            if (!String.IsNullOrEmpty(_SelectKey))
            {
                var job = PresentationMgr.Singleton.JOB_Get(_SelectKey);
                if (job != null && job.type != null)
                {
                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;
                    // job set

                    // 1. twin : 각각 jobSet
                    ////VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(job.jobKey, true);
                    ////if (!String.IsNullOrEmpty(job.type.ycTwinKey))
                    ////    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(job.type.ycTwinKey, true);

                    // 2. twin : list로 전달
                    //var jobKeyList = new List<String>();
                    //jobKeyList.Add(job.jobKey);
                    //if (!String.IsNullOrEmpty(job.type.ycTwinKey))
                    //    jobKeyList.Add(job.type.ycTwinKey);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, true);

                    var processingJobKey = PresentationMgr.Singleton.ProcessingJobKey;
                    var processingJob = PresentationMgr.Singleton.JOB_Get(processingJobKey);

                    // 3. PLC data를 통한 pick 시에는 setPickedContainer 로 전달   
                    if (String.IsNullOrEmpty(job.type.ycTwinKey))  // single job이면
                    {
                        // 2017/02/17 : 기존 processing job이 있으면 processing을 해제시킨다.
                        // => SetPickedContainer 응답에서 success시 기존 processing job을 해제
                        if (!string.IsNullOrEmpty(processingJobKey) && !job.jobKey.Equals(processingJobKey))
                        {
                            ContainerSelectionView.OldProcessingJobKeys.Add(processingJobKey);
                            if (processingJob != null && processingJob.type != null && !string.IsNullOrEmpty(processingJob.type.ycTwinKey))
                                ContainerSelectionView.OldProcessingJobKeys.Add(processingJob.type.ycTwinKey);                            
                        }

                        PresentationMgr.Singleton.SendPickedContainerAsk(job, null);
                    }
                    else
                    {
                        var twinJob = PresentationMgr.Singleton.JOB_Get(job.type.ycTwinKey);
                        if (twinJob != null)
                        {
                            // 2017/02/17 : 기존 processing job이 있으면 processing을 해제시킨다.
                            // => SetPickedContainer 응답에서 success시 기존 processing job을 해제
                            if (!string.IsNullOrEmpty(processingJobKey) &&
                                !job.jobKey.Equals(processingJobKey) && !twinJob.jobKey.Equals(processingJobKey))
                            {
                                ContainerSelectionView.OldProcessingJobKeys.Add(processingJobKey);                                
                                if (processingJob != null && processingJob.type != null && !string.IsNullOrEmpty(processingJob.type.ycTwinKey))
                                    ContainerSelectionView.OldProcessingJobKeys.Add(processingJob.type.ycTwinKey);                                    
                            }

                            if (this._isTwin)
                                PresentationMgr.Singleton.SendPickedContainerAsk(job, twinJob);
                            else    // twin JobOrder에 대해 single로 작업하는 경우 팝업을 띄우고 선택하게 한다.
                            {
                                PresentationMgr.MainView.UC_TwinSelectionView.SetTwinJobOrder(job.type.jobFlagInfo == "A" ? twinJob : job, job.type.jobFlagInfo == "A" ? job : twinJob, job.type.jobFlagInfo != "A");
                                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.TwinSelectionView);
                            }
                        }
                        else
                        {
                            // 2017/02/17 : 기존 processing job이 있으면 processing을 해제시킨다.
                            // => SetPickedContainer 응답에서 success시 기존 processing job을 해제
                            if (!string.IsNullOrEmpty(processingJobKey) && !job.jobKey.Equals(processingJobKey))
                            {
                                ContainerSelectionView.OldProcessingJobKeys.Add(processingJobKey);                                
                                if (processingJob != null && !string.IsNullOrEmpty(processingJob.type.ycTwinKey))
                                    ContainerSelectionView.OldProcessingJobKeys.Add(processingJob.type.ycTwinKey);                                    
                            }

                            PresentationMgr.Singleton.SendPickedContainerAsk(job, null);
                        }
                    }                
                    //PresentationMgr.Singleton.SendPickedContainerAsk(job,
                    //    (!this._isTwin || String.IsNullOrEmpty(job.type.ycTwinKey)) ? null : PresentationMgr.Singleton.JOB_Get(job.type.ycTwinKey));

                    if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                    {
                        if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(_SelectKey))
                            PresentationMgr.Singleton.RefreshJobInventory(job, true);
                    }
                }                
            }
        }

        private void ListBox_SearchJobOrder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is ContainerSearchControl))
                return;

            foreach (var cSearchControl in ListBox_SearchJobOrder.Children)
            {
                if (cSearchControl is ContainerSearchControl)
                {
                    (cSearchControl as ContainerSearchControl).Selected = false;
                }
            }

            ContainerSearchControl sSearchControl = e.Source as ContainerSearchControl;
            sSearchControl.Selected = true;
            _SelectKey = sSearchControl.JobOrder.jobKey;

            this.Btn_Done.IsEnabled = true;
        }
    }    
}
