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
using System.Reflection;

//20190108
using Common.Interface;
using System.Diagnostics;
using VMT_Data_JAT2.Objects;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for IndicatorView.xaml
    /// </summary>
    public partial class IndicatorView : UserControl
    {
        public IndicatorView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            // Set Current Version
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string ver = fvi.FileVersion;
            string[] vn = ver.Split('.');
            this.TextBox_Version.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0052", LanguageService.MESSAGE_GROUP) + " " + vn[0] + "." + vn[1] + "." + vn[2];

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~IndicatorView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Set Skin Image
            InitSkinImage();

            this.Btn_GoToMain.Click += new RoutedEventHandler(Btn_GoToMain_Click);

            this.CheckBox_Power.Click += new RoutedEventHandler(CheckBox_Power_Click);
            this.Image_Setting.MouseLeftButtonDown += new MouseButtonEventHandler(Image_Setting_MouseLeftButtonDown);
            this.CheckBox_Wifi.Click += new RoutedEventHandler(CheckBox_Wifi_Click);

            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);

            this.Label_UserID.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0041", LanguageService.MESSAGE_GROUP);
            this.Label_JobCount.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0059", LanguageService.LABEL_MAINWINDOW);
            this.Label_WS.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0079", LanguageService.LABEL_MAINWINDOW);
        }

        private void Btn_GoToMain_Click(object sender, RoutedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.LogInView)
                return;
            else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.MachineSettingView)
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.LogInView);
            else
            {
                if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerDetailView)
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevUIMode);
                else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BlockSelectionView || PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BaySelectionView)
                {
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevBlckBayUIMode);
                    PresentationMgr.Singleton.CurrentPostion = PresentationMgr.MainView.prevPos;
                }
                else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BlockSelectionView1 || PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BaySelectionView1
                    || PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BlockSelectionView2 || PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BaySelectionView2)
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerMoving);
                else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.SwapView &&
                    PresentationMgr.Singleton.PrevUIMode == PresentationMgr.UIMode.ContainerSearch)
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerSearch);
                else
                {
                    /*if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerSearch)
                        PresentationMgr.Singleton.SetCorrectionSelect(null, String.Empty, String.Empty);
                    */ //20200207 why remove position?
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
                }
            }
        }

        private void Btn_Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string buttonCacel = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0030", LanguageService.MESSAGE_GROUP);
            if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.LogInView)
            {
                string buttonSystemOff = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0031", LanguageService.MESSAGE_GROUP);
                string buttonTerminate = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0032", LanguageService.MESSAGE_GROUP);
                string popupButtons = buttonCacel + "," + buttonSystemOff + "," + buttonTerminate;
                //String popupMsg = "Are you sure want to close the application?";
                string popupMsg = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0033", LanguageService.MESSAGE_GROUP);
                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, buttonSystemOff,
                    popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallbackClosePopup), 0);
            }
            else
            {
                string buttonSystemOff = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0031", LanguageService.MESSAGE_GROUP);
                string buttonLogOut = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0060", LanguageService.LABEL_LOGIN);
                string popupButtons = buttonCacel + "," + buttonSystemOff + "," + buttonLogOut;
                //string buttonOK = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP);
                string popupMsg = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0033", LanguageService.MESSAGE_GROUP);
                //string labelPopup = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0036", LanguageService.MESSAGE_GROUP);
                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, buttonSystemOff,
                    popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallbackLogOffPopup), 0);
            }
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
                    this.TextBox_Version.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0054", LanguageService.MESSAGE_GROUP);
                    this.TextBox_Version.Foreground = Brushes.Red;
                }
                else
                {
                    this.Prgb_Download.Visibility = Visibility.Visible;
                    this.TextBox_Version.Visibility = Visibility.Hidden;
                }
            }
        }
        private void CheckBox_Power_Click(object sender, RoutedEventArgs e)
        {
            //String popupButtons = "Cancel" + "," + "OK";
            //String popupMsg = "Are you sure wish to log-off?";
            //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, "Log Off",
            //    popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallbackLogOffPopup), 0);
        }
        private void Image_Setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MachineSettingView);
        }
        private void CheckBox_Wifi_Click(object sender, RoutedEventArgs e)
        {

        }

        public void CallbackClosePopup(UC_PopupView.UC_PopupViewRetType selected)
        {
            switch (selected)
            {
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonLeft:
                    break;
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonCenter:
                    {
                        MainWindow.dtClockTime.Stop();
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
                        {
                            PresentationMgr.AppWin.PLCTimer.Stop();
                        }
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                        System.Diagnostics.Process.Start("shutdown.exe", "-s -f");
                    }
                    break;
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonRight:
                    {
                        MainWindow.dtClockTime.Stop();
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
                        {
                            PresentationMgr.AppWin.PLCTimer.Stop();
                        }
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                        PresentationMgr.APP_CloseApp();
                    }
                    break;
                default:
                    break;
            }
        }

        public void CallbackLogOffPopup(UC_PopupView.UC_PopupViewRetType selected)
        {
            switch (selected)
            {
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonLeft:
                    break;
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonCenter:
                    {
                        String title = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_POPUPOUT);
                        String message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0008", LanguageService.LABEL_POPUPOUT);
                        String button = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_POPUPOUT);
                        PresentationMgr.AppWin.UC_PopupOutView.ShowPopup(title, message, button);
                    }
                    break;
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonRight:
                    {
                        String title = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_POPUPOUT);
                        String message = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0005", LanguageService.LABEL_POPUPOUT);
                        String button = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_POPUPOUT);
                        PresentationMgr.AppWin.UC_PopupOutView.ShowPopup(title, message, button);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_GoToMain,
                    UIThemeMgr.Day.IndicatorView_ButtonBackDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonBackPressImage, UIThemeMgr.Day.IndicatorView_ButtonBackDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_Back_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Power,
                    UIThemeMgr.Day.IndicatorView_ButtonPowerDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonPowerDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_power_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_power_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Wifi,
                    UIThemeMgr.Day.IndicatorView_ButtonWifiDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonWifiDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_wifi_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_wifi_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_AutoFlg,
                    UIThemeMgr.Day.IndicatorView_ButtonSiemensDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonSiemensDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_giemems_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_giemems_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                    UIThemeMgr.Day.IndicatorView_ButtonLogoutDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonLogoutPressImage, UIThemeMgr.Day.IndicatorView_ButtonLogoutDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_Logout_Press_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Logout,
                //   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_logout_default.png", UriKind.Relative)),
                //   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_logout_press.png", UriKind.Relative))
                //   );

            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_GoToMain,
                    UIThemeMgr.Night.IndicatorView_ButtonBackDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonBackPressImage, UIThemeMgr.Night.IndicatorView_ButtonBackDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_Back_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Power,
                    UIThemeMgr.Night.IndicatorView_ButtonPowerDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonPowerDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_power_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_power_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Wifi,
                    UIThemeMgr.Night.IndicatorView_ButtonWifiDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonWifiDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_wifi_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_wifi_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_AutoFlg,
                    UIThemeMgr.Night.IndicatorView_ButtonSiemensDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonSiemensDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_giemems_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_giemems_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                    UIThemeMgr.Night.IndicatorView_ButtonLogoutDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonLogoutPressImage, UIThemeMgr.Night.IndicatorView_ButtonLogoutDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Press_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Logout,
                //   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_logout_default.png", UriKind.Relative)),
                //   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/IndicatorView/IndicatorView_logout_press.png", UriKind.Relative))
                //   );
            }
        }


        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int count;
            if (App.TEST_MODE)
                count = e.ClickCount;
            
            Window win = PresentationMgr.AppWin;
            if (win != null)
            {
                win.DragMove();
            }            
        }

        private int mHiddenMenuDownCount = 0;
        private void Image_wifi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.LogInView)
            //    return;

            //if (++mHiddenMenuDownCount > 5)
            //{
            //    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MachineSettingView);
            //    mHiddenMenuDownCount = 0;
            //}
        }

        private void sliderColor_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            setBrightnessAdjustment();
        }

        private void sliderColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            setBrightnessAdjustment();
        }

        private void setBrightnessAdjustment()
        {
            MainWindow mainWin = (MainWindow)Window.GetWindow(this);
            
            double sliderValue = ((int)sliderColor.Value) - 5;

            SolidColorBrush bgColor = new SolidColorBrush(sliderValue > -1 ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 0, 0, 0));

            sliderValue = 1 - ((Math.Abs(sliderValue) * 1.5) / 10);
            LayoutRoot.Opacity = sliderValue;
            mainWin.LayoutRoot.Opacity = sliderValue;
            mainWin.MainWin.Background = bgColor;
        }
    }
}