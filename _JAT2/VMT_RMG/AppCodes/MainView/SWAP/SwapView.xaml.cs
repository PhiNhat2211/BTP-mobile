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
    public partial class SwapView : UserControl
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

       

        public SwapView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~SwapView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.CheckBox_Block_All.Click += new RoutedEventHandler(CheckBox_Block_All_Click);
            this.Btn_Navigator.Click += new RoutedEventHandler(Btn_Navigator_Click);
            this.Btn_Swap.Click += new RoutedEventHandler(Btn_Swap_Click);
        }

        private void CheckBox_Block_All_Click(object sender, RoutedEventArgs e)
        {
            //if (String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            //{
            //    this.SelectFirstJob();
            //}
            var jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;

            //20210406 getSwapList blockName-bayName
            String bayName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text.ToString();
            if (bayName.Length > 1) bayName = "-" + bayName.Substring(0, 2);
            else bayName = String.Empty;

            VMT_Data_JAT2.VMT_DataMgr_Common.getSwapList_Ask(jobKey, PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text.ToString() + bayName, this.CheckBox_Block_All.IsChecked == true);
        }

        private void SelectFirstJob()
        {
            if (this.UC_JobListSwap.ListBox_Job.Items.Count > 0)
            {
                JobListItem item = this.UC_JobListSwap.ListBox_Job.Items.GetItemAt(0) as JobListItem;
                if (item != null)
                {
                    var job = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                    if (job != null)
                    {
                        UC_JobListSwap.ListBox_Job.SelectedIndex = 0;
                        if (UC_JobListSwap.ListBox_Job.SelectedItem != null)
                        {
                            (UC_JobListSwap.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = job.jobKey;
                        }
                    }
                }
            }
        }

        //public void Callback

        private void Btn_Navigator_Click(object sender, RoutedEventArgs e)
        {
            if (this.Btn_Navigator.IsChecked == true)
            {
                this.UC_NavigatorView.Visibility = System.Windows.Visibility.Visible;
                this.UC_NavigatorView.IsWaitingToHide = false;
            }
            else
            {
                this.UC_NavigatorView.Visibility = System.Windows.Visibility.Hidden;
                this.UC_NavigatorView.IsWaitingToHide = true;
                PresentationMgr.Singleton.SetInventoryDataSwap(null);
            }
        }

        public void Btn_Swap_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
            {
                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                if (target != null)
                {
                    this.Btn_Swap.IsEnabled = false;
                    this.UC_SwapPopupView.ShowPopup(PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content.ToString(),
                        this.UC_JobListSwap.swapItem.cntrNo, PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content.ToString(), this.UC_JobListSwap.swapItem.regoNo, new SwapPopupView.Callback_Popup(CallbackClosePopup));
                }
            }
        }

        public void CallbackClosePopup(SwapPopupView.UC_SwapViewRetType selected)
        {
            switch (selected)
            {
                case SwapPopupView.UC_SwapViewRetType.UC_PopupViewRetType_ClickButtonLeft:
                    break;
                case SwapPopupView.UC_SwapViewRetType.UC_PopupViewRetType_ClickButtonRight:
                    {
                        VMT_Data_JAT2.VMT_DataMgr_Common.setEmptySwap(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey, this.UC_JobListSwap.swapItem.cntrNo, this.UC_JobListSwap.swapItem.cntrPnt, PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_PartnerNumber.Content.ToString());
                    }
                    break;
                default:
                    break;
            }
            this.Btn_Swap.IsEnabled = true;
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator,
                    UIThemeMgr.Day.ButtonNavigatorOnImage, UIThemeMgr.Day.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Swap,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);
                PresentationMgr.SetSkinCheckBox(this.CheckBox_Block_All,
                    UIThemeMgr.Day.ButtonBlockOnImage, UIThemeMgr.Day.ButtonBlockOffImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinCheckBox(this.Btn_Navigator,
                    UIThemeMgr.Night.ButtonNavigatorOnImage, UIThemeMgr.Night.ButtonNavigatorOffImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_NavigatorView_On.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Navigator_Off.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_Swap,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage_1);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                //);
                PresentationMgr.SetSkinCheckBox(this.CheckBox_Block_All,
                    UIThemeMgr.Night.ButtonBlockOnImage, UIThemeMgr.Night.ButtonBlockOffImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}