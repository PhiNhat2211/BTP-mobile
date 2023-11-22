using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HessianComm.Interface;
using HessianComm.Objects;
using HessianCSharp.client;
using HessianCSharp.Class;

namespace HessianComm
{
    public enum HessianCommType
    {
        HessianCommType_Common_Start = 0x0000,
        KeepAlive,
        KeepAliveStandAlone,
        GetUserAccessRole,
        ChangeDriver,
        // GetLoginInfo4Machine,
        SetLogin4Machine,
        SetVMTMachineStatus,
        ChangeDriverCheck,
        GetChssUsingData,
        SetMachineStatusChanged,
        GetJobOrderList,
        GetJobOrderList_New,
        SetLogout4Machine,
        GetLogin4MachineList,
        getSwapList,
        getSwapListRTG,
        setEmptySwap,
        // Available
        GetMachineStopCodeList,
        GetMachineAccessAction,
        GetBlockListForYardSector,
        GetMachineStop,
        SetMachineStop,
        ReleaseYtFromJob,

        GetMachineNotice,
        SetMachineNotice,
        DGPSAlive, //-- 미구현 // gps sensor, always true // 5 Sec
        HessianCommType_Common_End = 0x0fff,

        HessianCommType_ITV_Start = 0x1000,
        SetMachinePassed,       // under RTG // Not Use, Use SetManualActivation
        SetManualActivation,    // under RTG
        SetMachineArrival,  // under STS
        SetMachineReady,
        SetItvDone,
        SetQCJobReleaseByYt, // Set Job Done For QC
        ChangeChassisNo,
        GetPrecedingYtList,     // Biz Logic
        SetPinningStation,      // Biz Logic
        SetConfirmJobByScanner, // Biz Logic
        HessianCommType_ITV_End = 0x1ffff,
        /////////////////////////

        /////////////////////////
        // - RMG
        HessianCommType_RMG_Start = 0x2000,
        GetInventoryList,
        GetInventoryListEx,
        GetInventoryListMulti,
        GetInventoryListMulti_New,
        GetInventoryList4Multi_Sync,
        GetInventoryListMulti_New1,
        GetInventoryListMulti_New2,
        GetInventoryListMultiSwap_New,
        GetInventoryListBackground,
        GetInventoryListBackgroundEx,
        GetInventoryListBackgroundMulti,
        GetInventoryListBackgroundMulti_New,
        GetInventoryListData,
        GetInventoryListDataEx,        
        GetInventoryListDataMulti,
        GetInventoryListDataMulti_New,
        GetInventoryListDataMulti_New1,
        GetInventoryListDataMulti_New2,
        GetInventoryListDataMultiSwap_New,
        GetInventory,
        GetBlockMapListForYt,
        GetBlockMapList,
        GetBlockMapList1,
        GetBlockMapList2,
        GetBlockMapSwapList,
        GetBlockList,    // Yard의 전체 block list 전달
        GetBlockListForBlockMap,
        GetVmtAutoStartConfig,
        GetEmptySwappingTargetList,
        DoEmptySwap,
        SetJobDone,
        SetJobDoneForLocationChange,
        SetJobStatus,       // Siemens 호출 API와 분리 Status : JOB_PROCESSING, JOB_INACTIVE
        SetPickedContainer, // PLC data로부터 lock 발생시 SetJobStatus를 대체함
        SetDetwinJob,       // detwin job
        GetMachineList,
        GetChangedMachineLocation,
        GetMachineList4LogoutCheck,
        GetMachineListOfPool,
        GetJobOrderByContainer,
        GetJobOrderByContainer_New,
        GetContainerInfo,
        GetTwinContainerInfo,
        DoSwap4Manual,
        GetMaxRow,
        GetNoWorkArea,
        GetNoWorkArea1,
        GetNoWorkArea2,
        GetNoWorkTier,
        GetNoWorkTier1,
        GetNoWorkTier2,
        SetChangePosition,
        GetJobOrderListByKeys,
        GetJobOrderListByKeys_New,
        GetJobOrderListByTruck,
        GetJobOrderListByTruck_New,
        GetMachineJobByKeys,
        GetMachineJobByKeys_New,
        GetMachineJobByKeys_Sync,
        GetMachineStatusChanged,
        GetMachineJobByTruck,
        GetMachineJobByTruck_New,
        GetIsValidLocation,
        ExceptionOccured,
        SetJobReOnChassis,
        GetDriverJobHistory, // Get Driver Job History
        SetParkLocation,
        GetParkLocation,
        CheckYcDeTwin,
        SetBestPick,
        Validate4LoadingSwapping,
        SetReallocation,
        ChassisOrderComplete,
        ValidChassisInfos,
        ItvLinkChassis,
        ItvUnLinkChassis,
        SetPLCAutoFlg, // PLC Auto
        SetPLCMsg,
        CheckPLCData,
        CheckPLCTwistLock,
        CancelPLC,
        ProcessPLC,
        ReleasePLCLock,
        InitPLCMessage,
        SetGateCancelJob,
        GetConfigValue,
        HessianCommType_RMG_End = 0x2ffff,
        /////////////////////////

        /////////////////////////
        // - Single Test for JAT2
        HessianCommType_Test_Start = 0x8000,
        KeepAlive_Test,
        GetInventoryList_Test,
        GetInventory_Test,
        GetBlockMapList_Test,
        GetBlockList_Test,    // Yard의 전체 block list 전달
        SetMachineStatusChanged_Test,
        SetMachineStop_Test,
        SetMachinePassed_Test, // Not Use, Use SetManualActivation 
        SetManualActivation_Test,
        SetMachineArrivalInfo_Test,
        SetMachineReady_Test,
        GetMachineStop_Test,
        GetMachineNotice_Test,
        SetMachineNotice_Test,
        GetPrecedingYtList_Test,
        SetJobDone_Test, // Status : Twist Lock(Pick-up), Twist Unlock(put-down)
        SetJobStatus_Test, // Status : JOB_PROCESSING, JOB_INACTIVE
        GetJobOrderList_Test,        
        GetUserAccessRole_Test,
        SetLogin4Machine_Test,
        SetLogout4Machine_Test,
        GetMachineStopCodeList_Test,        
        GetJobOrderByContainer_Test,
        GetMachineListOfPool_Test,
        DoSwap4Manual_Test,
        SetDetwinJob_Test,
        GetMachineList_Test,
        HessianCommType_Test_End = 0x8ffff,
        /////////////////////////
    }

    public partial class HessianException : Exception
    {
        public HessianException()
        {
        }

        public HessianException(string message)
            : base(message)
        {
        }

        public HessianException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public HessianCommType hessianCommType { get; set; }
    }

    public class HessianAPI
    {   
        #region variables
        private const string _hessian_System = @"/HESSIAN/HESSIAN_IfSystemControl";
        private const string _hessian_User = @"/HESSIAN/HESSIAN_IfVmtUserControl";
        private const string _hessian_Machine = @"/HESSIAN/HESSIAN_IfMachineControl";
        private const string _hessian_Job = @"/HESSIAN/HESSIAN_IfJobControl";
        private const string _hessian_VmtDefine = @"/HESSIAN/HESSIAN_IfVmtDefineControl";
        private const string _hessian_Container = @"/HESSIAN/HESSIAN_IfContainerControl";

        private const string _hessian_VmtWorkOrder = @"/HESSIAN/HESSIAN_IfVmtWorkOrderControl";
        private const string _hessian_IfVmtPLC = @"/HESSIAN/HESSIAN_IfVmtPLCControl";
        private const string _hessian_VmtPLC = @"/HESSIAN/HESSIAN_VmtPLCControl";
        private const string _hessian_VmtContainer = @"/HESSIAN/HESSIAN_IfVmtContainerControl";
        private const string _hessian_VmtMachine = @"/HESSIAN/HESSIAN_IfVmtMachineControl";
        private const string _hessian_VmtEmptySwap = @"/HESSIAN/HESSIAN_IfVmtEmptySwapControl";
        private const string _hessian_VmtSwap = @"/HESSIAN/HESSIAN_IfVmtSwapControl";
        private const string _hessian_VmtSwapService = @"/HESSIAN/HESSIAN_IfVmtSwapService";

        private static HessianAPI _instance = new HessianAPI();

        private string _ip = @"116.127.223.206";
        private int _port = 7110;        

        private IfSystemControl _systemControl = null;
        private IfVmtUserControl _userControl = null;
        private IfVmtMachineControl _vmtMachineControl = null;
        private IfJobControl _jobControl = null;
        private IfVmtDefineControl _vmtDefineControl = null;
        private IfContainerControl _containerControl = null;

        private IfVmtWorkOrderControl _vmtWorkOrderControl = null;
        private IfVmtPLCControl _ifVmtPLCControl = null;
        private VmtPLCControl _vmtPLCControl = null;
        private IfVmtContainerControl _vmtContainerControl = null;

        private VmtSwapService _vmtSwapService = null;
        private IfVmtEmptySwapControl _vmtEmptySwapControl = null;
        private IfVmtSwapControl _vmtSwapControl = null;

        private CHessianProxyFactory factory = new CHessianProxyFactory();

        private HessianExecuteThread _thread = null;

        private HessianExecuteThreadPriority _threadPriority = null;

        #endregion

        // Exception Throw object for External Module
        public delegate void ExceptionDelegate(Exception ex);

        public delegate void HessianCommCallback(HessianCommType type, object obj, bool isSend = false);

        public static List<ExceptionDelegate> ExceptionDelegatorList = new List<ExceptionDelegate>();

        public static void AddExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Add(ed);
        }

        public static void RemoveExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Remove(ed);
        }

        #region properties
        public IfSystemControl SystemControl
        {
            get
            {
                if (this._systemControl == null)
                {
                    this._systemControl = (IfSystemControl)this.factory.Create<IfSystemControl>(typeof(IfSystemControl), "http://" + this._ip + ":" + this._port + _hessian_System);
                }

                return this._systemControl;
            }
        }

        public IfVmtUserControl UserControl
        {
            get
            {
                if (this._userControl == null)
                {
                    this._userControl = (IfVmtUserControl)this.factory.Create<IfVmtUserControl>(typeof(IfVmtUserControl), "http://" + this._ip + ":" + this._port + _hessian_User);
                }

                return this._userControl;
            }
        }

        public IfVmtMachineControl vmtMachineControl
        {
            get
            {
                if (this._vmtMachineControl == null)
                {
                    this._vmtMachineControl = (IfVmtMachineControl)this.factory.Create<IfVmtMachineControl>(typeof(IfVmtMachineControl), "http://" + this._ip + ":" + this._port + _hessian_VmtMachine);
                }

                return this._vmtMachineControl;
            }
        }


        public IfJobControl JobControl
        {
            get
            {
                if (this._jobControl == null)
                {
                    this._jobControl = (IfJobControl)this.factory.Create<IfJobControl>(typeof(IfJobControl), "http://" + this._ip + ":" + this._port + _hessian_Job);
                }

                return this._jobControl;
            }
        }

        public IfVmtDefineControl vmtDefineControl
        {
            get
            {
                if (this._vmtDefineControl == null)
                {
                    this._vmtDefineControl = (IfVmtDefineControl)this.factory.Create<IfVmtDefineControl>(typeof(IfVmtDefineControl), "http://" + this._ip + ":" + this._port + _hessian_VmtDefine);
                }

                return this._vmtDefineControl;
            }
        }

        public IfContainerControl ContainerControl
        {
            get
            {
                if (this._containerControl == null)
                {
                    this._containerControl = (IfContainerControl)this.factory.Create<IfContainerControl>(typeof(IfContainerControl), "http://" + this._ip + ":" + this._port + _hessian_Container);
                }

                return this._containerControl;
            }
        }

        public IfVmtWorkOrderControl VmtWorkOrderControl
        {
            get
            {
                if (this._vmtWorkOrderControl == null)
                {
                    this._vmtWorkOrderControl = (IfVmtWorkOrderControl)this.factory.Create<IfVmtWorkOrderControl>(typeof(IfVmtWorkOrderControl), "http://" + this._ip + ":" + this._port + _hessian_VmtWorkOrder);
                }

                return this._vmtWorkOrderControl;
            }
        }

        public IfVmtPLCControl IfVmtPLCControl
        {
            get
            {
                if (this._ifVmtPLCControl == null)
                {
                    this._ifVmtPLCControl = (IfVmtPLCControl)this.factory.Create<IfVmtPLCControl>(typeof(IfVmtPLCControl), "http://" + this._ip + ":" + this._port + _hessian_IfVmtPLC);
                }

                return this._ifVmtPLCControl;
            }
        }

        public VmtPLCControl VmtPLCControl
        {
            get
            {
                if (this._vmtPLCControl == null)
                {
                    this._vmtPLCControl = (VmtPLCControl)this.factory.Create<VmtPLCControl>(typeof(VmtPLCControl), "http://" + this._ip + ":" + this._port + _hessian_VmtPLC);
                }

                return this._vmtPLCControl;
            }
        }

        public IfVmtContainerControl VmtContainerControl
        {
            get
            {
                if (this._vmtContainerControl == null)
                {
                    this._vmtContainerControl = (IfVmtContainerControl)this.factory.Create<IfVmtContainerControl>(typeof(IfVmtContainerControl), "http://" + this._ip + ":" + this._port + _hessian_VmtContainer);
                }

                return this._vmtContainerControl;
            }
        }
        public IfVmtEmptySwapControl VmtEmptySwapControl
        {
            get
            {
                if (this._vmtEmptySwapControl == null)
                {
                    this._vmtEmptySwapControl = (IfVmtEmptySwapControl)this.factory.Create<IfVmtEmptySwapControl>(typeof(IfVmtEmptySwapControl), "http://" + this._ip + ":" + this._port + _hessian_VmtEmptySwap);
                }

                return this._vmtEmptySwapControl;
            }
        }
        public IfVmtSwapControl VmtSwapControl
        {
            get
            {
                if (this._vmtSwapControl == null)
                {
                    this._vmtSwapControl = (IfVmtSwapControl)this.factory.Create<IfVmtSwapControl>(typeof(IfVmtSwapControl), "http://" + this._ip + ":" + this._port + _hessian_VmtSwap);
                }

                return this._vmtSwapControl;
            }
        }

        public VmtSwapService VmtSwapService
        {
            get
            {
                if (this._vmtSwapService == null)
                {
                    this._vmtSwapService = (VmtSwapService)this.factory.Create<VmtSwapService>(typeof(VmtSwapService), "http://" + this._ip + ":" + this._port + _hessian_VmtSwapService);
                }

                return this._vmtSwapService;
            }
        }

        #endregion

        private HessianAPI()
        {
            this._thread = new HessianExecuteThread(this);
            this._threadPriority = new HessianExecuteThreadPriority(this);
        }

        public static void Init(string ip, int port, HessianCommCallback callback)
        {
            _instance._ip = ip;
            _instance._port = port;
            _instance._thread.Callback = callback;
            _instance._threadPriority.Callback = callback;
        }

        public static void Start()
        {
            _instance._thread.Start();
        }
        public static void StartPriority()
        {
            _instance._threadPriority.Start();
        }
        public static void End()
        {
            _instance._thread.End();
        }
        public static void EndPriority()
        {
            _instance._threadPriority.End();
        }
        public static void StartPolling(HessianCommType type, object obj, int timeSpan)
        {
            _instance._thread.StartPolling(type, obj, timeSpan);
        }
        public static void StartPollingPriority(HessianCommType type, object obj, int timeSpan)
        {
            _instance._threadPriority.StartPolling(type, obj, timeSpan);
        }
        public static void StopPolling(HessianCommType type)
        {
            _instance._thread.StopPolling(type);
        }
        public static void StopPollingPriority(HessianCommType type)
        {
            _instance._threadPriority.StopPolling(type);
        }
        public static void KeepAlive()
        {
            _instance._thread.Do(HessianCommType.KeepAlive, null);
        }
        public static void KeepAlivePriority()
        {
            _instance._threadPriority.Do(HessianCommType.KeepAlive, null);
        }
        // bool
        public static void SetLogin4Machine(Login login)
        {
            _instance._thread.Do(HessianCommType.SetLogin4Machine, login);
        }
        public static void ChangeDriverCheck(Login login)
        {
            _instance._thread.Do(HessianCommType.ChangeDriverCheck, login);
        }
        public static void GetChssUsingData(String chssNo)
        {
            _instance._thread.Do(HessianCommType.GetChssUsingData, chssNo);
        }

        // object
        public static void GetUserAccessRole(User user)
        {
            _instance._thread.Do(HessianCommType.GetUserAccessRole, user);
        }

        public static void ChangeDriver(HessianList list)
        {
            _instance._thread.Do(HessianCommType.ChangeDriver, list);
        }
        // object
        // public static void GetLoginInfo4Machine(Login login)
        // {
        //     _instance._thread.Do(HessianCommType.GetLoginInfo4Machine, login);
        // }

        // Object
        public static void SetMachineStatusChanged(Machine machine)
        {
            _instance._thread.Do(HessianCommType.SetMachineStatusChanged, machine);
        }

        // List<object>
        public static void GetJobOrderList(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderList, machine);
        }        

        public static void GetJobOrderList_New(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderList_New, list);
        }

        public static void GetJobOrderListByKeys(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderListByKeys, list);
        }

        public static void GetJobOrderListByKeys_New(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderListByKeys_New, list);
        }

        public static void GetJobOrderListByTruck(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderListByTruck, list);
        }

        public static void GetJobOrderListByTruck_New(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderListByTruck_New, list);
        }

        public static void GetMachineJobByKeys(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetMachineJobByKeys, list);
        }

        public static void GetMachineJobByKeys_New(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetMachineJobByKeys_New, list);
        }

        public static void GetMachineJobByKeys_Sync(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetMachineJobByKeys_Sync, list);
        }

        public static void GetMachineStatusChanged(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetMachineStatusChanged, list);
        }

        public static void GetMachineJobByTruck(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetMachineJobByTruck, list);
        }

        public static void GetMachineJobByTruck_New(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetMachineJobByTruck_New, list);
        }

        public static void getSwapList(HessianList list)
        {
            _instance._thread.Do(HessianCommType.getSwapList, list);
        }
        public static void getSwapListRTG(HessianList list)
        {
            _instance._thread.Do(HessianCommType.getSwapListRTG, list);
        }

        public static void setEmptySwap(HessianList list)
        {
            _instance._thread.Do(HessianCommType.setEmptySwap, list);
        }

        // bool
        public static void SetLogout4Machine(LogOut logout)
        {
            _instance._thread.Do(HessianCommType.SetLogout4Machine, logout);
        }

        // Object
        public static void SetMachinePassed(YtTask ytTask)
        {
            _instance._thread.Do(HessianCommType.SetMachinePassed, ytTask);
        }

        public static void SetManualActivation(Task task)
        {
            _instance._thread.Do(HessianCommType.SetManualActivation, task);
        }

        // Object
        public static void SetMachineArrival(HessianList list)
        {
            _instance._thread.Do(HessianCommType.SetMachineArrival, list);
        }

        // Object
        public static void SetMachineReady(HessianList list)
        {
            _instance._thread.Do(HessianCommType.SetMachineReady, list);
        }

        // Object
        public static void GetMachineStopCodeList(string machineType)
        {
            _instance._thread.Do(HessianCommType.GetMachineStopCodeList, machineType);
        }

        public static void GetMachineAccessAction(HessianList accessInfo)
        {
            _instance._thread.Do(HessianCommType.GetMachineAccessAction, accessInfo);
        }

        public static void GetBlockListForYardSector(HessianList blockInfo)
        {
            _instance._thread.Do(HessianCommType.GetBlockListForYardSector, blockInfo);
        }

        // Object
        public static void GetMachineStop(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineStop, machine);
        }

        // Object
        public static void SetMachineStop(MachineStop machineStop)
        {
            _instance._thread.Do(HessianCommType.SetMachineStop, machineStop);
        }
        
        // Object
        public static void ReleaseYtFromJob(HessianList list)
        {
            _instance._thread.Do(HessianCommType.ReleaseYtFromJob, list);
        }

        // Object
        public static void GetMachineNotice(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineNotice, machine);
        }

        // Object
        public static void SetMachineNotice(Machine machine)
        {
            _instance._thread.Do(HessianCommType.SetMachineNotice, machine);
        }

        // List<object>
        public static void GetPrecedingYtList(YtTask ytTask)
        {
            _instance._thread.Do(HessianCommType.GetPrecedingYtList, ytTask);
        }

        // Object
        public static void SetConfirmJobByScanner(HessianList confirmJob)
        {
            _instance._thread.Do(HessianCommType.SetConfirmJobByScanner, confirmJob);
        }

        public static void SetItvDone(HessianList list)
        {
            _instance._thread.Do(HessianCommType.SetItvDone, list);
        }

        //Set Job Done For QC
        public static void SetQCJobReleaseByYt(HessianList taskList)
        {
            _instance._thread.Do(HessianCommType.SetQCJobReleaseByYt, taskList);
        }

        public static void ChangeChassisNo(HessianList list)
        {
            _instance._thread.Do(HessianCommType.ChangeChassisNo, list);
        }
                
        public static void GetBlockMapList(string blck)
        {           
            _instance._thread.Do(HessianCommType.GetBlockMapList, blck);           
        }

        public static void GetBlockMapListForYt(string blck)
        {           
            _instance._thread.Do(HessianCommType.GetBlockMapListForYt, blck);           
        }
        public static void GetBlockMapSwapList(string blck)
        {
            _instance._thread.Do(HessianCommType.GetBlockMapSwapList, blck);           
        }

        public static void GetBlockMapListMoving1(string blck)
        {
            _instance._thread.Do(HessianCommType.GetBlockMapList1, blck);
        }

        public static void GetBlockMapListMoving2(string blck)
        {
            _instance._thread.Do(HessianCommType.GetBlockMapList2, blck);
        }

        public static void GetBlockList()
        {
            _instance._thread.Do(HessianCommType.GetBlockList, null);            
        }

        public static void GetBlockListForBlockMap()
        {
            _instance._thread.Do(HessianCommType.GetBlockListForBlockMap, null);            
        }
        public static void GetVmtAutoStartConfig()
        {
            _instance._thread.Do(HessianCommType.GetVmtAutoStartConfig, null);
        }
        public static void GetEmptySwappingTargetList(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.GetEmptySwappingTargetList, hessianList);
        }
        public static void DoEmptySwap(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.DoEmptySwap, hessianList);
        }
        public static void SetJobStatus(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetJobStatus, hessianList);
        }

        public static void SetPickedContainer(HessianList hashTasks)
        {
            _instance._thread.Do(HessianCommType.SetPickedContainer, hashTasks);
        }

        public static void SetPLCAutoFlg(HessianList hashTasks)
        {
            _instance._thread.Do(HessianCommType.SetPLCAutoFlg, hashTasks);
        }

        public static void SetDetwinJob(string jobKey)
        {
            _instance._thread.Do(HessianCommType.SetDetwinJob, jobKey);
        }

        public static void GetInventoryList(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetInventoryList, loc);
        }

        public static void GetInventoryListEx(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListEx, loc);
        }

        public static void GetInventoryListMulti(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListMulti, locationList);
        }

        public static void GetInventoryListMulti_New(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListMulti_New, locationList);
        }

        public static void GetInventoryList4Multi_Sync(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryList4Multi_Sync, locationList);
        }

        public static void GetInventoryListByVirtualBlock_New(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListMulti_New, locationList);
        }

        public static void GetInventoryListMultiSwap_New(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListMultiSwap_New, locationList);
        } 

        public static void GetInventoryListBackground(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListBackground, loc);
        }

        public static void GetInventoryListBackgroundEx(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListBackgroundEx, loc);
        }

        public static void GetInventoryListBackgroundMulti(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListBackgroundMulti, locationList);
        }

        public static void GetInventoryListBackgroundMulti_New(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListBackgroundMulti_New, locationList);
        }

        public static void GetInventoryListData(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListData, loc);
        }

        public static void GetInventoryListDataEx(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListDataEx, loc);
        }

        public static void GetInventoryListDataMulti(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListDataMulti, locationList);
        }

        public static void GetInventoryListMulti_New1(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListMulti_New1, locationList);
        }

        public static void GetInventoryListMulti_New2(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListMulti_New2, locationList);
        }

        public static void GetInventoryListDataMulti_New1(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListDataMulti_New1, locationList);
        }

        public static void GetInventoryListDataMulti_New2(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListDataMulti_New2, locationList);
        }

        public static void GetInventoryListDataMulti_New(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListDataMulti_New, locationList);
        }

        public static void GetInventoryListDataMultiSwap_New(ArrayList locationList)
        {
            _instance._thread.Do(HessianCommType.GetInventoryListDataMultiSwap_New, locationList);
        }

        public static void GetMachineList(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineList, machine);
        }

        public static void GetChangedMachineLocation(HessianList list)
        {
            _instance._thread.Do(HessianCommType.GetChangedMachineLocation, list);
        }

        // public static void GetMachineList4LogoutCheck(Machine machine)
        // {
        //     _instance._thread.Do(HessianCommType.GetMachineList4LogoutCheck, machine);
        // }        

        public static void GetMachineListOfPool(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineListOfPool, machine);
        }

        public static void GetJobOrderByContainer(string searchString)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderByContainer, searchString);
        }

        public static void GetJobOrderByContainer_New(string searchString)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderByContainer_New, searchString);
        }

        public static void SetDetwinJob(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetDetwinJob, hessianList);
        }

        public static void GetContainerInfo(Container cntr)
        {
            _instance._thread.Do(HessianCommType.GetContainerInfo, cntr.cntrNo);
        }

        public static void GetTwinContainerInfo(Container cntr)
        {
            _instance._thread.Do(HessianCommType.GetTwinContainerInfo, cntr);
        }

        public static void SetJobDone(VmtWorkOrder task)
        {
            _instance._thread.Do(HessianCommType.SetJobDone, task);
        }

        public static void SetJobDoneForLocationChange(VmtWorkOrder task)
        {
            _instance._thread.Do(HessianCommType.SetJobDoneForLocationChange, task);
        }

        public static void DoSwap4Manual(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.DoSwap4Manual, hessianList);
        }

        public static void GetMaxRow()
        {
            _instance._thread.Do(HessianCommType.GetMaxRow, null);
        }

        public static void GetNoWorkArea(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetNoWorkArea, loc);
        }

        public static void GetNoWorkArea1(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetNoWorkArea1, loc);
        }

        public static void GetNoWorkArea2(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetNoWorkArea2, loc);
        }

        public static void GetNoWorkTier(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetNoWorkTier, loc);
        }

        public static void GetNoWorkTier1(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetNoWorkTier1, loc);
        }

        public static void GetNoWorkTier2(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetNoWorkTier2, loc);
        }

        public static void SetChangePosition(Task task)
        {
            _instance._thread.Do(HessianCommType.SetChangePosition, task);
        }

        public static void GetIsValidLocation(Location loc)
        {
            _instance._thread.Do(HessianCommType.GetIsValidLocation, loc);
        }

        public static void SetJobReOnChassis(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetJobReOnChassis, hessianList);
        }

        // Get Driver Job History
        public static void GetDriverJobHistory(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.GetDriverJobHistory, hessianList);
        }

        public static void SetParkLocation(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetParkLocation, hessianList);
        }

        public static void GetParkLocation(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.GetParkLocation, hessianList);
        }

        public static void CheckYcDeTwin(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.CheckYcDeTwin, hessianList);
        }

        public static void SetBestPick(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetBestPick, hessianList);
        }

        public static void Validate4LoadingSwapping(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.Validate4LoadingSwapping, hessianList);
        }
        
        public static void SetReallocation(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetReallocation, hessianList);
        }

        public static void ChassisOrderComplete(ChassisOrder chass)
        {
            _instance._thread.Do(HessianCommType.ChassisOrderComplete, chass);
        }

        public static void ValidChassisInfos(ChassisInventory chassIn)
        {
            _instance._thread.Do(HessianCommType.ValidChassisInfos, chassIn);
        }

        public static void ItvLinkChassis(ChassisOrder chass)
        {
            _instance._thread.Do(HessianCommType.ItvLinkChassis, chass);
        }

        public static void ItvUnLinkChassis(ChassisOrder chass)
        {
            _instance._thread.Do(HessianCommType.ItvUnLinkChassis, chass);
        }

        public static void SetGateCancelJob(HessianList list)
        {
            _instance._thread.Do(HessianCommType.SetGateCancelJob, list);
        }

        public static void InitPLCMessage(HessianList list)
        {
            _instance._thread.Do(HessianCommType.InitPLCMessage, list);
        }
        public static void ProcessPLC(VmtDomain domain)
        {
            _instance._thread.Do(HessianCommType.ProcessPLC, domain);
        }

        public static void CancelPLC(VmtDomain domain)
        {
            _instance._thread.Do(HessianCommType.CancelPLC, domain);
        }
        public static void ReleasePLCLock(HessianList list)
        {
            _instance._thread.Do(HessianCommType.ReleasePLCLock, list);
        }

        public static void GetConfigValue(String configId)
        {
            _instance._thread.Do(HessianCommType.GetConfigValue, configId);
        }
        //////////////////////////////////////////////////
        // - Hessian Single Test
        public static void KeepAlive_Test()
        {
            _instance._thread.Do(HessianCommType.KeepAlive_Test, null);
        }

        public static void GetInventoryList_Test(Location location)
        {
            _instance._thread.Do(HessianCommType.GetInventoryList_Test, location);
        }

        public static void GetInventory_Test(Container container)
        {
            _instance._thread.Do(HessianCommType.GetInventory_Test, container);
        }

        public static void GetBlockMapList_Test(string blck)
        {
            _instance._thread.Do(HessianCommType.GetBlockMapList_Test, blck);
        }        

        public static void SetMachineStatusChanged_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.SetMachineStatusChanged_Test, machine);
        }

        public static void SetMachineStop_Test(MachineStop machinestop)
        {
            _instance._thread.Do(HessianCommType.SetMachineStop_Test, machinestop);
        }

        public static void SetMachinePassed_Test(YtTask yttask)
        {
            _instance._thread.Do(HessianCommType.SetMachinePassed_Test, yttask);
        }

        public static void SetManualActivation_Test(Task task)
        {
            _instance._thread.Do(HessianCommType.SetManualActivation_Test, task);
        }

        public static void SetMachineArrivalInfo_Test(YtTask yttask)
        {
            _instance._thread.Do(HessianCommType.SetMachineArrivalInfo_Test, yttask);
        }

        public static void SetMachineReady_Test(Task task)
        {
            _instance._thread.Do(HessianCommType.SetMachineReady_Test, task);
        }

        public static void GetMachineStop_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineStop_Test, machine);
        }

        public static void GetMachineNotice_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineNotice_Test, machine);
        }

        public static void SetMachineNotice_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.SetMachineNotice_Test, machine);
        }

        public static void GetPrecedingYtList_Test(StsTask ststask)
        {
            _instance._thread.Do(HessianCommType.GetPrecedingYtList_Test, ststask);
        }

        public static void SetJobDone_Test(Task task)
        {
            _instance._thread.Do(HessianCommType.SetJobDone_Test, task);
        }

        public static void GetJobOrderList_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderList_Test, machine);
        }

        public static void GetUserAccessRole_Test(User user)
        {
            _instance._thread.Do(HessianCommType.GetUserAccessRole_Test, user);
        }

        public static void SetLogin4Machine_Test(Login login)
        {
            _instance._thread.Do(HessianCommType.SetLogin4Machine_Test, login);
        }

        public static void SetLogout4Machine_Test(LogOut logOut)
        {
            _instance._thread.Do(HessianCommType.SetLogout4Machine_Test, logOut);
        }

        public static void GetMachineStopCodeList_Test(string machineType)
        {
            _instance._thread.Do(HessianCommType.GetMachineStopCodeList_Test, machineType);
        }

        // 2015-12-28 추가
        public static void GetJobOrderByContainer_Test(string seachString)
        {
            _instance._thread.Do(HessianCommType.GetJobOrderByContainer_Test, seachString);
        }

        public static void GetMachineList_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineList_Test, machine);
        }

        public static void GetMachineListOfPool_Test(Machine machine)
        {
            _instance._thread.Do(HessianCommType.GetMachineListOfPool_Test, machine);
        }

        public static void DoSwap4Manual_Test(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.DoSwap4Manual_Test, hessianList);
        }

        public static void GetBlockList_Test()
        {
            _instance._thread.Do(HessianCommType.GetBlockList_Test, null);
        }

        public static void SetJobStatus_Test(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetJobStatus_Test, hessianList);
        }

        public static void SetDetwinJob_Test(HessianList hessianList)
        {
            _instance._thread.Do(HessianCommType.SetDetwinJob_Test, hessianList);
        }
        //////////////////////////////////////////////////
    }
}
