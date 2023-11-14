using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Common.Interface;
using VMT_Data_JAT2.Objects;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for ChassisChangeView.xaml
    /// </summary>
    public partial class ChassisChangeView : UserControl
    {
        private MainView mMainView = null;
        public ChassisChangeView()
        {
            this.InitializeComponent();
        }
        public void Init(MainView mainView)
        {
            mMainView = mainView;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLanguage();
        }       
        private void LoadLanguage()
        {
            this.Tbl_ChassisChange.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0001", LanguageService.LABEL_CHSSCHG);
            this.Lb_Before.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0002", LanguageService.LABEL_CHSSCHG);
            this.Lb_After.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0003", LanguageService.LABEL_CHSSCHG);
            this.Btn_Change.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0004", LanguageService.LABEL_CHSSCHG);
            this.Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0005", LanguageService.LABEL_CHSSCHG);
        }
        private void Tb_After_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.KeypadminiView.ShowKeyPad(Tb_After);
            Tb_After.Focus();
        }
        private void Tb_After_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.KeypadminiView.ShowKeyPad(Tb_After);
            Tb_After.Focus();
        }
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Tb_After_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void Tb_After_TextChanged(object senser, TextChangedEventArgs e)
        {
            CheckChangeBtn();
        }
        public void CheckChangeBtn()
        {
            this.Btn_Change.IsEnabled = Tb_After.Text.Length == 3;
        }
        private void Btn_Change_Click(object sender, RoutedEventArgs e)
        {
            if (Tb_After.Text != "000")
            {
                MainWindow.changeChssNo = true;
                String chssNo = Tb_After.Text;
                VMT_Data_JAT2.VMT_DataMgr_ITV.GetChssUsingData_Ask(chssNo);
            }
            else
            {

                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)PresentationMgr.AppWin.MainView.gJobOrderList[0];
                if ((currentJob.type.jobTp == "LD" || currentJob.type.jobTp == "DS") && currentJob.workingMchn.mchnSts == "L")
                {
                    PresentationMgr.AppWin.PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0042", LanguageService.LABEL_POPUP), PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0041", LanguageService.LABEL_POPUP), ""
                        , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
                }
                else
                {
                    String chassisNo = Tb_Before.Text;
                    String newChassisNo = Tb_After.Text;
                    VMT_Data_JAT2.VMT_DataMgr_ITV.changeChassisNo_Ask(chassisNo, newChassisNo);
                    this.Visibility = Visibility.Hidden;
                }
            }
        }

        public void ProcessChangeChassisNoCallback(String result)
        {
            if (String.IsNullOrEmpty(result) || result.ToUpper().Equals(ITV.ITV_User.gMchnID + "-" + Tb_After.Text) )
            {
                String chassisNo = Tb_Before.Text;
                String newChassisNo = Tb_After.Text;
                VMT_Data_JAT2.VMT_DataMgr_ITV.changeChassisNo_Ask(chassisNo, newChassisNo);
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                var mess = String.Format(PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0040", LanguageService.LABEL_POPUP), result);
                PresentationMgr.AppWin.PopupView.ShowPopup(1, PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0042", LanguageService.LABEL_POPUP), mess, ""
                    , PresentationMgr.Singleton.LanguageSer.GetResourceITV("MS0034", LanguageService.MESSAGE_GROUP), "", null, 0);
            }

        }
    }
}
