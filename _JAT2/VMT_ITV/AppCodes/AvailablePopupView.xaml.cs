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
using Common.Interface;
using System.Collections;
using System.Threading;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for AvailablePopupView.xaml
    /// </summary>
    public partial class AvailablePopupView : UserControl
    {
        public String _strReasonCd { get; set; }

        public String _strReasonNm { get; set; }

        private MainView mMainView = null;

        int page = 1;

        private ArrayList stopCodeList = new ArrayList();
        public AvailablePopupView()
        {
            this.InitializeComponent();
        }

        public void Init(MainView mainView)
        {
            mMainView = mainView;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.btn_Exit.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0008", LanguageService.LABEL_AVAILABLE);
            this.Btn_Prev.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0013", LanguageService.LABEL_AVAILABLE);
            this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0014", LanguageService.LABEL_AVAILABLE);

            this.Btn_Prev.Click += new RoutedEventHandler(Btn_Prev_Click);
            this.Btn_Next.Click += new RoutedEventHandler(Btn_Next_Click);

            HideButton();
        }

        public void setStopCodeList(ArrayList list)
        {
            //Assign List
            stopCodeList = list;

            //ADD NEW LIST
            page = 1;
            ShowButton();

            CheckPrevNextBtn();
        }
        private void HideButton()
        {
            foreach (Button btn in Grid_Available.Children)
            {
                if (btn.Content.ToString() != PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0008", LanguageService.LABEL_AVAILABLE))
                    btn.Visibility = Visibility.Hidden;
            }
        }
        private void ShowButton()
        {
            HideButton();
            int i = 0;
            foreach (AvailablePopupView item in stopCodeList)
            {
                i++;
                if ((i - 1) / 8 + 1 < page)
                {
                    continue;
                }
                foreach (Button btn in Grid_Available.Children)
                {
                    if (btn.Visibility == System.Windows.Visibility.Hidden)
                    {
                        btn.Content = item._strReasonNm;
                        btn.ToolTip = item._strReasonCd;
                        btn.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                }
            }
        }
        public void SetJobInfo(String strReasonNm, String strReasonCd)
        {
            _strReasonNm = strReasonNm;
            _strReasonCd = strReasonCd;
        }

        public void Show_Popup()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide_Popup()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            mMainView.TextBlock_StopPage.Background = mMainView.colorEnable;
            Hide_Popup();
        }

        private void HideAvaliableView()
        {
            mMainView.HideAvaliableView();
        }

        private void Btn_BreakDown_Click(object sender, RoutedEventArgs e)
        {
            Button selectButton = (Button)sender;
            string reasonNm = selectButton.Content.ToString();
            string reasonCd = selectButton.ToolTip.ToString();

            HideAvaliableView();

            VMT_Data_JAT2.Objects.Common.VD_Common_Available availableSelectData = new VMT_Data_JAT2.Objects.Common.VD_Common_Available();
            availableSelectData.ReasonNm = reasonNm;
            availableSelectData.ReasonCd = reasonCd;

            mMainView.ShowBreak(availableSelectData);
            mMainView.TextBlock_StopPage.Text = reasonNm;
            mMainView.Complete_Break_Event();
        }
        private void CheckPrevNextBtn()
        {
            Tbl_Page.Text = page.ToString() + "/" + (stopCodeList.Count / 8 + ((stopCodeList.Count % 8 > 0) ? 1 : 0));
            if (page <= 1)
            {
                Btn_Prev.IsEnabled = false;
                if (page <= stopCodeList.Count / 8 + ((stopCodeList.Count % 8 > 0) ? 0 : -1))
                {
                    Btn_Next.IsEnabled = true;
                }
                else Btn_Next.IsEnabled = false;
            }
            else if (page > stopCodeList.Count / 8 + ((stopCodeList.Count % 8 > 0) ? 0 : -1))
            {
                Btn_Next.IsEnabled = false;
                if (page > 1)
                {
                    Btn_Prev.IsEnabled = true;
                }
                else Btn_Prev.IsEnabled = false;
            }
            else
            {
                Btn_Prev.IsEnabled = true;
                Btn_Next.IsEnabled = true;
            }
        }
        private void Btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            page = page - 1;
            ShowButton();
            CheckPrevNextBtn();
        }
        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            page = page + 1;
            ShowButton();
            CheckPrevNextBtn();
        }
    }
}