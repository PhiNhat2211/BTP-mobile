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
using System.Windows.Threading;

//20190108
using Common.Interface;

namespace VMT_ITV
{
	/// <summary>
	/// Interaction logic for uc_Popup.xaml
	/// </summary>
	public partial class UnLinkPopupView : UserControl
	{
        private int unlinkIdx = 0, unLinkPopupIdx = 0;
        private String itemSelected = "", unlinkItemSeslected = "";
        public VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder = new VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder();
        private List<StringBuilder> unLinkList = new List<StringBuilder>(), unLinkPopupList = new List<StringBuilder>(), unLinkSearchList = new List<StringBuilder>();

        public UnLinkPopupView()
		{
            this.InitializeComponent();
		}
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Wrap_Unlink_Popup.MouseLeftButtonUp += new MouseButtonEventHandler(Wrap_Unlink_Popup_MouseLeftButtonUp);
        }

        private void Wrap_Unlink_Popup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is UnlinkPopupItem))
                return;

            foreach (var cSearchControl in Wrap_Unlink_Popup.Children)
            {
                if (cSearchControl is UnlinkPopupItem)
                {
                    (cSearchControl as UnlinkPopupItem).Selected = false;
                }
            }

            UnlinkPopupItem mSearchControl = e.Source as UnlinkPopupItem;
            mSearchControl.Selected = true;
            itemSelected = mSearchControl._strItemText;
        }

        private void ShowUnLinkItem()
        {
            Wrap_Unlink.Children.Clear();

            int i, c = unlinkIdx + 6;
            for (i = unlinkIdx; i < c; i++)
            {
                UnlinkItem unlinkItem = new UnlinkItem();
                unlinkItem.SetUnlinkItemInfo(unLinkList[i].ToString());
                Wrap_Unlink.Children.Add(unlinkItem);
            }
        }

        private void Btn_Unlink_Up_Click(object sender, MouseButtonEventArgs e)
        {
            if (unlinkIdx > 0)
            {
                unlinkIdx -= 1;
                ShowUnLinkItem();
            }
        }
        private void Btn_Unlink_Down_Click(object sender, MouseButtonEventArgs e)
        {
            if (unlinkIdx < unLinkList.Count - 1)
            {
                unlinkIdx += 1;
                ShowUnLinkItem();
            }
        }
        private void TextBox_BlockID_Click(object sender, MouseButtonEventArgs e)
        {
            Grid_Unlink_Popup.Visibility = System.Windows.Visibility.Visible;
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Ask();
        }
        private void Btn_Unlink_Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            unLinkList.Clear();
            Wrap_Unlink.Children.Clear();
        }
        private void Btn_Unlink_Unlink_Click(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            unLinkList.Clear();
            Wrap_Unlink.Children.Clear();
            if (!"".Equals(unlinkItemSeslected))
            {
                String[] item = unlinkItemSeslected.Split('-');
            }
            VMT_Data_JAT2.VMT_DataMgr_ITV.ItvUnLinkChassis(chassOrder);
        }

        private void Btn_Unlink_Popup_Up_Click(object sender, MouseButtonEventArgs e)
        {
            if (unLinkPopupIdx > 0)
            {
                unLinkPopupIdx -= 1;
                ShowUnLinkPopupItem();
            }
        }
        private void Btn_Unlink_Popup_Down_Click(object sender, MouseButtonEventArgs e)
        {
            if (unLinkPopupIdx < unLinkPopupList.Count - 3)
            {
                unLinkPopupIdx += 1;
                ShowUnLinkPopupItem();
            }
        }
        private void Btn_Unlink_Popup_Search_Click(object sender, MouseButtonEventArgs e)
        {
            Wrap_Unlink_Popup.Children.Clear();
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Ask();
        }


        public void AddBlockInfo(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            if (value != null)
            {
                Wrap_Unlink_Popup.Children.Clear();
                unLinkPopupList = new List<StringBuilder>();
                foreach (var info in value.DicBlock)
                {
                    unLinkPopupList.Add(new StringBuilder(info.Key.ToString()));
                }
                ShowUnLinkPopupItem();
                if (TextBox_Unlink_Popup_Search.Text != "")
                {
                    Wrap_Unlink_Popup.Children.Clear();
                    unLinkSearchList = new List<StringBuilder>();
                    for (int j = 0; j < unLinkPopupList.Count; j++)
                    {
                        if ((unLinkPopupList[j].ToString().ToUpper()).Contains(TextBox_Unlink_Popup_Search.Text.ToUpper()))
                        {
                            UnlinkPopupItem unlinkPopupItem = new UnlinkPopupItem();
                            unlinkPopupItem.SetUnlinkPopupItemInfo(unLinkPopupList[j].ToString());
                            Wrap_Unlink_Popup.Children.Add(unlinkPopupItem);
                            unLinkSearchList.Add(unLinkPopupList[j]);
                        }
                    }
                    unLinkPopupList.Clear();
                    unLinkPopupList = unLinkSearchList;
                }
            }
        }

        public void AddUnlinkBlockInfo(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            if (value.Count > 0)
            {
                foreach (var bay in value.DicBlock[PresentationMgr.Singleton.CurrentBlock].DicBay)
                {
                    foreach (var row in bay.Value.RowNameMap)
                    {
                        for (int i = 1; i <= value.DicBlock[PresentationMgr.Singleton.CurrentBlock].MaxTier; i++)
                        {
                            unLinkList.Add(new StringBuilder(bay.Value.BayName + " - " + row.Value.rowNm + " - " + i));
                        }
                    }
                }
                ShowUnLinkItem();
            }
        }

        private void ShowUnLinkPopupItem()
        {
            Wrap_Unlink_Popup.Children.Clear();
            int i, c = unLinkPopupIdx + 3;
            for (i = unLinkPopupIdx; i < c; i++)
            {
                if (i < unLinkPopupList.Count)
                {
                    UnlinkPopupItem unlinkPopupItem = new UnlinkPopupItem();
                    unlinkPopupItem.SetUnlinkPopupItemInfo(unLinkPopupList[i].ToString());
                    Wrap_Unlink_Popup.Children.Add(unlinkPopupItem);
                }
            }
        }

        private void Btn_Unlink_Popup_Select_Click(object sender, MouseButtonEventArgs e)
        {
            Grid_Unlink_Popup.Visibility = System.Windows.Visibility.Hidden;
            TextBox_Unlink_Popup_Search.Clear();
            unLinkPopupList.Clear();
            if (!"".Equals(itemSelected))
            {
                ShowPopup(itemSelected);
            }
        }

        private void Btn_Unlink_Popup_Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            TextBox_Unlink_Popup_Search.Clear();
            unLinkPopupList.Clear();
            Grid_Unlink_Popup.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ShowPopup(String block)
        {
            TextBox_BlockID.Text = block;
            PresentationMgr.Singleton.CurrentBlock = block;
            unLinkList.Clear();
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapListForYt_Ask(block);
            this.Visibility = System.Windows.Visibility.Visible;
        }

        private void Wrap_Unlink_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(e.Source is UnlinkItem))
                return;
            
            foreach (var cSearchControl in Wrap_Unlink.Children)
            {
                if (cSearchControl is UnlinkItem)
                {
                    (cSearchControl as UnlinkItem).Selected = false;
                }
            }

            UnlinkItem mSearchControl = e.Source as UnlinkItem;
            mSearchControl.Selected = true;
            unlinkItemSeslected = mSearchControl._strItemText;
        }
    }
}