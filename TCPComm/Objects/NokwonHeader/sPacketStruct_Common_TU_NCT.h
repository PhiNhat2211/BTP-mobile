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
// 로그인 하기 위한 ID 입력시 접속자에 대한 확인정보
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
     int             m_iResult;                // 0 접속불허,    1 접속허가.
     int             m_iLoginResult;        //  0 login이 안된상태   1  login이된상태 
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
//STruct 없음

// Ack  Tray -> UI
struct sTrayUI_ConnectionStatus
{
    int             m_iEagleEyeStatus;   //0 이면 연결안됨,    1이면 연결됨
    int             m_iGPSStatus;         // 0 이면 연결안됨,    1이면 연결됨
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
    int             m_iMessageType; //0: 없음 1: TOS  2: VT  3: EE.Server 4: Tray
    TCHAR       m_strMessage[EEV2_STRING_MAX_NOTICE_MSG];
    TCHAR       m_strMessage2[EEV2_STRING_MAX_SERVER_MESSAGE]; //EEServer 에만해당
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
// 로그인 하기 위한 ID 입력시 접속자에 대한 확인정보
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
	BOOL  bIsOn; // 허락, 실패
	TCHAR GroupListSeperator[500];// "Group1|Group2|Group3" 형태로 전달됨.
	TCHAR Notice[300];
};

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SetLogin4Machine
//                로그인 사용자의 이름를 화면에 표시한다.
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
	int   iLogin;		          // 0 : 로그인 실패  1: 로그인성공  2: 이미로그인상태
	TCHAR UserName[50];
	TCHAR Notice[300];
};




//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendMachineStatusChange
//                로그인 사용자의 이름를 화면에 표시한다.
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
    int            m_iResult;		      // 1:  성공 , 0 : 실패
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
// struct 없음.
// Request  Tray -> UI
// struct 없음.

//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendPubx05
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct 없음.
// Request  Tray -> UI
// struct 없음.


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendPubx06
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct 없음.
// Request  Tray -> UI
// struct 없음.


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendHighForward
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct 없음.
// Request  Tray -> UI
// struct 없음.


//---------------------------------------------------------
//  
// Protocol : VMT_TU_SendHighBackward
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct 없음.
// Request  Tray -> UI
// struct 없음.

//---------------------------------------------------------
//  
// Protocol : VMT_TU_RequestCfgekf
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct 없음.
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
// struct 없음.
// Request  Tray -> UI
// struct 없음.


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
// struct 없음.


// Response , Notify : Tray -> UI
struct sTrayUI_Available
{
    TCHAR	ReasonCd[EEV2_STRING_MAX_REASON_CODE];
    TCHAR	ReasonNm[EEV2_STRING_MAX_REASON_NAME];
};
// 
// -> Recv 형태
// struct sTrayUI_MachineStopCodeList
// {
//       int              m_iAvailableCount;   //갯수
//       sTrayUI_Available     m_pData[N];                //
// };


//---------------------------------------------------------
//  
// Protocol : VMT_TU_GettMachineStop
//                
//---------------------------------------------------------
// Request  UI -> Tray
// struct 없음.

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

// Ack   Tray -> UI struct 없음
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
     int            m_iResult;	      // 1:  성공 , 0 : 실패

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
    int               nVaildTime;  //Alram 유지타임(sec)
    double         nValue;        //서버가 감지한 알람상황의 근거데이터 ALRAM_CROSS_BLOCK = 1.0(최대)
                                          //0.0에 가까울수록 CROSS_BLOCK 근원지와 가까운것 
    TCHAR        szDesc[EEV2_STRING_MAX_ALRAM_DESC];//ex) SpeedOver, FuelLimit
};
// Ack  Tray -> UI Struct없음

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
//============  ITV의 경우
struct sTrayUI_ITVJobOrderSub
{
       TCHAR				szReadyArrivalFlag_0[EEV2_STRING_MAX_ITV_JOB_STAT];
       int						nETA;
       int                      iPlace; //0: None 1:Forward 2:After 3:Center
};
// -> 실제 Recv 형태
//
//  int                                      iCount; // 갯수
//  EEv3JobOrder                      JobOrder[N];
//  sTrayUI_ITVJobOrderSub     Sub[N];
//

//===========  RTG의 경우

// -> 실제 Recv 형태
//
//  int                     iCount; // 갯수
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
// Ack  UI -> Tray  Struct없음


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
// Ack  UI -> Tray  Struct없음


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
// Ack  Tray -> UI Struct없음

//---------------------------------------------------------
//  
// Protocol : VMT_TU_JobCancelAll
//                
//---------------------------------------------------------
// Notify    Tray -> UI  Struct없음
// Ack  UI -> Tray  Struct없음

#pragma pack()