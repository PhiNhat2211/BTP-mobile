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

using System.Data;

//20190108
using Common.Interface;

namespace VMT_ITV
{
	/// <summary>
    /// Interaction logic for WifiPopup.xaml
	/// </summary>
    public partial class TeamSelectPopup : UserControl
    {
        #region [variables]
        private MainWindow mMainWindow;

        public delegate void Callback_Popup(int seleted);

        static private Callback_Popup callback_popup;

        private DispatcherTimer TimerStop = null;
        #endregion [variables]

        public TeamSelectPopup()
		{
			this.InitializeComponent();
		}

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
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
        public void AddTeamList()
        {   
            DataTable dt = new DataTable();
            dt.Columns.Add("TeamName", typeof(String));
            DataRow dr = null;

            dr = dt.NewRow();
            dr[0] = "A Team";
            dt.Rows.Add(dr);


            dr = dt.NewRow();
            dr[0] = "B Team";

            dt.Rows.Add(dr);

            this.ListView_Team.DataContext = dt;
        }

        public void ShowPopup( int type, String title, String message, String LeftButton, String CenterButton, String RightButton, Callback_Popup callback, int sec)
        {
            TextBlock_popup_title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0144", LanguageService.LABEL_CUSTOMIZE);
            TextBlock_TwoButton_Left.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0016", LanguageService.LABEL_SETTING);
            TextBlock_TwoButton_Right.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_POPUP);

            TextBlock_popup_title.Text = title;
            // TextBlock_popup_message.Text = message;
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
            // this.TextBlock_popup_message.Text = "";
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

        #endregion [UI Event]
    }
}