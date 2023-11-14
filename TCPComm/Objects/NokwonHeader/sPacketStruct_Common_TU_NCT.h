#pragma once

#pragma pack(1)

//########################################################################
//
//
//                                                     Common  Sensing Device
//
//
//########################################################################

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SoftwareInfo
// �α��� �ϱ� ���� ID �Է½� �����ڿ� ���� Ȯ������
//  
//---------------------------------------------------------
// Request  UI -> Tray
struct  sTrayUI_SwinfoRQ
{
    TCHAR      m_VMTUI_Version[20];
    TCHAR      m_szMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR      m_szMchnTp[EEV2_STRING_MAX_MACHINE_TP];
};
// Response Tray -> UI
struct  sTrayUI_SwinfoRP
{
     int             m_iResult;                // 0 ���Ӻ���,    1 �����㰡.
     int             m_iLoginResult;        //  0 login�� �ȵȻ���   1  login�̵Ȼ��� 
    TCHAR        m_VMTTray_Version[20];
    TCHAR        m_UserID[EEV2_STRING_MAX_USER_ID];
    TCHAR        m_UserPW[EEV2_STRING_MAX_USER_PW];
    TCHAR        m_GroupName[EEV2_STRING_MAX_USER_GROUP];
    TCHAR        m_DriverName[EEV2_STRING_MAX_USER_NAME];
    TCHAR        m_szSite[EEV2_STRING_MAX_EXECUTE_SITE];   // KAP, JAT3 
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_ConnectionStatus
// 
//  
//---------------------------------------------------------
//Request UI -> Tray 
//STruct ����

// Ack  Tray -> UI
struct sTrayUI_ConnectionStatus
{
    int             m_iEagleEyeStatus;   //0 �̸� ����ȵ�,    1�̸� �����
    int             m_iGPSStatus;         // 0 �̸� ����ȵ�,    1�̸� �����
};
//---------------------------------------------------------
//  
// Protocol : VMT_TU_MachineNotice
// 
//  
//---------------------------------------------------------
// Nofity  Tray -> UI
struct sTrayUI_MachineNotice
{
    int             m_iMessageType; //0: ���� 1: TOS  2: VT  3: EE.Server 4: Tray
    TCHAR       m_strMessage[EEV2_STRING_MAX_NOTICE_MSG];
    TCHAR       m_strMessage2[EEV2_STRING_MAX_SERVER_MESSAGE]; //EEServer �����ش�
};

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
// �α��� �ϱ� ���� ID �Է½� �����ڿ� ���� Ȯ������
//  
//---------------------------------------------------------
// Request  UI -> Tray
struct sTrayUI_GetUSerAccesRoleRQ
{
    TCHAR     UserID[EEV2_STRING_MAX_USER_ID];
};
// Response Tray -> UI
struct sTrayUI_GetUSerAccesRoleRP
{
	BOOL  bIsOn; // ���, ����
	TCHAR GroupListSeperator[500];// "Group1|Group2|Group3" ���·� ���޵�.
	TCHAR Notice[300];
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SetLogin4Machine
//                �α��� ������� �̸��� ȭ�鿡 ǥ���Ѵ�.
//---------------------------------------------------------
// Request  UI -> Tray
struct sTrayUI_SetLogin4MachineRQ
{
	TCHAR UserID[7];
	TCHAR UserPW[7];
	TCHAR GroupName[50];     // 20
	TCHAR MchnID[EEV2_STRING_MAX_MACHINE_ID];
	TCHAR MchnTp[EEV2_STRING_MAX_MACHINE_TP];
};
// Response Tray -> UI
struct sTrayUI_SetLogin4MachineRP
{
	int   iLogin;		          // 0 : �α��� ����  1: �α��μ���  2: �̷̹α��λ���
	TCHAR UserName[50];
	TCHAR Notice[300];
};




//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendMachineStatusChange
//                �α��� ������� �̸��� ȭ�鿡 ǥ���Ѵ�.
//---------------------------------------------------------
// Request  UI -> Tray
struct sTrayUI_SendMachineStatusChangeRQ
{
    TCHAR     m_MchnID[EEV2_STRING_MAX_MACHINE_ID];
    BOOL       m_bisON;  //True  ,   False
};
// Response Tray -> UI
struct sTrayUI_ResultRP
{
    int            m_iResult;		      // 1:  ���� , 0 : ����
};

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
// struct ����.
// Request  Tray -> UI
// struct ����.

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendPubx05
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.
// Request  Tray -> UI
// struct ����.


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendPubx06
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.
// Request  Tray -> UI
// struct ����.


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendHighForward
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.
// Request  Tray -> UI
// struct ����.


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendHighBackward
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.
// Request  Tray -> UI
// struct ����.

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RequestCfgekf
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.
// Request  Tray -> UI
// Response Tray -> UI
struct sTrayUI_RequestCfgekfRP
{
    int            m_iDirection;		      
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SaveDGPSCfg
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.
// Request  Tray -> UI
// struct ����.


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
//                �ӽ� Stop Code �䱸
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.


// Response , Notify : Tray -> UI
struct sTrayUI_Available
{
    TCHAR	ReasonCd[EEV2_STRING_MAX_REASON_CODE];
    TCHAR	ReasonNm[EEV2_STRING_MAX_REASON_NAME];
};
// 
// -> Recv ����
// struct sTrayUI_MachineStopCodeList
// {
//       int              m_iAvailableCount;   //����
//       sTrayUI_Available     m_pData[N];                //
// };


//---------------------------------------------------------
//  
// Protocol : VMT_TU_GettMachineStop
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct ����.

// Response , Notify : Tray -> UI
struct sTrayUI_GetMachineStop
{
        sTrayUI_Available   Data;
        int             m_iBreak;//0 : No Break    1: Breaking
        __int64		StartTime;  // 
        __int64		FinishTime; //
};
//---------------------------------------------------------
//  
// Protocol : VMT_TU_SetMachineStop
//                
//---------------------------------------------------------
// Send  UI -> Tray
struct sTrayUI_SetMachineStop
{
    sTrayUI_Available  Data;
    int             m_iBreakStatus;//1 : Break   2: Release
    TCHAR      m_szMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR      m_szMchnTp[EEV2_STRING_MAX_MACHINE_TP];
    TCHAR      m_UserID[EEV2_STRING_MAX_USER_ID];    
    TCHAR      m_DriverName[20];
    __int64     m_StartTime;
    __int64     m_FinishTime;
};

// Ack   Tray -> UI struct ����
//---------------------------------------------------------
//  
// Protocol : VMT_TU_NotifyMachineStopResult
//                
//---------------------------------------------------------
struct sTrayUI_NotifyMachineStopResult
{
    sTrayUI_Available   Data;
    int             m_iBreakStatus;//1 : Break   2: Release
    TCHAR       m_szMchnID[EEV2_STRING_MAX_MACHINE_ID];
    TCHAR       m_szMchnTp[EEV2_STRING_MAX_MACHINE_TP];
    TCHAR       m_UserID[EEV2_STRING_MAX_USER_ID];    
    TCHAR       m_DriverName[20];
     int            m_iResult;	      // 1:  ���� , 0 : ����

     sTrayUI_NotifyMachineStopResult()
     {
         Default();
     }
     void Default()
     {
         ZeroMemory(this,sizeof(sTrayUI_NotifyMachineStopResult));
     }
     void GetMachineStop(sTrayUI_GetMachineStop *pStop)
     {
          if(pStop==nullptr)return;
          memcpy(&pStop->Data, &Data,sizeof(sTrayUI_Available));
          pStop->m_iBreak = m_iBreakStatus;
     }
};
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
struct sTrayUI_Alram
{
    enum enVmtAlram
    {
        ALRAM_OVER_SPEED = 1,
        ALRAM_FUEL_LIMIT, 
        ALRAM_TIRE_PRESSURE,
        ALRAM_OVRE_TEMP,
        ALRAM_CROSS_BLOCK,
    };
    enVmtAlram enType;
    int               nVaildTime;  //Alram ����Ÿ��(sec)
    double         nValue;        //������ ������ �˶���Ȳ�� �ٰŵ����� ALRAM_CROSS_BLOCK = 1.0(�ִ�)
                                          //0.0�� �������� CROSS_BLOCK �ٿ����� ������ 
    TCHAR        szDesc[EEV2_STRING_MAX_ALRAM_DESC];//ex) SpeedOver, FuelLimit
};
// Ack  Tray -> UI Struct����

//########################################################################
//
//
//                                                              Job  Common
//
//
//########################################################################

//---------------------------------------------------------
//  
// Protocol : VMT_TU_JobOrder
//                
//---------------------------------------------------------
// Notify    Tray -> UI
// 
//============  ITV�� ���
struct sTrayUI_ITVJobOrderSub
{
       TCHAR				szReadyArrivalFlag_0[EEV2_STRING_MAX_ITV_JOB_STAT];
       int						nETA;
       int                      iPlace; //0: None 1:Forward 2:After 3:Center
};
// -> ���� Recv ����
//
//  int                                      iCount; // ����
//  EEv3JobOrder                      JobOrder[N];
//  sTrayUI_ITVJobOrderSub     Sub[N];
//

//===========  RTG�� ���

// -> ���� Recv ����
//
//  int                     iCount; // ����
//  EEv3JobOrder      JobOrder[N];


//---------------------------------------------------------
//  
// Protocol : VMT_TU_JobCancel
//                
//---------------------------------------------------------
// Send   Tray -> UI
struct sTrayUI_JobKey
{
     TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
};
// Ack  UI -> Tray  Struct����


//---------------------------------------------------------
//  
// Protocol : VMT_TU_JobDone
//                
//---------------------------------------------------------
// Notify    Tray -> UI
struct sTrayUI_JobDone
{
    TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
};
// Ack  UI -> Tray  Struct����


//---------------------------------------------------------
//  
// Protocol : VMT_TU_ManualJobDone
//                
//---------------------------------------------------------
// Notify    UI -> Tray
struct sTrayUI_ManualJobDone
{
    TCHAR      jobKey[EEV2_STRING_MAX_JOB_KEY];
};
// Ack  Tray -> UI Struct����

//---------------------------------------------------------
//  
// Protocol : VMT_TU_JobCancelAll
//                
//---------------------------------------------------------
// Notify    Tray -> UI  Struct����
// Ack  UI -> Tray  Struct����

#pragma pack()