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
using VMT_Data_JAT2;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for EmptySwapPopupView.xaml
    /// </summary>
    public partial class EmptySwapPopupView : UserControl
    {
        public VMT_Data_JAT2.Objects.RTG.EmptySwapListSend emptySwapListSend = new VMT_Data_JAT2.Objects.RTG.EmptySwapListSend();
       
        public EmptySwapPopupView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitEvent();
            LoadLanguage();
        }
        private void InitEvent()
        {
            this.Grid_OK.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_OK_MouseLeftButtonDown);
            this.Grid_Cancel.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Cancel_MouseLeftButtonDown);
        }
        private void LoadLanguage()
        {
            Tbl_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0001", LanguageService.LABEL_EMPTYSWAP);
            Tbl_Message.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0002", LanguageService.LABEL_EMPTYSWAP);
            Tbl_TruckNo_lbl.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0005", LanguageService.LABEL_EMPTYSWAP);
            Tbl_Before_lbl.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_EMPTYSWAP);
            Tbl_After_lbl.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_EMPTYSWAP);
            Tbl_OK.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0003", LanguageService.LABEL_EMPTYSWAP);
            Tbl_Cancel.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0004", LanguageService.LABEL_EMPTYSWAP);
        }
        private void Grid_OK_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            VMT_DataMgr_RMG.DoEmptySwap_Ask(emptySwapListSend);
            //VMT_DataMgr_Common.setEmptySwap(emptySwapListSend.jobKey, emptySwapListSend.orgCntrNo, emptySwapListSend.newCntrNo, emptySwapListSend.regoNo);
        }
        private void Grid_Cancel_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
