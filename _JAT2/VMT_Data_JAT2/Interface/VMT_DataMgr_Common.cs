using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HessianComm;
//using TCPComm;
using Common.Util;
//using TCPComm.EEStruct;
using System.Runtime.InteropServices;
using System.Collections;
using System.Net;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.IO;
using VMT_Data_JAT2.Objects;
using System.Reflection;
using System.ComponentModel;
using HessianComm.Objects;
using UDPComm;
using WPSocketComm.Network;
using System.Diagnostics;

namespace VMT_Data_JAT2
{
    public class VMT_DataMgr_Common
    {
        static public String gHessianServerIP = "";
        static public String gHessianServerPort = "";
        static public String gTCPLocalIP = "127.0.0.1"; // Local Loopback Address
        //static public String gTCPLocalIP = "116.127.224.233";
        static public String gTCPLocalPort = "";
        static public String gTCPBypassPort = "";

        static private void LogMessage(String strValue)
        {
            strValue = "[VMT_DataMgr_Common] " + strValue;
            Util.LogMessage(strValue);
        }

        static private void StructToLogMessage(Object obj)
        {
            VMT_DataMgr_Common.LogMessage("== Snd StructToLogMessage Start ==");

            String strLog = String.Empty;
            strLog = obj.GetType().ToString();
            FieldInfo[] fieldInfo = obj.GetType().GetFields();
            foreach (FieldInfo f in fieldInfo)
            {
                String strTemp = f.Name;

                Object objTemp = "";
                if (f.GetValue(obj) != null)
                    objTemp = f.GetValue(obj);

                strLog += "\n\t\t-> " + strTemp + " : " + objTemp.ToString();
            }
            VMT_DataMgr_Common.LogMessage(strLog);
            VMT_DataMgr_Common.LogMessage("== Snd StructToLogMessage End ==");
        }

        //-----------------------------------------------------------------------------
        //- Method
        static public void SoftwareInfo_Ask(ref VMT_Data_JAT2.Objects.Common.VD_Common_Swinfo_Send value)
        {
            LogMessage("SoftwareInfo_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_SwinfoRQ swinfoRQ = new sTrayUI_SwinfoRQ();
                //swinfoRQ.m_VMTUI_Version = value.m_VMTUI_Version;
                //swinfoRQ.m_szMchnID = value.m_szMchnID;
                //swinfoRQ.m_szMchnTp = value.m_szMchnTp;

                //TCPComm.TCPAPI.SoftwareInfo(swinfoRQ);
            }
        }

        static public void ConnectionStatus_Ask()
        {
            LogMessage("ConnectionStatus_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.ConnectionStatus();
            }
        }

        static public void GetMchnID_Ask(ref String pValue)
        {
            pValue = UserInfo.gMchnID;
        }

        static public void GetMchnType_Ask(ref String pValue)
        {
            pValue = UserInfo.gMchnTp;
        }

        static public void JobOrderList_Ask()
        {
            LogMessage("JobOrderList_Ask");

            if (UserInfo.IsUseHessian)
            {
                //Machine machine = new Machine();
                //machine.mchnId = UserInfo.gMchnID;
                //machine.mchnTp = UserInfo.gMchnTp;
                //HessianComm.HessianAPI.GetJobOrderList(machine);

                // data downsizing
                var list = new hessiancsharp.Class.HessianList();
                list.Add(UserInfo.gMchnTp);
                list.Add(UserInfo.gMchnID);                
                HessianComm.HessianAPI.GetJobOrderList_New(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        static public void JobOrderListByKeys_Ask(Boolean useMchnId, String blckBay)
        {
            LogMessage("JobOrderListByKeys_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                list.Add(useMchnId == true ? UserInfo.gMchnID : "");
                list.Add("");
                list.Add(blckBay);

                HessianComm.HessianAPI.GetJobOrderListByKeys_New(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        static public void JobOrderListByTruck_Ask(Boolean useMchnId, String truckNo)
        {
            LogMessage("JobOrderListByTruck_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                list.Add(useMchnId == true ? UserInfo.gMchnID : "");
                list.Add(truckNo);
                list.Add("");

                HessianComm.HessianAPI.GetJobOrderListByTruck_New(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        static public void MachineJobByKeys_Ask(Boolean useMchnId, String blckBay, Boolean needCompleteJob, Boolean needRecentUpdated = false)
        {
            LogMessage("MachineJobByKeys_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                list.Add(useMchnId == true ? UserInfo.gMchnID : "");
                list.Add(UserInfo.gMchnTp);
                list.Add("");
                list.Add(blckBay);
                list.Add(needCompleteJob);
                list.Add(needRecentUpdated);

                HessianComm.HessianAPI.GetMachineJobByKeys_New(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        static public void GetMachineJobByKeys_Sync_Ask()
        {
            LogMessage("GetMachineJobByKeys_Sync_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                var isTruck = (UserInfo.MchnTp.MchnTp_ITV == UserInfo.GetMchnTp());

                if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.Common.BlckVal))
                {
                    list.Add(UserInfo.gMchnID);    // Yard Crane
                    list.Add(UserInfo.gMchnTp);    // Machine Type
                    list.Add("");    // Vehicle No
                    list.Add(VMT_Data_JAT2.Objects.Common.BlckVal);                                           // blckBay
                    list.Add(false);                                        // include Completed Job
                    list.Add(true);                                        // isExpire
                }
                else
                {
                    list.Add(isTruck ? String.Empty : UserInfo.gMchnID);    // Yard Crane
                    list.Add(UserInfo.gMchnTp);    // Machine Type
                    list.Add(isTruck ? UserInfo.gMchnID : String.Empty);    // Vehicle No
                    list.Add("");                                           // blckBay
                    if ("SC".Equals(UserInfo.gMchnTp) || UserInfo.MchnTp.MchnTp_ITV == UserInfo.GetMchnTp())
                        list.Add(true); // include Completed Job
                    else
                        list.Add(false);
                    list.Add(false);                                        // isExpire
                }

                HessianComm.HessianAPI.GetMachineJobByKeys_Sync(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        static public void MachineJobByKeysITV_Ask()
        {
            LogMessage("MachineJobByKeys_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();

                list.Add("");
                list.Add(UserInfo.gMchnTp);
                list.Add(UserInfo.gMchnID);
                list.Add("");
                list.Add(true);
                list.Add(false);

                HessianComm.HessianAPI.GetMachineJobByKeys_New(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        static public void MachineJobByTruck_Ask(Boolean useMchnId, String truckNo, Boolean needCompleteJob)
        {
            LogMessage("MachineJobByTruck_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                list.Add("");
                list.Add("YT");    // Machine Type
                list.Add(truckNo);
                list.Add("");
                list.Add(needCompleteJob);
                list.Add(false);

                HessianComm.HessianAPI.GetMachineJobByTruck_New(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.JobOrder();
            }
        }

        // 드라이버 그룹을 가져 옵니다.
        static public void UserAccesRole_Ask(ref VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send value)
        {
            LogMessage("UserAccesRole_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.User user = new HessianComm.Objects.User();
                user.usrId = value.UserID;
                user.mchnTp = UserInfo.gMchnTp;

                HessianComm.HessianAPI.GetUserAccessRole(user);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_GetUSerAccesRoleRQ getUserAccesRoleRQ = new TCPComm.EEStruct.sTrayUI_GetUSerAccesRoleRQ();
                //getUserAccesRoleRQ.UserID = value.UserID;

                //TCPComm.TCPAPI.GetUserAccessRole(getUserAccesRoleRQ);
            }
        }

        // 로그인 정보를 서버로 전송합니다.
        static public void Login4Machine_Ask(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value)
        {
            LogMessage("Login4Machine_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                UserInfo.gUserID = value.UserID;
                UserInfo.gUserPW = value.UserPW;

                Assembly assembly = Assembly.GetEntryAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                String verString = fvi.FileVersion;
                String[] verArr = fvi.FileVersion.Split('.');

                HessianComm.Objects.Login login = new HessianComm.Objects.Login();

                login.version = verArr.Length > 2 ? verArr[2] : String.Empty;

                login.user = new HessianComm.Objects.User();
                login.user.usrId = value.UserID;
                login.user.usrPasswd = value.UserPW;

                login.user.usrGrp = new ArrayList();
                login.user.usrGrp.Add(value.GroupName);

                login.machine = new HessianComm.Objects.Machine();
                login.machine.mchnId = UserInfo.gMchnID;
                login.machine.mchnTp = UserInfo.gMchnTp;
                login.machine.chssNo = value.chssNo;

                HessianComm.HessianAPI.SetLogin4Machine(login);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_SetLogin4MachineRQ setLogin4MachineRQ = new TCPComm.EEStruct.sTrayUI_SetLogin4MachineRQ();
                //setLogin4MachineRQ.UserID = value.UserID;
                //setLogin4MachineRQ.UserPW = value.UserPW;
                //setLogin4MachineRQ.GroupName = value.GroupName;
                //setLogin4MachineRQ.MchnID = UserInfo.gMchnID;
                //setLogin4MachineRQ.MchnTp = UserInfo.gMchnTp;

                //TCPComm.TCPAPI.SetLogin4Machine(setLogin4MachineRQ);
            }
        }

        static public void GetConfigValue_Ask(String value)
        {
            LogMessage("GetConfigValue_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetConfigValue(value);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_SetLogin4MachineRQ setLogin4MachineRQ = new TCPComm.EEStruct.sTrayUI_SetLogin4MachineRQ();
                //setLogin4MachineRQ.UserID = value.UserID;
                //setLogin4MachineRQ.UserPW = value.UserPW;
                //setLogin4MachineRQ.GroupName = value.GroupName;
                //setLogin4MachineRQ.MchnID = UserInfo.gMchnID;
                //setLogin4MachineRQ.MchnTp = UserInfo.gMchnTp;

                //TCPComm.TCPAPI.SetLogin4Machine(setLogin4MachineRQ);
            }
        }
        static public void ChangeDriverCheck_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value)
        {
            LogMessage("ChangeDriverCheck_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {               
                HessianComm.Objects.Login login = new HessianComm.Objects.Login();
                login.user = new HessianComm.Objects.User();
                login.user.usrId = value.UserID;
                login.user.usrPasswd = value.UserPW;

                login.user.usrGrp = new ArrayList();
                login.user.usrGrp.Add(value.GroupName);

                login.machine = new HessianComm.Objects.Machine();
                login.machine.mchnId = UserInfo.gMchnID;
                login.machine.mchnTp = UserInfo.gMchnTp;
                login.machine.chssNo = value.chssNo;

                HessianComm.HessianAPI.ChangeDriverCheck(login);
            }           
        }
        static public void GetMachineStatusChanged_Ask(String jobKey)
        {
            LogMessage("GetMachineStatusChanged_Ask");

            if (UserInfo.IsUseHessian)
            {     
                var list = new hessiancsharp.Class.HessianList();
                list.Add(UserInfo.gMchnID);    // Yard Crane
                list.Add(UserInfo.gMchnTp);    // Machine Type
                list.Add(UserInfo.gUserID);

                list.Add(jobKey);

                HessianComm.HessianAPI.GetMachineStatusChanged(list);
            }
        }
        // 차량의 상태 정보 On / Off를 전송합니다.
        static public void SendMachineStatusChanged_Ask(ref VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send value)
        {
            LogMessage("SendMachineStatusChanged_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.Machine machine = new HessianComm.Objects.Machine();
                machine.mchnId = UserInfo.gMchnID;
                machine.externalId = value.m_UserID;
                machine.isOn = value.m_bisON;
                machine.useRemark = value.m_buseRemark;
                machine.remark = value.m_remark;

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string ver = fvi.FileVersion;
                string[] vn = ver.Split('.');

               machine.noticeMsg = vn[0] + "." + vn[1] + "." + vn[2];

                HessianComm.HessianAPI.SetMachineStatusChanged(machine);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_SendMachineStatusChangeRQ sendMachineStatusChangeRQ = new TCPComm.EEStruct.sTrayUI_SendMachineStatusChangeRQ();
                //sendMachineStatusChangeRQ.m_MchnID = value.m_MchnID;
                //sendMachineStatusChangeRQ.m_bisON = value.m_bisON;

                //TCPComm.TCPAPI.SendMachineStatusChange(sendMachineStatusChangeRQ);
            }
        }

        // 시스템 종료 신호를 서버로 전송합니다.
        static public void SendSystemOff_Ask(ref VMT_Data_JAT2.Objects.Common.VD_Common_ShutDown value, string chssNo = "", bool reset = false)
        {
            LogMessage("SendSystemOff_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                EndPolling_Ask(HessianComm.HessianCommType.GetMachineNotice);
                EndPolling_Ask(HessianComm.HessianCommType.GetMachineJobByKeys_New);
                EndPolling_Ask(HessianComm.HessianCommType.GetPrecedingYtList);
                //EndPolling_Ask(HessianComm.HessianCommType.GetMachineList4LogoutCheck);
                EndPolling_Ask(HessianComm.HessianCommType.GetMachineStop);
                EndPolling_Ask(HessianComm.HessianCommType.GetMachineStatusChanged);
                EndPolling_Ask(HessianComm.HessianCommType.SetVMTMachineStatus);
                if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT"))
                {
                    EndPolling_Ask(HessianComm.HessianCommType.CheckPLCData);
                    EndPolling_Ask(HessianComm.HessianCommType.CheckPLCTwistLock);
                }

                HessianComm.Objects.LogOut logOut = new HessianComm.Objects.LogOut();
                logOut.user = new HessianComm.Objects.User();
                logOut.user.usrId = UserInfo.gUserID;
                logOut.machine = new HessianComm.Objects.Machine();
                logOut.machine.mchnId = UserInfo.gMchnID;
                logOut.machine.mchnTp = UserInfo.gMchnTp;
                logOut.machine.chssNo = chssNo;
                if (reset)
                {
                    logOut.logoutCheck = "F";
                }

                if (!"CLT".Equals(UserInfo.gUserID) && !"ACCESS".Equals(UserInfo.gUserPW) && !String.Empty.Equals(UserInfo.gUserID))
                    HessianComm.HessianAPI.SetLogout4Machine(logOut);

                if ("CLT".Equals(UserInfo.gUserID))
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive sendMachineStatusChang = new VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive();
                    sendMachineStatusChang.m_iResult = 1;
                    VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged(ref sendMachineStatusChang);
                }
                else
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send sendMachineStatusChange_Send = new Objects.Common.VD_Common_SendMachineStatusChange_Send();
                    sendMachineStatusChange_Send.m_MchnID = UserInfo.gMchnID;
                    sendMachineStatusChange_Send.m_UserID = UserInfo.gUserID;
                    sendMachineStatusChange_Send.m_bisON = false;
                    sendMachineStatusChange_Send.m_buseRemark = false;
                    sendMachineStatusChange_Send.m_remark = String.Empty;

                    SendMachineStatusChanged_Ask(ref sendMachineStatusChange_Send);
                }            
            }

            if (UserInfo.IsUseTCP)
            {
                //VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Send sendMachineStatusChange_Send = new Objects.Common.VD_Common_SendMachineStatusChange_Send();
                //sendMachineStatusChange_Send.m_MchnID = UserInfo.gMchnID;
                //sendMachineStatusChange_Send.m_bisON = false;

                //SendMachineStatusChanged_Ask(ref sendMachineStatusChange_Send);
            }
        }

        static public void ResetDGPS_Ask()
        {
            LogMessage("ResetDGPS_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.ResetDGPS();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Send_CFG_RST();
            }
        }

        static public void SendPubx05_Ask()
        {
            LogMessage("SendPubx05_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.SendPubx05();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Send_PUBX_05();
            }
        }

        static public void SendPubx06_Ask()
        {
            LogMessage("SendPubx06_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.SendPubx06();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Send_PUBX_06();
            }
        }

        static public void SendHighForward_Ask()
        {
            LogMessage("SendHighForward_Ask");

            if (UserInfo.IsUseHessian)
            {

            }
            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.SendHighForward();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Send_HighForward();
            }
        }

        static public void SendHighBackward_Ask()
        {
            LogMessage("SendHighBackward_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.SendHighBackward();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Send_HighBackward();
            }
        }

        static public void SaveDGPSCfg_Ask()
        {
            LogMessage("SaveDGPSCfg_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.SaveDGPSCfg();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Send_CFG_SAVE();
            }
        }

        static public void RequestCfgekf_Ask()
        {
            LogMessage("RequestCfgekf_Ask");

            if (UserInfo.IsUseHessian)
            {

            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.RequestCfgekf();
            }

            if (UserInfo.IsUseUDP)
            {
                UDPComm.UDPAPI.Request_CFG_EKF();
            }
        }

        // Job Order List를 Polling Start한다
        static public void StartPolling_Ask(HessianComm.HessianCommType type)
        {
            if (!UserInfo.IsUseHessian)
                return;

            Object obj = null;
            int timeSpan = 5;

            switch (type)
            {
                case HessianComm.HessianCommType.KeepAlive:                                        
                case HessianComm.HessianCommType.KeepAliveStandAlone:                                        
                case HessianComm.HessianCommType.DGPSAlive:
                    break;
                case HessianComm.HessianCommType.SetPinningStation:
                    timeSpan = 3;
                    break;
                case HessianComm.HessianCommType.GetJobOrderList:
                    {
                        var machine = new HessianComm.Objects.Machine();
                        machine.mchnId = UserInfo.gMchnID;
                        machine.mchnTp = UserInfo.gMchnTp;
                        obj = machine;                        

                        timeSpan = 10;
                    }
                    break;
                case HessianComm.HessianCommType.GetJobOrderList_New:
                    {                        
                        // data downsizing
                        var list = new hessiancsharp.Class.HessianList();
                        list.Add(UserInfo.gMchnTp);
                        list.Add(UserInfo.gMchnID);
                        obj = list;

                        timeSpan = 10;
                    }
                    break;
                case HessianComm.HessianCommType.GetMachineJobByKeys_New:
                    {
                        var list = new hessiancsharp.Class.HessianList();
                        var isTruck = (UserInfo.MchnTp.MchnTp_ITV == UserInfo.GetMchnTp());

                        timeSpan = 4; //10; //20200720 change refresh step time
                        if (!String.IsNullOrEmpty(VMT_Data_JAT2.Objects.Common.BlckVal)) {
                            list.Add(UserInfo.gMchnID);    // Yard Crane
                            list.Add(UserInfo.gMchnTp);    // Machine Type
                            list.Add("");    // Vehicle No
                            list.Add(VMT_Data_JAT2.Objects.Common.BlckVal);                                           // blckBay
                            list.Add(false);                                        // include Completed Job
                            list.Add(true);                                        // isExpire
                        }
                        else
                        {
                            list.Add(isTruck ? String.Empty : UserInfo.gMchnID);    // Yard Crane
                            list.Add(UserInfo.gMchnTp);    // Machine Type
                            list.Add(isTruck ? UserInfo.gMchnID : String.Empty);    // Vehicle No
                            list.Add("");                                           // blckBay
                            if ("SC".Equals(UserInfo.gMchnTp) || UserInfo.MchnTp.MchnTp_ITV == UserInfo.GetMchnTp())
                            {
                                timeSpan = 5;
                                list.Add(true); // include Completed Job
                            }
                            else
                                list.Add(false);                                        
                            list.Add(false);                                        // isExpire
                        }
                        
                        obj = list;

                    }
                    break;
                case HessianComm.HessianCommType.GetMachineStatusChanged:
                    {
                        var list = new hessiancsharp.Class.HessianList();
                        list.Add(UserInfo.gMchnID);    // Yard Crane
                        list.Add(UserInfo.gMchnTp);    // Machine Type
                        list.Add(UserInfo.gUserID);
                        if (UserInfo.MchnTp.MchnTp_ITV == UserInfo.GetMchnTp())
                        {
                            list.Add(VMT_Data_JAT2.Objects.ITV.ITV_User.jobKey);
                        }
                        else
                        {
                            var currentJob = (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count > 0 ) ? (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0] : null;
                            list.Add(currentJob == null ? "" : currentJob.jobKey);
                        }

                        obj = list;
                        timeSpan = 5;
                    }
                    break;
                case HessianComm.HessianCommType.GetMachineNotice:
                    {
                        var machine = new HessianComm.Objects.Machine();
                        machine.mchnId = UserInfo.gMchnID;
                        machine.mchnTp = UserInfo.gMchnTp;

                        obj = machine;
                        timeSpan = 10;
                    }                    
                    break;
                case HessianComm.HessianCommType.GetPrecedingYtList:
                    {
                        var currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

                        var stsTask = new HessianComm.Objects.StsTask();
                        stsTask.cntr = new HessianComm.Objects.Container();
                        stsTask.cntr.cntrNo = currentJob.cntr.cntrNo;
                        stsTask.vsl = new HessianComm.Objects.Vessel();
                        stsTask.vsl.vessel = currentJob.type.vslCd;
                        stsTask.vsl.voyage = currentJob.type.voyNo;

                        obj = stsTask;
                        timeSpan = 9;
                    }
                    break;
                case HessianComm.HessianCommType.GetMachineList4LogoutCheck:
                    {
                        var machine = new HessianComm.Objects.Machine();
                        machine.mchnId = UserInfo.gMchnID;
                        machine.mchnTp = UserInfo.gMchnTp;

                        obj = machine;
                        timeSpan = 10;
                    }
                    break;
                case HessianComm.HessianCommType.GetMachineStop:
                    {
                        var machine = new HessianComm.Objects.Machine();
                        machine.mchnId = UserInfo.gMchnID;
                        machine.mchnTp = UserInfo.gMchnTp;
                        
                        obj = machine;
                        timeSpan = 10;
                    }
                    break;
                case HessianComm.HessianCommType.CheckPLCData:
                    {
                        var list = new hessiancsharp.Class.HessianList();
                        list.Add(UserInfo.gMchnID);
                        
                        obj = list;
                        timeSpan = 1;
                    }
                    break;
                case HessianComm.HessianCommType.CheckPLCTwistLock:
                    {
                        var list = new hessiancsharp.Class.HessianList();
                        list.Add(UserInfo.gMchnID);
                        list.Add("");

                        obj = list;
                        timeSpan = 2;
                    }
                    break;
                case HessianComm.HessianCommType.GetChangedMachineLocation:
                    {
                        var list = new hessiancsharp.Class.HessianList();
                        list.Add(UserInfo.gMchnID);
                        list.Add(UserInfo.gMchnTp);
                        list.Add("");
                        list.Add("");

                        obj = list;

                        timeSpan = 4;
                    }
                    break;
                case HessianComm.HessianCommType.SetVMTMachineStatus:
                    {
                        var list = new hessiancsharp.Class.HessianList();
                        list.Add(UserInfo.gUserID);
                        list.Add(UserInfo.gMchnID);

                        obj = list;

                        timeSpan = 5;
                    }
                    break;
                default:
                    return;
            }

            HessianComm.HessianAPI.StartPolling(type, obj, timeSpan * 1000);            
        }
        static public void StartPollingPriority_Ask(HessianComm.HessianCommType type)
        {
            if (!UserInfo.IsUseHessian)
                return;

            Object obj = null;
            int timeSpan = 5;

            switch (type)
            {
                case HessianComm.HessianCommType.KeepAlive:
                    break;
                default:
                    return;
            }

            HessianComm.HessianAPI.StartPollingPriority(type, obj, timeSpan * 1000);
        }
        // Job Order List를 Polling End한다
        static public void EndPolling_Ask(HessianComm.HessianCommType type)
        {
            if (!UserInfo.IsUseHessian)
                return;

            HessianComm.HessianAPI.StopPolling(type);
        }
        static public void EndPollingPriority_Ask(HessianComm.HessianCommType type)
        {
            if (!UserInfo.IsUseHessian)
                return;

            HessianComm.HessianAPI.StopPollingPriority(type);
        }
        static public void KeepAlive_Ask()
        {
            if (UserInfo.IsUseHessian)
                HessianComm.HessianAPI.KeepAlive();
        }
        static public void KeepAlivePriority_Ask()
        {
            if (UserInfo.IsUseHessian)
                HessianComm.HessianAPI.KeepAlivePriority();
        }
        static public void getSwapList_Ask(String jobKey, String blckNm, bool filterBlck)
        {
            LogMessage("getSwapList_Ask");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                list.Add(jobKey);
                list.Add(filterBlck ? blckNm : "");

                HessianComm.HessianAPI.getSwapList(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.MaChineStopCodeList();
            }
        }

        static public void setEmptySwap(String jobKey, String swapCntrNo, String swapCntrPnt, String regoNo)
        {
            LogMessage("setEmptySwap");

            if (UserInfo.IsUseHessian)
            {
                var list = new hessiancsharp.Class.HessianList();
                list.Add(jobKey);
                list.Add(swapCntrNo);
                list.Add(swapCntrPnt);
                list.Add(regoNo);
                HessianComm.HessianAPI.setEmptySwap(list);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.MaChineStopCodeList();
            }
        }

        // Available Code를 Tos에서 받아 온다
        static public void GetMachineStopCodeList_Ask()
        {
            LogMessage("GetMachineStopCodeList_Ask");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetMachineStopCodeList(UserInfo.gMchnTp);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.MaChineStopCodeList();
            }
        }

        static public void GetMachineAccessAction_Ask(String usrId, String mchnId)
        {
            LogMessage("GetMachineAccessAction_Ask");

            if (UserInfo.IsUseHessian)
            {
                var accessInfo = new hessiancsharp.Class.HessianList();
                accessInfo.Add(usrId);
                accessInfo.Add(mchnId);
                HessianComm.HessianAPI.GetMachineAccessAction(accessInfo);
            }
        }

        static public void GetBlockListForYardSector_Ask(String mchnId, String blockName)
        {
            LogMessage("GetBlockListForYardSector_Ask");

            if (UserInfo.IsUseHessian)
            {
                var blockInfo = new hessiancsharp.Class.HessianList();
                blockInfo.Add(mchnId);
                blockInfo.Add(blockName);
                HessianComm.HessianAPI.GetBlockListForYardSector(blockInfo);
            }
        }

        // Available 상태인지 확인한다
        static public void GetMachineStop_Ask()
        {
            LogMessage("GetMachineStop_Ask");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.Machine machine = new HessianComm.Objects.Machine();
                machine.mchnId = UserInfo.gMchnID;
                machine.mchnTp = UserInfo.gMchnTp;

                HessianComm.HessianAPI.GetMachineStop(machine);
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.TCPAPI.GetMachineStop();
            }
        }

        // Machine Notice Message를 초기화한다
        static public void SetMachineNotice()
        {
            LogMessage("SetMachineNotice");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.Machine machine = new HessianComm.Objects.Machine();
                machine.mchnId = UserInfo.gMchnID;
                machine.mchnTp = UserInfo.gMchnTp;
                machine.noticeMsg = "";

                HessianComm.HessianAPI.SetMachineNotice(machine);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        // Available 정보를 서버로 전송
        static public void SetMachineStop_Ask(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send value)
        {
            LogMessage("SetMachineStop_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.MachineStop machineStop = new HessianComm.Objects.MachineStop();
                Boolean bStart = true;
                if (value.m_iBreakStatus == 1)
                    bStart = true;
                else
                    bStart = false;

                if (bStart)
                {
                    // Start
                    machineStop.mchn = new HessianComm.Objects.Machine();
                    machineStop.mchn.mchnId = UserInfo.gMchnID;
                    machineStop.mchn.mchnTp = UserInfo.gMchnTp;
                    machineStop.user = new HessianComm.Objects.User();
                    machineStop.user.usrId = UserInfo.gUserID;
                    machineStop.user.usrNm = UserInfo.gUserNm;
                    machineStop.reasonCd = value.Data.ReasonCd;
                    machineStop.reasonNm = value.Data.ReasonNm;
                    machineStop.isStop = true;
                    machineStop.approvalCd = "A";

                    HessianComm.HessianAPI.SetMachineStop(machineStop);
                }
                else
                {
                    // End
                    machineStop.mchn = new HessianComm.Objects.Machine();
                    machineStop.mchn.mchnId = UserInfo.gMchnID;
                    machineStop.mchn.mchnTp = UserInfo.gMchnTp;
                    machineStop.user = new HessianComm.Objects.User();
                    machineStop.user.usrId = UserInfo.gUserID;
                    machineStop.user.usrNm = UserInfo.gUserNm;
                    machineStop.reasonCd = value.Data.ReasonCd;
                    machineStop.reasonNm = value.Data.ReasonNm;
                    machineStop.isStop = false;
                    machineStop.approvalCd = "A";

                    HessianComm.HessianAPI.SetMachineStop(machineStop);
                }
            }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_SetMachineStop setMachineStop = new TCPComm.EEStruct.sTrayUI_SetMachineStop();
                //setMachineStop.Data = new sTrayUI_Available();
                //setMachineStop.Data.ReasonCd = value.Data.ReasonCd;
                //setMachineStop.Data.ReasonNm = value.Data.ReasonNm;
                //setMachineStop.m_iBreakStatus = value.m_iBreakStatus;
                //setMachineStop.m_szMchnID = UserInfo.gMchnID;
                //setMachineStop.m_szMchnTp = UserInfo.gMchnTp;
                //setMachineStop.m_UserID = UserInfo.gUserID;
                //setMachineStop.m_DriverName = UserInfo.gUserNm;

                //TCPComm.TCPAPI.SetMachineStop(setMachineStop);
            }
        }

        public static void GetBlockList_Ask()
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockList();
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockListForBlockMap_Ask()
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockListForBlockMap();
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }
        public static void GetVmtAutoStartConfig_Ask()
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetVmtAutoStartConfig();
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void ReleaseYtFromJob_Ask(String ytNo)
        {
            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
                hessianList.Add(ytNo);
                hessianList.Add(UserInfo.gUserID);

                HessianComm.HessianAPI.ReleaseYtFromJob(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }
        public static void SetItvDone(String ytNo)
        {
            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
                hessianList.Add(ytNo);
                hessianList.Add(UserInfo.gUserID);

                HessianComm.HessianAPI.ReleaseYtFromJob(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockMapList_Ask(String blck = "")
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockMapList(blck);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockMapListForYt_Ask(String blck = "")
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockMapListForYt(blck);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockMapListSwap_Ask(String blck = "")
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockMapSwapList(blck);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockMapListMoving1_Ask(String blck = "")
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockMapListMoving1(blck);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockMapListMoving2_Ask(String blck = "")
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockMapListMoving2(blck);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void DoSwap4Manual_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send value)
        {
            LogMessage("SetMachineStop_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                var task = new Task();

                //task.cntr = new HessianComm.Objects.Container();
                //task.cntr.cntrNo = send.cntrNo;
                //task.cntr.point = send.cntrPoint;

                task.partnerMchn = new Machine();
                task.partnerMchn.mchnId = value.partnerMachineID;

                //task.workingMchn = new Machine();

                //task.loc = new Location();

                task.jobId = value.jobid;
                task.externalId = value.externalId;

                if (!string.IsNullOrEmpty(value.chgPrgmId) && !string.IsNullOrEmpty(value.positionOnChassis))
                {
                    task.chgPrgmId = value.chgPrgmId;
                    task.positionOnChassis = value.positionOnChassis;                    
                }

                String newYtNo = value.newYTNo;

                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
                hessianList.Add(value.partnerMachineID);
                hessianList.Add(value.jobid);
                hessianList.Add(UserInfo.gMchnID);
                hessianList.Add(UserInfo.gMchnTp);
                hessianList.Add(newYtNo);                
                hessianList.Add(value.positionOnChassis);
                hessianList.Add("");

                HessianComm.HessianAPI.DoSwap4Manual(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {

            }
        }

        static public void SaveIniFile(ref VMT_Data_JAT2.Objects.Common.VD_Common_SaveIni value)
        {
            LogMessage("SaveIniFile");
            StructToLogMessage(value);

            VMT_Data_JAT2.Objects.UserInfo.gMchnTp = value.MchnType;
            VMT_Data_JAT2.Objects.UserInfo.gMchnID = value.MchnID;

            String strIniFile = VMT_Data_JAT2.Objects.UserInfo.GetIniDirectory() + @"MachineInfo.ini"; // NCT

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            ini.IniWriteValue("MACHINE", "TYPE", VMT_Data_JAT2.Objects.UserInfo.gMchnTp);
            ini.IniWriteValue("MACHINE", "ID", VMT_Data_JAT2.Objects.UserInfo.gMchnID);
            ini.IniWriteValue("MACHINE", "UISIZE", value.UISize);

            //ini.IniWriteValue("MACHINE", "LanguageType", value.LanguageType);
        }

        // KAP_eevmt_config.xml 파일의 AutoSeach, Port 변경및저장 함수
        static public void SaveXMLFile(ref VMT_Data_JAT2.Objects.Common.VD_Common_SaveXML value)
        {
        }

        // 현재 xml에 설정된 DGPS 포트번호 설명
        static public void GetXMLPortNumberFirst_ITV(ref int value)
        {
        }

        // 현재 xml에 설정된 SENSOR 포트번호 설명
        static public void GetXMLPortNumberSecond_ITV(ref int value)
        {
        }
        //-----------------------------------------------------------------------------

        static private WPSocketComm.Socket_TCP.socketServerTCP _socketServer;

        #region [Hessian Single Test]
        //////////////////////////////////////////////////
        // - Hessian Single Test
        public static String strUserID = "";
        public static String strUserNm = "";
        public static String strUserPW = "";
        public static String strMchnTp = "";
        public static String strMchnID = "";
        public static String strPartnerMchnID = "";

        public static String strBlock = "";
        public static String strBay = "";
        public static String strReasonCd = "";
        public static String strReasonNm = "";

        public static void KeepAlive_Test()
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.KeepAlive_Test();
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryList_Test(ref VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.blck = value.blck;
                location.bay = value.bay;

                HessianComm.HessianAPI.GetInventoryList_Test(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventory_Test(ref VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container value)
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.Container container = new HessianComm.Objects.Container();
                container.cntrNo = value.cntrNo;
                HessianComm.HessianAPI.GetInventory_Test(container);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetBlockMapList_Test()
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetBlockMapList_Test(strBlock);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void SetMachineStatusChanged_Test(Boolean isOn)
        {
            if (UserInfo.IsUseHessian)
            {
                Machine machine = new Machine();
                machine.mchnId = strMchnID;
                machine.isOn = isOn;

                HessianComm.HessianAPI.SetMachineStatusChanged_Test(machine);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void SetMachineStop_Test(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Send value)
        {
            MachineStop machineStop = new MachineStop();
            Boolean bStart = true;

            if (value.m_FinishTime == (Int64)0)
                bStart = true;
            else
                bStart = false;

            if (bStart)
            {
                // Start
                machineStop.mchn = new Machine();
                machineStop.mchn.mchnId = strMchnID;
                machineStop.mchn.mchnTp = strMchnTp;
                machineStop.user = new User();
                machineStop.user.usrId = strUserID;
                machineStop.user.usrNm = strUserNm;
                machineStop.reasonCd = value.Data.ReasonCd;
                machineStop.reasonNm = value.Data.ReasonNm;
                machineStop.isStop = true;

                HessianComm.HessianAPI.SetMachineStop_Test(machineStop);
            }
            else
            {
                // End
                machineStop.mchn = new Machine();
                machineStop.mchn.mchnId = strMchnID;
                machineStop.mchn.mchnTp = strMchnTp;
                machineStop.user = new User();
                machineStop.user.usrId = strUserID;
                machineStop.user.usrNm = strUserNm;
                machineStop.reasonCd = value.Data.ReasonCd;
                machineStop.reasonNm = value.Data.ReasonNm;
                machineStop.isStop = false;

                HessianComm.HessianAPI.SetMachineStop_Test(machineStop);
            }
        }

        public static void SetMachinePassed_Test(ref ITV.VD_ITV_SetManuaArrival_Send value) // Not Use, Use SetManualActivation
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            YtTask ytTask = new YtTask();
            ytTask.workingMchn = new Machine();
            ytTask.workingMchn.mchnId = strMchnID;
            ytTask.workingMchn.mchnTp = strMchnTp;
            ytTask.workingMchn.isOn = true;
            ytTask.loc = new Location();
            ytTask.loc.locTp = new LocationType();
            ytTask.loc.locTp.name = "YARD";
            ytTask.loc.blck = VMT_DataMgr_ITV.getBlckEntranceID(currentJob);
            ytTask.moveType = "I";

            HessianComm.HessianAPI.SetMachinePassed_Test(ytTask);
        }

        public static void SetManualActivation_Test(ref ITV.VD_ITV_SetManuaArrival_Send value)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            Task task = new Task();
            task.jobId = currentJob.jobKey;
            task.externalId = strPartnerMchnID;

            HessianComm.HessianAPI.SetManualActivation_Test(task);

        }

        public static void SetMachineArrivalInfo_Test(ref ITV.VD_ITV_SetManuaArrival_Send value)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            YtTask ytTask = new YtTask();
            ytTask.workingMchn = new Machine();
            ytTask.workingMchn.mchnId = strMchnID;
            ytTask.workingMchn.mchnTp = strMchnTp;
            ytTask.workingMchn.isOn = true;
            ytTask.loc = new Location();
            ytTask.loc.locTp = new LocationType();
            ytTask.loc.locTp.name = "LANE";
            ytTask.loc.bay = currentJob.locWorking.location;
            ytTask.moveType = "W";

            HessianComm.HessianAPI.SetMachineArrivalInfo_Test(ytTask);
        }

        public static void SetMachineReady_Test(ref ITV.VD_ITV_SetManualReady_Send value)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            Task task = new Task();
            task.workingMchn = new Machine();
            task.workingMchn.mchnId = currentJob.workingMchn.mchnId;
            task.workingMchn.mchnTp = currentJob.workingMchn.mchnTp;

            task.partnerMchn = new Machine();
            task.partnerMchn.mchnId = currentJob.partnerMchn.mchnId;
            task.partnerMchn.mchnTp = currentJob.partnerMchn.mchnTp;

            task.loc = new Location();
            task.loc.locTp = new LocationType();
            task.loc.locTp.name = currentJob.locWorking.locTp;
            if (currentJob.partnerMchn.mchnTp.Equals("QC"))
            {
                task.loc.bay = currentJob.locWorking.location;
            }
            else
            {
                task.loc.blck = currentJob.locWorking.blck;
                task.loc.bay = currentJob.locWorking.bay;
            }
            task.jobTp = currentJob.type.jobTp;

            HessianComm.HessianAPI.SetMachineReady_Test(task);
        }

        public static void GetMachineStop_Test()
        {
            Machine machine = new Machine();
            machine.mchnId = strMchnID;
            machine.mchnTp = strMchnTp;

            HessianComm.HessianAPI.GetMachineStop_Test(machine);
        }

        public static void GetMachineNotice_Test()
        {
            Machine machine = new Machine();
            machine.mchnId = strMchnID;
            machine.mchnTp = strMchnTp;

            HessianComm.HessianAPI.GetMachineNotice_Test(machine);
        }

        public static void SetMachineNotice_Test()
        {
            Machine machine = new Machine();
            machine.mchnId = strMchnID;
            machine.mchnTp = strMchnTp;
            machine.noticeMsg = "";

            HessianComm.HessianAPI.SetMachineNotice_Test(machine);
        }

        public static void GetPrecedingYtList_Test()
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            StsTask stsTask = new StsTask();
            stsTask.cntr = new HessianComm.Objects.Container();
            stsTask.cntr.cntrNo = currentJob.cntr.cntrNo;
            stsTask.vsl = new Vessel();
            stsTask.vsl.vessel = currentJob.type.vslCd;
            stsTask.vsl.voyage = currentJob.type.voyNo;

            HessianComm.HessianAPI.GetPrecedingYtList_Test(stsTask);
        }

        public static void SetJobDone_Test(Boolean bTwistLock)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            Task task = new Task();
            task.workingMchn = new Machine();
            task.workingMchn.mchnId = currentJob.workingMchn.mchnId;
            task.workingMchn.mchnTp = currentJob.workingMchn.mchnTp;

            task.partnerMchn = new Machine();
            task.partnerMchn.mchnId = currentJob.partnerMchn.mchnId;
            task.partnerMchn.mchnTp = currentJob.partnerMchn.mchnTp;

            task.loc = new Location();
            task.loc.locTp = new LocationType();
            task.loc.locTp.name = currentJob.locWorking.locTp;
            if (currentJob.partnerMchn.mchnTp.Equals("QC"))
            {
                task.loc.bay = currentJob.locWorking.location;
            }
            else
            {
                task.loc.blck = currentJob.locWorking.blck;
                task.loc.bay = currentJob.locWorking.bay;
            }
            task.jobTp = currentJob.type.jobTp;

            if (bTwistLock)
            {
            }
            else
            {
            }

            HessianComm.HessianAPI.SetJobDone_Test(task);
        }


        public static void GetJobOrderList_Test()
        {
            Machine machine = new Machine();
            machine.mchnId = strMchnID;
            machine.mchnTp = strMchnTp;

            HessianComm.HessianAPI.GetJobOrderList_Test(machine);
        }

        public static void GetUserAccessRole_Test(VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Send value)
        {
            User user = new User();
            user.usrId = value.UserID;
            // user.usrPasswd = value.UserPW;

            HessianComm.HessianAPI.GetUserAccessRole_Test(user);
        }

        public static void SetLogin4Machine_Test(VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Send value)
        {
            Login login = new Login();
            login.user = new User();
            login.user.usrId = value.UserID;
            login.user.usrPasswd = value.UserPW;

            login.user.usrGrp = new ArrayList();
            login.user.usrGrp.Add(value.GroupName);

            //login.user.usrGrp = new String[] { value.TeamName };
            //login.user.usrGrp.Add(value.TeamName);

            //login.user.usrGrp = value.TeamName;

            login.machine = new Machine();
            login.machine.mchnId = strMchnID;
            login.machine.mchnTp = strMchnTp;

            HessianComm.HessianAPI.SetLogin4Machine_Test(login);
        }

        public static void SetLogout4Machine_Test()
        {
            LogOut logOut = new LogOut();
            logOut.user = new User();
            logOut.user.usrId = strUserID;
            logOut.machine = new Machine();
            logOut.machine.mchnId = strMchnID;

            HessianComm.HessianAPI.SetLogout4Machine_Test(logOut);
        }

        public static void GetMachineStopCodeList_Test()
        {
            HessianComm.HessianAPI.GetMachineStopCodeList_Test(strMchnTp);
        }

        // 2015-12-28 추가
        public static void GetJobOrderByContainer_Test(String value)
        {
            HessianComm.HessianAPI.GetJobOrderByContainer_Test(value);
        }

        public static void GetMachineList_Test()
        {
            Machine machine = new Machine();
            machine.mchnTp = strMchnTp;//"YT";

            HessianComm.HessianAPI.GetMachineList_Test(machine);
        }

        public static void GetMachineListOfPool_Test()
        {
            Machine machine = new Machine();
            machine.mchnId = strMchnID;//"T010";
            machine.mchnTp = strMchnTp;//"YT";

            HessianComm.HessianAPI.GetMachineListOfPool_Test(machine);
        }

        public static void DoSwap4Manual_Test(String newYtNo)
        {
            Task task = new Task();
            task.workingMchn = new Machine();
            task.partnerMchn = new Machine();
            task.loc = new Location(); ;
            task.jobTp = String.Empty;

            hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(task.ToHessianHashtable());
            hessianList.Add(newYtNo);

            HessianComm.HessianAPI.DoSwap4Manual_Test(hessianList);
        }

        public static void GetBlockList_Test()
        {
            HessianComm.HessianAPI.GetBlockList_Test();
        }

        public static void SetJobStatus_Test(Boolean bActive)
        {
            //VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add("1234");//currentJob.jobKey);  

            HessianComm.Objects.JobStatusType jobStatus = new HessianComm.Objects.JobStatusType();
            jobStatus.name = bActive ? "JOB_PROCESSING" : "JOB_INACTIVE";
            hessianList.Add(jobStatus);
            hessianList.Add(strMchnID);
            hessianList.Add(strUserID);
            HessianComm.HessianAPI.SetJobStatus_Test(hessianList);
        }

        public static void SetDetwinJob_Test()
        {
            //VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];

            hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add("1234");//currentJob.jobKey);
            hessianList.Add(strUserID);
            HessianComm.HessianAPI.SetDetwinJob_Test(hessianList);
        }
        //////////////////////////////////////////////////
        #endregion [Hessian Single Test]
    }

    public class VMT_DataMgr_Common_Callback
    {
        //-----------------------------------------------------------------------------
        //- TCP Callback
        //static private void TCPCommonSendCallback(TCPCommType type, Object obj)
        //{
        //    try
        //    {
        //        LogMessage("TCPCommonSendCallback Type : " + type.ToString());

        //        TCPSendPacket packet = null;
        //        if (obj != null &&
        //            obj is TCPSendPacket)
        //            packet = obj as TCPSendPacket;

        //        switch (type)
        //        {
        //            //----------------------
        //            // Custom
        //            case TCPCommType.TCPSocketOpen:
        //            case TCPCommType.TCPSocketConnected:
        //            case TCPCommType.TCPSocketDisconnected:
        //                {
        //                    if (type == TCPCommType.TCPSocketOpen)
        //                        VMT_DataMgr_Common_Callback.TCPSocketOpen(ref obj);
        //                    else if (type == TCPCommType.TCPSocketDisconnected)
        //                        VMT_DataMgr_Common_Callback.TCPSocketDisconnected(ref obj);
        //                }
        //                break;
        //            default:
        //                {
        //                    // Retry
        //                    if (packet.PacketSize != packet.SendSize)
        //                    {
        //                        TCPAPI.TCPCommExcute(packet.CmdType, packet.Obj, 5000); // 5 sec
        //                    }
        //                    // Console.WriteLine(Log.ToString());
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        String errorMessage = ex.Message;
        //    }
        //}

        //static private void TCPCommonReceiveCallback(TCPCommType type, Object obj)
        //{
        //    try
        //    {
        //        if (type == TCPCommType.VMT_TU_ITV_DGPS_Periodic || type == TCPCommType.VMT_TU_RTG_PDS_Periodic) { }
        //        else
        //        {
        //            LogMessage("TCPCommonReceiveCallback Type : " + type.ToString());
        //        }

        //        EEPacket packet = null;
        //        if (obj != null &&
        //            obj is EEPacket)
        //            packet = obj as EEPacket;

        //        switch (type)
        //        {
        //            //----------------------
        //            // Custom
        //            case TCPCommType.TCPParsingFail: { VMT_DataMgr_Common_Callback.TCPParsingFail(ref obj); } break;
        //            /*
        //             ....
        //             */
        //            //----------------------
        //            //- Common
        //            case TCPCommType.VMT_TU_SoftwareInfo: { VMT_DataMgr_Common_Callback.SoftwareInfo_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_ConnectionStatus: { VMT_DataMgr_Common_Callback.ConnectionStatus_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_MachineNotice: { VMT_DataMgr_Common_Callback.MachineNotice_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_SetupDone: { VMT_DataMgr_Common_Callback.SetupDone_Rcv(ref obj); } break;
        //            //----------------------
        //            //- Login
        //            case TCPCommType.VMT_TU_GetUserAccessRole: { VMT_DataMgr_Common_Callback.UserAccesRole_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_SetLogin4Machine: { VMT_DataMgr_Common_Callback.Login4Machine_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_SendMachineStatusChange: { VMT_DataMgr_Common_Callback.MachineStatusChange_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_ResetDGPS: { VMT_DataMgr_Common_Callback.ResetDGPS_Rcv(ref obj); } break;
        //            case TCPCommType.VMT_TU_SendPubx05: { VMT_DataMgr_Common_Callback.SendPubx05_Rcv(ref obj); } break;
        //            case TCPCommType.VMT_TU_SendPubx06: { VMT_DataMgr_Common_Callback.SendPubx06_Rcv(ref obj); } break;
        //            case TCPCommType.VMT_TU_SendHighForward: { VMT_DataMgr_Common_Callback.SendHighForward_Rcv(ref obj); } break;
        //            case TCPCommType.VMT_TU_SendHighBackward: { VMT_DataMgr_Common_Callback.SendHighBackward_Rcv(ref obj); } break;
        //            case TCPCommType.VMT_TU_RequestCfgekf: { VMT_DataMgr_Common_Callback.RequestCfgekf_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_SaveDGPSCfg: { VMT_DataMgr_Common_Callback.SaveDGPSCfg_Rcv(ref obj); } break;
        //            case TCPCommType.VMT_TU_MaChineStopCodeList: { VMT_DataMgr_Common_Callback.MachineStopCodeList_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_GetMachineStop: { VMT_DataMgr_Common_Callback.GetMachineStop_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_SetMachineStop: { VMT_DataMgr_Common_Callback.SetMachineStop_Rcv(ref packet); } break;
        //            case TCPCommType.VMT_TU_NotifyMachineStopResult: { VMT_DataMgr_Common_Callback.NotifyMachineStopResult(ref packet); } break;
        //            case TCPCommType.VMT_TU_NotifyAlarm: 
        //                { 
        //                    VMT_DataMgr_Common_Callback.NotifyAlarm(ref packet);
        //                    VMT_DataMgr_Common_Callback.ExceptionAlarm(ref packet);
        //                } 
        //                break;
        //            //-----------------------------------------------------------------------------
        //            //- Common Job
        //            case TCPCommType.VMT_TU_JobOrder:
        //                {
        //                    if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
        //                        VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj);
        //                    else if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_RTG)
        //                        VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj);
        //                }
        //                break;
        //            case TCPCommType.VMT_TU_JobCancel: { VMT_DataMgr_Common_Callback.JobCancel(ref packet); } break;
        //            case TCPCommType.VMT_TU_JobDone: { VMT_DataMgr_Common_Callback.JobDone(ref packet); } break;
        //            case TCPCommType.VMT_TU_ManualJobDone: { VMT_DataMgr_Common_Callback.ManualJobDone(ref packet); } break;
        //            case TCPCommType.VMT_TU_JobCancelAll: { VMT_DataMgr_Common_Callback.JobCancelAll(ref obj); } break;
        //            //-----------------------------------------------------------------------------

        //            //-----------------------------------------------------------------------------
        //            //- ITV Only
        //            case TCPCommType.VMT_TU_ITV_DGPS_Periodic: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_PDS: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_SetChassis_Attach: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_NotifyChassis_Attach: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_NofityBlockEnterance: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_NotifyCPSAlign: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            //case TCPCommType.VMT_TU_ITV_NotifyPOW: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); }break;                    
        //            case TCPCommType.VMT_TU_ITV_SetManualArrival: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_NotifyManualArrival: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_SetManualReady: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_ITV_NotifyManualReady: { VMT_DataMgr_ITV_Callback.TCPITVReceiveCallback(type, obj); } break;
        //            //-----------------------------------------------------------------------------

        //            //-----------------------------------------------------------------------------
        //            //- RTG Only
        //            case TCPCommType.VMT_TU_RTG_PDS_Periodic: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_CPS_Align: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_PDS_PickDrop: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_RFID: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            //**** ITV
        //            case TCPCommType.VMT_TU_RTG_NotifyMachinePOW: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            //case TCPCommType.VMT_TU_RTG_NotifyMachineExit: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifyMachineBlockEnter: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifyMachineReadyITV: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_ManualReady: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            //**** Inven
        //            case TCPCommType.VMT_TU_RTG_SendBlockInfo: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifyBlockInfo: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_SendBlockInfoSimple: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifyBlockInfoSimple: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_SendCorrection: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifyCorrection: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            //**** Job
        //            case TCPCommType.VMT_TU_RTG_SetCurrentJob: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifySetCurrentJob: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_HandleJobDone: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_TargetJob: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_Marrying: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_NotifyMarring: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_Swap_Result: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            case TCPCommType.VMT_TU_RTG_Return_Cntr: { VMT_DataMgr_RTG_Callback.TCPRTGReceiveCallback(type, obj); } break;
        //            default: { } break; // Error : Not Define // Console.WriteLine(Log.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        String errorMessage = ex.Message;
        //    }
        //}
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        //- TCP Callback
        static public void UDPCommonSendCallback(UDPCommType type, Object obj)
        {
            try
            {
                switch (type)
                {
                    case UDPCommType.UDPSocketOpen:
                        break;
                    case UDPCommType.Send_CFG_RST:
                    case UDPCommType.Send_PUBX_05:
                    case UDPCommType.Send_PUBX_06:
                    case UDPCommType.Send_HighBackward:
                    case UDPCommType.Send_HighForward:
                    case UDPCommType.Send_CFG_SAVE:
                    case UDPCommType.SendChassisInfo:
                    case UDPCommType.Request_CFG_EKF:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                String errorMessage = ex.Message;
            }
        }

        static public void UDPCommonReceiveCallback(UDPCommType type, Object obj)
        {
            try
            {
                switch (type)
                {
                    case UDPCommType.UDPSocketOpen:
                        break;
                    case UDPCommType.Send_CFG_RST:
                    case UDPCommType.Send_PUBX_05:
                    case UDPCommType.Send_PUBX_06:
                    case UDPCommType.Send_HighBackward:
                    case UDPCommType.Send_HighForward:
                    case UDPCommType.Send_CFG_SAVE:
                    case UDPCommType.SendChassisInfo:
                    case UDPCommType.Request_CFG_EKF:
                        break;
                    case UDPCommType.Recieve_DgpsSignal:
                        Recieve_DgpsSignal_Callback(obj);
                        break;
                    case UDPCommType.Recieve_PdsSignal:
                        Recieve_PdsSignal_Callback(obj);
                        break;
                    case UDPCommType.Response_CFG_EKF:
                        Response_CFG_EKF_Callback(obj);
                        break;
                    default: { } break; // Error : Not Define // Console.WriteLine(Log.ToString());
                }
            }
            catch (Exception ex)
            {
                String errorMessage = ex.Message;
            }
        }
        //-----------------------------------------------------------------------------

        static private void Recieve_DgpsSignal_Callback(Object obj)
        {
            if (obj is TTMPCommand)
            {
                TTMPCommand ttmpCmd = (obj as TTMPCommand);

                String latitude = ttmpCmd.GetValue(TTMPCmdData.PARAM_Latitude);
                String longitude = ttmpCmd.GetValue(TTMPCmdData.PARAM_Longitude);

                VMT_Data_JAT2.Objects.Common.VD_Common_GPS gpsValue = new Objects.Common.VD_Common_GPS();
                gpsValue.Latitude = latitude;
                gpsValue.Longitude = longitude;

                String pulse = ttmpCmd.GetValue(TTMPCmdData.PARAM_Pulse);
                String gyro = ttmpCmd.GetValue(TTMPCmdData.PARAM_Gyro);
                String temp = ttmpCmd.GetValue(TTMPCmdData.PARAM_Temp);
                String speedpulse = ttmpCmd.GetValue(TTMPCmdData.PARAM_SpeedPulse);
                String gyrosf = ttmpCmd.GetValue(TTMPCmdData.PARAM_GyroSF);
                String gyrobias = ttmpCmd.GetValue(TTMPCmdData.PARAM_GyroBias);
                VMT_Data_JAT2.Objects.Common.VD_Common_Calibration calibrationValue = new Objects.Common.VD_Common_Calibration();
                calibrationValue.Pulse = pulse;
                calibrationValue.Gyro = gyro;
                calibrationValue.Temp = Convert.ToSingle(temp);
                calibrationValue.SpeedPulse = speedpulse[0];
                calibrationValue.GyroSF = gyrosf[0];
                calibrationValue.GyroBias = gyrobias[0];

                String speed = ttmpCmd.GetValue(TTMPCmdData.PARAM_Speed);
                float fSpeed = Convert.ToSingle(speed);

                if(static_NotifyGPSStatus != null)
                    static_NotifyGPS(ref gpsValue);

                if (static_NotifyCalibration != null)
                    static_NotifyCalibration(ref calibrationValue);

                if (static_NotifySpeedKm != null)
                    static_NotifySpeedKm(fSpeed);
            }
        }

        static private void Recieve_PdsSignal_Callback(Object obj)
        {
            if (obj is TTMPCommand)
            {
                TTMPCommand ttmpCmd = (obj as TTMPCommand);

                String fuel = ttmpCmd.GetValue(TTMPCmdData.PARAM_Fuel);
                if (!fuel.Equals(""))
                {
                    int nFuel = Convert.ToInt32(fuel);
                    if (static_NotifyFuelGage != null)
                        static_NotifyFuelGage(nFuel);
                    
                }

                String conechecker = ttmpCmd.GetValue(TTMPCmdData.PARAM_ConeChecker);
                if (!conechecker.Equals(""))
                {
                    int nConeChecker = Convert.ToInt32(conechecker);
                    if (static_NotifyEngineTemp != null)
                        static_NotifyEngineTemp(nConeChecker);
                }
            }
        }

        static private void Response_CFG_EKF_Callback(Object obj)
        {
            if (obj is TTMPCommand)
            {
                TTMPCommand ttmpCmd = (obj as TTMPCommand);

                String response = ttmpCmd.GetValue(TTMPCmdData.PARAM_Response);
                if (!response.Equals(""))
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive requestCfgekf = new Objects.Common.VD_Common_RequestCfgekf_Receive();
                    requestCfgekf.m_iDirection = Convert.ToInt32(response);

                    if (static_NotifyCFGEKF != null)
                        static_NotifyCFGEKF(ref requestCfgekf);
                }
            }
        }

        //============================================================
        //=
        //= Aggrigate Exception in HessianAPI
        //=
        //============================================================
        static public void ExceptionDelete_VMT_Data(Exception ex)
        {
            if (ex is UDPException)
            {
                UDPException uEx = (UDPException)ex;

                if (uEx.InnerException is WebException)
                {
                    WebException wE = uEx.InnerException as WebException;

                    switch (wE.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                        case WebExceptionStatus.Timeout:
                            {

                            }
                            break;
                        case WebExceptionStatus.ConnectionClosed:
                        case WebExceptionStatus.ServerProtocolViolation:
                        case WebExceptionStatus.UnknownError:
                            {
                                // bReConnect = true;
                            }
                            break;
                        default:
                            break;
                    }
                }

                switch (uEx.udpCommType)
                {
                    case UDPCommType.UDPSocketOpen: // UDP Open Fail
                        break;
                    default:
                        break;
                }
            }
        }

        //-------------------------------------------------------------------------
        //- Callback Functions
        public delegate void Callback_NotifyHandleLogApi(bool isSend, HessianCommType type, Object value);
        public static void SetCallback_NotifyHandleLogApi(Callback_NotifyHandleLogApi fp)
        {
            static_NotifyHandleLogApi = fp;
        }

        public delegate void CallBack_NotifySwinfoRP(ref VMT_Data_JAT2.Objects.Common.VD_Common_Swinfo_Receive value);
        static public void SetCallBack_NotifySwinfoRP(CallBack_NotifySwinfoRP fp)
        {
            static_NotifySwinfoRP = fp;
        }

        public delegate void CallBack_NotifyConnectionStatus(ref VMT_Data_JAT2.Objects.Common.VD_Common_ConnectionStatus_Receive value);
        static public void SetCallBack_NotifyConnectionStatus(CallBack_NotifyConnectionStatus fp)
        {
            //static_NotifyConnectionStatus = fp;
        }

        public delegate void CallBack_NotifyMachineNotice(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive value);
        static public void SetCallBack_NotifyMachineNotice(CallBack_NotifyMachineNotice fp)
        {
            static_NotifyMachineNotice = fp;
        }

        public delegate void CallBack_NotifySetupDone(int value);
        static public void SetCallBack_NotifySetupDone(CallBack_NotifySetupDone fp)
        { }

        // 지피에스 상태를 통지 받습니다.
        public delegate void Callback_NotifyGPSStatus(int value);
        static public void SetCallBack_NotifyGPSStatus(Callback_NotifyGPSStatus fp)
        {
            //static_NotifyGPSStatus = fp;
        }

        public delegate void Callback_NotifyGPS(ref VMT_Data_JAT2.Objects.Common.VD_Common_GPS value);
        static public void SetCallBack_NotifyGPS(Callback_NotifyGPS fp)
        {
            static_NotifyGPS = fp;
        }

        // 와이파이 상태를 통지 받습니다.
        public delegate void Callback_NotifyWIFIStatus(int value);
        static public void SetCallBack_NotifyWIFIStatus(Callback_NotifyWIFIStatus fp)
        {
            static_NotifyWIFIStatus = fp;
        }

        public delegate void Callback_NotifyKeepAlive(String value);
        public static void SetCallback_NotifyKeepAlive(Callback_NotifyKeepAlive fp)
        {
            static_NotifyKeepAlive = fp;
        }

        public delegate void Callback_NotifySpeedKm(float value);
        static public void SetCallBack_NotifySpeedKm(Callback_NotifySpeedKm fp)
        {
            static_NotifySpeedKm = fp;
        }

        // 엔진온도를 통지 받습니다.
        public delegate void Callback_NotifyEngineTemp(int value);
        static public void SetCallBack_NotifyEngineTemp(Callback_NotifyEngineTemp fp)
        {
            static_NotifyEngineTemp = fp;
        }

        // 연료상태를 통지 받습니다.
        public delegate void Callback_NotifyFuelGage(int value);
        static public void SetCallBack_NotifyFuelGage(Callback_NotifyFuelGage fp)
        {
            static_NotifyFuelGage = fp;
        }

        public delegate void Callback_NotifyAccessRole(ref VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive value);
        static public void SetCallBack_NotifyAccessRole(Callback_NotifyAccessRole fp)
        { }

        public delegate void Callback_NotifyLogin4Machine(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value);   // 드라이정보 요청 결과를 통지받습니다.
        static public void SetCallBack_NotifyLogin4Machine(Callback_NotifyLogin4Machine fp)
        {
            static_NotifyLogin4Machine = fp;
        }
        public delegate void Callback_NotifyChangeDriverCheck(String rsnCd);
        static public void SetCallBack_NotifyChangeDriverCheck(Callback_NotifyChangeDriverCheck fp)
        {
            static_NotifyChangeDriverCheck = fp;
        }
        public delegate void Callback_NotifyConfig(ref VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive value);
        static public void SetCallBack_NotifyConfig(Callback_NotifyConfig fp)
        {
            static_NotifyConfig = fp;
        }

        //public delegate void Callback_NotifyLogIn(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive value);             // 로그인요청 결과를 통지받습니다.
        //static public void SetCallBack_NotifyLoginInfo(Callback_NotifyLogIn fp)
        //{ }

        // 잡을 갯수만큼 여러번 넘김
        public delegate void Callback_NotifyJobOrder(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder value); // 요청한 잡정보를 통지받습니다.
        static public void SetCallBack_NotifyJobOrder(Callback_NotifyJobOrder fp)
        { }

        public delegate void Callback_NotifyJobChange(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder value); // 잡체인지 메시지를 통지받습니다.
        static public void SetCallBack_NotifyJobChange(Callback_NotifyJobChange fp)
        { }

        public delegate void Callback_NotifyJobDelete(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobKey value); // 잡딜리트 메시지를 통지받습니다.            
        static public void SetCallBack_NotifyJobDelete(Callback_NotifyJobDelete fp)
        { }

        public delegate void Callback_NotifyJobDeleteAll(int value); // 잡딜리트 올 메시지를 통지받습니다.
        static public void SetCallBack_NotifyJobDeleteAll(Callback_NotifyJobDeleteAll fp)
        { }

        public delegate void Callback_NotifyMachineStopCodeList(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive value);    // Available 종류에 대한 메시지를 통지 받는다.
        static public void SetCallBack_NotifyMachineStopCodeList(Callback_NotifyMachineStopCodeList fp)
        {
            static_NotifyMachineStopCodeList = fp;
        }

        public delegate void Callback_NotifyMachineAccessAction(ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineAccessAction_Receive value);
        static public void SetCallBack_NotifyMachineAccessAction(Callback_NotifyMachineAccessAction fp)
        {
            static_NotifyMachineAccessAction = fp;
        }


        public delegate void Callback_NotifyGetMachineStop(VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive value);
        static public void SetCallBack_NotifyAvailable(Callback_NotifyGetMachineStop fp)
        {
            static_NotifyGetMachineStop = fp;
        }

        public delegate void Callback_NotifySetMachineStop(ref VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive value);
        static public void SetCallBack_NotifySetMachineStop(Callback_NotifySetMachineStop fp)
        {
            static_NotifySetMachineStop = fp;
        }

        public delegate void Callback_NotifyMachineStopConfirm([In][Out] ref VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopConfirm_Receive value);
        public static void SetCallBack_NotifyMachineStopConfirm(Callback_NotifyMachineStopConfirm fp)
        {
            static_NotifyMachineStopConfirm = fp;
        }

        public delegate void Callback_NotifyErrorCode([MarshalAs(UnmanagedType.LPWStr)]String value);
        static public void SetCallBack_NotifyErrorCode(Callback_NotifyErrorCode fp)
        { }

        public delegate void Callback_NotifyCalibration(ref VMT_Data_JAT2.Objects.Common.VD_Common_Calibration value);
        static public void SetCallBack_NotifyCalibration(Callback_NotifyCalibration fp)
        {
            static_NotifyCalibration = fp;
        }

        public delegate void Callback_NotifyMachineStatusChanged(ref VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive value);
        static public void SetCallBack_NotifyMachineStatusChanged(Callback_NotifyMachineStatusChanged fp)
        {
            static_NotifyMachineStatusChanged = fp;
        }

        public delegate void Callback_NotifyCFGEKF(ref VMT_Data_JAT2.Objects.Common.VD_Common_RequestCfgekf_Receive value);
        static public void SetCallBack_NotifyCFGEKF(Callback_NotifyCFGEKF fp)
        {
            static_NotifyCFGEKF = fp;
        }

        public delegate void Callback_NotifyAlarm(ref VMT_Data_JAT2.Objects.Common.VD_Common_Alram_Receive value);
        static public void SetCallback_NotifyAlarm(Callback_NotifyAlarm fp)
        {
            static_NotifyAlarm = fp;
        }

        public delegate void Callback_NotifyExceptionAlarm(ref VMT_Data_JAT2.Objects.Common.VD_Common_Exception_Receive value);
        static public void SetCallback_NotifyExceptionAlarm(Callback_NotifyExceptionAlarm fp)
        {
            static_NotifyExceptionAlarm = fp;
        }

        public delegate void Callback_NotifyBlockList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value);
        static public void SetCallback_NotifyBlockList(Callback_NotifyBlockList fp)
        {
            static_NotifyBlockList = fp;
        }

        public delegate void Callback_NotifyBlockListForBlockMap(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value);
        static public void SetCallback_NotifyBlockListForBlockMap(Callback_NotifyBlockListForBlockMap fp)
        {
            static_NotifyBlockListForBlockMap = fp;
        }
        public delegate void Callback_NotifyGetVmtAutoStartConfig(string retStr);
        static public void SetCallback_NotifyGetVmtAutoStartConfig(Callback_NotifyGetVmtAutoStartConfig fp)
        {
            static_NotifyGetVmtAutoStartConfig = fp;
        }
        public delegate void Callback_NotifyBlockMapList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value);
        static public void SetCallback_NotifyBlockMapList(Callback_NotifyBlockMapList fp)
        {
            static_NotifyBlockMapList = fp;
        }

        public delegate void Callback_NotifyBlockMapListForYt(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value);
        static public void SetCallback_NotifyBlockMapListForYt(Callback_NotifyBlockMapListForYt fp)
        {
            static_NotifyBlockMapListForYt = fp;
        }

        static public void SetCallback_NotifyBlockMapList1(Callback_NotifyBlockMapList fp)
        {
            static_NotifyBlockMapList1 = fp;
        }

        static public void SetCallback_NotifyBlockMapList2(Callback_NotifyBlockMapList fp)
        {
            static_NotifyBlockMapList2 = fp;
        }

        public delegate void Callback_NotifyBlockMapSwapList(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value);
        static public void SetCallback_NotifyBlockMapSwapList(Callback_NotifyBlockMapSwapList fp)
        {
            static_NotifyBlockMapSwapList = fp;
        }

        public delegate void Callback_NotifyDoSwap4Manual(String value);
        static public void SetCallback_NotifyDoSwap4Manual(Callback_NotifyDoSwap4Manual fp)
        {
            static_NotifyDoSwap4Manual = fp;
        }


        public delegate void Callback_NotifyException(String value);
        static public void SetCallback_NotifyException(Callback_NotifyException fp)
        {
            static_NotifyException = fp;
        }

        public delegate void Callback_NotifyLogout(Boolean value);
        static public void SetCallback_NotifyLogout(Callback_NotifyLogout fp)
        {
            static_NotifyLogout = fp;
        }

        public delegate void Callback_NotifyGetConfigValue(String value);
        static public void SetCallback_NotifyGetConfigValue(Callback_NotifyGetConfigValue fp)
        {
            static_NotifyGetConfigValue = fp;
        }

        static public Callback_NotifyHandleLogApi static_NotifyHandleLogApi;

        static public CallBack_NotifySwinfoRP static_NotifySwinfoRP;

        static public Callback_NotifyGPSStatus static_NotifyGPSStatus;
        public static Callback_NotifyGPS static_NotifyGPS;
        static public Callback_NotifyWIFIStatus static_NotifyWIFIStatus;
        static public Callback_NotifyKeepAlive static_NotifyKeepAlive;
        //static public CallBack_NotifyConnectionStatus static_NotifyConnectionStatus;
        static public CallBack_NotifyMachineNotice static_NotifyMachineNotice;
        //static public Callback_NotifyGPS static_NotifyGPS;
        static public CallBack_NotifySetupDone static_NotifySetupDone;

        static public Callback_NotifyEngineTemp static_NotifyEngineTemp;
        static public Callback_NotifyFuelGage static_NotifyFuelGage;
        static public Callback_NotifySpeedKm static_NotifySpeedKm;

        static public Callback_NotifyAccessRole static_NotifyAccessRole;
        static public Callback_NotifyLogin4Machine static_NotifyLogin4Machine;
        static public Callback_NotifyChangeDriverCheck static_NotifyChangeDriverCheck;

        static public Callback_NotifyConfig static_NotifyConfig;
        //static public Callback_NotifyLogIn static_NotifyLogIn;
        static public Callback_NotifyMachineStatusChanged static_NotifyMachineStatusChanged;

        static public Callback_NotifyJobOrder static_NotifyJobOrder;
        static public Callback_NotifyJobChange static_NotifyJobChange;
        static public Callback_NotifyJobDelete static_NotifyJobDelete;
        static public Callback_NotifyJobDeleteAll static_NotifyJobDeleteAll;

        static public Callback_NotifyMachineStopCodeList static_NotifyMachineStopCodeList;
        static public Callback_NotifyMachineAccessAction static_NotifyMachineAccessAction;
        static public Callback_NotifyMachineAccessAction static_NotifyBlockListForYardSector;
        static public Callback_NotifyGetMachineStop static_NotifyGetMachineStop;
        static public Callback_NotifySetMachineStop static_NotifySetMachineStop;
        public static Callback_NotifyMachineStopConfirm static_NotifyMachineStopConfirm;

        static public Callback_NotifyErrorCode static_NotifyErrorCode;
        static public Callback_NotifyCalibration static_NotifyCalibration;
        static public Callback_NotifyCFGEKF static_NotifyCFGEKF;

        static public Callback_NotifyAlarm static_NotifyAlarm;
        static public Callback_NotifyExceptionAlarm static_NotifyExceptionAlarm;
        //-------------------------------------------------------------------------

        // 2016-1-12 추가
        static public Callback_NotifyBlockList static_NotifyBlockList;
        static public Callback_NotifyBlockListForBlockMap static_NotifyBlockListForBlockMap;
        static public Callback_NotifyGetVmtAutoStartConfig static_NotifyGetVmtAutoStartConfig;
        static public Callback_NotifyBlockMapListForYt static_NotifyBlockMapListForYt;
        static public Callback_NotifyBlockMapList static_NotifyBlockMapList;
        static public Callback_NotifyBlockMapList static_NotifyBlockMapList1;
        static public Callback_NotifyBlockMapList static_NotifyBlockMapList2;
        static public Callback_NotifyBlockMapSwapList static_NotifyBlockMapSwapList;

        static public Callback_NotifyDoSwap4Manual static_NotifyDoSwap4Manual;
        static public Callback_NotifyException static_NotifyException;
        static public Callback_NotifyLogout static_NotifyLogout;
        static public Callback_NotifyGetConfigValue static_NotifyGetConfigValue;


        /////////////////////////
        // - Single Test
        public delegate void Callback_NotifySingleTest([In][Out] ref Object value);
        public static void SetCallBack_NotifyKeepAlive_Test(Callback_NotifySingleTest fp) { static_NotifyKeepAlive_Test = fp; }
        public static void SetCallBack_NotifyGetInventoryList_Test(Callback_NotifySingleTest fp) { static_NotifyGetInventoryList_Test = fp; }
        public static void SetCallBack_NotifyGetInventory_Test(Callback_NotifySingleTest fp) { static_NotifyGetInventory_Test = fp; }
        public static void SetCallBack_NotifyGetBlockMapList_Test(Callback_NotifySingleTest fp) { static_NotifyGetBlockMapList_Test = fp; }
        public static void SetCallBack_NotifySetMachineStatusChanged_Test(Callback_NotifySingleTest fp) { static_NotifySetMachineStatusChanged_Test = fp; }
        public static void SetCallBack_NotifySetMachineStop_Test(Callback_NotifySingleTest fp) { static_NotifySetMachineStop_Test = fp; }
        public static void SetCallBack_NotifySetMachinePassed_Test(Callback_NotifySingleTest fp) { static_NotifySetMachinePassed_Test = fp; }
        public static void SetCallBack_NotifySetManualActivation_Test(Callback_NotifySingleTest fp) { static_NotifySetManualActivation_Test = fp; }
        public static void SetCallBack_NotifySetMachineArrivalInfo_Test(Callback_NotifySingleTest fp) { static_NotifySetMachineArrivalInfo_Test = fp; }
        public static void SetCallBack_NotifySetMachineReady_Test(Callback_NotifySingleTest fp) { static_NotifySetMachineReady_Test = fp; }
        public static void SetCallBack_NotifyGetMachineStop_Test(Callback_NotifySingleTest fp) { static_NotifyGetMachineStop_Test = fp; }
        public static void SetCallBack_NotifyGetMachineNotice_Test(Callback_NotifySingleTest fp) { static_NotifyGetMachineNotice_Test = fp; }
        public static void SetCallBack_NotifySetMachineNotice_Test(Callback_NotifySingleTest fp) { static_NotifySetMachineNotice_Test = fp; }
        public static void SetCallBack_NotifyGetPrecedingYtList_Test(Callback_NotifySingleTest fp) { static_NotifyGetPrecedingYtList_Test = fp; }
        public static void SetCallBack_NotifySetJobDone_Test(Callback_NotifySingleTest fp) { static_NotifySetJobDone_Test = fp; }
        public static void SetCallBack_NotifyGetJobOrderList_Test(Callback_NotifySingleTest fp) { static_NotifyGetJobOrderList_Test = fp; }
        public static void SetCallBack_NotifyGetUserAccessRole_Test(Callback_NotifySingleTest fp) { static_NotifyGetUserAccessRole_Test = fp; }
        public static void SetCallBack_NotifySetLogin4Machine_Test(Callback_NotifySingleTest fp) { static_NotifySetLogin4Machine_Test = fp; }
        public static void SetCallBack_NotifySetLogout4Machine_Test(Callback_NotifySingleTest fp) { static_NotifySetLogout4Machine_Test = fp; }
        public static void SetCallBack_NotifyGetMachineStopCodeList_Test(Callback_NotifySingleTest fp) { static_NotifyGetMachineStopCodeList_Test = fp; }
        // 2015-12-28 추가
        public static void SetCallBack_NotifyGetJobOrderByContainer_Test(Callback_NotifySingleTest fp) { static_NotifyGetJobOrderByContainer_Test = fp; }
        public static void SetCallBack_NotifyGetMachineList_Test(Callback_NotifySingleTest fp) { static_NotifyGetMachineList_Test = fp; }
        public static void SetCallBack_NotifyGetMachineListOfPool_Test(Callback_NotifySingleTest fp) { static_NotifyGetMachineListOfPool_Test = fp; }
        public static void SetCallBack_NotifyDoSwap4Manual_Test(Callback_NotifySingleTest fp) { static_NotifyDoSwap4Manual_Test = fp; }

        public static void SetCallBack_NotifyGetBlockList_Test(Callback_NotifySingleTest fp) { static_NotifyGetBlockList_Test = fp; }
        public static void SetCallBack_NotifySetJobStatus_Test(Callback_NotifySingleTest fp) { static_NotifySetJobStatus_Test = fp; }
        public static void SetCallBack_NotifySetDetwinJob_Test(Callback_NotifySingleTest fp) { static_NotifySetDetwinJob_Test = fp; }

        public static void SetCallBack_NotifyGetMahcineList_Test(Callback_NotifySingleTest fp) { static_NotifyGetMahcineList_Test = fp; }

        public static Callback_NotifySingleTest static_NotifyKeepAlive_Test;
        public static Callback_NotifySingleTest static_NotifyGetInventoryList_Test;
        public static Callback_NotifySingleTest static_NotifyGetInventory_Test;
        public static Callback_NotifySingleTest static_NotifyGetBlockMapList_Test;
        public static Callback_NotifySingleTest static_NotifySetMachineStatusChanged_Test;
        public static Callback_NotifySingleTest static_NotifySetMachineStop_Test;
        public static Callback_NotifySingleTest static_NotifySetMachinePassed_Test; // Not Use, Use SetManualActivation
        public static Callback_NotifySingleTest static_NotifySetManualActivation_Test;
        public static Callback_NotifySingleTest static_NotifySetMachineArrivalInfo_Test;
        public static Callback_NotifySingleTest static_NotifySetMachineReady_Test;
        public static Callback_NotifySingleTest static_NotifyGetMachineStop_Test;
        public static Callback_NotifySingleTest static_NotifyGetMachineNotice_Test;
        public static Callback_NotifySingleTest static_NotifySetMachineNotice_Test;
        public static Callback_NotifySingleTest static_NotifyGetPrecedingYtList_Test;
        public static Callback_NotifySingleTest static_NotifySetJobDone_Test;
        public static Callback_NotifySingleTest static_NotifyGetJobOrderList_Test;
        public static Callback_NotifySingleTest static_NotifyGetUserAccessRole_Test;
        public static Callback_NotifySingleTest static_NotifySetLogin4Machine_Test;
        public static Callback_NotifySingleTest static_NotifySetLogout4Machine_Test;
        public static Callback_NotifySingleTest static_NotifyGetMachineStopCodeList_Test;
        // 2015-12-28 추가
        public static Callback_NotifySingleTest static_NotifyGetJobOrderByContainer_Test;
        public static Callback_NotifySingleTest static_NotifyGetMachineList_Test;
        public static Callback_NotifySingleTest static_NotifyGetMachineListOfPool_Test;
        public static Callback_NotifySingleTest static_NotifyDoSwap4Manual_Test;

        public static Callback_NotifySingleTest static_NotifyGetBlockList_Test;
        public static Callback_NotifySingleTest static_NotifySetJobStatus_Test;
        public static Callback_NotifySingleTest static_NotifySetDetwinJob_Test;
        public static Callback_NotifySingleTest static_NotifyGetMahcineList_Test;

        /////////////////////////



    }
}