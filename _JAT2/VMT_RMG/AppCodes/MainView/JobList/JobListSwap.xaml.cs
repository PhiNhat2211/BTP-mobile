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

namespace VMT_RMG
{

    /// <summary>
    /// Interaction logic for MovingViewJobList.xaml
    /// </summary>
    public partial class JobListSwap : UserControl
    {
        public List<SwapListItem> swapListItems = null;
        //public Int32 JobFilter = JobFilter_Type.TYPE_ALL;
        public Int32 pageItemCount = 10;
        private Int32 _currentPageIndex = 0;
        private Int32 _totalPageCount = 0;
        private String _preSelectedTargetCont = String.Empty;
        public VMT_Data_JAT2.Objects.Common.VmtSwap swapItem = null;
        public VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = null;
        public bool SwapAutoSelection = false;
        bool firstShow = true;
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

        public JobListSwap()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this.ListBox_Job.SelectionChanged += new SelectionChangedEventHandler(RMG_JobListSelectionChanged);

            this.swapListItems = new List<SwapListItem>(this.pageItemCount);
            for (int i = 0; i < pageItemCount; i++)
            {
                this.swapListItems.Add(new SwapListItem());
            }
        }

        ~JobListSwap()
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

            this.Btn_PageDown.Click += new RoutedEventHandler(Btn_PageDown_Click);
            this.Btn_PageUp.Click += new RoutedEventHandler(Btn_PageUp_Click);

            this.ListBox_Job.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListBox_Job_PreviewMouseLeftButtonUp);

            if (!App.STANDALONE_MODE)
                this.ListBox_Job.Items.Clear();

        }

        // Button Event
        private void Grid_SeparateType_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (PresentationMgr.MainView.UC_JobTypeFilterView.Visibility != System.Windows.Visibility.Visible)
                PresentationMgr.MainView.UC_JobTypeFilterView.Visibility = System.Windows.Visibility.Visible;
        }

        public void ListBox_setJobSelection(String jobKey)
        {
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobKey;
            this._preSelectedTargetCont = jobKey;

            if (!String.IsNullOrEmpty(jobKey))
            {
                PresentationMgr.Singleton.NeedMoveToTargetJobPage = true;
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

        public void ListBoxSeletion()
        {
            string[] arr = swapItem.swapPos.ToString().Split('-');
            string cBlock = arr[0];
            string cBay = PresentationMgr.GetFrontOddBay(arr[1]);
            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
            {
                m_cBlock = cBlock,
                m_cBay = cBay,
                m_cRow = arr[2],//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                m_cTier = arr[3]//PresentationMgr.Singleton.CurrentPostion.m_cTier
            };
            PresentationMgr.Singleton.CurrentPostion = pos;

            //PresentationMgr.Singleton.row = Int32.Parse(arr[0]);
            //PresentationMgr.Singleton.col = Int32.Parse(arr[1]);
            var blockbayInfo = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;
            if (!blockbayInfo.ContainsKey(cBlock))
                return;

            if (String.IsNullOrEmpty(cBlock) || String.IsNullOrEmpty(cBay))
                return;

            var currentInventory = this.GetViewInventorys(cBlock, cBay);
            if (currentInventory == null)
                return;
            //else if (currentInventory.Count <= 0)
            //{                
            //    currentInventory = null;
            //    return;
            //}

            if (blockbayInfo[cBlock].DicBay != null &&
                blockbayInfo[cBlock].DicBay.ContainsKey(cBay) &&
                blockbayInfo[cBlock].DicBay[cBay].RowNameMap.Count > 0)
            {
                var rowMap = blockbayInfo[cBlock].DicBay[cBay].RowNameMap;
                var direction = blockbayInfo[cBlock].Direction;
                Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, arr[2], direction);
                Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(arr[3]) ? "0" : arr[3]);

                if (firstShow == true)
                {
                    if (tRowNum > 7) PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewPointRow = tRowNum - 7;
                    if (tTierNum > 7) PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewPointTier = tTierNum - 7;
                    firstShow = false;
                }

                Int32 startRowS = PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewPointRow;
                Int32 startTierS = PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.CurrentBayTier -
                    (PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewPointTier);


                PresentationMgr.Singleton.rowsw = tRowNum + 1 - startRowS;
                PresentationMgr.Singleton.colsw = tTierNum - startTierS;
                PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Target(tRowNum + 1 - startRowS, tTierNum - startTierS);

            }      
        }

        void ListBox_Job_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView &&
                PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.ContainerDetailView)
                return;

            if (this.ListBox_Job.SelectedIndex < 0)
                return;

            //PresentationMgr.Singleton.NeedJobAutoSelection = true;

            foreach (SwapListItem jobListItem in this.ListBox_Job.Items)
            {
                jobListItem.Selected = false;
            }

            if (this.ListBox_Job.SelectedItem is SwapListItem)
            {
                SwapListItem item = this.ListBox_Job.SelectedItem as SwapListItem;
                item.Selected = true;


                if (_preSelectedTargetCont != item.contNo.Text)
                {
                    //this.CheckBox_Refresh.IsChecked = this.IsRefreshReserved = false;

                    _preSelectedTargetCont = item.contNo.Text;
                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetContSwap = item.contNo.Text;
                }
                else
                    PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);

                swapItem = item.swapItem;

                string[] arr = swapItem.swapPos.ToString().Split('-');
                string cBlock = arr[0];
                string cBay = PresentationMgr.GetFrontOddBay(arr[1]);
                pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = cBlock,
                    m_cBay = cBay,
                    m_cRow = arr[2],//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = arr[3]//PresentationMgr.Singleton.CurrentPostion.m_cTier
                };
                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> GetViewInventorys(String block, String bay)
        {
            var inventory = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

            var inven = this.INV_GetInventory(block, bay);
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            inven = this.INV_GetInventory(block, PresentationMgr.GetFrontEvenBay(bay));
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            inven = this.INV_GetInventory(block, PresentationMgr.GetRearEvenBay(bay));
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            return inventory;
        }
        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory(String block, String bay)
        {
            return DataMgr.Singleton.INV_GetInventory(block, bay);
        }

        void Btn_PageDown_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex < TotalPageCount - 1)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex + 1);
                PresentationMgr.Singleton.JLS_Refresh(this, CurrentPageIndex + 1, true);
            }
        }

        void Btn_PageUp_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex > 0)
            {
                //PresentationMgr.Singleton.JL_SearchNRefresh(this, PresentationMgr.Singleton.SelectBlckName, CurrentPageIndex - 1);
                PresentationMgr.Singleton.JLS_Refresh(this, CurrentPageIndex - 1, true);
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.SelectionView_ButtonUpDefaultImage, UIThemeMgr.Day.SelectionView_ButtonUpPressImage, UIThemeMgr.Day.SelectionView_ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.SelectionView_ButtonDownDefaultImage, UIThemeMgr.Day.SelectionView_ButtonDownPressImage, UIThemeMgr.Day.SelectionView_ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Down_Disable.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Night.SelectionView_ButtonUpDefaultImage, UIThemeMgr.Night.SelectionView_ButtonUpPressImage, UIThemeMgr.Night.SelectionView_ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Night.SelectionView_ButtonDownDefaultImage, UIThemeMgr.Night.SelectionView_ButtonDownPressImage, UIThemeMgr.Night.SelectionView_ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Down_Disable.png", UriKind.Relative))
                //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public void JobList_Add(VMT_Data_JAT2.Objects.Common.VmtSwap swapJob)
        {
            JobList_Del(swapJob.regoNo);

            SwapListItem jItem = new SwapListItem();
            jItem.SetJobInfo(swapJob, ListBox_Job.Items.Count);
            this.ListBox_Job.Items.Add(jItem);
        }

        public void JobList_SelectFirstItem()
        {
            if (this.ListBox_Job.Items.Count == 0)
                return;
            if (!this.SwapAutoSelection)
                this.ListBox_Job.SelectedIndex = 0;
            ListBox_Job_PreviewMouseLeftButtonUp(null, null);
        }


        public void JobList_Del(String regoNo)
        {
            foreach (SwapListItem jItem in this.ListBox_Job.Items)
            {
                if (jItem.swapItem.regoNo.Equals(regoNo))
                {
                    this.ListBox_Job.Items.Remove(jItem);
                    break;
                }
            }
        }

        public void JobList_Clear()
        {
            this._preSelectedTargetCont = String.Empty;
            this.ListBox_Job.Items.Clear();
        }
    }
}