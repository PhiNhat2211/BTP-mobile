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
    /// Interaction logic for JobView.xaml
    /// </summary>

    public partial class BlockSelectionView1 : UserControl
    {
        private enum ViewMode
        {
            MODE_NORMAL = 0,
            MODE_CORRECTION,
        }
        public PresentationMgr.UIMode PrevUIMode = PresentationMgr.UIMode.MainView;
        private ViewMode _currentMode = ViewMode.MODE_NORMAL;

        private List<BlockJobControl1> _blockControlItems = null;
        private String _selectedBlockName = String.Empty;
        public String SelectedBlockName 
        {
            get { return this._selectedBlockName; }
            set { this.TextBox_BlockID.Text = _selectedBlockName = value;}            
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _jobOrder;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder JobOrder
        {
            get { return _jobOrder; }
            set { _jobOrder = value; }
            // set { _jobOrder = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(value); }
        }

        public BlockSelectionView1()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
            
            _blockControlItems = new List<BlockJobControl1>();
        }       

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_All_Block.Click += new RoutedEventHandler(Btn_All_Block_Click);
            this.Btn_Area_Block.Click += new RoutedEventHandler(Btn_Area_Block_Click);
            this.Btn_Virtual_Block.Click += new RoutedEventHandler(Btn_Virtual_Block_Click);

            this.Btn_Navi_Left.Click += new RoutedEventHandler(Btn_Navi_Left_Click);
            this.Btn_Navi_Right.Click += new RoutedEventHandler(Btn_Navi_Right_Click);

            this.Btn_Next.Click += new RoutedEventHandler(Btn_Confirm_Click);

            this.Wrap_BlockSelectionView.MouseLeftButtonUp += new MouseButtonEventHandler(Wrap_BlockSelectionView_MouseLeftButtonUp);
        }

        private void Btn_Virtual_Block_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            Btn_All_Block.IsChecked = false;
            Btn_Area_Block.IsChecked = false;

            if (Btn_Virtual_Block.IsChecked == false)
            {
                Btn_Virtual_Block.IsChecked = true;
                return;
            }

            this.MakeupBlockItems();
        }

        private void Btn_Area_Block_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            Btn_All_Block.IsChecked = false;
            Btn_Virtual_Block.IsChecked = false;

            if (Btn_Area_Block.IsChecked == false)
            {
                Btn_Area_Block.IsChecked = true;
                return;
            }            

            this.MakeupBlockItems();
        }

        private void Btn_All_Block_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            Btn_Area_Block.IsChecked = false;
            Btn_Virtual_Block.IsChecked = false;

            if (Btn_All_Block.IsChecked == false)
            {
                Btn_All_Block.IsChecked = true;
                return;
            }            

            this.MakeupBlockItems();
        }

        private void Scroll_BlockSelection_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
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

        private double scrollMoveOffSet = 220 * 4; // BlockJobControl DesignWidth
        private void Btn_Navi_Left_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BlockSelection.ContentHorizontalOffset;

            if (currentOffset - scrollMoveOffSet > 0)
                this.Scroll_BlockSelection.ScrollToHorizontalOffset(currentOffset - scrollMoveOffSet);
            else
                this.Scroll_BlockSelection.ScrollToHorizontalOffset(0);
        }

        private void Btn_Navi_Right_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BlockSelection.ContentHorizontalOffset;
            if (currentOffset + scrollMoveOffSet < this.Scroll_BlockSelection.ScrollableWidth)
                this.Scroll_BlockSelection.ScrollToHorizontalOffset(currentOffset + scrollMoveOffSet);
            else
                this.Scroll_BlockSelection.ScrollToHorizontalOffset(this.Scroll_BlockSelection.ScrollableWidth);
        }

        private void Btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //if (IsSelected())
            //String block = String.Empty;
            //if (GetSelectedBlockName(ref block))
            if (!String.IsNullOrEmpty(this.SelectedBlockName))
            {
                if (this._currentMode == ViewMode.MODE_NORMAL)
                {
                    var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = this.SelectedBlockName,
                        m_cBay = String.Empty,
                        m_cRow = PresentationMgr.Singleton.MovingPosition1.m_cRow,
                        m_cTier = PresentationMgr.Singleton.MovingPosition1.m_cTier
                    };

                    PresentationMgr.Singleton.MovingPosition1 = pos;

                    var btnText = PresentationMgr.MainView.UC_BlockSelectionView1.Btn_Next.Content;
                    if (btnText.Equals("NEXT"))
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BaySelectionView1);

                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "GetBlockMapList_Ask"), this.SelectedBlockName);

                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapListMoving1_Ask(this.SelectedBlockName);
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                    else // if (btnText.Equals("DONE"))
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                    }
                }
                else if (this._currentMode == ViewMode.MODE_CORRECTION)
                {
                    PresentationMgr.Singleton.MakeCorrection(new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = this.SelectedBlockName
                    });

                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                }
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                this.Image_BlockSelectionInfo.Source = UIThemeMgr.Day.SelectionView_BlockSelectionInfoImage;
                //this.Image_BlockSelectionInfo.Source = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionInfo.png", UriKind.Relative));

                PresentationMgr.SetSkinCheckBox(this.Btn_All_Block,
                    UIThemeMgr.Day.JobList_ButtonPressImage, UIThemeMgr.Day.JobList_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinCheckBox(this.Btn_Area_Block,
                    UIThemeMgr.Day.JobList_ButtonPressImage, UIThemeMgr.Day.JobList_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinCheckBox(this.Btn_Virtual_Block,
                    UIThemeMgr.Day.JobList_ButtonPressImage, UIThemeMgr.Day.JobList_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_Left,
                    UIThemeMgr.Day.SelectionView_ButtonBlockLeftDefaultImage, UIThemeMgr.Day.SelectionView_ButtonBlockLeftPressImage, UIThemeMgr.Day.SelectionView_ButtonBlockLeftDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionView_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionView_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionView_Left_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_Right,
                    UIThemeMgr.Day.SelectionView_ButtonBlockRightDefaultImage, UIThemeMgr.Day.SelectionView_ButtonBlockRightPressImage, UIThemeMgr.Day.SelectionView_ButtonBlockRightDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionView_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionView_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BlockSelectionView_Right_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Next,
                    UIThemeMgr.Day.SelectionView_ButtonDefaultImage, UIThemeMgr.Day.SelectionView_ButtonPressImage, UIThemeMgr.Day.SelectionView_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/Button_Default.png", UriKind.Relative))
                    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                this.Image_BlockSelectionInfo.Source = UIThemeMgr.Night.SelectionView_BlockSelectionInfoImage;
                //this.Image_BlockSelectionInfo.Source = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionInfo.png", UriKind.Relative));

                PresentationMgr.SetSkinCheckBox(this.Btn_All_Block,
                    UIThemeMgr.Night.JobList_ButtonPressImage, UIThemeMgr.Night.JobList_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinCheckBox(this.Btn_Area_Block,
                    UIThemeMgr.Night.JobList_ButtonPressImage, UIThemeMgr.Night.JobList_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinCheckBox(this.Btn_Virtual_Block,
                    UIThemeMgr.Night.JobList_ButtonPressImage, UIThemeMgr.Night.JobList_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_Left,
                    UIThemeMgr.Night.SelectionView_ButtonBlockLeftDefaultImage, UIThemeMgr.Night.SelectionView_ButtonBlockLeftPressImage, UIThemeMgr.Night.SelectionView_ButtonBlockLeftDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Left_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_Right,
                    UIThemeMgr.Night.SelectionView_ButtonBlockRightDefaultImage, UIThemeMgr.Night.SelectionView_ButtonBlockRightPressImage, UIThemeMgr.Night.SelectionView_ButtonBlockRightDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Right_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Next,
                    UIThemeMgr.Night.SelectionView_ButtonDefaultImage, UIThemeMgr.Night.SelectionView_ButtonPressImage, UIThemeMgr.Night.SelectionView_ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/Button_Default.png", UriKind.Relative))
                    //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public void MakeupBlockItemsWithTabSelection()
        {
            this._currentMode = ViewMode.MODE_NORMAL;
            this.Btn_Area_Block.IsEnabled = true;
            this.Btn_All_Block.IsEnabled = true;
            this.Btn_Virtual_Block.IsEnabled = true;

            if (!String.IsNullOrEmpty(this.SelectedBlockName) && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(this.SelectedBlockName))
            {
                if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.SelectedBlockName].IsVirtual)
                {
                    this.Btn_Area_Block.IsChecked = false;
                    this.Btn_Virtual_Block.IsChecked = true;
                    this.Btn_All_Block.IsChecked = false;
                    this.Btn_Next.Content = "DONE";
                }
                else
                {
                    this.Btn_Area_Block.IsChecked = true;
                    this.Btn_Virtual_Block.IsChecked = false;
                    this.Btn_All_Block.IsChecked = false;
                    this.Btn_Next.Content = "NEXT";
                }
            }
            else
            {
                this.Btn_Area_Block.IsChecked = false;
                this.Btn_Virtual_Block.IsChecked = false;
                this.Btn_All_Block.IsChecked = true;
                this.Btn_Next.Content = "NEXT";
            }

            this.MakeupBlockItems();
        }

        public void MakeupBlockItemsForVBlock()
        {
            this._currentMode = ViewMode.MODE_CORRECTION;

            this.SelectedBlockName = String.Empty;
            this.Btn_Area_Block.IsChecked = false;
            this.Btn_Area_Block.IsEnabled = false;
            this.Btn_All_Block.IsChecked = false;
            this.Btn_All_Block.IsEnabled = false;
            this.Btn_Virtual_Block.IsChecked = true;
            this.Btn_Virtual_Block.IsEnabled = true;

            this.Btn_Next.Content = "DONE";

            this.MakeupBlockItems();
        }

        public void MakeupBlockItems()
        {
            //String selectedBlock = String.Empty;
            //GetSelectedBlockName(ref selectedBlock);                       

            this.Wrap_BlockSelectionView.Children.Clear();
            this.Scroll_BlockSelection.ScrollToHorizontalOffset(0);

            Int32 controlCount = 0;
            BlockJobControl1 selectedControl = null;
            foreach (var item in DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.Values)
            {
                if (true == Btn_Area_Block.IsChecked && item.IsVirtual)
                    continue;
                else if (true == Btn_Virtual_Block.IsChecked && !item.IsVirtual)
                    continue;

                // job matching
                var jobForBlck = PresentationMgr.Singleton.JOB_GetForBlock(item.BlcName);
                //if (true == Btn_Area_Block.IsChecked && jobForBlck.Count <= 0)
                //    continue;

                BlockJobControl1 control = null;
                if (this._blockControlItems.Count <= controlCount)
                {
                    control = new BlockJobControl1();
                    _blockControlItems.Add(control);
                }
                else
                {
                    control = _blockControlItems[controlCount];
                    control.Clear();
                }
                
                control.BlckName = item.BlcName;
                if (!String.IsNullOrEmpty(this.SelectedBlockName) && control.BlckName.Equals(this.SelectedBlockName))
                {
                    control.IsSelected = true;
                    selectedControl = control;                    
                }

                foreach (var jobOrder in jobForBlck)
                    control.AddJobOrder(jobOrder);

                this.Wrap_BlockSelectionView.Children.Add(control);

                jobForBlck.Clear();
                jobForBlck = null;
                controlCount++;
            }

            if (selectedControl != null)
            {
                selectedControl.Focus();
                this.Btn_Next.IsEnabled = true;
            }
            else
            {
                this.SelectedBlockName = "BLOCK ID";
                this.Btn_Next.IsEnabled = false;
            }
        }

        private void MakeupTestItems()
        {
            String selectedBlock = "Z";

            this.Wrap_BlockSelectionView.Children.Clear();

            Int32 controlCount = 0;
            BlockJobControl1 selectedControl = null;
            char blockName = 'A';
            for (int i = 0; i < 100; i++, controlCount++)
            {
                BlockJobControl1 control = null;
                if (this._blockControlItems.Count <= controlCount)
                {
                    control = new BlockJobControl1();
                    _blockControlItems.Add(control);
                }
                else
                {
                    control = _blockControlItems[controlCount];
                    control.Clear();
                }

                control.BlckName = ((char)blockName++).ToString();
                if (!String.IsNullOrEmpty(selectedBlock) && control.BlckName.Equals(selectedBlock))
                {
                    control.IsSelected = true;
                    selectedControl = control;
                }

                this.Wrap_BlockSelectionView.Children.Add(control);
            }            
            
            if (selectedControl != null)
                selectedControl.Focus();
        }

        // Method Function
        //private Boolean GetSelectedBlockName(ref String block)
        //{
        //    foreach (var uiElement in this.Wrap_BlockSelectionView.Children)
        //    {
        //        if (uiElement is BlockJobControl)
        //        {
        //            BlockJobControl bjc = (BlockJobControl)uiElement;
        //            if (bjc.IsSelected)
        //            {
        //                block = bjc.BlckName;
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        private Boolean IsSelected()
        {
            return true;
        }

        private void Wrap_BlockSelectionView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MakeupTestItems();
        }
    }
}