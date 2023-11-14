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
using VMT_Data_JAT2.Objects;
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for VBlockView.xaml
    /// </summary>
    public partial class VBlockView : UserControl
    {
        public List<string> lVirtualBlck = new List<string>();
        int page = 1;
        public Boolean general = false;
        private SolidColorBrush colorSelected = Brushes.YellowGreen;

        public VBlockView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadLanguage
            LoadLanguage();
            //Load Data
            LoadData();
            //Init Event Handler
            InitEvent();
        }
        private void InitEvent()
        {
            this.Btn_BlockDown.Click += new RoutedEventHandler(Btn_BlockDown_Click);
            this.Btn_BlockUp.Click += new RoutedEventHandler(Btn_BlockUp_Click);
            this.Btn_BlockDown2.Click += new RoutedEventHandler(Btn_BlockDown2_Click);
            this.Btn_BlockUp2.Click += new RoutedEventHandler(Btn_BlockUp2_Click);
            this.Btn_Prev.Click += new RoutedEventHandler(Btn_Prev_Click);
            this.Btn_Next.Click += new RoutedEventHandler(Btn_Next_Click);
            this.Btn_Close.Click += new RoutedEventHandler(Btn_Close_Click);
            this.Btn_Virtual.Click += new RoutedEventHandler(Btn_Virtual_MouseLeftButtonUp);
            this.Btn_General.Click += new RoutedEventHandler(Btn_General_MouseLeftButtonUp);

            this.Grid_Detail1.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Detail1_MouseLeftButtonDown);
            this.Grid_Detail2.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Detail2_MouseLeftButtonDown);
            this.Grid_Detail3.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Detail3_MouseLeftButtonDown);
            this.Grid_Detail4.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Detail4_MouseLeftButtonDown);
            this.Grid_Detail5.MouseLeftButtonDown += new MouseButtonEventHandler(Grid_Detail5_MouseLeftButtonDown);
        }
        private void LoadLanguage()
        {
            this.Btn_Container.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0005", LanguageService.LABEL_VIRTUALBLOCK);
            this.Lb_Search.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0006", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Prev.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0007", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Next.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0008", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_Container.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0009", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_Class.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0010", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_ISO.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0011", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_FM.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0012", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_OPR.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0013", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_Block.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0014", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_Hold.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0015", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_DMG.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0016", LanguageService.LABEL_VIRTUALBLOCK);
            this.Tbl_Grade.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0017", LanguageService.LABEL_VIRTUALBLOCK);
            this.Lb_Location.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0018", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Virtual.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0019", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_General.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0020", LanguageService.LABEL_VIRTUALBLOCK);
            this.Btn_Close.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0021", LanguageService.LABEL_VIRTUALBLOCK);
        }
        #region [InitData]
        private void LoadData()
        {
            InitSkinImage();
        }
        public class ContainerDetail
        {
            public String cntrNo;
            public String cls;
            public String cntrIso;
            public String fullMty;
            public String opr;
            public String fmBlck;
            public String toBlck;
            public String isHold;
            public String isDmg;
            public String cntrGrade;
            public ContainerDetail()
            {
                cntrNo = String.Empty;
                cls = String.Empty;
                cntrIso = String.Empty;
                fullMty = String.Empty;
                opr = String.Empty;
                fmBlck = String.Empty;
                toBlck = String.Empty;
                isHold = String.Empty;
                isDmg = String.Empty;
                cntrGrade = String.Empty;
            }
        }
        private List<ContainerDetail> listContainer = new List<ContainerDetail>();
        public ContainerDetail containerSelected = null;

        private void SetContainerDetailGrid(int i)
        {
            ContainerDetail cntr = listContainer[5 * page - 6 + i];
            switch (i)
            {
                case 1:
                    Tbl_DetailContainer1.Text = cntr.cntrNo;
                    Tbl_DetailClass1.Text = cntr.cls;
                    Tbl_DetailISO1.Text = cntr.cntrIso;
                    Tbl_DetailFM1.Text = cntr.fullMty;
                    Tbl_DetailOPR1.Text = cntr.opr;
                    Tbl_DetailBlock1.Text = cntr.fmBlck;
                    Tbl_DetailHold1.Text = cntr.isHold;
                    Tbl_DetailDMG1.Text = cntr.isDmg;
                    Tbl_DetailGrade1.Text = cntr.cntrGrade;
                    if (containerSelected != null && containerSelected.cntrNo == cntr.cntrNo)
                        Grid_Detail1.Background = colorSelected;
                    break;
                case 2:
                    Tbl_DetailContainer2.Text = cntr.cntrNo;
                    Tbl_DetailClass2.Text = cntr.cls;
                    Tbl_DetailISO2.Text = cntr.cntrIso;
                    Tbl_DetailFM2.Text = cntr.fullMty;
                    Tbl_DetailOPR2.Text = cntr.opr;
                    Tbl_DetailBlock2.Text = cntr.fmBlck;
                    Tbl_DetailHold2.Text = cntr.isHold;
                    Tbl_DetailDMG2.Text = cntr.isDmg;
                    Tbl_DetailGrade2.Text = cntr.cntrGrade;
                    if (containerSelected != null && containerSelected.cntrNo == cntr.cntrNo)
                        Grid_Detail2.Background = colorSelected;
                    break;
                case 3:
                    Tbl_DetailContainer3.Text = cntr.cntrNo;
                    Tbl_DetailClass3.Text = cntr.cls;
                    Tbl_DetailISO3.Text = cntr.cntrIso;
                    Tbl_DetailFM3.Text = cntr.fullMty;
                    Tbl_DetailOPR3.Text = cntr.opr;
                    Tbl_DetailBlock3.Text = cntr.fmBlck;
                    Tbl_DetailHold3.Text = cntr.isHold;
                    Tbl_DetailDMG3.Text = cntr.isDmg;
                    Tbl_DetailGrade3.Text = cntr.cntrGrade;
                    if (containerSelected != null && containerSelected.cntrNo == cntr.cntrNo)
                        Grid_Detail3.Background = colorSelected;
                    break;
                case 4:
                    Tbl_DetailContainer4.Text = cntr.cntrNo;
                    Tbl_DetailClass4.Text = cntr.cls;
                    Tbl_DetailISO4.Text = cntr.cntrIso;
                    Tbl_DetailFM4.Text = cntr.fullMty;
                    Tbl_DetailOPR4.Text = cntr.opr;
                    Tbl_DetailBlock4.Text = cntr.fmBlck;
                    Tbl_DetailHold4.Text = cntr.isHold;
                    Tbl_DetailDMG4.Text = cntr.isDmg;
                    Tbl_DetailGrade4.Text = cntr.cntrGrade;
                    if (containerSelected != null && containerSelected.cntrNo == cntr.cntrNo)
                        Grid_Detail4.Background = colorSelected;
                    break;
                case 5:
                    Tbl_DetailContainer5.Text = cntr.cntrNo;
                    Tbl_DetailClass5.Text = cntr.cls;
                    Tbl_DetailISO5.Text = cntr.cntrIso;
                    Tbl_DetailFM5.Text = cntr.fullMty;
                    Tbl_DetailOPR5.Text = cntr.opr;
                    Tbl_DetailBlock5.Text = cntr.fmBlck;
                    Tbl_DetailHold5.Text = cntr.isHold;
                    Tbl_DetailDMG5.Text = cntr.isDmg;
                    Tbl_DetailGrade5.Text = cntr.cntrGrade;
                    if (containerSelected != null && containerSelected.cntrNo == cntr.cntrNo)
                        Grid_Detail5.Background = colorSelected;
                    break;
            }
        }

        private void SetGridData()
        {
            ClearGridData();
            for (int i = 1; i <= 5; i++)
            {
                if (5 * page + i - 5 >= 0 && 5 * page + i - 5 <= listContainer.Count)
                {
                    SetContainerDetailGrid(i);
                }
            }
        }
        private void ClearContainerDetailGrid(int i)
        {
            switch (i)
            {
                case 1:
                    Tbl_DetailContainer1.Text = String.Empty;
                    Tbl_DetailClass1.Text = String.Empty;
                    Tbl_DetailISO1.Text = String.Empty;
                    Tbl_DetailFM1.Text = String.Empty;
                    Tbl_DetailOPR1.Text = String.Empty;
                    Tbl_DetailBlock1.Text = String.Empty;
                    Tbl_DetailHold1.Text = String.Empty;
                    Tbl_DetailDMG1.Text = String.Empty;
                    Tbl_DetailGrade1.Text = String.Empty;
                    break;
                case 2:
                    Tbl_DetailContainer2.Text = String.Empty;
                    Tbl_DetailClass2.Text = String.Empty;
                    Tbl_DetailISO2.Text = String.Empty;
                    Tbl_DetailFM2.Text = String.Empty;
                    Tbl_DetailOPR2.Text = String.Empty;
                    Tbl_DetailBlock2.Text = String.Empty;
                    Tbl_DetailHold2.Text = String.Empty;
                    Tbl_DetailDMG2.Text = String.Empty;
                    Tbl_DetailGrade2.Text = String.Empty;
                    break;
                case 3:
                    Tbl_DetailContainer3.Text = String.Empty;
                    Tbl_DetailClass3.Text = String.Empty;
                    Tbl_DetailISO3.Text = String.Empty;
                    Tbl_DetailFM3.Text = String.Empty;
                    Tbl_DetailOPR3.Text = String.Empty;
                    Tbl_DetailBlock3.Text = String.Empty;
                    Tbl_DetailHold3.Text = String.Empty;
                    Tbl_DetailDMG3.Text = String.Empty;
                    Tbl_DetailGrade3.Text = String.Empty;
                    break;
                case 4:
                    Tbl_DetailContainer4.Text = String.Empty;
                    Tbl_DetailClass4.Text = String.Empty;
                    Tbl_DetailISO4.Text = String.Empty;
                    Tbl_DetailFM4.Text = String.Empty;
                    Tbl_DetailOPR4.Text = String.Empty;
                    Tbl_DetailBlock4.Text = String.Empty;
                    Tbl_DetailHold4.Text = String.Empty;
                    Tbl_DetailDMG4.Text = String.Empty;
                    Tbl_DetailGrade4.Text = String.Empty;
                    break;
                case 5:
                    Tbl_DetailContainer5.Text = String.Empty;
                    Tbl_DetailClass5.Text = String.Empty;
                    Tbl_DetailISO5.Text = String.Empty;
                    Tbl_DetailFM5.Text = String.Empty;
                    Tbl_DetailOPR5.Text = String.Empty;
                    Tbl_DetailBlock5.Text = String.Empty;
                    Tbl_DetailHold5.Text = String.Empty;
                    Tbl_DetailDMG5.Text = String.Empty;
                    Tbl_DetailGrade5.Text = String.Empty;
                    break;
            }
        }

        private void ClearGridData()
        {
            ResetSelectedContainer();
            for (int i = 1; i <= 5; i++)
            {
                ClearContainerDetailGrid(i);
            }
        }

        
        #endregion [InitData]

        
        private void CheckLayoutUpDownBtn()
        {
            if (lVirtualBlck.Count == 0)
            {
                Btn_BlockDown.IsEnabled = false;
                Btn_BlockUp.IsEnabled = false;

                Btn_BlockDown2.IsEnabled = false;
                Btn_BlockUp2.IsEnabled = false;
            }
            else
            {
                if (lVirtualBlck.IndexOf(Btn_BlockText.Content.ToString()) == 0)
                {
                    Btn_BlockUp.IsEnabled = false;
                    Btn_BlockDown.IsEnabled = true;
                }
                else if (lVirtualBlck.IndexOf(Btn_BlockText.Content.ToString()) == lVirtualBlck.Count - 1)
                {
                    Btn_BlockUp.IsEnabled = true;
                    Btn_BlockDown.IsEnabled = false;
                }
                else
                {
                    Btn_BlockUp.IsEnabled = true;
                    Btn_BlockDown.IsEnabled = true;
                }

                if (lVirtualBlck.IndexOf(Btn_BlockText2.Content.ToString()) == 0)
                {
                    Btn_BlockUp2.IsEnabled = false;
                    Btn_BlockDown2.IsEnabled = true;
                }
                else if (lVirtualBlck.IndexOf(Btn_BlockText2.Content.ToString()) == lVirtualBlck.Count - 1)
                {
                    Btn_BlockUp2.IsEnabled = true;
                    Btn_BlockDown2.IsEnabled = false;
                }
                else
                {
                    Btn_BlockUp2.IsEnabled = true;
                    Btn_BlockDown2.IsEnabled = true;
                }
            }           
        }
        private void CheckLayoutPrevNextBtn()
        {
            Tbl_Page.Text = page.ToString() + "/" + (listContainer.Count / 5 + ((listContainer.Count % 5 > 0) ? 1 : 0));
            if (page <= 1)
            {
                Btn_Prev.IsEnabled = false;
                if (page <= listContainer.Count / 5 + ((listContainer.Count % 5 > 0) ? 0 : -1))
                {
                    Btn_Next.IsEnabled = true;
                }
                else Btn_Next.IsEnabled = false;
            }
            else if (page > listContainer.Count / 5 + ((listContainer.Count % 5 > 0) ? 0 : -1))
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
        private void CheckLayoutVirtualBtn()
        {
            if (containerSelected == null)
            {
                Btn_Virtual.IsEnabled = false;
                Btn_General.IsEnabled = false;
            }
            else
            {
                Btn_Virtual.IsEnabled = true;
                Btn_General.IsEnabled = true;
            }
        }
        public void AddBlockBayVrtlInfoCallBack(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            lVirtualBlck.Clear();
            if (value != null)
            {
                foreach (var info in value.DicBlock)
                {
                    if (info.Value.isBolBlck == false)
                    {
                        lVirtualBlck.Add(info.Value.BlcName);
                    }
                }
                if (lVirtualBlck.Count > 0)
                {
                    Btn_BlockText.Content = lVirtualBlck[0];
                    Btn_BlockText2.Content = lVirtualBlck[0];
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListByVirtualBlock_New(this.Btn_BlockText.Content.ToString());
                }
            }
            CheckLayoutUpDownBtn();
        }
        
        void Btn_BlockUp_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName) || lVirtualBlck.Count == 0 || blockName == lVirtualBlck[0])
            {
                return;
            }
            this.Btn_BlockText.Content = lVirtualBlck[lVirtualBlck.IndexOf(blockName) - 1];
            CheckLayoutUpDownBtn();
            ClearGridData();
            VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListByVirtualBlock_New(this.Btn_BlockText.Content.ToString());
        }
        void Btn_BlockDown_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName) || lVirtualBlck.Count == 0 || blockName == lVirtualBlck[lVirtualBlck.Count - 1])
                return;

            this.Btn_BlockText.Content = lVirtualBlck[lVirtualBlck.IndexOf(blockName) + 1];
            CheckLayoutUpDownBtn();
            ClearGridData();
            VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListByVirtualBlock_New(this.Btn_BlockText.Content.ToString());
        }
        void Btn_BlockUp2_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText2.Content.ToString();
            if (String.IsNullOrEmpty(blockName) || lVirtualBlck.Count == 0 || blockName == lVirtualBlck[0])
            {
                return;
            }
            string newBlck = lVirtualBlck[lVirtualBlck.IndexOf(blockName) - 1];
            this.Btn_BlockText2.Content = newBlck;
            if (containerSelected != null)
                containerSelected.toBlck = newBlck;
            CheckLayoutUpDownBtn();
        }
        void Btn_BlockDown2_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText2.Content.ToString();
            if (String.IsNullOrEmpty(blockName) || lVirtualBlck.Count == 0 || blockName == lVirtualBlck[lVirtualBlck.Count - 1])
                return;

            string newBlck = lVirtualBlck[lVirtualBlck.IndexOf(blockName) + 1];
            this.Btn_BlockText2.Content = newBlck;
            if (containerSelected != null)
                containerSelected.toBlck = newBlck;
            CheckLayoutUpDownBtn();
        }

        public void GetInventoryListByVirtualBlock_New_CallBack(RMG.VD_RMG_InventoryInfo_Receive inventory)
        {
            if (inventory != null)
            {   
                foreach (var blockInfo in inventory.DicBlock)
                {
                    if (blockInfo.Key.ToString().Equals(Btn_BlockText.Content))
                    {
                        foreach (var bayInfo in blockInfo.Value.DicBay)
                        {
                            listContainer.Clear();
                            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory cntr in bayInfo.Value.invenList)
	                        {
                                ContainerDetail container = new ContainerDetail();
                                container.cntrNo = cntr.cntr.cntrNo;
                                container.cls = cntr.cntr.cls;
                                container.cntrIso = cntr.cntr.cntrIso;
                                container.fullMty = cntr.cntr.fullMty;
                                container.opr = cntr.cntr.opr;
                                container.isHold = (cntr.cntr.isHold) ? "Y" : "";
                                container.isDmg = (cntr.cntr.isDmg) ? "Y" : "";
                                container.cntrGrade = cntr.cntr.cntrGrade;
                                container.fmBlck = Btn_BlockText.Content.ToString();
                                container.toBlck = Btn_BlockText2.Content.ToString();
                                listContainer.Add(container);
	                        } 
                        }

                        CheckLayoutPrevNextBtn();
                        CheckLayoutVirtualBtn();
                        SetGridData();
                    }
                }

            }               
        }
        
        private void Btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            ResetSelectedContainer();
            page = page - 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }
        void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            ResetSelectedContainer();
            page = page + 1;
            SetGridData();
            CheckLayoutPrevNextBtn();
        }
        private void ResetSelectedContainer()
        {
            Grid_Detail1.Background = Brushes.White;
            Grid_Detail2.Background = Brushes.White;
            Grid_Detail3.Background = Brushes.White;
            Grid_Detail4.Background = Brushes.White;
            Grid_Detail5.Background = Brushes.White;
            CheckLayoutVirtualBtn();
        }
        private void Grid_Detail1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            containerSelected = null;
            if (Grid_Detail1.Background == colorSelected)
            {
                ResetSelectedContainer();
            }
            else
            {
                ResetSelectedContainer();
                if (5 * page - 5 < listContainer.Count)
                {
                    Grid_Detail1.Background = colorSelected;
                    containerSelected = listContainer[5 * page - 5];
                    CheckLayoutVirtualBtn();
                }
            }
        }
        private void Grid_Detail2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            containerSelected = null;
            if (Grid_Detail2.Background == colorSelected)
            {
                ResetSelectedContainer();
            }
            else
            {
                ResetSelectedContainer();
                if (5 * page - 4 < listContainer.Count)
                {
                    Grid_Detail2.Background = colorSelected;
                    containerSelected = listContainer[5 * page - 4];
                    CheckLayoutVirtualBtn();
                }               
            }
        }
        private void Grid_Detail3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            containerSelected = null;
            if (Grid_Detail3.Background == colorSelected)
            {
                ResetSelectedContainer();
            }
            else
            {
                ResetSelectedContainer();
                if (5 * page - 3 < listContainer.Count)
                {
                    Grid_Detail3.Background = colorSelected;
                    containerSelected = listContainer[5 * page - 3];
                    CheckLayoutVirtualBtn();
                }               
            }
        }
        private void Grid_Detail4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            containerSelected = null;
            if (Grid_Detail4.Background == colorSelected)
            {
                ResetSelectedContainer();
            }
            else
            {
                ResetSelectedContainer();
                if (5 * page - 2 < listContainer.Count)
                {
                    Grid_Detail4.Background = colorSelected;
                    containerSelected = listContainer[5 * page - 2];
                    CheckLayoutVirtualBtn();
                }             
            }
        }
        private void Grid_Detail5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            containerSelected = null;
            if (Grid_Detail5.Background == colorSelected)
            {
                ResetSelectedContainer();
            }
            else
            {
                ResetSelectedContainer();
                if (5 * page - 1 < listContainer.Count)
                {
                    Grid_Detail5.Background = colorSelected;
                    containerSelected = listContainer[5 * page - 1];
                    CheckLayoutVirtualBtn();
                }             
            }
        }
        
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
        
        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockDown2,
                   UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText2,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp2,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Prev,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Next,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Virtual,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_General,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockDown2,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_BlockText2,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp2,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);

                PresentationMgr.SetSkinButton(this.Btn_Prev,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Next,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Virtual,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_General,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);

                PresentationMgr.SetSkinButton(this.Btn_Close,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
            }
        }
        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void Btn_Virtual_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var target = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
            target.cntr.cntrNo = containerSelected.cntrNo;
            target.cntr.cntrIso = containerSelected.cntrIso;
            target.type.jobTp = "RH";
            target.type.ycTwinKey = "";
            target.locWorking.blck = containerSelected.toBlck;
            target.locWorking.bay = "";
            target.locWorking.row = "";
            target.locWorking.tier = "";
            target.workingMchn.mchnId = RMG.RMG_User.gMchnID;
            target.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;
            target.type.ycTwinKey = "";

            PresentationMgr.Singleton.SendJobDoneAsk(target);
            containerSelected = null;
            ResetSelectedContainer();
            VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListByVirtualBlock_New(this.Btn_BlockText.Content.ToString());
        }

        private void Btn_General_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            general = true;
            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = true;
            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = containerSelected.cntrNo;
            this.Visibility = Visibility.Hidden;
        }
    }
}
