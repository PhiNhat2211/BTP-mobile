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
using VMT_Data_JAT2.Objects;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for OtherMachineJobListView.xaml
    /// </summary>
    public partial class OtherMachineJobListView : UserControl
    {
        int page = 1;
        public OtherMachineJobListView()
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
            //SetGridData();
        }
        private void InitEvent()
        {
            this.Btn_Search.Click += new RoutedEventHandler(Btn_Search_Click);
            this.Btn_Prev.Click += new RoutedEventHandler(Btn_Prev_Click);
            this.Btn_Next.Click += new RoutedEventHandler(Btn_Next_Click);
            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);
        }
        private void LoadLanguage()
        {
            this.Tbl_OtherMachine.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_ID.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_Container.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_ISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_FM.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0005", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_Job.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_CurLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_PlanLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0008", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Tbl_Grade.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0009", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Btn_Search.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0010", LanguageService.LABEL_OTHERMACHINEJOB);
            this.Btn_Prev.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0008", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Close.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0011", LanguageService.LABEL_OTHERMACHINEJOB);
        }
        public class OtherMachineJob
        {
            public String ycNo;
            public String qcNo;
            public String cntrNo;
            public String cntrIso;
            public String fullMty;
            public String jobTp;
            public String jobTpKor;
            public String fmLocation;
            public String toLocation;
            public String cntrGrade;
            public OtherMachineJob()
            {
                ycNo = "1";
                cntrNo = "NTOS1000020";
                cntrIso = "ISO";
                fullMty = "Full";
                jobTpKor = "JobTp";
                jobTpKor = "JobTpKor";
                fmLocation = "A1";
                toLocation = "B2";
                cntrGrade = "Grade";
            }
        }
        private List<OtherMachineJob> listOtherMachineJob = new List<OtherMachineJob>();
        OtherMachineJob test = new OtherMachineJob();
        private void InitTest()
        {
            for (int i = 1; i <= 18; i++)
            {
                listOtherMachineJob.Add(test);
            }
        }
        private void SetOtherMachineJobGrid(int i)
        {
            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            OtherMachineJob job = listOtherMachineJob[7 * page - 8 + i];
            if(job.jobTp == "GI")
                job.fmLocation = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0012", LanguageService.LABEL_OTHERMACHINEJOB);
            else if (job.jobTp == "GO")
                job.toLocation = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0012", LanguageService.LABEL_OTHERMACHINEJOB);
            else if (job.jobTp == "LD")
                job.toLocation = job.qcNo;
            switch (i)
            {
                case 1:
                    Tbl_DetailID1.Text = job.ycNo;
                    Tbl_DetailContainer1.Text = job.cntrNo;
                    Tbl_DetailISO1.Text = job.cntrIso;
                    Tbl_DetailFM1.Text = job.fullMty;
                    Tbl_DetailJob1.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc1.Text = job.fmLocation;
                    Tbl_DetailPlanLoc1.Text = job.toLocation;
                    Tbl_DetailGrade1.Text = job.cntrGrade;
                    break;
                case 2:
                    Tbl_DetailID2.Text = job.ycNo;
                    Tbl_DetailContainer2.Text = job.cntrNo;
                    Tbl_DetailISO2.Text = job.cntrIso;
                    Tbl_DetailFM2.Text = job.fullMty;
                    Tbl_DetailJob2.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc2.Text = job.fmLocation;
                    Tbl_DetailPlanLoc2.Text = job.toLocation;
                    Tbl_DetailGrade2.Text = job.cntrGrade;
                    break;
                case 3:
                    Tbl_DetailID3.Text = job.ycNo;
                    Tbl_DetailContainer3.Text = job.cntrNo;
                    Tbl_DetailISO3.Text = job.cntrIso;
                    Tbl_DetailFM3.Text = job.fullMty;
                    Tbl_DetailJob3.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc3.Text = job.fmLocation;
                    Tbl_DetailPlanLoc3.Text = job.toLocation;
                    Tbl_DetailGrade3.Text = job.cntrGrade;
                    break;
                case 4:
                    Tbl_DetailID4.Text = job.ycNo;
                    Tbl_DetailContainer4.Text = job.cntrNo;
                    Tbl_DetailISO4.Text = job.cntrIso;
                    Tbl_DetailFM4.Text = job.fullMty;
                    Tbl_DetailJob4.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc4.Text = job.fmLocation;
                    Tbl_DetailPlanLoc4.Text = job.toLocation;
                    Tbl_DetailGrade4.Text = job.cntrGrade;
                    break;
                case 5:
                    Tbl_DetailID5.Text = job.ycNo;
                    Tbl_DetailContainer5.Text = job.cntrNo;
                    Tbl_DetailISO5.Text = job.cntrIso;
                    Tbl_DetailFM5.Text = job.fullMty;
                    Tbl_DetailJob5.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc5.Text = job.fmLocation;
                    Tbl_DetailPlanLoc5.Text = job.toLocation;
                    Tbl_DetailGrade5.Text = job.cntrGrade;
                    break;
                case 6:
                    Tbl_DetailID6.Text = job.ycNo;
                    Tbl_DetailContainer6.Text = job.cntrNo;
                    Tbl_DetailISO6.Text = job.cntrIso;
                    Tbl_DetailFM6.Text = job.fullMty;
                    Tbl_DetailJob6.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc6.Text = job.fmLocation;
                    Tbl_DetailPlanLoc6.Text = job.toLocation;
                    Tbl_DetailGrade6.Text = job.cntrGrade;
                    break;
                case 7:
                    Tbl_DetailID7.Text = job.ycNo;
                    Tbl_DetailContainer7.Text = job.cntrNo;
                    Tbl_DetailISO7.Text = job.cntrIso;
                    Tbl_DetailFM7.Text = job.fullMty;
                    Tbl_DetailJob7.Text = lang.Contains("Korea") ? job.jobTpKor : job.jobTp;
                    Tbl_DetailCurLoc7.Text = job.fmLocation;
                    Tbl_DetailPlanLoc7.Text = job.toLocation;
                    Tbl_DetailGrade7.Text = job.cntrGrade;
                    break;
            }
        }
        private void SetGridData()
        {
            ClearGridData();
            for (int i = 1; i <= 7; i++)
            {
                if (7 * page + i - 7 > 0 && 7 * page + i - 7 <= listOtherMachineJob.Count)
                {
                    SetOtherMachineJobGrid(i);
                }
            }
        }
        public void SetMachineList()
        {
            this.Cb_Machine.Items.Clear();
            var machineList = DataMgr.Singleton.List_MachineofPool.Machine.ToList();
            foreach (var machine in machineList)
            {
                this.Cb_Machine.Items.Add(machine.mchnId);
            }
            
            CheckLayoutPrevNextBtn();
            if (this.Cb_Machine.Items.Count > 0)
            {
                this.Cb_Machine.SelectedIndex = 0;
                Btn_Search_Click(null, null);
            }
        }
        public void SetListOtherMachineJobCallBack(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> listJob)
        {
            listOtherMachineJob.Clear();
            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder item in listJob)
            {
                OtherMachineJob row = new OtherMachineJob();
                row.ycNo = item.ycNo;
                row.qcNo = item.type.qcId;
                row.cntrNo = item.cntr.cntrNo;
                row.cntrIso = item.cntr.cntrIso;
                row.fullMty = item.cntr.fullMty;
                row.jobTp = item.type.jobTp;
                row.jobTpKor = item.jobTpKor;
                row.fmLocation = item.locFrom.location;
                row.toLocation = item.locWorking.location;
                row.cntrGrade = item.cntr.cntrGrade;

                listOtherMachineJob.Add(row);
            }
            page = listOtherMachineJob.Count > 0 ? 1 : 0;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }

        private void ClearOtherMachineJobGrid(int i)
        {
            switch (i)
            {
                case 1:
                    Tbl_DetailID1.Text = String.Empty;
                    Tbl_DetailContainer1.Text = String.Empty;
                    Tbl_DetailISO1.Text = String.Empty;
                    Tbl_DetailFM1.Text = String.Empty;
                    Tbl_DetailJob1.Text = String.Empty;
                    Tbl_DetailCurLoc1.Text = String.Empty;
                    Tbl_DetailPlanLoc1.Text = String.Empty;
                    Tbl_DetailGrade1.Text = String.Empty;
                    break;
                case 2:
                    Tbl_DetailID2.Text = String.Empty;
                    Tbl_DetailContainer2.Text = String.Empty;
                    Tbl_DetailISO2.Text = String.Empty;
                    Tbl_DetailFM2.Text = String.Empty;
                    Tbl_DetailJob2.Text = String.Empty;
                    Tbl_DetailCurLoc2.Text = String.Empty;
                    Tbl_DetailPlanLoc2.Text = String.Empty;
                    Tbl_DetailGrade2.Text = String.Empty;
                    break;
                case 3:
                    Tbl_DetailID3.Text = String.Empty;
                    Tbl_DetailContainer3.Text = String.Empty;
                    Tbl_DetailISO3.Text = String.Empty;
                    Tbl_DetailFM3.Text = String.Empty;
                    Tbl_DetailJob3.Text = String.Empty;
                    Tbl_DetailCurLoc3.Text = String.Empty;
                    Tbl_DetailPlanLoc3.Text = String.Empty;
                    Tbl_DetailGrade3.Text = String.Empty;
                    break;
                case 4:
                    Tbl_DetailID4.Text = String.Empty;
                    Tbl_DetailContainer4.Text = String.Empty;
                    Tbl_DetailISO4.Text = String.Empty;
                    Tbl_DetailFM4.Text = String.Empty;
                    Tbl_DetailJob4.Text = String.Empty;
                    Tbl_DetailCurLoc4.Text = String.Empty;
                    Tbl_DetailPlanLoc4.Text = String.Empty;
                    Tbl_DetailGrade4.Text = String.Empty;
                    break;
                case 5:
                    Tbl_DetailID5.Text = String.Empty;
                    Tbl_DetailContainer5.Text = String.Empty;
                    Tbl_DetailISO5.Text = String.Empty;
                    Tbl_DetailFM5.Text = String.Empty;
                    Tbl_DetailJob5.Text = String.Empty;
                    Tbl_DetailCurLoc5.Text = String.Empty;
                    Tbl_DetailPlanLoc5.Text = String.Empty;
                    Tbl_DetailGrade5.Text = String.Empty;
                    break;
                case 6:
                    Tbl_DetailID6.Text = String.Empty;
                    Tbl_DetailContainer6.Text = String.Empty;
                    Tbl_DetailISO6.Text = String.Empty;
                    Tbl_DetailFM6.Text = String.Empty;
                    Tbl_DetailJob6.Text = String.Empty;
                    Tbl_DetailCurLoc6.Text = String.Empty;
                    Tbl_DetailPlanLoc6.Text = String.Empty;
                    Tbl_DetailGrade6.Text = String.Empty;
                    break;
                case 7:
                    Tbl_DetailID7.Text = String.Empty;
                    Tbl_DetailContainer7.Text = String.Empty;
                    Tbl_DetailISO7.Text = String.Empty;
                    Tbl_DetailFM7.Text = String.Empty;
                    Tbl_DetailJob7.Text = String.Empty;
                    Tbl_DetailCurLoc7.Text = String.Empty;
                    Tbl_DetailPlanLoc7.Text = String.Empty;
                    Tbl_DetailGrade7.Text = String.Empty;
                    break;
            }
        }

        private void ClearGridData()
        {
            for (int i = 1; i <= 7; i++)
            {
                ClearOtherMachineJobGrid(i);
            }
        }

        private void CheckLayoutPrevNextBtn()
        {
            Tbl_Page.Text = page.ToString() + "/" + (listOtherMachineJob.Count / 7 + ((listOtherMachineJob.Count % 7 > 0) ? 1 : 0));
            if (page <= 1)
            {
                Btn_Prev.IsEnabled = false;
                if (page <= listOtherMachineJob.Count / 7 + ((listOtherMachineJob.Count % 7 > 0) ? 0 : -1))
                {
                    Btn_Next.IsEnabled = true;
                }
                else Btn_Next.IsEnabled = false;
            }
            else if (page > listOtherMachineJob.Count / 7 + ((listOtherMachineJob.Count % 7 > 0) ? 0 : -1))
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
        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            String mchnId = this.Cb_Machine.SelectedValue.ToString();
            if (!String.IsNullOrEmpty(mchnId))
            {
                var mchnLst =  DataMgr.Singleton.List_MachineofPool.Machine.ToList();
                string mchnTp = (mchnLst.Find(x => x.mchnId.Equals(mchnId)) != null ) ? mchnLst.Find(x => x.mchnId.Equals(mchnId)).mchnTp : "TC";

                var list = new hessiancsharp.Class.HessianList();
                list.Add(mchnTp);
                list.Add(mchnId);

                HessianComm.HessianAPI.GetJobOrderList_New(list);
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
