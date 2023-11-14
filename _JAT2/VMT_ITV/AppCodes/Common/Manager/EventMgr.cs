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
using System.Runtime.InteropServices;
using System.Windows.Input;

using VMT_Data_JAT2;


namespace VMT_ITV
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
                    
                    case Key.T:
                        ExcuteTestFunction();
                        break;

                    //Function key
                    case Key.Insert: // Speed Up
                        SendSpeedUp();
                        break;
                    case Key.Delete: // Speed Down
                        SendSpeedDown();
                        break;

                    case Key.Home: // Cone Icon Enable
                        SendConeIconEnable();
                        break;
                    case Key.End: // Cone Icon Disable
                        SendConeIconDisable();
                        break;

                    case Key.PageUp: // Fuel Icon Enable
                        SendFuelIconEnable();
                        break;
                    case Key.PageDown: // Fuel Icon Disable
                        SendFuelIconDisable();
                        break;

                    default:
                        break;
                }
            }
        }

        private int _nSpeed = 0;
        private void SendSpeedUp()
        {
            if( _nSpeed < 99) // 2 Digit
                _nSpeed++;

            PresentationMgr.MainView.ProcessBySpeedKmCallback(_nSpeed);
        }

        private void SendSpeedDown()
        {
            if (_nSpeed > 0) // 2 Digit
                _nSpeed--;

            PresentationMgr.MainView.ProcessBySpeedKmCallback(_nSpeed);
        }

        private void SendConeIconEnable()
        {
            PresentationMgr.MainView.ProcessByEngineTempCallback(1);
        }

        private void SendConeIconDisable()
        {
            PresentationMgr.MainView.ProcessByEngineTempCallback(0);
        }

        private void SendFuelIconEnable()
        {
            // miFuelGage : 80 default
            PresentationMgr.MainView.ProcessByFuelGageCallback(40);
        }

        private void SendFuelIconDisable()
        {
            PresentationMgr.MainView.ProcessByFuelGageCallback(100);
        }
        #endregion [Application Keyboard Input]


        #region [Common Helper Method]

        #endregion [Common Helper Method]

        private void ExcuteTestFunction()
        {
            //VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
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
            PresentationMgr.MainView.WarningIconSetPopup.Visibility = System.Windows.Visibility.Visible;
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

                PresentationMgr.AppWin.PopupView.ShowPopup(Convert.ToInt32(strType),
                    strTitle, strMessage,
                    strLeftButton, strCenterButton, strRightButton,
                    CallBack_ShowPopupDialog, Convert.ToInt32(strSec));
            }
        }

        public void CallBack_ShowPopupDialog(int seleted)
        {
            
        }
    }
}
