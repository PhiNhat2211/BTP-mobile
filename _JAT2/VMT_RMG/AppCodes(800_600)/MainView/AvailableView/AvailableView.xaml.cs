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

namespace VMT_RMG_800by600
{
	/// <summary>
	/// Interaction logic for AvailableView.xaml
	/// </summary>
	public partial class AvailableView : UserControl
	{
		public AvailableView()
		{
			this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
		}

        ~AvailableView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                this.Image_Title_Available.Source = UIThemeMgr.Day.AvailableView_TitleImage;    // new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/AvailableView/Title_Available.png", UriKind.Relative));
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                this.Image_Title_Available.Source = UIThemeMgr.Night.AvailableView_TitleImage;   //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/AvailableView/Title_Available.png", UriKind.Relative));
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        public void MakeupAvailableItems(VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive values)
        {
            this.Wrap_AvailableView.Children.Clear();

            foreach(VMT_Data_JAT2.Objects.Common.VD_Common_Available item in values.m_pData)
            {
                AvailableControl control = new AvailableControl();
                control.ReasonCd = item.ReasonCd;
                control.ReasonNm = item.ReasonNm;
                this.Wrap_AvailableView.Children.Add(control);
            }
        }
	}
}