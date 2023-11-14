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

//20190108
using System.Threading;
using Common.Interface;
using Microsoft.Win32;
using static Common.Util.Registry64;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for MachineSettingView.xaml
    /// </summary>
    public partial class MachineSettingView : UserControl
    {
        public String langString = String.Empty;
        private String _VersionInfo = String.Empty;
        private String _BuildInfo = String.Empty;
        private string _UISize = "";
        private string oldUpdateurl = "";

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
            this.GetUpdateInfo();

            LoadLanguage();
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
            _VersionInfo = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0044", LanguageService.MESSAGE_GROUP) + " {0}.{1}.{2}.{3}", vn[0], vn[1], vn[2], vn[3]);

            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(MainWindow.KeyCLTVMT_RMG);
            RegistryKey keyDir = _openSubKey(Registry.LocalMachine, MainWindow.KeyCLTVMT_RMG, false, Is64Bit() ? RegWow64Options.KEY_WOW64_64KEY : RegWow64Options.KEY_WOW64_32KEY);
            string strPackageDate = "";
            if (keyDir != null)
                strPackageDate = (String)keyDir.GetValue("PackageTime", @"Unknown");

            _BuildInfo = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0045", LanguageService.MESSAGE_GROUP) + ": {0}\r\n", strPackageDate);

            if (App.TEST_MODE)
                _BuildInfo += PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0046", LanguageService.MESSAGE_GROUP);
            else
                _BuildInfo += PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0047", LanguageService.MESSAGE_GROUP);

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
        
        private void GetUpdateInfo()
        {
            try
            {
                string cfgFile = getProductListFromCLTAgent();
                if (string.IsNullOrEmpty(cfgFile))
                    return;

                XmlDocument xml = new XmlDocument();
                xml.Load(cfgFile);

                XmlNode xnode = null;

                if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
                    xnode = xml.SelectSingleNode("//Product[@name='VMT-RTG for "+ MainWindow.Project +"']/UpdateServer/text()");

                if (xnode != null)
                {
                    String retAddress = xnode.InnerText;

                    retAddress = retAddress.Replace("sftp://", "");
                    retAddress = retAddress.Replace("ftp://", "");
                    retAddress = retAddress.Replace("FTP://", "");
                    retAddress = retAddress.Substring(0, retAddress.IndexOf('/'));

                    var arrAddress = retAddress.Split(':');
                    oldUpdateurl = retAddress;

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

        private string getProductListFromCLTAgent()
        {
            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(App.KeyCLTAgent);
            RegistryKey keyDir = _openSubKey(Registry.LocalMachine, App.KeyCLTAgent, false, Is64Bit() ? RegWow64Options.KEY_WOW64_64KEY : RegWow64Options.KEY_WOW64_32KEY);
            if (keyDir == null)
                return "";

            String CLTAgentDir = (String)keyDir.GetValue("InstallDir", @"C:\CLT\Applications\Common\CLTAgent");
            CLTAgentDir = CLTAgentDir.Replace("\"", "");
            String cfgFile = CLTAgentDir + @"\" + "ProductList.xml";
            if (!System.IO.File.Exists(cfgFile))
                return "";

            return cfgFile;
        }

        private void UpdateMachine()
        {
            try
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SaveIni value = new VMT_Data_JAT2.Objects.Common.VD_Common_SaveIni();
                value.MchnType = this.TextBox_MachineType.Text;
                value.MchnID = this.TextBox_MachineID.Text;
                value.UISize = Rad_Size1024.IsChecked == true ? "1024" : "800";
                
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
        
        private void SetUpdateServerInfo_XML(String ip, String port)
        {
            if (String.IsNullOrEmpty(ip) || String.IsNullOrEmpty(port))
                return;
            try
            {
                string cfgFile = getProductListFromCLTAgent();
                if (string.IsNullOrEmpty(cfgFile))
                    return;

                XmlDocument xml = new XmlDocument();
                xml.Load(cfgFile);

                XmlNode xnode = null;
                XmlNode xnode1 = null;

                if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
                {
                    xnode = xml.SelectSingleNode("//Product[@name='VMT-RTG for "+ MainWindow.Project +"']/UpdateServer/text()");
                    xnode1 = xml.SelectSingleNode("//Product[@name='VMT-RTG for " + MainWindow.Project + "']/ProvisionServer/text()");
                }

                if (xnode != null)
                {
                    int val = 0;
                    string newIPAdd = Int32.TryParse(port, out val) ?  ip +":"+port : ip;
                    xnode.InnerText = xnode.InnerText.Replace(oldUpdateurl, newIPAdd);
                    xnode1.InnerText = xnode1.InnerText.Replace(oldUpdateurl, newIPAdd);
                    xml.Save(cfgFile);
                }
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

            String strUpdateSvrCfgPath = AppCfgMgr.GetAppDirectory() + "UpdateSvrCfg.cmd";

            if (!File.Exists(strUpdateSvrCfgPath))
                return;

            String updateCmd = System.IO.File.ReadAllText(strUpdateSvrCfgPath);
            updateCmd = updateCmd.Replace("$(CLTAGENT_DIR)", CLTAgentDir);
            updateCmd = updateCmd.Replace("$(SERVER_ADDRESS)", ip + ":" + port);

            int cmdEnd = updateCmd.IndexOf(' ');
            String cmd = updateCmd.Substring(0, cmdEnd);
            cmdEnd++;
            String arguments = updateCmd.Substring(cmdEnd, updateCmd.Length - cmdEnd);

            PresentationMgr.ExecuteProcess(cmd, arguments);
        }

        private void TextBox_MachineID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void LoadLanguage()
        {

            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            Label_LanguageFileName.Text = ini.IniReadValue("MACHINE", "LanguagePath");

            langString = Label_LanguageFileName.Text;
            langString = String.IsNullOrEmpty(langString) ? "" : langString.Substring(langString.LastIndexOf("\\") + 1);

            _UISize = ini.IniReadValue("MACHINE", "UISIZE");
            if (_UISize == "1024" || _UISize == "")
            {
                Rad_Size1024.IsChecked = true;
                Rad_Size1280.IsChecked = false;
            }
            else
            {
                Rad_Size1024.IsChecked = false;
                Rad_Size1280.IsChecked = true;
            }

            this.Button_Select_Language.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0100", LanguageService.LABEL_CUSTOMIZE);
            this.Label_MachineType.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0014", LanguageService.LABEL_SETTING);
            this.Label_MachineID.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0015", LanguageService.LABEL_SETTING);
            this.Button_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0016", LanguageService.LABEL_SETTING);
        }
        private void Button_Select_Language_Clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog oFile = new System.Windows.Forms.OpenFileDialog();
            oFile.Filter = "xml files (*.xml)|*.xml";
            if (oFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = oFile.FileName;
                string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                ini.IniWriteValue("MACHINE", "LanguagePath", fileName);
                PresentationMgr.Singleton.LanguageSer.ReloadLanguage();
                LoadLanguage();
            }
        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.UpdateMachine();

            var tosIP = TextBox_Tos_IP1.Text + "." + TextBox_Tos_IP2.Text + "." + TextBox_Tos_IP3.Text + "." + TextBox_Tos_IP4.Text;
            var tosPort = TextBox_Tos_Port.Text;
            this.UpdateTosInfo(tosIP, tosPort);
            
            var updateIP = TextBox_Update_IP1.Text + "." + TextBox_Update_IP2.Text + "." + TextBox_Update_IP3.Text + "." + TextBox_Update_IP4.Text;
            var updatePort = TextBox_Update_Port.Text;
            this.SetUpdateServerInfo_XML(updateIP, updatePort);

            String popupButtons = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP);
            String popupMsg = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0037", LanguageService.MESSAGE_GROUP);

            PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_SystemOff, PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0038", LanguageService.MESSAGE_GROUP),
                popupMsg, popupButtons, new UC_PopupView.Callback_Popup(CallBack_Popup), 3);
        }

        public void CallBack_Popup(UC_PopupView.UC_PopupViewRetType selected)
        {
            MainWindow.dtClockTime.Stop();
            if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
            {
                PresentationMgr.AppWin.PLCTimer.Stop();
            }
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
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.LogInView);

        }

        private void Btn_ShowLogWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!MainWindow.LogWin.IsVisible)
            {
                MainWindow.LogWin.Show();
                this.Btn_ShowLogWindow.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0039", LanguageService.MESSAGE_GROUP);
            }
            else
            {
                MainWindow.LogWin.Hide();
                this.Btn_ShowLogWindow.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0040", LanguageService.MESSAGE_GROUP);
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