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
using static VMT_Data_JAT2.Objects.Common;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for BlockBayInfo.xaml
    /// </summary>
    public partial class BlockBayInfo : UserControl
    {
        public bool blockView = false;

        public int virtualBlockMode = 0; // 1: VBlockView - 2: VBlockChangeView - 0: Hidden
        public String BlockName
        {
            get
            {
                return this.Btn_BlockText.Text == null ? String.Empty : this.Btn_BlockText.Text.ToString();
            }

            set
            {
                this.Btn_BlockText.Text = value;
                if (this.Btn_BlockText.Text.Length >= 5)
                    this.Btn_BlockText.FontSize = 23;
                else
                    this.Btn_BlockText.FontSize = 27;
                this.Btn_BayText.IsEnabled = String.IsNullOrEmpty(value) || !PresentationMgr.Singleton.showViewINV ? false : true;
                if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value) &&
                    DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value].IsVirtual)
                    this.Btn_BayText.IsEnabled = false;
                this.CheckBlockLocation();
            }
        }

        public String BayName
        {
            get
            {
                return this.Btn_BayText.Text == null ? String.Empty : this.Btn_BayText.Text.ToString();
            }

            set
            {
                this.Btn_BayText.Text = value;
                this.CheckBayLocation();
            }
        }

        public BlockBayInfo()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~BlockBayInfo()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.Btn_BlockDown.Click += new RoutedEventHandler(Btn_BlockDown_Click);
            //this.Btn_BlockText.Click += new RoutedEventHandler(Btn_BlockText_Click);
            this.Btn_BlockUp.Click += new RoutedEventHandler(Btn_BlockUp_Click);
            this.Btn_BlockText.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_BlockText_PreviewMouseLeftButtonDown);
            this.Btn_BlockText.TextChanged += new TextChangedEventHandler(Btn_BlockText_TextChanged);
            this.Btn_BlockText.GotFocus += new RoutedEventHandler(Btn_BlockText_GotFocus);
            this.Btn_BlockText.LostFocus += new RoutedEventHandler(Btn_BlockText_LostFocus);

            this.Btn_BayDown.Click += new RoutedEventHandler(Btn_BayDown_Click);
            this.Btn_BayText.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Btn_BayText_PreviewMouseLeftButtonDown);
            //this.Btn_BayText.TextChanged += new TextChangedEventHandler(Btn_BayText_TextChanged);
            this.Btn_BayText.GotFocus += new RoutedEventHandler(Btn_BayText_GotFocus);
            this.Btn_BayText.LostFocus += new RoutedEventHandler(Btn_BayText_LostFocus);
            this.Btn_BayUp.Click += new RoutedEventHandler(Btn_BayUp_Click);

            this.Btn_V_Block.Click += new RoutedEventHandler(Btn_V_Block_Click);

            this.Btn_V_Block.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0069", LanguageService.LABEL_MAINWINDOW);
            PresentationMgr.AppWin.UC_KeypadView.DoneCallback += new Keypad.KeypadDoneCallback(this.KeypadDone);
        }

        public void CheckBlockLocation()
        {
            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
            {
                this.Btn_BlockUp.IsEnabled = false;
                this.Btn_BlockDown.IsEnabled = false;
                return;
            }

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ToList();

            this.Btn_BlockText.IsEnabled = PresentationMgr.Singleton.showViewINV ? true : false;

            this.Btn_BlockUp.IsEnabled = blockName.Equals(list.FindLast(o => !o.Value.IsVirtual).Key) || 
                                            blockName.Equals(list.FindLast(o => o.Value.IsVirtual).Key) ||
                                                !PresentationMgr.Singleton.showViewINV ? false : true;

            this.Btn_BlockDown.IsEnabled = blockName.Equals(list.Find(o => !o.Value.IsVirtual).Key) ||
                                            blockName.Equals(list.Find(o => o.Value.IsVirtual).Key) ||
                                                !PresentationMgr.Singleton.showViewINV ? false : true;
            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (0 == blockName.CompareTo(list[i].Value.BlcName))
            //    {
            //        this.Btn_BlockUp.IsEnabled = (i >= list.Count - 1) ? false : true;
            //        this.Btn_BlockDown.IsEnabled = i <= 0 ? false : true;
            //        break;
            //    }
            //}
        }

        public void CheckBayLocation()
        {
            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = this.Btn_BayText.Text.ToString();
            if (String.IsNullOrEmpty(bayName))
            {
                this.Btn_BayUp.IsEnabled = false;
                this.Btn_BayDown.IsEnabled = false;
                return;
            }

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 0; i < list.Count; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayUp.IsEnabled = (i >= list.Count - 1) || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayDown.IsEnabled = i <= 0 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    break;
                }
            }
        }

        // Button Event
        void Btn_BlockUp_Click(object sender, RoutedEventArgs e)
        {
            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            Btn_BayText.IsEnabled = false;

            String blockNameNext = String.Empty;

            //var dicBlock = new Dictionary<String, VD_Common_SimpleBlockInfo>();
            //foreach (var item in DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.Values)
            //{
            //    if (item.IsVirtual)
            //        continue;
            //    dicBlock.Add(item.BlcName, item);
            //}
            var dicBlock = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;           
            var list = dicBlock.ToList();

            int targetVirtual = list.Count - 1;
            int targetNotVirtual = list.Count - 1;
            for (int i = list.Count - 1; i >= 0; i--)
            {              
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    //this.Btn_BlockUp.IsEnabled = (i >= list.Count - 1) ? false : true;
                    //this.Btn_BlockDown.IsEnabled = true;
                    //blockNameNext = list[i + 1].Value.BlcName;
                    //if (list[i + 1].Value.IsVirtual)
                    //{
                    //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                    //    PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                    //    PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;
                    //}
                    if (list[i].Value.IsVirtual)
                    {
                        if (list[targetVirtual].Value.IsVirtual)
                            blockNameNext = list[targetVirtual].Value.BlcName;
                    }                       
                    else
                    {
                        if (!list[targetNotVirtual].Value.IsVirtual)
                            blockNameNext = list[targetNotVirtual].Value.BlcName;
                    }
                    break;
                }
                if (list[i].Value.IsVirtual)
                    targetVirtual = i;
                else
                    targetNotVirtual = i;
            }

            if (!String.IsNullOrEmpty(blockNameNext) && blockNameNext != blockName)
            {
                this.Btn_BlockText.Text = blockNameNext;

                //20201020 new API getBlockListForYardSector (VIEW_INV = true && VIEW_BLOCK_LIST = false) when select up/down button
                if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList)
                {
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, blockNameNext);
                } else
                {
                    var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = blockNameNext,
                        m_cBay = String.Empty,
                        m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                        m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                    };

                    PresentationMgr.Singleton.CurrentPostion = pos;

                    // call getBlockMapList
                    //PresentationMgr.AppWin.ShowProgressBar(0);
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockNameNext);
                }
            }
            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
            {
                PresentationMgr.Singleton.NeedJobAutoSelection = true;
                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(Btn_BlockText.Text);
                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            }
        }

        void Btn_BlockText_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //String blockName = this.Btn_BlockText.Text.ToString();
            //if (String.IsNullOrEmpty(blockName))
            //    return;
            blockView = true;
            PresentationMgr.Singleton.PrevBlckBayUIMode = PresentationMgr.Singleton.CurrentUIMode;
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Ask();
        }

        void Btn_BlockDown_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            Btn_BayText.IsEnabled = false;

            String blockNamePre = String.Empty;

            //var dicBlock = new Dictionary<String, VD_Common_SimpleBlockInfo>();
            //foreach (var item in DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.Values)
            //{
            //    if (item.IsVirtual)
            //        continue;
            //    dicBlock.Add(item.BlcName, item);
            //}
            var dicBlock = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;           
            var list = dicBlock.ToList();

            int targetVirtual = 0;
            int targetNotVirtual = 0;
            for (int i = 0; i <= list.Count - 1; i++)
            {
                if (0 == blockName.CompareTo(list[i].Value.BlcName))
                {
                    //this.Btn_BlockDown.IsEnabled = i <= 1 ? false : true;
                    //this.Btn_BlockUp.IsEnabled = true;
                    //blockNamePre = list[i - 1].Value.BlcName;
                    //if (list[i - 1].Value.IsVirtual)
                    //{
                    //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                    //    PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                    //    PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;
                    //}
                    if (list[i].Value.IsVirtual)
                    {
                        if (list[targetVirtual].Value.IsVirtual)
                            blockNamePre = list[targetVirtual].Value.BlcName;

                    }
                    else
                    {
                        if (!list[targetNotVirtual].Value.IsVirtual)
                            blockNamePre = list[targetNotVirtual].Value.BlcName;
                    }
                    break;
                }
                if (list[i].Value.IsVirtual)
                    targetVirtual = i;
                else
                    targetNotVirtual = i;
            }

            if (!String.IsNullOrEmpty(blockNamePre) && blockNamePre != blockName)
            {
                this.Btn_BlockText.Text = blockNamePre;

                //20201020 new API getBlockListForYardSector (VIEW_INV = true && VIEW_BLOCK_LIST = false) when select up/down button
                if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList)
                {
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, blockNamePre);
                } else
                {
                    var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = blockNamePre,
                        m_cBay = String.Empty,
                        m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                        m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                    };

                    PresentationMgr.Singleton.CurrentPostion = pos;

                    // call getBlockMapList
                    //PresentationMgr.AppWin.ShowProgressBar(0);
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockNamePre);
                }
            }
            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
            {
                PresentationMgr.Singleton.NeedJobAutoSelection = true;
                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(Btn_BlockText.Text);
                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            }
            if (PresentationMgr.MainView.UC_SwapView.CheckBox_Block_All.IsChecked == true)
            {
                var jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;

                //20210406 getSwapList blockName-bayName
                String bayName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text.ToString();
                if (bayName.Length > 1) bayName = "-" + bayName.Substring(0, 2);
                else bayName = String.Empty;

                VMT_Data_JAT2.VMT_DataMgr_Common.getSwapList_Ask(jobKey, PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text.ToString() + bayName, true);
            }
        }

        void Btn_BayUp_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = this.Btn_BayText.Text.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            PresentationMgr.Singleton.SaveLog("BAY_UP_CLICK_" + blockName + "_" + bayName);

            String bayNameNext = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayUp.IsEnabled = (i >= list.Count - 1) || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayDown.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                    bayNameNext = list[i + 1].BayName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(bayNameNext) && bayNameNext != bayName)
            {
                this.Btn_BayText.Text = bayNameNext;

                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = bayNameNext,
                    m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                };

                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        void Btn_BayText_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
            {
                m_cBlock = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text.ToString(),
                m_cBay = PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text.ToString(),
                m_cRow = "",
                m_cTier = ""
            };
            PresentationMgr.MainView.prevPos = pos;
            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;

            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "GetBlockMapList_Ask"), blockName);

            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockName);
            PresentationMgr.Singleton.PrevBlckBayUIMode = PresentationMgr.Singleton.CurrentUIMode;
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BaySelectionView);

            PresentationMgr.AppWin.ShowProgressBar(0);
            //PresentationMgr.MainView.UC_BaySelectionView.MakeupBayItems();
        }

        void Btn_BayDown_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String bayName = this.Btn_BayText.Text.ToString();
            if (String.IsNullOrEmpty(bayName))
                return;
            String blockName = this.Btn_BlockText.Text.ToString();
            if (String.IsNullOrEmpty(blockName))
                return;
            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            PresentationMgr.Singleton.SaveLog("BAY_DOWN_CLICK_" + blockName + "_" + bayName);

            String bayNamePre = String.Empty;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 1; i < list.Count; i++)
            {
                if (0 == bayName.CompareTo(list[i].BayName))
                {
                    this.Btn_BayDown.IsEnabled = i <= 1 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayUp.IsEnabled = !PresentationMgr.Singleton.showViewINV ? false : true;
                    bayNamePre = list[i - 1].BayName;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(bayNamePre) && bayNamePre != bayName)
            {
                this.Btn_BayText.Text = bayNamePre;

                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = bayNamePre,
                    m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                };

                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        void Btn_V_Block_Click(object sender, RoutedEventArgs e)
        {
            if ("".Equals(PresentationMgr.Singleton.CorrectionSource.CntrNo))
            {
                virtualBlockMode = 1;
            }
            else
            {
                virtualBlockMode = 2;
            }

            if (virtualBlockMode == 1)
            {
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Ask();
                PresentationMgr.MainView.UC_VBlockView.Visibility = Visibility.Visible;
            }
            else if (virtualBlockMode == 2)
            {
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Ask();
                PresentationMgr.MainView.UC_VBlockChangeView.Visibility = Visibility.Visible;
            }


        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                //);

                /*PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);*/

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_BayDown,
                    UIThemeMgr.Day.ButtonDownDefaultImage, UIThemeMgr.Day.ButtonDownPressImage, UIThemeMgr.Day.ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                //);

                /*PresentationMgr.SetSkinButton(this.Btn_BayText,
                    UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);*/

                PresentationMgr.SetSkinButton(this.Btn_BayUp,
                    UIThemeMgr.Day.ButtonUpDefaultImage, UIThemeMgr.Day.ButtonUpPressImage, UIThemeMgr.Day.ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_V_Block,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_BlockDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                //);

                /*PresentationMgr.SetSkinButton(this.Btn_BlockText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);*/

                PresentationMgr.SetSkinButton(this.Btn_BlockUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_BayDown,
                    UIThemeMgr.Night.ButtonDownDefaultImage, UIThemeMgr.Night.ButtonDownPressImage, UIThemeMgr.Night.ButtonDownDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Down_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Down_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Down_Disable.png", UriKind.Relative))
                //);

                /*PresentationMgr.SetSkinButton(this.Btn_BayText,
                    UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1, UIThemeMgr.Night.ButtonDefaultImage_1);
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default_1.png", UriKind.Relative))
                    //);*/

                PresentationMgr.SetSkinButton(this.Btn_BayUp,
                    UIThemeMgr.Night.ButtonUpDefaultImage, UIThemeMgr.Night.ButtonUpPressImage, UIThemeMgr.Night.ButtonUpDisableImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Up_Default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Up_Press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_Up_Disable.png", UriKind.Relative))
                //);

                PresentationMgr.SetSkinButton(this.Btn_V_Block,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_press.png", UriKind.Relative)),
                //new BitmapImage(new Uri(@"/VMT_RTG;component/Images(Night)/MainView/Button/Button_default.png", UriKind.Relative))
                //);
            }
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void Btn_BlockText_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Btn_BlockText.Text = "";

            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.Btn_BlockText);
            PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Visible;
            PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.UC_KeypadView.Grid_TextPad.Visibility = Visibility.Hidden;

            if (e.ClickCount > 1)
            {
                Btn_BlockText.Text = PresentationMgr.Singleton.CurrentBlock;
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentBlock,
                    m_cBay = PresentationMgr.Singleton.CurrentBay,
                    m_cRow = "",
                    m_cTier = ""
                };
                PresentationMgr.MainView.prevPos = pos;
                blockView = true;
                PresentationMgr.Singleton.PrevBlckBayUIMode = PresentationMgr.Singleton.CurrentUIMode;

                //20201020 new API getBlockListForYardSector (VIEW_INV = true && VIEW_BLOCK_LIST = false)
                if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList)
                {
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, PresentationMgr.Singleton.CurrentBlock);
                } else
                {
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForBlockMap_Ask();
                }

                PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Hidden;
                PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Visible;
                PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
            }
        }

        private void Btn_BlockText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Btn_BlockText.Text == PresentationMgr.Singleton.CurrentBlock)
                return;
            if (this.Btn_BlockText.Text.Length == 2 || this.Btn_BlockText.Text.Length == 4)
            {
                String blockName = String.Empty;

                String blockNamePre = PresentationMgr.Singleton.CurrentBlock;
                var dicBlock = new Dictionary<String, VD_Common_SimpleBlockInfo>();
                foreach (var item in DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.Values)
                {
                    if (item.IsVirtual)
                        continue;
                    dicBlock.Add(item.BlcName, item);
                }
                var list = dicBlock.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (0 == this.Btn_BlockText.Text.ToString().CompareTo(list[i].Value.BlcName))
                    {
                        this.Btn_BlockDown.IsEnabled = i == 0 ? false : true;
                        this.Btn_BlockUp.IsEnabled = i == list.Count - 1 ? false : true;
                        blockName = this.Btn_BlockText.Text.ToString();
                        break;
                    }
                }
                if (String.IsNullOrEmpty(blockName))
                {
                    if(this.Btn_BlockText.Text.Length == 4)
                    {
                        Btn_BlockText.Text = PresentationMgr.Singleton.CurrentBlock;
                        PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Visible;
                        PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Hidden;
                        PresentationMgr.AppWin.UC_KeypadView.Grid_TextPad.Visibility = Visibility.Hidden;
                        this.Btn_V_Block.Focus();
                    }
                    return;
                }

                if (blockNamePre != blockName)
                {
                    var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = blockName,
                        m_cBay = String.Empty,
                        m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                        m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                    };

                    PresentationMgr.Singleton.CurrentPostion = pos;

                    // call getBlockMapList
                    //PresentationMgr.AppWin.ShowProgressBar(0);
                    VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockName);
                }
                if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
                {
                    PresentationMgr.Singleton.NeedJobAutoSelection = true;
                    VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(Btn_BlockText.Text);
                    PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                    VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                    //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                    VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                }
                PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Hidden;
                PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Visible;
                PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
                this.Btn_V_Block.Focus();
            }
        }

        private void Btn_BlockText_GotFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.Btn_BlockText);
            PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Visible;
            PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.UC_KeypadView.Grid_TextPad.Visibility = Visibility.Hidden;
            this.Btn_BlockText.Focus();
        }

        private void Btn_BlockText_LostFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Visible;
            PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
            if (this.Btn_BlockText.Text == PresentationMgr.Singleton.CurrentBlock)
                return;
            String blockName = String.Empty;

            String blockNamePre = PresentationMgr.Singleton.CurrentBlock;
            var dicBlock = new Dictionary<String, VD_Common_SimpleBlockInfo>();
            foreach (var item in DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.Values)
            {
                if (item.IsVirtual)
                    continue;
                dicBlock.Add(item.BlcName, item);
            }
            var list = dicBlock.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (0 == this.Btn_BlockText.Text.ToString().CompareTo(list[i].Value.BlcName))
                {
                    this.Btn_BlockDown.IsEnabled = i == 0 ? false : true;
                    this.Btn_BlockUp.IsEnabled = i == list.Count - 1 ? false : true;
                    blockName = this.Btn_BlockText.Text.ToString();
                    break;
                }
            }
            if (String.IsNullOrEmpty(blockName))
            {
                Btn_BlockText.Text = PresentationMgr.Singleton.CurrentBlock;
                return;
            }

            if (blockNamePre != blockName)
            {
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = blockName,
                    m_cBay = String.Empty,
                    m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                };

                PresentationMgr.Singleton.CurrentPostion = pos;

                // call getBlockMapList
                PresentationMgr.AppWin.ShowProgressBar(0);
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockName);
            }
            if (VMT_Data_JAT2.Objects.Common.BlckVal != "")
            {
                PresentationMgr.Singleton.NeedJobAutoSelection = true;
                VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(Btn_BlockText.Text);
                PresentationMgr.MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
            }
        }

        private void Btn_BayText_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Btn_BayText.Text = "";
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.Btn_BayText);
            PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Visible;
            PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.UC_KeypadView.Grid_TextPad.Visibility = Visibility.Hidden;

            if (e.ClickCount > 1)
            {
                Btn_BayText.Text = PresentationMgr.Singleton.CurrentBay;
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentBlock,
                    m_cBay = PresentationMgr.Singleton.CurrentBay,
                    m_cRow = "",
                    m_cTier = ""
                };
                PresentationMgr.MainView.prevPos = pos;
                String blockName = this.Btn_BlockText.Text.ToString();
                if (String.IsNullOrEmpty(blockName))
                    return;

                InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "GetBlockMapList_Ask"), blockName);

                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(blockName);
                PresentationMgr.Singleton.PrevBlckBayUIMode = PresentationMgr.Singleton.CurrentUIMode;
                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BaySelectionView);

                PresentationMgr.AppWin.ShowProgressBar(0);
                //PresentationMgr.MainView.UC_BaySelectionView.MakeupBayItems();
                PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Hidden;
                PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Visible;
                PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
            }
        }

        private void Btn_BayText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Btn_BayText.Text == PresentationMgr.Singleton.CurrentBay)
                return;
            if (this.Btn_BayText.Text.Length > 1)
            {
                String blockName = this.Btn_BlockText.Text.ToString();
                if (String.IsNullOrEmpty(blockName))
                    return;
                String bayName = String.Empty;

                if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                    DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                    return;

                String bayNamePre = PresentationMgr.Singleton.CurrentBay;
                var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
                list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
                for (int i = 0; i < list.Count; i++)
                {
                    if (0 == this.Btn_BayText.Text.ToString().CompareTo(list[i].BayName))
                    {
                        this.Btn_BayDown.IsEnabled = i == 0 || !PresentationMgr.Singleton.showViewINV ? false : true;
                        this.Btn_BayUp.IsEnabled = i == list.Count - 1 || !PresentationMgr.Singleton.showViewINV ? false : true;
                        bayName = this.Btn_BayText.Text.ToString();
                        break;
                    }
                }

                if (String.IsNullOrEmpty(bayName))
                {
                    Btn_BayText.Text = PresentationMgr.Singleton.CurrentBay;
                    PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_KeypadView.Grid_TextPad.Visibility = Visibility.Hidden;
                    this.Btn_V_Block.Focus();
                    return;
                }

                if (bayNamePre != bayName)
                {
                    var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                    {
                        m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                        m_cBay = bayName,
                        m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                        m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                    };

                    PresentationMgr.Singleton.CurrentPostion = pos;
                }
                PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Hidden;
                PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Visible;
                PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
                this.Btn_V_Block.Focus();
            }
        }

        private void Btn_BayText_GotFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.ShowKeyPad(this.Btn_BayText);
            PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Visible;
            PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.UC_KeypadView.Grid_TextPad.Visibility = Visibility.Hidden;
            this.Btn_BayText.Focus();
        }

        private void Btn_BayText_LostFocus(object sender, RoutedEventArgs e)
        {
            PresentationMgr.AppWin.UC_KeypadView.Grid_FullPad.Visibility = Visibility.Hidden;
            PresentationMgr.AppWin.UC_KeypadView.Grid_DigitalPad.Visibility = Visibility.Visible;
            PresentationMgr.AppWin.UC_KeypadView.HideKeyPad();
            String blockName = PresentationMgr.Singleton.CurrentBlock;
            if (String.IsNullOrEmpty(blockName))
                return;
            String bayName = String.Empty;

            if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(blockName) ||
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay == null)
                return;

            String bayNamePre = PresentationMgr.Singleton.CurrentBay;
            var list = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[blockName].DicBay.Values.ToList();
            list.Sort(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBayInfo.Compare);
            for (int i = 0; i < list.Count; i++)
            {
                if (0 == this.Btn_BayText.Text.ToString().CompareTo(list[i].BayName))
                {
                    this.Btn_BayDown.IsEnabled = i == 0 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    this.Btn_BayUp.IsEnabled = i == list.Count - 1 || !PresentationMgr.Singleton.showViewINV ? false : true;
                    bayName = this.Btn_BayText.Text.ToString();
                    break;
                }
            }

            if (String.IsNullOrEmpty(bayName))
            {
                Btn_BayText.Text = PresentationMgr.Singleton.CurrentBay;
                return;
            }

            if (bayNamePre != bayName)
            {
                var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
                {
                    m_cBlock = PresentationMgr.Singleton.CurrentPostion.m_cBlock,
                    m_cBay = bayName,
                    m_cRow = String.Empty,//PresentationMgr.Singleton.CurrentPostion.m_cRow,
                    m_cTier = String.Empty//PresentationMgr.Singleton.CurrentPostion.m_cTier
                };

                PresentationMgr.Singleton.CurrentPostion = pos;
            }
        }

        private void KeypadDone() // FocusOut
        {
            this.Btn_V_Block.Focus();
        }
    }
}
