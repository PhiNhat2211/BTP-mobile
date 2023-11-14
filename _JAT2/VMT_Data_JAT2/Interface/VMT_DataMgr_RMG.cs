using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using TCPComm;
using Common.Util;
//using TCPComm.EEStruct;
using System.Runtime.InteropServices;
using VMT_Data_JAT2.Objects;
using System.Reflection;
using HessianComm.Objects;
using HessianComm;
using hessiancsharp.Class;
using System.IO;

namespace VMT_Data_JAT2
{
    public class VMT_DataMgr_RMG
    {
        static private void LogMessage(String strValue)
        {
            strValue = "[VMT_DataMgr_RMG] " + strValue;
            Util.LogMessage(strValue);
        }

        static private void StructToLogMessage(Object obj)
        {
            VMT_DataMgr_RMG.LogMessage("== Snd StructToLogMessage Start ==");

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
            VMT_DataMgr_RMG.LogMessage(strLog);
            VMT_DataMgr_RMG.LogMessage("== Snd StructToLogMessage End ==");
        }

        public static void SaveLog(string sJob) 
        {
            try
            {
                //string sRootPath = AppDomain.CurrentDomain.BaseDirectory;
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

        //-----------------------------------------------------------------------------
        //- Method
        static public void ManualReady_Ask(ref RMG.VD_RMG_ManualReady_Send value)
        {
            LogMessage("ManualReady_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_ManualReadyRMG manualReadyRMG = new TCPComm.EEStruct.sTrayUI_ManualReadyRMG();
                //manualReadyRMG.jobKey = value.jobKey;
                //manualReadyRMG.ITVMachineID = value.ITVMachineID;
                //manualReadyRMG.bReadyOnOff = value.bReadyOnOff;

                //TCPComm.TCPAPI.RMG_ManualReady(manualReadyRMG);
            }
        }

        static public void BlockInfo_Ask(ref RMG.VD_RMG_GetBlockInfo_Send value) // Block, Bay 정보를 요청합니다.
        {
            LogMessage("BlockInfo_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_BlockInfoRq blockInfoRq = new TCPComm.EEStruct.sTrayUI_BlockInfoRq();
                //blockInfoRq.szBlockName = value.szBlockName;
                //blockInfoRq.bayNo = value.bayNo;

                //TCPComm.TCPAPI.RMG_SendBlockInfo(blockInfoRq);
            }
        }

        static public void BlockInfoSimple_Ask(ref RMG.VD_RMG_GetBlockInfo_Send value)
        {
            LogMessage("BlockInfoSimple_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_BlockInfoRq blockInfoRq = new TCPComm.EEStruct.sTrayUI_BlockInfoRq();
                //blockInfoRq.szBlockName = value.szBlockName;
                //blockInfoRq.bayNo = value.bayNo;

                //TCPComm.TCPAPI.RMG_SendBlockInfoSimple(blockInfoRq);
            }
        }

        static public void Correction_Ask(ref RMG.VD_RMG_Correction_Send value)
        {
            LogMessage("Correction_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_Correction correction = new TCPComm.EEStruct.sTrayUI_Correction();
                //correction.cntrNo = value.cntrNo;
                //correction.fromLoc = new EEv2_Yard_Location();
                //correction.fromLoc.blck = value.fromLoc.blck;
                //correction.fromLoc.bay = value.fromLoc.bay;
                //correction.fromLoc.row = value.fromLoc.row;
                //correction.fromLoc.tier = value.fromLoc.tier;
                //correction.toLoc = new EEv2_Yard_Location();
                //correction.toLoc.blck = value.toLoc.blck;
                //correction.toLoc.bay = value.toLoc.bay;
                //correction.toLoc.row = value.toLoc.row;
                //correction.toLoc.tier = value.toLoc.tier;
                //correction.actionType = value.actionType;

                //TCPComm.TCPAPI.RMG_SendCorrection(correction);
            }
        }

        static public void SetCurrentJob_Ask(ref RMG.VD_RMG_SetCurrentJob_Send value)
        {
            LogMessage("SetCurrentJob_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_SetCurrentJob blockInfoRq = new TCPComm.EEStruct.sTrayUI_SetCurrentJob();
                //blockInfoRq.jobKey = value.jobKey;

                //TCPComm.TCPAPI.RMG_SetCurrentJob(blockInfoRq);
            }
        }

        static public void HandleJobDone_Ask(ref RMG.VD_RMG_HandleJobDone_Send value)
        {
            LogMessage("HandleJobDone_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_HandleJobDone handleJobDone = new TCPComm.EEStruct.sTrayUI_HandleJobDone();
                //handleJobDone.jobKey = value.jobKey;
                //handleJobDone.WorkingMachineID = value.WorkingMachineID;
                //handleJobDone.PartnerMachineID = value.PartnerMachineID;
                //handleJobDone.cntrNo = value.cntrNo;
                //handleJobDone.Loc = new EEv2_Job_Location();
                //handleJobDone.Loc.blck = value.Loc.blck;
                //handleJobDone.Loc.bay = value.Loc.bay;
                //handleJobDone.Loc.row = value.Loc.row;
                //handleJobDone.Loc.tier = value.Loc.tier;
                //handleJobDone.sprd = new EEv3_Spreader();
                //handleJobDone.sprd.sprdMd = value.sprd.sprdMd;
                //handleJobDone.sprd.sprdTp = value.sprd.sprdTp;
                //handleJobDone.sprd.sprdSts = value.sprd.sprdSts;

                //TCPComm.TCPAPI.RMG_HandleJobDone(handleJobDone);
            }
        }

        static public void Marrying_Ask(ref RMG.VD_RMG_RMGMarrying_Send value)
        {
            LogMessage("Marrying_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //TCPComm.EEStruct.sTrayUI_RMGMarrying marrying = new TCPComm.EEStruct.sTrayUI_RMGMarrying();
                //marrying.WorkingMachineID = value.WorkingMachineID;
                //marrying.WorkingMachineTP = value.WorkingMachineTP;
                //marrying.PartnerMachineID = value.PartnerMachineID;
                //marrying.PartnerMachineITP = value.PartnerMachineTP;
                //marrying.cntrNo = value.cntrNo;
                //marrying.Loc = new EEv2_Job_Location();
                //marrying.Loc.locTp = value.Loc.locTp;
                //marrying.Loc.blck = value.Loc.blck;
                //marrying.Loc.bay = value.Loc.bay;
                //marrying.Loc.row = value.Loc.row;
                //marrying.Loc.tier = value.Loc.tier;
                //marrying.Loc.lane = value.Loc.lane;
                //marrying.Loc.location = value.Loc.location;

                //TCPComm.TCPAPI.RMG_Marrying(marrying);
            }
        }

        static public void OTR_ManualBlockInOut_Ask(ref RMG.VD_RMG_OTR_ManualBlockInOut_Send value) // Delete
        {
            LogMessage("OTR_ManualBlockInOut_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            { }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_sECH_OTR_ManualBlockInOut manualBlockInOut = new sTrayUI_sECH_OTR_ManualBlockInOut();
                //manualBlockInOut.m_JobKey = value.m_JobKey;
                //manualBlockInOut.m_btInOut = value.m_btInOut;
                //manualBlockInOut.m_OTR_RFIDTagInfo = new sTrayUI_sECH_PDS_RFID_Payload();
                //manualBlockInOut.m_OTR_RFIDTagInfo.m_cAntennaID = value.m_OTR_RFIDTagInfo.m_cAntennaID;
                //manualBlockInOut.m_OTR_RFIDTagInfo.m_cFlag = value.m_OTR_RFIDTagInfo.m_cFlag;
                //manualBlockInOut.m_OTR_RFIDTagInfo.m_cTagID = value.m_OTR_RFIDTagInfo.m_cTagID;
                //manualBlockInOut.m_OTR_RFIDTagInfo.m_dwTime = value.m_OTR_RFIDTagInfo.m_dwTime;

                //TCPComm.TCPAPI.ECH_OTR_ManualBlockInOut(manualBlockInOut);
            }
        }

        public static void GetInventoryList_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.locTp = new LocationType() { name = "YARD" };
                location.blck = value.blck;
                location.bay = value.bay;
                HessianComm.HessianAPI.GetInventoryList(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListEx_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)    // for odd bay
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.locTp = new LocationType() { name = "YARD" };
                location.blck = value.blck;
                location.bay = value.bay;
                HessianComm.HessianAPI.GetInventoryListEx(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListMulti_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                
                HessianComm.HessianAPI.GetInventoryListMulti_New(locationList);                
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryList4Multi_Sync_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                
                HessianComm.HessianAPI.GetInventoryList4Multi_Sync(locationList);                
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListByVirtualBlock_New(String virtualBlock)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                locationList.Add(virtualBlock);

                HessianComm.HessianAPI.GetInventoryListByVirtualBlock_New(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListMultiSwap_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }

                HessianComm.HessianAPI.GetInventoryListMultiSwap_New(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListBackground_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.locTp = new LocationType() { name = "YARD" };
                location.blck = value.blck;
                location.bay = value.bay;
                HessianComm.HessianAPI.GetInventoryListBackground(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListBackgroundEx_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.locTp = new LocationType() { name = "YARD" };
                location.blck = value.blck;
                location.bay = value.bay;
                HessianComm.HessianAPI.GetInventoryListBackgroundEx(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListBackgroundMulti_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                HessianComm.HessianAPI.GetInventoryListBackgroundMulti_New(locationList);                
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListData_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.locTp = new LocationType() { name = "YARD" };
                location.blck = value.blck;
                location.bay = value.bay;
                HessianComm.HessianAPI.GetInventoryListData(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListDataEx_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location value)
        {
            if (UserInfo.IsUseHessian)
            {
                Location location = new Location();
                location.locTp = new LocationType() { name = "YARD" };
                location.blck = value.blck;
                location.bay = value.bay;
                HessianComm.HessianAPI.GetInventoryListDataEx(location);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListDataMulti_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                HessianComm.HessianAPI.GetInventoryListDataMulti_New(locationList);                
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListMulti1_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }

                HessianComm.HessianAPI.GetInventoryListMulti_New1(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListMulti2_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }

                HessianComm.HessianAPI.GetInventoryListMulti_New2(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListDataMulti1_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                HessianComm.HessianAPI.GetInventoryListDataMulti_New1(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListDataMulti2_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                HessianComm.HessianAPI.GetInventoryListDataMulti_New2(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetInventoryListDataMultiSwap_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location> values)    // for multi bay
        {
            if (UserInfo.IsUseHessian)
            {
                var locationList = new System.Collections.ArrayList();
                foreach (var value in values)
                {
                    var location = String.Format("{0}-{1}", value.blck, value.bay);
                    //Location location = new Location();
                    //location.locTp = new LocationType() { name = "YARD" };
                    //location.blck = value.blck;
                    //location.bay = value.bay;
                    //locationList.Add(location.ToHessianHashtable());
                    locationList.Add(location);
                }
                HessianComm.HessianAPI.GetInventoryListDataMulti_New(locationList);
            }
            else if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetMachineList_Ask(String machineTp, bool history = false)
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.Machine machine = new HessianComm.Objects.Machine();
                machine.mchnTp = machineTp;
                machine.isOn = history;
                HessianComm.HessianAPI.GetMachineList(machine);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetChangedMachineLocation_Ask(String blck, String bay)
        {
            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();

                hessianList.Add(RMG.RMG_User.gMchnID);
                hessianList.Add(RMG.RMG_User.gMchnTp);
                hessianList.Add(blck);
                hessianList.Add(bay);
                HessianComm.HessianAPI.GetChangedMachineLocation(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        //public static void GetMachineList4LogoutCheck_Ask()
        //{
        //    if (UserInfo.IsUseHessian)
        //    {
        //        HessianComm.Objects.Machine machine = new HessianComm.Objects.Machine();
        //        machine.mchnId = UserInfo.gMchnID;
        //        machine.mchnTp = UserInfo.gMchnTp;
        //        HessianComm.HessianAPI.GetMachineList4LogoutCheck(machine);
        //    }

        //    if (UserInfo.IsUseTCP)
        //    {
        //    }
        //}

        public static void GetMachineListofPool_Ask(String machineID)
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.Machine machine = new HessianComm.Objects.Machine();
                machine.mchnId = machineID;//UserInfo.gMchnID;
                machine.mchnTp = "YT";//UserInfo.gMchnTp;
                HessianComm.HessianAPI.GetMachineListOfPool(machine);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetJobOrderByContainer_Ask(String value)
        {
            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetJobOrderByContainer_New(value);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void SetJobStatus_Ask(String jobKey, Boolean bActive)
        {
            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();

                HessianComm.Objects.JobStatusType jobStatus = new HessianComm.Objects.JobStatusType();
                jobStatus.name = bActive ? "JOB_PROCESSING" : "JOB_INACTIVE";
                hessianList.Add(jobKey);
                hessianList.Add(jobStatus.ToHessianHashtable());
                hessianList.Add(RMG.RMG_User.gMchnID);
                hessianList.Add(RMG.RMG_User.gUserID);
                HessianComm.HessianAPI.SetJobStatus(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void SetJobStatus_Ask(System.Collections.ArrayList jobKeyList, Boolean bActive)
        {
            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();

                HessianComm.Objects.JobStatusType jobStatus = new HessianComm.Objects.JobStatusType();
                jobStatus.name = bActive ? "JOB_PROCESSING" : "JOB_INACTIVE";
                hessianList.Add(jobKeyList);
                hessianList.Add(jobStatus.ToHessianHashtable());
                hessianList.Add(RMG.RMG_User.gMchnID);
                hessianList.Add(RMG.RMG_User.gUserID);
                HessianComm.HessianAPI.SetJobStatus(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        public static void GetIsValidLocation(String blck, String bay, String row, String tier)
        {
            var loc = new Location();
            loc.blck = blck;
            loc.bay = bay;
            loc.row = row;
            loc.tier = tier;
            HessianComm.HessianAPI.GetIsValidLocation(loc);            
        }

        public static void SetJobDone_Ask(VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send jobDoneSend, Boolean isLocationChange = false)
        {
            VmtWorkOrder vmtWorkOrder = new VmtWorkOrder();
            vmtWorkOrder.jobKey = jobDoneSend.jobKey;
            vmtWorkOrder.cntrNo = jobDoneSend.cntrNo;
            vmtWorkOrder.mchnId = UserInfo.gMchnID;
            vmtWorkOrder.mchnTp = UserInfo.gMchnTp;
            vmtWorkOrder.isMvmtIn = false;
            vmtWorkOrder.isGcBtn = jobDoneSend.isGcBtn;
            vmtWorkOrder.locTp = jobDoneSend.Loc.locTp;
            vmtWorkOrder.blck = jobDoneSend.Loc.blck;
            vmtWorkOrder.bay = jobDoneSend.Loc.bay;
            vmtWorkOrder.row = jobDoneSend.Loc.row;
            vmtWorkOrder.tier = jobDoneSend.Loc.tier;
            vmtWorkOrder.qcLn = jobDoneSend.qcLn;
            vmtWorkOrder.userId = UserInfo.gUserID;
            vmtWorkOrder.workingStatus = jobDoneSend.workingStatus;

            if (UserInfo.gMchnTp == "YT" || UserInfo.gMchnTp == "ITV")
                vmtWorkOrder.ytNo = UserInfo.gMchnID;
            else
            {
                vmtWorkOrder.ytNo = string.IsNullOrEmpty(jobDoneSend.jobKey) ? "" : jobDoneSend.PartnerMachineID;
            }
            vmtWorkOrder.positionOnChassis = jobDoneSend.positionOnChassis;

            //VmtWorkOrder task = new VmtWorkOrder();
            //task.jobKey = "THKM0000033120190211184321";
            //task.cntrNo = "THKM0000033";
            //task.mchnId = "RTG08";
            //task.mchnTp = "TC";
            //task.isMvmtIn = false;
            //task.locTp = "YARD";
            //task.blck = "A03";
            //task.bay = "43";
            //task.row = "05";
            //task.tier = "1";
            //task.qcLn = "";
            //task.userId = "222222";

            //Task task = new Task();

            //task.jobId = jobDoneSend.jobKey;

            //task.cntr = new Container();
            //task.cntr.cntrNo = jobDoneSend.cntrNo;

            //task.loc = new Location();
            //task.loc.locTp = new LocationType();
            //task.loc.locTp.name = jobDoneSend.Loc.locTp;
            //task.loc.blck = jobDoneSend.Loc.blck;
            //task.loc.bay = jobDoneSend.Loc.bay;
            //task.loc.row = jobDoneSend.Loc.row;
            //task.loc.tier = jobDoneSend.Loc.tier;

            //task.workingMchn = new Machine();
            //task.workingMchn.mchnTp = jobDoneSend.WorkingMachineTP;
            //task.workingMchn.mchnId = jobDoneSend.WorkingMachineID;
            //task.workingMchn.movedDistance = 0;
            //task.workingMchn.moveTime = 0;

            //task.workingMchn.sprd = new Spreader();
            //task.workingMchn.sprd.sprdMd = new SpreaderMode();
            //task.workingMchn.sprd.sprdMd.name = jobDoneSend.sprd.sprdMd;
            //task.workingMchn.sprd.sprdTp = new SpreaderType();
            //task.workingMchn.sprd.sprdTp.name = jobDoneSend.sprd.sprdTp;
            //task.workingMchn.sprd.sprdSts = new SpreaderStatus();
            //task.workingMchn.sprd.sprdSts.name = jobDoneSend.sprd.sprdSts;

            //task.partnerMchn = new Machine();
            //task.partnerMchn.mchnTp = jobDoneSend.PartnerMachineTP;
            //task.partnerMchn.mchnId = jobDoneSend.PartnerMachineID;
            //task.partnerMchn.movedDistance = 0;
            //task.partnerMchn.moveTime = 0;
            //task.partnerMchn.sprd = new Spreader();
            //task.partnerMchn.sprd.sprdMd = new SpreaderMode();
            //task.partnerMchn.sprd.sprdMd.name = jobDoneSend.sprd.sprdMd;
            //task.partnerMchn.sprd.sprdTp = new SpreaderType();
            //task.partnerMchn.sprd.sprdTp.name = jobDoneSend.sprd.sprdTp;
            //task.partnerMchn.sprd.sprdSts = new SpreaderStatus();
            //task.partnerMchn.sprd.sprdSts.name = jobDoneSend.sprd.sprdSts;

            //task.positionOnChassis = jobDoneSend.positionOnChassis;

            if (isLocationChange == true)
                HessianComm.HessianAPI.SetJobDoneForLocationChange(vmtWorkOrder);
            else
                HessianComm.HessianAPI.SetJobDone(vmtWorkOrder);
        }

        private static Task GetTaskFromJobDoneObj(VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send jobDoneObj)
        {
            if (jobDoneObj == null)
                return null;
            else
            {
                Task task = new Task();

                task.jobId = jobDoneObj.jobKey;

                task.cntr = new Container();
                task.cntr.cntrNo = jobDoneObj.cntrNo;

                task.loc = new Location();
                task.loc.locTp = new LocationType();
                task.loc.locTp.name = jobDoneObj.Loc.locTp;
                task.loc.blck = jobDoneObj.Loc.blck;
                task.loc.bay = jobDoneObj.Loc.bay;
                task.loc.row = jobDoneObj.Loc.row;
                task.loc.tier = jobDoneObj.Loc.tier;

                task.workingMchn = new Machine();
                task.workingMchn.mchnTp = jobDoneObj.WorkingMachineTP;
                task.workingMchn.mchnId = jobDoneObj.WorkingMachineID;
                task.workingMchn.movedDistance = 0;
                task.workingMchn.moveTime = 0;
                task.workingMchn.isTwistLock = jobDoneObj.sprd.sprdSts == "LS_SPREADER_LOCKED" ? true : false;

                task.workingMchn.sprd = new Spreader();
                task.workingMchn.sprd.sprdMd = new SpreaderMode();
                task.workingMchn.sprd.sprdMd.name = jobDoneObj.sprd.sprdMd;
                task.workingMchn.sprd.sprdTp = new SpreaderType();
                task.workingMchn.sprd.sprdTp.name = jobDoneObj.sprd.sprdTp;
                task.workingMchn.sprd.sprdSts = new SpreaderStatus();
                task.workingMchn.sprd.sprdSts.name = jobDoneObj.sprd.sprdSts;

                task.partnerMchn = new Machine();
                task.partnerMchn.mchnTp = jobDoneObj.PartnerMachineTP;
                task.partnerMchn.mchnId = jobDoneObj.PartnerMachineID;
                task.partnerMchn.movedDistance = 0;
                task.partnerMchn.moveTime = 0;
                task.partnerMchn.sprd = new Spreader();
                task.partnerMchn.sprd.sprdMd = new SpreaderMode();
                task.partnerMchn.sprd.sprdMd.name = jobDoneObj.sprd.sprdMd;
                task.partnerMchn.sprd.sprdTp = new SpreaderType();
                task.partnerMchn.sprd.sprdTp.name = jobDoneObj.sprd.sprdTp;
                task.partnerMchn.sprd.sprdSts = new SpreaderStatus();
                task.partnerMchn.sprd.sprdSts.name = jobDoneObj.sprd.sprdSts;

                return task;
            }
        }

        public static void SetPickedContainer_Ask(VMT_Data_JAT2.Objects.RMG.VD_RMG_HandleJobDone_Send target)
        {
            var hessianList = new hessiancsharp.Class.HessianList();

            hessianList.Add(UserInfo.gMchnID);
            hessianList.Add(UserInfo.gMchnTp);
            hessianList.Add(target.jobKey);
            hessianList.Add(target.workingStatus);
            hessianList.Add(UserInfo.gMchnID);

            HessianComm.HessianAPI.SetPickedContainer(hessianList);
        }

        public static void SetDetwinJob_Ask(String jobKey)
        {
            hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(jobKey);
            hessianList.Add(UserInfo.gUserID);
            HessianComm.HessianAPI.SetDetwinJob(hessianList);
        }

        public static void GetContainerInfo_Ask(String cntrNo)
        {
            Container jobCntr = new Container();
            jobCntr.cntrNo = cntrNo;
            HessianComm.HessianAPI.GetContainerInfo(jobCntr);
        }

        public static void GetTwinContainerInfo_Ask(String cntrNo)
        {
            Container jobCntr = new Container();
            jobCntr.cntrNo = cntrNo;
            HessianComm.HessianAPI.GetTwinContainerInfo(jobCntr);
        }

        public static void GetMaxRow_Ask()
        {
            HessianComm.HessianAPI.GetMaxRow();
        }

        public static void GetNoWorkArea_Ask(String block)
        {   
            Location loc = new Location();
            loc.blck = block;
            HessianComm.HessianAPI.GetNoWorkArea(loc);
        }

        public static void GetNoWorkArea1_Ask(String block)
        {
            Location loc = new Location();
            loc.blck = block;
            HessianComm.HessianAPI.GetNoWorkArea1(loc);
        }

        public static void GetNoWorkArea2_Ask(String block)
        {
            Location loc = new Location();
            loc.blck = block;
            HessianComm.HessianAPI.GetNoWorkArea2(loc);
        }

        public static void GetNoWorkTier_Ask(String block, String fromBay, String toBay)
        {
            Location loc = new Location();
            loc.blck = block;
            loc.bay = fromBay;
            loc.location = toBay;
            HessianComm.HessianAPI.GetNoWorkTier(loc);
        }

        public static void GetNoWorkTier1_Ask(String block, String fromBay, String toBay)
        {
            Location loc = new Location();
            loc.blck = block;
            loc.bay = fromBay;
            loc.location = toBay;
            HessianComm.HessianAPI.GetNoWorkTier1(loc);
        }

        public static void GetNoWorkTier2_Ask(String block, String fromBay, String toBay)
        {
            Location loc = new Location();
            loc.blck = block;
            loc.bay = fromBay;
            loc.location = toBay;
            HessianComm.HessianAPI.GetNoWorkTier2(loc);
        }

        public static void SetChangePosition_ask(String block, String bay)
        {
            Task task = new Task();

            task.loc = new Location();
            task.loc.locTp = new LocationType();
            task.loc.locTp.name = "YARD";
            task.loc.blck = block;
            task.loc.bay = bay;            

            task.workingMchn = new Machine();
            task.workingMchn.mchnId = RMG.RMG_User.gMchnID;
            task.workingMchn.mchnTp = RMG.RMG_User.gMchnTp;

            HessianComm.HessianAPI.SetChangePosition(task);
        }

        public static void SetJobReOnChassis_Ask(String jobKey, String twinJobKey = "")
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(jobKey);
            hessianList.Add(twinJobKey);
            hessianList.Add(UserInfo.gUserID);
            HessianComm.HessianAPI.SetJobReOnChassis(hessianList);
        }

        // Get Driver Job History
        public static void GetDriverJobHistory_Ask()
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            String machineId = RMG.RMG_User.gMchnID;
            String userId = RMG.RMG_User.gUserID;
            hessianList.Add(machineId);
            hessianList.Add(userId);
            HessianComm.HessianAPI.GetDriverJobHistory(hessianList);
        }

        public static void CheckYcDeTwin(String ownItvNo, String changeItvNo)
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(ownItvNo);
            hessianList.Add(changeItvNo);
            HessianComm.HessianAPI.CheckYcDeTwin(hessianList);
        }

        public static void SetBestPick(String aprchLn, String jobId)
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(VMT_Data_JAT2.Objects.UserInfo.gMchnID);
            hessianList.Add(VMT_Data_JAT2.Objects.UserInfo.gMchnTp);
            hessianList.Add(aprchLn);
            hessianList.Add(jobId);
            hessianList.Add(VMT_Data_JAT2.Objects.UserInfo.gUserID);
            HessianComm.HessianAPI.SetBestPick(hessianList);
        }

        public static void Validate4LoadingSwapping_Ask(String cntrNo, String machineID, String chssPsn)
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(cntrNo);
            hessianList.Add(machineID);
            hessianList.Add(chssPsn);
            HessianComm.HessianAPI.Validate4LoadingSwapping(hessianList);
        }

        public static void InitPLCMessage_Ask()
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(UserInfo.gMchnID);
            hessianList.Add(UserInfo.gUserID);
            HessianComm.HessianAPI.InitPLCMessage(hessianList);
        }

        public static void ProcessPLC_Ask(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain domain)
        {
            var vmtDomain = new HessianComm.Objects.VmtDomain();
            vmtDomain.mchnId = domain.mchnId;
            vmtDomain.cntrNo = domain.cntrNo;
            vmtDomain.rsnDesc = domain.rsnDesc;
            vmtDomain.containerWeight = domain.containerWeight;
            vmtDomain.currentBay = domain.currentBay;
            vmtDomain.currentBlock = domain.currentBlock;
            vmtDomain.currentCell = domain.currentCell;
            vmtDomain.currentRow = domain.currentRow;
            vmtDomain.currentTier = domain.currentTier;
            vmtDomain.msgSeq = domain.msgSeq;
            vmtDomain.twistLockStatus = domain.twistLockStatus;
            vmtDomain.wrkCd = domain.wrkCd;
            vmtDomain.jbFlg = domain.jbFlg;
            vmtDomain.userId  = UserInfo.gUserID;

            HessianComm.HessianAPI.ProcessPLC(vmtDomain);
        }

        public static void CancelPLC_Ask(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain domain)
        {
            var vmtDomain = new HessianComm.Objects.VmtDomain();
            vmtDomain.mchnId = domain.mchnId;
            vmtDomain.cntrNo = domain.cntrNo;
            vmtDomain.rsnDesc = domain.rsnDesc;
            vmtDomain.containerWeight = domain.containerWeight;
            vmtDomain.currentBlock = domain.currentBlock;
            vmtDomain.currentBay = domain.currentBay;
            vmtDomain.currentCell = domain.currentCell;
            vmtDomain.currentRow = domain.currentRow;
            vmtDomain.currentTier = domain.currentTier;
            vmtDomain.msgSeq = domain.msgSeq;
            vmtDomain.twistLockStatus = domain.twistLockStatus;
            vmtDomain.wrkCd = domain.wrkCd;
            vmtDomain.jbFlg = domain.jbFlg;
            vmtDomain.userId = UserInfo.gUserID;

            HessianComm.HessianAPI.CancelPLC(vmtDomain);
        }
        public static void ReleasePLCLock_Ask(String jobKey, String mnchnId, String userId, String msgSeq)
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(jobKey);
            hessianList.Add(mnchnId);
            hessianList.Add(userId);
            hessianList.Add(msgSeq);
            HessianComm.HessianAPI.ReleasePLCLock(hessianList);
        }
        public static void GetEmptySwappingTargetList_Ask(String cntrNo, String regoNo)
        {
            HessianList hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(UserInfo.gUserID);
            hessianList.Add(cntrNo);
            hessianList.Add(regoNo);

            HessianComm.HessianAPI.GetEmptySwappingTargetList(hessianList);
        }
        public static void getSwapListRTG_Ask(String jobKey, String blckNm, bool filterBlck)
        {
            HessianList hessianList = new HessianList();
            hessianList.Add(jobKey);
            hessianList.Add(filterBlck ? blckNm : "");

            HessianComm.HessianAPI.getSwapListRTG(hessianList);
        }
        public static void DoEmptySwap_Ask(RTG.EmptySwapListSend emptySwapListSend)
        {
            var hessianList = new hessiancsharp.Class.HessianList();
            hessianList.Add(UserInfo.gUserID); //userId
            hessianList.Add(emptySwapListSend.orgCntrNo); //orgCntrNo
            hessianList.Add(emptySwapListSend.newCntrNo); //newCntrNo
            hessianList.Add(emptySwapListSend.regoNo); //regoNo
            hessianList.Add(UserInfo.gMchnID); //mchnId

            HessianComm.HessianAPI.DoEmptySwap(hessianList);
        }
        //-----------------------------------------------------------------------------
    }

    public class VMT_DataMgr_RMG_Callback
    {
        public delegate void Callback_RMG_PDS_Periodic_Payload(ref RMG.VD_RMG_PDS_Periodic_Payload value);
        static public void SetCallback_RMG_PDS_Periodic_Payload(Callback_RMG_PDS_Periodic_Payload fp)
        {
            static_RMG_PDS_Periodic_Payload = fp;
        }

        public delegate void Callback_RMG_CpsAlign_Payload(ref RMG.VD_RMG_CPS_Alignment_Payload value);
        static public void SetCallBack_RMG_CpsAlign_Payload(Callback_RMG_CpsAlign_Payload fp)
        {
            static_RMG_CpsAlign_Payload = fp;
        }

        public delegate void Callback_RMG_PickDrop_Payload(ref RMG.VD_RMG_PDS_PickDrop_Payload value);
        static public void SetCallBack_RMG_PickDrop_Payload(Callback_RMG_PickDrop_Payload fp)
        {
            static_RMG_PickDrop_Payload = fp;
        }

        public delegate void Callback_RMG_RFID_Payload(ref RMG.VD_RMG_PDS_RFID_Payload value);
        static public void SetCallBack_RMG_Rfid_Payload(Callback_RMG_RFID_Payload fp)
        {
            static_RMG_RFID_Payload = fp;
        }

        public delegate void CallBack_NotifyJobOrderRMG(RMG.VD_RMG_JobOrderList value);
        static public void SetCallBack_NotifyJobOrderRMG(CallBack_NotifyJobOrderRMG fp)
        {
            static_NotifyJobOrderRMG = fp;
        }

        public delegate void CallBack_NotifyJobOrderList(List<Objects.Common.VD_Common_JobOrder> listJobOrder);
        static public void SetCallBack_NotifyJobOrderList(CallBack_NotifyJobOrderList fp)
        {
            static_NotifyJobOrderList = fp;
        }

        public delegate void Callback_NotifyGetMachineStatusChanged(VMT_Data_JAT2.Objects.Common.VmtMachine value);
        static public void SetCallBack_NotifyGetMachineStatusChanged(Callback_NotifyGetMachineStatusChanged fp)
        {
            static_NotifyGetMachineStatusChanged = fp;
        }

        public delegate void CallBack_NotifySwapListRMG(RMG.VD_RMG_SwapList value);
        static public void SetCallBack_NotifySwapListRMG(CallBack_NotifySwapListRMG fp)
        {
            static_NotifySwapListRMG = fp;
        }

        public delegate void CallBack_NotifySwapListRTG(RMG.VD_RMG_SwapList value);
        static public void SetCallBack_NotifySwapListRTG(CallBack_NotifySwapListRTG fp)
        {
            static_NotifySwapListRTG = fp;
        }

        public delegate void CallBack_NotifySetSwapRMG(RMG.VD_RMG_VmtResult value);
        static public void SetCallBack_NotifySetSwapRMG(CallBack_NotifySetSwapRMG fp)
        {
            static_NotifySetSwapRMG = fp;
        }

        public delegate void Callback_RMG_NotifyPOWInfo(ref RMG.VD_RMG_POWInfo_Receive value);
        static public void SetCallback_RMG_NotifyPOWInfo(Callback_RMG_NotifyPOWInfo fp)
        {
            static_RMG_NotifyPOWInfo = fp;
        }

        public delegate void Callback_RMG_NotifyBlockEnteranceITV(ref RMG.VD_RMG_BlockEnteranceITV_Receive value);
        static public void SetCallback_RMG_NotifyBlockEnteranceITV(Callback_RMG_NotifyBlockEnteranceITV fp)
        {
            static_RMG_NotifyBlockEnteranceITV = fp;
        }

        public delegate void Callback_RMG_NotifyManualReadyITV(ref RMG.VD_RMG_ManualReadyITV_Receive value);
        static public void SetCallback_RMG_NotifyManualReadyITV(Callback_RMG_NotifyManualReadyITV fp)
        {
            static_RMG_NotifyManualReadyITV = fp;
        }

        public delegate void Callback_RMG_NotifyBlockBayInfo(ref RMG.VD_RMG_InventoryInfo_Receive value);
        static public void SetCallback_RMG_NotifyBlockBayInfo(Callback_RMG_NotifyBlockBayInfo fp)
        {
            static_RMG_NotifyBlockBayInfo = fp;
        }

        public delegate void Callback_RMG_NotifyBlockBayInfoSimple(ref RMG.VD_RMG_BlockBayInfoSimple_Receive value);
        static public void SetCallback_RMG_NotifyBlockBayInfoSimple(Callback_RMG_NotifyBlockBayInfoSimple fp)
        {
            static_RMG_NotifyBlockBayInfoSimple = fp;
        }

        public delegate void Callback_RMG_NotifyCorrection(ref RMG.VD_RMG_InventoryInfo_Receive value);
        static public void SetCallback_RMG_NotifyCorrection(Callback_RMG_NotifyCorrection fp)
        {
            static_RMG_NotifyCorrection = fp;
        }

        public delegate void Callback_RMG_NotifySetCurrentJob(ref RMG.VD_RMG_SetCurrentJob_Receive value);
        static public void SetCallback_RMG_NotifySetCurrentJob(Callback_RMG_NotifySetCurrentJob fp)
        {
            static_RMG_NotifySetCurrentJob = fp;
        }

        public delegate void Callback_RMG_TargetJob(ref RMG.VD_RMG_TargetJob_Receive value);
        static public void SetCallback_RMG_TargetJob(Callback_RMG_TargetJob fp)
        {
            static_RMG_TargetJob = fp;
        }

        public delegate void Callback_RMG_NotifyMarring(ref RMG.VD_RMG_RMGMarrying_Receive value);
        static public void SetCallback_RMG_NotifyMarring(Callback_RMG_NotifyMarring fp)
        {
            static_RMG_NotifyMarring = fp;
        }

        public delegate void Callback_RMG_SwapResult(ref RMG.VD_RMG_SwapResult_Receive value);
        static public void SetCallback_RMG_SwapResult(Callback_RMG_SwapResult fp)
        {
            static_RMG_SwapResult = fp;
        }

        public delegate void Callback_RMG_ReturnWarning(ref RMG.VD_RMG_ReturnWarning_Receive value);
        static public void SetCallback_RMG_ReturnWarning(Callback_RMG_ReturnWarning fp)
        {
            static_RMG_ReturnWarning = fp;
        }

        public delegate void CallBack_NotifyInventoryListRMG(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value);
        static public void SetCallback_RMG_NotifyInventoryListRMG(CallBack_NotifyInventoryListRMG fp)
        {
            static_NotifyInventoryListRMG = fp;
        }

        static public void SetCallback_RMG_NotifyInventoryListRMG1(CallBack_NotifyInventoryListRMG fp)
        {
            static_NotifyInventoryListRMG1 = fp;
        }

        static public void SetCallback_RMG_NotifyInventoryListRMG2(CallBack_NotifyInventoryListRMG fp)
        {
            static_NotifyInventoryListRMG2 = fp;
        }

        public delegate void CallBack_NotifyInventorySwapListRMG(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value);
        static public void SetCallback_RMG_NotifyInventorySwapListRMG(CallBack_NotifyInventorySwapListRMG fp)
        {
            static_NotifyInventorySwapListRMG = fp;
        }

        public delegate void CallBack_NotifyMachineList(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList value);
        static public void SetCallback_RMG_NotifyMachineList(CallBack_NotifyMachineList fp)
        {
            static_NotifyMachineList = fp;
        }

        public delegate void CallBack_NotifyChangedMachineLocation(VMT_Data_JAT2.Objects.Common.VmtMachine value);
        static public void SetCallback_RMG_NotifyChangedMachineLocation(CallBack_NotifyChangedMachineLocation fp)
        {
            static_NotifyChangedMachineLocation = fp;
        }

        public delegate void CallBack_NotifyMachineLogoutCheck(VMT_Data_JAT2.Objects.RMG.VD_RMG_MachineList_Receive value);
        static public void SetCallback_RMG_NotifyMachineLogoutCheck(CallBack_NotifyMachineLogoutCheck fp)
        {
            static_NotifyMachineLogoutCheck = fp;
        }

        public delegate void CallBack_NotifyMachineListofPool(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList value);
        static public void SetCallback_RMG_NotifyMachineListofPool(CallBack_NotifyMachineListofPool fp)
        {
            static_NotifyMachineListofPool = fp;
        }

        public delegate void CallBack_NotifyJobOrderByContainer(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList value);
        static public void SetCallback_RMG_NotifyJobOrderByContainer(CallBack_NotifyJobOrderByContainer fp)
        {
            static_NotifyJobOrderByContainer = fp;
        }

        public delegate void CallBack_NotifyDetwinJob(Boolean value);
        static public void SetCallback_RMG_NotifyDetwinJob(CallBack_NotifyDetwinJob fp)
        {
            static_NotifyDetwinJob = fp;
        }

        public delegate void CallBack_NotifySetJobStatus(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobSet_Receive value);
        static public void SetCallback_RMG_NotifySetJobStatus(CallBack_NotifySetJobStatus fp)
        {
            static_NotifySetJobStatus = fp;
        }

        public delegate void CallBack_NotifyGetContainerInfo(ref VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive value);
        static public void SetCallback_RMG_NotifyGetContainerInfo(CallBack_NotifyGetContainerInfo fp)
        {
            static_NotifyGetContainerInfo = fp;
        }
        static public void SetCallback_RMG_NotifyGetTwinContainerInfo(CallBack_NotifyGetContainerInfo fp)
        {
            static_NotifyGetTwinContainerInfo = fp;
        }

        public delegate void Callback_NotifySetMachineStop(String value);
        static public void SetCallBack_NotifySetMachineStop(Callback_NotifySetMachineStop fp)
        {
            static_NotifySetMachineStop = fp;
        }

        public delegate void CallBack_NotifyGetMaxRow(String value);
        static public void SetCallBack_NotifyGetMaxRow(CallBack_NotifyGetMaxRow fp)
        {
            static_NotifyGetMaxRow = fp;
        }

        public delegate void CallBack_NotifyGetNoWorkArea(VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive value);
        static public void SetCallBack_NotifyGetNoWorkArea(CallBack_NotifyGetNoWorkArea fp)
        {
            static_NotifyGetNoWorkArea = fp;
        }

        static public void SetCallBack_NotifyGetNoWorkArea1(CallBack_NotifyGetNoWorkArea fp)
        {
            static_NotifyGetNoWorkArea1 = fp;
        }

        static public void SetCallBack_NotifyGetNoWorkArea2(CallBack_NotifyGetNoWorkArea fp)
        {
            static_NotifyGetNoWorkArea2 = fp;
        }

        static public void SetCallBack_NotifyGetNoWorkTier(CallBack_NotifyGetNoWorkArea fp)
        {
            static_NotifyGetNoWorkTier = fp;
        }

        static public void SetCallBack_NotifyGetNoWorkTier1(CallBack_NotifyGetNoWorkArea fp)
        {
            static_NotifyGetNoWorkTier1 = fp;
        }

        static public void SetCallBack_NotifyGetNoWorkTier2(CallBack_NotifyGetNoWorkArea fp)
        {
            static_NotifyGetNoWorkTier2 = fp;
        }

        public delegate void CallBack_NotifyChangePosition(Object obj);
        static public void SetCallBack_NotifyChangePosition(CallBack_NotifyChangePosition fp)
        {
            static_NotifyChangePosition = fp;
        }

        public delegate void CallBack_NotifyJobDone(VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive value);
        static public void SetCallBack_NotifyJobDone(CallBack_NotifyJobDone fp)
        {
            static_NotifyJobDone = fp;
        }

        public delegate void CallBack_NotifyCheckPLCData(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain value);
        static public void SetCallBack_NotifyCheckPLCData(CallBack_NotifyCheckPLCData fp)
        {
            static_NotifyCheckPLCData = fp;
        }
        public delegate void CallBack_NotifyCheckPLCTwistLock(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain value);
        static public void SetCallBack_NotifyCheckPLCTwistLock(CallBack_NotifyCheckPLCTwistLock fp)
        {
            static_NotifyCheckPLCTwistLock = fp;
        }

        public delegate void CallBack_NotifyInitPLCMessage();
        static public void SetCallBack_NotifyInitPLCMessage(CallBack_NotifyInitPLCMessage fp)
        {
            static_NotifyInitPLCMessage = fp;
        }

        public delegate void CallBack_NotifyProcessPLC(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult value);
        static public void SetCallBack_NotifyProcessPLC(CallBack_NotifyProcessPLC fp)
        {
            static_NotifyProcessPLC = fp;
        }

        public delegate void CallBack_NotifyCancelPLC(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult value);
        static public void SetCallBack_NotifyCancelPLC(CallBack_NotifyCancelPLC fp)
        {
            static_NotifyCancelPLC = fp;
        }

        public delegate void CallBack_NotifyReleasePLCLock(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult value);
        static public void SetCallBack_NotifyReleasePLCLock(CallBack_NotifyReleasePLCLock fp)
        {
            static_NotifyReleasePLCLock = fp;
        }

        public delegate void CallBack_NotifyEmptySwappingTargetList(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapOutVO vmtEmptySwapOutVO);
        static public void SetCallBack_NotifyEmptySwappingTargetList(CallBack_NotifyEmptySwappingTargetList fp)
        {
            static_NotifyEmptySwappingTargetList = fp;
        }
        public delegate void CallBack_NotifyDoEmptySwap(VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult value);
        static public void SetCallBack_NotifyDoEmptySwap(CallBack_NotifyDoEmptySwap fp)
        {
            static_NotifyDoEmptySwap = fp;
        }
        static public void SetCallBack_NotifyJobDoneForLocationChange(CallBack_NotifyJobDone fp)
        {
            static_NotifyJobDoneForLocationChange = fp;
        }

        public delegate void CallBack_NotifyIsValidLocation(Boolean value);
        static public void SetCallBack_NotifyIsValidLocation(CallBack_NotifyIsValidLocation fp)
        {
            static_NotifyIsValidLocation = fp;
        }

        public delegate void CallBack_NotifySetJobReOnChassis(Boolean value);
        static public void SetCallBack_NotifySetJobReOnChassis(CallBack_NotifySetJobReOnChassis fp)
        {
            static_NotifySetJobReOnChassis = fp;
        }

        public delegate void CallBack_NotifyGetDriverJobHistory(RMG.VD_RMG_JobOrderList value);
        static public void SetCallBack_NotifyGetDriverJobHistory(CallBack_NotifyGetDriverJobHistory fp)
        {
            static_NotifyGetDriverJobHistory = fp;
        }


        static public void SetCallBack_NotifyPickedContainer(CallBack_NotifyJobDone fp)
        {
            static_NotifyPickedContainer = fp;
        }

        public delegate void CallBack_NotifyCheckYcDeTwin(Boolean value);
        static public void SetCallBack_NotifyCheckYcDeTwin(CallBack_NotifyCheckYcDeTwin fp)
        {
            static_NotifyCheckYcDeTwin = fp;
        }

        static public void SetCallBack_NotifySetBestPick(CallBack_NotifyJobDone fp)
        {
            static_NotifySetBestPick = fp;
        }

        static public void SetCallBack_NotifyValidate4LoadingSwapping(CallBack_NotifyJobDone fp)
        {
            static_NotifyValidate4LoadingSwapping = fp;
        }

        static public Callback_RMG_PDS_Periodic_Payload static_RMG_PDS_Periodic_Payload;
        static public Callback_RMG_CpsAlign_Payload static_RMG_CpsAlign_Payload;
        static public Callback_RMG_PickDrop_Payload static_RMG_PickDrop_Payload;
        static public Callback_RMG_RFID_Payload static_RMG_RFID_Payload;
        static public CallBack_NotifyJobOrderRMG static_NotifyJobOrderRMG;
        static public CallBack_NotifyJobOrderList static_NotifyJobOrderList;
        static public Callback_NotifyGetMachineStatusChanged static_NotifyGetMachineStatusChanged;
        static public CallBack_NotifySwapListRMG static_NotifySwapListRMG;
        static public CallBack_NotifySwapListRTG static_NotifySwapListRTG;
        static public CallBack_NotifySetSwapRMG static_NotifySetSwapRMG;
        static public Callback_RMG_NotifyPOWInfo static_RMG_NotifyPOWInfo;
        static public Callback_RMG_NotifyBlockEnteranceITV static_RMG_NotifyBlockEnteranceITV;
        static public Callback_RMG_NotifyManualReadyITV static_RMG_NotifyManualReadyITV;
        static public Callback_RMG_NotifyBlockBayInfo static_RMG_NotifyBlockBayInfo;
        static public Callback_RMG_NotifyBlockBayInfoSimple static_RMG_NotifyBlockBayInfoSimple;
        static public Callback_RMG_NotifyCorrection static_RMG_NotifyCorrection;
        static public Callback_RMG_NotifySetCurrentJob static_RMG_NotifySetCurrentJob;
        static public Callback_RMG_TargetJob static_RMG_TargetJob;
        static public Callback_RMG_NotifyMarring static_RMG_NotifyMarring;
        static public Callback_RMG_SwapResult static_RMG_SwapResult;
        static public Callback_RMG_ReturnWarning static_RMG_ReturnWarning;

        static public CallBack_NotifyInventoryListRMG static_NotifyInventoryListRMG;
        static public CallBack_NotifyInventoryListRMG static_NotifyInventoryListRMG1;
        static public CallBack_NotifyInventoryListRMG static_NotifyInventoryListRMG2;
        static public CallBack_NotifyInventorySwapListRMG static_NotifyInventorySwapListRMG; 

        static public CallBack_NotifyMachineList static_NotifyMachineList;
        static public CallBack_NotifyChangedMachineLocation static_NotifyChangedMachineLocation;
        static public CallBack_NotifyMachineLogoutCheck static_NotifyMachineLogoutCheck;

        static public CallBack_NotifyMachineListofPool static_NotifyMachineListofPool;
        static public CallBack_NotifyJobOrderByContainer static_NotifyJobOrderByContainer;
        static public CallBack_NotifyDetwinJob static_NotifyDetwinJob;
        static public CallBack_NotifySetJobStatus static_NotifySetJobStatus;
        static public CallBack_NotifyGetContainerInfo static_NotifyGetContainerInfo;
        static public CallBack_NotifyGetContainerInfo static_NotifyGetTwinContainerInfo;
        static public Callback_NotifySetMachineStop static_NotifySetMachineStop;

        static public CallBack_NotifyGetMaxRow static_NotifyGetMaxRow;
        static public CallBack_NotifyGetNoWorkArea static_NotifyGetNoWorkArea;
        static public CallBack_NotifyGetNoWorkArea static_NotifyGetNoWorkArea1;
        static public CallBack_NotifyGetNoWorkArea static_NotifyGetNoWorkArea2;

        static public CallBack_NotifyGetNoWorkArea static_NotifyGetNoWorkTier;
        static public CallBack_NotifyGetNoWorkArea static_NotifyGetNoWorkTier1;
        static public CallBack_NotifyGetNoWorkArea static_NotifyGetNoWorkTier2;

        static public CallBack_NotifyChangePosition static_NotifyChangePosition;

        static public CallBack_NotifyJobDone static_NotifyJobDone;
        static public CallBack_NotifyJobDone static_NotifyJobDoneForLocationChange;

        static public CallBack_NotifyIsValidLocation static_NotifyIsValidLocation;

        static public CallBack_NotifySetJobReOnChassis static_NotifySetJobReOnChassis;

        static public CallBack_NotifyGetDriverJobHistory static_NotifyGetDriverJobHistory; // Get Driver Job History

        static public CallBack_NotifyJobDone static_NotifyPickedContainer;

        static public CallBack_NotifyCheckYcDeTwin static_NotifyCheckYcDeTwin;

        static public CallBack_NotifyJobDone static_NotifySetBestPick;

        static public CallBack_NotifyJobDone static_NotifyValidate4LoadingSwapping;

        static public CallBack_NotifyCheckPLCData static_NotifyCheckPLCData;

        static public CallBack_NotifyCheckPLCTwistLock static_NotifyCheckPLCTwistLock;

        static public CallBack_NotifyInitPLCMessage static_NotifyInitPLCMessage;

        static public CallBack_NotifyProcessPLC static_NotifyProcessPLC;

        static public CallBack_NotifyCancelPLC static_NotifyCancelPLC;

        static public CallBack_NotifyReleasePLCLock static_NotifyReleasePLCLock;

        static public CallBack_NotifyEmptySwappingTargetList static_NotifyEmptySwappingTargetList;

        static public CallBack_NotifyDoEmptySwap static_NotifyDoEmptySwap;
    }
}
