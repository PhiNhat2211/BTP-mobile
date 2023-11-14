using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm.EEStruct
{
    //########################################################################
    //
    //
    //                                                      Sensing Device
    //
    //
    //########################################################################

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_PDS_Periodic
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sECH_PDS_Periodic_Payload : EEParentClass
    {
        public UInt32 m_dwTime;                            // 시간
        public Double m_dLatitude;                         // 위도
        public Double m_dLongitude;                        // 경도

        public float m_fHeadingDegree;               // 헤딩 정보
        public int m_cTrolleyPosition;               // 6자리 mm Unit 
        public int m_cHoistPosition;                 // 6자리 mm Unit 

        public int m_cGantryMoveOnOff;                // Gantry Move 0 = Not Moving, 1 = Moving  
        public int m_cDriveDirectionDegree;           // 0 degree = forward, 180 degree = backward
        public int m_cAntiCollisionDetectionSignal;   // 0 = Nothing, 1= stop ECH cause Anti-Collision Device alarm

        public int m_cRFIDStatus;                     // RFID 센서 상태 정보
        public int m_cFuelGage;                       // 연료게이지 센서 상태 정보
        public int m_cTirePressureCheck;              // 타이어 압력센서 상태 정보 

        //----sECH_PDS_TireFuel_Payload_Recv
        public int m_cFuelGageLiter;                 // 실제 연료량
        public int m_cTireRessurePSI;                // 실제 타이어 압력

        //---- 접속여부..
        public int m_CPS_Status;                      // 0 = disconnect, 1 = connect, 2 = Error
        public int m_PDS_Status;                      // 0 = disconnect, 1 = connect, 2 = Error  
        public int m_TOSClient_Status;                // 0 = disconnect, 1 = connect, 2 = Error
        public int m_EagleEyeEvent_Status;            // 0 = disconnect, 1 = connect, 2 = Error
        public int m_EagleEyePeriodic_Status;         // 0 = disconnect, 1 = connect, 2 = Error


        public int m_cBlockInOut;                     //Block In = 1, Block Out = 0
        public int m_cBay;                            //Bay Number
        public int m_cRow;                            // 1 = A, 2 = B 3 = C.... 26 = Z 
        public int m_cTier;                           // 1,2,3,4,5,6,7

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public String m_strBlockName;                 //BlockName
    }


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_CPS_Align
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sECH_CPS_Alignment_Payload : EEParentClass
    {
        public UInt32 m_dwTime;
        public Byte m_cAlignmentResult;  // '0'=Empty, '1' = Completed, '2'=Processing, '3' = Detected, '4'= Passed, 'E'=Error
        // (Completed : 완료, Processing : Alignment 진행 중, Detected : 차량진입발견, Passed = 발견된 차량이 지나간 경우)
        public Byte m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
        public Byte m_cDirection;        // '0'=Empty,(비어있음) , '1' = Normal Direction(정방향 접근), 
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_PDS_PickDrop 
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sECH_PDS_PickDrop_Payload : EEParentClass
    {
        public UInt32 m_dwTime;                // 시간
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Byte[] m_cBlock;             // Block Name '1A'

        public int m_cTrolleyPosition;   // 6자리 mm Unit
        public int m_cHoistPosition;     // 6자리 mm Unit

        // '0' = Null, '1' = 20ft, '2'=20ft_Twin,'3' = 40ft, '4' = 45ft,
        // '5'=48ft,   '8' = None containerized operation, '10'=Error
        public int m_cOperationType;

        public int m_cBay;                // 00, 01, ... 256 
        public int m_cRow;                // 1 = A, 2 = B 3 = C.... 26 = Z	
        public int m_cTier;               // 1, 2

        public int m_cTwistLockUnlock;    // 1 = Lock, 2 = Unlock, 3 = Error
        public int m_cContainerWeight;   // 컨테이너 무게Kg
    }

    // Notify   UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sECH_PDS_PickDropConfirm_Payload : EEParentClass
    {
        public sTrayUI_sECH_PDS_PickDrop_Payload m_sPickDrop; //PickDrop정보
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String m_JobKey; //Job키
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TYPE)]
        public String m_locTp;
        /*
            enum enType- 다 대문자임
            {
            "Vessel",
            "Yard",
            "Rail",
            "TP",
            "IP",
            "Lane"
                "XRAY"
            };
        */
        //public EEv2_Job_Location m_JobLocation; //위치정보
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_RFID 
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sECH_PDS_RFID_Payload : EEParentClass
    {
        public UInt32 m_dwTime;                 // 시간 정보
        public Byte m_cAntennaID;             // '1' = 1번, '2' = 2번, 'E' = error or Unknown (번호는 RFID Antenna ID를 따름)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.RFID_TAG_MAX)]
        public Byte[] m_cTagID;   // EPC 64bit ASCII Code String 'NCT00001' => 0x4E0x430x540x300x300x300x300x31 
        //                                             N   C   T   0   0   0   0    1
        public Byte m_cFlag;                  // '1' = Begin, '2' = End
    }

    //########################################################################
    //
    //
    //                                                         ITV Info
    //
    //
    //########################################################################
    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_NotifyMachinePOW
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_POWInfo : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
    //    public String ITVMachineID;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
    //    public String BlockName;
    //    public int iBay;
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public Boolean bPOW; //TRUE in   , FALSE out
    //}

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_NotifyMachineBlockEnter
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_BlockEnteranceITV : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
    //    public String ITVMachineID;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
    //    public String BlockName;
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public Boolean bEnterance; //TRUE in   , FALSE out
    //}

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_NotifyMachineReadyITV
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_ManualReadyITV : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
    //    public String ITVMachineID;
    //}

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_ManualReady
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ManualReadyECH : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String jobKey;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String ITVMachineID;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean bReadyOnOff;//( 1: On, 0 : Off)
    }

    // Ack   Tray -> UI  public class 없음

    //########################################################################
    //
    //
    //                                                         Inventory Correction
    //
    //
    //########################################################################

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_SendBlockInfo
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_BlockInfoRq : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
    //    public String szBlockName;
    //    public Byte bayNo;
    //}
    // Ack   Tray -> UI  public class없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_NotifyBlockInfo
    //                
    //---------------------------------------------------------
    // Notify   Tray  -> UI    
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class EEv2_Def_Inventory : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.Struct)]
    //    public EEv2_Def_Container cntr;
    //    [MarshalAs(UnmanagedType.Struct)]
    //    public EEv2_Def_Location loc;
    //    //1.4.1 구조
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_INVEN_INOUT)]
    //    public String inOut; //In/Out
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_INVEN_REASON)]
    //    public String reason; //reason
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public Boolean isTOSData; //TOS가 관리한
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public Boolean mgrEE; //EagleEye가 관리한  
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TYPE)]
    //    public String InvenTp; // INV,NWT,NWA,TNL,VRT
    //}


    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_sBlockBayInfo_Header : EEParentClass
    //{
    //    public int bay; // 해당 베이 번호
    //    public int row; // row max
    //    public int tier; // tier max
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.TIER_ROW_MAX)]
    //    public Byte[] BlockBay; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
    //    public Byte cntrCount;
    //}

    //Recv 패킷
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_sBlockBayInfo : EEParentClass
    //{
    //    public int bay; // 해당 베이 번호
    //    public int row; // row max
    //    public int tier; // tier max
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.TIER_ROW_MAX)]
    //    public Byte[] BlockBay; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
    //    public Byte cntrCount;
    //    public EEv2_Def_Inventory[] cntr; //--->CntrCount만큼 받음..
    //}


    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_SendBlockInfoSimple
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_BlockInfoRq : EEParentClass
    //{
    //      /*위에 정의*/
    //}
    // Ack   Tray -> UI  public class없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_NotifyBlockInfoSimple
    //                
    //---------------------------------------------------------
    // Notify   Tray  -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class EEv2_Simple_Conatainer : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
    //    public String cntrNo;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_ISO)]
    //    public String cntrIso;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_TYPE)]
    //    public String cntrTp;
    //    public EEv2_Def_Location loc;   //해당컨테이너의 Location 정보
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public Boolean isDmg;
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public Boolean isHold;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_REEFER_PLUGCD)]
    //    public String plugCd;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_FULL_MTY)]
    //    public String fullMty;
    //}

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_sBlockBayInfoSimple_Header : EEParentClass
    //{
    //    public int bay; // 해당 베이 번호
    //    public int row; // row max
    //    public int tier; // tier max
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.TIER_ROW_MAX)]
    //    public Byte[] BlockBay; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
    //    public Byte cntrCount;
    //}

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_sBlockBayInfoSimple : EEParentClass
    //{
    //    public int bay; // 해당 베이 번호
    //    public int row; // row max
    //    public int tier; // tier max
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.TIER_ROW_MAX)]
    //    public Byte[] BlockBay; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
    //    public Byte cntrCount;
    //    public EEv2_Simple_Conatainer[] cntr;  //--->CntrCount만큼 받음..
    //};

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_SendCorrection
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_Correction : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
    //    public String cntrNo;
    //    public EEv2_Yard_Location fromLoc;
    //    public EEv2_Yard_Location toLoc;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
    //    public String actionType;   // C=creation, D=delete, M=move 인벤토리정보(DB) 를 수정하는 형태
    //}
    // Ack   Tray -> UI  public class없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_NotifyCorrection
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    // Notify   Tray  -> UI    
    //public class  EEv2_Def_Inventory : EEParentClass
    //{
    //    /*위에 정의*/
    //}

    //Recv 패킷
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    // public class sTrayUI_sBlockBayInfo : EEParentClass
    // {
    //     /*위에 정의*/
    // }

    //########################################################################
    //
    //
    //                                                         ECH JobOrder
    //
    //
    //########################################################################
    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_SetCurrentJob
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_SetCurrentJob : EEParentClass // Delete
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
    //    public String jobKey;
    //}
    // Ack   Tray -> UI  public class없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_HandleJobDone
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_HandleJobDone : EEParentClass // Delete
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
    //    public String jobKey;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
    //    public String WorkingMachineID;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
    //    public String WorkingMachineTP;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
    //    public String PartnerMachineID;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
    //    public String PartnerMachineITP;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
    //    public String cntrNo;
    //    public EEv2_Job_Location Loc;
    //    public EEv3_Spreader sprd;
    //}
    // Ack   Tray -> UI  public class없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_TargetJob
    //                
    //---------------------------------------------------------
    // Send   Tray -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_TargetJob : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
    //    public String jobKey;
    //}
    // Ack   UI  -> Tray public class없음

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_ManualTargetJob : EEParentClass
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
    //    public String m_PreTargetJobKey;   //이전 TargetJob키
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
    //    public String m_TargetJobKey;       //선택된 TargetJob키
    //}

    //########################################################################
    //
    //
    //                                                         Marring
    //
    //
    //########################################################################
    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_Marring
    //                
    //---------------------------------------------------------
    // Send   UI  -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ECHMarrying : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String WorkingMachineID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String WorkingMachineTP;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String PartnerMachineID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_TP)]
        public String PartnerMachineITP;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        public EEv2_Job_Location Loc;
        public UInt32 AutoManual;
    }
    // Ack   Tray -> UI  public class없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_Swap_Result
    //                
    //---------------------------------------------------------
    // Send   Tray -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_swapResult
    //{
    //    public UInt32 swapResult; //PROCEED : 1  SUCCESS : 2  FAIL : 3  RETURN TO YARD  : 0
    //}
    // Ack   UI  -> Tray Struct없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ECH_Return_Cntr
    //                
    //---------------------------------------------------------
    // Send   Tray -> UI
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    //[Serializable]
    //public class sTrayUI_returnWarning
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
    //    public String PartnerMachineID;
    //    public UInt32 returnWarning; //0: Off      1: Display Warning
    //}
    // Ack   UI  -> Tray Struct없음

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sECH_OTR_ManualBlockInOut
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_JOB_KEY)]
        public String m_JobKey;
        public Byte m_btInOut; //     1byte     1 이면 In    (  0 이면 Out : Default )
        //----------------------------- Tag정보.
        public sTrayUI_sECH_PDS_RFID_Payload m_OTR_RFIDTagInfo;    //없으면 0값으로
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3JobOrderList_ECH : EEParentClass
    {
        public int iCount; // 갯수
        public EEv3JobOrder[] JobOrder;
    }
}