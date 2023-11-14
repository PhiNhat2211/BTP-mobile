using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
//using ExternalAPI;
using System.Runtime.InteropServices;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;
using System.Net.NetworkInformation;
using System.ComponentModel;

//20190108
using Common;
using Common.Interface;
using static VMT_Data_JAT2.Objects.Common;
using System.Text.RegularExpressions;

namespace VMT_RMG
{
    #region [ Presentation Manger Custom Routed Event Handler Defintion ]

    // UPdateUIEvent Event Handler Argument Class
    public class UpdateUIEventArgs : RoutedEventArgs
    {
        public UpdateUIEventArgs() { }
        public UpdateUIEventArgs(RoutedEvent routedEvent)
        {
            RoutedEvent = routedEvent;
        }
        public UpdateUIEventArgs(RoutedEvent routedEvent, object source)
        {
            RoutedEvent = routedEvent;
            Source = source;
        }

        public object Param { get; set; }
    }

    // UPdateDataEvent Event Handler Argument Class
    public class UpdateDataEventArgs : RoutedEventArgs
    {
        public UpdateDataEventArgs() { }
        public UpdateDataEventArgs(RoutedEvent routedEvent)
        {
            RoutedEvent = routedEvent;
        }
        public UpdateDataEventArgs(RoutedEvent routedEvent, object source)
        {
            RoutedEvent = routedEvent;
            Source = source;
        }

        public object Param { get; set; }
    }

    // UpdateUIEvent Handler Delegate
    public delegate void UpdateUIEventHandler(object sender, UpdateUIEventArgs e);
    // UpdateDataEvent Handler Delegate
    public delegate void UpdateDataEventHandler(object sender, UpdateDataEventArgs e);

    #endregion

    public class PresentationMgr : UIElement
    {
        public enum UIMode
        {
            LogInView, MainView,
            AvailableView, ContainerSearch, VirtualBlockSelectionView,
            BlockSelectionView, BaySelectionView,
            BlockSelectionView1, BaySelectionView1,
            BlockSelectionView2, BaySelectionView2,
            MachineSearch, ContainerDetailView,
            BreakTimeView, CorrectionView,
            MachineSettingView,
            ContainerSelectionView,
            TwinSelectionView, SwapView, ContainerMoving,
        }
        public String highlightRow = "";
        public Boolean sameBlckBay = false;
        public Boolean firstGetInven = true;
        private Thread thread = null;
        public int rowsw = -1;
        public int colsw = -1;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobAfterSearch = null;
        public bool flgNeedReset = true;
        public bool showSetting = false, showCHGLOC = false, showViewINV = false, showVBlock = false; //20201008 show/hide buttons (BP#471, 472, 475): setting (login view), btn_chg_loc (main view), mainview info
        public bool enableBayViewSelection = false;
        public String cntrSelected = String.Empty;
        public bool viewBLockList = false, callFirst1Time = false; //20201020 for new API getBlockListForYardSector
        public VMT_Data_JAT2.Objects.Common.VmtMachine machineLocationPrevious = null; //20201102 if value same -> no reload bayview

        public String bayviewModeTC9x7 = "TC";

        List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> oldInventory = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

        Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> oldDicItem = new Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>();

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> oldLstJobOrder = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

        List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation> oldArea = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation>();
        List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation> oldTier = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation>();

        public List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO> swapList = new List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO>();

        public List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO> reservedList = new List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO>();

        public List<VMT_Data_JAT2.Objects.Common.VmtSwap> swapListRTG = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();

        public UIMode PrevBlckBayUIMode = UIMode.MainView;

        public UIMode PrevUIMode = UIMode.LogInView;

        private UIMode _currentUIMode = UIMode.LogInView;

        public ContainerItem containerItemSelected;
        public ContainerItem preContainerItemSelected;
        public ContainerItem preContainerItemSelectedWrongColor;

        public SortedDictionary<int, VD_Common_SimpleRowInfo> rowMap;
        public VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB;

        public UIMode CurrentUIMode
        {
            get { return _currentUIMode; }
            set
            {
                this.UI_SwitchUI(value);
            }
        }

        private LanguageService languageSer = new LanguageService();

        public LanguageService LanguageSer
        {
            get { return this.languageSer; }
        }

        public enum UITheme
        {
            UITheme_Unknown, UITheme_Day, UITheme_Night
        }

        private UITheme _currentUITheme = UITheme.UITheme_Day;
        public UITheme CurrentUITheme
        {
            get { return _currentUITheme; }
            set
            {
                _currentUITheme = value;
                Change_UITheme();
                Notify_UITheme();
            }
        }

        private void Change_UITheme()
        {
            if (_currentUITheme == UITheme.UITheme_Day)
                UIThemeMgr.Singleton.Theme = AppTheme.Day;
            else if (_currentUITheme == UITheme.UITheme_Night)
                UIThemeMgr.Singleton.Theme = AppTheme.Night;
        }

        public event PropertyChangedEventHandler PropertyChanged_UITheme;
        private UITheme Notify_UITheme()
        {
            if (this.PropertyChanged_UITheme != null)
            {
                PropertyChanged_UITheme(this, new PropertyChangedEventArgs(CurrentUITheme.ToString()));
            }

            return _currentUITheme;
        }

        private bool _isUIInit = false;
        public bool IsUIInit
        {
            get { return _isUIInit; }
            set
            {
                _isUIInit = value;
            }
        }

        public void UIInit()
        {
            RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey += new System.ComponentModel.PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
        }

        void RMG_Member_PropertyChanged_TargetJobKey(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            String jobKey = RMG.RMG_Member.Singleton.TargetJobKey;
            if (String.IsNullOrEmpty(jobKey))
                return;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            if (jobOrder == null)
                return;

            /* Target Job Changed
             * Something
             */
        }

        private VMT_Data_JAT2.Marshalling.Geometry.sGeoPoint _currentGeoPoint;
        public VMT_Data_JAT2.Marshalling.Geometry.sGeoPoint CurrentGeoPoint
        {
            get { return _currentGeoPoint; }
            set
            {
                this._currentGeoPoint = value;
            }
        }

        private VMT_Data_JAT2.Marshalling.Geometry.sPosition _RevPosition = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
        public VMT_Data_JAT2.Marshalling.Geometry.sPosition RevPosition
        {
            get { return _RevPosition; }
            set
            {
                this._RevPosition = value;
                this.SetGeoPosition(value);
            }
        }

        private VMT_Data_JAT2.Marshalling.Geometry.sPosition _CurrentPostion = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
        public VMT_Data_JAT2.Marshalling.Geometry.sPosition CurrentPostion
        {
            get { return _CurrentPostion; }
            set
            {
                if (value.m_cBay != "" && Convert.ToInt32(BayRemoveChars(value.m_cBay)) % 2 == 0)
                {
                    value.m_cBay = (Convert.ToInt32(BayRemoveChars(value.m_cBay)) - 1).ToString();
                    if (value.m_cBay.Length == 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            value.m_cBay = "0" + value.m_cBay;
                        else
                            value.m_cBay = cha + value.m_cBay;
                    }
                }

                if (_CurrentPostion.m_cBlock != value.m_cBlock || _CurrentPostion.m_cBay != value.m_cBay ||
                    _CurrentPostion.m_cRow != value.m_cRow || _CurrentPostion.m_cTier != value.m_cTier)
                {
                    if (_CurrentPostion.m_cBlock != value.m_cBlock || _CurrentPostion.m_cBay != value.m_cBay)
                    {
                        if (_CurrentPostion.m_cBlock != value.m_cBlock)
                        {
                            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(value.m_cBlock);
                        }
                        if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value.m_cBlock) && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual != true && !String.IsNullOrEmpty(value.m_cBay))
                        {
                            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(value.m_cBlock, value.m_cBay, value.m_cBay);
                        }
                    }

                    if (this.CurrentUIMode == UIMode.SwapView)
                        CurrentPositionSwapChanged(value);
                    else
                    {
                        /*if (_CurrentPostion.m_cBlock == value.m_cBlock && _CurrentPostion.m_cBay == value.m_cBay)
                        {
                            if (PresentationMgr.MainView.selectedJobList1Time)
                            {
                                PresentationMgr.MainView.selectedJobList1Time = false;
                                PresentationMgr.Singleton.SetInventoryData(null);
                            }
                        }
                        else
                        {*/
                        CurrentPositionChanged(value);
                        //}
                    }
                }
                else if (!String.IsNullOrEmpty(value.m_cBlock) && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value.m_cBlock) && !DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].isBolBlck)
                {
                    sameBlckBay = true;
                    PresentationMgr.Singleton.SendGetInventoryAsk(value.m_cBlock, value.m_cBay, true, true, false);
                }
            }
        }

        public String getValueFromConfig(string group, string field)
        {
            string strIniFile = ForeignInfo.GetIniDirectory() + @"MachineInfo.ini";
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            return ini.IniReadValue(group, field);
        }

        private void CurrentPositionChanged(VMT_Data_JAT2.Marshalling.Geometry.sPosition value)
        {
            var prePosition = this._CurrentPostion;
            this._CurrentPostion = value;
            if (!MainView.selectedJobList && !String.IsNullOrEmpty(prePosition.m_cBlock) && value.m_cBlock != prePosition.m_cBlock)
                this.NeedMoveToTargetJobPage = true;

            if (flgNeedReset)
                PresentationMgr.MainView.UC_NavigatorView.NeedReset = true;
            else
                flgNeedReset = true;

            if (!String.IsNullOrEmpty(value.m_cBlock) && //!String.IsNullOrEmpty(value.m_cBay) &&
                                PresentationMgr.MainView.UC_BlockSelectionView.Visibility != Visibility.Visible &&
                                PresentationMgr.MainView.UC_BaySelectionView.Visibility != Visibility.Visible)
            {
                if (MainWindow.firstLoad || (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value.m_cBlock) ||
                    (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].DicBay == null//.Count <= 0) 
                     && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual != true && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].isBolBlck) || (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].isBolBlck && !String.IsNullOrEmpty(value.m_cBay))))
                {
                    //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, _CurrentPostion.m_cBlock);
                    if (!String.IsNullOrEmpty(value.m_cBlock))
                    {
                        // Aug 10 2022 Fix mixed bay first select time
                        if (value.m_cBlock != prePosition.m_cBlock || value.m_cBay != prePosition.m_cBay)
                            sameBlckBay = false;
                        //20201020 new API getBlockListForYardSector (VIEW_INV = true && VIEW_BLOCK_LIST = false) when change job
                        if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList && !value.m_cBlock.Equals(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName))
                        {
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, value.m_cBlock);
                        }

                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Ask(value.m_cBlock);
                        if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetVirtualContNo))
                        {
                            String cntrNo = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetVirtualContNo;
                            var listLeft = PresentationMgr.MainView.UC_ContainerArea.ListBox_Job;
                            int i = 0;
                            foreach (JobListItem jItem in listLeft.Items)
                            {
                                if (jItem.TextBlock_CntrNo.Text.Equals(cntrNo))
                                {
                                    listLeft.SelectedIndex = i;
                                    jItem.Selected = true;
                                    break;
                                }
                                i++;
                            }
                        }
                        //PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                }
                else
                {
                    // 동일한 inventory면 현재 데이터로 view갱신만 시켜준다.
                    if (value.m_cBlock == prePosition.m_cBlock && value.m_cBay == prePosition.m_cBay)
                    {
                        sameBlckBay = true;
                        //PresentationMgr.Singleton.SendGetInventoryAsk(value.m_cBlock, value.m_cBay, true, true, false);
                        //{
                        //    var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
                        //    location.blck = value.m_cBlock;
                        //    string bay = value.m_cBay;
                        //    string block = value.m_cBlock;

                        //    if (!String.IsNullOrEmpty(bay))
                        //    {
                        //        var intBay = Convert.ToInt32(bay);
                        //        if (intBay > 2 && intBay % 2 != 0)
                        //        {
                        //            if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                        //                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay != null &&
                        //                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay.ContainsKey(intBay - 2))
                        //            {
                        //                location.bay = Convert.ToString(intBay - 2);
                        //                if (location.bay.Length <= 1)
                        //                    location.bay = "0" + location.bay;

                        //                DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        //                DataMgr.Singleton.Inventory_AddItemToClear(location.blck, location.bay);
                        //            }
                        //        }

                        //        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(bay));
                        //    }
                        //    DataMgr.Singleton.Inventory_AddItemToClear(location.blck, bay);
                        //}

                        //this.thread = new Thread(new ThreadStart(SetInventoryDataJobClickSameLoc));
                        //this.thread.Start();
                        SetInventoryDataJobClickSameLoc();
                        //PresentationMgr.Singleton.SetInventoryData(null);
                    }
                    else
                    {
                        var bayName = value.m_cBay;

                        if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual)
                            bayName = String.Empty;

                        //if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual)
                        //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(value.m_cBlock, value.m_cBay, value.m_cBay);
                        //else
                        //    bayName = String.Empty;

                        sameBlckBay = false;
                        PresentationMgr.Singleton.SendGetInventoryAsk(value.m_cBlock, bayName);
                        //this.Dispatcher.Invoke(new Action(() => PresentationMgr.Singleton.SendGetInventoryAsk(value.m_cBlock, bayName)));

                        //20201020 new API getBlockListForYardSector (VIEW_INV = true && VIEW_BLOCK_LIST = false) when change job
                        if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList && !value.m_cBlock.Equals(PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName))
                        {
                            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, value.m_cBlock);
                        }
                    }
                }
            }

            PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.BayName = value.m_cBay;
            PresentationMgr.MainView.UC_BlockSelectionView.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_BlockSelectionView.SelectedBayName = value.m_cBay;
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBayName = value.m_cBay;


            PresentationMgr.MainView.Btn_Chg_Loc.IsEnabled =
                !String.IsNullOrEmpty(value.m_cBlock) && !String.IsNullOrEmpty(value.m_cBay);
        }

        public void CurrentPositionSwapChanged(VMT_Data_JAT2.Marshalling.Geometry.sPosition value)
        {
            var prePosition = this._CurrentPostion;
            if (PresentationMgr.MainView.UC_SwapView.CheckBox_Block_All.IsChecked == true && value.m_cBlock != this.CurrentBlock)
            {
                var jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;

                //20210406 getSwapList blockName-bayName
                String bayName = PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BayText.Text.ToString();
                if (bayName.Length > 1) bayName = "-" + bayName.Substring(0, 2);
                else bayName = String.Empty;

                VMT_Data_JAT2.VMT_DataMgr_Common.getSwapList_Ask(jobKey, PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text.ToString() + bayName, true);
            }
            this._CurrentPostion = value;

            PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.NeedReset = true;
            if (!String.IsNullOrEmpty(value.m_cBlock) && //!String.IsNullOrEmpty(value.m_cBay) &&
                                PresentationMgr.MainView.UC_SwapView.UC_BlockSelectionView.Visibility != Visibility.Visible &&
                                PresentationMgr.MainView.UC_SwapView.UC_BaySelectionView.Visibility != Visibility.Visible)
            {
                if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value.m_cBlock) ||
                    (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].DicBay == null//.Count <= 0) 
                     && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual != true))
                {
                    //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, _CurrentPostion.m_cBlock);
                    if (!String.IsNullOrEmpty(value.m_cBlock))
                    {
                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapListSwap_Ask(value.m_cBlock);
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                }
                else
                {
                    // 동일한 inventory면 현재 데이터로 view갱신만 시켜준다.
                    if (value.m_cBlock == prePosition.m_cBlock && value.m_cBay == prePosition.m_cBay)
                    {
                        PresentationMgr.Singleton.SetInventoryDataSwap(null);
                    }
                    else
                    {
                        var bayName = value.m_cBay;

                        if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual)
                            bayName = String.Empty;

                        //if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual)
                        //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(value.m_cBlock, value.m_cBay, value.m_cBay);
                        //else
                        //    bayName = String.Empty;

                        PresentationMgr.Singleton.SendGetInventorySwapAsk(value.m_cBlock, bayName);
                    }
                }
            }
            PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BayName = value.m_cBay;
            PresentationMgr.MainView.UC_SwapView.UC_BlockSelectionView.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_SwapView.UC_BaySelectionView.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_SwapView.UC_BaySelectionView.SelectedBayName = value.m_cBay;

        }
        private VMT_Data_JAT2.Marshalling.Geometry.sPosition _MovingPosition1 = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
        public VMT_Data_JAT2.Marshalling.Geometry.sPosition MovingPosition1
        {
            get { return _MovingPosition1; }
            set
            {
                CurrentPositionMovingChanged1(value);
            }
        }
        private VMT_Data_JAT2.Marshalling.Geometry.sPosition _MovingPosition2 = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
        public VMT_Data_JAT2.Marshalling.Geometry.sPosition MovingPosition2
        {
            get { return _MovingPosition2; }
            set
            {
                CurrentPositionMovingChanged2(value);
            }
        }
        public void CurrentPositionMovingChanged1(VMT_Data_JAT2.Marshalling.Geometry.sPosition value)
        {
            var prePosition = this._MovingPosition1;
            this._MovingPosition1 = value;

            PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.NeedReset = true;
            if (!String.IsNullOrEmpty(value.m_cBlock) && //!String.IsNullOrEmpty(value.m_cBay) &&
                                PresentationMgr.MainView.UC_BlockSelectionView1.Visibility != Visibility.Visible &&
                                PresentationMgr.MainView.UC_BaySelectionView1.Visibility != Visibility.Visible)
            {
                if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value.m_cBlock) ||
                    (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].DicBay == null//.Count <= 0) 
                     && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual != true))
                {
                    //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, _CurrentPostion.m_cBlock);
                    if (!String.IsNullOrEmpty(value.m_cBlock))
                    {
                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapListMoving1_Ask(value.m_cBlock);
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                }
                else
                {
                    // 동일한 inventory면 현재 데이터로 view갱신만 시켜준다.
                    if (value.m_cBlock == prePosition.m_cBlock && value.m_cBay == prePosition.m_cBay)
                    {
                        PresentationMgr.Singleton.SetInventoryDataBay1(null);
                    }
                    else
                    {
                        var bayName = value.m_cBay;
                        if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual)
                        {
                            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier1_Ask(value.m_cBlock, value.m_cBay, value.m_cBay);
                        }
                        else
                            bayName = String.Empty;

                        PresentationMgr.Singleton.SendGetInventory1Ask(value.m_cBlock, bayName);
                    }
                }
            }
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.BlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.BayName = value.m_cBay;
            PresentationMgr.MainView.UC_BlockSelectionView1.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_BaySelectionView1.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_BaySelectionView1.SelectedBayName = value.m_cBay;

            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.BlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.BayName = value.m_cBay;

        }

        public void CurrentPositionMovingChanged2(VMT_Data_JAT2.Marshalling.Geometry.sPosition value)
        {
            var prePosition = this._MovingPosition2;
            this._MovingPosition2 = value;

            PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.NeedReset = true;
            if (!String.IsNullOrEmpty(value.m_cBlock) && //!String.IsNullOrEmpty(value.m_cBay) &&
                                PresentationMgr.MainView.UC_BlockSelectionView2.Visibility != Visibility.Visible &&
                                PresentationMgr.MainView.UC_BaySelectionView2.Visibility != Visibility.Visible)
            {
                if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(value.m_cBlock) ||
                    (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].DicBay == null//.Count <= 0) 
                     && DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual != true))
                {
                    //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, _CurrentPostion.m_cBlock);
                    if (!String.IsNullOrEmpty(value.m_cBlock))
                    {
                        VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapListMoving2_Ask(value.m_cBlock);
                        PresentationMgr.AppWin.ShowProgressBar(0);
                    }
                }
                else
                {
                    // 동일한 inventory면 현재 데이터로 view갱신만 시켜준다.
                    if (value.m_cBlock == prePosition.m_cBlock && value.m_cBay == prePosition.m_cBay)
                    {
                        PresentationMgr.Singleton.SetInventoryDataBay2(null);
                    }
                    else
                    {
                        var bayName = value.m_cBay;
                        if (!DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[value.m_cBlock].IsVirtual)
                        {
                            VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier2_Ask(value.m_cBlock, value.m_cBay, value.m_cBay);
                        }
                        else
                            bayName = String.Empty;

                        PresentationMgr.Singleton.SendGetInventory2Ask(value.m_cBlock, bayName);
                    }
                }
            }
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.BlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.BayName = value.m_cBay;
            PresentationMgr.MainView.UC_BlockSelectionView2.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_BaySelectionView2.SelectedBlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_BaySelectionView2.SelectedBayName = value.m_cBay;

            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.BlockName = value.m_cBlock;
            PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.BayName = value.m_cBay;

        }
        public String CurrentBlock
        {
            get
            {
                if (_CurrentPostion.m_cBlock == null)
                    return String.Empty;

                return _CurrentPostion.m_cBlock;
            }
        }

        public String CurrentBay
        {
            get
            {
                if (_CurrentPostion.m_cBay == null)
                    return String.Empty;

                return _CurrentPostion.m_cBay;
            }
        }

        public String CurrentRow
        {
            get
            {
                if (_CurrentPostion.m_cRow == null)
                    return String.Empty;

                return _CurrentPostion.m_cRow;
            }
        }

        public String CurrentTier
        {
            get
            {
                if (_CurrentPostion.m_cTier == null)
                    return String.Empty;

                return _CurrentPostion.m_cTier;
            }
        }

        public String CurrentBlock1
        {
            get
            {
                if (_MovingPosition1.m_cBlock == null)
                    return String.Empty;

                return _MovingPosition1.m_cBlock;
            }
        }

        public String CurrentBay1
        {
            get
            {
                if (_MovingPosition1.m_cBay == null)
                    return String.Empty;

                return _MovingPosition1.m_cBay;
            }
        }

        public String CurrentRow1
        {
            get
            {
                if (_MovingPosition1.m_cRow == null)
                    return String.Empty;

                return _MovingPosition1.m_cRow;
            }
        }

        public String CurrentTier1
        {
            get
            {
                if (_MovingPosition1.m_cTier == null)
                    return String.Empty;

                return _MovingPosition1.m_cTier;
            }
        }

        public String CurrentBlock2
        {
            get
            {
                if (_MovingPosition2.m_cBlock == null)
                    return String.Empty;

                return _MovingPosition2.m_cBlock;
            }
        }

        public String CurrentBay2
        {
            get
            {
                if (_MovingPosition2.m_cBay == null)
                    return String.Empty;

                return _MovingPosition2.m_cBay;
            }
        }

        public String CurrentRow2
        {
            get
            {
                if (_MovingPosition2.m_cRow == null)
                    return String.Empty;

                return _MovingPosition2.m_cRow;
            }
        }

        public String CurrentTier2
        {
            get
            {
                if (_MovingPosition2.m_cTier == null)
                    return String.Empty;

                return _MovingPosition2.m_cTier;
            }
        }

        private double _trolleyPos;
        public double TrolleyPos
        {
            get { return _trolleyPos; }
            set
            {
                this._trolleyPos = value;
                //this.YDV_SetTrolleyPos(_trolleyPos);
            }
        }

        private double _hoistPosition;
        public double HoistPosition
        {
            get { return _hoistPosition; }
            set
            {
                this._hoistPosition = value;
                //this.YDV_SetHoistPos(_hoistPosition);
            }
        }

        public class LoadingSwapInfo
        {
            public string MachineID;
            public string ChssPsn;

            public LoadingSwapInfo()
            {
                MachineID = string.Empty;
                ChssPsn = string.Empty;
            }
        }

        public static Dictionary<string, LoadingSwapInfo> _loadingSwapInfos = new Dictionary<string, LoadingSwapInfo>();
        public static void AddLoadingSwapInfo(string jobKey, LoadingSwapInfo info)
        {
            if (PresentationMgr._loadingSwapInfos.ContainsKey(jobKey))
                PresentationMgr._loadingSwapInfos[jobKey] = info;
            else
                PresentationMgr._loadingSwapInfos.Add(jobKey, info);
        }
        public static void RemoveLoadingSwapInfo(string jobKey)
        {
            if (PresentationMgr._loadingSwapInfos.ContainsKey(jobKey))
            {
                PresentationMgr._loadingSwapInfos[jobKey] = null;
                PresentationMgr._loadingSwapInfos.Remove(jobKey);
            }
        }
        public static LoadingSwapInfo GetLoadingSwapInfo(string jobKey)
        {
            if (PresentationMgr._loadingSwapInfos.ContainsKey(jobKey))
                return PresentationMgr._loadingSwapInfos[jobKey];
            else
                return null;
        }
        public static void ClearLoadingSwapInfo()
        {
            var keys = PresentationMgr._loadingSwapInfos.Keys.ToList();
            foreach (var key in keys)
                PresentationMgr._loadingSwapInfos[key] = null;

            PresentationMgr._loadingSwapInfos.Clear();
        }

        public String ProcessingJobKey { get; set; }
        public SIEMENS.Packet.BodyRmgStatus CurrentRmgStatusPacket { get; set; }
        public SIEMENS.Packet.BodyRmgStatus LastestRmgStatusPacket { get; set; }
        public bool? IsTwistLock { get; set; }
        public Boolean IsSiemensConnectionChecked { get; set; }

        private Boolean _needJobAutoSelection;
        public Boolean NeedJobAutoSelection
        {
            get
            {
                return _needJobAutoSelection;
            }
            set
            {
                this._needJobAutoSelection = value;
            }
        }

        public static Boolean UseCorrection { get; set; }   // 추후 TOS로부터 설정값을 받아와 셋팅
        public static Boolean UseFromLocationForRehandling = true;
        public static Boolean ShowUnplugReeferOnly = false;
        public static String jobItemColor = "";
        public static Boolean ShowDispatchedLoadingJobOnly = true;  // 요구사항 : Only the  dispatched ITV should be shown for loading ( no need to show all the activated queue containers for loading )         

        private Boolean _needRehandlingJobRefresh = false;

        //private bool _isCPSAlign = false;
        //public bool IsCPSAlign
        //{
        //    get { return _isCPSAlign; }
        //    set
        //    {
        //        _isCPSAlign = value;
        //        this.UI_SetCPSStatus(_isCPSAlign);
        //    }
        //}

        //private string _twistLockStatus = "Unlock";
        //public string TwistLockStatus
        //{
        //    get { return _twistLockStatus; }
        //    set
        //    {
        //        _twistLockStatus = value;
        //        this.UI_SetTwistLock(_twistLockStatus);
        //    }
        //}

        #region [ Custom Routed Evet Registration ]
        public static readonly RoutedEvent UpdateUIEvent =
            EventManager.RegisterRoutedEvent("UpateUI", RoutingStrategy.Bubble, typeof(UpdateUIEventHandler), typeof(PresentationMgr));
        public static readonly RoutedEvent UpdateDataEvent =
            EventManager.RegisterRoutedEvent("UpdateData", RoutingStrategy.Bubble, typeof(UpdateDataEventHandler), typeof(PresentationMgr));

        public event UpdateUIEventHandler UpdateUI
        {
            add { AddHandler(PresentationMgr.UpdateUIEvent, value); }
            remove { RemoveHandler(PresentationMgr.UpdateUIEvent, value); }
        }

        public event UpdateDataEventHandler UpdateData
        {
            add { AddHandler(PresentationMgr.UpdateDataEvent, value); }
            remove { RemoveHandler(PresentationMgr.UpdateDataEvent, value); }
        }
        #endregion

        #region [ Constant Value Defintion ]
        //--------------------------------------------------------------
        //- Change Image Day <-> Night 
        //--------------------------------------------------------------
        public static SolidColorBrush brush_text_gray = new SolidColorBrush(Color.FromArgb(255, 157, 160, 158));
        #endregion [ Constant Value Definition ]

        // UI Component Object Reference Hash Table
        private Hashtable uiComponentTable = null;

        private static readonly PresentationMgr _theOnly = null;

        public static MainWindow AppWin
        {
            get { return Application.Current.MainWindow as MainWindow; }
        }

        public static MainView MainView
        {
            get
            {
                return PresentationMgr.Singleton.GetUIComponent("VMT_RMG.MainView") as MainView;
            }
        }

        public static PresentationMgr Singleton
        {
            get { return _theOnly; }
        }

        static PresentationMgr()
        {
            _theOnly = new PresentationMgr();
        }

        private PresentationMgr()
        {
            uiComponentTable = new Hashtable();
            // Register Custom Routed Event
            this.ProcessingJobKey = String.Empty;
            this.IsTwistLock = null;
            this.IsSiemensConnectionChecked = false;
            this.NeedJobAutoSelection = true;

            PresentationMgr.UseCorrection = false;
            this.CurrentRmgStatusPacket = null;
            this.LastestRmgStatusPacket = null;
            this.swapList = new List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO>();
            this.reservedList = new List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO>();
            this.swapListRTG = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();
        }

        public void ClearData()
        {
            PresentationMgr.MainView.UC_NavigatorView.ClearNaviItems();
            PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ClearNaviItems();
            PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
            PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Clear();
            PresentationMgr.MainView.UC_JobList.JobList_Clear();
            PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ClearNaviItems();
            PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ClearNaviItems();
            PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ContainerList_Clear();
            PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ContainerList_Clear();
            this.ProcessingJobKey = String.Empty;
            this.NeedJobAutoSelection = true;
            this._CurrentPostion.Clear();
            this._MovingPosition1.Clear();
            this._MovingPosition2.Clear();
            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
            this.CurrentPostion.Clear();
            this.MovingPosition1.Clear();
            this.MovingPosition2.Clear();
            if (this.CurrentUIMode == UIMode.SwapView)
                this.CurrentPositionSwapChanged(this._CurrentPostion);
            if (this.CurrentUIMode == UIMode.ContainerMoving)
            {
                this.CurrentPositionMovingChanged1(this._MovingPosition1);
                this.CurrentPositionMovingChanged2(this._MovingPosition2);
            }
            else
                this.CurrentPositionChanged(this._CurrentPostion);

            DataMgr.Singleton.List_MachineofPool.Clear();
        }

        // Add UIContent Object to UIComponent Dictionary
        public void AddUIComponent(string keyName, Object uiObj)
        {
            if (!uiComponentTable.ContainsKey(keyName))
                uiComponentTable.Add(keyName, uiObj);
        }

        // Get UIComponent Object by Keyname
        public Object GetUIComponent(string keyName)
        {
            return uiComponentTable[keyName];
        }

        // Remove UIComponent Object by Keyname
        public void RemoveUIComponent(string keyName)
        {
            uiComponentTable.Remove(keyName);
        }

        //-----------------------------------------------------------------
        //- Application Common Methods Section
        //-----------------------------------------------------------------
        #region [ Application Common Methods ]
        public static void APP_CloseApp(Boolean restart = false)
        {
            //PresentationMgr.AppWin.SaveAppCfg();

            // Stop BlockBay Information Retrieving Thread
            PresentationMgr.Singleton.StopUpdateThread();
            // Release Geometry Resource
            //VMT_Data_JAT2.Marshalling.Geometry.ReleaseGeometry();

            foreach (var process in System.Diagnostics.Process.GetProcessesByName(MainWindow.SIEMENS_PROCESSNAME))
                process.Kill();

            if (restart)
                System.Windows.Forms.Application.Restart();

            PresentationMgr.AppWin.SaveAppCfg();

            App.Current.Shutdown();
            Environment.Exit(0);
        }

        public static void FileTouchEvent_RunUpdate()
        {
            //--------------------------------------------------------------
            //- Save Touch Event for CLTAgent to run live update
            String touchCmd = "RUNLIVEUPDATE";

            String TouchFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            TouchFile += @"\CLTConnect";
            System.IO.Directory.CreateDirectory(TouchFile);
            TouchFile += @"\CLTAgent.touch";

            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(TouchFile);
            file.WriteLine(touchCmd);

            file.Close();
        }

        public class SingleShot
        {
            public delegate void SingleShotCallBack();
            private SingleShotCallBack m_callBackFunction;

            public SingleShot(int intervalMilliseconds, SingleShotCallBack CallBackFunction)
            {
                System.Timers.Timer m_timer = new System.Timers.Timer(intervalMilliseconds);
                m_callBackFunction = CallBackFunction;
                m_timer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsedFunc);
                // m_timer.Interval = intervalMilliseconds;
                m_timer.AutoReset = false;
                m_timer.Start();
            }

            ~SingleShot() { }

            public void timerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
            {
                // m_timer.Stop();
                // m_timer.Dispose();            
                m_callBackFunction();
            }
        }

        #endregion

        //-----------------------------------------------------------------
        //- Application Work Thread Section
        //-----------------------------------------------------------------
        #region [ Application Work Thread ]
        private Thread _threadUpdateBayInventory = null;

        private bool bStopUpdateThread = false;
        public AutoResetEvent ThreadResetEvent = new AutoResetEvent(false);
        public int RequestBayInventory = 0;

        public static void StartNetworkCheck()
        {
            PresentationMgr.AppWin.NotifyWIFIStatus(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() ? 1 : 0);

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged +=
                new NetworkAvailabilityChangedEventHandler(NetworkAvailabilityChanged);
        }

        private static void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            try
            {
                VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus(e.IsAvailable ? 1 : 0);

                if (!e.IsAvailable)
                    VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive(String.Empty);
                //else
                //{
                //    new System.Threading.Timer(new System.Threading.TimerCallback(
                //    delegate
                //    {
                //        VMT_Data_JAT2.VMT_DataMgr_Common.KeepAlive_Ask();
                //    }), null, 1000, System.Threading.Timeout.Infinite);
                //}
            }
            catch (Exception)
            {
            }
        }

        public void StartUpdateThread()
        {
            try
            {
                // Start Preffering Thread
                if (_threadUpdateBayInventory == null)
                {
                    _threadUpdateBayInventory = new Thread(new ThreadStart(ThreadUpdateData));
                    _threadUpdateBayInventory.Priority = ThreadPriority.Normal;
                    //_threadUpdateBayInventory.IsBackground = true;
                }
                //TimerdStart();
                bStopUpdateThread = false;

                if (!_threadUpdateBayInventory.IsAlive)
                    _threadUpdateBayInventory.Start();
            }
            catch (Exception)
            {
            }
        }

        public void StopUpdateThread()
        {
            try
            {
                if (_threadUpdateBayInventory != null)
                {
                    _threadUpdateBayInventory.Abort();
                    _threadUpdateBayInventory = null;
                }
                //TimerdStop();
                this.ThreadTimerStop();
                this.bStopUpdateThread = true;
                //ThreadResetEvent.Reset();
            }
            catch (Exception)
            {
            }
        }

        private Int32 _intervalMilliseconds = 3000; //7000; //20200720 change refresh step time
        private System.Timers.Timer _timer = null;
        public void ThreadTimerStart(Boolean isInstant = false)
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer(_intervalMilliseconds);
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timerElapsedFunc);
                _timer.AutoReset = true;

                if (isInstant == true)
                    this.ThreadResetEvent.Set();
            }
            else if (isInstant == true)
            {
                this.ThreadResetEvent.Set();
                return;
            }

            _timer.Start();
        }

        public void ThreadTimerStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                //_timer = null;
            }
        }

        public Boolean IsEnabledInventoryThread()
        {
            if (_timer != null)
                return _timer.Enabled;
            return false;
        }

        public void _timerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            ThreadResetEvent.Set();
        }

        public void ThreadUpdateData()
        {
            do
            {
                ThreadResetEvent.WaitOne(System.Threading.Timeout.Infinite);
                if (!String.IsNullOrEmpty(this.CurrentBlock) &&
                    (this._currentUIMode == UIMode.MainView || this._currentUIMode == UIMode.SwapView))
                {
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(this.CurrentBlock);
                    this.SendGetInventoryAsk_background();
                }
                //Thread.Sleep(5000);                
            }
            while (!bStopUpdateThread);
        }
        #endregion [ Applicatin Work Thread ]


        //-----------------------------------------------------------------
        //- Geometry Information Update Event Methods Section
        //-----------------------------------------------------------------
        #region [ Geometry Information Update Event Methods ]
        //public void GEO_SetGeoPoint(VMT_Data_JAT2.Marshalling.Geometry.sGeoPoint pt)
        //{
        //    VMT_Data_JAT2.Marshalling.Geometry.sBayPow pow = new VMT_Data_JAT2.Marshalling.Geometry.sBayPow();

        //    //-------------------------------------------------------------
        //    //- Update Yard View
        //    try
        //    {
        //        IntPtr ptr = VMT_Data_JAT2.Marshalling.Geometry.SetCurrentPos(pt.lo, pt.la);

        //        if (ptr != null && ptr != IntPtr.Zero)
        //        {
        //            pow = (VMT_Data_JAT2.Marshalling.Geometry.sBayPow)Marshal.PtrToStructure(ptr, typeof(VMT_Data_JAT2.Marshalling.Geometry.sBayPow));

        //            if (this.CurrentBlock == null)
        //                YDV_UpdateYard(pow);
        //            else if (!this.CurrentBlock.Equals(pow.m_szBlock))
        //                YDV_UpdateYard(pow);
        //        }
        //        else
        //        {
        //            pow = this._CurrentPow;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        String errMsg = ex.Message;
        //        //MessageBox.Show(ex.Message);
        //        return;
        //    }

        //    //-------------------------------------------------------------
        //    //- Update Moving View
        //    PresentationMgr.YardView.TextBlock_RMG_Position.Text = string.Format("LA:{0:F7}, LO:{1:F7}", pt.la, pt.lo);
        //    PresentationMgr.YardView.TextBlock_GantryBayNumber.Text = pow.m_szBayEst;

        //    double gentryXPos = PresentationMgr.YardView.UC_BlockControl_L.ActualWidth +
        //                        50 +  // Block to Block Padding Width is 50 Pixel
        //                        (((Util.Parse<Double>(pow.m_szBayEst) / 2) - 0.5) * 15) + //BayMapItemControl Width is 15Pixel
        //                        15 / 2; // Shift Single Bay Center of Width

        //    double xOffset = (PresentationMgr.YardView.Canvas_Blocks.ActualWidth / 2) - gentryXPos;

        //    PresentationMgr.YardView.Stack_Blocks.SetValue(Canvas.LeftProperty, xOffset);

        //    //-------------------------------------------------------------
        //    //- Update JobList Control
        //    if (this.CurrentBayEst == null || !this.CurrentBayEst.Equals(pow.m_szBayEst))
        //    {
        //        JOB_Reload(pow);
        //    }

        //    //-------------------------------------------------------------
        //    //- Update JobBanner Control

        //    // Set Current POW
        //    this.CurrentPow = pow;
        //}

        public void SetGeoPosition(VMT_Data_JAT2.Marshalling.Geometry.sPosition cp)
        {
            //if (this.CurrentBlock == null ||
            //    !this.CurrentBlock.Equals(cp.m_cBlock))
            //    YDV_UpdateYard(cp);

            //-------------------------------------------------------------
            //- Update Moving View
            //PresentationMgr.YardView.TextBlock_GantryBayNumber.Text = cp.m_cBay;
            //double gentryXPos = PresentationMgr.YardView.UC_BlockControl_L.ActualWidth +
            //                    50 +  // Block to Block Padding Width is 50 Pixel
            //                    (((Util.Parse<Double>(cp.m_cBay) / 2) - 0.5) * 15) + //BayMapItemControl Width is 15Pixel
            //                    15 / 2; // Shift Single Bay Center of Width

            //double xOffset = (PresentationMgr.YardView.Canvas_Blocks.ActualWidth / 2) - gentryXPos;

            //PresentationMgr.YardView.Stack_Blocks.SetValue(Canvas.LeftProperty, xOffset);
            //-------------------------------------------------------------
            //- Update JobList Control
            if (this.CurrentBay == null || !this.CurrentBay.Equals(cp.m_cBay))
            {
                JOB_Reload(cp);
            }

            //-------------------------------------------------------------
            //- Update JobBanner Control

            // Set Current POW
            this.CurrentPostion = cp;
        }
        #endregion [ Geometry Information Update Event Methods ]


        //-----------------------------------------------------------------
        //- Job Order Management
        //-----------------------------------------------------------------
        #region [ Job Order Management Method ]

        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder JOB_Get(String jobKey)
        {
            return DataMgr.Singleton.GetJobOrder(jobKey);
        }

        public VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory CONTVirtual_Get(String cntrNo)
        {
            return DataMgr.Singleton.GetVirtualContByCntrNo(cntrNo);
        }

        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder JOB_GetByCntrNo(String cntrNo)
        {
            return DataMgr.Singleton.GetJobOrderByCntrNo(cntrNo);
        }

        public void JOB_Sort()
        {
            if (DataMgr.Singleton.List_JobOrder.Count <= 1)
                return;


            Comparison<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> comparison = VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder.Compare;
            try
            {
                DataMgr.Singleton.List_JobOrder.Sort(comparison);
            }
            catch (Exception ex)
            {

            }

            this.JOB_TwinSort();
        }

        private void JOB_TwinSort()
        {
            int ListCount = DataMgr.Singleton.List_JobOrder.Count;
            VD_Common_JobOrder currentJob = null;
            if (ListCount <= 1)
                return;

            for (int i = 0; i < ListCount; i++)
            {
                currentJob = DataMgr.Singleton.List_JobOrder[i];
                if (currentJob != null && currentJob.type != null &&
                    currentJob.type.twinTandemFlg.Equals("W"))
                {
                    var twinJob = DataMgr.Singleton.List_JobOrder.FirstOrDefault(x => !currentJob.jobKey.Equals(x.jobKey) && "W".Equals(x.type.twinTandemFlg) && currentJob.type.twinTandumKey.Equals(x.type.twinTandumKey));
                    if (twinJob != null)
                    {
                        if (currentJob.type.jobFlagInfo.Equals("A") && twinJob.type.jobFlagInfo.Equals("F")) //Found the position After job first
                            continue;
                        if (DataMgr.Singleton.List_JobOrder.IndexOf(currentJob) > DataMgr.Singleton.List_JobOrder.IndexOf(twinJob)) //The current job has greater index than twin job
                            i--;
                        DataMgr.Singleton.List_JobOrder.Remove(twinJob);
                        DataMgr.Singleton.List_JobOrder.Insert(i + 1, twinJob);
                        i++;
                    }
                }
                //else if (currentJob != null && currentJob.partnerMchn != null &&
                //         !String.IsNullOrEmpty(currentJob.partnerMchn.mchnId))
                //{
                //    var twinJob = DataMgr.Singleton.List_JobOrder.FirstOrDefault(x => !currentJob.jobKey.Equals(x.jobKey) && currentJob.partnerMchn.mchnId.Equals(x.partnerMchn.mchnId));
                //    if (twinJob != null)
                //    {
                //        //if (currentJob.type.jobFlagInfo.Equals("A") && twinJob.type.jobFlagInfo.Equals("F")) //Found the position After job first
                //        //    continue;
                //        //if (DataMgr.Singleton.List_JobOrder.IndexOf(currentJob) > DataMgr.Singleton.List_JobOrder.IndexOf(twinJob)) //The current job has greater index than twin job
                //        //    i--;
                //        DataMgr.Singleton.List_JobOrder.Remove(twinJob);
                //        DataMgr.Singleton.List_JobOrder.Insert(i + 1, twinJob);
                //        i++;
                //    }
                //}
            }
        }
       
        public int JOB_Add(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            //-----------------------------------------------
            //- Add Primary JobData List
            if (DataMgr.Singleton.IsContain(jobOrder))
                this.JOB_Remove(jobOrder);

            // 냉동 컨테이너 display config
            if (PresentationMgr.ShowUnplugReeferOnly)
            {
                if ((jobOrder.cntr.cntrCgoTp == "R" || jobOrder.cntr.cntrCgoTp == "RH" || jobOrder.cntr.cntrCgoTp == "RD") &&
                    (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "MO" ||
                    jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                    !(jobOrder.reefer.plugCd == "POC" || jobOrder.reefer.plugCd == "ROC" || jobOrder.reefer.plugCd == "PIW" || jobOrder.reefer.plugCd == "RIW"))
                    return DataMgr.Singleton.List_JobOrder.Count;
            }

            if (PresentationMgr.ShowDispatchedLoadingJobOnly)
            {
                String ytAssigned = getValueFromConfig("MACHINE", "ytAssigned");
                if (ytAssigned.Equals("Y"))

                    if ((jobOrder.type.jobTp.Equals("LD") || jobOrder.type.jobTp.Equals("MO")) &&
                        (jobOrder.partnerMchn == null || String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId)))
                        return DataMgr.Singleton.List_JobOrder.Count;
            }

            if (jobOrder.type.jobStatus.Equals("P"))
            {
                if (jobOrder.workingMchn.mchnId.Equals(UserInfo.gMchnID))
                    PresentationMgr.Singleton.ProcessingJobKey = jobOrder.jobKey;
                else
                    return DataMgr.Singleton.List_JobOrder.Count;
            }

            var swapInfo = PresentationMgr.GetLoadingSwapInfo(jobOrder.jobKey);
            if (swapInfo != null)
            {
                jobOrder.partnerMchn.mchnId = swapInfo.MachineID;
                jobOrder.type.jobFlagInfo = swapInfo.ChssPsn;
            }

            DataMgr.Singleton.JobOrder_Add(jobOrder);

            /* MainWindows로 이동
            //-----------------------------------------------
            // Add JobOrder into the proper UI
            // ListBox_Job
            //BlockSelectionView blockView = null;
            JobList jobList = null;
            if (PresentationMgr.MainView.UC_BlockJobView.Visibility == Visibility.Visible)
            {
                //blockView = PresentationMgr.JobRelationView.UC_BlockSelectionView;
                jobList = PresentationMgr.JobRelationView.UC_JobList;
            }
            else
            {
                //blockView = PresentationMgr.MainView.UC_BlockSelectionView;
                jobList = PresentationMgr.MainView.UC_JobList;
            }
            //JL_SearchNRefresh(jobList, SelectBlckName, jobList.CurrentPageIndex);
            */

            //// Wrap_BlockView
            //BlockJobControl bjControl = null;
            //if (DataMgr.Singleton.Dic_BlockJobControl.TryGetValue(jobOrder.locWorking.blck, out bjControl))
            //{
            //    // exist
            //    bjControl.AddJobOrder(jobOrder);
            //}
            //else
            //{
            //    // no exist
            //    bjControl = new BlockJobControl();
            //    blockView.Wrap_BlockSelectionView.Children.Add(bjControl);
            //    DataMgr.Singleton.Dic_BlockJobControl.Add(jobOrder.locWorking.blck, bjControl);
            //    bjControl.AddJobOrder(jobOrder);
            //}

            //// Upate Bay Inventory Data
            //JOB_UpdateBayInventory();

            // Update JobCount
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;

            return DataMgr.Singleton.List_JobOrder.Count;
        }

        public int JOB_Remove(String jobKey, Boolean bJobDone = false)
        {
            if (jobKey.Equals(RMG.RMG_Member.Singleton.TargetJobKey))
            {
                // Banner Clear
                //PresentationMgr.Singleton.JBN_SetTargetJob_Clear();
            }

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
            // Wrap_BlockView
            BlockJobControl bjControl = null;
            if (DataMgr.Singleton.Dic_BlockJobControl.TryGetValue(jobOrder.locWorking.blck, out bjControl))
            {
                // exist
                bjControl.RemoveJobOrder(jobOrder);

                if (bjControl.nCount == 0)
                {
                    DataMgr.Singleton.Dic_BlockJobControl.Remove(jobOrder.locWorking.blck);
                }
            }
            else
            {
                // no exist
                // Error
            }

            if (bJobDone)
            {
            }
            else
            {
            }

            DataMgr.Singleton.JobOrder_Del(jobKey);

            // Update JobCount
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;

            return DataMgr.Singleton.List_JobOrder.Count;
        }

        public int JOB_Remove(VMT_Data_JAT2.Objects.Common.VD_Common_JobKey jobKey) // Job Cancel
        {
            return this.JOB_Remove(jobKey.jobKey);
        }

        public int JOB_Remove(VMT_Data_JAT2.Objects.Common.VD_Common_JobDone jobDone) // Job Done
        {
            return this.JOB_Remove(jobDone.jobKey, true);
        }

        public int JOB_Remove(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if (jobOrder.type.jobStatus.Equals("B"))
                return this.JOB_Remove(jobOrder.jobKey, false);
            else
                return this.JOB_Remove(jobOrder.jobKey, false);
        }

        public void JOB_Clear()
        {
            DataMgr.Singleton.JobOrder_Clear();

            //-----------------------------------------------
            // Remove JobOrder into the proper UI
            // ListBox_Job
            //PresentationMgr.MainView.UC_JobList.ListBox_Job.Items.Clear();

            // Wrap_BlockView
            DataMgr.Singleton.Dic_BlockJobControl.Clear();
            //PresentationMgr.MainView.UC_BlockSelectionView.Wrap_BlockSelectionView.Children.Clear();
            //PresentationMgr.MainView.UC_BaySelectionView.Wrap_BaySelectionView.Children.Clear();

            // Update BlockJobContainer 
            //YDV_UpdateBlockJobContainer();

            // Update JobCount
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;
        }

        //public void JOB_Reload(VMT_Data_JAT2.Marshalling.Geometry.sBayPow pow)
        //{
        //    PresentationMgr.MovingView.UC_JobList_Left.ListBox_Job.Items.Clear();
        //    PresentationMgr.MovingView.UC_JobList_Right.ListBox_Job.Items.Clear();

        //    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
        //    {
        //        JobListItem item = new JobListItem();
        //        item.SetJobInfo(jobOrder.jobKey);

        //        // Add Right Side
        //        if (Util.Parse<Int32>(jobOrder.locWorking.bay) >= Util.Parse<Int32>(pow.m_szBayEst))
        //            PresentationMgr.MovingView.UC_JobList_Right.ListBox_Job.Items.Add(item);
        //        // Add Left Side
        //        else
        //            PresentationMgr.MovingView.UC_JobList_Left.ListBox_Job.Items.Add(item);
        //    }

        //    // Update YardContainer 
        //    YDV_UpdateYardJobContainer();

        //    // Update JobCount
        //    this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;
        //}

        public void JOB_Reload(VMT_Data_JAT2.Marshalling.Geometry.sPosition cp)
        {
            // Update JobCount
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;
        }

        public List<String> JOB_Sibling(String JobKey)
        {
            return DataMgr.Singleton.SiblingJobKey(JobKey);
        }

        public void JOB_UpdateBayInventory()
        {
            //for (int i = 0; i < DataMgr.Singleton.List_JobOrder.Count; i++ )
            //{
            //    DataMgr.Singleton.List_JobOrder[i].
            //}
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForBlock(String block)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                var location = jobOrder.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                    jobOrder.type.jobStatus != "P")
                    location = jobOrder.locFrom;
                else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;

                if (0 == String.Compare(block, location.blck, true))
                    jobOrderList.Add(jobOrder);
            }

            return jobOrderList;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForBlockBay(String block, String bay)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            var isValidBlock = PresentationMgr.IsValidBlock(block);
            var isValidBay = PresentationMgr.IsValidBay(bay);

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                var location = jobOrder.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                    jobOrder.type.jobStatus != "P")
                    location = jobOrder.locFrom;
                else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;

                if ((!isValidBlock || 0 == String.Compare(block, location.blck, true)) &&
                    (!isValidBay || 0 == String.Compare(bay, location.bay, true))
                    )
                {
                    jobOrderList.Add(jobOrder);
                    //break;          // 테스트를 위해 현재는 하나의 최상위 잡만 전달한다
                }
            }

            return jobOrderList;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForLocation(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location loc)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            var isValidBlock = PresentationMgr.IsValidBlock(loc.blck);
            var isValidBay = PresentationMgr.IsValidBay(loc.bay);
            var isValidRow = PresentationMgr.IsValidRow(loc.row);
            var isValidTier = PresentationMgr.IsValidTier(loc.tier);

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                var location = jobOrder.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                    jobOrder.type.jobStatus != "P")
                    location = jobOrder.locFrom;
                else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;

                if ((!isValidBlock || loc.blck.Equals(location.blck)) &&
                    (!isValidBay || loc.bay.Equals(location.bay)) &&
                    (!isValidRow || loc.row.Equals(location.row)) &&
                    (!isValidTier || loc.tier.Equals(location.tier))
                    )
                    jobOrderList.Add(jobOrder);
            }
            return jobOrderList;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForLocation(String blck, String bay, String row, String tier)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            var isValidBlock = PresentationMgr.IsValidBlock(blck);
            var isValidBay = PresentationMgr.IsValidBay(bay);
            var isValidRow = PresentationMgr.IsValidRow(row);
            var isValidTier = PresentationMgr.IsValidTier(tier);

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                var location = jobOrder.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                    jobOrder.type.jobStatus != "P")
                    location = jobOrder.locFrom;
                else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;

                if ((!isValidBlock || blck.Equals(location.blck)) &&
                    (!isValidBay || bay.Equals(location.bay)) &&
                    (!isValidRow || row.Equals(location.row)) &&
                    (!isValidTier || tier.Equals(location.tier))
                    )
                    jobOrderList.Add(jobOrder);
            }
            return jobOrderList;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForYardLocation(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location loc)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            var isValidBlock = PresentationMgr.IsValidBlock(loc.blck);
            var isValidBay = PresentationMgr.IsValidBay(loc.bay);
            var isValidRow = PresentationMgr.IsValidRow(loc.row);
            var isValidTier = PresentationMgr.IsValidTier(loc.tier);

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                var location = jobOrder.locWorking;
                if (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH")
                {
                    if (PresentationMgr.UseFromLocationForRehandling == true)
                        location = jobOrder.locFrom;
                }
                else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;
                else //if (jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "MI" || jobOrder.type.jobTp == "GC" || jobOrder.type.jobTp == "LC") // 반입 job은 제외
                    continue;

                if ((!isValidBlock || loc.blck.Equals(location.blck)) &&
                    (!isValidBay || loc.bay.Equals(location.bay)) &&
                    (!isValidRow || loc.row.Equals(location.row)) &&
                    (!isValidTier || loc.tier.Equals(location.tier))
                    )
                    jobOrderList.Add(jobOrder);
            }
            return jobOrderList;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForYardPlan(String block, String bay, int spreadSize, Boolean isTwin = false)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            // 2017-02-15 요청사항 : 해당 Machine 이 할당 받은 모든 DS / MI / GI / GC / LC Job 이 Display
            //var isValidBlock = PresentationMgr.IsValidBlock(block);
            //var isValidBay = PresentationMgr.IsValidBay(bay);

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                //var location = jobOrder.locWorking;                
                //if ((!isValidBlock || 0 == String.Compare(block, location.blck, true)) &&
                //    (!isValidBay || 0 == String.Compare(bay, location.bay, true))
                //    )
                {
                    if (!(jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "MI" ||
                        jobOrder.type.jobTp == "GC" || jobOrder.type.jobTp == "LC")
                        //|| (Convert.ToString(spreadSize).First() != jobOrder.cntr.cntrLen.First()) 
                        //|| (isTwin && String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                        )
                        continue;

                    jobOrderList.Add(jobOrder);
                }
            }

            return jobOrderList;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> JOB_GetForContainerNo(String cntrNo)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> jobOrderList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            foreach (var jobOrder in DataMgr.Singleton.List_JobOrder)
            {
                if (jobOrder.cntr.cntrNo == cntrNo)
                    jobOrderList.Add(jobOrder);
            }
            return jobOrderList;
        }

        public List<String> GetPowInMachineLIst(Boolean isOTR = false)
        {
            var retList = new List<String>();

            var powJobs = from job in DataMgr.Singleton.List_JobOrder
                          where job.type.isArrivedItv == true
                          select job;

            if (powJobs != null)
            {
                if (isOTR)
                {
                    foreach (var j in powJobs)
                    {
                        if (j.type.jobTp == "GI" || j.type.jobTp == "GO" || j.type.jobTp == "GC")
                            retList.Add(j.partnerMchn.mchnId);
                    }
                }
                else
                {
                    foreach (var j in powJobs)
                    {
                        if (j.type.jobTp != "GI" && j.type.jobTp != "GO" && j.type.jobTp != "GC")
                            retList.Add(j.partnerMchn.mchnId);
                    }
                }
            }

            return retList;
        }

        public static Boolean UseInventoryMultiAPI = true;
        public void SendGetInventoryAsk_background(Boolean needFrontBay = true)
        {
            this.ThreadTimerStop();
            //int bayVal;
            //bool isInt = int.TryParse(this.CurrentBay, out bayVal);
            var valueList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>();
            //if (!isInt)
            //{
            //    valueList.Add(
            //                new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
            //                {
            //                    blck = this.CurrentBlock,
            //                    bay = "",
            //                });
            //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti_Ask(valueList);
            //    return;
            //}

            var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            location.blck = this.CurrentBlock;
            if (!String.IsNullOrEmpty(this.CurrentBay))
            {

                var intBay = Convert.ToInt32(BayRemoveChars(this.CurrentBay));
                if (needFrontBay &&
                    intBay > 2 && intBay % 2 != 0)
                {
                    var prevBay = Convert.ToString(intBay - 2);
                    if (prevBay.Length <= 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            prevBay = "0" + prevBay;
                        else
                            prevBay = cha + prevBay;
                    }
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(this.CurrentBlock) &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock].DicBay != null &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock].DicBay.ContainsKey(prevBay))
                    {
                        location.bay = prevBay;

                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, location.bay);
                    }
                    else
                    {
                        needFrontBay = false;
                        location.bay = this.CurrentBay;
                    }
                }
                else
                {
                    needFrontBay = false;
                    location.bay = this.CurrentBay;
                }

                DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(this.CurrentBay));
            }
            else
            {
                location.bay = String.Empty;
                needFrontBay = false;
            }

            valueList.Add(location);

            DataMgr.Singleton.Inventory_AddItemToClear(location.blck, this.CurrentBay);

            if (needFrontBay)
            {
                if (//PresentationMgr.UseInventoryMultiAPI == true &&
                    valueList != null && valueList.Count > 0)
                {
                    valueList.Add(
                        new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                        {
                            blck = location.blck,
                            bay = this.CurrentBay,
                        });
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListBackgroundMulti_Ask(valueList);
                }
                else
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListBackgroundMulti_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListBackgroundEx_Ask(location);
            }
            else
                VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListBackgroundMulti_Ask(valueList);
            //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListBackground_Ask(location);

            location = null;
            if (valueList != null)
            {
                valueList.Clear();
                valueList = null;
            }
        }

        public void SendGetInventoryAsk(String block, String bay, Boolean needFrontBay = true, Boolean needUIUpdate = true, Boolean progressBar = true)
        {
            this.ThreadTimerStop();
            //if (needUIUpdate)
            //    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = false;

            var valueList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>();
            var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            location.blck = block;

            if (!String.IsNullOrEmpty(bay))
            {
                var intBay = Convert.ToInt32(BayRemoveChars(bay));
                if (needFrontBay &&
                    intBay > 2 && intBay % 2 != 0)
                {
                    var prevBay = Convert.ToString(intBay - 2);
                    if (prevBay.Length <= 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            prevBay = "0" + prevBay;
                        else
                            prevBay = cha + prevBay;
                    }
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay != null &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay.ContainsKey(prevBay))
                    {
                        location.bay = prevBay;

                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, location.bay);
                    }
                    else
                    {
                        needFrontBay = false;
                        location.bay = bay;
                    }
                }
                else
                {
                    needFrontBay = false;
                    location.bay = bay;
                }

                DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(bay));
            }
            else
            {
                location.bay = String.Empty;
                needFrontBay = false;
            }

            valueList.Add(location);

            DataMgr.Singleton.Inventory_AddItemToClear(location.blck, bay);

            if (needFrontBay)
            {
                if (needUIUpdate)
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });

                        SaveLog("CALL_API_INVENTORY_" + CurrentBlock + "_" + CurrentBay);
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListEx_Ask(location);                        
                }
                else
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataEx_Ask(location);                        
                }
            }
            else
            {
                if (needUIUpdate)
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryList_Ask(location);                    
                else
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListData_Ask(location);                    
            }
            //if(progressBar)
            //    PresentationMgr.AppWin.ShowProgressBar(0);
            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
            , "GetInventoryList_Ask"), location);

            location = null;
            if (valueList != null)
            {
                valueList.Clear();
                valueList = null;
            }
        }

        public void SendGetInventoryList4Multi_Sync_Ask(String block, String bay)
        {
            var valueList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>();
            var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            location.blck = block;

            if (!String.IsNullOrEmpty(bay))
            {
                var intBay = Convert.ToInt32(BayRemoveChars(bay));
                if (intBay > 2 && intBay % 2 != 0)
                {
                    var prevBay = Convert.ToString(intBay - 2);
                    if (prevBay.Length <= 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            prevBay = "0" + prevBay;
                        else
                            prevBay = cha + prevBay;
                    }
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay != null &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay.ContainsKey(prevBay))
                    {
                        location.bay = prevBay;

                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, location.bay);
                    }
                    else
                    {
                        location.bay = bay;
                    }
                }
                else
                {
                    location.bay = bay;
                }

                DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(bay));
            }
            else
            {
                location.bay = String.Empty;
            }

            valueList.Add(location);

            DataMgr.Singleton.Inventory_AddItemToClear(location.blck, bay);

            if (valueList != null && valueList.Count > 0)
            {
                valueList.Add(
                    new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                    {
                        blck = location.blck,
                        bay = this.CurrentBay,
                    });
            }

            VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryList4Multi_Sync_Ask(valueList);

            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
            , "GetInventoryList_Ask"), location);

            location = null;
            if (valueList != null)
            {
                valueList.Clear();
                valueList = null;
            }
        }

        public void SendGetInventory1Ask(String block, String bay, Boolean needFrontBay = true, Boolean needUIUpdate = true)
        {
            this.ThreadTimerStop();
            //if (needUIUpdate)
            //    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = false;

            var valueList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>();
            var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            location.blck = block;

            if (!String.IsNullOrEmpty(bay))
            {
                var intBay = Convert.ToInt32(BayRemoveChars(bay));
                if (needFrontBay &&
                    intBay > 2 && intBay % 2 != 0)
                {
                    var prevBay = Convert.ToString(intBay - 2);
                    if (prevBay.Length <= 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            prevBay = "0" + prevBay;
                        else
                            prevBay = cha + prevBay;
                    }
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay != null &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay.ContainsKey(prevBay))
                    {
                        location.bay = Convert.ToString(intBay - 2);
                        if (location.bay.Length <= 1)
                            location.bay = "0" + location.bay;

                        DataMgr.Singleton.Inventory_AddItemToClear1(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        DataMgr.Singleton.Inventory_AddItemToClear1(location.blck, location.bay);
                    }
                    else
                    {
                        needFrontBay = false;
                        location.bay = bay;
                    }
                }
                else
                {
                    needFrontBay = false;
                    location.bay = bay;
                }

                DataMgr.Singleton.Inventory_AddItemToClear1(location.blck, PresentationMgr.GetRearEvenBay(bay));
            }
            else
            {
                location.bay = String.Empty;
                needFrontBay = false;
            }

            valueList.Add(location);

            DataMgr.Singleton.Inventory_AddItemToClear1(location.blck, bay);

            if (needFrontBay)
            {
                if (needUIUpdate)
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti1_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti1_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListEx_Ask(location);                        
                }
                else
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti1_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti1_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataEx_Ask(location);                        
                }
            }
            else
            {
                if (needUIUpdate)
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti1_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryList_Ask(location);                    
                else
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti1_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListData_Ask(location);                    
            }

            PresentationMgr.AppWin.ShowProgressBar(0);
            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
            , "GetInventoryList_Ask"), location);

            location = null;
            if (valueList != null)
            {
                valueList.Clear();
                valueList = null;
            }
        }

        public void SendGetInventory2Ask(String block, String bay, Boolean needFrontBay = true, Boolean needUIUpdate = true)
        {
            this.ThreadTimerStop();
            //if (needUIUpdate)
            //    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = false;

            var valueList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>();
            var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            location.blck = block;

            if (!String.IsNullOrEmpty(bay))
            {
                var intBay = Convert.ToInt32(BayRemoveChars(bay));
                if (needFrontBay &&
                    intBay > 2 && intBay % 2 != 0)
                {
                    var prevBay = Convert.ToString(intBay - 2);
                    if (prevBay.Length <= 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            prevBay = "0" + prevBay;
                        else
                            prevBay = cha + prevBay;
                    }
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay != null &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay.ContainsKey(prevBay))
                    {
                        location.bay = prevBay;

                        DataMgr.Singleton.Inventory_AddItemToClear2(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        DataMgr.Singleton.Inventory_AddItemToClear2(location.blck, location.bay);
                    }
                    else
                    {
                        needFrontBay = false;
                        location.bay = bay;
                    }
                }
                else
                {
                    needFrontBay = false;
                    location.bay = bay;
                }

                DataMgr.Singleton.Inventory_AddItemToClear2(location.blck, PresentationMgr.GetRearEvenBay(bay));
            }
            else
            {
                location.bay = String.Empty;
                needFrontBay = false;
            }

            valueList.Add(location);

            DataMgr.Singleton.Inventory_AddItemToClear2(location.blck, bay);

            if (needFrontBay)
            {
                if (needUIUpdate)
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti2_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti2_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListEx_Ask(location);                        
                }
                else
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti2_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti2_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataEx_Ask(location);                        
                }
            }
            else
            {
                if (needUIUpdate)
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMulti2_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryList_Ask(location);                    
                else
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMulti2_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListData_Ask(location);                    
            }

            PresentationMgr.AppWin.ShowProgressBar(0);
            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
            , "GetInventoryList_Ask"), location);

            location = null;
            if (valueList != null)
            {
                valueList.Clear();
                valueList = null;
            }
        }

        public void SendGetInventorySwapAsk(String block, String bay, Boolean needFrontBay = true, Boolean needUIUpdate = true)
        {
            this.ThreadTimerStop();
            //if (needUIUpdate)
            //    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = false;

            var valueList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>();
            var location = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            location.blck = block;

            if (!String.IsNullOrEmpty(bay))
            {
                var intBay = Convert.ToInt32(BayRemoveChars(bay));
                if (needFrontBay &&
                    intBay > 2 && intBay % 2 != 0)
                {
                    var prevBay = Convert.ToString(intBay - 2);
                    if (prevBay.Length <= 1)
                    {
                        var cha = PresentationMgr.BayRemoveInt(this.CurrentBay);
                        if (String.IsNullOrEmpty(cha))
                            prevBay = "0" + prevBay;
                        else
                            prevBay = cha + prevBay;
                    }
                    if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay != null &&
                        DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].DicBay.ContainsKey(prevBay))
                    {
                        location.bay = prevBay;

                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(location.bay));
                        DataMgr.Singleton.Inventory_AddItemToClear(location.blck, location.bay);
                    }
                    else
                    {
                        needFrontBay = false;
                        location.bay = bay;
                    }
                }
                else
                {
                    needFrontBay = false;
                    location.bay = bay;
                }

                DataMgr.Singleton.Inventory_AddItemToClear(location.blck, PresentationMgr.GetRearEvenBay(bay));
            }
            else
            {
                location.bay = String.Empty;
                needFrontBay = false;
            }

            valueList.Add(location);

            DataMgr.Singleton.Inventory_AddItemToClear(location.blck, bay);

            if (needFrontBay)
            {
                if (needUIUpdate)
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMultiSwap_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMultiSwap_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListEx_Ask(location);                        
                }
                else
                {
                    if (//PresentationMgr.UseInventoryMultiAPI == true &&
                        valueList != null && valueList.Count > 0)
                    {
                        valueList.Add(
                            new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
                            {
                                blck = block,
                                bay = bay,
                            });
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMultiSwap_Ask(valueList);
                    }
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMultiSwap_Ask(valueList);
                    //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataEx_Ask(location);                        
                }
            }
            else
            {
                if (needUIUpdate)
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListMultiSwap_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryList_Ask(location);                    
                else
                    VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListDataMultiSwap_Ask(valueList);
                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetInventoryListData_Ask(location);                    
            }

            PresentationMgr.AppWin.ShowProgressBar(0);
            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
            , "GetInventoryList_Ask"), location);

            location = null;
            if (valueList != null)
            {
                valueList.Clear();
                valueList = null;
            }
        }

        private static VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send GetJobDoneObjFromJobOrder(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job)
        {
            if (job == null)
                return null;

            var jobDoneObj = new VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send();

            jobDoneObj.jobKey = job.jobKey;
            jobDoneObj.WorkingMachineID = RMG.RMG_User.gMchnID;//job.workingMchn.mchnId;
            jobDoneObj.WorkingMachineTP = RMG.RMG_User.gMchnTp;//job.workingMchn.mchnTp;

            jobDoneObj.PartnerMachineID = job.partnerMchn.mchnId;
            jobDoneObj.PartnerMachineTP = job.partnerMchn.mchnTp;

            if (String.IsNullOrEmpty(job.type.ycTwinKey))
                jobDoneObj.sprd.sprdMd = "SINGLE";
            else
                jobDoneObj.sprd.sprdMd = "TWIN";

            var iso = job.cntr.cntrIso;
            if (iso.StartsWith("2"))
                jobDoneObj.sprd.sprdTp = "SINGLE_SPREADER20";
            else
                jobDoneObj.sprd.sprdTp = "SINGLE_SPREADER40";

            jobDoneObj.sprd.sprdSts = "LS_SPREADER_LOCKED";

            jobDoneObj.cntrNo = job.cntr.cntrNo;

            var location = job.locWorking;
            if (PresentationMgr.UseFromLocationForRehandling == true &&
                (job.type.jobTp.Equals("AH") || job.type.jobTp.Equals("RH")))
                location = job.locFrom;
            else if (job.type.jobTp.Equals("LD") || job.type.jobTp.Equals("GO") || job.type.jobTp.Equals("MO"))
                location = string.IsNullOrEmpty(job.locFrom.location) ? job.locWorking : job.locFrom;

            // picked시의 locTp 설정 확인 필요
            if (job.type.jobTp.Equals("DS") || job.type.jobTp.Equals("MI") || job.type.jobTp.Equals("GI") ||
                job.type.jobTp.Equals("LC") || job.type.jobTp.Equals("GC"))
            {
                if (job.partnerMchn.aprchLn.Contains("W"))
                    location.locTp = "TPW";
                else if (job.partnerMchn.aprchLn.Contains("L"))
                    location.locTp = "TPL";
                else
                    location.locTp = "LANE";
            }
            else if (job.type.jobTp.Equals("LD") || job.type.jobTp.Equals("MO") || job.type.jobTp.Equals("GO") ||
                job.type.jobTp.Equals("AH") || job.type.jobTp.Equals("RH"))
                location.locTp = "YARD";
            else
                location.locTp = "YARD";

            jobDoneObj.Loc = location;

            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "SetPickedContainer_Ask"), jobDoneObj);

            return jobDoneObj;
        }

        public void SendPickedContainerAsk(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder target,
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twin)
        {
            var targetObj = PresentationMgr.GetJobDoneObjFromJobOrder(target);
            if (targetObj != null)
            {
                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "SetPickedContainer_Ask left"), targetObj);
                Common.Util.Logger.Log("SetPickedContainer_Ask left jobkey : " + targetObj.jobKey);
            }

            var twinObj = PresentationMgr.GetJobDoneObjFromJobOrder(twin);
            if (twinObj != null)
            {
                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "SetPickedContainer_Ask right"), twinObj);
                Common.Util.Logger.Log("SetPickedContainer_Ask right jobkey : " + twinObj.jobKey);
            }

            if (targetObj != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG.SetPickedContainer_Ask(targetObj);
        }

        public void SendJobDoneAsk(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder, string newYtNo = "", string newPos = "")
        {
            if (jobOrder == null)
                return;

            VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send send = new VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send();
            send.jobKey = jobOrder.jobKey;
            send.WorkingMachineID = RMG.RMG_User.gMchnID;//jobOrder.workingMchn.mchnId;
            send.WorkingMachineTP = RMG.RMG_User.gMchnTp;//jobOrder.workingMchn.mchnTp;

            var swapInfo = PresentationMgr.GetLoadingSwapInfo(jobOrder.jobKey);
            if (swapInfo != null)
            {
                send.PartnerMachineID = swapInfo.MachineID;
                send.PartnerMachineTP = jobOrder.partnerMchn.mchnTp;
                send.positionOnChassis = swapInfo.ChssPsn;
            }
            else
            {
                send.PartnerMachineID = (newYtNo != "") ? newYtNo : jobOrder.partnerMchn.mchnId;
                send.PartnerMachineTP = jobOrder.partnerMchn.mchnTp;
                send.positionOnChassis = (newPos != "") ? newPos : jobOrder.type.jobFlagInfo;
            }

            if (String.IsNullOrEmpty(jobOrder.type.ycTwinKey))
                send.sprd.sprdMd = "SINGLE";
            else
                send.sprd.sprdMd = "TWIN";
            //var flg = jobOrder.type.twinTandemFlg; 
            //if (flg.Equals("S"))
            //    send.sprd.sprdMd = "SINGLE";
            //else if (flg.Equals("W"))
            //    send.sprd.sprdMd = "TWIN";
            //else if (flg.Equals("M"))
            //    send.sprd.sprdMd = "TANDEM";

            var iso = jobOrder.cntr.cntrIso;
            if (iso.StartsWith("2"))
                send.sprd.sprdTp = "SINGLE_SPREADER20";
            else
                send.sprd.sprdTp = "SINGLE_SPREADER40";

            send.sprd.sprdSts = "LS_SPREADER_UNLOCKED";

            send.cntrNo = jobOrder.cntr.cntrNo;

            // 기존의 locTp로 셋팅하지 않음
            //if (String.IsNullOrEmpty(jobOrder.locWorking.locTp))
            {
                if (jobOrder.type.jobTp.Equals("DS") || jobOrder.type.jobTp.Equals("MI") || jobOrder.type.jobTp.Equals("GI") ||
                    jobOrder.type.jobTp.Equals("LC") || jobOrder.type.jobTp.Equals("GC") || jobOrder.type.jobTp.Equals("AH") ||
                        jobOrder.type.jobTp.Equals("RH"))
                    jobOrder.locWorking.locTp = "YARD";
                else if (jobOrder.type.jobTp.Equals("LD") || jobOrder.type.jobTp.Equals("MO") || jobOrder.type.jobTp.Equals("GO"))
                {
                    if (jobOrder.partnerMchn.aprchLn.Contains("W"))
                        jobOrder.locWorking.locTp = "TPW";
                    else if (jobOrder.partnerMchn.aprchLn.Contains("L"))
                        jobOrder.locWorking.locTp = "TPL";
                    else
                    {
                        if (jobOrder.type.jobTp.Equals("GO"))
                            jobOrder.locWorking.locTp = "TPL";
                        else
                            jobOrder.locWorking.locTp = "TPW";
                    }
                }
                else if (!jobOrder.type.jobTp.Equals("NONE"))
                    jobOrder.locWorking.locTp = "YARD";
            }

            send.Loc = jobOrder.locWorking;
            send.isGcBtn = jobOrder.isGcBtn;

            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "SetJobDone_Ask"), send);

            VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobDone_Ask(send);

            //if(progressBar)
            //    PresentationMgr.AppWin.ShowProgressBar(0);
        }
        public void SendJobDoneToDiffLocation(VD_Common_JobOrder jobOrder, VMT_Data_JAT2.Marshalling.Geometry.sPosition pos)
        {
            if (jobOrder == null || jobOrder.type == null || jobOrder.cntr == null)
                return;

            VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send send = new VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send();
            send.jobKey = jobOrder.jobKey;
            send.cntrNo = jobOrder.cntr.cntrNo;
            send.positionOnChassis = jobOrder.type.jobFlagInfo;
            send.isGcBtn = jobOrder.isGcBtn;
            send.Loc.blck = pos.m_cBlock;
            send.Loc.bay = pos.m_cBay;
            send.Loc.row = pos.m_cRow;
            send.Loc.tier = pos.m_cTier;

            if (jobOrder.type.jobTp.Equals("DS") || jobOrder.type.jobTp.Equals("MI") || jobOrder.type.jobTp.Equals("GI") || jobOrder.type.jobTp.Equals("LC") ||
                jobOrder.type.jobTp.Equals("GC") || jobOrder.type.jobTp.Equals("AH") || jobOrder.type.jobTp.Equals("RH"))
                send.Loc.locTp = "YARD";
            else if (jobOrder.type.jobTp.Equals("LD") || jobOrder.type.jobTp.Equals("MO") || jobOrder.type.jobTp.Equals("GO"))
            {
                if (jobOrder.partnerMchn.aprchLn.Contains("W"))
                    send.Loc.locTp = "TPW";
                else if (jobOrder.partnerMchn.aprchLn.Contains("L"))
                    send.Loc.locTp = "TPL";
                else
                {
                    if (jobOrder.type.jobTp.Equals("GO"))
                        send.Loc.locTp = "TPL";
                    else
                        send.Loc.locTp = "TPW";
                }
            }
            else if (!jobOrder.type.jobTp.Equals("NONE"))
                send.Loc.locTp = "YARD";

            InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "SetJobDone_Ask"), send);

            VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobDone_Ask(send);
        }

        #endregion [ Job Order Management Method ]

        //-----------------------------------------------------------------
        //- BlockBay Inventory Management
        //-----------------------------------------------------------------
        #region [ Block Bay Management Method ]
        #region [ Constant Value Defintion ]
        public static Range const_RangeTrolley = new Range(0, 21800);
        public static Range const_RangeHoist = new Range(0, 21000);

        public static Range const_RangeTrolleyCV_MovingView = new Range(33, 99);
        public static double const_RagneTrolleyCV_MovingViewCPS = 115;
        public static Range const_RangeTrolleyCV = new Range(42, 458);
        public static Range const_RangeHoistCV = new Range(30, 408);

        public static double const_BayWork_ContainerWidth = 45; // 44.844 (416/21800 * w 2350)
        public static double const_BayWork_ContainerHeight = 46; // 45.607 (416/21000 * h 2390)
        public static double const_BayWork_ContainerHeightHQ = 51; // 51.427 (416/21000 * h 2695)
        #endregion [ Constant Value Definition ]

        public void InitDataMgr()
        {
            DataMgr.Singleton.InitData();
        }

        //public Dictionary<int, RMG.VD_RMG_InventoryInfo_Receive> GetBlockBayInfo()
        //{
        //    return DataMgr.Singleton.Dic_BlockBayInfo;
        //}

        //public Boolean INV_IsInventory(String block, String bay)
        //{
        //    return DataMgr.Singleton.INV_IsInventory(block, bay);
        //}

        public RMG.VD_RMG_InventoryInfo_Receive GetInventoryInfo()
        {
            return DataMgr.Singleton.InventoryInfo;
        }

        public Boolean INV_IsInventory(String block, String bay)
        {
            return DataMgr.Singleton.INV_hasInventory(block, bay);
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory(String block, String bay)
        {
            return DataMgr.Singleton.INV_GetInventory(block, bay);
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory1(String block, String bay)
        {
            return DataMgr.Singleton.INV_GetInventory1(block, bay);
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory2(String block, String bay)
        {
            return DataMgr.Singleton.INV_GetInventory2(block, bay);
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> GetViewInventorys(String block, String bay)
        {
            var inventory = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

            var inven = this.INV_GetInventory(block, bay);
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            inven = this.INV_GetInventory(block, PresentationMgr.GetFrontEvenBay(bay));
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            inven = this.INV_GetInventory(block, PresentationMgr.GetRearEvenBay(bay));
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            return inventory;
        }

        //INVENTORY AREA
        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> GetViewInventorys1(String block, String bay = "")
        {
            var inventory = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

            var inven = this.INV_GetInventory1(block, bay);
            if (inven != null)
                inventory.AddRange(inven);

            return inventory;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> GetViewInventorys2(String block, String bay)
        {
            var inventory = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

            var inven = this.INV_GetInventory2(block, bay);
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            inven = this.INV_GetInventory2(block, PresentationMgr.GetFrontEvenBay(bay));
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            inven = this.INV_GetInventory2(block, PresentationMgr.GetRearEvenBay(bay));
            if (inven != null)
                inventory.AddRange(inven);//Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(inven));

            return inventory;
        }

        public void SetNoWorkArea(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            if (!DataMgr.Singleton.Dic_NoWorkAreaInfo.ContainsKey(this.CurrentBlock))
                DataMgr.Singleton.Dic_NoWorkAreaInfo.Add(this.CurrentBlock, value);
            else
            {
                DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock].Dispose();
                DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock] = value;
            }
        }

        public void SetNoWorkArea1(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            if (!DataMgr.Singleton.Dic_NoWorkAreaInfo.ContainsKey(this.CurrentBlock1))
                DataMgr.Singleton.Dic_NoWorkAreaInfo.Add(this.CurrentBlock1, value);
            else
            {
                DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock1].Dispose();
                DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock1] = value;
            }
        }

        public void SetNoWorkArea2(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            if (!DataMgr.Singleton.Dic_NoWorkAreaInfo.ContainsKey(this.CurrentBlock2))
                DataMgr.Singleton.Dic_NoWorkAreaInfo.Add(this.CurrentBlock2, value);
            else
            {
                DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock2].Dispose();
                DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock2] = value;
            }
        }

        public void SetNoWorkTier(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            if (!DataMgr.Singleton.Dic_NoWorkTierInfo.ContainsKey(this.CurrentBlock))
                DataMgr.Singleton.Dic_NoWorkTierInfo.Add(this.CurrentBlock, value);
            else
            {
                DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock].Dispose();
                DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock] = value;
            }
        }

        public void SetNoWorkTier1(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            if (!DataMgr.Singleton.Dic_NoWorkTierInfo.ContainsKey(this.CurrentBlock1))
                DataMgr.Singleton.Dic_NoWorkTierInfo.Add(this.CurrentBlock1, value);
            else
            {
                DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock1].Dispose();
                DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock1] = value;
            }
        }

        public void SetNoWorkTier2(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value)
        {
            if (!DataMgr.Singleton.Dic_NoWorkTierInfo.ContainsKey(this.CurrentBlock2))
                DataMgr.Singleton.Dic_NoWorkTierInfo.Add(this.CurrentBlock2, value);
            else
            {
                DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock2].Dispose();
                DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock2] = value;
            }
        }

        public void SetInventoryDataEx(RMG.VD_RMG_InventoryInfo_Receive inventory, Boolean needContinue = true, Boolean isBackground = true)
        {
            try
            {
                if (inventory != null)
                {
                    DataMgr.Singleton.Inventory_ClearCurrentItems();
                    DataMgr.Singleton.AddInventoryInfo(inventory);
                }

                if (!needContinue)
                    return;

                if (String.IsNullOrEmpty(this._CurrentPostion.m_cBlock) || String.IsNullOrEmpty(this._CurrentPostion.m_cBay))
                    return;

                var intCurrentBay = Convert.ToInt32(BayRemoveChars(this._CurrentPostion.m_cBay)) - 1;
                if (intCurrentBay <= 1 || intCurrentBay % 2 != 0)
                    return;
            }
            catch (Exception e)
            {
                //throw e;
            }

            if (isBackground)
                this.SendGetInventoryAsk_background(false);
            else
                this.SendGetInventoryAsk(this._CurrentPostion.m_cBlock, this._CurrentPostion.m_cBay, false);
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory convertLDMOGOJobToInven(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job)
        {
            var cntr = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory();

            cntr.cntr.cntrNo = job.cntr.cntrNo;
            cntr.cntr.cntrTp = job.cntr.cntrTp;
            cntr.cntr.opr = job.cntr.opr;
            cntr.cntr.cntrLen = job.cntr.cntrLen;
            cntr.cntr.fullMty = job.cntr.fullMty;
            cntr.cntr.cntrWgt = job.cntr.cntrWgt;
            cntr.cntr.cntrGrade = job.cntr.cntrGrade;

            cntr.jobTpKorShort = job.jobTpKorShort;
            cntr.jobTp = job.type.jobTp;

            var loc = String.IsNullOrEmpty(job.locFrom.location) ? job.locWorking : job.locFrom;
            cntr.loc.blck = loc.blck;
            cntr.loc.bay = loc.bay;
            cntr.loc.row = loc.row;
            cntr.loc.tier = loc.tier;

            return cntr;
        }

        public void SetInventoryData(RMG.VD_RMG_InventoryInfo_Receive inventory, Boolean needUIUpdate = true)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                   new Action(delegate
                   {
                        try
                        {
                            SaveLog("CLEAR_BAYVIEW_" + CurrentBlock + "_" + CurrentBay);
                           if (inventory != null)
                            {
                                DataMgr.Singleton.Inventory_ClearCurrentItems();
                               DataMgr.Singleton.AddInventoryInfo(inventory);
                            }
                           this.ThreadTimerStart(false);
                           //PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();
                           
                           if (!needUIUpdate)
                                return;
                           
                           var blockbayInfo = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;
                           
                           if (!blockbayInfo.ContainsKey(this.CurrentBlock))
                                return;

                           if (String.IsNullOrEmpty(CurrentBay))
                           {
                               DataMgr.Singleton.List_ContVirtual = this.GetViewInventorys(this.CurrentBlock, "");
                               if (DataMgr.Singleton.List_ContVirtual == null)
                                   DataMgr.Singleton.List_ContVirtual = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

                               ContainerArea contList = MainView.UC_ContainerArea;
                               JLArea_Refresh(contList, contList.CurrentPageIndex, true);
                           }
                           else
                           {
                               if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                               PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;
                           }
                           if (PresentationMgr.MainView.UC_NavigatorView.Visibility == Visibility.Visible)
                                return;

                            //PresentationMgr.MainView.UC_NavigatorView.ClearNaviItems();

                            //AREA BLOCK
                            if (!blockbayInfo[this.CurrentBlock].isBolBlck && (String.IsNullOrEmpty(this.CurrentBay) || blockbayInfo[CurrentBlock].DicBay == null))
                            {
                                PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                                if (!blockbayInfo[CurrentBlock].isBolBlck)
                                {
                                    PresentationMgr.MainView.UC_BayView.Label_Row_A.Text = "A";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_B.Text = "B";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_C.Text = "C";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_D.Text = "D";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_E.Text = "E";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_F.Text = "F";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_G.Text = "G";
                                }
                                var jobOrder = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                                if (jobOrder != null && jobOrder.type != null)
                                {
                                    ContainerItem containerItem = PresentationMgr.FindChild<ContainerItem>(PresentationMgr.MainView.UC_BayView, "UC_Ctnr_1_1");
                                    containerItem.SetInventoryInfo(null);
                                    var currentInventoryArea = this.GetViewInventorys1(this.CurrentBlock);
                                    if (currentInventoryArea.Count == 0)
                                    {
                                        containerItem.SetJobInfo(jobOrder);
                                        if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                                        {
                                            var cntr = convertLDMOGOJobToInven(jobOrder);
                                            containerItem.SetInventoryInfo(cntr);
                                        }
                                        else
                                        {
                                            containerItem.ContainerPos.m_cBlock = jobOrder.locWorking.blck;
                                            containerItem.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(jobOrder.locWorking.bay);
                                            containerItem.ContainerPos.m_cRow = jobOrder.locWorking.row;
                                            containerItem.ContainerPos.m_cTier = jobOrder.locWorking.tier;
                                        }
                                        PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                    }
                                    else
                                    {
                                        var avai = false;
                                        foreach (var cntr in currentInventoryArea)
                                        {
                                            if (cntr.cntr.cntrNo == jobOrder.cntr.cntrNo)
                                            {
                                                containerItem.SetInventoryInfo(cntr);
                                                PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                                avai = true;
                                                break;
                                            }
                                        }
                                        if (!avai)
                                        {

                                            containerItem.SetJobInfo(jobOrder);
                                            if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                                            {
                                                var cntr = convertLDMOGOJobToInven(jobOrder);
                                                containerItem.SetInventoryInfo(cntr);
                                            }
                                            else
                                            {
                                                containerItem.ContainerPos.m_cBlock = jobOrder.locWorking.blck;
                                                containerItem.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(jobOrder.locWorking.bay);
                                                containerItem.ContainerPos.m_cRow = jobOrder.locWorking.row;
                                                containerItem.ContainerPos.m_cTier = jobOrder.locWorking.tier;
                                            }
                                            PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                        }
                                    }
                                }
                            }
                           
                            if (String.IsNullOrEmpty(this.CurrentBlock) || String.IsNullOrEmpty(this.CurrentBay))
                                return;

                            var currentInventory = this.GetViewInventorys(this.CurrentBlock, this.CurrentBay);
                           if (currentInventory == null)
                               return;

                            foreach (var inven in currentInventory)
                            {
                                if (inven.cntr.isBundle && !inven.cntr.isBundleMaster)
                                {
                                    VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory avaiInven;
                                    avaiInven = currentInventory.Find(x => (x.loc.blck == inven.loc.blck && PresentationMgr.GetFrontOddBay(x.loc.bay) == PresentationMgr.GetFrontOddBay(inven.loc.bay)
                                        && x.loc.row == inven.loc.row && x.loc.tier == inven.loc.tier && x.cntr.isBundleMaster == true && x.cntr.isBundle == true));
                                    if (avaiInven == null)
                                        inven.cntr.isBundleMaster = true;
                                }
                            }

                            if (blockbayInfo[CurrentBlock].DicBay != null &&
                                blockbayInfo[CurrentBlock].DicBay.ContainsKey(this.CurrentBay) &&
                                blockbayInfo[CurrentBlock].DicBay[this.CurrentBay].RowNameMap.Count > 0)
                            {
                                rowMap = blockbayInfo[CurrentBlock].DicBay[this.CurrentBay].RowNameMap;
                                direction = blockbayInfo[CurrentBlock].Direction;

                                String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder = null;
                                VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location targetLocation = null;

                                if (!String.IsNullOrEmpty(jobKey))
                                {
                                    if ((targetJobOrder = PresentationMgr.Singleton.JOB_Get(jobKey)) != null)
                                    {
                                        if (targetJobOrder.type != null && (targetJobOrder.type.jobTp == "LD" || targetJobOrder.type.jobTp == "GO" || targetJobOrder.type.jobTp == "MO"))
                                            targetLocation = string.IsNullOrEmpty(targetJobOrder.locFrom.location) ? targetJobOrder.locWorking : targetJobOrder.locFrom;
                                        else
                                            targetLocation = targetJobOrder.locWorking;
                                    }
                                }
                                else if (this.NeedJobAutoSelection == false &&
                                    !String.IsNullOrEmpty(this._CurrentPostion.m_cRow) && !String.IsNullOrEmpty(this._CurrentPostion.m_cTier))
                                {
                                    targetLocation = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location()
                                    {
                                        blck = this._CurrentPostion.m_cBlock,
                                        bay = this._CurrentPostion.m_cBay,
                                        row = this._CurrentPostion.m_cRow,
                                        tier = this._CurrentPostion.m_cTier,
                                    };
                                }

                                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkArea_Ask(this.CurrentBlock);
                                //VMT_Data_JAT2.VMT_DataMgr_RMG.GetNoWorkTier_Ask(this.CurrentBlock, this.CurrentBay, this.CurrentBay);

                                Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> dic_Items =
                                    new Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>();

                                if (firstGetInven || !sameBlckBay || !blockbayInfo[this.CurrentBlock].isBolBlck)
                                {
                                    PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                                    firstGetInven = false;
                                    sameBlckBay = true;
                                }
                                else
                                {
                                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory oldInven in oldInventory)
                                    {
                                        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory avaiInven;
                                        avaiInven = currentInventory.Find(x => (x.cntr.cntrNo == oldInven.cntr.cntrNo && x.loc.location == oldInven.loc.location));
                                        if (avaiInven == null)
                                        {
                                            Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, oldInven.loc.row, direction);
                                            Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(oldInven.loc.tier) ? "0" : oldInven.loc.tier);
                                            PresentationMgr.MainView.UC_BayView.Container_Clear(tRowNum + 1 - 0, tTierNum - 0);
                                        }
                                    }
                                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven in currentInventory)
                                    {
                                        foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory oldInven in oldInventory)
                                        {
                                            Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, oldInven.loc.row, direction);
                                            Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(oldInven.loc.tier) ? "0" : oldInven.loc.tier);
                                            if (inven.loc.location == oldInven.loc.location && inven.cntr.cntrNo != oldInven.cntr.cntrNo)
                                            {
                                                PresentationMgr.MainView.UC_BayView.Container_Clear(tRowNum + 1 - 0, tTierNum - 0);
                                            }
                                        }
                                    }
                                }

                                PresentationMgr.MainView.UC_BayView.ClearContainerSelected();

                                // make current inventory items for navigation enabled view
                                foreach (var cntr in currentInventory)
                                {
                                    Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                                    Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                                    if (!dic_Items.ContainsKey(tierNum))
                                    {
                                        Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                                            new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                                        dic_Items.Add(tierNum, item);
                                    }
                                    if (cntr.cntr.isBundle)
                                    {
                                        if (!dic_Items[tierNum].ContainsKey(rowNum) && cntr.cntr.isBundleMaster)
                                            dic_Items[tierNum].Add(rowNum, cntr);
                                    }
                                    else
                                    {
                                        if (!dic_Items[tierNum].ContainsKey(rowNum))
                                            dic_Items[tierNum].Add(rowNum, cntr);
                                    }
                                }


                                // make navigation view total items
                                PresentationMgr.MainView.UC_NavigatorView.SetViewPortItems(rowMap.Count, blockbayInfo[CurrentBlock].MaxTier, dic_Items, rowMap, direction, targetLocation);
                                // bay view row Names
                                Int32 startRow = PresentationMgr.MainView.UC_NavigatorView.ViewPointRow;
                                Int32 startTier = PresentationMgr.MainView.UC_NavigatorView.CurrentBayTier -
                                    (PresentationMgr.MainView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_NavigatorView.ViewPointTier);
                                Int32 startRowNum = startRow;
                                
                                /*
                                //20200214 optimize speed have relative when not use navigator
                                bool needReset = PresentationMgr.MainView.UC_NavigatorView.NeedReset;
                                int maxRowPerPage = PresentationMgr.MainView.UC_NavigatorView.ViewMaxRow, maxTierPerPage = PresentationMgr.MainView.UC_NavigatorView.ViewMaxTier;

                                PresentationMgr.MainView.UC_NavigatorView.CurrentBayRow = maxRowPerPage < rowMap.Count ? rowMap.Count : maxRowPerPage;
                                PresentationMgr.MainView.UC_NavigatorView.CurrentBayTier = maxTierPerPage < blockbayInfo[CurrentBlock].MaxTier ? blockbayInfo[CurrentBlock].MaxTier : maxTierPerPage;

                                if (needReset && targetLocation != null && !String.IsNullOrEmpty(targetLocation.row))
                                {
                                    PresentationMgr.MainView.UC_NavigatorView.NeedReset = false;
                                    string targetRow = targetLocation.row;

                                    if (targetLocation.blck.Equals(PresentationMgr.Singleton.CurrentBlock) && PresentationMgr.IsBayEqual(targetLocation.bay, PresentationMgr.Singleton.CurrentBay))
                                    {
                                        int startRowIndex = PresentationMgr.ConvertRowToNumber(rowMap, (direction.Equals(Row_Direction.TB) ? rowMap[0].rowNm : rowMap[rowMap.Count - 1].rowNm), direction);
                                        int currRowIndex = PresentationMgr.ConvertRowToNumber(rowMap, targetRow, direction);

                                        if (startRowIndex > -1 && currRowIndex > -1)
                                        {
                                            int rowIndexCurrent = startRowIndex - currRowIndex;
                                            if (rowIndexCurrent < 1)
                                            {
                                                int addIndex = 0;
                                                rowIndexCurrent = Math.Abs(rowIndexCurrent) + addIndex;
                                                if (!(PresentationMgr.MainView.UC_NavigatorView.ViewPointRow <= rowIndexCurrent && (MainView.UC_NavigatorView.ViewPointRow + maxRowPerPage) >= rowIndexCurrent))
                                                {
                                                    if (rowIndexCurrent + maxRowPerPage > rowMap.Count) rowIndexCurrent = rowMap.Count - maxRowPerPage;
                                                    PresentationMgr.MainView.UC_NavigatorView.ViewPointRow = rowIndexCurrent;
                                                }
                                            }
                                        }
                                    } else
                                    {
                                        PresentationMgr.MainView.UC_NavigatorView.ViewPointRow = 0;
                                    }
                                }
                                else if (needReset)
                                {
                                    PresentationMgr.MainView.UC_NavigatorView.ViewPointRow = 0;
                                    PresentationMgr.MainView.UC_NavigatorView.NeedReset = false;
                                }

                                Int32 startRow = PresentationMgr.MainView.UC_NavigatorView.ViewPointRow;
                                Int32 startTier = PresentationMgr.MainView.UC_NavigatorView.CurrentBayTier -
                                (PresentationMgr.MainView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_NavigatorView.ViewPointTier);
                                Int32 startRowNum = startRow;
                                */

                                if (!blockbayInfo[CurrentBlock].isBolBlck)
                                {
                                    PresentationMgr.MainView.UC_BayView.Label_Row_A.Text = "A";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_B.Text = "B";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_C.Text = "C";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_D.Text = "D";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_E.Text = "E";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_F.Text = "F";
                                    PresentationMgr.MainView.UC_BayView.Label_Row_G.Text = "G";
                                    //if (UserInfo.gMchnTp == "TC")
                                    //{
                                    //    PresentationMgr.MainView.UC_BayView.Label_Row_H.Text = "H";
                                    //    PresentationMgr.MainView.UC_BayView.Label_Row_I.Text = "I";
                                    //}
                                }
                                else
                                {
                                    PresentationMgr.MainView.UC_BayView.Label_Row_A.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    PresentationMgr.MainView.UC_BayView.Label_Row_B.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    PresentationMgr.MainView.UC_BayView.Label_Row_C.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    PresentationMgr.MainView.UC_BayView.Label_Row_D.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    PresentationMgr.MainView.UC_BayView.Label_Row_E.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    PresentationMgr.MainView.UC_BayView.Label_Row_F.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    PresentationMgr.MainView.UC_BayView.Label_Row_G.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    //if (UserInfo.gMchnTp == "TC")
                                    //{
                                    //    PresentationMgr.MainView.UC_BayView.Label_Row_H.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    //    PresentationMgr.MainView.UC_BayView.Label_Row_I.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                    //}

                                    SolidColorBrush color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2FED28"));
                                    if (Singleton.CurrentBlock.Equals(AppWin.MainWin.locChg.blck) && Singleton.CurrentBay.Equals(AppWin.MainWin.locChg.bay))
                                    {
                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_A.Text))
                                            MainView.UC_BayView.Label_Row_A.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_A.ClearValue(TextBlock.BackgroundProperty);

                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_B.Text))
                                            MainView.UC_BayView.Label_Row_B.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_B.ClearValue(TextBlock.BackgroundProperty);

                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_C.Text))
                                            MainView.UC_BayView.Label_Row_C.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_C.ClearValue(TextBlock.BackgroundProperty);

                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_D.Text))
                                            MainView.UC_BayView.Label_Row_D.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_D.ClearValue(TextBlock.BackgroundProperty);

                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_E.Text))
                                            MainView.UC_BayView.Label_Row_E.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_E.ClearValue(TextBlock.BackgroundProperty);

                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_F.Text))
                                            MainView.UC_BayView.Label_Row_F.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_F.ClearValue(TextBlock.BackgroundProperty);

                                        if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_G.Text))
                                            MainView.UC_BayView.Label_Row_G.Background = color;
                                        else
                                            MainView.UC_BayView.Label_Row_G.ClearValue(TextBlock.BackgroundProperty);
                                    }
                                    else
                                    {
                                        MainView.UC_BayView.Label_Row_A.ClearValue(TextBlock.BackgroundProperty);
                                        MainView.UC_BayView.Label_Row_B.ClearValue(TextBlock.BackgroundProperty);
                                        MainView.UC_BayView.Label_Row_C.ClearValue(TextBlock.BackgroundProperty);
                                        MainView.UC_BayView.Label_Row_D.ClearValue(TextBlock.BackgroundProperty);
                                        MainView.UC_BayView.Label_Row_E.ClearValue(TextBlock.BackgroundProperty);
                                        MainView.UC_BayView.Label_Row_F.ClearValue(TextBlock.BackgroundProperty);
                                        MainView.UC_BayView.Label_Row_G.ClearValue(TextBlock.BackgroundProperty);
                                    }

                                }

                                this.SetNoWorkArea(rowMap, direction);
                                this.SetNoWorkTier(rowMap, direction);

                                PresentationMgr.MainView.UC_BayView.SetContainerPositions(this.CurrentBlock, this.CurrentBay, startRow, startTier, blockbayInfo[CurrentBlock].MaxTier, rowMap, direction);

                                // set container items to bayview     
                                if (!blockbayInfo[this.CurrentBlock].isBolBlck)
                                {
                                    if (targetJobOrder != null)
                                    {
                                        foreach (var cntr in currentInventory)
                                        {
                                            if (cntr.cntr.cntrNo == targetJobOrder.cntr.cntrNo)
                                                PresentationMgr.MainView.UC_BayView.ContainerList_Add(1, 1, cntr);
                                        }
                                    }
                                }
                                else
                                {
                                    SaveLog("ADD_INVENTORY_BAYVIEW_" + CurrentBlock + "_" + CurrentBay);
                                    oldDicItem = Util.DeepCopy<Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>>(dic_Items);

                                    foreach (var tierKey in dic_Items.Keys)
                                    {
                                        foreach (var row in rowMap)
                                        {
                                            if (dic_Items[tierKey].ContainsKey(row.Key))
                                            {
                                                var cntr = dic_Items[tierKey][row.Key];

                                                if (!this.CorrectionSource.Pos.IsEmpty() &&
                                                        this.CorrectionSource.Pos.m_cBlock.Equals(cntr.loc.blck) && this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(cntr.loc.bay)) &&
                                                        this.CorrectionSource.Pos.m_cRow.Equals(cntr.loc.row) && this.CorrectionSource.Pos.m_cTier.Equals(cntr.loc.tier))
                                                    cntr.IsCorrectionSelect = true;
                                                else cntr.IsCorrectionSelect = false;

                                                PresentationMgr.MainView.UC_BayView.ContainerList_Add(row.Key + 1 - startRow, tierKey - startTier, cntr);
                                            }
                                            else
                                            {
                                                PresentationMgr.MainView.UC_BayView.Container_Clear(row.Key + 1 - startRow, tierKey - startTier);
                                            }
                                        }
                                        dic_Items[tierKey].Clear();
                                    }
                                }

                                dic_Items.Clear();
                                dic_Items = null;

                                // set target container info
                                if (targetJobOrder != null)
                                {
                                    var location = targetJobOrder.locWorking;
                                    if (targetJobOrder.type.jobTp == "RH"/* && PresentationMgr.UseFromLocationForRehandling == true && targetJobOrder.type.jobStatus != "P")*/
                                        || targetJobOrder.type.jobTp == "AH")
                                        location = targetJobOrder.locWorking;
                                    else if (targetJobOrder.type.jobTp == "LD" || targetJobOrder.type.jobTp == "GO" || targetJobOrder.type.jobTp == "MO")
                                        location = string.IsNullOrEmpty(targetJobOrder.locFrom.location) ? targetJobOrder.locWorking : targetJobOrder.locFrom;

                                    if (this.CurrentBlock.Equals(location.blck) && this.CurrentBay.Equals(PresentationMgr.GetFrontOddBay(location.bay)))
                                    {
                                        Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, location.row, direction);
                                        Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(location.tier) ? "0" : location.tier);
                                        if (!blockbayInfo[this.CurrentBlock].isBolBlck)
                                        {
                                            PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                            PresentationMgr.MainView.UC_BayView.SetContainerJobInfo(1, 1, targetJobOrder,
                                                !this.CorrectionSource.Pos.IsEmpty() &&
                                                this.CorrectionSource.Pos.m_cBlock.Equals(location.blck) &&
                                                this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(location.bay)) &&
                                                this.CorrectionSource.Pos.m_cRow.Equals(location.row) &&
                                                this.CorrectionSource.Pos.m_cTier.Equals(location.tier));
                                        }
                                        else
                                        {
                                            PresentationMgr.MainView.UC_BayView.ContainerList_Target(tRowNum + 1 - startRow, tTierNum - startTier);
                                            PresentationMgr.MainView.UC_BayView.SetContainerJobInfo(tRowNum + 1 - startRow, tTierNum - startTier, targetJobOrder,
                                                !this.CorrectionSource.Pos.IsEmpty() &&
                                                this.CorrectionSource.Pos.m_cBlock.Equals(location.blck) &&
                                                this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(location.bay)) &&
                                                this.CorrectionSource.Pos.m_cRow.Equals(location.row) &&
                                                this.CorrectionSource.Pos.m_cTier.Equals(location.tier));
                                        }

                                    }
                                }
                            }

                            oldInventory = Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>(currentInventory);
                            //foreach (var item in currentInventory)
                            //    item.Dispose();
                            currentInventory.Clear();
                            currentInventory = null;

                            SaveLog("LOADED_BAYVIEW_" + CurrentBlock + "_" + CurrentBay);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                   }));
        }

        public string PreSelectedAHRHJobKey = String.Empty; //pre AH RH jobkey

        public void SetInventoryDataJobClickSameLoc()
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate
                {
                    try
                    {
                        //PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();
                        var blockbayInfo = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;

                        //AREA BLOCK
                        if (!blockbayInfo[this.CurrentBlock].isBolBlck && (String.IsNullOrEmpty(this.CurrentBay) || blockbayInfo[CurrentBlock].DicBay == null))
                        {
                            PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                            if (!blockbayInfo[CurrentBlock].isBolBlck)
                            {
                                PresentationMgr.MainView.UC_BayView.Label_Row_A.Text = "A";
                                PresentationMgr.MainView.UC_BayView.Label_Row_B.Text = "B";
                                PresentationMgr.MainView.UC_BayView.Label_Row_C.Text = "C";
                                PresentationMgr.MainView.UC_BayView.Label_Row_D.Text = "D";
                                PresentationMgr.MainView.UC_BayView.Label_Row_E.Text = "E";
                                PresentationMgr.MainView.UC_BayView.Label_Row_F.Text = "F";
                                PresentationMgr.MainView.UC_BayView.Label_Row_G.Text = "G";
                            }
                            var jobOrder = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                            if (jobOrder != null && jobOrder.type != null)
                            {
                                ContainerItem containerItem = PresentationMgr.FindChild<ContainerItem>(PresentationMgr.MainView.UC_BayView, "UC_Ctnr_1_1");
                                containerItem.SetInventoryInfo(null);
                                var currentInventoryArea = this.GetViewInventorys1(this.CurrentBlock);
                                if (currentInventoryArea.Count == 0)
                                {
                                    containerItem.SetJobInfo(jobOrder);
                                    if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                                    {
                                        var cntr = convertLDMOGOJobToInven(jobOrder);
                                        containerItem.SetInventoryInfo(cntr);
                                    }
                                    else
                                    {
                                        containerItem.ContainerPos.m_cBlock = jobOrder.locWorking.blck;
                                        containerItem.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(jobOrder.locWorking.bay);
                                        containerItem.ContainerPos.m_cRow = jobOrder.locWorking.row;
                                        containerItem.ContainerPos.m_cTier = jobOrder.locWorking.tier;
                                    }
                                    PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                }
                                else
                                {
                                    var avai = false;
                                    foreach (var cntr in currentInventoryArea)
                                    {
                                        if (cntr.cntr.cntrNo == jobOrder.cntr.cntrNo)
                                        {
                                            containerItem.SetInventoryInfo(cntr);
                                            PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                            avai = true;
                                            break;
                                        }
                                    }
                                    if (!avai)
                                    {

                                        containerItem.SetJobInfo(jobOrder);
                                        if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                                        {
                                            var cntr = convertLDMOGOJobToInven(jobOrder);
                                            containerItem.SetInventoryInfo(cntr);
                                        }
                                        else
                                        {
                                            containerItem.ContainerPos.m_cBlock = jobOrder.locWorking.blck;
                                            containerItem.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(jobOrder.locWorking.bay);
                                            containerItem.ContainerPos.m_cRow = jobOrder.locWorking.row;
                                            containerItem.ContainerPos.m_cTier = jobOrder.locWorking.tier;
                                        }
                                        PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                    }
                                }
                            }
                        }

                        if (String.IsNullOrEmpty(CurrentBay))
                        {
                            DataMgr.Singleton.List_ContVirtual = this.GetViewInventorys(this.CurrentBlock, CurrentBay);
                            if (DataMgr.Singleton.List_ContVirtual == null)
                                DataMgr.Singleton.List_ContVirtual = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

                            ContainerArea contList = MainView.UC_ContainerArea;
                            JLArea_Refresh(contList, contList.CurrentPageIndex, true);
                        }
                        else
                        {
                            if (PresentationMgr.Singleton.showViewINV || DataMgr.Singleton.List_JobOrder.Count > 0) PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                            PresentationMgr.MainView.UC_ContainerArea.Visibility = Visibility.Hidden;
                        }

                        if (String.IsNullOrEmpty(this.CurrentBlock) || String.IsNullOrEmpty(this.CurrentBay))
                            return;

                        var currentInventory = oldInventory;
                        if (currentInventory == null)
                            return;

                        if (blockbayInfo[CurrentBlock].DicBay != null &&
                            blockbayInfo[CurrentBlock].DicBay.ContainsKey(this.CurrentBay) &&
                            blockbayInfo[CurrentBlock].DicBay[this.CurrentBay].RowNameMap.Count > 0)
                        {
                            var rowMap = blockbayInfo[CurrentBlock].DicBay[this.CurrentBay].RowNameMap;
                            var direction = blockbayInfo[CurrentBlock].Direction;

                            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder = null;
                            VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location targetLocation = null;

                            if (!String.IsNullOrEmpty(jobKey))
                            {
                                if ((targetJobOrder = PresentationMgr.Singleton.JOB_Get(jobKey)) != null)
                                {
                                    if (targetJobOrder.type != null && (targetJobOrder.type.jobTp == "LD" || targetJobOrder.type.jobTp == "GO" || targetJobOrder.type.jobTp == "MO"))
                                        targetLocation = string.IsNullOrEmpty(targetJobOrder.locFrom.location) ? targetJobOrder.locWorking : targetJobOrder.locFrom;
                                    else
                                        targetLocation = targetJobOrder.locWorking;
                                }
                            }
                            else if (this.NeedJobAutoSelection == false &&
                                !String.IsNullOrEmpty(this._CurrentPostion.m_cRow) && !String.IsNullOrEmpty(this._CurrentPostion.m_cTier))
                            {
                                targetLocation = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location()
                                {
                                    blck = this._CurrentPostion.m_cBlock,
                                    bay = this._CurrentPostion.m_cBay,
                                    row = this._CurrentPostion.m_cRow,
                                    tier = this._CurrentPostion.m_cTier,
                                };
                            }

                            Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> dic_Items = oldDicItem;

                            if (firstGetInven || !sameBlckBay || !blockbayInfo[this.CurrentBlock].isBolBlck)
                            {
                                PresentationMgr.MainView.UC_BayView.ContainerList_Clear();
                                firstGetInven = false;
                                sameBlckBay = true;
                            }

                            PresentationMgr.MainView.UC_BayView.ClearContainerSelected();

                            // bay view row Names
                            Int32 startRow = PresentationMgr.MainView.UC_NavigatorView.ViewPointRow;
                            Int32 startTier = PresentationMgr.MainView.UC_NavigatorView.CurrentBayTier -
                                (PresentationMgr.MainView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_NavigatorView.ViewPointTier);
                            Int32 startRowNum = startRow;

                            if (!blockbayInfo[CurrentBlock].isBolBlck)
                            {
                                PresentationMgr.MainView.UC_BayView.Label_Row_A.Text = "A";
                                PresentationMgr.MainView.UC_BayView.Label_Row_B.Text = "B";
                                PresentationMgr.MainView.UC_BayView.Label_Row_C.Text = "C";
                                PresentationMgr.MainView.UC_BayView.Label_Row_D.Text = "D";
                                PresentationMgr.MainView.UC_BayView.Label_Row_E.Text = "E";
                                PresentationMgr.MainView.UC_BayView.Label_Row_F.Text = "F";
                                PresentationMgr.MainView.UC_BayView.Label_Row_G.Text = "G";
                            }
                            else
                            {
                                PresentationMgr.MainView.UC_BayView.Label_Row_A.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                PresentationMgr.MainView.UC_BayView.Label_Row_B.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                PresentationMgr.MainView.UC_BayView.Label_Row_C.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                PresentationMgr.MainView.UC_BayView.Label_Row_D.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                PresentationMgr.MainView.UC_BayView.Label_Row_E.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                PresentationMgr.MainView.UC_BayView.Label_Row_F.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                                PresentationMgr.MainView.UC_BayView.Label_Row_G.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);

                                SolidColorBrush color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2FED28"));
                                if (Singleton.CurrentBlock.Equals(AppWin.MainWin.locChg.blck) && Singleton.CurrentBay.Equals(AppWin.MainWin.locChg.bay))
                                {
                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_A.Text))
                                        MainView.UC_BayView.Label_Row_A.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_A.ClearValue(TextBlock.BackgroundProperty);

                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_B.Text))
                                        MainView.UC_BayView.Label_Row_B.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_B.ClearValue(TextBlock.BackgroundProperty);

                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_C.Text))
                                        MainView.UC_BayView.Label_Row_C.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_C.ClearValue(TextBlock.BackgroundProperty);

                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_D.Text))
                                        MainView.UC_BayView.Label_Row_D.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_D.ClearValue(TextBlock.BackgroundProperty);

                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_E.Text))
                                        MainView.UC_BayView.Label_Row_E.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_E.ClearValue(TextBlock.BackgroundProperty);

                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_F.Text))
                                        MainView.UC_BayView.Label_Row_F.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_F.ClearValue(TextBlock.BackgroundProperty);

                                    if (!String.IsNullOrEmpty(highlightRow) && highlightRow.Equals(MainView.UC_BayView.Label_Row_G.Text))
                                        MainView.UC_BayView.Label_Row_G.Background = color;
                                    else
                                        MainView.UC_BayView.Label_Row_G.ClearValue(TextBlock.BackgroundProperty);

                                }
                                else
                                {
                                    MainView.UC_BayView.Label_Row_A.ClearValue(TextBlock.BackgroundProperty);
                                    MainView.UC_BayView.Label_Row_B.ClearValue(TextBlock.BackgroundProperty);
                                    MainView.UC_BayView.Label_Row_C.ClearValue(TextBlock.BackgroundProperty);
                                    MainView.UC_BayView.Label_Row_D.ClearValue(TextBlock.BackgroundProperty);
                                    MainView.UC_BayView.Label_Row_E.ClearValue(TextBlock.BackgroundProperty);
                                    MainView.UC_BayView.Label_Row_F.ClearValue(TextBlock.BackgroundProperty);
                                }
                            }

                            PresentationMgr.MainView.UC_BayView.SetContainerPositions(this.CurrentBlock, this.CurrentBay, startRow, startTier, blockbayInfo[CurrentBlock].MaxTier, rowMap, direction);

                            // SET CONTAINER ITEMS TO BAYVIEW     
                            if (!blockbayInfo[this.CurrentBlock].isBolBlck) //AREA
                            {
                                if (targetJobOrder != null)
                                {
                                    foreach (var cntr in currentInventory)
                                    {
                                        if (cntr.cntr.cntrNo == targetJobOrder.cntr.cntrNo)
                                            PresentationMgr.MainView.UC_BayView.ContainerList_Add(1, 1, cntr);
                                    }
                                }
                            }
                            else //REAL BLOCK
                            {
                                foreach (var tierKey in dic_Items.Keys)
                                {
                                    foreach (var row in rowMap)
                                    {
                                        if (dic_Items[tierKey].ContainsKey(row.Key))
                                        {
                                            var cntr = dic_Items[tierKey][row.Key];

                                            if (!this.CorrectionSource.Pos.IsEmpty() &&
                                                    this.CorrectionSource.Pos.m_cBlock.Equals(cntr.loc.blck) && this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(cntr.loc.bay)) &&
                                                    this.CorrectionSource.Pos.m_cRow.Equals(cntr.loc.row) && this.CorrectionSource.Pos.m_cTier.Equals(cntr.loc.tier))
                                                cntr.IsCorrectionSelect = true;
                                            else cntr.IsCorrectionSelect = false;

                                            PresentationMgr.MainView.UC_BayView.ContainerList_Add(row.Key + 1 - startRow, tierKey - startTier, cntr);
                                        }
                                        else
                                        {
                                            PresentationMgr.MainView.UC_BayView.Container_Clear(row.Key + 1 - startRow, tierKey - startTier);
                                        }
                                    }
                                    dic_Items[tierKey].Clear();
                                }
                            }
                            dic_Items.Clear();
                            dic_Items = null;

                            // set target container info
                            if (targetJobOrder != null)
                            {
                                if (!String.IsNullOrEmpty(PreSelectedAHRHJobKey))
                                {
                                    var preJob = PresentationMgr.Singleton.JOB_Get(PreSelectedAHRHJobKey);
                                    if (preJob != null)
                                    {
                                        Int32 preRowNum = PresentationMgr.ConvertRowToNumber(rowMap, preJob.locFrom.row, direction);
                                        Int32 preTierNum = Convert.ToInt32(String.IsNullOrEmpty(preJob.locFrom.tier) ? "0" : preJob.locFrom.tier);
                                        ContainerItem preCont =
                                                PresentationMgr.FindChild<ContainerItem>(MainView.UC_BayView, "UC_Ctnr_" + (preRowNum + 1 - startRow).ToString() + "_" + (preTierNum - startTier).ToString());
                                        if (preCont != null && preCont.Inventory != null && preCont.Inventory.cntr.cntrNo == preJob.cntr.cntrNo)
                                        {
                                            preCont.SetInventoryInfo(preCont.Inventory);
                                        }
                                    }                                    
                                }
                                var location = targetJobOrder.locWorking;
                                if (targetJobOrder.type.jobTp == "RH"/* && PresentationMgr.UseFromLocationForRehandling == true && targetJobOrder.type.jobStatus != "P")*/
                                    || targetJobOrder.type.jobTp == "AH")
                                {
                                    location = targetJobOrder.locWorking;
                                    Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, targetJobOrder.locFrom.row, direction);
                                    Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(targetJobOrder.locFrom.tier) ? "0" : targetJobOrder.locFrom.tier);
                                    ContainerItem containerItem =
                                            PresentationMgr.FindChild<ContainerItem>(MainView.UC_BayView, "UC_Ctnr_" + (tRowNum + 1 - startRow).ToString() + "_" + (tTierNum - startTier).ToString());
                                    if (containerItem != null && containerItem.Inventory != null && containerItem.Inventory.cntr.cntrNo == targetJobOrder.cntr.cntrNo)
                                    {
                                        containerItem.SetInventoryInfo(containerItem.Inventory);
                                    }
                                }
                                else if (targetJobOrder.type.jobTp == "LD" || targetJobOrder.type.jobTp == "GO" || targetJobOrder.type.jobTp == "MO")
                                    location = string.IsNullOrEmpty(targetJobOrder.locFrom.location) ? targetJobOrder.locWorking : targetJobOrder.locFrom;

                                if (this.CurrentBlock.Equals(location.blck) && this.CurrentBay.Equals(PresentationMgr.GetFrontOddBay(location.bay)))
                                {
                                    Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, location.row, direction);
                                    Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(location.tier) ? "0" : location.tier);
                                    if (!blockbayInfo[this.CurrentBlock].isBolBlck)
                                    {
                                        PresentationMgr.MainView.UC_BayView.ContainerList_Target(1, 1);
                                        PresentationMgr.MainView.UC_BayView.SetContainerJobInfo(1, 1, targetJobOrder,
                                            !this.CorrectionSource.Pos.IsEmpty() &&
                                            this.CorrectionSource.Pos.m_cBlock.Equals(location.blck) &&
                                            this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(location.bay)) &&
                                            this.CorrectionSource.Pos.m_cRow.Equals(location.row) &&
                                            this.CorrectionSource.Pos.m_cTier.Equals(location.tier));
                                    }
                                    else
                                    {
                                        PresentationMgr.MainView.UC_BayView.ContainerList_Target(tRowNum + 1 - startRow, tTierNum - startTier);
                                        PresentationMgr.MainView.UC_BayView.SetContainerJobInfo(tRowNum + 1 - startRow, tTierNum - startTier, targetJobOrder,
                                            !this.CorrectionSource.Pos.IsEmpty() &&
                                            this.CorrectionSource.Pos.m_cBlock.Equals(location.blck) &&
                                            this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(location.bay)) &&
                                            this.CorrectionSource.Pos.m_cRow.Equals(location.row) &&
                                            this.CorrectionSource.Pos.m_cTier.Equals(location.tier));
                                    }

                                }
                            }
                        }

                        currentInventory.Clear();
                        currentInventory = null;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                }));
        }

        public void SetInventoryDataSwap(RMG.VD_RMG_InventoryInfo_Receive inventory, Boolean needUIUpdate = true)
        {
            try
            {
                if (this.CurrentUIMode != UIMode.SwapView)
                    return;
                if (inventory != null)
                {
                    DataMgr.Singleton.Inventory_ClearCurrentItems();
                    DataMgr.Singleton.AddInventoryInfo(inventory);
                }

                if (!needUIUpdate)
                    return;

                var blockbayInfo = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;
                if (!blockbayInfo.ContainsKey(this.CurrentBlock))
                    return;

                // virtual block을 선택했을 경우
                //if (blockbayInfo[this.CurrentBlock].IsVirtual)
                //{
                //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                //    PresentationMgr.MainView.UC_VirtualBlockView.Visibility = Visibility.Visible;
                //    PresentationMgr.MainView.UC_VirtualBlockView.SetBlockItems(DataMgr.Singleton.Dic_InventoryListInfo[this.CurrentBlock][this.CurrentBay].cntr);
                //    PresentationMgr.MainView.Btn_Navigator.IsEnabled = false;
                //    return;
                //}
                //else
                //{
                //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                //    PresentationMgr.MainView.UC_VirtualBlockView.Visibility = Visibility.Hidden;
                //    PresentationMgr.MainView.Btn_Navigator.IsEnabled = true;
                //}

                if (PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.Visibility == Visibility.Visible)
                    return;

                PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ClearNaviItems();
                PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Clear();

                if (String.IsNullOrEmpty(this.CurrentBlock) || String.IsNullOrEmpty(this.CurrentBay))
                    return;

                var currentInventory = this.GetViewInventorys(this.CurrentBlock, this.CurrentBay);
                if (currentInventory == null)
                    return;
                //else if (currentInventory.Count <= 0)
                //{                
                //    currentInventory = null;
                //    return;
                //}

                if (blockbayInfo[CurrentBlock].DicBay != null &&
                    blockbayInfo[CurrentBlock].DicBay.ContainsKey(this.CurrentBay) &&
                    blockbayInfo[CurrentBlock].DicBay[this.CurrentBay].RowNameMap.Count > 0)
                {
                    var rowMap = blockbayInfo[CurrentBlock].DicBay[this.CurrentBay].RowNameMap;
                    var direction = blockbayInfo[CurrentBlock].Direction;

                    Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> dic_Items =
                        new Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>();

                    // make current inventory items for navigation enabled view
                    foreach (var cntr in currentInventory)
                    {
                        Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                        Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                        if (!dic_Items.ContainsKey(tierNum))
                        {
                            Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                                new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                            dic_Items.Add(tierNum, item);
                        }
                        if (!dic_Items[tierNum].ContainsKey(rowNum))
                            dic_Items[tierNum].Add(rowNum, cntr);
                    }

                    //if (currentBayNum > 1)
                    //{
                    //    // even bay를 탐색한다. (CurrentBay - 1)    
                    //    var searchBay = Convert.ToString(currentBayNum - 1);
                    //    if (DataMgr.Singleton.InventoryInfo.DicBlock.ContainsKey(this.CurrentBlock) &&
                    //        DataMgr.Singleton.InventoryInfo.DicBlock[this.CurrentBlock].DicBay.ContainsKey(searchBay))                    
                    //    {
                    //        var cntrList = DataMgr.Singleton.InventoryInfo.DicBlock[this.CurrentBlock].DicBay[searchBay].invenList;                        
                    //        if (cntrList != null)
                    //        {
                    //            foreach (var cntr in cntrList)
                    //            {
                    //                Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                    //                Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                    //                if (!dic_Items.ContainsKey(tierNum))
                    //                {
                    //                    Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                    //                        new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                    //                    dic_Items.Add(tierNum, item);
                    //                }
                    //                if (!dic_Items[tierNum].ContainsKey(rowNum))
                    //                    dic_Items[tierNum].Add(rowNum, cntr);
                    //            }
                    //        }
                    //    }
                    //}


                    VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location targetLocationS = null;
                    var pos = MainView.UC_SwapView.UC_JobListSwap.pos;
                    targetLocationS = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location()
                    {
                        blck = pos.m_cBlock,
                        bay = pos.m_cBay,
                        row = pos.m_cRow,
                        tier = pos.m_cTier,
                    };

                    // make navigation view total items
                    PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.SetViewPortItems(rowMap.Count, blockbayInfo[CurrentBlock].MaxTier, dic_Items, rowMap, direction, targetLocationS);

                    Int32 startRowS = PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewPointRow;
                    Int32 startTierS = PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.CurrentBayTier -
                        (PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.ViewPointTier);
                    // bay view row Names
                    Int32 startRowNumS = startRowS;

                    PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_A.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_B.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_C.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_D.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_E.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);
                    //PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_F.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);
                    //PresentationMgr.MainView.UC_SwapView.UC_BayView.Label_Row_G.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNumS++, direction);

                    this.SetNoWorkArea(rowMap, direction);
                    this.SetNoWorkTier(rowMap, direction);

                    PresentationMgr.MainView.UC_SwapView.UC_BayView.SetContainerPositions(this.CurrentBlock, this.CurrentBay, startRowS, startTierS, blockbayInfo[CurrentBlock].MaxTier, rowMap, direction);
                    var cntrIso = "";
                    String targetLoc = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetContSwap;
                    // set container items to bayview     
                    foreach (var tierKey in dic_Items.Keys)
                    {
                        //foreach (var rowKey in dic_Items[tierKey].Keys)
                        foreach (var row in rowMap)
                        {
                            if (!row.Value.isUse)
                            {
                                PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Add(row.Key + 1 - startRowS, tierKey - startTierS, null, ContainerItem.NoWorkValue.VALUE_TIER);
                            }
                            else if (dic_Items[tierKey].ContainsKey(row.Key))
                            {
                                var cntr = dic_Items[tierKey][row.Key];

                                if (!this.CorrectionSource.Pos.IsEmpty() &&
                                        this.CorrectionSource.Pos.m_cBlock.Equals(cntr.loc.blck) && this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(cntr.loc.bay)) &&
                                        this.CorrectionSource.Pos.m_cRow.Equals(cntr.loc.row) && this.CorrectionSource.Pos.m_cTier.Equals(cntr.loc.tier))
                                    cntr.IsCorrectionSelect = true;
                                if (DataMgr.Singleton.IsContainCntr(cntr.cntr.cntrNo))
                                    PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Available(row.Key + 1 - startRowS, tierKey - startTierS);
                                PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Add(row.Key + 1 - startRowS, tierKey - startTierS, cntr);
                                if (targetLoc != null && cntr.cntr.cntrNo == targetLoc)
                                    cntrIso = cntr.cntr.cntrIso;
                            }
                        }
                        dic_Items[tierKey].Clear();
                    }
                    dic_Items.Clear();
                    dic_Items = null;

                    // set target container info
                    if (targetLoc != null)
                    {

                        if (this.CurrentBlock.Equals(pos.m_cBlock) && this.CurrentBay.Equals(PresentationMgr.GetFrontOddBay(pos.m_cBay)))
                        {
                            Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, pos.m_cRow, direction);
                            Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(pos.m_cTier) ? "0" : pos.m_cTier);

                            PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Target(tRowNum + 1 - startRowS, tTierNum - startTierS);
                            PresentationMgr.MainView.UC_SwapView.UC_BayView.SetContainerJobInfo(tRowNum + 1 - startRowS, tTierNum - startTierS, null,
                                !this.CorrectionSource.Pos.IsEmpty() &&
                                this.CorrectionSource.Pos.m_cBlock.Equals(pos.m_cBlock) &&
                                this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(pos.m_cBay)) &&
                                this.CorrectionSource.Pos.m_cRow.Equals(pos.m_cRow) &&
                                this.CorrectionSource.Pos.m_cTier.Equals(pos.m_cTier));
                        }
                        if (!this.CorrectionSource.Pos.IsEmpty())// 현재 설정된 correction source가 있을 경우  
                        {
                            PresentationMgr.MainView.UC_SwapView.UC_BayView.ClearContainerCorrectionSelect();
                        }

                        this.CorrectionSource.SetPos(pos);
                        this.CorrectionSource.CntrNo = targetLoc;
                        this.CorrectionSource.CntrIso = cntrIso;
                    }
                    else if (this.NeedJobAutoSelection == false)
                    {
                        if (!String.IsNullOrEmpty(this._CurrentPostion.m_cRow) && !String.IsNullOrEmpty(this._CurrentPostion.m_cTier) &&
                            PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BlockName.Equals(this._CurrentPostion.m_cBlock) &&
                            PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.BayName.Equals(this._CurrentPostion.m_cBay))
                        {
                            Int32 tRowNum = PresentationMgr.ConvertRowToNumber(rowMap, this._CurrentPostion.m_cRow, direction);
                            Int32 tTierNum = Convert.ToInt32(String.IsNullOrEmpty(this._CurrentPostion.m_cTier) ? "0" : this._CurrentPostion.m_cTier);

                            PresentationMgr.MainView.UC_SwapView.UC_BayView.ContainerList_Target(tRowNum + 1 - startRowS, tTierNum - startTierS);
                        }
                    }
                }
                //foreach (var item in currentInventory)
                //    item.Dispose();
                currentInventory.Clear();
                currentInventory = null;
            }
            catch (Exception e)
            {
                //throw e;
            }

            if (PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked == true)
                this.ThreadTimerStart(false);
        }

        public void SetInventoryDataBay1(RMG.VD_RMG_InventoryInfo_Receive inventory, Boolean needUIUpdate = true)
        {
            try
            {
                if (this.CurrentUIMode != UIMode.ContainerMoving)
                    return;
                if (inventory != null)
                {
                    DataMgr.Singleton.Inventory_ClearCurrentItems1();
                    DataMgr.Singleton.AddInventoryInfo1(inventory);
                }
                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ClearContainerCorrectionSelect();

                if (!needUIUpdate)
                    return;

                var blockbayInfo = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;
                if (!blockbayInfo.ContainsKey(this.CurrentBlock1))
                    return;

                // virtual block을 선택했을 경우
                //if (blockbayInfo[this.CurrentBlock].IsVirtual)
                //{
                //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                //    PresentationMgr.MainView.UC_VirtualBlockView.Visibility = Visibility.Visible;
                //    PresentationMgr.MainView.UC_VirtualBlockView.SetBlockItems(DataMgr.Singleton.Dic_InventoryListInfo[this.CurrentBlock][this.CurrentBay].cntr);
                //    PresentationMgr.MainView.Btn_Navigator.IsEnabled = false;
                //    return;
                //}
                //else
                //{
                //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                //    PresentationMgr.MainView.UC_VirtualBlockView.Visibility = Visibility.Hidden;
                //    PresentationMgr.MainView.Btn_Navigator.IsEnabled = true;
                //}

                if (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.Visibility == Visibility.Visible)
                    return;

                PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ClearNaviItems();
                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ContainerList_Clear();

                if (String.IsNullOrEmpty(this.CurrentBlock1) || String.IsNullOrEmpty(this.CurrentBay1))
                    return;

                var currentInventory = this.GetViewInventorys1(this.CurrentBlock1, this.CurrentBay1);
                if (currentInventory == null)
                    return;
                //else if (currentInventory.Count <= 0)
                //{                
                //    currentInventory = null;
                //    return;
                //}

                if (blockbayInfo[CurrentBlock1].DicBay != null &&
                    blockbayInfo[CurrentBlock1].DicBay.ContainsKey(this.CurrentBay1) &&
                    blockbayInfo[CurrentBlock1].DicBay[this.CurrentBay1].RowNameMap.Count > 0)
                {
                    var rowMap = blockbayInfo[CurrentBlock1].DicBay[this.CurrentBay1].RowNameMap;
                    var direction = blockbayInfo[CurrentBlock1].Direction;

                    Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> dic_Items =
                        new Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>();

                    // make current inventory items for navigation enabled view
                    foreach (var cntr in currentInventory)
                    {
                        Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                        Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                        if (!dic_Items.ContainsKey(tierNum))
                        {
                            Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                                new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                            dic_Items.Add(tierNum, item);
                        }
                        if (!dic_Items[tierNum].ContainsKey(rowNum))
                            dic_Items[tierNum].Add(rowNum, cntr);
                    }

                    //if (currentBayNum > 1)
                    //{
                    //    // even bay를 탐색한다. (CurrentBay - 1)    
                    //    var searchBay = Convert.ToString(currentBayNum - 1);
                    //    if (DataMgr.Singleton.InventoryInfo.DicBlock.ContainsKey(this.CurrentBlock) &&
                    //        DataMgr.Singleton.InventoryInfo.DicBlock[this.CurrentBlock].DicBay.ContainsKey(searchBay))                    
                    //    {
                    //        var cntrList = DataMgr.Singleton.InventoryInfo.DicBlock[this.CurrentBlock].DicBay[searchBay].invenList;                        
                    //        if (cntrList != null)
                    //        {
                    //            foreach (var cntr in cntrList)
                    //            {
                    //                Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                    //                Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                    //                if (!dic_Items.ContainsKey(tierNum))
                    //                {
                    //                    Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                    //                        new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                    //                    dic_Items.Add(tierNum, item);
                    //                }
                    //                if (!dic_Items[tierNum].ContainsKey(rowNum))
                    //                    dic_Items[tierNum].Add(rowNum, cntr);
                    //            }
                    //        }
                    //    }
                    //}
                    VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location targetLocation = null;
                    var pos = Singleton.MovingPosition1;
                    targetLocation = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location()
                    {
                        blck = pos.m_cBlock,
                        bay = pos.m_cBay,
                        row = pos.m_cRow,
                        tier = pos.m_cTier,
                    };

                    // make navigation view total items
                    PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.SetViewPortItems(rowMap.Count, blockbayInfo[CurrentBlock1].MaxTier, dic_Items, rowMap, direction, targetLocation);

                    Int32 startRow = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewPointRow;
                    Int32 startTier = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.CurrentBayTier -
                        (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewMaxTier + PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewPointTier);
                    // bay view row Names
                    Int32 startRowNum = startRow;

                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_A.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_B.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_C.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_D.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_E.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    //PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_F.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    //PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.Label_Row_G.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);

                    this.SetNoWorkArea1(rowMap, direction);
                    this.SetNoWorkTier1(rowMap, direction);

                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.SetContainerPositions(this.CurrentBlock1, this.CurrentBay1, startRow, startTier, blockbayInfo[CurrentBlock1].MaxTier, rowMap, direction);

                    // set container items to bayview     
                    foreach (var tierKey in dic_Items.Keys)
                    {
                        //foreach (var rowKey in dic_Items[tierKey].Keys)
                        foreach (var row in rowMap)
                        {
                            if (!row.Value.isUse)
                            {
                                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ContainerList_Add(row.Key + 1 - startRow, tierKey - startTier, null, ContainerItemMoving.NoWorkValue.VALUE_TIER);
                            }
                            else if (dic_Items[tierKey].ContainsKey(row.Key))
                            {
                                var cntr = dic_Items[tierKey][row.Key];

                                if (!this.CorrectionSource.Pos.IsEmpty() &&
                                        this.CorrectionSource.Pos.m_cBlock.Equals(cntr.loc.blck) && this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(cntr.loc.bay)) &&
                                        this.CorrectionSource.Pos.m_cRow.Equals(cntr.loc.row) && this.CorrectionSource.Pos.m_cTier.Equals(cntr.loc.tier))
                                    cntr.IsCorrectionSelect = true;
                                else
                                    cntr.IsCorrectionSelect = false;
                                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ContainerList_Add(row.Key + 1 - startRow, tierKey - startTier, cntr);
                            }
                        }
                        dic_Items[tierKey].Clear();
                    }
                    dic_Items.Clear();
                    dic_Items = null;

                }
                //foreach (var item in currentInventory)
                //    item.Dispose();
                currentInventory.Clear();
                currentInventory = null;
            }
            catch (Exception e)
            {
                //throw e;
            }

            if (PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked == true)
                this.ThreadTimerStart(false);
        }

        public void SetInventoryDataBay2(RMG.VD_RMG_InventoryInfo_Receive inventory, Boolean needUIUpdate = true)
        {
            try
            {
                if (this.CurrentUIMode != UIMode.ContainerMoving)
                    return;
                if (inventory != null)
                {
                    DataMgr.Singleton.Inventory_ClearCurrentItems2();
                    DataMgr.Singleton.AddInventoryInfo2(inventory);
                }
                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ClearContainerCorrectionSelect();

                if (!needUIUpdate)
                    return;

                var blockbayInfo = DataMgr.Singleton.SimpleBlockBayInfo.DicBlock;
                if (!blockbayInfo.ContainsKey(this.CurrentBlock))
                    return;

                // virtual block을 선택했을 경우
                //if (blockbayInfo[this.CurrentBlock].IsVirtual)
                //{
                //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Hidden;
                //    PresentationMgr.MainView.UC_VirtualBlockView.Visibility = Visibility.Visible;
                //    PresentationMgr.MainView.UC_VirtualBlockView.SetBlockItems(DataMgr.Singleton.Dic_InventoryListInfo[this.CurrentBlock][this.CurrentBay].cntr);
                //    PresentationMgr.MainView.Btn_Navigator.IsEnabled = false;
                //    return;
                //}
                //else
                //{
                //    PresentationMgr.MainView.UC_BayView.Visibility = Visibility.Visible;
                //    PresentationMgr.MainView.UC_VirtualBlockView.Visibility = Visibility.Hidden;
                //    PresentationMgr.MainView.Btn_Navigator.IsEnabled = true;
                //}

                if (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.Visibility == Visibility.Visible)
                    return;

                PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ClearNaviItems();
                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ContainerList_Clear();

                if (String.IsNullOrEmpty(this.CurrentBlock2) || String.IsNullOrEmpty(this.CurrentBay2))
                    return;

                var currentInventory = this.GetViewInventorys2(this.CurrentBlock2, this.CurrentBay2);
                if (currentInventory == null)
                    return;
                //else if (currentInventory.Count <= 0)
                //{                
                //    currentInventory = null;
                //    return;
                //}

                if (blockbayInfo[CurrentBlock2].DicBay != null &&
                    blockbayInfo[CurrentBlock2].DicBay.ContainsKey(this.CurrentBay2) &&
                    blockbayInfo[CurrentBlock2].DicBay[this.CurrentBay2].RowNameMap.Count > 0)
                {
                    var rowMap = blockbayInfo[CurrentBlock2].DicBay[this.CurrentBay2].RowNameMap;
                    var direction = blockbayInfo[CurrentBlock2].Direction;

                    Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>> dic_Items =
                        new Dictionary<Int32, Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>>();

                    // make current inventory items for navigation enabled view
                    foreach (var cntr in currentInventory)
                    {
                        Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                        Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                        if (!dic_Items.ContainsKey(tierNum))
                        {
                            Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                                new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                            dic_Items.Add(tierNum, item);
                        }
                        if (!dic_Items[tierNum].ContainsKey(rowNum))
                            dic_Items[tierNum].Add(rowNum, cntr);
                    }

                    //if (currentBayNum > 1)
                    //{
                    //    // even bay를 탐색한다. (CurrentBay - 1)    
                    //    var searchBay = Convert.ToString(currentBayNum - 1);
                    //    if (DataMgr.Singleton.InventoryInfo.DicBlock.ContainsKey(this.CurrentBlock) &&
                    //        DataMgr.Singleton.InventoryInfo.DicBlock[this.CurrentBlock].DicBay.ContainsKey(searchBay))                    
                    //    {
                    //        var cntrList = DataMgr.Singleton.InventoryInfo.DicBlock[this.CurrentBlock].DicBay[searchBay].invenList;                        
                    //        if (cntrList != null)
                    //        {
                    //            foreach (var cntr in cntrList)
                    //            {
                    //                Int32 rowNum = PresentationMgr.ConvertRowToNumber(rowMap, cntr.loc.row, direction);
                    //                Int32 tierNum = Convert.ToInt32(String.IsNullOrEmpty(cntr.loc.tier) ? "0" : cntr.loc.tier);

                    //                if (!dic_Items.ContainsKey(tierNum))
                    //                {
                    //                    Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> item =
                    //                        new Dictionary<Int32, VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                    //                    dic_Items.Add(tierNum, item);
                    //                }
                    //                if (!dic_Items[tierNum].ContainsKey(rowNum))
                    //                    dic_Items[tierNum].Add(rowNum, cntr);
                    //            }
                    //        }
                    //    }
                    //}
                    VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location targetLocation = null;
                    var pos = Singleton.MovingPosition2;
                    targetLocation = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Location()
                    {
                        blck = pos.m_cBlock,
                        bay = pos.m_cBay,
                        row = pos.m_cRow,
                        tier = pos.m_cTier,
                    };

                    // make navigation view total items
                    PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.SetViewPortItems(rowMap.Count, blockbayInfo[CurrentBlock2].MaxTier, dic_Items, rowMap, direction, targetLocation);

                    Int32 startRow = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewPointRow;
                    Int32 startTier = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.CurrentBayTier -
                        (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewMaxTier + PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewPointTier);
                    // bay view row Names
                    Int32 startRowNum = startRow;

                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_A.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_B.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_C.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_D.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_E.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    //PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_F.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);
                    //PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.Label_Row_G.Text = PresentationMgr.ConvertNumberToRow(rowMap, startRowNum++, direction);

                    this.SetNoWorkArea2(rowMap, direction);
                    this.SetNoWorkTier2(rowMap, direction);

                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.SetContainerPositions(this.CurrentBlock2, this.CurrentBay2, startRow, startTier, blockbayInfo[CurrentBlock2].MaxTier, rowMap, direction);

                    // set container items to bayview     
                    foreach (var tierKey in dic_Items.Keys)
                    {
                        //foreach (var rowKey in dic_Items[tierKey].Keys)
                        foreach (var row in rowMap)
                        {
                            if (!row.Value.isUse)
                            {
                                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ContainerList_Add(row.Key + 1 - startRow, tierKey - startTier, null, ContainerItemMoving.NoWorkValue.VALUE_TIER);
                            }
                            else if (dic_Items[tierKey].ContainsKey(row.Key))
                            {
                                var cntr = dic_Items[tierKey][row.Key];

                                if (!this.CorrectionSource.Pos.IsEmpty() &&
                                        this.CorrectionSource.Pos.m_cBlock.Equals(cntr.loc.blck) && this.CorrectionSource.Pos.m_cBay.Equals(PresentationMgr.GetFrontOddBay(cntr.loc.bay)) &&
                                        this.CorrectionSource.Pos.m_cRow.Equals(cntr.loc.row) && this.CorrectionSource.Pos.m_cTier.Equals(cntr.loc.tier))
                                    cntr.IsCorrectionSelect = true;
                                else
                                    cntr.IsCorrectionSelect = false;
                                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ContainerList_Add(row.Key + 1 - startRow, tierKey - startTier, cntr);
                            }
                        }
                        dic_Items[tierKey].Clear();
                    }
                    dic_Items.Clear();
                    dic_Items = null;


                }
                //foreach (var item in currentInventory)
                //    item.Dispose();
                currentInventory.Clear();
                currentInventory = null;
            }
            catch (Exception e)
            {
                //throw e;
            }

            if (PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked == true)
                this.ThreadTimerStart(false);
        }

        public void SetNoWorkArea(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (DataMgr.Singleton.Dic_NoWorkAreaInfo.ContainsKey(this.CurrentBlock))
            {
                var noWorkArea = DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock].NoWorkArea;

                if (noWorkArea != null && noWorkArea.Count > 0)
                {
                    Int32 startRow = PresentationMgr.MainView.UC_NavigatorView.ViewPointRow;
                    Int32 StartTier = PresentationMgr.MainView.UC_NavigatorView.CurrentBayTier -
                        (PresentationMgr.MainView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_NavigatorView.ViewPointTier);

                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation Area in oldArea)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation avaiArea;
                        avaiArea = noWorkArea.Find(x => (x.FromBay == Area.FromBay && x.FromRow == Area.FromRow && x.FromTier == Area.FromTier
                            && x.ToBay == Area.ToBay && x.ToRow == Area.ToRow && x.ToTier == Area.ToTier && x.noWorkTp == Area.noWorkTp));
                        if (avaiArea == null)
                        {
                            if (this.CurrentBlock == Area.loc.blck &&
                            (this.CurrentBay.CompareTo(Area.FromBay) != -1 && this.CurrentBay.CompareTo(Area.ToBay) != 1))
                            {
                                var fromRow = ConvertRowToNumber(rowMap, Area.FromRow, direction);
                                var toRow = ConvertRowToNumber(rowMap, Area.ToRow, direction);

                                var fromTier = Convert.ToInt32(Area.FromTier);
                                var toTier = Convert.ToInt32(Area.ToTier);

                                for (int i = Math.Min(fromRow, toRow); i <= Math.Max(fromRow, toRow); i++)
                                {
                                    for (int j = fromTier; j <= toTier && j <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock].MaxTier; j++)
                                    {
                                        PresentationMgr.MainView.UC_BayView.NoWork_Clear(i + 1 - startRow, j - StartTier);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var area in noWorkArea)
                    {
                        if (this.CurrentBlock == area.loc.blck &&
                            (this.CurrentBay.CompareTo(area.FromBay) != -1 && this.CurrentBay.CompareTo(area.ToBay) != 1))
                        {
                            var fromRow = ConvertRowToNumber(rowMap, area.FromRow, direction);
                            var toRow = ConvertRowToNumber(rowMap, area.ToRow, direction);

                            var fromTier = Convert.ToInt32(area.FromTier);
                            var toTier = Convert.ToInt32(area.ToTier);

                            var noWorkValue = area.noWorkTp.Equals("AREA") ? ContainerItem.NoWorkValue.VALUE_AREA :
                                (area.noWorkTp.Equals("TUNNEL") ? ContainerItem.NoWorkValue.VALUE_TUNNEL :
                                (area.noWorkTp.Equals("TIER") ? ContainerItem.NoWorkValue.VALUE_TIER : ContainerItem.NoWorkValue.VALUE_NONE));

                            for (int i = Math.Min(fromRow, toRow); i <= Math.Max(fromRow, toRow); i++)
                            {
                                for (int j = fromTier; j <= toTier && j <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock].MaxTier; j++)
                                {
                                    PresentationMgr.MainView.UC_BayView.ContainerList_Add(i + 1 - startRow, j - StartTier, null, noWorkValue);
                                }
                            }
                        }
                    }
                }
                else if (oldArea.Count > 0)
                    PresentationMgr.MainView.UC_BayView.ContainerList_Clear();

                this.oldArea = Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation>>(noWorkArea);
            }
        }

        public void SetNoWorkTier(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (DataMgr.Singleton.Dic_NoWorkTierInfo.ContainsKey(this.CurrentBlock))
            {
                var noWorkTier = DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock].NoWorkArea;

                if (noWorkTier != null && noWorkTier.Count > 0)
                {
                    Int32 startRow = PresentationMgr.MainView.UC_NavigatorView.ViewPointRow;
                    Int32 StartTier = PresentationMgr.MainView.UC_NavigatorView.CurrentBayTier -
                        (PresentationMgr.MainView.UC_NavigatorView.ViewMaxTier + PresentationMgr.MainView.UC_NavigatorView.ViewPointTier);

                    foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation Tier in oldTier)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation avaiTier;
                        avaiTier = noWorkTier.Find(x => (x.FromBay == Tier.FromBay && x.FromRow == Tier.FromRow && x.FromTier == Tier.FromTier
                            && x.ToBay == Tier.ToBay && x.ToRow == Tier.ToRow && x.ToTier == Tier.ToTier && x.noWorkTp == Tier.noWorkTp));
                        if (avaiTier == null)
                        {
                            if (this.CurrentBlock == Tier.loc.blck &&
                            (this.CurrentBay == Tier.loc.bay))
                            {
                                var row = ConvertRowToNumber(rowMap, Tier.loc.row, direction);
                                var tier = Convert.ToInt32(Tier.loc.tier);

                                for (int t = tier - StartTier; t <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock].MaxTier - StartTier; t++)
                                {
                                    PresentationMgr.MainView.UC_BayView.NoWork_Clear(row + 1 - startRow, t);
                                }
                            }
                        }
                    }

                    foreach (var area in noWorkTier)
                    {
                        if (this.CurrentBlock == area.loc.blck &&
                            (this.CurrentBay == area.loc.bay))
                        {
                            var row = ConvertRowToNumber(rowMap, area.loc.row, direction);
                            var tier = Convert.ToInt32(area.loc.tier);

                            var noWorkValue = area.noWorkTp.Equals("TIER") ? ContainerItem.NoWorkValue.VALUE_TIER : ContainerItem.NoWorkValue.VALUE_NONE;
                            for (int t = tier - StartTier; t <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock].MaxTier - StartTier; t++)
                            {
                                PresentationMgr.MainView.UC_BayView.ContainerList_Add(row + 1 - startRow, t, null, noWorkValue);
                            }
                        }
                    }
                }
                else if (oldTier.Count > 0)
                    PresentationMgr.MainView.UC_BayView.ContainerList_Clear();

                this.oldTier = Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation>>(noWorkTier);
            }
        }

        public void SetNoWorkArea1(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (DataMgr.Singleton.Dic_NoWorkAreaInfo.ContainsKey(this.CurrentBlock1))
            {
                var noWorkArea = DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock1].NoWorkArea;

                if (noWorkArea != null)
                {
                    Int32 startRow = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewPointRow;
                    Int32 StartTier = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.CurrentBayTier -
                        (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewMaxTier + PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewPointTier);

                    foreach (var area in noWorkArea)
                    {
                        if (this.CurrentBlock1 == area.loc.blck &&
                            (this.CurrentBay1.CompareTo(area.FromBay) != -1 && this.CurrentBay1.CompareTo(area.ToBay) != 1))
                        {
                            var fromRow = ConvertRowToNumber(rowMap, area.FromRow, direction);
                            var toRow = ConvertRowToNumber(rowMap, area.ToRow, direction);

                            var fromTier = Convert.ToInt32(area.FromTier);
                            var toTier = Convert.ToInt32(area.ToTier);

                            var noWorkValue = area.noWorkTp.Equals("AREA") ? ContainerItemMoving.NoWorkValue.VALUE_AREA :
                                (area.noWorkTp.Equals("TUNNEL") ? ContainerItemMoving.NoWorkValue.VALUE_TUNNEL :
                                (area.noWorkTp.Equals("TIER") ? ContainerItemMoving.NoWorkValue.VALUE_TIER : ContainerItemMoving.NoWorkValue.VALUE_NONE));

                            for (int i = Math.Min(fromRow, toRow); i <= Math.Max(fromRow, toRow); i++)
                            {
                                for (int j = fromTier; j <= toTier && j <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock1].MaxTier; j++)
                                {
                                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ContainerList_Add(i + 1 - startRow, j - StartTier, null, noWorkValue);
                                }

                            }
                        }
                    }
                }
            }
        }

        public void SetNoWorkTier1(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (DataMgr.Singleton.Dic_NoWorkTierInfo.ContainsKey(this.CurrentBlock1))
            {
                var noWorkTier = DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock1].NoWorkArea;

                if (noWorkTier != null)
                {
                    Int32 startRow = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewPointRow;
                    Int32 StartTier = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.CurrentBayTier -
                        (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewMaxTier + PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView1.ViewPointTier);

                    foreach (var area in noWorkTier)
                    {
                        if (this.CurrentBlock1 == area.loc.blck &&
                            (this.CurrentBay1 == area.loc.bay))
                        {
                            var row = ConvertRowToNumber(rowMap, area.loc.row, direction);
                            var tier = Convert.ToInt32(area.loc.tier);

                            var noWorkValue = area.noWorkTp.Equals("TIER") ? ContainerItemMoving.NoWorkValue.VALUE_TIER : ContainerItemMoving.NoWorkValue.VALUE_NONE;
                            for (int t = tier - StartTier; t <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock1].MaxTier - StartTier; t++)
                            {
                                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ContainerList_Add(row + 1 - startRow, t, null, noWorkValue);
                            }
                        }
                    }
                }
            }
        }

        public void SetNoWorkArea2(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (DataMgr.Singleton.Dic_NoWorkAreaInfo.ContainsKey(this.CurrentBlock2))
            {
                var noWorkArea = DataMgr.Singleton.Dic_NoWorkAreaInfo[this.CurrentBlock2].NoWorkArea;

                if (noWorkArea != null)
                {
                    Int32 startRow = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewPointRow;
                    Int32 StartTier = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.CurrentBayTier -
                        (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewMaxTier + PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewPointTier);

                    foreach (var area in noWorkArea)
                    {
                        if (this.CurrentBlock2 == area.loc.blck &&
                            (this.CurrentBay2.CompareTo(area.FromBay) != -1 && this.CurrentBay2.CompareTo(area.ToBay) != 1))
                        {
                            var fromRow = ConvertRowToNumber(rowMap, area.FromRow, direction);
                            var toRow = ConvertRowToNumber(rowMap, area.ToRow, direction);

                            var fromTier = Convert.ToInt32(area.FromTier);
                            var toTier = Convert.ToInt32(area.ToTier);

                            var noWorkValue = area.noWorkTp.Equals("AREA") ? ContainerItemMoving.NoWorkValue.VALUE_AREA :
                                (area.noWorkTp.Equals("TUNNEL") ? ContainerItemMoving.NoWorkValue.VALUE_TUNNEL :
                                (area.noWorkTp.Equals("TIER") ? ContainerItemMoving.NoWorkValue.VALUE_TIER : ContainerItemMoving.NoWorkValue.VALUE_NONE));

                            for (int i = Math.Min(fromRow, toRow); i <= Math.Max(fromRow, toRow); i++)
                            {
                                for (int j = fromTier; j <= toTier && j <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock2].MaxTier; j++)
                                {
                                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ContainerList_Add(i + 1 - startRow, j - StartTier, null, noWorkValue);
                                }

                            }
                        }
                    }
                }
            }
        }

        public void SetNoWorkTier2(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> rowMap,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (DataMgr.Singleton.Dic_NoWorkTierInfo.ContainsKey(this.CurrentBlock2))
            {
                var noWorkTier = DataMgr.Singleton.Dic_NoWorkTierInfo[this.CurrentBlock2].NoWorkArea;

                if (noWorkTier != null)
                {
                    Int32 startRow = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewPointRow;
                    Int32 StartTier = PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.CurrentBayTier -
                        (PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewMaxTier + PresentationMgr.MainView.UC_ContainerMoving.UC_NavigatorView2.ViewPointTier);
                    foreach (var area in noWorkTier)
                    {
                        if (this.CurrentBlock2 == area.loc.blck &&
                            (this.CurrentBay2 == area.loc.bay))
                        {
                            var row = ConvertRowToNumber(rowMap, area.loc.row, direction);
                            var tier = Convert.ToInt32(area.loc.tier);

                            var noWorkValue = area.noWorkTp.Equals("TIER") ? ContainerItemMoving.NoWorkValue.VALUE_TIER : ContainerItemMoving.NoWorkValue.VALUE_NONE;
                            for (int t = tier - StartTier; t <= DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[this.CurrentBlock2].MaxTier - StartTier; t++)
                            {
                                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ContainerList_Add(row + 1 - startRow, t, null, noWorkValue);
                            }
                        }
                    }
                }
            }
        }

        public void INV_Clear()
        {
            DataMgr.Singleton.INV_Clear();
        }

        #endregion [ Block Bay Management Method ]

        //-----------------------------------------------------------------
        //- Machine Management
        //-----------------------------------------------------------------
        #region [ Machine Management Method ]
        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine> GetMachineList(String jobKey = "")
        {
            return DataMgr.Singleton.GetMachineList(jobKey);
        }

        public List<RMG.VD_RMG_ManualReadyITV_Receive> GetReadyITV()
        {
            return DataMgr.Singleton.List_Ready_ITV;
        }

        public Boolean ReadyITV_Add(RMG.VD_RMG_ManualReadyITV_Receive value)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderByMachine = DataMgr.Singleton.GetJobOrderByMachine(value.ITVMachineID);

            // Duplicate Check
            foreach (RMG.VD_RMG_ManualReadyITV_Receive Ready_ITV in DataMgr.Singleton.List_Ready_ITV)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderReadyITV = DataMgr.Singleton.GetJobOrderByMachine(Ready_ITV.ITVMachineID);

                if (
                    Ready_ITV.ITVMachineID.Equals(value.ITVMachineID) || // Same ID
                    (
                     jobOrderReadyITV.locWorking.bay.Equals(jobOrderByMachine.locWorking.bay) &&
                     jobOrderReadyITV.locWorking.bay.Equals(jobOrderByMachine.locWorking.bay)
                    )
                   ) // Same Block & Bay
                {
                    return false;
                }
            }

            DataMgr.Singleton.List_Ready_ITV.Add(value);

            return true;
        }

        public void ReadyITV_Del(RMG.VD_RMG_ManualReadyITV_Receive value)
        {
            this.ReadyITV_Del(value.ITVMachineID);
        }

        public void ReadyITV_Del(String ITVMachineID)
        {
            foreach (RMG.VD_RMG_ManualReadyITV_Receive Ready_ITV in DataMgr.Singleton.List_Ready_ITV)
            {
                if (Ready_ITV.ITVMachineID.Equals(ITVMachineID))
                {
                    DataMgr.Singleton.List_Ready_ITV.Remove(Ready_ITV);
                    break;
                }
            }
        }

        public void ReadyITV_Clear()
        {
            DataMgr.Singleton.List_Ready_ITV.Clear();
        }

        public List<RMG.VD_RMG_PDS_RFID_Payload> GetRFIDOTR()
        {
            return DataMgr.Singleton.List_RFID_OTR;
        }

        public void RFIDOTR_Add(RMG.VD_RMG_PDS_RFID_Payload value)
        {

            foreach (RMG.VD_RMG_PDS_RFID_Payload RFID_OTR in DataMgr.Singleton.List_RFID_OTR)
            {
                if (Encoding.UTF8.GetString(RFID_OTR.m_cTagID).Equals(
                    Encoding.UTF8.GetString(value.m_cTagID)))
                {
                    return;
                }
            }

            DataMgr.Singleton.List_RFID_OTR.Add(value);
        }

        public void RFIDOTR_Del(RMG.VD_RMG_PDS_RFID_Payload value)
        {
            RFIDOTR_Del(Encoding.UTF8.GetString(value.m_cTagID));
        }

        public void RFIDOTR_Del(String TagID)
        {
            foreach (RMG.VD_RMG_PDS_RFID_Payload RFID_OTR in DataMgr.Singleton.List_RFID_OTR)
            {
                if (Encoding.UTF8.GetString(RFID_OTR.m_cTagID).Equals(TagID))
                {
                    DataMgr.Singleton.List_RFID_OTR.Remove(RFID_OTR);
                    break;
                }
            }
        }

        public void RFIDOTR_Clear(String ITVMachineID)
        {
            DataMgr.Singleton.List_RFID_OTR.Clear();
        }

        public List<String> GetReadyMachineJobKeys()
        {
            return DataMgr.Singleton.GetReadyMachineJobKeys();
        }

        public List<String> GetListJobKey()
        {
            return DataMgr.Singleton.List_JobKey;
        }

        #endregion [ Machine Management Method ]

        #region [Marrying Management Method]
        public Boolean IsMarryAble(ref String marringJobKey)
        {
            Boolean retValue = false;

            foreach (String jobKey in this.GetReadyMachineJobKeys())
            {
                if (jobKey.Equals(RMG.RMG_Member.Singleton.TargetJobKey))
                {
                    retValue = true;
                    marringJobKey = jobKey;
                    break;
                }
            }

            return retValue;
        }

        public void Marrying_Send(String marringJobKey, RMG.VD_RMG_ManualReadyITV_Receive readyITV)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = this.JOB_Get(marringJobKey);

            if (jobOrder != null && jobOrder.cntr != null)
            {
                RMG.VD_RMG_RMGMarrying_Send value = new RMG.VD_RMG_RMGMarrying_Send();
                value.WorkingMachineID = jobOrder.workingMchn.mchnId;
                value.WorkingMachineTP = jobOrder.workingMchn.mchnTp;
                value.PartnerMachineID = readyITV.ITVMachineID;
                value.PartnerMachineTP = jobOrder.partnerMchn.mchnTp;
                value.cntrNo = jobOrder.cntr.cntrNo;
                value.Loc = jobOrder.locWorking;

                VMT_DataMgr_RMG.Marrying_Ask(ref value);
            }
        }
        #endregion [Marrying Management Method]

        
        //-----------------------------------------------------------------
        //- Application Common Methods Section
        //-----------------------------------------------------------------
        #region [ UI Common Methods ]

        public void UI_SwitchUI(UIMode mode)
        {
            this.PrevUIMode = this._currentUIMode;
            this._currentUIMode = mode;

            //if (mode != UIMode.MainView)
            //    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = false;

            switch (mode)
            {
                case UIMode.LogInView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MachineSettingView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.TextBox_WSOnOff.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Label_WS.Visibility = Visibility.Hidden;
                    break;
                case UIMode.MainView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_MachineSearchView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_VirtualBlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Hidden;

                    if (VMT_Data_JAT2.Objects.Common.BlckVal != "" && this.PrevUIMode == UIMode.BaySelectionView)
                    {
                        VMT_Data_JAT2.Objects.Common.BlckVal = Convert.ToString(MainView.UC_InfomationView.UC_BlockBayInfo.Btn_BlockText.Text);
                        MainView.currentBlock = VMT_Data_JAT2.Objects.Common.BlckVal;
                        VMT_Data_JAT2.VMT_DataMgr_Common.EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                        //VMT_Data_JAT2.VMT_DataMgr_Common.MachineJobByKeys_Ask(false, VMT_Data_JAT2.Objects.Common.BlckVal, false, false);
                        VMT_Data_JAT2.VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                    }
                    break;
                case UIMode.AvailableView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.SwapView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.ContainerMoving:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BreakTimeView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    if (PresentationMgr.MainView.UC_JobList.IsAvailableBreaking)
                    {
                        PresentationMgr.AppWin.MainWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Hidden;
                        PresentationMgr.MainView.UC_BreakTimeView.Btn_Cancel.IsEnabled = false;
                        PresentationMgr.MainView.UC_BreakTimeView.Bd_Cancel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PresentationMgr.AppWin.MainWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                        PresentationMgr.MainView.UC_BreakTimeView.Btn_Cancel.IsEnabled = true;
                        PresentationMgr.MainView.UC_BreakTimeView.Bd_Cancel.Visibility = Visibility.Hidden;
                    }
                    break;
                case UIMode.ContainerSearch:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_VirtualBlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.ContainerDetailView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.VirtualBlockSelectionView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_VirtualBlockSelectionView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BlockSelectionView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BaySelectionView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BlockSelectionView1:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView1.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BaySelectionView1:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView1.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BlockSelectionView2:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView2.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.BaySelectionView2:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerMoving.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_SwapView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSearchView.Visibility = Visibility.Hidden;

                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView1.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView2.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView2.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    //PresentationMgr.AppWin.UC_MainView.UC_CorrectionView.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                case UIMode.MachineSettingView:
                    PresentationMgr.AppWin.UC_MachineSettingView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_KeypadView.Visibility = Visibility.Hidden;
                    break;
                case UIMode.ContainerSelectionView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_VirtualBlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    break;
                case UIMode.TwinSelectionView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Visible;
                    break;
                case UIMode.CorrectionView:
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_LogInView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.Visibility = Visibility.Visible;
                    PresentationMgr.AppWin.UC_MainView.UC_AvailableView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BreakTimeView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BlockSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_BaySelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerDetailView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_ContainerSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_MainView.UC_TwinSelectionView.Visibility = Visibility.Hidden;
                    PresentationMgr.AppWin.UC_IndicatorView.Btn_GoToMain.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        public void GantryMove_UISwitch(Boolean bGantryMoveOnOff)
        {
            switch (_currentUIMode)
            {
                case UIMode.LogInView:
                case UIMode.AvailableView:
                case UIMode.BreakTimeView:
                default:
                    break;
            }
        }

        public void UI_SetCPSStatus(bool bStatus)
        {
            //PresentationMgr.StopView.UC_BayWorkPanel.Btn_CPS.IsChecked = bStatus;
        }



        //1. DS / GI / MI
        //1) Container Pick-up(f/ Truck) : PLC Location(Block-Bay) 정보를 기준으로 Job 조회("I", "A") 후 작업 진행. 
        //    만약, 두 건 이상의 Job 조회 시 Pop-up, RMG Driver의 선택 후 작업 진행(Processing)
        //2) Container Set down(t/ Yard) : 현재 선택된 VMT가 가지고 있는 Job 을 기준으로 Job complete 처리.  
        //    해당 Job complete location은 PLC 인식된 데이터에 포함된 Location(Block-Bay-Row-Tier)으로 처리
        //    * 이때 PLC 데이터의 Location(Block-Bay-Row-Tier) 정보 중 "?" 등의 특수 문자로 인식 불가능한 정보가 있을 시 현재 선택된 VMT가 가지고 있는 TOS의 Plan Location(Block-Bay-Row-Tier)으로 완료 처리 함

        //2. LD / GO / MO / RH / AH
        //1) Container Pick-up(f/ Yard) : PLC Location(Block-Bay-Row-Tier) 정보를 기준으로 Job 조회("I", "A") 후 작업 진행(Processing). 
        //    만약, 두 건 이상의 Job 조회 시 Pop-up, RMG Driver의 선택 후 작업 진행(Processing)
        //2) Container Set down(t/ Truck or Yard) : 현재 선택된 VMT가 가지고 있는 Job 을 기준으로 Job complete 처리.  
        //    단, RH / AH Job 처리 시에는 해당 Job complete location은 PLC 인식된 데이터에 포함된 Location(Block-Bay-Row-Tier)으로 처리
        //    * 이때 PLC 데이터의 Location(Block-Bay-Row-Tier) 정보 중 "?" 등의 특수 문자로 인식 불가능한 정보가 있을 시 현재 선택된 VMT가 가지고 있는 TOS의 Plan Location(Block-Bay-Row-Tier)으로 완료 처리 함        

        public void ProcessSiemensData(SIEMENS.Packet.BodyRmgStatus newStatus)
        {
            if (this.CurrentRmgStatusPacket != null)
            {
                //Common.Util.Logger.Log("[VMT_RMG SiemensStatus] " + "CurrentLockStatus : " +
                //    (this.CurrentRmgStatusPacket.twistLock == 0 ? "unlock" : "lock"));
                //Common.Util.Logger.Log("[VMT_RMG SiemensStatus] " + "newLockStatus : " +
                //    (newStatus.twistLock == 0 ? "unlock" : "lock") +
                //    ", spreaderSize : " + newStatus.spreaderSize.ToString() +
                //    ", lane : " + newStatus.actlane +
                //    ", block : " + newStatus.actblock +
                //    ", bay : " + newStatus.actbay +
                //    ", row : " + newStatus.actrow +
                //    ", tier : " + newStatus.acttier);

                if (this.CurrentRmgStatusPacket.twistLock == 0 && newStatus.twistLock == 1)     // job set
                {
                    // twistLock을 제외하고 가장 최근 수신되었던 정보를 적용한다.
                    //newStatus.CopyExceptLock(this.LastestRmgStatusPacket);
                    newStatus.CopyPosition(this.LastestRmgStatusPacket);

                    // 20pt twin 작업인지 확인
                    var bayName = newStatus.actbay;
                    if (newStatus.spreaderSize == 2020)
                        bayName = PresentationMgr.GetFrontOddBay(bayName);

                    if (newStatus.actlane != 3) // NOT YARD
                    {
                        if (this.Siemens_JobSetFromLane(newStatus.actblock, bayName, newStatus.spreaderSize))//newStatus.spreaderSize == 2020))
                        {
                            //PresentationMgr.Singleton.IsTwistLock = true;
                            //this.CurrentRmgStatusPacket = newStatus;
                        }
                    }
                    else                        // FROM YARD
                    {
                        if (this.Siemens_JobSetFromYard(newStatus.actblock, bayName, newStatus.actrow, newStatus.acttier, newStatus.spreaderSize == 2020))
                        {
                            //PresentationMgr.Singleton.IsTwistLock = true;
                            //this.CurrentRmgStatusPacket = newStatus;
                        }
                    }

                    PresentationMgr.Singleton.IsTwistLock = newStatus.twistLock == 1 ? true : false;
                    this.CurrentRmgStatusPacket = newStatus;
                }
                else if (this.CurrentRmgStatusPacket.twistLock == 1 && newStatus.twistLock == 0)    // set down / on chassis
                {
                    // twistLock을 제외하고 가장 최근 수신되었던 정보를 적용한다.
                    //newStatus.CopyExceptLock(this.LastestRmgStatusPacket);
                    newStatus.CopyPosition(this.LastestRmgStatusPacket);

                    // 장지석 수석 요청 - DS/MI/GI Job Container 를 실은 ITV/OTR 이 Lane(TPW/TPL) 에 위치해 있을때, RMG Crane이  Pickup 후, 동일 위치에 Setdown 시, 완료처리 Bug 보완
                    if (newStatus.actlane != 3 &&
                        this.CurrentRmgStatusPacket.actlane == newStatus.actlane &&
                        this.CurrentRmgStatusPacket.actblock == newStatus.actblock)
                    {
                        //(1) DS/MI/GI
                        //(2) PLC Lane 정보가 'Lane' 
                        //   - TPW : 1, 2
                        //   - TPL  : 4, 5
                        //(3) Setdown PLC 정보 수신 시
                        //(4) 이전 동작/상태 확인
                        //   - 이전 동작 RMG가 Pickup 
                        //   - 이전 상태 Job Processing
                        //(5) (4) pickup position (block/lane) 정보 -> (3) setdown position (block/lane) 정보 동일한 경우
                        this.Siemens_CancelCurrentJob();
                    }
                    else if (!String.IsNullOrEmpty(this.ProcessingJobKey))
                    {
                        if (this.Siemens_JobDoneWithProcessing(newStatus))
                        {
                            //PresentationMgr.Singleton.IsTwistLock = false;
                            //this.CurrentRmgStatusPacket = newStatus;
                        }
                    }
                    else                        // job이 없는 RH의 경우
                    {
                        if (this.Siemens_JobDoneWithOutProcessing(newStatus))
                        {
                            //PresentationMgr.Singleton.IsTwistLock = false;
                            //this.CurrentRmgStatusPacket = newStatus;
                        }
                    }
                    this.ProcessingJobKey = String.Empty;

                    PresentationMgr.Singleton.IsTwistLock = newStatus.twistLock == 1 ? true : false;
                    this.CurrentRmgStatusPacket = newStatus;
                }
            }
            else
            {
                PresentationMgr.Singleton.IsTwistLock = newStatus.twistLock == 1 ? true : false;
                this.CurrentRmgStatusPacket = newStatus;
            }
            this.LastestRmgStatusPacket = newStatus;

            PresentationMgr.MainView.CheckupButtonStatus();
        }

        public Boolean Siemens_JobSetFromLane(String block, String bay, int spreaderSize)//Boolean isTwin)
        {
            Common.Util.Logger.Log("Siemens_JobSetFromLane");
            Common.Util.Logger.Log("Parameter"
                + " block : " + block
                + " bay : " + bay
                + " spreaderSize : " + spreaderSize.ToString()
                //+ " isTwin : " + isTwin.ToString()
                );

            //var jobList = this.JOB_GetForBlockBay(block, bay);
            var jobList = this.JOB_GetForYardPlan(block, bay, spreaderSize, spreaderSize == 2020);
            if (jobList != null && jobList.Count > 0)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // 2017-02-15 RMG VMT - New Requirement 
                // 1. If "Job -Set" Button selected then don’t show popup message (dialog box)
                //    - DS / MI / GI / GC / LC 대상
                //    - 'Job Set' Button 이미 클릭 (Job Processing 존재) > 수신 PLC 시그널 (오직 Lane 에서 Pickup 시) > JOB 선택 팝업 오픈 안됨.
                //    - 하지만, Processing Job 의 Size 와 PLC 시그널 Pickup Size 정보 다를때 > JOB 선택 팝업 오픈 됨.
                // 2. If "Job-Set" Button "Not Selected" and twist-lock is engaged then open the dialog box with all the Active Containers (i.e. On-Chassis) for the respective block
                //    - DS / MI / GI / GC / LC 대상
                //    - RMG VMT Processing Job 없음 > 수신 PLC 시그널 (오직 Lane 에서 Pickup 시)  > JOB선택 팝업 오픈 (해당 Machine 이 할당 받은 모든 DS / MI / GI / GC / LC Job 이 Display)
                //    - 해당 machine 이 할당 받은 Job 이 1건 이라도 팝업 오픈
                // 요청사항 1 : Processing Job이 존재하면 기본적으로 ContainerSelectionView를 띄우지 않음
                if (string.IsNullOrEmpty(this.ProcessingJobKey))
                {
                    // 2017-02-15 요청사항 2 : 해당 Machine 이 할당 받은 모든 DS / MI / GI / GC / LC Job 이 Display (1건이라도 존재할경우 조회 팝업)
                    PresentationMgr.MainView.UC_ContainerSelectionView.SetSearchedJobList(jobList, spreaderSize == 2020);
                    this.UI_SwitchUI(UIMode.ContainerSelectionView);
                }
                else
                {
                    // 2017-02-15 요청사항 1 : Processing Job 의 Size 와 PLC 시그널 Pickup Size 정보 다를때는 ContainerSelectionView를 통해 선택
                    var processingJob = this.JOB_Get(this.ProcessingJobKey);
                    if (processingJob != null && processingJob.type != null &&
                        ((Convert.ToString(spreaderSize).First() != processingJob.cntr.cntrLen.First()) ||
                        (spreaderSize == 2020 && String.IsNullOrEmpty(processingJob.type.ycTwinKey)) ||
                        (spreaderSize != 2020 && !String.IsNullOrEmpty(processingJob.type.ycTwinKey)))
                        )
                    {
                        // ContainerSelectionView에서 다른 job을 선택하게되면 기존 Processing job은 어떻게 처리할 것인가? 
                        // => SelectionView 내부에서 처리 (기존 processing 해제)
                        PresentationMgr.MainView.UC_ContainerSelectionView.SetSearchedJobList(jobList, spreaderSize == 2020);
                        this.UI_SwitchUI(UIMode.ContainerSelectionView);
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //if (jobList.Count == 1)     // 하나밖에 없을때는 그걸로 진행
                //{
                //    // 3. PLC data를 통한 pick 시에는 setPickedContainer 로 전달
                //    if (String.IsNullOrEmpty(jobList[0].type.ycTwinKey))  // single job이면
                //        PresentationMgr.Singleton.SendPickedContainerAsk(jobList[0], null);
                //    else
                //    {
                //        var twinJob = PresentationMgr.Singleton.JOB_Get(jobList[0].type.ycTwinKey);
                //        if (twinJob != null)
                //        {
                //            if (spreaderSize == 2020)
                //                PresentationMgr.Singleton.SendPickedContainerAsk(jobList[0], twinJob);
                //            else    // twin JobOrder에 대해 single로 작업하는 경우 팝업을 띄우고 선택하게 한다.
                //            {
                //                PresentationMgr.MainView.UC_TwinSelectionView.SetTwinJobOrder(jobList[0].type.jobFlagInfo == "A" ? twinJob : jobList[0], jobList[0].type.jobFlagInfo == "A" ? jobList[0] : twinJob, jobList[0].type.jobFlagInfo != "A");
                //                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.TwinSelectionView);
                //            }
                //        }
                //        else
                //            PresentationMgr.Singleton.SendPickedContainerAsk(jobList[0], null);
                //    }                    
                //}
                //else
                //{
                //    Common.Util.Logger.Log("SetSearchedJobList");

                //    // block/bay 정보로 검색된 job들을 팝업띄워서 선택하게 한다. 
                //    PresentationMgr.MainView.UC_ContainerSelectionView.SetSearchedJobList(jobList, spreaderSize == 2020);
                //    this.UI_SwitchUI(UIMode.ContainerSelectionView);
                //}
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                jobList.Clear();
                jobList = null;

                return true;
            }
            else    // YARD가 아닌데 연관된 job이 없을 수 있는가? 게다가 row/tier가 의미없는 상태..
            {
            }
            return false;
        }

        public Boolean Siemens_JobSetFromYard(String block, String bay, String row, String tier, Boolean isTwin)
        {
            Common.Util.Logger.Log("Siemens_JobSetFromYard");
            Common.Util.Logger.Log("Parameter"
                + " block : " + block
                + " bay : " + bay
                + " row : " + row
                + " tier : " + tier
                + " isTwin : " + isTwin.ToString()
                );

            //var jobList = this.JOB_GetForLocation(new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
            var jobList = this.JOB_GetForYardLocation(new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location()
            {
                blck = block,
                bay = bay,
                row = row,
                tier = tier,
            });

            if (jobList != null && jobList.Count > 0)
            {
                if (jobList.Count == 1)
                {
                    var currentJob = jobList[0];
                    if (currentJob != null && currentJob.type != null) //&& currentJob.type.jobStatus.Equals("A"))
                    {
                        //Common.Util.Logger.Log("SetJobStatus_Ask jobKey : " + currentJob.jobKey);

                        //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                        //    , "SetJobStatus_Ask"), currentJob.jobKey + ", true");

                        // 1. twin : 각각 jobSet
                        ////if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                        ////{
                        ////    if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(currentJob.jobKey))
                        ////        PresentationMgr.Singleton.RefreshJobInventory(currentJob, true);
                        ////}
                        ////VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(currentJob.jobKey, true);                        
                        ////if (isTwin && !String.IsNullOrEmpty(currentJob.type.ycTwinKey))
                        ////{
                        ////    Common.Util.Logger.Log("SetJobStatus_Ask ycTwinKey : " + currentJob.type.ycTwinKey);
                        ////    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(currentJob.type.ycTwinKey, true);

                        ////    if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                        ////    {
                        ////        if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(currentJob.type.ycTwinKey))
                        ////            PresentationMgr.Singleton.RefreshJobInventory(this.JOB_Get(currentJob.type.ycTwinKey), true);
                        ////    }
                        ////}

                        // 2. twin : list로 전달
                        //var jobKeyList = new List<String>();
                        //jobKeyList.Add(currentJob.jobKey);
                        //if (isTwin && !String.IsNullOrEmpty(currentJob.type.ycTwinKey))
                        //{
                        //    Common.Util.Logger.Log("SetJobStatus_Ask ycTwinKey : " + currentJob.type.ycTwinKey);
                        //    jobKeyList.Add(currentJob.type.ycTwinKey);
                        //}
                        //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, true);

                        // 3. PLC data를 통한 pick 시에는 setPickedContainer 로 전달                        
                        PresentationMgr.Singleton.SendPickedContainerAsk(currentJob,
                            (String.IsNullOrEmpty(currentJob.type.ycTwinKey) || !isTwin) ? null : PresentationMgr.Singleton.JOB_Get(currentJob.type.ycTwinKey));

                        if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                        {
                            if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(currentJob.jobKey))
                                PresentationMgr.Singleton.RefreshJobInventory(currentJob, true);
                            else if (!String.IsNullOrEmpty(currentJob.type.ycTwinKey) &&
                                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(currentJob.type.ycTwinKey))
                                PresentationMgr.Singleton.RefreshJobInventory(this.JOB_Get(currentJob.type.ycTwinKey), true);
                        }
                    }
                }
                else
                {
                    Common.Util.Logger.Log("SetSearchedJobList");

                    //팝업띄워서 선택하게 한다.
                    PresentationMgr.MainView.UC_ContainerSelectionView.SetSearchedJobList(jobList, isTwin);
                    this.UI_SwitchUI(UIMode.ContainerSelectionView);
                }

                jobList.Clear();
                jobList = null;

                return true;
            }
            else    // RH의 경우
            {
                //// VMT 화면의 Bay View 에 없는 Container Pickup 정보(Opus Inventory 존재) 인 경우
                ////  A.	Pickup PLC 의 Yard Location 정보 수신인 경우
                ////  B.	해당 장비의 Job Order(Job 1건도 없는 케이스는 제외) 에 존재하지 않는 PLC(Pickup Location) 인 경우
                ////  C.	현재 VMT 화면의 Bay View 에 없는 Container Pickup 정보(Opus Inventory 존재) 인 경우 
                //if (this.IDV_JobCount > 0 &&
                //    (this.CurrentBlock != block ||
                //    (this.CurrentBay != PresentationMgr.GetFrontOddBay(bay) && this.CurrentBay != PresentationMgr.GetRearOddBay(bay) &&
                //    !(isTwin == true && this.CurrentBay == PresentationMgr.GetNextBay(bay)))
                //    ))
                //{
                //    var message = String.Format("Mismatched position\nPLC signal : {0}-{1}-{2}-{3}", block, bay, row, tier);
                //    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Notice",
                //        message, "OK", null, 0);
                //}
                //else
                {
                    if (isTwin)
                        bay = PresentationMgr.GetNextBay(bay);
                    else
                        bay = PresentationMgr.GetFrontOddBay(bay);  // 40ft의 경우 짝수bay로 올 수 있음..

                    //해당 block/bay의 최신 inventoryList를 얻어온다..완료가 되면 inventory를 탐색하여 jobSet한다. -> Siemens_JobSetForRH()

                    Common.Util.Logger.Log("SendGetInventoryAsk "
                        + " block" + block
                        + " bay" + bay
                        + " isTwin" + isTwin.ToString()
                        );

                    this.SendGetInventoryAsk(block, bay, isTwin, false);
                }

                return true;
            }
            return false;
        }

        public void Siemens_GetCurrentInventoryData()
        {
            Common.Util.Logger.Log("Siemens_GetCurrentInventoryData");

            if (PresentationMgr.Singleton.CurrentRmgStatusPacket != null)
            {
                var block = PresentationMgr.Singleton.CurrentRmgStatusPacket.actblock;
                var bay = PresentationMgr.Singleton.CurrentRmgStatusPacket.actbay;
                if (PresentationMgr.Singleton.CurrentRmgStatusPacket.spreaderSize == 2020)
                    bay = PresentationMgr.GetRearOddBay(bay);

                Common.Util.Logger.Log("SendGetInventoryAsk "
                    + " block" + block
                    + " bay" + bay
                    );

                PresentationMgr.Singleton.SendGetInventoryAsk(block, bay, false, false);
            }
        }

        public void Siemens_JobSetForRH()
        {
            Common.Util.Logger.Log("Siemens_JobSetForRH");

            if (PresentationMgr.Singleton.CurrentRmgStatusPacket != null)
            {
                var isTwin = PresentationMgr.Singleton.CurrentRmgStatusPacket.spreaderSize == 2020;
                var block = PresentationMgr.Singleton.CurrentRmgStatusPacket.actblock;
                var bay = PresentationMgr.Singleton.CurrentRmgStatusPacket.actbay;
                if (isTwin)
                    bay = PresentationMgr.GetFrontOddBay(bay);
                var row = PresentationMgr.Singleton.CurrentRmgStatusPacket.actrow;
                var tier = PresentationMgr.Singleton.CurrentRmgStatusPacket.acttier;

                // 해당 block/bay의 최신 inventoryList를 얻어온 상태에서 아래로직이 진행됨
                var invenList = this.INV_GetInventory(block, bay);  // RH 의 경우 target loc 과 Yard 현재 loc이 다르므로 inventory에서 찾는다
                String twinBay = String.Empty;
                if (isTwin)
                {
                    twinBay = PresentationMgr.GetRearOddBay(PresentationMgr.Singleton.CurrentRmgStatusPacket.actbay);
                    var twinInvenList = this.INV_GetInventory(block, twinBay);
                    if (invenList != null && twinInvenList != null)
                        invenList.AddRange(twinInvenList);
                    else if (invenList == null)
                        invenList = twinInvenList;
                }

                if (invenList != null)
                {
                    var ret = from inven in invenList
                              where inven.loc.row == row &&
                                      inven.loc.tier == tier
                              orderby (inven.loc.bay)
                              select (inven.cntr.cntrNo);

                    if (ret != null && ret.Count() > 0)
                    {
                        var jobForCntr = this.JOB_GetForContainerNo(ret.First());
                        if (jobForCntr.Count > 0)
                        {
                            var target = jobForCntr.First();
                            //InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                            //    , "SetJobStatus_Ask"), target.jobKey + ", true");

                            //Common.Util.Logger.Log("SetJobStatus_Ask jobKey : " + target.jobKey);

                            //1. twin : 각각 jobSet
                            ////VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(target.jobKey, true);

                            ////if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                            ////{
                            ////    if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(target.jobKey))
                            ////        PresentationMgr.Singleton.RefreshJobInventory(target, true);
                            ////}

                            ////if (isTwin && !String.IsNullOrEmpty(target.type.ycTwinKey))
                            ////{
                            ////    Common.Util.Logger.Log("SetJobStatus_Ask ycTwinKey : " + target.type.ycTwinKey);

                            ////    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(target.type.ycTwinKey, true);

                            ////    if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                            ////    {
                            ////        if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(target.type.ycTwinKey))
                            ////            PresentationMgr.Singleton.RefreshJobInventory(this.JOB_Get(target.type.ycTwinKey), true);
                            ////    }
                            ////}

                            //2. twin : list로 전달
                            //var jobKeyList = new List<String>();
                            //jobKeyList.Add(target.jobKey);
                            //if (isTwin && !String.IsNullOrEmpty(target.type.ycTwinKey))
                            //{
                            //    Common.Util.Logger.Log("SetJobStatus_Ask ycTwinKey : " + target.type.ycTwinKey);
                            //    jobKeyList.Add(target.type.ycTwinKey);
                            //}
                            //VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobStatus_Ask(jobKeyList, true);

                            // 3. PLC data를 통한 pick 시에는 setPickedContainer 로 전달
                            Common.Util.Logger.Log("SendPickedContainerAsk");
                            PresentationMgr.Singleton.SendPickedContainerAsk(target,
                                (String.IsNullOrEmpty(target.type.ycTwinKey) || !isTwin) ? null : PresentationMgr.Singleton.JOB_Get(target.type.ycTwinKey));

                            if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                            {
                                if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(target.jobKey))
                                    PresentationMgr.Singleton.RefreshJobInventory(target, true);
                                else if (target.type != null && !String.IsNullOrEmpty(target.type.ycTwinKey) &&
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(target.type.ycTwinKey))
                                    PresentationMgr.Singleton.RefreshJobInventory(this.JOB_Get(target.type.ycTwinKey), true);
                            }
                        }
                        else    // job이 없는 RH를 위해 set한 container의 No를 기억해놓아야함..
                        {
                            Common.Util.Logger.Log("CurrentRmgStatusPacket exData ret.count: " + ret.Count());

                            this.CurrentRmgStatusPacket.exData = ret.ToList();

                            // 3. PLC data를 통한 pick 시에는 setPickedContainer 로 전달
                            var target = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                            target.workingMchn.mchnId = RMG.RMG_User.gMchnID;
                            target.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;

                            target.locFrom.blck = target.locWorking.blck = block;
                            target.locFrom.bay = target.locWorking.bay = bay;
                            target.locFrom.row = target.locWorking.row = row;
                            target.locFrom.tier = target.locWorking.tier = tier;

                            target.type.ycTwinKey = isTwin == true ? "TWIN" : String.Empty;

                            target.cntr.cntrIso = isTwin == true ? "2" : (this.CurrentRmgStatusPacket.spreaderSize == 40 ? "40" : "20");
                            target.type.jobTp = "RH";
                            target.cntr.cntrNo = ret.First();

                            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twin = null;
                            if (isTwin == true && ret.Count() > 1)
                            {
                                twin = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                                twin.workingMchn.mchnId = RMG.RMG_User.gMchnID;
                                twin.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;

                                twin.locFrom.blck = twin.locWorking.blck = block;
                                twin.locFrom.bay = twin.locWorking.bay = twinBay;
                                twin.locFrom.row = twin.locWorking.row = row;
                                twin.locFrom.tier = twin.locWorking.tier = tier;

                                twin.type.ycTwinKey = "TWIN";

                                twin.cntr.cntrIso = "2";
                                twin.type.jobTp = "RH";
                                twin.cntr.cntrNo = ret.ElementAt(1);
                            }

                            Common.Util.Logger.Log("SendPickedContainerAsk");
                            PresentationMgr.Singleton.SendPickedContainerAsk(target, twin);
                        }
                    }
                    else    // YARD job인데 컨테이너가 없으면 팝업을 띄워주자.. (RH가 아닌 경우 포함)
                    {
                        var message = String.Format("Can not find Container\nPLC Signal : {0}-{1}-{2}-{3}", block, bay, row, tier);
                        PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Notice",
                            message, "OK", null, 0);
                    }
                }
                else    // YARD job인데 컨테이너가 없으면 팝업을 띄워주자.. (RH가 아닌 경우 포함)
                {
                    var message = String.Format("Can not find Container\nPLC Signal : {0}-{1}-{2}-{3}", block, bay, row, tier);
                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common, "Notice",
                        message, "OK", null, 0);
                }
            }
        }

        public Boolean Siemens_JobDoneWithProcessing(SIEMENS.Packet.BodyRmgStatus newStatus)
        {
            Common.Util.Logger.Log("Siemens_JobDoneWithProcessing");
            Common.Util.Logger.Log("Parameter"
                + " twistLock : " + newStatus.twistLock.ToString()
                + " spreaderSize : " + newStatus.spreaderSize.ToString()
                + " actblock : " + newStatus.actblock
                + " actbay : " + newStatus.actbay
                + " actrow : " + newStatus.actrow
                + " acttier : " + newStatus.acttier
                + " actlane : " + newStatus.actlane.ToString()
                + " pre spreaderSize : " + this.CurrentRmgStatusPacket.spreaderSize.ToString()
                );

            var target = this.JOB_Get(this.ProcessingJobKey);
            if (target != null && target.type != null)
            {
                // twin job (twist unlock시 최근 packet에서 twin일경우 (spreadsize=2020) bay는 짝수로 전달됨, 앞 뒤 bay로 jobDone 두개 날려줌)
                // (bay-1: afterjob, bay+1 : fore job)                            
                // 단, RH / AH Job 처리 시에는 해당 Job complete location은 PLC 인식된 데이터에 포함된 Location(Block-Bay-Row-Tier)으로 처리                            
                // block,bay,row, tier 에 알파벳 or 숫자가 아닌 문자 포함시 processing job의 location을 사용

                target.locWorking.blck = PresentationMgr.IsValidBlock(newStatus.actblock) ? newStatus.actblock : target.locWorking.blck;
                //Boolean isBayValid = PresentationMgr.IsValidBay(newStatus.actbay);
                target.locWorking.row = PresentationMgr.IsValidRow(newStatus.actrow) ? newStatus.actrow : target.locWorking.row;
                target.locWorking.tier = PresentationMgr.IsValidTier(newStatus.acttier) ? newStatus.acttier : target.locWorking.tier;

                // 야드에서 픽업해서 야드에서 셋다운 하는 케이스때에는 Validate4LoadingSwapping 으로 저장된 값은 초기화 한다.                
                var isCancel = (target.type.jobTp.Equals("LD") || target.type.jobTp.Equals("MO")) &&
                    (this.CurrentRmgStatusPacket.actlane == 3 && newStatus.actlane == 3);

                if (/*isBayValid && */this.CurrentRmgStatusPacket.spreaderSize == 2020)
                {
                    var midbay = Convert.ToInt32(BayRemoveChars(newStatus.actbay));
                    if (midbay % 2 == 0)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder twin = null;
                        if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                            twin = this.JOB_Get(target.type.ycTwinKey);

                        //// jobFlagInfo 로 bay위치를 판단할 수 없음 (plan location으로 판단)
                        //var targetbay = Convert.ToString(twin == null ? midbay - 1 :
                        //    (Convert.ToInt32(target.locWorking.bay) <= Convert.ToInt32(twin.locWorking.bay)) ? midbay - 1 : midbay + 1);                        
                        // 2017-01-12 : 정호진 수석 요청
                        // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                        var targetbay = Convert.ToString(target.type.jobFlagInfo.Equals("F") ? midbay + 1 : midbay - 1);
                        if (targetbay.Length == 1)
                        {
                            var cha = PresentationMgr.BayRemoveInt(newStatus.actbay);
                            if (String.IsNullOrEmpty(cha))
                                target.locWorking.bay = "0" + targetbay;
                            else
                                target.locWorking.bay = cha + targetbay;
                        }
                        else
                            target.locWorking.bay = targetbay;

                        if (newStatus.actlane == 3)
                            target.type.jobTp = "RH";

                        if (isCancel)
                            PresentationMgr.RemoveLoadingSwapInfo(target.jobKey);

                        Common.Util.Logger.Log("SendJobDoneAsk target "
                            + " blck : " + target.locWorking.blck
                            + " bay : " + target.locWorking.bay
                            + " row : " + target.locWorking.row
                            + " tier : " + target.locWorking.tier
                            + " flagInfo : " + target.type.jobFlagInfo
                            );

                        this.SendJobDoneAsk(target);

                        //if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                        //{
                        //    var twin = this.JOB_Get(target.type.ycTwinKey);
                        //if (twin != null && twin.type != null)
                        //{
                        //    if (newStatus.actlane == 3)
                        //        twin.type.jobTp = "RH";

                        //    //var twinbay = Convert.ToString((Convert.ToInt32(targetbay) <= midbay) ? midbay + 1 : midbay - 1);                            
                        //    // 2017-01-12 : 정호진 수석 요청
                        //    // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                        //    var twinbay = Convert.ToString(twin.type.jobFlagInfo.Equals("F") ? midbay + 1 : midbay - 1);
                        //    twin.locWorking.blck = target.locWorking.blck;
                        //    if (twinbay.Length == 1)
                        //        twin.locWorking.bay = "0" + twinbay;
                        //    else
                        //        twin.locWorking.bay = twinbay;
                        //    twin.locWorking.row = target.locWorking.row;
                        //    twin.locWorking.tier = target.locWorking.tier;

                        //    if (isCancel)
                        //        PresentationMgr.RemoveLoadingSwapInfo(twin.jobKey);

                        //    Common.Util.Logger.Log("SendJobDoneAsk twin "
                        //        + " blck : " + twin.locWorking.blck
                        //        + " bay : " + twin.locWorking.bay
                        //        + " row : " + twin.locWorking.row
                        //        + " tier : " + twin.locWorking.tier
                        //        + " flagInfo : " + twin.type.jobFlagInfo
                        //        );

                        //    this.SendJobDoneAsk(twin);
                        //}
                        //}

                        return true;
                    }
                    else            //  odd가 넘어오면 어떻게 해야하는가?                               
                    {
                        Common.Util.Logger.Log("odd" +
                            "block=" + newStatus.actblock + ", bay=" + newStatus.actbay + ", row=" + newStatus.actrow + ", tier=" + newStatus.acttier);

                        MainWindow.LogWin.WriteLog(String.Format("[{0}] => {1} {2}",
                            System.Reflection.MethodBase.GetCurrentMethod().Name, "twin bay in siemensData is not even!",
                            "block=" + newStatus.actblock + ", bay=" + newStatus.actbay + ", row=" + newStatus.actrow + ", tier=" + newStatus.acttier));
                    }
                }
                else
                {
                    //if (isBayValid) // spreaderSize 가 2020 아니었으므로..
                    //{
                        if (!String.IsNullOrEmpty(target.type.ycTwinKey))   // twin 중 어떤 job이 현재의 ProcessingJobKey 로 설정되었는지 알 수 없음
                        {
                            var twin = this.JOB_Get(target.type.ycTwinKey);
                            if (twin != null && twin.type != null && twin.type.jobStatus == "P")
                            {
                                if (0 == string.Compare(twin.locWorking.bay, this.CurrentRmgStatusPacket.actbay, true))
                                {
                                    twin.locWorking.blck = target.locWorking.blck;
                                    twin.locWorking.row = target.locWorking.row;
                                    twin.locWorking.tier = target.locWorking.tier;
                                    target = twin;
                                }
                            }
                        }

                        target.locWorking.bay = newStatus.actbay;
                        //var midbay = Convert.ToInt32(newStatus.actbay);
                        //var targetbay = Convert.ToString(target.type.jobFlagInfo.Equals("F") ? midbay + 1 : midbay - 1);
                        //if (targetbay.Length == 1)
                        //    target.locWorking.bay = "0" + targetbay;
                        //else
                        //    target.locWorking.bay = targetbay;
                    //}
                    //else if (this.CurrentRmgStatusPacket.spreaderSize == 2020 && !String.IsNullOrEmpty(target.type.ycTwinKey))
                    //{
                        // 전달받은 bay가 유효하지 않고 twinjob으로 완료해야 한다면
                        //var twin = this.JOB_Get(target.type.ycTwinKey);
                        //if (twin != null && twin.type != null)
                        //{
                        //    if (newStatus.actlane == 3)
                        //        twin.type.jobTp = "RH";

                        //    // spreaderSize 가 2020이지만 bay가 유효하지 않으므로 쓸수없다고 판단
                        //    //if (isBayValid)   
                        //    //{                                
                        //    //    var midbay = Convert.ToInt32(newStatus.actbay);
                        //    //    var twinbay = Convert.ToString(twin.type.jobFlagInfo.Equals("F") ? midbay + 1 : midbay - 1);
                        //    //    if (twinbay.Length == 1)
                        //    //        twin.locWorking.bay = "0" + twinbay;
                        //    //    else
                        //    //        twin.locWorking.bay = twinbay;
                        //    //}

                        //    twin.locWorking.blck = (CurrentRmgStatusPacket.spreaderSize == 2020 && PresentationMgr.IsValidBlock(newStatus.actblock)) ?
                        //        newStatus.actblock : twin.locWorking.blck;
                        //    twin.locWorking.row = (CurrentRmgStatusPacket.spreaderSize == 2020 && PresentationMgr.IsValidRow(newStatus.actrow)) ?
                        //        newStatus.actrow : twin.locWorking.row;
                        //    twin.locWorking.tier = (CurrentRmgStatusPacket.spreaderSize == 2020 && PresentationMgr.IsValidTier(newStatus.acttier)) ?
                        //        newStatus.acttier : twin.locWorking.tier;

                        //    var targetLoc = target.locWorking;
                        //    var twinLoc = twin.locWorking;
                        //    if (!string.IsNullOrEmpty(targetLoc.bay) && !string.IsNullOrEmpty(twinLoc.bay))
                        //    {
                        //        // 2017-01-12 : 정호진 수석 요청
                        //        // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                        //        if ((target.type.jobFlagInfo.Equals("F") && Convert.ToInt32(targetLoc.bay) < Convert.ToInt32(twinLoc.bay)) ||
                        //            (target.type.jobFlagInfo.Equals("A") && Convert.ToInt32(targetLoc.bay) > Convert.ToInt32(twinLoc.bay)))
                        //        {
                        //            target.locWorking = twinLoc;
                        //            twin.locWorking = targetLoc;
                        //        }
                        //    }

                        //    Common.Util.Logger.Log("SendJobDoneAsk twin "
                        //        + " blck : " + twin.locWorking.blck
                        //        + " bay : " + twin.locWorking.bay
                        //        + " row : " + twin.locWorking.row
                        //        + " tier : " + twin.locWorking.tier
                        //        );

                        //    this.SendJobDoneAsk(twin);
                        //}
                    //}

                    if (newStatus.actlane == 3)
                        target.type.jobTp = "RH";

                    Common.Util.Logger.Log("SendJobDoneAsk target "
                                + " blck : " + target.locWorking.blck
                                + " bay : " + target.locWorking.bay
                                + " row : " + target.locWorking.row
                                + " tier : " + target.locWorking.tier
                                );

                    this.SendJobDoneAsk(target);

                    return true;
                }
            }

            return false;
        }

        public String GetLocTpBySiemesLane(int lane)
        {
            String strRetVlaue = String.Empty;

            if (lane == 1)
                strRetVlaue = "TPW";
            else if (lane == 2)
                strRetVlaue = "TPW";
            else if (lane == 3)
                strRetVlaue = "YARD";
            else if (lane == 4)
                strRetVlaue = "TPL";
            else if (lane == 5)
                strRetVlaue = "TPL";

            return strRetVlaue;
        }

        public Boolean Siemens_JobDoneWithOutProcessing(SIEMENS.Packet.BodyRmgStatus newStatus)
        {
            Common.Util.Logger.Log("Siemens_JobDoneWithOutProcessing");
            Common.Util.Logger.Log("Parameter"
                + " twistLock : " + newStatus.twistLock.ToString()
                + " spreaderSize : " + newStatus.spreaderSize.ToString()
                + " actblock : " + newStatus.actblock
                + " actbay : " + newStatus.actbay
                + " actrow : " + newStatus.actrow
                + " acttier : " + newStatus.acttier
                + " actlane : " + newStatus.actlane.ToString()
                + " pre spreaderSize : " + this.CurrentRmgStatusPacket.spreaderSize.ToString()
                );

            if (this.CurrentRmgStatusPacket.exData != null &&
                this.CurrentRmgStatusPacket.exData is List<String>)
            {
                var cntrList = this.CurrentRmgStatusPacket.exData as List<String>;
                if (PresentationMgr.IsValidLocation(newStatus.actblock, newStatus.actbay, newStatus.actrow, newStatus.acttier) &&
                    cntrList != null)
                {
                    var target = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                    target.locWorking.blck = newStatus.actblock;
                    target.locWorking.row = newStatus.actrow;
                    target.locWorking.tier = newStatus.acttier;

                    target.workingMchn.mchnId = RMG.RMG_User.gMchnID;
                    target.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;

                    if (this.CurrentRmgStatusPacket.spreaderSize == 2020 && cntrList.Count >= 2)
                    {
                        var midbay = Convert.ToInt32(BayRemoveChars(newStatus.actbay));
                        if (midbay % 2 == 0)
                        {
                            // target job
                            var targetbay = Convert.ToString(midbay - 1);
                            if (targetbay.Length <= 1)
                            {
                                var cha = PresentationMgr.BayRemoveInt(newStatus.actbay);
                                if (String.IsNullOrEmpty(cha))
                                    target.locWorking.bay = "0" + targetbay;
                                else
                                    target.locWorking.bay = cha + targetbay;
                            }

                            target.type.ycTwinKey = "TWIN";
                            target.cntr.cntrIso = "2";
                            target.cntr.cntrNo = cntrList[0];
                            target.type.jobTp = "NONE";//"RH";
                            target.locWorking.locTp = GetLocTpBySiemesLane(newStatus.actlane);
                            //if (newStatus.actlane == 3)
                            //    target.type.jobTp = "RH";
                            //else
                            //    target.type.jobTp = "MO";
                            //target.partnerMchn.aprchLn = GetLocTpBySiemesLane(newStatus.actlane);

                            Common.Util.Logger.Log("SendJobDoneAsk target "
                                + " blck : " + target.locWorking.blck
                                + " bay : " + target.locWorking.bay
                                + " row : " + target.locWorking.row
                                + " tier : " + target.locWorking.tier
                                );

                            this.SendJobDoneAsk(target);

                            // twin job                            
                            var twinbay = Convert.ToString(midbay + 1);
                            target.locWorking.bay = twinbay.Length == 1 ? "0" + twinbay : twinbay;

                            target.cntr.cntrNo = cntrList[1];

                            Common.Util.Logger.Log("SendJobDoneAsk twin "
                                + " blck : " + target.locWorking.blck
                                + " bay : " + target.locWorking.bay
                                + " row : " + target.locWorking.row
                                + " tier : " + target.locWorking.tier
                                );

                            this.SendJobDoneAsk(target);

                            return true;
                        }
                        else            //  odd가 넘어오면 어떻게 해야하는가?
                        {
                            Common.Util.Logger.Log("Siemens_JobDoneWithOutProcessing "
                            + "block=" + newStatus.actblock + ", bay=" + newStatus.actbay + ", row=" + newStatus.actrow + ", tier=" + newStatus.acttier);

                            MainWindow.LogWin.WriteLog(String.Format("[{0}] => {1} {2}",
                                System.Reflection.MethodBase.GetCurrentMethod().Name, "twin bay in siemensData is not even!",
                                "block=" + newStatus.actblock + ", bay=" + newStatus.actbay + ", row=" + newStatus.actrow + ", tier=" + newStatus.acttier));
                        }
                    }
                    else if (cntrList.Count >= 1)
                    {
                        target.locWorking.bay = newStatus.actbay;

                        target.type.ycTwinKey = String.Empty;
                        target.cntr.cntrIso = this.CurrentRmgStatusPacket.spreaderSize == 40 ? "40" : "20";
                        target.cntr.cntrNo = cntrList.First();
                        target.type.jobTp = "NONE";//"RH";
                        target.locWorking.locTp = GetLocTpBySiemesLane(newStatus.actlane);
                        //if (newStatus.actlane == 3)
                        //    target.type.jobTp = "RH";
                        //else
                        //    target.type.jobTp = "MO";
                        //target.partnerMchn.aprchLn = GetLocTpBySiemesLane(newStatus.actlane);

                        Common.Util.Logger.Log("SendJobDoneAsk target Single"
                            + " blck : " + target.locWorking.blck
                            + " bay : " + target.locWorking.bay
                            + " row : " + target.locWorking.row
                            + " tier : " + target.locWorking.tier
                            + " cntrIso : " + target.cntr.cntrIso
                            + " jobTp : " + target.type.jobTp
                            + " cntrNo : " + target.cntr.cntrNo
                            );

                        this.SendJobDoneAsk(target);

                        return true;
                    }
                }
            }

            return false;
        }

        public Boolean Siemens_CancelCurrentJob()
        {
            var target = String.IsNullOrEmpty(this.ProcessingJobKey) ? null : this.JOB_Get(this.ProcessingJobKey);
            if (target != null && target.type != null)
            {
                if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                {
                    var twin = this.JOB_Get(target.type.ycTwinKey);
                    if (twin != null &&
                        Convert.ToInt32(BayRemoveChars(target.locWorking.bay)) > Convert.ToInt32(BayRemoveChars(twin.locWorking.bay)))
                        VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobReOnChassis_Ask(twin.jobKey, target.jobKey);
                    else
                        VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobReOnChassis_Ask(target.jobKey, twin == null ? String.Empty : twin.jobKey);
                }
                else
                    VMT_Data_JAT2.VMT_DataMgr_RMG.SetJobReOnChassis_Ask(target.jobKey);

                if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey))
                {
                    if (VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(target.jobKey))
                        PresentationMgr.Singleton.RefreshJobInventory(target, false);
                    else if (!String.IsNullOrEmpty(target.type.ycTwinKey) &&
                        VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(target.type.ycTwinKey))
                        PresentationMgr.Singleton.RefreshJobInventory(this.JOB_Get(target.type.ycTwinKey), false);
                }

                return true;
            }

            return false;
        }

        #endregion [ UI Common Methods ]


        //-----------------------------------------------------------------
        //- Login & Machine Info Methods Section
        //-----------------------------------------------------------------
        //#region [ Login & Machine Info - Methods]
        //public string MachineID
        //{
        //    get { return RMG.RMG_User.gMchnID; }
        //    set
        //    {
        //        RMG.RMG_User.gMchnID = value;
        //        AppWin.UC_IndicatorView.TextBlock_MachineID.Text = value;
        //    }
        //}

        //public string UserID
        //{
        //    get { return RMG.RMG_User.gUserID; }
        //    set
        //    {
        //        RMG.RMG_User.gUserID = value;
        //        AppWin.UC_IndicatorView.TextBox_UserID.Text = value;
        //    }
        //}

        //public string UserPW
        //{
        //    get { return RMG.RMG_User.gUserPW; }
        //    set
        //    {
        //        RMG.RMG_User.gUserPW = value;
        //    }
        //}
        //#endregion [ Login & Machine Info - Methods]


        //-----------------------------------------------------------------
        //- Indicator View Methods Section
        //-----------------------------------------------------------------
        #region [ Indicator View Methods ]

        public int IDV_JobCount
        {
            get { return int.Parse(AppWin.UC_IndicatorView.TextBox_JobCount.Text); }
            set
            {
                AppWin.UC_IndicatorView.TextBox_JobCount.Text = value.ToString();
                MainView.UC_InfomationView.TextBlock_jobCount.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0071", LanguageService.LABEL_MAINWINDOW) + " " + value.ToString();
            }
        }

        #endregion [ Indicator View Methods ]


        //-----------------------------------------------------------------
        //- JobView Methods Section
        //-----------------------------------------------------------------
        #region [JobView Methods]

        public void JV_SetTaskBlock(String block)
        {
            JobList jobList = null;
            BlockBaySelectionView blockSelectionView = null;
            jobList = PresentationMgr.MainView.UC_JobList;
            blockSelectionView = PresentationMgr.MainView.UC_BlockSelectionView;

            if (String.IsNullOrEmpty(block))
            {
                // Set All Block 
            }
            else
            {
                WrapPanel WrapBlockSelectionView = blockSelectionView.Wrap_BlockSelectionView;

                foreach (UIElement uiElement in WrapBlockSelectionView.Children)
                {
                    if (uiElement is BlockJobControl)
                    {
                        BlockJobControl bjc = (BlockJobControl)uiElement;
                        if (bjc.BlckName == block)
                        {
                            // Select BlockJobControl
                            bjc.IsSelected = true;

                            JL_SearchNRefresh(jobList, block);
                            JL_SelectBlckRefresh(block);
                        }
                        else
                        {
                            // Unselet BlockJobControl
                            bjc.IsSelected = false;
                        }
                    }
                }
            }
        }

        private String _SelectBlckName = "";
        public String SelectBlckName
        {
            get { return _SelectBlckName; }
            set { _SelectBlckName = value; }
        }

        public void JL_SearchNRefresh(JobList jobList, String BlckName = "", Int32 pageIndex = 0)
        {
            SelectBlckName = BlckName;

            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> list_Joborder = DataMgr.Singleton.List_JobOrder;
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> search_List_Joborder = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jo in list_Joborder)
            {
                if (String.IsNullOrEmpty(BlckName) || jo.locWorking.blck.Equals(BlckName))
                    search_List_Joborder.Add(jo);
            }

            Int32 itemViewIndex = 0;
            Int32 joborderIndex = pageIndex * jobList.pageItemCount;

            if (joborderIndex < 0)
                joborderIndex = 0;

            if (joborderIndex > search_List_Joborder.Count)
                return;

            jobList.ListBox_Job.Items.Clear();

            jobList.CurrentPageIndex = pageIndex;

            for (;
                joborderIndex < search_List_Joborder.Count && itemViewIndex < (jobList.CurrentPageIndex + 1) * jobList.pageItemCount;
                joborderIndex++)
            {
                if (String.IsNullOrEmpty(BlckName) || // Set All Block 
                    search_List_Joborder[joborderIndex].locWorking.blck.Equals(BlckName))
                {
                    JobListItem item = new JobListItem();

                    item.SetJobInfo(search_List_Joborder[joborderIndex].jobKey, joborderIndex);
                    jobList.ListBox_Job.Items.Add(item);

                    itemViewIndex++;
                }
            }

            if (search_List_Joborder.Count % jobList.pageItemCount == 0)
                jobList.TotalPageCount = (search_List_Joborder.Count / jobList.pageItemCount);
            else
                jobList.TotalPageCount = (search_List_Joborder.Count / jobList.pageItemCount) + 1;

            jobList.TextBlock_PageNum.Text = (jobList.CurrentPageIndex + 1).ToString() + "/" + jobList.TotalPageCount.ToString();

            if (jobList.TotalPageCount < 1)
            {
                jobList.Btn_PageDown.IsEnabled = false;
                jobList.Btn_PageUp.IsEnabled = false;
            }
            else
            {
                if (jobList.CurrentPageIndex == 0)
                    jobList.Btn_PageUp.IsEnabled = false;
                else
                    jobList.Btn_PageUp.IsEnabled = true;

                if (jobList.TotalPageCount == jobList.CurrentPageIndex + 1)
                    jobList.Btn_PageDown.IsEnabled = false;
                else
                    jobList.Btn_PageDown.IsEnabled = true;
            }
        }

        public int GetJobOrderIndex(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if (jobOrder == null)
                return -1;

            return DataMgr.Singleton.List_JobOrder.IndexOf(jobOrder);
        }

        public int GetVirtualContOrderIndex(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven)
        {
            if (inven == null)
                return -1;

            return DataMgr.Singleton.List_ContVirtual.IndexOf(inven);
        }

        public void JL_SelectBlckRefresh(String BlckName)
        {

        }

        public Boolean NeedMoveToTargetJobPage = false;
        public Boolean JobSet = false; // jobset = true when click Job Set button successfully to NeedMoveToTargetJobPage
        public int CheckAHJobAfterJobSetCount = 0; // Check generated AH job After JobSet
        // Aug 17 Move Job has been JobSet to top until it had been completed if AH job generated
        // Aug 19 Rollback this logic
        //public enum JobSetCheckType
        //{
        //    JobSetType,
        //    GeneratedType
        //}
        //public class JobKeyObjToCheckAfterJobSet
        //{
        //    public JobSetCheckType type;
        //    public String JobKey;
        //    public JobKeyObjToCheckAfterJobSet()
        //    {
        //        type = JobSetCheckType.JobSetType;
        //        JobKey = String.Empty;
        //    }
        //}
        //public List<JobKeyObjToCheckAfterJobSet> ListJobKeyObjToCheckAfterJobSet = new List<JobKeyObjToCheckAfterJobSet>();
        // if keepSelection == false, select first item
        public void JL_Refresh(JobList jobList, Int32 pageIndex = 0, Boolean keepSelection = false)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> list_Joborder = GetFilteredJobList(jobList.CurrentFilter);

            var targetJobKey = String.Empty;

            if (CheckAHJobAfterJobSetCount > 0)
            {
                var FirstAHJob = list_Joborder.Find(x => x.type != null && "AH".Equals(x.type.jobTp) && oldLstJobOrder.Find(x1 => x.jobKey == x1.jobKey) == null);
                if (FirstAHJob != null)
                {
                    MainView.UC_JobList.selectedJobKeyPriority = FirstAHJob.jobKey;
                    MainView.UC_JobList.PreSelectedTargetJobKey = FirstAHJob.jobKey;
                    this.NeedJobAutoSelection = true;
                    // Aug 17 Add generated AH job to check
                    // Aug 19 Rollback this logic
                    //if (PresentationMgr.Singleton.ListJobKeyObjToCheckAfterJobSet.Find(x => x.JobKey == FirstAHJob.jobKey) == null)
                    //{
                    //    JobKeyObjToCheckAfterJobSet jobKeyToCheckAfterJobSet = new JobKeyObjToCheckAfterJobSet();
                    //    jobKeyToCheckAfterJobSet.type = JobSetCheckType.GeneratedType;
                    //    jobKeyToCheckAfterJobSet.JobKey = FirstAHJob.jobKey;

                    //    ListJobKeyObjToCheckAfterJobSet.Add(jobKeyToCheckAfterJobSet);
                    //}                                   
                    CheckAHJobAfterJobSetCount = 0;
                }
                //else if (CheckAHJobAfterJobSetCount <= 1 && ListJobKeyObjToCheckAfterJobSet.Count > 0) // Not found generated AH job last check time
                //{
                //    ListJobKeyObjToCheckAfterJobSet.Clear();
                //}
                CheckAHJobAfterJobSetCount--;
            }
            // Aug 17 Move Job has been JobSet to top until it had been completed if AH job generated
            // Aug 19 Rollback this logic
            //if (CheckAHJobAfterJobSetCount <= 0 && ListJobKeyObjToCheckAfterJobSet.Count > 0)
            //{
            //    for (int i = 0; i < ListJobKeyObjToCheckAfterJobSet.Count; i++)
            //    {
            //        if (ListJobKeyObjToCheckAfterJobSet[i].type == JobSetCheckType.JobSetType)
            //        {
            //            var JobNeedToSortAfterJobSet = list_Joborder.Find(x => x.jobKey == Convert.ToString(ListJobKeyObjToCheckAfterJobSet[i].JobKey));
            //            if (JobNeedToSortAfterJobSet != null)
            //            {
            //                var GeneratedJobKeyObjAfterJobSet = ListJobKeyObjToCheckAfterJobSet.Find(x => x.type == JobSetCheckType.GeneratedType);
            //                if (GeneratedJobKeyObjAfterJobSet != null)
            //                {
            //                    String generatedJobKeyStrVal = GeneratedJobKeyObjAfterJobSet.JobKey;
            //                    var GeneratedAHJob = list_Joborder.Find(x => x.jobKey == generatedJobKeyStrVal);
            //                    if (GeneratedAHJob != null)
            //                    {
            //                        list_Joborder.Remove(JobNeedToSortAfterJobSet);
            //                        list_Joborder.Insert(list_Joborder.IndexOf(GeneratedAHJob) + 1 + i, JobNeedToSortAfterJobSet);
            //                    }
            //                    else // AH jobKey in ListJobKeyObjToCheckAfterJobSet, but not found it in real job list
            //                    {
            //                        list_Joborder.Remove(JobNeedToSortAfterJobSet);
            //                        list_Joborder.Insert(i, JobNeedToSortAfterJobSet);
            //                    }
            //                }
            //                else
            //                {
            //                    list_Joborder.Remove(JobNeedToSortAfterJobSet);
            //                    list_Joborder.Insert(i, JobNeedToSortAfterJobSet);
            //                }                          
            //            }
            //            else // Remove JobKey type JobSetType when it not exist in the real job list
            //            {
            //                ListJobKeyObjToCheckAfterJobSet.Remove(ListJobKeyObjToCheckAfterJobSet[i]);
            //            }
            //        }
            //        else // ListJobKeyObjToCheckAfterJobSet[i].type == JobSetCheckType.GeneratedType
            //        {
            //            if (ListJobKeyObjToCheckAfterJobSet.Find(x => x.type == JobSetCheckType.JobSetType) == null) // There is only 1 jobKey type GeneratedType in ListJobKeyObjToCheckAfterJobSet
            //            {
            //                ListJobKeyObjToCheckAfterJobSet.Remove(ListJobKeyObjToCheckAfterJobSet[i]);
            //            }
            //        }
            //    }              
            //}
            if (!String.IsNullOrEmpty(MainView.UC_JobList.selectedJobKeyPriority) && !String.IsNullOrEmpty(RMG.RMG_Member.Singleton.TargetJobKey) 
                && MainView.UC_JobList.selectedJobKeyPriority != RMG.RMG_Member.Singleton.TargetJobKey)
            {
                targetJobKey = MainView.UC_JobList.selectedJobKeyPriority;
                MainView.UC_JobList.selectedJobKeyPriority = String.Empty;
            }
            else
            {
                targetJobKey = RMG.RMG_Member.Singleton.TargetJobKey;              
            }
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder = null;
            if (!String.IsNullOrEmpty(targetJobKey))
            {
                //if ((targetJobOrder = this.JOB_Get(targetJobKey)) == null)
                //    targetJobKey = String.Empty;
                //else 
                if (this.NeedMoveToTargetJobPage == true)
                {
                    pageIndex = list_Joborder.IndexOf(this.JOB_Get(targetJobKey)) / PresentationMgr.MainView.UC_JobList.pageItemCount;
                    if(!JobSet)
                        this.NeedMoveToTargetJobPage = false;
                    JobSet = false;
                }
            }

            Int32 itemViewIndex = 0;
            Int32 joborderIndex = pageIndex * jobList.pageItemCount;
            if (joborderIndex < 0)
                joborderIndex = 0;

            if (joborderIndex > list_Joborder.Count)
            {
                JL_Refresh(jobList, 0, true);
                return;
            }
            jobList.isSelected = 1;
            jobList.CurrentPageIndex = pageIndex;

            var newJob = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            var idx = joborderIndex;
            for (;
                idx < list_Joborder.Count && itemViewIndex < (jobList.CurrentPageIndex + 1) * jobList.pageItemCount;
                idx++)
            {
                if (itemViewIndex >= jobList.ListItems.Count)
                    break;
                newJob.Add(list_Joborder[idx]);
            }

            //COMPARE NEW JOB LIST AND OLD JOB LIST
            if (newJob.Count != oldLstJobOrder.Count) //SAME QUANTITY
            {
                jobList.ListBox_Job.Items.Clear();
            }
            else
            {
                foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder oldJob in oldLstJobOrder)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder avaiJob;
                    avaiJob = newJob.Find(x => (x.jobKey == oldJob.jobKey));
                    if (avaiJob == null || !avaiJob.EqualsJob(oldJob)) //JOB - IN OLD JOB LIST - NOT IN NEW JOB LIST - OR DIFFERENT FIELD
                    {
                        jobList.ListBox_Job.Items.Clear();
                        break;
                    }
                }
            }
            oldLstJobOrder = Util.DeepCopy<List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>>(newJob);

            if (jobList.ListBox_Job.Items.Count == 0)
            {
                for (;
                    joborderIndex < list_Joborder.Count && itemViewIndex < (jobList.CurrentPageIndex + 1) * jobList.pageItemCount;
                    joborderIndex++)
                {
                    if (itemViewIndex >= jobList.ListItems.Count)
                        break;
                    jobList.isSelected = 1;
                    jobList.ListItems[itemViewIndex].SetJobInfo(list_Joborder[joborderIndex].jobKey, joborderIndex);
                    jobList.ListItems[itemViewIndex].Selected = false;
                    //JobListItem item = new JobListItem();
                    //item.SetJobInfo(list_Joborder[joborderIndex].jobKey, joborderIndex);
                    //jobList.ListBox_Job.Items.Add(item);                
                    jobList.ListBox_Job.Items.Add(jobList.ListItems[itemViewIndex]);
                    itemViewIndex++;
                }
            }

            //
            if (String.IsNullOrEmpty(targetJobKey)) // 현재 선택된 job item이 없을 경우   (첫번째 활성화 가능한 아이템을 선택)
            {
                if (VMT_Data_JAT2.Objects.Common.BlckVal != "" && this.NeedJobAutoSelection)
                {
                    if (jobList.ListBox_Job.Items.Count > 0)
                    {
                        JobListItem item = jobList.ListBox_Job.Items.GetItemAt(0) as JobListItem;
                        if (item != null)
                        {
                            var job = this.JOB_Get(item.JobKey);
                            if (job != null) // && (job.type.jobStatus == "A" || job.type.jobStatus == "P"))
                            {
                                //jobList.ListBox_Job.SelectedIndex = 0;
                                if (jobList.ListBox_Job.SelectedItem != null)
                                {
                                    (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                                    MainView.UC_JobList.ListBox_Job_PreviewMouseLeftButtonUp(null, null);
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = job.jobKey;
                                }
                                this.NeedJobAutoSelection = false;
                            }
                        }
                    }
                }
            }
            else                                    // 이전에 선택되어 있는 job item이 있을 경우 (현재 리스트에 없으면 selection을 만들지 않음)
            {
                foreach (var listitem in jobList.ListBox_Job.Items)
                {
                    JobListItem item = listitem as JobListItem;
                    if (item != null && String.Compare(targetJobKey, item.JobKey, true) == 0)
                    {
                        jobList.isSelected = 1;
                        item.Selected = true;
                        jobList.ListBox_Job.SelectedItem = item;
                        if (jobList.ListBox_Job.SelectedItem != null)
                            (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                        if (this.NeedJobAutoSelection)
                        {
                            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = item.JobKey;
                            this.NeedJobAutoSelection = false;
                        }
                        break;
                    }
                }
            }

            if (list_Joborder.Count != 0 && list_Joborder.Count % jobList.pageItemCount == 0)
                jobList.TotalPageCount = (list_Joborder.Count / jobList.pageItemCount);
            else
                jobList.TotalPageCount = (list_Joborder.Count / jobList.pageItemCount) + 1;

            jobList.TextBlock_PageNum.Text = (jobList.CurrentPageIndex + 1).ToString() + "/" + jobList.TotalPageCount.ToString();

            if (jobList.TotalPageCount < 1)
            {
                jobList.Btn_PageDown.IsEnabled = false;
                jobList.Btn_PageUp.IsEnabled = false;
            }
            else
            {
                if (jobList.CurrentPageIndex == 0)
                    jobList.Btn_PageUp.IsEnabled = false;
                else
                    jobList.Btn_PageUp.IsEnabled = true;

                if (jobList.TotalPageCount == jobList.CurrentPageIndex + 1)
                    jobList.Btn_PageDown.IsEnabled = false;
                else
                    jobList.Btn_PageDown.IsEnabled = true;
            }
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;

            if (jobList.ListBox_Job.SelectedItem == null && String.IsNullOrEmpty(targetJobKey)) // 현재 리스트에서 아이템이 선택되지 않았고, 유지가 필요한 아이템도 없다면
                RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
            else
            {
                PresentationMgr.MainView.CheckupButtonStatus();
                //PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo();
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.RefreshTargetJobInfo();

                if (this._needRehandlingJobRefresh)
                {
                    this._needRehandlingJobRefresh = false;
                    if (targetJobOrder != null &&
                         (targetJobOrder.type.jobTp == "RH" || targetJobOrder.type.jobTp == "AH"))
                    {
                        Boolean needSetLocation = false;
                        var location = targetJobOrder.locFrom;
                        if (targetJobOrder.type.jobStatus == "P")   // from -> to
                        {
                            if (this.CurrentBlock.Equals(location.blck) && this.CurrentBay.Equals(location.bay))
                            {
                                needSetLocation = true;
                                location = targetJobOrder.locWorking;
                            }
                        }
                        else if (this.CurrentBlock.Equals(targetJobOrder.locWorking.blck) &&
                            this.CurrentBay.Equals(targetJobOrder.locWorking.bay)) // to -> From                        
                            needSetLocation = true;

                        if (needSetLocation == true)
                        {
                            if (!this.CurrentBlock.Equals(location.blck) || !this.CurrentBay.Equals(location.bay))
                            {
                                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                pos.m_cBlock = location.blck;
                                pos.m_cBay = PresentationMgr.GetFrontOddBay(location.bay);
                                pos.m_cRow = location.row;
                                pos.m_cTier = location.tier;
                                this.CurrentPostion = pos;
                            }
                            else
                            {
                                PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.NeedReset = true;
                                this.SetInventoryData(null);
                            }
                        }
                    }
                }
            }
            MainView.UC_InfomationView.setCountJobType(DataMgr.Singleton.List_JobOrder);
            if (jobList.ListBox_Job.SelectedItem == null)
            {
                this.swapList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                this.reservedList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                this.swapListRTG = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();
            }

            list_Joborder.Clear();
            list_Joborder = null;

            //20200210 only for Search container
            if (PresentationMgr.Singleton.jobAfterSearch != null)
            {
                ReselectJobItem();
            }
        }

        public void JLArea_Refresh(ContainerArea jobList, Int32 pageIndex = 0, Boolean keepSelection = false)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> lst_JobVirtual = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven in DataMgr.Singleton.List_ContVirtual)
            {
                lst_JobVirtual.Add(inven);
            }

            var targetVirtualContNo = RMG.RMG_Member.Singleton.TargetVirtualContNo;
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory targetJobOrder = null;
            if (!String.IsNullOrEmpty(targetVirtualContNo))
            {
                if ((targetJobOrder = this.CONTVirtual_Get(targetVirtualContNo)) == null)
                    targetVirtualContNo = String.Empty;
                else if (this.NeedMoveToTargetJobPage == true)
                {
                    pageIndex = PresentationMgr.Singleton.GetVirtualContOrderIndex(targetJobOrder) / PresentationMgr.MainView.UC_ContainerArea.pageItemCount;
                    this.NeedMoveToTargetJobPage = false;
                }
            }

            Int32 itemViewIndex = 0;
            Int32 joborderIndex = pageIndex * jobList.pageItemCount;
            if (joborderIndex < 0)
                joborderIndex = 0;

            if (joborderIndex > lst_JobVirtual.Count)
            {
                JLArea_Refresh(jobList, 0, true);
                return;
            }


            jobList.ListBox_Job.Items.Clear();
            jobList.CurrentPageIndex = pageIndex;

            for (;
                joborderIndex < lst_JobVirtual.Count && itemViewIndex < (jobList.CurrentPageIndex + 1) * jobList.pageItemCount;
                joborderIndex++)
            {
                if (itemViewIndex >= jobList.ListItems.Count)
                    break;

                jobList.ListItems[itemViewIndex].SetVirtualContInfo(lst_JobVirtual[joborderIndex].cntr.cntrNo, joborderIndex);
                jobList.ListItems[itemViewIndex].Selected = false;
                //JobListItem item = new JobListItem();
                //item.SetJobInfo(list_Joborder[joborderIndex].jobKey, joborderIndex);
                //jobList.ListBox_Job.Items.Add(item);
                jobList.ListBox_Job.Items.Add(jobList.ListItems[itemViewIndex]);
                itemViewIndex++;
            }

            if (String.IsNullOrEmpty(targetVirtualContNo)) // 현재 선택된 job item이 없을 경우   (첫번째 활성화 가능한 아이템을 선택)
            {
                if (this.NeedJobAutoSelection)
                {
                    if (jobList.ListBox_Job.Items.Count > 0)
                    {
                        JobListItem item = jobList.ListBox_Job.Items.GetItemAt(0) as JobListItem;
                        if (item != null)
                        {
                            var job = this.JOB_Get(item.JobKey);
                            if (job != null) // && (job.type.jobStatus == "A" || job.type.jobStatus == "P"))
                            {
                                jobList.ListBox_Job.SelectedIndex = 0;
                                if (jobList.ListBox_Job.SelectedItem != null)
                                {
                                    (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                                    MainView.UC_ContainerArea.ListBox_Job_PreviewMouseLeftButtonUp(null, null);
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = job.jobKey;
                                }
                                this.NeedJobAutoSelection = false;
                            }
                        }
                    }
                }
            }
            else                                    // 이전에 선택되어 있는 job item이 있을 경우 (현재 리스트에 없으면 selection을 만들지 않음)
            {
                foreach (var listitem in jobList.ListBox_Job.Items)
                {
                    JobListItem item = listitem as JobListItem;
                    if (item != null && String.Compare(targetVirtualContNo, item.TextBlock_CntrNo.Text, true) == 0)
                    {
                        //item.Selected = true;
                        jobList.ListBox_Job.SelectedItem = item;
                        if (jobList.ListBox_Job.SelectedItem != null)
                            (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                        break;
                    }
                }
            }

            if (lst_JobVirtual.Count != 0 && lst_JobVirtual.Count % jobList.pageItemCount == 0)
                jobList.TotalPageCount = (lst_JobVirtual.Count / jobList.pageItemCount);
            else
                jobList.TotalPageCount = (lst_JobVirtual.Count / jobList.pageItemCount) + 1;

            jobList.TextBlock_PageNum.Text = (jobList.CurrentPageIndex + 1).ToString() + "/" + jobList.TotalPageCount.ToString();

            if (jobList.TotalPageCount < 1)
            {
                jobList.Btn_PageDown.IsEnabled = false;
                jobList.Btn_PageUp.IsEnabled = false;
            }
            else
            {
                if (jobList.CurrentPageIndex == 0)
                    jobList.Btn_PageUp.IsEnabled = false;
                else
                    jobList.Btn_PageUp.IsEnabled = true;

                if (jobList.TotalPageCount == jobList.CurrentPageIndex + 1)
                    jobList.Btn_PageDown.IsEnabled = false;
                else
                    jobList.Btn_PageDown.IsEnabled = true;
            }

            if (jobList.ListBox_Job.SelectedItem == null && String.IsNullOrEmpty(targetVirtualContNo)) // 현재 리스트에서 아이템이 선택되지 않았고, 유지가 필요한 아이템도 없다면
            {
                //RMG.RMG_Member.Singleton.TargetVirtualContNo = String.Empty; //20190923
            }
            else
            {
                //PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo();
                //PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_TargetJobInfo.RefreshTargetJobInfo();

                if (this._needRehandlingJobRefresh)
                {
                    this._needRehandlingJobRefresh = false;
                    if (targetJobOrder != null &&
                         (targetJobOrder.jobTp == "RH" || targetJobOrder.jobTp == "AH"))
                    {
                        Boolean needSetLocation = false;
                        var location = targetJobOrder.loc;

                        if (this.CurrentBlock.Equals(targetJobOrder.loc.blck)) // to -> From                        
                            needSetLocation = true;

                        if (needSetLocation == true)
                        {
                            if (!this.CurrentBlock.Equals(location.blck))
                            {
                                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                pos.m_cBlock = location.blck;
                                pos.m_cBay = "";
                                pos.m_cRow = "";
                                pos.m_cTier = "";
                                this.CurrentPostion = pos;
                            }
                            else
                            {
                                this.SetInventoryData(null);
                            }
                        }
                    }
                }
            }

            lst_JobVirtual.Clear();
            lst_JobVirtual = null;
        }

        // if keepSelection == false, select first item
        public void JL_RefreshChangePageClick(JobList jobList, Int32 pageIndex = 0, Boolean keepSelection = false)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> list_Joborder = GetFilteredJobList(jobList.CurrentFilter);

            var targetJobKey = RMG.RMG_Member.Singleton.TargetJobKey;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder = null;
            if (!String.IsNullOrEmpty(targetJobKey))
            {
                //if ((targetJobOrder = this.JOB_Get(targetJobKey)) == null)
                //    targetJobKey = String.Empty;
                //else 
                if (this.NeedMoveToTargetJobPage == true)
                {
                    pageIndex = list_Joborder.IndexOf(this.JOB_Get(targetJobKey)) / PresentationMgr.MainView.UC_JobList.pageItemCount;
                    this.NeedMoveToTargetJobPage = false;
                }
            }

            Int32 itemViewIndex = 0;
            Int32 joborderIndex = pageIndex * jobList.pageItemCount;
            if (joborderIndex < 0)
                joborderIndex = 0;

            if (joborderIndex > list_Joborder.Count)
            {
                JL_Refresh(jobList, 0, true);
                return;
            }

            jobList.isSelected = 1;
            jobList.ListBox_Job.Items.Clear();
            jobList.CurrentPageIndex = pageIndex;

            for (;
                joborderIndex < list_Joborder.Count && itemViewIndex < (jobList.CurrentPageIndex + 1) * jobList.pageItemCount;
                joborderIndex++)
            {
                if (itemViewIndex >= jobList.ListItems.Count)
                    break;
                jobList.isSelected = 1;
                jobList.ListItems[itemViewIndex].SetJobInfo(list_Joborder[joborderIndex].jobKey, joborderIndex);
                jobList.ListItems[itemViewIndex].Selected = false;
                //JobListItem item = new JobListItem();
                //item.SetJobInfo(list_Joborder[joborderIndex].jobKey, joborderIndex);
                //jobList.ListBox_Job.Items.Add(item);                
                jobList.ListBox_Job.Items.Add(jobList.ListItems[itemViewIndex]);
                itemViewIndex++;
            }
            if (String.IsNullOrEmpty(targetJobKey)) // 현재 선택된 job item이 없을 경우   (첫번째 활성화 가능한 아이템을 선택)
            {
                if (VMT_Data_JAT2.Objects.Common.BlckVal != "" && this.NeedJobAutoSelection)
                {
                    if (jobList.ListBox_Job.Items.Count > 0)
                    {
                        JobListItem item = jobList.ListBox_Job.Items.GetItemAt(0) as JobListItem;
                        if (item != null)
                        {
                            var job = this.JOB_Get(item.JobKey);
                            if (job != null) // && (job.type.jobStatus == "A" || job.type.jobStatus == "P"))
                            {
                                //jobList.ListBox_Job.SelectedIndex = 0;
                                if (jobList.ListBox_Job.SelectedItem != null)
                                {
                                    (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                                    MainView.UC_JobList.ListBox_Job_PreviewMouseLeftButtonUp(null, null);
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = job.jobKey;
                                }
                                this.NeedJobAutoSelection = false;
                            }
                        }
                    }
                }
            }
            else                                    // 이전에 선택되어 있는 job item이 있을 경우 (현재 리스트에 없으면 selection을 만들지 않음)
            {
                foreach (var listitem in jobList.ListBox_Job.Items)
                {
                    JobListItem item = listitem as JobListItem;
                    if (item != null && String.Compare(targetJobKey, item.JobKey, true) == 0)
                    {
                        jobList.isSelected = 1;
                        SaveLog("JL_Refresh Start: " + item.TextBlock_CntrNo.Text.ToString());
                        item.Selected = true;
                        jobList.ListBox_Job.SelectedItem = item;
                        if (jobList.ListBox_Job.SelectedItem != null)
                            (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                        if (this.NeedJobAutoSelection)
                        {
                            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
                            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = item.JobKey;
                            this.NeedJobAutoSelection = false;
                        }
                        SaveLog("JL_Refresh End: " + item.TextBlock_CntrNo.Text.ToString());
                        break;
                    }
                }
            }

            if (list_Joborder.Count != 0 && list_Joborder.Count % jobList.pageItemCount == 0)
                jobList.TotalPageCount = (list_Joborder.Count / jobList.pageItemCount);
            else
                jobList.TotalPageCount = (list_Joborder.Count / jobList.pageItemCount) + 1;

            jobList.TextBlock_PageNum.Text = (jobList.CurrentPageIndex + 1).ToString() + "/" + jobList.TotalPageCount.ToString();

            if (jobList.TotalPageCount < 1)
            {
                jobList.Btn_PageDown.IsEnabled = false;
                jobList.Btn_PageUp.IsEnabled = false;
            }
            else
            {
                if (jobList.CurrentPageIndex == 0)
                    jobList.Btn_PageUp.IsEnabled = false;
                else
                    jobList.Btn_PageUp.IsEnabled = true;

                if (jobList.TotalPageCount == jobList.CurrentPageIndex + 1)
                    jobList.Btn_PageDown.IsEnabled = false;
                else
                    jobList.Btn_PageDown.IsEnabled = true;
            }
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;

            if (jobList.ListBox_Job.SelectedItem == null && String.IsNullOrEmpty(targetJobKey)) // 현재 리스트에서 아이템이 선택되지 않았고, 유지가 필요한 아이템도 없다면
                RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;
            else
            {
                PresentationMgr.MainView.CheckupButtonStatus();
                //PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo();
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.RefreshTargetJobInfo();

                if (this._needRehandlingJobRefresh)
                {
                    this._needRehandlingJobRefresh = false;
                    if (targetJobOrder != null &&
                         (targetJobOrder.type.jobTp == "RH" || targetJobOrder.type.jobTp == "AH"))
                    {
                        Boolean needSetLocation = false;
                        var location = targetJobOrder.locFrom;
                        if (targetJobOrder.type.jobStatus == "P")   // from -> to
                        {
                            if (this.CurrentBlock.Equals(location.blck) && this.CurrentBay.Equals(location.bay))
                            {
                                needSetLocation = true;
                                location = targetJobOrder.locWorking;
                            }
                        }
                        else if (this.CurrentBlock.Equals(targetJobOrder.locWorking.blck) &&
                            this.CurrentBay.Equals(targetJobOrder.locWorking.bay)) // to -> From                        
                            needSetLocation = true;

                        if (needSetLocation == true)
                        {
                            if (!this.CurrentBlock.Equals(location.blck) || !this.CurrentBay.Equals(location.bay))
                            {
                                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                pos.m_cBlock = location.blck;
                                pos.m_cBay = PresentationMgr.GetFrontOddBay(location.bay);
                                pos.m_cRow = location.row;
                                pos.m_cTier = location.tier;
                                this.CurrentPostion = pos;
                            }
                            else
                            {
                                PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.NeedReset = true;
                                this.SetInventoryData(null);
                            }
                        }
                    }
                }
            }
            MainView.UC_InfomationView.setCountJobType(DataMgr.Singleton.List_JobOrder);
            if (jobList.ListBox_Job.SelectedItem == null)
            {
                this.swapList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                this.reservedList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                this.swapListRTG = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();
            }
            list_Joborder.Clear();
            list_Joborder = null;
        }

        public void SaveLog(string sJob)  // nDataType 0 EEv2JobOrder, 
        {
            try
            {
                //string sRootPath = AppCfgMgr.GetAppDirectory();
                //string sDirPath = sRootPath + @"JOBCLICK_log\"
                //    + System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day;
                //if (Directory.Exists(sDirPath) == false)
                //{
                //    Directory.CreateDirectory(sDirPath);
                //}
                //var dayBefore = System.DateTime.Now.AddDays(-3);

                //var oldFolderPath = sRootPath + @"JOBCLICK_log\"
                //    + dayBefore.Year + "." + dayBefore.Month + "." + dayBefore.Day;
                ////dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                //if (Directory.Exists(oldFolderPath) == true)
                //{
                //    var dir = new DirectoryInfo(oldFolderPath);
                //    dir.Delete(true);
                //}

                //string logFilePath = @sDirPath + "/Log_" + System.DateTime.Now.Hour + ".txt";

                //FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                //sw.WriteLine("//===========================================================================");
                //sw.WriteLine("[" + System.DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "]" + sJob);
                //sw.WriteLine("//===========================================================================\r\n");
                //sw.Flush();
                //sw.Close();
                //fs.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void ReselectJobItem()
        {
            if (PresentationMgr.Singleton.jobAfterSearch != null)
            {
                String cntrNo = jobAfterSearch.cntr != null ? jobAfterSearch.cntr.cntrNo : "";

                if (String.IsNullOrEmpty(cntrNo))
                {
                    jobAfterSearch = null;
                    return;
                }
                if (PresentationMgr.Singleton.CurrentRow.Equals(string.Empty))
                {
                    var jobList2 = PresentationMgr.Singleton.JOB_GetForContainerNo(cntrNo);
                    if (jobList2.Count > 0)
                    {
                        jobAfterSearch = null;
                        var job = jobList2.First();

                        if (PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem is JobListItem)
                        {
                            JobListItem item = PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem as JobListItem;
                            if (job.jobKey == item.JobKey)
                            {
                                PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = string.Empty;
                                if (PresentationMgr.Singleton.CurrentRow.Equals(string.Empty))
                                {
                                    PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
                                }
                            }
                        }

                        foreach (JobListItem jobListItem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
                        {
                            if (jobListItem.Selected)
                                jobListItem.Selected = false;
                        }

                        string jobCntrNo = job.cntr.cntrNo;
                        jobCntrNo = jobCntrNo.Length > 8 ? (jobCntrNo.Substring(0, 2) + jobCntrNo.Substring(jobCntrNo.Length - 7)) : jobCntrNo;

                        foreach (JobListItem jobListItem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
                        {
                            if (jobListItem.TextBlock_CntrNo.Text == jobCntrNo)
                            {
                                PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem = jobListItem;
                                PresentationMgr.MainView.UC_JobList.ListBox_Job_MouseUp(null, null);
                                return;
                            }
                        }
                    }
                    else
                    {
                        ReloadBayItems();
                    }
                }
                else
                {
                    ReloadBayItems();
                }
            }
        }

        private void ReloadBayItems()
        {
            if (jobAfterSearch.type == null) return;

            var location = jobAfterSearch.locWorking;
            if (PresentationMgr.UseFromLocationForRehandling == true &&
                (jobAfterSearch.type.jobTp == "RH" || jobAfterSearch.type.jobTp == "AH"))
                location = jobAfterSearch.locFrom;
            else if (jobAfterSearch.type.jobTp == "LD" || jobAfterSearch.type.jobTp == "GO" || jobAfterSearch.type.jobTp == "MO")
                location = string.IsNullOrEmpty(jobAfterSearch.locFrom.location) ? jobAfterSearch.locWorking : jobAfterSearch.locFrom;

            if (location.blck == "") return;

            var pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition()
            {
                m_cBlock = location.blck,
                m_cBay = PresentationMgr.GetFrontOddBay(location.bay),
                m_cRow = location.row,
                m_cTier = location.tier
            };

            PresentationMgr.Singleton.CurrentPostion = pos;
        }

        public void JLS_Refresh(JobListSwap jobList, Int32 pageIndex = 0, Boolean keepSelection = false)
        {

            List<VMT_Data_JAT2.Objects.Common.VmtSwap> list_Joborder = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();
            foreach (VMT_Data_JAT2.Objects.Common.VmtSwap swapJob in DataMgr.Singleton.List_swap)
            {
                list_Joborder.Add(swapJob);
            }
            var targetJobKey = RMG.RMG_Member.Singleton.TargetJobKey;
            var targetContSwap = RMG.RMG_Member.Singleton.TargetContSwap;
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder targetJobOrder = null;
            if (!String.IsNullOrEmpty(targetJobKey))
            {
                if ((targetJobOrder = this.JOB_Get(targetJobKey)) == null)
                    targetJobKey = String.Empty;
                else if (this.NeedMoveToTargetJobPage == true)
                {
                    pageIndex = PresentationMgr.Singleton.GetJobOrderIndex(targetJobOrder) / PresentationMgr.MainView.UC_JobList.pageItemCount;
                    this.NeedMoveToTargetJobPage = false;
                }
            }

            Int32 itemViewIndex = 0;
            Int32 joborderIndex = pageIndex * jobList.pageItemCount;
            if (joborderIndex < 0)
                joborderIndex = 0;

            if (joborderIndex > list_Joborder.Count)
            {
                JLS_Refresh(jobList, 0, true);
                return;
            }

            jobList.ListBox_Job.Items.Clear();
            jobList.CurrentPageIndex = pageIndex;

            for (;
                joborderIndex < list_Joborder.Count && itemViewIndex < (jobList.CurrentPageIndex + 1) * jobList.pageItemCount;
                joborderIndex++)
            {
                if (itemViewIndex >= jobList.swapListItems.Count)
                    break;

                jobList.swapListItems[itemViewIndex].SetJobInfo(list_Joborder[joborderIndex], joborderIndex);
                jobList.swapListItems[itemViewIndex].Selected = false;
                //JobListItem item = new JobListItem();
                //item.SetJobInfo(list_Joborder[joborderIndex].jobKey, joborderIndex);
                //jobList.ListBox_Job.Items.Add(item);
                jobList.ListBox_Job.Items.Add(jobList.swapListItems[itemViewIndex]);
                itemViewIndex++;
            }
            if (String.IsNullOrEmpty(targetContSwap)) // 현재 선택된 job item이 없을 경우   (첫번째 활성화 가능한 아이템을 선택)
            {
                if (this.NeedJobAutoSelection)
                {
                    if (jobList.ListBox_Job.Items.Count > 0)
                    {
                        SwapListItem item = jobList.ListBox_Job.Items.GetItemAt(0) as SwapListItem;
                        if (item != null)
                        {
                            if (item.contNo.Text != null && item.contNo.Text != "")
                            {
                                jobList.ListBox_Job.SelectedIndex = 0;
                                if (jobList.ListBox_Job.SelectedItem != null)
                                {
                                    (jobList.ListBox_Job.SelectedItem as JobListItem).Selected = true;
                                    VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetContSwap = item.contNo.Text;
                                }
                                this.NeedJobAutoSelection = false;
                            }
                        }
                    }
                }
            }
            else                                    // 이전에 선택되어 있는 job item이 있을 경우 (현재 리스트에 없으면 selection을 만들지 않음)
            {
                foreach (var listitem in jobList.ListBox_Job.Items)
                {
                    SwapListItem item = listitem as SwapListItem;
                    //item.Selected = true;
                    if (jobList.ListBox_Job.SelectedItem != null && String.Compare(targetContSwap, item.contNo.Text, true) == 0)
                    {
                        jobList.ListBox_Job.SelectedItem = item;
                        if (jobList.ListBox_Job.SelectedItem != null)
                            (jobList.ListBox_Job.SelectedItem as SwapListItem).Selected = true;
                        break;
                    }
                }
            }

            if (list_Joborder.Count != 0 && list_Joborder.Count % jobList.pageItemCount == 0)
                jobList.TotalPageCount = (list_Joborder.Count / jobList.pageItemCount);
            else
                jobList.TotalPageCount = (list_Joborder.Count / jobList.pageItemCount) + 1;

            jobList.TextBlock_PageNum.Text = (jobList.CurrentPageIndex + 1).ToString() + "/" + jobList.TotalPageCount.ToString();

            if (jobList.TotalPageCount < 1)
            {
                jobList.Btn_PageDown.IsEnabled = false;
                jobList.Btn_PageUp.IsEnabled = false;
            }
            else
            {
                if (jobList.CurrentPageIndex == 0)
                    jobList.Btn_PageUp.IsEnabled = false;
                else
                    jobList.Btn_PageUp.IsEnabled = true;

                if (jobList.TotalPageCount == jobList.CurrentPageIndex + 1)
                    jobList.Btn_PageDown.IsEnabled = false;
                else
                    jobList.Btn_PageDown.IsEnabled = true;
            }
            this.IDV_JobCount = DataMgr.Singleton.List_JobOrder.Count;
            if (jobList.ListBox_Job.SelectedItem == null && String.IsNullOrEmpty(targetContSwap)) // 현재 리스트에서 아이템이 선택되지 않았고, 유지가 필요한 아이템도 없다면
                RMG.RMG_Member.Singleton.TargetContSwap = String.Empty;
            else
            {
                //PresentationMgr.MainView.CheckupButtonStatus();
                //PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo();
                PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_TargetJobInfo.RefreshTargetJobInfo();

                if (this._needRehandlingJobRefresh)
                {
                    this._needRehandlingJobRefresh = false;
                    if (targetJobOrder != null &&
                         (targetJobOrder.type.jobTp == "RH" || targetJobOrder.type.jobTp == "AH"))
                    {
                        Boolean needSetLocation = false;
                        var location = targetJobOrder.locFrom;
                        if (targetJobOrder.type.jobStatus == "P")   // from -> to
                        {
                            if (this.CurrentBlock.Equals(location.blck) && this.CurrentBay.Equals(location.bay))
                            {
                                needSetLocation = true;
                                location = targetJobOrder.locWorking;
                            }
                        }
                        else if (this.CurrentBlock.Equals(targetJobOrder.locWorking.blck) &&
                            this.CurrentBay.Equals(targetJobOrder.locWorking.bay)) // to -> From                        
                            needSetLocation = true;

                        if (needSetLocation == true)
                        {
                            if (!this.CurrentBlock.Equals(location.blck) || !this.CurrentBay.Equals(location.bay))
                            {
                                VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                                pos.m_cBlock = location.blck;
                                pos.m_cBay = PresentationMgr.GetFrontOddBay(location.bay);
                                pos.m_cRow = location.row;
                                pos.m_cTier = location.tier;
                                this.CurrentPostion = pos;
                            }
                            else
                            {
                                PresentationMgr.MainView.UC_SwapView.UC_NavigatorView.NeedReset = true;
                                this.SetInventoryDataSwap(null);
                            }
                        }
                    }
                }
            }
            list_Joborder.Clear();
            list_Joborder = null;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> GetFilteredJobList(JobFilter filter)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> filteredJobList = null;
            filteredJobList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            foreach (var job in DataMgr.Singleton.List_JobOrder)
            {
                var location = job.locWorking;
                if (PresentationMgr.UseFromLocationForRehandling == true &&
                    (job.type.jobTp == "RH" || job.type.jobTp == "AH") &&
                    job.type.jobStatus != "P")
                    location = job.locFrom;
                else if (job.type.jobTp == "LD" || job.type.jobTp == "GO" || job.type.jobTp == "MO")
                    location = string.IsNullOrEmpty(job.locFrom.location) ? job.locWorking : job.locFrom;

                if (true == filter.FilterJobActive &&
                    (job.type.jobStatus != "A" && job.type.jobStatus != "P"))
                    continue;
                //else if (true == filter.FilterJobBlock &&  Khoa.Nguyen 2018/09/17
                //    !location.blck.Equals(this.CurrentBlock))
                //    continue;
                else if (true == filter.FilterJobBlock &&
                    !location.blck.Equals(MainView.currentBlock))
                    continue;
                else if (filter.FilterJobType != JobFilter.TYPE_ALL)
                {
                    if (0 == (filter.FilterJobType & JobFilter.TYPE_GI) &&
                        job.type.jobTp == "GI")
                        continue;
                    else if (0 == (filter.FilterJobType & JobFilter.TYPE_GO) &&
                        (job.type.jobTp == "GO" || job.type.jobTp == "GC"))
                        continue;
                    else if (0 == (filter.FilterJobType & JobFilter.TYPE_MI) &&
                        job.type.jobTp == "MI")
                        continue;
                    else if (0 == (filter.FilterJobType & JobFilter.TYPE_MO) &&
                        job.type.jobTp == "MO")
                        continue;
                    else if (0 == (filter.FilterJobType & JobFilter.TYPE_DS) &&
                        job.type.jobTp == "DS")
                        continue;
                    else if (0 == (filter.FilterJobType & JobFilter.TYPE_LD) &&
                        (job.type.jobTp == "LD" || job.type.jobTp == "LC"))
                        continue;
                    else if (0 == (filter.FilterJobType & JobFilter.TYPE_RH) &&
                        (job.type.jobTp == "RH" || job.type.jobTp == "AH"))
                        continue;
                }
                filteredJobList.Add(job);
            }

            return filteredJobList;
        }

        public static List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> GetPLCJobList()
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> filteredJobList = null;
            filteredJobList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            foreach (var job in DataMgr.Singleton.List_JobOrder)
            {
                if ((job.type.jobTp == "GI" || job.type.jobTp == "MI" || job.type.jobTp == "DS" || job.type.jobTp == "LC" || job.type.jobTp == "GC")
                    && job.locWorking.blck == PresentationMgr.Singleton.CurrentBlock
                    && !String.IsNullOrEmpty(PresentationMgr.Singleton.CurrentBay)
                    )
                {
                    if (!String.IsNullOrEmpty(job.locWorking.bay))
                    {
                        int bay = 1;

                        if (Convert.ToInt32(BayRemoveChars(job.locWorking.bay)) % 2 == 0)
                        {
                            bay = (Convert.ToInt32(BayRemoveChars(job.locWorking.bay)) - 1);
                        }
                        else
                            bay = Convert.ToInt32(BayRemoveChars(job.locWorking.bay));

                        if (bay == Convert.ToInt32(BayRemoveChars(PresentationMgr.Singleton.CurrentBay)))
                        {
                            var plcJob = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(job);
                            filteredJobList.Add(plcJob);
                        }
                    }
                }
            }

            return filteredJobList;
        }
        public static void GetPLCJobListEmptyContainer(List<VD_Common_Def_Inventory> lstCntr = null)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> filteredJobList = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            List<VD_Common_Def_Inventory> lstCntr2 = new List<VD_Common_Def_Inventory>();

            if (lstCntr != null)
            {
                lstCntr = lstCntr.OrderBy(x => x.loc.row).ToList();

                //So sorting logic is, 1. PLC msg's Row, Tier +- 1 2. Conatiner has job, 3. others 
                foreach (var cntr in lstCntr)
                {
                    int row = 0;
                    if (!Int32.TryParse(cntr.loc.row, out row))
                        row = parseCharToNum(cntr.loc.row);

                    int tier = 0;
                    if (!Int32.TryParse(cntr.loc.tier, out tier))
                        tier = parseCharToNum(cntr.loc.tier);


                    if (
                        (tier == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentTier
                        && (row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow
                        || row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow + 1
                        || row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow - 1)
                        )
                        ||
                        (tier == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentTier - 1
                        && (row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow
                        || row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow + 1
                        || row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow - 1)
                        )
                        ||
                        (tier == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentTier + 1
                        && (row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow
                        || row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow + 1
                        || row == PresentationMgr.MainView.UC_ListJobEmptyContainerView.currentRow - 1)
                        )
                    )
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job = new VD_Common_JobOrder();
                        job.cntr.cntrNo = cntr.cntr.cntrNo;
                        job.type.jobTp = cntr.jobTp;
                        job.locFrom.blck = cntr.loc.blck;
                        job.locFrom.bay = cntr.loc.bay;
                        job.locFrom.row = cntr.loc.row;
                        job.locFrom.tier = cntr.loc.tier;
                        job.locFrom.location = cntr.loc.location;

                        job.locWorking.blck = cntr.loc.blck;
                        job.locWorking.bay = cntr.loc.bay;
                        job.locWorking.row = cntr.loc.row;
                        job.locWorking.tier = cntr.loc.tier;
                        job.locWorking.location = cntr.loc.location;

                        filteredJobList.Add(job);
                    }
                    else
                    {
                        lstCntr2.Add(cntr);
                    }
                }


                lstCntr2 = lstCntr2.OrderByDescending(x => x.jobTp).ToList();
                foreach (var cntr in lstCntr2)
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job = new VD_Common_JobOrder();
                    job.cntr.cntrNo = cntr.cntr.cntrNo;
                    job.type.jobTp = cntr.jobTp;
                    job.locFrom.blck = cntr.loc.blck;
                    job.locFrom.bay = cntr.loc.bay;
                    job.locFrom.row = cntr.loc.row;
                    job.locFrom.tier = cntr.loc.tier;
                    job.locFrom.location = cntr.loc.location;

                    job.locWorking.blck = cntr.loc.blck;
                    job.locWorking.bay = cntr.loc.bay;
                    job.locWorking.row = cntr.loc.row;
                    job.locWorking.tier = cntr.loc.tier;
                    job.locWorking.location = cntr.loc.location;

                    filteredJobList.Add(job);
                }


                PresentationMgr.MainView.UC_ListJobEmptyContainerView.Visibility = Visibility.Visible;
                PresentationMgr.MainView.UC_ListJobEmptyContainerView.listContainer = filteredJobList;
                PresentationMgr.MainView.UC_ListJobEmptyContainerView.SetGridData();
            }

            //foreach (var job in DataMgr.Singleton.List_JobOrder)
            //{
            //    if (job.type.jobTp == "LD" || job.type.jobTp == "MO" || job.type.jobTp == "GO" || job.type.jobTp == "RH" || job.type.jobTp == "AH")
            //    {
            //        var plcJob = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(job);
            //        filteredJobList.Add(plcJob);
            //    }
            //}
        }

        public static int parseCharToNum(string x)
        {
            string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return (alpha.IndexOf(x) + 1);
        }


        #endregion [JobView Methods]

        //-----------------------------------------------------------------
        //- Yard View Methods Section
        //-----------------------------------------------------------------
        #region [ Stop View Methods ]

        public void STV_TriggerBayView()
        {
            //string block = this.CurrentPow.m_szBlock;
            //string bay = string.Format("{0:D2}", int.Parse(this.CurrentPow.m_szBayEst));

            //if (!DataMgr.Singleton.INV_IsInventory(block, bay))
            //    return;

            //this.UI_SwitchUI(UIMode.Stop);

            //this.STV_UpdateBayViewInventory(this.CurrentPow.m_szBayEst);

            //this.STV_UpdateJobList(bay);
        }

        public void STV_UpdateBayViewInventory(string bay)
        {
            //StopView stv = PresentationMgr.StopView;
            //BayWorkPanel bwp =  stv.UC_BayWorkPanel;

            //VMT_Data_JAT2.Common.sBlockBayInfo bbi = DataMgr.Singleton.INV_GetInventory(this.CurrentBlock, bay);

            //double rowPos = 0.0;
            //double tierPos = 0.0;

            //// Clear Block/Bay Inventory(Containers)
            //bwp.Canvas_BayWorkInventory.Children.Clear();

            //for (int k = 0; k < 49; k++)
            //{
            //    VMT_Data_JAT2.Common.EEv2_Def_Inventory inv = bbi.cntr[k];

            //    if (!string.IsNullOrEmpty(inv.cntr.cntrNo))
            //    {
            //        ContainerItem ci = new ContainerItem();
            //        ci.SetInventoryInfo(inv);

            //        // Calculate Row(X) Position
            //        int rowNum = DataMgr.ConvertRowToNumber(inv.locWorking.row);
            //        rowPos = BayWorkPanel.GetPosXByRow(rowNum);

            //        // Calculate Tier(Y) Position
            //        int tierNum = int.Parse(inv.locWorking.tier);
            //        tierPos = BayWorkPanel.GetPosYByTier(tierNum);

            //        // Set Container Position
            //        ci.SetValue(Canvas.LeftProperty, rowPos);
            //        ci.SetValue(Canvas.TopProperty, tierPos);

            //        bwp.Canvas_BayWorkInventory.Children.Add(ci);
            //    }
            //}
        }

        public void STV_UpdateJobList(string bay)
        {
            //StopView stv = PresentationMgr.StopView;

            //// Clear JobList
            //stv.UC_JobList.ListBox_Job.Items.Clear();

            //foreach(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jo in DataMgr.Singleton.List_JobOrder)
            //{
            //    if (jo.locWorking.bay == bay)
            //    {
            //        JobListItem item = new JobListItem();
            //        item.SetJobInfo(jo);
            //        // Add Right Side
            //        stv.UC_JobList.ListBox_Job.Items.Add(item);
            //    }
            //}
        }
        #endregion [ Stop View Methods ]

        //-----------------------------------------------------------------
        //- JobBanner Methods Section
        //-----------------------------------------------------------------
        #region [ JobBanner Methods ]

        public void JBN_SetTargetJob_Clear()
        {
            RTG.RTG_Member.Singleton.TargetJobKey = String.Empty;
            //PresentationMgr.MovingView.UC_MovingViewJobBanner.SetTargetJobClear();
        }

        public void JBN_AutoTargetJob()
        {
            return;

            // Set Target Job
            // If there is aleady same container exist then ignore it.
            //if (DataMgr.Singleton.List_JobOrder.Count == 0)
            //    PresentationMgr.Singleton.JBN_SetTargetJob_Clear();

            //foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder in DataMgr.Singleton.List_JobOrder)
            //{
            //    if (!String.IsNullOrEmpty(jobOrder.partnerMchn.mchnId))
            //    {
            //        PresentationMgr.Singleton.JBN_SetTargetJob(jobOrder.jobKey);
            //        break;
            //    }
            //}
        }

        public void JBN_SetTargetJob(String jobKey)
        {
            RMG.RMG_Member.Singleton.TargetJobKey = jobKey;
            RMG.RMG_Member.Singleton.TargetJobOrder = this.JOB_Get(jobKey);

            {
                //PresentationMgr.MovingView.UC_MovingViewJobBanner.SetTargetJob(jo);

                //// Show Target Job Markder
                //PresentationMgr.YardView.UC_BlockControl_L.UC_BlockMapControl.Image_JobTarget.Visibility = System.Windows.Visibility.Hidden;
                //PresentationMgr.YardView.UC_BlockControl_C.UC_BlockMapControl.Image_JobTarget.Visibility = System.Windows.Visibility.Visible;
                //PresentationMgr.YardView.UC_BlockControl_R.UC_BlockMapControl.Image_JobTarget.Visibility = System.Windows.Visibility.Hidden;

                //int bayNum = int.Parse(jo.locWorking.bay);
                //int rowNum = DataMgr.ConvertRowToNumber(jo.locWorking.row);

                //double left = ((bayNum - 1) / 2) * 15 + (15/2);
                //double top = (rowNum - 1) * 11 + (11/2);

                //Image imgMarker = PresentationMgr.YardView.UC_BlockControl_C.UC_BlockMapControl.Image_JobTarget;

                //imgMarker.SetValue(Canvas.LeftProperty, left);
                //imgMarker.SetValue(Canvas.TopProperty, top);
            }
        }

        #endregion [ JobBanner methods ]

        //----------------------------------------------------------------
        //- Common UI Helper Method
        //----------------------------------------------------------------
        #region [ Common UI Helper Method ]
        public Dictionary<String, BitmapImage> Dic_images = new Dictionary<string, BitmapImage>();
        public static BitmapImage GetImageSource(String name)
        {
            String imagePath;
            BitmapImage bitmap = null;

            //imagePath = @"pack://application:,,,/VMT_SC;component" + name;
            imagePath = @"/VMT_RTG;component/" + name;

            try
            {
                if (PresentationMgr.Singleton.Dic_images.ContainsKey(imagePath))
                    bitmap = PresentationMgr.Singleton.Dic_images[imagePath];
                else
                {
                    bitmap = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                    PresentationMgr.Singleton.Dic_images.Add(imagePath, bitmap);
                }
                //bitmap = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //SAVE LOG
            }


            return bitmap;
        }

        public static void SetSkinButton(Button btnObj, BitmapImage bmpDef, BitmapImage bmpPress, BitmapImage bmpDisable = null)
        {
            Image img = null;

            img = btnObj.Template.FindName("Image_Default", btnObj) as Image;
            //btnObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpDef;

            img = btnObj.Template.FindName("Image_Press", btnObj) as Image;
            //btnObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpPress;

            img = btnObj.Template.FindName("Image_Disable", btnObj) as Image;
            //btnObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpDisable;

        }

        public static void SetSkinRadioButton(RadioButton btnObj, BitmapImage bmpCheck, BitmapImage bmpUncheck, BitmapImage bmpDisable = null)
        {
            Image img = null;

            img = btnObj.Template.FindName("Image_Check", btnObj) as Image;
            //btnObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpCheck;

            img = btnObj.Template.FindName("Image_Uncheck", btnObj) as Image;
            //btnObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpUncheck;

            img = btnObj.Template.FindName("Image_Disable", btnObj) as Image;
            //btnObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpDisable;

        }

        public static void SetSkinCheckBox(CheckBox cbObj, BitmapImage bmpCheck, BitmapImage bmpUncheck, BitmapImage bmpDisable = null)
        {
            Image img = null;

            img = cbObj.Template.FindName("Image_Check", cbObj) as Image;
            //cbObj.Template.Resources.Clear();
            //if(img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpCheck;

            img = cbObj.Template.FindName("Image_Uncheck", cbObj) as Image;
            //cbObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpUncheck;

            img = cbObj.Template.FindName("Image_Disable", cbObj) as Image;
            //cbObj.Template.Resources.Clear();
            //if (img.Source != null) img.Source.Freeze();
            if (img != null) img.Source = bmpDisable;

        }

        public static void ShowPartnerMachineSearchPopup(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = null)
        {
            if (PresentationMgr.MainView.UC_MachineSearchView.Visibility == System.Windows.Visibility.Hidden)
            {
                PresentationMgr.MainView.UC_MachineSearchView.CurrentJob = jobOrder;

                PresentationMgr.MainView.UC_MachineSearchView.Visibility = System.Windows.Visibility.Visible;
            }
            else
                PresentationMgr.MainView.UC_MachineSearchView.Visibility = System.Windows.Visibility.Hidden;
            //PresentationMgr.AppWin.ShowProgressBar(0);
        }

        public static void ShowYtSwapPopup(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = null)
        {
            if ("GI".Equals(jobOrder.type.jobTp) || "GO".Equals(jobOrder.type.jobTp))
            {
                if (PresentationMgr.AppWin.MainWin.wrkCd == "2")
                {
                    PresentationMgr.AppWin.MainWin.wrkCd = "";
                }
                return;
            }

            if (PresentationMgr.MainView.UC_YtSwapView.Visibility == System.Windows.Visibility.Hidden)
            {
                PresentationMgr.MainView.UC_YtSwapView.CurrentJob = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(jobOrder);
                PresentationMgr.MainView.UC_YtSwapView.Visibility = System.Windows.Visibility.Visible;
                if (jobOrder != null)
                    PresentationMgr.MainView.UC_YtSwapView.SetJobInfo(jobOrder);
            }
            else
                PresentationMgr.MainView.UC_YtSwapView.Visibility = System.Windows.Visibility.Hidden;
            //PresentationMgr.AppWin.ShowProgressBar(0);
        }

        public static FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;

        }


        public static T FindChild<T>(DependencyObject depObj, string childName)
            where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null) return null;

            // success case
            if (depObj is T && ((FrameworkElement)depObj).Name == childName)
                return depObj as T;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                T obj = FindChild<T>(child, childName);

                if (obj != null)
                    return obj;
            }

            return null;
        }

        public static int FindChildByType<T>(DependencyObject depObj, List<T> objList)
            where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null) return 0;

            // success case
            if (depObj is T)
            {
                objList.Add(depObj as T);
                return objList.Count;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                int findCount = FindChildByType<T>(child, objList);
            }

            return objList.Count;
        }

        public void SetBlockSelection(String block)
        {
            WrapPanel WrapBlockSelectionView = PresentationMgr.MainView.UC_BlockSelectionView.Wrap_BlockSelectionView;
            foreach (UIElement uiElement in WrapBlockSelectionView.Children)
            {
                if (uiElement is BlockJobControl)
                {
                    BlockJobControl bjc = (BlockJobControl)uiElement;
                    bjc.IsSelected = bjc.BlckName == block ? true : false;
                }
            }
            PresentationMgr.MainView.UC_BlockSelectionView.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BlockSelectionView.Btn_Next.IsEnabled = true;

            if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].IsVirtual)
                PresentationMgr.MainView.UC_BlockSelectionView.Btn_Next.Content = "DONE";
            else
                PresentationMgr.MainView.UC_BlockSelectionView.Btn_Next.Content = "NEXT";
        }

        public void SetVirtualBlockSelection(String block)
        {
            WrapPanel WrapBlockSelectionView = PresentationMgr.MainView.UC_VirtualBlockSelectionView.Wrap_BlockSelectionView;
            foreach (UIElement uiElement in WrapBlockSelectionView.Children)
            {
                if (uiElement is VirtualBlockJobControl)
                {
                    VirtualBlockJobControl bjc = (VirtualBlockJobControl)uiElement;
                    bjc.IsSelected = bjc.BlckName == block ? true : false;
                }
            }
            PresentationMgr.MainView.UC_VirtualBlockSelectionView.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBlockName = block;
            PresentationMgr.MainView.UC_VirtualBlockSelectionView.Btn_Next.IsEnabled = true;

            if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].IsVirtual)
                PresentationMgr.MainView.UC_VirtualBlockSelectionView.Btn_Next.Content = "DONE";
            else
                PresentationMgr.MainView.UC_VirtualBlockSelectionView.Btn_Next.Content = "NEXT";
        }

        public void SetBlockSelection1(String block)
        {
            WrapPanel WrapBlockSelectionView = PresentationMgr.MainView.UC_BlockSelectionView1.Wrap_BlockSelectionView;
            foreach (UIElement uiElement in WrapBlockSelectionView.Children)
            {
                if (uiElement is BlockJobControl1)
                {
                    BlockJobControl1 bjc = (BlockJobControl1)uiElement;
                    bjc.IsSelected = bjc.BlckName == block ? true : false;
                }
            }
            PresentationMgr.MainView.UC_BlockSelectionView1.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BaySelectionView1.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BlockSelectionView1.Btn_Next.IsEnabled = true;

            if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].IsVirtual)
                PresentationMgr.MainView.UC_BlockSelectionView1.Btn_Next.Content = "DONE";
            else
                PresentationMgr.MainView.UC_BlockSelectionView1.Btn_Next.Content = "NEXT";
        }

        public void SetBlockSelection2(String block)
        {
            WrapPanel WrapBlockSelectionView = PresentationMgr.MainView.UC_BlockSelectionView2.Wrap_BlockSelectionView;
            foreach (UIElement uiElement in WrapBlockSelectionView.Children)
            {
                if (uiElement is BlockJobControl2)
                {
                    BlockJobControl2 bjc = (BlockJobControl2)uiElement;
                    bjc.IsSelected = bjc.BlckName == block ? true : false;
                }
            }
            PresentationMgr.MainView.UC_BlockSelectionView2.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BaySelectionView2.SelectedBlockName = block;
            PresentationMgr.MainView.UC_BlockSelectionView2.Btn_Next.IsEnabled = true;

            if (DataMgr.Singleton.SimpleBlockBayInfo.DicBlock.ContainsKey(block) &&
                DataMgr.Singleton.SimpleBlockBayInfo.DicBlock[block].IsVirtual)
                PresentationMgr.MainView.UC_BlockSelectionView2.Btn_Next.Content = "DONE";
            else
                PresentationMgr.MainView.UC_BlockSelectionView2.Btn_Next.Content = "NEXT";
        }

        public void SetBaySelection(String bay)
        {
            WrapPanel Wrap_BaySelectionView = PresentationMgr.MainView.UC_BaySelectionView.Wrap_BaySelectionView;
            foreach (UIElement uiElement in Wrap_BaySelectionView.Children)
            {
                if (uiElement is BayJobControl)
                {
                    BayJobControl bjc = (BayJobControl)uiElement;
                    bjc.IsSelected = bjc.BayName == bay ? true : false;
                }
            }
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBayName = bay;
            PresentationMgr.MainView.UC_BaySelectionView.Btn_Done_Two.IsEnabled = true;
        }

        public void SetBlockBaySelection(String bay)
        {
            WrapPanel Wrap_BaySelectionView = PresentationMgr.MainView.UC_BlockSelectionView.Wrap_BaySelectionView;
            foreach (UIElement uiElement in Wrap_BaySelectionView.Children)
            {
                if (uiElement is BayControl)
                {
                    BayControl bjc = (BayControl)uiElement;
                    bjc.IsSelected = bjc.BayName == bay ? true : false;
                }
            }

            PresentationMgr.MainView.UC_BlockSelectionView.SelectedBayName = bay;
            PresentationMgr.MainView.UC_BaySelectionView.SelectedBayName = bay;
            PresentationMgr.MainView.UC_BaySelectionView.Btn_Done_Two.IsEnabled = true;

            //20201020 new API getBlockListForYardSector (VIEW_INV = true && VIEW_BLOCK_LIST = false) when select block +bay in blockbay screen
            if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList && !PresentationMgr.MainView.UC_BlockSelectionView.TextBox_BlockID.Text.Equals(PresentationMgr.Singleton.CurrentBlock))
            {
                VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockListForYardSector_Ask(VMT_Data_JAT2.Objects.UserInfo.gMchnID, PresentationMgr.MainView.UC_BlockSelectionView.TextBox_BlockID.Text);
            }
        }

        public void SetBaySelection1(String bay)
        {
            WrapPanel Wrap_BaySelectionView = PresentationMgr.MainView.UC_BaySelectionView1.Wrap_BaySelectionView;
            foreach (UIElement uiElement in Wrap_BaySelectionView.Children)
            {
                if (uiElement is BayJobControl1)
                {
                    BayJobControl1 bjc = (BayJobControl1)uiElement;
                    bjc.IsSelected = bjc.BayName == bay ? true : false;
                }
            }
            PresentationMgr.MainView.UC_BaySelectionView1.SelectedBayName = bay;
            PresentationMgr.MainView.UC_BaySelectionView1.Btn_Done_Two.IsEnabled = true;
        }

        public void SetBaySelection2(String bay)
        {
            WrapPanel Wrap_BaySelectionView = PresentationMgr.MainView.UC_BaySelectionView2.Wrap_BaySelectionView;
            foreach (UIElement uiElement in Wrap_BaySelectionView.Children)
            {
                if (uiElement is BayJobControl2)
                {
                    BayJobControl2 bjc = (BayJobControl2)uiElement;
                    bjc.IsSelected = bjc.BayName == bay ? true : false;
                }
            }
            PresentationMgr.MainView.UC_BaySelectionView2.SelectedBayName = bay;
            PresentationMgr.MainView.UC_BaySelectionView2.Btn_Done_Two.IsEnabled = true;
        }

        public class CorrectionSelectInfo
        {
            public VMT_Data_JAT2.Marshalling.Geometry.sPosition Pos;
            public String CntrNo;
            public String CntrIso;
            public bool LDMOGOjob;

            public CorrectionSelectInfo()
            {
                Pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                Clear();
            }

            public void SetPos(VMT_Data_JAT2.Marshalling.Geometry.sPosition p)
            {
                this.Pos.m_cBlock = p.m_cBlock;
                this.Pos.m_cBay = p.m_cBay;
                this.Pos.m_cRow = p.m_cRow;
                this.Pos.m_cTier = p.m_cTier;
            }

            public void Clear()
            {
                if (PresentationMgr.Singleton != null)
                {
                    //PresentationMgr.Singleton.swapList.Clear();
                    //PresentationMgr.Singleton.reservedList.Clear();
                    //PresentationMgr.Singleton.swapListRTG.Clear();
                }
                this.Pos.Clear();
                this.CntrNo = String.Empty;
                this.CntrNo = String.Empty;
                this.LDMOGOjob = false;
            }
        }

        public CorrectionSelectInfo CorrectionSource = new CorrectionSelectInfo();

        public Boolean SetCorrectionSelect(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos, String cntrNo, String cntrIso)
        {
            if (pos == null || this.CorrectionSource.Pos.Equal(pos))        // 현재 correction source 해제
            {
                if (pos == null)
                {
                    PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.ClearContainerCorrectionSelect();
                }
                this.CorrectionSource.Clear();
                //PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                //PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;

                if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                {
                    //PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;
                    if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                        PresentationMgr.Singleton.ThreadTimerStart(false);
                }

                return false;
            }
            else
            {
                if (!this.CorrectionSource.Pos.IsEmpty())// 현재 설정된 correction source가 있을 경우  
                {
                    PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();
                    PresentationMgr.MainView.UC_SwapView.UC_BayView.ClearContainerCorrectionSelect();
                }

                this.CorrectionSource.SetPos(pos);
                this.CorrectionSource.CntrNo = cntrNo;
                this.CorrectionSource.CntrIso = cntrIso;
                //PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;
                //PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;

                //PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = false;

                return true;
            }
        }

        public void SetCorrectionSelectArea(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos, String cntrNo, String cntrIso)
        {
            PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;
            if (!this.CorrectionSource.Pos.IsEmpty())// 현재 설정된 correction source가 있을 경우  
                PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();

            this.CorrectionSource.SetPos(pos);
            this.CorrectionSource.CntrNo = cntrNo;
            this.CorrectionSource.CntrIso = cntrIso;
        }

        public Boolean SetCorrectionSelectMoving(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos, String cntrNo, String cntrIso)
        {
            if (pos == null || this.CorrectionSource.Pos.Equal(pos))        // 현재 correction source 해제
            {
                if (pos == null)
                {
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ClearContainerCorrectionSelect();
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ClearContainerCorrectionSelect();
                }
                this.CorrectionSource.Clear();
                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;

                return false;
            }
            else
            {
                if (!this.CorrectionSource.Pos.IsEmpty())// 현재 설정된 correction source가 있을 경우  
                {
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ClearContainerCorrectionSelect();
                    PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ClearContainerCorrectionSelect();
                }

                this.CorrectionSource.SetPos(pos);
                this.CorrectionSource.CntrNo = cntrNo;
                this.CorrectionSource.CntrIso = cntrIso;
                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;
                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;

                return true;
            }
        }

        public void MakeCorrection(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos)
        {
            if (MainView.UC_VBlockView.general &&
                pos != null &&
                !String.IsNullOrEmpty(pos.m_cBlock))
            {
                MainView.UC_VBlockView.general = false;
                var target = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                target.cntr.cntrNo = MainView.UC_VBlockView.containerSelected.cntrNo;
                target.cntr.cntrIso = MainView.UC_VBlockView.containerSelected.cntrIso;
                target.type.jobTp = "RH";

                target.locWorking.blck = pos.m_cBlock;
                if (MainView.UC_VBlockView.containerSelected.cntrIso.StartsWith("2"))
                    target.locWorking.bay = pos.m_cBay;
                else
                    target.locWorking.bay = PresentationMgr.GetRearEvenBay(pos.m_cBay);

                target.locWorking.row = pos.m_cRow;
                target.locWorking.tier = pos.m_cTier;

                target.workingMchn.mchnId = RMG.RMG_User.gMchnID;
                target.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;
                target.type.ycTwinKey = "";

                this.SendJobDoneAsk(target);
            }
            else if (!this.CorrectionSource.Pos.IsEmpty() &&    // correction select된 source가 있다면
                pos != null &&
                !String.IsNullOrEmpty(pos.m_cBlock) //&& !String.IsNullOrEmpty(pos.m_cBay) && !String.IsNullOrEmpty(pos.m_cRow) && !String.IsNullOrEmpty(pos.m_cTier)
                )
            {
                if (!String.IsNullOrEmpty(this.CorrectionSource.CntrNo))
                {
                    var target = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                    target.cntr.cntrNo = this.CorrectionSource.CntrNo;
                    target.cntr.cntrIso = this.CorrectionSource.CntrIso;
                    target.type.jobTp = "RH";

                    target.locWorking.blck = pos.m_cBlock;
                    if (this.CorrectionSource.CntrIso.StartsWith("2"))
                        target.locWorking.bay = pos.m_cBay;
                    else
                        target.locWorking.bay = PresentationMgr.GetRearEvenBay(pos.m_cBay);

                    target.locWorking.row = pos.m_cRow;
                    target.locWorking.tier = pos.m_cTier;

                    target.workingMchn.mchnId = RMG.RMG_User.gMchnID;
                    target.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;
                    target.type.ycTwinKey = "";
                    if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.SwapView)
                        PresentationMgr.MainView.UC_SwapView.UC_JobListSwap.SwapAutoSelection = true;
                    this.SendJobDoneAsk(target);
                }

                PresentationMgr.MainView.UC_BayView.ClearContainerCorrectionSelect();
                PresentationMgr.MainView.UC_SwapView.UC_BayView.ClearContainerCorrectionSelect();
                //PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                //PresentationMgr.MainView.UC_SwapView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;

                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView1.ClearContainerCorrectionSelect();
                PresentationMgr.MainView.UC_ContainerMoving.UC_BayView2.ClearContainerCorrectionSelect();
                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView1.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                PresentationMgr.MainView.UC_ContainerMoving.UC_InfomationView2.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                this.CorrectionSource.Clear();
                //this.ThreadTimerStart();
            }
        }

        public static void ScaleFont(ref Button btn)
        {
            var fakeImage = new System.Drawing.Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
            var graphics = System.Drawing.Graphics.FromImage(fakeImage);

            var extent = graphics.MeasureString(btn.Content.ToString(), new System.Drawing.Font(btn.FontFamily.ToString(), (float)btn.FontSize));

            float hRatio = (float)(btn.RenderSize.Height / extent.Height);
            float wRatio = (float)(btn.RenderSize.Width / extent.Width);
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = (float)btn.FontSize * ratio;

            btn.FontSize = newSize;
        }

        public static void ApplyGetMachineStop(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value)
        {
            PresentationMgr.MainView.UC_JobList.IsAvailableBreaking = true;

            PresentationMgr.MainView.UC_BreakTimeView.Grid_break_End_Time.Visibility = Visibility.Visible;
            PresentationMgr.MainView.UC_BreakTimeView.Grid_break_Reason.Visibility = Visibility.Visible;
            PresentationMgr.MainView.UC_BreakTimeView.Btn_Complete.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0027", LanguageService.LABEL_BREAKTIME);

            PresentationMgr.MainView.UC_BreakTimeView.TextBlock_Break_Start_Date.Text =
                    DateTime.ParseExact(Convert.ToString(value.StartTime), "yyyyMMddHHmmss", null).ToString("yyyy/MM/dd HH:mm:ss");
            PresentationMgr.MainView.UC_BreakTimeView.ReasonNm = value.Data.ReasonNm;
            PresentationMgr.MainView.UC_BreakTimeView.ReasonCd = value.Data.ReasonCd;
            PresentationMgr.MainView.Btn_Available.Content = value.Data.ReasonNm;
            PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Available.Content = value.Data.ReasonNm;
            PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.BreakTimeView);
            //PresentationMgr.ScaleFont(ref PresentationMgr.MainView.UC_JobList.Btn_Available);
        }

        public static void ApplySetMachineStop(String value)
        {
            if (String.IsNullOrEmpty(value))// || PresentationMgr.MainView.UC_JobList.IsAvailableBreaking)
            {
                PresentationMgr.MainView.UC_JobList.IsAvailableBreaking = false;

                PresentationMgr.MainView.Btn_Available.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0231", LanguageService.LABEL_CUSTOMIZE);
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_Available.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0231", LanguageService.LABEL_CUSTOMIZE);
                PresentationMgr.MainView.UC_BreakTimeView.Grid_break_End_Time.Visibility = Visibility.Hidden;
                PresentationMgr.MainView.UC_BreakTimeView.Grid_break_Reason.Visibility = Visibility.Hidden;
                PresentationMgr.MainView.UC_BreakTimeView.Btn_Complete.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0026", LanguageService.LABEL_BREAKTIME);
            }
            else
            {
                PresentationMgr.MainView.UC_JobList.IsAvailableBreaking = true;

                PresentationMgr.MainView.UC_BreakTimeView.Grid_break_End_Time.Visibility = Visibility.Visible;
                PresentationMgr.MainView.UC_BreakTimeView.Grid_break_Reason.Visibility = Visibility.Visible;
                PresentationMgr.MainView.UC_BreakTimeView.Btn_Complete.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0027", LanguageService.LABEL_BREAKTIME);

                PresentationMgr.MainView.Btn_Available.Content = PresentationMgr.MainView.UC_BreakTimeView.ReasonNm;
                if (value.Length > 14)
                    value = value.Substring(value.Length - 17);
                try
                {
                    PresentationMgr.MainView.UC_BreakTimeView.TextBlock_Break_Start_Date.Text =
                        DateTime.ParseExact(value, "yyyyMMddHHmmssfff", null).ToString("yyyy/MM/dd HH:mm:ss");
                }
                catch
                {
                    Common.Util.Logger.Log("Not exact datetime format : " + value);
                }
            }
            //PresentationMgr.ScaleFont(ref PresentationMgr.MainView.UC_JobList.Btn_Available);
        }

        public void RefreshJobInventory(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder, Boolean IsFromTo)
        {
            if (PresentationMgr.UseFromLocationForRehandling == true &&
                jobOrder != null &&
                (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH"))
            {
                this._needRehandlingJobRefresh = true;
            }
        }

        #endregion [ Common UI Helper Method ]


        #region [ Common Helper Method ]

        public static void ThreadStartingSplash()
        {
            //PleaseWaitDialog waitDlg = new PleaseWaitDialog();
            //waitDlg.Topmost = true;
            //waitDlg.ShowInTaskbar = false;
            ////IntPtr ownerWindowHandle = new WindowInteropHelper(m_splash).Handle;
            ////// Set the owned WPF window?�s owner with the non-WPF owner window
            ////WindowInteropHelper helper = new WindowInteropHelper(m_splash);
            ////helper.Owner = ownerWindowHandle;

            //MainFrame mf = PresentationMgr.Singleton.GetUIComponent("MPlayer.MainFrame") as MainFrame;
            //if ( mf != null && mf.IsVisible)
            //{
            //    waitDlg.WindowStartupLocation = WindowStartupLocation.Manual;
            //    double left = mf.normalWinLeft + (mf.normalWinWidth - waitDlg.ActualWidth) / 2;
            //    double top = mf.normalWinTop + (mf.normalWinHeight - waitDlg.ActualHeight) / 2;
            //    waitDlg.Left = left; 
            //    waitDlg.Top = top - 100; 
            //}

            //waitDlg.ShowDialog();
            //Thread.Sleep(1000);
        }

        public static void ExecuteProcess(string fileName, string arg, bool bShow = false)
        {
            //-----------------------------------------
            //- Run ContentPresenter with ContentName

            Process cp = new Process();

            if (string.IsNullOrEmpty(fileName))
                return;

            String filePath = AppCfgMgr.GetAppDirectory() + fileName;

            FileInfo fi = new FileInfo(filePath);

            if (!fi.Exists)
                return;

            string dir = fi.DirectoryName;

            cp.StartInfo.WorkingDirectory = dir;
            cp.StartInfo.FileName = filePath;
            cp.StartInfo.Arguments = arg;

            if (!bShow)
                cp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                cp.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static int ConvertRowToNumber(string row)
        {
            if (row.Equals("Z"))
                return 1;
            int asciiMinus = 63;
            if (row.CompareTo("I") >= 0)
            {
                asciiMinus++;
                if (row.CompareTo("O") >= 0)
                    asciiMinus++;
            }
            int rowNum = (int)row.ToUpper()[0] - asciiMinus;
            return rowNum > 0 ? rowNum : 0;
        }

        public static int ConvertRowToNumber(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> dic, string rowName,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            var reverseCnt = dic.Count - 1;
            foreach (var pair in dic)
            {
                if (rowName.Equals(pair.Value.rowNm))
                {
                    if (direction == VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
                        return pair.Key;
                    else // if (direction == VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockInfo.Row_Direction.BT)
                        return reverseCnt;
                }
                reverseCnt--;
            }

            return -1;
        }

        public static string ConvertNumberToRow(int rowNum)
        {
            if (rowNum == 1)
                return "Z";
            int asciiPlus = 63;
            if (rowNum + asciiPlus >= (int)'I')
            {
                if (rowNum + asciiPlus >= (int)'O')
                    asciiPlus++;
                asciiPlus++;
            }
            return Convert.ToChar(rowNum + asciiPlus).ToString();
        }

        public static string ConvertNumberToRow(SortedDictionary<int, VMT_Data_JAT2.Objects.Common.VD_Common_SimpleRowInfo> dic, int order,
            VMT_Data_JAT2.Objects.Common.Row_Direction direction = VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
        {
            if (dic.Count <= order || dic.Count == 0)
                return String.Empty;
            else
            {
                if (direction == VMT_Data_JAT2.Objects.Common.Row_Direction.TB)
                    return dic.Values.ToList()[order].rowNm;
                else
                    return dic.Values.ToList()[dic.Count - 1 - order].rowNm;
            }
        }

        public static String GetNextBay(String bay)
        {
            try
            {
                if (String.IsNullOrEmpty(bay))
                    return bay;

                var bayVal = Convert.ToInt32(BayRemoveChars(bay));
                if (bayVal > 0)
                {
                    var ret = Convert.ToString(bayVal + 2);
                    if (ret.Length == 1)
                    {
                        var cha = BayRemoveInt(bay);
                        if (String.IsNullOrEmpty(cha))
                            ret = "0" + ret;
                        else
                            ret = cha + ret;
                    }
                    return ret;
                }
            }
            catch (Exception e)
            {
            }

            return bay;
        }

        public static String GetFrontOddBay(String bay)
        {
            try
            {
                if (String.IsNullOrEmpty(bay))
                    return bay;

                var bayVal = Convert.ToInt32(BayRemoveChars(bay));
                if (bayVal != 0 && bayVal % 2 == 0)
                {
                    var ret = Convert.ToString(bayVal - 1);
                    if (ret.Length == 1)
                    {
                        var cha = BayRemoveInt(bay);
                        if (String.IsNullOrEmpty(cha))
                            ret = "0" + ret;
                        else
                            ret = cha + ret;
                    }
                    return ret;
                }
            }
            catch (Exception e)
            {
            }

            return bay;
        }

        public static String GetRearOddBay(String bay)
        {
            try
            {
                if (String.IsNullOrEmpty(bay))
                    return bay;

                var bayVal = Convert.ToInt32(BayRemoveChars(bay));
                if (bayVal != 0 && bayVal % 2 == 0)
                {
                    var ret = Convert.ToString(bayVal + 1);
                    if (ret.Length == 1)
                    {
                        var cha = BayRemoveInt(bay);
                        if (String.IsNullOrEmpty(cha))
                            ret = "0" + ret;
                        else
                            ret = cha + ret;
                    }
                    return ret;
                }
            }
            catch (Exception e)
            {
            }

            return bay;
        }

        public static String GetFrontEvenBay(String bay)
        {
            try
            {
                if (!String.IsNullOrEmpty(bay))
                {
                    var bayVal = Convert.ToInt32(BayRemoveChars(bay));
                    if (bayVal > 2 && bayVal % 2 == 1)
                    {
                        var ret = Convert.ToString(bayVal - 1);
                        if (ret.Length == 1)
                        {
                            var cha = BayRemoveInt(bay);
                            if (String.IsNullOrEmpty(cha))
                                ret = "0" + ret;
                            else
                                ret = cha + ret;
                        }
                        return ret;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public static String GetRearEvenBay(String bay)
        {
            try
            {
                if (!String.IsNullOrEmpty(bay))
                {
                    var bayVal = Convert.ToInt32(BayRemoveChars(bay));
                    if (bayVal > 0 && bayVal % 2 == 1)
                    {
                        var ret = Convert.ToString(bayVal + 1);
                        if (ret.Length == 1)
                        {
                            var cha = BayRemoveInt(bay);
                            if (String.IsNullOrEmpty(cha))
                                ret = "0" + ret;
                            else
                                ret = cha + ret;
                        }
                        return ret;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public static Boolean IsBayEqual(String targetBay, String currentBay)
        {
            if (string.IsNullOrEmpty(targetBay) || string.IsNullOrEmpty(currentBay))
                return false;

            var targetVal = Convert.ToInt32(BayRemoveChars(targetBay));
            var currentVal = Convert.ToInt32(BayRemoveChars(currentBay));
            if (targetVal == currentVal)
                return true;
            else if (targetVal != 0 && targetVal % 2 == 0)
                return (targetVal - 1 == currentVal || targetVal + 1 == currentVal) ? true : false;

            return false;
        }

        public static String GetContainerStatus(String jobTp, String jobStatus)
        {
            switch (jobTp)
            {
                case "GI":
                    return jobStatus != "C" ? "Stacking(Gate-In)" : "Gate-In";
                //return "Stacking(Gate-In)";
                case "GO":
                    return jobStatus != "C" ? "On Chassis(Gate-Out)" : "Gate-Out";
                //return "On Chassis(Gate-Out)";
                case "MI":
                    return jobStatus != "C" ? "Stacking(Move-In)" : "Move-In";
                //return "Stacking(Move-In)";
                case "MO":
                    return jobStatus != "C" ? "On Chassis(Move-Out)" : "Move-Out";
                //return "On Chassis(Move-Out)";
                case "DS":
                    return jobStatus != "C" ? "Stacking(Discharging)" : "Discharging";
                //return "Stacking(Discharging)";
                case "LD":
                    return jobStatus != "C" ? "On Chassis(Loading)" : "Loading";
                //return "On Chassis(Loading)";
                case "GC":
                    return jobStatus != "C" ? "Stacking(Gate-Out Cancel)" : "Gate-Out Cancel";
                //return "Stacking(Gate-Out Cancel)";
                case "LC":
                    return jobStatus != "C" ? "Stacking(Loading Cancel)" : "Loading Cancel";
                //return "Stacking(Loading Cancel)";
                case "AH":
                    return jobStatus != "C" ? "Stacking(Auto Rehandling)" : "Auto Rehandling";
                //return "Stacking(Auto Rehandling)";
                case "RH":
                    return jobStatus != "C" ? "Stacking(Rehandling)" : "Rehandling";
                //return "Stacking(Rehandling)";
                default:
                    return String.Empty;
            }
        }

        public static String GetJobDoneErrorMessage(String errCode)
        {
            switch (errCode)
            {
                case "S1":
                    return "Completed";
                case "F1":
                    return "Current Location is Null!!";
                case "F2":
                    return "Yard Location is not equal a JobOrder Location!!";
                case "F3":
                    return "There are another container on the current container!!";
                case "F4":
                    return "There are terminal in container job on the current truck!!";
                case "F5":
                    return "The status of reefer container is POW or ROW!!";
                case "F6":
                    return "The situation of container is not OYS or OYG!!";
                case "F7":
                    return "Check the Hold Information!!";
                case "F9":
                    return "Location is already occupied.";
                case "F10":
                    return "Container is not in yard";
                case "F11":
                    return "Processing Work Order can't change location";
                case "U1":
                default:
                    return "Unknown error";
            }
        }

        public static String GetJobSetErrorMessage(String errCode)
        {
            switch (errCode)
            {
                case "F1":
                    return "There is no usable location.";
                case "F2":
                    return "There is no container within the block.";
                case "F3":
                    return "Selected job does not belong to the machine.";
                case "F4":
                    return "There is no slot to be loaded within the same bay.";
                case "F5":
                    return "The status of reefer container is POW or ROW.";
                case "F6":
                    return "Container not plug out.";
                case "F7":
                    return "The error is not defined.";
                case "F8":
                    return "There is no container information.";
                case "F9":
                    return "There is no container within the general block.";
                default:
                    return "Unknown error";
            }
        }

        public static String GetEmptySwapErrorMessage(String errCode)
        {
            switch (errCode)
            {
                case "S1":
                    return "Completed";
                case "F1":
                    return "The job information is not exists";
                case "F2":
                    return "The Container is not in yard";
                case "F3":
                    return "The Swap Container is not exists a pre-advice";
                case "F4":
                    return "The Swap Container's booking information is wrong";
                case "F5":
                    return "The Container is Holded";
                case "F6":
                    return "The On-chassis Container Cancel is failed";
                case "F7":
                    return "GC  data exists";
                case "F8":
                    return "GC insert failed";
                case "F9":
                    return "The swap task fail";
                case "F10":
                    return "The Swap Container not in yard";
                case "F11":
                    return "The Swap update failed";
                case "F12":
                    return "The Swap Container has not booking no";
                case "F13":
                    return "The GO Job not completed";
                case "U1":
                default:
                    return "Unknown error";
            }
        }

        public static String GetLoadingSwapMessage(String errCode)
        {
            switch (errCode)
            {
                case "W0":
                    return "Swapping is Available.";
                case "W1":
                    return "Job type is different.";
                case "W2":
                    return "Loading vessel is different.";
                case "W3":
                    return "Working QC is different.";
                case "W4":
                    return "Chassis position is already occupied.";
                default:
                    return "Unknown error";
            }
        }

        public static Boolean IsValidLocation(String block, String bay, String row, String tier)
        {
            return PresentationMgr.IsValidBlock(block) && PresentationMgr.IsValidBay(bay) &&
                PresentationMgr.IsValidRow(row) && PresentationMgr.IsValidTier(tier);
        }

        public static Boolean IsValidBlock(String block)
        {
            return block.ToCharArray().All(c => (48 <= (int)c && (int)c <= 57) || (65 <= (int)c && (int)c <= 90));
        }

        public static Boolean IsValidBay(String bay)
        {
            // Jan 31 2023 Fix issue move to another bay after selecting container bay Z1
            return true;
            //return bay.ToCharArray().All(c => 48 <= (int)c && (int)c <= 57);
        }

        public static Boolean IsValidRow(String row)
        {
            return (row == "-1" || row.ToCharArray().All(c => 65 <= (int)c && (int)c <= 90));
        }

        public static Boolean IsValidTier(String tier)
        {
            return tier.ToCharArray().All(c => 48 <= (int)c && (int)c <= 57);
        }

        #endregion [Common Helper Method]

        //20201008 show/hide buttons follow by AccessAction api
        public void showHideButtonsAccessAction()
        {
            PresentationMgr.AppWin.UC_IndicatorView.Image_Setting.Visibility = showSetting ? Visibility.Visible : Visibility.Collapsed;
            //PresentationMgr.MainView.Btn_Chg_Loc.Visibility = showCHGLOC ? Visibility.Visible : Visibility.Collapsed;
            PresentationMgr.MainView.Col_ChgLoc.Width = new GridLength((showCHGLOC ? 14 : 0), GridUnitType.Star);
            if (!showViewINV) PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
        }

        public static String BayRemoveChars(String value)
        {
            return new string(value.Where(char.IsDigit).ToArray());
        }

        public static String BayRemoveInt(String value)
        {
            return Regex.Replace(value, @"[\d-]", string.Empty);
        }
    }
}
