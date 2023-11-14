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
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for ChangePopupView.xaml
    /// </summary>
    public partial class ChangeDriverPopupView : UserControl
    {
        public bool ChangeDriverClicked = false;
        public bool SetLogoutFromChangeDriver = false;
        public bool SetLoginFromChangeDriver = false;
        VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send available = new VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send();
        public ChangeDriverPopupView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextBlock_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0020", LanguageService.LABEL_CHANGEDRIVER); // Change Driver
            this.TextBlock_Start_Time.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0021", LanguageService.LABEL_CHANGEDRIVER); // Start Time
            this.TextBlock_End_Time.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0022", LanguageService.LABEL_CHANGEDRIVER); // End Time
            this.Tbl_Id.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0023", LanguageService.LABEL_CHANGEDRIVER); // ID:
            this.Tbl_Password.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0024", LanguageService.LABEL_CHANGEDRIVER); // PW:
            this.btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0025", LanguageService.LABEL_CHANGEDRIVER); // CANCEL
            this.btn_Change.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0026", LanguageService.LABEL_CHANGEDRIVER); // Change Complete

            this.Tb_Id.TextChanged += new TextChangedEventHandler(Tb_Id_TextChanged);
            this.Tb_Id.GotFocus += new RoutedEventHandler(Tb_Id_GotFocus);
            this.Pwb_Password.GotFocus += new RoutedEventHandler(Pwb_Password_GotFocus);
            this.Pwb_Password.PasswordChanged += new RoutedEventHandler(Pwb_Password_PasswordChanged);
            this.btn_Change.Click += new RoutedEventHandler(btn_Change_Click);
            this.btn_Cancel.Click += new RoutedEventHandler(btn_Cancel_Click);
        }
        public void ShowPopup()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ResetGridRow();
            CheckButton();
        }
        public void HidePopup()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            PresentationMgr.AppWin.KeypadView.HideKeyPad();
            ClearText();
        }
        private void CheckButton()
        {
            if (this.Tb_Id.Text.Length > 0 && this.Pwb_Password.Password.Length > 0)
            {
                this.btn_Change.IsEnabled = true;
                this.btn_Cancel.IsEnabled = false;
            }
            else
            {
                this.btn_Change.IsEnabled = false;
                this.btn_Cancel.IsEnabled = true;
            }
        }
        private void ClearText()
        {
            this.Tb_Id.Text = String.Empty;
            this.Pwb_Password.Password = String.Empty;
        }
        private void Tb_Id_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckButton();
        }
        private void Tb_Id_GotFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.KeypadView.ShowKeyPad(Tb_Id);
            Grid.SetRow(Grid_Change, 0);
            Tb_Id.Focus();
        }
        private void Pwb_Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckButton();
        }
        private void Pwb_Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.KeypadView.ShowKeyPad(Pwb_Password);
            Grid.SetRow(Grid_Change, 0);
            Pwb_Password.Focus();
        }
        public void ResetGridRow()
        {
            Grid.SetRow(Grid_Change, 1);
        }
        private void btn_Change_Click(object sender, RoutedEventArgs e)
        {
            if ("CLT".Equals(Tb_Id.Text) && "ACCESS".Equals(Pwb_Password.Password))
            {
                ProcessChangeDriverCheck();
            }
            else
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send();
                value.UserID = Tb_Id.Text;
                value.UserPW = Pwb_Password.Password;
                value.MchnID = ITV.ITV_User.gMchnID;
                value.MchnTp = ITV.ITV_User.gMchnTp;
                value.chssNo = PresentationMgr.AppWin.preChassisCd;

                VMT_Data_JAT2.VMT_DataMgr_Common.ChangeDriverCheck_Ask(value);
            }          
        }
        public void ProcessChangeDriverCheck()
        {
            this.SetLogoutFromChangeDriver = true;
            HessianComm.Objects.LogOut logOut = new HessianComm.Objects.LogOut();
            logOut.user = new HessianComm.Objects.User();
            logOut.user.usrId = UserInfo.gUserID;
            logOut.machine = new HessianComm.Objects.Machine();
            logOut.machine.mchnId = UserInfo.gMchnID;
            logOut.machine.mchnTp = UserInfo.gMchnTp;
            logOut.machine.chssNo = PresentationMgr.AppWin.preChassisCd;
            logOut.rsnCd = null;
            logOut.logoutCheck = "F";

            HessianComm.HessianAPI.SetLogout4Machine(logOut);
        }
        public void ProcessSetLogout4MachineCallBack()
        {
            this.SetLoginFromChangeDriver = true;

            if ("CLT".Equals(Tb_Id.Text) && "ACCESS".Equals(Pwb_Password.Password))
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive driverInfoRpValue = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive();
                driverInfoRpValue.iLogin = 1;
                driverInfoRpValue.UserName = "CLT";

                if (VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine != null)
                    VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine(ref driverInfoRpValue);
            }
            else
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send();
                value.UserID = Tb_Id.Text;
                value.UserPW = Pwb_Password.Password;
                value.MchnID = ITV.ITV_User.gMchnID;
                value.MchnTp = ITV.ITV_User.gMchnTp;
                value.chssNo = PresentationMgr.AppWin.preChassisCd;

                VMT_Data_JAT2.VMT_DataMgr_Common.Login4Machine_Ask(ref value);
            }            
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeDriverClicked = true;
            DateTime.TryParse(TextBlock_Break_Start_Date.Text, out DateTime startDate);
            DateTime.TryParse(TextBlock_Break_End_Date.Text, out DateTime endDate);

            TimeSpan startTs = startDate - new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan endTs = endDate - new DateTime(1970, 1, 1, 0, 0, 0);

            available.m_StartTime = Convert.ToInt64(startTs.TotalSeconds);
            available.m_FinishTime = Convert.ToInt64(endTs.TotalSeconds);
            available.m_iBreakStatus = 0;

            VMT_DataMgr_Common.SetMachineStop_Ask(ref available);

            this.HidePopup();
        }
        public void ProcessByGetMachineStopCallback(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value)
        {
            this.TextBlock_Break_Start_Date.Text = DateTime.ParseExact(Convert.ToString(value.StartTime), "yyyyMMddHHmmss", null).ToString("yyyy/MM/dd HH:mm:ss");
            available.Data = value.Data;
        }
        public void ProcessByLogin4MachineCallback()
        {                 
            DateTime.TryParse(TextBlock_Break_Start_Date.Text, out DateTime startDate);
            DateTime.TryParse(TextBlock_Break_End_Date.Text, out DateTime endDate);

            TimeSpan startTs = startDate - new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan endTs = endDate - new DateTime(1970, 1, 1, 0, 0, 0);

            available.m_StartTime = Convert.ToInt64(startTs.TotalSeconds);
            available.m_FinishTime = Convert.ToInt64(endTs.TotalSeconds);
            available.m_iBreakStatus = 0;

            VMT_DataMgr_Common.SetMachineStop_Ask(ref available);
        }
    }
}
