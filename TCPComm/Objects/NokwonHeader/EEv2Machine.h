#pragma once
#include "EEv2StringMax.h"
//===================================================
// lion - EEv2Machine
//===================================================
#pragma pack(1)
//===================================================
// Job관련 정보를 받을때 포함되는 대상 Machine의 정보 구조체
//===================================================
struct		EEv2_Job_Machine
{
	//머신 아이디
	TCHAR		mchnId[EEV2_STRING_MAX_MACHINE_ID];
	/*
	"Machine Code
 e.g) TT501, RS202,QC103"
	*/
	//머신 타입
	TCHAR		mchnTp[EEV2_STRING_MAX_MACHINE_TP];
	/*
	"Machine Type
 e.g) RS : Reach Stacker
       TC : Transfer Crane
       TH : Top Handler
       QC : Quay Crane
       YT : Internal Terminal Truck( Yard Tractor)"
	*/
	//장비 상태
	TCHAR		mchnSts[EEV2_STRING_MAX_MACHINE_STATUS];

	TCHAR		vrtlFlg[EEV2_STRING_MAX_MACHINE_VRTFLG]; //Virtual Crane Flag "Y",""
};
//===================================================
// 초기 TOS로 부터 배정된(Assigned) 머신정보를 담을 정보 구조체
//===================================================
struct		EEv2_Def_Machine
{
	//머신 아이디
	TCHAR		mchnId[EEV2_STRING_MAX_MACHINE_ID];
	/*
	"Machine Code
 e.g) TT501, RS202,QC103"
	*/
	//머신 타입
	TCHAR		mchnTp[EEV2_STRING_MAX_MACHINE_TP];
	/*
	"Machine Type
 e.g) RS : Reach Stacker
       TC : Transfer Crane
       TH : Top Handler
       QC : Quay Crane
       YT : Internal Terminal Truck( Yard Tractor)"
	*/
	bool			isLogOn;
	bool			isOn;
	//장비 상태
	TCHAR		mchnSts[EEV2_STRING_MAX_MACHINE_STATUS];
	/*
	Status (Idle/Standby/Parking/Active)
  e.g) S: Standby, P: Parking, I : Idle, A : Active
	*/
	TCHAR		vrtlFlg[EEV2_STRING_MAX_MACHINE_VRTFLG]; 
	//Virtual Crane Flag "Y",""
};

struct		EEv2_Summary_Machine
{
	//머신 아이디
	TCHAR		mchnId[EEV2_STRING_MAX_MACHINE_ID];
	/*
	"Machine Code
 e.g) TT501, RS202,QC103"
	*/
	//머신 타입
	TCHAR		mchnTp[EEV2_STRING_MAX_MACHINE_TP];
	/*
	"Machine Type
 e.g) RS : Reach Stacker
       TC : Transfer Crane
       TH : Top Handler
       QC : Quay Crane
       YT : Internal Terminal Truck( Yard Tractor)"
	*/
};


#pragma pack()