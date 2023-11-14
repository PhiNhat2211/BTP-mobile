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
using VMT_RMG;

namespace VMT_RMG_800by600
{
	/// <summary>
	/// Interaction logic for JobView.xaml
	/// </summary>
	public partial class BaySelectionView : UserControl
	{
        private List<BayJobControl> _bayControlItems = null;

        private String _selectedBlockName = String.Empty;
        public String SelectedBlockName
        {
            get { return this._selectedBlockName; }
            set { this.TextBox_BlockID.Text = _selectedBlockName = value; }
        }

        private String _selectedBayName = String.Empty;
        public String SelectedBayName
        {
            get { return this._selectedBayName; }
            set { this.TextBox_BayID.Text = _selectedBayName = value; }
        }


        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _jobOrder;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder JobOrder
        {
            get { return _jobOrder; }
            set { _jobOrder = value; }
            // set { _jobOrder = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(value); }
        }

		public BaySelectionView()
		{
			this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
		}
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_Navi_Left.Click += new RoutedEventHandler(Btn_Navi_Left_Click);
            this.Btn_Navi_Right.Click += new RoutedEventHandler(Btn_Navi_Right_Click);

            this.Btn_Previous.Click += new RoutedEventHandler(Btn_Back_Click);
            this.Btn_Done_Two.Click += new RoutedEventHandler(Btn_Confirm_Click);
            this.Btn_Done_One.Click += new RoutedEventHandler(Btn_Confirm_Click);
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(BaySelectionView_IsVisibleChanged);

            this.Wrap_BaySelectionView.MouseLeftButtonUp += new MouseButtonEventHandler(Wrap_BaySelectionView_MouseLeftButtonUp);

            this.SelectedBayName = String.Empty;
            _bayControlItems = new List<BayJobControl>();

            this.scrollMoveOffSet = this.Scroll_BaySelection.ViewportWidth - 1.8;
        }

        private void BaySelectionView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility != Visibility.Visible)
                return;

            this.Wrap_BaySelectionView.Children.Clear();
        }

        private void Scroll_BaySelection_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer))
                return;

            ScrollViewer scroller = sender as ScrollViewer;

            if (scroller.ContentHorizontalOffset == 0)
            {
                this.Btn_Navi_Left.IsEnabled = false;
            }
            else
            {
                this.Btn_Navi_Left.IsEnabled = true;
            }

            if (scroller.ScrollableWidth == scroller.ContentHorizontalOffset)
            {
                this.Btn_Navi_Right.IsEnabled = false;
            }
            else
            {
                this.Btn_Navi_Right.IsEnabled = true;
            }
        }

        private double scrollMoveOffSet = 80 * 9; // BayJobControl DesignWidth * 9
        void Btn_Navi_Left_Click(object sender, RoutedEventArgs e)
        {            
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BaySelection.ContentHorizontalOffset;

            if (currentOffset - scrollMoveOffSet > 0)
                this.Scroll_BaySelection.ScrollToHorizontalOffset(currentOffset - scrollMoveOffSet);
            else
                this.Scroll_BaySelection.ScrollToHorizontalOffset(0);
        }

        void Btn_Navi_Right_Click(object sender, RoutedEventArgs e)
        {            
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BaySelection.ContentHorizontalOffset;
            if (currentOffset + scrollMoveOffSet < this.Scroll_BaySelection.ScrollableWidth)
                this.Scroll_BaySelection.ScrollToHorizontalOffset(currentOffset + scrollMoveOffSet);
            else
                this.Scroll_BaySelection.ScrollToHorizontalOffset(this.Scroll_BaySelection.ScrollableWidth);
        }

        void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.BlockSelectionView);
            PresentationMgr.MainView.UC_BlockSelectionView.MakeupBlockItemsWithTabSelection();
        }
        
        void Btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();            
            //String bay = String.Empty;            
            //if (GetSelectedBayName(ref bay))
            if (!String.IsNullOrEmpty(this.SelectedBayName))
            {
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);

                PresentationMgr.Singleton.CurrentPostion.m_cBay = String.Empty;
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = this.SelectedBayName,
                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                };
                
                PresentationMgr.Singleton.CurrentPostion = pos;                               
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                this.Image_BaySelectionInfo.Source = UIThemeMgr.Day.SelectionView_BaySelectionInfoImage;
                //this.Image_BaySelectionInfo.Source = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionInfo.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_Navi_Left,
                    UIThemeMgr.Day.SelectionView_ButtonBayLeftDefaultImage, UIThemeMgr.Day.SelectionView_ButtonBayLeftPressImage, UIThemeMgr.Day.SelectionView_ButtonBayLeftDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_Right,
                    UIThemeMgr.Day.SelectionView_ButtonBayRightDefaultImage, UIThemeMgr.Day.SelectionView_ButtonBayRightPressImage, UIThemeMgr.Day.SelectionView_ButtonBayRightDisableImage);                    
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Previous,
                    UIThemeMgr.Day.SelectionView_ButtonLeftDefaultImage, UIThemeMgr.Day.SelectionView_ButtonLeftPressImage, UIThemeMgr.Day.SelectionView_ButtonLeftDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Left_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Done_Two,
                    UIThemeMgr.Day.SelectionView_ButtonRightDefaultImage, UIThemeMgr.Day.SelectionView_ButtonRightPressImage, UIThemeMgr.Day.SelectionView_ButtonRightDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Right_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Done_One,
                    UIThemeMgr.Day.SelectionView_ButtonDefaultImage, UIThemeMgr.Day.SelectionView_ButtonPressImage, UIThemeMgr.Day.SelectionView_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Default.png", UriKind.Relative))
                    //);

            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                this.Image_BaySelectionInfo.Source = UIThemeMgr.Night.SelectionView_BaySelectionInfoImage;
                //this.Image_BaySelectionInfo.Source = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionInfo.png", UriKind.Relative));

                PresentationMgr.SetSkinButton(this.Btn_Navi_Left,
                    UIThemeMgr.Night.SelectionView_ButtonBayLeftDefaultImage, UIThemeMgr.Night.SelectionView_ButtonBayLeftPressImage, UIThemeMgr.Night.SelectionView_ButtonBayLeftDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_Right,
                    UIThemeMgr.Night.SelectionView_ButtonBayRightDefaultImage, UIThemeMgr.Night.SelectionView_ButtonBayRightPressImage, UIThemeMgr.Night.SelectionView_ButtonBayRightDisableImage);                    
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Previous,
                    UIThemeMgr.Night.SelectionView_ButtonLeftDefaultImage, UIThemeMgr.Night.SelectionView_ButtonLeftPressImage, UIThemeMgr.Night.SelectionView_ButtonLeftDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Left_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Done_Two,
                    UIThemeMgr.Night.SelectionView_ButtonRightDefaultImage, UIThemeMgr.Night.SelectionView_ButtonRightPressImage, UIThemeMgr.Night.SelectionView_ButtonRightDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Right_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Done_One,
                    UIThemeMgr.Night.SelectionView_ButtonDefaultImage, UIThemeMgr.Night.SelectionView_ButtonPressImage, UIThemeMgr.Night.SelectionView_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_default.png", UriKind.Relative))
                    //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public void MakeupBayItems()
        {
            this.Wrap_BaySelectionView.Children.Clear();

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock))            
            {
                PresentationMgr.AppWin.ShowProgressBar(0);

                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "GetBlockMapList_Ask"), PresentationMgr.Singleton.CurrentBlock);

                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(PresentationMgr.Singleton.CurrentBlock);
                return;
            }

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay == null)
                return;

            var bayList = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay.Values.ToList();
            bayList.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);

            Int32 controlCount = 0;
            BayJobControl selectedControl = null;
            foreach (var item in bayList)
            {
                BayJobControl control = null;
                if (this._bayControlItems.Count <= controlCount)
                {
                    control = new BayJobControl();
                    this._bayControlItems.Add(control);
                }
                else
                {
                    control = this._bayControlItems[controlCount];
                    control.Clear();
                }
                
                control.BayName = item.BayName;
                if (!String.IsNullOrEmpty(this.SelectedBayName) && control.BayName.Equals(this.SelectedBayName))
                {
                    control.IsSelected = true;
                    selectedControl = control;
                }

                // job matching
                var jobForBlck = PresentationMgr.Singleton.JOB_GetForBlockBay(PresentationMgr.Singleton.CurrentBlock, item.BayName);
                var intBay = Convert.ToInt32(item.BayName);
                if (intBay % 2 == 1)
                {
                    var evenBay = Convert.ToString(intBay + 1);
                    if (evenBay.Length == 1)
                        evenBay = "0" + evenBay;
                    jobForBlck.AddRange(PresentationMgr.Singleton.JOB_GetForBlockBay(PresentationMgr.Singleton.CurrentBlock, evenBay));
                }
                foreach (var jobOrder in jobForBlck)
                {
                    control.AddJobOrder(jobOrder);                    
                }
                jobForBlck.Clear();
                jobForBlck = null;

                this.Wrap_BaySelectionView.Children.Add(control);

                controlCount++;
            }

            if (selectedControl != null)
            {
                selectedControl.Focus();
                this.Btn_Done_Two.IsEnabled = true;
            }
            else
            {
                this.SelectedBayName = String.Empty;
                this.Btn_Done_Two.IsEnabled = false;
            }
        }

        //private Boolean GetSelectedBayName(ref String block)
        //{
        //    foreach (var uiElement in this.Wrap_BaySelectionView.Children)
        //    {
        //        if (uiElement is BayJobControl)
        //        {
        //            BayJobControl bjc = (BayJobControl)uiElement;
        //            if (bjc.IsSelected)
        //            {
        //                block = bjc.BayName;
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        // Method Function
        private Boolean IsSelected()
        {
            return true;
        }

        void Wrap_BaySelectionView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (!(e.Source is BayJobControl))
            //    return;

            //foreach (var cSearchControl in Wrap_BaySelectionView.Children)
            //{
            //    if (cSearchControl is BayJobControl)
            //    {
            //        (cSearchControl as BayJobControl).IsSelected = false;
            //    }
            //}

            //BayJobControl bJobControl = e.Source as BayJobControl;
            //bJobControl.IsSelected = true;
        }

	}
}