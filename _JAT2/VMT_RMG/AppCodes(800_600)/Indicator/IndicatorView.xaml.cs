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

namespace VMT_RMG_800by600
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
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string[] vn = ver.Split('.');
            this.TextBox_Version.Text = "Software Version " + vn[0] + "." + vn[1] + "." + vn[2] + "." + vn[3];

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
            this.CheckBox_Wifi.Click += new RoutedEventHandler(CheckBox_Wifi_Click);
            this.CheckBox_Siemens.Click += new RoutedEventHandler(CheckBox_Siemens_Click);

            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);
        }

        private void Btn_GoToMain_Click(object sender, RoutedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.LogInView)
                return;
            else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.MachineSettingView)
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.LogInView);
            else
            {
                if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerDetailView &&
                    PresentationMgr.Singleton.PrevUIMode == PresentationMgr.UIMode.ContainerSearch)
                    PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.ContainerSearch);
                else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.BlockSelectionView &&
                    PresentationMgr.Singleton.PrevUIMode == PresentationMgr.UIMode.ContainerSearch)
                    PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.ContainerSearch);
                else
                {
                    if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerSearch)
                        PresentationMgr.Singleton.SetCorrectionSelect(null, String.Empty, String.Empty);

                    PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);
                }
            }
        }

        private void Btn_Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.LogInView)
            {
                String popupButtons = "Cancel" + "," + "System Off" + "," + "Terminate Application";
                //String popupMsg = "Are you sure want to close the application?";
                String popupMsg = "Are you sure wish to close?";
                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, "System Off",
                    popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallbackClosePopup), 0);
            }
            else
            {
                String popupButtons = "Cancel" + "," + "OK";
                String popupMsg = "Are you sure wish to log-off?";
                PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, "Log Off",
                    popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallbackLogOffPopup), 0);
            }
        }

        private void CheckBox_Power_Click(object sender, RoutedEventArgs e)
        {
            //String popupButtons = "Cancel" + "," + "OK";
            //String popupMsg = "Are you sure wish to log-off?";
            //PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, "Log Off",
            //    popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallbackLogOffPopup), 0);
        }

        private void CheckBox_Wifi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Siemens_Click(object sender, RoutedEventArgs e)
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
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                        System.Diagnostics.Process.Start("shutdown.exe", "-s -f");
                    }
                    break;
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonRight:
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
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
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickTwoButtonLeft:
                    break;
                case UC_PopupView.UC_PopupViewRetType.UC_PopupViewRetType_ClickTwoButtonRight:
                    {
                        // User ID should be logout to be reset.
                        PresentationMgr.AppWin.UC_LogInView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);
                        PresentationMgr.AppWin.ShowProgressBar(0);

                        VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                        VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
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
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Back_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Power,
                    UIThemeMgr.Day.IndicatorView_ButtonPowerDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonPowerDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_power_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_power_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Wifi,
                    UIThemeMgr.Day.IndicatorView_ButtonWifiDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonWifiDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_wifi_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_wifi_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Siemens,
                    UIThemeMgr.Day.IndicatorView_ButtonSiemensDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonSiemensDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_giemems_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_giemems_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                    UIThemeMgr.Day.IndicatorView_ButtonLogoutDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonLogoutPressImage, UIThemeMgr.Day.IndicatorView_ButtonLogoutDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Logout_Press_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Logout,
                //   new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_logout_default.png", UriKind.Relative)),
                //   new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_logout_press.png", UriKind.Relative))
                //   );

            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_GoToMain,
                    UIThemeMgr.Night.IndicatorView_ButtonBackDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonBackPressImage, UIThemeMgr.Night.IndicatorView_ButtonBackDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Back_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Power,
                    UIThemeMgr.Night.IndicatorView_ButtonPowerDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonPowerDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_power_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_power_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Wifi,
                    UIThemeMgr.Night.IndicatorView_ButtonWifiDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonWifiDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_wifi_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_wifi_disable_1.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinCheckBox(this.CheckBox_Siemens,
                    UIThemeMgr.Night.IndicatorView_ButtonSiemensDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonSiemensDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_giemems_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_giemems_disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                    UIThemeMgr.Night.IndicatorView_ButtonLogoutDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonLogoutPressImage, UIThemeMgr.Night.IndicatorView_ButtonLogoutDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Press_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Logout,
                //   new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_logout_default.png", UriKind.Relative)),
                //   new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_logout_press.png", UriKind.Relative))
                //   );
            }
        }


        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Window win = PresentationMgr.AppWin;
            if (win != null)
            {
                win.DragMove();
            }
        }



    }
}