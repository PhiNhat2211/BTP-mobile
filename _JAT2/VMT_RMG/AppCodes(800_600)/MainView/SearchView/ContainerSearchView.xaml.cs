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
using VMT_RMG;

namespace VMT_RMG_800by600
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

            this.UC_KeyPad.Button_keypad_done_1.Content = "Swap";
            this.UC_KeyPad.Button_keypad_done_2.Content = "Swap";

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

            if (PresentationMgr.UseJobOrderByKeysAPI)
                this.CheckBox_SearchType.IsEnabled = true;
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
                {
                    PresentationMgr.ShowPartnerMachineSearchPopup(this._selectedContainerJobOrder);

                    if (DataMgr.Singleton.List_MachineofPool.Count <= 0)
                    {
                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            , "GetMachineList_Ask"), machineID);

                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                    else
                        PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
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
            
            PresentationMgr.Singleton.SetCorrectionSelect(null, String.Empty, String.Empty);

            if (PresentationMgr.Singleton.PrevUIMode != PresentationMgr.UIMode.ContainerDetailView &&
                PresentationMgr.Singleton.PrevUIMode != PresentationMgr.UIMode.BlockSelectionView)
            {
                this.TextBox_Search.Text = String.Empty;
                this.TextBlock_ResultCount.Text = String.Empty;
                this.ClearListItems();
                this.CheckBox_SearchType.IsChecked = false;
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
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Search_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative))
                    //);

                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;    // new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Up_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_ChangeToVBlock,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_PositionToRBlock,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_ContainerDetail,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_SearchType,
                    UIThemeMgr.Day.ContainerSearchView_CheckboxSelectITV, UIThemeMgr.Day.ContainerSearchView_CheckboxSelectCont, UIThemeMgr.Day.ContainerSearchView_CheckboxSelectCont);                
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Night.ContainerSearchView_ButtonSearchDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonSearchPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonSearchDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative))
                    //);

                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;  //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Night.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Night.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_ChangeToVBlock,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_PositionToRBlock,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_ContainerDetail,
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
                    PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(job.jobKey);
                }
                else
                {
                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;
                    PresentationMgr.Singleton.NeedJobAutoSelection = false;
                    PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);

                    var location = sSearchControl.JobOrder.locWorking;
                    if (PresentationMgr.UseFromLocationForRehandling == true && 
                        (sSearchControl.JobOrder.type.jobTp == "RH" || sSearchControl.JobOrder.type.jobTp == "AH"))
                        location = sSearchControl.JobOrder.locFrom;                    
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
            }
            else if (PresentationMgr.UseCorrection || App.TEST_MODE)
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
            PresentationMgr.Singleton.SetCorrectionSelect(
                new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = location.blck,
                    m_cBay = PresentationMgr.GetFrontOddBay(location.bay),
                    m_cRow = location.row,
                    m_cTier = location.tier
                },
                this._selectedContainerJobOrder.cntr.cntrNo, this._selectedContainerJobOrder.cntr.cntrIso);

            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.BlockSelectionView);
            PresentationMgr.MainView.UC_BlockSelectionView.MakeupBlockItemsForVBlock();
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

            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);
        }

        private void Btn_ContainerDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            if (this._selectedContainerJobOrder != null && !String.IsNullOrEmpty(this._selectedContainerJobOrder.cntr.cntrNo))
            {
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.ContainerDetailView);
                PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this._selectedContainerJobOrder.cntr.cntrNo, null);
            }
        }

        public void RefreshSearchResult()
        {
            this.Btn_Search_Click(null, null);
        }
    }
}
