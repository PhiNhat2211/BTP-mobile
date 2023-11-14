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
using VMT_RMG;

namespace VMT_RMG_800by600
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
        }

        private void InitFirstContainerInfo()
        {
            this.Btn_Container.Content = String.Empty;
            this.Btn_Container_Target.Content = String.Empty;

            this.TextBox_First_Class.Text = String.Empty;
            this.TextBox_First_ISO.Text = String.Empty;

            this.TextBox_First_FM.Text = String.Empty;
            this.TextBox_First_Operator.Text = String.Empty;
            this.TextBox_First_WGT.Text = String.Empty;
            this.TextBox_First_Location.Text = String.Empty;
            this.TextBox_First_POD.Text = String.Empty;
            this.TextBox_First_Commodity.Text = String.Empty;
            this.TextBox_First_Damage.Text = String.Empty;
            this.TextBox_First_Hold.Text = String.Empty;
            this.TextBox_First_PreLocation.Text = String.Empty;

            this.TextBox_First_DlsVSL.Text = String.Empty;
            this.TextBox_First_LoadVSL.Text = String.Empty;
            this.TextBox_First_InStowage.Text = String.Empty;
            this.TextBox_First_OutStowage.Text = String.Empty;

            this.TextBox_First_Status.Text = String.Empty;
            this.TextBox_First_PlanSeq.Text = String.Empty;
            this.TextBox_First_Expected.Text = String.Empty;
        }

        private void InitSecondContainerInfo()
        {
            this.Btn_Container_Twin.Content = String.Empty;

            this.TextBox_Second_Class.Text = String.Empty;
            this.TextBox_Second_ISO.Text = String.Empty;

            this.TextBox_Second_FM.Text = String.Empty;
            this.TextBox_Second_Operator.Text = String.Empty;
            this.TextBox_Second_WGT.Text = String.Empty;
            this.TextBox_Second_Location.Text = String.Empty;
            this.TextBox_Second_POD.Text = String.Empty;
            this.TextBox_Second_Commodity.Text = String.Empty;
            this.TextBox_Second_Damage.Text = String.Empty;
            this.TextBox_Second_Hold.Text = String.Empty;
            this.TextBox_Second_PreLocation.Text = String.Empty;

            this.TextBox_Second_DlsVSL.Text = String.Empty;
            this.TextBox_Second_LoadVSL.Text = String.Empty;
            this.TextBox_Second_InStowage.Text = String.Empty;
            this.TextBox_Second_OutStowage.Text = String.Empty;

            this.TextBox_Second_Status.Text = String.Empty;
            this.TextBox_Second_PlanSeq.Text = String.Empty;
            this.TextBox_Second_Expected.Text = String.Empty;
        }

        public void Btn_Container_Target_Click(object sender, RoutedEventArgs e)
        {
            _SelectedGrid = SelectedGrid.SelectedGrid_TargetViewGrid;

            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }
            else
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }

            //Btn_Container_Target.IsFocused = true;
            //Btn_Container_Twin.IsFocused = false;

            Grid_First_Cotainer.Visibility = Visibility.Visible;
            Grid_Second_Cotainer.Visibility = Visibility.Hidden;
        }

        public void Btn_Container_Twin_Click(object sender, RoutedEventArgs e)
        {
            _SelectedGrid = SelectedGrid.SelectedGrid_TwinGrid;

            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                    UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
            }
            
            //Btn_Container_Target.IsFocused = false;
            //Btn_Container_Twin.IsFocused = true;

            Grid_First_Cotainer.Visibility = Visibility.Hidden;
            Grid_Second_Cotainer.Visibility = Visibility.Visible;
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.ContainerDetailView &&
                    PresentationMgr.Singleton.PrevUIMode == PresentationMgr.UIMode.ContainerSearch)
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.ContainerSearch);
            else
                PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);
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
            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.MainView);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container,
                    UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                //PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                //        new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //        new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //        new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                //PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                if (_SelectedGrid == SelectedGrid.SelectedGrid_TargetViewGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                    //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }
                else if (_SelectedGrid == SelectedGrid.SelectedGrid_TwinGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Day.SearchView_ButtonDisableImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Day.SearchView_ButtonDefaultImage, UIThemeMgr.Day.SearchView_ButtonPressImage, UIThemeMgr.Day.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }

                PresentationMgr.SetSkinButton(this.Btn_Save,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Detwin,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Save_Twin,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Container,
                    UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                //PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                //PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                //    );

                if (_SelectedGrid == SelectedGrid.SelectedGrid_TargetViewGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                    //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                    //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                    //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }
                else if (_SelectedGrid == SelectedGrid.SelectedGrid_TwinGrid)
                {
                    PresentationMgr.SetSkinButton(this.Btn_Container_Target,
                        UIThemeMgr.Night.SearchView_ButtonDisableImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);

                    PresentationMgr.SetSkinButton(this.Btn_Container_Twin,
                        UIThemeMgr.Night.SearchView_ButtonDefaultImage, UIThemeMgr.Night.SearchView_ButtonPressImage, UIThemeMgr.Night.SearchView_ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/Button_Disable.png", UriKind.Relative))
                        //);
                }

                PresentationMgr.SetSkinButton(this.Btn_Save,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Detwin,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Save_Twin,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
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

            if (cntrNo.Equals(this.Btn_Container_Target.Content.ToString()))
                return;            
            
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
            Grid_First_Cotainer.Visibility = Visibility.Visible;
            Grid_Second_Cotainer.Visibility = Visibility.Hidden;
            this.Btn_Container_Target_Click(null, null);            

            InitFirstContainerInfo();
            InitSecondContainerInfo();

            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (String.IsNullOrEmpty(jobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null)
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

                this.TextBox_First_FM.Text = containerInfo.cntr.fullMty;
                this.TextBox_First_Operator.Text = containerInfo.cntr.opr;
                this.TextBox_First_WGT.Text = containerInfo.cntr.cntrWgt;
                this.TextBox_First_Location.Text = containerInfo.fmLoc.location;
                this.TextBox_First_POD.Text = containerInfo.cntr.pod;
                this.TextBox_First_Commodity.Text = containerInfo.cntr.cntrCgoTp; // G, M, H, R, etc
                this.TextBox_First_Damage.Text = containerInfo.cntr.isDmg ? "D" : String.Empty;
                this.TextBox_First_Hold.Text = containerInfo.cntr.isHold ? "Y" : String.Empty;
                this.TextBox_First_PreLocation.Text = containerInfo.toLoc.location;    // plan location            
                
                this.TextBox_First_DlsVSL.Text += containerInfo.inVsl.vessel;
                if (!String.IsNullOrEmpty(containerInfo.inVsl.vessel))
                    this.TextBox_First_DlsVSL.Text += "-";
                this.TextBox_First_DlsVSL.Text += containerInfo.inVsl.voyage;  // In VVD - inbound Vessel/voyage (2018-001-2015)
                
                this.TextBox_First_LoadVSL.Text += containerInfo.outVsl.vessel;
                if (!String.IsNullOrEmpty(containerInfo.outVsl.vessel))
                    this.TextBox_First_LoadVSL.Text += "-";
                this.TextBox_First_LoadVSL.Text += containerInfo.outVsl.voyage; // out VVD - outbound Vessel/voyage (2018-001-2015)

                this.TextBox_First_InStowage.Text = containerInfo.inVsl.vslLoc.location; // inbound Vessel/voyage stowage (11-16-82)
                this.TextBox_First_OutStowage.Text = containerInfo.outVsl.vslLoc.location; // outbound Vessel/voyage stowage (11-16-82)
            }   

            // job 관련
            if (String.IsNullOrEmpty(_targetJobKey))
                return;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(_targetJobKey);
            if (jobOrder == null)
                return;

            this.TextBox_First_Status.Text = PresentationMgr.GetContainerStatus(jobOrder.type.jobTp, jobOrder.type.jobStatus); // Job Description 표시(i.e. On chassis for delivery,etc) 
            this.TextBox_First_PlanSeq.Text = String.IsNullOrEmpty(jobOrder.taskId) ? "0" : jobOrder.taskId; // 본선 Plan sequence 표시                         
            try
            {
                this.TextBox_First_Expected.Text = String.IsNullOrEmpty(jobOrder.type.etw) ? 
                    "" : DateTime.ParseExact(jobOrder.type.etw, "yyyyMMddHHmmssfff", null).ToString("yyyy_MM_dd HH:mm");
            }
            catch
            {
                this.TextBox_First_Expected.Text = jobOrder.type.etw;   // Estimated Time of Work : 작성 예상 시간 정보(i.e. YYYY-MM-DD HH:MM:SS)
            }

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twinJobOrder = null;
            //if (jobOrder.type.twinTandemFlg.Equals("W"))
            if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
            {
                this.Grid_ContainerNumber_Twin.Visibility = System.Windows.Visibility.Visible;
                this.Grid_Bottom_Twin.Visibility = System.Windows.Visibility.Visible;

                twinJobOrder = PresentationMgr.Singleton.JOB_Get(jobOrder.type.ycTwinKey);
                if (null != twinJobOrder)
                {
                    this.Btn_Container_Twin.Content = twinJobOrder.cntr.cntrNo;
                    this.TextBox_Second_PlanSeq.Text = String.IsNullOrEmpty(twinJobOrder.taskId) ? "0" : twinJobOrder.taskId; // 본선 Plan sequence 표시 
                    this.TextBox_Second_Expected.Text = twinJobOrder.type.etw;  // Estimated Time of Work : 작성 예상 시간 정보(i.e. YYYY-MM-DD HH:MM:SS)
                    try
                    {
                        this.TextBox_Second_Expected.Text = String.IsNullOrEmpty(twinJobOrder.type.etw) ?
                            "" : DateTime.ParseExact(twinJobOrder.type.etw, "yyyyMMddHHmmssfff", null).ToString("yyyy_MM_dd HH:mm");
                    }
                    catch
                    {
                        this.TextBox_Second_Expected.Text = twinJobOrder.type.etw;  // Estimated Time of Work : 작성 예상 시간 정보(i.e. YYYY-MM-DD HH:MM:SS)
                    }
                    this.TextBox_Second_Status.Text = PresentationMgr.GetContainerStatus(twinJobOrder.type.jobTp, twinJobOrder.type.jobStatus); // Job Description 표시(i.e. On chassis for delivery,etc)            
                }

                // ask second containerinfo
                if (twinJobOrder != null)
                {
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

                this.TextBox_Second_FM.Text = containerInfo.cntr.fullMty;
                this.TextBox_Second_Operator.Text = containerInfo.cntr.opr;
                this.TextBox_Second_WGT.Text = containerInfo.cntr.cntrWgt;
                this.TextBox_Second_Location.Text = containerInfo.fmLoc.location;
                this.TextBox_Second_POD.Text = containerInfo.cntr.pod;
                this.TextBox_Second_Commodity.Text = containerInfo.cntr.cntrCgoTp; // G, M, H, R, etc
                this.TextBox_Second_Damage.Text = containerInfo.cntr.isDmg ? "D" : String.Empty;
                this.TextBox_Second_Hold.Text = containerInfo.cntr.isHold ? "Y" : String.Empty;
                this.TextBox_Second_PreLocation.Text = containerInfo.toLoc.location;    // plan location            

                this.TextBox_Second_DlsVSL.Text += containerInfo.inVsl.vessel;
                if (!String.IsNullOrEmpty(containerInfo.inVsl.vessel))
                    this.TextBox_Second_DlsVSL.Text += "-";
                this.TextBox_Second_DlsVSL.Text += containerInfo.inVsl.voyage;  // In VVD - inbound Vessel/voyage (2018-001-2015)

                this.TextBox_Second_LoadVSL.Text += containerInfo.outVsl.vessel;
                if (!String.IsNullOrEmpty(containerInfo.outVsl.vessel))
                    this.TextBox_Second_LoadVSL.Text += "-";
                this.TextBox_Second_LoadVSL.Text += containerInfo.outVsl.voyage; // out VVD - outbound Vessel/voyage (2018-001-2015)

                this.TextBox_Second_InStowage.Text = containerInfo.inVsl.vslLoc.location; // inbound Vessel/voyage stowage (11-16-82)
                this.TextBox_Second_OutStowage.Text = containerInfo.outVsl.vslLoc.location; // outbound Vessel/voyage stowage (11-16-82)
            }

            //  이미 셋팅되어 있다.
            //this.TextBox_Second_Status.Text = ""; // Job Description 표시(i.e. On chassis for delivery,etc)            
            //this.TextBox_Second_PlanSeq.Text = String.IsNullOrEmpty(jobOrder.taskId) ? "0" : jobOrder.taskId; 
            //this.TextBox_Second_Expected.Text = jobOrder.type.etw;
        }        
    }
}
