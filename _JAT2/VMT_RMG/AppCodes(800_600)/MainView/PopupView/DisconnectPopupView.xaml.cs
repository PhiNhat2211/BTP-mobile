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
using System.Windows.Threading;

namespace VMT_RMG_800by600
{
	/// <summary>
	/// Interaction logic for UC_DisconnectPopupView.xaml
	/// </summary>
	public partial class UC_DisconnectPopupView : UserControl
	{
        public enum UC_PopupViewType : int
        {
            PopupViewType_Unknown = -1,
            
            PopupViewType_Common = 0,
            PopupViewType_NetworkDisconnect = 1,

            PopupViewType_Logout = 98,
            PopupViewType_SystemOff = 99,
        }

        public enum UC_PopupViewRetType : int
        {
            UC_PopupViewRetType_Unknown = -1,
            UC_PopupViewRetType_RunOutOfTime = 0,
            UC_PopupViewRetType_ClickOneButton = 1,
            UC_PopupViewRetType_ClickTwoButtonLeft = 1,
            UC_PopupViewRetType_ClickTwoButtonRight = 2,
            UC_PopupViewRetType_ClickThreeButtonLeft = 1,
            UC_PopupViewRetType_ClickThreeButtonCenter = 2,
            UC_PopupViewRetType_ClickThreeButtonRight = 3,
        }

        public delegate void Callback_Popup(UC_PopupViewRetType seleted);
        static private Callback_Popup callback_popup;

        private DispatcherTimer _DisplayTimer = null;

        private UC_PopupViewType _PopupViewType = UC_PopupViewType.PopupViewType_Unknown;

		public UC_DisconnectPopupView()
		{
			this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);
		}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSkinImage();
        }

        private void InitSkinImage()
        {
            PresentationMgr.SetSkinButton(this.Button_OneButton,
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_TwoButton_Left,
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_TwoButton_Right,
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_ThreeButton_Left,
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_ThreeButton_Center,
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_ThreeButton_Right,
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RMG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );
        }

        public void ShowPopup(UC_PopupViewType PopupViewType, String title, String message,
                                String ButtonCSV, Callback_Popup callback, int sec)
        {
            if( _PopupViewType >= PopupViewType )
                return;

            _PopupViewType = PopupViewType;

            this.TextBlock_popup_title.Text = title;
            message = message.Trim('\n');
            this.TextBlock_popup_message.Text = message;
            Grid_OneButton.Visibility = System.Windows.Visibility.Hidden;
            Grid_TwoButton.Visibility = System.Windows.Visibility.Hidden;
            Grid_ThreeButton.Visibility = System.Windows.Visibility.Hidden;

            String[] strButtonArray = ButtonCSV.Split(',');

            if( strButtonArray.Length == 1)
            {
                Grid_OneButton.Visibility = System.Windows.Visibility.Visible;
                Button_OneButton.Content = strButtonArray[0];
            }
            else if (strButtonArray.Length == 2)
            {
                Grid_TwoButton.Visibility = System.Windows.Visibility.Visible;
                Button_TwoButton_Left.Content = strButtonArray[0];
                Button_TwoButton_Right.Content = strButtonArray[1];
            }
            else if (strButtonArray.Length == 3)
            {
                Grid_ThreeButton.Visibility = System.Windows.Visibility.Visible;
                Button_ThreeButton_Left.Content = strButtonArray[0];
                Button_ThreeButton_Center.Content = strButtonArray[1];
                Button_ThreeButton_Right.Content = strButtonArray[2];
            }

            this.Visibility = System.Windows.Visibility.Visible;
            callback_popup = callback;

            if (sec > 0)
            {
                StartDisplayPopup(sec);
            }
        }

        public void HidePopup()
        {
            _PopupViewType = UC_PopupViewType.PopupViewType_Unknown;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void StartDisplayPopup(int sec)
        {
            if (_DisplayTimer == null)
            {
                _DisplayTimer = new DispatcherTimer();
                _DisplayTimer.Interval = new TimeSpan(0, 0, sec);
                _DisplayTimer.Tick += new EventHandler(AnimationTimer_Handler);
                _DisplayTimer.Start();
            }
        }

        void AnimationTimer_Handler(object sender, EventArgs e)
        {
            StopDisplayPopup();
            this.HidePopup();
        }

        public void StopDisplayPopup()
        {
            if (_DisplayTimer != null)
                _DisplayTimer.Stop();

            _DisplayTimer = null;

            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_RunOutOfTime);
        }

        private void Button_OneButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {	
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickOneButton);
        }

        private void Button_TwoButton_Left_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickTwoButtonLeft);
        }

        private void Button_TwoButton_Right_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickTwoButtonRight);
        }

        private void Button_ThreeButton_Left_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonLeft);
        }

        private void Button_ThreeButton_Center_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonCenter);
        }

        private void Button_ThreeButton_Right_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickThreeButtonRight);
        }
	}
}