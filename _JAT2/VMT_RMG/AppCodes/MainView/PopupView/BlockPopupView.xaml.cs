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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for LocationChangeView.xaml
    /// </summary>
    public partial class BlockPopupView : UserControl
    {
        public enum UC_BlockViewRetType : int
        {
            UC_PopupViewRetType_Unknown = -1,
            UC_PopupViewRetType_ClickButtonLeft = 1,
            UC_PopupViewRetType_ClickButtonRight = 2,
        }

        public delegate void Callback_BlockPopup(UC_BlockViewRetType seleted);
        static private Callback_BlockPopup callback_popup;

        public BlockPopupView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            UC_KeyPad.DoneCallback += new Keypad.KeypadDoneCallback(this.KeypadDone);
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                
                PresentationMgr.SetSkinButton(this.Btn_Ok,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);                
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                
                PresentationMgr.SetSkinButton(this.Btn_Ok,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);                
            }
        }

        private void KeypadDone()
        {
            this.UC_KeyPad.HideKeyPad();
        }

        public void ShowPopup(Callback_BlockPopup callback)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            callback_popup = callback;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UC_KeyPad.ShowKeyPad(sender as TextBox);
        }

        private void Button_TwoButton_Left_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_BlockViewRetType.UC_PopupViewRetType_ClickButtonLeft);
        }

        private void Button_TwoButton_Right_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_BlockViewRetType.UC_PopupViewRetType_ClickButtonRight);
        }

        public void HidePopup()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
