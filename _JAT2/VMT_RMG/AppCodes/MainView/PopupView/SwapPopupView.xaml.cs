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

//20190108
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for LocationChangeView.xaml
    /// </summary>
    public partial class SwapPopupView : UserControl
    {
        public enum UC_SwapViewRetType : int
        {
            UC_PopupViewRetType_Unknown = -1,
            UC_PopupViewRetType_ClickButtonLeft = 1,
            UC_PopupViewRetType_ClickButtonRight = 2,
        }

        public delegate void Callback_Popup(UC_SwapViewRetType seleted);
        static private Callback_Popup callback_popup;

        public SwapPopupView()
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

            TextBlock_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0069", LanguageService.LABEL_KEYPAD);
            this.txtRegoNo.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0103", LanguageService.LABEL_CUSTOMIZE);
            this.frm.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0104", LanguageService.LABEL_CUSTOMIZE);
            this.to.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0105", LanguageService.LABEL_CUSTOMIZE);
            this.Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0030", LanguageService.MESSAGE_GROUP);
            this.Btn_Ok.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP);
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

        public void ShowPopup(String fmCon, String toCon, String fmRegNo, String toRegNo, Callback_Popup callback)
        {
            this.fmCont.Text = fmCon;
            this.toCont.Text = toCon;
            this.txtRegoNo.Text = fmRegNo;
            //this.fmRegoNo.Text = fmRegNo;
            //this.toRegoNo.Text = toRegNo;
            this.Visibility = System.Windows.Visibility.Visible;
            callback_popup = callback;
        }

        private void Button_TwoButton_Left_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_SwapViewRetType.UC_PopupViewRetType_ClickButtonLeft);
        }

        private void Button_TwoButton_Right_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HidePopup();
            if (callback_popup != null)
                callback_popup(UC_SwapViewRetType.UC_PopupViewRetType_ClickButtonRight);
        }

        public void HidePopup()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
