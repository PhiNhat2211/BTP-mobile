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
using System.Xml;

using VMT_Data_JAT2;
using System.Diagnostics;

//20190108
using Common.Interface;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for MachineInfo.xaml
    /// </summary>
    public partial class CalibrationInfo : UserControl
    {
        String ip;
        String port;
        MainWindow mMainWindow;

        //----------------------------------------------
        //- Default Machine Configuration
        String strConstNull = @"NULL";
        String initDGPSDirection = "";

        public CalibrationInfo()
        {
            this.InitializeComponent();
        }

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        public void SetDGPSDirectionPinPolarity()
        {
            //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Request_CFG_EKF);
            VMT_Data_JAT2.VMT_DataMgr_Common.RequestCfgekf_Ask();
        }

        public void GetUpdateVale()
        {

        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SendDGPSDirectionPinPolarity();

            mMainWindow.HideCalibrationView();
        }

        public void CallBack_Popup(int seleted)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
            VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown, mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-').Length == 2 ? mMainWindow.IndicatorView.Lbl_YtNoChssNo.Content.ToString().Split('-')[1] : "");

            //--------------------------------------------------------------
            //- Kill VMT-ITV Tray Process, it will restart by launcher
            foreach (var process in Process.GetProcessesByName("EE_Sensor_JAEXE2.exe"))
            {
                process.Kill();
            }

            //--------------------------------------------------------------
            //- Save Touch Event for CLTAgent to restart application
            //PresentationMgr.FileTouchEvent_RestartApp();
            PresentationMgr.App_RestartApp();

            PresentationMgr.APP_CloseApp();
        }

        private void Button_Init_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mMainWindow.ShowCalibrationInitPopup();
        }

        public void CalibrationInit()
        {
            mMainWindow.ShowProgressBarTimer(210); // 3분 30초

            // Reset GPS Device
            // VMT_Data_JAT2.Common.Send_CFG_RST();
            //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_CFG_RST);
            VMT_Data_JAT2.VMT_DataMgr_Common.ResetDGPS_Ask();
        }

        public void End_Device_Init()
        {
            if (MainWindow.SERVICE_COMPANY.Equals("JAT3"))
                VMT_Data_JAT2.VMT_DataMgr_Common.SendHighBackward_Ask();
            else if (MainWindow.SERVICE_COMPANY.Equals("JAT2"))
                VMT_Data_JAT2.VMT_DataMgr_Common.SendHighForward_Ask();

            //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_CFG_SAVE);
            VMT_Data_JAT2.VMT_DataMgr_Common.SaveDGPSCfg_Ask();

            mMainWindow.PopupView.ShowPopup(0, PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0038", LanguageService.MESSAGE_GROUP)
                , PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0126", LanguageService.LABEL_CUSTOMIZE), "", "", "", CallBack_Popup, 3);
        }

        public void ProcessByCFGEKFCallback(VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive value)
        {
            tbCaliState.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0127", LanguageService.LABEL_CUSTOMIZE);
            tbValues.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0128", LanguageService.LABEL_CUSTOMIZE);
            tbSpeedPulse.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0129", LanguageService.LABEL_CUSTOMIZE) + " ";
            tbPulse.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0130", LanguageService.LABEL_CUSTOMIZE) + " ";
            tbGyroScaleFactor.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0131", LanguageService.LABEL_CUSTOMIZE) + " ";
            tbGyro.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0132", LanguageService.LABEL_CUSTOMIZE) + " ";
            tbGyroBias.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0133", LanguageService.LABEL_CUSTOMIZE) + " ";
            tbTemp.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0134", LanguageService.LABEL_CUSTOMIZE) + " ";
            Button_Init.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0135", LanguageService.LABEL_CUSTOMIZE);
            Button_OK.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_POPUP);

            if (MainWindow.SERVICE_COMPANY.Equals("JAT3"))
            {
                // ( 0: HighForwards, 1: HighBackwards )
                if (value.m_iDirection == 0)
                {
                    mMainWindow.PopupView.ShowPopup(0, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0136", LanguageService.LABEL_CUSTOMIZE)
                        , PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0137", LanguageService.LABEL_CUSTOMIZE), "", "", "", null, 3, 24);
                    // VMT_Data_NCT.Common.Send_HighBackward();
                    // VMT_Data_NCT.Common.Send_CFG_SAVE();
                    // VMT_DataMgr.SendEEClient_ByTCP(VMT_DataMgr.EEClient.Send_HighBackward);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendHighBackward_Ask();
                    // VMT_DataMgr.SendEEClient_ByTCP(VMT_DataMgr.EEClient.Send_CFG_SAVE);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SaveDGPSCfg_Ask();

                    // initDGPSDirection = "HighForward";
                    // RadioButton_HighForward.IsChecked = true;
                }

                initDGPSDirection = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0138", LanguageService.LABEL_CUSTOMIZE);
                RadioButton_HighBackward.IsChecked = true;
            }
            else if (MainWindow.SERVICE_COMPANY.Equals("JAT2"))
            { 
                // ( 0: HighForwards, 1: HighBackwards )
                if (value.m_iDirection == 1)
                {
                    mMainWindow.PopupView.ShowPopup(0, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0136", LanguageService.LABEL_CUSTOMIZE)
                        , PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0137", LanguageService.LABEL_CUSTOMIZE), "", "", "", null, 3, 24);
                    // VMT_Data_NCT.Common.Send_HighBackward();
                    // VMT_Data_NCT.Common.Send_CFG_SAVE();
                    // VMT_DataMgr.SendEEClient_ByTCP(VMT_DataMgr.EEClient.Send_HighBackward);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendHighForward_Ask();
                    // VMT_DataMgr.SendEEClient_ByTCP(VMT_DataMgr.EEClient.Send_CFG_SAVE);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SaveDGPSCfg_Ask();

                    // initDGPSDirection = "HighForward";
                    // RadioButton_HighForward.IsChecked = true;
                }

                initDGPSDirection = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0139", LanguageService.LABEL_CUSTOMIZE);
                RadioButton_HighForward.IsChecked = true;
            }
        }

        private void SendDGPSDirectionPinPolarity()
        {
            if (RadioButton_HighBackward.IsChecked == true)
            {
                MainWindow.DGPSDirectionPinPolarity = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0138", LanguageService.LABEL_CUSTOMIZE);

                if (initDGPSDirection != PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0138", LanguageService.LABEL_CUSTOMIZE))
                {
                    // VMT_Data_JAT2.Common.Send_HighBackward();
                    // VMT_Data_JAT2.Common.Send_CFG_SAVE();
                    //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_HighBackward);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendHighBackward_Ask();
                    //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_CFG_SAVE);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SaveDGPSCfg_Ask();
                }
            }
            else if (RadioButton_HighForward.IsChecked == true)
            {
                MainWindow.DGPSDirectionPinPolarity = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0139", LanguageService.LABEL_CUSTOMIZE);

                if (initDGPSDirection != PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0139", LanguageService.LABEL_CUSTOMIZE))
                {
                    // VMT_Data_JAT2.Common.Send_HighForward();
                    // VMT_Data_JAT2.Common.Send_CFG_SAVE();
                    //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_HighForward);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SendHighForward_Ask();
                    //VMT_DataMgr.SendEEClient_ByUDP(VMT_DataMgr.EEClient.Send_CFG_SAVE);
                    VMT_Data_JAT2.VMT_DataMgr_Common.SaveDGPSCfg_Ask();
                }
            }
            else
            {
            }
        }

        private void RadioButton_HighBackward_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // SendDGPSDirectionPinPolarity();
        }

        private void RadioButton_HighForward_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // SendDGPSDirectionPinPolarity();
        }
    }
}