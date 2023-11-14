using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace VMT_RMG_800by600
{
    public enum AppTheme { Day, Night };

    public class UIThemeMgr
    {
        private AppTheme currentTheme;
        private Dictionary<AppTheme, ResourceDictionary> themeDic;
        private static readonly UIThemeMgr _theOnly = null;

        public class Day
        {
            // mainview
            public static readonly BitmapImage ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative));
            public static readonly BitmapImage ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_disable.png", UriKind.Relative));

            public static readonly BitmapImage ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ButtonDefaultImage_1 = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative));
            
            public static readonly BitmapImage JobList_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative));
            public static readonly BitmapImage JobList_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative));

            //
            public static readonly BitmapImage ButtonBlockOnImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Block_On.png", UriKind.Relative));
            public static readonly BitmapImage ButtonBlockOffImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Block_Off.png", UriKind.Relative));

            public static readonly BitmapImage ButtonNavigatorOnImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative));
            public static readonly BitmapImage ButtonNavigatorOffImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative));            

            // searchview
            public static readonly BitmapImage SearchView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative));
            public static readonly BitmapImage SearchView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative));
            public static readonly BitmapImage SearchView_ButtonDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative));

            // machine search view
            public static readonly BitmapImage MachineSearchView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage MachineSearchView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Down_Disable.png", UriKind.Relative));

            public static readonly BitmapImage MachineSearchView_ButtonSearchDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Search_Default.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonSearchPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Search_Press.png", UriKind.Relative));

            // container searchview
            public static readonly BitmapImage ContainerSearchView_TitleImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_ButtonSearchDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonSearchPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Search_Press.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/ContainerSearchView_Down_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_CheckboxSelectCont = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Checkbox_Select_Cont.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_CheckboxSelectITV = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Checkbox_Select_ITV.png", UriKind.Relative));            

            // selectionview
            public static readonly BitmapImage SelectionView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Press.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_BlockSelectionInfoImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionInfo.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBlockLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionView_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionView_Left_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockLeftDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionView_Left_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBlockRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionView_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionView_Right_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockRightDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BlockSelectionView_Right_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_BaySelectionInfoImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionInfo.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBayLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayLeftDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Left_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBayRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayRightDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/BaySelectionVeiw_Right_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Left_Press.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Right_Press.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SelectionView/Button_Down_Disable.png", UriKind.Relative));

            // indicator
            public static readonly BitmapImage IndicatorView_ButtonBackDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonBackPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Back_Press.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonPowerDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_power_default_1.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonPowerDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_power_disable_1.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonWifiDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_wifi_default_1.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonWifiDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_wifi_disable_1.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonSiemensDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_giemems_default.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonSiemensDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_giemems_disable.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonLogoutDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonLogoutPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/IndicatorView/IndicatorView_Logout_Press_1.png", UriKind.Relative));

            // login
            public static readonly BitmapImage LoginView_ButtonResetDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_reset_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonResetPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_reset_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonResetDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_reset_disable.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonLoginDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_login_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonLoginPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_login_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonLoginDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_login_disable.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonLogoutDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_logout_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonLogoutPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_logout_press.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonDayPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_day_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonDayDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_day_default.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonNightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_night_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonNightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_night_default.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonConnectDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_connect_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonConnectPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_connect_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonConnectDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/LogInView/LogInView_connect_disable.png", UriKind.Relative));

            // available
            public static readonly BitmapImage AvailableView_TitleImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Title_Available.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_TitleBreakTimeImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Title_BreakTime.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Button_Default.png", UriKind.Relative));
            public static readonly BitmapImage AvailableView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Button_Press.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_ButtonMenuDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Available_menu_default.png", UriKind.Relative));
            public static readonly BitmapImage AvailableView_ButtonMenuPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Available_menu_press.png", UriKind.Relative));

            // bayview
            public static readonly BitmapImage AvailableView_BGImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/BayView_BG.png", UriKind.Relative));

            // navigatorview
            public static readonly BitmapImage NavigatorView_ButtonLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Left_Press.png", UriKind.Relative));

            public static readonly BitmapImage NavigatorView_ButtonRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Right_Press.png", UriKind.Relative));

            public static readonly BitmapImage NavigatorView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Up_Press.png", UriKind.Relative));

            public static readonly BitmapImage NavigatorView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/NavigatorView/NavigatorView_Down_Press.png", UriKind.Relative));
        }

        public class Night
        {
            // mainview
            public static readonly BitmapImage ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_disable.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative));

            public static readonly BitmapImage ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ButtonDefaultImage_1 = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative));                       

            public static readonly BitmapImage JobList_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/Common/JobList/JobList_btn_type_press.png", UriKind.Relative));
            public static readonly BitmapImage JobList_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/Common/JobList/JobList_btn_type_default.png", UriKind.Relative));

            //
            public static readonly BitmapImage ButtonBlockOnImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Block_On.png", UriKind.Relative));
            public static readonly BitmapImage ButtonBlockOffImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Block_Off.png", UriKind.Relative));

            public static readonly BitmapImage ButtonNavigatorOnImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative));
            public static readonly BitmapImage ButtonNavigatorOffImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative));

            // searchview
            public static readonly BitmapImage SearchView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative));
            public static readonly BitmapImage SearchView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative));
            public static readonly BitmapImage SearchView_ButtonDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative));

            // machine search view
            public static readonly BitmapImage MachineSearchView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage MachineSearchView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Down_Disable.png", UriKind.Relative));

            public static readonly BitmapImage MachineSearchView_ButtonSearchDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Search_Default.png", UriKind.Relative));
            public static readonly BitmapImage MachineSearchView_ButtonSearchPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Search_Press.png", UriKind.Relative));

            // container searchview
            public static readonly BitmapImage ContainerSearchView_TitleImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchViewTitle.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_ButtonSearchDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Default.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonSearchPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Search_Press.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/ContainerSearchView_Down_Disable.png", UriKind.Relative));

            public static readonly BitmapImage ContainerSearchView_CheckboxSelectCont = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Checkbox_Select_Cont.png", UriKind.Relative));
            public static readonly BitmapImage ContainerSearchView_CheckboxSelectITV = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Checkbox_Select_ITV.png", UriKind.Relative));
            
            // selectionview
            public static readonly BitmapImage SelectionView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Press.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_BlockSelectionInfoImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionInfo.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBlockLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Left_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockLeftDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Left_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBlockRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Right_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBlockRightDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BlockSelectionView_Right_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_BaySelectionInfoImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionInfo.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBayLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayLeftDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Left_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonBayRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonBayRightDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/BaySelectionVeiw_Right_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Left_Press.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Right_Press.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Up_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonUpDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Up_Disable.png", UriKind.Relative));

            public static readonly BitmapImage SelectionView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Down_Press.png", UriKind.Relative));
            public static readonly BitmapImage SelectionView_ButtonDownDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SelectionView/Button_Down_Disable.png", UriKind.Relative));

            // indicator
            public static readonly BitmapImage IndicatorView_ButtonBackDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Back_Default.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonBackPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Back_Press.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonPowerDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_power_default_1.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonPowerDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_power_disable_1.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonWifiDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_wifi_default_1.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonWifiDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_wifi_disable_1.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonSiemensDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_giemems_default.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonSiemensDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_giemems_disable.png", UriKind.Relative));

            public static readonly BitmapImage IndicatorView_ButtonLogoutDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Default_1.png", UriKind.Relative));
            public static readonly BitmapImage IndicatorView_ButtonLogoutPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/IndicatorView/IndicatorView_Logout_Press_1.png", UriKind.Relative));

            // login
            public static readonly BitmapImage LoginView_ButtonResetDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_reset_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonResetPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_reset_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonResetDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_reset_disable.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonLoginDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_login_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonLoginPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_login_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonLoginDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_login_disable.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonLogoutDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_logout_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonLogoutPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_logout_press.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonDayPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_day_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonDayDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_day_default.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonNightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_night_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonNightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_night_default.png", UriKind.Relative));

            public static readonly BitmapImage LoginView_ButtonConnectDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_connect_default.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonConnectPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_connect_press.png", UriKind.Relative));
            public static readonly BitmapImage LoginView_ButtonConnectDisableImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/LogInView/LogInView_connect_disable.png", UriKind.Relative));

            // available
            public static readonly BitmapImage AvailableView_TitleImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Title_Available.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_TitleBreakTimeImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Title_BreakTime.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_ButtonDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Button_Default.png", UriKind.Relative));
            public static readonly BitmapImage AvailableView_ButtonPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Button_Press.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_ButtonMenuDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Available_menu_default.png", UriKind.Relative));
            public static readonly BitmapImage AvailableView_ButtonMenuPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Available_menu_press.png", UriKind.Relative));

            public static readonly BitmapImage AvailableView_BGImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/BayView_BG.png", UriKind.Relative));

            // navigatorview
            public static readonly BitmapImage NavigatorView_ButtonLeftDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonLeftPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Left_Press.png", UriKind.Relative));

            public static readonly BitmapImage NavigatorView_ButtonRightDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonRightPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Right_Press.png", UriKind.Relative));

            public static readonly BitmapImage NavigatorView_ButtonUpDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Up_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonUpPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Up_Press.png", UriKind.Relative));

            public static readonly BitmapImage NavigatorView_ButtonDownDefaultImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Down_Default.png", UriKind.Relative));
            public static readonly BitmapImage NavigatorView_ButtonDownPressImage = new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/NavigatorView/NavigatorView_Down_Press.png", UriKind.Relative));
        }

        public static SolidColorBrush LayoutRootSelectedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xA5, 0x0F));
        public static SolidColorBrush LayoutRootNormalBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xED, 0xF1, 0xF2));

        public static SolidColorBrush TextBlockSelectedForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        public static SolidColorBrush TextBlockNormalForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x48, 0x48));

        public AppTheme Theme
        {
            get { return currentTheme; }
            set
            {
                Application.Current.Resources.MergedDictionaries[0] = themeDic[value];
                currentTheme = value;
            }
        }

        public static UIThemeMgr Singleton
        {
            get { return _theOnly; }
        }

        static UIThemeMgr()
        {
            _theOnly = new UIThemeMgr();
        }

        private UIThemeMgr()
        {
            themeDic = new Dictionary<AppTheme, ResourceDictionary>();
            currentTheme = AppTheme.Day;

            // Initalize Theme Resource Data
            InitThemeResource();
        }

        private void InitThemeResource()
        {
            try
            {
                themeDic[AppTheme.Day] = Application.LoadComponent(
                            new Uri("/VMT_RMG;component/Resource_Dic/ResDic_RMG_Properties_Day.xaml",
                            UriKind.Relative)) as ResourceDictionary;

                themeDic[AppTheme.Night] = Application.LoadComponent(
                            new Uri("/VMT_RMG;component/Resource_Dic/ResDic_RMG_Properties_Night.xaml",
                            UriKind.Relative)) as ResourceDictionary;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

        }
    }
}
