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
using System.ComponentModel;
using Common.Interface;
using VMT_Data_JAT2.Objects;
//using ExternalAPI;

namespace VMT_RMG
{
	/// <summary>
	/// Interaction logic for InfomationView.xaml
	/// </summary>
	public partial class InfomationView : UserControl
	{
		public InfomationView()
		{
			this.InitializeComponent();
            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);
            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
                new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
        }

        ~InfomationView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSkinImage();
            InitEvent();
            LoadLanguage();
        }
        private void InitEvent()
        {
            this.TextBlock_userName.Click += new RoutedEventHandler(TextBlock_userName_Click);
            this.CheckBox_All.Click += new RoutedEventHandler(CheckBox_All_Click);
            this.CheckBox_Type_GI.Click += new RoutedEventHandler(CheckBox_Type_GI_Click);
            this.CheckBox_Type_GO.Click += new RoutedEventHandler(CheckBox_Type_GO_Click);
            this.CheckBox_Type_MI.Click += new RoutedEventHandler(CheckBox_Type_MI_Click);
            this.CheckBox_Type_MO.Click += new RoutedEventHandler(CheckBox_Type_MO_Click);
            this.CheckBox_Type_DS.Click += new RoutedEventHandler(CheckBox_Type_DS_Click);
            this.CheckBox_Type_LD.Click += new RoutedEventHandler(CheckBox_Type_LD_Click);
            this.CheckBox_Type_RH.Click += new RoutedEventHandler(CheckBox_Type_RH_Click);
            this.TextBlock_loginLong.Click += new RoutedEventHandler(TextBlock_loginLong_Click);
            this.TextBlock_jobCount.Click += new RoutedEventHandler(TextBlock_jobCount_Click);
        }
        private void LoadLanguage()
        {
            this.CheckBox_All.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("ALL", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_GI.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("GI", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_GO.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("GO", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_MI.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MI", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_MO.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MO", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_DS.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("DS", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_LD.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LD", LanguageService.LABEL_JOBTYPE);
            this.CheckBox_Type_RH.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("RH", LanguageService.LABEL_JOBTYPE);
            this.TextBlock_loginTime.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0070", LanguageService.LABEL_MAINWINDOW) + " " + "00:00"; //Login Time: 00:00
            this.TextBlock_loginLong.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0072", LanguageService.LABEL_MAINWINDOW) + " 0 " + PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0073", LanguageService.LABEL_MAINWINDOW); //Start 0 min
        }
        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.TextBlock_userName,
                UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.TextBlock_loginTime,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.TextBlock_loginLong,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.TextBlock_jobCount,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDisableImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.TextBlock_userName,
                UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.TextBlock_loginTime,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.TextBlock_loginLong,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);

                PresentationMgr.SetSkinButton(this.TextBlock_jobCount,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDisableImage);
            }
        }

        private void checkCheckBoxType()
        {
            int checkCnt = 0;
            string[] type = {"GI", "GO", "MI", "MO", "DS", "LD", "RH"};
            for (int i = 0; i < type.Length; i++)
            {
                CheckBox preItem = PresentationMgr.FindChild<CheckBox>(this, "CheckBox_Type_" + type[i]);
                if(preItem.IsChecked == true)
                    checkCnt++;
            }
            if (checkCnt == 0)
                CheckBox_All_Click(null, null);
        }
        private void TextBlock_userName_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.MainView.UC_ExternalTruckNoView.Visibility = Visibility;
        }
        private void CheckBox_All_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_Type_GI.IsChecked = false;
            this.CheckBox_Type_GO.IsChecked = false;
            this.CheckBox_Type_MI.IsChecked = false;
            this.CheckBox_Type_MO.IsChecked = false;
            this.CheckBox_Type_DS.IsChecked = false;
            this.CheckBox_Type_LD.IsChecked = false;
            this.CheckBox_Type_RH.IsChecked = false;
            //this.CheckBox_Status_Active.IsChecked = false;

            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = JobFilter.TYPE_ALL;

            if (this.CheckBox_All.IsChecked == false)
                this.CheckBox_All.IsChecked = true;
            
            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
        }

        private void CheckBox_Type_RH_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = (true == this.CheckBox_Type_RH.IsChecked) ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_RH) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_RH;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }

        private void CheckBox_Type_LD_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();            
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = true == this.CheckBox_Type_LD.IsChecked ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_LD) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_LD;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }

        private void CheckBox_Type_DS_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = true == this.CheckBox_Type_DS.IsChecked ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_DS) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_DS;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }

        private void CheckBox_Type_MO_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = true == this.CheckBox_Type_MO.IsChecked ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_MO) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_MO;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }

        private void CheckBox_Type_MI_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = true == this.CheckBox_Type_MI.IsChecked ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_MI) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_MI;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }

        private void CheckBox_Type_GO_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = true == this.CheckBox_Type_GO.IsChecked ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_GO) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_GO;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }

        private void CheckBox_Type_GI_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.CheckBox_All.IsChecked = false;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType &= ~JobFilter.TYPE_ALL;
            PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType = true == this.CheckBox_Type_GI.IsChecked ?
                (PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType | JobFilter.TYPE_GI) : PresentationMgr.MainView.UC_JobList.CurrentFilter.FilterJobType & ~JobFilter.TYPE_GI;

            PresentationMgr.Singleton.JL_Refresh(PresentationMgr.MainView.UC_JobList);
            checkCheckBoxType();
        }
        private void TextBlock_loginLong_Click(object sender, RoutedEventArgs e)
        {
            VMT_Data_JAT2.VMT_DataMgr_RMG.GetDriverJobHistory_Ask();
            PresentationMgr.MainView.UC_DriverWorkingHistory.Visibility = Visibility;
        }
        private void TextBlock_jobCount_Click(object sender, RoutedEventArgs e)
        {
            VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask(UserInfo.gMchnTp, true);
            PresentationMgr.MainView.UC_OtherMachineJobListView.Visibility = Visibility;
        }
        public void RMG_Member_PropertyChanged_TargetJobKey(object sender, PropertyChangedEventArgs e)
        {   
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (!String.IsNullOrEmpty(jobKey))
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                if (jobOrder != null && jobOrder.type != null)
                {
                    var location = jobOrder.locWorking;
                    if (jobOrder.type.jobTp == "RH"/* && PresentationMgr.UseFromLocationForRehandling == true && jobOrder.type.jobStatus != "P")*/
                        || jobOrder.type.jobTp == "AH")
                        location = jobOrder.locWorking;
                    else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                        location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;

                    if (location.bay == null || location.bay == "")
                    {
                        PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                        if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Visible;
                        PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = false;
                    }
                    else
                    {
                        PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                        if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                        PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;
                    }
                    //if (!PresentationMgr.MainView.cntrLockMode ||
                    //      (PresentationMgr.MainView.cntrLockMode && PresentationMgr.Singleton.CurrentPostion.m_cBlock == location.blck && PresentationMgr.Singleton.CurrentPostion.m_cBay == location.bay))
                    //{
                        VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                        pos.m_cBlock = location.blck;
                        pos.m_cBay = PresentationMgr.GetFrontOddBay(location.bay);
                        pos.m_cRow = location.row;
                        pos.m_cTier = location.tier;
                        PresentationMgr.Singleton.CurrentPostion = pos;
                    //}
                    //else
                    //{
                    //    PresentationMgr.Singleton.CorrectionSource.Clear();
                    //    PresentationMgr.Singleton.SetInventoryData(null);
                    //}
                    //if (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") //re-setInventory for AH/RH job - base on locfrom
                        //PresentationMgr.Singleton.SetInventoryData(null);
                }
            }
            else
            {
                PresentationMgr.Singleton.CurrentPostion.m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cTier = String.Empty;
                PresentationMgr.Singleton.SetInventoryData(null, !PresentationMgr.MainView.deselectJobList);
                //PresentationMgr.Singleton.SetInventoryData(null);
            }

            this.UC_TargetJobInfo.RefreshTargetJobInfo();
        }

        public void setCountJobType(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> list_Joborder)
        {
            this.CheckBox_Type_GI.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("GI", LanguageService.LABEL_JOBTYPE) + " " + list_Joborder.Count(x => x.type.jobTp == "GI");
            this.CheckBox_Type_GO.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("GO", LanguageService.LABEL_JOBTYPE) + " " + list_Joborder.Count(x => x.type.jobTp == "GO" || x.type.jobTp == "GC");
            this.CheckBox_Type_MI.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MI", LanguageService.LABEL_JOBTYPE) + " " + list_Joborder.Count(x => x.type.jobTp == "MI");
            this.CheckBox_Type_MO.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MO", LanguageService.LABEL_JOBTYPE) + " " + list_Joborder.Count(x => x.type.jobTp == "MO");
            this.CheckBox_Type_DS.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("DS", LanguageService.LABEL_JOBTYPE) + " " + list_Joborder.Count(x => x.type.jobTp == "DS");
            this.CheckBox_Type_LD.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LD", LanguageService.LABEL_JOBTYPE) + " " + list_Joborder.Count(x => x.type.jobTp == "LD" || x.type.jobTp == "LC");
            this.CheckBox_Type_RH.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("RH", LanguageService.LABEL_JOBTYPE) + " " + (list_Joborder.Count(x => x.type.jobTp == "RH") + list_Joborder.Count(x => x.type.jobTp == "AH"));
            ResizeChkBxJobTp();
        }     
        private void ResizeChkBxJobTp()
        {
            this.CheckBox_Type_GI.FontSize = CheckBox_Type_GI.Content.ToString().Length >= 6 ? 15 : 18;
            this.CheckBox_Type_GO.FontSize = CheckBox_Type_GO.Content.ToString().Length >= 6 ? 15 : 18;
            this.CheckBox_Type_MI.FontSize = CheckBox_Type_MI.Content.ToString().Length >= 6 ? 15 : 18;
            this.CheckBox_Type_MO.FontSize = CheckBox_Type_MO.Content.ToString().Length >= 6 ? 15 : 18;
            this.CheckBox_Type_DS.FontSize = CheckBox_Type_DS.Content.ToString().Length >= 6 ? 15 : 18;
            this.CheckBox_Type_LD.FontSize = CheckBox_Type_LD.Content.ToString().Length >= 6 ? 15 : 18;
            this.CheckBox_Type_RH.FontSize = CheckBox_Type_RH.Content.ToString().Length >= 6 ? 15 : 18;
        }
    }
}