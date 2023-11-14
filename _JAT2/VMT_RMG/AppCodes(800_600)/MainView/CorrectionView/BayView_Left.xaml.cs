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
    /// Interaction logic for BayView_Left.xaml
    /// </summary>
    public partial class BayView_Left : UserControl
    {
        //public enum BayView_BayPositionType
        //{
        //    BayView_BayPositionType_Unknown,
        //    BayView_BayPositionType_Prev,
        //    BayView_BayPositionType_Current,
        //    BayView_BayPositionType_Next
        //}

        public BayView_Left()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            //VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
            //    new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
        }

        ~BayView_Left()
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
            BayView.BayView_BayPositionType positionType = BayView.BayView_BayPositionType.BayView_BayPositionType_Current)
        {
            if (row < 1 || row > 7 ||
                tier < 1 || tier > 7)
                return;

            ContainerItem containerItem =
                PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + row.ToString() + "_" + tier.ToString());

            if (containerItem != null)
            {
                if (inven == null)
                    containerItem.SetNoWorkValue(noWork);
                    //containerItem.Image_ContainerType_Cover.Source = containerItem.GetCoverImageByContainerType(coverType);
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
                    }
                }
            }
        }

        public void SetContainerJobInfo(Int32 tRow, Int32 tTier, VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder)
        {
            ContainerItem containerItem =
                PresentationMgr.FindChild<ContainerItem>(this, "UC_Ctnr_" + Convert.ToString(tRow) + "_" + Convert.ToString(tTier));
            if (containerItem != null)
                containerItem.SetJobInfo(targetJobOrder);
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
    }
}