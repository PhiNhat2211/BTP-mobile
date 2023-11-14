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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for TargetJobInfo.xaml
    /// </summary>
    public partial class TargetJobInfo1 : UserControl
    {
        public TargetJobInfo1()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~TargetJobInfo1()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_PartnerNumber.Click += new RoutedEventHandler(Btn_PartnerNumber_Click);
            this.Btn_ContainerNumber.Click += new RoutedEventHandler(Btn_ContainerNumber_Click);
            this.Btn_ContainerNumber_Twin.Click += new RoutedEventHandler(Btn_ContainerNumber_Twin_Click);
        }

        // Button Event
        void Btn_PartnerNumber_Click(object sender, RoutedEventArgs e)
        {
            //if (this.Btn_PartnerNumber.Content == null)
            //    return;

            //if (String.IsNullOrEmpty(this.Btn_PartnerNumber.Content.ToString()))
            {
                String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                if (jobOrder == null || jobOrder.type == null)
                    return;
                if ("GC".Equals(jobOrder.type.jobTp))
                {
                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobKey;
                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                    PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
                    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerMoving);         
                    PresentationMgr.Singleton.MovingPosition1 = PresentationMgr.Singleton.CurrentPostion;
                    PresentationMgr.Singleton.MovingPosition2 = PresentationMgr.Singleton.CurrentPostion;
                } 
                else
                {
                    var machineID = Btn_PartnerNumber.Content.ToString();
                    PresentationMgr.ShowPartnerMachineSearchPopup();
                    if (DataMgr.Singleton.List_MachineofPool.Count <= 0)
                    {
                        InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            , "GetMachineList_Ask"), machineID);

                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                    else
                        PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
                }
            }
            //else
            //{
            //    var machineID = Btn_PartnerNumber.Content.ToString();
            //    PresentationMgr.ShowPartnerMachineSearchPopup();

            //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
            //        , "GetMachineListofPool_Ask"), machineID);

            //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineListofPool_Ask(machineID);
            //    PresentationMgr.AppWin.ShowProgressBar(0);
            //}
        }

        void Btn_ContainerNumber_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Btn_ContainerNumber.Content == null && String.IsNullOrEmpty(this.Btn_ContainerNumber.Content.ToString()))
                return;
            PresentationMgr.Singleton.PrevBlckBayUIMode = PresentationMgr.Singleton.CurrentUIMode;            
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);            
            //PresentationMgr.MainView.UC_ContainerDetailView.Btn_Container_Target_Click(null, null);
            PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this.Btn_ContainerNumber.Content.ToString(),
                (String.IsNullOrEmpty(this.Btn_ContainerNumber_Twin.Content.ToString())? null : (bool?)false));
        }

        void Btn_ContainerNumber_Twin_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Btn_ContainerNumber_Twin.Content == null && String.IsNullOrEmpty(this.Btn_ContainerNumber_Twin.Content.ToString()))
                return;

            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
            //PresentationMgr.MainView.UC_ContainerDetailView.Btn_Container_Twin_Click(null, null);
            PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this.Btn_ContainerNumber.Content.ToString(), (bool?)true);
        }

        public void RefreshTargetJobInfo()
        {
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;

            this.Btn_ContainerNumber_Twin.IsEnabled = false;
            this.Btn_ContainerNumber_Twin.Content = String.Empty;
            this.Image_Twin.Visibility = Visibility.Hidden;

            if (String.IsNullOrEmpty(jobKey))
            {
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);                    
                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                        UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);                    
                }
                this.Btn_PartnerNumber.Content = String.Empty;
                this.Btn_ContainerNumber.Content = String.Empty;
                this.Btn_ContainerNumber.IsEnabled = false;
                return;
            }

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null || jobOrder.type == null)
                return;

            this.Btn_PartnerNumber.Content = jobOrder.partnerMchn.mchnId;
            this.Btn_ContainerNumber.Content = jobOrder.cntr.cntrNo;

            //this.Btn_PartnerNumber.IsEnabled = String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId) ? false : true;
            this.Btn_PartnerNumber.IsEnabled = true;
            this.Btn_ContainerNumber.IsEnabled = String.IsNullOrEmpty(jobOrder.cntr.cntrNo) ? false : true;

            //if (jobOrder.type.twinTandemFlg.Equals("W"))            
            {
                if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twinJobOrder = PresentationMgr.Singleton.JOB_Get(jobOrder.type.ycTwinKey);
                    if (null != twinJobOrder && null != twinJobOrder.cntr)
                    {
                        this.Image_Twin.Visibility = Visibility.Visible;
                        this.Btn_ContainerNumber_Twin.Content = twinJobOrder.cntr.cntrNo;
                        this.Btn_ContainerNumber_Twin.IsEnabled = String.IsNullOrEmpty(twinJobOrder.cntr.cntrNo) ? false : true;
                    }
                }
            }

            if (jobOrder.type.jobStatus == "P")
            {
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                        UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);                        

                    PresentationMgr.SetSkinButton(this.Btn_ContainerNumber,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);                        
                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                        UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);                        

                    PresentationMgr.SetSkinButton(this.Btn_ContainerNumber,
                        UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonDisableImage);                        
                }
            }
            else
            {
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);                        

                    PresentationMgr.SetSkinButton(this.Btn_ContainerNumber,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_disable.png", UriKind.Relative))
                        //);

                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                        UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);                        
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_disable.png", UriKind.Relative))
                        //);

                    PresentationMgr.SetSkinButton(this.Btn_ContainerNumber,
                        UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);                        
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                        //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_disable.png", UriKind.Relative))
                        //);
                }
            }
            //this.Btn_PartnerNumber.IsEnabled = jobOrder.type.jobStatus == "Q" ? false : true;
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_ContainerNumber,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_ContainerNumber_Twin,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_disable.png", UriKind.Relative))
                    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_PartnerNumber,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_ContainerNumber,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_ContainerNumber_Twin,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_disable.png", UriKind.Relative))
                    //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}
