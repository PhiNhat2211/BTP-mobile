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
using UDPComm;

namespace VMT_Data_JAT2
{
    public class VMT_DataMgr
    {
        static public String gHessianServerIP = "172.20.130.156";
        static public String gHessianServerPort = "7110";
        static public String gHessianServerMgtIP = "";
        static public String gHessianServerMgtPort = "";
        //static public String gTCPLocalIP = "127.0.0.1"; // Local Loopback Address
        //static public String gTCPLocalIP = "116.127.224.233";
        //static public String gTCPLocalPort = "";
        //static public String gTCPBypassPort = "";
        static public String gUDPClientPort = "";
        static public String gUDPServerPort = "";

        static public DateTime startDateTime = DateTime.Now;

        static private void LogMessage(String strValue)
        {
            strValue = "[VMT_DataMgr] " + strValue;
            Util.LogMessage(strValue);
        }

        static private void StructToLogMessage(Object obj)
        {
            VMT_DataMgr.LogMessage("== Snd StructToLogMessage Start ==");

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
            VMT_DataMgr.LogMessage(strLog);
            VMT_DataMgr.LogMessage("== Snd StructToLogMessage End ==");
        }

        // Application Init
        static public bool CreateVMTClient(Boolean isStandAlone = false, Boolean isTestMode = false)
        {
            LogMessage("== CreateVMTClient Start ==");

            if (UserInfo.IsUseHessian)
            {
                //--------------------------------------------------
                //- Register Exception Delegate
                HessianAPI.AddExceptionDelegator(VMT_DataMgr_Common_HessianCallback.ExceptionDelete_VMT_Data);

                int nPort;
                Int32.TryParse(gHessianServerPort, out nPort);
                HessianAPI.Init(gHessianServerIP, nPort, VMT_DataMgr.HessianCommResultCallback);
                HessianAPI.Start();
                HessianAPI.StartPriority();

                //Hessian Mgt
                HessianMgtAPI.AddExceptionDelegator(VMT_DataMgr_Common_HessianCallback.ExceptionDelete_VMT_Data);

                int nPortMgt;
                Int32.TryParse(gHessianServerMgtPort, out nPortMgt);
                HessianMgtAPI.Init(gHessianServerMgtIP, nPortMgt, VMT_DataMgr.HessianCommMgtResultCallback);
                HessianMgtAPI.Start();

                //if (isStandAlone)
                //{
                //    if (isTestMode)
                //    { }
                //    else
                //        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.KeepAliveStandAlone);
                //}
                //else
                //{
                //    if (UserInfo.gMchnTp == "YT")
                //        VMT_DataMgr_Common.StartPollingPriority_Ask(HessianCommType.KeepAlive);
                //    else
                //        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.KeepAlive);
                //}

                //if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                //    VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.DGPSAlive);
            }

            //if (UserInfo.IsUseTCP)
            //{
            //    //--------------------------------------------------
            //    //- Register Exception Delegate

            //    TCPAPI.AddExceptionDelegator(VMT_DataMgr_Common_Callback.ExceptionDelete_VMT_Data);
            //    int tcpPort;
            //    Int32.TryParse(gTCPLocalPort, out tcpPort);
            //    TCPAPI.Init(gTCPLocalIP, tcpPort, TCPCommonSendCallback, TCPCommonReceiveCallback);
            //    TCPAPI.Start();
            //    TCPAPI.TCPSocketOpen();
            //}

            //if (UserInfo.IsUseTCP && isTestMode == true)
            //{
            //    _socketServer = new WPSocketComm.Socket_TCP.socketServerTCP();
            //    int tcpPort;
            //    Int32.TryParse(gTCPBypassPort, out tcpPort);
            //    _socketServer.socketServerInit(tcpPort);
            //    _socketServer.startLisner();
            //    _socketServer.tcpRcvMsgEvt += new WPSocketComm.Socket_TCP.socketServerTCP.TcpSrvRcvMsgEventHandler(_socketServer_tcpRcvMsgEvt);
            //}

            if (UserInfo.IsUseUDP)
            {
                //--------------------------------------------------
                //- Register Exception Delegate
                UDPAPI.AddExceptionDelegator(VMT_DataMgr_Common_Callback.ExceptionDelete_VMT_Data);
                UDPAPI.Init(Convert.ToInt32(gUDPClientPort), Convert.ToInt32(gUDPServerPort), UDPCommonSendCallback, UDPCommonReceiveCallback);
                UDPAPI.Start();
                UDPAPI.UDPSocketOpen();
            }

            LogMessage("== CreateVMTClient End ==");

            return true;
        }

        static void _socketServer_tcpRcvMsgEvt(object sender, byte[] rcvMsg)
        {
            //TCPAPI.TCPCommExcute(TCPCommType.TCPBypassMessage, rcvMsg, 0);
        }

        // Application MainView Init
        static public void MainView_Init(String strServiceCompany, Boolean needJobPolling = true)
        {
            LogMessage("== MainView Init Start ==");

            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Clear();

            if (UserInfo.IsUseHessian)
            {
                if (needJobPolling)
                {
                    VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineJobByKeys_New);
                    VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineStatusChanged);
                    if (UserInfo.gMchnTp.Equals("TC") && !UserInfo.gUserID.Equals("CLT")) //ADD 3rd pilot issue - This is not from Pilot operation, from me.
                    {
                        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.CheckPLCData);
                        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.CheckPLCTwistLock);
                    }
                }

                VMT_DataMgr_Common.GetMachineStop_Ask();

                if (strServiceCompany.Equals("BTP"))
                {
                    VMT_DataMgr_Common.StartPolling_Ask(HessianComm.HessianCommType.GetMachineStop);

                    if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                    {
                        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineNotice);
                        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.SetPinningStation);
                    }
                    else
                    {
                        VMT_DataMgr_Common.GetBlockList_Ask();
                        VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineNotice); // 20190806
                    }
                }
            }

            if (UserInfo.IsUseTCP)
            {
                VMT_DataMgr_Common.GetMachineStop_Ask();
                VMT_DataMgr_Common.ConnectionStatus_Ask();
            }

            LogMessage("== MainView Init End ==");
        }

        // Application Terminate
        static public void DestroyVMTClient()
        {
            if (UserInfo.IsUseHessian)
                HessianAPI.End();

            //if (UserInfo.IsUseTCP)
            //    TCPAPI.End();
        }
        
        //-----------------------------------------------------------------------------
         //- Hessian Mgt Callback
        static private void HessianCommMgtResultCallback(HessianCommMgtType type, Object obj)
        {
            DateTime typeDateTime = DateTime.Now;
            TimeSpan tSpan = DateTime.Now - typeDateTime;

            try
            {
                LogMessage("HessianCommMgtResultCallback Type : " + type.ToString());

                //System.Diagnostics.Trace.WriteLine("[VMT RMG Callback Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + type.ToString() + "(+)");

                switch (type)
                {
                    case HessianCommMgtType.getSwapList:                        
                            VMT_DataMgr_RMG_HessianCallback.GetSwapList(obj);
                        break;
                    case HessianCommMgtType.setEmptySwap:                        
                            VMT_DataMgr_RMG_HessianCallback.SetEmptySwap(ref obj);
                        break;
                    /////////////////////////

                    default: break;
                }

                //System.Diagnostics.Trace.WriteLine("[VMT RMG Callback Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + type.ToString() + "(-)");
                //System.Diagnostics.Trace.WriteLine("[VMT RMG Callback Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());
            }
            catch (Exception ex)
            {                
                Common.Util.Logger.Log("[HessianCommResultCallback Exception]" + DateTime.Now.ToString("[HH:mm:ss:fff] ") + type.ToString() + " : " + ex.Message);
            }
        }

        //- Hessian Callback
        static private void HessianCommResultCallback(HessianCommType type, Object obj, bool isSend = false)
        {
            DateTime typeDateTime = DateTime.Now;
            TimeSpan tSpan = DateTime.Now - typeDateTime;

            try
            {
                LogMessage("HessianCommResultCallback Type : " + type.ToString());

                //System.Diagnostics.Trace.WriteLine("[VMT RMG Callback Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + type.ToString() + "(+)");

                // Aug 09 2023 HandleLog
                if (isSend)
                {
                    VMT_DataMgr_Common_HessianCallback.HandleLogApi(true, type, obj);
                    return;
                }
                else
                {
                    VMT_DataMgr_Common_HessianCallback.HandleLogApi(false, type, obj);
                }

                switch (type)
                {
                    case HessianCommType.KeepAlive: { VMT_DataMgr_Common_HessianCallback.KeepAlive(ref obj); } break;
                    case HessianCommType.KeepAliveStandAlone: { VMT_DataMgr_Common_HessianCallback.KeepAliveStandAlone(ref obj); } break;
                    case HessianCommType.DGPSAlive: { VMT_DataMgr_Common_HessianCallback.DGPSAlive(ref obj); } break;
                    case HessianCommType.GetUserAccessRole: { VMT_DataMgr_Common_HessianCallback.GetUserAccessRole(ref obj); } break;
                    case HessianCommType.ChangeDriver: { VMT_DataMgr_ITV_HessianCallback.ChangeDriver(obj); } break;
                    //case HessianCommType.GetLoginInfo4Machine: { GetLoginInfo4Machine(ref obj); } break;
                    case HessianCommType.SetLogin4Machine: { VMT_DataMgr_Common_HessianCallback.SetLogin4Machine(ref obj); } break;
                    case HessianCommType.ChangeDriverCheck: { VMT_DataMgr_Common_HessianCallback.ChangeDriverCheck(obj); } break;
                    case HessianCommType.SetMachineStatusChanged: { VMT_DataMgr_Common_HessianCallback.SetMachineStatusChanged(ref obj); } break;
                    case HessianCommType.SetLogout4Machine: { VMT_DataMgr_Common_HessianCallback.SetLogout4Machine(ref obj); } break;

                    case HessianCommType.SetMachinePassed: { VMT_DataMgr_ITV_HessianCallback.SetMachinePassed(ref obj); } break;
                    case HessianCommType.SetManualActivation: { VMT_DataMgr_ITV_HessianCallback.SetManualActivation(ref obj); } break;
                    case HessianCommType.SetMachineArrival: { VMT_DataMgr_ITV_HessianCallback.SetMachineArrival(obj); } break;
                    case HessianCommType.SetMachineReady: { VMT_DataMgr_ITV_HessianCallback.SetMachineReady(obj); } break;
                    case HessianCommType.SetItvDone: { VMT_DataMgr_ITV_HessianCallback.SetItvDone(obj); } break;
                    case HessianCommType.SetQCJobReleaseByYt: { VMT_DataMgr_ITV_HessianCallback.SetQCJobReleaseByYt(obj); } break;
                    case HessianCommType.ChangeChassisNo: { VMT_DataMgr_ITV_HessianCallback.ChangeChassisNo(obj); } break;
                    case HessianCommType.GetChssUsingData: { VMT_DataMgr_ITV_HessianCallback.GetChssUsingData(obj); } break;

                    case HessianCommType.GetMachineStopCodeList: { VMT_DataMgr_Common_HessianCallback.GetMachineStopCodeList(ref obj); } break;
                    case HessianCommType.GetMachineAccessAction: { VMT_DataMgr_Common_HessianCallback.GetMachineAccessAction(ref obj); } break;
                    case HessianCommType.GetBlockListForYardSector: { VMT_DataMgr_Common_HessianCallback.GetBlockListForYardSector(ref obj); } break;
                    case HessianCommType.GetMachineStop: { VMT_DataMgr_Common_HessianCallback.TOSGetMachineStop(ref obj); } break;
                    case HessianCommType.SetMachineStop:
                        {
                            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                                VMT_DataMgr_Common_HessianCallback.SetMachineStop(ref obj);
                            else //if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_RMG)
                                VMT_DataMgr_RMG_HessianCallback.SetMachineStop(ref obj);
                        }
                        break;
                    case HessianCommType.GetMachineNotice: { VMT_DataMgr_Common_HessianCallback.GetMachineNotice(ref obj); } break;
                    case HessianCommType.SetMachineNotice: { VMT_DataMgr_Common_HessianCallback.SetMachineNotice(ref obj); } break;

                    case HessianCommType.GetPrecedingYtList: { VMT_DataMgr_ITV_HessianCallback.GetPrecedingYtList(ref obj); } break;
                    case HessianCommType.SetConfirmJobByScanner: { VMT_DataMgr_ITV_HessianCallback.SetConfirmJobByScanner(ref obj); } break;

                    case HessianCommType.GetInventoryList: 
                    case HessianCommType.GetInventoryListMulti: 
                        { VMT_DataMgr_RMG_HessianCallback.GetInventoryList(obj); } 
                        break;
                    case HessianCommType.GetInventoryListEx: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListEx(obj); } break;
                    case HessianCommType.GetInventoryListMulti_New: { VMT_DataMgr_RMG_HessianCallback.GetInventoryList_New(obj); } break;
                    case HessianCommType.GetInventoryList4Multi_Sync: { VMT_DataMgr_RMG_HessianCallback.GetInventoryList_New(obj); } break;
                    case HessianCommType.GetInventoryListMultiSwap_New: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListSwap_New(obj); } break;
                    case HessianCommType.GetInventoryListMulti_New1: { VMT_DataMgr_RMG_HessianCallback.GetInventoryList_New1(obj); } break;
                    case HessianCommType.GetInventoryListMulti_New2: { VMT_DataMgr_RMG_HessianCallback.GetInventoryList_New2(obj); } break;

                    case HessianCommType.GetInventoryListBackground: 
                    case HessianCommType.GetInventoryListBackgroundMulti: 
                        { VMT_DataMgr_RMG_HessianCallback.GetInventoryListBackground(obj); } 
                        break;
                    case HessianCommType.GetInventoryListBackgroundEx: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListBackgroundEx(obj); } break;
                    case HessianCommType.GetInventoryListBackgroundMulti_New: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListBackground_New(obj); } break;

                    case HessianCommType.GetInventoryListData: 
                    case HessianCommType.GetInventoryListDataMulti: 
                        { VMT_DataMgr_RMG_HessianCallback.GetInventoryListData(obj); } 
                        break;
                    case HessianCommType.GetInventoryListDataEx: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListDataEx(obj); } break;
                    case HessianCommType.GetInventoryListDataMulti_New: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListData_New(obj); } break;
                    case HessianCommType.GetInventoryListDataMultiSwap_New: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListDataSwap_New(obj); } break;
                    case HessianCommType.GetInventoryListDataMulti_New1: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListData_New1(obj); } break;
                    case HessianCommType.GetInventoryListDataMulti_New2: { VMT_DataMgr_RMG_HessianCallback.GetInventoryListData_New2(obj); } break;

                    case HessianCommType.GetJobOrderList:
                    case HessianCommType.GetJobOrderListByKeys:
                    case HessianCommType.GetMachineJobByKeys:
                        {
                            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                                VMT_DataMgr_ITV_HessianCallback.GetJobOrderList(ref obj);                                
                            else // if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_RMG)
                                VMT_DataMgr_RMG_HessianCallback.GetJobOrderList(obj);
                                
                        }
                        break;

                    case HessianCommType.GetMachineStatusChanged:
                        {
                            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                                VMT_DataMgr_ITV_HessianCallback.GetMachineStatusChanged(obj);
                            else
                                VMT_DataMgr_RMG_HessianCallback.GetMachineStatusChanged(obj);
                        }
                        break;
                    case HessianCommType.GetJobOrderList_New:
                        {
                            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                                VMT_DataMgr_ITV_HessianCallback.GetJobOrderList_New(ref obj);
                            else // if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_RMG)                             
                                VMT_DataMgr_RMG_HessianCallback.GetJobOrderList_RMG(obj);
                        }
                        break;
                    case HessianCommType.GetJobOrderListByKeys_New:
                    case HessianCommType.GetMachineJobByKeys_New:
                        {
                            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                                VMT_DataMgr_ITV_HessianCallback.GetJobOrderList_New(ref obj);
                            else // if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_RMG)                             
                                VMT_DataMgr_RMG_HessianCallback.GetJobOrderList_New(obj);                           
                        }
                        break;
                    case HessianCommType.GetMachineJobByKeys_Sync:
                        {
                            if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_ITV)
                                VMT_DataMgr_ITV_HessianCallback.GetJobOrderList_New(ref obj);
                            else // if (UserInfo.GetMchnTp() == UserInfo.MchnTp.MchnTp_RMG)                             
                                VMT_DataMgr_RMG_HessianCallback.GetJobOrderList_New(obj);
                        }
                        break;
                    case HessianCommType.GetBlockList: { VMT_DataMgr_Common_HessianCallback.GetBlockList(ref obj); } break;
                    case HessianCommType.GetBlockListForBlockMap: { VMT_DataMgr_Common_HessianCallback.GetBlockListForBlockMap(ref obj); } break;
                    case HessianCommType.GetVmtAutoStartConfig: { VMT_DataMgr_Common_HessianCallback.GetVmtAutoStartConfig(ref obj); } break;
                    case HessianCommType.GetEmptySwappingTargetList: { VMT_DataMgr_RMG_HessianCallback.GetEmptySwappingTargetList(ref obj); } break;
                    case HessianCommType.DoEmptySwap: { VMT_DataMgr_RMG_HessianCallback.DoEmptySwap(ref obj); } break;
                    case HessianCommType.GetBlockMapListForYt: { VMT_DataMgr_Common_HessianCallback.GetBlockMapListForYt(ref obj); } break;
                    case HessianCommType.GetBlockMapList: { VMT_DataMgr_Common_HessianCallback.GetBlockMapList(ref obj); } break;
                    case HessianCommType.GetBlockMapList1: { VMT_DataMgr_Common_HessianCallback.GetBlockMapList1(ref obj); } break;
                    case HessianCommType.GetBlockMapList2: { VMT_DataMgr_Common_HessianCallback.GetBlockMapList2(ref obj); } break;
                    case HessianCommType.GetBlockMapSwapList: { VMT_DataMgr_Common_HessianCallback.GetBlockMapSwapList(ref obj); } break;

                    case HessianCommType.GetMachineList: { VMT_DataMgr_RMG_HessianCallback.GetMachineList(ref obj); } break;
                    case HessianCommType.GetChangedMachineLocation: { VMT_DataMgr_RMG_HessianCallback.GetChangedMachineLocation(obj); } break;
                    case HessianCommType.GetMachineList4LogoutCheck: { VMT_DataMgr_RMG_HessianCallback.GetMachineList4LogoutCheck(ref obj); } break;
                    case HessianCommType.GetMachineListOfPool: { VMT_DataMgr_RMG_HessianCallback.GetMachineListOfPool(ref obj); } break;

                    case HessianCommType.GetJobOrderByContainer:
                    case HessianCommType.GetMachineJobByTruck:
                    case HessianCommType.GetJobOrderListByTruck:
                        { VMT_DataMgr_RMG_HessianCallback.GetJobOrderByContainer(ref obj); }
                        break;

                    case HessianCommType.GetJobOrderByContainer_New:
                    case HessianCommType.GetMachineJobByTruck_New:
                    case HessianCommType.GetJobOrderListByTruck_New:
                        { VMT_DataMgr_RMG_HessianCallback.GetJobOrderByContainer_New(ref obj); }
                        break;
                    case HessianCommType.getSwapListRTG:
                        VMT_DataMgr_RMG_HessianCallback.GetSwapListRTG(obj);
                        break;
                    case HessianCommType.getSwapList:
                        VMT_DataMgr_RMG_HessianCallback.GetSwapList(obj);
                        break;
                    case HessianCommType.setEmptySwap:
                        VMT_DataMgr_RMG_HessianCallback.SetEmptySwap(ref obj);
                        break;
                    case HessianCommType.SetDetwinJob: { VMT_DataMgr_RMG_HessianCallback.SetDetwinJob(ref obj); } break;
                    case HessianCommType.SetJobStatus: { VMT_DataMgr_RMG_HessianCallback.SetJobStatus(ref obj); } break;
                    case HessianCommType.SetPickedContainer: { VMT_DataMgr_RMG_HessianCallback.SetPickedContainer(ref obj); } break;
                    case HessianCommType.GetContainerInfo: { VMT_DataMgr_RMG_HessianCallback.GetContainerInfo(ref obj); } break;
                    case HessianCommType.GetTwinContainerInfo: { VMT_DataMgr_RMG_HessianCallback.GetTwinContainerInfo(ref obj); } break;
                    case HessianCommType.SetJobDone: { VMT_DataMgr_RMG_HessianCallback.SetJobDone(ref obj); } break;
                    case HessianCommType.SetJobDoneForLocationChange: { VMT_DataMgr_RMG_HessianCallback.SetJobDoneForLocationChange(ref obj); } break;
                    case HessianCommType.DoSwap4Manual: { VMT_DataMgr_Common_HessianCallback.DoSwap4Manual(ref obj); } break;
                    case HessianCommType.GetMaxRow: { VMT_DataMgr_RMG_HessianCallback.GetMaxRow(ref obj); } break;
                    case HessianCommType.GetNoWorkArea: { VMT_DataMgr_RMG_HessianCallback.GetNoWorkArea(ref obj); } break;
                    case HessianCommType.GetNoWorkArea1: { VMT_DataMgr_RMG_HessianCallback.GetNoWorkArea1(ref obj); } break;
                    case HessianCommType.GetNoWorkArea2: { VMT_DataMgr_RMG_HessianCallback.GetNoWorkArea2(ref obj); } break;
                    case HessianCommType.GetNoWorkTier: { VMT_DataMgr_RMG_HessianCallback.GetNoWorkTier(ref obj); } break;
                    case HessianCommType.GetNoWorkTier1: { VMT_DataMgr_RMG_HessianCallback.GetNoWorkTier1(ref obj); } break;
                    case HessianCommType.GetNoWorkTier2: { VMT_DataMgr_RMG_HessianCallback.GetNoWorkTier2(ref obj); } break;
                    case HessianCommType.SetChangePosition: { VMT_DataMgr_RMG_HessianCallback.SetChagePosition(obj); } break;
                    case HessianCommType.GetIsValidLocation: { VMT_DataMgr_RMG_HessianCallback.GetIsValidLocation(obj); } break;
                    case HessianCommType.SetJobReOnChassis: { VMT_DataMgr_RMG_HessianCallback.SetJobReOnChassis(obj); } break;
                    case HessianCommType.GetDriverJobHistory: { VMT_DataMgr_RMG_HessianCallback.GetDriverJobHistory(obj); } break; // Get Driver Job History
                    case HessianCommType.CheckYcDeTwin: { VMT_DataMgr_RMG_HessianCallback.CheckYcDeTwin(obj); } break;
                    case HessianCommType.SetBestPick: { VMT_DataMgr_RMG_HessianCallback.SetBestPick(obj); } break;
                    case HessianCommType.Validate4LoadingSwapping: { VMT_DataMgr_RMG_HessianCallback.Validate4LoadingSwapping(obj); } break;
                    case HessianCommType.ExceptionOccured: { VMT_DataMgr_Common_HessianCallback.ExceptionOccured(ref obj); } break;
                    case HessianCommType.SetReallocation: { VMT_DataMgr_ITV_HessianCallback.SetReallocation(obj); } break;
                    case HessianCommType.ChassisOrderComplete: { VMT_DataMgr_ITV_HessianCallback.ChassisOrderComplete(obj); } break;
                    case HessianCommType.ValidChassisInfos: { VMT_DataMgr_ITV_HessianCallback.ValidChassisInfos(obj); } break;
                    case HessianCommType.ItvLinkChassis: { VMT_DataMgr_ITV_HessianCallback.ItvLinkChassis(obj); } break;
                    case HessianCommType.ItvUnLinkChassis: { VMT_DataMgr_ITV_HessianCallback.ItvUnLinkChassis(obj); } break;
                    case HessianCommType.ReleaseYtFromJob: { VMT_DataMgr_ITV_HessianCallback.ReleaseYtFromJob(obj); } break;
                    case HessianCommType.SetPLCAutoFlg: { VMT_DataMgr_ITV_HessianCallback.SetPLCAutoFlg(obj); } break;
                    case HessianCommType.CheckPLCData: { VMT_DataMgr_RMG_HessianCallback.CheckPLCData(obj); } break;
                    case HessianCommType.CheckPLCTwistLock: { VMT_DataMgr_RMG_HessianCallback.CheckPLCTwistLock(obj); } break;
                    case HessianCommType.InitPLCMessage: { VMT_DataMgr_RMG_HessianCallback.InitPLCMessage(obj); } break;
                    case HessianCommType.ProcessPLC: { VMT_DataMgr_RMG_HessianCallback.ProcessPLC(obj); } break;
                    case HessianCommType.CancelPLC: { VMT_DataMgr_RMG_HessianCallback.CancelPLC(obj); } break;
                    case HessianCommType.ReleasePLCLock: { VMT_DataMgr_RMG_HessianCallback.ReleasePLCLock(obj); } break;
                    case HessianCommType.SetGateCancelJob: { VMT_DataMgr_ITV_HessianCallback.SetGateCancelJob(obj); } break;
                    case HessianCommType.GetConfigValue: { VMT_DataMgr_Common_HessianCallback.GetConfigValue(ref obj); } break;
                    /////////////////////////
                    // - Single Test
                    case HessianCommType.KeepAlive_Test: { VMT_DataMgr_Common_HessianCallback.KeepAlive_Test(ref obj); } break;
                    case HessianCommType.GetInventoryList_Test: { VMT_DataMgr_Common_HessianCallback.GetInventoryList_Test(ref obj); } break;
                    case HessianCommType.GetInventory_Test: { VMT_DataMgr_Common_HessianCallback.GetInventory_Test(ref obj); } break;
                    case HessianCommType.GetBlockMapList_Test: { VMT_DataMgr_Common_HessianCallback.GetBlockMapList_Test(ref obj); } break;
                    case HessianCommType.SetMachineStatusChanged_Test: { VMT_DataMgr_Common_HessianCallback.SetMachineStatusChanged_Test(ref obj); } break;
                    case HessianCommType.SetMachineStop_Test: { VMT_DataMgr_Common_HessianCallback.SetMachineStop_Test(ref obj); } break;
                    case HessianCommType.SetMachinePassed_Test: { VMT_DataMgr_Common_HessianCallback.SetMachinePassed_Test(ref obj); } break;
                    case HessianCommType.SetManualActivation_Test: { VMT_DataMgr_Common_HessianCallback.SetManualActivation_Test(ref obj); } break;
                    case HessianCommType.SetMachineArrivalInfo_Test: { VMT_DataMgr_Common_HessianCallback.SetMachineArrivalInfo_Test(ref obj); } break;
                    case HessianCommType.SetMachineReady_Test: { VMT_DataMgr_Common_HessianCallback.SetMachineReady_Test(ref obj); } break;
                    case HessianCommType.GetMachineStop_Test: { VMT_DataMgr_Common_HessianCallback.GetMachineStop_Test(ref obj); } break;
                    case HessianCommType.GetMachineNotice_Test: { VMT_DataMgr_Common_HessianCallback.GetMachineNotice_Test(ref obj); } break;
                    case HessianCommType.SetMachineNotice_Test: { VMT_DataMgr_Common_HessianCallback.SetMachineNotice_Test(ref obj); } break;
                    case HessianCommType.GetPrecedingYtList_Test: { VMT_DataMgr_Common_HessianCallback.GetPrecedingYtList_Test(ref obj); } break;
                    case HessianCommType.SetJobDone_Test: { VMT_DataMgr_Common_HessianCallback.SetJobDone_Test(ref obj); } break; // Status : Twist Lock(Pick-up), Twist Unlock(put-down)
                    case HessianCommType.GetJobOrderList_Test: { VMT_DataMgr_Common_HessianCallback.GetJobOrderList_Test(ref obj); } break;
                    case HessianCommType.GetUserAccessRole_Test: { VMT_DataMgr_Common_HessianCallback.GetUserAccessRole_Test(ref obj); } break;
                    case HessianCommType.SetLogin4Machine_Test: { VMT_DataMgr_Common_HessianCallback.SetLogin4Machine_Test(ref obj); } break;
                    case HessianCommType.SetLogout4Machine_Test: { VMT_DataMgr_Common_HessianCallback.SetLogout4Machine_Test(ref obj); } break;
                    case HessianCommType.GetMachineStopCodeList_Test: { VMT_DataMgr_Common_HessianCallback.GetMachineStopCodeList_Test(ref obj); } break;
                    // 2015-12-28 추가
                    case HessianCommType.GetJobOrderByContainer_Test: { VMT_DataMgr_Common_HessianCallback.GetJobOrderByContainer_Test(ref obj); } break;
                    case HessianCommType.GetMachineList_Test: { VMT_DataMgr_Common_HessianCallback.GetMachineList_Test(ref obj); } break;
                    case HessianCommType.GetMachineListOfPool_Test: { VMT_DataMgr_Common_HessianCallback.GetMachineListOfPool_Test(ref obj); } break;
                    case HessianCommType.DoSwap4Manual_Test: { VMT_DataMgr_Common_HessianCallback.DoSwap4Manual_Test(ref obj); } break;
                    case HessianCommType.GetBlockList_Test: { VMT_DataMgr_Common_HessianCallback.GetBlockList_Test(ref obj); } break;
                    case HessianCommType.SetDetwinJob_Test: { VMT_DataMgr_Common_HessianCallback.SetDetwinJob_Test(ref obj); } break;
                    /////////////////////////

                    default: break;
                }

                //System.Diagnostics.Trace.WriteLine("[VMT RMG Callback Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + type.ToString() + "(-)");
                //System.Diagnostics.Trace.WriteLine("[VMT RMG Callback Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());
            }
            catch (Exception ex)
            {                
                Common.Util.Logger.Log("[HessianCommResultCallback Exception]" + DateTime.Now.ToString("[HH:mm:ss:fff] ") + type.ToString() + " : " + ex.Message);
            }
        }

        //-----------------------------------------------------------------------------
        //- UDP Callback
        static private void UDPCommonSendCallback(UDPCommType type, Object obj)
        {
            try
            {
                VMT_DataMgr_Common_Callback.UDPCommonSendCallback(type, obj);
            }
            catch (Exception ex)
            {
                String errorMessage = ex.Message;
            }
        }

        static private void UDPCommonReceiveCallback(UDPCommType type, Object obj)
        {
            try
            {
                VMT_DataMgr_Common_Callback.UDPCommonReceiveCallback(type, obj);
            }
            catch (Exception ex)
            {
                String errorMessage = ex.Message;
            }
        }
        //-----------------------------------------------------------------------------
    }

    public class Util
    {
        #region [help function]
        //-------------------------------------------------------------------------
        //- Data Conversion Static Method
        static public string TranscodeByteArrayToString(byte[] btData)
        {
            string strRet = "";

            strRet = Encoding.ASCII.GetString(btData, 0, btData.Length);

            return strRet;
        }

        static public byte[] TranscodeStringToByte(string strData, int padding = 12)
        {
            byte[] btHEX = new byte[padding];

            // Set to Value Zero
            for (int i = 0; i < btHEX.Length; i++)
                btHEX[i] = 0x00;

            byte[] bTransHEX = Encoding.ASCII.GetBytes(strData);

            for (int i = 0; i < btHEX.Length && i < bTransHEX.Length; i++)
                btHEX[i] = bTransHEX[i];

            return btHEX;
        }

        static public string TranscodeByteArrayToHexString(byte[] btData, bool bDash = true)
        {
            string strHex = "";

            strHex = BitConverter.ToString(btData);

            if (!bDash)
                strHex = strHex.Replace("-", string.Empty);

            return strHex;
        }

        static public byte[] TranscodeHEXStringToByteArray(string strHex)
        {
            if (strHex.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", strHex));
            }

            byte[] HexAsBytes = new byte[strHex.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = strHex.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }


        static public T DeepCopy<T>(T obj)
        {
            if (obj == null)
                return (T)obj;


            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter s = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                s.Serialize(ms, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                ms.Position = 0;
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                T t = (T)s.Deserialize(ms);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                s = null;
                return t;
            }
        }


        static public string ExportMessageObjToXml<T>(XmlDocument xmlDoc, string strFunctionName, T obj)
        {
            if (xmlDoc == null)
                return "";

            //XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                // Make XML Message Instance from input Template Class Object
                XmlDocument xmlMessage = new XmlDocument();
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlMessage.Load(ms);


                //-------------------------------------------------------------
                //- Create InterfaceMessage Element
                XmlElement msgEle = xmlDoc.CreateElement("InterfaceMessage");
                msgEle.SetAttribute("function", strFunctionName);
                msgEle.SetAttribute("parameter", obj.ToString()); // xmlMessage.DocumentElement.Name
                //msgEle.SetAttribute("parameter", xmlMessage.DocumentElement.Name);
                msgEle.SetAttribute("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                // Import Message XML Nodes for copying to Target XML Documnet
                XmlNode copyNode = xmlDoc.ImportNode(xmlMessage.DocumentElement, true);
                while (copyNode.ChildNodes.Count > 0)
                {
                    msgEle.AppendChild(copyNode.FirstChild);
                }

                // Appending new Message XML to XML Documnet
                xmlDoc.DocumentElement.AppendChild(msgEle);
                //XmlNode refChild = xmlDoc.DocumentElement.LastChild;
                //xmlDoc.DocumentElement.InsertAfter(msgEle, refChild);
            }


            return xmlDoc.InnerXml;
        }

        static private String _strXmlRootEndTag = "</InterfaceMessageList>";
        static public void ExportMessageObjToXml_Refactoring<T>(String strFilePath, String strFunctionName, T obj)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                // Make XML Message Instance from input Template Class Object
                XmlDocument xmlMessage = new XmlDocument();
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlMessage.Load(ms);

                //-------------------------------------------------------------
                //- Create InterfaceMessage Element
                XmlDocument _xmlDoc = new XmlDocument();
                XmlElement msgEle = _xmlDoc.CreateElement("InterfaceMessage");
                msgEle.SetAttribute("function", strFunctionName);
                msgEle.SetAttribute("parameter", obj.ToString()); // xmlMessage.DocumentElement.Name                
                msgEle.SetAttribute("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                // Import Message XML Nodes for copying to Target XML Documnet
                XmlNode copyNode = _xmlDoc.ImportNode(xmlMessage.DocumentElement, true);
                while (copyNode.ChildNodes.Count > 0)
                {
                    msgEle.AppendChild(copyNode.FirstChild);
                }

                using (FileStream f = new FileStream(strFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Search Insert Position
                    f.Seek(-_strXmlRootEndTag.Length, SeekOrigin.End); // Delete
                    using (StreamWriter s = new StreamWriter(f))
                    {
                        s.AutoFlush = true;
                        s.Write(msgEle.OuterXml);
                        s.Write("\r\n");
                        s.Write(_strXmlRootEndTag); // Add
                    }
                }
                msgEle.RemoveAll();
                _xmlDoc = null;
                xmlMessage = null;

            }
        }

        static public Object CreateMarshalDynamicLegthClass<T>(Byte[] orgIntPtr, ref int offset, Boolean bList, int nSize = 1)
        {
            Object retValue = null;

            try
            {
                Type type = typeof(T);

                if (bList == true)
                    retValue = new List<T>();

                for (int nCount = 0; nCount < nSize; nCount++)
                {
                    int stSize = Marshal.SizeOf(type);
                    byte[] arr = new byte[stSize];
                    IntPtr buff = Marshal.AllocHGlobal(stSize);
                    Marshal.Copy(orgIntPtr, offset, buff, stSize);

                    if (bList)
                    {
                        (retValue as IList).Add(Marshal.PtrToStructure(buff, type));
                    }
                    else
                    {
                        retValue = (T)Marshal.PtrToStructure(buff, type);
                    }

                    Marshal.FreeHGlobal(buff);
                    offset += stSize;
                }
            }
            catch (Exception ex)
            {
                String strError = ex.Message;
            }

            return retValue;
        }

        static public T Parse<T>(String strValue)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    //Cast ConvertFromString(string text) : object to (T)
                    return (T)converter.ConvertFromString(strValue);
                }
            }
            catch (Exception ex)
            {
                String errMsg = ex.Message;
                return default(T);
            }
            return default(T);
        }
        //-------------------------------------------------------------------------

        static public event logEvtHandle logEvt;
        public delegate void logEvtHandle(String strValue);
        static public void LogMessage(String strValue)
        {
            if (logEvt != null)
                logEvt(strValue);
        }

        static public String HashtableToXmlString(Hashtable obj)
        {
            List<KeyValuePair> tempKeyValuePair = new List<KeyValuePair>(obj.Count);
            foreach (String key in obj.Keys)
            {
                if (obj[key] is Hashtable)
                {
                    tempKeyValuePair.Add(new KeyValuePair(key, HashtableToXmlString(obj[key] as Hashtable)));
                }
                else
                {
                    tempKeyValuePair.Add(new KeyValuePair(key, obj[key].ToString()));
                }
            }

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<KeyValuePair>));
            StringWriter stringWriter = new StringWriter();
            System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(stringWriter, tempKeyValuePair, ns);
            return stringWriter.ToString();

            /*
            //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                // Make XML Message Instance from input Template Class Object
                XmlDocument xmlMessage = new XmlDocument();
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlMessage.Load(ms);

                //-------------------------------------------------------------
                //- Create InterfaceMessage Element
                XmlDocument _xmlDoc = new XmlDocument();
                XmlElement msgEle = _xmlDoc.CreateElement("Hashtable");

                // Import Message XML Nodes for copying to Target XML Documnet
                XmlNode copyNode = _xmlDoc.ImportNode(xmlMessage.DocumentElement, true);
                while (copyNode.ChildNodes.Count > 0)
                {
                    msgEle.AppendChild(copyNode.FirstChild);
                }

                return msgEle.OuterXml;
            }
            */
        }

        #endregion [help function]
    }

    public class KeyValuePair
    {
        public string Key;
        public Object Value;

        public KeyValuePair(string key, Object value)
        {
            Key = key;
            Value = value;
        }
    }
}