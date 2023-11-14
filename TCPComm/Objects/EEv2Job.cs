using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm.EEStruct
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Job_Type : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_TYPE)]
        public String jobTp;
        /*
        DS : Discharge
        LD : Load
        MI : Movement In Yard
        MO : Movement Out
        RH : Rehandleing
        AH : Auto Rehandleing
        LC : Loading Cancel
        GI : Gate In
        GO : Gate Out
        GC : Gate Out Cancel"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_STATUS)]
        public String jobStatus;//2
        //Active = "A", Inactive="Q", Processing = "P", Completed = "C"
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_VESSEL_CODE)]
        public String vslCd;//10
        //<vslCd>HJLN</vslCd> // DS,LOAD 때만 기입
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_VOY_NO)]
        public String voyNo;//16
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_PLAN_SQ)]
        public String planSeq;//16
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_FLAG)]
        public String twinTandemFlg;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_KEY)]
        public String twinTandumKey;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String tandemJoinYT;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_JOB_FLAG_INFO)]
        public String jobFlagInfo;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_CONE_PLAN)]
        public String conePlan;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3_Job_Type : EEv2_Job_Type
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_ETW)]
        public String etw;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_EVENT_ID)]
        public String eventId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOBTYPE_QUEUE)]
        public String queue;
    }

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class EEv3JobOrder
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
    //    public String key;
    //    EEv2_Job_Machine workingMchn;
    //    EEv2_Job_Machine partnerMchn;
    //    EEv2_Job_Location loc;           //일할장소 또는 RH 작업 시 to 위치
    //    EEv2_Job_Location locForm;	 //RH 작업 시 from-to 구조일때 from위치

    //    // 1by1 고려하여 추가된사항 v3로 정의 추가
    //    EEv3_Job_Container cntr;
    //    EEv3_Job_Type type;
    //}

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3JobOrderSub : EEv3JobOrder
    {
        int m_iArrowIndex;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2JobOrder : EEParentClass
    {
        EEv2_Job_Machine workingMchn;
        EEv2_Job_Machine partnerMchn;
        EEv2_Job_Container cntr;
        EEv2_Job_Location loc;
        EEv2_Job_Type type;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2OtrJob : EEParentClass
    {
        EEv2_Job_Machine partnerMchn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        EEv2_Job_Container[] cntr;
        EEv2_Job_Location loc;
        EEv2_Job_Type type;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_ITV_Job : EEParentClass
    {
        EEv2JobOrder sJob_Order_0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_ITV_JOB_STAT)]
        public String szReadyArrivalFlag_0;
        int nETA_0;
        EEv2JobOrder sJob_Order_1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_ITV_JOB_STAT)]
        public String szReadyArrivalFlag_1;
        int nETA_1;
        int nReserved_0;
        int nReserved_1;
        int nReserved_2;
        int nReserved_3;
    };
}