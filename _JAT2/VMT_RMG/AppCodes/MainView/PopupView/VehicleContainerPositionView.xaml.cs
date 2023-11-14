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

//20190108
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for VehicleContainerPositionView.xaml
    /// </summary>
    public partial class VehicleContainerPositionView : UserControl
    {
        public String CurrentJobKey { get; set; }
        //public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder CurrentJobOrder { get; set; }
        public VehicleContainerPositionView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            this.CurrentJobKey = String.Empty;
            //CurrentJobOrder = null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(VehicleContainerPositionView_IsVisibleChanged);

            Button_OneButton.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0039", LanguageService.LABEL_POPUP);
            TextBlock_popup_title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0038", LanguageService.LABEL_POPUP);
            Button_TwoButton_Left.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0016", LanguageService.LABEL_SETTING);
            Button_TwoButton_Right.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0027", LanguageService.LABEL_POPUP);
        }

        private void InitSkinImage()
        {
            PresentationMgr.SetSkinButton(this.Button_OneButton,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_TwoButton_Left,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Button_TwoButton_Right,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/PopupButton/popup_btn_default.png", UriKind.Relative))
                );
        }

        private void VehicleContainerPositionView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility != System.Windows.Visibility.Visible)
            {
                //PresentationMgr.AppWin.UC_IndicatorView.Btn_Close.IsEnabled = true;
                return;
            }
            //PresentationMgr.AppWin.UC_IndicatorView.Btn_Close.IsEnabled = false;
                        
            //if (this.CurrentJobOrder == null)
            if (String.IsNullOrEmpty(this.CurrentJobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(this.CurrentJobKey);
            if (jobOrder == null || jobOrder.type == null)
                return;

            if (jobOrder.type.jobFlagInfo == "F")
            {
                this.Grid_Chassis_40F_double_front.Visibility = Visibility.Visible;
                this.Image_container_state_40F_double_front.Source =
                    JobListItem.GetJobTypeIcon(jobOrder.type.jobTp);
                this.TextBlock_JobCode_40F_double_front.Text = jobOrder.cntr.cntrNo;

                this.Grid_Chassis_40F_double_back.Visibility = Visibility.Hidden;
            }
            else // if (jobOrder.type.jobFlagInfo == "A")
            {                
                this.Grid_Chassis_40F_double_back.Visibility = Visibility.Visible;
                this.Image_container_state_40F_double_back.Source =
                    JobListItem.GetJobTypeIcon(jobOrder.type.jobTp);
                this.TextBlock_JobCode_40F_double_back.Text = jobOrder.cntr.cntrNo;

                this.Grid_Chassis_40F_double_front.Visibility = Visibility.Hidden;
            }
        }        

        private void Button_OneButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            if (String.IsNullOrEmpty(this.CurrentJobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(this.CurrentJobKey);
            if (jobOrder != null && jobOrder.type != null)
            {
                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "SetJobStatus_Ask"), jobOrder.jobKey + ", true");

                //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.jobKey, true);
                //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                //    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobOrder.type.ycTwinKey, true);

                var jobKeyList = new System.Collections.ArrayList();    // List<string>();
                jobKeyList.Add(jobOrder.jobKey);
                //if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                //    jobKeyList.Add(jobOrder.type.ycTwinKey);
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, true);

                PresentationMgr.AppWin.ShowProgressBar(0);

                PresentationMgr.Singleton.RefreshJobInventory(jobOrder, true);
            }            
            //this.CurrentJobOrder = null;            
            this.CurrentJobKey = String.Empty;
            
            //this.HidePopup();
            //if (callback_popup != null)
            //    callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickOneButton);
        }

        private void Button_TwoButton_Left_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.HidePopup();
            //if (callback_popup != null)
            //    callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickTwoButtonLeft);
        }

        private void Button_TwoButton_Right_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.HidePopup();
            //if (callback_popup != null)
            //    callback_popup(UC_PopupViewRetType.UC_PopupViewRetType_ClickTwoButtonRight);
        }
    }
}
