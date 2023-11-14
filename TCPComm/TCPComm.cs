using TCPComm.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Util;
using TCPComm.EEStruct;

namespace TCPComm
{
    public enum TCPCommType_Test
    {
        // Common
        VMT_TU_GetUserAccessRole = 21,
        VMT_TU_SetLogin4Machine = 22,
        VMT_TU_SendMachineStatusChange = 23,
        VMT_TU_SetMachineStop = 62,
        VMT_TU_ManualJobDone = 82,

        // ITV
        VMT_TU_ITV_SetChassis_Attach = 110,

        // RTG
        VMT_TU_RTG_ManualReady = 224,
        VMT_TU_RTG_SendBlockInfo = 240,
        VMT_TU_RTG_SendCorrection = 244,
        VMT_TU_RTG_SetCurrentJob = 260,
        VMT_TU_RTG_HandleJobDone = 261,

        VMT_TU_RTG_Marring = 270,
    }

    public enum TCPCommType
    {
        TCPBypassMessage = -95, // TCP Socket Bypass Message
        TCPSocketOpen = -96, // TCP Socket Open
        TCPSocketConnected = -97, // TCP Socket Connected
        TCPSocketDisconnected = -98, // TCP Socket Disconnected
        TCPParsingFail = -99, // TCP Parsing Fail        

        VMT_TU_SoftwareInfo = 1,
        VMT_TU_ConnectionStatus = 2,
        VMT_TU_MachineNotice = 3,
        VMT_TU_SetupDone = 4, // nopayload
        VMT_TU_GetUserAccessRole = 21,
        VMT_TU_SetLogin4Machine = 22,
        VMT_TU_SendMachineStatusChange = 23,
        //---------------------- Common Sensing
        VMT_TU_ResetDGPS = 40,
        VMT_TU_SendPubx05 = 41,
        VMT_TU_SendPubx06 = 42,
        VMT_TU_SendHighForward = 43,
        VMT_TU_SendHighBackward = 44,
        VMT_TU_RequestCfgekf = 45,
        VMT_TU_SaveDGPSCfg = 46,
        //---------------------- Availiable
        VMT_TU_MaChineStopCodeList = 60,
        VMT_TU_GetMachineStop = 61,
        VMT_TU_SetMachineStop = 62,
        VMT_TU_NotifyMachineStopResult = 63,
        //---------------------- Alarm
        VMT_TU_NotifyAlarm = 65,
        //---------------------- Common Job
        VMT_TU_JobOrder = 80,
        VMT_TU_JobCancel = 81,
        VMT_TU_JobDone = 82,
        VMT_TU_ManualJobDone = 83,
        VMT_TU_JobCancelAll = 84,

        ////////////////////////////////////////////////////////////////////////////////////////
        //---------------------- ITV Only
        VMT_TU_ITV_DGPS_Periodic = 100,
        VMT_TU_ITV_PDS = 101,
        VMT_TU_ITV_SetChassis_Attach = 110,
        VMT_TU_ITV_NotifyChassis_Attach = 118,
        //-----
        VMT_TU_ITV_NofityBlockEnterance = 111,
        VMT_TU_ITV_NotifyCPSAlign = 112,
        //VMT_TU_ITV_NotifyPOW = 113,
        VMT_TU_ITV_SetManualReady = 114,
        VMT_TU_ITV_NotifyManualReady = 115,
        VMT_TU_ITV_SetManualArrival = 116,
        VMT_TU_ITV_NotifyManualArrival = 117,

        ////////////////////////////////////////////////////////////////////////////////////////
        //---------------------- RTG Only
        VMT_TU_RTG_PDS_Periodic = 200,
        VMT_TU_RTG_CPS_Align = 201,
        VMT_TU_RTG_PDS_PickDrop = 202,
        VMT_TU_RTG_RFID = 203,
        VMT_TU_RTG_PDS_PickDropConfirm = 204,
        //**** ITV
        VMT_TU_RTG_NotifyMachinePOW = 220,
        //VMT_TU_RTG_NotifyMachineExit = 221,
        VMT_TU_RTG_NotifyMachineBlockEnter = 222,
        VMT_TU_RTG_NotifyMachineReadyITV = 223,
        VMT_TU_RTG_ManualReady = 224,
        //**** Inven
        VMT_TU_RTG_SendBlockInfo = 240,
        VMT_TU_RTG_NotifyBlockInfo = 241,
        VMT_TU_RTG_SendBlockInfoSimple = 242,
        VMT_TU_RTG_NotifyBlockInfoSimple = 243,
        VMT_TU_RTG_SendCorrection = 244,
        VMT_TU_RTG_NotifyCorrection = 245,
        //**** Job
        VMT_TU_RTG_SetCurrentJob = 260, // Delete
        VMT_TU_RTG_HandleJobDone = 261,
        VMT_TU_RTG_TargetJob = 262,
        VMT_TU_RTG_NotifySetCurrentJob = 264, // Delete
        VMT_TU_RTG_ManualTargetJob = 263,
        //----- Marring
        VMT_TU_RTG_Marrying = 270,
        VMT_TU_RTG_Swap_Result = 271,
        VMT_TU_RTG_Return_Cntr = 272,
        VMT_TU_RTG_NotifyMarring = 273,
        VMT_TU_RTG_OTR_ManualBlockInOut = 280,

        ////////////////////////////////////////////////////////////////////////////////////////
        //------------------------ EH RS Only
        VMT_TU_RSEH_PDS_Periodic = 300,
        VMT_TU_RSEH_PDS_PickDrop = 301,
        VMT_TU_RSEH_PDS_RFID = 302,
        VMT_TU_RSEH_PDS_PickDropConfirm = 303,
        //**** EH RS
        VMT_TU_RSEH_NotifyMachinePOW = 320,
        VMT_TU_RSEH_NotifyMachineBlockEnter = 322,
        VMT_TU_RSEH_NotifyMachineReadyITV = 323,
        VMT_TU_RSEH_ManualReady = 324,
        VMT_TU_RSEH_NotifyManualReady = 325,
        //**** Inven
        VMT_TU_RSEH_SendBlockInfo = 340,
        VMT_TU_RSEH_NotifyBlockInfo = 341,
        VMT_TU_RSEH_SendBlockInfoSimple = 342,
        VMT_TU_RSEH_NotifyBlockInfoSimple = 343,
        VMT_TU_RSEH_SendCorrection = 344,
        VMT_TU_RSEH_NotifyCorrection = 345,
        //**** Job
        VMT_TU_RSEH_SetCurrentJob = 360, // Delete
        VMT_TU_RSEH_HandleJobDone = 361,
        VMT_TU_RSEH_TargetJob = 362,
        VMT_TU_RSEH_ManualTargetJob = 363,
        VMT_TU_RSEH_NotifySetCurrentJob = 364, // Delete
        //----- Marring
        VMT_TU_RSEH_Marring = 370,
        VMT_TU_RSEH_Swap_Result = 371,
        VMT_TU_RSEH_Return_Cntr = 372,
        VMT_TU_RSEH_NotifyMarring = 373,
        VMT_TU_RSEH_OTR_ManualBlockInOut = 380
    }

    public class TCPException : Exception
    {
        public TCPException()
        {
        }

        public TCPException(String message)
            : base(message)
        {
        }

        public TCPException(String message, Exception inner)
            : base(message, inner)
        {
        }
        public TCPCommType tcpCommType { get; set; }
    }

    public class TCPAPI
    {
        public delegate void ExceptionDelegate(Exception ex);
        public static List<ExceptionDelegate> ExceptionDelegatorList = new List<ExceptionDelegate>();

        public static void AddExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Add(ed);
        }

        public static void RemoveExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Remove(ed);
        }

        public delegate void TCPCommCallback(TCPCommType type, Object obj);

        #region variables
        private static TCPAPI _instance = new TCPAPI();

        private String _ip = @"127.0.0.1"; // LoopBack Address
        private int _port = 30000;
        private TCPCommCallback _sendCallback = null;
        private TCPCommCallback _receiveCallback = null;

        private IfTCPControl _tcpControl = null;

        private TCPExecuteThread _thread = null;
        #endregion variables

        #region properties
        public IfTCPControl TCPControl
        {
            get
            {
                if (_tcpControl == null)
                {
                    _tcpControl = new IfTCPControl(_ip, _port, _instance._sendCallback, _instance._receiveCallback);
                }

                return _tcpControl;
            }
        }
        #endregion properties

        private TCPAPI()
        {
            _thread = new TCPExecuteThread(this);
        }

        public static void Init(String ip, int port, TCPCommCallback sendCallback, TCPCommCallback receiveCallback)
        {
            _instance._ip = ip;
            _instance._port = port;
            _instance._sendCallback = sendCallback;
            _instance._receiveCallback = receiveCallback;
            _instance._thread.sendCallback = sendCallback;
            _instance._thread.receiveCallback = receiveCallback;
        }

        public static void Start()
        {
            _instance._thread.Start();
        }

        public static void End()
        {
            _instance._thread.End();
        }

        public static void StartPolling(TCPCommType type, Object obj, int timeSpan)
        {
            _instance._thread.StartPolling(type, obj, timeSpan);
        }

        public static void StopPolling(TCPCommType type)
        {
            _instance._thread.StopPolling(type);
        }

        #region method

        public static void TCPCommExcute(TCPCommType tpcCommType, Object value, Int32 interval)
        {
            if (interval != 0)
                new TCPCommandSingleShot(TCPCommandSingleShotCallbackFunction, (Int32)tpcCommType, value, interval);
            else
                _instance._thread.Do((TCPCommType)tpcCommType, value);
        }

        private static void TCPCommandSingleShotCallbackFunction(Int32 tpcCommType, Object value)
        {
            _instance._thread.Do((TCPCommType)tpcCommType, value);
        }

        // void
        public static void TCPSocketOpen()
        {
            _instance._thread.Do(TCPCommType.TCPSocketOpen, null);
        }

        #region Common
        public static void SoftwareInfo(EEParentClass value) // sTrayUI_SwinfoRQ
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SoftwareInfo, value);
        }

        // void
        public static void ConnectionStatus()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_ConnectionStatus, null);
        }

        public static void MachineNotice()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_MachineNotice, null);
        }
        #endregion Common

        #region Login
        public static void GetUserAccessRole(EEParentClass value) // sTrayUI_GetUserAccessRoleRQ
        {
            _instance._thread.Do(TCPCommType.VMT_TU_GetUserAccessRole, value);
        }

        public static void SetLogin4Machine(EEParentClass value) // sTrayUI_SetLogin4MachineRQ
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SetLogin4Machine, value);
        }

        public static void SendMachineStatusChange(EEParentClass value) // sTrayUI_SendMachineStatusChangeRQ        
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SendMachineStatusChange, value);
        }
        #endregion Login

        #region Common Sensing Device
        public static void ResetDGPS()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_ResetDGPS, null);
        }

        public static void SendPubx05()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SendPubx05, null);
        }

        public static void SendPubx06()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SendPubx06, null);
        }

        public static void SendHighForward()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SendHighForward, null);
        }

        public static void SendHighBackward()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SendHighBackward, null);
        }

        public static void RequestCfgekf()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RequestCfgekf, null);
        }

        public static void SaveDGPSCfg()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SaveDGPSCfg, null);
        }
        #endregion Common Sensing Device

        #region Available
        public static void MaChineStopCodeList()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_MaChineStopCodeList, null);
        }

        public static void GetMachineStop()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_GetMachineStop, null);
        }

        public static void SetMachineStop(EEParentClass value)
        // sTrayUI_SetMachineStop
        // public static void SetMachineStop(sTrayUI_SetMachineStop value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_SetMachineStop, value);
        }

        public static void NotifyMachineStopResult()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_NotifyMachineStopResult, null);
        }
        #endregion Available

        #region Alarm
        public static void NotifyAlarm()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_NotifyAlarm, null);
        }
        #endregion Alarm

        #region Common JobOrder
        public static void JobOrder()
        {
            _instance._thread.Do(TCPCommType.VMT_TU_JobOrder, null);
        }

        public static void JobCancel()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_JobCancel, null);
        }

        public static void JobDone()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_JobDone, null);
        }

        public static void ManualJobDone(Object value)
        // TCHAR jobKey[EEV2_STRING_MAX_JOB_KEY];
        // public static void ManualJobDone(jobKey value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_ManualJobDone, value);
        }

        public static void JobCancelAll()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_JobCancelAll, null);
        }
        #endregion Common JobOrder

        //---------------------------------------
        // ITV Only
        #region Sensing Device
        public static void ITV_DGPS_Periodic()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_DGPS_Periodic, null);
        }

        public static void ITV_PDS()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_PDS, null);
        }
        #endregion Sensing Device

        #region MainView
        public static void ITV_SetChassis_Attach(EEParentClass value) // sTrayUI_ChassisAttachInfo
        // public static void ITV_SetChassis_Attach(sTrayUI_ChassisAttachInfo value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_ITV_SetChassis_Attach, value);
        }

        public static void ITV_NofityBlockEnterance()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_NofityBlockEnterance, null);
        }

        public static void ITV_NotifyCPSAlign()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_NotifyCPSAlign, null);
        }

        public static void ITV_NotifyPOW()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_NotifyPOW, null);
        }

        public static void ITV_SetManualArrival(EEParentClass value) // sTrayUI_SetManualArrivalRQ        
        // public static void ITV_SetChassis_Attach(sTrayUI_ITV_SetManualArrivalRQ value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_ITV_SetManualArrival, value);
        }

        public static void ITV_NotifyManualArrival()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_NotifyManualArrival, null);
        }

        public static void ITV_SetManualReady(EEParentClass value)
        // sTrayUI_ITV_SetManualReadyRQ
        // public static void ITV_SetChassis_Attach(sTrayUI_ITV_SetManualReadyRQ value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_ITV_SetManualReady, value);
        }

        public static void ITV_NotifyManualReady()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_ITV_NotifyManualReady, null);
        }
        #endregion MainView

        //---------------------------------------
        // RTG Only
        #region Sensing Device
        public static void RTG_PDS_Periodic()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_PDS_Periodic, null);
        }

        public static void RTG_CPS_Align()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_CPS_Align, null);
        }

        public static void RTG_PDS_PickDrop()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_PDS_PickDrop, null);
        }

        public static void RTG_RFID()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_RFID, null);
        }
        #endregion Sensing Device

        #region ITV Info
        public static void RTG_NotifyMachinePOW()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyMachinePOW, null);
        }

        //public static void RTG_NotifyMachineExit()
        //{
        //    return;
        //    //_instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyMachineExit, null);
        //}

        public static void RTG_NotifyMachineBlockEnter()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyMachineBlockEnter, null);
        }

        public static void RTG_NotifyMachineReadyITV()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyMachineReadyITV, null);
        }

        public static void RTG_ManualReady(sTrayUI_ManualReadyRTG value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_ManualReady, value);
        }
        #endregion ITV Info

        #region Inventory Correction
        public static void RTG_SendBlockInfo(sTrayUI_BlockInfoRq value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_SendBlockInfo, value);
        }

        public static void RTG_NotifyBlockInfo()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyBlockInfo, null);
        }

        public static void RTG_SendBlockInfoSimple(sTrayUI_BlockInfoRq value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_SendBlockInfoSimple, value);
        }

        public static void RTG_NotifyBlockInfoSimple()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyBlockInfoSimple, null);
        }

        public static void RTG_SendCorrection(sTrayUI_Correction value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_SendCorrection, value);
        }

        public static void RTG_NotifyCorrection()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_NotifyCorrection, null);
        }
        #endregion Inventory Correction

        #region RTG JobOrder
        public static void RTG_ManualTargetJob(sTrayUI_ManualTargetJob value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_ManualTargetJob, value);
        }

        public static void RTG_SetCurrentJob(sTrayUI_SetCurrentJob value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_SetCurrentJob, value);
        }

        public static void RTG_HandleJobDone(sTrayUI_HandleJobDone value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_HandleJobDone, value);
        }

        public static void RTG_PDS_PickDropConfirm(sTrayUI_sRTG_PDS_PickDropConfirm_Payload value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_PDS_PickDropConfirm, value);
        }

        public static void RTG_OTR_ManualBlockInOut(sTrayUI_sRTG_OTR_ManualBlockInOut value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_OTR_ManualBlockInOut, value);
        }

        public static void RTG_TargetJob()
        {
            return;
            // _instance._thread.Do(TCPCommType.VMT_TU_RTG_TargetJob, null);
        }
        #endregion RTG JobOrder

        public static void ECH_PDS_PickDropConfirm(sTrayUI_sRTG_PDS_PickDropConfirm_Payload value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RSEH_PDS_PickDropConfirm, value);
        }

        #region Marrying
        public static void RTG_Marrying(sTrayUI_RTGMarrying value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RTG_Marrying, value);
        }

        public static void ECH_Marrying(sTrayUI_ECHMarrying value)
        {
            _instance._thread.Do(TCPCommType.VMT_TU_RSEH_Marring, value);
        }
        #endregion Marrying
        /*
        ...
        */
        #endregion method
    }
}