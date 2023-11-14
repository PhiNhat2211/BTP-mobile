using System;
using System.Collections.Generic;
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
	public partial class LinkPopupView : UserControl
	{
        public delegate void Callback_Popup(int seleted);


        private int linkPopupIdx = 0;
        private String itemSelected = "";
        public VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder = new VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder();
        private List<StringBuilder> linkPopupList = new List<StringBuilder>(), linkSearchList = new List<StringBuilder>();

        public LinkPopupView()
		{
			this.InitializeComponent();
		}
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Wrap_Link_Popup.MouseLeftButtonUp += new MouseButtonEventHandler(Wrap_Link_Popup_MouseLeftButtonUp);
        }

        private void Wrap_Link_Popup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is UnlinkPopupItem))
                return;

            foreach (var cSearchControl in Wrap_Link_Popup.Children)
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

        public void AddLinkInfo(VMT_Data_JAT2.Objects.ITV.VD_Common_ChassisInventory value)
        {
            if (value != null)
            {
                Wrap_Link_Popup.Children.Clear();
                linkPopupList = new List<StringBuilder>();
                //foreach (var info in value.DicBlock)
                //{
                //    linkPopupList.Add(new StringBuilder(info.Key.ToString()));
                //}
                for (int i = 0; i < 25; i++)
                {
                    linkPopupList.Add(new StringBuilder(i.ToString()));
                }
                ShowLinkPopupItem();
                if (TextBox_Link_Popup_Search.Text != "")
                {
                    Wrap_Link_Popup.Children.Clear();
                    linkSearchList = new List<StringBuilder>();
                    for (int j = 0; j < linkPopupList.Count; j++)
                    {
                        if ((linkPopupList[j].ToString().ToUpper()).Contains(TextBox_Link_Popup_Search.Text.ToUpper()))
                        {
                            UnlinkPopupItem unlinkPopupItem = new UnlinkPopupItem();
                            unlinkPopupItem.SetUnlinkPopupItemInfo(linkPopupList[j].ToString());
                            Wrap_Link_Popup.Children.Add(unlinkPopupItem);
                            linkSearchList.Add(linkPopupList[j]);
                        }
                    }
                    linkPopupList.Clear();
                    linkPopupList = linkSearchList;
                }
            }
        }

        private void Btn_Link_Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            linkPopupList.Clear();
        }

        private void Btn_Link_Click(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            linkPopupList.Clear();
            VMT_Data_JAT2.VMT_DataMgr_ITV.ItvLinkChassis(chassOrder);
        }

        private void Btn_Link_Popup_Up_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (linkPopupIdx > 0)
            {
                linkPopupIdx -= 1;
                ShowLinkPopupItem();
            }
        }
        private void Btn_Link_Popup_Down_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (linkPopupIdx < linkPopupList.Count - 3)
            {
                linkPopupIdx += 1;
                ShowLinkPopupItem();
            }
        }
        private void Btn_Link_Popup_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Wrap_Link_Popup.Children.Clear();
            VMT_Data_JAT2.VMT_DataMgr_ITV.ValidChassisInfos(chassOrder);
        }

        private void ShowLinkPopupItem()
        {
            Wrap_Link_Popup.Children.Clear();
            int i, c = linkPopupIdx + 3;
            for (i = linkPopupIdx; i < c; i++)
            {
                if(i < linkPopupList.Count)
                {
                    UnlinkPopupItem linkPopupItem = new UnlinkPopupItem();
                    linkPopupItem.SetUnlinkPopupItemInfo(linkPopupList[i].ToString());
                    Wrap_Link_Popup.Children.Add(linkPopupItem);
                }
            }
        }
        //  public void Show
    }
}