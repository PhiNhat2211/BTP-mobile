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
using System.ComponentModel;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for NavigatorView.xaml
    /// </summary>
    public partial class NavigatorView : UserControl
    {
        private Rectangle _rectViewPoint = new Rectangle();
        private List<ColumnDefinition> _columnDefs = new List<ColumnDefinition>();
        private List<RowDefinition> _rowDefs = new List<RowDefinition>();
        private List<NavigatorItem> _navigatorItems = new List<NavigatorItem>();
        private static Int32 _maxRowDefCount = 20;
        private static Int32 _maxColumnDefCount = 50;

        public NavigatorView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
                new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);

            InitRowAndColumnDefine();

            this.IsWaitingToHide = false;          
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(NavigatorView_IsVisibleChanged);

            // Init Event Handler
            this.Btn_Left.Click += new RoutedEventHandler(Btn_Left_Click);
            this.Btn_Right.Click += new RoutedEventHandler(Btn_Right_Click);
            this.Btn_Up.Click += new RoutedEventHandler(Btn_Up_Click);
            this.Btn_Down.Click += new RoutedEventHandler(Btn_Down_Click);

            this.ClearNaviItems();

            _rectViewPoint.Fill = new SolidColorBrush(Color.FromArgb(00, 255, 00, 00));
            _rectViewPoint.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 00, 00));
            _rectViewPoint.StrokeThickness = 2;
            _ViewMaxRow = 7;
        }

        public bool NeedReset = true;
        public void RMG_Member_PropertyChanged_TargetJobKey(object sender, PropertyChangedEventArgs e)
        {
            //if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            //    this.NeedReset = true;
        }        

        public Boolean IsViewPointChanged
        {
            get
            {
                return (_preViewPointRow != _ViewPointRow) || (_preViewPointTier != _ViewPointTier);
            }
        }

        private Int32 _preViewPointRow = 0;
        private Int32 _preViewPointTier = 0;
        
        private Int32 _CurrentBayRow = 10;
        private Int32 _CurrentBayTier = 10;

        private Int32 _ViewMaxRow = 7;
        private Int32 _ViewMaxTier = 7;

        private Int32 _ViewPointRow = 0;
        private Int32 _ViewPointTier = 0;

        public Int32 CurrentBayRow
        {
            get { return _CurrentBayRow; }
            set { _CurrentBayRow = value; }
        }

        public Int32 CurrentBayTier
        {
            get { return _CurrentBayTier; }
            set { _CurrentBayTier = value; }
        }

        public Int32 ViewMaxRow
        {
            get { return _ViewMaxRow; }
            set { _ViewMaxRow = value; }
        }

        public Int32 ViewMaxTier
        {
            get { return _ViewMaxTier; }
            set { _ViewMaxTier = value; }
        }

        public Int32 ViewPointRow
        {
            get
            {
                return _ViewPointRow;
            }
            set
            {
                if (value < 0)
                    _ViewPointRow = 0;
                else if (value > _CurrentBayRow - _ViewMaxRow)
                    _ViewPointRow = _CurrentBayRow - _ViewMaxRow;
                else
                    _ViewPointRow = value;
            }
        }

        public Int32 ViewPointTier
        {
            get
            {
                return _ViewPointTier;
            }
            set
            {
                if (value < 0)
                    _ViewPointTier = 0;
                else if (value > _CurrentBayTier - ViewMaxTier)
                    _ViewPointTier = _CurrentBayTier - ViewMaxTier;
                else
                    _ViewPointTier = value;
            }
        }

        public Boolean IsWaitingToHide { get; set; }
        
        private void InitRowAndColumnDefine()
        {
            _columnDefs.Clear();
            for (int i = 0; i < _maxColumnDefCount; i++)
                _columnDefs.Add(new ColumnDefinition());

            _rowDefs.Clear();
            for (int i = 0; i < _maxRowDefCount; i++)
                _rowDefs.Add(new RowDefinition());
        }
        public void ChangeMaxColDefCount(String maxRow)
        {
            _maxColumnDefCount = Int32.TryParse(maxRow, out int parseIntVal) && parseIntVal >= 10 ? parseIntVal : 50;
            InitRowAndColumnDefine();
        }
        private void NavigatorView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            _preViewPointRow = _ViewPointRow;
            _preViewPointTier = _ViewPointTier;
        }

        public void ClearNaviItems()
        {
            for (int i = 0; i < this.Gird_BayNavigatorItem.Children.Count; i++)
            {
                var item = this.Gird_BayNavigatorItem.Children[i] as NavigatorItem;
                item = null;
            }            
            this.Gird_BayNavigatorItem.Children.Clear();
            
            this.Gird_BayNavigatorItem.ColumnDefinitions.Clear();           
            this.Gird_BayNavigatorItem.RowDefinitions.Clear();
        }

        public void SetViewPortItems(Int32 maxRow, Int32 maxTier, 
            Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> dic_items,
            SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB,
            VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location targetLoc = null)
        {
            this.Gird_BayNavigatorItem.Children.Clear();
            this.Gird_BayNavigatorItem.ColumnDefinitions.Clear();
            this.Gird_BayNavigatorItem.RowDefinitions.Clear();

            this._CurrentBayRow = _ViewMaxRow < maxRow ? maxRow : _ViewMaxRow;
            this._CurrentBayTier = _ViewMaxTier < maxTier ? maxTier : _ViewMaxTier;

            var diff = Math.Max(_ViewMaxRow < maxRow ? maxRow : _ViewMaxRow, _ViewMaxTier < maxTier ? maxTier : _ViewMaxTier);

            for (int i = 0; i < diff; i++)
            {
                if (i < _maxColumnDefCount)
                    this.Gird_BayNavigatorItem.ColumnDefinitions.Add(_columnDefs[i]);
                else
                {
                    _CurrentBayRow = _maxColumnDefCount;
                    break;
                }
                //ColumnDefinition cDefinition = new ColumnDefinition();                
                //this.Gird_BayNavigatorItem.ColumnDefinitions.Add(cDefinition);
            }

            for (int j = 0; j < diff; j++)
            {
                if (j < _maxRowDefCount)
                    this.Gird_BayNavigatorItem.RowDefinitions.Add(_rowDefs[j]);
                else
                {
                    _CurrentBayTier = _maxRowDefCount;
                    break;
                }
                //RowDefinition rDefinition = new RowDefinition();                
                //this.Gird_BayNavigatorItem.RowDefinitions.Add(rDefinition);
            }

            Int32 naviItemIdx = 0;
            for (int i = this._CurrentBayTier - 1; i >= 0; i--)
            {
                for (int j = 0; j < this._CurrentBayRow; j++, naviItemIdx++)
                {
                    NavigatorItem nItem = null;
                    if (naviItemIdx >= this._navigatorItems.Count)
                    {
                        nItem = new NavigatorItem();
                        this._navigatorItems.Add(nItem);
                    }
                    else
                    {
                        nItem = this._navigatorItems[naviItemIdx];
                        nItem.SetInfo(false);
                        nItem.SetSelection(false);
                    }
                    
                    if (dic_items.ContainsKey(this._CurrentBayTier - i) && dic_items[this._CurrentBayTier - i].ContainsKey(j))
                    {
                        if (PresentationMgr.Singleton.CurrentBlock.Equals(dic_items[this._CurrentBayTier - i][j].loc.blck) &&
                            PresentationMgr.IsBayEqual(dic_items[this._CurrentBayTier - i][j].loc.bay, PresentationMgr.Singleton.CurrentBay))
                            nItem.SetInfo(true);
                    }
                    else if (j >= maxRow || (this._CurrentBayTier - i) > maxTier)
                        nItem.Btn_NavigatorItem.Visibility = Visibility.Hidden;

                    if (targetLoc != null)
                    {
                        var targetRowInt = PresentationMgr.ConvertRowToNumber(rowMap, targetLoc.row, direction);
                        var targetTierInt = Convert.ToInt32(String.IsNullOrEmpty(targetLoc.tier) ? "0" : targetLoc.tier);
                        if (targetLoc != null &&
                            targetLoc.blck.Equals(PresentationMgr.Singleton.CurrentBlock) &&
                            PresentationMgr.IsBayEqual(targetLoc.bay, PresentationMgr.Singleton.CurrentBay) &&
                            targetRowInt == j && targetTierInt == this._CurrentBayTier - i)
                        {
                            nItem.SetSelection(true);

                            if (NeedReset)
                            {
                                this.NeedReset = false;
                                this._ViewPointRow = 0;
                                this._ViewPointTier = this._CurrentBayTier > this._ViewMaxTier ? this._CurrentBayTier - this._ViewMaxTier : 0;
                                if (targetRowInt >= this._ViewMaxRow)
                                    this._ViewPointRow = Math.Min(targetRowInt, this._CurrentBayRow - this._ViewMaxRow);
                                else if (targetTierInt > _ViewMaxTier)
                                    this._ViewPointTier = 0;
                            }
                        }
                    }

                    this.Gird_BayNavigatorItem.Children.Add(nItem);
                    Grid.SetColumn(nItem, j);
                    Grid.SetRow(nItem, i);
                }
            }

            if (this.NeedReset)
            {
                this.NeedReset = false;
                this._ViewPointRow = 0;
                this._ViewPointTier = this._CurrentBayTier > this._ViewMaxTier? this._CurrentBayTier - this._ViewMaxTier : 0;
            }

            //Rectangle RectViewPoint = new Rectangle();
            //RectViewPoint.Fill = new SolidColorBrush(Color.FromArgb(00, 255, 00, 00));
            //RectViewPoint.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 00, 00));
            //RectViewPoint.StrokeThickness = 2;
            this.Gird_BayNavigatorItem.Children.Add(_rectViewPoint);
            
            Grid.SetRow(_rectViewPoint, ViewPointTier);
            Grid.SetColumn(_rectViewPoint, ViewPointRow);
            Grid.SetRowSpan(_rectViewPoint, ViewMaxTier);
            Grid.SetColumnSpan(_rectViewPoint, ViewMaxRow);

            //this._RectViewPoint = RectViewPoint;
        }
        
        public void Btn_Left_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            ViewPointRow = ViewPointRow - ViewMaxRow;

            if (_rectViewPoint != null)
                Grid.SetColumn(_rectViewPoint, ViewPointRow);
        }

        public void Btn_Right_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            ViewPointRow = ViewPointRow + ViewMaxRow;

            if (_rectViewPoint != null)
                Grid.SetColumn(_rectViewPoint, ViewPointRow);
        }

        private void Btn_Up_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            ViewPointTier--;

            if (_rectViewPoint != null)
                Grid.SetRow(_rectViewPoint, ViewPointTier);
        }

        private void Btn_Down_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            ViewPointTier++;

            if (_rectViewPoint != null)
                Grid.SetRow(_rectViewPoint, ViewPointTier);
        }

        public bool ChangePageBayView(String row)
        {
            try
            {
                if (PresentationMgr.Singleton.rowMap == null)
                    return false;
                var targetRowInt = PresentationMgr.ConvertRowToNumber(PresentationMgr.Singleton.rowMap, row, PresentationMgr.Singleton.direction);
                int targetPage = 0;
                if (targetRowInt >= this._ViewMaxRow)
                    targetPage = Math.Min(targetRowInt, this._CurrentBayRow - this._ViewMaxRow);

                if (ViewPointRow == targetPage)
                    return false;
                else
                {
                    ViewPointRow = targetPage;
                    if (_rectViewPoint != null)
                        Grid.SetColumn(_rectViewPoint, ViewPointRow);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Left,
                    UIThemeMgr.Day.NavigatorView_ButtonLeftDefaultImage, UIThemeMgr.Day.NavigatorView_ButtonLeftPressImage, UIThemeMgr.Day.NavigatorView_ButtonLeftDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Right,
                    UIThemeMgr.Day.NavigatorView_ButtonRightDefaultImage, UIThemeMgr.Day.NavigatorView_ButtonRightPressImage, UIThemeMgr.Day.NavigatorView_ButtonRightDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Up,
                    UIThemeMgr.Day.NavigatorView_ButtonUpDefaultImage, UIThemeMgr.Day.NavigatorView_ButtonUpPressImage, UIThemeMgr.Day.NavigatorView_ButtonUpDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Up_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Down,
                    UIThemeMgr.Day.NavigatorView_ButtonDownDefaultImage, UIThemeMgr.Day.NavigatorView_ButtonDownPressImage, UIThemeMgr.Day.NavigatorView_ButtonDownDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/NavigatorView/NavigatorView_Down_Default.png", UriKind.Relative))
                    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Left,
                    UIThemeMgr.Night.NavigatorView_ButtonLeftDefaultImage, UIThemeMgr.Night.NavigatorView_ButtonLeftPressImage, UIThemeMgr.Night.NavigatorView_ButtonLeftDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Right,
                    UIThemeMgr.Night.NavigatorView_ButtonRightDefaultImage, UIThemeMgr.Night.NavigatorView_ButtonRightPressImage, UIThemeMgr.Night.NavigatorView_ButtonRightDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Up,
                    UIThemeMgr.Night.NavigatorView_ButtonUpDefaultImage, UIThemeMgr.Night.NavigatorView_ButtonUpPressImage, UIThemeMgr.Night.NavigatorView_ButtonUpDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Up_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Down,
                    UIThemeMgr.Night.NavigatorView_ButtonDownDefaultImage, UIThemeMgr.Night.NavigatorView_ButtonDownPressImage, UIThemeMgr.Night.NavigatorView_ButtonDownDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Down_Default.png", UriKind.Relative))
                    //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}
