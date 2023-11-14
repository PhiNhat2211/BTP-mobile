#pragma  once

#pragma pack(1)

struct sTrayUI_sRSEH_PDS_Periodic_Payload
{
    //----------------PayLoad 77
    DWORD             m_dwTime;                   // __time32_t Ÿ���� �ð�.
    double            m_dLatitude;                // ���� ����
    double            m_dLongitude;               // �浵 ����
    float               m_fHeadingDegree;           // ��� ����
    float               m_fSpeedOver;               // �ӵ� ����

    int                  m_ucFuelGage;            // 0~999�����뷮
    int                  m_ucTireGage;            // 0~999Ÿ�̾�뷮

    int                   m_BForwardBackward;     // 0x00 = forward, 0x01 = backward
    int                   m_BShockSensor;         // 0x00 = Run, 0x01 = Break Down
    int                   m_BAccelerator;         // 0x00 = Run, 0x01 = Break Down

    int                   m_cDistanceSensor;		  // 0x00 = Run, 0x01 = Break Down  �Ÿ� ����
    int                   m_cHeightSensor;            // 0x00 = Run, 0x01 = Break Down  ���� ����
    int                   m_cRFIDSensor;			  // 0x00 = Run, 0x01 = Break Down  RFID ����
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
    DWORD             m_dwTime;              // __time32_t Ÿ���� �ð�.
    char                   m_cTwistLockUnlock;    // "0��= Null, 1�� = Lock,��2��= Unlock,��E�� = Error
    char                   m_cOperationType;      // "0" = Null, "1" = 20ft,"3" = 40ft, "8" = None containerized operation, "E"=Error	*45ft ,48ft �߰� ���ɼ� ����

    DWORD             m_dwSpreaderHeight;    // mm ����(from Ground)
    float                  m_fReachAngle;         // 4 digit place Radian,  ex)32.9545 rad = ��32.9545��  Machine���� �����ϴ� ������ �� ������ 
    

    int                     m_dwReachLength;       // mm ����(extended distance size)*Machine ���������� m ����
    int                     m_wDistanceSensorData; // cm ����(from frontend)
    int                     m_wHeightSensorData;   // cm ����(from Ground)
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
    DWORD             m_dwTime;     // �ð� ����
    char                  m_cAntennaID; // "1" = 1��, "2" = 2��, "E" = error or Unknown (��ȣ�� RFID Antenna ID�� ����)
    char                  m_cTagID[9];  // NCTXXXXX =>  ex) ��NCT00001�� 
    char                  m_cFlag;      // "1" = Begin, "2" = End
};


#pragma pack()