using Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

//using ExternalAPI;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for ContainerItem.xaml
    /// </summary>
    public partial class ContainerItem : UserControl
    {
        public class NoWorkValue
        {
            public const Int32 VALUE_NONE = 0x00;
            public const Int32 VALUE_AREA = 0x01;
            public const Int32 VALUE_TUNNEL = 0x02;
            public const Int32 VALUE_TIER = 0x04;
        }

        public class OverValue
        {
            public const Int32 VALUE_NONE = 0x00;
            public const Int32 VALUE_LEFT = 0x01;
            public const Int32 VALUE_RIGHT = 0x02;
            public const Int32 VALUE_TOP = 0x04;
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory _inventory;
        private String _jobKey = String.Empty;
        private Boolean _Selected = false;
        private Boolean _available = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();
        public SolidColorBrush blackColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
        public SolidColorBrush whiteColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF");
        //private SolidColorBrush _textBlockSelectedForeground = new SolidColorBrush(Color.FromArgb(255, 70, 72, 72));
        //private SolidColorBrush _textBlockNormalForeground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private Int32 _noWorkValues = NoWorkValue.VALUE_NONE;
        private Int32 _overValues = OverValue.VALUE_NONE;
        private bool useColor = false;
        private bool rmkClick = false;
        DispatcherTimer dtClockTime = new DispatcherTimer();
        DispatcherTimer dtClockTime1 = new DispatcherTimer();

        List<string> lstContNoLogged = new List<string>();//list contNo logged

        public VMT_Data_JAT2.Marshalling.Geometry.sPosition ContainerPos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();

        public VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory Inventory
        {
            get { return _inventory; }
            set
            {
                _inventory = value;
                SetInventoryInfo(_inventory);
            }
        }

        public String JobKey
        {
            get { return _jobKey; }
            set
            {
                _jobKey = value;
            }
        }

        public Boolean Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                if (value == true)
                    PresentationMgr.Singleton.containerItemSelected = this;
                //this.useColor = false;
                RefreshContainerItem();
            }
        }

        public Boolean Available
        {
            get { return _available; }
            set
            {
                _available = value;
                RefreshContainerItem();
            }
        }

        public Boolean CorrectionSelected
        {
            get
            {
                //if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                if (this.Grid9x7.Visibility == Visibility.Visible)
                    return this.Image_Container_CorrectionSelect.Visibility == Visibility.Visible;
                else
                    return this.Image_Container_CorrectionSelect1.Visibility == Visibility.Visible;
                //else
                //  return this.Image_Container_CorrectionSelect.Visibility == Visibility.Hidden;
            }
            set
            {
                if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                {
                    this.Image_Container_CorrectionSelect.Visibility = value == true ?
                        Visibility.Visible : Visibility.Hidden;
                    this.Image_Container_CorrectionSelect1.Visibility = value == true ?
                       Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        public ContainerItem()
        {
            this.InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            AppendTextBlockList();
            this._noWorkValues = NoWorkValue.VALUE_NONE;
        }

        ~ContainerItem()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(TextBlock_VesselCode);
            _TextBlockList.Add(TextBlock_Operator);
            _TextBlockList.Add(TextBlock_SizeType);
            _TextBlockList.Add(TextBlock_Fm);
            _TextBlockList.Add(TextBlock_JobType);
            _TextBlockList.Add(TextBlock_JobTypeKor);
            _TextBlockList.Add(Tbl_QcNo);
            _TextBlockList.Add(Tbl_QcEtw);
            _TextBlockList.Add(TextBlock_TruckNo);
            _TextBlockList.Add(TextBlock_Hold);
            _TextBlockList.Add(TextBlock_Inventory_Number);

            _TextBlockList.Add(TextBlock_VesselCode1);
            _TextBlockList.Add(TextBlock_Operator1);
            _TextBlockList.Add(TextBlock_SizeType1);
            _TextBlockList.Add(TextBlock_Fm1);
            _TextBlockList.Add(TextBlock_JobType1);
            _TextBlockList.Add(TextBlock_JobTypeKor1);
            _TextBlockList.Add(TextBlock_TruckNo1);
            _TextBlockList.Add(Tbl_QcNo1);
            _TextBlockList.Add(Tbl_QcEtw1);
            _TextBlockList.Add(TextBlock_Hold1);
            _TextBlockList.Add(TextBlock_Inventory_Number1);
        }

        public void Clear()
        {
            this.Selected = false;
            this.Available = false;
            this._noWorkValues = NoWorkValue.VALUE_NONE;
            this._overValues = OverValue.VALUE_NONE;
            this.SetInventoryInfo(null);
            this.ContainerPos.Clear();
            this._jobKey = String.Empty;
            this.useColor = false;
        }

        public void ClearWithoutPos()
        {
            this.Selected = false;
            this.Available = false;
            this._noWorkValues = NoWorkValue.VALUE_NONE;
            this._overValues = OverValue.VALUE_NONE;
            this.SetInventoryInfo(null);
            this._jobKey = String.Empty;
            this.useColor = false;
        }

        private void RefreshContainerItem()
        {
            this.Image_ContainerType.Source = GetBGImageByContainerType("Exist");
            this.Image_ContainerType1.Source = GetBGImageByContainerType("Exist");

            //setModelayout();
            //foreach (TextBlock tBlock in _TextBlockList)
            //{
            //    if (_Selected)
            //        tBlock.Foreground = _textBlockSelectedForeground;
            //    //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 70, 72, 72));
            //    else
            //        tBlock.Foreground = _textBlockNormalForeground;
            //    //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            //}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.MouseLeftButtonDown += new MouseButtonEventHandler(ContainerItem_MouseLeftButtonDown);
            this.MouseRightButtonDown += new MouseButtonEventHandler(ContainerItem_MouseRightButtonDown);
            this.TextBlock_RMK.MouseLeftButtonDown += new MouseButtonEventHandler(TextBlock_RMK_MouseRightButtonDown);
            this.TextBlock_RMK1.MouseLeftButtonDown += new MouseButtonEventHandler(TextBlock_RMK_MouseRightButtonDown);

            dtClockTime.Interval = new TimeSpan(0, 0, 1);
            dtClockTime.Tick += new EventHandler(blinkSelected_Tick);
            dtClockTime1.Interval = new TimeSpan(0, 0, 1);
            dtClockTime1.Tick += new EventHandler(blinkSelected_Tick1);
        }

        public void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_inventory != null)
            {
                this.Image_ContainerType.Source = GetBGImageByContainerType("Exist");
                this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(_inventory.cntr.cntrTp);
                this.Image_ContainerType1.Source = GetBGImageByContainerType("Exist");
                this.Image_ContainerType_Cover1.Source = GetCoverImageByContainerType(_inventory.cntr.cntrTp);
            }
            else
            {
                this.Image_ContainerType.Source = GetBGImageByContainerType(String.Empty);
                this.Image_ContainerType1.Source = GetBGImageByContainerType(String.Empty);

            }
        }

        private void InitSkinImage()
        {
            if (App.STANDALONE_MODE)
                return;
            this.TextBlock_VesselCode.Text = String.Empty;
            this.TextBlock_Operator.Text = String.Empty;
            this.TextBlock_SizeType.Text = String.Empty;
            this.TextBlock_Fm.Text = String.Empty;
            this.TextBlock_JobType.Text = String.Empty;
            this.TextBlock_JobTypeKor.Text = String.Empty;
            this.TextBlock_ReFlug.Text = String.Empty;
            this.Tbl_QcNo.Text = String.Empty;
            this.Tbl_QcEtw.Text = String.Empty;
            this.TextBlock_TruckNo.Text = String.Empty;
            this.TextBlock_Hold.Text = String.Empty;
            this.TextBlock_Inventory_Number.Text = String.Empty;

            this.TextBlock_VesselCode1.Text = String.Empty;
            this.TextBlock_Operator1.Text = String.Empty;
            this.TextBlock_SizeType1.Text = String.Empty;
            this.TextBlock_Fm1.Text = String.Empty;
            this.TextBlock_JobType1.Text = String.Empty;
            this.TextBlock_JobTypeKor1.Text = String.Empty;
            this.TextBlock_ReFlug1.Text = String.Empty;
            this.Tbl_QcNo1.Text = String.Empty;
            this.Tbl_QcEtw1.Text = String.Empty;
            this.TextBlock_TruckNo1.Text = String.Empty;
            this.TextBlock_Hold1.Text = String.Empty;
            this.TextBlock_Inventory_Number1.Text = String.Empty;

            this._Selected = false;
            this.Image_ContainerType.Source = GetBGImageByContainerType(String.Empty);
            this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(String.Empty);
            this.Image_ContainerType1.Source = GetBGImageByContainerType(String.Empty);
            this.Image_ContainerType_Cover1.Source = GetCoverImageByContainerType(String.Empty);
            this.Blinked_Up.Visibility = Visibility.Hidden;
            this.Blinked_Down.Visibility = Visibility.Hidden;
            this.Blinked_Up1.Visibility = Visibility.Hidden;
            this.Blinked_Down1.Visibility = Visibility.Hidden;
            if (!_Selected)
                this.dtClockTime.Stop();
            MakeHiddenControls();
        }

        private void InitSkinImageForBlink()
        {
            if (App.STANDALONE_MODE)
                return;
            this.TextBlock_VesselCode.Text = String.Empty;
            this.TextBlock_Operator.Text = String.Empty;
            this.TextBlock_SizeType.Text = String.Empty;
            this.TextBlock_Fm.Text = String.Empty;
            this.TextBlock_JobType.Text = String.Empty;
            this.TextBlock_JobTypeKor.Text = String.Empty;
            this.TextBlock_ReFlug.Text = String.Empty;
            this.Tbl_QcNo.Text = String.Empty;
            this.Tbl_QcEtw.Text = String.Empty;
            this.TextBlock_TruckNo.Text = String.Empty;
            this.TextBlock_Hold.Text = String.Empty;
            this.TextBlock_Inventory_Number.Text = String.Empty;
            TextBlock_TruckNo.Background = Brushes.Transparent;
            TextBlock_JobType.Background = Brushes.Transparent;
            TextBlock_JobTypeKor.Background = Brushes.Transparent;
            TextBlock_Inventory_Number.Text = String.Empty;

            this.TextBlock_VesselCode1.Text = String.Empty;
            this.TextBlock_Operator1.Text = String.Empty;
            this.TextBlock_SizeType1.Text = String.Empty;
            this.TextBlock_Fm1.Text = String.Empty;
            this.TextBlock_JobType1.Text = String.Empty;
            this.TextBlock_JobTypeKor1.Text = String.Empty;
            this.TextBlock_ReFlug1.Text = String.Empty;
            this.Tbl_QcNo1.Text = String.Empty;
            this.Tbl_QcEtw1.Text = String.Empty;
            this.TextBlock_TruckNo1.Text = String.Empty;
            this.TextBlock_Hold1.Text = String.Empty;
            this.TextBlock_Inventory_Number1.Text = String.Empty;
            TextBlock_TruckNo1.Background = Brushes.Transparent;
            TextBlock_JobType1.Background = Brushes.Transparent;
            TextBlock_JobTypeKor1.Background = Brushes.Transparent;
            TextBlock_Inventory_Number1.Text = String.Empty;

            this.Blinked_Up.Visibility = Visibility.Hidden;
            this.Blinked_Down.Visibility = Visibility.Hidden;
            this.Blinked_Up1.Visibility = Visibility.Hidden;
            this.Blinked_Down1.Visibility = Visibility.Hidden;
            if (!_Selected)
                this.dtClockTime.Stop();
            MakeHiddenControls();
        }

        private void HiddenInfoSelected()
        {
            if (App.STANDALONE_MODE)
                return;
            this.TextBlock_VesselCode.Text = String.Empty;
            this.TextBlock_Operator.Text = String.Empty;
            this.TextBlock_SizeType.Text = String.Empty;
            this.TextBlock_Fm.Text = String.Empty;
            this.TextBlock_JobType.Text = String.Empty;
            this.TextBlock_JobType.Background = Brushes.Transparent;
            this.TextBlock_JobTypeKor.Text = String.Empty;
            this.TextBlock_JobTypeKor.Background = Brushes.Transparent;
            this.TextBlock_ReFlug.Text = String.Empty;
            this.Tbl_QcNo.Text = String.Empty;
            this.Tbl_QcEtw.Text = String.Empty;
            this.TextBlock_TruckNo.Text = String.Empty;
            TextBlock_TruckNo.Background = Brushes.Transparent;
            this.TextBlock_Hold.Text = String.Empty;
            this.TextBlock_Inventory_Number.Text = String.Empty;
            TextBlock_Inventory_Number.Text = String.Empty;

            this.TextBlock_VesselCode1.Text = String.Empty;
            this.TextBlock_Operator1.Text = String.Empty;
            this.TextBlock_SizeType1.Text = String.Empty;
            this.TextBlock_Fm1.Text = String.Empty;
            this.TextBlock_JobType1.Text = String.Empty;
            this.TextBlock_JobType1.Background = Brushes.Transparent;
            this.TextBlock_JobTypeKor1.Text = String.Empty;
            this.TextBlock_JobTypeKor1.Background = Brushes.Transparent;
            this.TextBlock_ReFlug1.Text = String.Empty;
            this.Tbl_QcNo1.Text = String.Empty;
            this.Tbl_QcEtw1.Text = String.Empty;
            this.TextBlock_TruckNo1.Text = String.Empty;
            TextBlock_TruckNo1.Background = Brushes.Transparent;
            this.TextBlock_Hold1.Text = String.Empty;
            this.TextBlock_Inventory_Number1.Text = String.Empty;
            TextBlock_Inventory_Number1.Text = String.Empty;

            MakeHiddenControls();
        }

        private void ContainerItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!PresentationMgr.UseCorrection)
                return;

            PresentationMgr.AppWin.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (rmkClick)
                            {
                                rmkClick = false;
                                //return;  //maybe un-comment in the future
                            }
                            var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                            if (this._inventory != null)    // empty 영역이 아니면   
                            {
                                if (this._inventory.cntr != null)
                                {
                                    if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView)
                                    {
                                        if (this.CorrectionSelected && this.Selected)
                                        {
                                            PresentationMgr.Singleton.swapList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                                            PresentationMgr.Singleton.reservedList = new List<RMG.VD_RMG_VmtEmptySwapVO>();
                                            PresentationMgr.Singleton.swapListRTG = new List<VMT_Data_JAT2.Objects.Common.VmtSwap>();

                                            if (target != null && target.type != null)
                                            {
                                                if (target.type.jobTp == "LD" || target.type.jobTp == "MO" || target.type.jobTp == "GO")
                                                {
                                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0038", LanguageService.MESSAGE_GROUP)
                                                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0060", LanguageService.MESSAGE_GROUP)
                                                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);

                                                    //PresentationMgr.MainView.jobDone = true;
                                                    //PresentationMgr.ShowYtSwapPopup(target);

                                                    //if (DataMgr.Singleton.List_MachineofPool.Count <= 0)
                                                    //{
                                                    //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                                    //        , "GetMachineList_Ask"), target.partnerMchn.mchnId);

                                                    //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
                                                    //    PresentationMgr.AppWin.ShowProgressBar(0);
                                                    //}
                                                    //else
                                                    //    PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
                                                }
                                                else
                                                {
                                                    //if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                                                    //{
                                                    //    var twin = PresentationMgr.Singleton.JOB_Get(target.type.ycTwinKey);
                                                    //    if (null != twin && twin.type != null && twin.type.jobStatus == "P")
                                                    //    {
                                                    //        var targetLoc = target.locWorking;
                                                    //        var twinLoc = twin.locWorking;
                                                    //        if (!string.IsNullOrEmpty(targetLoc.bay) && !string.IsNullOrEmpty(twinLoc.bay))
                                                    //        {
                                                    //            // 2017-01-12 : 정호진 수석 요청
                                                    //            // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                                                    //            if ((target.type.jobFlagInfo.Equals("F") && Convert.ToInt32(targetLoc.bay) < Convert.ToInt32(twinLoc.bay)) ||
                                                    //                (target.type.jobFlagInfo.Equals("A") && Convert.ToInt32(targetLoc.bay) > Convert.ToInt32(twinLoc.bay)))
                                                    //            {
                                                    //                target.locWorking = twinLoc;
                                                    //                twin.locWorking = targetLoc;
                                                    //            }
                                                    //        }

                                                    //        PresentationMgr.Singleton.SendJobDoneAsk(twin);
                                                    //    }
                                                    //}
                                                    PresentationMgr.Singleton.SendJobDoneAsk(target);
                                                }
                                            }
                                            return;
                                        }
                                        else
                                        {
                                            if ((Swap_Circle.IsVisible || Swap_Circle1.IsVisible) && target != null)
                                            {
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.Visibility = Visibility.Visible;
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.emptySwapListSend = new VMT_Data_JAT2.Objects.RTG.EmptySwapListSend();

                                                //PresentationMgr.MainView.UC_EmptySwapPopupView.emptyList.Add(UserInfo.gUserID); //userId
                                                //PresentationMgr.MainView.UC_EmptySwapPopupView.emptyList.Add(target.cntr.cntrNo); //orgCntrNo
                                                //PresentationMgr.MainView.UC_EmptySwapPopupView.emptyList.Add(this._inventory.cntr.cntrNo); //newCntrNo
                                                //PresentationMgr.MainView.UC_EmptySwapPopupView.emptyList.Add(target.partnerMchn.mchnId); //regoNo
                                                //PresentationMgr.MainView.UC_EmptySwapPopupView.emptyList.Add(UserInfo.gMchnID); //mchnId

                                                PresentationMgr.MainView.UC_EmptySwapPopupView.emptySwapListSend.jobKey = target.jobKey; //jobKey
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.emptySwapListSend.orgCntrNo = target.cntr.cntrNo; //orgCntrNo
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.emptySwapListSend.newCntrNo = this._inventory.cntr.cntrNo; //newCntrNo
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.emptySwapListSend.regoNo = target.partnerMchn.mchnId; //regoNo

                                                PresentationMgr.MainView.UC_EmptySwapPopupView.Tbl_TruckNo.Text = target.partnerMchn.mchnId; //regoNo
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.Tbl_Before.Text = target.cntr.cntrNo; //orgCntrNo
                                                PresentationMgr.MainView.UC_EmptySwapPopupView.Tbl_After.Text = _inventory.cntr.cntrNo; //newCntrNo

                                            }
                                            else
                                            {
                                                PresentationMgr.Singleton.CorrectionSource.LDMOGOjob = false;
                                                PresentationMgr.Singleton.swapList.Clear();
                                                PresentationMgr.Singleton.reservedList.Clear();
                                                PresentationMgr.Singleton.swapListRTG.Clear();

                                                PresentationMgr.MainView.UC_VBlockView.general = false;
                                                for (int i = 0; i < PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children.Count; i++)
                                                {
                                                    if (PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children[i] is ContainerItem)
                                                    {
                                                        var itm = PresentationMgr.MainView.UC_BayView.Canvas_BayWorkInventory.Children[i] as ContainerItem;
                                                        if (itm.Selected)
                                                        {
                                                            itm.Image_ContainerType.Source = null;
                                                            itm.Image_ContainerType1.Source = null;
                                                            itm._Selected = false;
                                                            itm.SetInventoryInfo(itm._inventory);
                                                            itm.Blinked_Up.Visibility = Visibility.Hidden;
                                                            itm.Blinked_Down.Visibility = Visibility.Hidden;
                                                            itm.Blinked_Up1.Visibility = Visibility.Hidden;
                                                            itm.Blinked_Down1.Visibility = Visibility.Hidden;
                                                            itm.dtClockTime.Stop();
                                                        }
                                                    }
                                                }
                                                JobListItem item = PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem as JobListItem;
                                                if (item != null)
                                                {
                                                    item.Selected = false;
                                                    PresentationMgr.MainView.deselectJobList = true;
                                                    PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem = null;
                                                    PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
                                                    PresentationMgr.Singleton.NeedJobAutoSelection = false;
                                                }
                                                this.CorrectionSelected = PresentationMgr.Singleton.SetCorrectionSelect(this.ContainerPos,
                                                    this._inventory.cntr.cntrNo, this._inventory.cntr.cntrIso);
                                                setTargetContainerNumber(this.ContainerPos, this._inventory.cntr.cntrNo);

                                                if (PresentationMgr.Singleton.enableBayViewSelection)
                                                {
                                                    var job = String.IsNullOrEmpty(this._jobKey) ? null : PresentationMgr.Singleton.JOB_Get(this._jobKey);
                                                    if (job != null && (job.type.jobTp == "LD" || job.type.jobTp == "MO" || job.type.jobTp == "GO" || job.type.jobTp == "RH" || job.type.jobTp == "AH"))
                                                    {
                                                        PresentationMgr.MainView.UC_JobList.selectedJobKeyPriority = this._jobKey;
                                                        PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(this._jobKey);
                                                    }
                                                }
                                            }
                                            //if (this.CorrectionSelected)
                                            //{
                                            //    foreach (var listitem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
                                            //    {
                                            //        JobListItem itm = listitem as JobListItem;
                                            //        var jobOrder = PresentationMgr.Singleton.JOB_Get(itm.JobKey);
                                            //        if (jobOrder != null && jobOrder.cntr.cntrNo == this._inventory.cntr.cntrNo)
                                            //        {
                                            //            if ((UserInfo.gMchnTp.Equals("RS") || UserInfo.gMchnTp.Equals("ES") || UserInfo.gMchnTp.Equals("EH")) &&
                                            //                jobOrder.type.jobTp.Equals("GO") && jobOrder.cntr.fullMty.Equals("M"))
                                            //            {
                                            //                VMT_DataMgr_RMG.GetEmptySwappingTargetList_Ask(jobOrder.cntr.cntrNo, jobOrder.partnerMchn.mchnId);
                                            //            }
                                            //            itm.Selected = true;
                                            //            PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem = itm;
                                            //            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = itm.JobKey;
                                            //            break;
                                            //        }
                                            //    }
                                            //}
                                            if (this.CorrectionSelected)
                                            {
                                                if (!PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.showVBlock && PresentationMgr.Singleton.cntrSelected.Equals(String.Empty) && this._inventory != null && this._inventory.cntr != null)
                                                {
                                                    PresentationMgr.Singleton.showVBlock = true;
                                                    PresentationMgr.Singleton.cntrSelected = this._inventory.cntr.cntrNo;
                                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;
                                                }

                                                if (!String.IsNullOrEmpty(this._inventory.cntr.rmk))
                                                {
                                                    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
                                                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0058", LanguageService.LABEL_CONTAINERDETAIL), this._inventory.cntr.rmk
                                                                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
                                                }
                                            }
                                            else
                                            {
                                                if (!PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.cntrSelected.Equals(String.Empty) && this._inventory != null && this._inventory.cntr != null && this._inventory.cntr.cntrNo.Equals(PresentationMgr.Singleton.cntrSelected) && PresentationMgr.Singleton.showVBlock)
                                                {
                                                    PresentationMgr.Singleton.showVBlock = false;
                                                    PresentationMgr.Singleton.cntrSelected = String.Empty;
                                                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                                                }
                                            }
                                        }

                                    }
                                }

                            }
                            else
                            {
                                if (PresentationMgr.MainView.cntrLockMode)
                                    return;
                                if (target != null)
                                {
                                    if (target.type != null && target.type.jobStatus == "P")
                                    {
                                        if (target.type.jobTp == "GI" || target.type.jobTp == "GC" || target.type.jobTp == "MI" ||
                                            target.type.jobTp == "DS" || target.type.jobTp == "LC" || target.type.jobTp == "RH")
                                            PresentationMgr.MainView.SetJobDoneToDiffLocation(this.ContainerPos);
                                        else
                                            PresentationMgr.MainView.Btn_JobDone_Click(null, null);
                                    }
                                    return;
                                }
                                setTargetContainerNumber(null, "");
                                PresentationMgr.Singleton.swapList.Clear();
                                PresentationMgr.Singleton.reservedList.Clear();
                                PresentationMgr.Singleton.swapListRTG.Clear();

                                if (!PresentationMgr.Singleton.CorrectionSource.Pos.IsEmpty() && PresentationMgr.Singleton.CorrectionSource.LDMOGOjob)
                                    return;
                                //var jobList = PresentationMgr.Singleton.JOB_GetForLocation(this.ContainerPos.m_cBlock, this.ContainerPos.m_cBay,
                                //    this.ContainerPos.m_cRow, this.ContainerPos.m_cTier);
                                //if (jobList.Count > 0)      
                                var job = String.IsNullOrEmpty(this._jobKey) ? null : PresentationMgr.Singleton.JOB_Get(this._jobKey);
                                if (job != null && this.Selected)
                                {
                                    if (PresentationMgr.Singleton.CorrectionSource.Pos.Equal(PresentationMgr.Singleton.CurrentPostion) && this.Selected)
                                    {
                                        if (job.type != null && (job.type.jobTp == "LD" || job.type.jobTp == "MO"))
                                        {
                                            PresentationMgr.MainView.jobDone = true;
                                            PresentationMgr.ShowYtSwapPopup(job);
                                            //if (DataMgr.Singleton.List_MachineofPool.Count <= 0)
                                            //{
                                            //    InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                                            //        , "GetMachineList_Ask"), job.partnerMchn.mchnId);

                                            //    VMT_Data_JAT2.VMT_DataMgr_RMG.GetMachineList_Ask("YT");
                                            //    PresentationMgr.AppWin.ShowProgressBar(0);
                                            //}
                                            //else
                                            //    PresentationMgr.MainView.UC_MachineSearchView.SetMachineList();
                                        }
                                        else
                                        {
                                            //if (!String.IsNullOrEmpty(target.type.ycTwinKey))
                                            //{
                                            //    var twin = PresentationMgr.Singleton.JOB_Get(target.type.ycTwinKey);
                                            //    if (null != twin && twin.type.jobStatus == "P")
                                            //    {
                                            //        var targetLoc = job.locWorking;
                                            //        var twinLoc = twin.locWorking;
                                            //        if (!string.IsNullOrEmpty(targetLoc.bay) && !string.IsNullOrEmpty(twinLoc.bay))
                                            //        {
                                            //            // 2017-01-12 : 정호진 수석 요청
                                            //            // 완료처리를 할 때, 높은 BAY는 FORE, 낮은 BAY는 AFTER로 값을 채워서 TOS에 호출 
                                            //            if (job.type != null && ((job.type.jobFlagInfo.Equals("F") && Convert.ToInt32(targetLoc.bay) < Convert.ToInt32(twinLoc.bay)) ||
                                            //                (job.type.jobFlagInfo.Equals("A") && Convert.ToInt32(targetLoc.bay) > Convert.ToInt32(twinLoc.bay))))
                                            //            {
                                            //                job.locWorking = twinLoc;
                                            //                twin.locWorking = targetLoc;
                                            //            }
                                            //        }

                                            //        PresentationMgr.Singleton.SendJobDoneAsk(twin);
                                            //    }
                                            //}
                                            PresentationMgr.Singleton.SendJobDoneAsk(job);
                                        }
                                        return;
                                    }
                                    //var job = jobList.First();
                                    if (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView && job.type != null)
                                        if (job.type.jobTp == "GI" || job.type.jobTp == "DS" || job.type.jobTp == "MI" ||
                                            /*job.type.jobTp == "RH" || */job.type.jobTp == "LC" || job.type.jobTp == "GC")
                                            this.CorrectionSelected = PresentationMgr.Singleton.SetCorrectionSelect(this.ContainerPos,
                                                job.cntr.cntrNo, job.cntr.cntrIso);
                                }
                                else
                                {
                                    PresentationMgr.Singleton.NeedJobAutoSelection = false;
                                    PresentationMgr.Singleton.MakeCorrection(this.ContainerPos);
                                }

                                //jobList = null;                
                            }
                        }));
        }

        private void setTargetContainerNumber(VMT_Data_JAT2.Marshalling.Geometry.sPosition pos, string contNumberText)
        {
            if (pos != null && !PresentationMgr.Singleton.CorrectionSource.Pos.Equal(pos))        // 현재 correction source 해제
            {
                MainView.contItmSelected = true;
                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetVirtualContNo = String.Empty;
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = true;
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = contNumberText;
            }
            else
            {
                MainView.contItmSelected = false;
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.IsEnabled = false;
                PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.Btn_ContainerNumber.Content = "";
            }
            JobListItem item = PresentationMgr.MainView.UC_JobList.ListBox_Job.SelectedItem as JobListItem;
            if (item != null)
                item.Selected = false;

            if (!VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey.Equals(String.Empty)) PresentationMgr.Singleton.CorrectionSource.Clear();

            VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey = String.Empty;

            PresentationMgr.Singleton.PreSelectedAHRHJobKey = String.Empty;
            PresentationMgr.MainView.UC_JobList.PreSelectedTargetJobKey = String.Empty;
            PresentationMgr.MainView.UC_JobList.ListBox_setJobSelection(String.Empty);
            PresentationMgr.MainView.deselectJobList = true;
            PresentationMgr.Singleton.NeedJobAutoSelection = false;
            PresentationMgr.MainView.UC_InfomationView.RMG_Member_PropertyChanged_TargetJobKey(null, null);
        }

        private void TextBlock_RMK_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            rmkClick = true;
            //if (!String.IsNullOrEmpty(this._inventory.cntr.rmk))
            //{
            //    PresentationMgr.AppWin.UC_PopupView.ShowPopup(UC_PopupView.UC_PopupViewType.PopupViewType_Common
            //                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0058", LanguageService.LABEL_CONTAINERDETAIL), this._inventory.cntr.rmk
            //                        , PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0034", LanguageService.MESSAGE_GROUP), null, 0);
            //}
        }

        private void ContainerItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PresentationMgr.AppWin.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (this._inventory != null && this._inventory.cntr != null && !String.IsNullOrEmpty(this._inventory.cntr.cntrNo))
                            {
                                PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
                                PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(this._inventory.cntr.cntrNo, null);
                            }
                            else
                            {
                                var job = String.IsNullOrEmpty(this._jobKey) ? null : PresentationMgr.Singleton.JOB_Get(this._jobKey);
                                if (job != null && job.type != null)
                                {
                                    if (job.type.jobTp == "GI" || job.type.jobTp == "DS" || job.type.jobTp == "MI" ||
                                        /*job.type.jobTp == "RH" || */job.type.jobTp == "LC" || job.type.jobTp == "GC")
                                    {
                                        PresentationMgr.Singleton.UI_SwitchUI(PresentationMgr.UIMode.ContainerDetailView);
                                        PresentationMgr.MainView.UC_ContainerDetailView.RefreshContainerInfo(job.cntr.cntrNo, null);
                                    }
                                }
                            }
                        }));
        }

        public void SetNoWorkValue(Int32 value)
        {
            this._noWorkValues |= value;
            String coverType = String.Empty;
            switch (this._noWorkValues)
            {
                case NoWorkValue.VALUE_AREA:
                    coverType = "NA";
                    break;
                case NoWorkValue.VALUE_TUNNEL:
                    coverType = "TN";
                    break;
                case NoWorkValue.VALUE_TIER:
                    coverType = "NT";
                    break;
                case NoWorkValue.VALUE_AREA | NoWorkValue.VALUE_TUNNEL:
                    coverType = "NA&TN";
                    break;
                case NoWorkValue.VALUE_AREA | NoWorkValue.VALUE_TIER:
                    coverType = "NA&NT";
                    break;
                case NoWorkValue.VALUE_TUNNEL | NoWorkValue.VALUE_TIER:
                    coverType = "TN&NT";
                    break;
                case NoWorkValue.VALUE_AREA | NoWorkValue.VALUE_TUNNEL | NoWorkValue.VALUE_TIER:
                    coverType = "NA&TN&NT";
                    break;
                default:
                    break;
            }
            this.Image_ContainerType_Cover.Source = this.GetCoverImageByContainerType(coverType);
            this.Image_ContainerType_Cover1.Source = this.GetCoverImageByContainerType(coverType);
        }

        private BitmapImage GetOvervalueImage(Int32 value)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }

            switch (value)
            {
                case OverValue.VALUE_LEFT:
                    imgUri += "Inventory_OvervalueL";
                    break;
                case OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueR";
                    break;
                case OverValue.VALUE_TOP:
                    imgUri += "Inventory_OvervalueB";
                    break;
                case OverValue.VALUE_LEFT | OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueLR";
                    break;
                case OverValue.VALUE_TOP | OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueRB";
                    break;
                case OverValue.VALUE_TOP | OverValue.VALUE_LEFT:
                    imgUri += "Inventory_OvervalueLB";
                    break;
                case OverValue.VALUE_TOP | OverValue.VALUE_LEFT | OverValue.VALUE_RIGHT:
                    imgUri += "Inventory_OvervalueLRB";
                    break;
                default:
                    break;
            }
            imgUri += ".png";

            return PresentationMgr.GetImageSource(imgUri);
        }

        public void SetInventoryInfo(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven)
        {
            _inventory = inven;
            InitSkinImage();
            if (_inventory == null)
            {
                return;
            }

            if (inven.cntr.isBundle && !inven.cntr.isBundleMaster)
                return;


            this.ContainerPos.m_cBlock = _inventory.loc.blck;
            this.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(_inventory.loc.bay);
            this.ContainerPos.m_cRow = _inventory.loc.row;
            this.ContainerPos.m_cTier = _inventory.loc.tier;

            this.Image_ContainerType.Source = GetBGImageByContainerType("Exist"); // Exist Container
            this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(inven.cntr.cntrTp);
            this.Image_ContainerType1.Source = GetBGImageByContainerType("Exist"); // Exist Container
            this.Image_ContainerType_Cover1.Source = GetCoverImageByContainerType(inven.cntr.cntrTp);
            //this.Grid_TextArear.Visibility = System.Windows.Visibility.Visible;

            /*
             * •	1 : Length
               •	2 : F/E (F:Full / E:Empty)
               •	3 : Commodity
               •	4 : Class code
               •	5 : Container type / HC
               (For a high cubic container, instead of a container type, HC is displayed.)
               •	6 : Operator
               •	7 : Container grade (SD)
               •	8 : Last 4 digit of Container number 
             */
            this.TextBlock_VesselCode.Text = inven.cntr.outVsl;
            this.TextBlock_VesselCode1.Text = inven.cntr.outVsl;

            this.TextBlock_Operator.Text = inven.cntr.opr;
            this.TextBlock_Operator1.Text = inven.cntr.opr;

            this.TextBlock_SizeType.Text = inven.cntr.cntrLen + inven.cntr.cntrTp;
            this.TextBlock_SizeType1.Text = inven.cntr.cntrLen + inven.cntr.cntrTp;

            this.TextBlock_Fm.Text = inven.cntr.fullMty == "F" ? (inven.cntr.fullMty + "/" + inven.cntr.cntrWgt) : "M";
            this.TextBlock_Fm1.Text = inven.cntr.fullMty == "F" ? (inven.cntr.fullMty + "/" + inven.cntr.cntrWgt) : "M";

            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;

            this.TextBlock_JobTypeKor.Text = inven.jobTpKorShort;
            this.TextBlock_JobTypeKor1.Text = inven.jobTpKorShort;

            this.TextBlock_JobType.Text = inven.jobTp;
            this.TextBlock_JobType1.Text = inven.jobTp;

            if (lang.Contains("Korea"))
            {
                this.TextBlock_JobType.Visibility = Visibility.Hidden;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Visible;
                this.TextBlock_JobType1.Visibility = Visibility.Hidden;
                this.TextBlock_JobTypeKor1.Visibility = Visibility.Visible;
            }
            else
            {
                this.TextBlock_JobType.Visibility = Visibility.Visible;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Hidden;
                this.TextBlock_JobType1.Visibility = Visibility.Visible;
                this.TextBlock_JobTypeKor1.Visibility = Visibility.Hidden;
            }

            this.TextBlock_ReFlug.Text = inven.cntr.reefer.plugCd;
            this.TextBlock_ReFlug1.Text = inven.cntr.reefer.plugCd;
            //Show Image PluggedIn for PIM ROW POW plugCd
            if (inven.cntr.reefer.plugCd == "PIM" || inven.cntr.reefer.plugCd == "ROW" || inven.cntr.reefer.plugCd == "POW")
            {
                this.Img_PluggedIn.Visibility = Visibility.Visible;
                this.Img_PluggedIn1.Visibility = Visibility.Visible;
                this.TextBlock_ReFlug.Visibility = Visibility.Hidden;
                this.TextBlock_ReFlug1.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Img_PluggedIn.Visibility = Visibility.Hidden;
                this.Img_PluggedIn1.Visibility = Visibility.Hidden;
                this.TextBlock_ReFlug.Visibility = Visibility.Visible;
                this.TextBlock_ReFlug1.Visibility = Visibility.Visible;
            }

            if (inven.jobTp == "GI" || inven.jobTp == "GO" || inven.jobTp == "GC")
            {
                this.TextBlock_TruckNo.Text = inven.batNo;
                this.TextBlock_TruckNo1.Text = inven.batNo;
            }
            else
            {
                this.TextBlock_TruckNo.Text = inven.vehicle;
                this.TextBlock_TruckNo1.Text = inven.vehicle;
            }
            this.TextBlock_TruckNo.Visibility = PresentationMgr.MainView.UC_ExternalTruckNoView.hideExternalTruckBay == "Y" ? Visibility.Hidden : Visibility.Visible;
            this.TextBlock_TruckNo1.Visibility = PresentationMgr.MainView.UC_ExternalTruckNoView.hideExternalTruckBay == "Y" ? Visibility.Hidden : Visibility.Visible;

            this.Tbl_QcNo.Text = inven.cntr.qcNo;
            this.Tbl_QcNo1.Text = inven.cntr.qcNo;

            String qcEtw = inven.cntr.qcEtw;
            if (qcEtw.Length > 8)
                qcEtw = qcEtw.Substring(qcEtw.Length - 8);
            this.Tbl_QcEtw.Text = qcEtw;
            this.Tbl_QcEtw1.Text = qcEtw;

            this.TextBlock_Inventory_Number.Text = inven.cntr.cntrNo.Length > 3 ?
                inven.cntr.cntrNo.Substring(inven.cntr.cntrNo.Length - 4) : String.Empty;

            this.TextBlock_Inventory_Number1.Text = inven.cntr.cntrNo.Length > 3 ?
             inven.cntr.cntrNo.Substring(inven.cntr.cntrNo.Length - 4) : String.Empty;

            //inven.fColor = "#null";
            //inven.bColor = "#null";
            if (inven.bColor.Equals("#null") && !lstContNoLogged.Contains(inven.cntr.cntrNo + inven.cntr.pod))
            {
                SaveLog("Container Number: " + inven.cntr.cntrNo + ", Location: " + inven.loc.location + ", POD: " + inven.cntr.pod +
                        ", Background Color: " + inven.bColor + ", Fore color: " + inven.fColor);
                lstContNoLogged.Add(inven.cntr.cntrNo + inven.cntr.pod);
                inven.bColor = "#ffffff";
            }
            if (!String.IsNullOrEmpty(inven.bColor))
            {
                this.Image_ContainerType.Source = null;
                this.Image_ContainerType1.Source = null;
                useColor = true;
                try
                {
                    this.Layout.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(inven.bColor));
                    this.Layout1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(inven.bColor));
                }
                catch (Exception e)
                {
                    useColor = false;
                    RefreshContainerItem();
                    if (!lstContNoLogged.Contains(inven.cntr.cntrNo + inven.cntr.pod))
                    {
                        SaveLog("Container Number: " + inven.cntr.cntrNo + ", Location: " + inven.loc.location + ", POD: " + inven.cntr.pod +
                            ", Background Color: " + inven.bColor + ", Fore color: " + inven.fColor);
                        lstContNoLogged.Add(inven.cntr.cntrNo + inven.cntr.pod);
                    }
                }
            }
            else
            {
                useColor = false;
                RefreshContainerItem();
            }

            if (inven.fColor.Equals("#null") && !lstContNoLogged.Contains(inven.cntr.cntrNo + inven.cntr.pod))
            {
                SaveLog("Container Number: " + inven.cntr.cntrNo + ", Location: " + inven.loc.location + ", POD: " + inven.cntr.pod +
                        ", Background Color: " + inven.bColor + ", Fore color: " + inven.fColor);
                lstContNoLogged.Add(inven.cntr.cntrNo + inven.cntr.pod);
                inven.fColor = "#000000";
            }

            if (!String.IsNullOrEmpty(inven.fColor))
            {
                try
                {
                    SolidColorBrush fontColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(inven.fColor));
                    this.TextBlock_VesselCode.Foreground = fontColor;
                    this.TextBlock_Operator.Foreground = fontColor;
                    this.TextBlock_Inventory_Number.Foreground = fontColor;
                    this.TextBlock_SizeType.Foreground = fontColor;
                    this.TextBlock_Fm.Foreground = fontColor;
                    this.TextBlock_JobType.Foreground = fontColor;
                    this.TextBlock_JobTypeKor.Foreground = fontColor;
                    this.TextBlock_ReFlug.Foreground = fontColor;
                    this.Tbl_QcNo.Foreground = fontColor;
                    this.Tbl_QcEtw.Foreground = fontColor;
                    this.TextBlock_TruckNo.Foreground = fontColor;
                    this.TextBlock_Hold.Foreground = fontColor;
                    this.TextBlock_Bundle_Content.Foreground = fontColor;

                    this.TextBlock_VesselCode1.Foreground = fontColor;
                    this.TextBlock_Operator1.Foreground = fontColor;
                    this.TextBlock_Inventory_Number1.Foreground = fontColor;
                    this.TextBlock_SizeType1.Foreground = fontColor;
                    this.TextBlock_Fm1.Foreground = fontColor;
                    this.TextBlock_JobType1.Foreground = fontColor;
                    this.TextBlock_JobTypeKor1.Foreground = fontColor;
                    this.TextBlock_ReFlug1.Foreground = fontColor;
                    this.Tbl_QcNo1.Foreground = fontColor;
                    this.Tbl_QcEtw1.Foreground = fontColor;
                    this.TextBlock_TruckNo1.Foreground = fontColor;
                    this.TextBlock_Hold1.Foreground = fontColor;
                    this.TextBlock_Bundle_Content1.Foreground = fontColor;
                }
                catch (Exception e)
                {
                    if (!lstContNoLogged.Contains(inven.cntr.cntrNo + inven.cntr.pod))
                    {
                        SaveLog("Container Number: " + inven.cntr.cntrNo + ", Location: " + inven.loc.location + ", POD: " + inven.cntr.pod +
                            ", Background Color: " + inven.bColor + ", Fore color: " + inven.fColor);
                        lstContNoLogged.Add(inven.cntr.cntrNo + inven.cntr.pod);
                    }
                    this.TextBlock_VesselCode.Foreground = Brushes.Black;
                    this.TextBlock_Operator.Foreground = Brushes.Black;
                    this.TextBlock_Inventory_Number.Foreground = Brushes.Black;
                    this.TextBlock_SizeType.Foreground = Brushes.Black;
                    this.TextBlock_Fm.Foreground = Brushes.Black;
                    this.TextBlock_JobType.Foreground = Brushes.Black;
                    this.TextBlock_JobTypeKor.Foreground = Brushes.Black;
                    this.TextBlock_ReFlug.Foreground = Brushes.Black;
                    this.Tbl_QcNo.Foreground = Brushes.Black;
                    this.Tbl_QcEtw.Foreground = Brushes.Black;
                    this.TextBlock_TruckNo.Foreground = Brushes.Black;
                    this.TextBlock_Hold.Foreground = Brushes.Black;
                    this.TextBlock_Bundle_Content.Foreground = Brushes.Black;

                    this.TextBlock_VesselCode1.Foreground = Brushes.Black;
                    this.TextBlock_Operator1.Foreground = Brushes.Black;
                    this.TextBlock_Inventory_Number1.Foreground = Brushes.Black;
                    this.TextBlock_SizeType1.Foreground = Brushes.Black;
                    this.TextBlock_Fm1.Foreground = Brushes.Black;
                    this.TextBlock_JobType1.Foreground = Brushes.Black;
                    this.TextBlock_JobTypeKor1.Foreground = Brushes.Black;
                    this.TextBlock_ReFlug1.Foreground = Brushes.Black;
                    this.Tbl_QcNo1.Foreground = Brushes.Black;
                    this.Tbl_QcEtw1.Foreground = Brushes.Black;
                    this.TextBlock_TruckNo1.Foreground = Brushes.Black;
                    this.TextBlock_Hold1.Foreground = Brushes.Black;
                    this.TextBlock_Bundle_Content1.Foreground = Brushes.Black;
                }
            }

            if (!string.IsNullOrEmpty(inven.jobTp))
            {
                TextBlock_JobType.Background = Brushes.White;
                TextBlock_JobType.Foreground = Brushes.Black;
                TextBlock_JobType1.Background = Brushes.White;
                TextBlock_JobType1.Foreground = Brushes.Black;

                TextBlock_JobTypeKor.Background = Brushes.White;
                TextBlock_JobTypeKor.Foreground = Brushes.Black;
                TextBlock_JobTypeKor1.Background = Brushes.White;
                TextBlock_JobTypeKor1.Foreground = Brushes.Black;
            }
            if ("GO".Equals(inven.jobTp))
            {
                TextBlock_JobType.Foreground = Brushes.Red;
                TextBlock_JobTypeKor.Foreground = Brushes.Red;
                TextBlock_JobType1.Foreground = Brushes.Red;
                TextBlock_JobTypeKor1.Foreground = Brushes.Red;
            }
            if (!String.IsNullOrEmpty(TextBlock_TruckNo.Text))
            {
                TextBlock_TruckNo.Background = Brushes.White;
                TextBlock_TruckNo.Foreground = Brushes.Black;
                TextBlock_TruckNo1.Background = Brushes.White;
                TextBlock_TruckNo1.Foreground = Brushes.Black;
            }
            if (inven.cntr.isHold)
            {
                this.TextBlock_Hold.Visibility = Visibility.Visible;
                this.TextBlock_Hold1.Visibility = Visibility.Visible;
            }
            else
            {
                this.TextBlock_Hold.Visibility = Visibility.Hidden;
                this.TextBlock_Hold1.Visibility = Visibility.Hidden;
            }

            if (inven.cntr.isDmg)
            {
                TextBlock_Damage.Visibility = Visibility.Visible;
                TextBlock_Damage1.Visibility = Visibility.Visible;
            }

            if (!String.IsNullOrEmpty(inven.cntr.rmk))
            {
                TextBlock_RMK.Visibility = Visibility.Visible;
                TextBlock_RMK1.Visibility = Visibility.Visible;
            }

            if (inven.cntr.isHighCubic)
            {
                TextBlock_HighCubic.Visibility = Visibility.Visible;
                TextBlock_HighCubic1.Visibility = Visibility.Visible;
            }

            if (inven.cntr.isBundle)
            {
                TextBlock_Bundle.Visibility = Visibility.Visible;
                TextBlock_Bundle1.Visibility = Visibility.Visible;
            }

            if ("1".Equals(inven.pickSetChk) || this._inventory.jobTp == "RH" || this._inventory.jobTp == "AH")
            {
                Pol_PickUp.Visibility = Visibility.Visible;
                Pol_PickUp1.Visibility = Visibility.Visible;

                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);

                if (target != null && target.cntr != null && target.cntr.cntrNo.Equals(inven.cntr.cntrNo)
                    && (this._inventory.jobTp == "RH" || this._inventory.jobTp == "AH"))
                {
                    this.Layout.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff00cc"));
                    this.Layout1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff00cc"));
                }
            }
            else if ("2".Equals(inven.pickSetChk))
            {
                Pol_SetDown.Visibility = Visibility.Visible;
                Pol_SetDown1.Visibility = Visibility.Visible;
            }

            //Pol_SetDown.Stroke = new SolidColorBrush(Colors.Black);
            //Pol_SetDown.StrokeThickness = 1;
            //Pol_PickUp.Stroke = new SolidColorBrush(Colors.Black);
            //Pol_PickUp.StrokeThickness = 1;
            //foreach (var listitem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
            {
                //JobListItem item = listitem as JobListItem;
                //var jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                //if (jobOrder != null && jobOrder.type.jobStatus == "P")
                {
                    //if (jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH")
                    {
                        if (PresentationMgr.MainView.plcDomainTwistLock.cntrNo == this._inventory.cntr.cntrNo
                            //&& PresentationMgr.MainView.plcdomain.wrkCd != "2"
                            )
                        {
                            this.Layout.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEF6A4"));
                            this.Layout1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEF6A4"));
                            Pol_SetDown.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_SetDown.StrokeThickness = 2;
                            Pol_SetDown1.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_SetDown1.StrokeThickness = 2;

                            Pol_PickUp.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_PickUp.StrokeThickness = 2;
                            Pol_PickUp1.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_PickUp1.StrokeThickness = 2;

                            if (!Pol_PickUp.IsVisible && !Pol_SetDown.IsVisible)
                                Pol_PickUp.Visibility = Visibility.Visible;

                            if (!Pol_PickUp1.IsVisible && !Pol_SetDown1.IsVisible)
                                Pol_PickUp1.Visibility = Visibility.Visible;

                            //MAKE ALL CONTENT TO BLACK COLOR
                            this.TextBlock_VesselCode.Foreground = Brushes.Black;
                            this.TextBlock_Operator.Foreground = Brushes.Black;
                            this.TextBlock_Inventory_Number.Foreground = Brushes.Black;
                            this.TextBlock_SizeType.Foreground = Brushes.Black;
                            this.TextBlock_Fm.Foreground = Brushes.Black;
                            this.TextBlock_JobType.Foreground = Brushes.Black;
                            this.TextBlock_JobTypeKor.Foreground = Brushes.Black;
                            this.TextBlock_ReFlug.Foreground = Brushes.Black;
                            this.Tbl_QcNo.Foreground = Brushes.Black;
                            this.Tbl_QcEtw.Foreground = Brushes.Black;
                            this.TextBlock_TruckNo.Foreground = Brushes.Black;
                            this.TextBlock_Hold.Foreground = Brushes.Black;
                            this.TextBlock_Bundle_Content.Foreground = Brushes.Black;

                            this.TextBlock_VesselCode1.Foreground = Brushes.Black;
                            this.TextBlock_Operator1.Foreground = Brushes.Black;
                            this.TextBlock_Inventory_Number1.Foreground = Brushes.Black;
                            this.TextBlock_SizeType1.Foreground = Brushes.Black;
                            this.TextBlock_Fm1.Foreground = Brushes.Black;
                            this.TextBlock_JobType1.Foreground = Brushes.Black;
                            this.TextBlock_JobTypeKor1.Foreground = Brushes.Black;
                            this.TextBlock_ReFlug1.Foreground = Brushes.Black;
                            this.Tbl_QcNo1.Foreground = Brushes.Black;
                            this.Tbl_QcEtw1.Foreground = Brushes.Black;
                            this.TextBlock_TruckNo1.Foreground = Brushes.Black;
                            this.TextBlock_Hold1.Foreground = Brushes.Black;
                            this.TextBlock_Bundle_Content1.Foreground = Brushes.Black;
                        }
                        else
                        {
                            Pol_SetDown.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_SetDown.StrokeThickness = 1;
                            Pol_SetDown1.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_SetDown1.StrokeThickness = 1;

                            Pol_PickUp.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_PickUp.StrokeThickness = 1;
                            Pol_PickUp1.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_PickUp1.StrokeThickness = 1;
                        }
                    }
                }
            }
            // Check Background & Foreground white
            if (((SolidColorBrush)Layout.Background).Color == whiteColor.Color && ((SolidColorBrush)TextBlock_Inventory_Number.Foreground).Color == whiteColor.Color)
            {
                this.TextBlock_VesselCode.Foreground = blackColor;
                this.TextBlock_Operator.Foreground = blackColor;
                this.TextBlock_Inventory_Number.Foreground = blackColor;
                this.TextBlock_SizeType.Foreground = blackColor;
                this.TextBlock_Fm.Foreground = blackColor;
                this.TextBlock_JobType.Foreground = blackColor;
                this.TextBlock_JobTypeKor.Foreground = blackColor;
                this.TextBlock_ReFlug.Foreground = blackColor;
                this.Tbl_QcNo.Foreground = blackColor;
                this.Tbl_QcEtw.Foreground = blackColor;
                this.TextBlock_TruckNo.Foreground = blackColor;
                this.TextBlock_Hold.Foreground = blackColor;
                this.TextBlock_Bundle_Content.Foreground = blackColor;

                this.TextBlock_VesselCode1.Foreground = blackColor;
                this.TextBlock_Operator1.Foreground = blackColor;
                this.TextBlock_Inventory_Number1.Foreground = blackColor;
                this.TextBlock_SizeType1.Foreground = blackColor;
                this.TextBlock_Fm1.Foreground = blackColor;
                this.TextBlock_JobType1.Foreground = blackColor;
                this.TextBlock_JobTypeKor1.Foreground = blackColor;
                this.TextBlock_ReFlug1.Foreground = blackColor;
                this.Tbl_QcNo1.Foreground = blackColor;
                this.Tbl_QcEtw1.Foreground = blackColor;
                this.TextBlock_TruckNo1.Foreground = blackColor;
                this.TextBlock_Hold1.Foreground = blackColor;
                this.TextBlock_Bundle_Content1.Foreground = blackColor;
            }
            if (inven.cntr.reefer.plugCd == "POC")
            {
                this.TextBlock_ReFlug.Foreground = Brushes.Blue;
                this.TextBlock_ReFlug1.Foreground = Brushes.Blue;
            }
            if (inven.isTopOog)
            {
                Pol_Up.Visibility = Visibility.Visible;
                Pol_Up1.Visibility = Visibility.Visible;
            }
            if (inven.isLeftOog)
            {
                Pol_Left.Visibility = Visibility.Visible;
                Pol_Left1.Visibility = Visibility.Visible;
            }
            if (inven.isRightOog)
            {
                Pol_Right.Visibility = Visibility.Visible;
                Pol_Right1.Visibility = Visibility.Visible;
            }

            //Check swap item
            //if ((PresentationMgr.Singleton.swapList.Find(item => item.cntrNo == inven.cntr.cntrNo) != null) || (PresentationMgr.Singleton.reservedList.Find(item => item.cntrNo == inven.cntr.cntrNo) != null))
            if (PresentationMgr.Singleton.swapListRTG.Find(item => item.cntrNo == inven.cntr.cntrNo) != null)
            {
                Swap_Circle.Visibility = Visibility.Visible;
                Swap_Circle1.Visibility = Visibility.Visible;
            }
            else
            {
                Swap_Circle.Visibility = Visibility.Hidden;
                Swap_Circle1.Visibility = Visibility.Hidden;
            }

            this.CorrectionSelected = inven.IsCorrectionSelect;
        }

        public void SaveLog(string sJob)  // nDataType 0 EEv2JobOrder, 
        {
            //try
            //{
            //    string sRootPath = AppCfgMgr.GetAppDirectory();
            //    string sDirPath = sRootPath + @"{0}\Log\"
            //        + System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day;

            //    if (Directory.Exists(sDirPath) == false)
            //    {
            //        Directory.CreateDirectory(sDirPath);
            //    }

            //    string logFilePath = @sDirPath + "/YC_LOG_" + System.DateTime.Now.Hour + ".txt";

            //    FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            //    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            //    sw.WriteLine("//===========================================================================");
            //    sw.WriteLine("[" + System.DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "]" + sJob);
            //    sw.WriteLine("//===========================================================================\r\n");
            //    sw.Flush();
            //    sw.Close();
            //    fs.Close();
            //}
            //catch (Exception ex)
            //{

            //}
        }

        public void SetOverValue(Int32 value)
        {
            //this._overValues |= value;
            //BitmapImage img = this.GetOvervalueImage(this._overValues);

            //if (img != null)
            //{
            //    Image_ContainerType_OverValue.Source = img;
            //    Image_ContainerType_OverValue.Visibility = Visibility.Visible;
            //}
            //else
            //    Image_ContainerType_OverValue.Visibility = Visibility.Hidden;
        }

        public void MakeHiddenControls()
        {
            Pol_SetDown.Visibility = Visibility.Hidden;
            Pol_PickUp.Visibility = Visibility.Hidden;
            Pol_Up.Visibility = Visibility.Hidden;
            Pol_Left.Visibility = Visibility.Hidden;
            Pol_Right.Visibility = Visibility.Hidden;
            Swap_Circle.Visibility = Visibility.Hidden;
            TextBlock_Bundle.Visibility = Visibility.Hidden;
            TextBlock_HighCubic.Visibility = Visibility.Hidden;
            TextBlock_Damage.Visibility = Visibility.Hidden;
            TextBlock_RMK.Visibility = Visibility.Hidden;
            Img_PluggedIn.Visibility = Visibility.Hidden;

            Pol_SetDown1.Visibility = Visibility.Hidden;
            Pol_PickUp1.Visibility = Visibility.Hidden;
            Pol_Up1.Visibility = Visibility.Hidden;
            Pol_Left1.Visibility = Visibility.Hidden;
            Pol_Right1.Visibility = Visibility.Hidden;
            Swap_Circle1.Visibility = Visibility.Hidden;
            TextBlock_Bundle1.Visibility = Visibility.Hidden;
            TextBlock_HighCubic1.Visibility = Visibility.Hidden;
            TextBlock_Damage1.Visibility = Visibility.Hidden;
            TextBlock_RMK1.Visibility = Visibility.Hidden;
            Img_PluggedIn1.Visibility = Visibility.Hidden;
        }

        public void SetJobInfo(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if (jobOrder == null)
                return;

            this._jobKey = jobOrder.jobKey;

            //this.Image_Inventory_JobType.Source = GetJobTypeIcon(jobOrder.type.jobTp);
            //this.Grid_TextArear.Visibility = Visibility.Visible;
            //this.Image_Inventory_JobType.Visibility = Visibility.Visible;
        }

        /*
        public void SetInventoryCover(String jobKey, Boolean bUIChange)
        {
            _jobKey = jobKey;

            if (jobKey == null)
            {
                // Hidden
                this.Image_ContainerType_Cover.Visibility = System.Windows.Visibility.Hidden;
                this.Image_ContainerType_Cover.Source = null;
            }
            else
            {
                // set & visible
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);

                if (jobOrder == null)
                    return;

                this.Image_ContainerType_Cover.Visibility = System.Windows.Visibility.Visible;
                this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(jobOrder.cntr.cntrTp);
            }
        }
        */

        public void SetNoneItem()
        {
            this.Image_ContainerType.Source = GetBGImageByContainerType("NoTier");
            this.Image_ContainerType1.Source = GetBGImageByContainerType("NoTier");
        }

        public Boolean setBlink = false;

        public BitmapImage GetBGImageByContainerType(String cntrTp)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }
            if (!dtClockTime.IsEnabled)
                dtClockTime.Start();
            if (_Selected == true)
            {
                if (PresentationMgr.MainView.selectedJobList)
                {
                    var job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                    if (!PresentationMgr.Singleton.CorrectionSource.Pos.Equal(this.ContainerPos))
                    {
                        if (job != null && job.type != null)
                        {
                            if (job.type.jobTp == "LD" || job.type.jobTp == "MO" || job.type.jobTp == "GO")
                                PresentationMgr.Singleton.CorrectionSource.LDMOGOjob = true;
                            else
                                PresentationMgr.Singleton.CorrectionSource.LDMOGOjob = false;
                            this.CorrectionSelected = PresentationMgr.Singleton.SetCorrectionSelect(this.ContainerPos, job.cntr.cntrNo, job.cntr.cntrIso);
                        }
                    }
                    else if (PresentationMgr.MainView.deselectJobList)
                    {
                        this._Selected = false;
                        PresentationMgr.Singleton.CorrectionSource.Clear();
                        this.CorrectionSelected = false;
                    }
                }
                PresentationMgr.MainView.selectedJobList = false;
                PresentationMgr.MainView.deselectJobList = false;//move here by the below line jobType LD/MO/GO retun null
                if (!PresentationMgr.Singleton.CorrectionSource.Pos.IsEmpty() && this.ContainerPos.Equal(PresentationMgr.Singleton.CorrectionSource.Pos))    // 현재 correction source 해제
                {
                    this.CorrectionSelected = true;
                    var job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                    if (job != null && job.type != null)
                    {
                        if (job.type.jobStatus != "P" && this._inventory != null && (job.type.jobTp == "LD" || job.type.jobTp == "GO" || job.type.jobTp == "MO"))
                        {
                            this.Blinked_Up.Visibility = Visibility.Visible;
                            this.Blinked_Up1.Visibility = Visibility.Visible;
                        }
                        if (job.type.jobStatus == "P")
                        {
                            if (job.type.jobTp == "LD" || job.type.jobTp == "MO" || job.type.jobTp == "GO")
                                return null;
                            else
                                imgUri += "Inventory_Select";
                        }
                        else
                            imgUri += "Inventory_Select";

                        if (PresentationMgr.Singleton.preContainerItemSelectedWrongColor != null && PresentationMgr.Singleton.preContainerItemSelectedWrongColor.Name != this.Name)
                        {
                            PresentationMgr.Singleton.preContainerItemSelectedWrongColor.Selected = false;
                            PresentationMgr.Singleton.preContainerItemSelectedWrongColor.CorrectionSelected = false;

                        }
                    }
                    else
                    {
                        return null;
                    }
                    HiddenInfoSelected();
                }
                else
                {
                    this._Selected = false;
                    if (this._inventory != null && this._inventory.cntr != null)
                    {
                        this.SetInventoryInfo(this._inventory);
                        return null;
                    }
                    else
                    {
                        this.Image_ContainerType.Source = PresentationMgr.GetImageSource(imgUri + "Inventory_Default.png");
                        this.Image_ContainerType1.Source = PresentationMgr.GetImageSource(imgUri + "Inventory_Default.png");
                        imgUri += "Inventory_Default";
                    }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(cntrTp))
                {
                    TextBlock_TruckNo.Background = Brushes.Transparent;
                    TextBlock_JobType.Background = Brushes.Transparent;
                    TextBlock_JobTypeKor.Background = Brushes.Transparent;
                    TextBlock_TruckNo1.Background = Brushes.Transparent;
                    TextBlock_JobType1.Background = Brushes.Transparent;
                    TextBlock_JobTypeKor1.Background = Brushes.Transparent;
                    imgUri += "Inventory_Default";
                }
                else if (cntrTp.Equals("NoTier"))
                {
                    TextBlock_TruckNo.Background = Brushes.Transparent;
                    TextBlock_JobType.Background = Brushes.Transparent;
                    TextBlock_JobTypeKor.Background = Brushes.Transparent;
                    TextBlock_TruckNo1.Background = Brushes.Transparent;
                    TextBlock_JobType1.Background = Brushes.Transparent;
                    TextBlock_JobTypeKor1.Background = Brushes.Transparent;

                    this.Layout.ClearValue(Control.BackgroundProperty);
                    this.Layout1.ClearValue(Control.BackgroundProperty);
                    return null;
                }
                else if (_available || (PresentationMgr.Singleton.CurrentUIMode != PresentationMgr.UIMode.SwapView && !useColor))
                    imgUri += "Inventory_Default";
                else if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.SwapView)
                    imgUri += "Inventory_Disabled";
                else
                    return null;

            }
            if (!PresentationMgr.Singleton.showViewINV) //20201014 alway enable VBlock if select container
            {
                if (imgUri.Contains("Inventory_Select") && !PresentationMgr.Singleton.showVBlock && PresentationMgr.Singleton.cntrSelected.Equals(String.Empty) && this._inventory != null && this._inventory.cntr != null)
                {
                    PresentationMgr.Singleton.showVBlock = true;
                    PresentationMgr.Singleton.cntrSelected = this._inventory.cntr.cntrNo;
                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = true;
                }
                else if (!imgUri.Contains("Inventory_Select") && !PresentationMgr.Singleton.cntrSelected.Equals(String.Empty)
                    && ((this._inventory != null && this._inventory.cntr != null && this._inventory.cntr.cntrNo.Equals(PresentationMgr.Singleton.cntrSelected)) || this._inventory == null) && PresentationMgr.Singleton.showVBlock)
                {
                    PresentationMgr.Singleton.showVBlock = false;
                    PresentationMgr.Singleton.cntrSelected = String.Empty;
                    PresentationMgr.MainView.UC_InfomationView.UC_BlockBayInfo.Btn_V_Block.IsEnabled = false;
                }
            }

            //if (imgUri[imgUri.Length - 1].Equals('/')) imgUri += "Inventory_Default";

            imgUri += ".png";
            return PresentationMgr.GetImageSource(imgUri);
        }

        private void blinkSelected_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (_Selected == true)
                {
                    dtClockTime1.Stop();
                    Grid blinkedGrid = new Grid();
                    Grid blinkedGrid1 = new Grid();
                    var cntrTp = "";
                    var jobSts = "";
                    var job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                    if (job == null || job.type == null)
                        return;
                    cntrTp = job.type.jobTp;
                    jobSts = job.type.jobStatus;

                    if (cntrTp == "LD" || cntrTp == "GO" || cntrTp == "MO")
                    {
                        blinkedGrid = this.Blinked_Up;
                        blinkedGrid1 = this.Blinked_Up1;
                    }
                    else if (cntrTp == "DS" || cntrTp == "GI" || cntrTp == "MI"
                            || cntrTp == "RH" || cntrTp == "AH" || cntrTp == "GC" || cntrTp == "LC")
                    {
                        blinkedGrid = this.Blinked_Down;
                        blinkedGrid1 = this.Blinked_Down1;
                    }

                    if (setBlink)
                    {
                        if (jobSts == "P")
                        {
                            if (cntrTp == "DS" || cntrTp == "MI" || cntrTp == "GI"
                                || cntrTp == "RH" || cntrTp == "AH" || cntrTp == "GC" || cntrTp == "LC")
                            {
                                InitSkinImageForBlink();
                                blinkedGrid.Visibility = Visibility.Visible;
                                blinkedGrid1.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            InitSkinImageForBlink();
                            blinkedGrid.Visibility = Visibility.Visible;
                            blinkedGrid1.Visibility = Visibility.Visible;
                        }

                        setBlink = false;
                    }
                    else
                    {
                        if ((jobSts != "P" && (cntrTp == "LD" || cntrTp == "GO" || cntrTp == "MO"))
                                               || ((cntrTp == "DS" || cntrTp == "MI" || cntrTp == "GI" || cntrTp == "LC" || cntrTp == "GC" || cntrTp == "RH" || cntrTp == "AH") && _inventory != null && _inventory.cntr != null && !String.IsNullOrEmpty(this._inventory.cntr.cntrNo))
                            )
                        {
                            SetLDMOGOInfo(this._inventory);
                        }
                        blinkedGrid.Visibility = Visibility.Hidden;
                        blinkedGrid1.Visibility = Visibility.Hidden;
                        setBlink = true;
                    }
                }
                else
                {
                    this.Blinked_Up.Visibility = Visibility.Hidden;
                    this.Blinked_Down.Visibility = Visibility.Hidden;
                    this.Blinked_Up1.Visibility = Visibility.Hidden;
                    this.Blinked_Down1.Visibility = Visibility.Hidden;
                    dtClockTime.Stop();
                }
            }));
        }
        private void blinkSelected_Tick1(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (_Selected == true)
                {
                    Grid blinkedGrid = new Grid();
                    Grid blinkedGrid1 = new Grid();
                    var cntrTp = "";
                    var jobSts = "";
                    var job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                    if (job == null || job.type == null)
                        return;
                    cntrTp = job.type.jobTp;
                    jobSts = job.type.jobStatus;

                    if (cntrTp == "LD" || cntrTp == "GO" || cntrTp == "MO")
                    {
                        blinkedGrid = this.Blinked_Up;
                        blinkedGrid1 = this.Blinked_Up1;
                    }
                    else if (cntrTp == "DS" || cntrTp == "GI" || cntrTp == "MI"
                            || cntrTp == "RH" || cntrTp == "AH" || cntrTp == "GC" || cntrTp == "LC")
                    {
                        blinkedGrid = this.Blinked_Down;
                        blinkedGrid1 = this.Blinked_Down1;
                    }

                    if (setBlink)
                    {
                        if (jobSts == "P")
                        {
                            if (cntrTp == "DS" || cntrTp == "MI" || cntrTp == "GI"
                                || cntrTp == "RH" || cntrTp == "AH" || cntrTp == "GC" || cntrTp == "LC")
                            {
                                InitSkinImageForBlink();
                                blinkedGrid.Visibility = Visibility.Visible;
                                blinkedGrid1.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            InitSkinImageForBlink();
                            blinkedGrid.Visibility = Visibility.Visible;
                            blinkedGrid1.Visibility = Visibility.Visible;
                        }

                        setBlink = false;
                    }
                    else
                    {
                        if ((jobSts != "P" && (cntrTp == "LD" || cntrTp == "GO" || cntrTp == "MO"))
                                               || ((cntrTp == "DS" || cntrTp == "MI" || cntrTp == "GI" || cntrTp == "LC" || cntrTp == "GC" || cntrTp == "RH" || cntrTp == "AH") && _inventory != null && !String.IsNullOrEmpty(this._inventory.cntr.cntrNo))
                            )
                        {
                            SetLDMOGOInfo(this._inventory);
                        }
                        blinkedGrid.Visibility = Visibility.Hidden;
                        blinkedGrid1.Visibility = Visibility.Hidden;
                        setBlink = true;
                    }
                }
                else
                {
                    this.Blinked_Up.Visibility = Visibility.Hidden;
                    this.Blinked_Down.Visibility = Visibility.Hidden;
                    this.Blinked_Up1.Visibility = Visibility.Hidden;
                    this.Blinked_Down1.Visibility = Visibility.Hidden;
                }
            }));
        }
        public void SetBlinkSelected(String jobType)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                String imgUri = String.Empty;
                if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
                {
                    imgUri = @"/Images/Common/Inventory/";
                }
                else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
                {
                    imgUri = @"/Images(Night)/Common/Inventory/";
                }
                imgUri += "Inventory_Select.png";
                PresentationMgr.GetImageSource(imgUri);
                this.Image_ContainerType.Source = PresentationMgr.GetImageSource(imgUri);
                this.Image_ContainerType1.Source = PresentationMgr.GetImageSource(imgUri);
                //if (_Selected == true)
                //{
                Grid blinkedGrid = new Grid();
                Grid blinkedGrid1 = new Grid();
                var cntrTp = jobType;
                var jobSts = "";
                //var job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                //if (job == null)
                //    return;
                //cntrTp = job.type.jobTp;
                //jobSts = job.type.jobStatus;

                if (cntrTp == "LD" || cntrTp == "GO" || cntrTp == "MO")
                {
                    blinkedGrid = this.Blinked_Up;
                    blinkedGrid1 = this.Blinked_Up1;
                }
                else if (cntrTp == "DS" || cntrTp == "GI" || cntrTp == "MI"
                        || cntrTp == "RH" || cntrTp == "AH" || cntrTp == "GC" || cntrTp == "LC")
                {
                    blinkedGrid = this.Blinked_Down;
                    blinkedGrid1 = this.Blinked_Down1;
                }
                InitSkinImageForBlink();
                blinkedGrid.Visibility = Visibility.Visible;
                blinkedGrid1.Visibility = Visibility.Visible;
                dtClockTime1.Start();
                //if (true)
                //{
                //    if (jobSts == "P")
                //    {
                //        if (cntrTp == "DS" || cntrTp == "MI" || cntrTp == "GI"
                //            || cntrTp == "RH" || cntrTp == "AH" || cntrTp == "GC" || cntrTp == "LC")
                //        {
                //            InitSkinImageForBlink();
                //            blinkedGrid.Visibility = Visibility.Visible;
                //            blinkedGrid1.Visibility = Visibility.Visible;
                //        }
                //    }
                //    else
                //    {
                //        InitSkinImageForBlink();
                //        blinkedGrid.Visibility = Visibility.Visible;
                //        blinkedGrid1.Visibility = Visibility.Visible;
                //    }

                //    setBlink = false;
                //}
                //else
                //{
                //    if (jobSts != "P" && (cntrTp == "LD" || cntrTp == "GO" || cntrTp == "MO"))
                //    {
                //        SetLDMOGOInfo(this._inventory);
                //    }
                //    blinkedGrid.Visibility = Visibility.Hidden;
                //    blinkedGrid1.Visibility = Visibility.Hidden;
                //    setBlink = true;
                //}
                //}
                //else
                //{
                //    this.Blinked_Up.Visibility = Visibility.Hidden;
                //    this.Blinked_Down.Visibility = Visibility.Hidden;
                //    this.Blinked_Up1.Visibility = Visibility.Hidden;
                //    this.Blinked_Down1.Visibility = Visibility.Hidden;
                //    dtClockTime.Stop();
                //}
            }));
        }
        public void SetLDMOGOInfo(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory inven)
        {
            _inventory = inven;
            if (_inventory == null)
            {
                InitSkinImage();
                return;
            }

            if (inven.cntr.isBundle && !inven.cntr.isBundleMaster)
                return;

            this.ContainerPos.m_cBlock = _inventory.loc.blck;
            this.ContainerPos.m_cBay = PresentationMgr.GetFrontOddBay(_inventory.loc.bay);
            this.ContainerPos.m_cRow = _inventory.loc.row;
            this.ContainerPos.m_cTier = _inventory.loc.tier;

            this.Image_ContainerType_Cover.Source = GetCoverImageByContainerType(inven.cntr.cntrTp);
            this.Image_ContainerType_Cover1.Source = GetCoverImageByContainerType(inven.cntr.cntrTp);

            //this.Grid_TextArear.Visibility = System.Windows.Visibility.Visible;

            /*
             * •	1 : Length
               •	2 : F/E (F:Full / E:Empty)
               •	3 : Commodity
               •	4 : Class code
               •	5 : Container type / HC
               (For a high cubic container, instead of a container type, HC is displayed.)
               •	6 : Operator
               •	7 : Container grade (SD)
               •	8 : Last 4 digit of Container number 
             */
            this.TextBlock_VesselCode.Text = inven.cntr.outVsl;
            this.TextBlock_VesselCode1.Text = inven.cntr.outVsl;

            this.TextBlock_Operator.Text = inven.cntr.opr;
            this.TextBlock_Operator1.Text = inven.cntr.opr;

            this.TextBlock_SizeType.Text = inven.cntr.cntrLen + inven.cntr.cntrTp;
            this.TextBlock_SizeType1.Text = inven.cntr.cntrLen + inven.cntr.cntrTp;

            this.TextBlock_Fm.Text = inven.cntr.fullMty == "F" ? (inven.cntr.fullMty + "/" + inven.cntr.cntrWgt) : "M";
            this.TextBlock_Fm1.Text = inven.cntr.fullMty == "F" ? (inven.cntr.fullMty + "/" + inven.cntr.cntrWgt) : "M";

            String lang = PresentationMgr.AppWin.UC_MachineSettingView.langString;

            this.TextBlock_JobTypeKor.Text = inven.jobTpKorShort;
            this.TextBlock_JobTypeKor1.Text = inven.jobTpKorShort;

            this.TextBlock_JobType.Text = inven.jobTp;
            this.TextBlock_JobType1.Text = inven.jobTp;

            if (lang.Contains("Korea"))
            {
                this.TextBlock_JobType.Visibility = Visibility.Hidden;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Visible;
                this.TextBlock_JobType1.Visibility = Visibility.Hidden;
                this.TextBlock_JobTypeKor1.Visibility = Visibility.Visible;
            }
            else
            {
                this.TextBlock_JobType.Visibility = Visibility.Visible;
                this.TextBlock_JobTypeKor.Visibility = Visibility.Hidden;
                this.TextBlock_JobType1.Visibility = Visibility.Visible;
                this.TextBlock_JobTypeKor1.Visibility = Visibility.Hidden;
            }

            this.TextBlock_ReFlug.Text = inven.cntr.reefer.plugCd;
            this.TextBlock_ReFlug1.Text = inven.cntr.reefer.plugCd;
            //Show Image PluggedIn for PIM ROW POW plugCd
            if (inven.cntr.reefer.plugCd == "PIM" || inven.cntr.reefer.plugCd == "ROW" || inven.cntr.reefer.plugCd == "POW")
            {
                this.Img_PluggedIn.Visibility = Visibility.Visible;
                this.Img_PluggedIn1.Visibility = Visibility.Visible;
                this.TextBlock_ReFlug.Visibility = Visibility.Hidden;
                this.TextBlock_ReFlug1.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Img_PluggedIn.Visibility = Visibility.Hidden;
                this.Img_PluggedIn1.Visibility = Visibility.Hidden;
                this.TextBlock_ReFlug.Visibility = Visibility.Visible;
                this.TextBlock_ReFlug1.Visibility = Visibility.Visible;
            }
            if (inven.jobTp == "GI" || inven.jobTp == "GO" || inven.jobTp == "GC")
            {
                this.TextBlock_TruckNo.Text = inven.batNo;
                this.TextBlock_TruckNo1.Text = inven.batNo;
            }
            else
            {
                this.TextBlock_TruckNo.Text = inven.vehicle;
                this.TextBlock_TruckNo1.Text = inven.vehicle;
            }
            this.TextBlock_TruckNo.Visibility = PresentationMgr.MainView.UC_ExternalTruckNoView.hideExternalTruckBay == "Y" ? Visibility.Hidden : Visibility.Visible;
            this.TextBlock_TruckNo1.Visibility = PresentationMgr.MainView.UC_ExternalTruckNoView.hideExternalTruckBay == "Y" ? Visibility.Hidden : Visibility.Visible;

            this.Tbl_QcNo.Text = inven.cntr.qcNo;
            this.Tbl_QcNo1.Text = inven.cntr.qcNo;

            String qcEtw = inven.cntr.qcEtw;
            if (qcEtw.Length > 8)
                qcEtw = qcEtw.Substring(qcEtw.Length - 8);
            this.Tbl_QcEtw.Text = qcEtw;
            this.Tbl_QcEtw1.Text = qcEtw;

            this.TextBlock_Inventory_Number.Text = inven.cntr.cntrNo.Length > 3 ?
                inven.cntr.cntrNo.Substring(inven.cntr.cntrNo.Length - 4) : String.Empty;

            this.TextBlock_Inventory_Number1.Text = inven.cntr.cntrNo.Length > 3 ?
                inven.cntr.cntrNo.Substring(inven.cntr.cntrNo.Length - 4) : String.Empty;

            if (inven.fColor.Equals("#null"))
            {
                SaveLog("Container Number: " + inven.cntr.cntrNo + ", Location: " + inven.loc.location + ", POD: " + inven.cntr.pod +
                        ", Background Color: " + inven.bColor + ", Fore color: " + inven.fColor);
                inven.fColor = "#000000";
            }

            if (!String.IsNullOrEmpty(inven.fColor))
            {
                try
                {
                    SolidColorBrush fontColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(inven.fColor));
                    this.TextBlock_VesselCode.Foreground = fontColor;
                    this.TextBlock_Operator.Foreground = fontColor;
                    this.TextBlock_Inventory_Number.Foreground = fontColor;
                    this.TextBlock_SizeType.Foreground = fontColor;
                    this.TextBlock_Fm.Foreground = fontColor;
                    this.TextBlock_JobType.Foreground = fontColor;
                    this.TextBlock_JobTypeKor.Foreground = fontColor;
                    this.TextBlock_ReFlug.Foreground = fontColor;
                    this.Tbl_QcNo.Foreground = fontColor;
                    this.Tbl_QcEtw.Foreground = fontColor;
                    this.TextBlock_TruckNo.Foreground = fontColor;
                    this.TextBlock_Hold.Foreground = fontColor;
                    this.TextBlock_Bundle_Content.Foreground = fontColor;

                    this.TextBlock_VesselCode1.Foreground = fontColor;
                    this.TextBlock_Operator1.Foreground = fontColor;
                    this.TextBlock_Inventory_Number1.Foreground = fontColor;
                    this.TextBlock_SizeType1.Foreground = fontColor;
                    this.TextBlock_Fm1.Foreground = fontColor;
                    this.TextBlock_JobType1.Foreground = fontColor;
                    this.TextBlock_JobTypeKor1.Foreground = fontColor;
                    this.TextBlock_ReFlug1.Foreground = fontColor;
                    this.Tbl_QcNo1.Foreground = fontColor;
                    this.Tbl_QcEtw1.Foreground = fontColor;
                    this.TextBlock_TruckNo1.Foreground = fontColor;
                    this.TextBlock_Hold1.Foreground = fontColor;
                    this.TextBlock_Bundle_Content1.Foreground = fontColor;
                }
                catch (Exception e)
                {
                    SaveLog("Container Number: " + inven.cntr.cntrNo + ", Location: " + inven.loc.location + ", POD: " + inven.cntr.pod +
                        ", Background Color: " + inven.bColor + ", Fore color: " + inven.fColor);
                    this.TextBlock_VesselCode.Foreground = Brushes.Black;
                    this.TextBlock_Operator.Foreground = Brushes.Black;
                    this.TextBlock_Inventory_Number.Foreground = Brushes.Black;
                    this.TextBlock_SizeType.Foreground = Brushes.Black;
                    this.TextBlock_Fm.Foreground = Brushes.Black;
                    this.TextBlock_JobType.Foreground = Brushes.Black;
                    this.TextBlock_JobTypeKor.Foreground = Brushes.Black;
                    this.TextBlock_ReFlug.Foreground = Brushes.Black;
                    this.Tbl_QcNo.Foreground = Brushes.Black;
                    this.Tbl_QcEtw.Foreground = Brushes.Black;
                    this.TextBlock_TruckNo.Foreground = Brushes.Black;
                    this.TextBlock_Hold.Foreground = Brushes.Black;
                    this.TextBlock_Bundle_Content.Foreground = Brushes.Black;

                    this.TextBlock_VesselCode1.Foreground = Brushes.Black;
                    this.TextBlock_Operator1.Foreground = Brushes.Black;
                    this.TextBlock_Inventory_Number1.Foreground = Brushes.Black;
                    this.TextBlock_SizeType1.Foreground = Brushes.Black;
                    this.TextBlock_Fm1.Foreground = Brushes.Black;
                    this.TextBlock_JobType1.Foreground = Brushes.Black;
                    this.TextBlock_JobTypeKor1.Foreground = Brushes.Black;
                    this.TextBlock_ReFlug1.Foreground = Brushes.Black;
                    this.Tbl_QcNo1.Foreground = Brushes.Black;
                    this.Tbl_QcEtw1.Foreground = Brushes.Black;
                    this.TextBlock_TruckNo1.Foreground = Brushes.Black;
                    this.TextBlock_Hold1.Foreground = Brushes.Black;
                    this.TextBlock_Bundle_Content1.Foreground = Brushes.Black;
                }
            }

            if (!string.IsNullOrEmpty(inven.jobTp))
            {
                TextBlock_JobType.Background = Brushes.White;
                TextBlock_JobType.Foreground = Brushes.Black;
                TextBlock_JobType1.Background = Brushes.White;
                TextBlock_JobType1.Foreground = Brushes.Black;

                TextBlock_JobTypeKor.Background = Brushes.White;
                TextBlock_JobTypeKor.Foreground = Brushes.Black;
                TextBlock_JobTypeKor1.Background = Brushes.White;
                TextBlock_JobTypeKor1.Foreground = Brushes.Black;
            }
            if ("GO".Equals(inven.jobTp))
            {
                TextBlock_JobType.Foreground = Brushes.Red;
                TextBlock_JobTypeKor.Foreground = Brushes.Red;
                TextBlock_JobType1.Foreground = Brushes.Red;
                TextBlock_JobTypeKor1.Foreground = Brushes.Red;
            }
            if (!String.IsNullOrEmpty(TextBlock_TruckNo.Text))
            {
                TextBlock_TruckNo.Background = Brushes.White;
                TextBlock_TruckNo.Foreground = Brushes.Black;
                TextBlock_TruckNo1.Background = Brushes.White;
                TextBlock_TruckNo1.Foreground = Brushes.Black;
            }
            if (inven.cntr.isHold)
            {
                this.TextBlock_Hold.Visibility = Visibility.Visible;
                this.TextBlock_Hold1.Visibility = Visibility.Visible;
            }
            else
            {
                this.TextBlock_Hold.Visibility = Visibility.Hidden;
                this.TextBlock_Hold1.Visibility = Visibility.Hidden;
            }

            if (inven.cntr.isDmg)
            {
                TextBlock_Damage.Visibility = Visibility.Visible;
                TextBlock_Damage1.Visibility = Visibility.Visible;
            }

            if (!String.IsNullOrEmpty(inven.cntr.rmk))
            {
                TextBlock_RMK.Visibility = Visibility.Visible;
                TextBlock_RMK1.Visibility = Visibility.Visible;
            }

            if (inven.cntr.isHighCubic)
            {
                TextBlock_HighCubic.Visibility = Visibility.Visible;
                TextBlock_HighCubic1.Visibility = Visibility.Visible;
            }

            if (inven.cntr.isBundle)
            {
                TextBlock_Bundle.Visibility = Visibility.Visible;
                TextBlock_Bundle1.Visibility = Visibility.Visible;
            }

            if ("1".Equals(inven.pickSetChk) || this._inventory.jobTp == "RH" || this._inventory.jobTp == "AH")
            {
                Pol_PickUp.Visibility = Visibility.Visible;
                Pol_PickUp1.Visibility = Visibility.Visible;

                var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
            }
            else if ("2".Equals(inven.pickSetChk))
            {
                Pol_SetDown.Visibility = Visibility.Visible;
                Pol_SetDown1.Visibility = Visibility.Visible;
            }

            Pol_SetDown.Stroke = new SolidColorBrush(Colors.Black);
            Pol_SetDown.StrokeThickness = 1;
            Pol_SetDown1.Stroke = new SolidColorBrush(Colors.Black);
            Pol_SetDown1.StrokeThickness = 1;

            Pol_PickUp.Stroke = new SolidColorBrush(Colors.Black);
            Pol_PickUp.StrokeThickness = 1;
            Pol_PickUp1.Stroke = new SolidColorBrush(Colors.Black);
            Pol_PickUp1.StrokeThickness = 1;
            //foreach (var listitem in PresentationMgr.MainView.UC_JobList.ListBox_Job.Items)
            {
                //JobListItem item = listitem as JobListItem;
                //var jobOrder = PresentationMgr.Singleton.JOB_Get(item.JobKey);
                //if (jobOrder != null && jobOrder.type.jobStatus == "P")
                {
                    //if (jobOrder.type.jobTp == "MO" || jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH")
                    {
                        if (PresentationMgr.MainView.plcDomainTwistLock.cntrNo == this._inventory.cntr.cntrNo
                            //&& PresentationMgr.MainView.plcdomain.wrkCd != "2"
                            )
                        {
                            this.Layout.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEF6A4")); //bright yellow
                            this.Layout1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEF6A4"));
                            Pol_SetDown.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_SetDown.StrokeThickness = 2;
                            Pol_SetDown1.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_SetDown1.StrokeThickness = 2;

                            Pol_PickUp.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_PickUp.StrokeThickness = 2;
                            Pol_PickUp1.Stroke = new SolidColorBrush(Colors.Blue);
                            Pol_PickUp1.StrokeThickness = 2;

                            if (!Pol_PickUp.IsVisible && !Pol_SetDown.IsVisible)
                                Pol_PickUp.Visibility = Visibility.Visible;

                            if (!Pol_PickUp1.IsVisible && !Pol_SetDown1.IsVisible)
                                Pol_PickUp1.Visibility = Visibility.Visible;

                            //MAKE ALL CONTENT TO BLACK COLOR
                            this.TextBlock_VesselCode.Foreground = Brushes.Black;
                            this.TextBlock_Operator.Foreground = Brushes.Black;
                            this.TextBlock_Inventory_Number.Foreground = Brushes.Black;
                            this.TextBlock_SizeType.Foreground = Brushes.Black;
                            this.TextBlock_Fm.Foreground = Brushes.Black;
                            this.TextBlock_JobType.Foreground = Brushes.Black;
                            this.TextBlock_JobTypeKor.Foreground = Brushes.Black;
                            this.TextBlock_ReFlug.Foreground = Brushes.Black;
                            this.Tbl_QcNo.Foreground = Brushes.Black;
                            this.Tbl_QcEtw.Foreground = Brushes.Black;
                            this.TextBlock_TruckNo.Foreground = Brushes.Black;
                            this.TextBlock_Hold.Foreground = Brushes.Black;
                            this.TextBlock_Bundle_Content.Foreground = Brushes.Black;

                            this.TextBlock_VesselCode1.Foreground = Brushes.Black;
                            this.TextBlock_Operator1.Foreground = Brushes.Black;
                            this.TextBlock_Inventory_Number1.Foreground = Brushes.Black;
                            this.TextBlock_SizeType1.Foreground = Brushes.Black;
                            this.TextBlock_Fm1.Foreground = Brushes.Black;
                            this.TextBlock_JobType1.Foreground = Brushes.Black;
                            this.TextBlock_JobTypeKor1.Foreground = Brushes.Black;
                            this.TextBlock_ReFlug1.Foreground = Brushes.Black;
                            this.Tbl_QcNo1.Foreground = Brushes.Black;
                            this.Tbl_QcEtw1.Foreground = Brushes.Black;
                            this.TextBlock_TruckNo1.Foreground = Brushes.Black;
                            this.TextBlock_Hold1.Foreground = Brushes.Black;
                            this.TextBlock_Bundle_Content1.Foreground = Brushes.Black;
                        }
                        else
                        {
                            Pol_SetDown.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_SetDown.StrokeThickness = 1;
                            Pol_SetDown1.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_SetDown1.StrokeThickness = 1;

                            Pol_PickUp.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_PickUp.StrokeThickness = 1;
                            Pol_PickUp1.Stroke = new SolidColorBrush(Colors.Black);
                            Pol_PickUp1.StrokeThickness = 1;
                        }
                    }
                }
            }
            // Check Background & Foreground white
            if (((SolidColorBrush)Layout.Background).Color == whiteColor.Color && ((SolidColorBrush)TextBlock_Inventory_Number.Foreground).Color == whiteColor.Color)
            {
                this.TextBlock_VesselCode.Foreground = blackColor;
                this.TextBlock_Operator.Foreground = blackColor;
                this.TextBlock_Inventory_Number.Foreground = blackColor;
                this.TextBlock_SizeType.Foreground = blackColor;
                this.TextBlock_Fm.Foreground = blackColor;
                this.TextBlock_JobType.Foreground = blackColor;
                this.TextBlock_JobTypeKor.Foreground = blackColor;
                this.TextBlock_ReFlug.Foreground = blackColor;
                this.Tbl_QcNo.Foreground = blackColor;
                this.Tbl_QcEtw.Foreground = blackColor;
                this.TextBlock_TruckNo.Foreground = blackColor;
                this.TextBlock_Hold.Foreground = blackColor;
                this.TextBlock_Bundle_Content.Foreground = blackColor;

                this.TextBlock_VesselCode1.Foreground = blackColor;
                this.TextBlock_Operator1.Foreground = blackColor;
                this.TextBlock_Inventory_Number1.Foreground = blackColor;
                this.TextBlock_SizeType1.Foreground = blackColor;
                this.TextBlock_Fm1.Foreground = blackColor;
                this.TextBlock_JobType1.Foreground = blackColor;
                this.TextBlock_JobTypeKor1.Foreground = blackColor;
                this.TextBlock_ReFlug1.Foreground = blackColor;
                this.Tbl_QcNo1.Foreground = blackColor;
                this.Tbl_QcEtw1.Foreground = blackColor;
                this.TextBlock_TruckNo1.Foreground = blackColor;
                this.TextBlock_Hold1.Foreground = blackColor;
                this.TextBlock_Bundle_Content1.Foreground = blackColor;
            }
            if (inven.cntr.reefer.plugCd == "POC")
            {
                this.TextBlock_ReFlug.Foreground = Brushes.Blue;
                this.TextBlock_ReFlug1.Foreground = Brushes.Blue;
            }
            if (inven.isTopOog)
            {
                Pol_Up.Visibility = Visibility.Visible;
                Pol_Up1.Visibility = Visibility.Visible;
            }
            if (inven.isLeftOog)
            {
                Pol_Left.Visibility = Visibility.Visible;
                Pol_Left1.Visibility = Visibility.Visible;
            }
            if (inven.isRightOog)
            {
                Pol_Right.Visibility = Visibility.Visible;
                Pol_Right1.Visibility = Visibility.Visible;
            }

            //Check swap item
            //if ((PresentationMgr.Singleton.swapList.Find(item => item.cntrNo == inven.cntr.cntrNo) != null) || (PresentationMgr.Singleton.reservedList.Find(item => item.cntrNo == inven.cntr.cntrNo) != null))
            if (PresentationMgr.Singleton.swapListRTG.Find(item => item.cntrNo == inven.cntr.cntrNo) != null)
            {
                Swap_Circle.Visibility = Visibility.Visible;
                Swap_Circle1.Visibility = Visibility.Visible;
            }
            else
            {
                Swap_Circle.Visibility = Visibility.Hidden;
                Swap_Circle1.Visibility = Visibility.Hidden;
            }
        }

        public BitmapImage GetBGImageBySelect(Boolean selected)
        {
            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }


            if (selected == true)
                imgUri += "Inventory_CorrectionSelect";
            else
                imgUri += "";


            imgUri += ".png";
            return PresentationMgr.GetImageSource(imgUri);
        }

        public BitmapImage GetCoverImageByContainerType(String cntrTp)
        {
            if (String.IsNullOrEmpty(cntrTp))
                return null;

            String imgUri = String.Empty;
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                imgUri = @"/Images/Common/Inventory/";
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                imgUri = @"/Images(Night)/Common/Inventory/";
            }

            // 커버만 Display되는 경우
            switch (cntrTp)
            {
                case "GE": //GE : General
                case "TK": //TK : Tank
                case "FR": //FR : Flat Rack
                case "OP": //OT : Open Top
                case "BK": //BK : Dry Bulk
                case "AS": // AS : Air Surface"
                case "RF": //RF :  Reefer                
                    return null;
                //imgUri += "Inventory_Default";
                //break;
                //case "HZ":
                //    {
                //        imgUri += "Inventory_Hazard";
                //    }
                //    break;
                //case "RF": //RF :  Reefer
                //    {
                //        if (_inventory.cntr.reefer.plugCd.Equals("PIM") ||
                //           _inventory.cntr.reefer.plugCd.Equals("POW") ||
                //           _inventory.cntr.reefer.plugCd.Equals("ROW"))
                //        {
                //            imgUri += "Inventory_Reefer_PlugIn"; // Red
                //        }
                //        else
                //        {
                //            imgUri += "Inventory_Reefer_PlugOut"; // Blue
                //        }
                //    }
                //    break;
                //case "DG": // DG :Damage
                //    imgUri += "Inventory_Damaged";
                //    break;
                //case "HD": // HD :Hold
                //    imgUri += "Inventory_Hold";
                //    break;
                case "ET": // ET :Extended
                    imgUri += "Inventory_Extend40ft";
                    break;
                case "NA": // NW :No Work Area
                    imgUri += "Inventory_NoWorkArea";
                    break;
                case "NT": // NT :No Work Tier
                    imgUri += "Inventory_NoWorkTier";
                    break;
                case "TN": // TN : Tunnel
                    imgUri += "Inventory_Tunnel";
                    break;
                case "NA&TN":
                    imgUri += "Inventory_NoWorkAreaNTunnel";
                    break;
                case "TN&NT":
                    imgUri += "Inventory_NoWorkTunnelNTier";
                    break;
                case "NA&NT":
                    imgUri += "Inventory_NoWorkAreaNTier";
                    break;
                case "NA&TN&NT":
                    imgUri += "Inventory_NoWorkAreaNTunnelNTier";
                    break;
                default:
                    return null;
                    //imgUri += "Inventory_Default";
                    //break;
            }

            imgUri += ".png";
            return PresentationMgr.GetImageSource(imgUri);
        }
        public void SetInventoryInfo()
        {
            SetInventoryInfo(this.Inventory);
        }
    }
}