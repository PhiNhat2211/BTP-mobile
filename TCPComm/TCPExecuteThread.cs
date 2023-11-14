using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Common.Util;
using System.Net;

using TCPComm.EEStruct;

namespace TCPComm
{
    public class TCPExecuteThread
    {
        // Thread Queue
        private class QueueData
        {
            public TCPCommType type { get; private set; }
            public Object obj { get; private set; }

            public QueueData(TCPCommType type, Object obj)
            {
                this.type = type;
                this.obj = obj;
            }
        }

        Queue<QueueData> _queue = new Queue<QueueData>();
        AutoResetEvent _queueEvent = new AutoResetEvent(false);
        AutoResetEvent _quitEvent = new AutoResetEvent(false);
        // WaitHandle[] _events = new WaitHandle[] { new AutoResetEvent(false), new AutoResetEvent(false) };
        WaitHandle[] _events = null;

        private TCPAPI.TCPCommCallback _sendCallback = null;
        public TCPAPI.TCPCommCallback sendCallback { set { _sendCallback = value; } }

        private TCPAPI.TCPCommCallback _receiveCallback = null;
        public TCPAPI.TCPCommCallback receiveCallback { set { _receiveCallback = value; } }

        private Thread _thread = null;

        private TCPAPI _parent = null;

        public TCPExecuteThread(TCPAPI parent)
        {
            _parent = parent;
            _thread = new Thread(this.ThreadFunc);
            _events = new WaitHandle[] { _quitEvent, _queueEvent };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void End()
        {
            _quitEvent.Set();
        }

        //Timer __queuetimer = null;
        //public void __queueTimerCallback(Object state)
        //{
        //    _queueEvent.Set();
        //}

        //public void StartQueueTimer()
        //{
        //    if (__queuetimer == null)
        //        __queuetimer = new Timer(__queueTimerCallback);

        //    __queuetimer.Change(0, 200);
        //}

        //public void StopQueueTimer()
        //{
        //    if (__queuetimer != null)
        //    {
        //        __queuetimer.Dispose();
        //        __queuetimer = null;
        //    }
        //}

        public void StartPolling(TCPCommType type, Object obj, int timeSpan)
        {
            switch (type)
            {
                //case TCPCommType.Timer:
                //    {
                //        if (_timer == null)
                //            _timer = new Timer(TimerCallback);

                //        _timer.Change(0, timeSpan);
                //    }
                //    break;
                /*
                ....
                             
                             
                             
                */
            }
        }

        public void StopPolling(TCPCommType type)
        {
            switch (type)
            {
                //case TCPCommType.Timer:
                //    if (_timer != null)
                //    {
                //        _timer.Dispose();
                //        _timer = null;
                //    }
                //    break;
                /*
                ....
                             
                             
                             
                */
            }
        }

        public void Do(TCPCommType type, Object obj)
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                bool bEnqueue = true;

                if (type == TCPCommType.VMT_TU_RTG_PDS_PickDropConfirm ||
                    type == TCPCommType.VMT_TU_RSEH_PDS_PickDropConfirm)
                { }
                else
                {
                    for (int i = 0; i < _queue.Count; i++)
                    {
                        if (_queue.ElementAt(i).type == type)
                        {
                            bEnqueue = false;
                            break;
                        }
                    }
                }
                if (bEnqueue)
                {
                    if (obj != null && obj is EEParentClass)
                        (obj as EEParentClass).RelocateByteArray();

                    _queue.Enqueue(new QueueData(type, obj));
                }
            }

            _queueEvent.Set(); // __queuetimer
        }

        private void ThreadFunc(Object obj)
        {
            try
            {
                int ret = 0;
                QueueData data = null;
                while (true)
                {
                    ret = WaitHandle.WaitAny(_events, -1); // 0.2 sec handle get waiting

                    if (ret == 0)    // QuitEvent
                    {
                        break;
                    }
                    else if (ret == 1) // QueueEvent
                    {

                        lock (((ICollection)_queue).SyncRoot)
                        {
                            if (_queue.Count == 0)
                                continue;

                            data = _queue.Dequeue();

                            if (_queue.Count != 0)
                                _queueEvent.Set();
                        }

                        try
                        {
                            Logger.Log("Do Execute : " + data.type.ToString());

                            switch (data.type)
                            {
                                case TCPCommType.TCPBypassMessage:
                                    _sendCallback(TCPCommType.TCPBypassMessage, _parent.TCPControl.SendByPassPacket(data.type, data.obj));
                                    break;
                                case TCPCommType.TCPSocketOpen:
                                    _sendCallback(TCPCommType.TCPSocketOpen, _parent.TCPControl.tcpSocketOpen());
                                    break;
                                case TCPCommType.VMT_TU_SoftwareInfo:
                                case TCPCommType.VMT_TU_ConnectionStatus:
                                case TCPCommType.VMT_TU_MachineNotice:
                                case TCPCommType.VMT_TU_GetUserAccessRole:
                                case TCPCommType.VMT_TU_SetLogin4Machine:
                                case TCPCommType.VMT_TU_SendMachineStatusChange:
                                case TCPCommType.VMT_TU_ResetDGPS:
                                case TCPCommType.VMT_TU_SendPubx05:
                                case TCPCommType.VMT_TU_SendPubx06:
                                case TCPCommType.VMT_TU_SendHighForward:
                                case TCPCommType.VMT_TU_SendHighBackward:
                                case TCPCommType.VMT_TU_RequestCfgekf:
                                case TCPCommType.VMT_TU_SaveDGPSCfg:
                                case TCPCommType.VMT_TU_MaChineStopCodeList:
                                case TCPCommType.VMT_TU_GetMachineStop:
                                case TCPCommType.VMT_TU_SetMachineStop:
                                case TCPCommType.VMT_TU_NotifyMachineStopResult:
                                case TCPCommType.VMT_TU_NotifyAlarm:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                //------------------------ Common Job
                                case TCPCommType.VMT_TU_JobOrder:
                                case TCPCommType.VMT_TU_JobCancel:
                                case TCPCommType.VMT_TU_JobDone:
                                case TCPCommType.VMT_TU_ManualJobDone:
                                case TCPCommType.VMT_TU_JobCancelAll:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                //------------------------ ITV Only
                                case TCPCommType.VMT_TU_ITV_DGPS_Periodic:
                                case TCPCommType.VMT_TU_ITV_PDS:
                                case TCPCommType.VMT_TU_ITV_SetChassis_Attach:
                                case TCPCommType.VMT_TU_ITV_NofityBlockEnterance:
                                case TCPCommType.VMT_TU_ITV_NotifyCPSAlign:
                                //case TCPCommType.VMT_TU_ITV_NotifyPOW:
                                case TCPCommType.VMT_TU_ITV_SetManualReady:
                                case TCPCommType.VMT_TU_ITV_NotifyManualReady:
                                case TCPCommType.VMT_TU_ITV_SetManualArrival:
                                case TCPCommType.VMT_TU_ITV_NotifyManualArrival:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                //------------------------ RTG Only
                                case TCPCommType.VMT_TU_RTG_PDS_Periodic:
                                case TCPCommType.VMT_TU_RTG_CPS_Align:
                                case TCPCommType.VMT_TU_RTG_PDS_PickDrop:
                                case TCPCommType.VMT_TU_RTG_RFID:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                //**** ITV
                                case TCPCommType.VMT_TU_RTG_NotifyMachinePOW:
                                //case TCPCommType.VMT_TU_RTG_NotifyMachineExit:
                                case TCPCommType.VMT_TU_RTG_NotifyMachineBlockEnter:
                                case TCPCommType.VMT_TU_RTG_NotifyMachineReadyITV:
                                case TCPCommType.VMT_TU_RTG_ManualReady:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                //**** Inven
                                case TCPCommType.VMT_TU_RTG_SendBlockInfo:
                                case TCPCommType.VMT_TU_RTG_NotifyBlockInfo:
                                case TCPCommType.VMT_TU_RTG_SendBlockInfoSimple:
                                case TCPCommType.VMT_TU_RTG_NotifyBlockInfoSimple:
                                case TCPCommType.VMT_TU_RTG_SendCorrection:
                                case TCPCommType.VMT_TU_RTG_NotifyCorrection:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                //**** Job
                                case TCPCommType.VMT_TU_RTG_ManualTargetJob:
                                case TCPCommType.VMT_TU_RTG_SetCurrentJob:
                                case TCPCommType.VMT_TU_RTG_HandleJobDone:
                                case TCPCommType.VMT_TU_RTG_PDS_PickDropConfirm:
                                case TCPCommType.VMT_TU_RTG_TargetJob:                                
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                case TCPCommType.VMT_TU_RTG_Marrying:
                                case TCPCommType.VMT_TU_RTG_Swap_Result:
                                case TCPCommType.VMT_TU_RTG_Return_Cntr:
                                case TCPCommType.VMT_TU_RTG_OTR_ManualBlockInOut:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;

                                //**** EH RS
                                case TCPCommType.VMT_TU_RSEH_PDS_Periodic:
                                case TCPCommType.VMT_TU_RSEH_PDS_PickDrop:
                                case TCPCommType.VMT_TU_RSEH_PDS_RFID:
                                case TCPCommType.VMT_TU_RSEH_PDS_PickDropConfirm:
                                case TCPCommType.VMT_TU_RSEH_NotifyMachinePOW:
                                case TCPCommType.VMT_TU_RSEH_NotifyMachineBlockEnter:
                                case TCPCommType.VMT_TU_RSEH_NotifyMachineReadyITV:
                                case TCPCommType.VMT_TU_RSEH_ManualReady:
                                case TCPCommType.VMT_TU_RSEH_NotifyManualReady:
                                //**** Inven
                                case TCPCommType.VMT_TU_RSEH_SendBlockInfo:
                                case TCPCommType.VMT_TU_RSEH_NotifyBlockInfo:
                                case TCPCommType.VMT_TU_RSEH_SendBlockInfoSimple:
                                case TCPCommType.VMT_TU_RSEH_NotifyBlockInfoSimple:
                                case TCPCommType.VMT_TU_RSEH_SendCorrection:
                                case TCPCommType.VMT_TU_RSEH_NotifyCorrection:
                                //**** Job
                                case TCPCommType.VMT_TU_RSEH_SetCurrentJob: // Delete
                                case TCPCommType.VMT_TU_RSEH_HandleJobDone:
                                case TCPCommType.VMT_TU_RSEH_TargetJob:
                                case TCPCommType.VMT_TU_RSEH_ManualTargetJob:
                                case TCPCommType.VMT_TU_RSEH_NotifySetCurrentJob: // Delete
                                //----- Marring
                                case TCPCommType.VMT_TU_RSEH_Marring:
                                case TCPCommType.VMT_TU_RSEH_Swap_Result:
                                case TCPCommType.VMT_TU_RSEH_Return_Cntr:
                                case TCPCommType.VMT_TU_RSEH_NotifyMarring:
                                case TCPCommType.VMT_TU_RSEH_OTR_ManualBlockInOut:
                                    {
                                        _sendCallback(data.type, _parent.TCPControl.SendPacket(data.type, data.obj));
                                    }
                                    break;
                                /*
                                 ....
                             
                             
                             
                                 */
                            }
                        }
                        catch (Exception ex)
                        {
                            TCPException hex = new TCPException(ex.Message, ex.InnerException);
                            hex.tcpCommType = data.type;

                            Logger.Log("TCPException Type : " + hex.tcpCommType.ToString());
                            Logger.Log("TCPException Message : " + hex.Message);

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

                            foreach (TCPAPI.ExceptionDelegate ed in TCPAPI.ExceptionDelegatorList)
                            {
                                ed(hex);
                            }

                            if (TCPAPI.ExceptionDelegatorList.Count == 0)
                                throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception Message : " + ex.Message);

                foreach (TCPAPI.ExceptionDelegate ed in TCPAPI.ExceptionDelegatorList)
                {
                    ed(ex);
                }

                if (TCPAPI.ExceptionDelegatorList.Count == 0)
                    throw ex;
            }
        }
    }
}
