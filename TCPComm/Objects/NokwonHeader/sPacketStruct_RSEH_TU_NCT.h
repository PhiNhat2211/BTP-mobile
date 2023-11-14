#pragma  once

#pragma pack(1)

struct sTrayUI_sRSEH_PDS_Periodic_Payload
{
    //----------------PayLoad 77
    DWORD             m_dwTime;                   // __time32_t 타입의 시간.
    double            m_dLatitude;                // 위도 정보
    double            m_dLongitude;               // 경도 정보
    float               m_fHeadingDegree;           // 헤딩 정보
    float               m_fSpeedOver;               // 속도 정보

    int                  m_ucFuelGage;            // 0~999주유용량
    int                  m_ucTireGage;            // 0~999타이어용량

    int                   m_BForwardBackward;     // 0x00 = forward, 0x01 = backward
    int                   m_BShockSensor;         // 0x00 = Run, 0x01 = Break Down
    int                   m_BAccelerator;         // 0x00 = Run, 0x01 = Break Down

    int                   m_cDistanceSensor;		  // 0x00 = Run, 0x01 = Break Down  거리 센서
    int                   m_cHeightSensor;            // 0x00 = Run, 0x01 = Break Down  높이 센서
    int                   m_cRFIDSensor;			  // 0x00 = Run, 0x01 = Break Down  RFID 센서
    int                   m_cAntiCollisionDetection;  // 0x00 = Run, 0x01 = Break Down
    int                   m_cFuelGage;				  // 0x00 = Run, 0x01 = Break Down
    int                   m_cTirePressure;            // 0x00 = Run, 0x01 = Break Down


    int                   m_BaseStation_Status;         // 0 = disconnect, 1 = connect, 2 = Error
    int                   m_DGPS_Status;                // 0 = disconnect, 1 = connect, 2 = Error
    int                   m_PDS_Status;                 // 0 = disconnect, 1 = connect, 2 = Error  
    int                   m_TOSClient_Status;           // 0 = disconnect, 1 = connect, 2 = Error
    int                   m_EagleEyeEvent_Status;       // 0 = disconnect, 1 = connect, 2 = Error
    int                   m_EagleEyePeriodic_Status;	  // 0 = disconnect, 1 = connect, 2 = Error

    int                   m_cBlockInOut;                          //Block In = 1, Block Out = 0
    int                   m_cBay;                                    //Bay Number
    int                   m_cRow;                                  // 1 = A, 2 = B 3 = C.... 26 = Z	
    TCHAR             m_strBlockName[6];                       //BlockName
};

struct sTrayUI_sRSEH_PDS_PickDrop_Payload
{
    DWORD             m_dwTime;              // __time32_t 타입의 시간.
    char                   m_cTwistLockUnlock;    // "0”= Null, 1” = Lock,“2”= Unlock,“E” = Error
    char                   m_cOperationType;      // "0" = Null, "1" = 20ft,"3" = 40ft, "8" = None containerized operation, "E"=Error	*45ft ,48ft 추가 가능성 있음

    DWORD             m_dwSpreaderHeight;    // mm 단위(from Ground)
    float                  m_fReachAngle;         // 4 digit place Radian,  ex)32.9545 rad = “32.9545”  Machine에서 제공하는 단위는 도 단위임 
    

    int                     m_dwReachLength;       // mm 단위(extended distance size)*Machine 제공단위는 m 단위
    int                     m_wDistanceSensorData; // cm 단위(from frontend)
    int                     m_wHeightSensorData;   // cm 단위(from Ground)
    int                     m_ucDeviceStatus;    // 0 = DeviceNoStart, 1 = Run, 3 = BreakDown
     
    int                     m_sShockSensorX;    // -512 ~ 511
    int                     m_sShockSensorY;    // -512 ~ 511
    int                     m_sShockSensorZ;    // -512 ~ 511

    int                     m_sAcceleratorX;    // -512 ~ 511
    int                     m_sAcceleratorY;    // -512 ~ 511
    int                     m_sAcceleratorZ;    // -512 ~ 511 

};

struct sTrayUI_sRSEH_PDS_RFID_Payload
{
    //----------------PayLoad 77
    DWORD             m_dwTime;     // 시간 정보
    char                  m_cAntennaID; // "1" = 1번, "2" = 2번, "E" = error or Unknown (번호는 RFID Antenna ID를 따름)
    char                  m_cTagID[9];  // NCTXXXXX =>  ex) “NCT00001” 
    char                  m_cFlag;      // "1" = Begin, "2" = End
};


#pragma pack()