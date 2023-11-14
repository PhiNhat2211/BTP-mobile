#pragma once
#include "EEv2StringMax.h"
//===================================================
// lion - EEv2Location
//===================================================

//const		TCHAR*	LocationType[]=
//{
//	_T("UNDEFINED"),
//	_T("VESSEL"),
//	_T("YARD"),
//	_T("RAIL"),
//	_T("TPW"),
//	_T("TPL"),
//	_T("IP"),
//	_T("AP"),
//	_T("LANE"),
//};
#pragma pack(1)
//===================================================
// Job관련 정보를 받을때 포함되는 목적위치의 정보 구조체
//===================================================
struct		EEv2_Job_Location
{
	TCHAR			locTp[EEV2_STRING_MAX_LOCATION_TYPE];
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
	TCHAR			blck[EEV2_STRING_MAX_LOCATION_BLOCK];		
	/*
	"Block Name 
e.g) 1A, 2A, 1B, …."
	*/
	TCHAR			bay[EEV2_STRING_MAX_LOCATION_BAY];
	/*
	"Bay Name
e.g) 1, 2, 3, 4, …."
	*/
	TCHAR			row[EEV2_STRING_MAX_LOCATION_ROW];
	/*
	"Row Name
e.g) A, B, C, D, …."
	*/
	TCHAR			tier[EEV2_STRING_MAX_LOCATION_TIER];
	/*
	"Tier Name
e.g) 1, 2, 3, 4, …."
	*/
	TCHAR			lane[EEV2_STRING_MAX_LOCATION_LANE];
	//W : Water-side //L :Land-side
	TCHAR			location[EEV2_STRING_MAX_LOCATION_LOCATION];
	/*
	"Location                                      if Lane Type
e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
	*/
};

struct		EEv2_Yard_Location
{
		TCHAR			blck[EEV2_STRING_MAX_LOCATION_BLOCK];		
	/*
	"Block Name 
e.g) 1A, 2A, 1B, …."
	*/
	TCHAR			bay[EEV2_STRING_MAX_LOCATION_BAY];
	/*
	"Bay Name
e.g) 1, 2, 3, 4, …."
	*/
	TCHAR			row[EEV2_STRING_MAX_LOCATION_ROW];
	/*
	"Row Name
e.g) A, B, C, D, …."
	*/
	TCHAR			tier[EEV2_STRING_MAX_LOCATION_TIER];
	/*
	"Tier Name
e.g) 1, 2, 3, 4, …."
	*/
};


struct		EEv2_Def_Location
{
	TCHAR			locTp[EEV2_STRING_MAX_LOCATION_TYPE];
	/*
	enum enType- 다 대문자임
	{
		"Vessel",
		"Yard",
		"Rail",
		"TP",
		"IP",
		"Lane"
	};
	*/
	TCHAR			blck[EEV2_STRING_MAX_LOCATION_BLOCK];		
	/*
	"Block Name 
e.g) 1A, 2A, 1B, …."
	*/
	TCHAR			bay[EEV2_STRING_MAX_LOCATION_BAY];
	/*
	"Bay Name
e.g) 1, 2, 3, 4, …."
	*/
	TCHAR			row[EEV2_STRING_MAX_LOCATION_ROW];
	/*
	"Row Name
e.g) A, B, C, D, …."
	*/
	TCHAR			tier[EEV2_STRING_MAX_LOCATION_TIER];
	/*
	"Tier Name
e.g) 1, 2, 3, 4, …."
	*/
	TCHAR			lane[EEV2_STRING_MAX_LOCATION_LANE];
	//W : Water-side //L :Land-side
	TCHAR			location[EEV2_STRING_MAX_LOCATION_LOCATION];
	/*
	"Location                                      if Lane Type
e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
	*/
	static		void		Set( EEv2_Def_Location *pDes,
		TCHAR*	locTp,
		TCHAR* blck,
		TCHAR* bay,
		TCHAR* row,
		TCHAR* tier,
		TCHAR*	location)
	{
		_stprintf_s(pDes->locTp,EEV2_STRING_MAX_LOCATION_TYPE,locTp);			
		_stprintf_s( pDes->blck,EEV2_STRING_MAX_LOCATION_BLOCK,blck);			
		_stprintf_s( pDes->bay,EEV2_STRING_MAX_LOCATION_BAY,bay);
		_stprintf_s( pDes->row,EEV2_STRING_MAX_LOCATION_ROW,row);
		_stprintf_s( pDes->tier,EEV2_STRING_MAX_LOCATION_TIER,tier);
		_stprintf_s( pDes->location,EEV2_STRING_MAX_LOCATION_LOCATION,location);
		
	}
};

struct EEv2_Area
{
	TCHAR from[EEV2_STRING_MAX_AREA_FROM];
	TCHAR to[EEV2_STRING_MAX_AREA_TO];
};
struct EEv2_NoWorkLocation
{
	TCHAR blck[EEV2_STRING_MAX_AREA_BLOCK];
	TCHAR noWorkTp[EEV2_STRING_MAX_AREA_TYPE];
	EEv2_Area bay;
	EEv2_Area row;
	EEv2_Area tier;

	EEv2_NoWorkLocation()
	{
		ZeroMemory(this,sizeof(EEv2_NoWorkLocation));
	}
};

struct EEv3_Spreader
{
    TCHAR   sprdMd[EEV2_STRING_MAX_SPREADER_MODE]; //sy
    //    "SINGLE"
    //    "TWIN"
    //    "TANDEM"
    TCHAR   sprdTp[EEV2_STRING_MAX_SPREADER_TP]; //sy
    //    "SINGLE_SPREADER20"
    //    "SINGLE_SPREADER40"
    //    "SINGLE_SPREADER45"
    TCHAR   sprdSts[EEV2_STRING_MAX_SPREADER_STATE]; //sy
    //   "LS_SPREADER_LOCKED"
    //   "LS_SPREADER_UNLOCKED"
};


#pragma pack()