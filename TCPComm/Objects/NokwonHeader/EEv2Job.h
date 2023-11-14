#pragma once
#include "EEv2StringMax.h"
#include "EEv2Machine.h"
#include "EEv2Location.h"
#include "EEv2Container.h"
//===================================================
// lion - EEv2Job
//===================================================
#pragma pack(1)
//===================================================
// lion - version	2014.01.24
//===================================================


struct		EEv2_Job_Type
{
	TCHAR			jobTp[EEV2_STRING_MAX_JOBTYPE_TYPE];
	/*
	"DS : Discharge
LD : Load
MI : Movement In Yard
MO : Movement Out
RH : Rehandleing
AH : Auto Rehandleing
LC : Loading Cancel
GI : Gate In
GO : Gate Out
GC : Gate Out Cancel"
	*/
	TCHAR			jobStatus[EEV2_STRING_MAX_JOBTYPE_STATUS];//2
	//Active = "A", Inactive="Q", Processing = "P", Completed = "C"
	TCHAR			vslCd[EEV2_STRING_MAX_JOBTYPE_VESSEL_CODE];//10
	//<vslCd>HJLN</vslCd> // DS,LOAD 때만 기입
	TCHAR			voyNo[EEV2_STRING_MAX_JOBTYPE_VOY_NO];//16
	TCHAR			planSeq[EEV2_STRING_MAX_JOBTYPE_PLAN_SQ];//16
	TCHAR			twinTandemFlg[EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_FLAG];
	TCHAR			twinTandumKey[EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_KEY];
	TCHAR			tandemJoinYT[EEV2_STRING_MAX_MACHINE_ID];
	TCHAR			jobFlagInfo[EEV2_STRING_MAX_JOBTYPE_JOB_FLAG_INFO];
	TCHAR			conePlan[EEV2_STRING_MAX_JOBTYPE_CONE_PLAN];
};

struct		EEv3_Job_Type	: public EEv2_Job_Type
{
    TCHAR etw[EEV2_STRING_MAX_JOBTYPE_ETW];
    TCHAR eventId[EEV2_STRING_MAX_JOBTYPE_EVENT_ID];
    TCHAR queue[EEV2_STRING_MAX_JOBTYPE_QUEUE];
};

struct		EEv3JobOrder
{
    TCHAR						    key[EEV2_STRING_MAX_JOB_KEY];
    EEv2_Job_Machine			workingMchn;
    EEv2_Job_Machine			partnerMchn;
    EEv2_Job_Location			loc;           //일할장소 또는 RH 작업 시 to 위치
    EEv2_Job_Location			locForm;	 //RH 작업 시 from-to 구조일때 from위치

    // 1by1 고려하여 추가된사항 v3로 정의 추가
    EEv3_Job_Container		cntr;		
    EEv3_Job_Type				type;
    EEv3JobOrder()
    {
        ZeroMemory(key, sizeof(TCHAR)*EEV2_STRING_MAX_JOB_KEY);
        ZeroMemory(&workingMchn, sizeof(EEv2_Job_Machine));
        ZeroMemory(&partnerMchn, sizeof(EEv2_Job_Machine));
        ZeroMemory(&loc, sizeof(EEv2_Job_Location));
        ZeroMemory(&locForm, sizeof(EEv2_Job_Location));
        ZeroMemory(&cntr, sizeof(EEv3_Job_Container));
        ZeroMemory(&type, sizeof(EEv3_Job_Type));
    }
    void        SetEEv3JobOrder(EEv3JobOrder *pKap)
    {
        memcpy(this, pKap, sizeof(EEv3JobOrder));
    }
    void        GetEEv3JobOrder(EEv3JobOrder *pGetKap)
    {
        memcpy(pGetKap, this, sizeof(EEv3JobOrder));
    }
};


struct sITVJobOrderSub
{
    TCHAR				szReadyArrivalFlag_0[EEV2_STRING_MAX_ITV_JOB_STAT];
    int						nETA;
    int                      iPlace; //0: None 1:Forward 2:After 3:Center
};

struct		EEv3JobOrderSub : public EEv3JobOrder
{
    int                             m_iArrowIndex;
    sITVJobOrderSub        m_sITVJobSub;
    EEv3JobOrderSub()
    {
           m_iArrowIndex = 0;
           memset(&m_sITVJobSub,0,sizeof(sITVJobOrderSub));
    }
};


struct		EEv2JobOrder
{
	EEv2_Job_Machine			workingMchn;
	EEv2_Job_Machine			partnerMchn;
	EEv2_Job_Container			cntr;	
	EEv2_Job_Location			loc;
	EEv2_Job_Type				type;

	void mchn_swap()
	{
		EEv2_Job_Machine	mchnTemp;
		ZeroMemory(&mchnTemp,sizeof( EEv2_Job_Machine));
		
		memcpy( &mchnTemp,&workingMchn,sizeof(EEv2_Job_Machine));
		memcpy( &workingMchn,&partnerMchn,sizeof(EEv2_Job_Machine));
		memcpy( &partnerMchn,&mchnTemp,sizeof(EEv2_Job_Machine));
	}
	void	Set_locTp( TCHAR*	locTp )
	{
		_stprintf_s( loc.locTp,locTp);
	}

	bool	Validation()
	{
		if( _tcscmp( workingMchn.mchnId,_T("")) ==0 )
			return false;
		if( _tcscmp( workingMchn.mchnTp,_T("")) ==0 )
			return false;

		if( _tcscmp( cntr.cntrNo,_T("")) ==0 )
			return false;
		/*if( _tcscmp( cntr.cntrIso,_T("")) ==0 )
			return false;*/
		if( _tcscmp( loc.locTp,_T("")) ==0 )
			return false;
		if( loc.locTp[0] ==_T('T') && loc.locTp[1] ==_T('P') )
		{
			if( _tcscmp( loc.blck,_T("")) ==0 )
				return false;
			if( _tcscmp( loc.bay,_T("")) ==0 )
				return false;
			if( _tcscmp( loc.row,_T("")) ==0 )
				return false;
			if( _tcscmp( loc.tier,_T("")) ==0 )
				return false;
		}
		if( loc.locTp[0] ==_T('L') && loc.locTp[1] ==_T('A') )
		{
			if( _tcscmp( loc.location,_T("")) ==0 )
				return false;
		}
		return true;
	}
};

struct		EEv2OtrJob
{
	EEv2_Job_Machine			partnerMchn;
	EEv2_Job_Container			cntr[2];	
	EEv2_Job_Location			loc;
	EEv2_Job_Type				type;
};


struct		EEv2_ITV_Job
{
	EEv2JobOrder		sJob_Order_0;
	TCHAR				szReadyArrivalFlag_0[EEV2_STRING_MAX_ITV_JOB_STAT];
	int						nETA_0;
	EEv2JobOrder		sJob_Order_1;
	TCHAR				szReadyArrivalFlag_1[EEV2_STRING_MAX_ITV_JOB_STAT];
	int						nETA_1;
	int						nReserved_0;
	int						nReserved_1;
	int						nReserved_2;
	int						nReserved_3;

	EEv2_ITV_Job()
	{
		ZeroMemory(&sJob_Order_0,sizeof(EEv2JobOrder));
		ZeroMemory(&sJob_Order_1,sizeof(EEv2JobOrder));
		memset(szReadyArrivalFlag_0,0,EEV2_STRING_MAX_ITV_JOB_STAT);
		memset(szReadyArrivalFlag_1,0,EEV2_STRING_MAX_ITV_JOB_STAT);
		nETA_0 = 0;
		nETA_1 = 0;
		nReserved_0 = 0;
		nReserved_1 = 0;
		nReserved_2 = 0;
		nReserved_3 = 0;
	}
};
#pragma pack()