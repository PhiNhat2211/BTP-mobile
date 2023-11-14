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

using System.Runtime.InteropServices;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using HessianComm.Objects;

//20190108
using Common.Interface;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.NetworkInformation;

namespace VMT_ITV
{
    /* // Exceptional Version 3.0.0.36
    public class LogInSessionInfo
    {
        public enum LogInSessionInfoStatus
        {
            LogInSessionInfoStatus_Login,
            LogInSessionInfoStatus_ReLoging,
            LogInSessionInfoStatus_Loging,
            LogInSessionInfoStatus_Logout
        }

        public LogInSessionInfoStatus logInStatus;
        public string UserID;
        public string UserPW;
        public string DriverName;
        public string TeamName;

        public LogInSessionInfo()
        {
            logInStatus = LogInSessionInfoStatus.LogInSessionInfoStatus_Logout;
            UserID = "";
            UserPW = "";
            DriverName = "";
            TeamName = "";
        }

        ~LogInSessionInfo() { }
    }
    */

    /// <summary>
    /// Interaction logic for LogInView.xaml
    /// </summary>
    public partial class LogInView : UserControl
    {
        #region [variables]
        public enum LOGIN_STATUS { en_NoAuth = 0, en_Auth, en_LogIn, en_Connected };
        public LOGIN_STATUS LogInStatus = LOGIN_STATUS.en_NoAuth;

        // public LogInSessionInfo logInSessionInfo = new LogInSessionInfo(); // Exceptional Version 3.0.0.36
        public bool userAccessRole = false;
        public bool mIsLogin = false;

        public const int MAX_USERID_LEN = 10;

        public string LOGIN_STEP_0
        {
            get
            {
                return PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0001", LanguageService.MESSAGE_GROUP);
            }
        }

        public string LOGIN_STEP_1
        {
            get
            {
                return PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0002", LanguageService.MESSAGE_GROUP);
            }
        }
        public string LOGIN_STEP_2
        {
            get
            {
                return PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0003", LanguageService.MESSAGE_GROUP);
            }
        }
        public string LOGIN_STEP_3
        {
            get
            {
                return PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0004", LanguageService.MESSAGE_GROUP);
            }
        }

        public string LOGIN_FAIL_WIFI
        {
            get
            {
                return PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0005", LanguageService.MESSAGE_GROUP);
            }
        }
        public string LOGIN_FAIL_IDPASS
        {
            get
            {
                return PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0006", LanguageService.MESSAGE_GROUP);
            }
        }

        MainWindow mMainWindow = null;

        public Boolean isReceivedDeviceInfo = false;
        
        /*  1: 20 Feet Terminal Chassis, 
		    2: 45 Feet Terminal Chassis, 
		    3: 45 Feet GooseNeck Chassis, 
		    4: Special Chassis                  */
        public int gSelectedContainerChassic = 2;

        string langeType = "";

        public static readonly BitmapImage LoginView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_ITV;component/Images/LogInView/day/login_btn_03-1_press.png", UriKind.Relative));
        public static readonly BitmapImage LoginView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_ITV;component/Images/LogInView/day/login_btn_03-2_default.png", UriKind.Relative));

        #endregion [variables]

        public LogInView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            //TextBox_IDNumber.MaxLength = MAX_USERID_LEN;
            //PasswordBox_Password.MaxLength = MAX_USERID_LEN;

            gSelectedContainerChassic = 2;
        }

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
            Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_sel.png");
            SetDayOrNight();
            UserControl_Loaded();
        }

        private void UserControl_Loaded()
        {
            this.Label_IDNumber.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0001", LanguageService.LABEL_LOGIN);
            this.Label_Password.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0002", LanguageService.LABEL_LOGIN);
            this.Label_Name.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0003", LanguageService.LABEL_LOGIN);
            this.Label_Shift.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0014", LanguageService.LABEL_LOGIN);
            this.Label_Btn_Reset.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0010", LanguageService.LABEL_LOGIN);
            this.Label_Btn_LogIn.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0011", LanguageService.LABEL_LOGIN);
            this.Label_ScreenMode.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0006", LanguageService.LABEL_LOGIN);
            this.Label_LastLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0016", LanguageService.LABEL_LOGIN);
            this.TextBlock_Log.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0012", LanguageService.LABEL_LOGIN);
            this.Label_Btn_Connect.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0013", LanguageService.LABEL_LOGIN);
            ReadLastLocation();
        }

        public void ReadLastLocation()
        {
            String strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT
            Ini.IniFile ini = new Ini.IniFile(strIniFile);

            this.TextBox_LastLoc.Text = ini.IniReadValue("MACHINE", "LastLocation", "");
        }

        private static string GetIniDirectory()
        {
            string path = null;

            DirectoryInfo dInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            string strParentPath = dInfo.Parent.FullName;
            string strCommonPath = strParentPath + @"\Common";

            if (!Directory.Exists(strCommonPath))
            {
                Directory.CreateDirectory(strCommonPath);
            }

            path = strCommonPath + @"\";

            return path;
        }

        #region [Function List]
        public void UpdateLogInStep(LOGIN_STATUS status)
        {
            switch (status)
            {
                case LOGIN_STATUS.en_NoAuth:
                    {
                        this.TextBox_IDNumber.IsEnabled = true;
                        this.PasswordBox_Password.IsEnabled = true;
                        this.Combobox_Team.Items.Clear();
                        this.Combobox_Team.IsEnabled = false;

                        //Image_LogInStepBar.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_stepbqr_img_01.png");
                        Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
                        Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_disable.png");
                    }
                    break;
                case LOGIN_STATUS.en_Auth:
                    {
                        this.TextBox_IDNumber.IsEnabled = true;
                        this.PasswordBox_Password.IsEnabled = true;
                        this.PasswordBox_Password.Focus();
                        this.Combobox_Team.IsEnabled = true;

                        //Image_LogInStepBar.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_stepbqr_img_01.png");
                        Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
                        Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_disable.png");
                    }
                    break;
                case LOGIN_STATUS.en_LogIn:
                    {
                        this.TextBox_IDNumber.IsEnabled = false;
                        this.PasswordBox_Password.IsEnabled = false;
                        this.Combobox_Team.IsEnabled = false;
                    }
                    break;
                case LOGIN_STATUS.en_Connected:
                    break;
                default:
                    break;
            }
        }

        public void ReFlash()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            Grid_LogIn.Visibility = System.Windows.Visibility.Visible;
            Grid_AfterChassisAttach.Visibility = System.Windows.Visibility.Hidden;
            Grid_ChassisAttached.Visibility = System.Windows.Visibility.Hidden;
            Grid_ChassisSelection.Visibility = System.Windows.Visibility.Hidden;

            //TextBox_IDNumber.MaxLength = MAX_USERID_LEN;
            //PasswordBox_Password.MaxLength = MAX_USERID_LEN;
            PasswordBox_Password.IsEnabled = true;
            gSelectedContainerChassic = 2;
            //Image_LogInStepBar.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_stepbqr_img_01.png");
            TextBox_IDNumber.Text = "";
            PasswordBox_Password.Password = "";
            TextBlock_Name.Text = "";
            TextBox_Shift.Text = "";
            Combobox_Team.Items.Clear();
            Label_Btn_LogIn.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0011", LanguageService.LABEL_LOGIN);
            TextBlock_Log.Text = LOGIN_STEP_0;
            Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
            Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_disable.png");
            isReceivedDeviceInfo = false;
            this.Focusable = true;
            this.Focus();
        }

        public void SetDayOrNight()
        {
            if (mMainWindow.gIsDay)
            {
                Image_Day.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_day_sel.png");
            }
            else
            {
                Image_Night.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_night_sel.png.png");
            }
        }

        // Check Log-In Button Availablity 
        private Boolean CanLoginImageAvaliable()
        {
            Boolean ret = false;

            if (isReceivedDeviceInfo)
                return true;

            TextBlock_Log.Text = LOGIN_STEP_0;

            if (userAccessRole || "CLT".Equals(TextBox_IDNumber.Text))
            {
                TextBlock_Log.Text = LOGIN_STEP_1;

                if (PasswordBox_Password.Password.Length > 0) // Jun 5 Remove Text Length Validation
                {
                    ret = true;
                }
            }
            return ret;
        }
        #endregion [Function List]

        #region [UI Login Event]
        ///////////////////////////////////////////
        //------------------UI Event
        // Ordering : Leave /  Down / Up
        #region [TextBox ID Event]
        private void TextBox_IDNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Make event ID Text Completion
            if (/*this.TextBox_IDNumber.Text.Length == 10 ||*/ "CLT".Equals(TextBox_IDNumber.Text))  // Jun 5 Remove Text length validation
            {
                if (checkNetworkStatus() == false)
                {
                    return;
                }

                if (MainWindow.STANDALONE_MODE)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive();
                    value.bIsOn = true;
                    value.GroupListSeperator = "ITV Driver";
                    value.Notice = LOGIN_STEP_1;
                    ProcessByGetAccessRoleCallback(value);

                    return;
                }

                VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send roleRq = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send();
                roleRq.UserID = this.TextBox_IDNumber.Text;
                userAccessRole = false;

                VMT_Data_JAT2.VMT_DataMgr_Common.UserAccesRole_Ask(ref roleRq);
                mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
            }

            if (CanLoginImageAvaliable())
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
            else
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
        }

        private void TextBox_IDNumber_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //   System.Diagnostics.Process ps = new System.Diagnostics.Process();
            mMainWindow.KeypadView.ShowKeyPad(TextBox_IDNumber);
            mMainWindow.KeypadminiView.HideKeyPad();
            //ps.StartInfo.FileName = @"C:\Windows\System32\osk.exe";
            //ps.Start();
            TextBox_IDNumber.Focus();
            //   Process.Start("IExplore.exe");
        }

        private bool checkNetworkStatus()
        {
            if (mMainWindow.gIsServerConnected == false)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0110", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0023", LanguageService.LABEL_POPUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                }
                else
                {
                    PresentationMgr.AppWin.PopupView.ShowPopup(1,
                                    PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0110", LanguageService.LABEL_CUSTOMIZE), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0023", LanguageService.LABEL_POPUP)
                                    , "", PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                    
                }
                ReFlash();
                return false;
            }

            return true;
        }

        private void TextBox_IDNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (/*this.TextBox_IDNumber.Text.Length == 8 || this.TextBox_IDNumber.Text.Length == 6*/ this.TextBox_IDNumber.Text.Length > 0) // Jun 5 Remove Text Validation
            {
                if (checkNetworkStatus() == false)
                {
                    return;
                }

                if (MainWindow.STANDALONE_MODE)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive();
                    value.bIsOn = true;
                    value.GroupListSeperator = "ITV Driver";
                    value.Notice = LOGIN_STEP_1;
                    ProcessByGetAccessRoleCallback(value);

                    return;
                }

                VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send roleRq = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send();
                roleRq.UserID = this.TextBox_IDNumber.Text;
                userAccessRole = false;

                VMT_Data_JAT2.VMT_DataMgr_Common.UserAccesRole_Ask(ref roleRq);
                mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
            }

            if (CanLoginImageAvaliable())
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
            else
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
        }
        private void TextBox_IDNumber_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mMainWindow.KeypadView.ShowKeyPad(TextBox_IDNumber);
            //ps.StartInfo.FileName = @"C:\Windows\System32\osk.exe";
            //ps.Start();
            TextBox_IDNumber.Focus();
        }
        #endregion [TextBox ID Event]

        #region [TextBox Password Event]
        private void PasswordBox_Password_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            //if(PasswordBox_Password.Password.Length >= 4)
            //    TextBox_Chassis.IsEnabled = true;
            //else
            //    TextBox_Chassis.IsEnabled = false;

            if (CanLoginImageAvaliable())
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
            else
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
        }

        private void PasswordBox_Password_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //    System.Diagnostics.Process ps = new System.Diagnostics.Process();
            mMainWindow.KeypadView.ShowKeyPad(PasswordBox_Password);
            mMainWindow.KeypadminiView.HideKeyPad();
            PasswordBox_Password.Focus();
            //ps.StartInfo.FileName = @"C:\Windows\System32\osk.exe";
            //ps.Start();
        }

        private void PasswordBox_Password_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mMainWindow.KeypadView.ShowKeyPad(PasswordBox_Password);
            PasswordBox_Password.Focus();
        }
        #endregion [TextBox Password Event]

        #region [Textbox Chassis ID Event]

        #endregion [Textbox Chassis ID Event]

        #region [Button Day Event]
        private void Image_Day_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //    Image_Day.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_day_default.png");
        }

        public void Image_Day_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_Day.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_day_sel.png");
            Image_Night.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_night_default.png");
            mMainWindow.gIsDay = true;

            PresentationMgr.Singleton.ChangeDayMode(PresentationMgr.AppWin.gIsDay);
        }

        private void Image_Day_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //  Image_Day.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_day_default.png");
            // 연결
        }
        #endregion [Button Day Event]

        #region [Button Night Event]
        private void Image_Night_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //  Image_Night.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_night_default.png");
        }

        public void Image_Night_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_Night.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_night_sel.png");
            Image_Day.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_day_default.png");
            mMainWindow.gIsDay = false;

            PresentationMgr.Singleton.ChangeDayMode(PresentationMgr.AppWin.gIsDay);
        }

        private void Image_Night_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //    Image_Night.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_night_default.png");
        }
        #endregion [Button Night Event]

        #region [Button Reset Event]
        // Reset LogIn Context 
        private void Grid_ResetBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_Reset.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
        }

        private void Grid_ResetBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_Reset.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_press.png");
        }

        private void Grid_ResetBtn_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_Reset.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");

            ResetIDAndPassword();
        }

        public void ResetIDAndPassword()
        {
            // User ID should be logout to be reset.
            VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
            VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "", true);

            // LogIn Step (No Authorized)
            this.UpdateLogInStep(LOGIN_STATUS.en_NoAuth);

            ReFlash();
        }
        #endregion [Button Reset Event]

        #region [Button LogIn Event]
        private void Grid_LogInBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CanLoginImageAvaliable())
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
            else
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
        }

        private void Grid_LogInBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CanLoginImageAvaliable())
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_press.png");
            else
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
        }

        private void Grid_LogInBtn_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MainWindow.STANDALONE_MODE)
            {
                if (isReceivedDeviceInfo)
                {
                    mMainWindow.MainView.TextBlock_ID.Text = "";
                    VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "");
                    ReFlash();
                    mMainWindow.LogOut(this);
                }
                else
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive();
                    value.iLogin = 1;
                    value.UserName = "M.Jordan";
                    value.Notice = LOGIN_STEP_3;
                    ProcessByLogin4MachineCallback(value);
                    return;
                }
            }

            if (isReceivedDeviceInfo)
            {
                // anything?
                Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
                mMainWindow.PopupView.ShowPopup(2, "Log Off", "Are you sure wish to log-off?", "Cancel", "", "OK", mMainWindow.MainView.CallBack_Popup_LogOff, 0);
            }
            else
            {
                if (checkNetworkStatus() == false)
                {
                    return;
                }

                if (CanLoginImageAvaliable())
                {
                    Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_default.png");
                    mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);

                    if ("CLT".Equals(TextBox_IDNumber.Text) && "ACCESS".Equals(PasswordBox_Password.Password))
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive driverInfoRpValue = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive();
                        driverInfoRpValue.iLogin = 1;
                        driverInfoRpValue.UserName = "CLT";

                        if (VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine != null)
                            VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine(ref driverInfoRpValue);
                    }
                    else
                    {

                        VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send();
                        value.UserID = TextBox_IDNumber.Text;
                        value.UserPW = PasswordBox_Password.Password;
                        value.GroupName = Combobox_Team.SelectedItem.ToString();
                        value.MchnID = ITV.ITV_User.gMchnID;
                        value.MchnTp = ITV.ITV_User.gMchnTp;

                        VMT_Data_JAT2.VMT_DataMgr_Common.Login4Machine_Ask(ref value);
                    }

                    mMainWindow.KeypadView.HideKeyPad();
                }
                else
                    Image_LogIn.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_01_disable.png");
            }
            // send Login
        }

        public void ProcessGetChssUsingDataCallback(String result)
        {
            if (String.IsNullOrEmpty(result))
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send();
                value.UserID = TextBox_IDNumber.Text;
                value.UserPW = PasswordBox_Password.Password;
                value.GroupName = Combobox_Team.SelectedItem.ToString();
                value.MchnID = ITV.ITV_User.gMchnID;
                value.MchnTp = ITV.ITV_User.gMchnTp;

                VMT_Data_JAT2.VMT_DataMgr_Common.Login4Machine_Ask(ref value);
            }
            else
            {
                var mess = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0040", LanguageService.LABEL_POPUP), result);
                PresentationMgr.AppWin.PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("F8", LanguageService.MESSAGE_SERVER_GROUP), mess, ""
                    , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
            }
            
        }

        #endregion [Button LogIn Event]


        #region [Button Connect Event]
        private void Image_Connect_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isReceivedDeviceInfo)
                Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_default.png");
        }

        private void Image_Connect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (isReceivedDeviceInfo)
                Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_press.png");
        }

        private void Image_Connect_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MainWindow.STANDALONE_MODE)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive();
                ProcessByMachineStatusChangeCallback(value);
                return;
            }

            if (isReceivedDeviceInfo)
            {
                if (checkNetworkStatus() == false)
                {
                    return;
                }
                PresentationMgr.MainView.selectedContainer = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_default.png");

                // Connection Complete

                VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send sendMachineStatusChange = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send();
                sendMachineStatusChange.m_MchnID = ITV.ITV_User.gMchnID;
                sendMachineStatusChange.m_UserID = ITV.ITV_User.gUserID;
                sendMachineStatusChange.m_bisON = true;
                sendMachineStatusChange.m_buseRemark = true;
                sendMachineStatusChange.m_remark = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if (TextBox_IDNumber.Text != "CLT")
                {
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendMachineStatusChanged_Ask(ref sendMachineStatusChange);
                }

                VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive statusChange = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive();
                ProcessByMachineStatusChangeCallback(statusChange);
                //mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
            }
        }
        #endregion [Button Connect Event]

        #endregion [UI Login Event]

        #region [UI Chassis Event]
        ///////////////////////////////////////////
        //------------------UI Event
        // Ordering : Leave /  Down / Up
        #region [Button No Event]
        private void Grid_no_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_no.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03-1_default.png");
        }

        private void Grid_no_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_no.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03-1_press.png");
        }

        private void Grid_no_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_no.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03-1_default.png");
            GoToNextAfterChassisAttach();
        }
        #endregion [Button No Event]

        #region [Button Yes Event]
        private void Grid_yes_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_yes.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03-1_default.png");
        }

        private void Grid_yes_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_yes.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03-1_press.png");
        }

        private void Grid_yes_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_yes.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03-1_default.png");
            GoToNextChassisSelection();
        }
        #endregion [Button Yes Event]

        #region [Button Continue Event]
        private void Grid_Continue_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_Continue.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03_default.png");
        }

        private void Grid_Continue_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_Continue.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03_press.png");
        }

        private void Grid_Continue_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_Continue.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_03_default.png");
            Grid_AfterChassisAttach.Visibility = System.Windows.Visibility.Collapsed;
            Grid_ChassisSelection.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion [Button Continue Event]

        #region [Button Cancel Event]
        private void Grid_Cancel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_cancel.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
        }

        private void Grid_Cancel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_cancel.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_press.png");
        }

        private void Grid_Cancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_cancel.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
            Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_sel.png");
            Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_default.png");
            Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_default.png");
            Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_default.png");
            gSelectedContainerChassic = 2;
        }
        #endregion [Button Cancel Event]

        #region [Button Ok Event]
        private void Grid_OK_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image_OK.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
        }

        private void Grid_OK_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_OK.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_press.png");

        }

        private void Grid_OK_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_OK.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_04-1_default.png");
            
            if (MainWindow.SERVICE_COMPANY.Equals("JAT2"))
            {
                mMainWindow.ShowChassisNumberDlg();
            }
        }
        #endregion [Button Ok Event]

        public void GoToNextChassisSelection()
        {
            Grid_ChassisAttached.Visibility = System.Windows.Visibility.Collapsed;
            Grid_ChassisSelection.Visibility = System.Windows.Visibility.Visible;
            //Image_LogInStepBar.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_stepbqr_img_03.png");
        }

        private void GoToNextChassisAttached()
        {
            Grid_LogIn.Visibility = System.Windows.Visibility.Collapsed;
            Grid_ChassisAttached.Visibility = System.Windows.Visibility.Visible;
            //Image_LogInStepBar.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_stepbqr_img_02.png");
        }

        private void GoToNextAfterChassisAttach()
        {
            Grid_ChassisAttached.Visibility = System.Windows.Visibility.Collapsed;
            Grid_AfterChassisAttach.Visibility = System.Windows.Visibility.Visible;
            //Image_LogInStepBar.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_stepbqr_img_02.png");
        }

        private void GoToNextMainView()
        {   
            VMT_DataMgr.MainView_Init(MainWindow.SERVICE_COMPANY);
            PresentationMgr.MainView.ProcessBySetMachineStopConfirm();
            mMainWindow.gotoMainView();
        }

        #region [Button Chassis1 Event]
        private void Image_chassis1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //   Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_default.png");
        }

        private void Image_chassis1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_sel.png");
            Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_default.png");
            Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_default.png");
            Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_default.png");
            gSelectedContainerChassic = 2;
        }

        private void Image_chassis1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //  Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_sel.png");
        }
        #endregion [Button Chassis1 Event]

        #region [Button Chassis2 Event]
        private void Image_chassis2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //  Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_default.png");
        }

        private void Image_chassis2_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_default.png");
            //Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_sel.png");
            //Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_default.png");
            //Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_default.png");
            //gSelectedContainerChassic = 1;
        }

        private void Image_chassis2_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //   Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_sel.png");
        }
        #endregion [Button Chassis2 Event]

        #region [Button Chassis3 Event]
        private void Image_chassis3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //   Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_default.png");
        }

        private void Image_chassis3_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_default.png");
            //Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_default.png");
            //Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_sel.png");
            //Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_default.png");
            //gSelectedContainerChassic = 3;
        }

        private void Image_chassis3_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //  Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_sel.png");
        }
        #endregion [Button Chassis3 Event]

        #region [Button Chassis4 Event]
        private void Image_chassis4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //   Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_default.png");
        }

        private void Image_chassis4_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Image_chassis1.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis02_default.png");
            //Image_chassis2.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis01_default.png");
            //Image_chassis3.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis03_default.png");
            //Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_sel.png");
            //gSelectedContainerChassic = 4;
        }

        private void Image_chassis4_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //  Image_chassis4.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_chassis04_sel.png");
        }
        #endregion [Button Chassis4 Event]

        #endregion [UI Chassis Event]

        #region [Process Callback]
        ///////////////////////////////////////////
        //------------------Process Callback
        ///////////////////////////////////////////
        #region [GetAccessRole Callback]
        // Return UserAccessRole Interface
        public void ProcessByGetAccessRoleCallback(VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value)
        {
            // Clear Team List
            Combobox_Team.Items.Clear();

            if (value.GroupListSeperator is String)
            {
                String[] teamNameList = value.GroupListSeperator.Split('|');
                foreach (String teamName in teamNameList)
                {
                    this.Combobox_Team.Items.Add(teamName);
                    //this.Combobox_Team.Content = teamName; break;
                }
            }
            else
            {
                foreach (Object grpName in value.GroupListSeperator)
                {
                    String strGrpName = grpName as String;
                    this.Combobox_Team.Items.Add(strGrpName);
                    //this.Combobox_Team.Content = grpName; break;
                }
            }

            Combobox_Team.SelectedIndex = 0;

            TextBlock_Name.Text = value.usrNm;
            if (value.shift.Equals("AA"))
                TextBox_Shift.Text = "A";
            else if (value.shift.Equals("BB"))
                TextBox_Shift.Text = "B";
            else
                TextBox_Shift.Text = value.shift;

            if (value.Notice.Length > 0)
                TextBlock_Log.Text = value.Notice;

            // LogIn Step (Authorized)
            this.UpdateLogInStep(LOGIN_STATUS.en_Auth);

            mMainWindow.HideProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
        }
        #endregion [GetAccessRole Callback]

        #region [DiviceInfo Callback]
        public void ProcessByLogin4MachineCallback(VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value)
        {
            /* // Exceptional Version 3.0.0.36
            if (logInSessionInfo.logInStatus == LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_ReLoging)
            {
                VMT_DataMgr.SVMT_LogInRq logInRq = new VMT_DataMgr.SVMT_LogInRq();
                logInRq.UserID = logInSessionInfo.UserID;
                logInRq.UserPW = logInSessionInfo.UserPW;
                logInRq.DriverName = logInSessionInfo.DriverName;
                logInRq.TeamName = logInSessionInfo.TeamName;

                VMT_DataMgr.SendLogIn(ref logInRq);

                mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
                return;
            }
            */

            // TextBlock_Log.
            mIsLogin = true;
            ITV.ITV_User.gUserID = TextBox_IDNumber.Text;
            ITV.ITV_User.gUserPW = PasswordBox_Password.Password;
            ITV.ITV_User.gUserNm = value.UserName;

            isReceivedDeviceInfo = true;
            TextBlock_Name.Text = value.UserName;

            Label_Btn_LogIn.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0060", LanguageService.LABEL_LOGIN);

            Image_Connect.Source = mMainWindow.getImageByDayOrNightForLogIn(@"login_btn_02_default.png");

            if (value.Notice.Length > 5)
                TextBlock_Log.Text = value.Notice;
            else
                TextBlock_Log.Text = LOGIN_STEP_3;

            mMainWindow.MainView.SetUserID(ITV.ITV_User.gUserID);

            this.UpdateLogInStep(LOGIN_STATUS.en_LogIn);
            MainWindow.login = true;
        }
        #endregion [DiviceInfo Callback]

        #region [LogIn Callback]
        public void ProcessByMachineStatusChangeCallback(VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value)
        {
            /* // Exceptional Version 3.0.0.36
            if (logInSessionInfo.logInStatus == LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_ReLoging)
            {
                logInSessionInfo.logInStatus = LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_Login;
                return;
            }
            else
            {
                logInSessionInfo.logInStatus = LogInSessionInfo.LogInSessionInfoStatus.LogInSessionInfoStatus_Login;
                // TextBlock_Log.
                GoToNextChassisAttached();
            }
            */

            bool isChassisAttach = false;
            bool isShowChassis = false;
            if (isChassisAttach)
            {
                /////////////////////////////////////////////////////////
                //- Go to Chassis Attach
                GoToNextChassisAttached();
                /////////////////////////////////////////////////////////
            }
            else if (isShowChassis)
            {
                /////////////////////////////////////////////////////////
                //-Go to Chassis Number Input
                GoToNextChassisSelection();
                mMainWindow.ShowChassisNumberDlg();
                /////////////////////////////////////////////////////////
            }
            else
            {
                /////////////////////////////////////////////////////////
                //-Go to MainView
                GoToNextMainView();
                /////////////////////////////////////////////////////////
            }
        }
        #endregion [LogIn Callback]

        #endregion [Process Callback]

        private void Btn_A_Click(object sender, RoutedEventArgs e)
        {
            Btn_A.Background = new SolidColorBrush(Colors.DimGray);
            Btn_B.Background = new SolidColorBrush(Colors.DarkGray);
        }

        private void Btn_B_Click(object sender, RoutedEventArgs e)
        {
            Btn_A.Background = new SolidColorBrush(Colors.DarkGray);
            Btn_B.Background = new SolidColorBrush(Colors.DimGray);
        }
    }
}