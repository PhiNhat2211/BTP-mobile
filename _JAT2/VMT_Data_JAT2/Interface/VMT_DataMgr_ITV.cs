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

namespace VMT_Data_JAT2
{
    public class VMT_DataMgr_ITV
    {
        static private void LogMessage(String strValue)
        {
            strValue = "[VMT_DataMgr_ITV] " + strValue;
            Util.LogMessage(strValue);
        }

        static private void StructToLogMessage(Object obj)
        {
            VMT_DataMgr_ITV.LogMessage("== Snd StructToLogMessage Start ==");

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
            VMT_DataMgr_ITV.LogMessage(strLog);
            VMT_DataMgr_ITV.LogMessage("== Snd StructToLogMessage End ==");
        }

        //-----------------------------------------------------------------------------
        //- Method
        static public void ChassisInfo_Ask(ref ITV.VD_ITV_ChassisAttachInfo_Send value)
        {
            LogMessage("ChassisInfo_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ChassisAttachInfo chassisAttachInfo = new sTrayUI_ChassisAttachInfo();
                //chassisAttachInfo.nType = (int)value.nType;
                //chassisAttachInfo.m_ChassisNumber = value.m_ChassisNumber;

                //TCPComm.TCPAPI.ITV_SetChassis_Attach(chassisAttachInfo);
            }
        }

        // 메뉴얼 얼라이벌 신호를 서버로 전송합니다.
        static public void ManualArrival_Ask(ref ITV.VD_ITV_SetManuaArrival_Send value) // Not Use, Use SetManualActivation
        {
            LogMessage("ManualArrival_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)ITV.ITV_User.gJobOrderList[0];

                if (currentJob.locWorking.blck != null &&
                    currentJob.locWorking.blck.Length > 1)
                {
                    HessianComm.Objects.YtTask ytTask = new HessianComm.Objects.YtTask();
                    ytTask.workingMchn = new HessianComm.Objects.Machine();
                    ytTask.workingMchn.mchnId = ITV.ITV_User.gMchnID;
                    ytTask.workingMchn.mchnTp = ITV.ITV_User.gMchnTp;
                    ytTask.workingMchn.isOn = true;
                    ytTask.loc = new HessianComm.Objects.Location();
                    ytTask.loc.locTp = new HessianComm.Objects.LocationType();
                    ytTask.loc.locTp.name = "YARD";
                    ytTask.loc.blck = getBlckEntranceID(currentJob);
                    ytTask.moveType = "I";

                    HessianComm.HessianAPI.SetMachinePassed(ytTask);
                }
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_SetManuaArrivalRQ setManuaArrivalRQ = new sTrayUI_SetManuaArrivalRQ();
                //setManuaArrivalRQ.WorkingMchnID = value.WorkingMchnID;
                //setManuaArrivalRQ.PartnerMchnID = value.PartnerMchnID;

                //TCPComm.TCPAPI.ITV_SetManualArrival(setManuaArrivalRQ);
            }
        }

        // 메뉴얼 엑티베이션 신호를 서버로 전송합니다.
        static public void ManualActivation_Ask(ref ITV.VD_ITV_SetManuaArrival_Send value) // Not Use, Use SetManualActivation
        {
            LogMessage("ManualActivation_Ask");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)ITV.ITV_User.gJobOrderList[0];

                if (currentJob.locWorking.blck != null &&
                    currentJob.locWorking.blck.Length > 1)
                {
                    HessianComm.Objects.Task task = new HessianComm.Objects.Task();
                    task.jobId = currentJob.jobKey;
                    task.externalId = currentJob.partnerMchn.mchnId;

                    HessianComm.HessianAPI.SetManualActivation(task);
                }
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_SetManuaArrivalRQ setManuaArrivalRQ = new sTrayUI_SetManuaArrivalRQ();
                //setManuaArrivalRQ.WorkingMchnID = value.WorkingMchnID;
                //setManuaArrivalRQ.PartnerMchnID = value.PartnerMchnID;

                //TCPComm.TCPAPI.ITV_SetManualArrival(setManuaArrivalRQ);
            }
        }

        static public void SetMachineArrival_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob)
        {
            LogMessage("SetMachineArrival_Ask");
            StructToLogMessage(currentJob.jobKey);

            if (UserInfo.IsUseHessian)
            {
                //VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob = (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder)ITV.ITV_User.gJobOrderList[0];

                //if (currentJob.locWorking.blck != null &&
                //    currentJob.locWorking.blck.Length > 1)
                {
                    hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                    strList.Add(currentJob.workingMchn.mchnId);
                    strList.Add(currentJob.ycNo);
                    String locTp = "";
                    if (currentJob.type.jobTp.Equals("MI") || currentJob.type.jobTp.Equals("LC") || currentJob.type.jobTp.Equals("GC"))
                    {
                        if (currentJob.ytAprchLn.Length > 0)
                            locTp += "TP" + currentJob.ytAprchLn.Substring(0, 1);
                    }
                    else if (currentJob.type.jobTp.Equals("DS") || (currentJob.type.jobTp.Equals("LD") && currentJob.type.jobStatus.Equals("C")))
                    {
                        locTp += "LANE";
                    }
                    else
                    {
                        locTp += currentJob.locWorking.locTp;
                    }
                    strList.Add(locTp);
                    if ("MO".Equals(currentJob.type.jobTp) || "LD".Equals(currentJob.type.jobTp))
                    {
                        strList.Add(currentJob.locFrom.blck);
                        strList.Add(currentJob.locFrom.bay);
                    }
                    else
                    {
                        strList.Add(currentJob.locWorking.blck);
                        strList.Add(currentJob.locWorking.bay);
                    }
                    strList.Add(currentJob.type.jobTp);
                    strList.Add(currentJob.cancelYn); //cancelYn Cancel check flag ( Y: Cancel, N: Arrival )
                    strList.Add(UserInfo.gUserID);

                    //HessianComm.Objects.YtTask ytTask = new HessianComm.Objects.YtTask();
                    //ytTask.workingMchn = new HessianComm.Objects.Machine();
                    //ytTask.workingMchn.mchnId = ITV.ITV_User.gMchnID;
                    //ytTask.workingMchn.mchnTp = ITV.ITV_User.gMchnTp;
                    //ytTask.workingMchn.isOn = true;
                    //ytTask.partnerMchn = new HessianComm.Objects.Machine();
                    //ytTask.partnerMchn.mchnId = currentJob.partnerMchn.mchnId;
                    //ytTask.partnerMchn.mchnTp = currentJob.partnerMchn.mchnTp;
                    //ytTask.partnerMchn.isOn = true;
                    //ytTask.loc = new HessianComm.Objects.Location();
                    //ytTask.loc.locTp = new HessianComm.Objects.LocationType();
                    //ytTask.loc.locTp.name = "LANE";
                    //ytTask.loc.bay = currentJob.locWorking.location;
                    ////ytTask.loc.blck = getBlckEntranceID(currentJob);
                    //ytTask.moveType = "W";

                    HessianComm.HessianAPI.SetMachineArrival(strList);
                }
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_SetManuaArrivalRQ setManuaArrivalRQ = new sTrayUI_SetManuaArrivalRQ();
                //setManuaArrivalRQ.WorkingMchnID = value.WorkingMchnID;
                //setManuaArrivalRQ.PartnerMchnID = value.PartnerMchnID;

                //TCPComm.TCPAPI.ITV_SetManualArrival(setManuaArrivalRQ);
            }
        }

        static public void SetMachineReady_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob)
        {
            LogMessage("ManualReady_Ask");
            StructToLogMessage(currentJob.jobKey);

            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                strList.Add(currentJob.workingMchn.mchnId);
                strList.Add(currentJob.ycNo);

                if ("MO".Equals(currentJob.type.jobTp) || "LD".Equals(currentJob.type.jobTp))
                {
                    strList.Add(currentJob.locFrom.blck);
                    strList.Add(currentJob.locFrom.bay);
                }
                else
                {
                    strList.Add(currentJob.locWorking.blck);
                    strList.Add(currentJob.locWorking.bay);
                }

                strList.Add(UserInfo.gUserID);
                strList.Add(currentJob.jobKey);


                //HessianComm.Objects.Task task = new HessianComm.Objects.Task();
                //task.workingMchn = new HessianComm.Objects.Machine();
                //task.workingMchn.mchnId = currentJob.workingMchn.mchnId;
                //task.workingMchn.mchnTp = currentJob.workingMchn.mchnTp;

                //task.partnerMchn = new HessianComm.Objects.Machine();
                //task.partnerMchn.mchnId = currentJob.partnerMchn.mchnId;
                //task.partnerMchn.mchnTp = currentJob.partnerMchn.mchnTp;

                //task.loc = new HessianComm.Objects.Location();
                //task.loc.locTp = new HessianComm.Objects.LocationType();
                //task.loc.locTp.name = currentJob.locWorking.locTp;
                //task.loc.blck = currentJob.locWorking.blck;
                //// task.loc.blck = getBlckEntranceID(currentJob); 
                //task.loc.bay = currentJob.locWorking.bay;
                //task.jobTp = currentJob.type.jobTp;

                HessianComm.HessianAPI.SetMachineReady(strList);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_SetManualReadyRQ setManuaArrivalRQ = new sTrayUI_SetManualReadyRQ();
                //setManuaArrivalRQ.WorkingMchnID = value.WorkingMchnID;
                //setManuaArrivalRQ.PartnerMchnID = value.PartnerMchnID;

                //TCPComm.TCPAPI.ITV_SetManualReady(setManuaArrivalRQ);
            }
        }

        public static void SetJobDone_Ask(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobDoneSend)
        {
            if (jobDoneSend.type.jobTp.Equals("DS") || jobDoneSend.type.jobTp.Equals("MI") || jobDoneSend.type.jobTp.Equals("GI") ||
                  jobDoneSend.type.jobTp.Equals("LC") || jobDoneSend.type.jobTp.Equals("GC") || jobDoneSend.type.jobTp.Equals("AH") ||
                      jobDoneSend.type.jobTp.Equals("RH"))
                jobDoneSend.locWorking.locTp = "YARD";
            else if (jobDoneSend.type.jobTp.Equals("LD") || jobDoneSend.type.jobTp.Equals("MO") || jobDoneSend.type.jobTp.Equals("GO"))
            {
                if (jobDoneSend.partnerMchn.aprchLn.Contains("W"))
                    jobDoneSend.locWorking.locTp = "TPW";
                else if (jobDoneSend.partnerMchn.aprchLn.Contains("L"))
                    jobDoneSend.locWorking.locTp = "TPL";
                else
                {
                    if (jobDoneSend.type.jobTp.Equals("GO"))
                        jobDoneSend.locWorking.locTp = "TPL";
                    else
                        jobDoneSend.locWorking.locTp = "TPW";
                }
            }
            else if (!jobDoneSend.type.jobTp.Equals("NONE"))
                jobDoneSend.locWorking.locTp = "YARD";

            VmtWorkOrder vmtWorkOrder = new VmtWorkOrder();
            vmtWorkOrder.jobKey = jobDoneSend.jobKey;
            vmtWorkOrder.cntrNo = jobDoneSend.cntr.cntrNo;
            vmtWorkOrder.mchnId = jobDoneSend.ycNo;
            vmtWorkOrder.mchnTp = jobDoneSend.partnerMchn.mchnTp;
            vmtWorkOrder.isMvmtIn = false;
            vmtWorkOrder.isGcBtn = jobDoneSend.isGcBtn;
            vmtWorkOrder.locTp = jobDoneSend.locWorking.locTp;
            vmtWorkOrder.blck = jobDoneSend.locWorking.blck;
            vmtWorkOrder.bay = jobDoneSend.locWorking.bay;
            vmtWorkOrder.row = jobDoneSend.locWorking.row;
            vmtWorkOrder.tier = jobDoneSend.locWorking.tier;
            vmtWorkOrder.qcLn = jobDoneSend.qcLn;
            vmtWorkOrder.userId = UserInfo.gUserID;
            vmtWorkOrder.workingStatus = jobDoneSend.workingStatus;
            vmtWorkOrder.ytNo = UserInfo.gMchnID;
            vmtWorkOrder.positionOnChassis = jobDoneSend.type.jobFlagInfo;           

            HessianComm.HessianAPI.SetJobDone(vmtWorkOrder);
        }

        static public void SetItvDone_Ask(Objects.Common.VD_Common_JobOrder selectJob, Objects.Common.VD_Common_JobOrder unSelectJob)
        {
            LogMessage("setItvDone");
            StructToLogMessage(selectJob.jobKey);

            if (UserInfo.IsUseHessian)
            {
                if (selectJob != null && selectJob.type != null && selectJob.partnerMchn != null && selectJob.ytJbSts != null &&
                    selectJob.type.jobTp.Equals("LD") && selectJob.partnerMchn.mchnSts.Equals("C") && selectJob.ytJbSts.Equals("A"))
                {
                    if (unSelectJob != null && unSelectJob.cntr != null && unSelectJob.type != null && unSelectJob.ytJbSts != null &&
                        unSelectJob.type.qcId == selectJob.type.qcId && unSelectJob.ytJbSts == selectJob.ytJbSts)
                    {
                        hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();

                        strList.Add(selectJob.type.qcId);
                        strList.Add(selectJob.workingMchn.mchnId);
                        strList.Add(selectJob.cntr.cntrNo);
                        strList.Add(unSelectJob.cntr.cntrNo);
                        strList.Add(UserInfo.gUserID);

                        HessianComm.HessianAPI.SetQCJobReleaseByYt(strList);
                    }
                    else
                    {
                        hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();

                        strList.Add(selectJob.type.qcId);
                        strList.Add(selectJob.workingMchn.mchnId);
                        strList.Add(selectJob.cntr.cntrNo);
                        strList.Add("");
                        strList.Add(UserInfo.gUserID);

                        HessianComm.HessianAPI.SetQCJobReleaseByYt(strList);
                    }                   
                }
                else
                {
                    hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                    strList.Add(selectJob.jobKey);
                    strList.Add(ITV.ITV_User.gUserID);
                    strList.Add(ITV.ITV_User.gMchnID);

                    HessianComm.HessianAPI.SetItvDone(strList);
                }
               
                //HessianComm.Objects.YtTask ytTask = new HessianComm.Objects.YtTask();
                //ytTask.jobId = value.jobKey;
                //ytTask.workingMchn = new HessianComm.Objects.Machine();
                //ytTask.workingMchn.mchnId = ITV.ITV_User.gMchnID;
                //ytTask.workingMchn.mchnTp = ITV.ITV_User.gMchnTp;
                //ytTask.externalId = ITV.ITV_User.gMchnID;

            }

            if (UserInfo.IsUseTCP)
            {
            }
        }
        
        //Set Job Done For QC
        static public void SetQCJobReleaseByYt_Ask(List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> tasks)
        {
            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList hessianList = new hessiancsharp.Class.HessianList();

                if (tasks.Count >= 1)
                {
                    hessianList.Add(tasks[0].type.qcId);
                    hessianList.Add(UserInfo.gMchnID);
                    hessianList.Add(tasks[0].cntr.cntrNo);
                    hessianList.Add((tasks.Count == 2 && tasks[1] != null) ? tasks[1].cntr.cntrNo : "");
                    hessianList.Add(UserInfo.gUserID);
                }
                HessianComm.HessianAPI.SetQCJobReleaseByYt(hessianList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        static public void changeChassisNo_Ask(String chssNo, String newChssNo)
        {
            LogMessage("setItvDone");
            StructToLogMessage(chssNo);

            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                strList.Add(ITV.ITV_User.gMchnID);
                strList.Add(ITV.ITV_User.gMchnTp);
                strList.Add(chssNo);
                strList.Add(newChssNo);
                strList.Add(ITV.ITV_User.gUserID);

                //HessianComm.Objects.YtTask ytTask = new HessianComm.Objects.YtTask();
                //ytTask.jobId = value.jobKey;
                //ytTask.workingMchn = new HessianComm.Objects.Machine();
                //ytTask.workingMchn.mchnId = ITV.ITV_User.gMchnID;
                //ytTask.workingMchn.mchnTp = ITV.ITV_User.gMchnTp;
                //ytTask.externalId = ITV.ITV_User.gMchnID;

                HessianComm.HessianAPI.ChangeChassisNo(strList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        static public void changeDriver_Ask()
        {
            LogMessage("changeDriver");

            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                strList.Add(ITV.ITV_User.gUserID);
                strList.Add(ITV.ITV_User.gUserNm);
                strList.Add(ITV.ITV_User.gMchnID);
                strList.Add(ITV.ITV_User.gMchnTp);

                HessianComm.HessianAPI.ChangeDriver(strList);
            }

            if (UserInfo.IsUseTCP)
            {
            }
        }

        static public void GetChssUsingData_Ask(String chssNo)
        {
            LogMessage("GetChssUsingData_Ask");
            StructToLogMessage(chssNo);

            if (UserInfo.IsUseHessian)
            {
                HessianComm.HessianAPI.GetChssUsingData(chssNo);
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

        // X-Ray Scan
        static public void SetConfirmJobByScanner(ref VMT_Data_JAT2.Objects.Common.VD_Common_ManualJobDone_Send value)
        {
            LogMessage("SetConfirmJobByScanner");
            StructToLogMessage(value);

            if (UserInfo.IsUseHessian)
            {
                String jobKey = value.jobKey;
                String userId = ITV.ITV_User.gUserID;

                hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                strList.Add(jobKey);
                strList.Add(userId);

                HessianComm.HessianAPI.SetConfirmJobByScanner(strList);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        static public void ChassisOrderComplete(VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder)
        {
            LogMessage("ChassisOrderComplete");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.ChassisOrder chasOrd = new HessianComm.Objects.ChassisOrder();
                chasOrd.ordSeq = chassOrder.ordSeq;
                chasOrd.chssNo = chassOrder.chssNo;
                chasOrd.crntPsnIdxNo1 = chassOrder.crntPsnIdxNo1;
                chasOrd.crntPsnIdxNo2 = chassOrder.crntPsnIdxNo2;
                chasOrd.crntPsnIdxNo3 = chassOrder.crntPsnIdxNo3;
                chasOrd.crntPsnIdxNo4 = chassOrder.crntPsnIdxNo4;
                chasOrd.plnPsnIdxNo1 = chassOrder.plnPsnIdxNo1;
                chasOrd.plnPsnIdxNo2 = chassOrder.plnPsnIdxNo2;
                chasOrd.plnPsnIdxNo3 = chassOrder.plnPsnIdxNo3;
                chasOrd.plnPsnIdxNo4 = chassOrder.plnPsnIdxNo4;
                chasOrd.itvCd = chassOrder.itvCd;
                chasOrd.ordStsCd = chassOrder.ordStsCd;
                chasOrd.cntrNo = chassOrder.cntrNo;
                HessianComm.HessianAPI.ChassisOrderComplete(chasOrd);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        static public void SetRealLocation(string jobKey)
        {
            LogMessage("SetRealLocation");

            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                strList.Add(jobKey);

                HessianComm.HessianAPI.SetReallocation(strList);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        static public void ItvUnLinkChassis(VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder)
        {
            LogMessage("ItvUnLinkChassis");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.ChassisOrder chasOrd = new HessianComm.Objects.ChassisOrder();
                chasOrd.ordSeq = chassOrder.ordSeq;
                chasOrd.chssNo = chassOrder.chssNo;
                chasOrd.crntPsnIdxNo1 = chassOrder.crntPsnIdxNo1;
                chasOrd.crntPsnIdxNo2 = chassOrder.crntPsnIdxNo2;
                chasOrd.crntPsnIdxNo3 = chassOrder.crntPsnIdxNo3;
                chasOrd.crntPsnIdxNo4 = chassOrder.crntPsnIdxNo4;
                chasOrd.plnPsnIdxNo1 = chassOrder.plnPsnIdxNo1;
                chasOrd.plnPsnIdxNo2 = chassOrder.plnPsnIdxNo2;
                chasOrd.plnPsnIdxNo3 = chassOrder.plnPsnIdxNo3;
                chasOrd.plnPsnIdxNo4 = chassOrder.plnPsnIdxNo4;
                chasOrd.itvCd = chassOrder.itvCd;
                chasOrd.ordStsCd = chassOrder.ordStsCd;
                chasOrd.cntrNo = chassOrder.cntrNo;
                //chasOrd.isArrival = chassOrder.isArrival; //tien isArrival
                HessianComm.HessianAPI.ItvUnLinkChassis(chasOrd);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        static public void ItvLinkChassis(VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder)
        {
            LogMessage("ItvLinkChassis");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.ChassisOrder chasOrd = new HessianComm.Objects.ChassisOrder();
                chasOrd.ordSeq = chassOrder.ordSeq;
                chasOrd.chssNo = chassOrder.chssNo;
                chasOrd.crntPsnIdxNo1 = chassOrder.crntPsnIdxNo1;
                chasOrd.crntPsnIdxNo2 = chassOrder.crntPsnIdxNo2;
                chasOrd.crntPsnIdxNo3 = chassOrder.crntPsnIdxNo3;
                chasOrd.crntPsnIdxNo4 = chassOrder.crntPsnIdxNo4;
                chasOrd.plnPsnIdxNo1 = chassOrder.plnPsnIdxNo1;
                chasOrd.plnPsnIdxNo2 = chassOrder.plnPsnIdxNo2;
                chasOrd.plnPsnIdxNo3 = chassOrder.plnPsnIdxNo3;
                chasOrd.plnPsnIdxNo4 = chassOrder.plnPsnIdxNo4;
                chasOrd.itvCd = chassOrder.itvCd;
                chasOrd.ordStsCd = chassOrder.ordStsCd;
                chasOrd.cntrNo = chassOrder.cntrNo;
                HessianComm.HessianAPI.ItvLinkChassis(chasOrd);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        static public void SetGateCancelJob(string jobKey)
        {
            LogMessage("SetGateCancelJob ");

            if (UserInfo.IsUseHessian)
            {
                hessiancsharp.Class.HessianList strList = new hessiancsharp.Class.HessianList();
                strList.Add(jobKey);
                strList.Add(UserInfo.gUserID);
                HessianComm.HessianAPI.SetGateCancelJob(strList);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        static public void ValidChassisInfos(VMT_Data_JAT2.Objects.Common.VD_Common_ChassisOrder chassOrder)
        {
            LogMessage("ValidChassisInfos");

            if (UserInfo.IsUseHessian)
            {
                HessianComm.Objects.ChassisInventory chasOrd = new HessianComm.Objects.ChassisInventory();
                if (chassOrder != null)
                {
                    chasOrd.chssNo = chassOrder.chssNo;
                    chasOrd.crntPsnIdxNo1 = chassOrder.crntPsnIdxNo1;
                    chasOrd.crntPsnIdxNo2 = chassOrder.crntPsnIdxNo2;
                    chasOrd.crntPsnIdxNo3 = chassOrder.crntPsnIdxNo3;
                    chasOrd.crntPsnIdxNo4 = chassOrder.crntPsnIdxNo4;
                    chasOrd.itvCd = chassOrder.itvCd;
                }
                
                HessianComm.HessianAPI.ValidChassisInfos(chasOrd);
            }

            if (UserInfo.IsUseTCP)
            {
                //sTrayUI_ManualJobDone manualJobDone = new sTrayUI_ManualJobDone();
                //manualJobDone.jobKey = value.jobKey;

                //TCPComm.TCPAPI.ManualJobDone(manualJobDone);
            }
        }

        public static void SetPLCAutoFlg_Ask(Boolean plcAutoFlag)
        {
            var hessianList = new hessiancsharp.Class.HessianList();

            hessianList.Add(UserInfo.gMchnID);
            hessianList.Add(UserInfo.gMchnTp);
            hessianList.Add(plcAutoFlag);
            hessianList.Add(UserInfo.gUserID);

            HessianComm.HessianAPI.SetPLCAutoFlg(hessianList);
        }
        //-----------------------------------------------------------------------------

        #region [Help Function]
        public static String getBlckEntranceID(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder currentJob)
        {   
            //String[] blckArray = { "A", "B", "C", "D", "E", "F", "G" };
            String[] blckArray = { "A", "B", "C", "D", "E" };

            String retValue = "";

            String strBlck = currentJob.locWorking.blck;
            String strlocTp = currentJob.locWorking.locTp;

            for (int i = 0; i < blckArray.Length; i++)
            {
                if (strBlck.Substring(strBlck.Length - 1).Equals(blckArray[i]))
                {
                    if (strlocTp.Equals("TPW"))
                    {
                        if (strBlck.Substring(strBlck.Length - 1).Equals("A"))
                        {
                            retValue = strBlck + "T";
                        }
                        else
                        {
                            retValue = strBlck.Substring(0, strBlck.Length - 1) + blckArray[i - 1] + strBlck;
                        }
                    }
                    else if (strlocTp.Equals("TPL"))
                    {
                        if (strBlck.Substring(strBlck.Length - 1).Equals("E"))
                        {
                            retValue = strBlck + "B";
                        }
                        else
                        {
                            retValue = strBlck + strBlck.Substring(0, strBlck.Length - 1) + blckArray[i + 1];
                        }
                    }
                    break;
                }
            }

            return retValue;
        }
        #endregion [Help Function]
    }

    public class VMT_DataMgr_ITV_Callback
    {
        // ITV 잡정보를 통지받습니다
        public delegate void Callback_NotifyJobOrderITV(ref ITV.VD_ITV_JobOrderList value);
        static public void SetCallBack_NotifyJobOrderITV(Callback_NotifyJobOrderITV fp)
        {
            static_NotifyJobOrderITV = fp;
        }

        public delegate void Callback_NotifyGetMachineStatusChanged(VMT_Data_JAT2.Objects.Common.VmtMachine value);
        static public void SetCallBack_NotifyGetMachineStatusChanged(Callback_NotifyGetMachineStatusChanged fp)
        {
            static_NotifyGetMachineStatusChanged = fp;
        }

        // Ariival
        public delegate void Callback_NotifySetMachineArrival(ITV.VD_ITV_Result value);
        static public void SetCallBack_NotifySetMachineArrival(Callback_NotifySetMachineArrival fp)
        {
            static_ITV_NotifySetMachineArrival = fp;
        }

        // Ready
        public delegate void Callback_NotifySetMachineReady(ITV.VD_ITV_Result value);
        static public void SetCallBack_NotifySetMachineReady(Callback_NotifySetMachineReady fp)
        {
            static_ITV_NotifySetMachineReady = fp;
        }

        // Done
        public delegate void Callback_NotifySetItvDone(ITV.VD_ITV_Result value);
        static public void SetCallBack_NotifySetItvDone(Callback_NotifySetItvDone fp)
        {
            static_ITV_NotifySetItvDone = fp;
        }

        //Set Job Done For QC
        public delegate void Callback_NotifySetQCJobDoneByYt(ITV.VD_ITV_Result value);
        static public void SetCallBack_NotifySetQCJobDoneByYt(Callback_NotifySetQCJobDoneByYt fp)
        {
            static_ITV_NotifySetQCJobReleaseByYt = fp;
        }

        public delegate void Callback_NotifyChangeChassisNo(ITV.VD_ITV_Result value);
        static public void SetCallBack_NotifyChangeChassisNo(Callback_NotifyChangeChassisNo fp)
        {
            static_ITV_NotifyChangeChassisNo = fp;
        }
        public delegate void Callback_NotifyChangeDriver(Boolean value);
        static public void SetCallBack_NotifyChangeDriver(Callback_NotifyChangeDriver fp)
        {
            static_ITV_NotifyChangeDriver = fp;
        }
        public delegate void Callback_NotifyGetChssUsingData(String value);
        static public void SetCallBack_NotifyGetChssUsingData(Callback_NotifyGetChssUsingData fp)
        {
            static_ITV_NotifyGetChssUsingData = fp;
        }

        // ITV 주기적 데이터 (속도)
        public delegate void Callback_ITV_PDS_Periodic_Payload(ref ITV.VD_ITV_PDS_Periodic_Payload value);
        static public void SetCallback_ITV_PDS_Periodic_Payload(Callback_ITV_PDS_Periodic_Payload fp)
        {
            static_ITV_PDS_Periodic_Payload = fp;
        }

        public delegate void Callback_ITV_PDS_Event_Payload(ref ITV.VD_ITV_PDS_Event_Payload value);
        static public void SetCallback_ITV_PDS_Event_Payload(Callback_ITV_PDS_Event_Payload fp)
        {
            static_ITV_PDS_Event_Payload = fp;
        }

        public delegate void Callback_ITV_NotifyChassis_Attach(ref ITV.VD_ITV_ChassisAttachInfo_Receive value);
        static public void SetCallback_ITV_NotifyChassis_Attach(Callback_ITV_NotifyChassis_Attach fp)
        {
            static_ITV_NotifyChassis_Attach = fp;
        }

        public delegate void Callback_BlockEnterance(ref ITV.VD_ITV_NotifyBlockEnter_Receive value);
        static public void SetCallback_BlockEnterance(Callback_BlockEnterance fp)
        {
            static_ITV_NotifyBlockEnterance = fp;
        }

        public delegate void Callback_CPSAlign(ref ITV.VD_ITV_NotifyCPSAlign_Receive value);
        static public void SetCallback_NotifyCPSAlign(Callback_CPSAlign fp)
        {
            static_ITV_NotifyCPSAlign = fp;
        }

        public delegate void Callback_NotifySTSLDSeq(ref ITV.VD_ITV_STS_LDPlan value);
        static public void SetCallback_NotifySTSLDSeq(Callback_NotifySTSLDSeq fp)
        {
            static_NotifySTSLDSeq = fp;
        }

        public delegate void Callback_NotifyPinningStation(ref Object value);
        public static void SetCallBack_NotifyPinningStation(Callback_NotifyPinningStation fp)
        {
            static_NotifyPinningStation = fp;
        }

        public delegate void Callback_NotifyConfirmJobByScanner(Boolean value);
        static public void SetCallback_NotifyConfirmJobByScanner(Callback_NotifyConfirmJobByScanner fp) // NCT
        {
            static_NotifyConfirmJobByScanner = fp;
        }

        public delegate void Callback_NotifyArrvdMchnAtPow(String value);
        static public void SetCallBack_NotifyArrvdMchnAtPow(Callback_NotifyArrvdMchnAtPow fp)
        {
            static_NotifyArrvdMchnAtPow = fp;
        }

        public delegate void Callback_NotifyItvLinkChassis(ITV.VD_ITV_Result value);
        static public void SetCallback_NotifyItvLinkChassis(Callback_NotifyItvLinkChassis fp)
        {
            static_NotifyItvLinkChassis = fp;
        }

        public delegate void Callback_NotifyItvUnLinkChassis(ITV.VD_ITV_Result value);
        static public void SetCallback_NotifyItvUnLinkChassis(Callback_NotifyItvUnLinkChassis fp)
        {
            static_NotifyItvUnLinkChassis = fp;
        }

        public delegate void Callback_NotifyChassisOrderCompletes(ITV.VD_ITV_Result value);
        static public void SetCallback_NotifyChassisOrderCompletes(Callback_NotifyChassisOrderCompletes fp)
        {
            static_NotifyChassisOrderComplete = fp;
        }

        public delegate void Callback_static_NotifySetReallocation(ITV.VD_ITV_Result value);
        static public void SetCallback_static_NotifySetReallocation(Callback_static_NotifySetReallocation fp)
        {
            static_NotifySetReallocation = fp;
        }

        public delegate void Callback_NotifyValidChassisInfos(ITV.VD_Common_ChassisInventory value);
        static public void SetCallback_NotifyValidChassisInfos(Callback_NotifyValidChassisInfos fp)
        {
            static_NotifyValidChassisInfos = fp;
        }

        public delegate void Callback_NotifySetPLCAutoFlg(ITV.VD_ITV_Result value);
        static public void SetCallback_NotifySetPLCAutoFlg(Callback_NotifySetPLCAutoFlg fp)
        {
            static_NotifySetPLCAutoFlg = fp;
        }

        public delegate void Callback_NotifySetGateCancelJob(ITV.VD_ITV_Result value);
        static public void SetCallback_NotifySetGateCancelJob(Callback_NotifySetGateCancelJob fp)
        {
            static_NotifySetGateCancelJob = fp;
        }

        public delegate void Callback_NotifyReleaseYtFromJob(ITV.VD_ITV_Result value);
        static public void SetCallback_NotifyReleaseYtFromJob(Callback_NotifyReleaseYtFromJob fp)
        {
            static_NotifyReleaseYtFromJob = fp;
        }


        static public Callback_ITV_PDS_Periodic_Payload static_ITV_PDS_Periodic_Payload;
        static public Callback_ITV_PDS_Event_Payload static_ITV_PDS_Event_Payload;
        static public Callback_ITV_NotifyChassis_Attach static_ITV_NotifyChassis_Attach;
        static public Callback_NotifyJobOrderITV static_NotifyJobOrderITV;
        static public Callback_NotifyGetMachineStatusChanged static_NotifyGetMachineStatusChanged;
        static public Callback_NotifySetMachineArrival static_ITV_NotifySetMachineArrival;
        static public Callback_NotifySetMachineReady static_ITV_NotifySetMachineReady;
        static public Callback_NotifySetItvDone static_ITV_NotifySetItvDone;
        static public Callback_NotifySetQCJobDoneByYt static_ITV_NotifySetQCJobReleaseByYt; // Set Job Done For QC
        static public Callback_NotifyChangeChassisNo static_ITV_NotifyChangeChassisNo;
        static public Callback_NotifyChangeDriver static_ITV_NotifyChangeDriver;
        static public Callback_NotifyGetChssUsingData static_ITV_NotifyGetChssUsingData;
        static public Callback_BlockEnterance static_ITV_NotifyBlockEnterance;
        static public Callback_CPSAlign static_ITV_NotifyCPSAlign;
        static public Callback_NotifySTSLDSeq static_NotifySTSLDSeq;
        static public Callback_NotifyPinningStation static_NotifyPinningStation;
        static public Callback_NotifyArrvdMchnAtPow static_NotifyArrvdMchnAtPow;
        static public Callback_NotifyConfirmJobByScanner static_NotifyConfirmJobByScanner;

        static public Callback_NotifyChassisOrderCompletes static_NotifyChassisOrderComplete;
        static public Callback_static_NotifySetReallocation static_NotifySetReallocation;
        static public Callback_NotifyItvLinkChassis static_NotifyItvLinkChassis;
        static public Callback_NotifyItvUnLinkChassis static_NotifyItvUnLinkChassis;
        static public Callback_NotifyValidChassisInfos static_NotifyValidChassisInfos;
        static public Callback_NotifySetPLCAutoFlg static_NotifySetPLCAutoFlg;
        static public Callback_NotifySetGateCancelJob static_NotifySetGateCancelJob;
        static public Callback_NotifyReleaseYtFromJob static_NotifyReleaseYtFromJob;
        //static public Callback_NotifyConfirmJobByScanner static_NotifyConfirmJobByScanner;
        //static public Callback_NotifyConfirmJobByScanner static_NotifyConfirmJobByScanner;
        //static public Callback_NotifyConfirmJobByScanner static_NotifyConfirmJobByScanner;
    }
}