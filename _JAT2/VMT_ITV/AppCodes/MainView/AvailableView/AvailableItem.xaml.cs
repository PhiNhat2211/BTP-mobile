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
	public partial class AvailableItem : UserControl
	{
        public String _strRreasonNm { get; set; }
        public String _strReasonCd { get; set; }

		public AvailableItem()
		{
			this.InitializeComponent();
		}

        public void SetJobInfo(String strRreasonNm, String strReasonCd)
        {
            _strRreasonNm = strRreasonNm;
            _strReasonCd = strReasonCd;

            this.TextBlock_AvailableItem.Text = strRreasonNm;
        }

        private void Grid_Available_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_AvailableItem.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"main_available_btn_01_default.png");
        }

        private void Grid_Available_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_AvailableItem.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"main_available_btn_01_press.png");
        }

        private void Grid_Available_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_AvailableItem.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"main_available_btn_01_default.png");
        }
	}
}