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
    /// Interaction logic for ListJobEmptyContainerView.xaml
    /// </summary>
    public partial class ListJobEmptyContainerView : UserControl
    {
        int page = 1;
        int length = 0;
        public int currentRow = 0;
        public int currentTier = 0;
        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> listContainer = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder selectedContainer = null;
        private SolidColorBrush colorSelected = Brushes.YellowGreen;
        public ListJobEmptyContainerView()
        {
            this.InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitEvent();
            LoadLanguage();
            InitSkinImage();
            //TestData();
            //SetGridData();
            //CheckLayoutPrevNextBtn();

        }
        //private void TestData()
        //{
        //    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder container = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
        //    container.cntr.cntrNo = "111111";
        //    container.type.jobTp = "JobTp";
        //    container.jobTpKor = "JobTpKor";
        //    for (int i = 1; i <= 16; i++)
        //    {
        //        listContainer.Add(container);
        //    }
        //}
        private void InitEvent()
        {
            this.Btn_Prev.Click += new RoutedEventHandler(Btn_Prev_Click);
            this.Btn_Next.Click += new RoutedEventHandler(Btn_Next_Click);
            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);

            this.Grid_1.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail1_MouseLeftButtonDown);
            this.Grid_2.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail2_MouseLeftButtonDown);
            this.Grid_3.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail3_MouseLeftButtonDown);
            this.Grid_4.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail4_MouseLeftButtonDown);
            this.Grid_5.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail5_MouseLeftButtonDown);
            this.Grid_6.MouseLeftButtonUp += new MouseButtonEventHandler(Grid_Detail6_MouseLeftButtonDown);
            this.IsVisibleChanged += ListJobPopupView_IsVisibleChanged;
        }

        private void ListJobPopupView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (this.IsVisible == false)
            //        PresentationMgr.AppWin.MainWin.wrkCd = "";
        }

        private void LoadLanguage()
        {
            this.Lbl_Title.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0049", LanguageService.MESSAGE_GROUP);
        }

        private void SetContainerGrid(int i)
        {
            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder cntr = listContainer[6 * page - 7 + i];
            switch (i)
            {
                case 1:
                    Lb_ContainerNo1.Content = cntr.cntr.cntrNo;
                    Lb_JobType1.Content = lang.Contains("Korea") ? cntr.jobTpKor : cntr.type.jobTp;
                    Lb_YtNo1.Content = cntr.locFrom.location;
                    break;
                case 2:
                    Lb_ContainerNo2.Content = cntr.cntr.cntrNo;
                    Lb_JobType2.Content = lang.Contains("Korea") ? cntr.jobTpKor : cntr.type.jobTp;
                    Lb_YtNo2.Content = cntr.locFrom.location;
                    break;
                case 3:
                    Lb_ContainerNo3.Content = cntr.cntr.cntrNo;
                    Lb_JobType3.Content = lang.Contains("Korea") ? cntr.jobTpKor : cntr.type.jobTp;
                    Lb_YtNo3.Content = cntr.locFrom.location;
                    break;
                case 4:
                    Lb_ContainerNo4.Content = cntr.cntr.cntrNo;
                    Lb_JobType4.Content = lang.Contains("Korea") ? cntr.jobTpKor : cntr.type.jobTp;
                    Lb_YtNo4.Content = cntr.locFrom.location;
                    break;
                case 5:
                    Lb_ContainerNo5.Content = cntr.cntr.cntrNo;
                    Lb_JobType5.Content = lang.Contains("Korea") ? cntr.jobTpKor : cntr.type.jobTp;
                    Lb_YtNo5.Content = cntr.locFrom.location;
                    break;
                case 6:
                    Lb_ContainerNo6.Content = cntr.cntr.cntrNo;
                    Lb_JobType6.Content = lang.Contains("Korea") ? cntr.jobTpKor : cntr.type.jobTp;
                    Lb_YtNo6.Content = cntr.locFrom.location;
                    break;
            }
        }
        public void SetGridData()
        {
            ClearGridData();
            ResetSelectedContainer();
            length = listContainer.Count;
            CheckLayoutPrevNextBtn();
            for (int i = 1; i <= 6; i++)
            {
                if (6 * page + i - 6 >= 0 && 6 * page + i - 6 <= length)
                {
                    SetContainerGrid(i);
                }
            }
        }
        public void ClearContainerBundleGrid(int i)
        {
            switch (i)
            {
                case 1:
                    Lb_ContainerNo1.Content = String.Empty;
                    Lb_JobType1.Content = String.Empty;
                    Lb_YtNo1.Content = String.Empty;
                    break;
                case 2:
                    Lb_ContainerNo2.Content = String.Empty;
                    Lb_JobType2.Content = String.Empty;
                    Lb_YtNo2.Content = String.Empty;
                    break;
                case 3:
                    Lb_ContainerNo3.Content = String.Empty;
                    Lb_JobType3.Content = String.Empty;
                    Lb_YtNo3.Content = String.Empty;
                    break;
                case 4:
                    Lb_ContainerNo4.Content = String.Empty;
                    Lb_JobType4.Content = String.Empty;
                    Lb_YtNo4.Content = String.Empty;
                    break;
                case 5:
                    Lb_ContainerNo5.Content = String.Empty;
                    Lb_JobType5.Content = String.Empty;
                    Lb_YtNo5.Content = String.Empty;
                    break;
                case 6:
                    Lb_ContainerNo6.Content = String.Empty;
                    Lb_JobType6.Content = String.Empty;
                    Lb_YtNo6.Content = String.Empty;
                    break;
            }
        }
        private void ClearGridData()
        {
            for (int i = 1; i <= 6; i++)
            {
                ClearContainerBundleGrid(i);
            }
        }
        private void CheckLayoutPrevNextBtn()
        {
            Tbl_Page.Text = page.ToString() + "/" + (length / 6 + ((length % 6 > 0) ? 1 : 0));
            if (page <= 1)
            {
                Btn_Next.IsEnabled = false;
                if (page <= length / 6 + ((length % 6 > 0) ? 0 : -1))
                {
                    Btn_Prev.IsEnabled = true;
                }
                else Btn_Prev.IsEnabled = false;
            }
            else if (page > length / 6 + ((length % 6 > 0) ? 0 : -1))
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
            selectedContainer = null;
            if (6 * page - 6 < length)
            {
                Grid_1.Background = colorSelected;
                selectedContainer = listContainer[6 * page - 6];
                setAutoClickContainer(selectedContainer.cntr.cntrNo);
            }
        }
        private void Grid_Detail2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedContainer = null;
            if (6 * page - 5 < length)
            {
                Grid_2.Background = colorSelected;
                selectedContainer = listContainer[6 * page - 5];
                setAutoClickContainer(selectedContainer.cntr.cntrNo);
            }
        }
        private void Grid_Detail3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedContainer = null;
            if (6 * page - 4 < length)
            {
                Grid_3.Background = colorSelected;
                selectedContainer = listContainer[6 * page - 4];
                setAutoClickContainer(selectedContainer.cntr.cntrNo);
            }
        }
        private void Grid_Detail4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedContainer = null;
            if (6 * page - 3 < length)
            {
                Grid_4.Background = colorSelected;
                selectedContainer = listContainer[6 * page - 3];
                setAutoClickContainer(selectedContainer.cntr.cntrNo);
            }
        }
        private void Grid_Detail5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedContainer = null;
            if (6 * page - 2 < length)
            {
                Grid_5.Background = colorSelected;
                selectedContainer = listContainer[6 * page - 2];
                setAutoClickContainer(selectedContainer.cntr.cntrNo);
            }
        }
        private void Grid_Detail6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedContainer = null;
            if (6 * page - 1 < length)
            {
                Grid_6.Background = colorSelected;
                selectedContainer = listContainer[6 * page - 1];
                setAutoClickContainer(selectedContainer.cntr.cntrNo);
            }
        }
        private void setAutoClickContainer(String cntrNo)
        {
            PresentationMgr.MainView.plcdomain.cntrNo = cntrNo;
            if(!String.IsNullOrEmpty(selectedContainer.type.jobTp))
                PresentationMgr.MainView.plcdomain.jbFlg = "Y";
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
