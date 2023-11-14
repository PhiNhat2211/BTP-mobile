using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
//using ExternalAPI;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;
using VMT_RMG;

namespace VMT_RMG_800by600
{

    public class EventMgr : UIElement
    {

        #region [Singleton Pattern Implementation]
        private static readonly EventMgr _theOnly = null;

        public static EventMgr Singleton
        {
            get { return _theOnly; }
        }

        static EventMgr()
        {
            _theOnly = new EventMgr();
        }

        private EventMgr()
        {
        }

        #endregion


        public bool IsAltPressed = false;



        //-----------------------------------------------------------------
        //- Application Keyboard Input Events
        //-----------------------------------------------------------------
        #region [Application Keyboard Input ]

        public void Process_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.Print("Modifier : {0},  Key : {1} ", Keyboard.Modifiers, e.Key);

            ProcessKey(sender, e);

            // ProcessKeyInApp(sender, e);
        }

        private void ProcessKey(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                switch (e.Key)
                {   
                    case Key.S:
                        LoadInterfaceMessage_Schdule();
                        break;
                    case Key.P:
                        PlayInterfaceMessage_Schdule();
                        break;
                    case Key.L:
                        LoadInterfaceMessage_StepByStep();
                        break;
                    case Key.F7:
                        ExecuteInterfaceMessage_Prev();
                        break;
                    case Key.F8:
                        ExecuteInterfaceMessage();
                        break;
                    case Key.F9:
                        ExecuteInterfaceMessage_Next();
                        break;

                    case Key.I: // Warning Icon
                        ExcuteShowWarningIconSetPopup();
                        break;

                    case Key.D: // Show Popup Dialog
                        ExcuteShowPopupDialog();
                        break;

                    case Key.T: // Test Commnad
                        ExcuteTestCommand();
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion [Application Keyboard Input]


        #region [Common Helper Method]

        #endregion [Common Helper Method]

        private void SendTestJobOrder(string strValue)
        {
            // PresentationMgr.MainView.ProcessByJobDeleteAllCallback(2);
        }

        private void LoadInterfaceMessage_Schdule()
        {
            Microsoft.Win32.OpenFileDialog dlgOpen = new Microsoft.Win32.OpenFileDialog();
            dlgOpen.DefaultExt = ".xml";
            dlgOpen.Filter = "Select the xml file to load (.xml)|*.xml";

            Nullable<bool> resultOpen = dlgOpen.ShowDialog();
            if (resultOpen == true)
            {
                string strFileName = dlgOpen.FileName;
                InterfaceMessageLoader.instance().InterfaceMessageSchedule_Load(strFileName);
                InterfaceMessageLoader.instance().InterfaceMessageSchedule_Play();
            }
        }

        private void PlayInterfaceMessage_Schdule()
        {
            InterfaceMessageLoader.instance().InterfaceMessageSchedule_Play();
        }

        private void LoadInterfaceMessage_StepByStep()
        {
            Microsoft.Win32.OpenFileDialog dlgOpen = new Microsoft.Win32.OpenFileDialog();
            dlgOpen.DefaultExt = ".xml";
            dlgOpen.Filter = "Select the xml file to load (.xml)|*.xml";

            Nullable<bool> resultOpen = dlgOpen.ShowDialog();
            if (resultOpen == true)
            {
                string strFileName = dlgOpen.FileName;
                InterfaceMessageLoader.instance().LoadInterfaceMessageXml(strFileName);
            }
        }

        private void ExecuteInterfaceMessage()
        {
            if (InterfaceMessageLoader.instance().isLoaded())
                InterfaceMessageLoader.instance().ExcuteInterfaceMessage();
        }

        private void ExecuteInterfaceMessage_Next()
        {
            if (InterfaceMessageLoader.instance().isLoaded())
                InterfaceMessageLoader.instance().NextInterfaceMessage();
        }

        private void ExecuteInterfaceMessage_Prev()
        {
            if (InterfaceMessageLoader.instance().isLoaded())
                InterfaceMessageLoader.instance().PrevInterfaceMessage();
        }

        private void ExcuteShowWarningIconSetPopup()
        {
            //PresentationMgr.MainView.WarningIconSetPopup.Visibility = System.Windows.Visibility.Visible;
        }

        private void ExcuteShowPopupDialog()
        {
            String RegAddr = @"SOFTWARE\CyberLogitec\TempPopup";

            String strType = "0";
            String strTitle = "Title";
            String strMessage = "Message";
            String strLeftButton = "LetfButton";
            String strCenterButton = "CenterButton";
            String strRightButton = "RightButton";
            String strSec = "0";

            Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegAddr);

            if (keyDir != null)
            {
                strType = (String)keyDir.GetValue("Type", @"Type");
                strTitle = (String)keyDir.GetValue("Title", @"Title");
                strMessage = (String)keyDir.GetValue("Message", @"Message");

                strLeftButton = (String)keyDir.GetValue("LeftButton", @"LeftButton");
                strCenterButton = (String)keyDir.GetValue("CenterButton", @"CenterButton");
                strRightButton = (String)keyDir.GetValue("RightButton", @"RightButton");
                strSec = (String)keyDir.GetValue("Sec", @"Sec");

                //PresentationMgr.AppWin.PopupView.ShowPopup(Convert.ToInt32(strType),
                //    strTitle, strMessage,
                //    strLeftButton, strCenterButton, strRightButton,
                //    CallBack_ShowPopupDialog, Convert.ToInt32(strSec));
            }
        }

        public void CallBack_ShowPopupDialog(int seleted)
        {

        }

        private void AutoRefresh()
        {
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(PresentationMgr.Singleton.CurrentBlock);

            //if (String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBlock) || String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
            //    return;

            //VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            //value.blck = PresentationMgr.Singleton.CurrentBlock;
            //if (!String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay))
            //{
            //    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(PresentationMgr.Singleton.CurrentBlock);
            //    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(PresentationMgr.Singleton.CurrentBlock, PresentationMgr.Singleton.CurrentBay, PresentationMgr.Singleton.CurrentBay);

            //    var intBay = Convert.ToInt32(PresentationMgr.Singleton.CurrentBay);
            //    if (intBay > 2 && intBay % 2 != 0)
            //    {
            //        value.bay = Convert.ToString(intBay - 2);
            //        if (value.bay.Length <= 1)
            //            value.bay = "0" + value.bay;
            //        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListEx_Ask(value);
            //    }
            //    else
            //    {
            //        value.bay = PresentationMgr.Singleton.CurrentBay;
            //        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryList_Ask(value);
            //    }
            //}
            //else
            //{
            //    value.bay = PresentationMgr.Singleton.CurrentBay;
            //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListBackground_Ask(value);
            //}
        }

        public void timerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            AutoRefresh();
                        }));
        }

        //private Boolean _isPlaying = false;
        private void ExcuteTestCommand()
        {
            //_isPlaying = !_isPlaying;
            //ViewModel.Singleton.isPlaying = _isPlaying;

            //if( PresentationMgr.Singleton.CurrentUITheme != PresentationMgr.UITheme.UITheme_Day)
            //    PresentationMgr.Singleton.CurrentUITheme = PresentationMgr.UITheme.UITheme_Day;
            //else if (PresentationMgr.Singleton.CurrentUITheme != PresentationMgr.UITheme.UITheme_Night)
            //    PresentationMgr.Singleton.CurrentUITheme = PresentationMgr.UITheme.UITheme_Night;

            //System.Timers.Timer _timer = new System.Timers.Timer(10000); // 10Sec
            //_timer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsedFunc);
            //_timer.AutoReset = true;
            //_timer.Start();

            //double dOpacity = PresentationMgr.AppWin.PopupProgressView.BG_PopupProgress.Opacity;
            //dOpacity += 0.1;
            //if (dOpacity >= 0.95)
            //    dOpacity = 0.0;
            //PresentationMgr.AppWin.PopupProgressView.Visibility = System.Windows.Visibility.Visible;
            //PresentationMgr.AppWin.PopupProgressView.BG_PopupProgress.Opacity = dOpacity;
            //PresentationMgr.AppWin.PopupProgressView.ShowProgressText(true, ((dOpacity) * 100).ToString() + " %");
        }
    }
}
