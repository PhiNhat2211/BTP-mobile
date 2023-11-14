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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for VBlockChangeView.xaml
    /// </summary>
    public partial class VBlockChangeView : UserControl
    {
        public List<string> lVirtualBlck = new List<string>();

        public VBlockChangeView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSkinImage();
            InitEvent();
            LoadLanguage();
        }
        private void InitEvent()
        {
            this.Btn_BlockDown.Click += new RoutedEventHandler(Btn_BlockDown_Click);
            this.Btn_BlockUp.Click += new RoutedEventHandler(Btn_BlockUp_Click);
            this.Btn_Change.Click += new RoutedEventHandler(Btn_Change_Click);
            this.Btn_Cancel.Click += new RoutedEventHandler(Btn_Cancel_Click);
        }
        private void LoadLanguage()
        {
            this.Tbl_VBlockChange.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_VIRTUALBLOCK);
            this.Lb_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Change.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_VIRTUALBLOCK);
        }
        public void AddBlockBayVrtlInfoCallBack(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            lVirtualBlck.Clear();
            if (value != null)
            {
                foreach (var info in value.DicBlock)
                {
                    if (info.Value.isBolBlck == false)
                    {
                        lVirtualBlck.Add(info.Value.BlcName);
                    }
                }
                if (lVirtualBlck.Count() > 0)
                {
                    Btn_BlockText.Content = lVirtualBlck[0];
                }
            }
            checkLayoutUpDownBtn();
        }
        void Btn_BlockUp_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName) || lVirtualBlck.Count == 0 || blockName == lVirtualBlck[0])
            {
                return;
            }
            this.Btn_BlockText.Content = lVirtualBlck[lVirtualBlck.IndexOf(blockName) - 1];
            checkLayoutUpDownBtn();
        }
        void Btn_BlockDown_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName) || lVirtualBlck.Count == 0 || blockName == lVirtualBlck[lVirtualBlck.Count - 1])
                return;

            this.Btn_BlockText.Content = lVirtualBlck[lVirtualBlck.IndexOf(blockName) + 1];
            checkLayoutUpDownBtn();
        }
        private void checkLayoutUpDownBtn()
        {
            if (lVirtualBlck.Count == 0)
            {
                Btn_BlockDown.IsEnabled = false;
                Btn_BlockUp.IsEnabled = false;
            }
            else
            {
                if (lVirtualBlck.IndexOf(Btn_BlockText.Content.ToString()) == 0)
                {
                    Btn_BlockUp.IsEnabled = false;
                    Btn_BlockDown.IsEnabled = true;
                }
                else if (lVirtualBlck.IndexOf(Btn_BlockText.Content.ToString()) == lVirtualBlck.Count - 1)
                {
                    Btn_BlockUp.IsEnabled = true;
                    Btn_BlockDown.IsEnabled = false;
                }
                else
                {
                    Btn_BlockUp.IsEnabled = true;
                    Btn_BlockDown.IsEnabled = true;
                }
            }           
        }
        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
        private void Btn_Change_Click(object sender, RoutedEventArgs e)
        {
            if (lVirtualBlck.Count() > 0)
            {
                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                pos.m_cBlock = Btn_BlockText.Content.ToString();
                pos.m_cBay = "";
                pos.m_cRow = "";
                pos.m_cTier = "";
                PresentationMgr.Singleton.MakeCorrection(pos);
                this.Visibility = Visibility.Hidden;
            }           
        }
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
