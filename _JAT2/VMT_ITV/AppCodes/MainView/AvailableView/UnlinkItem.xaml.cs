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

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for AvailableItem.xaml
    /// </summary>
    public partial class UnlinkItem : UserControl
    {
        public String _strItemText { get; set; }
        private Boolean _Selected = false;
        public Boolean Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                RefreshContainerSearchControl();
            }
        }

        public UnlinkItem()
        {
            this.InitializeComponent();
        }

        public void SetUnlinkItemInfo(String strItemText)
        {
            _strItemText = strItemText;

            this.TextBlock_UnlinkItem.Text = strItemText;
        }

        private void RefreshContainerSearchControl()
        {
            if (_Selected)
            {
                Grid_UnlinkItem.Background = new SolidColorBrush(Colors.Blue);
            }
            else
            {
                Grid_UnlinkItem.Background = new SolidColorBrush(Colors.DimGray);
            }
        }

        private void Grid_UnlinkItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Image_UnlinkItem.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"main_available_btn_01_default.png");
        }

        private void Grid_UnlinkItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Image_UnlinkItem.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"main_available_btn_01_press.png");
        }

        private void Grid_UnlinkItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Image_UnlinkItem.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"main_available_btn_01_default.png");
        }
    }
}
