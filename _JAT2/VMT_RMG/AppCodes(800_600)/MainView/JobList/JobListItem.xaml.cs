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
//using ExternalAPI;

namespace VMT_RMG_800by600
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
            _TextBlockList.Add(TextBlock_CntrTp);
            _TextBlockList.Add(TextBlock_CM);
            _TextBlockList.Add(TextBlock_Weight);
            _TextBlockList.Add(TextBlock_ClassCd);
            //_TextBlockList.Add(TextBlock_Opr);
            _TextBlockList.Add(TextBlock_CntrNo);
            _TextBlockList.Add(TextBlock_JobSts);
            //_TextBlockList.Add(TextBlock_VesselCd);
            //_TextBlockList.Add(TextBlock_POD);
            _TextBlockList.Add(TextBlock_YardLoc);
            _TextBlockList.Add(TextBlock_ITVNo);
            _TextBlockList.Add(TextBlock_ApproachLane);
            _TextBlockList.Add(TextBlock_QCID);
        }

        private void RefreshJobListItem()
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(_jobKey);

            if (jobOrder == null)
                return;

            if (_Selected)
                //this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 240, 165, 15));
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;
            else
            {
                ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                var strRec = rec["Gird_Background_8"];

                if (strRec is SolidColorBrush)
                    this.LayoutRoot.Background = strRec as SolidColorBrush;
            }

            foreach (TextBlock tBlock in _TextBlockList)
            {
                if (_Selected)
                    //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    tBlock.Foreground = UIThemeMgr.TextBlockSelectedForeground;
                else
                {
                    ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                    var strRec = rec["TextBox_Foreground_3"];

                    if( strRec is SolidColorBrush)
                        tBlock.Foreground = strRec as SolidColorBrush;
                }
            }

            this.Image_Vehicle.Source = GetVehicleIcon(jobOrder.type.jobFlagInfo);
        }

        public void SetJobInfo(String jobKey, Int32 itemIndex)
        {
            _jobKey = jobKey;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);

            if (jobOrder == null)
                return;

            this.Image_HotJob.Visibility = String.IsNullOrEmpty(jobOrder.priorityJob) ? Visibility.Hidden : Visibility.Visible;
            this.Image_Twin.Visibility = String.IsNullOrEmpty(jobOrder.type.ycTwinKey) ? Visibility.Hidden : Visibility.Visible;//jobOrder.type.twinTandemFlg.Equals("W") ? Visibility.Visible : Visibility.Hidden;
            this.Image_JobType.Source = JobListItem.GetJobTypeIcon(jobOrder.type.jobTp);
            this.TextBlock_Length.Text = jobOrder.cntr.cntrLen;
            if (!String.IsNullOrEmpty(jobOrder.cntr.cntrHgt) && jobOrder.cntr.cntrLen != "45")//if (jobOrder.cntr.cntrHgt == "HC") //(!String.IsNullOrEmpty(jobOrder.cntr.cntrHgt) && Convert.ToDouble(jobOrder.cntr.cntrHgt) > 9)
                this.TextBlock_Length.Text = String.Format("{0:C}H", this.TextBlock_Length.Text.First());
            this.TextBlock_Full.Text = jobOrder.cntr.fullMty;
            this.TextBlock_CntrTp.Text = jobOrder.cntr.cntrTp;
            this.TextBlock_ClassCd.Text = jobOrder.cntr.cls;
            this.TextBlock_CntrNo.Text = jobOrder.cntr.cntrNo;
            this.TextBlock_JobSts.Text = (jobOrder.type.jobStatus == "A" || jobOrder.type.jobStatus == "P") ? jobOrder.type.jobStatus : String.Empty;
            //this.TextBlock_VesselCd.Text = jobOrder.type.vslCd;

            if (PresentationMgr.UseFromLocationForRehandling == true && 
                (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                jobOrder.type.jobStatus != "P")
                this.TextBlock_YardLoc.Text = jobOrder.locFrom.location;
            else
                this.TextBlock_YardLoc.Text = jobOrder.locWorking.location;

            this.TextBlock_ITVNo.Text = jobOrder.partnerMchn.mchnId;
            this.TextBlock_CM.Text = jobOrder.cntr.cntrCgoTp;
            //this.TextBlock_Opr.Text = jobOrder.cntr.opr;
            this.TextBlock_ApproachLane.Text = jobOrder.partnerMchn.aprchLn;
            this.TextBlock_Weight.Text = jobOrder.cntr.cntrWgt;
            //this.TextBlock_POD.Text = jobOrder.cntr.pod;

            this.TextBlock_QCID.Text = jobOrder.type.qcId;
            this.TextBlock_QCID.Visibility = jobOrder.type.jobTp == "LD" ? Visibility.Visible : Visibility.Hidden;                

            if (jobOrder.cntr.cntrCgoTp == "R" || jobOrder.cntr.cntrCgoTp == "RD")
            {
                this.Image_reefer.Source = PresentationMgr.GetImageSource(@"/Images/Common/JobList/Icon/Reefer.png");
                this.Image_reefer.Visibility = Visibility.Visible;
            }
            else if (jobOrder.cntr.cntrCgoTp == "MH" || jobOrder.cntr.cntrCgoTp == "RH" || jobOrder.cntr.cntrCgoTp == "H")
            {
                this.Image_reefer.Source = PresentationMgr.GetImageSource(@"/Images/Common/JobList/Icon/Hazard.png");
                this.Image_reefer.Visibility = Visibility.Visible;
            }
            else
                this.Image_reefer.Visibility = Visibility.Hidden;
            //this.Image_reefer.Visibility = (jobOrder.cntr.cntrCgoTp == "R" || jobOrder.cntr.cntrCgoTp == "RD" || jobOrder.cntr.cntrCgoTp == "RH") ?
            //    Visibility.Visible : Visibility.Hidden;            

            this.Image_OOG.Visibility = jobOrder.cntr.cntrSpTp.Equals("OOG") ? Visibility.Visible : Visibility.Hidden;
            /////////////////////////////////////

            this.Image_Vehicle.Source = GetVehicleIcon(jobOrder.type.jobFlagInfo);

            if (false)
            {
                this.Image_PowIn.Visibility = Visibility.Visible;
                if (jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "GC")
                    this.Image_PowIn.Source = PresentationMgr.GetImageSource(@"/Images/Common/JobList/Vehicle/Vehicle_OTR_PowIn.png");
                else
                    this.Image_PowIn.Source = PresentationMgr.GetImageSource(@"/Images/Common/JobList/Vehicle/Vehicle_ITV_PowIn.png");
            }
            else            
                this.Image_PowIn.Visibility = Visibility.Hidden;            

            if (jobOrder.type.jobStatus == "A" || jobOrder.type.jobStatus == "P")
                this.Grid_Dim.Visibility = System.Windows.Visibility.Hidden;
            else
                this.Grid_Dim.Visibility = System.Windows.Visibility.Visible;
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
            }

            return PresentationMgr.GetImageSource(imgUri);
        }

        public BitmapImage GetVehicleIcon(String jobFlagInfo)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/JobList/Vehicle/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/JobList/Vehicle/";
            }

            if (_Selected)
            {
                switch (jobFlagInfo)
                {
                    case "C":
                        imgUri += "Vehicle_40F_Selected.png";
                        break;
                    case "F":
                        imgUri += "Vehicle_20F_Fore_Selected.png";
                        break;
                    case "A":
                        imgUri += "Vehicle_20F_After_Selected.png";
                        break;
                    default:
                        imgUri += "Vehicle_Unknown_Selected.png";
                        break;
                }
            }
            else
            {
                switch (jobFlagInfo)
                {
                    case "C":
                        imgUri += "Vehicle_40F.png";
                        break;
                    case "F":
                        imgUri += "Vehicle_20F_Fore.png";
                        break;
                    case "A":
                        imgUri += "Vehicle_20F_After.png";
                        break;
                    default:
                        imgUri += "Vehicle_Unknown.png";
                        break;
                }
            }

            return PresentationMgr.GetImageSource(imgUri);
        }
    }
}