using Common.Interface;
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
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for TargetJobInfo.xaml
    /// </summary>
    public partial class TargetJobInfo : UserControl
    {
        public String jobKeyProcessing = String.Empty;
        public TargetJobInfo()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~TargetJobInfo()
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
            this.Btn_MovingContainer.Click += new RoutedEventHandler(Btn_MovingContainer_Click);
            this.CheckBox_Refresh.Click += new RoutedEventHandler(CheckBox_Refresh_Click);
            this.Btn_Refresh.Click += new RoutedEventHandler(Btn_Refresh_Click);
            this.Btn_Unlock.Click += new RoutedEventHandler(Btn_Unlock_Click);
            this.Btn_Available.Click += new RoutedEventHandler(Btn_Available_Click);

            this.CheckBox_Refresh.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0020", LanguageService.LABEL_MAINWINDOW);
            this.Btn_Refresh.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0020", LanguageService.LABEL_MAINWINDOW);
            this.Btn_Unlock.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0021", LanguageService.LABEL_MAINWINDOW);
            this.Label_OnOff.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0076", LanguageService.LABEL_MAINWINDOW);
            Btn_Available.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0231", LanguageService.LABEL_CUSTOMIZE);

            if (UserInfo.gMchnTp.Equals("TC"))
            {
                this.Btn_Unlock.Visibility = Visibility.Visible;
                this.Btn_Refresh.Visibility = Visibility.Hidden;
            }
            else //TC or RS
            {
                this.Btn_Unlock.Visibility = Visibility.Hidden;
                this.Btn_Refresh.Visibility = Visibility.Visible;
                this.Btn_Refresh.IsEnabled = true;
            }
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
                //if ("GO".Equals(jobOrder.type.jobTp) && jobOrder.isSwap)
                //{
                //    //VMT_Data_JAT2.VMT_DataMgr_Common.setEmptySwap("TGHU3029767120170222160416","SKLU0735281","1","부산22나2446");
                //    PresentationMgr.MainView.UC_SwapView.UC_JobListSwap.SwapAutoSelection = false;
                //    VMT_Data_JAT2.VMT_DataMgr_Common.getSwapList_Ask(jobKey, PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Content.ToString(), PresentationMgr.MainView.UC_SwapView.CheckBox_Block_All.IsChecked == true);
                //    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobKey;
                //    PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
                //    //PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Content =  PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Content;
                //    //PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Content = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Content;
                //    PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
                //    PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.SwapView);
                //} 
                //else 
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
                    var job = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(jobOrder);
                    PresentationMgr.ShowYtSwapPopup(job);
                    //if (DataMgr.Singleton.List_MachineofPool.Count <= 0)
                    //{
                    //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    //        , "GetMachineList_Ask"), machineID);

                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
                    //    PresentationMgr.AppWin.ShowProgressBar(0);
                    //}
                    //else
                    //    PresentationMgr.MainView.UC_YtSwapView.SetMachineList();
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

        void Btn_MovingContainer_Click(object sender, RoutedEventArgs e)
        {
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null || jobOrder.type == null)
                return;
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = jobKey;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_BlockText.IsEnabled = true;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerMoving);
            PresentationMgr.Singleton.MovingPosition1 = PresentationMgr.Singleton.CurrentPostion;
            PresentationMgr.Singleton.MovingPosition2 = PresentationMgr.Singleton.CurrentPostion;
        }

        private void CheckBox_Refresh_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.MainView.UC_JobList.IsRefreshReserved = Convert.ToBoolean(this.CheckBox_Refresh.IsChecked);
            if (this.CheckBox_Refresh.IsChecked == true)
            {
                if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
                {
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
                    if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);
                }

                //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                PresentationMgr.Singleton.ThreadTimerStart(true);
            }

            //PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay);
        }
        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock))
            {
                PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, Convert.ToString(PresentationMgr.Singleton.CurrentBay));
            }
        }
        private void Btn_Unlock_Click(object sender, RoutedEventArgs e)
        {
            var listJobProcessing = DataMgr.Singleton.List_JobOrder.FindAll(x => x.type.jobStatus == "P");

            if (listJobProcessing.Count() == 0)
                jobKeyProcessing = String.Empty;
            else if (String.IsNullOrEmpty(jobKeyProcessing))
                jobKeyProcessing = listJobProcessing.First().jobKey;

            PresentationMgr.AppWin.wrkCdPLC = "2";
            PresentationMgr.AppWin.cntrReleasePLC = PresentationMgr.MainView.plcDomainTwistLock.cntrNo;
            VMT_DataMgr_RMG.ReleasePLCLock_Ask(jobKeyProcessing, UserInfo.gMchnID, UserInfo.gUserID, PresentationMgr.MainView.plcDomainTwistLock.msgSeq);
            jobKeyProcessing = String.Empty;
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
                (String.IsNullOrEmpty(Convert.ToString(this.Btn_ContainerNumber_Twin.Content)) ? null : (bool?)false));
        }

        void Btn_ContainerNumber_Twin_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Btn_ContainerNumber_Twin.Content == null && String.IsNullOrEmpty(Convert.ToString(this.Btn_ContainerNumber_Twin.Content)))
                return;

            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
            //PresentationMgr.MainView.UC_ContainerDetailView.Btn_Container_Twin_Click(null, null);
            PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this.Btn_ContainerNumber.Content.ToString(), (bool?)true);
        }
        private void Btn_Available_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Btn_Available.Content.Equals(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0231", LanguageService.LABEL_CUSTOMIZE)))
            {
                if (PresentationMgr.MainView.UC_AvailableView.Wrap_AvailableView.Children.Count <= 0)
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStopCodeList_Ask();

                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.AvailableView);
            }
            else
            {
                //PresentationMgr.MainView.UC_BreakTimeView.SetEndDateTime();
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BreakTimeView);
            }
        }

        public void RefreshTargetJobInfo(string jobKeySet = "")
        {
            String jobKey = !String.IsNullOrEmpty(jobKeySet) ? jobKeySet : VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            this.Btn_MovingContainer.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0132", LanguageService.LABEL_CUSTOMIZE);
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
                if (!MainView.contItmSelected)
                {
                    this.Btn_ContainerNumber.Content = String.Empty;
                    this.Btn_ContainerNumber.IsEnabled = false;
                }
                return;
            }

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null || jobOrder.type == null)
                return;
            if (jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "GC")
                this.Btn_PartnerNumber.FontSize = 20;
            else
                this.Btn_PartnerNumber.FontSize = 24;
            this.Btn_PartnerNumber.Content = jobOrder.partnerMchn.mchnId;
            if (!MainView.contItmSelected)
                this.Btn_ContainerNumber.Content = jobOrder.cntr.cntrNo;
            //PresentationMgr.MainView.UC_YtSwapView.SetJobInfo(jobOrder);

            //this.Btn_PartnerNumber.IsEnabled = String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId) ? false : true;
            this.Btn_PartnerNumber.IsEnabled = true;
            this.Btn_ContainerNumber.IsEnabled = String.IsNullOrEmpty(jobOrder.cntr.cntrNo) ? false : true;
            
            //if (jobOrder.type.twinTandemFlg.Equals("W"))            
            {
                if (!String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twinJobOrder = PresentationMgr.Singleton.JOB_Get(jobOrder.type.ycTwinKey);
                    if (null != twinJobOrder &&  twinJobOrder.type != null)
                    {
                        //Khoa.Nguyen cmt 2018/02/27 disable button
                        //this.Image_Twin.Visibility = Visibility.Visible;
                        //this.Btn_ContainerNumber_Twin.Content = twinJobOrder.cntr.cntrNo;
                        //this.Btn_ContainerNumber_Twin.IsEnabled = String.IsNullOrEmpty(twinJobOrder.cntr.cntrNo) ? false : true;
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

                PresentationMgr.SetSkinButton(this.Btn_MovingContainer,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Refresh,
                        UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Unlock,
                      UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
                PresentationMgr.SetSkinButton(this.Btn_Available,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
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
                PresentationMgr.SetSkinButton(this.Btn_MovingContainer,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Refresh,
                   UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Unlock,
                  UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
                PresentationMgr.SetSkinButton(this.Btn_Available,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}
