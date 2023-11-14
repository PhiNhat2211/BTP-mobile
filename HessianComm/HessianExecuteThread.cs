using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Common.Util;
using hessiancsharp.Class;

namespace HessianComm
{
    public class HessianExecuteThread
    {
        // Thread Queue
        public class QueueData
        {
            public HessianCommType type { get; private set; }

            public object obj { get; set; }

            public QueueData(HessianCommType type, object obj)
            {
                this.type = type;
                this.obj = obj;
            }
        }

        private Queue<QueueData> _queue = new Queue<QueueData>();
        private AutoResetEvent _queueEvent = new AutoResetEvent(false);
        private AutoResetEvent _quitEvent = new AutoResetEvent(false);
        // WaitHandle[] _events = new WaitHandle[] { new AutoResetEvent(false), new AutoResetEvent(false) };
        private WaitHandle[] _events = null;
        private Thread _thread = null;
        private HessianAPI _parent = null;

        private Timer _timerKeepAlive = null;
        private Timer _timerKeepAliveStandAlone = null;
        private Timer _timerDGPSAlive = null;
        private Timer _timerPinningStation = null;
        private Timer _timerJobOrderList = null;
        private Timer _timerMachineStatusChanged = null;
        private Timer _timerMachineNotice = null;
        private Timer _timerPrecedingYtList = null;
        private Timer _timerSetVMTMachineStatus = null;
        private Timer _timerCheckMachineLogout = null;
        private Timer _timerCheckMachineStop = null;
        private Timer _timerCheckPLCData = null;
        private Timer _timerCheckPLCTwistLock = null;
        private Timer _timerGetChangedMachineLocation = null;
        
        private int _IntervalJobOrderListTimer = 0;
        private int _IntervalGetMachineStatusChangedTimer = 0;
        private int _IntervalKeepAliveTimer = 0;
        private int _IntervalGetMachineStopTimer = 0;
        private int _IntervalCheckPLCDataTimer = 0;
        private int _IntervalCheckPLCTwistLockTimer = 0;
        private int _IntervalGetChangedMachineLocationTimer = 0;

        private HessianAPI.HessianCommCallback _callback = null;

        public HessianAPI.HessianCommCallback Callback
        {
            set { this._callback = value; }
        }        

        public HessianExecuteThread(HessianAPI parent)
        {
            this._parent = parent;
            this._thread = new Thread(this.ThreadFunc);
            this._events = new WaitHandle[] { this._quitEvent, this._queueEvent };
        }                

        public void Start()
        {
            this._thread.Start();            
        }      

        public void End()
        {
            this._quitEvent.Set();            
        }        

        public void KeepAliveTimerCallback(object state)
        {
            this.Do(HessianCommType.KeepAlive, null);
        }

        public void KeepAliveStandAloneTimerCallback(object state)
        {
            this.Do(HessianCommType.KeepAliveStandAlone, null);
        }

        public void DGPSAliveTimerCallback(object state)
        {
            this.Do(HessianCommType.DGPSAlive, null);
        }

        public void PinningStationTimerCallback(object state)
        {
            this.Do(HessianCommType.SetPinningStation, null);
        }

        public void GetJobOrderTimerCallback(object state)
        {
            this.Do(HessianCommType.GetJobOrderList, state);
        }

        public void GetJobOrderTimerCallback_New(object state)
        {
            this.Do(HessianCommType.GetJobOrderList_New, state);
        }

        public void GetMachineJobTimerCallback_New(object state)
        {
            this.Do(HessianCommType.GetMachineJobByKeys_New, state);
        }

        public void GetMachineStatusChangedTimerCallback_New(object state)
        {
            this.Do(HessianCommType.GetMachineStatusChanged, state);
        }        

        public void GetMachineNoticeCallback(object state)
        {
            this.Do(HessianCommType.GetMachineNotice, state);
        }

        public void GetPrecedingYtListCallback(object state)
        {
            this.Do(HessianCommType.GetPrecedingYtList, state);
        }

        public void CheckMachineLogoutCallback(object state)
        {
            this.Do(HessianCommType.GetMachineList4LogoutCheck, state);
        }

        public void CheckMachineStopCallback(object state)
        {
            this.Do(HessianCommType.GetMachineStop, state);
        }

        public void CheckPLCDataCallback(object state)
        {
            this.Do(HessianCommType.CheckPLCData, state);
        }
        public void CheckPLCTwistLockCallback(object state)
        {
            this.Do(HessianCommType.CheckPLCTwistLock, state);
        }
        public void GetChangedMachineLocationCallback(object state)
        {
            this.Do(HessianCommType.GetChangedMachineLocation, state);
        }

        public void SetVMTMachineStatusCallback(object state)
        {
            this.Do(HessianCommType.SetVMTMachineStatus, state);
        }

        public void StartPolling(HessianCommType type, object obj, int timeSpan)
        {
            switch (type)
            {
                case HessianCommType.KeepAlive:
                    {
                        if (this._timerKeepAlive == null)
                        {
                            this._timerKeepAlive = new Timer(this.KeepAliveTimerCallback);
                        }

                        this._timerKeepAlive.Change(0, timeSpan);
                        this._IntervalKeepAliveTimer = timeSpan;
                    }
                    break;
                case HessianCommType.KeepAliveStandAlone:
                    {
                        if (this._timerKeepAliveStandAlone == null)
                        {
                            this._timerKeepAliveStandAlone = new Timer(this.KeepAliveStandAloneTimerCallback);
                        }

                        this._timerKeepAliveStandAlone.Change(0, timeSpan);
                    }
                    break;
                // Aug 09 2023 Remove DGPS & Pinning Api
                case HessianCommType.DGPSAlive:
                    {
                        //if (this._timerDGPSAlive == null)
                        //{
                        //    this._timerDGPSAlive = new Timer(this.DGPSAliveTimerCallback);
                        //}

                        //this._timerDGPSAlive.Change(0, timeSpan);
                    }
                    break;
                case HessianCommType.SetPinningStation:
                    {
                        //if (this._timerPinningStation == null)
                        //{
                        //    this._timerPinningStation = new Timer(this.PinningStationTimerCallback);
                        //}

                        //this._timerPinningStation.Change(0, timeSpan);
                    }
                    break;


                case HessianCommType.GetMachineJobByKeys_New:
                    {
                        if (this._timerJobOrderList == null)
                        {
                            this._timerJobOrderList = new Timer(this.GetMachineJobTimerCallback_New, obj, 0, 0);
                        }

                        this._timerJobOrderList.Change(0, timeSpan);
                        this._IntervalJobOrderListTimer = timeSpan;
                    }
                    break;
                case HessianCommType.GetMachineStatusChanged:
                    {
                        if (this._timerMachineStatusChanged == null)
                        {
                            this._timerMachineStatusChanged = new Timer(this.GetMachineStatusChangedTimerCallback_New, obj, 0, 0);
                        }

                        this._timerMachineStatusChanged.Change(0, timeSpan);
                        this._IntervalGetMachineStatusChangedTimer = timeSpan;
                    }
                    break;
                case HessianCommType.GetMachineNotice:
                    {
                        if (this._timerMachineNotice == null)
                        {
                            this._timerMachineNotice = new Timer(this.GetMachineNoticeCallback, obj, 0, 0);
                        }

                        this._timerMachineNotice.Change(0, timeSpan);
                    }
                    break;
                case HessianCommType.GetPrecedingYtList:
                    {
                        if (this._timerPrecedingYtList == null)
                        {
                            this._timerPrecedingYtList = new Timer(this.GetPrecedingYtListCallback, obj, 0, 0);
                        }

                        this._timerPrecedingYtList.Change(0, timeSpan);
                    }
                    break;
                case HessianCommType.GetMachineList4LogoutCheck:
                    {
                        if (this._timerCheckMachineLogout == null)
                        {
                            this._timerCheckMachineLogout = new Timer(this.CheckMachineLogoutCallback, obj, timeSpan, timeSpan);
                        }

                        this._timerCheckMachineLogout.Change(timeSpan, timeSpan);
                    }
                    break;
                case HessianCommType.GetMachineStop:
                    {
                        if (this._timerCheckMachineStop == null)
                        {
                            this._timerCheckMachineStop = new Timer(this.CheckMachineStopCallback, obj, 0, timeSpan);
                        }
                        
                        this._IntervalGetMachineStopTimer = timeSpan;
                    }
                    break;
                // Aug 09 27 2023 Remove Plc Api
                case HessianCommType.CheckPLCData:
                    {
                        //if (this._timerCheckPLCData == null)
                        //{
                        //    this._timerCheckPLCData = new Timer(this.CheckPLCDataCallback, obj, 0, timeSpan);
                        //}

                        //this._IntervalCheckPLCDataTimer = timeSpan;
                    }
                    break;
                case HessianCommType.CheckPLCTwistLock:
                    {
                        //if (this._timerCheckPLCTwistLock == null)
                        //{
                        //    this._timerCheckPLCTwistLock = new Timer(this.CheckPLCTwistLockCallback, obj, 0, timeSpan);
                        //}

                        //this._IntervalCheckPLCTwistLockTimer = timeSpan;
                    }
                    break;
                case HessianCommType.GetChangedMachineLocation:
                    {
                        if (this._timerGetChangedMachineLocation == null)
                        {
                            this._timerGetChangedMachineLocation = new Timer(this.GetChangedMachineLocationCallback, obj, 0, timeSpan);
                        }

                        this._IntervalGetChangedMachineLocationTimer = timeSpan;
                    }
                    break;
                case HessianCommType.SetVMTMachineStatus:
                    {
                        if (this._timerSetVMTMachineStatus == null)
                        {
                            this._timerSetVMTMachineStatus = new Timer(this.SetVMTMachineStatusCallback, obj, 0, 0);
                        }

                        this._timerSetVMTMachineStatus.Change(0, timeSpan);
                    }
                    break;
            }
        }

        public void StopPolling(HessianCommType type)
        {
            switch (type)
            {
                case HessianCommType.KeepAlive:
                    if (this._timerKeepAlive != null)
                    {
                        this._timerKeepAlive.Dispose();
                        this._timerKeepAlive = null;
                    }
                    break;
                case HessianCommType.KeepAliveStandAlone:
                    if (this._timerKeepAliveStandAlone != null)
                    {
                        this._timerKeepAliveStandAlone.Dispose();
                        this._timerKeepAliveStandAlone = null;
                    }
                    break;
                case HessianCommType.DGPSAlive:
                    if (this._timerDGPSAlive != null)
                    {
                        this._timerDGPSAlive.Dispose();
                        this._timerDGPSAlive = null;
                    }
                    break;
                case HessianCommType.SetPinningStation:
                    if (this._timerPinningStation != null)
                    {
                        this._timerPinningStation.Dispose();
                        this._timerPinningStation = null;
                    }
                    break;
                case HessianCommType.GetJobOrderList:
                    if (this._timerJobOrderList != null)
                    {
                        this._timerJobOrderList.Dispose();
                        this._timerJobOrderList = null;
                    }
                    break;
                case HessianCommType.GetMachineNotice:
                    if (this._timerMachineNotice != null)
                    {
                        this._timerMachineNotice.Dispose();
                        this._timerMachineNotice = null;
                    }
                    break;
                case HessianCommType.GetPrecedingYtList:
                    if (this._timerPrecedingYtList != null)
                    {
                        this._timerPrecedingYtList.Dispose();
                        this._timerPrecedingYtList = null;
                    }
                    break;
                case HessianCommType.GetMachineList4LogoutCheck:
                    if (this._timerCheckMachineLogout != null)
                    {
                        this._timerCheckMachineLogout.Dispose();
                        this._timerCheckMachineLogout = null;
                    }
                    break;
                case HessianCommType.GetMachineStop:
                    if (this._timerCheckMachineStop != null)
                    {
                        this._timerCheckMachineStop.Dispose();
                        this._timerCheckMachineStop = null;
                    }
                    break;
                case HessianCommType.GetMachineJobByKeys_New:
                    if (this._timerJobOrderList != null)
                    {
                        this._timerJobOrderList.Dispose();
                        this._timerJobOrderList = null;
                    }
                    break;
                case HessianCommType.GetMachineStatusChanged:
                    if (this._timerMachineStatusChanged != null)
                    {
                        this._timerMachineStatusChanged.Dispose();
                        this._timerMachineStatusChanged = null;
                    }
                    break;
                case HessianCommType.CheckPLCData:
                    if (this._timerCheckPLCData != null)
                    {
                        this._timerCheckPLCData.Dispose();
                        this._timerCheckPLCData = null;
                    }
                    break;
                case HessianCommType.CheckPLCTwistLock:
                    if (this._timerCheckPLCTwistLock != null)
                    {
                        this._timerCheckPLCTwistLock.Dispose();
                        this._timerCheckPLCTwistLock = null;
                    }
                    break;
                case HessianCommType.GetChangedMachineLocation:
                    if (this._timerGetChangedMachineLocation != null)
                    {
                        this._timerGetChangedMachineLocation.Dispose();
                        this._timerGetChangedMachineLocation = null;
                    }
                    break;
                case HessianCommType.SetVMTMachineStatus:
                    if (this._timerSetVMTMachineStatus != null)
                    {
                        this._timerSetVMTMachineStatus.Dispose();
                        this._timerSetVMTMachineStatus = null;
                    }
                    break;
            }
        }

        public void Do(HessianCommType type, object obj)
        {
            lock (((ICollection)this._queue).SyncRoot)
            {
                bool bEnqueue = true;

                if (type == HessianCommType.SetJobStatus ||
                    type == HessianCommType.SetJobDone)
                {
                }
                else
                {
                    // job Order list를 요청할 때 마다 타이머를 갱신해주자..(polling에 의한 요청이 아닌 instant 요청이 오게될 경우 중복호출 방지)
                    if (this._timerJobOrderList != null &&
                        (type == HessianCommType.GetJobOrderList || type == HessianCommType.GetJobOrderList_New ||
                        type == HessianCommType.GetMachineJobByKeys_New))
                    {
                        this._timerJobOrderList.Change(this._IntervalJobOrderListTimer, this._IntervalJobOrderListTimer);
                    }
                    else if (this._timerMachineStatusChanged != null && type == HessianCommType.GetMachineStatusChanged)
                    {
                        this._timerMachineStatusChanged.Change(this._IntervalGetMachineStatusChangedTimer, this._IntervalGetMachineStatusChangedTimer);
                    }
                    else if (type == HessianCommType.KeepAlive &&
                        this._timerKeepAlive != null)
                    {
                        this._timerKeepAlive.Change(this._IntervalKeepAliveTimer, this._IntervalKeepAliveTimer);
                    }
                    else if (type == HessianCommType.GetMachineStop &&
                        this._timerCheckMachineStop != null)
                    {
                        this._timerCheckMachineStop.Change(this._IntervalGetMachineStopTimer, this._IntervalGetMachineStopTimer);
                    }

                    for (int i = 0; i < this._queue.Count; i++)
                    {
                        if (this._queue.ElementAt(i).type == type)
                        {
                            this._queue.ElementAt(i).obj = obj;
                            bEnqueue = false;
                            break;
                        }
                    }
                }

                if (bEnqueue)
                {
                    this._queue.Enqueue(new QueueData(type, obj));
                }
            }

            this._queueEvent.Set(); // __queuetimer
        }

        private void HessianToLog(Hashtable hashTable)
        {
            Dictionary<string, string> keyPairs = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> items in hashTable)
            {
                keyPairs.Add(items.Key, items.Value);
                // Logging.Instance.WriteInformation(string.Format("Key: {0} \t Value: {1}", items.Key, items.Value));
                // Logger.Log("Do Execute : " + data.type.ToString());
            }
        }

        // private DateTime totalDateTime = DateTime.Now;     
        private void ThreadFunc(object obj)
        {
            try
            {
                int ret = 0;
                QueueData data = null;
                while (true)
                {
                    ret = WaitHandle.WaitAny(this._events, -1); // 0.2 sec handle get waiting
                                        
                    if (ret == 0)
                    {
                        // QuitEvent
                        break;
                    }
                    else if (ret == 1)
                    {
                        // QueueEvent

                        lock (((ICollection)this._queue).SyncRoot)
                        {
                            if (this._queue.Count == 0)
                            {
                                continue;
                            }

                            data = this._queue.Dequeue();

                            if (this._queue.Count != 0)
                            {
                                this._queueEvent.Set();
                            }
                        }

                        DateTime typeDateTime = DateTime.Now;
                        TimeSpan tSpan = DateTime.Now - typeDateTime;

                        try
                        {
                            // Logger.Log("Do Execute : " + data.type.ToString());

                            //System.Diagnostics.Trace.WriteLine("[VMT Timestamp]");
                            //System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(+)");

                            //Common.Util.Logger.Log("[VMT Timestamp]");
                            //Common.Util.Logger.Log("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(+)");

                            // Aug 09 2023 HandleLog
                            this._callback(data.type, data.obj, true);

                            switch (data.type)
                            {
                                case HessianCommType.KeepAlive:
                                    this._callback(HessianCommType.KeepAlive, this._parent.SystemControl.keepAlive());
                                    break;
                                case HessianCommType.KeepAliveStandAlone:
                                    this._callback(HessianCommType.KeepAliveStandAlone, true);
                                    break;
                                case HessianCommType.DGPSAlive:
                                    this._callback(HessianCommType.DGPSAlive, true);
                                    break;
                                case HessianCommType.SetPinningStation:
                                    this._callback(HessianCommType.SetPinningStation, null);
                                    break;
                                case HessianCommType.GetUserAccessRole:
                                    this._callback(HessianCommType.GetUserAccessRole, this._parent.UserControl.getUserAccessRole(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.ChangeDriver:
                                    this._callback(HessianCommType.ChangeDriver, this._parent.UserControl.changeDriver((HessianList) data.obj));
                                    break;
                                // case HessianCommType.GetLoginInfo4Machine:
                                //    this._callback(HessianCommType.GetLoginInfo4Machine, this._parent.UserControl.getLoginInfo4Machine(data.obj.ToHessianHashtable()));
                                //    break;
                                case HessianCommType.SetLogin4Machine:
                                    this._callback(HessianCommType.SetLogin4Machine, this._parent.UserControl.setLogin4Machine(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.ChangeDriverCheck:
                                    this._callback(HessianCommType.ChangeDriverCheck, this._parent.UserControl.changeDriverCheck(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineStatusChanged:
                                    this._callback(HessianCommType.SetMachineStatusChanged, this._parent.vmtMachineControl.setMachineStatusChanged(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetJobOrderList:
                                    this._callback(HessianCommType.GetJobOrderList, this._parent.JobControl.getJobOrderList(data.obj.ToHessianHashtable()));                                    
                                    break;
                                case HessianCommType.GetJobOrderList_New:
                                    this._callback(HessianCommType.GetJobOrderList_New, this._parent.VmtWorkOrderControl.getJobOrderList((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetLogout4Machine:
                                    this._callback(HessianCommType.SetLogout4Machine, this._parent.UserControl.setLogout4Machine(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetLogin4MachineList:
                                    this._callback(HessianCommType.GetLogin4MachineList, this._parent.UserControl.getLogin4MachineList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachinePassed:
                                    this._callback(HessianCommType.SetMachinePassed, this._parent.vmtMachineControl.setMachinePassed(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetManualActivation:
                                    this._callback(HessianCommType.SetManualActivation, this._parent.JobControl.setManualActivation(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineArrival:
                                    this._callback(HessianCommType.SetMachineArrival, this._parent.vmtMachineControl.setMachineArrival((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetMachineReady:
                                    this._callback(HessianCommType.SetMachineReady, this._parent.vmtMachineControl.setMachineReady((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetItvDone:
                                    this._callback(HessianCommType.SetItvDone, this._parent.vmtMachineControl.setItvDone((HessianList)data.obj));
                                    break;
                                // Set Job Done For QC
                                case HessianCommType.SetQCJobReleaseByYt:
                                    this._callback(HessianCommType.SetQCJobReleaseByYt, this._parent.VmtWorkOrderControl.setQCJobReleaseByYt((HessianList)data.obj));
                                    break;
                                case HessianCommType.ChangeChassisNo:
                                    this._callback(HessianCommType.ChangeChassisNo, this._parent.vmtMachineControl.changeChassisNo((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetChssUsingData:
                                    this._callback(HessianCommType.GetChssUsingData, this._parent.vmtMachineControl.getChssUsingData((string)data.obj));
                                    break;
                                case HessianCommType.GetMachineStopCodeList:
                                    this._callback(HessianCommType.GetMachineStopCodeList, this._parent.vmtDefineControl.getMachineStopCodeList((string)data.obj));
                                    break;
                                case HessianCommType.GetMachineAccessAction:
                                    this._callback(HessianCommType.GetMachineAccessAction, this._parent.UserControl.getMachineAccessAction((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetBlockListForYardSector:
                                    this._callback(HessianCommType.GetBlockListForYardSector, this._parent.vmtDefineControl.getBlockListForYardSector((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineStop:
                                    this._callback(HessianCommType.GetMachineStop, this._parent.vmtMachineControl.getMachineStop(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineStop:
                                    this._callback(HessianCommType.SetMachineStop, this._parent.vmtMachineControl.setMachineStop(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetMachineNotice:
                                    this._callback(HessianCommType.GetMachineNotice, this._parent.vmtMachineControl.getMachineNotice(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineNotice:
                                    this._callback(HessianCommType.SetMachineNotice, this._parent.vmtMachineControl.setMachineNotice(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetPrecedingYtList:
                                    this._callback(HessianCommType.GetPrecedingYtList, this._parent.vmtMachineControl.getPrecedingYtList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetConfirmJobByScanner:
                                    this._callback(HessianCommType.SetConfirmJobByScanner, this._parent.JobControl.setConfirmJobByScanner((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetBlockMapListForYt:
                                    this._callback(HessianCommType.GetBlockMapListForYt, this._parent.vmtDefineControl.getBlockMapListForYt((string)data.obj));
                                    break;
                                case HessianCommType.GetBlockMapList:
                                    this._callback(HessianCommType.GetBlockMapList, this._parent.vmtDefineControl.getBlockMapList((string)data.obj));
                                    break;
                                case HessianCommType.GetBlockMapList1:
                                    this._callback(HessianCommType.GetBlockMapList1, this._parent.vmtDefineControl.getBlockMapList((string)data.obj));
                                    break;
                                case HessianCommType.GetBlockMapList2:
                                    this._callback(HessianCommType.GetBlockMapList2, this._parent.vmtDefineControl.getBlockMapList((string)data.obj));
                                    break;
                                case HessianCommType.GetBlockMapSwapList:
                                    this._callback(HessianCommType.GetBlockMapSwapList, this._parent.vmtDefineControl.getBlockMapList((string)data.obj));
                                    break;
                                case HessianCommType.GetBlockList:
                                    this._callback(HessianCommType.GetBlockList, this._parent.vmtDefineControl.getBlockList());
                                    break;
                                case HessianCommType.GetBlockListForBlockMap:
                                    this._callback(HessianCommType.GetBlockListForBlockMap, this._parent.vmtDefineControl.getBlockListForBlockMap());
                                    break;
                                case HessianCommType.GetVmtAutoStartConfig:
                                    this._callback(HessianCommType.GetVmtAutoStartConfig, this._parent.vmtDefineControl.getVmtAutoStartConfig());
                                    break;
                                case HessianCommType.GetEmptySwappingTargetList:
                                    this._callback(HessianCommType.GetEmptySwappingTargetList, this._parent.VmtEmptySwapControl.getEmptySwappingTargetList((HessianList)data.obj));
                                    break;
                                case HessianCommType.DoEmptySwap:
                                    this._callback(HessianCommType.DoEmptySwap, this._parent.VmtEmptySwapControl.doEmptySwap((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetJobStatus:
                                    this._callback(HessianCommType.SetJobStatus, this._parent.JobControl.setJobStatus((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetPickedContainer:
                                    //this._callback(HessianCommType.SetPickedContainer, this._parent.JobControl.setPickedContainer((HessianList)data.obj));
                                    this._callback(HessianCommType.SetPickedContainer, this._parent.VmtWorkOrderControl.setPickedContainer((HessianList)data.obj));
                                    ((HessianList)data.obj).Clear();
                                    break;
                                case HessianCommType.SetDetwinJob:
                                    this._callback(HessianCommType.SetDetwinJob, this._parent.JobControl.setDetwinJob((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetInventory:
                                    this._callback(HessianCommType.GetInventoryList, this._parent.ContainerControl.getInventory(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryList:
                                    this._callback(HessianCommType.GetInventoryList, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryListEx:
                                    this._callback(HessianCommType.GetInventoryListEx, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryListMulti:
                                    this._callback(HessianCommType.GetInventoryListMulti, this._parent.ContainerControl.getInventoryList4Multi((ArrayList)data.obj));                                    
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListMulti_New:
                                    this._callback(HessianCommType.GetInventoryListMulti_New, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryList4Multi_Sync:
                                    this._callback(HessianCommType.GetInventoryList4Multi_Sync, this._parent.VmtContainerControl.getInventoryList4Multi_Sync((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListMultiSwap_New:
                                    this._callback(HessianCommType.GetInventoryListMultiSwap_New, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListMulti_New1:
                                    this._callback(HessianCommType.GetInventoryListMulti_New1, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListMulti_New2:
                                    this._callback(HessianCommType.GetInventoryListMulti_New2, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListBackground:
                                    this._callback(HessianCommType.GetInventoryListBackground, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryListBackgroundEx:
                                    this._callback(HessianCommType.GetInventoryListBackgroundEx, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryListBackgroundMulti:
                                    this._callback(HessianCommType.GetInventoryListBackgroundMulti, this._parent.ContainerControl.getInventoryList4Multi((ArrayList)data.obj));                                    
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListBackgroundMulti_New:
                                    this._callback(HessianCommType.GetInventoryListBackgroundMulti_New, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListData:
                                    this._callback(HessianCommType.GetInventoryListData, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryListDataEx:
                                    this._callback(HessianCommType.GetInventoryListDataEx, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventoryListDataMulti:
                                    this._callback(HessianCommType.GetInventoryListDataMulti, this._parent.ContainerControl.getInventoryList4Multi((ArrayList)data.obj));                                   
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListDataMultiSwap_New:
                                    this._callback(HessianCommType.GetInventoryListDataMultiSwap_New, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListDataMulti_New1:
                                    this._callback(HessianCommType.GetInventoryListDataMulti_New1, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetInventoryListDataMulti_New2:
                                    this._callback(HessianCommType.GetInventoryListDataMulti_New2, this._parent.VmtContainerControl.getInventoryList4Multi((ArrayList)data.obj));
                                    ((ArrayList)data.obj).Clear();
                                    break;
                                case HessianCommType.GetMachineList:
                                    Hashtable tbl = data.obj.ToHessianHashtable();
                                    if (Convert.ToBoolean(tbl["isOn"]))
                                        this._callback(HessianCommType.GetMachineList, this._parent.vmtMachineControl.getMachineList());
                                    else
                                        this._callback(HessianCommType.GetMachineList, this._parent.vmtMachineControl.getMachineListByType(tbl) );
                                    break;
                                case HessianCommType.GetChangedMachineLocation:
                                    this._callback(HessianCommType.GetChangedMachineLocation, this._parent.vmtMachineControl.getChangedMachineLocation((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineList4LogoutCheck:
                                    this._callback(HessianCommType.GetMachineList4LogoutCheck, this._parent.vmtMachineControl.getMachineListByType(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetMachineListOfPool:
                                    this._callback(HessianCommType.GetMachineListOfPool, this._parent.vmtMachineControl.getMachineListOfPool(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.GetJobOrderByContainer:
                                    this._callback(HessianCommType.GetJobOrderByContainer, this._parent.JobControl.getJobOrderByContainer((string)data.obj));                                    
                                    break;
                                case HessianCommType.GetJobOrderByContainer_New:
                                    this._callback(HessianCommType.GetJobOrderByContainer_New, this._parent.VmtWorkOrderControl.getJobOrderByContainer((string)data.obj));
                                    break;

                                case HessianCommType.GetContainerInfo:
                                    //this._callback(HessianCommType.GetContainerInfo, this._parent.VmtContainerControl.getContainerInfo(data.obj.ToHessianHashtable()));
                                    this._callback(HessianCommType.GetContainerInfo, this._parent.VmtContainerControl.getContainerInfo((string)data.obj));
                                    break;

                                case HessianCommType.GetTwinContainerInfo:
                                    //this._callback(HessianCommType.GetTwinContainerInfo, this._parent.VmtContainerControl.getContainerInfo(data.obj.ToHessianHashtable()));
                                    this._callback(HessianCommType.GetTwinContainerInfo, this._parent.VmtContainerControl.getContainerInfo((string)data.obj));
                                    break;

                                case HessianCommType.SetJobDone:
                                    //this._callback(HessianCommType.SetJobDone, this._parent.JobControl.setJobDone(data.obj.ToHessianHashtable()));
                                    this._callback(HessianCommType.SetJobDone, this._parent.VmtWorkOrderControl.setJobDone(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.SetJobDoneForLocationChange:
                                    this._callback(HessianCommType.SetJobDoneForLocationChange, this._parent.JobControl.setJobDone(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.DoSwap4Manual:
                                    this._callback(HessianCommType.DoSwap4Manual, this._parent.vmtMachineControl.doSwap4Manual((HessianList)data.obj));
                                    ((HessianList)data.obj).Clear();
                                    break;

                                case HessianCommType.GetMaxRow:
                                    this._callback(HessianCommType.GetMaxRow, this._parent.vmtDefineControl.getMaxRow());
                                    break;

                                case HessianCommType.GetNoWorkArea:
                                    this._callback(HessianCommType.GetNoWorkArea, this._parent.vmtDefineControl.getNoWorkArea(data.obj.ToHessianHashtable()));
                                    break;                               

                                case HessianCommType.GetNoWorkArea1:
                                    this._callback(HessianCommType.GetNoWorkArea1, this._parent.vmtDefineControl.getNoWorkArea(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.GetNoWorkArea2:
                                    this._callback(HessianCommType.GetNoWorkArea2, this._parent.vmtDefineControl.getNoWorkArea(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.GetNoWorkTier:
                                    this._callback(HessianCommType.GetNoWorkTier, this._parent.vmtDefineControl.getNoWorkTier(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.GetNoWorkTier1:
                                    this._callback(HessianCommType.GetNoWorkTier1, this._parent.vmtDefineControl.getNoWorkTier(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.GetNoWorkTier2:
                                    this._callback(HessianCommType.GetNoWorkTier2, this._parent.vmtDefineControl.getNoWorkTier(data.obj.ToHessianHashtable()));
                                    break;

                                case HessianCommType.SetChangePosition:
                                    Hashtable machine = (Hashtable)data.obj.ToHessianHashtable()["workingMchn"];
                                    Hashtable loc = (Hashtable)data.obj.ToHessianHashtable()["loc"];
                                    this._callback(HessianCommType.SetChangePosition, this._parent.vmtMachineControl.doChangePosition(machine, loc));
                                    break;

                                case HessianCommType.GetJobOrderListByKeys:
                                    this._callback(HessianCommType.GetJobOrderListByKeys, this._parent.JobControl.getJobOrderListByKeys((HessianList)data.obj));                                    
                                    break;
                                case HessianCommType.GetJobOrderListByKeys_New:
                                    this._callback(HessianCommType.GetJobOrderListByKeys_New, this._parent.VmtWorkOrderControl.getJobOrderListByKeys((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetJobOrderListByTruck:
                                    this._callback(HessianCommType.GetJobOrderListByTruck, this._parent.JobControl.getJobOrderListByKeys((HessianList)data.obj));                                    
                                    break;
                                case HessianCommType.GetJobOrderListByTruck_New:
                                    this._callback(HessianCommType.GetJobOrderListByTruck_New, this._parent.VmtWorkOrderControl.getJobOrderListByKeys((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineJobByKeys:
                                    this._callback(HessianCommType.GetMachineJobByKeys, this._parent.JobControl.getMachineJobByKeys((HessianList)data.obj));                                    
                                    break;
                                case HessianCommType.GetMachineJobByKeys_New:
                                    //if (((HessianList)data.obj)[1].ToString() == "YT")
                                    //    SaveLog("GetMachineJobByKeys Called", "");
                                    this._callback(HessianCommType.GetMachineJobByKeys_New, this._parent.VmtWorkOrderControl.getMachineJobByKeys((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineJobByKeys_Sync:
                                    this._callback(HessianCommType.GetMachineJobByKeys_Sync, this._parent.VmtWorkOrderControl.getMachineJobByKeys_Sync((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineStatusChanged:
                                    //if (((HessianList)data.obj)[1].ToString() == "YT")
                                    //    SaveLog("GetMachineStatusChanged Called", "");
                                    this._callback(HessianCommType.GetMachineStatusChanged, this._parent.vmtMachineControl.getMachineStatusChanged((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineJobByTruck:
                                    this._callback(HessianCommType.GetMachineJobByTruck, this._parent.JobControl.getMachineJobByKeys((HessianList)data.obj));                                    
                                    break;
                                case HessianCommType.GetMachineJobByTruck_New:
                                    this._callback(HessianCommType.GetMachineJobByTruck_New, this._parent.VmtWorkOrderControl.getMachineJobByKeys((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetIsValidLocation:
                                    this._callback(HessianCommType.GetIsValidLocation, this._parent.ContainerControl.isValidLocation(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetJobReOnChassis:
                                    this._callback(HessianCommType.SetJobReOnChassis, this._parent.JobControl.setJobReOnChassis((HessianList)data.obj));
                                    break;
                                // Get Driver Job History
                                case HessianCommType.GetDriverJobHistory:
                                    this._callback(HessianCommType.GetDriverJobHistory, this._parent.vmtMachineControl.getDriverJobHistory((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetParkLocation:
                                    this._callback(HessianCommType.SetParkLocation, this._parent.vmtMachineControl.setParkLocation((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetParkLocation:
                                    this._callback(HessianCommType.GetParkLocation, this._parent.vmtMachineControl.getParkLocation((HessianList)data.obj));
                                    break;

                                case HessianCommType.CheckYcDeTwin:
                                    this._callback(HessianCommType.CheckYcDeTwin, this._parent.VmtWorkOrderControl.checkYcDeTwin((HessianList)data.obj));
                                    break;

                                case HessianCommType.SetBestPick:
                                    this._callback(HessianCommType.SetBestPick, this._parent.VmtWorkOrderControl.setBestPick((HessianList)data.obj));
                                    break;

                                case HessianCommType.Validate4LoadingSwapping:
                                    this._callback(HessianCommType.Validate4LoadingSwapping, this._parent.VmtWorkOrderControl.validate4LoadingSwapping((HessianList)data.obj));
                                    break;
                                case HessianCommType.getSwapList:
                                    this._callback(HessianCommType.getSwapList, this._parent.VmtWorkOrderControl.getSwapList((HessianList)data.obj));
                                    break;
                                case HessianCommType.getSwapListRTG:
                                    this._callback(HessianCommType.getSwapListRTG, this._parent.VmtWorkOrderControl.getSwapList((HessianList)data.obj));
                                    break;
                                case HessianCommType.setEmptySwap:
                                    this._callback(HessianCommType.setEmptySwap, this._parent.VmtWorkOrderControl.setEmptySwap((HessianList)data.obj));
                                    break; 
                                case HessianCommType.SetReallocation:
                                    this._callback(HessianCommType.SetReallocation, this._parent.VmtWorkOrderControl.setReallocation((HessianList)data.obj));
                                break;
                                case HessianCommType.ChassisOrderComplete:
                                    this._callback(HessianCommType.ChassisOrderComplete, this._parent.VmtWorkOrderControl.chassisOrderComplete(data.obj.ToHessianHashtable()));
                                break;
                                case HessianCommType.ValidChassisInfos:
                                    this._callback(HessianCommType.ValidChassisInfos, this._parent.VmtWorkOrderControl.validChassisInfos(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.ItvLinkChassis:
                                    this._callback(HessianCommType.ItvLinkChassis, this._parent.VmtWorkOrderControl.itvLinkChassis(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.ItvUnLinkChassis:
                                    this._callback(HessianCommType.ItvUnLinkChassis, this._parent.VmtWorkOrderControl.itvUnLinkChassis(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetGateCancelJob:
                                    this._callback(HessianCommType.SetGateCancelJob, this._parent.VmtWorkOrderControl.setGateCancelJob((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetPLCAutoFlg:
                                    this._callback(HessianCommType.SetPLCAutoFlg, this._parent.IfVmtPLCControl.setPLCAutoFlg((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetPLCMsg:
                                    this._callback(HessianCommType.SetPLCMsg, this._parent.IfVmtPLCControl.setPLCMsg((HessianList)data.obj));
                                    break;
                                case HessianCommType.CheckPLCData:
                                    this._callback(HessianCommType.CheckPLCData, this._parent.VmtPLCControl.checkPLCData((HessianList)data.obj));
                                    break;
                                case HessianCommType.CheckPLCTwistLock:
                                    this._callback(HessianCommType.CheckPLCTwistLock, this._parent.VmtPLCControl.checkPLCTwistLock((HessianList)data.obj));
                                    break;
                                case HessianCommType.InitPLCMessage:
                                    this._callback(HessianCommType.InitPLCMessage, this._parent.VmtPLCControl.initPLCMessage((HessianList)data.obj));
                                    break;
                                case HessianCommType.ProcessPLC:
                                    this._callback(HessianCommType.ProcessPLC, this._parent.VmtPLCControl.processPLC(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.CancelPLC:
                                    this._callback(HessianCommType.CancelPLC, this._parent.VmtPLCControl.cancelPLC(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.ReleasePLCLock:
                                    this._callback(HessianCommType.ReleasePLCLock, this._parent.VmtPLCControl.releasePLCLock((HessianList)data.obj));
                                    break;
                                case HessianCommType.ReleaseYtFromJob:
                                    this._callback(HessianCommType.ReleaseYtFromJob, this._parent.VmtWorkOrderControl.releaseYtFromJob((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetVMTMachineStatus:
                                    this._callback(HessianCommType.SetVMTMachineStatus, this._parent.UserControl.setVMTMachineStatus((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetConfigValue:
                                    this._callback(HessianCommType.GetConfigValue, this._parent.vmtDefineControl.getConfigValue((String)data.obj));
                                    break;
                                /////////////////////////
                                // - Single Test
                                case HessianCommType.KeepAlive_Test:
                                    this._callback(HessianCommType.KeepAlive_Test, this._parent.SystemControl.keepAlive());
                                    break;
                                case HessianCommType.GetInventoryList_Test:
                                    this._callback(HessianCommType.GetInventoryList_Test, this._parent.ContainerControl.getInventoryList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetInventory_Test:
                                    this._callback(HessianCommType.GetInventory_Test, this._parent.ContainerControl.getInventory(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetBlockMapList_Test:
                                    this._callback(HessianCommType.GetBlockMapList_Test, this._parent.vmtDefineControl.getBlockMapList((string)data.obj));
                                    break;
                                case HessianCommType.SetMachineStatusChanged_Test:
                                    this._callback(HessianCommType.SetMachineStatusChanged_Test, this._parent.vmtMachineControl.setMachineStatusChanged(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineStop_Test:
                                    this._callback(HessianCommType.SetMachineStop_Test, this._parent.vmtMachineControl.setMachineStop(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachinePassed_Test:
                                    this._callback(HessianCommType.SetMachinePassed_Test, this._parent.vmtMachineControl.setMachinePassed(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetManualActivation_Test:
                                    this._callback(HessianCommType.SetManualActivation_Test, this._parent.JobControl.setManualActivation(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineArrivalInfo_Test:
                                    this._callback(HessianCommType.SetMachineArrivalInfo_Test, this._parent.vmtMachineControl.setMachineArrival((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetMachineReady_Test:
                                    this._callback(HessianCommType.SetMachineReady_Test, this._parent.vmtMachineControl.setMachineReady((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetMachineStop_Test:
                                    this._callback(HessianCommType.GetMachineStop_Test, this._parent.vmtMachineControl.getMachineStop(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetMachineNotice_Test:
                                    this._callback(HessianCommType.GetMachineNotice_Test, this._parent.vmtMachineControl.getMachineNotice(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetMachineNotice_Test:
                                    this._callback(HessianCommType.SetMachineNotice_Test, this._parent.vmtMachineControl.setMachineNotice(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetPrecedingYtList_Test:
                                    this._callback(HessianCommType.GetPrecedingYtList_Test, this._parent.vmtMachineControl.getPrecedingYtList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetJobDone_Test:
                                    this._callback(HessianCommType.SetJobDone_Test, this._parent.JobControl.setJobDone(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetJobOrderList_Test:
                                    this._callback(HessianCommType.GetJobOrderList_Test, this._parent.JobControl.getJobOrderList(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetUserAccessRole_Test:
                                    this._callback(HessianCommType.GetUserAccessRole_Test, this._parent.UserControl.getUserAccessRole(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetLogin4Machine_Test:
                                    this._callback(HessianCommType.SetLogin4Machine_Test, this._parent.UserControl.setLogin4Machine(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.SetLogout4Machine_Test:
                                    this._callback(HessianCommType.SetLogout4Machine_Test, this._parent.UserControl.setLogout4Machine(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetMachineStopCodeList_Test:
                                    this._callback(HessianCommType.GetMachineStopCodeList_Test, this._parent.vmtDefineControl.getMachineStopCodeList((string)data.obj));
                                    break;
                                // 2015-12-28 추가
                                case HessianCommType.GetJobOrderByContainer_Test:
                                    this._callback(HessianCommType.GetJobOrderByContainer_Test, this._parent.JobControl.getJobOrderByContainer((string)data.obj));
                                    break;
                                case HessianCommType.GetMachineList_Test:
                                    this._callback(HessianCommType.GetMachineList_Test, this._parent.vmtMachineControl.getMachineListByType(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.GetMachineListOfPool_Test:
                                    this._callback(HessianCommType.GetMachineListOfPool_Test, this._parent.vmtMachineControl.getMachineListOfPool(data.obj.ToHessianHashtable()));
                                    break;
                                case HessianCommType.DoSwap4Manual_Test:
                                    this._callback(HessianCommType.DoSwap4Manual_Test, this._parent.vmtMachineControl.doSwap4Manual((HessianList)data.obj));
                                    break;
                                case HessianCommType.GetBlockList_Test:
                                    this._callback(HessianCommType.GetBlockList_Test, this._parent.vmtDefineControl.getBlockList());
                                    break;
                                case HessianCommType.SetJobStatus_Test:
                                    this._callback(HessianCommType.SetJobStatus_Test, this._parent.JobControl.setJobStatus((HessianList)data.obj));
                                    break;
                                case HessianCommType.SetDetwinJob_Test:
                                    this._callback(HessianCommType.SetDetwinJob_Test, this._parent.JobControl.setDetwinJob((HessianList)data.obj));
                                    break;
                                /////////////////////////
                            }

                            //System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(-)");
                            //Common.Util.Logger.Log("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(-)");

                            tSpan = DateTime.Now - typeDateTime;

                            if (tSpan.TotalMilliseconds >= 1000.0)
                            {
                                //System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString() + " Response Slow");
                                //Common.Util.Logger.Log("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString() + " Response Slow");
                            }
                            else
                            {
                                //System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());
                                //Common.Util.Logger.Log("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());
                            }

                            data.obj = null;
                        }
                        catch (Exception ex)
                        {
                            this._callback(HessianCommType.ExceptionOccured, ex.Message + "@" + data.type);

                            HessianException hex = new HessianException(ex.Message, ex.InnerException);
                            hex.hessianCommType = data.type;

                            Logger.Log("HessianException Type : " + hex.hessianCommType.ToString());
                            Logger.Log("HessianException Message : " + hex.Message);

                            // tSpan = DateTime.Now - typeDateTime;
                            // Common.Util.Logger.Log("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());

                            if (ex.InnerException is WebException)
                            {
                                WebException wE = ex.InnerException as WebException;

                                switch (wE.Status)
                                {
                                    case WebExceptionStatus.ConnectFailure:
                                    case WebExceptionStatus.Timeout:
                                        {
                                            Logger.Log("HessianException Status : " + wE.Status.ToString());
                                        }
                                        break;
                                    default:
                                        {
                                            Logger.Log("HessianException Status : " + wE.Status.ToString());
                                        }
                                        break;
                                }
                            }

                            foreach (HessianAPI.ExceptionDelegate ed in HessianAPI.ExceptionDelegatorList)
                            {
                                ed(hex);
                            }

                            if (HessianAPI.ExceptionDelegatorList.Count == 0)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception Message : " + ex.Message);

                foreach (HessianAPI.ExceptionDelegate ed in HessianAPI.ExceptionDelegatorList)
                {
                    ed(ex);
                }

                if (HessianAPI.ExceptionDelegatorList.Count == 0)
                {
                    throw ex;
                }
            }
        }
        static public void SaveLog(string sJob, string sJobData)  // nDataType 0 EEv2JobOrder, 
        {
            //try
            //{
            //    string sRootPath = AppDomain.CurrentDomain.BaseDirectory;

            //    string sDirPath = sRootPath + @"{0}\Log\"
            //        + System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day;
            //    if (Directory.Exists(sDirPath) == false)
            //    {
            //        Directory.CreateDirectory(sDirPath);
            //    }

            //    string logFilePath = @sDirPath + "/ITV_LOG_" + System.DateTime.Now.Hour + ".txt";

            //    FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            //    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            //    sw.WriteLine("//===========================================================================");
            //    sw.WriteLine("[" + System.DateTime.Now.ToString() + "]" + sJob);
            //    sw.WriteLine(sJobData);
            //    sw.WriteLine("//===========================================================================\r\n");
            //    sw.Flush();
            //    sw.Close();
            //    fs.Close();
            //}
            //catch (Exception ex)
            //{

            //}
        }
    }
}
