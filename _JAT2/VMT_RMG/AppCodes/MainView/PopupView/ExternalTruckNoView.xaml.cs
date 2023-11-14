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
using Common.Interface;


namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for ExternalTruckNoView.xaml
    /// </summary>
    public partial class ExternalTruckNoView : UserControl
    {
        public string externalTruck = "N";
        public string ytAssigned = "N";
        public string hideExternalTruckBay = "N";

        public ExternalTruckNoView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitEvent();
            LoadLanguage();
            LoadDefaultStatus();
        }
        private void InitEvent()
        {
            this.Checkbox_ExternalTruckAll.Click += new RoutedEventHandler(Checkbox_ExternalTruckAll_Click);
            this.Checkbox_YTAssigned.Click += new RoutedEventHandler(Checkbox_YTAssigned_Click);
            this.Checkbox_HideExternalTruckBay.Click += new RoutedEventHandler(Checkbox_HideExternalTruckBay_Click);

            this.Grid_Close.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Close_MouseLeftButtonUp);
        }
        private void LoadLanguage()
        {
            this.Tbl_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_DISPLAYOPTION);
            this.Tbl_ExternalTruck.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_DISPLAYOPTION);
            this.Tbl_YTAssigned.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002a", LanguageService.LABEL_DISPLAYOPTION);
            this.Tbl_HideExternalTruckBay.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002b", LanguageService.LABEL_DISPLAYOPTION);
            this.Tbl_All.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_DISPLAYOPTION);

            this.Tbl_Close.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_DISPLAYOPTION);
        }
        private void LoadDefaultStatus()
        {
            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            externalTruck = ini.IniReadValue("MACHINE", "ExternalTruck");
            ytAssigned = ini.IniReadValue("MACHINE", "ytAssigned");
            hideExternalTruckBay = ini.IniReadValue("MACHINE", "hideExternalTruckBay");

            this.Checkbox_ExternalTruckAll.IsChecked = externalTruck == "Y" ? true : false;
            this.Checkbox_YTAssigned.IsChecked = ytAssigned == "Y" ? true : false;
            this.Checkbox_HideExternalTruckBay.IsChecked = hideExternalTruckBay == "Y" ? true : false;
        }
        private void Grid_Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
            {
                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            }
            else
                VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(true, String.Empty, false, false);
            PresentationMgr.Singleton.SendGetInventoryAsk(PresentationMgr.Singleton.CurrentBlock, Convert.ToString(PresentationMgr.Singleton.CurrentBay));
            PresentationMgr.Singleton.oldLstJobOrder.Clear();
            //PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
        }
        private void Checkbox_ExternalTruckAll_Click(object sender, RoutedEventArgs e)
        {
            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            externalTruck = this.Checkbox_ExternalTruckAll.IsChecked == true ? "Y" : "N";
            ini.IniWriteValue("MACHINE", "ExternalTruck", externalTruck);
        }
        private void Checkbox_YTAssigned_Click(object sender, RoutedEventArgs e)
        {
            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            ytAssigned = this.Checkbox_YTAssigned.IsChecked == true ? "Y" : "N";
            ini.IniWriteValue("MACHINE", "ytAssigned", ytAssigned);
        }
        private void Checkbox_HideExternalTruckBay_Click(object sender, RoutedEventArgs e)
        {
            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            hideExternalTruckBay = this.Checkbox_HideExternalTruckBay.IsChecked == true ? "Y" : "N";
            ini.IniWriteValue("MACHINE", "hideExternalTruckBay", hideExternalTruckBay);
        }
    }
}
