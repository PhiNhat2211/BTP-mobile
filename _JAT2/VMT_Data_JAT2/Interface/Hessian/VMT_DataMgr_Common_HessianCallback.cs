using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HessianComm;
using System.Net;
using System.Collections;
using Common.Util;

namespace VMT_Data_JAT2
{
    public class SortAlphabetLength : System.Collections.IComparer
    {
        public int Compare(Object x, Object y)
        {
            Hashtable xx = x as Hashtable;
            string blockX = Convert.ToString(xx["blck"]);

            Hashtable yy = y as Hashtable;
            string blockY = Convert.ToString(yy["blck"]);

            if (blockX.ToString().Length == blockY.ToString().Length)
            {
                int idxNotNumberX = 0;
                int idxNotNumberY = 0;

                for (int i = 0; i < blockX.Length; i++)
                {
                    if (int.TryParse(blockX[i].ToString(), out int b) == false)
                    {
                        idxNotNumberX = i;
                        break;
                    }
                }

                for (int j = 0; j < blockY.Length; j++)
                {
                    if (int.TryParse(blockY[j].ToString(), out int b) == false)
                    {
                        idxNotNumberY = j;
                        break;
                    }
                }


                int result = string.Compare(blockX.Substring(idxNotNumberX), blockY.Substring(idxNotNumberY));
                if(result == 0)
                {
                    result = string.Compare(blockX.ToString(), blockY.ToString());
                }
                return result;
            }
            else if (blockX.ToString().Length > blockY.ToString().Length)
                return 1;
            else
                return -1;
        }
    }
    public class VMT_DataMgr_Common_HessianCallback
    {
        static public void HandleLogApi(bool isSend, HessianCommType type, Object obj)
        {
            if (VMT_DataMgr_Common_Callback.static_NotifyHandleLogApi != null)
                VMT_DataMgr_Common_Callback.static_NotifyHandleLogApi(isSend, type, obj);
        }

        static public void KeepAlive(ref Object obj)
        {
            if (obj is String)
            {
                String retStr = Convert.ToString(obj);

                //if (VMT_Data_JAT2.Objects.UserInfo.GetMchnTp() == VMT_Data_JAT2.Objects.UserInfo.MchnTp.MchnTp_RMG)
                //{
                if (VMT_DataMgr_Common_Callback.static_NotifyKeepAlive != null)
                    VMT_DataMgr_Common_Callback.static_NotifyKeepAlive(retStr);
                //}
                //else
                //{
                //    if (VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus != null)
                //        VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus(1);
                //}
            }
        }

        static public void KeepAliveStandAlone(ref Object obj)
        {
            if (obj is Boolean)
            {
                //if (VMT_Data_JAT2.Objects.UserInfo.GetMchnTp() == VMT_Data_JAT2.Objects.UserInfo.MchnTp.MchnTp_RMG)
                //{
                if (VMT_DataMgr_Common_Callback.static_NotifyKeepAlive != null)
                    VMT_DataMgr_Common_Callback.static_NotifyKeepAlive(Convert.ToString(obj));
                //}
                //else
                //{
                //    if (VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus != null)
                //        VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus(1);
                //}
            }
        }

        static public void DGPSAlive(ref Object obj)
        {
            if (obj is Boolean)
            {
                if (VMT_DataMgr_Common_Callback.static_NotifyGPSStatus != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGPSStatus(1);
            }
        }

        static public void GetUserAccessRole(ref Object obj)
        {
            if (obj == null)
            {
                // Incorrect ID
                if (VMT_DataMgr_Common_Callback.static_NotifyErrorCode != null)
                    VMT_DataMgr_Common_Callback.static_NotifyErrorCode("U2");
            }
            else if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;

                if (Convert.ToBoolean(retHashtable["isUsrLogin"]) == false)
                {
                    if (retHashtable["usrGrp"] != null)
                    {
                        ArrayList usrGrp = retHashtable["usrGrp"] as ArrayList;

                        VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive accessRoleRpValue = new VMT_Data_JAT2.Objects.Common.VD_Common_GetUserAccesRole_Receive();
                        var sb = new StringBuilder(1024);
                        int count = 0;
                        foreach (Object grpName in usrGrp)
                        {
                            if (grpName != null)
                            {
                                count++;
                                sb.Append(grpName.ToString());
                                if (count < usrGrp.Count)
                                    sb.Append("|");
                            }
                        }
                        accessRoleRpValue.GroupListSeperator = sb.ToString();
                        accessRoleRpValue.shift = Convert.ToString(retHashtable["shift"]);
                        accessRoleRpValue.usrNm = Convert.ToString(retHashtable["usrNm"]);

                        if (VMT_DataMgr_Common_Callback.static_NotifyAccessRole != null)
                            VMT_DataMgr_Common_Callback.static_NotifyAccessRole(ref accessRoleRpValue);
                    }
                    else
                    {
                        // Not Exist Your Team
                        if (VMT_DataMgr_Common_Callback.static_NotifyErrorCode != null)
                            VMT_DataMgr_Common_Callback.static_NotifyErrorCode("Not Exist Your Team");
                    }
                }
                else
                {
                    // ID is aleady Login
                    if (VMT_DataMgr_Common_Callback.static_NotifyErrorCode != null)
                        VMT_DataMgr_Common_Callback.static_NotifyErrorCode("ID is aleady Login");
                }
            }
        }

        static public void SetLogin4Machine(ref Object obj)
        {
            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;
                Hashtable retUser = null;
                Hashtable retMachine = null;
                ArrayList retConfig = null;

                if (Convert.ToString(retHashtable["rsnCd"]).Equals("S")) // Success
                {
                    if (retHashtable["user"] is Hashtable)
                        retUser = retHashtable["user"] as Hashtable;
                    if (retHashtable["machine"] is Hashtable)
                        retMachine = retHashtable["machine"] as Hashtable;

                    string strusrNm = Convert.ToString(retUser["usrNm"]);
                    string strChssNo = Convert.ToString(retMachine["chssNo"]);


                    VMT_Data_JAT2.Objects.Common.VD_Common_SetLogin4Machine_Receive driverInfoRpValue = new Objects.Common.VD_Common_SetLogin4Machine_Receive();
                    driverInfoRpValue.iLogin = 1;
                    driverInfoRpValue.UserName = strusrNm;
                    driverInfoRpValue.chssNo = strChssNo;

                    if (retHashtable["machine"] is Hashtable)
                        retMachine = retHashtable["machine"] as Hashtable;
                    Hashtable retLoc = new Hashtable();
                    if (retMachine["loc"] is Hashtable)
                        retLoc = retMachine["loc"] as Hashtable;
                    driverInfoRpValue.loc.blck = Convert.ToString(retLoc["blck"]);
                    driverInfoRpValue.loc.bay = Convert.ToString(retLoc["bay"]);

                    if (VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine != null)
                        VMT_DataMgr_Common_Callback.static_NotifyLogin4Machine(ref driverInfoRpValue);

                    if (retHashtable["config"] is ArrayList)
                        retConfig = retHashtable["config"] as ArrayList;

                    VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive configRp = new VMT_Data_JAT2.Objects.Common.VD_Common_Config_Receive();

                    if (retConfig != null)
                    {
                        foreach (var config in retConfig)
                        {
                            if (config is Hashtable)
                            {
                                String configId = Convert.ToString((config as Hashtable)["configId"]);
                                String configValue = Convert.ToString((config as Hashtable)["configValue"]);
                                if (configId.Equals("VMT_ARRIVAL_FLAG"))
                                {
                                    if (configValue.Equals("Y"))
                                        configRp.Arrival = true;
                                    else if (configValue.Equals("N"))
                                        configRp.Arrival = false;
                                }
                                else if (configId.Equals("VMT_READY_FLAG"))
                                {
                                    if (configValue.Equals("Y"))
                                        configRp.Ready = true;
                                    else if (configValue.Equals("N"))
                                        configRp.Ready = false;
                                }
                                else if (configId.Equals("VMT_DONE_FLAG"))
                                {
                                    if (configValue.Equals("Y"))
                                        configRp.Done = true;
                                    else if (configValue.Equals("N"))
                                        configRp.Done = false;
                                }
                                else if (configId.Equals("VMT_SHOW_UNPLUG_REEFER_ONLY"))
                                {
                                    if (configValue.Equals("Y"))
                                        configRp.ShowUnplugReeferOnly = true;
                                    else if (configValue.Equals("N"))
                                        configRp.ShowUnplugReeferOnly = false;
                                }
                                else if (configId.Equals("VBS_BACK_COLOR"))
                                {
                                    configRp.jobItemColor = configValue;
                                }
                            }
                        }
                    }
                    if (retHashtable["machine"] is Hashtable)
                        retMachine = retHashtable["machine"] as Hashtable;
                    if (retMachine["autoFlg"] != null)
                    {
                        Type tp = retMachine["autoFlg"].GetType();
                        if (tp.Equals(typeof(String)))
                            configRp.autoFlg = (Convert.ToString(retMachine["autoFlg"]) == "Y" ? true : false);
                        else if (tp.Equals(typeof(Boolean)))
                            configRp.autoFlg = Convert.ToBoolean(retMachine["autoFlg"]);
                    }
                    
                    configRp.chssNo = Convert.ToString(retMachine["chssNo"]);
                    configRp.loc = Convert.ToString(retMachine["loc"]);
                    configRp.mchnId = Convert.ToString(retMachine["mchnId"]);
                    configRp.mchnTp = Convert.ToString(retMachine["mchnTp"]);

                    if (VMT_DataMgr_Common_Callback.static_NotifyConfig != null)
                        VMT_DataMgr_Common_Callback.static_NotifyConfig(ref configRp);
                }
                else if (Convert.ToString(retHashtable["rsnCd"]).Equals("D")) // login machine duplicate
                {
                    String strError = Convert.ToString(retHashtable["rsnDesc"]);
                    // Macine Duplicate
                    if (VMT_DataMgr_Common_Callback.static_NotifyErrorCode != null)
                        VMT_DataMgr_Common_Callback.static_NotifyErrorCode(strError);
                }
                else if (Convert.ToString(retHashtable["rsnCd"]).Contains("F")) // wrong password
                {
                    String rsnCd = Convert.ToString(retHashtable["rsnCd"]);
                    String rsnDesc = Convert.ToString(retHashtable["rsnDesc"]);
                    rsnCd += "@" + rsnDesc;
                    // Incorrect Password
                    if (VMT_DataMgr_Common_Callback.static_NotifyErrorCode != null)
                        //VMT_DataMgr_Common_Callback.static_NotifyErrorCode("Incorrect Password\nPlease Retry");
                        VMT_DataMgr_Common_Callback.static_NotifyErrorCode(rsnCd);
                }
            }
        }
        static public void ChangeDriverCheck(Object obj)
        {
            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;
                
                if (Convert.ToString(retHashtable["rsnCd"]).Equals("S")) // Success
                {
                    String rsnCd = Convert.ToString(retHashtable["rsnCd"]);

                    if (VMT_DataMgr_Common_Callback.static_NotifyChangeDriverCheck != null)
                        VMT_DataMgr_Common_Callback.static_NotifyChangeDriverCheck(rsnCd);
                }               
                else
                {
                    String rsnCd = Convert.ToString(retHashtable["rsnCd"]);
                    String rsnDesc = Convert.ToString(retHashtable["rsnDesc"]);
                    rsnCd += "@" + rsnDesc;
                    if (VMT_DataMgr_Common_Callback.static_NotifyErrorCode != null)
                        VMT_DataMgr_Common_Callback.static_NotifyErrorCode(rsnCd);
                }
            }
        }
        static public void SetMachineStatusChanged(ref Object obj)
        {
            Object retObject = obj;  // Always Null return void type

            VMT_Data_JAT2.Objects.Common.VD_Common_SendMachineStatusChange_Receive sendMachineStatusChang = new Objects.Common.VD_Common_SendMachineStatusChange_Receive();
            sendMachineStatusChang.m_iResult = 1;

            if (VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged != null)
                VMT_DataMgr_Common_Callback.static_NotifyMachineStatusChanged(ref sendMachineStatusChang);
        }

        static void EEv2JobOrderInit(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            jobOrder.workingMchn = new Objects.Common.VD_Common_Job_Machine();
            jobOrder.workingMchn.mchnId = "";
            jobOrder.workingMchn.mchnTp = "";
            jobOrder.workingMchn.mchnSts = "";
            jobOrder.workingMchn.vrtlFlg = "";
            jobOrder.partnerMchn = new Objects.Common.VD_Common_Job_Machine();
            jobOrder.partnerMchn.mchnId = "";
            jobOrder.partnerMchn.mchnTp = "";
            jobOrder.partnerMchn.mchnSts = "";
            jobOrder.partnerMchn.vrtlFlg = "";
            jobOrder.cntr = new Objects.Common.VD_Common_Job_Container();
            jobOrder.cntr.cntrNo = "";
            jobOrder.cntr.cntrIso = "";
            jobOrder.cntr.cntrTp = "";
            jobOrder.cntr.cls = "";
            jobOrder.cntr.opr = "";
            jobOrder.cntr.cntrCgoTp = "";
            jobOrder.cntr.fullMty = "";
            jobOrder.locTo = new Objects.Common.VD_Common_Job_Location();
            jobOrder.locTo.locTp = "";
            jobOrder.locTo.blck = "";
            jobOrder.locTo.bay = "";
            jobOrder.locTo.row = "";
            jobOrder.locTo.tier = "";
            jobOrder.locTo.lane = "";
            jobOrder.locTo.location = "";
            jobOrder.locFrom = new Objects.Common.VD_Common_Job_Location();
            jobOrder.locFrom.locTp = "";
            jobOrder.locFrom.blck = "";
            jobOrder.locFrom.bay = "";
            jobOrder.locFrom.row = "";
            jobOrder.locFrom.tier = "";
            jobOrder.locFrom.lane = "";
            jobOrder.locFrom.location = "";
            jobOrder.locWorking = new Objects.Common.VD_Common_Job_Location();
            jobOrder.locWorking.locTp = "";
            jobOrder.locWorking.blck = "";
            jobOrder.locWorking.bay = "";
            jobOrder.locWorking.row = "";
            jobOrder.locWorking.tier = "";
            jobOrder.locWorking.lane = "";
            jobOrder.locWorking.location = "";
            jobOrder.type = new Objects.Common.VD_Common_Job_Type();
            jobOrder.type.jobTp = "";
            jobOrder.type.jobStatus = "";
            jobOrder.type.vslCd = "";
            jobOrder.type.voyNo = "";
            jobOrder.type.planSeq = "";
            jobOrder.type.twinTandemFlg = "";
            jobOrder.type.twinTandumKey = "";
            jobOrder.type.tandemJoinYT = "";
            jobOrder.type.jobFlagInfo = "";
            jobOrder.type.conePlan = "";
        }

        static public void SetLogout4Machine(ref Object obj)
        {
            Boolean retBoolean = Convert.ToBoolean(obj);
            if (VMT_DataMgr_Common_Callback.static_NotifyLogout != null)
                VMT_DataMgr_Common_Callback.static_NotifyLogout(retBoolean);
        }

        static public void GetMachineStopCodeList(ref Object obj)
        {
            List<VMT_Data_JAT2.Objects.Common.VD_Common_Available> availableList = new List<Objects.Common.VD_Common_Available>();
            if (obj is ArrayList)
            {
                ArrayList availableArrayList = obj as ArrayList;
                foreach (Object objAvailable in availableArrayList)
                {
                    if (objAvailable is Hashtable)
                    {
                        Hashtable hashAvailable = objAvailable as Hashtable;

                        VMT_Data_JAT2.Objects.Common.VD_Common_Available available = new Objects.Common.VD_Common_Available();
                        available.ReasonCd = Convert.ToString(hashAvailable["reasonCd"]);
                        available.ReasonNm = Convert.ToString(hashAvailable["reasonNm"]);

                        availableList.Add(available);
                    }
                }

                VMT_Data_JAT2.Objects.Common.VD_Common_MachineStopCodeList_Receive availableUI = new Objects.Common.VD_Common_MachineStopCodeList_Receive();
                availableUI.m_iAvailableCount = availableList.Count;
                availableUI.m_pData = availableList;

                if (VMT_DataMgr_Common_Callback.static_NotifyMachineStopCodeList != null)
                    VMT_DataMgr_Common_Callback.static_NotifyMachineStopCodeList(ref availableUI);
            }
        }

        static public void GetMachineAccessAction(ref Object obj)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_MachineAccessAction_Receive accessInfo = new Objects.Common.VD_Common_MachineAccessAction_Receive();
            if (obj is ArrayList)
            {
                ArrayList accessList = obj as ArrayList;
                
                foreach (Object row in accessList)
                {
                    if (row is Hashtable)
                    {
                        Hashtable retHashtable = row as Hashtable;
                        if (Convert.ToString(retHashtable["pgmId"]).ToUpper().Equals("BTN_SETTING"))
                        {
                            accessInfo.showSetting = Convert.ToString(retHashtable["pgmUseFlg"]).ToUpper().Equals("TRUE");
                        }
                        else if (Convert.ToString(retHashtable["pgmId"]).ToUpper().Equals("BTN_CHG_LOC"))
                        {
                            accessInfo.showCHGLOC = Convert.ToString(retHashtable["pgmUseFlg"]).ToUpper().Equals("TRUE");
                        }
                        else if (Convert.ToString(retHashtable["pgmId"]).ToUpper().Equals("VIEW_INV"))
                        {
                            accessInfo.showViewINV = Convert.ToString(retHashtable["pgmUseFlg"]).ToUpper().Equals("TRUE");
                        }
                        else if (Convert.ToString(retHashtable["pgmId"]).ToUpper().Equals("VIEW_BLOCK_LIST"))
                        {
                            accessInfo.viewBLockList = Convert.ToString(retHashtable["pgmUseFlg"]).ToUpper().Equals("TRUE");
                        }
                        else if (Convert.ToString(retHashtable["pgmId"]).ToUpper().Equals("BTN_ITV_SWAP"))
                        {
                            accessInfo.enableItvSwap = Convert.ToString(retHashtable["pgmUseFlg"]).ToUpper().Equals("TRUE");
                        }
                    }
                }
            }
            if (VMT_DataMgr_Common_Callback.static_NotifyMachineAccessAction != null)
                VMT_DataMgr_Common_Callback.static_NotifyMachineAccessAction(ref accessInfo);
        }

        static public void GetBlockListForYardSector(ref Object obj)
        {
            try
            {
                if (obj is ArrayList)
                {
                    using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                    {
                        SortAlphabetLength alphaLen = new SortAlphabetLength();
                        ArrayList blckListSorted = obj as ArrayList;
                        blckListSorted.Sort(alphaLen);
                        foreach (Object blockItem in blckListSorted)
                        {
                            if (blockItem is Hashtable)
                            {
                                Hashtable hashBlock = blockItem as Hashtable;

                                var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                                simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                                simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                                //simpleBlock.isBolBlck = Convert.ToBoolean(hashBlock["isBolBlck"]);
                                simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                    Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                                simpleBlock.userCode = Convert.ToString(hashBlock["userCode"]);
                                simpleBlock.loginUser = Convert.ToString(hashBlock["loginUser"]);

                                simpleBlock.goCont = Convert.ToString(hashBlock["goCont"]) == "" ? "0" : Convert.ToString(hashBlock["goCont"]);
                                simpleBlock.giCont = Convert.ToString(hashBlock["giCont"]) == "" ? "0" : Convert.ToString(hashBlock["giCont"]);
                                simpleBlock.moCont = Convert.ToString(hashBlock["moCont"]) == "" ? "0" : Convert.ToString(hashBlock["moCont"]);
                                simpleBlock.miCont = Convert.ToString(hashBlock["miCont"]) == "" ? "0" : Convert.ToString(hashBlock["miCont"]);
                                simpleBlock.dsCont = Convert.ToString(hashBlock["dsCont"]) == "" ? "0" : Convert.ToString(hashBlock["dsCont"]);
                                simpleBlock.ldCont = Convert.ToString(hashBlock["ldCont"]) == "" ? "0" : Convert.ToString(hashBlock["ldCont"]);
                                simpleBlock.etcCont = Convert.ToString(hashBlock["etcCont"]) == "" ? "0" : Convert.ToString(hashBlock["etcCont"]);
                                simpleBlock.totalCont = Convert.ToString(hashBlock["totalCont"]) == "" ? "0" : Convert.ToString(hashBlock["totalCont"]);

                                simpleBlock.isBolBlck = Convert.ToBoolean(hashBlock["isBolBlck"]);
                                if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                    blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                                else
                                    blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                            }
                        }

                        if (VMT_DataMgr_Common_Callback.static_NotifyBlockListForBlockMap != null)
                            VMT_DataMgr_Common_Callback.static_NotifyBlockListForBlockMap(blockInfo);
                    }
                }
                else //20201020 block list null -> Show current block
                {
                    using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                    {
                        if (VMT_DataMgr_Common_Callback.static_NotifyBlockListForBlockMap != null)
                            VMT_DataMgr_Common_Callback.static_NotifyBlockListForBlockMap(blockInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void TOSGetMachineStop(ref Object obj)
        {
            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;

                String reasonCd = Convert.ToString(retHashtable["reasonCd"]);
                String reasonNm = Convert.ToString(retHashtable["reasonNm"]);
                String mchnStopDt = Convert.ToString(retHashtable["mchnStopDt"]);
                String remark = Convert.ToString(retHashtable["remark"]);

                VMT_Data_JAT2.Objects.Common.VD_Common_GetMachineStop_Receive machineStopAvailable = new Objects.Common.VD_Common_GetMachineStop_Receive();
                machineStopAvailable.Data.ReasonCd = reasonCd;
                machineStopAvailable.Data.ReasonNm = reasonNm;
                machineStopAvailable.m_iBreak = 1;
                long lStartTime;
                long.TryParse(mchnStopDt, out lStartTime);
                machineStopAvailable.StartTime = lStartTime;
                machineStopAvailable.FinishTime = 0;
                machineStopAvailable.remark = remark;

                if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop(machineStopAvailable);
            }
            else if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop != null)
                VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop(null);
        }

        static public void SetMachineStop(ref Object obj) // Always Null return void type
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_SetMachineStop_Receive value = new Objects.Common.VD_Common_SetMachineStop_Receive();

            if (VMT_DataMgr_Common_Callback.static_NotifySetMachineStop != null)
                VMT_DataMgr_Common_Callback.static_NotifySetMachineStop(ref value);

            /*VMT_Data_JAT2.Objects.Common.VD_Common_Available availableSelectData = new VMT_Data_JAT2.Objects.Common.VD_Common_Available();
            availableSelectData.ReasonNm = reasonNm;
            availableSelectData.ReasonCd = reasonCd;

            mMainView.ShowBreak(availableSelectData);
            mMainView.TextBlock_Available.Text = reasonNm;
             */
        }

        static public void GetMachineNotice(ref Object obj)
        {
            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;

                String noticeMsg = Convert.ToString(retHashtable["noticeMsg"]);
                String displayMsg = Convert.ToString(retHashtable["displayMsg"]);

                VMT_Data_JAT2.Objects.Common.VD_Common_MachineNotice_Receive noticeMessage = new Objects.Common.VD_Common_MachineNotice_Receive();
                noticeMessage.m_iMessageType = 1;
                noticeMessage.m_strMessage = noticeMsg;
                noticeMessage.m_strMessage2 = displayMsg;

                if (VMT_DataMgr_Common_Callback.static_NotifyMachineNotice != null)
                {
                    VMT_DataMgr_Common_Callback.static_NotifyMachineNotice(ref noticeMessage);
                    if(!String.IsNullOrEmpty(noticeMsg))
                        VMT_DataMgr_Common.SetMachineNotice();
                }
            }
        }

        static public void SetMachineNotice(ref Object obj) // Always Null return void type
        {
            Object retObject = obj;
        }

        static public void GetBlockList(ref Object obj)
        {
            try
            {
                if (obj is ArrayList)
                {
                    using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                    {
                        SortAlphabetLength alphaLen = new SortAlphabetLength();
                        ArrayList blckListSorted = obj as ArrayList;
                        blckListSorted.Sort(alphaLen);

                        foreach (Object blockItem in blckListSorted)
                        {
                            if (blockItem is Hashtable)
                            {
                                Hashtable hashBlock = blockItem as Hashtable;

                                var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                                simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                                simpleBlock.userCode = Convert.ToString(hashBlock["userCode"]);
                                simpleBlock.loginUser = Convert.ToString(hashBlock["loginUser"]);

                                simpleBlock.goCont = Convert.ToString(hashBlock["goCont"]) == "" ? "0" : Convert.ToString(hashBlock["goCont"]);
                                simpleBlock.giCont = Convert.ToString(hashBlock["giCont"]) == "" ? "0" : Convert.ToString(hashBlock["giCont"]);
                                simpleBlock.moCont = Convert.ToString(hashBlock["moCont"]) == "" ? "0" : Convert.ToString(hashBlock["moCont"]);
                                simpleBlock.miCont = Convert.ToString(hashBlock["miCont"]) == "" ? "0" : Convert.ToString(hashBlock["miCont"]);
                                simpleBlock.dsCont = Convert.ToString(hashBlock["dsCont"]) == "" ? "0" : Convert.ToString(hashBlock["dsCont"]);
                                simpleBlock.ldCont = Convert.ToString(hashBlock["ldCont"]) == "" ? "0" : Convert.ToString(hashBlock["ldCont"]);
                                simpleBlock.etcCont = Convert.ToString(hashBlock["etcCont"]) == "" ? "0" : Convert.ToString(hashBlock["etcCont"]);
                                simpleBlock.totalCont = Convert.ToString(hashBlock["totalCont"]) == "" ? "0" : Convert.ToString(hashBlock["totalCont"]);

                                simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                                simpleBlock.isBolBlck = Convert.ToBoolean(hashBlock["isBolBlck"]);
                                simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                    Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;

                                if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                    blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                                else
                                    blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                            }
                        }

                        if (VMT_DataMgr_Common_Callback.static_NotifyBlockList != null)
                            VMT_DataMgr_Common_Callback.static_NotifyBlockList(blockInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void GetBlockListForBlockMap(ref Object obj)
        {
            try
            {
                if (obj is ArrayList)
                {
                    using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                    {
                        SortAlphabetLength alphaLen = new SortAlphabetLength();
                        ArrayList blckListSorted = obj as ArrayList;
                        blckListSorted.Sort(alphaLen);

                        foreach (Object blockItem in blckListSorted)
                        {
                            if (blockItem is Hashtable)
                            {
                                Hashtable hashBlock = blockItem as Hashtable;

                                var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                                simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                                simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                                //simpleBlock.isBolBlck = Convert.ToBoolean(hashBlock["isBolBlck"]);
                                simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                    Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                                simpleBlock.userCode = Convert.ToString(hashBlock["userCode"]);
                                simpleBlock.loginUser = Convert.ToString(hashBlock["loginUser"]);

                                simpleBlock.goCont = Convert.ToString(hashBlock["goCont"]) == "" ? "0" : Convert.ToString(hashBlock["goCont"]);
                                simpleBlock.giCont = Convert.ToString(hashBlock["giCont"]) == "" ? "0" : Convert.ToString(hashBlock["giCont"]);
                                simpleBlock.moCont = Convert.ToString(hashBlock["moCont"]) == "" ? "0" : Convert.ToString(hashBlock["moCont"]);
                                simpleBlock.miCont = Convert.ToString(hashBlock["miCont"]) == "" ? "0" : Convert.ToString(hashBlock["miCont"]);
                                simpleBlock.dsCont = Convert.ToString(hashBlock["dsCont"]) == "" ? "0" : Convert.ToString(hashBlock["dsCont"]);
                                simpleBlock.ldCont = Convert.ToString(hashBlock["ldCont"]) == "" ? "0" : Convert.ToString(hashBlock["ldCont"]);
                                simpleBlock.etcCont = Convert.ToString(hashBlock["etcCont"]) == "" ? "0" : Convert.ToString(hashBlock["etcCont"]);
                                simpleBlock.totalCont = Convert.ToString(hashBlock["totalCont"]) == "" ? "0" : Convert.ToString(hashBlock["totalCont"]);

                                simpleBlock.isBolBlck = Convert.ToBoolean(hashBlock["isBolBlck"]);
                                if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                    blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                                else
                                    blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                            }
                        }

                        if (VMT_DataMgr_Common_Callback.static_NotifyBlockListForBlockMap != null)
                            VMT_DataMgr_Common_Callback.static_NotifyBlockListForBlockMap(blockInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static public void GetVmtAutoStartConfig(ref Object obj)
        {
            if (obj is String)
            {
                String retStr = Convert.ToString(obj);
                if (VMT_DataMgr_Common_Callback.static_NotifyGetVmtAutoStartConfig != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetVmtAutoStartConfig(retStr);

            }
        }

        static public void GetBlockMapList(ref Object obj)
        {
            if (obj is ArrayList)
            {
                using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                {

                    foreach (Object item in obj as ArrayList)
                    {
                        if (item is Hashtable)
                        {
                            Hashtable hashBlock = item as Hashtable;

                            var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                            simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                            simpleBlock.userCode = Convert.ToString(hashBlock["userCode"]);
                            simpleBlock.loginUser = Convert.ToString(hashBlock["loginUser"]);

                            simpleBlock.goCont = Convert.ToString(hashBlock["goCont"]) == "" ? "0" : Convert.ToString(hashBlock["goCont"]);
                            simpleBlock.giCont = Convert.ToString(hashBlock["giCont"]) == "" ? "0" : Convert.ToString(hashBlock["giCont"]);
                            simpleBlock.moCont = Convert.ToString(hashBlock["moCont"]) == "" ? "0" : Convert.ToString(hashBlock["moCont"]);
                            simpleBlock.miCont = Convert.ToString(hashBlock["miCont"]) == "" ? "0" : Convert.ToString(hashBlock["miCont"]);
                            simpleBlock.dsCont = Convert.ToString(hashBlock["dsCont"]) == "" ? "0" : Convert.ToString(hashBlock["dsCont"]);
                            simpleBlock.ldCont = Convert.ToString(hashBlock["ldCont"]) == "" ? "0" : Convert.ToString(hashBlock["ldCont"]);
                            simpleBlock.etcCont = Convert.ToString(hashBlock["etcCont"]) == "" ? "0" : Convert.ToString(hashBlock["etcCont"]);
                            simpleBlock.totalCont = Convert.ToString(hashBlock["totalCont"]) == "" ? "0" : Convert.ToString(hashBlock["totalCont"]);

                            simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                            simpleBlock.isBolBlck = Convert.ToBoolean(hashBlock["isBolBlck"]);

                            simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                            simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                            simpleBlock.MaxTier = Convert.ToInt32(hashBlock["maxTier"]);
                            simpleBlock.MaxTier = 7;

                            if (hashBlock["blckBay"] is ArrayList)
                            {
                                foreach (var bay in hashBlock["blckBay"] as ArrayList)
                                {
                                    if (bay is Hashtable)
                                    {
                                        Hashtable hashBay = bay as Hashtable;

                                        var simpleBay = new Objects.Common.VD_Common_SimpleBayInfo();

                                        simpleBay.BayName = Convert.ToString(hashBay["bay20Ft"]);
                                        simpleBay.opr = Convert.ToString(hashBay["opr"]);
                                        simpleBay.cntrLen = Convert.ToString(hashBay["cntrLen"]);
                                        simpleBay.cntrTp = Convert.ToString(hashBay["cntrTp"]);
                                        simpleBay.inspCode = Convert.ToString(hashBay["inspCode"]);
                                        simpleBay.categoryName = Convert.ToString(hashBay["categoryName"]);
                                        if (hashBay["blckRow"] is ArrayList)
                                        {
                                            foreach (var row in hashBay["blckRow"] as ArrayList)
                                            {
                                                if (row is Hashtable)
                                                {
                                                    var hashRow = row as Hashtable;

                                                    var simpleRow = new Objects.Common.VD_Common_SimpleRowInfo();
                                                    simpleRow.rowNm = Convert.ToString(hashRow["rowNm"]);
                                                    simpleRow.abbRowNo = Convert.ToString(hashRow["abbRowNo"]);
                                                    if (hashRow["isUse"] != null)
                                                        simpleRow.isUse = Convert.ToBoolean(hashRow["isUse"]);

                                                    String rowIdxNoStr = Convert.ToString(hashRow["rowIdxNo"]);
                                                    if (!String.IsNullOrEmpty(rowIdxNoStr))
                                                    {
                                                        var rowIdxNo = Convert.ToInt32(rowIdxNoStr);
                                                        if (!simpleBay.RowNameMap.ContainsKey(rowIdxNo))
                                                            simpleBay.RowNameMap.Add(rowIdxNo, simpleRow);
                                                    }                                                  
                                                }
                                            }
                                        }
                                        if (simpleBlock.DicBay == null)
                                            simpleBlock.DicBay = new SortedDictionary<string, Objects.Common.VD_Common_SimpleBayInfo>();

                                        if (!String.IsNullOrEmpty(BayRemoveChars(simpleBay.BayName)))
                                        {
                                            if (!simpleBlock.DicBay.ContainsKey(simpleBay.BayName))
                                                simpleBlock.DicBay.Add(simpleBay.BayName, simpleBay);
                                            else
                                                simpleBlock.DicBay[simpleBay.BayName] = simpleBay;
                                        }                                      
                                    }
                                }
                            }

                            if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                            else
                                blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                        }
                    }

                    if (VMT_DataMgr_Common_Callback.static_NotifyBlockMapList != null)
                        VMT_DataMgr_Common_Callback.static_NotifyBlockMapList(blockInfo);
                }
            }
        }

        private static String BayRemoveChars(String value)
        {
            return new string(value.Where(char.IsDigit).ToArray());
        }

        static public void GetBlockMapListForYt(ref Object obj)
        {
            if (obj is ArrayList)
            {
                using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                {

                    foreach (Object item in obj as ArrayList)
                    {
                        if (item is Hashtable)
                        {
                            Hashtable hashBlock = item as Hashtable;

                            var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                            simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                            simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                            simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                            simpleBlock.MaxTier = Convert.ToInt32(hashBlock["maxTier"]);

                            if (hashBlock["blckBay"] is ArrayList)
                            {
                                foreach (var bay in hashBlock["blckBay"] as ArrayList)
                                {
                                    if (bay is Hashtable)
                                    {
                                        Hashtable hashBay = bay as Hashtable;

                                        var simpleBay = new Objects.Common.VD_Common_SimpleBayInfo();

                                        simpleBay.BayName = Convert.ToString(hashBay["bay20Ft"]);
                                        if (hashBay["blckRow"] is ArrayList)
                                        {
                                            foreach (var row in hashBay["blckRow"] as ArrayList)
                                            {
                                                if (row is Hashtable)
                                                {
                                                    var hashRow = row as Hashtable;

                                                    var simpleRow = new Objects.Common.VD_Common_SimpleRowInfo();
                                                    simpleRow.rowNm = Convert.ToString(hashRow["rowNm"]);
                                                    simpleRow.abbRowNo = Convert.ToString(hashRow["abbRowNo"]);
                                                    if (hashRow["isUse"] != null)
                                                        simpleRow.isUse = Convert.ToBoolean(hashRow["isUse"]);

                                                    String rowIdxNoStr = Convert.ToString(hashRow["rowIdxNo"]);
                                                    if (!String.IsNullOrEmpty(rowIdxNoStr))
                                                    {
                                                        var rowIdxNo = Convert.ToInt32(rowIdxNoStr);
                                                        if (!simpleBay.RowNameMap.ContainsKey(rowIdxNo))
                                                            simpleBay.RowNameMap.Add(rowIdxNo, simpleRow);
                                                    }
                                                }
                                            }
                                        }
                                        if (simpleBlock.DicBay == null)
                                            simpleBlock.DicBay = new SortedDictionary<string, Objects.Common.VD_Common_SimpleBayInfo>();

                                        if (!String.IsNullOrEmpty(BayRemoveChars(simpleBay.BayName)))
                                        {
                                            if (!simpleBlock.DicBay.ContainsKey(simpleBay.BayName))
                                                simpleBlock.DicBay.Add(simpleBay.BayName, simpleBay);
                                            else
                                                simpleBlock.DicBay[simpleBay.BayName] = simpleBay;
                                        }
                                    }
                                }
                            }

                            if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                            else
                                blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                        }
                    }

                    if (VMT_DataMgr_Common_Callback.static_NotifyBlockMapListForYt != null)
                        VMT_DataMgr_Common_Callback.static_NotifyBlockMapListForYt(blockInfo);
                }
            }
        }

        static public void GetBlockMapList1(ref Object obj)
        {
            if (obj is ArrayList)
            {
                using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                {

                    foreach (Object item in obj as ArrayList)
                    {
                        if (item is Hashtable)
                        {
                            Hashtable hashBlock = item as Hashtable;

                            var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                            simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                            simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                            simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                            simpleBlock.MaxTier = Convert.ToInt32(hashBlock["maxTier"]);

                            if (hashBlock["blckBay"] is ArrayList)
                            {
                                foreach (var bay in hashBlock["blckBay"] as ArrayList)
                                {
                                    if (bay is Hashtable)
                                    {
                                        Hashtable hashBay = bay as Hashtable;

                                        var simpleBay = new Objects.Common.VD_Common_SimpleBayInfo();

                                        simpleBay.BayName = Convert.ToString(hashBay["bay20Ft"]);

                                        if (hashBay["blckRow"] is ArrayList)
                                        {
                                            foreach (var row in hashBay["blckRow"] as ArrayList)
                                            {
                                                if (row is Hashtable)
                                                {
                                                    var hashRow = row as Hashtable;

                                                    var simpleRow = new Objects.Common.VD_Common_SimpleRowInfo();
                                                    simpleRow.rowNm = Convert.ToString(hashRow["rowNm"]);
                                                    simpleRow.abbRowNo = Convert.ToString(hashRow["abbRowNo"]);
                                                    if (hashRow["isUse"] != null)
                                                        simpleRow.isUse = Convert.ToBoolean(hashRow["isUse"]);

                                                    String rowIdxNoStr = Convert.ToString(hashRow["rowIdxNo"]);
                                                    if (!String.IsNullOrEmpty(rowIdxNoStr))
                                                    {
                                                        var rowIdxNo = Convert.ToInt32(rowIdxNoStr);
                                                        if (!simpleBay.RowNameMap.ContainsKey(rowIdxNo))
                                                            simpleBay.RowNameMap.Add(rowIdxNo, simpleRow);
                                                    }
                                                }
                                            }
                                        }
                                        if (simpleBlock.DicBay == null)
                                            simpleBlock.DicBay = new SortedDictionary<string, Objects.Common.VD_Common_SimpleBayInfo>();

                                        if (!String.IsNullOrEmpty(BayRemoveChars(simpleBay.BayName)))
                                        {
                                            if (!simpleBlock.DicBay.ContainsKey(simpleBay.BayName))
                                                simpleBlock.DicBay.Add(simpleBay.BayName, simpleBay);
                                            else
                                                simpleBlock.DicBay[simpleBay.BayName] = simpleBay;
                                        }                                      
                                    }
                                }
                            }

                            if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                            else
                                blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                        }
                    }

                    if (VMT_DataMgr_Common_Callback.static_NotifyBlockMapList1 != null)
                        VMT_DataMgr_Common_Callback.static_NotifyBlockMapList1(blockInfo);
                }
            }
        }

        static public void GetBlockMapList2(ref Object obj)
        {
            if (obj is ArrayList)
            {
                using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                {

                    foreach (Object item in obj as ArrayList)
                    {
                        if (item is Hashtable)
                        {
                            Hashtable hashBlock = item as Hashtable;

                            var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                            simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                            simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                            simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                            simpleBlock.MaxTier = Convert.ToInt32(hashBlock["maxTier"]);

                            if (hashBlock["blckBay"] is ArrayList)
                            {
                                foreach (var bay in hashBlock["blckBay"] as ArrayList)
                                {
                                    if (bay is Hashtable)
                                    {
                                        Hashtable hashBay = bay as Hashtable;

                                        var simpleBay = new Objects.Common.VD_Common_SimpleBayInfo();

                                        simpleBay.BayName = Convert.ToString(hashBay["bay20Ft"]);

                                        if (hashBay["blckRow"] is ArrayList)
                                        {
                                            foreach (var row in hashBay["blckRow"] as ArrayList)
                                            {
                                                if (row is Hashtable)
                                                {
                                                    var hashRow = row as Hashtable;

                                                    var simpleRow = new Objects.Common.VD_Common_SimpleRowInfo();
                                                    simpleRow.rowNm = Convert.ToString(hashRow["rowNm"]);
                                                    simpleRow.abbRowNo = Convert.ToString(hashRow["abbRowNo"]);
                                                    if (hashRow["isUse"] != null)
                                                        simpleRow.isUse = Convert.ToBoolean(hashRow["isUse"]);

                                                    String rowIdxNoStr = Convert.ToString(hashRow["rowIdxNo"]);
                                                    if (!String.IsNullOrEmpty(rowIdxNoStr))
                                                    {
                                                        var rowIdxNo = Convert.ToInt32(rowIdxNoStr);
                                                        if (!simpleBay.RowNameMap.ContainsKey(rowIdxNo))
                                                            simpleBay.RowNameMap.Add(rowIdxNo, simpleRow);
                                                    }
                                                }
                                            }
                                        }
                                        if (simpleBlock.DicBay == null)
                                            simpleBlock.DicBay = new SortedDictionary<string, Objects.Common.VD_Common_SimpleBayInfo>();

                                        if (!String.IsNullOrEmpty(BayRemoveChars(simpleBay.BayName)))
                                        {
                                            if (!simpleBlock.DicBay.ContainsKey(simpleBay.BayName))
                                                simpleBlock.DicBay.Add(simpleBay.BayName, simpleBay);
                                            else
                                                simpleBlock.DicBay[simpleBay.BayName] = simpleBay;
                                        }
                                    }
                                }
                            }

                            if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                            else
                                blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                        }
                    }

                    if (VMT_DataMgr_Common_Callback.static_NotifyBlockMapList2 != null)
                        VMT_DataMgr_Common_Callback.static_NotifyBlockMapList2(blockInfo);
                }
            }
        }

        static public void GetBlockMapSwapList(ref Object obj)
        {
            if (obj is ArrayList)
            {
                using (var blockInfo = new Objects.Common.VD_Common_SimpleBlockBayInfo_Receive())
                {

                    foreach (Object item in obj as ArrayList)
                    {
                        if (item is Hashtable)
                        {
                            Hashtable hashBlock = item as Hashtable;

                            var simpleBlock = new Objects.Common.VD_Common_SimpleBlockInfo();

                            simpleBlock.BlcName = Convert.ToString(hashBlock["blck"]);
                            simpleBlock.IsVirtual = Convert.ToBoolean(hashBlock["isVrtl"]);
                            simpleBlock.Direction = Convert.ToString(hashBlock["rowDir"]).Equals("TB") ?
                                Objects.Common.Row_Direction.TB : Objects.Common.Row_Direction.BT;
                            simpleBlock.MaxTier = Convert.ToInt32(hashBlock["maxTier"]);

                            if (hashBlock["blckBay"] is ArrayList)
                            {
                                foreach (var bay in hashBlock["blckBay"] as ArrayList)
                                {
                                    if (bay is Hashtable)
                                    {
                                        Hashtable hashBay = bay as Hashtable;

                                        var simpleBay = new Objects.Common.VD_Common_SimpleBayInfo();

                                        simpleBay.BayName = Convert.ToString(hashBay["bay20Ft"]);

                                        if (hashBay["blckRow"] is ArrayList)
                                        {
                                            foreach (var row in hashBay["blckRow"] as ArrayList)
                                            {
                                                if (row is Hashtable)
                                                {
                                                    var hashRow = row as Hashtable;

                                                    var simpleRow = new Objects.Common.VD_Common_SimpleRowInfo();
                                                    simpleRow.rowNm = Convert.ToString(hashRow["rowNm"]);
                                                    simpleRow.abbRowNo = Convert.ToString(hashRow["abbRowNo"]);
                                                    if (hashRow["isUse"] != null)
                                                        simpleRow.isUse = Convert.ToBoolean(hashRow["isUse"]);

                                                    String rowIdxNoStr = Convert.ToString(hashRow["rowIdxNo"]);
                                                    if (!String.IsNullOrEmpty(rowIdxNoStr))
                                                    {
                                                        var rowIdxNo = Convert.ToInt32(rowIdxNoStr);
                                                        if (!simpleBay.RowNameMap.ContainsKey(rowIdxNo))
                                                            simpleBay.RowNameMap.Add(rowIdxNo, simpleRow);
                                                    }
                                                }
                                            }
                                        }
                                        if (simpleBlock.DicBay == null)
                                            simpleBlock.DicBay = new SortedDictionary<string, Objects.Common.VD_Common_SimpleBayInfo>();

                                        if (!String.IsNullOrEmpty(BayRemoveChars(simpleBay.BayName)))
                                        {
                                            if (!simpleBlock.DicBay.ContainsKey(simpleBay.BayName))
                                                simpleBlock.DicBay.Add(simpleBay.BayName, simpleBay);
                                            else
                                                simpleBlock.DicBay[simpleBay.BayName] = simpleBay;
                                        }
                                    }
                                }
                            }

                            if (!blockInfo.DicBlock.ContainsKey(simpleBlock.BlcName))
                                blockInfo.DicBlock.Add(simpleBlock.BlcName, simpleBlock);
                            else
                                blockInfo.DicBlock[simpleBlock.BlcName] = simpleBlock;
                        }
                    }

                    if (VMT_DataMgr_Common_Callback.static_NotifyBlockMapSwapList != null)
                        VMT_DataMgr_Common_Callback.static_NotifyBlockMapSwapList(blockInfo);
                }
            }
        }

        static public void DoSwap4Manual(ref Object obj)
        {
            if (obj is String)
            {
                if (VMT_DataMgr_Common_Callback.static_NotifyDoSwap4Manual != null)
                    VMT_DataMgr_Common_Callback.static_NotifyDoSwap4Manual((String)obj);
            }
        }

        static public void ExceptionOccured(ref Object obj)
        {

            if (VMT_DataMgr_Common_Callback.static_NotifyException != null)
                VMT_DataMgr_Common_Callback.static_NotifyException((String)obj);
        }

        static public void GetConfigValue(ref Object obj)
        {
            if (obj is String)
            {
                String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetConfigValue != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetConfigValue(retStr);
            }
        }

        /////////////////////////
        // - Single Test
        static public void KeepAlive_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyKeepAlive_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyKeepAlive_Test(ref obj);
            }
        }

        static public void GetInventoryList_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetInventoryList_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetInventoryList_Test(ref obj);
            }
        }

        static public void GetInventory_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetInventory_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetInventory_Test(ref obj);
            }
        }

        static public void GetBlockMapList_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetBlockMapList_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetBlockMapList_Test(ref obj);
            }
        }

        static public void SetMachineStatusChanged_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetMachineStatusChanged_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetMachineStatusChanged_Test(ref obj);
            }
        }

        static public void SetMachineStop_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetMachineStop_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetMachineStop_Test(ref obj);
            }
        }

        static public void SetMachinePassed_Test(ref Object obj) // Not Use, Use SetManualActivation
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetMachinePassed_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetMachinePassed_Test(ref obj);
            }
        }

        static public void SetManualActivation_Test(ref Object obj)
        {
            if (VMT_DataMgr_Common_Callback.static_NotifySetManualActivation_Test != null)
                VMT_DataMgr_Common_Callback.static_NotifySetManualActivation_Test(ref obj);
        }

        static public void SetMachineArrivalInfo_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetMachineArrivalInfo_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetMachineArrivalInfo_Test(ref obj);
            }
        }

        static public void SetMachineReady_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetMachineReady_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetMachineReady_Test(ref obj);
            }
        }

        static public void GetMachineStop_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetMachineStop_Test(ref obj);
            }
        }

        static public void GetMachineNotice_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineNotice_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetMachineNotice_Test(ref obj);
            }
        }

        static public void SetMachineNotice_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetMachineNotice_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetMachineNotice_Test(ref obj);
            }
        }

        static public void GetPrecedingYtList_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetPrecedingYtList_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetPrecedingYtList_Test(ref obj);
            }
        }

        static public void SetJobDone_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetJobDone_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetJobDone_Test(ref obj);
            }
        }


        static public void GetJobOrderList_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                //if (obj is List<Object>)
                //{
                //    List<Object> objList = obj as List<Object>;

                //    foreach(Object jobObj in objList)
                //    {
                //        if(jobObj is Hashtable)
                //        {
                //            Hashtable hashObj = jobObj as Hashtable;
                //        }
                //    }
                //}

                if (VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderList_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderList_Test(ref obj);
            }
        }

        static public void GetUserAccessRole_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetUserAccessRole_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetUserAccessRole_Test(ref obj);
            }
        }

        static public void SetLogin4Machine_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetLogin4Machine_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetLogin4Machine_Test(ref obj);
            }
        }

        static public void SetLogout4Machine_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifySetLogout4Machine_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifySetLogout4Machine_Test(ref obj);
            }
        }

        static public void GetMachineStopCodeList_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineStopCodeList_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetMachineStopCodeList_Test(ref obj);
            }
        }

        // 2015-12-28 추가
        static public void GetJobOrderByContainer_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderByContainer_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetJobOrderByContainer_Test(ref obj);
            }
        }

        static public void GetMachineList_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineList_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetMachineList_Test(ref obj);
            }
        }

        static public void GetMachineListOfPool_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyGetMachineListOfPool_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyGetMachineListOfPool_Test(ref obj);
            }
        }

        static public void DoSwap4Manual_Test(ref Object obj)
        {
            //if (obj is String)
            {
                //String retStr = Convert.ToString(obj);

                if (VMT_DataMgr_Common_Callback.static_NotifyDoSwap4Manual_Test != null)
                    VMT_DataMgr_Common_Callback.static_NotifyDoSwap4Manual_Test(ref obj);
            }
        }

        static public void GetBlockList_Test(ref Object obj)
        {
            if (VMT_DataMgr_Common_Callback.static_NotifyGetBlockList_Test != null)
                VMT_DataMgr_Common_Callback.static_NotifyGetBlockList_Test(ref obj);

        }

        static public void SetJobStatus_Test(ref Object obj)
        {
            if (VMT_DataMgr_Common_Callback.static_NotifySetJobStatus_Test != null)
                VMT_DataMgr_Common_Callback.static_NotifySetJobStatus_Test(ref obj);

        }

        static public void SetDetwinJob_Test(ref Object obj)
        {
            if (VMT_DataMgr_Common_Callback.static_NotifySetDetwinJob_Test != null)
                VMT_DataMgr_Common_Callback.static_NotifySetDetwinJob_Test(ref obj);
        }

        /////////////////////////

        static public void ExceptionDelete_VMT_Data(Exception ex)
        {
            if (ex is HessianException)
            {
                HessianException hEx = (HessianException)ex;

                if (hEx.InnerException is WebException)
                {
                    WebException wE = hEx.InnerException as WebException;

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
                                
                            }
                            break;
                        default:
                            break;
                    }
                }

                Object obj = hEx;
                switch (hEx.hessianCommType)
                {
                    case HessianCommType.KeepAlive:
                        // Hessian Call Failed (Network Error)
                        //if (VMT_Data_JAT2.Objects.UserInfo.GetMchnTp() == VMT_Data_JAT2.Objects.UserInfo.MchnTp.MchnTp_RMG)
                        //{
                        if (VMT_DataMgr_Common_Callback.static_NotifyKeepAlive != null)
                            VMT_DataMgr_Common_Callback.static_NotifyKeepAlive(String.Empty);
                        //}
                        //else
                        //{
                        //    if (VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus != null)
                        //        VMT_DataMgr_Common_Callback.static_NotifyWIFIStatus(0);
                        //}
                        break;
                    case HessianCommType.KeepAliveStandAlone:
                        break;
                    case HessianCommType.DGPSAlive:
                        break;
                    case HessianCommType.SetLogin4Machine:
                        break;
                    case HessianCommType.ChangeDriverCheck:
                        break;
                    case HessianCommType.GetUserAccessRole:
                        break;
                    //case HessianCommType.GetLoginInfo4Machine:
                    //    break;
                    case HessianCommType.SetMachineStatusChanged:
                        break;
                    case HessianCommType.GetJobOrderList:
                        break;
                    case HessianCommType.SetLogout4Machine:
                        break;
                    case HessianCommType.SetMachinePassed: // Not Use, Use SetManualActivation
                        break;
                    case HessianCommType.SetManualActivation:
                        break;
                    case HessianCommType.SetMachineReady:
                        break;
                    case HessianCommType.GetMachineStopCodeList:
                        break;
                    case HessianCommType.GetMachineAccessAction:
                        break;
                    case HessianCommType.GetBlockListForYardSector:
                        break;
                    case HessianCommType.GetMachineStop:
                        break;
                    case HessianCommType.GetMachineNotice:
                        break;
                    case HessianCommType.GetPrecedingYtList:
                        break;
                    case HessianCommType.SetConfirmJobByScanner:
                        break;
                    case HessianCommType.GetBlockMapList:
                        break;
                    case HessianCommType.GetBlockList:
                        break;
                    case HessianCommType.GetBlockListForBlockMap:
                        break;
                    case HessianCommType.GetVmtAutoStartConfig:
                        break;
                    case HessianCommType.SetJobStatus:
                        break;
                    case HessianCommType.SetDetwinJob:
                        break;
                    /////////////////////////
                    // - Single Test
                    case HessianCommType.KeepAlive_Test: { KeepAlive_Test(ref obj); } break;
                    case HessianCommType.GetInventoryList_Test: { GetInventoryList_Test(ref obj); } break;
                    case HessianCommType.GetInventory_Test: { GetInventory_Test(ref obj); } break;
                    case HessianCommType.GetBlockMapList_Test: { GetBlockMapList_Test(ref obj); } break;
                    case HessianCommType.SetMachineStatusChanged_Test: { SetMachineStatusChanged_Test(ref obj); } break;
                    case HessianCommType.SetMachineStop_Test: { SetMachineStop_Test(ref obj); } break;
                    case HessianCommType.SetMachinePassed_Test: { SetMachinePassed_Test(ref obj); } break;
                    case HessianCommType.SetManualActivation_Test: { SetManualActivation_Test(ref obj); } break;
                    case HessianCommType.SetMachineArrivalInfo_Test: { SetMachineArrivalInfo_Test(ref obj); } break;
                    case HessianCommType.SetMachineReady_Test: { SetMachineReady_Test(ref obj); } break;
                    case HessianCommType.GetMachineStop_Test: { GetMachineStop_Test(ref obj); } break;
                    case HessianCommType.GetMachineNotice_Test: { GetMachineNotice_Test(ref obj); } break;
                    case HessianCommType.SetMachineNotice_Test: { SetMachineNotice_Test(ref obj); } break;
                    case HessianCommType.GetPrecedingYtList_Test: { GetPrecedingYtList_Test(ref obj); } break;
                    case HessianCommType.SetJobDone_Test: { SetJobDone_Test(ref obj); } break;
                    case HessianCommType.GetJobOrderList_Test: { GetJobOrderList_Test(ref obj); } break;
                    case HessianCommType.GetUserAccessRole_Test: { GetUserAccessRole_Test(ref obj); } break;
                    case HessianCommType.SetLogin4Machine_Test: { SetLogin4Machine_Test(ref obj); } break;
                    case HessianCommType.SetLogout4Machine_Test: { SetLogout4Machine_Test(ref obj); } break;
                    case HessianCommType.GetMachineStopCodeList_Test: { GetMachineStopCodeList_Test(ref obj); } break;
                    // 2015-12-28 추가
                    case HessianCommType.GetJobOrderByContainer_Test: { GetJobOrderByContainer_Test(ref obj); } break;
                    case HessianCommType.GetMachineList_Test: { GetMachineList_Test(ref obj); } break;
                    case HessianCommType.GetMachineListOfPool_Test: { GetMachineListOfPool_Test(ref obj); } break;
                    case HessianCommType.DoSwap4Manual_Test: { DoSwap4Manual_Test(ref obj); } break;
                    case HessianCommType.GetBlockList_Test: { GetBlockList_Test(ref obj); } break;
                    case HessianCommType.SetJobStatus_Test: { SetJobStatus_Test(ref obj); } break;
                    case HessianCommType.SetDetwinJob_Test: { SetDetwinJob_Test(ref obj); } break;
                    /////////////////////////
                    default:
                        break;
                }
            }
        }
    }
}
