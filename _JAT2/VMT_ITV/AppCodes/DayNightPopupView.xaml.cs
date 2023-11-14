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
using Common.Interface;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for DayNightPopupView.xaml
    /// </summary>
    public partial class DayNightPopupView : UserControl
    {
        private MainView mMainView = null;

        public DayNightPopupView()
        {
            this.InitializeComponent();
        }

        public void Init(MainView mainView)
        {
            mMainView = mainView;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextBlock_Day.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0009", LanguageService.LABEL_AVAILABLE);
            this.TextBlock_Night.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0010", LanguageService.LABEL_AVAILABLE);
            this.btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0011", LanguageService.LABEL_AVAILABLE);
            this.btn_Ok.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0012", LanguageService.LABEL_AVAILABLE);
        }

        public void Show_Popup()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide_Popup()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void TextBlock_DayNight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock selectTextBlock = (TextBlock)sender;
            if (selectTextBlock.Name.Equals("TextBlock_Day"))
            {
                selectTextBlock.Background = Brushes.Red;
                TextBlock_Night.Background = mMainView.colorEnable;
                Image_dayView.Source = new BitmapImage(new Uri(@"/VMT_ITV;component/Images/MainView/day/main_available_img_screen_day.png", UriKind.Relative));
                mMainView.mIsDay = true;
            }
            else if (selectTextBlock.Name.Equals("TextBlock_Night"))
            {
                selectTextBlock.Background = Brushes.Red;
                TextBlock_Day.Background = mMainView.colorEnable;
                Image_dayView.Source = new BitmapImage(new Uri(@"/VMT_ITV;component/Images/MainView/day/main_available_img_screen_night.png", UriKind.Relative));
                mMainView.mIsDay = false;
            }
            else
            {
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            mMainView.HideScreenMode();
            mMainView.TextBlock_StopPage.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0030", LanguageService.LABEL_MAINWINDOW);
            mMainView.TextBlock_StopPage.Background = mMainView.colorEnable;
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            mMainView.Change_Day_Night();
        }
    }
}
