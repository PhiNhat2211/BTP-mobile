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

using VMT_Data_JAT2;

//using ExternalAPI;

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for BayView.xaml
    /// </summary>
    public partial class BayView : UserControl
    {
        public enum BayView_BayPositionType
        {
            BayView_BayPositionType_Unknown,
            BayView_BayPositionType_Prev,
            BayView_BayPositionType_Current,
            BayView_BayPositionType_Next
        }

        public BayView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            //VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
            //    new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
        }

        ~BayView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                this.Image_InventoryPanel_BG.Source = UIThemeMgr.Day.AvailableView_BGImage; // new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/BayView_BG.png", UriKind.Relative));
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                this.Image_InventoryPanel_BG.Source = UIThemeMgr.Night.AvailableView_BGImage; //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/BayView_BG.png", UriKind.Relative));
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);

            //PresentationMgr.SetSkinCheckBox(this.Btn_CPS,
            //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/StopView/InventoryView/InventoryView_cps_indicator_on.png", UriKind.Relative)),
            //    new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/StopView/InventoryView/InventoryView_cps_indicator_off.png", UriKind.Relative))
            //    );
        }       

        public Boolean ContainerList_Target(Int32 row, Int32 tier)
        {
            if (row < 1 || row > 7 ||
                tier < 1 || tier > 7)
                return false;

            ContainerItem containerItem =
                PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + row.ToString() + "_" + tier.ToString());

            if (containerItem != null)
            {
                containerItem.Selected = true;
                return true;
            }
            return false;
        }

        public void ContainerList_Add(Int32 row, Int32 tier, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven, 
            int noWork = ContainerItem.NoWorkValue.VALUE_NONE, //String coverType = "",            
            BayView_BayPositionType positionType = BayView.BayView_BayPositionType.BayView_BayPositionType_Current)
        {
            if (row < 1 || row > 7 ||
                tier < 1 || tier > 7)
                return;

            ContainerItem containerItem =
                PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + row.ToString() + "_" + tier.ToString());

            if (containerItem != null)
            {
                if (inven == null)
                {
                    containerItem.SetNoWorkValue(noWork);
                    //containerItem.Image_ContainerType_Cover.Source = containerItem.GetCoverImageByContainerType(coverType);
                }
                else
                {
                    var intCurrentBay = Convert.ToInt32(PresentationMgr.Singleton.CurrentBay);
                    var intBay = Convert.ToInt32(inven.loc.bay);

                    if (inven.loc.blck.Equals(PresentationMgr.Singleton.CurrentBlock))
                    {
                        if (intCurrentBay - 1 == intBay)
                            inven.cntr.cntrTp = "ET";   // Inventory_container_extend40ft // TODO
                        else if (inven.cntr.isDmg)
                            inven.cntr.cntrTp = "DG";
                        else if (inven.cntr.isHold)
                            inven.cntr.cntrTp = "HD";
                        else if (intCurrentBay > intBay)
                            return;

                        containerItem.SetInventoryInfo(inven);

                        var jobList = PresentationMgr.Singleton.JOB_GetForLocation(inven.loc);
                        if (jobList != null && jobList.Count > 0)
                        {
                            containerItem.SetJobInfo(jobList[0]);
                            jobList.Clear();
                            jobList = null;
                        }

                        // OOG(Out of Gauge) : Over Slot Gauge(F/B/L/R/T) = 11/12/13/14/15
                        if (!String.IsNullOrEmpty(inven.cntr.overValue))
                        {
                            var valueArray = inven.cntr.overValue.Split('/');
                            if (valueArray.Length > 2 && !String.IsNullOrEmpty(valueArray[2]) && row > 1)
                            {
                                var leftCntr =
                                    PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + (row - 1).ToString() + "_" + tier.ToString());
                                leftCntr.SetOverValue(ContainerItem.OverValue.VALUE_RIGHT);
                            }
                            if (valueArray.Length > 3 && !String.IsNullOrEmpty(valueArray[3]) && row < 7)
                            {
                                var rightCntr =
                                    PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + (row + 1).ToString() + "_" + tier.ToString());
                                rightCntr.SetOverValue(ContainerItem.OverValue.VALUE_LEFT);
                            }
                            if (valueArray.Length > 4 && !String.IsNullOrEmpty(valueArray[4]) && tier < 7)
                            {
                                var topCntr =
                                    PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + row.ToString() + "_" + (tier + 1).ToString());
                                topCntr.SetOverValue(ContainerItem.OverValue.VALUE_TOP);
                            }
                        }
                    }
                }
            }
        }

        public void SetContainerJobInfo(Int32 tRow, Int32 tTier, VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder, Boolean isCorrectionSelected = false)
        {
            ContainerItem containerItem =
                PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + Convert.ToString(tRow) + "_" + Convert.ToString(tTier));
            if (containerItem != null)
            {
                containerItem.SetJobInfo(targetJobOrder);
                containerItem.CorrectionSelected = isCorrectionSelected;
            }
        }

        public void ContainerList_Del(Int32 row, Int32 tier)
        {
            if (row < 1 || row > 7 ||
                tier < 1 || tier > 7)
                return;

            ContainerItem containerItem =
                PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + row.ToString() + "_" + tier.ToString());

            if (containerItem != null)            
                containerItem.Clear();            
        }

        public void ContainerList_Clear()
        {
            //List<ContainerItem> containerItemList = new List<ContainerItem>();
            //PresentationMgr.FindChildByType<ContainerItem>((DependencyObject)this, containerItemList);
            
            //foreach (ContainerItem containerItem in containerItemList)
            //{
            //    containerItem.Selected = false;
            //    containerItem.SetInventoryInfo(null);
            //}

            for (int i = 0; i < this.Canvas_BayWorkInventory.Children.Count; i++)
            {
                if (this.Canvas_BayWorkInventory.Children[i] is ContainerItem)
                {
                    var item = this.Canvas_BayWorkInventory.Children[i] as ContainerItem;
                    item.Clear();                    
                }
            }
        }

        public void ClearContainerCorrectionSelect()
        {
            for (int i = 0; i < this.Canvas_BayWorkInventory.Children.Count; i++)
            {
                if (this.Canvas_BayWorkInventory.Children[i] is ContainerItem)
                {
                    var item = this.Canvas_BayWorkInventory.Children[i] as ContainerItem;
                    item.CorrectionSelected = false;
                }
            }
        }

        public void SetContainerPositions(String block, String bay, int startRow, int startTier, Int32 maxTier,
            SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> dic,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            for (int r = 1; r <= 7; r++)
            {
                for (int t = 1; t <= 7; t++)
                {
                    ContainerItem containerItem =
                        PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + r.ToString() + "_" + t.ToString());
                    if (containerItem != null)
                    {                        
                        containerItem.ContainerPos.m_cBlock = block;
                        containerItem.ContainerPos.m_cBay = bay;
                        containerItem.ContainerPos.m_cRow = PresentationMgr.ConvertNumberToRow(dic, startRow + r - 1, direction);
                        containerItem.ContainerPos.m_cTier = (startTier + t).ToString();

                        if (startTier + t > maxTier)
                            containerItem.SetNoneItem();
                    }
                }
            }            
        }
    }
}