using Common.Interface;
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
//using ExternalAPI;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for JobListItem.xaml
    /// </summary>
    public partial class JobListItem : UserControl
    {
        public Boolean IsFresh { get; set; }
        private String _jobKey;
        private Boolean _Selected = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();

        private SolidColorBrush _itvPowInBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x20, 0x00));//SolidColorBrush(Color.FromArgb(0xFF, 0xE9, 0x20, 0x20));
        private SolidColorBrush _otrPowInBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x40, 0xFF));//SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0xE9));

        public String JobKey
        {
            get { return _jobKey; }
        }

        public Boolean Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                RefreshJobListItem();
            }
        }

        public JobListItem()
        {
            this.InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this.IsFresh = false;

            AppendTextBlockList();
        }

        ~JobListItem()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                RefreshJobListItem();
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                RefreshJobListItem();
            }
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(TextBlock_Length);
            _TextBlockList.Add(TextBlock_Full);
            _TextBlockList.Add(TextBlock_JobType);
            _TextBlockList.Add(TextBlock_Full);
            _TextBlockList.Add(TextBlock_PosRF);
            _TextBlockList.Add(TextBlock_ITVNo);
            _TextBlockList.Add(TextBlock_VslHoldDeck);
            _TextBlockList.Add(TextBlock_CntrNo);
            _TextBlockList.Add(TextBlock_ETW);
            _TextBlockList.Add(TextBlock_YardLoc);
        }

        private void RefreshJobListItem()
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(_jobKey);

            if (jobOrder == null)
            {
                if (PresentationMgr.MainView.UC_ContainerArea.Visibility == Visibility.Visible && _Selected)
                {
                    this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;
                }
                else
                {
                    this.LayoutRoot.Background = UIThemeMgr.LayoutRootNormalBackground;
                }
                return;
            }

            //20210406 getSwapList blockName-bayName   
            if (_Selected && jobOrder != null && jobOrder.type != null && "GO".Equals(jobOrder.type.jobTp))
            {
                var block = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName;
                var bay = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BayName;

                if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block))
                {
                    if (PresentationMgr.MainView.UC_JobList.needToGetEmptySwapWithSelectedLocation)
                    {
                        PresentationMgr.MainView.UC_JobList.needToGetEmptySwapWithSelectedLocation = false;
                        var location = String.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                        block = location.blck;
                        bay = !String.IsNullOrEmpty(location.bay) ? PresentationMgr.GetFrontOddBay(location.bay) : String.Empty;
                    }

                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].isBolBlck && !String.IsNullOrEmpty(bay))
                    {
                        block = block + "-" + bay;
                    }
                    VMT_DataMgr_RMG.getSwapListRTG_Ask(jobOrder.jobKey, block, true);
                }
                else
                {
                    PresentationMgr.Singleton.swapList.Clear();
                    PresentationMgr.Singleton.reservedList.Clear();
                    PresentationMgr.Singleton.swapListRTG.Clear();
                }

            }

            if (_Selected && jobOrder.type.jobStatus != "P")
            {
                if (!MainWindow.firstLoad)
                {
                    foreach (JobListItem jobListItem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
                    {
                        if (jobListItem.TextBlock_CntrNo.Text != this.TextBlock_CntrNo.Text)
                        {
                            jobListItem.Selected = false;
                        }
                    }
                }

                //this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 240, 165, 15));
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;

                //PresentationMgr.Singleton.swapList.Clear();
                //if (/*(UserInfo.gMchnTp.Equals("RS") || UserInfo.gMchnTp.Equals("ES") || UserInfo.gMchnTp.Equals("EH")) &&*/ 
                //    jobOrder.type.jobTp.Equals("GO") && jobOrder.cntr.fullMty.Equals("M"))
                //{
                //    VMT_DataMgr_RMG.GetEmptySwappingTargetList_Ask(jobOrder.cntr.cntrNo, jobOrder.partnerMchn.mchnId);
                //}
            }
            else if (jobOrder.type.jobStatus == "P")
                this.LayoutRoot.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F75348"));
            else
            {
                ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                var strRec = rec["Gird_Background_8"];
                if (strRec is SolidColorBrush)
                    this.LayoutRoot.Background = strRec as SolidColorBrush;

                if (!String.IsNullOrEmpty(jobOrder.vbsDate))
                {
                    try
                    {
                        this.LayoutRoot.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(PresentationMgr.jobItemColor));
                    }
                    catch (Exception e)
                    {
                    }
                }    
            }

            foreach (TextBlock tBlock in _TextBlockList)
            {
                if (_Selected)
                    //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    tBlock.Foreground = UIThemeMgr.TextBlockSelectedForeground;
                else
                {
                    if (!String.IsNullOrEmpty(jobOrder.vbsDate))
                    {
                        SolidColorBrush colorGrey = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF464848")); //Grey of DayMode
                        tBlock.Foreground = colorGrey; 
                    }
                    else
                    {
                        ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                        var strRec = rec["TextBox_Foreground_3"];

                        if (strRec is SolidColorBrush)
                            tBlock.Foreground = strRec as SolidColorBrush;
                    }
                }
                if (jobOrder.type.jobTp == "GO")
                {
                    if (tBlock.Name == "TextBlock_JobType")
                    {
                        this.TextBlock_JobType.Foreground = Brushes.Black;
                        this.TextBlock_JobTypeKor.Foreground = Brushes.Black;
                        this.TextBlock_JobType.Background = Brushes.Transparent;
                        this.TextBlock_JobTypeKor.Background = Brushes.Transparent;
                    }
                    if (tBlock.Name == "TextBlock_ITVNo")
                    {
                        this.TextBlock_ITVNo.Foreground = Brushes.Black;
                        this.TextBlock_ITVNo.Background = Brushes.Transparent;
                    }
                    if (tBlock.Name == "TextBlock_YardLoc")
                    {
                        this.TextBlock_YardLoc.Foreground = Brushes.Black;
                        this.TextBlock_YardLoc.Background = Brushes.Transparent;
                    }
                    if (tBlock.Name == "TextBlock_CntrNo")
                    {
                        this.TextBlock_CntrNo.Foreground = Brushes.Black;
                        this.TextBlock_CntrNo.Background = Brushes.Transparent;
                    }
                }
                else if (jobOrder.type.jobTp == "GI")
                {
                    if (tBlock.Name == "TextBlock_JobType")
                    {
                        this.TextBlock_JobType.Foreground = Brushes.Black;
                        this.TextBlock_JobTypeKor.Foreground = Brushes.Black;
                        this.TextBlock_JobType.Background = Brushes.Transparent;
                        this.TextBlock_JobTypeKor.Background = Brushes.Transparent;
                    }
                    if (tBlock.Name == "TextBlock_ITVNo")
                    {
                        this.TextBlock_ITVNo.Foreground = Brushes.Black;
                        this.TextBlock_ITVNo.Background = Brushes.Transparent;
                    }
                    if (tBlock.Name == "TextBlock_YardLoc")
                    {
                        this.TextBlock_YardLoc.Foreground = Brushes.Black;
                        this.TextBlock_YardLoc.Background = Brushes.Transparent;
                    }
                    if (tBlock.Name == "TextBlock_CntrNo")
                    {
                        this.TextBlock_CntrNo.Foreground = Brushes.Black;
                        this.TextBlock_CntrNo.Background = Brushes.Transparent;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(jobOrder.vbsDate))
                    {
                        SolidColorBrush colorGrey = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF464848")); //Grey of DayMode

                        if (tBlock.Name == "TextBlock_JobType")
                        {
                            this.TextBlock_JobType.Foreground = colorGrey;
                            this.TextBlock_JobTypeKor.Foreground = colorGrey;
                            this.TextBlock_JobType.Background = Brushes.Transparent;
                            this.TextBlock_JobTypeKor.Background = Brushes.Transparent;
                        }
                        if (tBlock.Name == "TextBlock_ITVNo")
                        {
                            this.TextBlock_ITVNo.Foreground = colorGrey;
                            this.TextBlock_ITVNo.Background = Brushes.Transparent;
                        }

                        if (tBlock.Name == "TextBlock_YardLoc")
                        {
                            this.TextBlock_YardLoc.Foreground = colorGrey;
                            this.TextBlock_YardLoc.Background = Brushes.Transparent;
                        }
                        if (tBlock.Name == "TextBlock_CntrNo")
                        {
                            ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                            var strRec = rec["TextBox_Foreground_1"];
                            this.TextBlock_CntrNo.Foreground = strRec as SolidColorBrush;
                            this.TextBlock_CntrNo.Background = Brushes.Transparent;
                        }
                    }
                    else
                    {
                        ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                        var strRec = rec["TextBox_Foreground_3"];

                        if (strRec is SolidColorBrush)
                        {
                            if (tBlock.Name == "TextBlock_JobType")
                            {
                                this.TextBlock_JobType.Foreground = strRec as SolidColorBrush;
                                this.TextBlock_JobTypeKor.Foreground = strRec as SolidColorBrush;
                                this.TextBlock_JobType.Background = Brushes.Transparent;
                                this.TextBlock_JobTypeKor.Background = Brushes.Transparent;
                            }
                            if (tBlock.Name == "TextBlock_ITVNo")
                            {
                                this.TextBlock_ITVNo.Foreground = strRec as SolidColorBrush;
                                this.TextBlock_ITVNo.Background = Brushes.Transparent;
                            }

                            if (tBlock.Name == "TextBlock_YardLoc")
                            {
                                this.TextBlock_YardLoc.Foreground = strRec as SolidColorBrush;
                                this.TextBlock_YardLoc.Background = Brushes.Transparent;
                            }
                            if (tBlock.Name == "TextBlock_CntrNo")
                            {
                                strRec = rec["TextBox_Foreground_1"];
                                this.TextBlock_CntrNo.Foreground = strRec as SolidColorBrush;
                                this.TextBlock_CntrNo.Background = Brushes.Transparent;
                            }
                        }
                        /*
                        //20200129 change color from dark gray to black
                        ResourceDictionary recBlack = Application.Current.Resources.MergedDictionaries[0];
                        var strRecBlack = recBlack["TextBox_Foreground_1"];

                        if (strRecBlack is SolidColorBrush) TextBlock_CntrNo.Foreground = strRecBlack as SolidColorBrush;
                        */
                    }
                }
            }          
        }

        public void SetJobInfo(String jobKey, Int32 itemIndex)
        {
            _jobKey = jobKey;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);

            if (jobOrder == null || jobOrder.type == null)
                return;

            this.Image_HotJob.Visibility = String.IsNullOrEmpty(jobOrder.priorityJob) ? Visibility.Hidden : Visibility.Visible;
            this.Image_Twin.Visibility = String.IsNullOrEmpty(jobOrder.type.twinTandumKey) ? Visibility.Hidden : Visibility.Visible;//jobOrder.type.twinTandemFlg.Equals("W") ? Visibility.Visible : Visibility.Hidden;

            this.Tbl_JobSts.Text = "A".Equals(jobOrder.type.jobStatus) ? "A" : String.Empty;

            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            this.TextBlock_JobTypeKor.Text = jobOrder.jobTpKorShort;
            this.TextBlock_JobType.Text = jobOrder.type.jobTp;

            if (lang.Contains("Korea"))
            {
                this.TextBlock_JobType.Visibility = Visibility.Hidden;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Visible;
            }
            else
            {
                this.TextBlock_JobType.Visibility = Visibility.Visible;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Hidden;
            }
            //this.TextBlock_Length.Text = jobOrder.cntr.cntrLen + jobOrder.cntr.cntrTp + " " + (jobOrder.podCd.Length > 2 ? jobOrder.podCd.Substring(jobOrder.podCd.Length-3) : jobOrder.podCd);
            this.TextBlock_Length.Text = jobOrder.cntr.cntrLen + jobOrder.cntr.cntrTp; //20201008 hidden POD

            this.TextBlock_Full.Text = "F".Equals(jobOrder.cntr.fullMty) ? jobOrder.cntr.fullMty + "/" + jobOrder.cntr.cntrWgt : jobOrder.cntr.fullMty + "/" + jobOrder.cntr.opr;
            String foreAfterOpt = lang.Contains("Korea") ? jobOrder.foreAfterKor : jobOrder.type.jobFlagInfo;
            this.TextBlock_PosRF.Text = foreAfterOpt;
            if ("R".Equals(jobOrder.cntr.cntrCgoTp) && !String.IsNullOrEmpty(jobOrder.reefer.plugCd))
            {
                Run run = new Run();
                run.Text = "/" + jobOrder.reefer.plugCd;
                if (jobOrder.reefer.plugCd == "POC")
                    run.Foreground = Brushes.Blue;               
                this.TextBlock_PosRF.Inlines.Add(run);
            }

            if (!String.IsNullOrEmpty(jobOrder.vbsDate) && jobOrder.type.jobStatus != "P")
            {
                try {
                    this.LayoutRoot.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(PresentationMgr.jobItemColor));
                }
                catch (Exception e) {
                }
            } 

            string cntrNo = jobOrder.cntr.cntrNo;
            this.TextBlock_CntrNo.Text = cntrNo; //20200618 full cntrNo // cntrNo.Length > 8 ? (cntrNo.Substring(0, 2) + cntrNo.Substring(cntrNo.Length - 7)) : cntrNo;

            if (jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH")
                this.TextBlock_YardLoc.Text = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking.location : jobOrder.locFrom.location;
            else
                this.TextBlock_YardLoc.Text = jobOrder.locWorking.location;

            this.TextBlock_YardLoc.Text = this.TextBlock_YardLoc.Text.Replace("-", "");
            this.TextBlock_YardLoc.FontSize = this.TextBlock_YardLoc.Text.Length <= 7 ? 26 : this.TextBlock_YardLoc.Text.Length <= 8 ? 24 : this.TextBlock_YardLoc.Text.Length <= 10 ? 21 : 16;

            //if (this.TextBlock_YardLoc.Text.Length >= 8) //jindo cmt 20200609
            //{
            //    this.TextBlock_YardLoc.FontSize = 19;
            //    this.TextBlock_YardLoc.Margin = new Thickness(73, 13, 0, -3.54);
            //}
            //else
            //{
            //    this.TextBlock_YardLoc.FontSize = 23;
            //    this.TextBlock_YardLoc.Margin = new Thickness(73, 4.541, 0, -3.54);
            //}

            if (jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "GC")
            {
                this.TextBlock_ITVNo.Text = jobOrder.batNo;
            }
            else
            {
                this.TextBlock_ITVNo.Text = jobOrder.partnerMchn.mchnId;
            }

            //if (jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "GC")
            //{
            //    this.TextBlock_ETW.Text = jobOrder.type.waitingTime/* + ' ' + jobOrder.taskId*/;

            //    //string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            //    //Ini.IniFile ini = new Ini.IniFile(strIniFile);
            //    //String externalTruck = ini.IniReadValue("MACHINE", "ExternalTruck");

            //    //if (jobOrder.partnerMchn.mchnId.Length >= 4 && externalTruck == "N")
            //    //{
            //    //    //this.TextBlock_ITVNo.FontSize = 22;
            //    //    this.TextBlock_ITVNo.Text = jobOrder.partnerMchn.mchnId.Substring(jobOrder.partnerMchn.mchnId.Length - 4, 4);
            //    //}
            //}
            //else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "DS")
            //    this.TextBlock_ETW.Text = jobOrder.type.qcId + ' ' + jobOrder.taskId;
            //else
            //    this.TextBlock_ETW.Text = "";
            this.TextBlock_ETW.Text = jobOrder.type.waitingTime;

            if (this.TextBlock_ETW.Text.Length <= 5)
                this.TextBlock_ETW.FontSize = 17; //20200618 bigger font size
            else
                this.TextBlock_ETW.FontSize = 15;

            this.TextBlock_VslHoldDeck.Text = jobOrder.vslHoldDeck;

            //this.Image_reefer.Visibility = (jobOrder.cntr.cntrCgoTp == "R" || jobOrder.cntr.cntrCgoTp == "RD" || jobOrder.cntr.cntrCgoTp == "RH") ?
            //    Visibility.Visible : Visibility.Hidden;            

            //if (jobOrder.type.jobStatus == "A" || jobOrder.type.jobStatus == "P")
            //    this.Grid_Dim.Visibility = System.Windows.Visibility.Hidden;
            //else
            //    this.Grid_Dim.Visibility = System.Windows.Visibility.Visible;
        }

        public static BitmapImage GetJobTypeIcon(String jobtp)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/JobList/JobType/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/JobList/JobType/";
            }

            switch (jobtp)
            {
                case "DS":
                    imgUri += "job_ds.png";
                    break;
                case "LD":
                    imgUri += "job_ld.png";
                    break;
                case "MI":
                    imgUri += "job_mi.png";
                    break;
                case "MO":
                    imgUri += "job_mo.png";
                    break;
                case "GI":
                    imgUri += "job_gi.png";
                    break;
                case "GO":
                    imgUri += "job_go.png";
                    break;
                case "RH":
                    imgUri += "job_rh.png";
                    break;
                case "AH":
                    imgUri += "job_ah.png";
                    break;
                case "GC":
                    imgUri += "job_gc.png";
                    break;
                case "LC":
                    imgUri += "job_lc.png";
                    break;
                case "MV":
                    imgUri += "job_mv.png";
                    break;
            }

            return PresentationMgr.GetImageSource(imgUri);
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            return;
            //RefreshJobListItem();
        }

        public void SetVirtualContInfo(String cntrNo, Int32 itemIndex)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven = PresentationMgr.Singleton.CONTVirtual_Get(cntrNo);

            if (inven == null)
                return;

            this.Image_HotJob.Visibility = Visibility.Hidden;
            this.Image_Twin.Visibility = Visibility.Hidden;//inven.type.twinTandemFlg.Equals("W") ? Visibility.Visible : Visibility.Hidden;
            this.Grid_Dim.Visibility = System.Windows.Visibility.Hidden;

            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            this.TextBlock_JobTypeKor.Text = inven.jobTpKorShort;
            this.TextBlock_JobType.Text = inven.jobTp;

            if (lang.Contains("Korea"))
            {
                this.TextBlock_JobType.Visibility = Visibility.Hidden;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Visible;
            }
            else
            {
                this.TextBlock_JobType.Visibility = Visibility.Visible;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Hidden;
            }
            //this.TextBlock_Length.Text = inven.cntr.cntrLen + inven.cntr.cntrTp + " " + (inven.cntr.pod.Length > 2 ? inven.cntr.pod.Substring(inven.cntr.pod.Length - 3) : inven.cntr.pod);
            this.TextBlock_Length.Text = inven.cntr.cntrLen + inven.cntr.cntrTp; //20201008 hidden POD

            this.TextBlock_Full.Text = "F".Equals(inven.cntr.fullMty) ? inven.cntr.fullMty + "/" + inven.cntr.cntrWgt : inven.cntr.fullMty + "/" + inven.cntr.opr;

            this.TextBlock_CntrNo.Text = inven.cntr.cntrNo;

            this.TextBlock_YardLoc.Text = inven.loc.location;
            this.TextBlock_YardLoc.Text = this.TextBlock_YardLoc.Text.Replace("-", "");

            this.TextBlock_PosRF.Text = "";
            this.TextBlock_ITVNo.Text = "";
            this.TextBlock_ETW.Text = "";
            this.TextBlock_VslHoldDeck.Text = "";
        }
    }
}