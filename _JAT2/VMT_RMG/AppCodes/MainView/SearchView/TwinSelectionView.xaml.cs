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

//20190108
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// TwinSelectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TwinSelectionView : UserControl
    {
        private String _selectKey = String.Empty;

        public TwinSelectionView()
        {
            InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {            
            InitSkinImage();
                        
            this.Btn_Done.Click += new RoutedEventHandler(Btn_Done_Click);
            this.Btn_Cancel.Click += new RoutedEventHandler(Btn_Cancel_Click);


            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(ContainerSearchView_IsVisibleChanged);

            this.ForeJobControl.MouseLeftButtonUp += new MouseButtonEventHandler(ForeJobControl_MouseLeftButtonUp);
            this.AfterJobControl.MouseLeftButtonUp += new MouseButtonEventHandler(AfterJobControl_MouseLeftButtonUp);

            tbContainer.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0065", LanguageService.LABEL_CONTAINERDETAIL);
            tbISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0041", LanguageService.LABEL_CONTAINERDETAIL);
            tbTruckNo.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0066", LanguageService.LABEL_CONTAINERDETAIL);
            tbJob.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0109", LanguageService.LABEL_CUSTOMIZE);
            tbLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0049", LanguageService.LABEL_CONTAINERDETAIL);
            tbPlanLoc.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0050", LanguageService.LABEL_CONTAINERDETAIL);
            tbSelectTarget.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0108", LanguageService.LABEL_CUSTOMIZE) + " ";
            Btn_Done.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0110", LanguageService.LABEL_CUSTOMIZE);
            Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0065", LanguageService.LABEL_MAINWINDOW);
        }

        private void InitSkinImage()
        {
            this.OnPropertyChanged_UITheme(null, null);
        }

        private void OnPropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Day.SelectionView_ButtonDefaultImage, UIThemeMgr.Day.SelectionView_ButtonPressImage, UIThemeMgr.Day.SelectionView_ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Done,
                    UIThemeMgr.Day.SelectionView_ButtonDefaultImage, UIThemeMgr.Day.SelectionView_ButtonPressImage, UIThemeMgr.Day.SelectionView_ButtonDefaultImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Night.SelectionView_ButtonDefaultImage, UIThemeMgr.Night.SelectionView_ButtonPressImage, UIThemeMgr.Night.SelectionView_ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Done,
                    UIThemeMgr.Night.SelectionView_ButtonDefaultImage, UIThemeMgr.Night.SelectionView_ButtonPressImage, UIThemeMgr.Night.SelectionView_ButtonDefaultImage);
            }
        }

        private void ContainerSearchView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            //this.Btn_Save.IsEnabled = false;
        }

        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            //this.Btn_Save.IsEnabled = false;
            var job = PresentationMgr.Singleton.JOB_Get(this._selectKey);
            if (job != null && job.type != null)
            {
                var otherJob = PresentationMgr.Singleton.JOB_Get(this._selectKey == this.ForeJobControl.JobKey ?
                    this.AfterJobControl.JobKey : this.ForeJobControl.JobKey);
                if (otherJob != null && otherJob.type != null && otherJob.type.jobStatus == "P")                
                    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobReOnChassis_Ask(otherJob.jobKey);                    
                
                if (job.type.jobStatus != "P")
                    PresentationMgr.Singleton.SendPickedContainerAsk(job, null);
            }

            this._selectKey = string.Empty;
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this._selectKey = string.Empty;
            //this.Btn_Save.IsEnabled = false;
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.MainView);
        }      

        public void SetTwinJobOrder(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder fore, VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder after, Boolean selectFore)
        {
            this.ForeJobControl.Selected = selectFore;
            this.AfterJobControl.Selected = !selectFore;
            this.Btn_Done.IsEnabled = true;            

            this.ForeJobControl.SetJobInfo(fore);
            this.AfterJobControl.SetJobInfo(after);

            if (selectFore)
                this._selectKey = this.ForeJobControl.JobKey;
            else
                this._selectKey = this.AfterJobControl.JobKey;
        }

        private void ForeJobControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ForeJobControl.Selected = true;
            this.AfterJobControl.Selected = false;

            this._selectKey = this.ForeJobControl.JobKey;

            this.Btn_Done.IsEnabled = true;
        }

        private void AfterJobControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ForeJobControl.Selected = false;
            this.AfterJobControl.Selected = true;

            this._selectKey = this.AfterJobControl.JobKey;

            this.Btn_Done.IsEnabled = true;
        }      
    }
}
