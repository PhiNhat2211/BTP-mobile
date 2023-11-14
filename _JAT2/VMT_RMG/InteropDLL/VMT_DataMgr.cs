using System;
using System.Runtime.InteropServices;
using System.Text;
using VMT_RMG;
using HANDLE = System.IntPtr;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace ExternalAPI
{
    
    public class VMT_DataMgr
    {
        //---------------------------------------------------------
        //- Constant Definition
        #region [ Constant Defninition ]

        private const int EEVE_TIER_MAX_ROW = 49;

        private const int EEV2_STRING_MAX_ROOT = 20;

        private const int EEV2_STRING_MAX_MACHINE_ID	= 20;
        private const int EEV2_STRING_MAX_MACHINE_TP = EEV2_STRING_MAX_MACHINE_ID;
        private const int EEV2_STRING_MAX_MACHINE_STATUS	= 3;
        private const int EEV2_STRING_MAX_MACHINE_VRTFLG	= 3;

        private const int EEV2_STRING_MAX_LOCATION_TYPE = 10;
        private const int EEV2_STRING_MAX_LOCATION_BLOCK	= 9+1; //FENCE
        private const int EEV2_STRING_MAX_LOCATION_BAY = 3+1;
        private const int EEV2_STRING_MAX_LOCATION_ROW = 3+1;
        private const int EEV2_STRING_MAX_LOCATION_TIER = 2+1;
        private const int EEV2_STRING_MAX_LOCATION_LANE = 2+1;
        private const int EEV2_STRING_MAX_LOCATION_LOCATION = 30;
        private const int EEV2_STRING_MAX_LOCATION_BAY_NAME = 10;

        private const int EEV2_STRING_MAX_BOUND_ID = 20;

        private const int EEV2_STRING_MAX_VESSEL_BLOCK = 20;
        private const int EEV2_STRING_MAX_VESSEL_CNTR_NO	= 16;
        private const int EEV2_STRING_MAX_VESSEL_MACHINE_ID = 10;


        private const int EEV2_STRING_MAX_CNTR_NO = 13;
        private const int EEV2_STRING_MAX_CNTR_ISO = 10;
        private const int EEV2_STRING_MAX_CNTR_TYPE = 4;
        private const int EEV2_STRING_MAX_CNTR_STYPE_DESC = 8;
        private const int EEV2_STRING_MAX_CNTR_OPRCODE = 10;
        private const int EEV2_STRING_MAX_CNTR_CLASS = 4;
        private const int EEV2_STRING_MAX_CNTR_CARGO_TYPE = 2+1;
        private const int EEV2_STRING_MAX_CNTR_FULL_MTY = 2+1;
        private const int EEV2_STRING_MAX_CNTR_LENGTH = 10;
        private const int EEV2_STRING_MAX_CNTR_HEIGHT = 10;
        private const int EEV2_STRING_MAX_CNTR_WEIGHT = 10;
        private const int EEV2_STRING_MAX_CNTR_PORT_OF_DISCHARGE	= 16;
        private const int EEV2_STRING_MAX_CNTR_NEXT_PORT	= 16;
        private const int EEV2_STRING_MAX_CNTR_PORT_OF_LOAD = 16;
        private const int EEV2_STRING_MAX_CNTR_FINAL_PORT = 16;
        private const int EEV2_STRING_MAX_CNTR_GRADE = 8;
        private const int EEV2_STRING_MAX_CNTR_DOOR_DIRECTION = 3;


        private const int EEV2_STRING_MAX_JOBTYPE_TYPE = 3;
        private const int EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_FLAG = 3;
        private const int EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_KEY = 40;
        private const int EEV2_STRING_MAX_JOBTYPE_STATUS = 2;
        private const int EEV2_STRING_MAX_JOBTYPE_VESSEL_CODE = 10;
        private const int EEV2_STRING_MAX_JOBTYPE_VOY_NO = 16;
        private const int EEV2_STRING_MAX_JOBTYPE_PLAN_SQ = 20; //
        private const int EEV2_STRING_MAX_JOBTYPE_JOB_FLAG_INFO = 3;


        private const int EEV2_STRING_MAX_DMG_CD = 10;
        private const int EEV2_STRING_MAX_DMG_INOUT = 3;
        private const int EEV2_STRING_MAX_DMG_PART = 3;
        private const int EEV2_STRING_MAX_DMG_RANGE = 20;
        private const int EEV2_STRING_MAX_DMG_DESC = 128;

        private const int EEV2_STRING_MAX_IMDG_IMDG = 10;
        private const int EEV2_STRING_MAX_IMDG_UNNO = 10;
        private const int EEV2_STRING_MAX_IMDG_FIRECODE = 10;

        private const int EEV2_STRING_MAX_SEAL_SEALNO = 20;
        private const int EEV2_STRING_MAX_SEAL_TYPE = 3;

        private const int EEV2_STRING_MAX_GANG_POOL = 30;


        private const int EEV2_STRING_MAX_COLORCODE_CODE = 10;
        private const int EEV2_STRING_MAX_COLORCODE_FORE = 10;
        private const int EEV2_STRING_MAX_COLORCODE_BACK	= 10;

        private const int EEV2_STRING_MAX_VESSEL_CODE = 10;
        private const int EEV2_STRING_MAX_VESSEL_VESSEL = 50;

        private const int EEV2_STRING_MAX_REASON_CODE = 20;
        private const int EEV2_STRING_MAX_REASON_NAME = 30;


        private const int EEV2_STRING_MAX_REEFER_PLUGCD = 3+1;

        private const int EEV2_BLOCKMAP_LIST_MAX = 100; //
        private const int EEV2_BLOCK_BAY_MAX	= 150;
        private const int EEV2_BLOCK_ROW_MAX = 10;
        private const int EEV2_BLOCK_TIER_MAX = 10;

        private const int EEV2_STRING_MAX_USER_ID = 7;
        private const int EEV2_STRING_MAX_USER_PW = 7;
        private const int EEV2_STRING_MAX_USER_NAME = 20;
        private const int EEV2_STRING_MAX_USER_GROUP = 50;
        private const int EEV2_STRING_MAX_USER_STATE	= 10;


        private const int EEV2_STRING_MAX_CONFIGURE_DESC = 10;
        private const int EEV2_STRING_MAX_CONFIGURE_GROUP_NAME = 50;

        private const int EEV2_STRING_MAX_INVEN_INOUT = 4;
        private const int EEV2_STRING_MAX_INVEN_REASON = 10;

        private const int EEV2_STRING_MAX_ORPHAN_REASON_DESC = 50;
        private const int EEV2_STRING_MAX_ORPHAN_ZONE = 10;

        private const int EEV2_STRING_MAX_ATTR_NAME = 20;
        private const int EEV2_STRING_MAX_ATTR_ENTERANCE = 10;
        private const int EEV2_STRING_MAX_ATTR_LANE_TYPE	= 10;
        private const int EEV2_STRING_MAX_ATTR_LANE_NAME = 10;

        private const int EEV2_STRING_MAX_PORT_CODE = 10;
        private const int EEV2_STRING_MAX_RFID_READER_ID = EEV2_STRING_MAX_MACHINE_ID;
        private const int EEV2_STRING_MAX_RFID_READER_TP = 10;

        private const int EEV2_STRING_MAX_ZONE_ID = 10;
        // YYYYMMDDhhmmss
        private const int EEV2_STRING_MAX_TIME = 15;

        private const int EEV2_STRING_MAX_ALERT_TP = 10;
        private const int EEV2_STRING_MAX_ALERT_DESC = 100;
        private const int EEV2_STRING_MAX_ALERT_ARG = 30;

        private const int EEV2_STRING_MAX_NOTICE_MSG = 300;

        private const int EEV2_STRING_MAX_AREA_TYPE = 10;
        private const int EEV2_STRING_MAX_AREA_BLOCK = 10;
        private const int EEV2_STRING_MAX_AREA_FROM = 10;
        private const int EEV2_STRING_MAX_AREA_TO = 10;

        private const int TIER_ROW_MAX = 49;
        private const int COUNT_MAX_DAMAGE = 10;
        private const int COUNT_MAX_SEAL = 4;
        private const int COUNT_MAX_IMDG = 10;

        #endregion [ Constant Defninition ]

        #region [VMT_DataMgr Marshalling Interface ]

        public delegate int CallBack_GetCurrentTime([MarshalAs(UnmanagedType.LPTStr)] string strTime);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct Member
        {
            public int a;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public String mb;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sKAP_TestData
        {
            //----------------Postamble
            public byte boolMember;
            public double doubleMember;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public Member[] member;

            public byte m_btPostamble;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public String m_szMachinName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public String m_cTrolleyPosition;
            public Int32 m_nInstanceID;
            public UInt32 m_dwDataSize;
            public UInt32 m_ulTime;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string m_szDLLName;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CallBack_GetCurrentTime m_pfnGetGPSStatus;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SaveXML
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool AutoSeach1;     // xml com port 첫번째 
            public int Port1;
            [MarshalAs(UnmanagedType.I1)]
            public bool AutoSeach2;    // xml com port 두번째
            public int Port2;
        };


        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Callback_GetCurrentTime_test([MarshalAs(UnmanagedType.LPWStr, SizeConst = 10)] [In][Out] String value);

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        extern static public bool SetCVMT_TestData([In][Out] ref sKAP_TestData data);

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetCVMT_TestData();

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTStringPointer();

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetStringPointer();

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartCallbackThread();

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopCallbackThread();

        [DllImport("VMT_DataMgr.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StructureArgTest([In][Out] ref SaveXML value);


        public static UInt64 GetCurrentTime(int value)
        {
            DateTime firstDate = new DateTime(2013, 01, 14);
            DateTime theDate = DateTime.Now;

            TimeSpan dateDiff;
            dateDiff = theDate.Subtract(firstDate);

            return (UInt64)dateDiff.TotalSeconds;
        }

        public static VMT_DataMgr.CallBack_GetCurrentTime static_Callback_CurrentTime;



        #endregion [VMT_DataMgr Marshalling Interface ]



        #region [EagleEye Import Structure for ITV]

        //------------------------------------------
        // EAHS_VMT_DRIVER_INFO
        //------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_NoticeMessage
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String strMessage;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct Available
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String ReasonCd;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public String RreasonNm;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_AvailableUI
        {
            public float fLimitSpeed;
            public int iFuelGage;
            public int iAvailableCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public Available[] AData;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_AvailableSend
        {
            public Available Data;

            public long StartTime;  // Nov 30 18:10:16 2013 형식으로 나옴
            public long FinishTime; //

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Yard_Location
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String blck;
            /*
            "Block Name 
        e.g) 1A, 2A, 1B, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String bay;
            /*
            "Bay Name
        e.g) 1, 2, 3, 4, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String row;
            /*
            "Row Name
        e.g) A, B, C, D, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String tier;
            /*
            "Tier Name
        e.g) 1, 2, 3, 4, …."
            */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_TC_Container
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
            public String cntrNo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String cntrIso;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String cntrTp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String cls;//수출 수입
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String opr; //e.g) HJS
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String cntrCgoTp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String fullMty;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public String cntrGrade;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Yard_Inven
        {
            EEv2_Yard_Location loc;
            EEv2_TC_Container cntr;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_DriverInfoRq
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
            public String UserID; //6개 입력 받지만 문자열 끝 고려해 +1 – lion_131126
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
            public String UserPW; //6개 입력 받지만 문자열 끝 고려해 +1 – lion_131126

        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_DriverInfoRp
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String MchnID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String DriverName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public String TeamName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
            public String Notice;
        };


        //-------------------------------------------
        // EAHS_VMT_LOGIN
        //-------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_LogInRq
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
            public String UserID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
            public String UserPW;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String DriverName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public String TeamName;


        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_LogInRp
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String MchnID;		// 결과 머신 아이디	
            public byte bLogin;			// TRUE: 로그인 성공 , FALSE : 실패
        };


        //---------------------------------------------
        // EAHS_VMT_MANUAL_ARRIVAL 
        // OnVMTClient_ManualArrival()
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ManualArrivalRq
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String WorkingMchnID;		// 결과 머신 아이디	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String PartnerMchnID;

            //SVMT_ManualArrivalInfo()
            //{
            //    memset(&WorkingMchnID, 0, sizeof(TCHAR)*STRING_MAX_MACHINE_ID);
            //    memset(&PartnerMchnID, 0, sizeof(TCHAR)*STRING_MAX_MACHINE_ID);
            //}
        };

        ////-----------------------------------------------
        //// EAHS_VMT_SHUTDOWN
        //// OnVMTClient_ManualReady()
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ManualArrivalRp
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String MchnID;		// 결과 머신 아이디	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String PowID;
        };

        ////-----------------------------------------------
        //// EAHS_VMT_SHUTDOWN
        //// OnVMTClient_ShutDown()
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ManualReadyRq
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String WorkingMchnID;		// 결과 머신 아이디	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String PartnerMchnID;

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ManualReadyRp
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String WorkingMchnID;		// 결과 머신 아이디	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String ReadyMchnID;

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ShutDown
        {

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_BlockInfoRq
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String BlockName;
            public Byte BayNumber;
        };

        //-------------------------------------------------
        // EAHS_VMT_CHASSIS_ATTACH
        // OnVMTClient_ChassisAttach()
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ChassisAttachInfo
        {
            enum ChassisType
            {
                None = 0,
                Foot20,
                Foot40,
                GooseNeck,
                Special
            };
            public int nType;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public String ChassisNumber;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Job_Container
        {

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
            public String cntrNo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String cntrIso;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String cntrTp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String cls;//수출 수입
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String opr; //e.g) HJS
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String cntrCgoTp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String fullMty;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Job_Machine
        {
            //머신 아이디
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String mchnId;
            /*
            "Machine Code
         e.g) TT501, RS202,QC103"
            */
            //머신 타입
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String mchnTp;
            /*
            "Machine Type
         e.g)  RS : Reach Stacker 
               TC : Transfer Crane (JAT2 AMGC)(NCT RTG)
               TH : Top Handler
               QC : Quay Crane (STS)
               YT : Internal Terminal Truck( Yard Tractor)" 
            */
            //장비 상태
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String mchnSts;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String vrtlFlg; //Virtual Crane Flag "Y",""
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Job_Location
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String locTp;
            /*
            enum enType- 다 대문자임
            {
                "Vessel",
                "Yard",
                "Rail",
                "TPL",
                "TPW",
                "IP",
                "Lane"
            };
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String blck;
            /*
            "Block Name 
        e.g) 1A, 2A, 1B, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String bay;
            /*
            "Bay Name
        e.g) 1, 2, 3, 4, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String row;
            /*
            "Row Name
        e.g) A, B, C, D, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String tier;
            /*
            "Tier Name
        e.g) 1, 2, 3, 4, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String lane;
            /* Lane Number
        e.g) 1, 2, 3, 4, …."
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public String location;
            /*
            "Location                                      if Lane Type
        e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
            */
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Job_Type
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String jobTp;
            /*
             * DS : Discharge
               LD : Load
               MI : Movement In Yard
               MO : Movement Out
               RH : Rehandleing
               AH : Auto Rehandleing
               LC : Loading Cancel
               GI : Gate In
               GO : Gate Out
               GC : Gate Out Cancel"
             * 
             * 1. DS+STS = 빈차
             * 2. DS+RTG = 상차
             * 3. LD+STS = 상차
             * 4. LD+RTG = 빈차
             * 5. MO+RTG = 빈차
             * 6. MI+RTG = 상차
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
            public String jobStatus;//2
            //Active = "A", Inactive="Q", Processing = "P", Completed = "C", ByPass = "B"
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String vslCd;//10
            //<vslCd>HJLN</vslCd> // DS,LOAD 때만 기입
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public String voyNo;//16
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String planSeq;//16
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String twinTandemFlg;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public String twinTandumKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String tandemJoinYT;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String jobFlagInfo; // F, A
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public String conePlan; // Y, N, Null
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2JobOrder
        {
            public EEv2_Job_Machine workingMchn;
            public EEv2_Job_Machine partnerMchn;
            public EEv2_Job_Container cntr;
            public EEv2_Job_Location loc;
            public EEv2_Job_Type type;
        };

        // tslee
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2JobOrderForITV
        {
            public EEv2JobOrder firstJob;
	        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public String        firstJobStatus; // A, R, I
            public int          firstJobETA;

            public EEv2JobOrder secondJob;
	        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public String        secondJobStatus;	       
            public int          secondJobETA;

            public int kpiInfo1;
            public int kpiInfo2;
            public int kpiInfo3;
            public int kpiInfo4;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_SaveIni
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String MchnID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String MchnType;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct Calibration
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public String Pulse;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public String Gyro;
            public float Temp;
            public Char SpeedPulse;
            public Char GyroSF;
            public Char GyroBias;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_SaveXML
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool AutoSeach1;     // xml com port 첫번째 
            public int Port1;
            [MarshalAs(UnmanagedType.U1)]
            public bool AutoSeach2;    // xml com port 두번째
            public int Port2;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sITV_PDS_Periodic_Payload_Recv
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] m_cUTCTime;           //hhmmssss format
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] m_cLatitude;         //37.123456 => "37123456", -128.123456 => “-128.123456
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] m_cLongitude;        //128.123456 => "128123456", -128.123456 => “-128.123456”
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cHeadingDegree;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cSpeedOver;

            public char m_cForwardBackward;      //“F” = forward, “B” = backward
            byte m_BShockSensor;          //0x00 = Run, 0x01 = Break Down
            byte m_BAccelerator;          //0x00 = Run, 0x01 = Break Down
            byte m_BChassisSensor;        //0x00 = Run, 0x01 = Break Down
            byte m_BFuelGage;             //0x00 = Run, 0x01 = Break Down
            byte m_BCollisionDetection;   //0x00 = Run, 0x01 = Break Down
            byte m_BTirePressure;         //0x00 = Run, 0x01 = Break Down
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            byte[] m_BContainerConcheck; //1st  byte = Container, 2nd byte = Cone
            byte m_BDGPS_INS;             //0x00 = Run, 0x01 = Break Down
            byte m_BOdometerPulse;        //0x00 = Run, 0x01 = Break Down

            //----------------Postamble
            byte m_cPostamble;
        };

        // tslee
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct SVMT_ControlMessage
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public String topMessage;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public String bottomMessage;
        };

        #endregion [EagleEye Import Structure for ITV]


        #region [EagleEye Import Structure for RMG]

        //--------------------------------------------------------------------------------
        //
        //               RMG Payload   -   PDS (Serial)
        //
        //--------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------
        // pds -> ee.c  Periodic (Periodic에 DGPS미포함) PACKET_KAP_RMG_PDS_Periodic = 900
        //
        // PDS 에서 EE.VMT 로 들어오는 주기적인 데이터 (위경도 값이 없는 구조체)
        //---------------------------------------------------------------------------------
       [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
       [Serializable]
        public struct sRMG_PDS_Periodic_Payload_Recv1
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] m_cTimeInstance;           // DateTime Format ex) YYYYMMDDHHMMSS
            public byte m_cGantryMoveOnOff;              // On-off signal for RMG is in Gantry motion or not => "0" = Not Moving, "1" = Moving
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] m_cDriveDirectionDegree;      // 0 degree = forward, 180 degree = backward, based on the RMG’s directional definition
            public byte m_cAntiCollisionDetectionSignal; // "0"= Nothing, "1" = stop RMG cause Anti-collision Device alarm
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cTrolleyPosition;           // Actual Trolley Load Position, mm unit
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cHoistPosition;             // Actual height of spreader, measured from E-house side rail to bottom of spreader surface
            // (Bottom of Spreader), mm unit
            public byte m_cRFIDStatus;                   // “1” = Run, “3” = Break Down
            public byte m_cFuelGage;                     // “1” = Run, “3” = Break Down
            public byte m_cTirePressurRMGeck;            // “1” = Run, “3” = Break Down

            //----------------Postamble
            public byte m_cPostamble;
        };


        //------------------------------------------------------------------------------------
        // pds -> ee.c  Periodic (Periodic에 DGPS포함시)  PACKET_KAP_RMG_PDS_Periodic = 900
        //
        // PDS 에서 EE.VMT 로 들어오는 주기적인 데이터 ( DGPS로 부터 신호를 받는다. 위경도 있음)
        //------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sRMG_PDS_Periodic_Payload_Recv2
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] m_cTimeInstance;             // DateTime Format ex) YYYYMMDDHHMMSS
            public byte m_cGantryMoveOnOff;              // On-off signal for RMG is in Gantry motion or not => "0" = Not Moving, "1" = Moving
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] m_cDriveDirectionDegree;      // 0 degree = forward, 180 degree = backward, based on the RMG’s directional definition
            public byte m_cAntiCollisionDetectionSignal; // "0"= Nothing, "1" = stop RMG cause Anti-collision Device alarm
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] m_cLatitude;                 // Latitude
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] m_cLongitude;                // Longitude
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cTrolleyPosition;           // Actual Trolley Load Position, mm unit
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cHoistPosition;             // Actual height of spreader, measured from E-house side rail to bottom of spreader surface
            // (Bottom of Spreader), mm unit
            public byte m_cRFIDStatus;                   // “1” = Run, “3” = Break Down
            public byte m_cFuelGage;                     // “1” = Run, “3” = Break Down
            public byte m_cTirePressurRMGeck;            // “1” = Run, “3” = Break Down

            //----------------Postamble
            public byte m_cPostamble;
        };


        //------------------------------------------------------------------------------
        // pds -> ee.c PickDrop (Twist/lockunlock)  PACKET_KAP_RMG_PDS_PickDrop = 901,
        //
        // PDS 에서 EE.VMT로 들어오는 픽/드랍 이벤트성 데이터
        //------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sRMG_PDS_PickDrop_Payload_Recv
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] m_cTimeInstance;   // DateTime Format ex) YYYYMMDDHHMMSS
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cTrolleyPosition; // Actual Trolley Load Position, mm unit
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cHoistPosition;   // Actual height of spreader, measured from E-house side rail to bottom of spreader surface
            public byte m_cOperationType;      // "0" = Null, "1" = 20ft, "2"=20ft_Twin,"3" = 40ft, "4" = 45ft, "5"=48ft, "8" = None containerized operation, "E"=Error
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] m_cBlock;           // Block Name, ex)"1A","2B"," etc.. 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] m_cBay;             // Row Name  ex) "00","01, etc..." 
            public byte m_cRow;                // Bay Name, Alphabet 1 Character (A~Z) 
            public byte m_cTier;               // Tier Name, 1 Digit place ex) "1" ,"2" 
            public byte m_cTwistLockUnlock;    // "1" = Lock(Closed),   "2" = Unlock(Open), "E" = Error
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] m_cContainerWeight; // Kilograms with leading zeros 	

            //----------------Postamble
            public byte m_cPostamble;
        };

        //---------------------------------------------------------------------------------------
        // pds -> ee.c Tire Pressure / Fuel Gage Event Data   PACKET_KAP_RMG_PDS_TireFuel = 903
        //
        // PDS 에서 EE.VMT로 들오는 타이어 공기압/ 연료량 에 대한 이벤트 데이터
        //---------------------------------------------------------------------------------------
       [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
       [Serializable]
        public struct sRMG_PDS_TireFuel_Payload_Recv
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] m_cFuelGage;         // LiterUnit
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] m_cTireRessurRMGeck; // PSI Unit

            //----------------Postamble
            public byte m_cPostamble;
        };


        //----------------------------------------------------------------------------
        // pds -> ee.c  PACKET_KAP_RMG_PDS_RFID     = 902
        //
        // PDS 에서 EE.VMT 로 들어오는 RFID 이벤트성 데이터
        //----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sRMG_PDS_RFID_Payload_Recv
        {
            //----------------PayLoad 77
            public byte m_cAntennaID; //
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] m_cTagID;  // 15자리로 늘리자고 함 대기중
            public byte m_cFlag;      //

            //----------------Postamble
            public byte m_cPostamble;
        };


        //--------------------------------------------------------------------
        //
        //               RMG Payload   -   CPS (TCP/IP)
        //
        //--------------------------------------------------------------------

        //---------------------------------------------------------------------
        // ee.c -> CPS    Trigger 
        //
        // EE.VMT 에서 CPS 로 보내는 트리거 데이터
        //---------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sRMG_CPS_Trigger_Payload_Send
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] m_cTimeInstance; //DateTime Format ex) YYYYMMDDHHMMSS
            public byte m_cGantryMoveOnOff;  //On-off signal for RMG is in Gantry motion or not => "0" = Not Moving, "1" = Moving
            public byte m_cOperationType;    //"0" = Null, "1" = 20ft, "2"=20ft_Twin,"3" = 40ft, "4" = 45ft, "5"=48ft, "8" = None containerized operation, "E"=Error
            public byte m_cForeAfter;        //“0”= Null, “1”=Fore., “2” = Aft, “E”=error
            public byte m_cJobType;          //“D” : Yard Out , “L” : Yard In

            //----------------Postamble
            public byte m_cPostamble;
        };


        //------------------------------------------------------------------
        // CPS -> ee.c   Alignment Event Message PAYLOAD 
        //
        // CPS 에서 EE.VMT 로 보내는 결과 데이터  
        //------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sRMG_CPS_Alignment_Payload_Recv
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] m_cTimeInstance; //DateTime Format ex) YYYYMMDDHHMMSS
            public byte m_cDirection;        //"0”=Empty,(비어있음) , “1” = Normal Direction(정방향 접근), 
            //“2”= Reverse Direction (역방향 접근), “3” =Full(차량이 존재함), “E”=Error

            public byte m_cAlignmentResult;  //"0”=Empty, “1” = Completed, ”2”=Processing, “3” = Detected, “4”= Passed, “E”=Error
            //	(Completed : 완료, Processing : Alignment 진행 중, Detected : 차량진입발견, Passed = 발견된 차량이 지나간 경우)
            public byte m_cForeAfter;        //“0”= Null, “1”=Fore., “2” = Aft, “E”=error
            public byte m_cJobType;          //“D” : Yard Out , “L” : Yard In


            //----------------Postamble
            public byte m_cPostamble;
        };


        //--------------------------------------------------------------------------------------
        //
        //         RMG Payload   -   DGPS (Serial)  PACKET_KAP_RMG_DGPS_Periodic = 950,
        //
        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //   DGPS -> ee.c  DGPS Message PAYLOAD 
        //
        //   DGPS 로 부터 받은정보를 담는 구조체  
        //
        //--------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sRMG_DGPS_Payload_Recv
        {
            //----------------PayLoad 77
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] m_cLatitude;   //37.123456 => "37123456", -128.123456 => “-128.123456
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] m_cLongitude;  //128.123456 => "128123456", -128.123456 => “-128.123456”

            //----------------Postamble
            public byte m_cPostamble;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Location
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_TYPE)]
	        public String locTp;
	        /*
	        enum enType- 다 대문자임
	        {
		        "Vessel",
		        "Yard",
		        "Rail",
		        "TP",
		        "IP",
		        "Lane"
	        };
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_BLOCK)]
	        public String blck;		
	        /*
	        "Block Name 
        e.g) 1A, 2A, 1B, …."
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_BAY)]
	        public String bay;
	        /*
	        "Bay Name
        e.g) 1, 2, 3, 4, …."
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_ROW)]
	        public String row;
	        /*
	        "Row Name
        e.g) A, B, C, D, …."
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_TIER)]
	        public String tier;
	        /*
	        "Tier Name
        e.g) 1, 2, 3, 4, …."
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_LANE)]
	        public String lane;
	        //W : Water-side //L :Land-side
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_LOCATION)]
	        public String location;
	        /*
	        "Location                                      if Lane Type
        e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
	        */

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Reefer
        {

	        public float reeferTemp;		//온도
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_REEFER_PLUGCD)]
	        public String plugCd;
	        /*
	        "status of Plug
	        PIW : Wait for Plug-In
	        PIM : Monitoring of Plug-In
	        POW : Wait for Plug-Out
	        POC :  Completed of Plug-Out"
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
	        public String unit;
	        /*
	        "unit of temperature
	        F : Fahrenheit
	        C : Celsius "
	        */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Damage
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_DMG_CD)]
	        public String dmgCd;//
	        /*
	        "Damage Code
        e.g) AS :  ALL SNOW
              PU : PUSHED
              BM : Band Missing"

	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_DMG_INOUT)]
	        public String dmgInOut; //Inside / Outside
	        /*
	        "Inside / Outside
        e.g) I : Inside, O : Outside"

	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_DMG_PART)]
            public String dmgPart;//Damage part
	        /*
	        "Damage part
        e.g) F : Fore, R : Rear, L : Left, R : Right, T : Top, B : BottomR)"

	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_DMG_RANGE)]
	        public String dmgRange;//Damage Range
	        /*
	        "Damage Range
        e.g) 10*10*10"

	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_DMG_DESC)]
	        public String dmgDesc;//ISO Code

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Seal
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_SEAL_SEALNO)]
	        public String sealNo;//IMDG
	        /*
	        "Seal No
        e.g.) 12311231231"

	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_SEAL_TYPE)]
	        public String sealTp;
	        /*
	        "C : Custom
	        O : Operator"

	        */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Imdg
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_IMDG_IMDG)]
	        public String imdg;//IMDG
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_IMDG_UNNO)]
	        public String unNo;//UNNO
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_IMDG_FIRECODE)]
	        public String fireCd;//Fire Code
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Container
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_NO)]
	        public String cntrNo;
	        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_ISO)]
	        public String cntrIso; 

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_TYPE)]
	        public String cntrTp;
	        /*
	        "Container Type
	        GE : General
	        RF :  Reefer
	        TK : Tank
	        FR : Flat Rack
	        OT : Open Top
	        BK : Dry Bulk
	        AS : Air Surface"
	        */

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_LENGTH)]
	        public String cntrLen;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_HEIGHT)]
	        public String cntrHgt; 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_OPRCODE)]
	        public String opr; //e.g) HJS
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_WEIGHT)]
	        public String cntrWgt;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_CLASS)]
	        public String cls;//수출 수입
	        /*
	        "Class : Import/Export
	        II: Import
	        TI: Trashipment Import
	        XX: Export
	        TX : Transhipment Export
	        TL: Transshipment
	        S1: Shift 1Time
	        S2: Shift 2Time
	        YY: Storage Empty
	        OT : Through Cargo"
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_STYPE_DESC)]
	        public String cntrSpTp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_CARGO_TYPE)]
	        public String cntrCgoTp;
	        /*
	        "Cargo Type
         G : General
         M: Empty
         MH : Empty Hazardous
         R : Reefer
         RH : Reefer Hazardous
         H : Hazardous"
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_FULL_MTY)]
	        public String fullMty;
	        /*
	        "F: Full
        M: Empty"
	        */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_PORT_OF_DISCHARGE)]
	        public String pod;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_NEXT_PORT)]
	        public String nPod;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_PORT_OF_LOAD)]
	        public String pol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_FINAL_PORT)]
	        public String fPol;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_GRADE)]
	        public String cntrGrade;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_CNTR_DOOR_DIRECTION)]
	        public String doorDirect;
	        /*
	        Fore:F
	        After:A
	        */
            public Int32 isDmg;
            public Int32 isSeal;
            public Int32 isHold;
            public EEv2_Def_Reefer reefer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = COUNT_MAX_DAMAGE)]
            public EEv2_Def_Damage[] dmg;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = COUNT_MAX_SEAL)]
            public EEv2_Def_Seal[] seal;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = COUNT_MAX_IMDG)]
            public EEv2_Def_Imdg[] imdg;

        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct EEv2_Def_Inventory
        {
	        public EEv2_Def_Container		cntr;
	        public EEv2_Def_Location		loc;
	        //1.4.1 구조
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_INVEN_INOUT)]
	        public String inOut; //In/Out
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_INVEN_REASON)]
	        public String reason; //reason


	        public Int32				isTOSData; //TOS가 관리한
	        public Int32				mgrEE; //EagleEye가 관리한 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEV2_STRING_MAX_LOCATION_TYPE)]
	        public String		InvenTp; // INV,NWT,NWA,TNL,VRT
        }
	

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sBlockBayInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = TIER_ROW_MAX)]
	        public byte[] BlockBay;
	        public byte cntrCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 49)]
            public EEv2_Def_Inventory[] cntr;

        };

        #endregion [EagleEye Import Structure for RMG]


        #region [EagleEye Import API for ITV]

        //--------------------------------------------------------------------------
        //Import form dll : dll에서 외부로 노출된 함수
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CreateVMTClient();                 // 시작할때 호출
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyVMTClient();                // 종료할때 호출
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVMTType();

        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetSoftwareInfo([In][Out] ref int value);           // 소프트웨어 버전 정보를 얻는다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMchnID2([MarshalAs(UnmanagedType.LPWStr)][In][Out] String pValue, [In][Out] ref int value);           // 머신 아이디는 20자 고정.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetJobList();                      // 잡요청함수: 잡을 요청하면 서버에서 Callback_GetJobList로 값이 들어옴.

        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendDriverInfo([In][Out] ref SVMT_DriverInfoRq value);          // 드라이버 정보를 서버로 전송합니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendLogIn([In][Out] ref SVMT_LogInRq value);                    // 로그인 정보를 서버로 전송합니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendChassisInfo([In][Out] ref SVMT_ChassisAttachInfo value);    // 샤시 정보를 서버로 전송합니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendManualArrival([In][Out] ref SVMT_ManualArrivalRq value);  // 메뉴얼 얼라이벌 신호를 서버로 전송합니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendManualReady([In][Out] ref SVMT_ManualReadyRq value);      // 메뉴얼 레디 신호를 서버로 전송합니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendSystemOff([In][Out] ref SVMT_ShutDown value);               // 시스템 종료 신호를 서버로 전송합니다.

        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendAvailalbe([In][Out] ref SVMT_AvailableSend value);          // Available 정보를 서버로 샌드
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SaveIniFile([In][Out] ref SVMT_SaveIni value);// JAT2_eevmt.ini 파일의 머신아이디, 머신타입 변경및저장 함수
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SaveXMLFile([In][Out] ref SVMT_SaveXML value);                  // KAP_eevmt_config.xml 파일의 AutoSeach, Port 변경및저장 함수
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetXMLPortNumberFirst_ITV([In][Out] ref int value);                    // 현재 xml에 설정된 DGPS 포트번호 설명
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetXMLPortNumberSecond_ITV([In][Out] ref int value);                  // 현재 xml에 설정된 SENSOR 포트번호 설명


        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Send_CFG_RST();                                   // DGPS Reset  요청 명령
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
	    public static extern void Send_PUBX_05();                                   // PUBX05 신호 요청 명령  
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
	    public static extern void Send_PUBX_06();                                   // PUBX06 신호 요청 명령 
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Send_HighBackward();                            // HighBackward 신호 보내기
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Send_HighForward();                               // HighForward 신호 보내기

        // tslee
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Request_CFG_EKF();  


        //-------------------------------------------------------------------------
        //- Callback Functions
        //-------------------------------------------------------------------------
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_Report([MarshalAs(UnmanagedType.LPStr, SizeConst = 150)]String str);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_Report(Callback_Report fp);

        // 지피에스 상태를 통지 받습니다.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyGPSStatus(int value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyGPSStatus(Callback_NotifyGPSStatus fp);

        // 와이파이 상태를 통지 받습니다.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyWIFIStatus(int value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyWIFIStatus(Callback_NotifyWIFIStatus fp);

        // 엔진온도를 통지 받습니다.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyEngineTemp(int value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyEngineTemp(Callback_NotifyEngineTemp fp);

        // 연료상태를 통지 받습니다.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyFuelGage(int value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyFuelGage(Callback_NotifyFuelGage fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifySpeedKm(float value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]                   // 속도 를 통지 받습니다.
        public static extern void SetCallBack_NotifySpeedKm(Callback_NotifySpeedKm fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyDriverInfo([In][Out] ref SVMT_DriverInfoRp value);   // 드라이정보 요청 결과를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyDriverInfo([In][Out] Callback_NotifyDriverInfo fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyLogIn([In][Out] ref SVMT_LogInRp value);             // 로그인요청 결과를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyLoginInfo([In][Out]Callback_NotifyLogIn fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyJobOrderITV([In][Out] ref EEv2JobOrderForITV value);    // ITV 잡정보를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]               // 요청한 잡정보를 통지받습니다.
        public static extern void SetCallBack_NotifyJobOrderITV(Callback_NotifyJobOrderITV fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyJobDeleteAll(int value);                // 잡딜리트 올 메시지를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]               // 요청한 잡정보를 통지받습니다.
        public static extern void SetCallBack_NotifyJobDeleteAll(Callback_NotifyJobDeleteAll fp);

        // 잡을 갯수만큼 여러번 넘김
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyJobOrder([In][Out] ref EEv2JobOrder value);               // 요청한 잡정보를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]               // 요청한 잡정보를 통지받습니다.
        public static extern void SetCallBack_NotifyJobOrder([In][Out]Callback_NotifyJobOrder fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyJobChange([In][Out] ref EEv2JobOrder value);              // 잡체인지 메시지를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyJobChange(Callback_NotifyJobChange fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyJobDelete([In][Out] ref EEv2JobOrder value);    // 잡딜리트 메시지를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]  // 잡딜리트 메시지를 통지받습니다.                
        public static extern void SetCallBack_NotifyJobDelete(Callback_NotifyJobDelete fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyJobDone([In][Out] ref EEv2JobOrder value);         // 잡돈 메시지를 통지받습니다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]         // 잡돈 메시지를 통지받습니다.
        public static extern void SetCallBack_NotifyJobDone([In][Out]Callback_NotifyJobDone fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyArrival([In][Out] ref SVMT_ManualArrivalRp value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] // 얼라이벌 신호를 통지받습니다.
        public static extern void SetCallBack_NotifyArrival([In][Out]Callback_NotifyArrival fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyReady([In][Out] ref SVMT_ManualReadyRp value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]     // 레디 신호를 통지받습니다.
        public static extern void SetCallBack_NotifyReady([In][Out]Callback_NotifyReady fp);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyTosMessage([In][Out] ref SVMT_NoticeMessage value);  // 서버 메시지를 통지받는다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]     // 레디 신호를 통지받습니다.
        public static extern void SetCallBack_NotifyTosMessage(Callback_NotifyTosMessage fp);

        //----------------------------------------------------------------------------
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyVtMessage([In][Out] ref SVMT_NoticeMessage value);  // 버쳘 터미널 메시지를 통지받는다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]     // 레디 신호를 통지받습니다.
        public static extern void SetCallBack_NotifyVtMessage(Callback_NotifyVtMessage fp);

        //----------------------------------------------------------------------------
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyAvailable([In][Out] ref SVMT_AvailableUI value);    // Available 종류에 대한 메시지를 통지 받는다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]     // 레디 신호를 통지받습니다.
        public static extern void SetCallBack_NotifyAvailable(Callback_NotifyAvailable fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyErrorCode([MarshalAs(UnmanagedType.LPWStr)]String value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]         // 서버와의 통신 에러 메시지 내용.
        public static extern void SetCallBack_NotifyErrorCode(Callback_NotifyErrorCode fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyCalibration([In][Out] ref Calibration value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]         // 서버와의 통신 에러 메시지 내용.
        public static extern void SetCallBack_NotifyCalibration(Callback_NotifyCalibration fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_ITV_Periodic([In][Out] ref sITV_PDS_Periodic_Payload_Recv value);     // ITV 주기적 데이터 (속도)
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_ITVPeriodic(Callback_ITV_Periodic fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyPowOut(int value);                //Pow Out 을 UI로 올린다.
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]               // 요청한 잡정보를 통지받습니다.
        public static extern void SetCallBack_NotifyPowOut(Callback_NotifyPowOut fp);

        // [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //    public delegate void Callback_ITV_ShockSensor(sITV_PDS_ShockSensor_Payload_Recv value); // ITV 충격센서
        //DLL_IMPORT void SetCallBack_ITVShockSensor(Callback_ITV_ShockSensor fp);

        ////-------------------------------------------------------------------------------------------
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //    public delegate void Callback_ITV_Accelerator(sITV_PDS_Accelerator_Payload_Recv value); // ITV 가속센서
        //DLL_IMPORT void SetCallBack_ITVAccelerator(Callback_ITV_Accelerator fp);

        // tslee
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyControlMessage([In][Out] ref SVMT_ControlMessage value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]               // 요청한 잡정보를 통지받습니다.
        public static extern void SetCallBack_NotifyControlMessage(Callback_NotifyControlMessage fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyCFGEKF(int value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]               // 요청한 잡정보를 통지받습니다.
        public static extern void SetCallBack_NotifyCFGEKF(Callback_NotifyCFGEKF fp);


        public static VMT_DataMgr.Callback_Report static_Report;
        public static VMT_DataMgr.Callback_NotifyGPSStatus static_NotifyGPSStatus;
        public static VMT_DataMgr.Callback_NotifyWIFIStatus static_NotifyWIFIStatus;
        public static VMT_DataMgr.Callback_NotifyEngineTemp static_NotifyEngineTemp;
        public static VMT_DataMgr.Callback_NotifyFuelGage static_NotifyFuelGage;
        public static VMT_DataMgr.Callback_NotifySpeedKm static_NotifySpeedKm;
        public static VMT_DataMgr.Callback_NotifyDriverInfo static_NotifyDriverInfo;
        public static VMT_DataMgr.Callback_NotifyLogIn static_NotifyLogIn;
        public static VMT_DataMgr.Callback_NotifyJobOrderITV static_NotifyJobOrderITV;
        public static VMT_DataMgr.Callback_NotifyJobDeleteAll static_NotifyJobDeleteAll;
        public static VMT_DataMgr.Callback_NotifyJobOrder static_NotifyJobOrder;
        public static VMT_DataMgr.Callback_NotifyJobChange static_NotifyJobChange;
        public static VMT_DataMgr.Callback_NotifyJobDelete static_NotifyJobDelete;
        public static VMT_DataMgr.Callback_NotifyJobDone static_NotifyJobDone;
        public static VMT_DataMgr.Callback_NotifyArrival static_NotifyArrival;
        public static VMT_DataMgr.Callback_NotifyReady static_NotifyReady;
        public static VMT_DataMgr.Callback_NotifyTosMessage static_NotifyTosMessage;
        public static VMT_DataMgr.Callback_NotifyVtMessage static_NotifyVtMessage;
        public static VMT_DataMgr.Callback_NotifyAvailable static_NotifyAvailable;
        public static VMT_DataMgr.Callback_NotifyErrorCode static_NotifyErrorCode;
        public static VMT_DataMgr.Callback_NotifyCalibration static_NotifyCalibration;
        public static VMT_DataMgr.Callback_ITV_Periodic static_ITVPeriodic;
        public static VMT_DataMgr.Callback_NotifyPowOut static_NotifyPowOut;
        //public static VMT_DataMgr.Callback_ITV_ShockSensor static_ITVShockSensor;
        //public static VMT_DataMgr.Callback_ITV_Accelerator static_ITVAccelerator;
        public static VMT_DataMgr.Callback_NotifyControlMessage static_NotifyControlMessage;
        public static VMT_DataMgr.Callback_NotifyCFGEKF static_NotifyCFGEKF;


        #endregion [EagleEye Import API for ITV]



        #region [EagleEye Import API for RMG]
        ///////////////////////////////////////////////////////////////////////////
        // RMG
        ///////////////////////////////////////////////////////////////////////////
        

        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendBlockInfo([In][Out] ref SVMT_BlockInfoRq value);            // Block, Bay 정보를 요청합니다.



        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_RMG_Periodic([In][Out] ref sRMG_PDS_Periodic_Payload_Recv2 value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_RMGPeriodic(Callback_RMG_Periodic fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_RMG_PickDrop([In][Out] ref sRMG_PDS_PickDrop_Payload_Recv value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_RMGPickDrop(Callback_RMG_PickDrop fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_RMG_TireFuel([In][Out] ref sRMG_PDS_TireFuel_Payload_Recv value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_RMGTireFuel(Callback_RMG_TireFuel fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_RMG_RFID([In][Out] ref sRMG_PDS_RFID_Payload_Recv value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_RMGRfid(Callback_RMG_RFID fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_RMG_CpsAlign([In][Out] ref sRMG_CPS_Alignment_Payload_Recv value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_RMGCpsAlign(Callback_RMG_CpsAlign fp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback_NotifyBlockInfo([In][Out] IntPtr value);
        [DllImport(MainWindow.VMT_EngineDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallBack_NotifyBlockInfo(Callback_NotifyBlockInfo fp);



        public static VMT_DataMgr.Callback_RMG_Periodic static_RMG_Periodic;
        public static VMT_DataMgr.Callback_RMG_PickDrop static_RMG_PickDrop;
        public static VMT_DataMgr.Callback_RMG_TireFuel static_RMG_TireFuel;
        public static VMT_DataMgr.Callback_RMG_RFID static_RMG_RFID;
        public static VMT_DataMgr.Callback_RMG_CpsAlign static_RMG_CpsAlign;
        public static VMT_DataMgr.Callback_NotifyBlockInfo static_NotifyBlockInfo;


        #endregion [EagleEye Import API for RMG]





         #region [Geometry Manager Import Structure for RMG]

         //--------------------------------------------------------------------------------------
         //
         // For VMT_DataMgr DLL for Geometry Data
         //
         //--------------------------------------------------------------------------------------
         [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
         [Serializable]
         public struct sPixelPoint
         {
             public int x;
             public int y;
         };

         [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
         [Serializable]
         public struct sGeoPoint
         {
             public double lo;
             public double la;
         }

         [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
         [Serializable]
         public struct sBayPow
         {
             [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
             public String m_szID;
             [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
             public String m_szBlock;
             [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
             public String m_szBay;
             [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
             public String m_szBayEst;

             public int m_nRow;

             public sGeoPoint posTL;
             public sGeoPoint posTR;
             public sGeoPoint posBL;
             public sGeoPoint posBR;
         }

         #endregion [Geometry Manager Import Structure for RMG]



        // #region [Geometry Manager Import API for RMG]

        // //----------------------------------------------------------
        // //- Geometry Relation Functions
        // //----------------------------------------------------------
        // [DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern bool InitGeometry();

        // [DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern bool ReleaseGeometry();

        // [DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern IntPtr GetCurrentPos([In][Out] ref double lo, [In][Out] ref double la);

        // [DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern IntPtr SetCurrentPos(double lo, double la);

        // [DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern int GetTotalBlockCount();

        // [DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern int GetTotalBayCount();

        //[DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        // public static extern int GetBayCount([MarshalAs(UnmanagedType.LPTStr)][In][Out] String szBlock);

        //[DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int GetRowCount([MarshalAs(UnmanagedType.LPTStr)][In][Out] String szBay);

        //[DllImport(MainWindow.VMT_DataMgrDLL, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int GetTierCount([MarshalAs(UnmanagedType.LPTStr)][In][Out] String szRow);


         [DllImport("advapi32.dll")]
         public static extern void InitiateSystemShutdown(string lpMachineName, string lpMessage, int dwTimeout, bool bForceAppsClosed, bool bRebootAfterShutdown);

        // #endregion [Geometry Manager Import API for RMG]




         //-----------------------------------------------------------
        //- Data Conversion Static Method
        //-----------------------------------------------------------

        public static string TranscodeByteArrayToString(byte[] btData)
        {
            string strRet = "";

            strRet = Encoding.ASCII.GetString(btData, 0, btData.Length);

            return strRet;
        }

        public static byte[] TranscodeStringToByte(string strData, int padding = 12)
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

        public static string TranscodeByteArrayToHexString(byte[] btData, bool bDash = true)
        {
            string strHex = "";

            strHex = BitConverter.ToString(btData);

            if (!bDash)
                strHex = strHex.Replace("-", string.Empty);

            return strHex;
        }

        public static byte[] TranscodeHEXStringToByteArray(string strHex)
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
            BinaryFormatter s = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                s.Serialize(ms, obj);
                ms.Position = 0;
                T t = (T)s.Deserialize(ms);

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
                msgEle.SetAttribute("parameter", xmlMessage.DocumentElement.Name);
                msgEle.SetAttribute("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                // Import Message XML Nodes for copying to Target XML Documnet
                XmlNode copyNode = xmlDoc.ImportNode(xmlMessage.DocumentElement, true);
                while (copyNode.ChildNodes.Count > 0)
                {
                    msgEle.AppendChild(copyNode.FirstChild);
                }

                // Appending new Message XML to XML Documnet
                xmlDoc.DocumentElement.AppendChild(msgEle);
            }


            return xmlDoc.InnerXml;
        }


    }
}