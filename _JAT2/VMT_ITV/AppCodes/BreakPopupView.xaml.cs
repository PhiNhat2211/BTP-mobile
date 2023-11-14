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

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for BreakPopupView.xaml
    /// </summary>
    public partial class BreakPopupView : UserControl
    {
        private MainView mMainView = null;
        public BreakPopupView()
        {
            this.InitializeComponent();
        }

        public void Init(MainView mainView)
        {
            mMainView = mainView;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Lbl_ByControl.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0020", LanguageService.LABEL_BREAKTIME);
            this.TextBlock_Start_Time.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0022", LanguageService.LABEL_BREAKTIME);
            this.TextBlock_End_Time.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0024", LanguageService.LABEL_BREAKTIME);
            this.btn_Complete_Job.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0025", LanguageService.LABEL_BREAKTIME);
            this.btn_Release_Job.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0026", LanguageService.LABEL_BREAKTIME);
            this.btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0027", LanguageService.LABEL_BREAKTIME);
        }
        public void CheckButton(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if ((jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "DS") && jobOrder.workingMchn.mchnSts == "U")
            {
                btn_Release_Job.IsEnabled = true;
            }
            else          
                btn_Release_Job.IsEnabled = false;
            
            if (jobOrder.type.jobTp.Equals("LD") && jobOrder.pinChkFlg.Equals("Y"))
            {
                btn_Complete_Job.IsEnabled = true;
            }
            else
                btn_Complete_Job.IsEnabled = false;
        }
        public void DisableButton()
        {
            btn_Release_Job.IsEnabled = false;
            btn_Complete_Job.IsEnabled = false;
        }
        public void Show_Popup()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide_Popup()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            mMainView.Complete_Break_Event();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            mMainView.Cancel_Break_Event();
        }

        private void btn_Complete_Job_Click(object sender, RoutedEventArgs e)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> tasks = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count >= 1)
                tasks.Add((VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0]);
            if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count >= 2)
                tasks.Add((VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[1]);
            if (tasks.Count > 0)
            {
                if (tasks[0].type.jobTp == "LD")
                    VMT_Data_JAT2.VMT_DataMgr_ITV.SetQCJobReleaseByYt_Ask(tasks);

                else if (tasks[0].type.jobTp == "DS" || tasks[0].type.jobTp == "MI" || tasks[0].type.jobTp == "LC")
                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder task in tasks)
                    {
                        VMT_Data_JAT2.VMT_DataMgr_ITV.SetJobDone_Ask(task);
                    }
                else
                    mMainView.Complete_Job_Event();
            }         
        }

        private void btn_Release_Job_Click(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.ShowProgressBar(0);
            this.btn_Release_Job.IsEnabled = false;
            mMainView.Replease_Job_Event();
        }
    }
}
