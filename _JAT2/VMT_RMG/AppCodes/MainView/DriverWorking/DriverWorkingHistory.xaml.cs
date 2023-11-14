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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for DriverWorkingHistory.xaml
    /// </summary>
    public partial class DriverWorkingHistory : UserControl
    {
        int page = 1;
        public DriverWorkingHistory()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadLanguage
            LoadLanguage();         
            //Init Event Handler
            InitEvent();
            //InitTest();
        }
        private void InitEvent()
        {
            this.Btn_Prev.Click += new RoutedEventHandler(Btn_Prev_Click);
            this.Btn_Next.Click += new RoutedEventHandler(Btn_Next_Click);
            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);
        }
        private void LoadLanguage()
        {
            this.Tbl_DriverWorking.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_DRIVERWORKING);          
            this.Tbl_No.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_DRIVERWORKING);
            this.Tbl_Job.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_DRIVERWORKING);
            this.Tbl_Container.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_DRIVERWORKING);
            this.Tbl_Location.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0005", LanguageService.LABEL_DRIVERWORKING);
            this.Tbl_Truck.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_DRIVERWORKING);
            this.Tbl_SZTP.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_DRIVERWORKING);
            this.Tbl_POD.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0008", LanguageService.LABEL_DRIVERWORKING);          
            this.Btn_Close.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0009", LanguageService.LABEL_DRIVERWORKING);
        }
        public class DriverWorking
        {
            public String seq;
            public String jobTp;
            public String jobTpKor;
            public String cntrNo;
            public String toLocation;
            public String ytNo;
            public String cntrIso;
            public String podCd;           
            public DriverWorking()
            {
                seq = "1";
                jobTp = "GO";
                jobTpKor = "KOR";
                cntrNo = "NTOS1000020";
                toLocation = "5D-89-B-1";
                ytNo = "부산99바9999";
                cntrIso = "2210";
                podCd = "PUS";
            }
        }
        private List<DriverWorking> listDriverWorking = new List<DriverWorking>();
        DriverWorking test = new DriverWorking();
        private void InitTest()
        {
            for (int i = 1; i <= 18; i++)
            {
                listDriverWorking.Add(test);
            }
        }
        private void SetDriverWorkingGrid(int i)
        {
            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            Boolean isUsingKorean = lang.Contains("Korea");
            DriverWorking drvWorking = listDriverWorking[7 * page - 8 + i];
            switch (i)
            {
                case 1:
                    Tbl_DetailNo1.Text = drvWorking.seq;
                    Tbl_DetailJob1.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer1.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation1.Text = drvWorking.toLocation;
                    Tbl_DetailTruck1.Text = drvWorking.ytNo;
                    Tbl_DetailTruck1.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30; 
                    Tbl_DetailSZTP1.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD1.Text = drvWorking.podCd;
                    break;
                case 2:
                    Tbl_DetailNo2.Text = drvWorking.seq;
                    Tbl_DetailJob2.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer2.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation2.Text = drvWorking.toLocation;
                    Tbl_DetailTruck2.Text = drvWorking.ytNo;
                    Tbl_DetailTruck2.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30;
                    Tbl_DetailSZTP2.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD2.Text = drvWorking.podCd;
                    break;
                case 3:
                    Tbl_DetailNo3.Text = drvWorking.seq;
                    Tbl_DetailJob3.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer3.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation3.Text = drvWorking.toLocation;
                    Tbl_DetailTruck3.Text = drvWorking.ytNo;
                    Tbl_DetailTruck3.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30;
                    Tbl_DetailSZTP3.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD3.Text = drvWorking.podCd;
                    break;
                case 4:
                    Tbl_DetailNo4.Text = drvWorking.seq;
                    Tbl_DetailJob4.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer4.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation4.Text = drvWorking.toLocation;
                    Tbl_DetailTruck4.Text = drvWorking.ytNo;
                    Tbl_DetailTruck4.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30;
                    Tbl_DetailSZTP4.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD4.Text = drvWorking.podCd;
                    break;
                case 5:
                    Tbl_DetailNo5.Text = drvWorking.seq;
                    Tbl_DetailJob5.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer5.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation5.Text = drvWorking.toLocation;
                    Tbl_DetailTruck5.Text = drvWorking.ytNo;
                    Tbl_DetailTruck5.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30;
                    Tbl_DetailSZTP5.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD5.Text = drvWorking.podCd;
                    break;
                case 6:
                    Tbl_DetailNo6.Text = drvWorking.seq;
                    Tbl_DetailJob6.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer6.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation6.Text = drvWorking.toLocation;
                    Tbl_DetailTruck6.Text = drvWorking.ytNo;
                    Tbl_DetailTruck6.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30;
                    Tbl_DetailSZTP6.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD6.Text = drvWorking.podCd;
                    break;
                case 7:
                    Tbl_DetailNo7.Text = drvWorking.seq;
                    Tbl_DetailJob7.Text = isUsingKorean ? drvWorking.jobTpKor : drvWorking.jobTp;
                    Tbl_DetailContainer7.Text = drvWorking.cntrNo;
                    Tbl_DetailLocation7.Text = drvWorking.toLocation;
                    Tbl_DetailTruck7.Text = drvWorking.ytNo;
                    Tbl_DetailTruck7.FontSize = (drvWorking.jobTp == "GI" || drvWorking.jobTp == "GO") ? 24 : 30;
                    Tbl_DetailSZTP7.Text = drvWorking.cntrIso;
                    Tbl_DetailPOD7.Text = drvWorking.podCd;
                    break;
            }
        }
        private void SetGridData()
        {
            ClearGridData();
            for (int i = 1; i <= 7; i++)
            {
                if (7 * page + i - 7 >= 0 && 7 * page + i - 7 <= listDriverWorking.Count)
                {
                    SetDriverWorkingGrid(i);
                }
            }
        }
        public void SetListDriverWorkingCallBack(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> listJob)
        {
            listDriverWorking.Clear();
            int j = 0;
            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder item in listJob)
            {
                j++;
                DriverWorking row = new DriverWorking();
                row.seq = j.ToString();
                row.jobTp = item.type.jobTp;
                row.jobTpKor = item.jobTpKor;
                row.cntrNo = item.cntr.cntrNo;
                row.toLocation = item.locWorking.location;
                row.ytNo = item.partnerMchn.mchnId;
                row.cntrIso = item.cntr.cntrIso;
                row.podCd = item.podCd;

                listDriverWorking.Add(row);
            }
            page = 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }

        private void ClearDriverWorkingGrid(int i)
        {
            switch (i)
            {
                case 1:
                    Tbl_DetailNo1.Text = String.Empty;
                    Tbl_DetailJob1.Text = String.Empty;
                    Tbl_DetailContainer1.Text = String.Empty;
                    Tbl_DetailLocation1.Text = String.Empty;
                    Tbl_DetailTruck1.Text = String.Empty;
                    Tbl_DetailSZTP1.Text = String.Empty;
                    Tbl_DetailPOD1.Text = String.Empty;
                    break;
                case 2:
                    Tbl_DetailNo2.Text = String.Empty;
                    Tbl_DetailJob2.Text = String.Empty;
                    Tbl_DetailContainer2.Text = String.Empty;
                    Tbl_DetailLocation2.Text = String.Empty;
                    Tbl_DetailTruck2.Text = String.Empty;
                    Tbl_DetailSZTP2.Text = String.Empty;
                    Tbl_DetailPOD2.Text = String.Empty;
                    break;
                case 3:
                    Tbl_DetailNo3.Text = String.Empty;
                    Tbl_DetailJob3.Text = String.Empty;
                    Tbl_DetailContainer3.Text = String.Empty;
                    Tbl_DetailLocation3.Text = String.Empty;
                    Tbl_DetailTruck3.Text = String.Empty;
                    Tbl_DetailSZTP3.Text = String.Empty;
                    Tbl_DetailPOD3.Text = String.Empty;
                    break;
                case 4:
                    Tbl_DetailNo4.Text = String.Empty;
                    Tbl_DetailJob4.Text = String.Empty;
                    Tbl_DetailContainer4.Text = String.Empty;
                    Tbl_DetailLocation4.Text = String.Empty;
                    Tbl_DetailTruck4.Text = String.Empty;
                    Tbl_DetailSZTP4.Text = String.Empty;
                    Tbl_DetailPOD4.Text = String.Empty;
                    break;
                case 5:
                    Tbl_DetailNo5.Text = String.Empty;
                    Tbl_DetailJob5.Text = String.Empty;
                    Tbl_DetailContainer5.Text = String.Empty;
                    Tbl_DetailLocation5.Text = String.Empty;
                    Tbl_DetailTruck5.Text = String.Empty;
                    Tbl_DetailSZTP5.Text = String.Empty;
                    Tbl_DetailPOD5.Text = String.Empty;
                    break;
                case 6:
                    Tbl_DetailNo6.Text = String.Empty;
                    Tbl_DetailJob6.Text = String.Empty;
                    Tbl_DetailContainer6.Text = String.Empty;
                    Tbl_DetailLocation6.Text = String.Empty;
                    Tbl_DetailTruck6.Text = String.Empty;
                    Tbl_DetailSZTP6.Text = String.Empty;
                    Tbl_DetailPOD6.Text = String.Empty;
                    break;
                case 7:
                    Tbl_DetailNo7.Text = String.Empty;
                    Tbl_DetailJob7.Text = String.Empty;
                    Tbl_DetailContainer7.Text = String.Empty;
                    Tbl_DetailLocation7.Text = String.Empty;
                    Tbl_DetailTruck7.Text = String.Empty;
                    Tbl_DetailSZTP7.Text = String.Empty;
                    Tbl_DetailPOD7.Text = String.Empty;
                    break;
            }
        }

        private void ClearGridData()
        {
            for (int i = 1; i <= 7; i++)
            {
                ClearDriverWorkingGrid(i);
            }
        }

        private void CheckLayoutPrevNextBtn()
        {
            Tbl_Page.Text = page.ToString() + "/" + (listDriverWorking.Count / 7 + ((listDriverWorking.Count % 7 > 0) ? 1 : 0));
            if (page <= 1)
            {
                Btn_Prev.IsEnabled = false;
                if (page <= listDriverWorking.Count / 7 + ((listDriverWorking.Count % 7 > 0) ? 0 : -1))
                {
                    Btn_Next.IsEnabled = true;
                }
                else Btn_Next.IsEnabled = false;
            }
            else if (page > listDriverWorking.Count / 7 + ((listDriverWorking.Count % 7 > 0) ? 0 : -1))
            {
                Btn_Next.IsEnabled = false;
                if (page > 1)
                {
                    Btn_Prev.IsEnabled = true;
                }
                else Btn_Prev.IsEnabled = false;
            }
            else
            {
                Btn_Prev.IsEnabled = true;
                Btn_Next.IsEnabled = true;
            }
        }
        private void Btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            page = page - 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }
        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            page = page + 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
