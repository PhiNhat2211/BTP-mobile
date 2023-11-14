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
    /// Interaction logic for ContainerSearchView.xaml
    /// </summary>
    public partial class ContainerSearchView : UserControl
    {
        private Int32 pageItemCount = 4;

        private List<ContainerSearchControl> _containerControlItems = null;
        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _selectedContainerJobOrder = null;
        private String _selectedContainerNum = String.Empty;

        public ContainerSearchView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this._containerControlItems = new List<ContainerSearchControl>();
        }

        ~ContainerSearchView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.TextBox_Search.Text = String.Empty;
            this.TextBlock_ResultCount.Text = String.Empty;
            this.ListBox_SearchJobOrder.Children.Clear();

            //this.UC_KeyPad.Button_keypad_done_1.Content = "Swap";
            //this.UC_KeyPad.Button_keypad_done_2.Content = "Swap";
            this.UC_KeyPad.Button_keypad_done_1.Visibility = Visibility.Hidden;
            this.UC_KeyPad.Button_keypad_done_2.Visibility = Visibility.Hidden;

            this.UC_KeyPad.ShowKeyPad(this.TextBox_Search);

            // Init Event Handler
            this.Btn_Search.PreviewMouseLeftButtonUp +=
                new MouseButtonEventHandler((s, arg) => this._selectedContainerNum = String.Empty);
            this.Btn_Search.Click += new RoutedEventHandler(Btn_Search_Click);

            this.Btn_PageUp.Click += new RoutedEventHandler(Btn_PageUp_Click);
            this.Btn_PageDown.Click += new RoutedEventHandler(Btn_PageDown_Click);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(ContainerSearchView_IsVisibleChanged);

            this.ListBox_SearchJobOrder.MouseDown += new MouseButtonEventHandler(ListBox_SearchJobOrder_MouseDown); // Double Click
            this.ListBox_SearchJobOrder.MouseLeftButtonUp += new MouseButtonEventHandler(ListBox_SearchJobOrder_MouseLeftButtonUp);

            this.UC_KeyPad.DoneCallback += new Keypad.KeypadDoneCallback(this.KeypadDone);

            this.Btn_ChangeToVBlock.Click += new RoutedEventHandler(this.Btn_ChangeToVBlock_Click);
            this.Btn_PositionToRBlock.Click += new RoutedEventHandler(this.Btn_PositionToRBlock_Click);
            this.Btn_ContainerDetail.Click += new RoutedEventHandler(this.Btn_ContainerDetail_Click);
            this.CheckBox_SearchType.Click += new RoutedEventHandler(this.CheckBox_SearchType_Click);
            this.Btn_Chassis.Click += new RoutedEventHandler(this.Btn_Chassis_Click);
            this.Btn_Flip.Click += new RoutedEventHandler(this.Btn_Flip_Click);

            this.CheckBox_SearchType.IsEnabled = true;

            this.UC_KeyPad.ShowTruckKey(true);
            LoadLanguage();
        }
        private void LoadLanguage()
        {
            TextBlock_Result.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0067", LanguageService.LABEL_SEARCH);
            Btn_Chassis.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0068", LanguageService.LABEL_SEARCH);
            Btn_Flip.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0069", LanguageService.LABEL_SEARCH);
            Btn_ChangeToVBlock.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0070", LanguageService.LABEL_SEARCH);
            Btn_PositionToRBlock.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0071", LanguageService.LABEL_SEARCH);
            Btn_ContainerDetail.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0072", LanguageService.LABEL_SEARCH);
            Tbl_Container.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0073", LanguageService.LABEL_SEARCH);
            Tbl_ISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0074", LanguageService.LABEL_SEARCH);
            Tbl_TruckNo.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0075", LanguageService.LABEL_SEARCH);
            Tbl_WaitJob.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0076", LanguageService.LABEL_SEARCH);
            Tbl_Location.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0077", LanguageService.LABEL_SEARCH);
            Tbl_PlanLocation.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0078", LanguageService.LABEL_SEARCH);
        }
        private void KeypadDone() // Swap
        {
            String machineID = String.Empty;
            if (GetSelectedMachineID(ref machineID))
            {
                //if (!String.IsNullOrEmpty(machineID))
                //{
                //    PresentationMgr.ShowPartnerMachineSearchPopup();

                //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                //        , "GetMachineListofPool_Ask"), machineID);

                //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineListofPool_Ask(machineID);
                //    PresentationMgr.AppWin.ShowProgressBar(0);
                //}
                //else
                String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                if (jobOrder == null)
                    return;
                if (!"GO".Equals(jobOrder.type.jobTp))
                {
                    PresentationMgr.ShowYtSwapPopup(this._selectedContainerJobOrder);

                    //if (DataMgr.Singleton.List_MachineofPool.Count <= 0)
                    //{
                    //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    //        , "GetMachineList_Ask"), machineID);

                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
                    //    PresentationMgr.AppWin.ShowProgressBar(0);
                    //}
                    //else
                    //    PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
                }
                else
                {
                    //20210406 getSwapList blockName-bayName
                    String bayName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text.ToString();
                    if (bayName.Length > 1) bayName = "-" + bayName.Substring(0, 2);
                    else bayName = String.Empty;
                    VMT_Data_JAT2.VMT_DataMgr_Common.getSwapList_Ask(jobKey, PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text.ToString() + bayName, PresentationMgr.MainView.UC_SwapView.CheckBox_Block_All.IsChecked == true);
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.SwapView);
                }
            }
        }

        private Boolean GetSelectedJob(ref String jobKey)
        {
            foreach (var uiElement in this.ListBox_SearchJobOrder.Children)
            {
                if (uiElement is ContainerSearchControl)
                {
                    ContainerSearchControl control = (ContainerSearchControl)uiElement;
                    if (control.Selected)
                    {
                        jobKey = control.JobOrder.jobKey;
                        return true;
                    }
                }
            }
            return false;
        }

        private Boolean GetSelectedMachineID(ref String machineID)
        {
            foreach (var uiElement in this.ListBox_SearchJobOrder.Children)
            {
                if (uiElement is ContainerSearchControl)
                {
                    ContainerSearchControl control = (ContainerSearchControl)uiElement;
                    if (control.Selected)
                    {
                        machineID = control.TextBlock_TruckNo.Text;
                        return true;
                    }
                }
            }
            return false;
        }

        private void ContainerSearchView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            //PresentationMgr.Singleton.SetCorrectionSelect(null, String.Empty, String.Empty); //20200207 why remove position?

            if (PresentationMgr.Singleton.PrevUIMode != PresentationMgr.UIMode.ContainerDetailView &&
                PresentationMgr.Singleton.PrevUIMode != PresentationMgr.UIMode.BlockSelectionView)
            {
                this.TextBox_Search.Text = String.Empty;
                this.TextBlock_ResultCount.Text = String.Empty;
                this.ClearListItems();
                this.CheckBox_SearchType.IsChecked = true;
                this._selectedContainerNum = String.Empty;
            }

            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                this.TextBox_Search.Focus();
            }));
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String searchStr = this.TextBox_Search.Text.ToUpper();

            if (true == this.CheckBox_SearchType.IsChecked) // jobOrderByTruck search
            {
                if (searchStr.Length < 3)
                    return;

                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "GetJobOrderListByTruck_Ask"), searchStr);
                //VMT_Data_JAT2.VMT_DataMgr_Common.JobOrderListByTruck_Ask(false, searchStr);
                VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByTruck_Ask(false, searchStr, true);
            }
            else
            {
                if (searchStr.Length < 4)
                    return;

                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "GetJobOrderByContainer_Ask"), searchStr);
                VMT_Data_JAT2.VMT_DataMgr_RMG.GetJobOrderByContainer_Ask(searchStr);
            }
            PresentationMgr.AppWin.ShowProgressBar(0);

            this.Scroll_SearchJobOrder.ScrollToVerticalOffset(0);
        }

        private void CheckBox_SearchType_Click(object sender, RoutedEventArgs e)
        {
            this.TextBox_Search.Text = String.Empty;
            this.TextBox_Search.Focus();
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Day.ContainerSearchView_ButtonSearchDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonSearchPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonSearchDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Search_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative))
                //);

                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;    // new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/ContainerSearchView_Down_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_ChangeToVBlock,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_PositionToRBlock,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_ContainerDetail,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Chassis,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Flip,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_SearchType,
                    UIThemeMgr.Day.ContainerSearchView_CheckboxSelectITV, UIThemeMgr.Day.ContainerSearchView_CheckboxSelectCont, UIThemeMgr.Day.ContainerSearchView_CheckboxSelectCont);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Night.ContainerSearchView_ButtonSearchDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonSearchPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonSearchDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative))
                //);

                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;  //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Night.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Night.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_ChangeToVBlock,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_PositionToRBlock,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_ContainerDetail,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Chassis,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Flip,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_SearchType,
                    UIThemeMgr.Night.ContainerSearchView_CheckboxSelectITV, UIThemeMgr.Night.ContainerSearchView_CheckboxSelectCont, UIThemeMgr.Night.ContainerSearchView_CheckboxSelectCont);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void ClearListItems()
        {
            this.ClearListSelection();

            this.ListBox_SearchJobOrder.Children.Clear();

            if (this._SearchJobOrderList != null)
            {
                this._SearchJobOrderList.Clear();
                this._SearchJobOrderList = null;
            }
        }

        private VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList _SearchJobOrderList = null;
        public void SetSearchedJobList(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderList)
        {
            this.ClearListItems();

            this._SearchJobOrderList = jobOrderList;

            Int32 controlCount = 0;
            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in jobOrderList.JobOrder)
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
                if (this._selectedContainerNum.Equals(jobOrder.cntr.cntrNo))
                    this.ListBox_SearchJobOrder_OnSelection(control);

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
            this.ClearListSelection();
        }

        private double scrollMoveOffSet = 65; // ContainerSearchControl DesignHeight
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

        private void ListBox_SearchJobOrder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
                return;

            if (!(e.Source is ContainerSearchControl))
                return;

            // Double Click
            ContainerSearchControl sSearchControl = e.Source as ContainerSearchControl;

            if (sSearchControl.JobOrder != null)
            {
                String cntrNo = sSearchControl.JobOrder.cntr.cntrNo;
                var jobList = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);
                if (jobList.Count > 0)
                {
                    var job = jobList.First();

                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;

                    if (PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem is JobListItem)
                    {
                        JobListItem item = PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem as JobListItem;
                        if (job.jobKey == item.JobKey)
                        {
                            PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = string.Empty;
                            if (PresentationMgr.Singleton.CurrentRow.Equals(string.Empty))
                            {
                                PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
                            }
                        }
                    }

                    foreach (JobListItem jobListItem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
                    {
                        if (jobListItem.Selected)
                            jobListItem.Selected = false;
                    }

                    string jobCntrNo = job.cntr.cntrNo;
                    jobCntrNo = jobCntrNo.Length > 8 ? (jobCntrNo.Substring(0, 2) + jobCntrNo.Substring(jobCntrNo.Length - 7)) : jobCntrNo;

                    foreach (JobListItem jobListItem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
                    {
                        if (jobListItem.TextBlock_CntrNo.Text == jobCntrNo)
                        {
                            PresentationMgr.Singleton.NeedMoveToTargetJobPage = true;
                            PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem = jobListItem;
                            PresentationMgr.MainView.UC_JobList.ListBox_Job_MouseUp(null, null);
                            return;
                        }

                    }
                }

                PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;
                PresentationMgr.Singleton.NeedJobAutoSelection = false;
                //NeedMoveToTargetJobPage
                PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
                PresentationMgr.Singleton.jobAfterSearch = sSearchControl.JobOrder;

                var location = sSearchControl.JobOrder.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (sSearchControl.JobOrder.type.jobTp == "RH" || sSearchControl.JobOrder.type.jobTp == "AH"))
                    location = sSearchControl.JobOrder.locFrom;
                else if (sSearchControl.JobOrder.type.jobTp == "LD" || sSearchControl.JobOrder.type.jobTp == "GO" || sSearchControl.JobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(sSearchControl.JobOrder.locFrom.location) ? sSearchControl.JobOrder.locWorking : sSearchControl.JobOrder.locFrom;

                PresentationMgr.MainView.selectedJobList = true;

                if (!"".Equals(PresentationMgr.GetContainerStatus(sSearchControl.JobOrder.type.jobTp, sSearchControl.JobOrder.type.jobStatus)))
                {
                    if (location != null && !String.IsNullOrEmpty(location.blck)) PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text = location.blck;
                    PresentationMgr.Singleton.NeedMoveToTargetJobPage = true;
                    PresentationMgr.MainView.UC_JobList.ListBox_Job.Items.Clear();
                    PresentationMgr.MainView.SetBlockMode(sSearchControl.JobOrder);
                } else
                {
                    PresentationMgr.Singleton.CurrentPostion.m_cRow = String.Empty;
                    PresentationMgr.Singleton.CurrentPostion.m_cTier = String.Empty;

                    PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.ClearContainerCorrectionSelect();
                    PresentationMgr.Singleton.CorrectionSource.Clear();

                    var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = location.blck,
                        m_cBay = PresentationMgr.GetFrontOddBay(location.bay),
                        m_cRow = location.row,
                        m_cTier = location.tier
                    };
                    PresentationMgr.Singleton.CurrentPostion = pos;
                }
                
            }
        }

        private void ListBox_SearchJobOrder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is ContainerSearchControl))
                return;

            this.ListBox_SearchJobOrder_OnSelection(e.Source as ContainerSearchControl);
        }

        private void ListBox_SearchJobOrder_OnSelection(ContainerSearchControl control)
        {
            this.ClearListSelection();

            control.Selected = true;
            this._selectedContainerJobOrder = control.JobOrder;

            if (this._selectedContainerJobOrder == null || String.IsNullOrEmpty(this._selectedContainerJobOrder.cntr.cntrNo))
            {
                this.Btn_ContainerDetail.IsEnabled = false;
                this.Btn_ChangeToVBlock.IsEnabled = false;
                this.Btn_PositionToRBlock.IsEnabled = false;
                this.Btn_Chassis.IsEnabled = false;
                this.Btn_Flip.IsEnabled = false;
            }
            else if (PresentationMgr.UseCorrection)
            {
                this._selectedContainerNum = control.JobOrder.cntr.cntrNo;

                this.Btn_ContainerDetail.IsEnabled = true;

                if (!String.IsNullOrEmpty(this._selectedContainerJobOrder.jobKey) &&
                    null == PresentationMgr.Singleton.JOB_Get(this._selectedContainerJobOrder.jobKey))
                {
                    this.Btn_ChangeToVBlock.IsEnabled = false;
                    this.Btn_PositionToRBlock.IsEnabled = false;
                }
                else
                {
                    var location = this._selectedContainerJobOrder.locWorking;
                    if (PresentationMgr.UseFromLocationForRehandling == true &&
                        (this._selectedContainerJobOrder.type.jobTp == "RH" || this._selectedContainerJobOrder.type.jobTp == "AH"))
                        location = this._selectedContainerJobOrder.locFrom;
                    else if (_selectedContainerJobOrder.type.jobTp == "LD" || _selectedContainerJobOrder.type.jobTp == "GO" || _selectedContainerJobOrder.type.jobTp == "MO")
                        location = string.IsNullOrEmpty(_selectedContainerJobOrder.locFrom.location) ? _selectedContainerJobOrder.locWorking : _selectedContainerJobOrder.locFrom;

                    var containerBlock = location.blck;
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(containerBlock))
                    {
                        if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[containerBlock].IsVirtual)
                            this.Btn_PositionToRBlock.IsEnabled = this.Btn_ChangeToVBlock.IsEnabled =
                                String.IsNullOrEmpty(location.blck) ? false : true;
                        else
                        {
                            this.Btn_PositionToRBlock.IsEnabled = this.Btn_ChangeToVBlock.IsEnabled =
                                (String.IsNullOrEmpty(location.blck) ||
                                String.IsNullOrEmpty(location.bay) ||
                                String.IsNullOrEmpty(location.row) ||
                                String.IsNullOrEmpty(location.tier)) ? false : true;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(containerBlock))
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(containerBlock);

                        this.Btn_ChangeToVBlock.IsEnabled = !String.IsNullOrEmpty(location.location) ? true : false;
                        this.Btn_PositionToRBlock.IsEnabled = !String.IsNullOrEmpty(location.location) ? true : false;
                    }
                }
            }
        }

        private void ClearListSelection()
        {
            foreach (var cSearchControl in ListBox_SearchJobOrder.Children)
            {
                if (cSearchControl is ContainerSearchControl)
                {
                    (cSearchControl as ContainerSearchControl).Selected = false;
                }
            }

            this._selectedContainerJobOrder = null;
            this.Btn_ContainerDetail.IsEnabled = false;
            this.Btn_ChangeToVBlock.IsEnabled = false;
            this.Btn_PositionToRBlock.IsEnabled = false;
            this.Btn_Chassis.IsEnabled = false;
            this.Btn_Flip.IsEnabled = false;
        }

        // phase 2
        private void Btn_ChangeToVBlock_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            if (this._selectedContainerJobOrder == null || String.IsNullOrEmpty(this._selectedContainerJobOrder.cntr.cntrNo))
                return;

            var location = this._selectedContainerJobOrder.locWorking;
            if (PresentationMgr.UseFromLocationForRehandling == true &&
                (this._selectedContainerJobOrder.type.jobTp == "RH" || this._selectedContainerJobOrder.type.jobTp == "AH"))
                location = this._selectedContainerJobOrder.locFrom;
            else if (_selectedContainerJobOrder.type.jobTp == "LD" || _selectedContainerJobOrder.type.jobTp == "GO" || _selectedContainerJobOrder.type.jobTp == "MO")
                location = string.IsNullOrEmpty(_selectedContainerJobOrder.locFrom.location) ? _selectedContainerJobOrder.locWorking : _selectedContainerJobOrder.locFrom;

            PresentationMgr.Singleton.SetCorrectionSelect(
                new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = location.blck,
                    m_cBay = PresentationMgr.GetFrontOddBay(location.bay),
                    m_cRow = location.row,
                    m_cTier = location.tier
                },
                this._selectedContainerJobOrder.cntr.cntrNo, this._selectedContainerJobOrder.cntr.cntrIso);
            PresentationMgr.Singleton.PrevBlckBayUIMode = PresentationMgr.Singleton.CurrentUIMode;
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.VirtualBlockSelectionView);
            PresentationMgr.MainView.UC_VirtualBlockSelectionView.MakeupBlockItemsForVBlock();
        }

        private void Btn_PositionToRBlock_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            PresentationMgr.Singleton.SetInventoryData(null);
            if (this._selectedContainerJobOrder != null)
            {
                var location = this._selectedContainerJobOrder.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (this._selectedContainerJobOrder.type.jobTp == "RH" || this._selectedContainerJobOrder.type.jobTp == "AH"))
                    location = this._selectedContainerJobOrder.locFrom;
                else if (_selectedContainerJobOrder.type.jobTp == "LD" || _selectedContainerJobOrder.type.jobTp == "GO" || _selectedContainerJobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(_selectedContainerJobOrder.locFrom.location) ? _selectedContainerJobOrder.locWorking : _selectedContainerJobOrder.locFrom;

                String cntrNo = this._selectedContainerJobOrder.cntr.cntrNo;
                var jobList = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);
                if (jobList.Count > 0)
                {
                    var job = jobList.First();
                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;
                    PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(job.jobKey);
                }
                else
                {
                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;
                    PresentationMgr.Singleton.NeedJobAutoSelection = false;
                    PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);

                    var block = location.blck;
                    var bay = location.bay;
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) && !DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].IsVirtual)
                    {
                        var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                        {
                            m_cBlock = block,
                            m_cBay = PresentationMgr.GetFrontOddBay(bay),
                            m_cRow = location.row,
                            m_cTier = location.tier
                        };

                        PresentationMgr.Singleton.CurrentPostion = pos;
                    }
                }

                PresentationMgr.Singleton.SetCorrectionSelect(
                    new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = location.blck,
                        m_cBay = PresentationMgr.GetFrontOddBay(location.bay),
                        m_cRow = location.row,
                        m_cTier = location.tier
                    },
                    this._selectedContainerJobOrder.cntr.cntrNo, this._selectedContainerJobOrder.cntr.cntrIso);
            }

            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
        }

        private void Btn_ContainerDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            if (this._selectedContainerJobOrder != null && !String.IsNullOrEmpty(this._selectedContainerJobOrder.cntr.cntrNo))
            {
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
                PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this._selectedContainerJobOrder.cntr.cntrNo, null);
            }
        }

        private void Btn_Chassis_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            //DEFINE LATER
        }

        private void Btn_Flip_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            //DEFINE LATER
        }

        public void RefreshSearchResult()
        {
            this.Btn_Search_Click(null, null);
        }
    }
}
