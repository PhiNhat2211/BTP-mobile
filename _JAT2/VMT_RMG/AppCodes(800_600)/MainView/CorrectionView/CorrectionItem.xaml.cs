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
    /// Interaction logic for CorrectionItem.xaml
    /// </summary>
    public partial class CorrectionItem : UserControl
    {
        public class NoWorkValue
        {
            public const Int32 VALUE_NONE   = 0x00;
            public const Int32 VALUE_AREA   = 0x01;
            public const Int32 VALUE_TUNNEL = 0x02;
            public const Int32 VALUE_TIER   = 0x04;
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory _inventory;
        private String _jobKey = String.Empty;
        private Boolean _Selected = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();
        private SolidColorBrush _textBlockSelectedForeground = new SolidColorBrush(Color.FromArgb(255, 70, 72, 72));
        private SolidColorBrush _textBlockNormalForeground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private Int32 _noWorkValues = NoWorkValue.VALUE_NONE;

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
                RefreshCorrectionItem();
            }
        }

        public CorrectionItem()
        {
            this.InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            AppendTextBlockList();
            this._noWorkValues = NoWorkValue.VALUE_NONE;
        }

        ~CorrectionItem()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(TextBlock_Inventory_Length);
            _TextBlockList.Add(TextBlock_Inventory_FullNEmpty);
            //_TextBlockList.Add(TextBlock_Inventory_Commodity);
            //_TextBlockList.Add(TextBlock_Inventory_ClassCode);
            _TextBlockList.Add(TextBlock_Inventory_Type);
            _TextBlockList.Add(TextBlock_Inventory_Operator);
            //_TextBlockList.Add(TextBlock_Inventory_Grade);
            _TextBlockList.Add(TextBlock_Inventory_Number);
        }

        public void Clear()
        {
            this.Selected = false;
            this._noWorkValues = NoWorkValue.VALUE_NONE;
            this.SetInventoryInfo(null);
        }

        private void RefreshCorrectionItem()
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
        }

        public void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_inventory != null)
            {
                this.Image_ContainerType.Source = GetBGImageByContainerType("Exist");
                this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(_inventory.cntr.cntrTp);
            }
            else
            {
                this.Image_ContainerType.Source = GetBGImageByContainerType(String.Empty);
            }
        }

        private void InitSkinImage()
        {
            this.Image_ContainerType.Source = GetBGImageByContainerType(String.Empty);
            this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(String.Empty);

            this.Image_Inventory_JobType.Source = null;
            this.Image_Inventory_OOG.Source = null;
            //this.Grid_TextArear.Visibility = System.Windows.Visibility.Visible;            

            this.TextBlock_Inventory_Length.Text = String.Empty;
            this.TextBlock_Inventory_FullNEmpty.Text = String.Empty;
            this.TextBlock_Inventory_Commodity.Text = String.Empty;
            this.TextBlock_Inventory_ClassCode.Text = String.Empty;
            this.TextBlock_Inventory_Type.Text = String.Empty;
            this.TextBlock_Inventory_Operator.Text = String.Empty;
            this.TextBlock_Inventory_Grade.Text = String.Empty;
            this.TextBlock_Inventory_Number.Text = String.Empty;
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
            this.Image_ContainerType_Cover.Source = this.GetCoverImageByContainerType(coverType);
        }

        public void SetInventoryInfo(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven)
        {
            _inventory = inven;

            if (inven == null)
            {
                InitSkinImage();                
                return;
            }

            this.Image_ContainerType.Source = GetBGImageByContainerType("Exist"); // Exist Container
            this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(inven.cntr.cntrTp);
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
            this.TextBlock_Inventory_Length.Text = inven.cntr.cntrLen;
            this.TextBlock_Inventory_FullNEmpty.Text = inven.cntr.fullMty;
            this.TextBlock_Inventory_Commodity.Text = inven.cntr.cntrCgoTp;
            this.TextBlock_Inventory_ClassCode.Text = inven.cntr.cls;
            this.TextBlock_Inventory_Type.Text = inven.cntr.cntrTp;
            this.TextBlock_Inventory_Operator.Text = inven.cntr.opr;
            this.TextBlock_Inventory_Grade.Text = inven.cntr.cntrGrade;
            this.TextBlock_Inventory_Number.Text = inven.cntr.cntrNo.Substring(inven.cntr.cntrNo.Length - 4, 4);

            this.Image_Inventory_OOG.Source = inven.cntr.cntrSpTp.Equals("OOG") ? PresentationMgr.GetImageSource(@"/Images/Common/JobList/Icon/OOG.png") : null;
        }

        public void SetJobInfo(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if (jobOrder == null)
                return;

            _jobKey = jobOrder.jobKey;
            this.Image_Inventory_JobType.Source = GetJobTypeIcon(jobOrder.type.jobTp);
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
                else
                    imgUri += "Inventory_Default";
            }

            imgUri += ".png";
            return PresentationMgr.GetImageSource(imgUri);
        }

        public BitmapImage GetCoverImageByContainerType(String cntrTp)
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

            if (String.IsNullOrEmpty(cntrTp))
            {
                return null;
                //imgUri += "Inventory_Empty.png";
                //return PresentationMgr.GetImageSource(imgUri);
            }

            switch (cntrTp)
            {
                case "GE": //GE : General
                case "TK": //TK : Tank
                case "FR": //FR : Flat Rack
                case "OP": //OT : Open Top
                case "BK": //BK : Dry Bulk
                case "AS": // AS : Air Surface"
                    return null;
                //imgUri += "Inventory_Default";
                //break;
                case "RF": //RF :  Reefer
                    {
                        if (_inventory.cntr.reefer.plugCd.Equals("PIM") ||
                           _inventory.cntr.reefer.plugCd.Equals("POW") ||
                           _inventory.cntr.reefer.plugCd.Equals("ROW"))
                        {
                            imgUri += "Inventory_Reefer_PlugIn"; // Red
                        }
                        else
                        {
                            imgUri += "Inventory_Reefer_PlugOut"; // Blue
                        }
                    }
                    break;
                case "DG": // DG :Damage
                    imgUri += "Inventory_Damaged";
                    break;
                case "ET": // ET :Extended
                    imgUri += "Inventory_Extend40ft";
                    break;
                case "HD": // HD :Hold
                    imgUri += "Inventory_Hold";
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
            }

            return PresentationMgr.GetImageSource(imgUri);
        }
    }
}