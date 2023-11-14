using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Util;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VMT_Data_JAT2
{
    public class VMT_DataMgr_Common_TCPCallback
    {
        //static public Exception_Alarm_Callback Exception_Alarm_Callback;

        static public void TCPSocketOpen(ref Object obj)
        {
            if (obj is Boolean)
            {
                Boolean bConnect = Convert.ToBoolean(obj);
                if (bConnect)
                {
                    // TCP Socket Open Success
                }
                else
                {
                    // TCP Socket Open Fail & Retry
                    new SingleShot(5000, TCPSocketConnnectionRetry); // 5 sec
                }
            }
        }

        static public void TCPSocketDisconnected(ref Object obj)
        {
            new SingleShot(5000, TCPSocketConnnectionRetry); // 5 sec
        }

        static public void TCPSocketConnnectionRetry()
        {
            //TCPAPI.TCPSocketOpen();
        }

        static public void TCPParsingFail(ref Object obj)
        {
            if (obj is Byte[])
            {
                // decBytes2 = str.Split('-').Select(ch => Convert.ToByte(ch, 16)).ToArray();

                StringBuilder s = new StringBuilder();
                foreach (byte b in obj as Byte[])
                {
                    s.Append(b.ToString("x2"));
                    s.Append(" ");
                }

                String result = s.ToString();

                //-----------------------------------------------------------------------------------------
                //- Write Log
                //
            }
        }

        static private void LogMessage(String strValue)
        {
            strValue = "[VMT_DataMgr_Common_TCPCallback] " + strValue;
            Util.LogMessage(strValue);
        }

        static private void StructToLogMessage(Object obj)
        {
            VMT_DataMgr_Common_TCPCallback.LogMessage("== Rcv StructToLogMessage Start ==");

            String strLog = String.Empty;
            strLog = obj.GetType().ToString();
            FieldInfo[] fieldInfo = obj.GetType().GetFields();
            foreach (FieldInfo f in fieldInfo)
            {
                String strTemp = f.Name;

                Object objTemp = "";
                if (f.GetValue(obj) != null)
                    objTemp = f.GetValue(obj);

                strLog += "\n\t\t" + strTemp + " : " + objTemp.ToString();
            }
            VMT_DataMgr_Common_TCPCallback.LogMessage(strLog);
            VMT_DataMgr_Common_TCPCallback.LogMessage("== Rcv StructToLogMessage End ==");
        }

        //static public void SoftwareInfo_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("SoftwareInfo_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_SwinfoRP value = (sTrayUI_SwinfoRP)Marshal.PtrToStructure(packet.PtrStrem,
        //                                typeof(sTrayUI_SwinfoRP));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_Swinfo_Receive retValue = new Objects.Common.VD_Common_Swinfo_Receive();
        //    retValue.m_iResult = value.m_iResult;
        //    retValue.m_iLoginResult = value.m_iLoginResult;
        //    retValue.m_VMTTray_Version = value.m_VMTTray_Version;
        //    retValue.m_UserID = value.m_UserID;
        //    retValue.m_UserPW = value.m_UserPW;
        //    retValue.m_GroupName = value.m_GroupName;
        //    retValue.m_DriverName = value.m_DriverName;
        //    retValue.m_szSite = value.m_szSite;

        //    if (static_NotifySwinfoRP != null)
        //        static_NotifySwinfoRP(ref retValue);
        //}

        //static public void ConnectionStatus_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("ConnectionStatus_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_ConnectionStatus value = (sTrayUI_ConnectionStatus)Marshal.PtrToStructure(packet.PtrStrem,
        //                                        typeof(sTrayUI_ConnectionStatus));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_ConnectionStatus_Receive retValue = new Objects.Common.VD_Common_ConnectionStatus_Receive();
        //    retValue.m_iEagleEyeStatus = value.m_iEagleEyeStatus;
        //    retValue.m_iGPSStatus = value.m_iGPSStatus;


        //    if (static_NotifyWIFIStatus != null)
        //        static_NotifyWIFIStatus(retValue.m_iEagleEyeStatus);

        //    if (static_NotifyGPSStatus != null)
        //        static_NotifyGPSStatus(retValue.m_iGPSStatus);
        //}

        //static public void MachineNotice_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("MachineNotice_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_MachineNotice value = (sTrayUI_MachineNotice)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_MachineNotice));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive retValue = new Objects.Common.VD_Common_MachineNotice_Receive();
        //    retValue.m_iMessageType = value.m_iMessageType;
        //    retValue.m_strMessage = value.m_strMessage;
        //    retValue.m_strMessage2 = value.m_strMessage2;

        //    if (static_NotifyMachineNotice != null)
        //        static_NotifyMachineNotice(ref retValue);
        //}

        //static public void SetupDone_Rcv(ref Object obj)
        //{
        //    LogMessage("SetupDone_Rcv");

        //    if (static_NotifySetupDone != null)
        //        static_NotifySetupDone(Convert.ToInt32(true));
        //}

        //static public void UserAccesRole_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("UserAccesRole_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_GetUSerAccesRoleRP value = (sTrayUI_GetUSerAccesRoleRP)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_GetUSerAccesRoleRP));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive retValue = new Objects.Common.VD_Common_GetUserAccesRole_Receive();
        //    retValue.bIsOn = value.bIsOn;
        //    retValue.GroupListSeperator = value.GroupListSeperator;
        //    retValue.Notice = value.Notice;

        //    if (static_NotifyAccessRole != null)
        //        static_NotifyAccessRole(ref retValue);
        //}

        //static public void Login4Machine_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("Login4Machine_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_SetLogin4MachineRP value = (sTrayUI_SetLogin4MachineRP)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_SetLogin4MachineRP));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive retValue = new Objects.Common.VD_Common_SetLogin4Machine_Receive();
        //    retValue.iLogin = value.iLogin;
        //    retValue.UserName = value.UserName;
        //    retValue.Notice = value.Notice;

        //    if (static_NotifyLogin4Machine != null)
        //        static_NotifyLogin4Machine(ref retValue);
        //}

        //static public void MachineStatusChange_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("MachineStatusChange_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_ResultRP value = (sTrayUI_ResultRP)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_ResultRP));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive retValue = new Objects.Common.VD_Common_SendMachineStatusChange_Receive();
        //    retValue.m_iResult = value.m_iResult;

        //    if (static_NotifyMachineStatusChanged != null)
        //        static_NotifyMachineStatusChanged(ref retValue);
        //}

        //static public void ResetDGPS_Rcv(ref Object obj) { LogMessage("ResetDGPS_Rcv"); }
        //static public void SendPubx05_Rcv(ref Object obj) { LogMessage("SendPubx05_Rcv"); }
        //static public void SendPubx06_Rcv(ref Object obj) { LogMessage("SendPubx06_Rcv"); }
        //static public void SendHighForward_Rcv(ref Object obj) { LogMessage("SendHighForward_Rcv"); }
        //static public void SendHighBackward_Rcv(ref Object obj) { LogMessage("SendHighBackward_Rcv"); }
        //static public void RequestCfgekf_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("RequestCfgekf_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_RequestCfgekfRP value = (sTrayUI_RequestCfgekfRP)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_RequestCfgekfRP));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive retValue = new Objects.Common.VD_Common_RequestCfgekf_Receive();
        //    retValue.m_iDirection = value.m_iDirection;

        //    if (static_NotifyCFGEKF != null)
        //        static_NotifyCFGEKF(ref retValue);
        //}
        //static public void SaveDGPSCfg_Rcv(ref Object obj) { LogMessage("SaveDGPSCfg_Rcv"); }

        //static public void MachineStopCodeList_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("MachineStopCodeList_Rcv");
        //    StructToLogMessage(packet);

        //    int offset = 0;
        //    Int32 m_iAvailableCount = (Int32)Util.CreateMarshalDynamicLegthClass<Int32>(packet.ByteStream, ref offset, false);
        //    List<sTrayUI_Available> m_pData = (List<sTrayUI_Available>)
        //                                Util.CreateMarshalDynamicLegthClass<sTrayUI_Available>(packet.ByteStream, ref offset, true, m_iAvailableCount);

        //    sTrayUI_MachineStopCodeList value = new sTrayUI_MachineStopCodeList();
        //    value.m_iAvailableCount = m_iAvailableCount;
        //    value.m_pData = m_pData.ToArray();

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive retValue = new Objects.Common.VD_Common_MachineStopCodeList_Receive();
        //    retValue.m_iAvailableCount = value.m_iAvailableCount;

        //    foreach (sTrayUI_Available pData in value.m_pData)
        //    {
        //        VMT_Data_JAT2.Objects.Common.VD_Common_Available available = new Objects.Common.VD_Common_Available();
        //        available.ReasonCd = pData.ReasonCd;
        //        available.ReasonNm = pData.ReasonNm;
        //        retValue.m_pData.Add(available);
        //    }

        //    if (static_NotifyMachineStopCodeList != null)
        //        static_NotifyMachineStopCodeList(ref retValue);
        //}

        //static public void GetMachineStop_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("GetMachineStop_Rcv");
        //    StructToLogMessage(packet);

        //    sTrayUI_GetMachineStop value = (sTrayUI_GetMachineStop)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_GetMachineStop));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive retValue = new Objects.Common.VD_Common_GetMachineStop_Receive();
        //    retValue.Data.ReasonCd = value.Data.ReasonCd;
        //    retValue.Data.ReasonNm = value.Data.ReasonNm;
        //    retValue.m_iBreak = value.m_iBreak;
        //    retValue.StartTime = value.StartTime;
        //    retValue.FinishTime = value.FinishTime;

        //    if (static_NotifyGetMachineStop != null)
        //        static_NotifyGetMachineStop(ref retValue);
        //}

        //static public void SetMachineStop_Rcv(ref EEPacket packet)
        //{
        //    LogMessage("GetMachineStop_Rcv");
        //}

        //static public void NotifyMachineStopResult(ref EEPacket packet)
        //{
        //    LogMessage("NotifyMachineStopResult");
        //    StructToLogMessage(packet);

        //    sTrayUI_NotifyMachineStopResult value = (sTrayUI_NotifyMachineStopResult)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_NotifyMachineStopResult));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive retValue = new Objects.Common.VD_Common_SetMachineStop_Receive();
        //    retValue.Data.ReasonCd = value.Data.ReasonCd;
        //    retValue.Data.ReasonNm = value.Data.ReasonNm;
        //    retValue.m_iBreakStatus = value.m_iBreakStatus;
        //    retValue.m_szMchnID = value.m_szMchnID;
        //    retValue.m_szMchnTp = value.m_szMchnTp;
        //    retValue.m_UserID = value.m_UserID;
        //    retValue.m_DriverName = value.m_DriverName;
        //    retValue.m_iResult = value.m_iResult;


        //    if (static_NotifySetMachineStop != null)
        //        static_NotifySetMachineStop(ref retValue);
        //}

        //static public void NotifyAlarm(ref EEPacket packet)
        //{
        //    LogMessage("NotifyAlarm");
        //    StructToLogMessage(packet);

        //    sTrayUI_Alram value = (sTrayUI_Alram)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_Alram));

        //    StructToLogMessage(value);


        //    VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive retValue = new Objects.Common.VD_Common_Alram_Receive();
        //    retValue.enType = (VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive.enVmtAlram)value.enType;
        //    retValue.nVaildTime = value.nVaildTime;
        //    retValue.nValue = value.nValue;
        //    retValue.szDesc = value.szDesc;

        //    if (static_NotifyAlarm != null)
        //        static_NotifyAlarm(ref retValue);
        //}


        ////------------------------
        ////- Exception Alarm
        //static public void ExceptionAlarm(ref EEPacket packet)
        //{
        //    LogMessage("ExceptionAlarm");
        //    StructToLogMessage(packet);

        //    sTrayUI_Alram value = (sTrayUI_Alram)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_Alram));
        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_Exception_Receive retValue = new Objects.Common.VD_Common_Exception_Receive();
        //    //retValue.enType = (VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive.enVmtAlram)value.enType;

        //    VMT_Data_JAT2.BreakCode.AlarmCodesMgr alarmCode = new BreakCode.AlarmCodesMgr();
        //    alarmCode.NotifyAlarm(retValue);

        //    //if (static_NotifyAlarm != null)
        //    //    static_NotifyAlarm(ref retValue);
        //}

        ////------------------------
        ////- Common Job
        //static public void JobCancel(ref EEPacket packet)
        //{
        //    LogMessage("JobCancel");
        //    StructToLogMessage(packet);

        //    sTrayUI_JobKey value = (sTrayUI_JobKey)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_JobKey));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_JobKey retValue = new Objects.Common.VD_Common_JobKey();
        //    retValue.jobKey = value.jobKey;

        //    if (static_NotifyJobDelete != null)
        //        static_NotifyJobDelete(ref retValue);
        //}

        //static public void JobDone(ref EEPacket packet)
        //{
        //    LogMessage("JobDone");
        //    StructToLogMessage(packet);

        //    sTrayUI_JobDone value = (sTrayUI_JobDone)Marshal.PtrToStructure(packet.PtrStrem,
        //                                                        typeof(sTrayUI_JobDone));

        //    StructToLogMessage(value);

        //    VMT_Data_JAT2.Objects.Common.VD_Common_JobDone retValue = new Objects.Common.VD_Common_JobDone();
        //    retValue.jobKey = value.jobKey;

        //    if (static_NotifyJobDone != null)
        //        static_NotifyJobDone(ref retValue);
        //}

        //static public void ManualJobDone(ref EEPacket packet)
        //{
        //    LogMessage("ManualJobDone");
        //}

        //static public void JobCancelAll(ref Object obj)
        //{
        //    LogMessage("JobCancelAll");

        //    if (static_NotifyJobDeleteAll != null)
        //        static_NotifyJobDeleteAll(Convert.ToInt32(true));
        //}


        ////============================================================
        ////=
        ////= Aggrigate Exception in HessianAPI
        ////=
        ////============================================================
        //static public void ExceptionDelete_VMT_Data(Exception ex)
        //{
        //    if (ex is TCPException)
        //    {
        //        TCPException tEx = (TCPException)ex;

        //        if (tEx.InnerException is WebException)
        //        {
        //            WebException wE = tEx.InnerException as WebException;

        //            switch (wE.Status)
        //            {
        //                case WebExceptionStatus.ConnectFailure:
        //                case WebExceptionStatus.Timeout:
        //                    {

        //                    }
        //                    break;
        //                case WebExceptionStatus.ConnectionClosed:
        //                case WebExceptionStatus.ServerProtocolViolation:
        //                case WebExceptionStatus.UnknownError:
        //                    {
        //                        // bReConnect = true;
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        switch (tEx.tcpCommType)
        //        {
        //            case TCPCommType.TCPSocketOpen: // TCP Open Fail
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
    }
}
