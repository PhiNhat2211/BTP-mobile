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
using VMT_Data_JAT2.Objects;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for PopupOutView.xaml
    /// </summary>
    public partial class UC_PopupOutView : UserControl
    {
        public UC_PopupOutView()
        {
            this.InitializeComponent();

            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Load Data
            LoadData();
            //Init Event Handler
            this.Btn_BlockDown.Click += new RoutedEventHandler(Btn_BlockDown_Click);
            this.Btn_BlockUp.Click += new RoutedEventHandler(Btn_BlockUp_Click);
            this.Btn_BayDown.Click += new RoutedEventHandler(Btn_BayDown_Click);
            this.Btn_BayUp.Click += new RoutedEventHandler(Btn_BayUp_Click);
            this.Btn_Cancel.Click += new RoutedEventHandler(Btn_Cancel_Click);
            this.Btn_OK.Click += new RoutedEventHandler(Btn_Ok_Click);
        }

        public void ShowPopup(String title, String message, String button)
        {
            this.Tbl_Title.Text = title;
            this.Tbl_Message.Text = message;
            this.Btn_OK.Content = button;
            this.Tbl_Machine.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_POPUPOUT);
            this.Tbl_ID.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_POPUPOUT);
            this.Tbl_Name.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_POPUPOUT);
            this.Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_POPUPOUT);
            this.Visibility = System.Windows.Visibility.Visible;
            SetContentData();
            CheckBlockLocation();
        }

        public void HidePopup()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        #region [InitData]
        private void LoadData()
        {
            InitSkinImage();
        }
        public void SetContentData()
        {
            this.Btn_BlockText.Content = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text;
            this.Btn_BayText.Content = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text;

            this.Tb_Machine.Content = PresentationMgr.AppWin.UC_IndicatorView.TextBlock_MachineID.Text;
            this.Tb_ID.Content = PresentationMgr.AppWin.UC_IndicatorView.TextBox_UserID.Text;
            this.Tb_Name.Content = PresentationMgr.AppWin.UC_LogInView.TextBlock_Name.Text;
        }
        #endregion [InitData]

        #region [CheckLayoutButton]
        public void CheckBlockLocation()
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
            {
                this.Btn_BlockUp.IsEnabled = false;
                this.Btn_BlockDown.IsEnabled = false;
                return;
            }

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockUp.IsEnabled = (i >= list.Count - 1) || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BlockDown.IsEnabled = i <= 0 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    break;
                }
            }
        }
        public void CheckBayLocation()
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = this.Btn_BayText.Content.ToString();
            if (String.IsNullOrEmpty(bayName))
            {
                this.Btn_BayUp.IsEnabled = false;
                this.Btn_BayDown.IsEnabled = false;
                return;
            }

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 0; i < list.Count; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayUp.IsEnabled = (i >= list.Count - 1) || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayDown.IsEnabled = i <= 0 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    break;
                }
            }
            Btn_BayText.Content = list[0].BayName;
            Btn_BayDown.IsEnabled = false;
        }
        #endregion [CheckLayoutButton]

        #region [UIEvent]
        void Btn_BlockUp_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            Btn_BayText.IsEnabled = false;

            String blockNameNext = String.Empty;

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockUp.IsEnabled = (i >= list.Count - 1) || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BlockDown.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                    blockNameNext = list[i + 1].Value.BlcName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(blockNameNext) && blockNameNext != blockName)
            {
                this.Btn_BlockText.Content = blockNameNext;
                // call getBlockMapList
                PresentationMgr.AppWin.ShowProgressBar(0);
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockNameNext);
            }
        }
        void Btn_BlockDown_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            Btn_BayText.IsEnabled = false;

            String blockNamePre = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();
            for (int i = 1; i < list.Count; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockDown.IsEnabled = i <= 1 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BlockUp.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                    blockNamePre = list[i - 1].Value.BlcName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(blockNamePre) && blockNamePre != blockName)
            {
                this.Btn_BlockText.Content = blockNamePre;
                // call getBlockMapList
                PresentationMgr.AppWin.ShowProgressBar(0);
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockNamePre);
            }

        }
        void Btn_BayUp_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = this.Btn_BayText.Content.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            String bayNameNext = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayUp.IsEnabled = (i >= list.Count - 2) || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayDown.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                    bayNameNext = list[i + 1].BayName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(bayNameNext) && bayNameNext != bayName)
            {
                this.Btn_BayText.Content = bayNameNext;
            }
        }
        void Btn_BayDown_Click(object sender, RoutedEventArgs e)
        {
            String bayName = this.Btn_BayText.Content.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            String bayNamePre = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 1; i < list.Count; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayDown.IsEnabled = i <= 1 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayUp.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                    bayNamePre = list[i - 1].BayName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(bayNamePre) && bayNamePre != bayName)
            {
                this.Btn_BayText.Content = bayNamePre;
            }
        }       
        void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            var block = Convert.ToString(this.Btn_BlockText.Content);
            var bay = Convert.ToString(this.Btn_BayText.Content);
            // Aug 11 2023 CLT not call SetChangePosition
            if (!String.IsNullOrEmpty(block) && !UserInfo.gUserID.Equals("CLT"))
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetChangePosition_ask(block, bay);

            Button btnSelected = (Button)sender;
            if (btnSelected.Content.Equals(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_POPUPOUT))) //LOGOUT
            {
                // User ID should be logout to be reset.
                PresentationMgr.AppWin.UC_LogInView.UpdateLogInStep(LogInView.LOGIN_STATUS.en_NoAuth);

                //PresentationMgr.AppWin.UC_IndicatorView.Image_Setting.Visibility = System.Windows.Visibility.Visible;
                PresentationMgr.Singleton.showSetting = false; //20201008 hide setting after logout
                PresentationMgr.Singleton.showHideButtonsAccessAction();

                PresentationMgr.AppWin.UC_IndicatorView.Image_WS.Visibility = System.Windows.Visibility.Collapsed;
                PresentationMgr.AppWin.ShowProgressBar(0);
                MainWindow.dtClockTime.Stop();
                if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
                {
                    PresentationMgr.AppWin.PLCTimer.Stop();
                }
                VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                //VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Ask();
                HidePopup();
            }
            else if (btnSelected.Content.Equals(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_POPUPOUT))) //TURN OFF
            {
                MainWindow.dtClockTime.Stop();
                if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
                {
                    PresentationMgr.AppWin.PLCTimer.Stop();
                }
                VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
                VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);
                System.Diagnostics.Process.Start("shutdown.exe", "-s -f");

                HidePopup();
            }
        }
        void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            HidePopup();
        }
        #endregion [UIEvent]

        #region [InitSkin]
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

                PresentationMgr.SetSkinButton(this.Btn_BayDown,
                   UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BayText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BayUp,
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

                PresentationMgr.SetSkinButton(this.Btn_BayDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BayText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BayUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);
            }
        }
        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
        #endregion [InitSkin]
    }
}
