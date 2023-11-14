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
    /// Interaction logic for UnlinkPopupItem.xaml
    /// </summary>
    public partial class UnlinkPopupItem : UserControl
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

        public UnlinkPopupItem()
        {
            this.InitializeComponent();
        }

        public void SetUnlinkPopupItemInfo(String strItemText)
        {
            _strItemText = strItemText;

            this.TextBlock_Unlink_Popup_Item.Text = strItemText;
        }

        private void RefreshContainerSearchControl()
        {
            if (_Selected)
                this.Grid_Unlink_Popup_Item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xA5, 0x0F));   //new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xA5, 0x0F));
            else
            {
                this.Grid_Unlink_Popup_Item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xED, 0xF1, 0xF2)); //new SolidColorBrush(Color.FromArgb(0xFF, 0xED, 0xF1, 0xF2));
            }

            if (_Selected)
                this.TextBlock_Unlink_Popup_Item.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF)); //new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            else
            {
                this.TextBlock_Unlink_Popup_Item.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x48, 0x48));   // new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x48, 0x48));
            }
        }
    }
}
