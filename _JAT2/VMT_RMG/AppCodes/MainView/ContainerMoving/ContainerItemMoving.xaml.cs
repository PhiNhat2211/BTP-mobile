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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for ContainerItem.xaml
    /// </summary>
    public partial class ContainerItemMoving : UserControl
    {
        public class NoWorkValue
        {
            public const Int32 VALUE_NONE = 0x00;
            public const Int32 VALUE_AREA = 0x01;
            public const Int32 VALUE_TUNNEL = 0x02;
            public const Int32 VALUE_TIER = 0x04;
        }

        public class OverValue
        {
            public const Int32 VALUE_NONE = 0x00;
            public const Int32 VALUE_LEFT = 0x01;
            public const Int32 VALUE_RIGHT = 0x02;
            public const Int32 VALUE_TOP = 0x04;
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory _inventory;
        private String _jobKey = String.Empty;
        private Boolean _Selected = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();
        private SolidColorBrush _textBlockSelectedForeground = new SolidColorBrush(Color.FromArgb(255, 70, 72, 72));
        private SolidColorBrush _textBlockNormalForeground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private Int32 _noWorkValues = NoWorkValue.VALUE_NONE;
        private Int32 _overValues = OverValue.VALUE_NONE;

        public VMT_Data_JAT2.Marshalling.Geometry.sPosition ContainerPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();

        public VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory Inventory
        {
            get { return _inventory; }
            set
            {
                _inventory = value;
                SetInventoryInfo(_inventory);
            }
        }

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
                RefreshContainerItem();
            }
        }

        public Boolean CorrectionSelected
        {
            get
            {
                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                    return this.Image_Container_CorrectionSelect.Visibility == Visibility.Visible;
                else
                    return this.Image_Container_CorrectionSelect.Visibility == Visibility.Hidden;
            }
            set
            {
                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                    this.Image_Container_CorrectionSelect.Visibility = value == true ?
                        Visibility.Visible : Visibility.Hidden;
            }
        }

        public ContainerItemMoving()
        {
            this.InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            AppendTextBlockList();
            this._noWorkValues = NoWorkValue.VALUE_NONE;
        }

        ~ContainerItemMoving()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(TextBlock_Inventory_Length);
            _TextBlockList.Add(TextBlock_Inventory_FullNEmpty);
            _TextBlockList.Add(TextBlock_Inventory_Pod);
            _TextBlockList.Add(TextBlock_Inventory_Opr);
            _TextBlockList.Add(TextBlock_Inventory_Type);
            _TextBlockList.Add(TextBlock_Inventory_StackingDay);
            //_TextBlockList.Add(TextBlock_Inventory_Operator);
            //_TextBlockList.Add(TextBlock_Inventory_Grade);
            _TextBlockList.Add(TextBlock_Inventory_Number);
            //_TextBlockList.Add(TextBlock_Inventory_Number);
        }

        public void Clear()
        {
            this.Selected = false;
            this._noWorkValues = NoWorkValue.VALUE_NONE;
            this._overValues = OverValue.VALUE_NONE;
            this.SetInventoryInfo(null);
            this.ContainerPos.Clear();
            this._jobKey = String.Empty;
        }

        private void RefreshContainerItem()
        {
            this.Image_ContainerType.Source = GetBGImageByContainerType("Exist");

            foreach (TextBlock tBlock in _TextBlockList)
            {
                if (_Selected)
                    tBlock.Foreground = _textBlockSelectedForeground;
                //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 70, 72, 72));
                else
                    tBlock.Foreground = _textBlockNormalForeground;
                //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.MouseLeftButtonUp += new MouseButtonEventHandler(ContainerItem_MouseLeftButtonUp);
            this.MouseRightButtonDown += new MouseButtonEventHandler(ContainerItem_MouseRightButtonDown);
        }

        public void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_inventory != null)
            {
                this.Image_ContainerType.Source = GetBGImageByContainerType("Exist");
            }
            else
            {
                this.Image_ContainerType.Source = GetBGImageByContainerType(String.Empty);
            }
        }

        private void InitSkinImage()
        {
            if (App.STANDALONE_MODE)
                return;

            this.Image_ContainerType.Source = GetBGImageByContainerType(String.Empty);
            this.TextBlock_JobType.Text = "";
            this.TextBlock_OTR.Text = "";
            this.TextBlock_Inventory_WaitingTime.Text = "";

            this.TextBlock_Inventory_Pod.Text = String.Empty;
            this.TextBlock_Inventory_Opr.Text = String.Empty;
            this.TextBlock_Inventory_Length.Text = String.Empty;
            this.TextBlock_Inventory_FullNEmpty.Text = String.Empty;
            this.TextBlock_Inventory_Type.Text = String.Empty;
            TextBlock_Inventory_StackingDay.Text = String.Empty;
            TextBlock_Inventory_Number.Text = String.Empty;
            ClearInfo();
        }

        private void ContainerItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!PresentationMgr.UseCorrection && !App.TEST_MODE)
                return;

            PresentationMgr.AppWin.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (this._inventory != null)    // empty 영역이 아니면   
                            {
                                if (this._inventory.cntr != null)
                                    this.CorrectionSelected = PresentationMgr.Singleton.SetCorrectionSelectMoving(this.ContainerPos,
                                        this._inventory.cntr.cntrNo, this._inventory.cntr.cntrIso);
                                if ((PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_BlockText.Content).Equals(PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_BlockText.Content)
                                    && (PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_BayText.Content).Equals(PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_BayText.Content))
                                {
                                    ContainerItemMoving containerItem1 = PresentationMgr.FindChild<ContainerItemMoving>(PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1, this.Name);
                                    containerItem1.CorrectionSelected = this.CorrectionSelected;
                                    ContainerItemMoving containerItem2 = PresentationMgr.FindChild<ContainerItemMoving>(PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2, this.Name);
                                    containerItem2.CorrectionSelected = this.CorrectionSelected;
                                }
                            }
                            else
                            {
                                //var jobList = PresentationMgr.Singleton.JOB_GetForLocation(this.ContainerPos.m_cBlock, this.ContainerPos.m_cBay,
                                //    this.ContainerPos.m_cRow, this.ContainerPos.m_cTier);
                                //if (jobList.Count > 0)      
                                var job = String.IsNullOrEmpty(this._jobKey) ? null : PresentationMgr.Singleton.JOB_Get(this._jobKey);
                                if (job != null && job.type != null)
                                {
                                    //var job = jobList.First();
                                    if (job.type.jobTp == "GI" || job.type.jobTp == "DS" || job.type.jobTp == "MI" ||
                                        /*job.type.jobTp == "RH" || */job.type.jobTp == "LC" || job.type.jobTp == "GC")
                                        this.CorrectionSelected = PresentationMgr.Singleton.SetCorrectionSelectMoving(this.ContainerPos,
                                            job.cntr.cntrNo, job.cntr.cntrIso);
                                }
                                else
                                {
                                    PresentationMgr.Singleton.NeedJobAutoSelection = false;
                                    PresentationMgr.Singleton.MakeCorrection(this.ContainerPos);
                                }

                                //jobList = null;                
                            }
                        }));
        }

        private void ContainerItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PresentationMgr.AppWin.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (this._inventory != null && this._inventory.cntr != null && !String.IsNullOrEmpty(this._inventory.cntr.cntrNo))
                            {
                                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
                                PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this._inventory.cntr.cntrNo, null);
                            }
                            else
                            {
                                var job = String.IsNullOrEmpty(this._jobKey) ? null : PresentationMgr.Singleton.JOB_Get(this._jobKey);
                                if (job != null && job.type != null)
                                {
                                    if (job.type.jobTp == "GI" || job.type.jobTp == "DS" || job.type.jobTp == "MI" ||
                                        /*job.type.jobTp == "RH" || */job.type.jobTp == "LC" || job.type.jobTp == "GC")
                                    {
                                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
                                        PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(job.cntr.cntrNo, null);
                                    }
                                }
                            }
                        }));
        }

        public void SetNoWorkValue(Int32 value)
        {
            this._noWorkValues |= value;
            String coverType = String.Empty;
            switch (this._noWorkValues)
            {
                case NoWorkValue.VALUE_AREA:
                    coverType = "NA";
                    break;
                case NoWorkValue.VALUE_TUNNEL:
                    coverType = "TN";
                    break;
                case NoWorkValue.VALUE_TIER:
                    coverType = "NT";
                    break;
                case NoWorkValue.VALUE_AREA | NoWorkValue.VALUE_TUNNEL:
                    coverType = "NA&TN";
                    break;
                case NoWorkValue.VALUE_AREA | NoWorkValue.VALUE_TIER:
                    coverType = "NA&NT";
                    break;
                case NoWorkValue.VALUE_TUNNEL | NoWorkValue.VALUE_TIER:
                    coverType = "TN&NT";
                    break;
                case NoWorkValue.VALUE_AREA | NoWorkValue.VALUE_TUNNEL | NoWorkValue.VALUE_TIER:
                    coverType = "NA&TN&NT";
                    break;
                default:
                    break;
            }
        }

        private BitmapImage GetOvervalueImage(Int32 value)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }

            switch (value)
            {
                case OverValue.VALUE_LEFT:
                    imgUri += "Inventory_OvervalueL";
                    break;
                case OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueR";
                    break;
                case OverValue.VALUE_TOP:
                    imgUri += "Inventory_OvervalueB";
                    break;
                case OverValue.VALUE_LEFT | OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueLR";
                    break;
                case OverValue.VALUE_TOP | OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueRB";
                    break;
                case OverValue.VALUE_TOP | OverValue.VALUE_LEFT:
                    imgUri += "Inventory_OvervalueLB";
                    break;
                case OverValue.VALUE_TOP | OverValue.VALUE_LEFT | OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueLRB";
                    break;
                default:
                    break;
            }
            imgUri += ".png";

            return PresentationMgr.GetImageSource(imgUri);
        }

        public void SetInventoryInfo(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven)
        {
            _inventory = inven;

            if (inven == null)
            {
                InitSkinImage();
                return;
            }

            this.ContainerPos.m_cBlock = _inventory.loc.blck;
            this.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(_inventory.loc.bay);
            this.ContainerPos.m_cRow = _inventory.loc.row;
            this.ContainerPos.m_cTier = _inventory.loc.tier;

            this.Image_ContainerType.Source = GetBGImageByContainerType("Exist"); // Exist Container
            //this.Grid_TextArear.Visibility = System.Windows.Visibility.Visible;

            /*
             * •	1 : Length
               •	2 : F/E (F:Full / E:Empty)
               •	3 : Commodity
               •	4 : Class code
               •	5 : Container type / HC
               (For a high cubic container, instead of a container type, HC is displayed.)
               •	6 : Operator
               •	7 : Container grade (SD)
               •	8 : Last 4 digit of Container number 
             */

            this.TextBlock_Inventory_StackingDay.Text = inven.stkDay;
            if (!"".Equals(inven.stkDay))
            {
                int day = Convert.ToInt32(inven.stkDay);
                if(day <= 10)
                    this.TextBlock_Inventory_StackingDay.Foreground = new SolidColorBrush(Colors.Gray);
                else
                    this.TextBlock_Inventory_StackingDay.Foreground = new SolidColorBrush(Colors.Red);

            }
            this.TextBlock_Inventory_Pod.Text = inven.cntr.pod;
            this.TextBlock_Inventory_Opr.Text = inven.cntr.opr;
            this.TextBlock_Inventory_Length.Text = inven.cntr.cntrLen;
            this.TextBlock_Inventory_FullNEmpty.Text = inven.cntr.fullMty;
            this.TextBlock_Inventory_Type.Text = inven.cntr.cntrTp;
            this.TextBlock_Inventory_Number.Text = inven.cntr.cntrNo.Length > 3 ?
                inven.cntr.cntrNo.Substring(inven.cntr.cntrNo.Length - 4, 4) : String.Empty;
            if (inven.cntr.chkHold)
            {
                this.TextBlock_Inventory_Number.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                this.TextBlock_Inventory_Number.Foreground = new SolidColorBrush(Colors.Gray);
            }

            ClearInfo();

            if ("1".Equals(inven.pickSetChk) || "2".Equals(inven.pickSetChk))
            {
                TextBlock_JobType.Text = inven.jobTp;
                TextBlock_OTR.Text = inven.vehicle.Length > 3 ?
                inven.vehicle.Substring(inven.vehicle.Length - 4, 4) : String.Empty; ;
                TextBlock_Inventory_WaitingTime.Text = inven.otrWaitTime;
                if ("1".Equals(inven.pickSetChk)){
                    Pol_SetDown.Visibility = Visibility.Hidden;
                    Pol_PickUp.Visibility = Visibility.Visible;
                }
                else
                {
                    Pol_SetDown.Visibility = Visibility.Visible;
                    Pol_PickUp.Visibility = Visibility.Hidden;
                }

            }
            else if (inven.qrntn)
            {
                TextBlock_OTR.Text = inven.vehicle.Length > 3 ? inven.vehicle.Substring(inven.vehicle.Length - 4, 4) : String.Empty;
                Image_Quarantine.Visibility = Visibility.Visible;
            }
            else if (inven.qrntnRf)
            {
                TextBlock_OTR.Text = inven.vehicle.Length > 3 ?
                inven.vehicle.Substring(inven.vehicle.Length - 4, 4) : String.Empty;
                Image_Quarantine.Visibility = Visibility.Visible;
                Border_QuarantineRF.Visibility = Visibility.Visible;
            }
            else if (!"".Equals(inven.groupCode))
            {
                TextBlock_Inventory_ETB.Text = inven.etb;
                Star_AutoPick.Visibility = Visibility.Visible;
            }

            this.CorrectionSelected = inven.IsCorrectionSelect;
        }

        public void SetOverValue(Int32 value)
        {
            //this._overValues |= value;
            //BitmapImage img = this.GetOvervalueImage(this._overValues);

            //if (img != null)
            //{
            //    Image_ContainerType_OverValue.Source = img;
            //    Image_ContainerType_OverValue.Visibility = Visibility.Visible;
            //}
            //else
            //    Image_ContainerType_OverValue.Visibility = Visibility.Hidden;
        }

        public void ClearInfo()
        {
            Pol_SetDown.Visibility = Visibility.Hidden;
            Pol_PickUp.Visibility = Visibility.Hidden;
            TextBlock_JobType.Text = "";
            TextBlock_OTR.Text = "";
            TextBlock_Inventory_WaitingTime.Text = "";
            TextBlock_Inventory_ETB.Text = "";
            Border_QuarantineRF.Visibility = Visibility.Hidden;
            Star_AutoPick.Visibility = Visibility.Hidden;
            Image_Quarantine.Visibility = Visibility.Hidden;
        }

        public void SetJobInfo(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if (jobOrder == null)
                return;

            this._jobKey = jobOrder.jobKey;
            //this.Image_Inventory_JobType.Source = GetJobTypeIcon(jobOrder.type.jobTp);
            //this.Grid_TextArear.Visibility = Visibility.Visible;
            //this.Image_Inventory_JobType.Visibility = Visibility.Visible;
        }

        /*
        public void SetInventoryCover(String jobKey, Boolean bUIChange)
        {
            _jobKey = jobKey;

            if (jobKey == null)
            {
                // Hidden
                this.Image_ContainerType_Cover.Visibility = System.Windows.Visibility.Hidden;
                this.Image_ContainerType_Cover.Source = null;
            }
            else
            {
                // set & visible
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);

                if (jobOrder == null)
                    return;

                this.Image_ContainerType_Cover.Visibility = System.Windows.Visibility.Visible;
                this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(jobOrder.cntr.cntrTp);
            }
        }
        */

        public void SetNoneItem()
        {
            this.Image_ContainerType.Source = GetBGImageByContainerType("NoTier");
        }

        public BitmapImage GetBGImageByContainerType(String cntrTp)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }

            if (_Selected == true)
                imgUri += "Inventory_Select";
            else
            {
                if (String.IsNullOrEmpty(cntrTp))
                    imgUri += "Inventory_Empty";
                else if (cntrTp.Equals("NoTier"))
                    return null;
                else
                    imgUri += "Inventory_Default";
            }

            imgUri += ".png";
            return PresentationMgr.GetImageSource(imgUri);
        }

        public BitmapImage GetCoverImageByContainerType(String cntrTp)
        {
            if (String.IsNullOrEmpty(cntrTp))
                return null;

            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }

            // 커버만 Display되는 경우
            switch (cntrTp)
            {
                case "GE": //GE : General
                case "TK": //TK : Tank
                case "FR": //FR : Flat Rack
                case "OP": //OT : Open Top
                case "BK": //BK : Dry Bulk
                case "AS": // AS : Air Surface"
                case "RF": //RF :  Reefer                
                    return null;
                //imgUri += "Inventory_Default";
                //break;
                //case "HZ":
                //    {
                //        imgUri += "Inventory_Hazard";
                //    }
                //    break;
                //case "RF": //RF :  Reefer
                //    {
                //        if (_inventory.cntr.reefer.plugCd.Equals("PIM") ||
                //           _inventory.cntr.reefer.plugCd.Equals("POW") ||
                //           _inventory.cntr.reefer.plugCd.Equals("ROW"))
                //        {
                //            imgUri += "Inventory_Reefer_PlugIn"; // Red
                //        }
                //        else
                //        {
                //            imgUri += "Inventory_Reefer_PlugOut"; // Blue
                //        }
                //    }
                //    break;
                //case "DG": // DG :Damage
                //    imgUri += "Inventory_Damaged";
                //    break;
                //case "HD": // HD :Hold
                //    imgUri += "Inventory_Hold";
                //    break;
                case "ET": // ET :Extended
                    imgUri += "Inventory_Extend40ft";
                    break;
                case "NA": // NW :No Work Area
                    imgUri += "Inventory_NoWorkArea";
                    break;
                case "NT": // NT :No Work Tier
                    imgUri += "Inventory_NoWorkTier";
                    break;
                case "TN": // TN : Tunnel
                    imgUri += "Inventory_Tunnel";
                    break;
                case "NA&TN":
                    imgUri += "Inventory_NoWorkAreaNTunnel";
                    break;
                case "TN&NT":
                    imgUri += "Inventory_NoWorkTunnelNTier";
                    break;
                case "NA&NT":
                    imgUri += "Inventory_NoWorkAreaNTier";
                    break;
                case "NA&TN&NT":
                    imgUri += "Inventory_NoWorkAreaNTunnelNTier";
                    break;
                default:
                    return null;
                //imgUri += "Inventory_Default";
                //break;
            }

            imgUri += ".png";
            return PresentationMgr.GetImageSource(imgUri);
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
    }
}