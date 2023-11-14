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
using HessianComm;
using System.Collections;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using VMT_Data_JAT2.Objects;
//using ExternalAPI;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for JobView.xaml
    /// </summary>
    public partial class HessianView : UserControl
    {
        private List<Button> _btnList = new List<Button>();

        public HessianView()
        {
            this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            AddButtonList();
            InitHessianCallbackFunctions();

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(HessianView_IsVisibleChanged);
        }

        void HessianView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                TextBox_UserID.Text = VMT_Data_JAT2.Objects.RMG.RMG_User.gUserID;
                TextBox_UserNm.Text = VMT_Data_JAT2.Objects.RMG.RMG_User.gUserNm;
                TextBox_UserPW.Text = VMT_Data_JAT2.Objects.RMG.RMG_User.gUserPW;
                TextBox_MchnTp.Text = VMT_Data_JAT2.Objects.RMG.RMG_User.gMchnTp;
                TextBox_MchnID.Text = VMT_Data_JAT2.Objects.RMG.RMG_User.gMchnID;
            }
        }

        private void AddButtonList()
        {
            _btnList.Add(Btn_keepAlive);
            _btnList.Add(Btn_getInventoryList);
            _btnList.Add(Btn_getInventory);
            _btnList.Add(Btn_getBlockMapList);
            _btnList.Add(Btn_setMachineStatusChanged);
            _btnList.Add(Btn_setMachineStop);
            _btnList.Add(Btn_setMachinePassed);
            _btnList.Add(Btn_setMachineArrivalInfo);
            _btnList.Add(Btn_setMachineReady);
            _btnList.Add(Btn_getMachineStop);
            _btnList.Add(Btn_getMachineNotice);
            _btnList.Add(Btn_setMachineNotice);
            _btnList.Add(Btn_getPrecedingYtList);
            _btnList.Add(Btn_setJobDone);
            _btnList.Add(Btn_getJobOrderList);
            _btnList.Add(Btn_getUserAccessRole);
            _btnList.Add(Btn_setLogin4Machine);
            _btnList.Add(Btn_setLogout4Machine);
            _btnList.Add(Btn_getMachineStopCodeList);
            
            _btnList.Add(Btn_getJobOrderByContainer);
            _btnList.Add(Btn_getMachineListOfPool);
            _btnList.Add(Btn_doSwap4Manual);
            
            _btnList.Add(Btn_getBlockList);
            _btnList.Add(Btn_setJobStatus);
            _btnList.Add(Btn_setDetwinJob);
            _btnList.Add(Btn_getMachineList);
        }

        private void InitHessianCallbackFunctions()
        {
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyKeepAlive_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyKeepAlive_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyKeepAlive_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetInventoryList_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetInventoryList_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetInventoryList_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetInventoryList_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetInventory_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetInventory_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetInventory_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetInventory_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetBlockMapList_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetBlockMapList_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetBlockMapList_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetBlockMapList_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineStatusChanged_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetMachineStatusChanged_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachineStatusChanged_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineStatusChanged_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineStop_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetMachineStop_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachineStop_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineStop_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachinePassed_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetMachinePassed_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachinePassed_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachinePassed_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineArrivalInfo_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetMachineArrivalInfo_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachineArrivalInfo_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineArrivalInfo_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineReady_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetMachineReady_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachineReady_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineReady_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetMachineStop_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetMachineStop_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineNotice_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetMachineNotice_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetMachineNotice_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineNotice_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineNotice_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetMachineNotice_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetMachineNotice_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetMachineNotice_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetPrecedingYtList_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetPrecedingYtList_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetPrecedingYtList_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetPrecedingYtList_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetJobDone_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetJobDone_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetJobDone_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetJobDone_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderList_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetJobOrderList_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetJobOrderList_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderList_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetUserAccessRole_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetUserAccessRole_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetUserAccessRole_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetUserAccessRole_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetLogin4Machine_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetLogin4Machine_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetLogin4Machine_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetLogin4Machine_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetLogout4Machine_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetLogout4Machine_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetLogout4Machine_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetLogout4Machine_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineStopCodeList_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetMachineStopCodeList_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetMachineStopCodeList_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineStopCodeList_Test);

            // 2015-12-28 추가
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderByContainer_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetJobOrderByContainer_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetJobOrderByContainer_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderByContainer_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineListOfPool_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetMachineListOfPool_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetMachineListOfPool_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetMachineListOfPool_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyDoSwap4Manual_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyDoSwap4Manual_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyDoSwap4Manual_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyDoSwap4Manual_Test);
            
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetBlockList_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetBlockList_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetBlockList_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifyGetBlockList_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetJobStatus_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetJobStatus_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetJobStatus_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetJobStatus_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetDetwinJob_Test = new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifySetDetwinJob_Test);
            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifySetDetwinJob_Test(VMT_Data_JAT2.VMT_DataMgr_Common_Callback.static_NotifySetDetwinJob_Test);

            VMT_Data_JAT2.VMT_DataMgr_Common_Callback.SetCallBack_NotifyGetMahcineList_Test(new VMT_Data_JAT2.VMT_DataMgr_Common_Callback.Callback_NotifySingleTest(NotifyGetMachineList_Test));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();
        }

        private void InitSkinImage()
        {
            foreach (Button btn in _btnList)
            {
                PresentationMgr.SetSkinButton(btn,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/MenuBar/Menubar_btn_available_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/MenuBar/Menubar_btn_available_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/MainView/MenuBar/Menubar_btn_available_disable.png", UriKind.Relative))
                );

                btn.FontSize = 15;
            }

            //Btn_keepAlive.Content = "IfSystemControl.\nkeepAlive";
            //Btn_getInventoryList.Content = "IfContainerControl.\ngetInventoryList";
            //Btn_getInventory.Content = "IfContainerControl.\ngetInventory";
            //Btn_getBlockMapList.Content = "IfDefineControl.\ngetBlockMapList";
            //Btn_setMachineStatusChanged.Content = "IfMachineControl.\nsetMachineStatusChanged";
            //Btn_setMachineStop.Content = "IfMachineControl.\nsetMachineStop";
            //Btn_setMachinePassed.Content = "IfMachineControl.\nsetMachinePassed";
            //Btn_setMachineArrivalInfo.Content = "IfMachineControl.\nsetMachineArrivalInfo";
            //Btn_setMachineReady.Content = "IfMachineControl.\nsetMachineReady";
            //Btn_getMachineStop.Content = "IfMachineControl.\ngetMachineStop";
            //Btn_getMachineNotice.Content = "IfMachineControl.\ngetMachineNotice";
            //Btn_setMachineNotice.Content = "IfMachineControl.\nsetMachineNotice";
            //Btn_getPrecedingYtList.Content = "IfMachineControl.\ngetPrecedingYtList";
            //Btn_setJobDone.Content = "IfJobControl.\nsetJobDone";
            //Btn_getJobOrderList.Content = "IfJobControl.\ngetJobOrderList";
            //Btn_getUserAccessRole.Content = "IfUserControl.\ngetUserAccessRole";
            //Btn_setLogin4Machine.Content = "IfUserControl.\nsetLogin4Machine";
            //Btn_setLogout4Machine.Content = "IfUserControl.\nsetLogout4Machine";
            //Btn_getMachineStopCodeList.Content = "IfDefineControl.\ngetMachineStopCodeList";

            //Btn_getJobOrderByContainer.Content = "IfJobControl.\ngetJobOrderByContainer";
            //Btn_getMachineListOfPool.Content = "IfMachineControl.\ngetMachineListOfPool";
            //Btn_doSwap4Manual.Content = "IfMachineControl.\ndoSwap4Manual";


            Btn_keepAlive.Content = "keepAlive";
            Btn_getInventoryList.Content = "getInventoryList";
            Btn_getInventory.Content = "getInventory";
            Btn_getBlockMapList.Content = "getBlockMapList";
            Btn_setMachineStatusChanged.Content = "setMachineStatusChanged";
            Btn_setMachineStop.Content = "setMachineStop";
            Btn_setMachinePassed.Content = "setMachinePassed";
            Btn_setMachineArrivalInfo.Content = "setMachineArrivalInfo";
            Btn_setMachineReady.Content = "setMachineReady";
            Btn_getMachineStop.Content = "getMachineStop";
            Btn_getMachineNotice.Content = "getMachineNotice";
            Btn_setMachineNotice.Content = "setMachineNotice";
            Btn_getPrecedingYtList.Content = "getPrecedingYtList";
            Btn_setJobDone.Content = "setJobDone";            
            Btn_getJobOrderList.Content = "getJobOrderList";
            Btn_getUserAccessRole.Content = "getUserAccessRole";
            Btn_setLogin4Machine.Content = "setLogin4Machine";
            Btn_setLogout4Machine.Content = "setLogout4Machine";
            Btn_getMachineStopCodeList.Content = "getMachineStopCodeList";

            Btn_getJobOrderByContainer.Content = "getJobOrderByContainer";
            Btn_getMachineListOfPool.Content = "getMachineListOfPool";
            Btn_doSwap4Manual.Content = "doSwap4Manual";
            Btn_getBlockList.Content = "getBlockList";
            Btn_setJobStatus.Content = "setJobStatus";
            Btn_setDetwinJob.Content = "setDetwinJob";
            Btn_getMachineList.Content = "getMachineList";
            
            Btn_setMachinePassed.IsEnabled = false;
            Btn_setMachineArrivalInfo.IsEnabled = false;
            Btn_setMachineReady.IsEnabled = false;
            Btn_getPrecedingYtList.IsEnabled = false;
            //Btn_setJobDone.IsEnabled = false;
            //Btn_setJobStatus.IsEnabled = false;
            //Btn_setDetwinJob.IsEnabled = false;
        }

        private void SetInfo()
        {
            VMT_Data_JAT2.VMT_DataMgr_Common.strUserID = TextBox_UserID.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strUserNm = TextBox_UserNm.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strUserPW = TextBox_UserPW.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strMchnTp = TextBox_MchnTp.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strMchnID = TextBox_MchnID.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strPartnerMchnID = TextBox_PartnerMchnID.Text;

            VMT_Data_JAT2.VMT_DataMgr_Common.strBlock = TextBox_Block.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strBay = TextBox_Bay.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strReasonCd = TextBox_ReasonCd.Text;
            VMT_Data_JAT2.VMT_DataMgr_Common.strReasonNm = TextBox_ReasonNm.Text;
        }

        private void Btn_keepAlive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfSystemControl.keepAlive
            VMT_Data_JAT2.VMT_DataMgr_Common.KeepAlive_Test();
        }

        private void Btn_getInventoryList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfContainerControl.getInventoryList
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            value.blck = TextBox_Block.Text;
            value.bay = TextBox_Bay.Text;

            VMT_Data_JAT2.VMT_DataMgr_Common.GetInventoryList_Test(ref value);
        }

        private void Btn_getInventory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfContainerControl.getInventory
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container value = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container();
            value.cntrNo = "BBBB9000002";

            VMT_Data_JAT2.VMT_DataMgr_Common.GetInventory_Test(ref value);
        }

        private void Btn_getBlockMapList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfDefineControl.getBlockMapList
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockMapList_Test();
        }

        private void Btn_setMachineStatusChanged_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.setMachineStatusChanged
            VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineStatusChanged_Test(true);
        }

        private Boolean bBreakDown = false;
        private long _avaliableStartTime = 0;
        private void Btn_setMachineStop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.setMachineStop
            if (bBreakDown == false)
            {
                bBreakDown = true;

                VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send();
                VMT_Data_JAT2.Objects.Common.VD_Common_Available availableSelectData = new VMT_Data_JAT2.Objects.Common.VD_Common_Available();
                availableSelectData.ReasonCd = TextBox_ReasonCd.Text;
                availableSelectData.ReasonNm = TextBox_ReasonNm.Text;
                value.Data = availableSelectData;

                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                if (!String.IsNullOrEmpty(ITV.ITV_User.gServerTime))
                {
                    String currentServerTime = ITV.ITV_User.gServerTime;
                    DateTime currentServerDateTime = Convert.ToDateTime(currentServerTime);
                    ts = (currentServerDateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0));
                }

                value.m_StartTime = Convert.ToInt64(ts.TotalSeconds);
                value.m_FinishTime = 0;
                _avaliableStartTime = value.m_StartTime;

                VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineStop_Test(ref value);
            }
            else
            {
                bBreakDown = false;

                VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send();
                VMT_Data_JAT2.Objects.Common.VD_Common_Available availableSelectData = new VMT_Data_JAT2.Objects.Common.VD_Common_Available();
                availableSelectData.ReasonCd = TextBox_ReasonCd.Text;
                availableSelectData.ReasonNm = TextBox_ReasonNm.Text;
                value.Data = availableSelectData;

                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.ITV.ITV_User.gServerTime))
                {
                    String currentServerTime = VMT_Data_JAT2.Objects.ITV.ITV_User.gServerTime;
                    DateTime currentServerDateTime = Convert.ToDateTime(currentServerTime);
                    ts = (currentServerDateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0));
                }

                value.m_StartTime = _avaliableStartTime;
                value.m_FinishTime = Convert.ToInt64(ts.TotalSeconds);
                _avaliableStartTime = 0;

                VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineStop_Test(ref value);
            }
        }

        private void Btn_setMachinePassed_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.setMachinePassed
            ITV.VD_ITV_SetManuaArrival_Send value = new ITV.VD_ITV_SetManuaArrival_Send();
            //value.WorkingMchnID = currentJob.workingMchn.mchnId;
            //value.PartnerMchnID = currentJob.partnerMchn.mchnId;
            value.WorkingMchnID = TextBox_MchnID.Text;
            value.PartnerMchnID = TextBox_PartnerMchnID.Text;
            
            VMT_Data_JAT2.VMT_DataMgr_Common.SetMachinePassed_Test(ref value);
        }

        private void Btn_setMachineArrivalInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.setMachineArrivalInfo
            ITV.VD_ITV_SetManuaArrival_Send value = new ITV.VD_ITV_SetManuaArrival_Send();
            //value.WorkingMchnID = currentJob.workingMchn.mchnId;
            //value.PartnerMchnID = currentJob.partnerMchn.mchnId;
            value.WorkingMchnID = TextBox_MchnID.Text;
            value.PartnerMchnID = TextBox_PartnerMchnID.Text;

            VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineArrivalInfo_Test(ref value);
        }

        private void Btn_setMachineReady_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.setMachineReady
            ITV.VD_ITV_SetManualReady_Send value = new ITV.VD_ITV_SetManualReady_Send();
            value.WorkingMchnID = TextBox_MchnID.Text;
            value.PartnerMchnID = TextBox_PartnerMchnID.Text;

            VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineReady_Test(ref value);
        }

        private void Btn_getMachineStop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.getMachineStop
            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStop_Test();
        }

        private void Btn_getMachineNotice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.getMachineNotice
            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineNotice_Test();
        }

        private void Btn_setMachineNotice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.setMachineNotice
            VMT_Data_JAT2.VMT_DataMgr_Common.SetMachineNotice_Test();
        }

        private void Btn_getPrecedingYtList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfMachineControl.getPrecedingYtList
            VMT_Data_JAT2.VMT_DataMgr_Common.GetPrecedingYtList_Test();
        }

        private Boolean bTwistLock = true;
        private void Btn_setJobDone_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfJobControl.setJobDone
            VMT_Data_JAT2.VMT_DataMgr_Common.SetJobDone_Test(bTwistLock);
            bTwistLock = !bTwistLock;
        }        

        private void Btn_getJobOrderList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfJobControl.getJobOrderList
            VMT_Data_JAT2.VMT_DataMgr_Common.GetJobOrderList_Test();
        }

        private void Btn_getUserAccessRole_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfUserControl.getUserAccessRole
            VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send();
            value.UserID = TextBox_UserID.Text;

            VMT_Data_JAT2.VMT_DataMgr_Common.GetUserAccessRole_Test(value);
        }

        private void Btn_setLogin4Machine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfUserControl.setLogin4Machine
            VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value = new VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send();
            value.UserID = TextBox_UserID.Text;
            value.UserPW = TextBox_UserPW.Text;
            value.GroupName = "SUPERMANAGER";

            VMT_Data_JAT2.VMT_DataMgr_Common.SetLogin4Machine_Test(value);
        }

        private void Btn_setLogout4Machine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfUserControl.setLogout4Machine
            VMT_Data_JAT2.VMT_DataMgr_Common.SetLogout4Machine_Test();
        }

        private void Btn_getMachineStopCodeList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfDefineControl.getMachineStopCodeList
            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineStopCodeList_Test();
        }

        // 2015-12-28 추가
        private void Btn_getJobOrderByContainer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfJobControl.getJobOrderByContainer
            String strSearch = "0021";
            VMT_Data_JAT2.VMT_DataMgr_Common.GetJobOrderByContainer_Test(strSearch);
        }

        private void Btn_getMachineListOfPool_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfJobControl.getJobOrderByContainer
            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineListOfPool_Test();
        }

        private void Btn_doSwap4Manual_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            //IfJobControl.getJobOrderByContainer
            String newYtNo = "TT001";
            VMT_Data_JAT2.VMT_DataMgr_Common.DoSwap4Manual_Test(newYtNo);
        }
                
        private void Btn_getBlockList_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            VMT_Data_JAT2.VMT_DataMgr_Common.GetBlockList_Test();            
        }

        private Boolean bActive = true;
        private void Btn_setJobStatus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            VMT_Data_JAT2.VMT_DataMgr_Common.SetJobStatus_Test(bActive);
            bActive = !bActive;
        }

        private void Btn_setDetwinJob_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();            
            VMT_Data_JAT2.VMT_DataMgr_Common.SetDetwinJob_Test();
        }

        private void Btn_getMachineList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetInfo();
            VMT_Data_JAT2.VMT_DataMgr_Common.GetMachineList_Test();
        }

        //- Common Notify Function
        public void NotifyHessian_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }
                        }));
        }

        public void NotifyKeepAlive_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            InterfaceMessageLoader.instance().WriteInterfaceMessage<String>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone as String);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_keepAlive.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_keepAlive.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_keepAlive.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_keepAlive.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetInventoryList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_InventoryInfo_Receive>(System.Reflection.MethodBase.GetCurrentMethod().Name, VMT_DataMgr_RMG_HessianCallback.GetInventoryListFromObject(clone));

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getInventoryList.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getInventoryList.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getInventoryList.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getInventoryList.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetInventory_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);                       

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getInventory.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getInventory.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getInventory.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getInventory.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetBlockMapList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getBlockMapList.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getBlockMapList.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                    {
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                        //this.WriteLog(Util.HashtableToXmlString(obj as Hashtable));
                                    }   
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getBlockMapList.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getBlockMapList.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetMachineStatusChanged_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setMachineStatusChanged.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setMachineStatusChanged.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setMachineStatusChanged.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setMachineStatusChanged.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetMachineStop_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setMachineStop.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setMachineStop.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setMachineStop.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setMachineStop.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetMachinePassed_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getBlockMapList.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getBlockMapList.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getBlockMapList.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getBlockMapList.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetMachineArrivalInfo_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setMachineArrivalInfo.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setMachineArrivalInfo.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setMachineArrivalInfo.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setMachineArrivalInfo.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetMachineReady_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setMachineReady.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setMachineReady.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setMachineReady.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setMachineReady.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetMachineStop_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getMachineStop.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getMachineStop.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getMachineStop.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getMachineStop.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetMachineNotice_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getMachineNotice.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getMachineNotice.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getMachineNotice.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getMachineNotice.Background = new SolidColorBrush(Colors.Green);
                        }));
        }
        public void NotifySetMachineNotice_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setMachineNotice.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setMachineNotice.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setMachineNotice.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setMachineNotice.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetPrecedingYtList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getPrecedingYtList.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getPrecedingYtList.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getPrecedingYtList.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getPrecedingYtList.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetJobDone_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setJobDone.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setJobDone.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setJobDone.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setJobDone.Background = new SolidColorBrush(Colors.Green);
                        }));
        }


        public void NotifyGetJobOrderList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            InterfaceMessageLoader.instance().WriteInterfaceMessage<RMG.VD_RMG_JobOrderList>(System.Reflection.MethodBase.GetCurrentMethod().Name, VMT_DataMgr_RMG_HessianCallback.GetJobOrderListFromObject(clone));

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getJobOrderList.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getJobOrderList.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }                       
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getJobOrderList.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getJobOrderList.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        
        

        public void NotifyGetUserAccessRole_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getUserAccessRole.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getUserAccessRole.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getUserAccessRole.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getUserAccessRole.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifySetLogin4Machine_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            //InterfaceMessageLoader.instance().WriteInterfaceMessage<Hashtable>(System.Reflection.MethodBase.GetCurrentMethod().Name, clone as Hashtable);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setLogin4Machine.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setLogin4Machine.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setLogin4Machine.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setLogin4Machine.Background = new SolidColorBrush(Colors.Green);
                        }));
        }
        public void NotifySetLogout4Machine_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setLogout4Machine.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_setLogout4Machine.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setLogout4Machine.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_setLogout4Machine.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetMachineStopCodeList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getMachineStopCodeList.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getMachineStopCodeList.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getMachineStopCodeList.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getMachineStopCodeList.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        // 2015-12-28 추가
        public void NotifyGetJobOrderByContainer_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getJobOrderByContainer.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getJobOrderByContainer.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            WriteObjectToLog(clone);

                            Btn_getJobOrderByContainer.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getJobOrderByContainer.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetMachineListOfPool_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getMachineListOfPool.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_getMachineListOfPool.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            WriteObjectToLog(clone);

                            Btn_getMachineListOfPool.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_getMachineListOfPool.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyDoSwap4Manual_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_doSwap4Manual.Foreground = new SolidColorBrush(Colors.Red);
                                //Btn_doSwap4Manual.Background = new SolidColorBrush(Colors.Red);
                                return;
                            }

                            WriteObjectToLog(clone);

                            Btn_doSwap4Manual.Foreground = new SolidColorBrush(Colors.Green);
                            //Btn_doSwap4Manual.Background = new SolidColorBrush(Colors.Green);
                        }));
        }

        public void NotifyGetBlockList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getBlockList.Foreground = new SolidColorBrush(Colors.Red);                                
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_getBlockList.Foreground = new SolidColorBrush(Colors.Green);                            
                        }));
        }

        public void NotifySetJobStatus_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_setJobStatus.Foreground = new SolidColorBrush(Colors.Red);                                
                                return;
                            }

                            if (clone is ArrayList)
                            {
                                ArrayList arrayList = clone as ArrayList;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Object obj = arrayList[i];
                                    if (obj is Hashtable)
                                        this.HashtableToWriteLog(i, obj as Hashtable);
                                }
                            }
                            else if (clone is Hashtable)
                            {
                                this.HashtableToWriteLog(-1, clone as Hashtable);
                            }
                            else if (clone is String)
                            {
                                this.WriteLog(clone as String);
                            }

                            Btn_setJobStatus.Foreground = new SolidColorBrush(Colors.Green);                            
                        }));
        }

        public void NotifySetDetwinJob_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);                                
                                Btn_setDetwinJob.Foreground = new SolidColorBrush(Colors.Red);                                
                                return;
                            }

                            WriteObjectToLog(clone);
                            Btn_setDetwinJob.Foreground = new SolidColorBrush(Colors.Green);                            
                        }));
        }

        public void NotifyGetMachineList_Test(ref Object value)
        {
            String strMessage = String.Empty;
            Boolean isException = false;
            if (value is HessianException)
            {
                isException = true;
                strMessage = (value as HessianException).Message;
            }

            Object clone = null;
            if (!isException)
                clone = Util.DeepCopy<Object>(value);

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            if (isException)
                            {
                                this.WriteLog(strMessage);
                                Btn_getMachineList.Foreground = new SolidColorBrush(Colors.Red);                                
                                return;
                            }

                            WriteObjectToLog(clone);
                            Btn_getMachineList.Foreground = new SolidColorBrush(Colors.Green);                            
                        }));
        }

        private void WriteObjectToLog(Object clone)
        {
            if (clone is ArrayList)
            {
                ArrayList arrayList = clone as ArrayList;
                for (int i = 0; i < arrayList.Count; i++)
                {
                    Object obj = arrayList[i];
                    if (obj is Hashtable)
                        this.HashtableToWriteLog(i, obj as Hashtable);
                }
            }
            //else if (clone is List<Object>)
            //{
            //    List<Object> objectList = clone as List<Object>;
            //    for (int i = 0; i < objectList.Count; i++)
            //    {
            //        Object obj = objectList[i];
            //        if (obj is Hashtable)
            //            this.HashtableToWriteLog(i, obj as Hashtable);
            //    }
            //}
            else if (clone is Hashtable)
            {
                this.HashtableToWriteLog(-1, clone as Hashtable);
            }
            else if (clone is String || clone == null)
            {
                this.WriteLog(clone as String);
            }
        }

        #region [Help Function]
        private String strHashtableFormat = "{0} : {1}\r\n";
        private void HashtableToWriteLog(Int32 nIndex, Hashtable hashtable)
        {
            String strResult = String.Empty;
            if (nIndex != -1)
                strResult += "index : " + nIndex.ToString() + "\r\n";

            foreach (DictionaryEntry entry in hashtable)
            {
                if (entry.Value == null)
                    strResult += String.Format(strHashtableFormat, entry.Key, "null");
                else
                    strResult += String.Format(strHashtableFormat, entry.Key, entry.Value);
            }
            this.WriteLog(strResult);
        }

        private void WriteLog(String strValue)
        {
            MainWindow.LogWin.WriteLog(strValue);
        }
        #endregion [Help Function]
    }
}