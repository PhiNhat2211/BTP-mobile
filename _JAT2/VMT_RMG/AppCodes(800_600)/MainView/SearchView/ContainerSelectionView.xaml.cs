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

namespace VMT_RMG_800by600
{
    /// <summary>
    /// ContainerSelectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContainerSelectionView : UserControl
    {
        //private Int32 pageItemCount = 7;
        private List<ContainerSearchControl> _containerControlItems = null;
        private String _SelectKey = String.Empty;

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
                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;    // new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpDisableImage);                    

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownDisableImage);                    

                PresentationMgr.SetSkinButton(this.Btn_Done,
                    UIThemeMgr.Day.SelectionView_ButtonDefaultImage, UIThemeMgr.Day.SelectionView_ButtonPressImage, UIThemeMgr.Day.SelectionView_ButtonDefaultImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {    
                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;  //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

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
        
        public void SetSearchedJobList(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList)
        {
            this.Btn_Done.IsEnabled = false;
            
            this.ListBox_SearchJobOrder.Children.Clear();

            Int32 controlCount = 0;
            foreach (var jobOrder in jobOrderList)
            {
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

                this.ListBox_SearchJobOrder.Children.Add(control);

                controlCount++;
            }

            this.TextBlock_ResultCount.Text = Convert.ToString(this.ListBox_SearchJobOrder.Children.Count);
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

        private double scrollMoveOffSet = 69; // ContainerSearchControl DesignHeight
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
            if (!String.IsNullOrEmpty(_SelectKey))
            {
                var job = PresentationMgr.Singleton.JOB_Get(_SelectKey);
                if (job != null)
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

                    // 3. PLC data를 통한 pick 시에는 setPickedContainer 로 전달
                    Common.Util.Logger.Log("SendPickedContainerAsk");
                    PresentationMgr.Singleton.SendPickedContainerAsk(job,
                        String.IsNullOrEmpty(job.type.ycTwinKey) ? null : PresentationMgr.Singleton.JOB_Get(job.type.ycTwinKey));

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
