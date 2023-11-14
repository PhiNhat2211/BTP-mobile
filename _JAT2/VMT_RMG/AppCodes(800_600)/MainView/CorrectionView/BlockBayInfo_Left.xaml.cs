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
using VMT_RMG;

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for BlockBayInfo_Left.xaml
    /// </summary>
    public partial class BlockBayInfo_Left : UserControl
    {
        public String BlockName
        {
            get
            {
                return this.Btn_BlockText.Content == null ? String.Empty : this.Btn_BlockText.Content.ToString();
            }

            set
            {
                this.Btn_BlockText.Content = value;
                this.Btn_BayText.IsEnabled = String.IsNullOrEmpty(value) ? false : true;
                this.CheckBlockLocation();
            }
        }

        public String BayName
        {
            get
            {
                return this.Btn_BayText.Content == null ? String.Empty : this.Btn_BayText.Content.ToString();
            }

            set
            {
                this.Btn_BayText.Content = value;
                this.CheckBayLocation();
            }
        }

        public BlockBayInfo_Left()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~BlockBayInfo_Left()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_BlockDown.Click += new RoutedEventHandler(Btn_BlockDown_Click);
            this.Btn_BlockText.Click += new RoutedEventHandler(Btn_BlockText_Click);
            this.Btn_BlockUp.Click += new RoutedEventHandler(Btn_BlockUp_Click);

            this.Btn_BayDown.Click += new RoutedEventHandler(Btn_BayDown_Click);
            this.Btn_BayText.Click += new RoutedEventHandler(Btn_BayText_Click);
            this.Btn_BayUp.Click += new RoutedEventHandler(Btn_BayUp_Click);

            //this.Btn_R_Block.Click += new RoutedEventHandler(Btn_R_Block_Click);
        }

        void CheckBlockLocation()
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockUp.IsEnabled = (i >= list.Count - 1) ? false : true;
                    this.Btn_BlockDown.IsEnabled = i <= 0 ? false : true;                    
                    break;
                }
            }
        }

        void CheckBayLocation()
        {
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = this.Btn_BayText.Content.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName))
                return;

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayUp.IsEnabled = (i >= list.Count - 1) ? false : true;
                    this.Btn_BayDown.IsEnabled = i <= 0 ? false : true;
                    break;
                }
            }
        }

        // Button Event
        void Btn_BlockUp_Click(object sender, RoutedEventArgs e)
        {   
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            Btn_BayText.IsEnabled = false;

            String blockNameNext = String.Empty;

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockUp.IsEnabled = (i >= list.Count - 1) ? false : true;
                    this.Btn_BlockDown.IsEnabled = true;
                    blockNameNext = list[i + 1].Value.BlcName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(blockNameNext) && blockNameNext != blockName)
            {
                this.Btn_BlockText.Content = blockNameNext;

                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                };
                pos.m_cBlock = blockNameNext;
                pos.m_cBay = String.Empty;
                PresentationMgr.Singleton.CurrentPostion = pos;                
                
                // call getBlockMapList
                PresentationMgr.AppWin.ShowProgressBar(0);
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockNameNext);
            }
        }

        void Btn_BlockText_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //String blockName = this.Btn_BlockText.Content.ToString();
            //if (String.IsNullOrEmpty(blockName))
            //    return;

            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.BlockSelectionView);
            PresentationMgr.MainView.UC_BlockSelectionView.SelectedBlockName = BlockName;
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBlockName = BlockName;
            PresentationMgr.MainView.UC_BlockSelectionView.MakeupBlockItemsWithTabSelection();
        }
        
        void Btn_BlockDown_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            Btn_BayText.IsEnabled = false;

            String blockNamePre = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();
            for (int i = 1; i < list.Count; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockDown.IsEnabled = i <= 1 ? false : true;
                    this.Btn_BlockUp.IsEnabled = true;
                    blockNamePre = list[i - 1].Value.BlcName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(blockNamePre) && blockNamePre != blockName)
            {
                this.Btn_BlockText.Content = blockNamePre;

                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                };
                pos.m_cBlock = blockNamePre;
                pos.m_cBay = String.Empty;
                PresentationMgr.Singleton.CurrentPostion = pos;                
                
                // call getBlockMapList
                PresentationMgr.AppWin.ShowProgressBar(0);
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockNamePre);
            }            
        }

        void Btn_BayUp_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = this.Btn_BayText.Content.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;
            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName))
                return;

            String bayNameNext = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            for (int i = 0; i < list.Count-1; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
               {
                    this.Btn_BayUp.IsEnabled = (i >= list.Count - 1) ? false : true;
                    this.Btn_BayDown.IsEnabled = true;
                    bayNameNext = list[i + 1].BayName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(bayNameNext) && bayNameNext != bayName)
            {
                this.Btn_BayText.Content = bayNameNext;

                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition(){
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                };
                pos.m_cBay = bayNameNext;
                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        void Btn_BayText_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;            

            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "GetBlockMapList_Ask"), blockName);
            
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockName);

            PresentationMgr.Singleton.UI_SwichUI(PresentationMgr.UIMode.BaySelectionView);
            
            PresentationMgr.AppWin.ShowProgressBar(0);
            //PresentationMgr.MainView.UC_BaySelectionView.MakeupBayItems();
        }

        void Btn_BayDown_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String bayName = this.Btn_BayText.Content.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;
            String blockName = this.Btn_BlockText.Content.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;            
            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName))
                return;

            String bayNamePre = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            for (int i = 1; i < list.Count; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayDown.IsEnabled = i <= 1 ? false : true;
                    this.Btn_BayUp.IsEnabled = true;
                    bayNamePre = list[i - 1].BayName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(bayNamePre) && bayNamePre != bayName)
            {
                this.Btn_BayText.Content = bayNamePre;

                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = PresentationMgr.Singleton.CurrentPostion.m_cBay,
                    m_cRow = PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = PresentationMgr.Singleton.CurrentPostion.m_cTier
                };
                pos.m_cBay = bayNamePre;
                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        void Btn_R_Block_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BayDown,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BayText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BayUp,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                    //);

                //PresentationMgr.SetSkinButton(this.Btn_R_Block,
                //    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BayDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BayText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_BayUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                    //);

                //PresentationMgr.SetSkinButton(this.Btn_R_Block,
                //    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //    //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);

            
        }
    }
}
