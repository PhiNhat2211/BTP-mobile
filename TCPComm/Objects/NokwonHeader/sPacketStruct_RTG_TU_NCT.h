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
    DWORD             m_dwTime;                            // �ð�
    double            m_dLatitude;                         // ����
    double            m_dLongitude;                        // �浵

    float                m_fHeadingDegree;               // ��� ����
    int                   m_cTrolleyPosition;               // 6�ڸ� mm Unit 
    int                   m_cHoistPosition;                 // 6�ڸ� mm Unit 

    int                   m_cGantryMoveOnOff;                // Gantry Move 0 = Not Moving, 1 = Moving  
    int                   m_cDriveDirectionDegree;           // 0 degree = forward, 180 degree = backward
    int                   m_cAntiCollisionDetectionSignal;  // 0 = Nothing, 1= stop RTG cause Anti-Collision Device alarm

    //----sRTG_PDS_TireFuel_Payload_Recv
    int                   m_cFuelGageLiter;                 // ���� ���ᷮ
    int                   m_cTireRessurePSI;                // ���� Ÿ�̾� �з�

    //---- ���ӿ���..
    int                  m_cRFIDStatus;                     // RFID ���� ���� ����
    int                  m_cFuelGage;                       // ��������� ���� ���� ����
    int                  m_cTirePressureCheck;         // Ÿ�̾� �з¼��� ���� ���� 

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
//     unsigned          m_cRFIDStatus:2;                     // RFID ���� ���� ����
//     unsigned          m_cFuelGage:2;                       // ��������� ���� ���� ����
//     unsigned          m_cTirePressureCheck:2;              // Ÿ�̾� �з¼��� ���� ���� 
// 
//     //----sRTG_PDS_TireFuel_Payload_Recv
//     unsigned          m_cFuelGageLiter:10;                 // ���� ���ᷮ
//     unsigned          m_cTireRessurePSI:10;                // ���� Ÿ�̾� �з�
// 
//     //---- ���ӿ���..
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
    // (Completed : �Ϸ�, Processing : Alignment ���� ��, Detected : �������Թ߰�, Passed = �߰ߵ� ������ ������ ���)
    char              m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
    char              m_cDirection;        // '0'=Empty,(�������) , '1' = Normal Direction(������ ����), 
//     char              m_cAlignmentResult;  // '0'=Empty, '1' = Completed, '2'=Processing, '3' = Detected, '4'= Passed, 'E'=Error
//     // (Completed : �Ϸ�, Processing : Alignment ���� ��, Detected : �������Թ߰�, Passed = �߰ߵ� ������ ������ ���)
//     char              m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
//     char              m_cDirection;        // '0'=Empty,(�������) , '1' = Normal Direction(������ ����), 
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_PDS_PickDrop 
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sRTG_PDS_PickDrop_Payload
{
    DWORD             m_dwTime;                // �ð�
    char              m_cBlock[10];             // Block Name '1A'

    int                 m_cTrolleyPosition;   // 6�ڸ� mm Unit
    int                 m_cHoistPosition;     // 6�ڸ� mm Unit

    // '0' = Null, '1' = 20ft, '2'=20ft_Twin,'3' = 40ft, '4' = 45ft,
    // '5'=48ft,   '8' = None containerized operation, '10'=Error
    int                 m_cOperationType;      

    int                 m_cBay;                // 00, 01, ... 256 
    int                 m_cRow;                // 1 = A, 2 = B 3 = C.... 26 = Z	
    int                 m_cTier;               // 1, 2

    int                 m_cTwistLockUnlock;    // 1 = Lock, 2 = Unlock, 3 = Error
    int                 m_cContainerWeight;   // �����̳� ����Kg



//     signed            m_cTrolleyPosition:20;   // 6�ڸ� mm Unit
//     signed            m_cHoistPosition:20;     // 6�ڸ� mm Unit
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
//     unsigned          m_cContainerWeight:20;   // �����̳� ����Kg
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RTG_RFID 
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sRTG_PDS_RFID_Payload
{
    DWORD             m_dwTime;                 // �ð� ����
    char              m_cAntennaID;             // '1' = 1��, '2' = 2��, 'E' = error or Unknown (��ȣ�� RFID Antenna ID�� ����)
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

// Ack   Tray -> UI  Struct ����


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
// Ack   Tray -> UI  Struct����

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
//     //1.4.1 ����
//     TCHAR    inOut[EEV2_STRING_MAX_INVEN_INOUT]; //In/Out
//     TCHAR    reason[EEV2_STRING_MAX_INVEN_REASON]; //reason
// 
//     BOOL    isTOSData; //TOS�� ������
//     BOOL    mgrEE; //EagleEye�� ������  
//     TCHAR    InvenTp[EEV2_STRING_MAX_LOCATION_TYPE]; // INV,NWT,NWA,TNL,VRT
// };


//Recv ��Ŷ
// struct sTrayUI_sBlockBayInfo
// {
//     int bay; // �ش� ���� ��ȣ
//     int row; // row max
//     int tier; // tier max
//     char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
//     byte cntrCount;
//     EEv2_Def_Inventory cntr[N}; //--->CntrCount��ŭ ����..
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
// Ack   Tray -> UI  Struct����


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
//     EEv2_Def_Location  loc;   //�ش������̳��� Location ����
// 
//     BOOL   isDmg;
//     BOOL   isHold;
//     TCHAR plugCd[EEV2_STRING_MAX_REEFER_PLUGCD];
//     TCHAR  fullMty[EEV2_STRING_MAX_CNTR_FULL_MTY];
// };
// struct sTrayUI_sBlockBayInfoSimple
// {
//     int bay; // �ش� ���� ��ȣ
//     int row; // row max
//     int tier; // tier max
//     char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
//     byte cntrCount;      
//     EEv2_Simple_Conatainer cntr[N];  //--->CntrCount��ŭ ����..
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
    TCHAR   actionType;   // C=creation, D=delete, M=move �κ��丮����(DB) �� �����ϴ� ����
};
// Ack   Tray -> UI  Struct����


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
//     //1.4.1 ����
//     TCHAR    inOut[EEV2_STRING_MAX_INVEN_INOUT]; //In/Out
//     TCHAR    reason[EEV2_STRING_MAX_INVEN_REASON]; //reason
// 
//     BOOL    isTOSData; //TOS�� ������
//     BOOL    mgrEE; //EagleEye�� ������  
//     TCHAR    InvenTp[EEV2_STRING_MAX_LOCATION_TYPE]; // INV,NWT,NWA,TNL,VRT
// };


//Recv ��Ŷ
// struct sTrayUI_sBlockBayInfo
// {
//     int bay; // �ش� ���� ��ȣ
//     int row; // row max
//     int tier; // tier max
//     char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
//     byte cntrCount;
//     EEv2_Def_Inventory cntr[N}; //--->CntrCount��ŭ ����..
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
// Ack   Tray -> UI  Struct����

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
// Ack   Tray -> UI  Struct����

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
// Ack   UI  -> Tray Struct����


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
	 unsigned int  AutoManual; // Marring�� ( Auto�̸� 0  , Manual�̸� 1)
};
// Ack   Tray -> UI  Struct����


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
// Ack   UI  -> Tray Struct����

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
// Ack   UI  -> Tray Struct����

#pragma  pack()