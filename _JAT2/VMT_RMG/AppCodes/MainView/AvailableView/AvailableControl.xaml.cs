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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AvailableControl : UserControl
    {
        private String _strReasonCd;
        private String _strReasonNm;

        public String ReasonCd
        {
            get { return _strReasonCd; }
            set { _strReasonCd = value; }
        }

        public String ReasonNm
        {
            get { return _strReasonNm; }
            set
            {
                _strReasonNm = value;
                this.Button_Available.Content = _strReasonNm;
            }
        }

        public AvailableControl()
        {
            InitializeComponent();
            // PreviewMouseLeftButtonUp
            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~AvailableControl()
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
                PresentationMgr.SetSkinButton(this.Button_Available,
                    UIThemeMgr.Day.AvailableView_ButtonMenuDefaultImage, UIThemeMgr.Day.AvailableView_ButtonMenuPressImage, UIThemeMgr.Day.AvailableView_ButtonMenuDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/AvailableView/Available_menu_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/AvailableView/Available_menu_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/AvailableView/Available_menu_default.png", UriKind.Relative)));
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Button_Available,
                    UIThemeMgr.Night.AvailableView_ButtonMenuDefaultImage, UIThemeMgr.Night.AvailableView_ButtonMenuPressImage, UIThemeMgr.Night.AvailableView_ButtonMenuDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/AvailableView/Available_menu_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/AvailableView/Available_menu_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/AvailableView/Available_menu_default.png", UriKind.Relative)));
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void Button_Available_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            switch (ReasonCd)
            {
                //case "Day/Night":
                //    {
                //        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
                //    }
                //    break;
                //case "Change Chassis":
                //    {
                //        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
                //    }
                //    break;
                case "Change Driver":
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.LogInView);
                    }
                    break;
                default:
                    {
                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BreakTimeView);
                        
                        BreakTimeView view = PresentationMgr.MainView.UC_BreakTimeView;
                        view.ReasonCd = ReasonCd;
                        view.ReasonNm = ReasonNm;
                        view.Waiting = BreakTimeView.WaitingType.SET;
                        PresentationMgr.MainView.UC_BreakTimeView.setMachineFlg = true;
                        VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStop_Ask();
                        //view.SetStartDateTime();
                        //if (PresentationMgr.MainView.UC_BlockJobView.Visibility == Visibility.Visible)
                        //    view = PresentationMgr.MainView.UC_BlockJobView.UC_BreakTimeView;
                        //else
                        //    view = PresentationMgr.MainView.UC_BreakTimeView;                        
                    }
                    break;
            }
        }
    }
}
