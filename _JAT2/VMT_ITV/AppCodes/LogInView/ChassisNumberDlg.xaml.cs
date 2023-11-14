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

using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

//20190108
using Common.Interface;

namespace VMT_ITV
{
	/// <summary>
	/// Interaction logic for ChassisNumberDlg.xaml
	/// </summary>
	public partial class ChassisNumberDlg : UserControl
	{
        MainWindow mMainWindow = null;

		public ChassisNumberDlg()
		{
			this.InitializeComponent();
		}

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        public void refreshView()
        {
            TextBox_ChassisNumber.Text = "";
            mMainWindow.KeypadView.ShowKeyPad(TextBox_ChassisNumber);
            TextBox_ChassisNumber.Focus();

            TextBlock_popup_message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_MAINWINDOW);
            TextBlock_TwoButton_Right.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_MAINWINDOW);
        }

		private void TextBox_ChassisNumber_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{	
            mMainWindow.KeypadView.ShowKeyPad(TextBox_ChassisNumber);
            TextBox_ChassisNumber.Focus();
		}

		private void TextBox_ChassisNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			
		}

		private void Grid_TwoButton_Left_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{	
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
		}

		private void Grid_TwoButton_Left_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{	
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_press.png");
		}

		private void Grid_TwoButton_Left_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{	
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
            mMainWindow.HideChassisNumberDlg();
		}

		private void Grid_TwoButton_Right_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{	
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
		}

		private void Grid_TwoButton_Right_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{	
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_press.png");
		}

		private void Grid_TwoButton_Right_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{	
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
            
            ITV.VD_ITV_ChassisAttachInfo_Send chassisAttachInfo = new ITV.VD_ITV_ChassisAttachInfo_Send();
            chassisAttachInfo.nType = ITV.VD_ITV_ChassisAttachInfo_Send.ChassisType.Foot40;
            chassisAttachInfo.m_ChassisNumber = TextBox_ChassisNumber.Text;
            VMT_DataMgr_ITV.ChassisInfo_Ask(ref chassisAttachInfo);
            
            VMT_DataMgr.MainView_Init(MainWindow.SERVICE_COMPANY);
            mMainWindow.gotoMainView();
		}
	}
}