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
// Protocol : VMT_TU_ITV_DGPS_Periodic
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sITV_PDS_Periodic_Payload
{
    //----------------PayLoad 77
    DWORD             m_dwTime;                   // __time32_t 타입의 시간.
    double            m_dLatitude;                // 위도
    double            m_dLongitude;               // 경도
    float             m_fHeadingDegree;           // 헤딩
    float             m_fSpeedOver;               // 속도

    int                m_BForwardBackward;     // 0x00 = forward, 0x01 = backward
    int                m_BShockSensor;         // 0x00 = Run, 0x01 = Break Down
    int                m_BAccelerator;         // 0x00 = Run, 0x01 = Break Down
    int                m_BChassisSensor;       // 0x00 = Run, 0x01 = Break Down
    int                m_BFuelGage;            // 0x00 = Run, 0x01 = Break Down
    int                m_BCollisionDetection;  // 0x00 = Run, 0x01 = Break Down
    int                m_BTirePressure;        // 0x00 = Run, 0x01 = Break Down
    int                m_BContainerCheck;      // 0x00 = Run, 0x01 = Break Down
    int                m_BConCheck;            // 0x00 = Run, 0x01 = Break Down
    int                m_BDGPS_INS;            // 0x00 = Run, 0x01 = Break Down
    int                m_BOdometerPulse;       // 0x00 = Run, 0x01 = Break Down

    int                m_BaseStation_Status;     // 0 = disconnect, 1 = connect, 2 = Error
    int                m_DGPS_Status;            // 0 = disconnect, 1 = connect, 2 = Error
    int                m_PDS_Status;             // 0 = disconnect, 1 = connect, 2 = Error  
    int                m_TOSClient_Status;       // 0 = disconnect, 1 = connect, 2 = Error
    int                m_EagleEyeEvent_Status;   // 0 = disconnect, 1 = connect, 2 = Error
    int                m_EagleEyePeriodic_Status;// 0 = disconnect, 1 = connect, 2 = Error  

//     unsigned          m_BForwardBackward : 1;     // 0x00 = forward, 0x01 = backward
//     unsigned          m_BShockSensor : 1;         // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BAccelerator : 1;         // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BChassisSensor : 1;       // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BFuelGage : 1;            // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BCollisionDetection : 1;  // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BTirePressure : 1;        // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BContainerCheck : 1;      // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BConCheck : 1;            // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BDGPS_INS : 1;            // 0x00 = Run, 0x01 = Break Down
//     unsigned          m_BOdometerPulse : 1;       // 0x00 = Run, 0x01 = Break Down
// 
//     unsigned          m_BaseStation_Status:2;     // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_DGPS_Status:2;            // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_PDS_Status:2;             // 0 = disconnect, 1 = connect, 2 = Error  
//     unsigned          m_TOSClient_Status:2;       // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_EagleEyeEvent_Status:2;   // 0 = disconnect, 1 = connect, 2 = Error
//     unsigned          m_EagleEyePeriodic_Status:2;// 0 = disconnect, 1 = connect, 2 = Error  
};



//---------------------------------------------------------
//  
// Protocol : VMT_TU_ITV_PDS
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct  sTrayUI_sITV_PDS_Event_Payload
{
    //----------------PayLoad 77
    DWORD             m_dwTime;					  // __time32_t 타입의 시간.

    int                    m_sShockSensorX;         // -512 ~ 511
    int                    m_sShockSensorY;         // -512 ~ 511
    int                   m_sShockSensorZ;		  // -512 ~ 511

    int                   m_sAcceleratorX;         // -512 ~ 511
    int                   m_sAcceleratorY;         // -512 ~ 511 
    int                   m_sAcceleratorZ;         // -512 ~ 511

    int                   m_ucDeviceStatus;         // 0 = DeviceNoStart, 1 = Run, 3 = BreakDown

    int                   m_BChassisSensor;         // 샤시 장착 
    int                   m_ucFuelGage;            // 0~999 주유용량
    int                   m_ucTireGage;            // 0~999 타이어용량
    int                   m_BCollisionDetection;    // 충돌감지? (두자리라 99까지)
    int                   m_BContainerCheck;        // 1 = 0n, 0 = 0ff = Container
    int                   m_BConCheck;              // 1 = 0n, 0 = 0ff = Cone

//     signed            m_sShockSensorX:11;         // -512 ~ 511
//     signed            m_sShockSensorY:11;         // -512 ~ 511
//     signed            m_sShockSensorZ:11;		  // -512 ~ 511
// 
//     signed            m_sAcceleratorX:11;         // -512 ~ 511
//     signed            m_sAcceleratorY:11;         // -512 ~ 511 
//     signed            m_sAcceleratorZ:11;         // -512 ~ 511
// 
//     unsigned          m_ucDeviceStatus:2;         // 0 = DeviceNoStart, 1 = Run, 3 = BreakDown
// 
//     unsigned          m_BChassisSensor:1;         // 샤시 장착 
//     unsigned          m_ucFuelGage:10;            // 0~999 주유용량
//     unsigned          m_ucTireGage:10;            // 0~999 타이어용량
//     unsigned          m_BCollisionDetection:7;    // 충돌감지? (두자리라 99까지)
//     unsigned          m_BContainerCheck:1;        // 1 = 0n, 0 = 0ff = Container
//     unsigned          m_BConCheck:1;              // 1 = 0n, 0 = 0ff = Cone

};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SetChassis_Attach
//                
//---------------------------------------------------------
// Send  UI -> Tray
struct  sTrayUI_ChassisAttachInfo
{
    enum ChassisType
    {
        None = 0,
        Foot20,
        Foot40,
        GooseNeck,
        Special
    };

    int         nType;
    TCHAR  m_ChassisNumber[30];
};
// Ack    Tray -> UI
// struct 없음


//---------------------------------------------------------
//  
// Protocol : VMT_TU_NofityBlockEnterance
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct sTrayUI_BlockEnter
{
    TCHAR    m_BlockName[EEV2_STRING_MAX_LOCATION_BLOCK];
    BOOL      m_bEntrance; //TRUE: in, FALSE: out
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_NotifyCPSAlign
//                
//---------------------------------------------------------
// Notify   Tray -> UI
struct sTrayUI_NotifyCPSAlign
{
     TCHAR PartnerMchnID[EEV2_STRING_MAX_MACHINE_ID];
     int        iAlignment;
};


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SetManualReady
//                
//---------------------------------------------------------
// Send    UI -> Tray
struct sTrayUI_SetManualReadyRQ
{
    TCHAR   WorkingMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR   PartnerMchnID[EEV2_STRING_MAX_MACHINE_ID];
};
// Responce  Tray -> UI
// struct없음.

//---------------------------------------------------------
//  
// Protocol : VMT_TU_NotifyManualReady
//                
//---------------------------------------------------------
// Notify  Tray -> UI
struct sTrayUI_NotifyManualReady
{
    TCHAR   WorkingMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR   PartnerMchnID[EEV2_STRING_MAX_MACHINE_ID];
};


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SetManualArrival
//                
//---------------------------------------------------------
// Send    UI -> Tray
struct sTrayUI_SetManuaArrivalRQ
{
    TCHAR   WorkingMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR   PartnerMchnID[EEV2_STRING_MAX_MACHINE_ID];
};
// Responce  Tray -> UI
// struct없음.

//---------------------------------------------------------
//  
// Protocol : VMT_TU_NotifyManualReady
//                
//---------------------------------------------------------
// Notify , Responce  Tray -> UI
struct sTrayUI_NotifyManualArrivalRP
{
    TCHAR   WorkingMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR   PartnerMchnID[EEV2_STRING_MAX_MACHINE_ID];
    BOOL     m_bPOWIN; // TRUE : in     FALSE  : out
};

#pragma pack()