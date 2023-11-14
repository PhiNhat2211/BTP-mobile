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
    public class VMT_DataMgr_RMG_HessianCallback
    {
        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive GetInventoryListFromObject(Object obj)
        {
            ArrayList retArrayList = new ArrayList();
            if (obj is ArrayList)
                retArrayList = obj as ArrayList;

            try
            {
                var inventoryReceive = new VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive();

                if (retArrayList.Count > 0)
                {
                    foreach (Object retObject in retArrayList)
                    {
                        if (retObject is Hashtable)
                        {
                            var inventory = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory();

                            Hashtable retHashtable = retObject as Hashtable;

                            if (retHashtable.ContainsKey("cntr") && retHashtable["cntr"] is Hashtable)
                            {
                                var cntr = GetContainerInfoFromHashtable(retHashtable["cntr"] as Hashtable);
                                inventory.cntr = cntr;
                            }

                            if (retHashtable.ContainsKey("loc") && retHashtable["loc"] is Hashtable)
                            {
                                Hashtable locationHash = retHashtable["loc"] as Hashtable;

                                var loc = GetLocationFromHashtable(retHashtable["loc"] as Hashtable);
                                inventory.loc = loc;
                            }

                            var blockInfo = new VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BlockInfo();
                            blockInfo.BlcName = inventory.loc.blck;
                            if (!inventoryReceive.DicBlock.ContainsKey(blockInfo.BlcName))
                                inventoryReceive.DicBlock.Add(blockInfo.BlcName, blockInfo);

                            var bayInfo = new VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo();
                            bayInfo.BayName = inventory.loc.bay;
                            if (!inventoryReceive.DicBlock[blockInfo.BlcName].DicBay.ContainsKey(bayInfo.BayName))
                                inventoryReceive.DicBlock[blockInfo.BlcName].DicBay.Add(bayInfo.BayName, bayInfo);

                            inventoryReceive.DicBlock[blockInfo.BlcName].DicBay[bayInfo.BayName].invenList.Add(inventory);
                        }
                    }
                }

                return inventoryReceive;

            }
            catch (Exception e)
            {
                String msg = e.Message;
            }

            return null;
        }

        static public void GetInventoryListBackground(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListBackgroundEx(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND_Ex;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryList(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListEx(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND_Ex;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListData(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListDataEx(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA_Ex;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList GetJobOrderListFromObject(Object obj)
        {
            //-----------------------------------------------------------------------------------------            
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderForRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList();
            jobOrderForRMGValue.JobOrder = new List<Objects.Common.VD_Common_JobOrder>();

            List<Object> retObjectList = new List<object>();
            if (obj is List<Object>)
                retObjectList = obj as List<Object>;

            //Logger.Log("===========================================================");

            int activeCount = 0;
            int nJobInsertCount = 0;
            foreach (Object retObject in retObjectList)
            {
                nJobInsertCount++;

                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;
                    Hashtable retWorkingMchn = null;
                    Hashtable retPartnerMchn = null;
                    Hashtable retCntr = null;
                    Hashtable retLoc = null;

                    if (retHashtable.ContainsKey("jobSts") && retHashtable["jobSts"] != null &&
                        retHashtable.ContainsKey("jobTp") && retHashtable["jobTp"] != null)
                    {
                        if (Convert.ToString(retHashtable["jobSts"]).CompareTo("A") == 0 && Convert.ToString(retHashtable["jobTp"]).CompareTo("DS") == 0)
                            activeCount++;
                    }

                    if (retHashtable.ContainsKey("workingMchn") && retHashtable["workingMchn"] is Hashtable)
                        retWorkingMchn = retHashtable["workingMchn"] as Hashtable;

                    if (retHashtable.ContainsKey("partnerMchn") && retHashtable["partnerMchn"] is Hashtable)
                        retPartnerMchn = retHashtable["partnerMchn"] as Hashtable;

                    if (retHashtable.ContainsKey("cntr") && retHashtable["cntr"] is Hashtable)
                        retCntr = retHashtable["cntr"] as Hashtable;

                    if (retHashtable.ContainsKey("loc") && retHashtable["loc"] is Hashtable)
                        retLoc = retHashtable["loc"] as Hashtable;

                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderValue = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                    jobOrderValue.workingMchn = new Objects.Common.VD_Common_Job_Machine();
                    jobOrderValue.partnerMchn = new Objects.Common.VD_Common_Job_Machine();
                    jobOrderValue.cntr = new Objects.Common.VD_Common_Job_Container();
                    jobOrderValue.locWorking = new Objects.Common.VD_Common_Job_Location();
                    jobOrderValue.type = new Objects.Common.VD_Common_Job_Type();

                    if (retHashtable.ContainsKey("jobId") && retHashtable["jobId"] != null)
                        jobOrderValue.jobKey = Convert.ToString(retHashtable["jobId"]);
                    else
                        jobOrderValue.jobKey = String.Empty;

                    jobOrderValue.taskId = Convert.ToString(retHashtable["taskId"]);
                    jobOrderValue.priorityJob = Convert.ToString(retHashtable["priorityJob"]);     // hot job                    

                    if (retWorkingMchn != null)
                    {
                        jobOrderValue.workingMchn.mchnId = Convert.ToString(retWorkingMchn["mchnId"]);
                        jobOrderValue.workingMchn.mchnTp = Convert.ToString(retWorkingMchn["mchnTp"]);
                        jobOrderValue.workingMchn.mchnSts = Convert.ToString(retWorkingMchn["mchnSts"]);
                        jobOrderValue.workingMchn.vrtlFlg = Convert.ToString(retWorkingMchn["vrtlFlg"]);
                    }

                    if (retPartnerMchn != null)
                    {
                        jobOrderValue.partnerMchn.mchnId = Convert.ToString(retPartnerMchn["mchnId"]);
                        jobOrderValue.partnerMchn.mchnTp = Convert.ToString(retPartnerMchn["mchnTp"]);
                        jobOrderValue.partnerMchn.mchnSts = Convert.ToString(retPartnerMchn["mchnSts"]);
                        jobOrderValue.partnerMchn.vrtlFlg = Convert.ToString(retPartnerMchn["vrtlFlg"]);
                        jobOrderValue.partnerMchn.aprchLn = Convert.ToString(retPartnerMchn["aprchLn"]);
                    }

                    if (retCntr != null)
                    {
                        jobOrderValue.cntr.cntrNo = Convert.ToString(retCntr["cntrNo"]);
                        jobOrderValue.cntr.cntrIso = Convert.ToString(retCntr["cntrIso"]);
                        jobOrderValue.cntr.cntrLen = Convert.ToString(retCntr["cntrLen"]);
                        jobOrderValue.cntr.cntrTp = Convert.ToString(retCntr["cntrTp"]);
                        jobOrderValue.cntr.cls = Convert.ToString(retCntr["cls"]);
                        jobOrderValue.cntr.opr = Convert.ToString(retCntr["opr"]);
                        jobOrderValue.cntr.cntrCgoTp = Convert.ToString(retCntr["cntrCgoTp"]);
                        jobOrderValue.cntr.fullMty = Convert.ToString(retCntr["fullMty"]);
                        String wgt = Convert.ToString(retCntr["cntrWgt"]);
                        //jobOrderValue.cntr.cntrWgt = wgt.Length > 3 ? wgt.Substring(0, wgt.Length - 3) : wgt;
                        if (String.IsNullOrEmpty(wgt))
                            jobOrderValue.cntr.cntrWgt = wgt;
                        else
                            jobOrderValue.cntr.cntrWgt = (Convert.ToInt32(Convert.ToDouble(wgt)) / 1000).ToString();
                        jobOrderValue.cntr.pod = Convert.ToString(retCntr["pod"]);
                        jobOrderValue.cntr.cntrSpTp = Convert.ToString(retCntr["cntrSpTp"]);
                        if (retCntr.ContainsKey("cntrHgt"))
                            jobOrderValue.cntr.cntrHgt = Convert.ToString(retCntr["cntrHgt"]);

                        if (retCntr.ContainsKey("imdgCd"))
                            jobOrderValue.cntr.imdgCd = Convert.ToString(retCntr["imdgCd"]);

                        // jobOrderValue.type.conePlan = Convert.ToString(retCntr["conePlan"]);
                        if (Convert.ToString(retCntr["conePlan"]).Length > 0)
                            jobOrderValue.type.conePlan = "Y";
                        else
                            jobOrderValue.type.conePlan = "N";

                        // RH/AH from location 추가
                        if (retCntr.ContainsKey("curLoc") && retCntr["curLoc"] is Hashtable)
                        {
                            var fromLoc = retCntr["curLoc"] as Hashtable;
                            if (fromLoc.ContainsKey("locTp") && fromLoc["locTp"] is Hashtable)
                                jobOrderValue.locFrom.locTp = Convert.ToString((fromLoc["locTp"] as Hashtable)["name"]);
                            jobOrderValue.locFrom.location = Convert.ToString(fromLoc["location"]);
                            jobOrderValue.locFrom.blck = Convert.ToString(fromLoc["blck"]);
                            jobOrderValue.locFrom.bay = Convert.ToString(fromLoc["bay"]);
                            jobOrderValue.locFrom.row = Convert.ToString(fromLoc["row"]);
                            jobOrderValue.locFrom.tier = Convert.ToString(fromLoc["tier"]);
                        }

                        // reefer 정보 추가
                        if (retCntr["reefer"] != null && retCntr["reefer"] is Hashtable)
                        {
                            Hashtable reefer = retCntr["reefer"] as Hashtable;
                            jobOrderValue.reefer.plugCd = Convert.ToString(reefer["plugCd"]);
                        }
                    }

                    //jobOrderValue.locTo;
                    //jobOrderValue.locFrom
                    if (retLoc != null)
                    {
                        if (retLoc.ContainsKey("locTp") && retLoc["locTp"] is Hashtable)
                            jobOrderValue.locWorking.locTp = Convert.ToString((retLoc["locTp"] as Hashtable)["name"]);

                        jobOrderValue.locWorking.location = Convert.ToString(retLoc["location"]);
                        jobOrderValue.locWorking.blck = Convert.ToString(retLoc["blck"]);
                        jobOrderValue.locWorking.bay = Convert.ToString(retLoc["bay"]);
                        jobOrderValue.locWorking.row = Convert.ToString(retLoc["row"]);
                        jobOrderValue.locWorking.tier = Convert.ToString(retLoc["tier"]);
                    }

                    // blck, bay, row, tier 가 block/bay info의 것과 매칭되지 않음..우선 location의 것으로 사용
                    //if (!String.IsNullOrEmpty(jobOrderValue.locWorking.location))
                    //{
                    //    String[] arry = jobOrderValue.locWorking.location.Split('-');
                    //    if (arry.Length == 4)
                    //    {
                    //        jobOrderValue.locWorking.blck = arry[0];
                    //        jobOrderValue.locWorking.bay = arry[1];
                    //        jobOrderValue.locWorking.row = arry[2];
                    //        jobOrderValue.locWorking.tier = arry[3];
                    //    }
                    //    else
                    //    {
                    //        int a = 0;
                    //    }
                    //}
                    //else
                    //{
                    //    int a = 0;
                    //}

                    if (retHashtable.ContainsKey("jobTp"))
                        jobOrderValue.type.jobTp = Convert.ToString(retHashtable["jobTp"]);
                    //if (jobOrderValue.type.jobTp.Equals("LC") || jobOrderValue.type.jobTp.Equals("GC"))
                    //{
                    //    int a = 0;
                    //}
                    if (retHashtable.ContainsKey("jobSts"))
                        jobOrderValue.type.jobStatus = Convert.ToString(retHashtable["jobSts"]);

                    if (retHashtable.ContainsKey("vsl"))
                    {
                        jobOrderValue.type.vslCd = Convert.ToString((retHashtable["vsl"] as Hashtable)["vessel"]);
                        jobOrderValue.type.voyNo = Convert.ToString((retHashtable["vsl"] as Hashtable)["voyage"]);
                        jobOrderValue.type.planSeq = Convert.ToString((retHashtable["vsl"] as Hashtable)["planSeq"]);
                    }

                    jobOrderValue.type.twinTandemFlg = Convert.ToString(retHashtable["twinTandemCd"]);
                    jobOrderValue.type.twinTandumKey = Convert.ToString(retHashtable["twinTandemKey"]);
                    jobOrderValue.type.ycTwinKey = Convert.ToString(retHashtable["ycTwinKey"]);
                    jobOrderValue.type.tandemJoinYT = Convert.ToString(retHashtable["neighborYt"]);
                    jobOrderValue.type.jobFlagInfo = Convert.ToString(retHashtable["positionOnChassis"]);
                    jobOrderValue.type.etw = Convert.ToString(retHashtable["etw"]);
                    jobOrderValue.commCode = Convert.ToString(retHashtable["commCode"]);
                    jobOrderValue.regBr = Convert.ToString(retHashtable["regBr"]);
                    jobOrderValue.prcMchnList = new List<Objects.Common.VD_Common_VmtPrcMachineList>();
                    if (retHashtable["prcMchnList"] != null)
                    {
                        foreach (Hashtable prcMchn in (ArrayList)retHashtable["prcMchnList"])
                        {
                            if (prcMchn.Count > 0)
                            {
                                Objects.Common.VD_Common_VmtPrcMachineList item = new Objects.Common.VD_Common_VmtPrcMachineList();
                                item.mchnId = Convert.ToString((prcMchn)["mchnId"]);
                                item.cntrNo = Convert.ToString((prcMchn)["cntrNo"]);
                                item.point = Convert.ToString((prcMchn)["point"]);
                                item.foreAfter = Convert.ToString((prcMchn)["foreAfter"]);
                                Hashtable retLoca = new Hashtable();
                                if ((prcMchn)["loc"] is Hashtable)
                                    retLoc = (prcMchn)["loc"] as Hashtable;
                                item.loc.blck = Convert.ToString(retLoca["blck"]);
                                item.loc.bay = Convert.ToString(retLoca["bay"]);
                                item.loc.row = Convert.ToString(retLoca["row"]);
                                item.loc.tier = Convert.ToString(retLoca["tier"]);
                                item.loc.location = Convert.ToString(retLoc["location"]);
                                item.wrkSts = Convert.ToString((prcMchn)["wrkSts"]);
                                item.twinTandemCd = Convert.ToString((prcMchn)["twinTandemCd"]);
                                item.twinTandemKey = Convert.ToString((prcMchn)["twinTandemKey"]);

                                jobOrderValue.prcMchnList.Add(item);
                            }
                        }
                    }
                    if (retHashtable.ContainsKey("waitingTime"))
                        jobOrderValue.type.waitingTime = Convert.ToString(retHashtable["waitingTime"]);

                    if (retHashtable.ContainsKey("qcId"))
                        jobOrderValue.type.qcId = Convert.ToString(retHashtable["qcId"]);

                    jobOrderForRMGValue.JobOrder.Add(jobOrderValue);
                }
            }

            //Logger.Log("===========================================================");

            //-----------------------------------------------------------------------------------------
            //- arrvdMchnAtPow (2015/01/07)
            //foreach (Object retObject in retObjectList)
            //{
            //    if (retObject is Hashtable)
            //    {
            //        Hashtable retHashtable = retObject as Hashtable;

            //        Hashtable retWorkingMchn = null;
            //        if (retHashtable["workingMchn"] is Hashtable)
            //            retWorkingMchn = retHashtable["workingMchn"] as Hashtable;

            //        String mchnTp = Convert.ToString(retWorkingMchn["mchnTp"]);

            //        String arrvdMchnAtPow = "";
            //        try
            //        {
            //            arrvdMchnAtPow = Convert.ToString(retHashtable["arrvdMchnAtPow"]);
            //        }
            //        catch (Exception ex)
            //        {
            //            String errorMessage = ex.Message;
            //            arrvdMchnAtPow = "";
            //        }
            //        finally
            //        {
            //            if (mchnTp.Equals("YT"))
            //            {
            //                //if (ITV.static_NotifyArrvdMchnAtPow != null)
            //                //    ITV.static_NotifyArrvdMchnAtPow(arrvdMchnAtPow);
            //            }
            //        }
            //    }
            //    break; // Only First Job
            //}
            return jobOrderForRMGValue;
        }

        static public void GetJobOrderList(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderForRMGValue = GetJobOrderListFromObject(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderRMG(jobOrderForRMGValue);

            jobOrderForRMGValue.Clear();
        }

        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList GetMachineListFromObject(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList partnerMachineRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList();

            ArrayList retArrayList = new ArrayList();
            if (obj is ArrayList)
                retArrayList = obj as ArrayList;

            try
            {
                foreach (Object retObject in retArrayList)
                {
                    if (retObject is Hashtable)
                    {
                        Hashtable machineHash = retObject as Hashtable;

                        if ((machineHash["mchnTp"] != null)) //&& Convert.ToString(machineHash["mchnTp"]).Equals("YT")
                        {
                            VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine machine = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine();
                            machine.mchnId = Convert.ToString(machineHash["mchnId"]);
                            machine.mchnTp = Convert.ToString(machineHash["mchnTp"]);
                            machine.mchnSts = Convert.ToString(machineHash["mchnSts"]);
                            machine.vrtlFlg = Convert.ToString(machineHash["vrtlFlg"]);

                            partnerMachineRMGValue.Machine.Add(machine);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                String msg = e.Message;
            }

            return partnerMachineRMGValue;
        }

        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList GetMachineListofPoolFromObject(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList partnerMachineListForRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList();

            ArrayList retArrayList = new ArrayList();
            if (obj is ArrayList)
                retArrayList = obj as ArrayList;

            try
            {
                foreach (Object retObject in retArrayList)
                {
                    if (retObject is Hashtable)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine machine = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine();
                        Hashtable machineHash = retObject as Hashtable;
                        machine.mchnId = Convert.ToString(machineHash["mchnId"]);
                        machine.mchnTp = Convert.ToString(machineHash["mchnTp"]);
                        machine.mchnSts = Convert.ToString(machineHash["mchnSts"]);
                        machine.vrtlFlg = Convert.ToString(machineHash["vrtlFlg"]);

                        partnerMachineListForRMGValue.Machine.Add(machine);
                    }
                }
            }
            catch (Exception e)
            {
                String msg = e.Message;
            }

            return partnerMachineListForRMGValue;
        }

        static public void GetMachineList(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList machineListForRMGValue = GetMachineListFromObject(obj);

            //var mahcineList = machineListForRMGValue.Machine.Where(machine => machine.mchnId.StartsWith("T")).ToList();

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyMachineList != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyMachineList(ref machineListForRMGValue);
        }

        static public void GetChangedMachineLocation(Object obj)
        {
            VMT_Data_JAT2.Objects.Common.VmtMachine machine = new VMT_Data_JAT2.Objects.Common.VmtMachine();

            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;

                machine.blck = Convert.ToString(retHashtable["blck"]);
                machine.bay = Convert.ToString(retHashtable["bay"]);
                machine.row = Convert.ToString(retHashtable["row"]);
                //machine.tier = Convert.ToString(retHashtable["tier"]);
                machine.locChgFlg = Convert.ToString(retHashtable["locChgFlg"]);
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyChangedMachineLocation != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyChangedMachineLocation(machine);
        }

        static public void GetMachineList4LogoutCheck(ref Object obj)
        {
            try
            {
                if (obj is ArrayList)
                {
                    var ret = new VMT_Data_JAT2.Objects.RMG.VD_RMG_MachineList_Receive();
                    foreach (Object retObject in obj as ArrayList)
                    {
                        if (retObject is Hashtable)
                        {
                            Hashtable machineHash = retObject as Hashtable;
                            var machine = new HessianComm.Objects.Machine();
                            machine.mchnId = Convert.ToString(machineHash["mchnId"]);
                            machine.mchnTp = Convert.ToString(machineHash["mchnTp"]);
                            machine.isLogOn = Convert.ToBoolean(machineHash["isLogOn"]);
                            machine.isOn = Convert.ToBoolean(machineHash["isOn"]);
                            machine.mchnSts = Convert.ToString(machineHash["mchnSts"]);
                            machine.vrtlFlg = Convert.ToString(machineHash["vrtlFlg"]);
                            machine.noticeMsg = Convert.ToString(machineHash["noticeMsg"]);
                            machine.autoFlg = Convert.ToString(machineHash["autoFlg"]);
                            if (machineHash.ContainsKey("loginUsrLst") && machineHash["loginUsrLst"] is ArrayList)
                            {
                                machine.loginUsrLst = new List<String>();
                                foreach (var loginUsr in machineHash["loginUsrLst"] as ArrayList)
                                    machine.loginUsrLst.Add(Convert.ToString(loginUsr));
                            }

                            ret.MachineList.Add(machine);
                        }
                    }

                    if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyMachineLogoutCheck != null)
                        VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyMachineLogoutCheck(ret);

                    ret.Dispose();
                    ret = null;
                }
            }
            catch (Exception e)
            {
                String msg = e.Message;
            }
        }

        static public void GetMachineListOfPool(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_PartnerMachineList machineListofPoolForRMGValue = GetMachineListofPoolFromObject(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyMachineListofPool != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyMachineListofPool(ref machineListofPoolForRMGValue);
        }

        static public void GetJobOrderByContainer(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderForRMGValue = GetJobOrderListFromObject(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderByContainer != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderByContainer(ref jobOrderForRMGValue);

            jobOrderForRMGValue.Clear();
        }

        static public void SetDetwinJob(ref Object obj)
        {
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyDetwinJob != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyDetwinJob((Boolean)obj);
        }

        static public void SetJobStatus(ref Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobSet_Receive();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTP = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifySetJobStatus != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifySetJobStatus(value);

            value = null;
        }

        static public void SetPickedContainer(ref Object obj)
        {
            //VMT_DataMgr_RMG_HessianCallback.SetJobDone(ref obj);
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTP = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }

            if (VMT_DataMgr_RMG_Callback.static_NotifyPickedContainer != null)
                VMT_DataMgr_RMG_Callback.static_NotifyPickedContainer(value);

            value = null;
        }

        static public void SetBestPick(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTP = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifySetBestPick != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifySetBestPick(value);

            value = null;
        }

        static public void Validate4LoadingSwapping(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTP = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyValidate4LoadingSwapping != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyValidate4LoadingSwapping(value);

            value = null;
        }

        static private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container GetContainerInfoFromHashtable(Hashtable cntrHash)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container cntr = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container();

            cntr.cntrNo = Convert.ToString(cntrHash["cntrNo"]);
            cntr.cntrIso = Convert.ToString(cntrHash["cntrIso"]);
            cntr.cntrLen = Convert.ToString(cntrHash["cntrLen"]);
            cntr.cntrHgt = Convert.ToString(cntrHash["cntrHgt"]);
            cntr.cntrTp = Convert.ToString(cntrHash["cntrTp"]);
            cntr.opr = Convert.ToString(cntrHash["opr"]);

            cntr.groupCode = Convert.ToString(cntrHash["groupCode"]);
            cntr.rfTemp = Convert.ToString(cntrHash["rfTemp"]);
            cntr.rfPlug = Convert.ToString(cntrHash["rfPlug"]);
            cntr.imdgCd = Convert.ToString(cntrHash["imdgCd"]);
            cntr.bkgNo = Convert.ToString(cntrHash["bkgNo"]);
            cntr.stkDay = Convert.ToString(cntrHash["stkDay"]);
            cntr.inVsl = Convert.ToString(cntrHash["inVsl"]);
            cntr.outVsl = Convert.ToString(cntrHash["outVsl"]);
            cntr.qcNo = Convert.ToString(cntrHash["qcNo"]);
            cntr.qcEtw = Convert.ToString(cntrHash["qcEtw"]);

            String wgt = Convert.ToString(cntrHash["cntrWgt"]);
            //cntr.cntrWgt = wgt.Length > 3 ? wgt.Substring(0, wgt.Length - 3) : wgt;
            if (String.IsNullOrEmpty(wgt))
                cntr.cntrWgt = wgt;
            else
                cntr.cntrWgt = (Convert.ToInt32(Convert.ToDouble(wgt)) / 1000).ToString();
            cntr.cls = Convert.ToString(cntrHash["cls"]);
            cntr.cntrSpTp = Convert.ToString(cntrHash["cntrSpTp"]);
            cntr.cntrCgoTp = Convert.ToString(cntrHash["cntrCgoTp"]);
            cntr.fullMty = Convert.ToString(cntrHash["fullMty"]);
            cntr.pod = Convert.ToString(cntrHash["pod"]);
            cntr.nPod = Convert.ToString(cntrHash["nPod"]);
            cntr.pol = Convert.ToString(cntrHash["pol"]);
            cntr.fPod = Convert.ToString(cntrHash["fPod"]);
            cntr.cntrGrade = Convert.ToString(cntrHash["cntrGrade"]);
            cntr.isHold = cntrHash.ContainsKey("isHold") ? Convert.ToBoolean(cntrHash["isHold"]) : false;
            cntr.chkHold = cntrHash.ContainsKey("chkHold") ? Convert.ToBoolean(cntrHash["chkHold"]) : false;
            cntr.chkDamage = cntrHash.ContainsKey("chkDamage") ? Convert.ToBoolean(cntrHash["chkDamage"]) : false;
            cntr.isDmg = (cntrHash["isDamage"] == null) ? false : Convert.ToBoolean(cntrHash["isDamage"]);
            cntr.isBundle = (cntrHash["isBundle"] == null) ? false : Convert.ToBoolean(cntrHash["isBundle"]);
            cntr.isBundleMaster = (cntrHash["isBundleMaster"] == null) ? false : Convert.ToBoolean(cntrHash["isBundleMaster"]);
            cntr.isHighCubic = (cntrHash["isHighCubic"] == null) ? false : Convert.ToBoolean(cntrHash["isHighCubic"]);
            cntr.rmk = Convert.ToString(cntrHash["rmk"]); //"TEST REMARK"
            //cntr.rmk = Convert.ToString("TEST REMARK"); //"TEST REMARK"

            if (cntrHash.ContainsKey("overValue") && cntrHash["overValue"] != null)
                cntr.overValue = Convert.ToString(cntrHash["overValue"]);

            if (cntrHash["reefer"] != null && cntrHash["reefer"] is Hashtable)
            {
                Hashtable reefer = cntrHash["reefer"] as Hashtable;
                cntr.reefer.reeferTemp = (float)Convert.ToDouble(Convert.ToString((reefer["reeferTemp"])));
                cntr.reefer.plugCd = Convert.ToString(reefer["plugCd"]);
                cntr.reefer.unit = Convert.ToString(reefer["unit"]);
            }
            else
                cntr.reefer.plugCd = (cntrHash["plugCd"] == null) ? String.Empty : Convert.ToString(cntrHash["plugCd"]);

            //if (cntrHash["dmg"] != null && cntrHash["dmg"] is ArrayList)
            //{
            //    foreach (Object item in cntrHash["dmg"] as ArrayList)
            //    {
            //        if (item is Hashtable)
            //        {
            //            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Damage dmg = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Damage();

            //            Hashtable dmgHash = item as Hashtable;
            //            dmg.dmgCd = (dmgHash["dmgCd"] == null) ? String.Empty : dmgHash["dmgCd"]);
            //            dmg.dmgInOut = (dmgHash["dmgInOut"] == null) ? String.Empty : dmgHash["dmgInOut"]);
            //            dmg.dmgPart = (dmgHash["dmgPart"] == null) ? String.Empty : dmgHash["dmgPart"]);
            //            dmg.dmgRange = (dmgHash["dmgRange"] == null) ? String.Empty : dmgHash["dmgRange"]);
            //            dmg.dmgDesc = (dmgHash["dmgDesc"] == null) ? String.Empty : dmgHash["dmgDesc"]);

            //            cntr.dmgList.Add(dmg);
            //        }
            //    }
            //    cntr.isDmg = cntr.dmgList.Count > 0;
            //}
            //else

            if (cntrHash["seal"] != null && cntrHash["seal"] is ArrayList)
            {
                foreach (Object item in cntrHash["seal"] as ArrayList)
                {
                    if (item is Hashtable)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Seal seal = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Seal();

                        Hashtable sealHash = item as Hashtable;
                        seal.sealNo = Convert.ToString(sealHash["sealNo"]);
                        seal.sealTp = Convert.ToString(sealHash["sealTp"]);

                        cntr.seaList.Add(seal);
                    }
                }
                cntr.isSeal = cntr.seaList.Count > 0;
            }

            if (cntrHash["imdg"] != null && cntrHash["imdg"] is ArrayList)
            {
                foreach (Object item in cntrHash["imdg"] as ArrayList)
                {
                    if (item is Hashtable)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Imdg imdg = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Imdg();

                        Hashtable imdgHash = item as Hashtable;
                        imdg.imdg = Convert.ToString(imdgHash["imdg"]);
                        imdg.unNo = Convert.ToString(imdgHash["unNo"]);
                        imdg.fireCd = Convert.ToString(imdgHash["fireCd"]);

                        cntr.imdgList.Add(imdg);
                    }
                }
            }

            return cntr;
        }

        static private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location GetLocationFromHashtable(Hashtable locationHash)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
            if (locationHash.ContainsKey("locTp") && locationHash["locTp"] is Hashtable)
                loc.locTp = Convert.ToString((locationHash["locTp"] as Hashtable)["name"]);
            loc.blck = Convert.ToString(locationHash["blck"]);
            loc.bay = Convert.ToString(locationHash["bay"]);
            loc.row = Convert.ToString(locationHash["row"]);
            loc.tier = Convert.ToString(locationHash["tier"]);
            if (locationHash.ContainsKey("location"))
                loc.location = Convert.ToString(locationHash["location"]);
            else
                loc.location = String.Format("{0}-{1}-{2}-{3}", loc.blck, loc.bay, loc.row, loc.tier);

            if (locationHash.ContainsKey("lane"))
                loc.lane = Convert.ToString(locationHash["lane"]);

            return loc;
        }

        static private VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel GetVesselFromHashtable(Hashtable vesselHash)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel vsl = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel();
            vsl.vessel = Convert.ToString(vesselHash["vessel"]);
            vsl.voyage = Convert.ToString(vesselHash["voyage"]);
            if (vesselHash.ContainsKey("vslLoc") && vesselHash["vslLoc"] is Hashtable)
            {
                vsl.vslLoc = GetLocationFromHashtable(vesselHash["vslLoc"] as Hashtable);
                vsl.vslLoc.location = String.Format("{0}-{1}-{2}",
                        vsl.vslLoc.bay, vsl.vslLoc.row, vsl.vslLoc.tier);
            }

            return vsl;
        }

        static private VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive GetContainerInfoFromObj(Object obj)
        {
            if (obj is Hashtable)
            {
                VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive containerInfoRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive();

                /*Hashtable retHashtable = obj as Hashtable;
                if (retHashtable.ContainsKey("cntr") && retHashtable["cntr"] is Hashtable)
                    containerInfoRMGValue.cntr = GetContainerInfoFromHashtable(retHashtable["cntr"] as Hashtable);

                if (retHashtable.ContainsKey("fmLoc") && retHashtable["fmLoc"] is Hashtable)
                    containerInfoRMGValue.fmLoc = GetLocationFromHashtable(retHashtable["fmLoc"] as Hashtable);

                if (retHashtable.ContainsKey("toLoc") && retHashtable["toLoc"] is Hashtable)
                    containerInfoRMGValue.toLoc = GetLocationFromHashtable(retHashtable["toLoc"] as Hashtable);

                if (retHashtable.ContainsKey("inVsl") && retHashtable["inVsl"] is Hashtable)
                    containerInfoRMGValue.inVsl = GetVesselFromHashtable(retHashtable["inVsl"] as Hashtable);

                if (retHashtable.ContainsKey("outVsl") && retHashtable["outVsl"] is Hashtable)
                    containerInfoRMGValue.outVsl = GetVesselFromHashtable(retHashtable["outVsl"] as Hashtable);*/

                Hashtable retHashtable = obj as Hashtable;
                containerInfoRMGValue.cntr.cls = Convert.ToString(retHashtable["cls"]);
                containerInfoRMGValue.cntr.cntrIso = Convert.ToString(retHashtable["cntrIso"]);
                containerInfoRMGValue.cntr.opr = Convert.ToString(retHashtable["opr"]);
                containerInfoRMGValue.cntr.fullMty = Convert.ToString(retHashtable["fullMty"]);
                containerInfoRMGValue.cntr.chkDamage = (Convert.ToString(retHashtable["isDamage"]).ToLower()) == "true" ? true : false;
                containerInfoRMGValue.cntr.isDmg = (Convert.ToString(retHashtable["isDamage"]).ToLower()) == "true" ? true : false;
                containerInfoRMGValue.cntr.groupCode = Convert.ToString(retHashtable["groupCode"]);
                containerInfoRMGValue.cntr.rfTemp = Convert.ToString(retHashtable["rfTemp"]);
                containerInfoRMGValue.cntr.rfPlug = Convert.ToString(retHashtable["rfPlug"]);
                containerInfoRMGValue.cntr.imdgCd = Convert.ToString(retHashtable["imdgCd"]);
                containerInfoRMGValue.cntr.cntrWgt = Convert.ToString(retHashtable["cntrWgt"]);
                containerInfoRMGValue.cntr.bkgNo = Convert.ToString(retHashtable["bkgNo"]);
                containerInfoRMGValue.cntr.stkDay = Convert.ToString(retHashtable["stkDay"]);

                containerInfoRMGValue.fmLoc.bay = Convert.ToString(retHashtable["fmBay"]);
                containerInfoRMGValue.fmLoc.blck = Convert.ToString(retHashtable["fmBlck"]);
                containerInfoRMGValue.fmLoc.location = Convert.ToString(retHashtable["fmBlck"]) != "" ? (Convert.ToString(retHashtable["fmBlck"]) + "-" + Convert.ToString(retHashtable["fmBay"]) + "-" + Convert.ToString(retHashtable["fmRow"]) + "-" + Convert.ToString(retHashtable["fmTier"])) : Convert.ToString(retHashtable["fmBlck"]);
                containerInfoRMGValue.fmLoc.locTp = Convert.ToString(retHashtable["fmLocTp"]);
                containerInfoRMGValue.fmLoc.row = Convert.ToString(retHashtable["fmRow"]);
                containerInfoRMGValue.fmLoc.tier = Convert.ToString(retHashtable["fmTier"]);

                containerInfoRMGValue.toLoc.bay = Convert.ToString(retHashtable["toBay"]);
                containerInfoRMGValue.toLoc.blck = Convert.ToString(retHashtable["toBlck"]);
                containerInfoRMGValue.toLoc.location = Convert.ToString(retHashtable["toBlck"]) != "" ? (Convert.ToString(retHashtable["toBlck"]) + "-" + Convert.ToString(retHashtable["toBay"]) + "-" + Convert.ToString(retHashtable["toRow"]) + "-" + Convert.ToString(retHashtable["toTier"])) : Convert.ToString(retHashtable["toBlck"]);
                containerInfoRMGValue.toLoc.locTp = Convert.ToString(retHashtable["toLocTp"]);
                containerInfoRMGValue.toLoc.row = Convert.ToString(retHashtable["toRow"]);
                containerInfoRMGValue.toLoc.tier = Convert.ToString(retHashtable["toTier"]);

                containerInfoRMGValue.inVsl.vessel = Convert.ToString(retHashtable["inVsl"]);
                containerInfoRMGValue.outVsl.vessel = Convert.ToString(retHashtable["outVsl"]);

                containerInfoRMGValue.cntr.pod = Convert.ToString(retHashtable["pod"]);
                containerInfoRMGValue.cntr.overValue = Convert.ToString(retHashtable["overValue"]);

                containerInfoRMGValue.rmk = Convert.ToString(retHashtable["rmk"]);
                return containerInfoRMGValue;
            }
            else
                return null;
        }

        static public void GetContainerInfo(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive containerInfo = GetContainerInfoFromObj(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetContainerInfo != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetContainerInfo(ref containerInfo);
        }

        static public void GetTwinContainerInfo(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive containerInfo = GetContainerInfoFromObj(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetTwinContainerInfo != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetTwinContainerInfo(ref containerInfo);
        }

        static public void SetMachineStop(ref Object obj) // Always Null return void type
        {
            if (VMT_DataMgr_RMG_Callback.static_NotifySetMachineStop != null)
                VMT_DataMgr_RMG_Callback.static_NotifySetMachineStop((String)obj);
        }

        static private VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive GetNoWorkAreaFromObj(Object obj)
        {
            var noWorkArea = new VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive();

            foreach (var item in (obj as ArrayList))
            {
                if (item is Hashtable)
                {
                    var noWorkLocation = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_NoWorkLocation();

                    Hashtable retHashtable = item as Hashtable;

                    if (retHashtable.ContainsKey("noWorkTp") && retHashtable["noWorkTp"] is Hashtable)
                    {
                        noWorkLocation.noWorkTp = Convert.ToString((retHashtable["noWorkTp"] as Hashtable)["name"]);
                    }

                    //if (retHashtable.ContainsKey("loc") && retHashtable["loc"] is Hashtable)
                    {
                        //noWorkLocation.loc = GetLocationFromHashtable(retHashtable["loc"] as Hashtable);
                        noWorkLocation.loc = GetLocationFromHashtable(retHashtable);

                        var bays = noWorkLocation.loc.bay.Split('-');
                        if (bays.Length >= 2)
                        {
                            noWorkLocation.FromBay = bays[0];
                            noWorkLocation.ToBay = bays[1];
                        }

                        var rows = noWorkLocation.loc.row.Split('-');
                        if (rows.Length >= 2)
                        {
                            noWorkLocation.FromRow = rows[0];
                            noWorkLocation.ToRow = rows[1];
                        }

                        var tiers = noWorkLocation.loc.tier.Split('-');
                        if (tiers.Length >= 2)
                        {
                            noWorkLocation.FromTier = tiers[0];
                            noWorkLocation.ToTier = tiers[1];
                        }
                    }

                    noWorkArea.NoWorkArea.Add(noWorkLocation);
                }
            }

            return noWorkArea;
        }

        static public void GetMaxRow(ref Object obj)
        {
            if (obj is String)
            {
                String maxRow = Convert.ToString(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetMaxRow != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetMaxRow(maxRow);

            }
        }

        static public void GetNoWorkArea(ref Object obj)
        {
            if (obj is ArrayList)
            {
                var noWorkArea = GetNoWorkAreaFromObj(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkArea != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkArea(noWorkArea);

                noWorkArea.Dispose();
            }
        }

        static public void GetNoWorkArea1(ref Object obj)
        {
            if (obj is ArrayList)
            {
                var noWorkArea = GetNoWorkAreaFromObj(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkArea1 != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkArea1(noWorkArea);

                noWorkArea.Dispose();
            }
        }

        static public void GetNoWorkArea2(ref Object obj)
        {
            if (obj is ArrayList)
            {
                var noWorkArea = GetNoWorkAreaFromObj(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkArea2 != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkArea2(noWorkArea);

                noWorkArea.Dispose();
            }
        }

        static public void GetNoWorkTier(ref Object obj)
        {
            if (obj is ArrayList)
            {
                var noWorkTier = GetNoWorkAreaFromObj(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkTier != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkTier(noWorkTier);

                noWorkTier.Dispose();
            }
        }

        static public void GetNoWorkTier1(ref Object obj)
        {
            if (obj is ArrayList)
            {
                var noWorkTier = GetNoWorkAreaFromObj(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkTier1 != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkTier1(noWorkTier);

                noWorkTier.Dispose();
            }
        }

        static public void GetNoWorkTier2(ref Object obj)
        {
            if (obj is ArrayList)
            {
                var noWorkTier = GetNoWorkAreaFromObj(obj);

                if (VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkTier2 != null)
                    VMT_DataMgr_RMG_Callback.static_NotifyGetNoWorkTier2(noWorkTier);

                noWorkTier.Dispose();
            }
        }

        static public void SetChagePosition(Object obj)
        {
            if (VMT_DataMgr_RMG_Callback.static_NotifyChangePosition != null)
                VMT_DataMgr_RMG_Callback.static_NotifyChangePosition(obj);
        }

        static public void GetIsValidLocation(Object obj)
        {
            if (VMT_DataMgr_RMG_Callback.static_NotifyIsValidLocation != null)
                VMT_DataMgr_RMG_Callback.static_NotifyIsValidLocation(Convert.ToBoolean(obj));
        }

        static public void SetJobDone(ref Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTP = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }

            if (VMT_DataMgr_RMG_Callback.static_NotifyJobDone != null)
                VMT_DataMgr_RMG_Callback.static_NotifyJobDone(value);

            value = null;
        }

        static public void SetJobDoneForLocationChange(ref Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobDone_Receive();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTP = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }

            if (VMT_DataMgr_RMG_Callback.static_NotifyJobDoneForLocationChange != null)
                VMT_DataMgr_RMG_Callback.static_NotifyJobDoneForLocationChange(value);

            value = null;
        }

        static public void SetJobReOnChassis(Object obj)
        {
            if (VMT_DataMgr_RMG_Callback.static_NotifySetJobReOnChassis != null)
                VMT_DataMgr_RMG_Callback.static_NotifySetJobReOnChassis(Convert.ToBoolean(obj));
        }

        static public void GetDriverJobHistory(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList();
            value.JobOrder = new List<Objects.Common.VD_Common_JobOrder>();

            List<Object> retObjectList = new List<object>();

            if (obj is List<Object>)
                retObjectList = obj as List<Object>;
            else if (obj is ArrayList)
                retObjectList = (obj as ArrayList).Cast<object>().ToList();

            foreach (Object retObject in retObjectList)
            {
                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;

                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderValue = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                    jobOrderValue.workingMchn.mchnId = Convert.ToString(retHashtable["mchnId"]);
                    jobOrderValue.cntr.cntrNo = Convert.ToString(retHashtable["cntrNo"]);
                    jobOrderValue.cntr.cntrIso = Convert.ToString(retHashtable["cntrIso"]);
                    jobOrderValue.type.jobTp = Convert.ToString(retHashtable["jobTp"]);
                    jobOrderValue.jobTpKor = Convert.ToString(retHashtable["jobTpKor"]);
                    jobOrderValue.partnerMchn.mchnId = Convert.ToString(retHashtable["ytNo"]);
                    jobOrderValue.locWorking.location = Convert.ToString(retHashtable["toLocation"]);
                    jobOrderValue.podCd = Convert.ToString(retHashtable["podCd"]);

                    value.JobOrder.Add(jobOrderValue);
                }
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetDriverJobHistory != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetDriverJobHistory(value);
        }

        static public void CheckYcDeTwin(Object obj)
        {
            if (VMT_DataMgr_RMG_Callback.static_NotifyCheckYcDeTwin != null)
                VMT_DataMgr_RMG_Callback.static_NotifyCheckYcDeTwin(Convert.ToBoolean(obj));
        }

        static public void GetJobOrderList_New(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderForRMGValue = GetJobOrderListFromObject_New(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderRMG(jobOrderForRMGValue);

            jobOrderForRMGValue.Clear();
        }
        static public void GetJobOrderList_RMG(Object obj)
        {
            List<Objects.Common.VD_Common_JobOrder> listJobOrder = new List<Objects.Common.VD_Common_JobOrder>();

            List<Object> retObjectList = new List<object>();

            if (obj is List<Object>)
                retObjectList = obj as List<Object>;

            foreach (Object retObject in retObjectList)
            {
                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;

                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderValue = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                    jobOrderValue.workingMchn = new Objects.Common.VD_Common_Job_Machine();
                    jobOrderValue.partnerMchn = new Objects.Common.VD_Common_Job_Machine();
                    jobOrderValue.cntr = new Objects.Common.VD_Common_Job_Container();
                    jobOrderValue.locWorking = new Objects.Common.VD_Common_Job_Location();
                    jobOrderValue.type = new Objects.Common.VD_Common_Job_Type();

                    if (retHashtable.ContainsKey("jobId") && retHashtable["jobId"] != null)
                        jobOrderValue.jobKey = Convert.ToString(retHashtable["jobId"]);
                    else
                        jobOrderValue.jobKey = String.Empty;

                    jobOrderValue.taskId = Convert.ToString(retHashtable["taskId"]);
                    jobOrderValue.priorityJob = Convert.ToString(retHashtable["priorityJob"]);     // hot job     

                    jobOrderValue.vbsDate = Convert.ToString(retHashtable["vbsDate"]); //20190627 for sort job

                    // working machine                    
                    jobOrderValue.workingMchn.mchnId = Convert.ToString(retHashtable["ycNo"]);
                    jobOrderValue.ycNo = Convert.ToString(retHashtable["ycNo"]);
                    jobOrderValue.workingMchn.mchnTp = Convert.ToString(retHashtable["ycTp"]);    // RMG / ECH
                    jobOrderValue.workingMchn.mchnSts = Convert.ToString(retHashtable["ycSts"]);
                    //

                    // partner machine                    
                    jobOrderValue.partnerMchn.mchnId = Convert.ToString(retHashtable["ytNo"]);
                    jobOrderValue.partnerMchn.mchnTp = "YT";     // ITV / OTR
                    jobOrderValue.partnerMchn.mchnSts = Convert.ToString(retHashtable["ytSts"]);
                    jobOrderValue.partnerMchn.aprchLn = Convert.ToString(retHashtable["ytAprchLn"]);
                    //

                    // container  
                    jobOrderValue.cntr.cntrNo = Convert.ToString(retHashtable["cntrNo"]);
                    jobOrderValue.cntr.cntrIso = Convert.ToString(retHashtable["cntrIso"]);
                    jobOrderValue.cntr.cntrLen = Convert.ToString(retHashtable["cntrLen"]);
                    jobOrderValue.cntr.cntrTp = Convert.ToString(retHashtable["cntrTp"]);
                    jobOrderValue.cntr.cls = Convert.ToString(retHashtable["cls"]);
                    jobOrderValue.cntr.opr = Convert.ToString(retHashtable["opr"]);
                    jobOrderValue.cntr.cntrCgoTp = Convert.ToString(retHashtable["cntrCgoTp"]);
                    jobOrderValue.cntr.fullMty = Convert.ToString(retHashtable["fullMty"]);
                    String wgt = Convert.ToString(retHashtable["cntrWgt"]);
                    //jobOrderValue.cntr.cntrWgt = wgt.Length > 3 ? wgt.Substring(0, wgt.Length - 3) : wgt;
                    if (String.IsNullOrEmpty(wgt))
                        jobOrderValue.cntr.cntrWgt = wgt;
                    else
                        jobOrderValue.cntr.cntrWgt = (Convert.ToInt32(Convert.ToDouble(wgt)) / 1000).ToString();
                    jobOrderValue.cntr.pod = Convert.ToString(retHashtable["pod"]);
                    jobOrderValue.cntr.cntrSpTp = Convert.ToString(retHashtable["cntrSpTp"]);
                    jobOrderValue.cntr.cntrHgt = Convert.ToString(retHashtable["cntrHgt"]);

                    if (retHashtable.ContainsKey("imdgCd"))
                        jobOrderValue.cntr.imdgCd = Convert.ToString(retHashtable["imdgCd"]);

                    if (Convert.ToString(retHashtable["conePlan"]).Length > 0)
                        jobOrderValue.type.conePlan = "Y";
                    else
                        jobOrderValue.type.conePlan = "N";

                    // From Location : RH/AH from location 추가                    
                    if (retHashtable.ContainsKey("fmLocTp") && retHashtable["fmLocTp"] is Hashtable)
                        jobOrderValue.locFrom.locTp = Convert.ToString((retHashtable["fmLocTp"] as Hashtable)["name"]);
                    jobOrderValue.locFrom.location = Convert.ToString(retHashtable["fmLocation"]);
                    jobOrderValue.locFrom.blck = Convert.ToString(retHashtable["fmBlck"]);
                    jobOrderValue.locFrom.bay = Convert.ToString(retHashtable["fmBay"]);
                    jobOrderValue.locFrom.row = Convert.ToString(retHashtable["fmRow"]);
                    jobOrderValue.locFrom.tier = Convert.ToString(retHashtable["fmTier"]);
                    //

                    // reefer 정보 추가
                    if (retHashtable.ContainsKey("reefer") && retHashtable["reefer"] is Hashtable)
                    {
                        Hashtable reefer = retHashtable["reefer"] as Hashtable;
                        jobOrderValue.reefer.plugCd = Convert.ToString(reefer["plugCd"]);
                    }
                    else
                        jobOrderValue.reefer.plugCd = Convert.ToString(retHashtable["plugCd"]);

                    // To Location
                    if (retHashtable.ContainsKey("toLocTp") && retHashtable["toLocTp"] is Hashtable)
                        jobOrderValue.locWorking.locTp = Convert.ToString((retHashtable["toLocTp"] as Hashtable)["name"]);
                    jobOrderValue.locWorking.location = Convert.ToString(retHashtable["toLocation"]);
                    jobOrderValue.locWorking.blck = Convert.ToString(retHashtable["toBlck"]);
                    jobOrderValue.locWorking.bay = Convert.ToString(retHashtable["toBay"]);
                    jobOrderValue.locWorking.row = Convert.ToString(retHashtable["toRow"]);
                    jobOrderValue.locWorking.tier = Convert.ToString(retHashtable["toTier"]);
                    //

                    if (retHashtable.ContainsKey("jobTp"))
                        jobOrderValue.type.jobTp = Convert.ToString(retHashtable["jobTp"]);

                    if (retHashtable.ContainsKey("jobSts"))
                        jobOrderValue.type.jobStatus = Convert.ToString(retHashtable["jobSts"]);

                    if (retHashtable.ContainsKey("cntrGrade"))
                        jobOrderValue.cntr.cntrGrade = Convert.ToString(retHashtable["cntrGrade"]);

                    jobOrderValue.type.vslCd = Convert.ToString(retHashtable["vessel"]);
                    jobOrderValue.type.voyNo = Convert.ToString(retHashtable["voyage"]);
                    jobOrderValue.type.planSeq = Convert.ToString(retHashtable["planSeq"]);

                    jobOrderValue.type.twinTandemFlg = Convert.ToString(retHashtable["twinTandemCd"]);
                    jobOrderValue.type.twinTandumKey = Convert.ToString(retHashtable["twinTandemKey"]);
                    jobOrderValue.type.ycTwinKey = Convert.ToString(retHashtable["ycTwinKey"]);
                    jobOrderValue.type.tandemJoinYT = Convert.ToString(retHashtable["neighborYt"]);
                    jobOrderValue.type.jobFlagInfo = Convert.ToString(retHashtable["positionOnChassis"]);
                    jobOrderValue.type.etw = Convert.ToString(retHashtable["etw"]);

                    if (retHashtable.ContainsKey("waitingTime"))
                        jobOrderValue.type.waitingTime = Convert.ToString(retHashtable["waitingTime"]);

                    if (retHashtable.ContainsKey("qcNo"))
                        jobOrderValue.type.qcId = Convert.ToString(retHashtable["qcNo"]);

                    if (retHashtable.ContainsKey("isArrivedItv"))
                        jobOrderValue.type.isArrivedItv = Convert.ToBoolean(retHashtable["isArrivedItv"]);

                    jobOrderValue.isCmtJob = Convert.ToBoolean(retHashtable["isCmtJob"]);
                    jobOrderValue.isSwap = Convert.ToBoolean(retHashtable["isSwap"]);
                    jobOrderValue.commCode = Convert.ToString(retHashtable["commCode"]);
                    jobOrderValue.regBr = Convert.ToString(retHashtable["regBr"]);
                    jobOrderValue.vslHoldDeck = Convert.ToString(retHashtable["vslHoldDeck"]);
                    jobOrderValue.doorDir = Convert.ToString(retHashtable["doorDir"]);
                    jobOrderValue.podCd = Convert.ToString(retHashtable["podCd"]);
                    jobOrderValue.ytJbSts = Convert.ToString(retHashtable["ytJbSts"]);

                    jobOrderValue.jobTpKor = Convert.ToString(retHashtable["jobTpKor"]);
                    jobOrderValue.jobTpKorShort = Convert.ToString(retHashtable["jobTpKorShort"]);
                    jobOrderValue.foreAfterKor = Convert.ToString(retHashtable["foreAfterKor"]);
                    jobOrderValue.doorKor = Convert.ToString(retHashtable["DoorKor"]);

                    jobOrderValue.prcMchnList = new List<Objects.Common.VD_Common_VmtPrcMachineList>();
                    if (retHashtable["prcMchnList"] != null)
                    {
                        foreach (Hashtable prcMchn in (ArrayList)retHashtable["prcMchnList"])
                        {
                            if (prcMchn.Count > 0)
                            {
                                Objects.Common.VD_Common_VmtPrcMachineList item = new Objects.Common.VD_Common_VmtPrcMachineList();
                                item.mchnId = Convert.ToString((prcMchn)["mchnId"]);
                                item.cntrNo = Convert.ToString((prcMchn)["cntrNo"]);
                                item.point = Convert.ToString((prcMchn)["point"]);
                                item.foreAfter = Convert.ToString((prcMchn)["foreAfter"]);
                                Hashtable retLoc = new Hashtable();
                                if ((prcMchn)["loc"] != null && (prcMchn)["loc"] is Hashtable)
                                {
                                    retLoc = (prcMchn)["loc"] as Hashtable;

                                    item.loc.blck = Convert.ToString(retLoc["blck"]);
                                    item.loc.bay = Convert.ToString(retLoc["bay"]);
                                    item.loc.row = Convert.ToString(retLoc["row"]);
                                    item.loc.tier = Convert.ToString(retLoc["tier"]);
                                    item.loc.location = Convert.ToString(retLoc["location"]);
                                    item.wrkSts = Convert.ToString((prcMchn)["wrkSts"]);
                                    item.twinTandemCd = Convert.ToString((prcMchn)["twinTandemCd"]);
                                    item.twinTandemKey = Convert.ToString((prcMchn)["twinTandemKey"]);
                                }

                                jobOrderValue.prcMchnList.Add(item);
                            }
                        }
                    }
                    listJobOrder.Add(jobOrderValue);
                }
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderList != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderList(listJobOrder);
        }
        static public void GetMachineStatusChanged(Object obj)
        {
            VMT_Data_JAT2.Objects.Common.VmtMachine machine = new VMT_Data_JAT2.Objects.Common.VmtMachine();

            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;

                machine.mchnId = Convert.ToString(retHashtable["mchnId"]);
                machine.mchnTp = Convert.ToString(retHashtable["mchnTp"]);
                machine.isLogOn = Convert.ToBoolean(retHashtable["isLogOn"]);
                machine.isOn = Convert.ToBoolean(retHashtable["isOn"]);
                machine.mchnSts = Convert.ToString(retHashtable["mchnSts"]);
                machine.vrtlFlg = Convert.ToString(retHashtable["vrtlFlg"]);
                machine.noticeMsg = Convert.ToString(retHashtable["noticeMsg"]);
                machine.noticeMsg = Convert.ToString(retHashtable["noticeMsg"]);
                machine.autoFlg = Convert.ToString(retHashtable["autoFlg"]);
                machine.wgtSysStsCd = Convert.ToString(retHashtable["wgtSysStsCd"]);

                machine.loginUsrLst = new List<String>();
                if (retHashtable["loginUsrLst"] is ArrayList)
                {
                    foreach (var log in retHashtable["loginUsrLst"] as ArrayList)
                        machine.loginUsrLst.Add(Convert.ToString(log));
                }
                machine.hatchQcList = new List<String>();
                if (retHashtable["hatchQcList"] is ArrayList)
                {
                    foreach (var hatch in retHashtable["hatchQcList"] as ArrayList)
                        machine.hatchQcList.Add(Convert.ToString(hatch));
                }

                if (retHashtable["mchnMd"] != null) machine.mchnMd = Convert.ToString(retHashtable["mchnMd"]);
            }

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetMachineStatusChanged != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyGetMachineStatusChanged(machine);
        }

        static public void GetSwapList(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_SwapList swapListForRMGValue = GetSwapListFromObject(obj);

            if (VMT_DataMgr_RMG_Callback.static_NotifySwapListRMG != null)
                VMT_DataMgr_RMG_Callback.static_NotifySwapListRMG(swapListForRMGValue);

            swapListForRMGValue.Clear();
        }
        static public void GetSwapListRTG(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_SwapList swapListRTG = new VMT_Data_JAT2.Objects.RMG.VD_RMG_SwapList();
            swapListRTG.vmtSwap = new List<Objects.Common.VmtSwap>();

            List<Object> retObjectList = new List<object>();
            if (obj is List<Object>)
                retObjectList = obj as List<Object>;

            foreach (Object retObject in retObjectList)
            {
                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;

                    VMT_Data_JAT2.Objects.Common.VmtSwap swapValue = new VMT_Data_JAT2.Objects.Common.VmtSwap();

                    swapValue.regoNo = Convert.ToString(retHashtable["regoNo"]); //truck No
                    swapValue.cntrNo = Convert.ToString(retHashtable["cntrNo"]);
                    swapValue.cntrPnt = Convert.ToString(retHashtable["cntrPnt"]);
                    swapValue.cntrTp = Convert.ToString(retHashtable["cntrTp"]);
                    swapValue.cntrIso = Convert.ToString(retHashtable["cntrIso"]);
                    swapValue.cntrCmdt = Convert.ToString(retHashtable["cntrCmdt"]);
                    swapValue.opr = Convert.ToString(retHashtable["opr"]);
                    swapValue.swapPos = Convert.ToString(retHashtable["swapPos"]); //Container Position

                    swapValue.psnIdx1 = Convert.ToString(retHashtable["psnIdx1"]);
                    swapValue.psnIdx2 = Convert.ToString(retHashtable["psnIdx2"]);
                    swapValue.psnIdx3 = Convert.ToString(retHashtable["psnIdx3"]);
                    swapValue.psnIdx4 = Convert.ToString(retHashtable["psnIdx4"]);

                    swapListRTG.vmtSwap.Add(swapValue);
                }
            }
            if (VMT_DataMgr_RMG_Callback.static_NotifySwapListRTG != null)
                VMT_DataMgr_RMG_Callback.static_NotifySwapListRTG(swapListRTG);

            swapListRTG.Clear();
        }
        static public void SetEmptySwap(ref object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTp = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_DataMgr_RMG_Callback.static_NotifySetSwapRMG != null)
                VMT_DataMgr_RMG_Callback.static_NotifySetSwapRMG(value);

            value = null;
        }

        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_SwapList GetSwapListFromObject(Object obj)
        {
            //-----------------------------------------------------------------------------------------            
            VMT_Data_JAT2.Objects.RMG.VD_RMG_SwapList swapForRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_SwapList();
            swapForRMGValue.vmtSwap = new List<Objects.Common.VmtSwap>();

            List<Object> retObjectList = new List<object>();
            if (obj is List<Object>)
                retObjectList = obj as List<Object>;

            //Logger.Log("===========================================================");

            foreach (Object retObject in retObjectList)
            {
                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;

                    VMT_Data_JAT2.Objects.Common.VmtSwap swapValue = new VMT_Data_JAT2.Objects.Common.VmtSwap();

                    swapValue.regoNo = Convert.ToString(retHashtable["regoNo"]); //truck No
                    swapValue.cntrNo = Convert.ToString(retHashtable["cntrNo"]);
                    swapValue.cntrPnt = Convert.ToString(retHashtable["cntrPnt"]);
                    swapValue.cntrTp = Convert.ToString(retHashtable["cntrTp"]);
                    swapValue.cntrIso = Convert.ToString(retHashtable["cntrIso"]);
                    swapValue.cntrCmdt = Convert.ToString(retHashtable["cntrCmdt"]);
                    swapValue.opr = Convert.ToString(retHashtable["opr"]);
                    swapValue.swapPos = Convert.ToString(retHashtable["swapPos"]); //Container Position

                    swapValue.psnIdx1 = Convert.ToString(retHashtable["psnIdx1"]);
                    swapValue.psnIdx2 = Convert.ToString(retHashtable["psnIdx2"]);
                    swapValue.psnIdx3 = Convert.ToString(retHashtable["psnIdx3"]);
                    swapValue.psnIdx4 = Convert.ToString(retHashtable["psnIdx4"]);

                    swapForRMGValue.vmtSwap.Add(swapValue);
                }
            }
            return swapForRMGValue;
        }

        static public void GetJobOrderByContainer_New(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderForRMGValue = GetJobOrderListFromObject_New(obj);

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderByContainer != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyJobOrderByContainer(ref jobOrderForRMGValue);

            jobOrderForRMGValue.Clear();
        }

        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList GetJobOrderListFromObject_New(Object obj)
        {
            //-----------------------------------------------------------------------------------------            
            VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList jobOrderForRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_JobOrderList();
            jobOrderForRMGValue.JobOrder = new List<Objects.Common.VD_Common_JobOrder>();

            List<Object> retObjectList = new List<object>();
            if (obj is List<Object>)
                retObjectList = obj as List<Object>;

            //Logger.Log("===========================================================");
            int count = retObjectList.Count;
            for (int i = 0; i < count; i++)
            {
                if (retObjectList[i] is Hashtable)
                {
                    Hashtable retHashtable = retObjectList[i] as Hashtable;

                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderValue = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                    jobOrderValue.workingMchn = new Objects.Common.VD_Common_Job_Machine();
                    jobOrderValue.partnerMchn = new Objects.Common.VD_Common_Job_Machine();
                    jobOrderValue.cntr = new Objects.Common.VD_Common_Job_Container();
                    jobOrderValue.locWorking = new Objects.Common.VD_Common_Job_Location();
                    jobOrderValue.type = new Objects.Common.VD_Common_Job_Type();

                    if (retHashtable.ContainsKey("jobId") && retHashtable["jobId"] != null)
                        jobOrderValue.jobKey = Convert.ToString(retHashtable["jobId"]);
                    else
                        jobOrderValue.jobKey = String.Empty;

                    jobOrderValue.taskId = Convert.ToString(retHashtable["taskId"]);
                    jobOrderValue.priorityJob = Convert.ToString(retHashtable["priorityJob"]);     // hot job     

                    jobOrderValue.vbsDate = Convert.ToString(retHashtable["vbsDate"]); //20190627 for sort job

                    // working machine                    
                    jobOrderValue.workingMchn.mchnId = Convert.ToString(retHashtable["ycNo"]);
                    jobOrderValue.workingMchn.mchnTp = Convert.ToString(retHashtable["ycTp"]);    // RMG / ECH
                    jobOrderValue.workingMchn.mchnSts = Convert.ToString(retHashtable["ycSts"]);
                    //

                    // partner machine                    
                    jobOrderValue.partnerMchn.mchnId = Convert.ToString(retHashtable["ytNo"]);
                    jobOrderValue.partnerMchn.mchnTp = "YT";     // ITV / OTR
                    jobOrderValue.partnerMchn.mchnSts = Convert.ToString(retHashtable["ytSts"]);
                    jobOrderValue.partnerMchn.aprchLn = Convert.ToString(retHashtable["ytAprchLn"]);
                    //

                    // container  
                    jobOrderValue.cntr.cntrNo = Convert.ToString(retHashtable["cntrNo"]);
                    jobOrderValue.cntr.cntrIso = Convert.ToString(retHashtable["cntrIso"]);
                    jobOrderValue.cntr.cntrLen = Convert.ToString(retHashtable["cntrLen"]);
                    jobOrderValue.cntr.cntrTp = Convert.ToString(retHashtable["cntrTp"]);
                    jobOrderValue.cntr.cls = Convert.ToString(retHashtable["cls"]);
                    jobOrderValue.cntr.opr = Convert.ToString(retHashtable["opr"]);
                    jobOrderValue.cntr.cntrCgoTp = Convert.ToString(retHashtable["cntrCgoTp"]);
                    jobOrderValue.cntr.fullMty = Convert.ToString(retHashtable["fullMty"]);
                    String wgt = Convert.ToString(retHashtable["cntrWgt"]);
                    //jobOrderValue.cntr.cntrWgt = wgt.Length > 3 ? wgt.Substring(0, wgt.Length - 3) : wgt;
                    if (String.IsNullOrEmpty(wgt))
                        jobOrderValue.cntr.cntrWgt = wgt;
                    else
                        jobOrderValue.cntr.cntrWgt = (Convert.ToInt32(Convert.ToDouble(wgt)) / 1000).ToString();
                    jobOrderValue.cntr.pod = Convert.ToString(retHashtable["pod"]);
                    jobOrderValue.cntr.cntrSpTp = Convert.ToString(retHashtable["cntrSpTp"]);
                    jobOrderValue.cntr.cntrHgt = Convert.ToString(retHashtable["cntrHgt"]);

                    if (retHashtable.ContainsKey("imdgCd"))
                        jobOrderValue.cntr.imdgCd = Convert.ToString(retHashtable["imdgCd"]);

                    if (Convert.ToString(retHashtable["conePlan"]).Length > 0)
                        jobOrderValue.type.conePlan = "Y";
                    else
                        jobOrderValue.type.conePlan = "N";

                    // From Location : RH/AH from location 추가                    
                    if (retHashtable.ContainsKey("fmLocTp") && retHashtable["fmLocTp"] is Hashtable)
                        jobOrderValue.locFrom.locTp = Convert.ToString((retHashtable["fmLocTp"] as Hashtable)["name"]);
                    jobOrderValue.locFrom.location = Convert.ToString(retHashtable["fmLocation"]);
                    jobOrderValue.locFrom.blck = Convert.ToString(retHashtable["fmBlck"]);
                    jobOrderValue.locFrom.bay = Convert.ToString(retHashtable["fmBay"]);
                    jobOrderValue.locFrom.row = Convert.ToString(retHashtable["fmRow"]);
                    jobOrderValue.locFrom.tier = Convert.ToString(retHashtable["fmTier"]);
                    //

                    // reefer 정보 추가
                    if (retHashtable.ContainsKey("reefer") && retHashtable["reefer"] is Hashtable)
                    {
                        Hashtable reefer = retHashtable["reefer"] as Hashtable;
                        jobOrderValue.reefer.plugCd = Convert.ToString(reefer["plugCd"]);
                    }
                    else
                        jobOrderValue.reefer.plugCd = Convert.ToString(retHashtable["plugCd"]);

                    // To Location
                    if (retHashtable.ContainsKey("toLocTp") && retHashtable["toLocTp"] is Hashtable)
                        jobOrderValue.locWorking.locTp = Convert.ToString((retHashtable["toLocTp"] as Hashtable)["name"]);
                    jobOrderValue.locWorking.location = Convert.ToString(retHashtable["toLocation"]);
                    jobOrderValue.locWorking.blck = Convert.ToString(retHashtable["toBlck"]);
                    jobOrderValue.locWorking.bay = Convert.ToString(retHashtable["toBay"]);
                    jobOrderValue.locWorking.row = Convert.ToString(retHashtable["toRow"]);
                    jobOrderValue.locWorking.tier = Convert.ToString(retHashtable["toTier"]);
                    //

                    if (retHashtable.ContainsKey("jobTp"))
                        jobOrderValue.type.jobTp = Convert.ToString(retHashtable["jobTp"]);
                    //if (jobOrderValue.type.jobTp == "DS")
                    //    jobOrderValue.type.jobTp = "GC";

                    if (retHashtable.ContainsKey("jobSts"))
                        jobOrderValue.type.jobStatus = Convert.ToString(retHashtable["jobSts"]);


                    jobOrderValue.type.vslCd = Convert.ToString(retHashtable["vessel"]);
                    jobOrderValue.type.voyNo = Convert.ToString(retHashtable["voyage"]);
                    jobOrderValue.type.planSeq = Convert.ToString(retHashtable["planSeq"]);

                    jobOrderValue.type.twinTandemFlg = Convert.ToString(retHashtable["twinTandemCd"]);
                    jobOrderValue.type.twinTandumKey = Convert.ToString(retHashtable["twinTandemKey"]);
                    jobOrderValue.type.ycTwinKey = Convert.ToString(retHashtable["ycTwinKey"]);
                    jobOrderValue.type.tandemJoinYT = Convert.ToString(retHashtable["neighborYt"]);
                    jobOrderValue.type.jobFlagInfo = Convert.ToString(retHashtable["positionOnChassis"]);
                    jobOrderValue.type.etw = Convert.ToString(retHashtable["etw"]);

                    if (retHashtable.ContainsKey("waitingTime"))
                        jobOrderValue.type.waitingTime = Convert.ToString(retHashtable["waitingTime"]);

                    if (retHashtable.ContainsKey("qcNo"))
                        jobOrderValue.type.qcId = Convert.ToString(retHashtable["qcNo"]);

                    if (retHashtable.ContainsKey("isArrivedItv"))
                        jobOrderValue.type.isArrivedItv = Convert.ToBoolean(retHashtable["isArrivedItv"]);

                    jobOrderValue.isCmtJob = Convert.ToBoolean(retHashtable["isCmtJob"]);
                    jobOrderValue.isSwap = Convert.ToBoolean(retHashtable["isSwap"]);
                    jobOrderValue.commCode = Convert.ToString(retHashtable["commCode"]);
                    jobOrderValue.regBr = Convert.ToString(retHashtable["regBr"]);
                    jobOrderValue.vslHoldDeck = Convert.ToString(retHashtable["vslHoldDeck"]);
                    jobOrderValue.doorDir = Convert.ToString(retHashtable["doorDir"]);
                    jobOrderValue.podCd = Convert.ToString(retHashtable["podCd"]);
                    jobOrderValue.ytJbSts = Convert.ToString(retHashtable["ytJbSts"]);

                    jobOrderValue.jobTpKor = Convert.ToString(retHashtable["jobTpKor"]);
                    jobOrderValue.jobTpKorShort = Convert.ToString(retHashtable["jobTpKorShort"]);
                    jobOrderValue.foreAfterKor = Convert.ToString(retHashtable["foreAfterKor"]);
                    jobOrderValue.doorKor = Convert.ToString(retHashtable["DoorKor"]);
                    jobOrderValue.spndFlg = Convert.ToString(retHashtable["spndFlg"]);
                    jobOrderValue.batNo = Convert.ToString(retHashtable["batNo"]);

                    jobOrderValue.prcMchnList = new List<Objects.Common.VD_Common_VmtPrcMachineList>();
                    if (retHashtable["prcMchnList"] != null)
                    {
                        foreach (Hashtable prcMchn in (ArrayList)retHashtable["prcMchnList"])
                        {
                            if (prcMchn.Count > 0)
                            {
                                Objects.Common.VD_Common_VmtPrcMachineList item = new Objects.Common.VD_Common_VmtPrcMachineList();
                                item.mchnId = Convert.ToString((prcMchn)["mchnId"]);
                                item.cntrNo = Convert.ToString((prcMchn)["cntrNo"]);
                                item.point = Convert.ToString((prcMchn)["point"]);
                                item.foreAfter = Convert.ToString((prcMchn)["foreAfter"]);
                                Hashtable retLoc = new Hashtable();
                                if ((prcMchn)["loc"] != null && (prcMchn)["loc"] is Hashtable)
                                {
                                    retLoc = (prcMchn)["loc"] as Hashtable;

                                    item.loc.blck = Convert.ToString(retLoc["blck"]);
                                    item.loc.bay = Convert.ToString(retLoc["bay"]);
                                    item.loc.row = Convert.ToString(retLoc["row"]);
                                    item.loc.tier = Convert.ToString(retLoc["tier"]);
                                    item.loc.location = Convert.ToString(retLoc["location"]);
                                    item.wrkSts = Convert.ToString((prcMchn)["wrkSts"]);
                                    item.twinTandemCd = Convert.ToString((prcMchn)["twinTandemCd"]);
                                    item.twinTandemKey = Convert.ToString((prcMchn)["twinTandemKey"]);
                                }

                                jobOrderValue.prcMchnList.Add(item);
                            }
                        }
                    }
                    jobOrderForRMGValue.JobOrder.Add(jobOrderValue);
                }
            }
            return jobOrderForRMGValue;
        }

        static public void GetInventoryListBackground_New(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_BACKGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryList_New(Object obj)
        {
            //VMT_DataMgr_RMG.SaveLog("CAST_DATA_INVENTORY_BAYVIEW");

            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryList_New1(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG1 != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG1(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryList_New2(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG2 != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG2(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListData_New1(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG1 != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG1(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListData_New2(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG2 != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG2(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListData_New(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventoryListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListSwap_New(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_FOREGROUND;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventorySwapListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventorySwapListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public void GetInventoryListDataSwap_New(Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive inventoryReceive = GetInventoryListFromObject_New(obj);
            inventoryReceive.enPurposeType = Objects.RMG.VD_RMG_InventoryInfo_Receive.Purpose_type.TYPE_DATA;

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventorySwapListRMG != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyInventorySwapListRMG(inventoryReceive);

            inventoryReceive.Dispose();
        }

        static public VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive GetInventoryListFromObject_New(Object obj)
        {
            ArrayList retArrayList = new ArrayList();
            if (obj is ArrayList)
                retArrayList = obj as ArrayList;

            try
            {
                var inventoryReceive = new VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive();
                int count = retArrayList.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (retArrayList[i] is Hashtable)
                        {
                            var inventory = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory();

                            Hashtable retHashtable = retArrayList[i] as Hashtable;

                            var cntr = GetContainerInfoFromHashtable(retHashtable);
                            inventory.cntr = cntr;
                            inventory.bColor = Convert.ToString(retHashtable["bColor"]);
                            inventory.fColor = Convert.ToString(retHashtable["fColor"]);
                            inventory.stkDay = Convert.ToString(retHashtable["stkDay"]);
                            inventory.vehicle = Convert.ToString(retHashtable["vehicle"]);
                            inventory.batNo = Convert.ToString(retHashtable["batNo"]);
                            inventory.qrntn = Convert.ToBoolean(retHashtable["qrntn"]);
                            inventory.qrntnRf = Convert.ToBoolean(retHashtable["qrntnRf"]);
                            inventory.jobTp = Convert.ToString(retHashtable["jobTp"]);
                            inventory.pickSetChk = Convert.ToString(retHashtable["pickSetChk"]);
                            inventory.otrWaitTime = Convert.ToString(retHashtable["otrWaitTime"]);
                            inventory.groupCode = Convert.ToString(retHashtable["groupCode"]);
                            inventory.etb = Convert.ToString(retHashtable["etb"]);
                            inventory.isTopOog = Convert.ToBoolean(retHashtable["isTopOog"]);
                            inventory.isLeftOog = Convert.ToBoolean(retHashtable["isLeftOog"]);
                            inventory.isRightOog = Convert.ToBoolean(retHashtable["isRightOog"]);
                            inventory.jobTpKorShort = Convert.ToString(retHashtable["jobTpKorShort"]);

                            // Location
                            VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location loc = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();

                            if (retHashtable.ContainsKey("fmLocTp") && retHashtable["fmLocTp"] is Hashtable)
                                loc.locTp = Convert.ToString((retHashtable["fmLocTp"] as Hashtable)["name"]);
                            loc.blck = Convert.ToString(retHashtable["fmBlck"]);
                            loc.bay = Convert.ToString(retHashtable["fmBay"]);
                            loc.row = Convert.ToString(retHashtable["fmRow"]);
                            loc.tier = Convert.ToString(retHashtable["fmTier"]);
                            if (retHashtable.ContainsKey("fmLocation"))
                                loc.location = Convert.ToString(retHashtable["fmLocation"]);
                            else
                                loc.location = String.Format("{0}-{1}-{2}-{3}", loc.blck, loc.bay, loc.row, loc.tier);

                            inventory.loc = loc;
                            //

                            var blockInfo = new VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BlockInfo();
                            blockInfo.BlcName = inventory.loc.blck;
                            if (!inventoryReceive.DicBlock.ContainsKey(blockInfo.BlcName))
                                inventoryReceive.DicBlock.Add(blockInfo.BlcName, blockInfo);

                            var bayInfo = new VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo();
                            bayInfo.BayName = inventory.loc.bay;
                            if (!inventoryReceive.DicBlock[blockInfo.BlcName].DicBay.ContainsKey(bayInfo.BayName))
                                inventoryReceive.DicBlock[blockInfo.BlcName].DicBay.Add(bayInfo.BayName, bayInfo);

                            inventoryReceive.DicBlock[blockInfo.BlcName].DicBay[bayInfo.BayName].invenList.Add(inventory);
                        }
                    }
                }

                return inventoryReceive;

            }
            catch (Exception e)
            {
                String msg = e.Message;
            }

            return null;
        }

        static public void CheckPLCData(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain();
            //value.mchnId = "dg";
            //value.cntrNo = "dsg";
            //value.twistLockStatus = "";
            //value.errorCode = "";
            //value.containerWeight = "";
            //value.currentBlock = "";
            //value.currentBay = "";
            //value.currentCell = "";
            //value.currentTier = "";
            //value.msgSeq = "";
            //value.wrkCd = "4";
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                value.mchnId = Convert.ToString(result["mchnId"]);
                value.cntrNo = Convert.ToString(result["cntrNo"]);
                value.twistLockStatus = Convert.ToString(result["twistLockStatus"]);
                value.errorCode = Convert.ToString(result["errorCode"]);
                value.containerWeight = Convert.ToString(result["containerWeight"]);
                value.currentBlock = Convert.ToString(result["currentBlock"]);
                value.currentBay = Convert.ToString(result["currentBay"]);
                value.currentCell = Convert.ToString(result["currentCell"]);
                if (value.currentCell.Length == 1)
                    value.currentRow = Convert.ToString(result["currentCell"]);
                else
                    value.currentRow = "1";
                value.currentTier = Convert.ToString(result["currentTier"]);

                value.msgSeq = Convert.ToString(result["msgSeq"]);
                value.wrkCd = Convert.ToString(result["wrkCd"]);
                value.jbFlg = Convert.ToString(result["jbFlg"]);
            }
            else value = null;
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyCheckPLCData != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyCheckPLCData(value);

            value = null;
        }
        static public void CheckPLCTwistLock(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtDomain();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                value.mchnId = Convert.ToString(result["mchnId"]);
                value.cntrNo = Convert.ToString(result["cntrNo"]);
                value.twistLockStatus = Convert.ToString(result["twistLockStatus"]);
                value.currentBlock = Convert.ToString(result["currentBlock"]);
                value.currentBay = Convert.ToString(result["currentBay"]);
                value.currentCell = Convert.ToString(result["currentCell"]);
                value.currentRow = Convert.ToString(result["currentRow"]);
                value.currentTier = Convert.ToString(result["currentTier"]);
                value.msgSeq = Convert.ToString(result["msgSeq"]);
            }

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyCheckPLCTwistLock != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyCheckPLCTwistLock(value);

        }
        static public void InitPLCMessage(Object obj)
        {
            //var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult();
            //if (obj != null && obj is Hashtable)
            //{
            //    var result = obj as Hashtable;
            //    if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
            //    {
            //        var resultTpHash = result["resultTp"] as Hashtable;
            //        if (resultTpHash.ContainsKey("name"))
            //            value.resultTp = Convert.ToString(resultTpHash["name"]);
            //    }
            //    if (result.ContainsKey("resultObj"))
            //    {
            //        if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
            //            value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
            //        else
            //            value.resultObj = Convert.ToString(result["resultObj"]);
            //    }
            //}
            if (VMT_DataMgr_RMG_Callback.static_NotifyInitPLCMessage != null)
                VMT_DataMgr_RMG_Callback.static_NotifyInitPLCMessage();

            //value = null;
        }

        static public void ProcessPLC(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTp = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_DataMgr_RMG_Callback.static_NotifyProcessPLC != null)
                VMT_DataMgr_RMG_Callback.static_NotifyProcessPLC(value);

            value = null;
        }

        static public void CancelPLC(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTp = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_DataMgr_RMG_Callback.static_NotifyCancelPLC != null)
                VMT_DataMgr_RMG_Callback.static_NotifyCancelPLC(value);

            value = null;
        }
        static public void ReleasePLCLock(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTp = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
            }
            if (VMT_DataMgr_RMG_Callback.static_NotifyReleasePLCLock != null)
                VMT_DataMgr_RMG_Callback.static_NotifyReleasePLCLock(value);

            value = null;
        }
        static public void GetEmptySwappingTargetList(ref Object obj)
        {
            VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapOutVO returnList = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapOutVO();

            if (obj is Hashtable)
            {
                Hashtable retHashtable = obj as Hashtable;
                if (retHashtable["reservedList"] != null)
                {
                    var reservedList = new List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO>();
                    foreach (Hashtable reservedItem in (ArrayList)retHashtable["reservedList"])
                    {
                        if (reservedItem.Count > 0)
                        {
                            Objects.RMG.VD_RMG_VmtEmptySwapVO item = new Objects.RMG.VD_RMG_VmtEmptySwapVO();
                            item.cntrNo = Convert.ToString((reservedItem)["cntrNo"]);
                            item.blck = Convert.ToString((reservedItem)["blck"]);
                            item.bay = Convert.ToString((reservedItem)["bay"]);
                            item.row = Convert.ToString((reservedItem)["row"]);
                            item.tier = Convert.ToString((reservedItem)["tier"]);

                            reservedList.Add(item);
                        }
                    }
                    returnList.reservedList = reservedList;
                }
                if (retHashtable["swappingList"] != null)
                {
                    var swappingList = new List<VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtEmptySwapVO>();
                    foreach (Hashtable swappingItem in (ArrayList)retHashtable["swappingList"])
                    {
                        if (swappingItem.Count > 0)
                        {
                            Objects.RMG.VD_RMG_VmtEmptySwapVO item = new Objects.RMG.VD_RMG_VmtEmptySwapVO();
                            item.cntrNo = Convert.ToString((swappingItem)["cntrNo"]);
                            item.blck = Convert.ToString((swappingItem)["blck"]);
                            item.bay = Convert.ToString((swappingItem)["bay"]);
                            item.row = Convert.ToString((swappingItem)["row"]);
                            item.tier = Convert.ToString((swappingItem)["tier"]);

                            swappingList.Add(item);
                        }
                    }
                    returnList.swappingList = swappingList;
                }
            }

            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyEmptySwappingTargetList != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyEmptySwappingTargetList(returnList);
        }
        static public void DoEmptySwap(ref Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.RMG.VD_RMG_VmtResult();

            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if (result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
                {
                    var resultTpHash = result["resultTp"] as Hashtable;
                    if (resultTpHash.ContainsKey("name"))
                        value.resultTp = Convert.ToString(resultTpHash["name"]);
                }
                if (result.ContainsKey("resultObj"))
                {
                    if (result["resultObj"] is Hashtable && (result["resultObj"] as Hashtable).ContainsKey("name"))
                        value.resultObj = Convert.ToString((result["resultObj"] as Hashtable)["name"]);
                    else
                        value.resultObj = Convert.ToString(result["resultObj"]);
                }
                if (result.ContainsKey("resultMsg"))
                {
                    if (result["resultMsg"] is Hashtable && (result["resultMsg"] as Hashtable).ContainsKey("name"))
                        value.resultMsg = Convert.ToString((result["resultMsg"] as Hashtable)["name"]);
                    else
                        value.resultMsg = Convert.ToString(result["resultMsg"]);
                }
            }
            if (VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyDoEmptySwap != null)
                VMT_Data_JAT2.VMT_DataMgr_RMG_Callback.static_NotifyDoEmptySwap(value);

            value = null;
        }
        //static private VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive GetContainerInfoFromObj_New(Object obj)
        //{
        //    if (obj is Hashtable)
        //    {
        //        VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive containerInfoRMGValue = new VMT_Data_JAT2.Objects.RMG.VD_RMG_ContainerInfo_Receive();

        //        Hashtable retHashtable = obj as Hashtable;

        //        containerInfoRMGValue.cntr = GetContainerInfoFromHashtable(retHashtable);

        //        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location fmLoc = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
        //        if (retHashtable.ContainsKey("fmLocTp") && retHashtable["fmLocTp"] is Hashtable)
        //            fmLoc.locTp = Convert.ToString((retHashtable["fmLocTp"] as Hashtable)["name"]);
        //        fmLoc.blck = Convert.ToString(retHashtable["fmBlck"]);
        //        fmLoc.bay = Convert.ToString(retHashtable["fmBay"]);
        //        fmLoc.row = Convert.ToString(retHashtable["fmRow"]);
        //        fmLoc.tier = Convert.ToString(retHashtable["fmTier"]);
        //        if (retHashtable.ContainsKey("fmLocation"))
        //            fmLoc.location = Convert.ToString(retHashtable["fmLocation"]);
        //        else
        //            fmLoc.location = String.Format("{0}-{1}-{2}-{3}", fmLoc.blck, fmLoc.bay, fmLoc.row, fmLoc.tier);

        //        containerInfoRMGValue.fmLoc = fmLoc;

        //        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location toLoc = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Location();
        //        if (retHashtable.ContainsKey("toLocTp") && retHashtable["toLocTp"] is Hashtable)
        //            toLoc.locTp = Convert.ToString((retHashtable["toLocTp"] as Hashtable)["name"]);
        //        toLoc.blck = Convert.ToString(retHashtable["toBlck"]);
        //        toLoc.bay = Convert.ToString(retHashtable["toBay"]);
        //        toLoc.row = Convert.ToString(retHashtable["toRow"]);
        //        toLoc.tier = Convert.ToString(retHashtable["toTier"]);
        //        if (retHashtable.ContainsKey("toLocation"))
        //            toLoc.location = Convert.ToString(retHashtable["toLocation"]);
        //        else
        //            toLoc.location = String.Format("{0}-{1}-{2}-{3}", toLoc.blck, toLoc.bay, toLoc.row, toLoc.tier);

        //        containerInfoRMGValue.toLoc = toLoc;

        //        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel inVsl = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel();
        //        inVsl.vessel = Convert.ToString(retHashtable["inVessel"]);
        //        inVsl.voyage = Convert.ToString(retHashtable["inVoyage"]);
        //        inVsl.vslLoc.bay = Convert.ToString(retHashtable["inVslBay"]);
        //        inVsl.vslLoc.row = Convert.ToString(retHashtable["inVslRow"]);
        //        inVsl.vslLoc.tier = Convert.ToString(retHashtable["inVslTier"]);                
        //        inVsl.vslLoc.location = String.Format("{0}-{1}-{2}",
        //                inVsl.vslLoc.bay, inVsl.vslLoc.row, inVsl.vslLoc.tier);

        //        containerInfoRMGValue.inVsl = inVsl;                

        //        VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel outVsl = new VMT_Data_JAT2.Objects.Common.VD_Common_Def_Vessel();
        //        outVsl.vessel = Convert.ToString(retHashtable["outVessel"]);
        //        outVsl.voyage = Convert.ToString(retHashtable["outVoyage"]);
        //        outVsl.vslLoc.bay = Convert.ToString(retHashtable["outVslBay"]);
        //        outVsl.vslLoc.row = Convert.ToString(retHashtable["outVslRow"]);
        //        outVsl.vslLoc.tier = Convert.ToString(retHashtable["outVslTier"]);
        //        outVsl.vslLoc.location = String.Format("{0}-{1}-{2}",
        //                outVsl.vslLoc.bay, outVsl.vslLoc.row, outVsl.vslLoc.tier);

        //        containerInfoRMGValue.outVsl = outVsl;

        //        return containerInfoRMGValue;
        //    }
        //    else
        //        return null;
        //}
    }
}
