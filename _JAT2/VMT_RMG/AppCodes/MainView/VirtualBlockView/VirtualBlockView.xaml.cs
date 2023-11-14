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
    /// VirtualBlockView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VirtualBlockView : UserControl
    {
        private List<VirtualBlockItem> _blockItems = null;
        private String _selectedCntrNo = String.Empty;

        public VirtualBlockView()
        {
            InitializeComponent();

            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);
            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void VirtualBlockView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {            
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;  

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Day.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Day.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Day.ContainerSearchView_ButtonDownDisableImage);
                
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                this.Image_BG_Copy.Source = UIThemeMgr.Day.ContainerSearchView_TitleImage;

                PresentationMgr.SetSkinButton(this.Btn_PageUp,
                    UIThemeMgr.Night.ContainerSearchView_ButtonUpDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_PageDown,
                    UIThemeMgr.Night.ContainerSearchView_ButtonDownDefaultImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownPressImage, UIThemeMgr.Night.ContainerSearchView_ButtonDownDisableImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_PageUp.Click += new RoutedEventHandler(Btn_PageUp_Click);
            this.Btn_PageDown.Click += new RoutedEventHandler(Btn_PageDown_Click);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.VirtualBlockView_IsVisibleChanged);
            this.TextBlock_ResultCount.Text = String.Empty;
            this.ListBox_BlockItem.Children.Clear();

            this.ListBox_BlockItem.MouseLeftButtonUp += new MouseButtonEventHandler(ListBox_MouseLeftButtonUp);

            this._blockItems = new List<VirtualBlockItem>();

            TextBlock_Result.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0117", LanguageService.LABEL_CUSTOMIZE);
            Text_Container.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0065", LanguageService.LABEL_CONTAINERDETAIL);
            Text_ISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0041", LanguageService.LABEL_CONTAINERDETAIL);
        }

        public void SetBlockItems(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> invenList)
        {
            this.ListBox_BlockItem.Children.Clear();

            Int32 controlCount = 0;
            foreach (var inven in invenList)
            {
                VirtualBlockItem control = null;
                if (this._blockItems.Count <= controlCount)
                {
                    control = new VirtualBlockItem();
                    _blockItems.Add(control);
                }
                else
                    control = _blockItems[controlCount];

                control.SetCntrInfo(inven.cntr);
                if (this._selectedCntrNo == control.CntrNo)
                    control.Selected = true;

                this.ListBox_BlockItem.Children.Add(control);

                controlCount++;
            }

            this.TextBlock_ResultCount.Text = Convert.ToString(this.ListBox_BlockItem.Children.Count);
        }

        private void Scroll_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
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

        private double scrollMoveOffSet = 67;
        private void Btn_PageUp_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BlockItem.ContentVerticalOffset;

            if (currentOffset - scrollMoveOffSet > 0)
                this.Scroll_BlockItem.ScrollToVerticalOffset(currentOffset - scrollMoveOffSet);
            else
                this.Scroll_BlockItem.ScrollToVerticalOffset(0);
        }

        private void Btn_PageDown_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_BlockItem.ContentVerticalOffset;
            if (currentOffset + scrollMoveOffSet < this.Scroll_BlockItem.ScrollableHeight)
                this.Scroll_BlockItem.ScrollToVerticalOffset(currentOffset + scrollMoveOffSet);
            else
                this.Scroll_BlockItem.ScrollToVerticalOffset(this.Scroll_BlockItem.ScrollableHeight);
        }      

        private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is VirtualBlockItem))
                return;

            foreach (var cSearchControl in ListBox_BlockItem.Children)
            {
                if (cSearchControl is VirtualBlockItem)
                {
                    (cSearchControl as VirtualBlockItem).Selected = false;
                }
            }

            var sSearchControl = e.Source as VirtualBlockItem;
            sSearchControl.Selected = true;            

            _selectedCntrNo = sSearchControl.CntrNo;
        }
    }
}
