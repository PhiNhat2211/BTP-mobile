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
using System.ComponentModel;

//20190108
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for ContainerDetailView.xaml
    /// </summary>
    public partial class ContainerDetailView : UserControl
    {   
        private String _targetJobKey = String.Empty;

        private enum SelectedGrid
        {
            SelectedGrid_Unknown,
            SelectedGrid_TargetViewGrid,
            SelectedGrid_TwinGrid
        }

        private SelectedGrid _SelectedGrid = SelectedGrid.SelectedGrid_Unknown;

        public ContainerDetailView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
                new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
        }

        ~ContainerDetailView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_Container_Target.Click += new RoutedEventHandler(Btn_Container_Target_Click);
            this.Btn_Container_Twin.Click += new RoutedEventHandler(Btn_Container_Twin_Click);

            this.Btn_Save.Click += new RoutedEventHandler(Btn_Save_Click);

            this.Btn_Detwin.Click += new RoutedEventHandler(Btn_Detwin_Click);
            this.Btn_Save_Twin.Click += new RoutedEventHandler(Btn_Save_Twin_Click);            

            this.Grid_ContainerNumber_Twin.Visibility = System.Windows.Visibility.Hidden;

            this.InitFirstContainerInfo();
            this.InitSecondContainerInfo();

            this.Grid_Bottom_Twin.Visibility = System.Windows.Visibility.Hidden;

            LoadLanguage();
        }

        private void LoadLanguage()
        {
            this.TextBlock_First_Class.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0040", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_ISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0041", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_Operator.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0044", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_FM.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0043", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_Damage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0047", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_WGT.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0045", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_CurLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0049", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_rmk.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0058", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_PlanLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0050", LanguageService.LABEL_CONTAINERDETAIL);

            this.TextBlock_First_GroupCode.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0069", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_RFTemp.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0070", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_RFPlug.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0071", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_DGClass.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0072", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_Booking.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0073", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_SDay.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0074", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_DSVSL.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0075", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_LDVSL.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0076", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_POD.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0057", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_OVD.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0077", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_H.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0078", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_L.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0079", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_R.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0080", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_F.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0081", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_A.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0082", LanguageService.LABEL_CONTAINERDETAIL);

            this.TextBlock_First_Repair.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0083", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_Period.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0084", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_In.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0085", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_First_Out.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0086", LanguageService.LABEL_CONTAINERDETAIL);

            this.Btn_Save.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0087", LanguageService.LABEL_CONTAINERDETAIL);
            this.Btn_Save_Twin.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0087", LanguageService.LABEL_CONTAINERDETAIL);
            this.Btn_Detwin.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0106", LanguageService.LABEL_CUSTOMIZE);

            this.TextBlock_Second_Class.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0040", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_ISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0041", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_Operator.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0044", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_FM.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0043", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_Damage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0047", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_WGT.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0045", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_CurLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0049", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_PlanLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0050", LanguageService.LABEL_CONTAINERDETAIL);
            
            this.TextBlock_Second_GroupCode.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0069", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_RFTemp.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0070", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_RFPlug.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0071", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_DGClass.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0072", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_Booking.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0073", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_SDay.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0074", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_DSVSL.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0075", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_LDVSL.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0076", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_POD.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0057", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_OVD.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0077", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_H.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0078", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_L.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0079", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_R.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0080", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_F.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0081", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_A.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0082", LanguageService.LABEL_CONTAINERDETAIL);

            this.TextBlock_Second_Repair.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0083", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_Period.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0084", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_In.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0085", LanguageService.LABEL_CONTAINERDETAIL);
            this.TextBlock_Second_Out.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0086", LanguageService.LABEL_CONTAINERDETAIL);
        }

        private void InitFirstContainerInfo()
        {
            this.Btn_Container.Content = String.Empty;
            this.Btn_Container_Target.Content = String.Empty;

            this.TextBox_First_Class.Text = String.Empty;
            this.TextBox_First_ISO.Text = String.Empty;
            this.TextBox_First_Operator.Text = String.Empty;
            this.TextBox_First_FM.Text = String.Empty;
            this.TextBox_First_Damage.Text = String.Empty;
            this.TextBox_First_WGT.Text = String.Empty;
            this.TextBox_First_CurLoc.Text = String.Empty;
            this.TextBox_First_rmk.Text = String.Empty;
            this.TextBox_First_PlanLoc.Text = String.Empty;

            this.TextBox_First_GroupCode.Text = String.Empty;
            this.TextBox_First_RFTemp.Text = String.Empty;
            this.TextBox_First_RFPlug.Text = String.Empty;
            this.TextBox_First_DGClass.Text = String.Empty;
            this.TextBox_First_Booking.Text = String.Empty;
            this.TextBox_First_SDay.Text = String.Empty;
            this.TextBox_First_DSVSL.Text = String.Empty;
            this.TextBox_First_LDVSL.Text = String.Empty;
            this.TextBox_First_POD.Text = String.Empty;
            this.TextBox_First_H.Text = String.Empty;
            this.TextBox_First_L.Text = String.Empty;
            this.TextBox_First_R.Text = String.Empty;
            this.TextBox_First_F.Text = String.Empty;
            this.TextBox_First_A.Text = String.Empty;

        }

        private void InitSecondContainerInfo()
        {
            this.Btn_Container_Twin.Content = String.Empty;

            this.TextBox_Second_Class.Text = String.Empty;
            this.TextBox_Second_ISO.Text = String.Empty;
            this.TextBox_Second_Operator.Text = String.Empty;
            this.TextBox_Second_FM.Text = String.Empty;
            this.TextBox_Second_Damage.Text = String.Empty;
            this.TextBox_Second_WGT.Text = String.Empty;
            this.TextBox_Second_CurLoc.Text = String.Empty;
            this.TextBox_Second_PlanLoc.Text = String.Empty;

            this.TextBox_Second_GroupCode.Text = String.Empty;
            this.TextBox_Second_RFTemp.Text = String.Empty;
            this.TextBox_Second_RFPlug.Text = String.Empty;
            this.TextBox_Second_DGClass.Text = String.Empty;
            this.TextBox_Second_Booking.Text = String.Empty;
            this.TextBox_Second_SDay.Text = String.Empty;
            this.TextBox_Second_DSVSL.Text = String.Empty;
            this.TextBox_Second_LDVSL.Text = String.Empty;
            this.TextBox_Second_POD.Text = String.Empty;
            this.TextBox_Second_H.Text = String.Empty;
            this.TextBox_Second_L.Text = String.Empty;
            this.TextBox_Second_R.Text = String.Empty;
            this.TextBox_Second_F.Text = String.Empty;
            this.TextBox_Second_A.Text = String.Empty;

        }

        public void Btn_Container_Target_Click(object sender, RoutedEventArgs e)
        {
            _SelectedGrid = SelectedGrid.SelectedGrid_TargetViewGrid;

            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }
            else
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }

            //Btn_Container_Target.IsFocused = true;
            //Btn_Container_Twin.IsFocused = false;

            Grid_First_Container.Visibility = Visibility.Visible;
            Grid_Second_Container.Visibility = Visibility.Hidden;
        }

        public void Btn_Container_Twin_Click(object sender, RoutedEventArgs e)
        {
            _SelectedGrid = SelectedGrid.SelectedGrid_TwinGrid;

            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }
            
            //Btn_Container_Target.IsFocused = false;
            //Btn_Container_Twin.IsFocused = true;

            Grid_First_Container.Visibility = Visibility.Hidden;
            Grid_Second_Container.Visibility = Visibility.Visible;
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerDetailView &&
                    PresentationMgr.Singleton.PrevUIMode == PresentationMgr.UIMode.ContainerSearch)
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerSearch);
            else
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.Singleton.PrevUIMode);
        }

        private void Btn_Detwin_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (!String.IsNullOrEmpty(_targetJobKey))
            {
                PresentationMgr.AppWin.ShowProgressBar(0);

                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "SetDetwinJob_Ask"), _targetJobKey);

                VMT_Data_JAT2.VMT_DataMgr_RMG.SetDetwinJob_Ask(_targetJobKey);
            }
        }

        private void Btn_Save_Twin_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                //PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                //        new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //        new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //        new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                //PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                if (_SelectedGrid == SelectedGrid.SelectedGrid_TargetViewGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                    //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }
                else if (_SelectedGrid == SelectedGrid.SelectedGrid_TwinGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }

                PresentationMgr.SetSkinButton(this.Btn_Save,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Detwin,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Save_Twin,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                //PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                //PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                if (_SelectedGrid == SelectedGrid.SelectedGrid_TargetViewGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                    //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }
                else if (_SelectedGrid == SelectedGrid.SelectedGrid_TwinGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }

                PresentationMgr.SetSkinButton(this.Btn_Save,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Detwin,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Save_Twin,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public void RMG_Member_PropertyChanged_TargetJobKey(object sender, PropertyChangedEventArgs e)
        {            
            //this.RefreshContainerInfo();
        }

        public void RefreshContainerInfo(String cntrNo, bool? isTwin = null)
        {   
            if (isTwin == null)
            {
                this.Grid_ContainerNumber_Twin.Visibility = System.Windows.Visibility.Hidden;
                this.Grid_Bottom_Twin.Visibility = System.Windows.Visibility.Hidden;
                this.Btn_Container_Target_Click(null, null);
            }
            else
            {
                this.Grid_ContainerNumber_Twin.Visibility = System.Windows.Visibility.Visible;
                this.Grid_Bottom_Twin.Visibility = System.Windows.Visibility.Visible;
                if (isTwin == true)
                    this.Btn_Container_Twin_Click(null, null);
                else
                    this.Btn_Container_Target_Click(null, null);
            }

            //if (cntrNo.Equals(this.Btn_Container_Target.Content.ToString()))
            //    return;            
            
            InitFirstContainerInfo();
            InitSecondContainerInfo();

            var jobOrderList = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);
            this._targetJobKey = (jobOrderList == null || jobOrderList.Count <= 0 || jobOrderList.First() == null) ?
                String.Empty : jobOrderList.First().jobKey;

            Btn_Container.Content = cntrNo;
            Btn_Container_Target.Content = cntrNo;

            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "GetContainerInfo_Ask"), cntrNo);

            VMT_Data_JAT2.VMT_DataMgr_RMG.GetContainerInfo_Ask(cntrNo);
            PresentationMgr.AppWin.ShowProgressBar(0);
        }

        public void RefreshContainerInfo()
        {
            Grid_First_Container.Visibility = Visibility.Visible;
            Grid_Second_Container.Visibility = Visibility.Hidden;
            this.Btn_Container_Target_Click(null, null);            

            InitFirstContainerInfo();
            InitSecondContainerInfo();

            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (String.IsNullOrEmpty(jobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null || jobOrder.cntr == null)
                return;

            _targetJobKey = jobKey;

            Btn_Container.Content = jobOrder.cntr.cntrNo;
            Btn_Container_Target.Content = jobOrder.cntr.cntrNo;

            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "GetContainerInfo_Ask"), jobOrder.cntr.cntrNo);

            VMT_Data_JAT2.VMT_DataMgr_RMG.GetContainerInfo_Ask(jobOrder.cntr.cntrNo);
        }

        public void SetTargetJobContainerInfo(VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive containerInfo)
        {            
            if (containerInfo != null)
            {
                // set first container
                this.TextBox_First_Class.Text = containerInfo.cntr.cls;
                this.TextBox_First_ISO.Text = containerInfo.cntr.cntrIso;
                this.TextBox_First_Operator.Text = containerInfo.cntr.opr;
                this.TextBox_First_FM.Text = containerInfo.cntr.fullMty;
                //this.TextBox_First_Damage.Text = containerInfo.cntr.isDmg ? "D" : String.Empty; //20190705
                this.TextBox_First_Damage.Text = containerInfo.cntr.isDmg ? "Y" : "N";
                this.TextBox_First_WGT.Text = containerInfo.cntr.cntrWgt;
                this.TextBox_First_CurLoc.Text = containerInfo.fmLoc.location;
                this.TextBox_First_rmk.Text = containerInfo.rmk;
                this.TextBox_First_PlanLoc.Text = containerInfo.toLoc.location;    // plan location   

                this.TextBox_First_GroupCode.Text = containerInfo.cntr.groupCode;
                this.TextBox_First_RFTemp.Text = containerInfo.cntr.rfTemp;
                this.TextBox_First_RFPlug.Text = containerInfo.cntr.rfPlug;
                this.TextBox_First_DGClass.Text = containerInfo.cntr.imdgCd;
                this.TextBox_First_Booking.Text = containerInfo.cntr.bkgNo;
                this.TextBox_First_SDay.Text = containerInfo.cntr.stkDay;
                //this.TextBox_First_DSVSL.Text = containerInfo.cntr.inVsl;  //20190705
                //this.TextBox_First_LDVSL.Text = containerInfo.cntr.outVsl; //20190705
                this.TextBox_First_DSVSL.Text = containerInfo.inVsl.vessel;
                this.TextBox_First_LDVSL.Text = containerInfo.outVsl.vessel;
                this.TextBox_First_POD.Text = containerInfo.cntr.pod;
                if (!String.IsNullOrEmpty(containerInfo.cntr.overValue))
                {
                    String[] ovdList = containerInfo.cntr.overValue.Split('/');
                    this.TextBox_First_H.Text = (ovdList.Length >= 1) ? ovdList[0] : "";
                    this.TextBox_First_L.Text = (ovdList.Length >= 2) ? ovdList[1] : "";
                    this.TextBox_First_R.Text = (ovdList.Length >= 3) ? ovdList[2] : "";
                    this.TextBox_First_F.Text = (ovdList.Length >= 4) ? ovdList[3] : "";
                    this.TextBox_First_A.Text = (ovdList.Length >= 5) ? ovdList[4] : "";
                }

            }   

            // job 관련
            if (String.IsNullOrEmpty(_targetJobKey))
                return;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(_targetJobKey);
            if (jobOrder == null || jobOrder.type == null)
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twinJobOrder = null;
            //if (jobOrder.type.twinTandemFlg.Equals("W"))
            if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
            {
                this.Grid_ContainerNumber_Twin.Visibility = System.Windows.Visibility.Visible;
                this.Grid_Bottom_Twin.Visibility = System.Windows.Visibility.Visible;

                twinJobOrder = PresentationMgr.Singleton.JOB_Get(jobOrder.type.ycTwinKey);
                if (null != twinJobOrder && twinJobOrder.cntr != null)
                {
                    this.Btn_Container_Twin.Content = twinJobOrder.cntr.cntrNo;
                              
                    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        , "GetTwinContainerInfo_Ask"), twinJobOrder.cntr.cntrNo);

                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetTwinContainerInfo_Ask(twinJobOrder.cntr.cntrNo);
                }
            }
            else
            {
                this.Grid_ContainerNumber_Twin.Visibility = System.Windows.Visibility.Hidden;
                this.Grid_Bottom_Twin.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void SetTwinJobContainerInfo(VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive containerInfo)
        {
            if (containerInfo != null)
            {
                this.TextBox_Second_Class.Text = containerInfo.cntr.cls;
                this.TextBox_Second_ISO.Text = containerInfo.cntr.cntrIso;
                this.TextBox_Second_Operator.Text = containerInfo.cntr.opr;
                this.TextBox_Second_FM.Text = containerInfo.cntr.fullMty;
                this.TextBox_Second_Damage.Text = containerInfo.cntr.isDmg ? "D" : String.Empty;
                this.TextBox_Second_WGT.Text = containerInfo.cntr.cntrWgt;
                this.TextBox_Second_CurLoc.Text = containerInfo.fmLoc.location;
                this.TextBox_Second_PlanLoc.Text = containerInfo.toLoc.location;    // plan location            

                this.TextBox_Second_GroupCode.Text = containerInfo.cntr.groupCode;
                this.TextBox_Second_RFTemp.Text = containerInfo.cntr.rfTemp;
                this.TextBox_Second_RFPlug.Text = containerInfo.cntr.rfPlug;
                this.TextBox_Second_DGClass.Text = containerInfo.cntr.imdgCd;
                this.TextBox_Second_Booking.Text = containerInfo.cntr.bkgNo;
                this.TextBox_Second_SDay.Text = containerInfo.cntr.stkDay;
                this.TextBox_Second_DSVSL.Text = containerInfo.cntr.inVsl;
                this.TextBox_Second_LDVSL.Text = containerInfo.cntr.outVsl;
                this.TextBox_Second_POD.Text = containerInfo.cntr.pod;
                if (!String.IsNullOrEmpty(containerInfo.cntr.overValue))
                {
                    String[] ovdList = containerInfo.cntr.overValue.Split('/');
                    this.TextBox_Second_H.Text = (ovdList.Length >= 1) ? ovdList[0] : "";
                    this.TextBox_Second_L.Text = (ovdList.Length >= 2) ? ovdList[1] : "";
                    this.TextBox_Second_R.Text = (ovdList.Length >= 3) ? ovdList[2] : "";
                    this.TextBox_Second_F.Text = (ovdList.Length >= 4) ? ovdList[3] : "";
                    this.TextBox_Second_A.Text = (ovdList.Length >= 5) ? ovdList[4] : "";
                }
            }

            //  이미 셋팅되어 있다.
            //this.TextBox_Second_Status.Text = ""; // Job Description 표시(i.e. On chassis for delivery,etc)            
            //this.TextBox_Second_PlanSeq.Text = String.IsNullOrEmpty(jobOrder.taskId) ? "0" : jobOrder.taskId; 
            //this.TextBox_Second_Expected.Text = jobOrder.type.etw;
        }        
    }
}
