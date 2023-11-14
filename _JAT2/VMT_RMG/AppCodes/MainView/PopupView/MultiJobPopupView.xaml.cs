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
using VMT_Data_JAT2;
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for MultiJobPopupView.xaml
    /// </summary>
    public partial class MultiJobPopupView : UserControl
    {
        int page = 1;
        int length = 0;
        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> listJob = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder selectedJob = null;
        private SolidColorBrush colorSelected = Brushes.YellowGreen;

        private String jobTpGrid1 = String.Empty;
        private String jobTpGrid2 = String.Empty;
        private String jobTpGrid3 = String.Empty;
        private String jobTpGrid4 = String.Empty;

        public MultiJobPopupView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitEvent();
            LoadLanguage();
            InitSkinImage();
        }
        private void InitEvent()
        {
            this.Btn_Prev.Click += new RoutedEventHandler(Btn_Prev_Click);
            this.Btn_Next.Click += new RoutedEventHandler(Btn_Next_Click);
            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);

            this.Grid_1.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail1_MouseLeftButtonDown);
            this.Grid_2.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail2_MouseLeftButtonDown);
            this.Grid_3.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail3_MouseLeftButtonDown);
            this.Grid_4.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail4_MouseLeftButtonDown);
        }
        private void LoadLanguage()
        {
            this.Lbl_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0059", LanguageService.MESSAGE_GROUP);
        }

        private void SetJobGrid(int i)
        {
            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job = listJob[4 * page - 5 + i];
            switch (i)
            {
                case 1:
                    Lb_ContainerNo1.Content = job.cntr.cntrNo;
                    Lb_JobType1.Content = lang.Contains("Korea") ? job.jobTpKor : job.type.jobTp;
                    jobTpGrid1 = job.type.jobTp;
                    break;
                case 2:
                    Lb_ContainerNo2.Content = job.cntr.cntrNo;
                    Lb_JobType2.Content = lang.Contains("Korea") ? job.jobTpKor : job.type.jobTp;
                    jobTpGrid2 = job.type.jobTp;
                    break;
                case 3:
                    Lb_ContainerNo3.Content = job.cntr.cntrNo;
                    Lb_JobType3.Content = lang.Contains("Korea") ? job.jobTpKor : job.type.jobTp;
                    jobTpGrid3 = job.type.jobTp;
                    break;
                case 4:
                    Lb_ContainerNo4.Content = job.cntr.cntrNo;
                    Lb_JobType4.Content = lang.Contains("Korea") ? job.jobTpKor : job.type.jobTp;
                    jobTpGrid4 = job.type.jobTp;
                    break;              
            }
        }
        public void SetGridData()
        {
            ClearGridData();
            ResetSelectedContainer();
            length = listJob.Count;
            CheckLayoutPrevNextBtn();
            for (int i = 1; i <= 4; i++)
            {
                if (4 * page + i - 4 >= 0 && 4 * page + i - 4 <= length)
                {
                    SetJobGrid(i);
                }
            }
        }
        public void ClearJobGrid(int i)
        {
            switch (i)
            {
                case 1:
                    Lb_ContainerNo1.Content = String.Empty;
                    Lb_JobType1.Content = String.Empty;
                    jobTpGrid1 = String.Empty;
                    break;
                case 2:
                    Lb_ContainerNo2.Content = String.Empty;
                    Lb_JobType2.Content = String.Empty;
                    jobTpGrid2 = String.Empty;
                    break;
                case 3:
                    Lb_ContainerNo3.Content = String.Empty;
                    Lb_JobType3.Content = String.Empty;
                    jobTpGrid3 = String.Empty;
                    break;
                case 4:
                    Lb_ContainerNo4.Content = String.Empty;
                    Lb_JobType4.Content = String.Empty;
                    jobTpGrid4 = String.Empty;
                    break;             
            }
        }
        private void ClearGridData()
        {
            for (int i = 1; i <= 4; i++)
            {
                ClearJobGrid(i);
            }
        }
        private void CheckLayoutPrevNextBtn()
        {
            Tbl_Page.Text = page.ToString() + "/" + (length / 4 + ((length % 4 > 0) ? 1 : 0));
            if (page <= 1)
            {
                Btn_Next.IsEnabled = false;
                if (page <= length / 4 + ((length % 4 > 0) ? 0 : -1))
                {
                    Btn_Prev.IsEnabled = true;
                }
                else Btn_Prev.IsEnabled = false;
            }
            else if (page > length / 4 + ((length % 4 > 0) ? 0 : -1))
            {
                Btn_Prev.IsEnabled = false;
                if (page > 1)
                {
                    Btn_Next.IsEnabled = true;
                }
                else Btn_Next.IsEnabled = false;
            }
            else
            {
                Btn_Prev.IsEnabled = true;
                Btn_Next.IsEnabled = true;
            }
        }
        private void Btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            page = page + 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }
        void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            page = page - 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }
        private void ResetSelectedContainer()
        {
            Grid_1.Background = Brushes.LightGray;
            Grid_2.Background = Brushes.LightGray;
            Grid_3.Background = Brushes.LightGray;
            Grid_4.Background = Brushes.LightGray;
        }
        private void Grid_Detail1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedJob = null;
            if (4 * page - 4 < length)
            {
                Grid_1.Background = colorSelected;
                selectedJob = listJob[4 * page - 4];
                setAutoClickJob(jobTpGrid1);
            }
        }
        private void Grid_Detail2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedJob = null;
            if (4 * page - 3 < length)
            {
                Grid_2.Background = colorSelected;
                selectedJob = listJob[4 * page - 3];
                setAutoClickJob(jobTpGrid2);
            }
        }
        private void Grid_Detail3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedJob = null;
            if (4 * page - 2 < length)
            {
                Grid_3.Background = colorSelected;
                selectedJob = listJob[4 * page - 2];
                setAutoClickJob(jobTpGrid3);
            }
        }
        private void Grid_Detail4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedJob = null;
            if (4 * page - 1 < length)
            {
                Grid_4.Background = colorSelected;
                selectedJob = listJob[4 * page - 1];
                setAutoClickJob(jobTpGrid4);
            }
        }      
        private void setAutoClickJob(String jobTp)
        {
            PresentationMgr.MainView.plcdomain.rsnDesc = jobTp;
            VMT_DataMgr_RMG.ProcessPLC_Ask(PresentationMgr.MainView.plcdomain);
            this.Visibility = Visibility.Hidden;
        }


        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.MainWin.wrkCd = "";
            VMT_DataMgr_RMG.CancelPLC_Ask(PresentationMgr.MainView.plcdomain);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Prev,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Next,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                   UIThemeMgr.Day.IndicatorView_ButtonBackDefaultImage, UIThemeMgr.Day.IndicatorView_ButtonBackPressImage, UIThemeMgr.Day.IndicatorView_ButtonBackDefaultImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Prev,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Next,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                   UIThemeMgr.Night.IndicatorView_ButtonBackDefaultImage, UIThemeMgr.Night.IndicatorView_ButtonBackPressImage, UIThemeMgr.Night.IndicatorView_ButtonBackDefaultImage);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }
    }
}
