#pragma once

#pragma pack(1)




//########################################################################
//
//
//                                                      Sensing Device
//
//
//########################################################################


//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_PDS_Periodic
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sRTG_PDS_Periodic_Payload
{
    DWORD             m_dwTime;                            // 시간
    double            m_dLatitude;                         // 위도
    double            m_dLongitude;                        // 경도

    float                m_fHeadingDegree;               // 헤딩 정보
    int                   m_cTrolleyPosition;               // 6자리 mm Unit 
    int                   m_cHoistPosition;                 // 6자리 mm Unit 

    int                   m_cGantryMoveOnOff;                // Gantry Move 0 = Not Moving, 1 = Moving  
    int                   m_cDriveDirectionDegree;           // 0 degree = forward, 180 degree = backward
    int                   m_cAntiCollisionDetectionSignal;  // 0 = Nothing, 1= stop RTG cause Anti-Collision Device alarm

    //----sRTG_PDS_TireFuel_Payload_Recv
    int                   m_cFuelGageLiter;                 // 실제 연료량
    int                   m_cTireRessurePSI;                // 실제 타이어 압력

    //---- 접속여부..
    int                  m_cRFIDStatus;                     // RFID 센서 상태 정보
    int                  m_cFuelGage;                       // 연료게이지 센서 상태 정보
    int                  m_cTirePressureCheck;         // 타이어 압력센서 상태 정보 

    int                  m_CPS_Status;                      // 0 = disconnect, 1 = connect, 2 = Error
    int                  m_PDS_Status;                      // 0 = disconnect, 1 = connect, 2 = Error  
    int                  m_TOSClient_Status;                // 0 = disconnect, 1 = connect, 2 = Error
    int                  m_EagleEyeEvent_Status;            // 0 = disconnect, 1 = connect, 2 = Error
    int                   m_EagleEyePeriodic_Status;         // 0 = disconnect, 1 = connect, 2 = Error

    int                  m_cBlockInOut;                          //Block In = 1, Block Out = 0
    int                  m_cBay;                                    //Bay Number
    TCHAR            m_strBlockName[6];                    //BlockName

//     unsigned          m_cGantryMoveOnOff:1;                // Gantry Move 0 = Not Moving, 1 = Moving  
//     unsigned          m_cDriveDirectionDegree:9;           // 0 degree = forward, 180 degree = backward
//     unsigned          m_cAntiCollisionDetectionSignal:1;   // 0 = Nothing, 1= stop RTG cause Anti-Collision Device alarm
// 
//     unsigned          m_cRFIDStatus:2;                     // RFID 센서 상태 정보
//     unsigned          m_cFuelGage:2;                       // 연료게이지 센서 상태 정보
//     unsigned          m_cTirePressureCheck:2;              // 타이어 압력센서 상태 정보 
// 
//     //----sRTG_PDS_TireFuel_Payload_Recv
//     unsigned          m_cFuelGageLiter:10;                 // 실제 연료량
//     unsigned          m_cTireRessurePSI:10;                // 실제 타이어 압력
// 
//     //---- 접속여부..
//     unsigned          m_CPS_Status:2;                      // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_PDS_Status:2;                      // 0 = disconnect, 1 = connect, 2 = Error  
//     unsigned          m_TOSClient_Status:2;                // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_EagleEyeEvent_Status:2;            // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_EagleEyePeriodic_Status:2;         // 0 = disconnect, 1 = connect, 2 = Error
// 
//     unsigned          m_cBlockInOut:1;                          //Block In = 1, Block Out = 0
//     unsigned          m_cBay:8;                                    //Bay Number
//     TCHAR            m_strBlockName[6];                       //BlockName
};


//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_CPS_Align
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sRTG_CPS_Alignment_Payload
{
    DWORD             m_dwTime;
    char              m_cAlignmentResult;  // '0'=Empty, '1' = Completed, '2'=Processing, '3' = Detected, '4'= Passed, 'E'=Error
    // (Completed : 완료, Processing : Alignment 진행 중, Detected : 차량진입발견, Passed = 발견된 차량이 지나간 경우)
    char              m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
    char              m_cDirection;        // '0'=Empty,(비어있음) , '1' = Normal Direction(정방향 접근), 
//     char              m_cAlignmentResult;  // '0'=Empty, '1' = Completed, '2'=Processing, '3' = Detected, '4'= Passed, 'E'=Error
//     // (Completed : 완료, Processing : Alignment 진행 중, Detected : 차량진입발견, Passed = 발견된 차량이 지나간 경우)
//     char              m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
//     char              m_cDirection;        // '0'=Empty,(비어있음) , '1' = Normal Direction(정방향 접근), 
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_PDS_PickDrop 
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sRTG_PDS_PickDrop_Payload
{
    DWORD             m_dwTime;                // 시간
    char              m_cBlock[10];             // Block Name '1A'

    int                 m_cTrolleyPosition;   // 6자리 mm Unit
    int                 m_cHoistPosition;     // 6자리 mm Unit

    // '0' = Null, '1' = 20ft, '2'=20ft_Twin,'3' = 40ft, '4' = 45ft,
    // '5'=48ft,   '8' = None containerized operation, '10'=Error
    int                 m_cOperationType;      

    int                 m_cBay;                // 00, 01, ... 256 
    int                 m_cRow;                // 1 = A, 2 = B 3 = C.... 26 = Z	
    int                 m_cTier;               // 1, 2

    int                 m_cTwistLockUnlock;    // 1 = Lock, 2 = Unlock, 3 = Error
    int                 m_cContainerWeight;   // 컨테이너 무게Kg



//     signed            m_cTrolleyPosition:20;   // 6자리 mm Unit
//     signed            m_cHoistPosition:20;     // 6자리 mm Unit
// 
//     // '0' = Null, '1' = 20ft, '2'=20ft_Twin,'3' = 40ft, '4' = 45ft,
//     // '5'=48ft,   '8' = None containerized operation, '10'=Error
//     unsigned          m_cOperationType:4;      
// 
//     unsigned          m_cBay:8;                // 00, 01, ... 256 
//     unsigned          m_cRow:5;                // 1 = A, 2 = B 3 = C.... 26 = Z	
//     unsigned          m_cTier:3;               // 1, 2
// 
//     unsigned          m_cTwistLockUnlock:2;    // 1 = Lock, 2 = Unlock, 3 = Error
//     unsigned          m_cContainerWeight:20;   // 컨테이너 무게Kg
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_RFID 
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sRTG_PDS_RFID_Payload
{
    DWORD             m_dwTime;                 // 시간 정보
    char              m_cAntennaID;             // '1' = 1번, '2' = 2번, 'E' = error or Unknown (번호는 RFID Antenna ID를 따름)
    char              m_cTagID[RFID_TAG_MAX];   // EPC 64bit ASCII Code String 'NCT00001' => 0x4E0x430x540x300x300x300x300x31 
    //                                             N   C   T   0   0   0   0    1
    char              m_cFlag;                  // '1' = Begin, '2' = End
};



//########################################################################
//
//
//                                                         ITV Info
//
//
//########################################################################
//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_NotifyMachinePOW
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct sTrayUI_POWInfo
{
    TCHAR     ITVMachineID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR     BlockName[EEV2_STRING_MAX_LOCATION_BLOCK];
    int           iBay;
    BOOL       bPOW; //TRUE in   , FALSE out
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_NotifyMachineBlockEnter
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct sTrayUI_BlockEnteranceITV
{
    TCHAR     ITVMachineID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR     BlockName[EEV2_STRING_MAX_LOCATION_BLOCK];
    BOOL       bEnterance; //TRUE in   , FALSE out
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_NotifyMachineReadyITV
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct sTrayUI_ManualReadyITV
{
    TCHAR     ITVMachineID[EEV2_STRING_MAX_MACHINE_ID];
};



//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_ManualReady
//                
//---------------------------------------------------------
// Send   UI  -> Tray
struct sTrayUI_ManualReadyRTG
{
     TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
     TCHAR      ITVMachineID[EEV2_STRING_MAX_MACHINE_ID];
     BOOL        bReadyOnOff;//( 1: On, 0 : Off)
};

// Ack   Tray -> UI  Struct 없음


//########################################################################
//
//
//                                                         Inventory Correction
//
//
//########################################################################

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_SendBlockInfo
//                
//---------------------------------------------------------
// Send   UI  -> Tray
struct  sTrayUI_BlockInfoRq
{
    TCHAR szBlockName[EEV2_STRING_MAX_LOCATION_BLOCK];
    BYTE    bayNo;
};
// Ack   Tray -> UI  Struct없음

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_NotifyBlockInfo
//                
//---------------------------------------------------------
// Notify   Tray  -> UI
//#define TIER_ROW_MAX 49

// struct  EEv2_Def_Inventory
// {
//     EEv2_Def_Container  cntr;
//     EEv2_Def_Location    loc;
//     //1.4.1 구조
//     TCHAR    inOut[EEV2_STRING_MAX_INVEN_INOUT]; //In/Out
//     TCHAR    reason[EEV2_STRING_MAX_INVEN_REASON]; //reason
// 
//     BOOL    isTOSData; //TOS가 관리한
//     BOOL    mgrEE; //EagleEye가 관리한  
//     TCHAR    InvenTp[EEV2_STRING_MAX_LOCATION_TYPE]; // INV,NWT,NWA,TNL,VRT
// };


//Recv 패킷
// struct sTrayUI_sBlockBayInfo
// {
//     int bay; // 해당 베이 번호
//     int row; // row max
//     int tier; // tier max
//     char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
//     byte cntrCount;
//     EEv2_Def_Inventory cntr[N}; //--->CntrCount만큼 받음..
// };


//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_SendBlockInfoSimple
//                
//---------------------------------------------------------
// Send   UI  -> Tray
// struct  sTrayUI_BlockInfoRq
// {
//     TCHAR szBlockName[EEV2_STRING_MAX_LOCATION_BLOCK];
//     BYTE    bayNo;
// };
// Ack   Tray -> UI  Struct없음


//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_NotifyBlockInfoSimple
//                
//---------------------------------------------------------
// Notify   Tray  -> UI
//#define TIER_ROW_MAX 49

// struct EEv2_Simple_Conatainer
// {
//     TCHAR  cntrNo[EEV2_STRING_MAX_CNTR_NO]; 
//     TCHAR  cntrIso[EEV2_STRING_MAX_CNTR_ISO]; 
//     TCHAR  cntrTp[EEV2_STRING_MAX_CNTR_TYPE];
//     EEv2_Def_Location  loc;   //해당컨테이너의 Location 정보
// 
//     BOOL   isDmg;
//     BOOL   isHold;
//     TCHAR plugCd[EEV2_STRING_MAX_REEFER_PLUGCD];
//     TCHAR  fullMty[EEV2_STRING_MAX_CNTR_FULL_MTY];
// };
// struct sTrayUI_sBlockBayInfoSimple
// {
//     int bay; // 해당 베이 번호
//     int row; // row max
//     int tier; // tier max
//     char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
//     byte cntrCount;      
//     EEv2_Simple_Conatainer cntr[N];  //--->CntrCount만큼 받음..
// };

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_SendCorrection
//                
//---------------------------------------------------------
// Send   UI  -> Tray
struct sTrayUI_Correction
{
    TCHAR   cntrNo[EEV2_STRING_MAX_CNTR_NO];
    EEv2_Yard_Location fromLoc;
    EEv2_Yard_Location toLoc;
    TCHAR   actionType;   // C=creation, D=delete, M=move 인벤토리정보(DB) 를 수정하는 형태
};
// Ack   Tray -> UI  Struct없음


//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_NotifyCorrection
//                
//---------------------------------------------------------
// Send   UI  -> Tray
// Notify   Tray  -> UI
//#define TIER_ROW_MAX 49

// struct  EEv2_Def_Inventory
// {
//     EEv2_Def_Container  cntr;
//     EEv2_Def_Location    loc;
//     //1.4.1 구조
//     TCHAR    inOut[EEV2_STRING_MAX_INVEN_INOUT]; //In/Out
//     TCHAR    reason[EEV2_STRING_MAX_INVEN_REASON]; //reason
// 
//     BOOL    isTOSData; //TOS가 관리한
//     BOOL    mgrEE; //EagleEye가 관리한  
//     TCHAR    InvenTp[EEV2_STRING_MAX_LOCATION_TYPE]; // INV,NWT,NWA,TNL,VRT
// };


//Recv 패킷
// struct sTrayUI_sBlockBayInfo
// {
//     int bay; // 해당 베이 번호
//     int row; // row max
//     int tier; // tier max
//     char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
//     byte cntrCount;
//     EEv2_Def_Inventory cntr[N}; //--->CntrCount만큼 받음..
// };


//########################################################################
//
//
//                                                         RTG JobOrder
//
//
//########################################################################
//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_SetCurrentJob
//                
//---------------------------------------------------------
// Send   UI  -> Tray
struct sTrayUI_SetCurrentJob
{
    TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
};
// Ack   Tray -> UI  Struct없음

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_HandleJobDone
//                
//---------------------------------------------------------
// Send   UI  -> Tray
struct sTrayUI_HandleJobDone
{
    TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
    TCHAR     WorkingMachineID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR     WorkingMachineTP[EEV2_STRING_MAX_MACHINE_TP];
    TCHAR     PartnerMachineID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR     PartnerMachineITP[EEV2_STRING_MAX_MACHINE_TP];
    TCHAR     cntrNo[EEV2_STRING_MAX_CNTR_NO];
    EEv2_Job_Location    Loc;     
    EEv3_Spreader       sprd;
};
// Ack   Tray -> UI  Struct없음

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_TargetJob
//                
//---------------------------------------------------------
// Send   Tray -> UI
struct sTrayUI_TargetJob
{
    TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
};
// Ack   UI  -> Tray Struct없음


//########################################################################
//
//
//                                                         Marring
//
//
//########################################################################
//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_Marring
//                
//---------------------------------------------------------
// Send   UI  -> Tray
struct sTrayUI_RTGMarring
{
     TCHAR     WorkingMachineID[EEV2_STRING_MAX_MACHINE_ID];
     TCHAR     WorkingMachineTP[EEV2_STRING_MAX_MACHINE_TP];
     TCHAR     PartnerMachineID[EEV2_STRING_MAX_MACHINE_ID];
     TCHAR     PartnerMachineITP[EEV2_STRING_MAX_MACHINE_TP];
     TCHAR     cntrNo[EEV2_STRING_MAX_CNTR_NO];
     EEv2_Job_Location   Loc;   
	 unsigned int  AutoManual; // Marring이 ( Auto이면 0  , Manual이면 1)
};
// Ack   Tray -> UI  Struct없음


//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_Swap_Result
//                
//---------------------------------------------------------
// Send   Tray -> UI
struct sTrayUI_swapResult
{
    unsigned int swapResult; //PROCEED : 1     SUCCESS : 2         FAIL : 3        RETURN TO YARD  : 0
};
// Ack   UI  -> Tray Struct없음

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_Return_Cntr
//                
//---------------------------------------------------------
// Send   Tray -> UI
struct sTrayUI_returnWarning
{
    TCHAR        PartnerMachineID[EEV2_STRING_MAX_MACHINE_ID];
    unsigned int returnWarning; //0: Off      1: Display Warning
};
// Ack   UI  -> Tray Struct없음

#pragma  pack()