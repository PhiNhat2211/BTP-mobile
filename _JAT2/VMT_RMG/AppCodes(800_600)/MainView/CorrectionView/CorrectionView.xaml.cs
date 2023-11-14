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

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for CorrectionView.xaml
    /// </summary>
    public partial class CorrectionView : UserControl
    {
        public CorrectionView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            //this.Btn_Navigator_Left.Click += new RoutedEventHandler(Btn_Navigator_Left_Click);
            //this.Btn_Test_1.Click += new RoutedEventHandler(Btn_Test_1_Click);
            //this.Btn_Test_2.Click += new RoutedEventHandler(Btn_Test_2_Click);

            //this.Btn_Navigator_Right.Click += new RoutedEventHandler(Btn_Navigator_Right_Click);
            //this.Btn_Test_3.Click += new RoutedEventHandler(Btn_Test_3_Click);
            //this.Btn_Test_4.Click += new RoutedEventHandler(Btn_Test_4_Click);
        }

        private void Btn_Navigator_Left_Click(object sender, RoutedEventArgs e)
        {
            //if (this.Btn_Navigator_Left.IsChecked == true)
            //{
            //}
            //else
            //{
            //}
        }

        void Btn_Test_1_Click(object sender, RoutedEventArgs e)
        {

        }

        void Btn_Test_2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Navigator_Right_Click(object sender, RoutedEventArgs e)
        {
            //if (this.Btn_Navigator_Right.IsChecked == true)
            //{
            //}
            //else
            //{
            //}
        }

        void Btn_Test_3_Click(object sender, RoutedEventArgs e)
        {

        }

        void Btn_Test_4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                //PresentationMgr.SetSkinCheckBox(this.Btn_Navigator_Left,
                //    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_1,
                //    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_2,
                //    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinCheckBox(this.Btn_Navigator_Right,
                //    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_3,
                //    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_4,
                //    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                //PresentationMgr.SetSkinCheckBox(this.Btn_Navigator_Left,
                //    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_1,
                //    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_2,
                //    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinCheckBox(this.Btn_Navigator_Right,
                //    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_3,
                //    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);

                //PresentationMgr.SetSkinButton(this.Btn_Test_4,
                //    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}
