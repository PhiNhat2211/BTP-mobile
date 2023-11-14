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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ContainerMoving : UserControl
    {
        public enum BtnJobSetType
        {
            BtnJobSetType_Unknown,
            BtnJobSetType_Normal,
            BtnJobSetType_Processing,
            BtnJobSetType_Lock
        }

        private readonly BitmapImage _jobSetDefaultDayImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Default.png", UriKind.Relative));
        private readonly BitmapImage _jobSetDefaultNightImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Default.png", UriKind.Relative));

        private readonly BitmapImage _jobSetEnableDayImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative));
        private readonly BitmapImage _jobSetEnableNightImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Enable.png", UriKind.Relative));

        private readonly BitmapImage _jobSetLockDayImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_JobSet_Lock.png", UriKind.Relative));
        private readonly BitmapImage _jobSetLockNightImg = new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_JobSet_Lock.png", UriKind.Relative));

       

        public ContainerMoving()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~ContainerMoving()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler

            this.Btn_Navigator1.Click += new RoutedEventHandler(Btn_Navigator_Click1);
            this.Btn_Navigator2.Click += new RoutedEventHandler(Btn_Navigator_Click2);
        }

        //public void Callback

        private void Btn_Navigator_Click1(object sender, RoutedEventArgs e)
        {
            if (this.Btn_Navigator1.IsChecked == true)
            {
                this.UC_NavigatorView1.Visibility = System.Windows.Visibility.Visible;
                this.UC_NavigatorView1.IsWaitingToHide = false;
            }
            else
            {
                this.UC_NavigatorView1.Visibility = System.Windows.Visibility.Hidden;
                this.UC_NavigatorView1.IsWaitingToHide = true;
                PresentationMgr.Singleton.SetInventoryDataBay1(null);
            }
        }
        private void Btn_Navigator_Click2(object sender, RoutedEventArgs e)
        {
            if (this.Btn_Navigator2.IsChecked == true)
            {
                this.UC_NavigatorView2.Visibility = System.Windows.Visibility.Visible;
                this.UC_NavigatorView2.IsWaitingToHide = false;
            }
            else
            {
                this.UC_NavigatorView2.Visibility = System.Windows.Visibility.Hidden;
                this.UC_NavigatorView2.IsWaitingToHide = true;
                PresentationMgr.Singleton.SetInventoryDataBay2(null);
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator1,
                    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);
                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator2,
                    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator1,
                    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);
                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator2,
                    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}