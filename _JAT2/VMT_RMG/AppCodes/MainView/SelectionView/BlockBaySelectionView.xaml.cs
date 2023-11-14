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
    /// Interaction logic for JobView.xaml
    /// </summary>

    public partial class BlockBaySelectionView : UserControl
    {
        private enum ViewMode
        {
            MODE_NORMAL = 0,
            MODE_CORRECTION,
        }
        public PresentationMgr.UIMode PrevUIMode = PresentationMgr.UIMode.MainView;
        private ViewMode _currentMode = ViewMode.MODE_NORMAL;

        private List<BlockJobControl> _blockControlItems = null;
        private String _selectedBlockName = String.Empty;
        public String SelectedBlockName 
        {
            get { return this._selectedBlockName; }
            set { this.TextBox_BlockID.Text = _selectedBlockName = value;}            
        }

        private List<BayControl> _bayControlItems = null;
        private bool blockAreaFilter = true;

        private String _selectedBayName = String.Empty;
        public String SelectedBayName
        {
            get { return this._selectedBayName; }
            set { this._selectedBayName = value; 
            }
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _jobOrder;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder JobOrder
        {
            get { return _jobOrder; }
            set { _jobOrder = value; }
            // set { _jobOrder = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(value); }
        }

        public BlockBaySelectionView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
            
            _blockControlItems = new List<BlockJobControl>();
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

            this.Btn_Navi_LeftB.Click += new RoutedEventHandler(Btn_Navi_LeftB_Click);
            this.Btn_Navi_RightB.Click += new RoutedEventHandler(Btn_Navi_RightB_Click);
            this.Wrap_BaySelectionView.MouseLeftButtonUp += new MouseButtonEventHandler(Wrap_BaySelectionView_MouseLeftButtonUp);

            this.SelectedBayName = String.Empty;
            _bayControlItems = new List<BayControl>();

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(BaySelectionView_IsVisibleChanged);

            this.btn_Block.MouseLeftButtonDown += new MouseButtonEventHandler(Btn_Block_Click);
            this.btn_Area.MouseLeftButtonDown += new MouseButtonEventHandler(Btn_Area_Click);

            LoadLanguage();
        }
        private void LoadLanguage()
        {
            TextBox_BlockID.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0111", LanguageService.LABEL_CUSTOMIZE);
            Lbl_MachineList.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0224", LanguageService.LABEL_CUSTOMIZE);
            Btn_All_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0115", LanguageService.LABEL_CUSTOMIZE);
            Btn_Area_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0113", LanguageService.LABEL_CUSTOMIZE);
            Btn_Virtual_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0114", LanguageService.LABEL_CUSTOMIZE);

            this.Lb_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0077", LanguageService.LABEL_MAINWINDOW);
            this.Lb_Machine.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0068", LanguageService.LABEL_CONTAINERDETAIL);
            this.Lb_UsrId.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0058", LanguageService.LABEL_MAINWINDOW);

            this.Lb_GI.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("GI", LanguageService.LABEL_JOBTYPE);
            this.Lb_GO.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("GO", LanguageService.LABEL_JOBTYPE);
            this.Lb_MI.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MI", LanguageService.LABEL_JOBTYPE);
            this.Lb_MO.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MO", LanguageService.LABEL_JOBTYPE);
            this.Lb_DS.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("DS", LanguageService.LABEL_JOBTYPE);
            this.Lb_LD.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LD", LanguageService.LABEL_JOBTYPE);
            this.Lb_ETC.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("ETC", LanguageService.LABEL_JOBTYPE);
            this.Lb_SUM.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("SUM", LanguageService.LABEL_JOBTYPE);

            btn_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0232", LanguageService.LABEL_CUSTOMIZE);
            btn_Area.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0233", LanguageService.LABEL_CUSTOMIZE);
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
                        m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                        m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                    };
                    
                    PresentationMgr.Singleton.CurrentPostion = pos;

                    var btnText = PresentationMgr.MainView.UC_BlockSelectionView.Btn_Next.Content;
                    if (btnText.Equals(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0116", LanguageService.LABEL_CUSTOMIZE)))
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BaySelectionView);

                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "GetBlockMapList_Ask"), this.SelectedBlockName);

                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(this.SelectedBlockName);
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                    else // if (btnText.Equals("DONE"))
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                        PresentationMgr.Singleton.CurrentPostion.m_cBlock = String.Empty;
                        var posVirtual = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                        {
                            m_cBlock = this.SelectedBlockName,
                            m_cBay = String.Empty,
                            m_cRow = String.Empty,
                            m_cTier = String.Empty
                        };
                        PresentationMgr.Singleton.CurrentPostion = posVirtual;
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
                //this.Image_BlockSelectionInfo.Source = UIThemeMgr.Day.SelectionView_BlockSelectionInfoImage;
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

                PresentationMgr.SetSkinButton(this.Btn_Navi_LeftB,
                        UIThemeMgr.Day.SelectionView_ButtonBayLeftDefaultImage, UIThemeMgr.Day.SelectionView_ButtonBayLeftPressImage, UIThemeMgr.Day.SelectionView_ButtonBayLeftDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_RightB,
                    UIThemeMgr.Day.SelectionView_ButtonBayRightDefaultImage, UIThemeMgr.Day.SelectionView_ButtonBayRightPressImage, UIThemeMgr.Day.SelectionView_ButtonBayRightDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Disable.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                //this.Image_BlockSelectionInfo.Source = UIThemeMgr.Night.SelectionView_BlockSelectionInfoImage;
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


                PresentationMgr.SetSkinButton(this.Btn_Navi_LeftB,
                    UIThemeMgr.Night.SelectionView_ButtonBayLeftDefaultImage, UIThemeMgr.Night.SelectionView_ButtonBayLeftPressImage, UIThemeMgr.Night.SelectionView_ButtonBayLeftDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Navi_RightB,
                    UIThemeMgr.Night.SelectionView_ButtonBayRightDefaultImage, UIThemeMgr.Night.SelectionView_ButtonBayRightPressImage, UIThemeMgr.Night.SelectionView_ButtonBayRightDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Disable.png", UriKind.Relative))
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
                    Btn_Area_Click(null, null);
                    this.Btn_Area_Block.IsChecked = false;
                    this.Btn_Virtual_Block.IsChecked = true;
                    this.Btn_All_Block.IsChecked = false;
                    this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0110", LanguageService.LABEL_CUSTOMIZE);
                }
                else
                {
                    Btn_Block_Click(null, null);
                    this.Btn_Area_Block.IsChecked = true;
                    this.Btn_Virtual_Block.IsChecked = false;
                    this.Btn_All_Block.IsChecked = false;
                    this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0116", LanguageService.LABEL_CUSTOMIZE);
                }
            }
            else
            {
                this.Btn_Area_Block.IsChecked = false;
                this.Btn_Virtual_Block.IsChecked = false;
                this.Btn_All_Block.IsChecked = true;
                this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0116", LanguageService.LABEL_CUSTOMIZE);
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

            this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0110", LanguageService.LABEL_CUSTOMIZE);

            this.MakeupBlockItems();
        }

        public void MakeupBlockItems()
        {
            //String selectedBlock = String.Empty;
            //GetSelectedBlockName(ref selectedBlock);                       

            this.Wrap_BlockSelectionView.Children.Clear();
            this.Scroll_BlockSelection.ScrollToHorizontalOffset(0);

            Int32 controlCount = 0;
            BlockJobControl selectedControl = null;
            foreach (var item in DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.Values)
            {
                //if (true == Btn_Area_Block.IsChecked && item.IsVirtual)
                //    continue;
                //else if (true == Btn_Virtual_Block.IsChecked && !item.IsVirtual)
                //    continue;
                //20200921 add block/area filter button
                /*if (item.IsVirtual)
                    continue;*/

                if (blockAreaFilter && item.IsVirtual)
                    continue;
                else if (!blockAreaFilter && !item.IsVirtual)
                    continue;

                // job matching
                var jobForBlck = PresentationMgr.Singleton.JOB_GetForBlock(item.BlcName);
                //if (true == Btn_Area_Block.IsChecked && jobForBlck.Count <= 0)
                //    continue;

                BlockJobControl control = null;
                if (this._blockControlItems.Count <= controlCount)
                {
                    control = new BlockJobControl();
                    _blockControlItems.Add(control);
                }
                else
                {
                    control = _blockControlItems[controlCount];
                    control.Clear();
                }

                control.BlckName = item.BlcName;
                control.UserCode = item.userCode;
                control.LoginUser = item.loginUser;
                control.GoCont = item.goCont;
                control.GiCont = item.giCont;
                control.MoCont = item.moCont;
                control.MiCont = item.miCont;
                control.DsCont = item.dsCont;
                control.LdCont = item.ldCont;
                control.EtcCont = item.etcCont;
                control.TextBlock_Total.Text = item.totalCont;

                control.IsVirtual = item.IsVirtual;
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
                this.Tbl_MachineList.Text = selectedControl.UserCode;
                this.Btn_Next.IsEnabled = true;
            }
            else
            {
                this.SelectedBlockName = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0111", LanguageService.LABEL_CUSTOMIZE);
                this.Tbl_MachineList.Text = String.Empty;
                this.Btn_Next.IsEnabled = false;
            }
        }

        private void MakeupTestItems()
        {
            String selectedBlock = "Z";

            this.Wrap_BlockSelectionView.Children.Clear();

            Int32 controlCount = 0;
            BlockJobControl selectedControl = null;
            char blockName = 'A';
            for (int i = 0; i < 100; i++, controlCount++)
            {
                BlockJobControl control = null;
                if (this._blockControlItems.Count <= controlCount)
                {
                    control = new BlockJobControl();
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

        public void Wrap_BlockSelectionView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MakeupTestItems();
            if (!String.IsNullOrEmpty(this.SelectedBlockName))
            {
                if (this._currentMode == ViewMode.MODE_NORMAL)
                {
                    BlockJobControl blck = _blockControlItems.Find(x => this.SelectedBlockName.Equals(x.BlckName));
                    if (blck != null && blck.IsVirtual)
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                        PresentationMgr.Singleton.CurrentPostion.m_cBlock = String.Empty;

                        var posVirtual = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                        {
                            m_cBlock = this.SelectedBlockName,
                            m_cBay = String.Empty,
                            m_cRow = String.Empty,
                            m_cTier = String.Empty
                        };
                        PresentationMgr.Singleton.CurrentPostion = posVirtual;
                        PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                        if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Visible;

                        ReloadJobListInBLOCKFilter();
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                    else
                    {
                        var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                        {
                            m_cBlock = this.SelectedBlockName,
                            m_cBay = String.Empty,
                            m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                            m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                        };

                        PresentationMgr.Singleton.CurrentPostion = pos;
                        if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                        PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;

                        //PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BaySelectionView);

                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "GetBlockMapList_Ask"), this.SelectedBlockName);

                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(this.SelectedBlockName);
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                }
                else if (this._currentMode == ViewMode.MODE_CORRECTION)
                {
                    PresentationMgr.Singleton.MakeCorrection(new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = this.SelectedBlockName
                    });

                    //PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                }
            }
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
                this.Btn_Navi_LeftB.IsEnabled = false;
            }
            else
            {
                this.Btn_Navi_LeftB.IsEnabled = true;
            }

            if (scroller.ScrollableWidth == scroller.ContentHorizontalOffset)
            {
                this.Btn_Navi_RightB.IsEnabled = false;
            }
            else
            {
                this.Btn_Navi_RightB.IsEnabled = true;
            }
        }

        private double scrollMoveOffSetB = 143 * 6; // BlockBayJobControl DesignWidth * 9
        void Btn_Navi_LeftB_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BaySelection.ContentHorizontalOffset;

            if (currentOffset - scrollMoveOffSetB > 0)
                this.Scroll_BaySelection.ScrollToHorizontalOffset(currentOffset - scrollMoveOffSetB);
            else
                this.Scroll_BaySelection.ScrollToHorizontalOffset(0);
        }

        void Btn_Navi_RightB_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BaySelection.ContentHorizontalOffset;
            if (currentOffset + scrollMoveOffSetB < this.Scroll_BaySelection.ScrollableWidth)
                this.Scroll_BaySelection.ScrollToHorizontalOffset(currentOffset + scrollMoveOffSetB);
            else
                this.Scroll_BaySelection.ScrollToHorizontalOffset(this.Scroll_BaySelection.ScrollableWidth);
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
            BayControl selectedControl = null;
            foreach (var item in bayList)
            {
                BayControl control = null;
                if (this._bayControlItems.Count <= controlCount)
                {
                    control = new BayControl();
                    this._bayControlItems.Add(control);
                }
                else
                {
                    control = this._bayControlItems[controlCount];
                    control.Clear();
                }

                control.BayName = item.BayName;
                control.CategoryName = item.categoryName;
                control.Opr = item.opr;
                control.CntrLenTp = item.cntrLen + item.cntrTp;
                control.InspCode = item.inspCode;
                if (!String.IsNullOrEmpty(this.SelectedBayName) && control.BayName.Equals(this.SelectedBayName))
                {
                    control.IsSelected = true;
                    selectedControl = control;
                }

                // job matching
                var jobForBlck = PresentationMgr.Singleton.JOB_GetForBlockBay(PresentationMgr.Singleton.CurrentBlock, item.BayName);
                var intBay = Convert.ToInt32(PresentationMgr.BayRemoveChars(item.BayName));
                if (intBay % 2 == 1)
                {
                    var evenBay = Convert.ToString(intBay + 1);
                    if (evenBay.Length == 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(item.BayName);
                        if (String.IsNullOrEmpty(cha))
                            evenBay = "0" + evenBay;
                        else
                            evenBay = cha + evenBay;
                    }
                    jobForBlck.AddRange(PresentationMgr.Singleton.JOB_GetForBlockBay(PresentationMgr.Singleton.CurrentBlock, evenBay));
                }

                jobForBlck.Clear();
                jobForBlck = null;

                this.Wrap_BaySelectionView.Children.Add(control);

                controlCount++;
            }

            if (selectedControl != null)
            {
                selectedControl.Focus();
                //this.Btn_Done_Two.IsEnabled = true;
            }
            else
            {
                this.SelectedBayName = String.Empty;
                //this.Btn_Done_Two.IsEnabled = false;
            }
        }

        //public void MakeupBayItems()
        //{
        //    this.Wrap_BaySelectionView.Children.Clear();

        //    if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock))
        //    {
        //        PresentationMgr.AppWin.ShowProgressBar(0);

        //        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
        //        , "GetBlockMapList_Ask"), PresentationMgr.Singleton.CurrentBlock);

        //        if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
        //            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(PresentationMgr.Singleton.CurrentBlock);
        //        return;
        //    }

        //    if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(PresentationMgr.Singleton.CurrentBlock) ||
        //        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay == null)
        //        return;

        //    var bayList = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay.Values.ToList();
        //    bayList.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);

        //    Int32 controlCount = 0;
        //    BlockBayJobControl selectedControl = null;
        //    foreach (var item in bayList)
        //    {
        //        BlockBayJobControl control = null;
        //        if (this._bayControlItems.Count <= controlCount)
        //        {
        //            control = new BlockBayJobControl();
        //            this._bayControlItems.Add(control);
        //        }
        //        else
        //        {
        //            control = this._bayControlItems[controlCount];
        //            control.Clear();
        //        }

        //        control.BayName = item.BayName;
        //        if (!String.IsNullOrEmpty(this.SelectedBayName) && control.BayName.Equals(this.SelectedBayName))
        //        {
        //            control.IsSelected = true;
        //            selectedControl = control;
        //        }

        //        // job matching
        //        var jobForBlck = PresentationMgr.Singleton.JOB_GetForBlockBay(PresentationMgr.Singleton.CurrentBlock, item.BayName);
        //        var intBay = Convert.ToInt32(item.BayName);
        //        if (intBay % 2 == 1)
        //        {
        //            var evenBay = Convert.ToString(intBay + 1);
        //            if (evenBay.Length == 1)
        //                evenBay = "0" + evenBay;
        //            jobForBlck.AddRange(PresentationMgr.Singleton.JOB_GetForBlockBay(PresentationMgr.Singleton.CurrentBlock, evenBay));
        //        }
        //        foreach (var jobOrder in jobForBlck)
        //        {
        //            control.AddJobOrder(jobOrder);
        //        }
        //        jobForBlck.Clear();
        //        jobForBlck = null;

        //        this.Wrap_BaySelectionView.Children.Add(control);

        //        controlCount++;
        //    }

        //    if (selectedControl != null)
        //    {
        //        selectedControl.Focus();
        //        //this.Btn_Done_Two.IsEnabled = true;
        //    }
        //    else
        //    {
        //        this.SelectedBayName = String.Empty;
        //        //this.Btn_Done_Two.IsEnabled = false;
        //    }
        //}

        void Wrap_BaySelectionView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.SelectedBayName))
            {
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);

                //RELOAD JOBLIST WITH NEW BLOCK/BAY IF BLOCK MODE IS SELECTED
                if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.Common.BlckVal))
                {
                    PresentationMgr.Singleton.NeedJobAutoSelection = true;
                    PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal = PresentationMgr.Singleton.CurrentPostion.m_cBlock;

                    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                    //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                }

                PresentationMgr.Singleton.CurrentPostion.m_cBay = String.Empty;
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = this.SelectedBayName,
                    m_cRow = String.Empty,
                    m_cTier = String.Empty
                };

                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        public void ReloadJobListInBLOCKFilter()
        {
            //RELOAD JOBLIST WITH NEW BLOCK/BAY IF BLOCK MODE IS SELECTED
            //if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.Common.BlckVal))
            //{
            PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal = PresentationMgr.Singleton.CurrentPostion.m_cBlock;

            VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
            VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            //}
        }

        private void Btn_Block_Click(object sender, MouseButtonEventArgs e)
        {           
            if (!blockAreaFilter)
            {
                blockAreaFilter = true;
                btn_Block.Background = Brushes.Orange;
                btn_Area.Background = Brushes.Gray;
                this.MakeupBlockItems();
            }
        }
        private void Btn_Area_Click(object sender, MouseButtonEventArgs e)
        {
            if (blockAreaFilter)
            {
                blockAreaFilter = false;
                btn_Block.Background = Brushes.Gray;
                btn_Area.Background = Brushes.Orange;
                this.Wrap_BaySelectionView.Children.Clear();
                this.MakeupBlockItems();
            }
        }
    }
}