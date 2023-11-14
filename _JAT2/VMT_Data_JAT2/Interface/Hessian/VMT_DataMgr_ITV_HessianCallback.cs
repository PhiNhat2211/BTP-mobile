using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HessianComm;
using System.Net;
using System.Collections;
using Common.Util;
using System.IO;

namespace VMT_Data_JAT2
{
    public class VMT_DataMgr_ITV_HessianCallback
    {
        static public void SetMachinePassed(ref Object obj) // Not Use, Use SetManualActivation
        {
            //VMT_Data_JAT2.Objects.ITV.VD_ITV_SetManualArrival_Receive manualArrivalRp = new VMT_Data_JAT2.Objects.ITV.VD_ITV_SetManualArrival_Receive();
            //manualArrivalRp.WorkingMchnID = VMT_Data_JAT2.Objects.UserInfo.gMchnID;
            //manualArrivalRp.PartnerMchnID = "TC";
            //manualArrivalRp.m_bPOWIN = true;

            //if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival != null)
            //    VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival(ref manualArrivalRp);
        }

        static public void SetManualActivation(ref Object obj)
        {
            //Boolean retBoolean = false;
            //if (obj is Boolean)
            //    retBoolean = Convert.ToBoolean(obj);

            //VMT_Data_JAT2.Objects.ITV.VD_ITV_SetManualArrival_Receive manualArrivalRp = new VMT_Data_JAT2.Objects.ITV.VD_ITV_SetManualArrival_Receive();
            //manualArrivalRp.WorkingMchnID = VMT_Data_JAT2.Objects.UserInfo.gMchnID;
            //manualArrivalRp.PartnerMchnID = "TC";
            //manualArrivalRp.m_bPOWIN = retBoolean;

            //if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival != null)
            //    VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival(ref manualArrivalRp);
        }

        static public void SetMachineArrival(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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

            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineArrival(value);

            value = null;    
        }

        static public void SetMachineReady(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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

            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineReady != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetMachineReady(value);

            value = null;        
        }

        static public void SetItvDone(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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

            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetItvDone != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetItvDone(value);

            value = null;
        }

        //Set Job Done For QC
        static public void SetQCJobReleaseByYt(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                if(result.ContainsKey("resultTp") && result["resultTp"] is Hashtable)
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
                    {
                        value.resultObj = Convert.ToString(result["resultObj"]);
                    }
                }
            }

            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetQCJobReleaseByYt != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifySetQCJobReleaseByYt(value);

            value = null;
        }

        static public void ChangeChassisNo(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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

            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyChangeChassisNo != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyChangeChassisNo(value);
        }
        static public void ChangeDriver(Object obj)
        {
            var retBoolean = new Boolean();
            if (obj is Boolean)
            {
                retBoolean = Convert.ToBoolean(obj);             
            }
            if (VMT_DataMgr_ITV_Callback.static_ITV_NotifyChangeDriver != null)
                VMT_DataMgr_ITV_Callback.static_ITV_NotifyChangeDriver(retBoolean);
        }
        static public void GetChssUsingData(Object obj)
        {
            String result = Convert.ToString(obj);
            
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyGetChssUsingData != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_ITV_NotifyGetChssUsingData(result);
        }

        static public void GetPrecedingYtList(ref Object obj)
        {
            if (obj is ArrayList)
            {
                ArrayList ytArrayList = obj as ArrayList;

                VMT_Data_JAT2.Objects.ITV.VD_ITV_STS_LDPlan STS_LDPlan = new Objects.ITV.VD_ITV_STS_LDPlan();
                STS_LDPlan.StsID = "";

                //const int MaxDisplayCount = 5;

                //if (ytArrayList.Count > MaxDisplayCount)
                //    STS_LDPlan.nCount = MaxDisplayCount; // Max Display 5
                //else
                STS_LDPlan.nCount = ytArrayList.Count;

                List<VMT_Data_JAT2.Objects.ITV.VD_ITV_PlanSeq> SVMT_PlanSeqList = new List<Objects.ITV.VD_ITV_PlanSeq>();
                for (int i = 0; i < ytArrayList.Count; i++)
                {
                    if (ytArrayList[i] is String)
                    {
                        String strYt = ytArrayList[i] as String;
                        VMT_Data_JAT2.Objects.ITV.VD_ITV_PlanSeq planSeq = new VMT_Data_JAT2.Objects.ITV.VD_ITV_PlanSeq();
                        planSeq.MchnID = strYt;
                        planSeq.MchnType = "YT";
                        planSeq.planSeq = i;

                        SVMT_PlanSeqList.Add(planSeq);
                    }
                }

                STS_LDPlan.MchnPlan = SVMT_PlanSeqList;

                if (VMT_DataMgr_ITV_Callback.static_NotifySTSLDSeq != null)
                    VMT_DataMgr_ITV_Callback.static_NotifySTSLDSeq(ref STS_LDPlan);
            }
        }

        static public void SetConfirmJobByScanner(ref Object obj)
        {
            if (obj is Boolean)
            {
                Boolean retBoolean = Convert.ToBoolean(obj);
                if (VMT_DataMgr_ITV_Callback.static_NotifyConfirmJobByScanner != null)
                    VMT_DataMgr_ITV_Callback.static_NotifyConfirmJobByScanner(retBoolean);
            }
        }

        static bool NeedPositionChange(ref VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList jobOrderForITVValue)
        {
            bool retValue = false;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder firstJob = new Objects.Common.VD_Common_JobOrder();
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder secondJob = new Objects.Common.VD_Common_JobOrder();

            if (jobOrderForITVValue.Count > 0)
                firstJob = jobOrderForITVValue.JobOrder[0];
            if (jobOrderForITVValue.Count > 1)
                secondJob = jobOrderForITVValue.JobOrder[1];

            if (firstJob.workingMchn.mchnSts.Equals("U") &&
                secondJob.workingMchn.mchnSts.Equals("U"))
            {
                if (firstJob.locWorking.blck == "" ||
                    firstJob.locWorking.bay == "")
                {
                    retValue = false;
                }
                else if (firstJob.locWorking.blck.Equals(secondJob.locWorking.blck) &&
                    firstJob.locWorking.bay.Equals(secondJob.locWorking.bay))
                {
                    // Fore 선 -> After 후
                    if (firstJob.type.jobFlagInfo.Equals("F") &&
                        secondJob.type.jobFlagInfo.Equals("A"))
                    {
                        retValue = false;
                    }
                    else if (firstJob.type.jobFlagInfo.Equals("A") &&
                            secondJob.type.jobFlagInfo.Equals("F"))
                    {
                        //Logger.Log("HaveToSwap : NeedJobChange");
                        retValue = true;
                    }
                }
                else
                {
                    // After 선 -> Fore 후
                    if (firstJob.type.jobFlagInfo.Equals("F") &&
                        secondJob.type.jobFlagInfo.Equals("A"))
                    {
                        //Logger.Log("HaveToSwap : NeedJobChange");
                        retValue = true;
                    }
                    else if (firstJob.type.jobFlagInfo.Equals("A") &&
                            secondJob.type.jobFlagInfo.Equals("F"))
                    {
                        retValue = false;
                    }
                }
            }
            else if (firstJob.workingMchn.mchnSts.Equals("L") &&
                    secondJob.workingMchn.mchnSts.Equals("L"))
            {
                // Fore 선 -> After 후
                if (firstJob.type.jobFlagInfo.Equals("F") &&
                    secondJob.type.jobFlagInfo.Equals("A"))
                {
                    retValue = false;
                }
                else if (firstJob.type.jobFlagInfo.Equals("A") &&
                        secondJob.type.jobFlagInfo.Equals("F"))
                {
                    //Logger.Log("HaveToSwap : NeedJobChange");
                    retValue = true;
                }
            }

            else if (firstJob.workingMchn.mchnSts != secondJob.workingMchn.mchnSts)
            {
                // U 선 -> L 후
                if (firstJob.workingMchn.mchnSts.Equals("U") &&
                    secondJob.workingMchn.mchnSts.Equals("L"))
                {
                    //Logger.Log("HaveToSwap : No Swap");
                    retValue = false;
                }
                else if (firstJob.workingMchn.mchnSts.Equals("L") &&
                        secondJob.workingMchn.mchnSts.Equals("U"))
                {
                    //Logger.Log("HaveToSwap : Swap");
                    retValue = true;
                }
                else
                {
                    //Logger.Log("HaveToSwap : Not Define");
                }
            }
            else
            {
                // Fore 선 -> After 후
                if (firstJob.type.jobFlagInfo.Equals("F") &&
                    secondJob.type.jobFlagInfo.Equals("A"))
                {
                    //Logger.Log("HaveToSwap : No Swap");
                    retValue = false;
                }
                else if (firstJob.type.jobFlagInfo.Equals("A") &&
                        secondJob.type.jobFlagInfo.Equals("F"))
                {
                    //Logger.Log("HaveToSwap : Swap");
                    retValue = true;
                }
                else
                {
                    //Logger.Log("HaveToSwap : Not Define");
                }
            }

            return retValue;
        }

        static void PositionChange(ref VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList jobOrderForITVValue)
        {
            var tempJobOrderForITV = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>(jobOrderForITVValue.JobOrder[0]);
            jobOrderForITVValue.JobOrder[0] = jobOrderForITVValue.JobOrder[1];
            jobOrderForITVValue.JobOrder[1] = tempJobOrderForITV;
        }

        static bool isSameJobOrderForITV(ref VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList jobOrderForITVValue)
        {
            bool retValue = false;

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder receiveFirstJob = new Objects.Common.VD_Common_JobOrder();
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder receiveSecondJob = new Objects.Common.VD_Common_JobOrder();

            if (jobOrderForITVValue.Count > 0)
                receiveFirstJob = jobOrderForITVValue.JobOrder[0];
            if (jobOrderForITVValue.Count > 1)
                receiveSecondJob = jobOrderForITVValue.JobOrder[1];

            //return true;
            if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count == 0 &&
                (receiveFirstJob.type.jobStatus == null ||
                receiveFirstJob.type.jobStatus.Equals("")))
            {
                retValue = true;
            }
            else if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count == 1)
            {
                if (receiveSecondJob.type.jobStatus != null &&
                    !receiveSecondJob.type.jobStatus.Equals(""))
                {
                    retValue = false;
                }
                else
                {
                    VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder firstJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];
                    //retValue = receiveFirstJob.Equals(firstJob) && receiveFirstJob.type.jobStatus.Equals(VMT_Data_JAT2.Objects.ITV.ITV_User.gFirstJobStatus);
                    retValue = receiveFirstJob.Equals(firstJob);
                }
            }
            else if (VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList.Count == 2)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder firstJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[0];
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder secondJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)VMT_Data_JAT2.Objects.ITV.ITV_User.gJobOrderList[1];

                //retValue = receiveFirstJob.jobKey.Equals(firstJob.jobKey) &&
                //    receiveFirstJob.type.jobStatus.Equals(VMT_Data_JAT2.Objects.ITV.ITV_User.gFirstJobStatus) &&
                //    receiveSecondJob.jobKey.Equals(secondJob.jobKey) &&
                //    receiveSecondJob.type.jobStatus.Equals(VMT_Data_JAT2.Objects.ITV.ITV_User.gSecondJobStatus);

                retValue = receiveFirstJob.Equals(firstJob) &&
                    receiveSecondJob.Equals(secondJob);
            }

            return retValue;
        }

        static String GetJobStatusByJobOrder(ref VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderValue, Boolean is1stPart)
        {
            //Logger.Log("GetJobStatusByJobOrder jobStatus: " + jobOrderValue.type.jobStatus + " is1stPart : " + is1stPart.ToString());

            String retJobStatus = "I";

            if (jobOrderValue.type.jobStatus.Equals("B"))
                retJobStatus = "B"; // ByPass
            else if (jobOrderValue.type.jobStatus.Equals("Q"))
                retJobStatus = "I"; // Idle
            else if (jobOrderValue.type.jobStatus.Equals("A"))
            {
                retJobStatus = "I"; // Idle // None
            }
            else if (jobOrderValue.type.jobStatus.Equals("P"))
            {
                if (is1stPart == true)
                    retJobStatus = "A"; // Arrival
                else
                    retJobStatus = "R"; // Ready
            }
            else
                retJobStatus = "I"; // Idle

            return retJobStatus;
        }

        static public void GetJobOrderList(ref Object obj)
        {
            //-----------------------------------------------------------------------------------------
            //- Create EEv2JobOrderForITV
            VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList jobOrderForITVValue = new VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList();
            jobOrderForITVValue.JobOrder = new List<Objects.Common.VD_Common_JobOrder>();
            jobOrderForITVValue.Sub = new List<Objects.ITV.VD_ITV_JobOrderSub>();

            //jobOrderForITVValue.firstJobStatus = "";
            //jobOrderForITVValue.secondJobStatus = "";
            //-----------------------------------------------------------------------------------------

            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder firstJob = new Objects.Common.VD_Common_JobOrder();
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder secondJob = new Objects.Common.VD_Common_JobOrder();

            if (jobOrderForITVValue.Count > 0)
                firstJob = jobOrderForITVValue.JobOrder[0];
            if (jobOrderForITVValue.Count > 1)
                secondJob = jobOrderForITVValue.JobOrder[1];

            List<Object> retObjectList = new List<object>();
            if (obj is List<Object>)
                retObjectList = obj as List<Object>;

            //if (retObjectList.Count == 0) // 현재 Job Done으로 판단
            //{
            //    // if (VMT_DataMgr_Common_Callback.static_NotifyJobDone!= null)
            //    //    VMT_DataMgr_Common_Callback.static_NotifyJobDone(ref firstJobOrderValue);
            //}

            //Logger.Log("===========================================================");

            int nJobInsertCount = 0;
            foreach (Object retObject in retObjectList)
            {
                //Logger.Log("===JobInsertCount : " + nJobInsertCount.ToString());
                //Logger.Log("workingMchn mchnTp : " + ((retObject as Hashtable)["workingMchn"] as Hashtable)["mchnTp"]));
                //Logger.Log("partnerMchn mchnTp : " + ((retObject as Hashtable)["partnerMchn"] as Hashtable)["mchnTp"]));
                //Logger.Log("cntrNo : " + ((retObject as Hashtable)["cntr"] as Hashtable)["cntrNo"]));
                //Logger.Log("locTp : " + Convert.ToString((((retObject as Hashtable)["loc"] as Hashtable)["locTp"] as Hashtable)["name"]));
                //Logger.Log("location : " + ((retObject as Hashtable)["loc"] as Hashtable)["location"]));
                //Logger.Log("jobTp : " + ((retObject as Hashtable)["jobTp"])));
                //Logger.Log("jobSts : " + ((retObject as Hashtable)["jobSts"])));
                //Logger.Log("positionOnChassis : " + ((retObject as Hashtable)["positionOnChassis"])));
                //Logger.Log("jobId : " + ((retObject as Hashtable)["jobId"])));

                nJobInsertCount++;
                if (nJobInsertCount > 2)
                    break;

                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;
                    Hashtable retWorkingMchn = null;
                    Hashtable retPartnerMchn = null;
                    Hashtable retCntr = null;
                    Hashtable retLoc = null;

                    if (retHashtable["workingMchn"] is Hashtable)
                        retWorkingMchn = retHashtable["workingMchn"] as Hashtable;

                    if (retHashtable["partnerMchn"] is Hashtable)
                        retPartnerMchn = retHashtable["partnerMchn"] as Hashtable;

                    if (retHashtable["cntr"] is Hashtable)
                        retCntr = retHashtable["cntr"] as Hashtable;

                    if (retHashtable["loc"] is Hashtable)
                        retLoc = retHashtable["loc"] as Hashtable;

                    /*
                     * Second Job Add Part
                     */
                    if (nJobInsertCount == 2)
                    {
                        /*
                         * First Job 20F
                         * Second Job 20F
                         * Second Job twinTandemCd "S" or "W" or "M" // Single, Twin, Tandem
                         */
                        if (!(firstJob.cntr.cntrIso.StartsWith("2") &&
                        Convert.ToString(retCntr["cntrIso"]).StartsWith("2") &&
                        (Convert.ToString(retHashtable["twinTandemCd"]).Equals("S") ||
                         Convert.ToString(retHashtable["twinTandemCd"]).Equals("W") ||
                         Convert.ToString(retHashtable["twinTandemCd"]).Equals("M"))
                            ))
                        {
                            break;
                        }
                        /*
                         * positionOnChassis가 First Job / Second Job ==> F, F / A, A로 오는 경우 무시
                         */
                        else if (firstJob.type.jobFlagInfo.Equals(Convert.ToString(retHashtable["positionOnChassis"])))
                        {
                            break;
                        }
                    }

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

                    jobOrderValue.workingMchn.mchnId = Convert.ToString(retWorkingMchn["mchnId"]);
                    jobOrderValue.workingMchn.mchnTp = Convert.ToString(retWorkingMchn["mchnTp"]);
                    jobOrderValue.workingMchn.mchnSts = Convert.ToString(retWorkingMchn["mchnSts"]);
                    jobOrderValue.workingMchn.vrtlFlg = Convert.ToString(retWorkingMchn["vrtlFlg"]);
                    jobOrderValue.workingMchn.aprchLn = Convert.ToString(retWorkingMchn["aprchLn"]);

                    jobOrderValue.partnerMchn.mchnId = Convert.ToString(retPartnerMchn["mchnId"]);
                    jobOrderValue.partnerMchn.mchnTp = Convert.ToString(retPartnerMchn["mchnTp"]);
                    jobOrderValue.partnerMchn.mchnSts = Convert.ToString(retPartnerMchn["mchnSts"]);
                    jobOrderValue.partnerMchn.vrtlFlg = Convert.ToString(retPartnerMchn["vrtlFlg"]);
                    jobOrderValue.partnerMchn.aprchLn = Convert.ToString(retPartnerMchn["aprchLn"]);

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

                    if (retCntr.ContainsKey("imdgCd"))
                        jobOrderValue.cntr.imdgCd = Convert.ToString(retCntr["imdgCd"]);

                    //jobOrderValue.locTo;
                    //jobOrderValue.locFrom;
                    if (retLoc["locTp"] is Hashtable)
                        jobOrderValue.locWorking.locTp = Convert.ToString((retLoc["locTp"] as Hashtable)["name"]);

                    jobOrderValue.locWorking.location = Convert.ToString(retLoc["location"]);
                    jobOrderValue.locWorking.blck = Convert.ToString(retLoc["blck"]);
                    jobOrderValue.locWorking.bay = Convert.ToString(retLoc["bay"]);
                    jobOrderValue.locWorking.row = Convert.ToString(retLoc["row"]);
                    jobOrderValue.locWorking.tier = Convert.ToString(retLoc["tier"]);

                    jobOrderValue.locTo.location = Convert.ToString(retLoc["location"]);
                    jobOrderValue.locTo.blck = Convert.ToString(retLoc["blck"]);
                    jobOrderValue.locTo.bay = Convert.ToString(retLoc["bay"]);
                    jobOrderValue.locTo.row = Convert.ToString(retLoc["row"]);
                    jobOrderValue.locTo.tier = Convert.ToString(retLoc["tier"]);

                    jobOrderValue.type.jobTp = Convert.ToString(retHashtable["jobTp"]);
                    jobOrderValue.type.jobStatus = Convert.ToString(retHashtable["jobSts"]);
                    jobOrderValue.type.vslCd = Convert.ToString((retHashtable["vsl"] as Hashtable)["vessel"]);
                    jobOrderValue.type.voyNo = Convert.ToString((retHashtable["vsl"] as Hashtable)["voyage"]);
                    jobOrderValue.type.planSeq = Convert.ToString((retHashtable["vsl"] as Hashtable)["planSeq"]);
                    jobOrderValue.type.twinTandemFlg = Convert.ToString(retHashtable["twinTandemCd"]);
                    jobOrderValue.type.twinTandumKey = Convert.ToString(retHashtable["twinTandemKey"]);
                    jobOrderValue.type.ycTwinKey = Convert.ToString(retHashtable["ycTwinKey"]);
                    jobOrderValue.type.tandemJoinYT = Convert.ToString(retHashtable["neighborYt"]);
                    jobOrderValue.type.jobFlagInfo = Convert.ToString(retHashtable["positionOnChassis"]);
                    jobOrderValue.type.etw = Convert.ToString(retHashtable["etw"]);
                    
                    // jobOrderValue.type.conePlan = Convert.ToString(retCntr["conePlan"]);
                    if (Convert.ToString(retCntr["conePlan"]).Length > 0)
                        jobOrderValue.type.conePlan = "Y";
                    else
                        jobOrderValue.type.conePlan = "N";

                    // String strAlignmentFlg = Convert.ToString(retWorkingMchn["alignmentFlg"]);
                    Boolean is1stPart = Convert.ToBoolean(retHashtable["is1stPart"]);

                    if (nJobInsertCount == 1) // firstJob
                        firstJob = jobOrderValue;
                    else // secondJob
                        secondJob = jobOrderValue;

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
                                item.chassNo = Convert.ToString((prcMchn)["chassNo"]);
                                item.isExistFlg = ((prcMchn)["isExistFlg"] != null) ? Convert.ToBoolean((prcMchn)["isExistFlg"]) : false;
                                item.blockLoc = Convert.ToString((prcMchn)["blockLoc"]);
                                Hashtable retLoca = new Hashtable();
                                if ((prcMchn)["loc"] is Hashtable)
                                    retLoca = (prcMchn)["loc"] as Hashtable;
                                item.loc.blck = Convert.ToString(retLoca["blck"]);
                                item.loc.bay = Convert.ToString(retLoca["bay"]);
                                item.loc.row = Convert.ToString(retLoca["row"]);
                                item.loc.tier = Convert.ToString(retLoca["tier"]);
                                item.loc.location = Convert.ToString(retLoca["location"]);
                                item.wrkSts = Convert.ToString((prcMchn)["wrkSts"]);
                                item.twinTandemCd = Convert.ToString((prcMchn)["twinTandemCd"]);
                                item.twinTandemKey = Convert.ToString((prcMchn)["twinTandemKey"]);

                                jobOrderValue.prcMchnList.Add(item);
                            }
                        }
                    }
                    //jobOrderValue.type.jobStatus = GetJobStatusByJobOrder(ref jobOrderValue, is1stPart);
                    jobOrderValue.type.etw = String.IsNullOrEmpty(Convert.ToString(retHashtable["etw"])) ? "3000" : Convert.ToString(retHashtable["etw"]); // 5min is default
                    jobOrderForITVValue.JobOrder.Add(jobOrderValue);
                }
            }

            //Logger.Log("===========================================================");

            if (nJobInsertCount == 2 && // Double, Twin, Tandem
                NeedPositionChange(ref jobOrderForITVValue))
            {
                PositionChange(ref jobOrderForITVValue);
            }

            //-----------------------------------------------------------------------------------------
            //- arrvdMchnAtPow (2015/01/07)
            foreach (Object retObject in retObjectList)
            {
                if (retObject is Hashtable)
                {
                    Hashtable retHashtable = retObject as Hashtable;

                    Hashtable retWorkingMchn = null;
                    if (retHashtable["workingMchn"] is Hashtable)
                        retWorkingMchn = retHashtable["workingMchn"] as Hashtable;

                    String mchnTp = Convert.ToString(retWorkingMchn["mchnTp"]);

                    String arrvdMchnAtPow = "";
                    try
                    {
                        arrvdMchnAtPow = Convert.ToString(retHashtable["arrvdMchnAtPow"]);
                    }
                    catch (Exception ex)
                    {
                        String errorMessage = ex.Message;
                        arrvdMchnAtPow = "";
                    }
                    finally
                    {
                        if (mchnTp.Equals("YT"))
                        {
                            //if (ITV.static_NotifyArrvdMchnAtPow != null)
                            //    ITV.static_NotifyArrvdMchnAtPow(arrvdMchnAtPow);
                        }
                    }
                }
                break; // Only First Job
            }
            //-----------------------------------------------------------------------------------------

            if (isSameJobOrderForITV(ref jobOrderForITVValue))
                return;

            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyJobOrderITV != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyJobOrderITV(ref jobOrderForITVValue);
        }

        static public void GetMachineStatusChanged(Object obj)
        {
            try {
                VMT_Data_JAT2.Objects.Common.VmtMachine machine = new VMT_Data_JAT2.Objects.Common.VmtMachine();

                if (obj is Hashtable)
                {
                    Hashtable retHashtable = obj as Hashtable;

                    machine.mchnId = Convert.ToString(retHashtable["mchnId"]);
                    machine.mchnTp = Convert.ToString(retHashtable["mchnTp"]);
                    machine.isLogOn = Convert.ToBoolean(retHashtable["isLogOn"]);
                    machine.isOn = Convert.ToBoolean(retHashtable["isOn"]);
                    machine.mchnSts = Convert.ToString(retHashtable["mchnSts"]);
                    machine.rfidBlck = Convert.ToString(retHashtable["rfidBlck"]);
                    machine.armgReadFlg = Convert.ToString(retHashtable["armgReadFlg"]);

                    //SaveLog("[GetMachineStatusChanged Callback]", "[rfidBlck]" + Convert.ToString(retHashtable["rfidBlck"]) + " [armgReadFlg]" + Convert.ToString(retHashtable["armgReadFlg"]));

                    machine.vrtlFlg = Convert.ToString(retHashtable["vrtlFlg"]);
                    machine.noticeMsg = Convert.ToString(retHashtable["noticeMsg"]);
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

                if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyGetMachineStatusChanged != null)
                    VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyGetMachineStatusChanged(machine);
            }
            catch (Exception ex)
            {
                SaveLog("[Exception GetMachineStatusChanged Callback]", "[Exception message]: " + ex.Message + "[InnerException]: " + ex.InnerException + Environment.NewLine
                    + "[Source]:" + ex.Source + Environment.NewLine + "[StackTrace]:" + ex.StackTrace);
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

        static public void GetJobOrderList_New(ref Object obj)
        {
            try
            {
                //-----------------------------------------------------------------------------------------
                //- Create EEv2JobOrderForITV
                VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList jobOrderForITVValue = new VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList();

                jobOrderForITVValue.JobOrder = new List<Objects.Common.VD_Common_JobOrder>();
                jobOrderForITVValue.Sub = new List<Objects.ITV.VD_ITV_JobOrderSub>();

                //-----------------------------------------------------------------------------------------

                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder firstJob = new Objects.Common.VD_Common_JobOrder();
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder secondJob = new Objects.Common.VD_Common_JobOrder();

                if (jobOrderForITVValue.Count > 0)
                    firstJob = jobOrderForITVValue.JobOrder[0];
                if (jobOrderForITVValue.Count > 1)
                    secondJob = jobOrderForITVValue.JobOrder[1];

                List<Object> retObjectList = new List<object>();
                if (obj is List<Object>)
                    retObjectList = obj as List<Object>;

                int nJobInsertCount = 0;
                foreach (Object retObject in retObjectList)
                {
                    nJobInsertCount++;
                    if (nJobInsertCount > 2)
                        break;

                    if (retObject is Hashtable)
                    {
                        Hashtable retHashtable = retObject as Hashtable;

                        /*
                         * Second Job Add Part
                         */
                        if (nJobInsertCount == 2)
                        {
                            /*
                             * First Job 20F
                             * Second Job 20F
                             * Second Job twinTandemCd "S" or "W" or "M" // Single, Twin, Tandem
                             */
                            if (
                                !(firstJob.cntr.cntrIso.StartsWith("2") &&
                            Convert.ToString(retHashtable["cntrIso"]).StartsWith("2") &&
                            (Convert.ToString(retHashtable["twinTandemCd"]).Equals("S") ||
                             Convert.ToString(retHashtable["twinTandemCd"]).Equals("W") ||
                             Convert.ToString(retHashtable["twinTandemCd"]).Equals("M"))
                                ))
                            {
                                if (!firstJob.type.jobFlagInfo.Equals("F") && !firstJob.type.jobFlagInfo.Equals("A"))
                                    break;
                            }
                            /*
                             * positionOnChassis가 First Job / Second Job ==> F, F / A, A로 오는 경우 무시
                             */
                            if (firstJob.type.jobFlagInfo.Equals(Convert.ToString(retHashtable["positionOnChassis"])))
                            {
                                break;
                            }
                        }

                        VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrderValue = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                        jobOrderValue.workingMchn = new Objects.Common.VD_Common_Job_Machine();
                        jobOrderValue.partnerMchn = new Objects.Common.VD_Common_Job_Machine();
                        jobOrderValue.cntr = new Objects.Common.VD_Common_Job_Container();
                        jobOrderValue.locWorking = new Objects.Common.VD_Common_Job_Location();
                        jobOrderValue.type = new Objects.Common.VD_Common_Job_Type();

                        jobOrderValue.chassisOrder = new Objects.Common.VD_Common_ChassisOrder();
                        if (retHashtable["chassisOrder"] != null)
                        {
                            ArrayList chass = retHashtable["chassisOrder"] as ArrayList;
                            if (chass.Count > 0)
                            {
                                Objects.Common.VD_Common_ChassisOrder item = new Objects.Common.VD_Common_ChassisOrder();
                                item.ordSeq = Convert.ToString(((Hashtable)chass[0])["ordSeq"]);
                                item.chssNo = Convert.ToString(((Hashtable)chass[0])["chssNo"]);
                                item.crntPsnIdxNo1 = Convert.ToString(((Hashtable)chass[0])["crntPsnIdxNo1"]);
                                item.crntPsnIdxNo2 = Convert.ToString(((Hashtable)chass[0])["crntPsnIdxNo2"]);
                                item.crntPsnIdxNo3 = Convert.ToString(((Hashtable)chass[0])["crntPsnIdxNo3"]);
                                item.crntPsnIdxNo4 = Convert.ToString(((Hashtable)chass[0])["crntPsnIdxNo4"]);
                                item.plnPsnIdxNo1 = Convert.ToString(((Hashtable)chass[0])["plnPsnIdxNo1"]);
                                item.plnPsnIdxNo2 = Convert.ToString(((Hashtable)chass[0])["plnPsnIdxNo2"]);
                                item.plnPsnIdxNo3 = Convert.ToString(((Hashtable)chass[0])["plnPsnIdxNo3"]);
                                item.plnPsnIdxNo4 = Convert.ToString(((Hashtable)chass[0])["plnPsnIdxNo4"]);
                                item.itvCd = Convert.ToString(((Hashtable)chass[0])["itvCd"]);
                                item.ordStsCd = Convert.ToString(((Hashtable)chass[0])["ordStsCd"]);
                                item.cntrNo = Convert.ToString(((Hashtable)chass[0])["cntrNo"]);

                                jobOrderValue.chassisOrder = item;
                            }
                        }

                        jobOrderValue.isOcpyAndAlct = Convert.ToBoolean(retHashtable["isOcpyAndAlct"]);
                        jobOrderValue.isCmtJob = Convert.ToBoolean(retHashtable["isCmtJob"]);
                        jobOrderValue.prvCntrNo = Convert.ToString(retHashtable["prvCntrNo"]);
                        jobOrderValue.prvLocation = Convert.ToString(retHashtable["prvLocation"]);

                        jobOrderValue.qlnLocation = Convert.ToString(retHashtable["qlnLocation"]);
                        jobOrderValue.qlnBlck = Convert.ToString(retHashtable["qlnBlck"]);
                        jobOrderValue.qlnBay = Convert.ToString(retHashtable["qlnBay"]);
                        jobOrderValue.qlnRow = Convert.ToString(retHashtable["qlnRow"]);
                        jobOrderValue.qlnTier = Convert.ToString(retHashtable["qlnTier"]);
                        jobOrderValue.tpLocation = Convert.ToString(retHashtable["tpLocation"]);
                        jobOrderValue.tpBlck = Convert.ToString(retHashtable["tpBlck"]);
                        jobOrderValue.tpBay = Convert.ToString(retHashtable["tpBay"]);
                        jobOrderValue.tpRow = Convert.ToString(retHashtable["tpRow"]);
                        jobOrderValue.tpTier = Convert.ToString(retHashtable["tpTier"]);
                        jobOrderValue.workingStatus = Convert.ToString(retHashtable["workingStatus"]);
                        jobOrderValue.korJobAct = Convert.ToString(retHashtable["korJobAct"]);
                        jobOrderValue.commCode = Convert.ToString(retHashtable["commCode"]);
                        jobOrderValue.regBr = Convert.ToString(retHashtable["regBr"]);
                        jobOrderValue.chassisNo = Convert.ToString(retHashtable["chassisNo"]);
                        jobOrderValue.ytAprchLn = Convert.ToString(retHashtable["ytAprchLn"]);
                        jobOrderValue.rfidNo = Convert.ToString(retHashtable["rfidNo"]);
                        jobOrderValue.rfidChk = Convert.ToString(retHashtable["rfidChk"]);
                        jobOrderValue.vslHoldDeck = Convert.ToString(retHashtable["vslHoldDeck"]);
                        jobOrderValue.doorDir = Convert.ToString(retHashtable["doorDir"]);
                        jobOrderValue.podCd = Convert.ToString(retHashtable["podCd"]);
                        jobOrderValue.ytJbSts = Convert.ToString(retHashtable["ytJbSts"]);

                        jobOrderValue.locFrom.blck = Convert.ToString(retHashtable["fmBlck"]);
                        jobOrderValue.locFrom.bay = Convert.ToString(retHashtable["fmBay"]);
                        jobOrderValue.locFrom.row = Convert.ToString(retHashtable["fmRow"]);
                        jobOrderValue.locFrom.tier = Convert.ToString(retHashtable["fmTier"]);

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
                                    item.chassNo = Convert.ToString((prcMchn)["chassNo"]);
                                    item.isExistFlg = ((prcMchn)["isExistFlg"] != null) ? Convert.ToBoolean((prcMchn)["isExistFlg"]) : false;
                                    item.blockLoc = Convert.ToString((prcMchn)["blockLoc"]);
                                    Hashtable retLoc = new Hashtable();
                                    if ((prcMchn)["loc"] is Hashtable)
                                        retLoc = (prcMchn)["loc"] as Hashtable;
                                    item.loc.blck = Convert.ToString(retLoc["blck"]);
                                    item.loc.bay = Convert.ToString(retLoc["bay"]);
                                    item.loc.row = Convert.ToString(retLoc["row"]);
                                    item.loc.tier = Convert.ToString(retLoc["tier"]);
                                    item.loc.location = Convert.ToString(retLoc["location"]);
                                    item.wrkSts = Convert.ToString((prcMchn)["wrkSts"]);
                                    item.twinTandemCd = Convert.ToString((prcMchn)["twinTandemCd"]);
                                    item.twinTandemKey = Convert.ToString((prcMchn)["twinTandemKey"]);

                                    jobOrderValue.prcMchnList.Add(item);
                                }
                            }
                        }
                        jobOrderValue.isWstp = Convert.ToBoolean(retHashtable["isWstp"]);
                        jobOrderValue.isLink = Convert.ToBoolean(retHashtable["isLink"]);
                        jobOrderValue.isarrival = Convert.ToBoolean(retHashtable["isarrival"]);
                        jobOrderValue.isPick = Convert.ToBoolean(retHashtable["isPick"]);
                        jobOrderValue.isSetDown = Convert.ToBoolean(retHashtable["isSetDown"]);
                        jobOrderValue.isSwap = Convert.ToBoolean(retHashtable["isSwap"]);

                        jobOrderValue.jobTpKor = Convert.ToString(retHashtable["jobTpKor"]);
                        jobOrderValue.jobTpKorShort = Convert.ToString(retHashtable["jobTpKorShort"]);
                        jobOrderValue.foreAfterKor = Convert.ToString(retHashtable["foreAfterKor"]);
                        jobOrderValue.doorKor = Convert.ToString(retHashtable["doorKor"]);

                        jobOrderValue.prvItvList = new List<Objects.Common.VD_Common_ChassisOrder>();
                        if (retHashtable["prvItvList"] != null)
                        {
                            foreach (Hashtable chassisOrder in (ArrayList)retHashtable["prvItvList"])
                            {
                                if (chassisOrder.Count > 0)
                                {
                                    Objects.Common.VD_Common_ChassisOrder item = new Objects.Common.VD_Common_ChassisOrder();
                                    item.ordSeq = Convert.ToString((chassisOrder)["ordSeq"]);
                                    item.chssNo = Convert.ToString((chassisOrder)["chssNo"]);
                                    item.crntPsnIdxNo1 = Convert.ToString((chassisOrder)["crntPsnIdxNo1"]);
                                    item.crntPsnIdxNo2 = Convert.ToString((chassisOrder)["crntPsnIdxNo2"]);
                                    item.crntPsnIdxNo3 = Convert.ToString((chassisOrder)["crntPsnIdxNo3"]);
                                    item.crntPsnIdxNo4 = Convert.ToString((chassisOrder)["crntPsnIdxNo4"]);
                                    item.plnPsnIdxNo1 = Convert.ToString((chassisOrder)["plnPsnIdxNo1"]);
                                    item.plnPsnIdxNo2 = Convert.ToString((chassisOrder)["plnPsnIdxNo2"]);
                                    item.plnPsnIdxNo3 = Convert.ToString((chassisOrder)["plnPsnIdxNo3"]);
                                    item.plnPsnIdxNo4 = Convert.ToString((chassisOrder)["plnPsnIdxNo4"]);
                                    item.itvCd = Convert.ToString((chassisOrder)["itvCd"]);
                                    item.ordStsCd = Convert.ToString((chassisOrder)["ordStsCd"]);
                                    item.cntrNo = Convert.ToString((chassisOrder)["cntrNo"]);

                                    jobOrderValue.prvItvList.Add(item);
                                }
                            }
                        }

                        if (retHashtable.ContainsKey("jobId") && retHashtable["jobId"] != null)
                            jobOrderValue.jobKey = Convert.ToString(retHashtable["jobId"]);
                        else
                            jobOrderValue.jobKey = String.Empty;

                        jobOrderValue.taskId = Convert.ToString(retHashtable["taskId"]);
                        jobOrderValue.jobCount = retObjectList.Count.ToString();
                        jobOrderValue.priorityJob = Convert.ToString(retHashtable["priorityJob"]);     // hot job      
                        jobOrderValue.pinChkFlg = Convert.ToString(retHashtable["pinChkFlg"]);
                        jobOrderValue.spndFlg = Convert.ToString(retHashtable["spndFlg"]);
                        jobOrderValue.type.qcId = Convert.ToString(retHashtable["qcNo"]);
                        jobOrderValue.qcLn = Convert.ToString(retHashtable["qcLn"]);
                        jobOrderValue.hold = Convert.ToString(retHashtable["hold"]);

                        jobOrderValue.workingMchn.mchnId = Convert.ToString(retHashtable["ytNo"]);
                        jobOrderValue.workingMchn.mchnTp = "YT";//Convert.ToString(retHashtable["mchnTp"]);
                        jobOrderValue.workingMchn.ycTp = Convert.ToString(retHashtable["ycTp"]);    // RMG / ECH
                        jobOrderValue.workingMchn.mchnSts = Convert.ToString(retHashtable["ytSts"]);
                        //jobOrderValue.workingMchn.vrtlFlg = Convert.ToString(retHashtable["vrtlFlg"]);
                        jobOrderValue.workingMchn.aprchLn = Convert.ToString(retHashtable["ytAprchLn"]);

                        var qcNo = Convert.ToString(retHashtable["qcNo"]);
                        var ycNo = Convert.ToString(retHashtable["ycNo"]);
                        jobOrderValue.partnerMchn.mchnId = String.IsNullOrEmpty(qcNo) ? ycNo : qcNo;
                        jobOrderValue.ycNo = Convert.ToString(retHashtable["ycNo"]);
                        jobOrderValue.autoType = Convert.ToString(retHashtable["autoType"]);
                        jobOrderValue.is1stPart = String.IsNullOrEmpty(Convert.ToString(retHashtable["is1stPart"])) ? false : Convert.ToBoolean(retHashtable["is1stPart"]);
                        jobOrderValue.partnerMchn.mchnTp = String.IsNullOrEmpty(qcNo) ? Convert.ToString(retHashtable["ycTp"]) : "QC";
                        jobOrderValue.partnerMchn.mchnSts = Convert.ToString(retHashtable["ycSts"]);
                        //jobOrderValue.partnerMchn.vrtlFlg = Convert.ToString(retHashtable["vrtlFlg"]);
                        //jobOrderValue.partnerMchn.aprchLn = Convert.ToString(retHashtable["aprchLn"]);

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

                        if (retHashtable.ContainsKey("imdgCd"))
                            jobOrderValue.cntr.imdgCd = Convert.ToString(retHashtable["imdgCd"]);

                        //jobOrderValue.locFrom; 
                        jobOrderValue.locFrom.location = Convert.ToString(retHashtable["fmLocation"]);

                        //jobOrderValue.locTo;                                    
                        if (retHashtable.ContainsKey("toLocTp") && retHashtable["toLocTp"] is Hashtable)
                            jobOrderValue.locWorking.locTp = Convert.ToString((retHashtable["toLocTp"] as Hashtable)["name"]);
                        jobOrderValue.locWorking.location = Convert.ToString(retHashtable["toLocation"]);
                        jobOrderValue.locWorking.blck = Convert.ToString(retHashtable["toBlck"]);
                        jobOrderValue.locWorking.bay = Convert.ToString(retHashtable["toBay"]);
                        jobOrderValue.locWorking.row = Convert.ToString(retHashtable["toRow"]);
                        jobOrderValue.locWorking.tier = Convert.ToString(retHashtable["toTier"]);

                        jobOrderValue.locTo.location = Convert.ToString(retHashtable["toLocation"]);
                        jobOrderValue.locTo.blck = Convert.ToString(retHashtable["toBlck"]);
                        jobOrderValue.locTo.bay = Convert.ToString(retHashtable["toBay"]);
                        jobOrderValue.locTo.row = Convert.ToString(retHashtable["toRow"]);
                        jobOrderValue.locTo.tier = Convert.ToString(retHashtable["toTier"]);
                        //                    

                        jobOrderValue.type.jobTp = Convert.ToString(retHashtable["jobTp"]);
                        jobOrderValue.type.jobStatus = Convert.ToString(retHashtable["jobSts"]);
                        jobOrderValue.type.vslCd = Convert.ToString(retHashtable["vessel"]);
                        jobOrderValue.type.voyNo = Convert.ToString(retHashtable["voyage"]);
                        jobOrderValue.type.planSeq = retHashtable["planSeq"] == null ? Convert.ToString(retHashtable["taskId"]) : Convert.ToString(retHashtable["planSeq"]);
                        jobOrderValue.type.twinTandemFlg = Convert.ToString(retHashtable["twinTandemCd"]);
                        jobOrderValue.type.twinTandumKey = Convert.ToString(retHashtable["twinTandemKey"]);
                        jobOrderValue.type.ycTwinKey = Convert.ToString(retHashtable["ycTwinKey"]);
                        jobOrderValue.type.tandemJoinYT = Convert.ToString(retHashtable["neighborYt"]);
                        jobOrderValue.type.jobFlagInfo = Convert.ToString(retHashtable["positionOnChassis"]);
                        jobOrderValue.type.etw = Convert.ToString(retHashtable["etw"]);

                        // jobOrderValue.type.conePlan = Convert.ToString(retCntr["conePlan"]);
                        if (Convert.ToString(retHashtable["conePlan"]).Length > 0)
                            jobOrderValue.type.conePlan = "Y";
                        else
                            jobOrderValue.type.conePlan = "N";

                        // String strAlignmentFlg = Convert.ToString(retWorkingMchn["alignmentFlg"]);
                        Boolean is1stPart = String.IsNullOrEmpty(Convert.ToString(retHashtable["is1stPart"])) ? true : Convert.ToBoolean(retHashtable["is1stPart"]);

                        if (nJobInsertCount == 1) // firstJob
                            firstJob = jobOrderValue;
                        else // secondJob
                            secondJob = jobOrderValue;

                        //jobOrderValue.type.jobStatus = GetJobStatusByJobOrder(ref jobOrderValue, is1stPart);
                        jobOrderValue.type.etw = String.IsNullOrEmpty(Convert.ToString(retHashtable["etw"])) ? "3000" : Convert.ToString(retHashtable["etw"]); // 5min is default
                        jobOrderForITVValue.JobOrder.Add(jobOrderValue);

                        //SaveLog("GetMachinesJobByKeys Callback",
                        //    "WorkingMchn"
                        //    + "[JobStatus]" + jobOrderValue.type.jobStatus
                        //    + " [mchnId]" + jobOrderValue.workingMchn.mchnId + " [mchnTp]" + jobOrderValue.workingMchn.mchnTp
                        //    + " [mchnSts]" + jobOrderValue.workingMchn.mchnSts
                        //    + "\nPartnerMchn"
                        //    + " [mchnId]" + jobOrderValue.partnerMchn.mchnId + " [mchnTp]" + jobOrderValue.partnerMchn.mchnTp
                        //    + " [mchnSts]" + jobOrderValue.partnerMchn.mchnSts
                        //    + "\nCntr"
                        //    + " [cntrNo]" + jobOrderValue.cntr.cntrNo
                        //    + "\nType"
                        //    + " [jobTp]" + jobOrderValue.type.jobTp + " [jobStatus]" + jobOrderValue.type.jobStatus
                        //    + " [ytJbSts]" + jobOrderValue.ytJbSts
                        //);

                    }
                }

                //Logger.Log("===========================================================");

                if (nJobInsertCount == 2 && // Double, Twin, Tandem
                NeedPositionChange(ref jobOrderForITVValue))
                {
                    PositionChange(ref jobOrderForITVValue);
                }

                //-----------------------------------------------------------------------------------------

                if (jobOrderForITVValue.Count > 0 && isSameJobOrderForITV(ref jobOrderForITVValue))
                    return;

                

                if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyJobOrderITV != null)
                    VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyJobOrderITV(ref jobOrderForITVValue);
            }
            catch (Exception ex)
            {
                SaveLog("[Exception GetMachineJobByKeys Callback]", "[Exception message]: " + ex.Message + "[InnerException]: " + ex.InnerException + Environment.NewLine
                    + "[Source]:" + ex.Source + Environment.NewLine + "[StackTrace]:" + ex.StackTrace);
            }   
        }
        
        static public void SetReallocation(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyItvLinkChassis != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyItvLinkChassis(value);

            value = null;
        }

        static public void ChassisOrderComplete(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyChassisOrderComplete != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyChassisOrderComplete(value);

            value = null;
        }

        static public void ValidChassisInfos(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_Common_ChassisInventory();
            if (obj != null && obj is Hashtable)
            {
                var result = obj as Hashtable;
                value.ordSeq = Convert.ToString(result["ordSeq"]);
                value.chssNo = Convert.ToString(result["chssNo"]);
                value.crntPsnIdxNo1 = Convert.ToString(result["crntPsnIdxNo1"]);
                value.crntPsnIdxNo2 = Convert.ToString(result["crntPsnIdxNo2"]);
                value.crntPsnIdxNo3 = Convert.ToString(result["crntPsnIdxNo3"]);
                value.crntPsnIdxNo4 = Convert.ToString(result["crntPsnIdxNo4"]);
                value.plnPsnIdxNo1 = Convert.ToString(result["plnPsnIdxNo1"]);
                value.plnPsnIdxNo2 = Convert.ToString(result["plnPsnIdxNo2"]);
                value.plnPsnIdxNo3 = Convert.ToString(result["plnPsnIdxNo3"]);
                value.plnPsnIdxNo4 = Convert.ToString(result["plnPsnIdxNo4"]);
                value.ordCrntPsnIdxNo1 = Convert.ToString(result["ordCrntPsnIdxNo1"]);
                value.ordCrntPsnIdxNo2 = Convert.ToString(result["ordCrntPsnIdxNo2"]);
                value.ordCrntPsnIdxNo3 = Convert.ToString(result["ordCrntPsnIdxNo3"]);
                value.ordCrntPsnIdxNo4 = Convert.ToString(result["ordCrntPsnIdxNo4"]);
                value.itvCd = Convert.ToString(result["itvCd"]);
                value.itvIdxNo = Convert.ToString(result["itvIdxNo"]);
                value.ordItvCd = Convert.ToString(result["ordItvCd"]);
                value.ordStsCd = Convert.ToString(result["ordStsCd"]);
                value.jobOdrSeqNo = Convert.ToString(result["jobOdrSeqNo"]);
                value.crntPsnBlck = Convert.ToString(result["crntPsnBlck"]);
                value.crntPsnBay = Convert.ToString(result["crntPsnBay"]);
                value.crntPsnRow = Convert.ToString(result["crntPsnRow"]);
                value.crntPsnTier = Convert.ToString(result["crntPsnTier"]);
                value.plnPsnBlck = Convert.ToString(result["plnPsnBlck"]);
                value.plnPsnBay = Convert.ToString(result["crntPsnBay"]);
                value.plnPsnRow = Convert.ToString(result["plnPsnRow"]);
                value.plnPsnTier = Convert.ToString(result["plnPsnTier"]);
            }
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyValidChassisInfos != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyValidChassisInfos(value);

            value = null;
        }

        static public void ItvLinkChassis(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyItvLinkChassis != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyItvLinkChassis(value);

            value = null;
        }

        static public void ItvUnLinkChassis(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyItvUnLinkChassis != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyItvUnLinkChassis(value);

            value = null;
        }

        static public void ReleaseYtFromJob(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyReleaseYtFromJob != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifyReleaseYtFromJob(value);

            value = null;
        }

        static public void SetPLCAutoFlg(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifySetPLCAutoFlg != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifySetPLCAutoFlg(value);

            value = null;
        }

        static public void SetGateCancelJob(Object obj)
        {
            var value = new VMT_Data_JAT2.Objects.ITV.VD_ITV_Result();
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
            if (VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifySetGateCancelJob != null)
                VMT_Data_JAT2.VMT_DataMgr_ITV_Callback.static_NotifySetGateCancelJob(value);

            value = null;
        }
    }
}
