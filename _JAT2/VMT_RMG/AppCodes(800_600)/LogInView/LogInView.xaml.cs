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

//using ExternalAPI;
using System.Runtime.InteropServices;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using VMT_RMG;

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for LogInView.xaml
    /// </summary>
    public partial class LogInView : UserControl
    {
        #region [variables]
        public enum LOGIN_STATUS { en_NoAuth = 0, en_Auth, en_LogIn, en_Connected };
        public LOGIN_STATUS LogInStatus = LOGIN_STATUS.en_NoAuth;

        public bool mIsLogin = false;
        
        public const int MAX_USERID_LEN = 6;

        public const String LOGIN_STEP_0 = "Input Your ID No.";
        public const String LOGIN_STEP_1 = "Input Password\nPress the Log-In Button";
        public const String LOGIN_STEP_2 = "Select Your Team\nWellcome to ";
        public const String LOGIN_STEP_3 = "Press the Connect Button";

        public const String LOGIN_FAIL_WIFI = "Check your WIFI connection";
        public const String LOGIN_FAIL_IDPASS = "Incorrect ID/Password. Please Retry";

        //private String ID = "";
        //private String Password = "";

        public Boolean isReceivedDeviceInfo = false;        

        #endregion [variables]

        public LogInView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            TextBox_IDNumber.MaxLength = MAX_USERID_LEN;
            PasswordBox_Password.MaxLength = MAX_USERID_LEN;

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~LogInView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitSkinImage();

            if (App.STANDALONE_MODE)
                this.Btn_Connect.IsEnabled = true;

            if (PresentationMgr.AppWin != null)
                PresentationMgr.AppWin.UC_KeypadView.DoneCallback += new Keypad.KeypadDoneCallback(this.KeypadDone);
        }

        private void KeypadDone()
        {
            PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                //PresentationMgr.SetSkinButton(this.Btn_Team,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_team_default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_team_press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_team_disable.png", UriKind.Relative))
                //    );

                PresentationMgr.SetSkinButton(this.Btn_Reset,
                    UIThemeMgr.Day.LoginView_ButtonResetDefaultImage, UIThemeMgr.Day.LoginView_ButtonResetPressImage, UIThemeMgr.Day.LoginView_ButtonResetDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_reset_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_reset_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_reset_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Login,
                    UIThemeMgr.Day.LoginView_ButtonLoginDefaultImage, UIThemeMgr.Day.LoginView_ButtonLoginPressImage, UIThemeMgr.Day.LoginView_ButtonLoginDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_login_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_login_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_login_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Logout,
                    UIThemeMgr.Day.LoginView_ButtonLogoutDefaultImage, UIThemeMgr.Day.LoginView_ButtonLogoutPressImage, UIThemeMgr.Day.LoginView_ButtonLogoutPressImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_logout_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_logout_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_logout_press.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinRadioButton(this.Btn_Day,
                    UIThemeMgr.Day.LoginView_ButtonDayPressImage, UIThemeMgr.Day.LoginView_ButtonDayDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_day_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_day_default.png", UriKind.Relative)));

                PresentationMgr.SetSkinRadioButton(this.Btn_Night,
                    UIThemeMgr.Day.LoginView_ButtonNightPressImage, UIThemeMgr.Day.LoginView_ButtonNightDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_night_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_night_default.png", UriKind.Relative)));

                PresentationMgr.SetSkinButton(this.Btn_Connect,
                    UIThemeMgr.Day.LoginView_ButtonConnectDefaultImage, UIThemeMgr.Day.LoginView_ButtonConnectPressImage, UIThemeMgr.Day.LoginView_ButtonConnectDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_connect_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_connect_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_connect_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Setting,
                    UIThemeMgr.Day.LoginView_ButtonResetDefaultImage, UIThemeMgr.Day.LoginView_ButtonResetPressImage, UIThemeMgr.Day.LoginView_ButtonResetDisableImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                //PresentationMgr.SetSkinButton(this.Btn_Team,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_team_default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_team_press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_team_disable.png", UriKind.Relative))
                //    );

                PresentationMgr.SetSkinButton(this.Btn_Reset,
                    UIThemeMgr.Night.LoginView_ButtonResetDefaultImage, UIThemeMgr.Night.LoginView_ButtonResetPressImage, UIThemeMgr.Night.LoginView_ButtonResetDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_reset_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_reset_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_reset_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Login,
                    UIThemeMgr.Night.LoginView_ButtonLoginDefaultImage, UIThemeMgr.Night.LoginView_ButtonLoginPressImage, UIThemeMgr.Night.LoginView_ButtonLoginDisableImage);
                //new BitmapImage(ew Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_login_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_login_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_login_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Logout,
                    UIThemeMgr.Night.LoginView_ButtonLogoutDefaultImage, UIThemeMgr.Night.LoginView_ButtonLogoutPressImage, UIThemeMgr.Night.LoginView_ButtonLogoutPressImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_logout_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_logout_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_logout_press.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinRadioButton(this.Btn_Day,
                    UIThemeMgr.Night.LoginView_ButtonDayPressImage, UIThemeMgr.Night.LoginView_ButtonDayDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_day_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_day_default.png", UriKind.Relative)));

                PresentationMgr.SetSkinRadioButton(this.Btn_Night,
                    UIThemeMgr.Night.LoginView_ButtonNightPressImage, UIThemeMgr.Night.LoginView_ButtonNightDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_night_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_night_default.png", UriKind.Relative)));

                PresentationMgr.SetSkinButton(this.Btn_Connect,
                    UIThemeMgr.Night.LoginView_ButtonConnectDefaultImage, UIThemeMgr.Night.LoginView_ButtonConnectPressImage, UIThemeMgr.Night.LoginView_ButtonConnectDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_connect_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_connect_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_connect_disable.png", UriKind.Relative))
                //);
                PresentationMgr.SetSkinButton(this.Btn_Setting,
                    UIThemeMgr.Night.LoginView_ButtonResetDefaultImage, UIThemeMgr.Night.LoginView_ButtonResetPressImage, UIThemeMgr.Night.LoginView_ButtonResetDisableImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        #region [Function List]
        public void UpdateLogInStep(LOGIN_STATUS status)
        {
            switch (status)
            {
                case LOGIN_STATUS.en_NoAuth:
                    {
                        this.TextBox_IDNumber.IsEnabled = true;
                        this.PasswordBox_Password.IsEnabled = false;
                        this.Combobox_Team.Items.Clear();
                        this.Combobox_Team.IsEnabled = false;

                        this.Btn_Login.IsEnabled = false;
                        this.Btn_Login.Visibility = System.Windows.Visibility.Visible;
                        this.Btn_Logout.IsEnabled = false;
                        this.Btn_Logout.Visibility = System.Windows.Visibility.Hidden;

                        this.Btn_Connect.IsEnabled = false;

                        // UI Memnber Clear
                        isReceivedDeviceInfo = false;
                        this.TextBox_IDNumber.MaxLength = MAX_USERID_LEN;
                        this.PasswordBox_Password.MaxLength = MAX_USERID_LEN;
                        this.TextBox_IDNumber.Text = "";
                        this.PasswordBox_Password.Password = "";
                        this.TextBlock_Name.Text = "";
                        this.TextBlock_Log.Text = LOGIN_STEP_0;
                    }
                    break;
                case LOGIN_STATUS.en_Auth:
                    {
                        this.TextBox_IDNumber.IsEnabled = true;
                        this.PasswordBox_Password.IsEnabled = true;
                        this.Combobox_Team.IsEnabled = true;

                        this.Btn_Login.IsEnabled = false;
                        this.Btn_Login.Visibility = System.Windows.Visibility.Visible;
                        this.Btn_Logout.IsEnabled = false;
                        this.Btn_Logout.Visibility = System.Windows.Visibility.Hidden;

                        this.Btn_Connect.IsEnabled = false;
                    }
                    break;
                case LOGIN_STATUS.en_LogIn:
                    {
                        this.TextBox_IDNumber.IsEnabled = false;
                        this.PasswordBox_Password.IsEnabled = false;
                        this.Combobox_Team.IsEnabled = false;

                        this.Btn_Login.IsEnabled = false;
                        this.Btn_Login.Visibility = System.Windows.Visibility.Hidden;
                        this.Btn_Logout.IsEnabled = true;
                        this.Btn_Logout.Visibility = System.Windows.Visibility.Visible;

                        this.Btn_Connect.IsEnabled = true;
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

            TextBox_IDNumber.MaxLength = MAX_USERID_LEN;
            PasswordBox_Password.MaxLength = MAX_USERID_LEN;
            TextBox_IDNumber.Text = "";
            PasswordBox_Password.Password = "";
            TextBlock_Name.Text = "";
            Combobox_Team.Items.Clear();
            Btn_Login.Visibility = System.Windows.Visibility.Visible;
            Btn_Logout.Visibility = System.Windows.Visibility.Hidden;
            TextBlock_Log.Text = LOGIN_STEP_0;
            Btn_Login.IsEnabled = false;
            Btn_Connect.IsEnabled = false;
            isReceivedDeviceInfo = false;
            this.Focusable = true;
            this.Focus();
        }

        // Check Log-In Button Availablity 
        private Boolean CanLoginImageAvaliable()
        {
            Boolean ret = false;

            if (isReceivedDeviceInfo)
                return true;

            TextBlock_Log.Text = LOGIN_STEP_0;

            if (TextBox_IDNumber.Text.Length >= MAX_USERID_LEN)
            {
                TextBlock_Log.Text = LOGIN_STEP_1;

                if (PasswordBox_Password.Password.Length >= MAX_USERID_LEN)
                {
                    TextBlock_Log.Text = LOGIN_STEP_1;
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
            if (this.TextBox_IDNumber.Text.Length == 6)  // 6 is Fixed LogIn ID Length
            {
                if (App.STANDALONE_MODE)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive();
                    value.bIsOn = true;
                    value.GroupListSeperator = "RMG Driver";
                    value.Notice = LOGIN_STEP_1;
                    ProcessByGetAccessRoleCallback(value);

                    return;
                }

                VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send roleRq = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send();
                roleRq.UserID = this.TextBox_IDNumber.Text;

                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "UserAccesRole_Ask"), roleRq);

                VMT_Data_JAT2.VMT_DataMgr_Common.UserAccesRole_Ask(ref roleRq);
                PresentationMgr.AppWin.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
            }

            if (CanLoginImageAvaliable())
                Btn_Login.IsEnabled = true;
            else
                Btn_Login.IsEnabled = false;
        }

        private void TextBox_IDNumber_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.TextBox_IDNumber);
            this.TextBox_IDNumber.Focus();
        }

        private void TextBox_IDNumber_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.TextBox_IDNumber);
            this.TextBox_IDNumber.Focus();
        }
        #endregion [TextBox ID Event]

        #region [TextBox Password Event]
        private void PasswordBox_Password_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CanLoginImageAvaliable())
                Btn_Login.IsEnabled = true;
            else
                Btn_Login.IsEnabled = false;
        }

        private void PasswordBox_Password_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.PasswordBox_Password);
            this.PasswordBox_Password.Focus();
        }

        private void PasswordBox_Password_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.PasswordBox_Password);
            this.PasswordBox_Password.Focus();
        }
        #endregion [TextBox Password Event]

        #region [Button Day Event]
        private void Btn_Day_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme != PresentationMgr.UITheme.UITheme_Day)
                PresentationMgr.Singleton.CurrentUITheme = PresentationMgr.UITheme.UITheme_Day;

        }
        #endregion [Button Day Event]

        #region [Button Night Event]
        private void Btn_Night_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme != PresentationMgr.UITheme.UITheme_Night)
                PresentationMgr.Singleton.CurrentUITheme = PresentationMgr.UITheme.UITheme_Night;
        }
        #endregion [Button Night Event]

        #region [Button Reset Event]
        private void Btn_Reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetIDAndPassword();
        }

        public void ResetIDAndPassword()
        {
            // User ID should be logout to be reset.
            VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
            VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);

            // LogIn Step (No Authorized)
            this.UpdateLogInStep(LOGIN_STATUS.en_NoAuth);

            ReFlash();
        }
        #endregion [Button Reset Event]

        #region [Button LogIn Event]
        private void Btn_Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (App.STANDALONE_MODE)
            {
                if (isReceivedDeviceInfo)
                {
                    TextBox_IDNumber.Text = "";
                    VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                    this.UpdateLogInStep(LOGIN_STATUS.en_NoAuth);
                    ReFlash();
                    //PresentationMgr.AppWin.LogOut(this);
                }
                else
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive();
                    value.iLogin = 1;
                    value.UserName = "M.Jordan";
                    value.Notice = LOGIN_STEP_3;
                    ProcessByLogin4MachineCallback(value);
                    PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
                    return;
                }
            }

            if (isReceivedDeviceInfo)
            {
                // anything?
                Btn_Login.IsEnabled = true;
                //PresentationMgr.AppWin.PopupView.ShowPopup(2, "Log Off", "Are you sure wish to log-off?", "Cancel", "", "OK", mMainWindow.MainView.CallBack_Popup_LogOff, 0);
            }
            else
            {
                if (PresentationMgr.AppWin.gIsServerConnected == false)
                {
                    //PresentationMgr.AppWin.PopupView.ShowPopup(0, "Notice", "Network Disconnected", "", "", "", null, 3);
                    TextBlock_Log.Text = LOGIN_FAIL_WIFI;

                    return;
                }

                if (CanLoginImageAvaliable())
                {
                    Btn_Login.IsEnabled = true;
                    PresentationMgr.AppWin.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);

                    VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send();
                    value.UserID = TextBox_IDNumber.Text;
                    value.UserPW = PasswordBox_Password.Password;
                    value.GroupName = Combobox_Team.SelectedItem.ToString();
                    value.MchnID = RMG.RMG_User.gMchnID;
                    value.MchnTp = RMG.RMG_User.gMchnTp;

                    InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "Login4Machine_Ask"), value);

                    VMT_Data_JAT2.VMT_DataMgr_Common.Login4Machine_Ask(ref value);
                    PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
                }
                else
                    Btn_Login.IsEnabled = false;
            }
        }
        #endregion [Button LogIn Event]

        #region [Button LogOut Event]
        private void Btn_Logout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox_IDNumber.Text = "";
            VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
            VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
            this.UpdateLogInStep(LOGIN_STATUS.en_NoAuth);
            ReFlash();
            //PresentationMgr.AppWin.LogOut(this);
        }
        #endregion [Button LogOut Event]

        #region [Button Connect Event]
        private void Btn_Connect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (App.STANDALONE_MODE)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive();
                ProcessByMachineStatusChangeCallback(value);
                return;
            }

            if (isReceivedDeviceInfo)
            {
                if (PresentationMgr.AppWin.gIsServerConnected == false)
                {
                    //PresentationMgr.AppWin.PopupView.ShowPopup(0, "Notice", "Network Disconnected", "", "", "", null, 3);
                    return;
                }

                Btn_Connect.IsEnabled = true;

                // Connection Complete
                VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send sendMachineStatusChange = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send();
                sendMachineStatusChange.m_MchnID = RMG.RMG_User.gMchnID;
                sendMachineStatusChange.m_bisON = true;
                sendMachineStatusChange.m_buseRemark = true;
                sendMachineStatusChange.m_remark = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "SendMachineStatusChanged_Ask"), sendMachineStatusChange);

                VMT_Data_JAT2.VMT_DataMgr_Common.SendMachineStatusChanged_Ask(ref sendMachineStatusChange);

                //VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive statusChange = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive();
                //ProcessByMachineStatusChangeCallback(statusChange);
                //mMainWindow.ShowProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);

                PresentationMgr.AppWin.UC_IndicatorView.TextBox_UserID.Text = RMG.RMG_User.gUserID;
                PresentationMgr.Singleton.JOB_Clear();
                PresentationMgr.Singleton.ClearData();
                VMT_DataMgr.MainView_Init(MainWindow.SERVICE_COMPANY);
            }
        }
        #endregion [Button Connect Event]

        private void Btn_Setting_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MachineSettingView);
        }

        #endregion [UI Login Event]

        #region [Process Callback]
        ///////////////////////////////////////////
        //------------------Process Callback
        ///////////////////////////////////////////
        #region [GetAccessRole Callback]
        public void ProcessByGetAccessRoleCallback(VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value)
        {
            // Clear Team List
            Combobox_Team.Items.Clear();

            // TODO
            //if( value.usrGrp.Count == 1)
            //{
            //    Label_Team.Text = value.usrGrp[0] as String;
            //}
            //else if( value.usrGrp.Count > 1)
            //{
            //    // Show Team Select Popup
            //}

            if (value.GroupListSeperator is String)
            {
                String[] teamNameList = value.GroupListSeperator.Split('|');
                foreach (String teamName in teamNameList)
                {
                    Combobox_Team.Items.Add(teamName);
                }
            }
            else
            {
                foreach (Object grpName in value.GroupListSeperator)
                {
                    String strGrpName = grpName as String;
                    Combobox_Team.Items.Add(strGrpName);
                }
            }

            Combobox_Team.SelectedIndex = 0;

            if (value.Notice.Length > 0)
                TextBlock_Log.Text = value.Notice;

            // LogIn Step (Authorized)
            this.UpdateLogInStep(LOGIN_STATUS.en_Auth);

            PresentationMgr.AppWin.HideProgressBar(MainWindow.MESSAGE_TO_EEVMT_LOGIN);
        }
        #endregion [GetAccessRole Callback]

        #region [DiviceInfo Callback]
        public void ProcessByLogin4MachineCallback(VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value)
        {
            // TextBlock_Log.
            mIsLogin = true;
            RMG.RMG_User.gUserID = TextBox_IDNumber.Text;
            RMG.RMG_User.gUserPW = PasswordBox_Password.Password;
            RMG.RMG_User.gUserNm = value.UserName;

            //PresentationMgr.Singleton.UserID = RMG.RMG_User.gUserID;
            //PresentationMgr.Singleton.UserPW = RMG.RMG_User.gUserPW;

            isReceivedDeviceInfo = true;
            TextBlock_Name.Text = value.UserName;

            Btn_Login.Visibility = System.Windows.Visibility.Hidden;
            Btn_Logout.Visibility = System.Windows.Visibility.Visible;

            Btn_Connect.IsEnabled = true;

            if (value.Notice.Length > 5)
                TextBlock_Log.Text = value.Notice;
            else
            {
                TextBlock_Log.Text = LOGIN_STEP_3 + "\n\nWelcome " + TextBlock_Name.Text + ", " + RMG.RMG_User.gMchnID; ;
            }

            PresentationMgr.UseCorrection = true;

            this.UpdateLogInStep(LOGIN_STATUS.en_LogIn);
        }
        #endregion [DiviceInfo Callback]

        #region [MachineStatusChange Callback]
        public void ProcessByMachineStatusChangeCallback(VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value)
        {
            if (value.m_iResult == 1)
            {
                if (this.isReceivedDeviceInfo)
                {
                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.MainView;
                    // Start BlockBay Inventory Retrieving Thread
                    PresentationMgr.Singleton.StartUpdateThread();
                    VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineList4LogoutCheck);                    
                    MainWindow.IsMachineLogOn = true;
                }
                else
                {
                    PresentationMgr.Singleton.StopUpdateThread();
                    // LogIn Step (No Authorized)
                    //PresentationMgr.AppWin.UC_LogInView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                    PresentationMgr.Singleton.CurrentUIMode = PresentationMgr.UIMode.LogInView;
                    MainWindow.IsMachineLogOn = false;
                }
            }
        }
        #endregion [MachineStatusChange Callback]
        #endregion [Process Callback]
    }
}