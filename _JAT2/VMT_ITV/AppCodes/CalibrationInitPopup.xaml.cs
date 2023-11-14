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

//20190108
using Common.Interface;

namespace VMT_ITV
{
	/// <summary>
	/// Interaction logic for ChassisNumberDlg.xaml
	/// </summary>
	public partial class CalibrationInitPopup : UserControl
    {
        #region [variables]
        MainWindow mMainWindow = null;
        #endregion [variables]

        public CalibrationInitPopup()
		{
			this.InitializeComponent();
		}

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        #region [Function List]
        public void refreshView()
        {
            TextBox_CalibrationPassword.Password = "";
            mMainWindow.KeypadView.ShowKeyPad(TextBox_CalibrationPassword);
            TextBox_CalibrationPassword.Focus();

            TextBlock_popup_message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_MAINWINDOW);
            TextBlock_TwoButton_Left.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0016", LanguageService.LABEL_SETTING);
            TextBlock_TwoButton_Right.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_MAINWINDOW);
        }
        #endregion [Function List]

        #region [UI Event]
        ///////////////////////////////////////////
        //------------------UI Event
        // Ordering : Leave /  Down / Up
        #region [TextBox ChassisNumber Event]
        private void TextBox_ChassisNumber_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
            mMainWindow.KeypadView.ShowKeyPad(TextBox_CalibrationPassword);
            TextBox_CalibrationPassword.Focus();
		}

		private void TextBox_ChassisNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
		}
        #endregion [TextBox ChassisNumber Event]

        #region [Button Two_Left Event]
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
            mMainWindow.HideCalibrationInitPopup();
		}
        #endregion [Button Two_Left Event]

        #region [Button Two_Right Event]
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

            if (AppCfgMgr.Singleton.GetValueByKey("CalibrationPassword").Equals(this.TextBox_CalibrationPassword.Password))
            {
                mMainWindow.HideCalibrationInitPopup();
                mMainWindow.CalibrationInfoView.CalibrationInit();
            }
            else
            {
                this.refreshView();
            }
        }
        #endregion [Button Two_Right Event]

        #endregion [UI Event]
    }
}