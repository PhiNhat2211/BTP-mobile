using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm.EEStruct
{
    //---------------------------------------------------------
    // char -> Byte // ByValArray
    // TCHAR = String // ByValTStr
    //---------------------------------------------------------

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SoftwareInfo
    // 로그인 하기 위한 ID 입력시 접속자에 대한 확인정보
    //  
    //---------------------------------------------------------
    // Request  UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SwinfoRQ : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public String m_VMTUI_Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String m_szMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String m_szMchnTp;
    }

    // Response Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SwinfoRP : EEParentClass
    {
        public int m_iResult;                // 0 접속불허,    1 접속허가.
        public int m_iLoginResult;           //  0 login이 안된상태   1  login이된상태 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public String m_VMTTray_Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_ID)]
        public String m_UserID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_PW)]
        public String m_UserPW;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_GROUP)]
        public String m_GroupName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_NAME)]
        public String m_DriverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_EXECUTE_SITE)]
        public String m_szSite; // KAP, JAT3 
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ConnectionStatus    
    //  
    //---------------------------------------------------------
    //Request UI -> Tray 
    //class 없음
    // Ack  Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ConnectionStatus : EEParentClass
    {
        public int m_iEagleEyeStatus; //0 이면 연결안됨,    1이면 연결됨
        public int m_iGPSStatus; // 0 이면 연결안됨,    1이면 연결됨
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_MachineNotice    
    //  
    //---------------------------------------------------------
    // Nofity  Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_MachineNotice : EEParentClass
    {
        public int m_iMessageType; //0: 없음 1: TOS  2: VT  3: EE.Server 4: Tray
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_NOTICE_MSG)]
        public String m_strMessage;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_SERVER_MESSAGE)]
        public String m_strMessage2;
    }

    //########################################################################
    //
    //
    //                                                     Login
    //
    //
    //########################################################################

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_GetUserAccessRole
    // 로그인 하기 위한 ID 입력시 접속자에 대한 확인정보
    //  
    //---------------------------------------------------------
    // Request  UI -> Tray 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_GetUSerAccesRoleRQ : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_ID)]
        public String UserID;
    }

    // Response Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_GetUSerAccesRoleRP : EEParentClass
    {
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean bIsOn; // TRUE 성공,FALSE 실패
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 500)]
        public String GroupListSeperator; // "Group1|Group2|Group3" 형태로 전달됨.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
        public String Notice;
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SetLogin4Machine
    //                로그인 사용자의 이름를 화면에 표시한다.
    //---------------------------------------------------------
    // Request  UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SetLogin4MachineRQ : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public String UserID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public String UserPW;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public String GroupName; // 20
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String MchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String MchnTp;
    }
    // Response Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SetLogin4MachineRP : EEParentClass
    {
        public int iLogin; // 0 : 로그인 실패  1: 로그인성공  2: 이미로그인상태        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public String UserName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
        public String Notice;
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SendMachineStatusChange
    //                로그인 사용자의 이름를 화면에 표시한다.
    //---------------------------------------------------------
    // Request  UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SendMachineStatusChangeRQ : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String m_MchnID;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean m_bisON;  //True  ,   False
    };
    // Response Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ResultRP : EEParentClass
    {
        public int m_iResult;		      // 1:  성공 , 0 : 실패
    }

    //########################################################################
    //
    //
    //                                                     Common  Sensing Device
    //
    //
    //########################################################################

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ResetDGPS
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // class 없음.
    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SendPubx05
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // class 없음.


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SendPubx06
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // class 없음.


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SendHighForward
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // class 없음.


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SendHighBackward
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // class 없음.

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_RequestCfgekf
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // Response Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_RequestCfgekfRP : EEParentClass
    {
        public int m_iDirection;
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SaveDGPSCfg
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Request  Tray -> UI
    // class 없음.


    //########################################################################
    //
    //
    //                                                     Available
    //
    //
    //########################################################################

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_MachineStopCodeList
    //                머신 Stop Code 요구
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Response , Notify : Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_Available : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_REASON_CODE)]
        public String ReasonCd;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_REASON_NAME)]
        public String ReasonNm;
    }

    // -> Recv 형태
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_MachineStopCodeList : EEParentClass
    {
        public int m_iAvailableCount;   //갯수
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] // TODO : m_iAvailableCount
        public sTrayUI_Available[] m_pData; // Dynamic
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_GettMachineStop
    //                
    //---------------------------------------------------------
    // Request  UI -> Tray
    // class 없음.
    // Response , Notify : Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_GetMachineStop : EEParentClass
    {
        public sTrayUI_Available Data;
        public int m_iBreak;//0 : No Break    1: Breaking
        public long StartTime;  // 
        public long FinishTime; //
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SetMachineStop
    //                
    //---------------------------------------------------------
    // Send  UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SetMachineStop : EEParentClass
    {
        public sTrayUI_Available Data;
        public int m_iBreakStatus;//1 : Break   2: Release
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String m_szMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String m_szMchnTp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_ID)]
        public String m_UserID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public String m_DriverName;
        public long m_StartTime;  // 
        public long m_FinishTime; //
    }

    // Ack   Tray -> UI class 없음
    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_NotifyMachineStopResult
    //                
    //---------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_NotifyMachineStopResult : EEParentClass
    {
        public sTrayUI_Available Data;
        public int m_iBreakStatus;//1 : Break   2: Release
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String m_szMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String m_szMchnTp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_USER_ID)]
        public String m_UserID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public String m_DriverName;
        public int m_iResult;	      // 1:  성공 , 0 : 실패
    }

    //########################################################################
    //
    //
    //                                                              Alarm
    //
    //
    //########################################################################
    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_Alarm
    //                
    //---------------------------------------------------------
    // Notify    UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_Alram : EEParentClass
    {
        public enum enVmtAlram : int
        {
            ALRAM_OVER_SPEED = 1,
            ALRAM_FUEL_LIMIT,
            ALRAM_TIRE_PRESSURE,
            ALRAM_OVRE_TEMP,
            ALRAM_CROSS_BLOCK,
        };
        public enVmtAlram enType;
        public int nVaildTime;  //Alram 유지타임(sec)
        public Double nValue;        //서버가 감지한 알람상황의 근거데이터 ALRAM_CROSS_BLOCK = 1.0(최대)
        //0.0에 가까울수록 CROSS_BLOCK 근원지와 가까운것 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_ALRAM_DESC)]
        public String szDesc;//ex) SpeedOver, FuelLimit
    };
    // Ack  Tray -> UI Struct없음

    //########################################################################
    //
    //
    //                                                              Job  Common
    //
    //
    //########################################################################

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Job_Machine : EEParentClass
    {
        //머신 아이디
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String mchnId;
        /*
        "Machine Code
     e.g) TT501, RS202,QC103"
        */
        //머신 타입
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String mchnTp;
        /*
        "Machine Type
     e.g) RS : Reach Stacker 
           TC : Transfer Crane (JAT3 AMGC)(NCT RTG)
           TH : Top Handler
           QC : Quay Crane (STS)
           YT : Internal Terminal Truck( Yard Tractor)" 
        */
        //장비 상태
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_STATUS)]
        public String mchnSts;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_VRTFLG)]
        public String vrtlFlg; //Virtual Crane Flag "Y",""
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_JobOrder
    //                
    //---------------------------------------------------------
    // Notify    Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3JobOrder : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String key;
        public EEv2_Job_Machine workingMchn;
        public EEv2_Job_Machine partnerMchn;
        public EEv2_Job_Location locTo;           //일할장소 또는 RH 작업 시 to 위치
        public EEv2_Job_Location locForm;	    //RH 작업 시 from-to 구조일때 from위치
        // 1by1 고려하여 추가된사항 v3로 정의 추가
        public EEv3_Job_Container cntr;
        public EEv3_Job_Type type;
    }
    //  ITV의 경우
    // -> Recv 형태
    //
    //  int                        iCount; // 갯수
    //  EEv3JobOrder               JobOrder[N];
    //  sTrayUI_ITVJobOrderSub     Sub[N];

    //  RTG의 경우    
    // -> Recv 형태
    //
    //  int                        iCount; // 갯수
    //  EEv3JobOrder               JobOrder[N];

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_JobCancel
    //                
    //---------------------------------------------------------
    // Send   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_JobKey : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String jobKey;
    };
    // Ack  UI -> Tray  Struct없음


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_JobDone
    //                
    //---------------------------------------------------------
    // Notify    Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_JobDone : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String jobKey;
    };
    // Ack  UI -> Tray  Struct없음


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ManualJobDone
    //                
    //---------------------------------------------------------
    // Notify    UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ManualJobDone : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String jobKey;
    };
    // Ack  Tray -> UI Struct없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_JobCancelAll
    //                
    //---------------------------------------------------------
    // Notify    Tray -> UI  Struct없음
    // Ack  UI -> Tray  Struct없음


    //---------------------------------------------------------
    //-- 참고 자료
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class EncodedByteArray
    //{
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    //    public Byte[] _data;
    //    public EncodedByteArray(int nLenth)
    //    {
    //        _data = new Byte[nLenth];

    //        //FieldInfo field = typeof(EncodedByteArray).GetField("_data");
    //        //object[] attributes = field.GetCustomAttributes(typeof(MarshalAsAttribute), false);
    //        //MarshalAsAttribute marshal = (MarshalAsAttribute)attributes[0];
    //        //int sizeConst = marshal.SizeConst;
    //    }

    //    public Byte[] SetValue(String str)
    //    {
    //        int nCount = _data.Length;
    //        _data = Encoding.UTF8.GetBytes(str);
    //        Array.Resize(ref _data, nCount);

    //        return _data;
    //    }
    //}
}
