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
using System.IO;
using System.Reflection;
using System.Diagnostics;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using VMT_RMG;

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for MachineSettingView.xaml
    /// </summary>
    public partial class MachineSettingView : UserControl
    {        
        private String _VersionInfo = String.Empty;
        private String _BuildInfo = String.Empty;

        public MachineSettingView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~MachineSettingView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            this.InitSkinImage();

            this.Init();
            this.GetAppInfo();
            this.GetTosInfo();
            this.GetSiemensInfo();
            this.GetUpdateInfo();
        }

        public void Init()
        {
            this.TextBox_Tos_IP1.MaxLength = 3;
            this.TextBox_Tos_IP2.MaxLength = 3;
            this.TextBox_Tos_IP3.MaxLength = 3;
            this.TextBox_Tos_IP4.MaxLength = 3;
            this.TextBox_Tos_Port.MaxLength = 5;

            this.TextBox_Siemens_IP1.MaxLength = 3;
            this.TextBox_Siemens_IP2.MaxLength = 3;
            this.TextBox_Siemens_IP3.MaxLength = 3;
            this.TextBox_Siemens_IP4.MaxLength = 3;
            this.TextBox_Siemens_Port.MaxLength = 5;

            this.TextBox_Update_IP1.MaxLength = 3;
            this.TextBox_Update_IP2.MaxLength = 3;
            this.TextBox_Update_IP3.MaxLength = 3;
            this.TextBox_Update_IP4.MaxLength = 3;
            this.TextBox_Update_Port.MaxLength = 5;
        }

        private void GetAppInfo()
        {
            //------------------------------------------
            //- Get Software Version
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string[] vn = ver.Split('.');
            _VersionInfo = String.Format("Software Version {0}.{1}.{2}.{3}", vn[0], vn[1], vn[2], vn[3]);

            Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(MainWindow.KeyCLTVMT_RMG);
            string strPackageDate = "";
            if (keyDir != null)
                strPackageDate = (String)keyDir.GetValue("PackageTime", @"Unknown");

            _BuildInfo = String.Format("Package Date:{0}\r\n", strPackageDate);

            if (App.TEST_MODE)
                _BuildInfo += "Purpose : Test";
            else
                _BuildInfo += "Purpose : Official Release";

            this.TextBox_VersionInfo.Text = _VersionInfo;
            this.TextBox_DetailInfo.Text = _BuildInfo;

            this.TextBox_MachineID.Text = RMG.RMG_User.gMchnID;
            this.TextBox_MachineType.Text = RMG.RMG_User.gMchnTp;
        }        

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Button_OK,
                        UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Button_Cancel,
                        UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_ShowLogWindow,
                        UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Button_OK,
                        UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Button_Cancel,
                        UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_ShowLogWindow,
                        UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void TextBox_MachineID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.TextBox_MachineID);
            this.TextBox_MachineID.Focus();
        }

        //private void TextBox_MachineType_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    
        //    mMainWindow.KeypadView.ShowKeyPad(TextBox_MachineType);
        //    //ps.StartInfo.FileName = @"C:\Windows\System32\osk.exe";
        //    //ps.Start();
        //    TextBox_MachineType.Focus();
        //}

        //private void TextBox_MachineType_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    
        //    mMainWindow.KeypadView.ShowKeyPad(TextBox_MachineType);
        //    //ps.StartInfo.FileName = @"C:\Windows\System32\osk.exe";
        //    //ps.Start();
        //    TextBox_MachineType.Focus();
        //}

        //private void TextBox_MachineType_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
        //    
        //}

        //public String GetUpdateServerAddress()
        //{
        //    String retAddress = "";

        //    // Set Default Update Server Address
        //    Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(MainWindow.KeyCLTAgent);

        //    if (keyDir == null)
        //        return retAddress;

        //    String CLTAgentDir = (String)keyDir.GetValue("InstallDir", @"C:\CLT\Applications\Common\CLTAgent");
        //    CLTAgentDir = CLTAgentDir.Replace("\"", "");
        //    String cfgFile = CLTAgentDir + @"\" + "ProductList.xml";

        //    if (!System.IO.File.Exists(cfgFile))
        //        return retAddress;

        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(cfgFile);

        //    XmlNode xnode = null;

        //    if (MainWindow.SERVICE_COMPANY.Equals("JAT2"))
        //        xnode = xml.SelectSingleNode("//Product[@name='VMT-RMG for JAT2']/UpdateServer/text()");

        //    if (xnode != null)
        //    {
        //        retAddress = xnode.InnerText;

        //        retAddress = retAddress.Replace("ftp://", "");
        //        retAddress = retAddress.Replace("FTP://", "");
        //        retAddress = retAddress.Substring(0, retAddress.IndexOf('/'));
        //    }

        //    return retAddress;
        //}

        private void GetTosInfo()
        {
            try
            {
                var tosIP = VMT_DataMgr.gHessianServerIP;
                var tosPort = VMT_DataMgr.gHessianServerPort;

                var ipArray = tosIP.Split('.');
                if (ipArray.Length >= 4)
                {
                    this.TextBox_Tos_IP1.Text = ipArray[0];
                    this.TextBox_Tos_IP2.Text = ipArray[1];
                    this.TextBox_Tos_IP3.Text = ipArray[2];
                    this.TextBox_Tos_IP4.Text = ipArray[3];
                }
                this.TextBox_Tos_Port.Text = tosPort;
            }
            catch
            {
            }
        }

        private void GetSiemensInfo()
        {
            try
            {
                String strIniFile = this.GetSiemensIniDirectory() + @"SIEMENSInterface.ini";
                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                var siemensIP = ini.IniReadValue("CONNECTION", "SIEMENS_SERVER_IP");
                var siemensPort = ini.IniReadValue("CONNECTION", "SIEMENS_SERVER_PORT");
                ini = null;

                var ipArray = siemensIP.Split('.');
                if (ipArray.Length >= 4)
                {
                    this.TextBox_Siemens_IP1.Text = ipArray[0];
                    this.TextBox_Siemens_IP2.Text = ipArray[1];
                    this.TextBox_Siemens_IP3.Text = ipArray[2];
                    this.TextBox_Siemens_IP4.Text = ipArray[3];
                }
                this.TextBox_Siemens_Port.Text = siemensPort;
            }
            catch
            {
            }
        }

        private void GetUpdateInfo()
        {
            try
            {
                // Set Default Update Server Address
                Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(App.KeyCLTAgent);
                if (keyDir == null)
                    return;

                String CLTAgentDir = (String)keyDir.GetValue("InstallDir", @"C:\CLT\Applications\Common\CLTAgent");
                CLTAgentDir = CLTAgentDir.Replace("\"", "");
                String cfgFile = CLTAgentDir + @"\" + "ProductList.xml";
                if (!System.IO.File.Exists(cfgFile))
                    return;

                XmlDocument xml = new XmlDocument();
                xml.Load(cfgFile);

                XmlNode xnode = null;

                if (MainWindow.SERVICE_COMPANY.Equals("JAT2"))
                    xnode = xml.SelectSingleNode("//Product[@name='VMT-RMG for JAT2']/UpdateServer/text()");

                if (xnode != null)
                {
                    String retAddress = xnode.InnerText;

                    retAddress = retAddress.Replace("ftp://", "");
                    retAddress = retAddress.Replace("FTP://", "");
                    retAddress = retAddress.Substring(0, retAddress.IndexOf('/'));

                    var arrAddress = retAddress.Split(':');
                    if (arrAddress.Length > 0)
                    {
                        var ip = arrAddress[0];
                        var ipArray = ip.Split('.');
                        if (ipArray.Length >= 4)
                        {
                            this.TextBox_Update_IP1.Text = ipArray[0];
                            this.TextBox_Update_IP2.Text = ipArray[1];
                            this.TextBox_Update_IP3.Text = ipArray[2];
                            this.TextBox_Update_IP4.Text = ipArray[3];
                        }

                        if (arrAddress.Length > 1)                        
                            this.TextBox_Update_Port.Text = arrAddress[1];                        
                    }
                }
            }
            catch
            {
            }
        }

        private void UpdateMachine()
        {
            try
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SaveIni value = new VMT_Data_JAT2.Objects.Common.VD_Common_SaveIni();
                value.MchnType = this.TextBox_MachineType.Text;
                value.MchnID = this.TextBox_MachineID.Text;
                VMT_Data_JAT2.VMT_DataMgr_Common.SaveIniFile(ref value);
                value = null;
            }
            catch
            {
            }
        }

        private void UpdateTosInfo(String ip, String port)
        {
            try
            {
                if (!String.IsNullOrEmpty(AppCfgMgr.Singleton.FilePath))
                {
                    AppCfgMgr.Singleton.SetValueByKey("HessianServerIP", ip);
                    AppCfgMgr.Singleton.SetValueByKey("HessianServerPort", port);

                    AppCfgMgr.Singleton.SaveFile();
                }
            }
            catch
            {
            }
        }

        private void UpdateSiemensInfo(String ip, String port)
        {
            try
            {
                String strIniFile = this.GetSiemensIniDirectory() + @"SIEMENSInterface.ini";
                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                ini.IniWriteValue("CONNECTION", "SIEMENS_SERVER_IP", ip);
                ini.IniWriteValue("CONNECTION", "SIEMENS_SERVER_PORT", port);
                ini = null;
            }
            catch
            {
            }
        }

        private void SetUpdateServerInfo(String ip, String port)
        {
            if (String.IsNullOrEmpty(ip) || String.IsNullOrEmpty(port))
                return;

            Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(App.KeyCLTAgent);            
            String CLTAgentDir = "";
            if (keyDir != null)
            {
                CLTAgentDir = (String)keyDir.GetValue("InstallDir", @"C:\CLT\Applications\Common\CLTAgent");
                CLTAgentDir = CLTAgentDir.Replace("\"", "");
            }

            if (String.IsNullOrEmpty(CLTAgentDir))
                CLTAgentDir = @"C:\CLT\Applications\Common\CLTAgent";

            System.IO.Directory.CreateDirectory(CLTAgentDir);

            if (!File.Exists("UpdateSvrCfg.cmd"))
                return;

            String updateCmd = System.IO.File.ReadAllText("UpdateSvrCfg.cmd");
            updateCmd = updateCmd.Replace("$(CLTAGENT_DIR)", CLTAgentDir);
            updateCmd = updateCmd.Replace("$(SERVER_ADDRESS)", ip + ":" + port);

            int cmdEnd = updateCmd.IndexOf(' ');
            String cmd = updateCmd.Substring(0, cmdEnd);
            cmdEnd++;
            String arguments = updateCmd.Substring(cmdEnd, updateCmd.Length - cmdEnd);

            PresentationMgr.ExecuteProcess(cmd, arguments);
        }

        private string GetSiemensIniDirectory()
        {
            String path = null;

            DirectoryInfo dInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            String strParentPath = dInfo.FullName;
            String strSiemensPath = strParentPath + @"\SIEMENS_Interface";

            if (!Directory.Exists(strSiemensPath))
                Directory.CreateDirectory(strSiemensPath);

            path = strSiemensPath + @"\";

            return path;
        }     

        private void TextBox_MachineID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.UpdateMachine();

            var tosIP = TextBox_Tos_IP1.Text + "." + TextBox_Tos_IP2.Text + "." + TextBox_Tos_IP3.Text + "." + TextBox_Tos_IP4.Text;
            var tosPort = TextBox_Tos_Port.Text;
            this.UpdateTosInfo(tosIP, tosPort);

            var siemensIP = TextBox_Siemens_IP1.Text + "." + TextBox_Siemens_IP2.Text + "." + TextBox_Siemens_IP3.Text + "." + TextBox_Siemens_IP4.Text;
            var simensPort = TextBox_Siemens_Port.Text;
            this.UpdateSiemensInfo(siemensIP, simensPort);

            var updateIP = TextBox_Update_IP1.Text + "." + TextBox_Update_IP2.Text + "." + TextBox_Update_IP3.Text + "." + TextBox_Update_IP4.Text;
            var updatePort = TextBox_Update_Port.Text;
            this.SetUpdateServerInfo(updateIP, updatePort);

            String popupButtons = "OK";
            String popupMsg = "Terminate application After 3 Sec";

            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, "Information",
                popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallBack_Popup), 3);
        }

        public void CallBack_Popup(UC_PopupView.UC_PopupViewRetType selected)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown shutDown = new VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown();
            VMT_Data_JAT2.VMT_DataMgr_Common.SendSystemOff_Ask(ref shutDown);      

            // Close Process
            PresentationMgr.APP_CloseApp(true);
        }

        private void TextBox_Ip1_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad((TextBox)sender);
            ((TextBox)sender).Focus();
        }

        

        private void Button_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.Visibility = System.Windows.Visibility.Hidden;
            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.LogInView);
            
        }

        private void Btn_ShowLogWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!MainWindow.LogWin.IsVisible)
            {
                MainWindow.LogWin.Show();
                this.Btn_ShowLogWindow.Content = "Hide Log Window";
            }
            else
            {
                MainWindow.LogWin.Hide();
                this.Btn_ShowLogWindow.Content = "Show Log Window";
            }
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad((TextBox)sender);
            ((TextBox)sender).Focus();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad((TextBox)sender);
            ((TextBox)sender).Focus();
        }
    }
}