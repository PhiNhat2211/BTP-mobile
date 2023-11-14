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
using System.Reflection;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;


//20190108
using Common.Interface;
using System.Diagnostics;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for Indicator.xaml
    /// </summary>
    public partial class Indicator : UserControl
    {
        MainWindow mMainWindow = null;
        public int gSoftWareInfo = 0;
        DispatcherTimer TimerJobTime;
        private int jobTime;
        private int ETATime;
        private int mHiddenMenuDownCount = 0;
        private int mHiddenMenuDownCount_Cab = 0;
        private int connectGPSCoutn = 0;
        private double dLatitude = 0;
        private double dLongitude = 0;

        public static int nTestValue = 0;

        public Indicator()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            // Set Current Version
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string ver = fvi.FileVersion;
            string[] vn = ver.Split('.');
            this.TextBox_Version.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0050", LanguageService.MESSAGE_GROUP) + " " + vn[0] + "." + vn[1] + "." + vn[2];

            //   ComboBox_OrderList.ItemsSource = "1A-31";
            //   ComboBox_OrderList.Items.Add("1A-31");
        }

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
            this.Lbl_User_Val.Content = String.Empty;
            this.Lbl_YtNoChssNo.Content = UserInfo.gMchnID;
        }

        public void ReFlash()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            Label_time.Content = "";
            //Image_close.Visibility = System.Windows.Visibility.Visible;
            Image_bg.Source = mMainWindow.getImageByDayOrNight(@"g_indicator_bg02.png");

            //TextBox_Version.Foreground = new SolidColorBrush(Colors.White);
            TextBox_Job.Foreground = new SolidColorBrush(Colors.White);

            StopJobTimer();
        }

        private int Abs(int a)
        {
            int r = 0;
            if (a >= 0)
            {
                r = a;
            }
            else
            {
                r -= a; //넘겨온 값이 마이너스이면 -(음수)기호로 +로
            }
            return r;
        }

        public void StartJobTimer(int JobTime, int ETATime)
        {
            this.jobTime = JobTime;
            this.ETATime = ETATime;
            int job_min = JobTime / 60;
            int job_sec = JobTime - (job_min * 60);
            int eta_min = ETATime / 60;
            int eta_sec = ETATime - (eta_min * 60);

            string strETA = string.Format("{0:D2}:{1:D2}/{2:D2}:{3:D2}",
                job_min, job_sec, eta_min, eta_sec);

            Label_time.Content = strETA;

            if (TimerJobTime == null)
            {
                //Dec 12 2019 Don't need to show
                //Label_time.Visibility = System.Windows.Visibility.Visible;
                TimerJobTime = new DispatcherTimer();
                TimerJobTime.Interval = new TimeSpan(0, 0, 0, 1);
                TimerJobTime.Tick += new EventHandler(AnimationTimer_Handler);
                TimerJobTime.Start();
            }
        }

        public void StopJobTimer()
        {
            jobTime = 0;
            ETATime = 300; // 5min is default
            if (TimerJobTime != null)
                TimerJobTime.Stop();

            TimerJobTime = null;
            Label_time.Visibility = System.Windows.Visibility.Hidden;
        }

        void AnimationTimer_Handler(object sender, EventArgs e)
        {
            this.jobTime++;
            if (this.jobTime < this.ETATime)
            {
                if (mMainWindow.gIsDay)
                    Label_time.Foreground = PresentationMgr.brush_text_black;
                else
                    Label_time.Foreground = PresentationMgr.brush_text_white;
            }
            else
                Label_time.Foreground = new SolidColorBrush(Colors.Red);

            int job_min = this.jobTime / 60;
            int job_sec = this.jobTime - (job_min * 60);
            int eta_min = this.ETATime / 60;
            int eta_sec = this.ETATime - (eta_min * 60);

            string strETA = string.Format("{0:D2}:{1:D2}/{2:D2}:{3:D2}",
                job_min, job_sec, eta_min, eta_sec);

            Label_time.Content = strETA;
        }

        // private System.Timers.Timer m_timer = null; // Exceptional Version 3.0.0.36
        // private System.Timers.Timer m_RestartTimer = null;
        private bool bRestart = false;
        public void setWifiData(int value)
        {
            if (value == 0)
                mMainWindow.gIsServerConnected = false;
            else
                mMainWindow.gIsServerConnected = true;


            if (value == 0)
                Image_wifi.Source = mMainWindow.getImageByDayOrNight(@"g_Indicator_img_wifi_disable.png");
            else
                Image_wifi.Source = mMainWindow.getImageByDayOrNight(@"g_Indicator_img_wifi_default.png");


            /* // Exceptional Version 3.0.0.36
            if (value == 0)
            {   
                if (mMainWindow.LoginView.logInSessionInfo.logInStatus == LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_Login)
                {
                    mMainWindow.LoginView.logInSessionInfo.logInStatus = LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_ReLoging;

                    //VMT_DataMgr.SVMT_LogInRq logInRq = new VMT_DataMgr.SVMT_LogInRq();
                    //logInRq.UserID = mMainWindow.LoginView.logInSessionInfo.UserID;
                    //logInRq.UserPW = mMainWindow.LoginView.logInSessionInfo.UserPW;
                    //logInRq.DriverName = mMainWindow.LoginView.logInSessionInfo.DriverName;
                    //logInRq.TeamName = mMainWindow.LoginView.logInSessionInfo.TeamName;

                    //VMT_DataMgr.SendLogIn(ref logInRq);
                    //mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);

                    //VMT_DataMgr.SVMT_DriverInfoRq driverInfoRq = new VMT_DataMgr.SVMT_DriverInfoRq();
                    //driverInfoRq.UserID = mMainWindow.LoginView.logInSessionInfo.UserID;
                    //driverInfoRq.UserPW = mMainWindow.LoginView.logInSessionInfo.UserPW;

                    //VMT_DataMgr.SendDriverInfo(ref driverInfoRq);
                    //mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
                }
                else if (mMainWindow.LoginView.logInSessionInfo.logInStatus == LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_ReLoging)
                {
                    if (m_timer != null)
                    {
                        m_timer.Stop();
                    }
                    else
                    {
                        m_timer = new System.Timers.Timer(3000);
                        m_timer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsedFunc);
                        m_timer.AutoReset = false;
                    }
                    m_timer.Start();
                }
            }
            */

            if (value == 0 && mMainWindow.countKeepAliveFail >= 3 && mMainWindow.LoginView.mIsLogin == true && !mMainWindow.LoginView.IsVisible && mMainWindow.MainView.popupShowed == false)
            {
                // reStart Application Module
                /*
                if (m_RestartTimer == null)
                {
                    m_RestartTimer = new System.Timers.Timer(1 * 60 * 1000); // 1 min
                    // m_RestartTimer = new System.Timers.Timer(10 * 1000); // test 10 sec
                    m_RestartTimer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsedFunc);
                    m_RestartTimer.AutoReset = false;
                    m_RestartTimer.Start();
                }
                */

                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                       new Action(delegate
                       {
                           if (mMainWindow.WifiPopup.Visibility != System.Windows.Visibility.Visible &&
                               bRestart == false)
                               mMainWindow.WifiPopup.Visibility = System.Windows.Visibility.Visible;
                       }));
            }
            else
            {
                // reStart Application Module
                /*
                if (m_RestartTimer != null)
                {
                    m_RestartTimer.Stop();
                }
                m_RestartTimer = null;
                */

                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                       new Action(delegate
                       {
                           if (mMainWindow.WifiPopup.Visibility == System.Windows.Visibility.Visible)
                               mMainWindow.WifiPopup.Visibility = System.Windows.Visibility.Hidden;
                       }));
            }
        }

        // reStart Application Module
        /*
        private void timerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_RestartTimer.Stop();

             this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                       new Action(delegate
                       {
                           bRestart = true;
                           if (mMainWindow.WifiPopup.Visibility == System.Windows.Visibility.Visible)
                               mMainWindow.WifiPopup.Visibility = System.Windows.Visibility.Hidden;

                            mMainWindow.RestartPopup.ShowPopup(1, "Warning", "Wi-Fi was disconnected for 1 minutes.\nYou need restart application.", "", "OK", "", CallBack_Popup_Restart, 0);
                       }));
        }

        private void CallBack_Popup_Restart(int seleted)
        {
            VMT_DataMgr.SVMT_ShutDown shutDown = new VMT_DataMgr.SVMT_ShutDown();
            VMT_DataMgr.SendSystemOff(ref shutDown);

            //--------------------------------------------------------------
            //- Save Touch Event for CLTAgent to restart application
            //PresentationMgr.FileTouchEvent_RestartApp();
            PresentationMgr.RestartApp();

            PresentationMgr.APP_CloseApp();
        }
        */

        /* // Exceptional Version 3.0.0.36
        private void timerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timer.Stop();
            // mMainWindow.LoginView.logInSessionInfo.logInStatus = LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_Loging;
            PresentationMgr.SingleShot(10000, SendLogIn); // 10 sec
        }

        private void SendLogIn()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                       new Action(delegate
                       {
                           //VMT_DataMgr.SVMT_ShutDown shutDown = new VMT_DataMgr.SVMT_ShutDown();
                           //VMT_DataMgr.SendSystemOff(ref shutDown);

                           //System.Threading.Thread.Sleep(500);

                           //VMT_DataMgr.SVMT_LogInRq logInRqTest = new VMT_DataMgr.SVMT_LogInRq();
                           //logInRqTest.UserID = "asdasd";
                           //logInRqTest.UserPW = "asdasd";
                           //logInRqTest.DriverName = "asdasd";
                           //logInRqTest.TeamName = "asdasd";

                           //VMT_DataMgr.SendLogIn(ref logInRqTest);

                           //System.Threading.Thread.Sleep(500);

                           VMT_DataMgr.SVMT_DriverInfoRq driverInfoRq = new VMT_DataMgr.SVMT_DriverInfoRq();
                           driverInfoRq.UserID = mMainWindow.LoginView.logInSessionInfo.UserID;
                           driverInfoRq.UserPW = mMainWindow.LoginView.logInSessionInfo.UserPW;

                           VMT_DataMgr.SendDriverInfo(ref driverInfoRq);
                       }));
        }
        */

        public void setGPSData(int value)
        {
            if (value == 0)
                Image_gps.Source = mMainWindow.getImageByDayOrNight(@"g_indicator_img_gps_disable.png");
            else
            {
                if (connectGPSCoutn == 0)
                {
                    //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_PUBX_05);
                    if (MainWindow.SERVICE_COMPANY.Equals("JAT3"))
                        VMT_DataMgr_Common.SendPubx05_Ask(); // T2 Delete
                    connectGPSCoutn++;
                }
                else if (connectGPSCoutn == 1)
                {
                    //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_PUBX_06);
                    if (MainWindow.SERVICE_COMPANY.Equals("JAT3"))
                        VMT_DataMgr_Common.SendPubx06_Ask(); // T2 Delete
                    connectGPSCoutn++;
                }

                Image_gps.Source = mMainWindow.getImageByDayOrNight(@"g_indicator_img_gps_default.png");
            }

            if (value > 3)
            {
                // TODO
                // Warning Message 추가 
            }
        }

        public void SetGeoFence(ref VMT_Data_JAT2.Objects.Common.VD_Common_GPS value)
        {
            double.TryParse(value.Latitude, out dLatitude);
            double.TryParse(value.Longitude, out dLongitude);
        }

        public Point GeoFence_LT; // left top
        public Point GeoFence_RT; // right top
        public Point GeoFence_RB; // right bottom
        public Point GeoFence_LB; // left bottom

        private List<Point> listPoly = new List<Point>();
        public void InitGeoFence()
        {
            // Init Polygon points
            listPoly.Add(GeoFence_LT); // left top
            listPoly.Add(GeoFence_RT); // right top
            listPoly.Add(GeoFence_RB); // right bottom
            listPoly.Add(GeoFence_LB); // left bottom

            // Scaling Point Value
            for (int i = 0; i < listPoly.Count; i++)
            {
                Point pt = listPoly[i];

                pt.X *= 100000;
                pt.Y *= 100000;

                listPoly[i] = pt;
            }
        }

        //  Globals which should be set before calling this function:
        //
        //  int    polySides  =  how many corners the polygon has
        //  float  polyX[]    =  horizontal coordinates of corners
        //  float  polyY[]    =  vertical coordinates of corners
        //  float  x, y       =  point to be tested
        //
        //  (Globals are used in this example for purposes of speed.  Change as
        //  desired.)
        //
        //  The function will return YES if the point x,y is inside the polygon, or
        //  NO if it is not.  If the point is exactly on the edge of the polygon,
        //  then the function may return YES or NO.
        //
        //  Note that division by zero is avoided because the division is protected
        //  by the "if" clause which surrounds it.
        public bool isGeoFenceIn()
        {
            double x = dLatitude * 100000; // Scaling
            double y = dLongitude * 100000; // Scaling

            int polySides = listPoly.Count;

            int i, j = polySides - 1;
            bool oddNodes = false;

            for (i = 0; i < polySides; i++)
            {
                if (listPoly[i].Y < y && listPoly[j].Y >= y ||
                    listPoly[j].Y < y && listPoly[i].Y >= y)
                {
                    if (listPoly[i].X + (y - listPoly[i].Y) / (listPoly[j].Y - listPoly[i].Y) * (listPoly[j].X - listPoly[i].X) < x)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        public void setJobTime(String value)
        {
            Label_time.Content = value;
        }

        public void ChangeMainView()
        {
            Image_close.Visibility = System.Windows.Visibility.Hidden;
            Image_logout.Visibility = Visibility.Visible;
            Image_Setting.Visibility = System.Windows.Visibility.Hidden;
            this.Lbl_User_Val.Content = UserInfo.gUserNm;
            //sliderColor.Width = 150;
            //sliderColor.Margin = new Thickness(0, 20, 70, 19);
            //sliderColor.Visibility = Visibility.Hidden;
            //mMainWindow.MainView.sliderColor.Value = sliderColor.Value;
            //Image_bg.Source = mMainWindow.getImageByDayOrNight(@"g_indicator_bg01.png");
            //if (mMainWindow.gIsDay)
            //{
            //    TextBox_Version.Foreground = new SolidColorBrush(Colors.Black);
            //    TextBox_Job.Foreground = new SolidColorBrush(Colors.Black);
            //}
            //else
            //{
            //    TextBox_Version.Foreground = new SolidColorBrush(Colors.White);
            //    TextBox_Job.Foreground = new SolidColorBrush(Colors.White);
            //}
        }
        public void SetDownloadProgress(int curValue, int maxValue)
        {
            if (curValue > 0 && maxValue > 0)
            {
                this.Prgb_Download.Maximum = maxValue;
                this.Prgb_Download.Value = curValue;
                if (curValue == maxValue)
                {
                    this.Prgb_Download.Visibility = Visibility.Hidden;
                    this.TextBox_Version.Visibility = Visibility.Visible;
                    this.TextBox_Version.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0051", LanguageService.MESSAGE_GROUP); // Need Restart
                    this.TextBox_Version.Foreground = Brushes.Red;
                }
                else
                {
                    this.Prgb_Download.Visibility = Visibility.Visible;
                    this.TextBox_Version.Visibility = Visibility.Hidden;
                }
            }         
        }      

        private void Image_close_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_close.Source = mMainWindow.getImageByDayOrNight("g_indicator_btn_close_default.png");
        }

        private void Image_close_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_close.Source = mMainWindow.getImageByDayOrNight("g_indicator_btn_close_press.png");
            mMainWindow.PopupView.ShowPopup(3, PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0036", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0033", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0030", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0031", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0032", LanguageService.MESSAGE_GROUP), CallBack_Popup_SystemOff, 0);
        }

        private void Image_close_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_close.Source = mMainWindow.getImageByDayOrNight("g_indicator_btn_close_default.png");
        }
        private void Image_Setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mMainWindow.ShowMachineView();
        }
        private void Image_logout_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_logout.Source = mMainWindow.getImageByDayOrNight("IndicatorView_Logout_Default_1.png");
        }

        private void Image_logout_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_logout.Source = mMainWindow.getImageByDayOrNight("IndicatorView_Logout_Default_1.png");
            mMainWindow.PopupView.ShowPopup(2, PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0036", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0035", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0030", LanguageService.MESSAGE_GROUP), ""
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), CallBack_Popup_LogOff, 0);
        }

        private void Image_logout_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_logout.Source = mMainWindow.getImageByDayOrNight("IndicatorView_Logout_Default_1.png");
        }      
        public void CallBack_Popup_SystemOff(int seleted)
        {
            switch (seleted)
            {
                case 0: // left cancel

                    break;
                case 1: // center system off
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "");
                        System.Diagnostics.Process.Start("shutdown.exe", "-s -f");
                    }
                    break;
                case 2: // right log out
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        //MainWindow.logOff = true;
                        //VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);

                        PresentationMgr.APP_CloseApp();

                        // this.Close();
                        // VMT_DataMgr.SVMT_ShutDown shutDown = new VMT_DataMgr.SVMT_ShutDown();
                        // VMT_DataMgr.SendSystemOff(shutDown);
                        // mMainWindow.LogOut(this);
                    }
                    break;
            }
        }
        public void CallBack_Popup_LogOff(int seleted)
        {
            switch (seleted)
            {
                case 0: // left cancel
                    break;
                case 1: // center system off
                    {
                    }
                    break;
                case 2: // right log out
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, PresentationMgr.AppWin.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? PresentationMgr.AppWin.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "");
                        this.Image_close.Visibility = Visibility.Visible;
                        this.Image_logout.Visibility = Visibility.Hidden;
                        mMainWindow.LoginView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                        mMainWindow.LoginView.ReFlash();
                        this.Lbl_YtNoChssNo.Content = UserInfo.gMchnID;
                        this.Lbl_User_Val.Content = String.Empty;
                        this.Image_Setting.Visibility = Visibility.Visible;

                        ReFlash();

                        mMainWindow.LogOut(mMainWindow.MainView);
                    }
                    break;
            }
        }
        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int count = 0;
            if (!MainWindow.TEST_MODE)
                count = e.ClickCount;

            Window win = PresentationMgr.AppWin;
            if (win != null)
            {
                win.DragMove();
            }
        }


        #region [Hidden Menue]
        private void Image_wifi_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //// [TEST_CODE]
            //if (nTestValue == 1)
            //{
            //    mMainWindow.NotifyEngineTemp(nTestValue);
            //    nTestValue = 0;
            //}
            //else
            //{
            //    mMainWindow.NotifyEngineTemp(nTestValue);
            //    nTestValue = 1;
            //}

            if (++mHiddenMenuDownCount > 5)
            {
                mMainWindow.ShowMachineView();
                mHiddenMenuDownCount = 0;
            }            
        }

        private void Image_gps_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (mHiddenMenuDownCount_Cab > 5)
            {
                mMainWindow.ShowCalibrationView();
                mHiddenMenuDownCount_Cab = 0;
            }
            mHiddenMenuDownCount_Cab++;
        }

        #endregion [Hidden Menu]



        private void sliderColor_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            setFormLight();
        }

        private void sliderColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            setFormLight();
        }

        private void setFormLight()
        {
            MainWindow mainWin = (MainWindow)Window.GetWindow(this);

            if (sliderColor.Value == 1)
            {
                mMainWindow.LoginView.Image_Night_MouseLeftButtonDown(null, null);
                mainWin.LayoutRoot.Opacity = 1;
                return;
            }
            else
            {
                mMainWindow.LoginView.Image_Day_MouseLeftButtonDown(null, null);
            }

            double sliderValue = ((int)sliderColor.Value) - 5;
            SolidColorBrush bgColor = new SolidColorBrush(sliderValue > -1 ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 0, 0, 0));

            sliderValue = 1 - ((Math.Abs(sliderValue) * 1.5) / 10);
            LayoutRoot.Opacity = sliderValue;
            mainWin.LayoutRoot.Opacity = sliderValue;
            mainWin.MainWin.Background = bgColor;
        }

    }
}