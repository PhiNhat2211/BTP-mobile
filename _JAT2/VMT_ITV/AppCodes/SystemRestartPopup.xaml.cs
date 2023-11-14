using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

//20190108
using Common.Interface;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for SystemRestartPopup.xaml
    /// </summary>
    public partial class SystemRestartPopup : UserControl
    {
        #region [variables]
        private MainWindow mMainWindow;

        public delegate void Callback_Popup(int seleted);

        static private Callback_Popup callback_popup;

        private DispatcherTimer TimerStop = null;
        #endregion [variables]

        public SystemRestartPopup()
        {
            this.InitializeComponent();
        }

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.SetSkinButton(this.Btn_OneButton,
                    new BitmapImage(new Uri(@"/VMT_SC;component/Images/Common/pop_btn_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_SC;component/Images/Common/pop_btn_disable.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_SC;component/Images/Common/pop_btn_press.png", UriKind.Relative))
                    );

            this.Btn_OneButton.Content = "Close";
            this.Btn_OneButton.IsEnabled = true;
        }


        #region [Animation Event]
        public void StartAnimationTruck(int sec)
        {
            if (TimerStop == null)
            {
                TimerStop = new DispatcherTimer();
                TimerStop.Interval = new TimeSpan(0, 0, sec);
                TimerStop.Tick += new EventHandler(AnimationTimer_Handler);
                TimerStop.Start();
            }
        }

        public void StopAnimationTruck()
        {
            if (TimerStop != null)
                TimerStop.Stop();
            TimerStop = null;

            if (callback_popup != null)
                callback_popup(0);
        }

        void AnimationTimer_Handler(object sender, EventArgs e)
        {
            StopAnimationTruck();
            HidePopup();
        }
        #endregion [Animation Event]

        #region [Function List]
        public void ShowPopup(int type, String title, String message, String LeftButton, String CenterButton, String RightButton, Callback_Popup callback, int sec)
        {
            TextBlock_popup_title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0038", LanguageService.LABEL_POPUP);
            TextBlock_popup_message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0143", LanguageService.LABEL_CUSTOMIZE);

            TextBlock_popup_title.Text = title;
            TextBlock_popup_message.Text = message;
            Grid_OneButton.Visibility = System.Windows.Visibility.Hidden;
            Grid_TwoButton.Visibility = System.Windows.Visibility.Hidden;
            Grid_ThreeButton.Visibility = System.Windows.Visibility.Hidden;
            if (type == 0)
            {

            }
            else if (type == 1) // one button
            {
                Grid_OneButton.Visibility = System.Windows.Visibility.Visible;
                TextBlock_popup_Onebutton.Text = CenterButton;
            }
            else if (type == 2) // two button
            {
                Grid_TwoButton.Visibility = System.Windows.Visibility.Visible;
                TextBlock_TwoButton_Left.Text = LeftButton;
                TextBlock_TwoButton_Right.Text = RightButton;
            }
            else
            {
                Grid_ThreeButton.Visibility = System.Windows.Visibility.Visible;
                TextBlock_ThreeButton_Left.Text = LeftButton;
                TextBlock_ThreeButton_Center.Text = CenterButton;
                TextBlock_ThreeButton_Right.Text = RightButton;
            }
            this.Visibility = System.Windows.Visibility.Visible;
            callback_popup = callback;

            if (sec > 0)
            {
                StartAnimationTruck(sec);
            }
        }

        public void HidePopup()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            this.TextBlock_popup_message.Text = "";
        }
        #endregion [Function List]

        #region [UI Event]
        ///////////////////////////////////////////
        //------------------UI Event
        // Ordering : Leave /  Down / Up
        #region [Button One Event]
        private void Grid_OneButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_OK_OneButton.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
        }

        private void Grid_OneButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_OK_OneButton.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_press.png");
        }

        private void Grid_OneButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_OK_OneButton.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
            HidePopup();
            if (callback_popup != null)
                callback_popup(1);
        }
        #endregion [Button One Event]

        #region [Button Two_Left Event]
        private void Grid_TwoButton_Left_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
        }

        private void Grid_TwoButton_Left_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_press.png");
        }

        private void Grid_TwoButton_Left_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
            HidePopup();
            if (callback_popup != null)
                callback_popup(0);
        }
        #endregion [Button Two_Left Event]

        #region [Button Two_Right Event]
        private void Grid_TwoButton_Right_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
        }

        private void Grid_TwoButton_Right_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_press.png");
        }

        private void Grid_TwoButton_Right_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
            HidePopup();
            if (callback_popup != null)
                callback_popup(2);
        }
        #endregion [Button Two_Right Event]

        #region [Button Three_Left Event]
        private void Grid_ThreeButton_Left_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_ThreeButton_Left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-1_default.png");
        }

        private void Grid_ThreeButton_Left_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_ThreeButton_Left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-1_press.png");
        }

        private void Grid_ThreeButton_Left_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_ThreeButton_Left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-1_default.png");
            HidePopup();
            if (callback_popup != null)
                callback_popup(0);
        }
        #endregion [Button Three_Left Event]

        #region [Button Three_Center Event]
        private void Grid_ThreeButton_Center_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_ThreeButton_Center.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-2_default.png");
        }

        private void Grid_ThreeButton_Center_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_ThreeButton_Center.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-2_press.png");
        }

        private void Grid_ThreeButton_Center_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_ThreeButton_Center.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-2_default.png");
            HidePopup();
            if (callback_popup != null)
                callback_popup(1);
        }
        #endregion [Button Three_Center Event]

        #region [Button Three_Right Event]
        private void Grid_ThreeButton_Right_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_ThreeButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-3_default.png");
        }

        private void Grid_ThreeButton_Right_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_ThreeButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-3_press.png");
        }

        private void Grid_ThreeButton_Right_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_ThreeButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-3_default.png");
            HidePopup();
            if (callback_popup != null)
                callback_popup(2);
        }
        #endregion [Button Three_Right Event]

        public Int32 _nSystemRestart_Sec = 20;
        public void decreaseOneSec()
        {
            if (_nSystemRestart_Sec < 1)
            {
                this.Btn_OneButton.IsEnabled = true;
                PresentationMgr.AppWin.HideSystemRestartPopup();
                new PresentationMgr.SingleShot(3000, SystemRestart);
            }

            // String strFormat = "The system will reboot,\nAfter {0} Second(s)";
            // String strMessage = String.Format(strFormat, _nSystemRestart_Sec.ToString());

            String strMessage = "The system will reboot soon";

            TextBlock_popup_message.Text = strMessage;

            _nSystemRestart_Sec--;
            
        }

        private void SystemRestart()
        {   
            // PresentationMgr.App_SystemRestart();
        }

        private void Btn_OneButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // PresentationMgr.App_SystemRestart();
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion [UI Event]
    }
}