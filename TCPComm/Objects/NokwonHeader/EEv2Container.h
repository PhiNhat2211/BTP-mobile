#pragma once
#include "EEv2StringMax.h"
//===================================================
// lion - EEv2Container
//===================================================


#pragma pack(1)

#define	COUNT_MAX_DAMAGE			10

struct		EEv2_Def_Damage
{
	TCHAR			dmgCd[EEV2_STRING_MAX_DMG_CD];//
	/*
	"Damage Code
e.g) AS :  ALL SNOW
      PU : PUSHED
      BM : Band Missing"

	*/
	TCHAR			dmgInOut[EEV2_STRING_MAX_DMG_INOUT]; //Inside / Outside
	/*
	"Inside / Outside
e.g) I : Inside, O : Outside"

	*/
	TCHAR			dmgPart[EEV2_STRING_MAX_DMG_PART];//Damage part
	/*
	"Damage part
e.g) F : Fore, R : Rear, L : Left, R : Right, T : Top, B : BottomR)"

	*/
	TCHAR			dmgRange[EEV2_STRING_MAX_DMG_RANGE];//Damage Range
	/*
	"Damage Range
e.g) 10*10*10"

	*/
	TCHAR			dmgDesc[EEV2_STRING_MAX_DMG_DESC];//ISO Code

	EEv2_Def_Damage()
	{
		ZeroMemory(this,sizeof(EEv2_Def_Damage));
	}
};

#define	COUNT_MAX_SEAL			4

struct		EEv2_Def_Seal
{
	TCHAR			sealNo[EEV2_STRING_MAX_SEAL_SEALNO];//IMDG
	/*
	"Seal No
e.g.) 12311231231"

	*/
	TCHAR			sealTp[EEV2_STRING_MAX_SEAL_TYPE];
	/*
	"C : Custom
	O : Operator"

	*/
	EEv2_Def_Seal()
	{
		ZeroMemory(this,sizeof(EEv2_Def_Seal));
	}
};

struct		EEv2_Def_Reefer
{

   float      reeferTemp;      //온도
   float      workTemp;/////////////@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   TCHAR   plugCd[EEV2_STRING_MAX_REEFER_PLUGCD];
   /*
   "status of Plug
   PIW : Wait for Plug-In
   PIM : Monitoring of Plug-In
   POW : Wait for Plug-Out
   POC :  Completed of Plug-Out"
   */
   TCHAR   unit;
   /*
   "unit of temperature
   F : Fahrenheit
   C : Celsius "
   */
};

#define	COUNT_MAX_IMDG			10

struct		EEv2_Def_Imdg
{
	TCHAR			imdg[EEV2_STRING_MAX_IMDG_IMDG];//IMDG
	TCHAR			unNo[EEV2_STRING_MAX_IMDG_UNNO];//UNNO
	TCHAR			fireCd[EEV2_STRING_MAX_IMDG_FIRECODE];//Fire Code

	EEv2_Def_Imdg()
	{
		ZeroMemory(this,sizeof(EEv2_Def_Imdg));
	}
};

//===================================================
// Job관련 정보를 받을때 포함되는 컨테이너 정보구조체
//===================================================
struct		EEv2_Job_Container
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];	
	TCHAR		cntrIso[EEV2_STRING_MAX_CNTR_ISO]; 
	TCHAR		cntrTp[EEV2_STRING_MAX_CNTR_TYPE];
	TCHAR		cls[EEV2_STRING_MAX_CNTR_CLASS];//수출 수입
	TCHAR		opr[EEV2_STRING_MAX_CNTR_OPRCODE]; //e.g) HJS
	TCHAR		cntrCgoTp[EEV2_STRING_MAX_CNTR_CARGO_TYPE];
	TCHAR		fullMty[EEV2_STRING_MAX_CNTR_FULL_MTY]; 

};

struct		EEv2_Visual_Container
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];
	/*
	"12-character container ID.  
	e.g.) AWSU1234567"
	*/
	
	TCHAR		cntrIso[EEV2_STRING_MAX_CNTR_ISO];
	/*
	e.g.) 22G0
	*/	
	BYTE		cntrTp;
	/*
	"Container Type
	GE : General
	RF :  Reefer
	TK : Tank
	FR : Flat Rack
	OT : Open Top
	BK : Dry Bulk
	AS : Air Surface"
	// SP : SpatialType(  8)
	*/
	TCHAR		cntrSpTp[EEV2_STRING_MAX_CNTR_STYPE_DESC];
	/*
	"Container Special Type
 e.g) AKD,OOG (AKD-Awkward,OOG -Out of Gauge)"
	*/


	TCHAR		opr[EEV2_STRING_MAX_CNTR_OPRCODE];
	/*
	e.g) HJS
	*/

	BYTE		cls;
	/*
	"Class : Import/Export
	II: Import
	TI: Trashipment Import
	XX: Export
	TX : Transhipment Export
	TL: Transshipment
	S1: Shift 1Time
	S2: Shift 2Time
	YY: Storage Empty
	OT : Through Cargo"

	*/
	BYTE		cntrCgoTp;
	/*
	"Cargo Type
 G : General
 M: Empty
 MH : Empty Hazardous
 R : Reefer
 RH : Reefer Hazardous
 H : Hazardous"
	*/
	BYTE	fullMty; // 
	/*
	"F: Full
	 M: Empty"
	*/

	TCHAR		location[EEV2_STRING_MAX_LOCATION_LOCATION];
	/*
	61A-34-A-2
	VIRTUAL BLOCK
	1A-17-C-5
	ZONE 47
	*/
};


struct		EEv2_CNTR
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];
	/*
	"12-character container ID.  
	e.g.) AWSU1234567"
	*/
	TCHAR		cntrIso[8];
	/*
	e.g.) 22G0
	*/
	TCHAR		cntrLen[3];//
	/*
	e.g.) 20,40,45,48,53
	*/
	BYTE		cntrTp;
	/*
	"Container Type
	GE : General
	RF :  Reefer
	TK : Tank
	FR : Flat Rack
	OT : Open Top
	BK : Dry Bulk
	AS : Air Surface"
	// SP : SpatialType(  8)
	*/

	TCHAR		opr[16];
	/*
	e.g) HJS
	*/
	TCHAR		cntrSpTp[8];
	/*
	"Container Special Type
 e.g) AKD,OOG (AKD-Awkward,OOG -Out of Gauge)"
	*/
	BYTE		cls;
	/*
	"Class : Import/Export
	II: Import
	TI: Trashipment Import
	XX: Export
	TX : Transhipment Export
	TL: Transshipment
	S1: Shift 1Time
	S2: Shift 2Time
	YY: Storage Empty
	OT : Through Cargo"

	*/
	BYTE		cntrCgoTp;
	/*
	"Cargo Type
 G : General
 M: Empty
 MH : Empty Hazardous
 R : Reefer
 RH : Reefer Hazardous
 H : Hazardous"
	*/
	BYTE	fullMty; // 
	/*
	"F: Full
	 M: Empty"
	*/

	TCHAR		location[EEV2_STRING_MAX_LOCATION_LOCATION];
	/*
	
	
	*/
};


struct		EEv2_CNTR_LOAD
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];
	/*
	"12-character container ID.  
	e.g.) AWSU1234567"
	*/
	TCHAR		cntrIso[8];
	/*
	e.g.) 22G0
	*/
	TCHAR		cntrLen[3];//
	/*
	e.g.) 20,40,45,48,53
	*/
	BYTE		cntrTp;
	/*
	"Container Type
	GE : General
	RF :  Reefer
	TK : Tank
	FR : Flat Rack
	OT : Open Top
	BK : Dry Bulk
	AS : Air Surface"
	// SP : SpatialType(  8)
	*/

	TCHAR		opr[16];
	/*
	e.g) HJS
	*/	
};
struct		EEv2_Def_Container
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];	
	TCHAR		cntrIso[EEV2_STRING_MAX_CNTR_ISO]; 

	TCHAR		cntrTp[EEV2_STRING_MAX_CNTR_TYPE];
	/*
	"Container Type
	GE : General
	RF :  Reefer
	TK : Tank
	FR : Flat Rack
	OT : Open Top
	BK : Dry Bulk
	AS : Air Surface"
	*/

	TCHAR		cntrLen[EEV2_STRING_MAX_CNTR_LENGTH]; 
	TCHAR		cntrHgt[EEV2_STRING_MAX_CNTR_HEIGHT]; 
	TCHAR		opr[EEV2_STRING_MAX_CNTR_OPRCODE]; //e.g) HJS
	TCHAR		cntrWgt[EEV2_STRING_MAX_CNTR_WEIGHT];
	TCHAR		cls[EEV2_STRING_MAX_CNTR_CLASS];//수출 수입
	/*
	"Class : Import/Export
	II: Import
	TI: Trashipment Import
	XX: Export
	TX : Transhipment Export
	TL: Transshipment
	S1: Shift 1Time
	S2: Shift 2Time
	YY: Storage Empty
	OT : Through Cargo"
	*/
	TCHAR		cntrSpTp[EEV2_STRING_MAX_CNTR_STYPE_DESC];
	TCHAR		cntrCgoTp[EEV2_STRING_MAX_CNTR_CARGO_TYPE];
	/*
	"Cargo Type
 G : General
 M: Empty
 MH : Empty Hazardous
 R : Reefer
 RH : Reefer Hazardous
 H : Hazardous"
	*/
	TCHAR		fullMty[EEV2_STRING_MAX_CNTR_FULL_MTY];
	/*
	"F: Full
M: Empty"
	*/
	TCHAR		pod[EEV2_STRING_MAX_CNTR_PORT_OF_DISCHARGE];
	TCHAR		nPod[EEV2_STRING_MAX_CNTR_NEXT_PORT];
	TCHAR		pol[EEV2_STRING_MAX_CNTR_PORT_OF_LOAD];
	TCHAR		fPol[EEV2_STRING_MAX_CNTR_FINAL_PORT];
	TCHAR		cntrGrade[EEV2_STRING_MAX_CNTR_GRADE];

	TCHAR		doorDirect[EEV2_STRING_MAX_CNTR_DOOR_DIRECTION];
	/*
	Fore:F
	After:A
	*/
	BOOL			isDmg;
	BOOL			isSeal;
	BOOL			isHold;
	EEv2_Def_Reefer				reefer;
	EEv2_Def_Damage			dmg[COUNT_MAX_DAMAGE];
	EEv2_Def_Seal					seal[COUNT_MAX_SEAL];	
	EEv2_Def_Imdg				imdg[COUNT_MAX_IMDG];

};

struct		EEv2_TC_Container
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];	
	TCHAR		cntrIso[EEV2_STRING_MAX_CNTR_ISO]; 
	TCHAR		cntrTp[EEV2_STRING_MAX_CNTR_TYPE];
	TCHAR		cls[EEV2_STRING_MAX_CNTR_CLASS];//수출 수입
	TCHAR		opr[EEV2_STRING_MAX_CNTR_OPRCODE]; //e.g) HJS
	TCHAR		cntrCgoTp[EEV2_STRING_MAX_CNTR_CARGO_TYPE];
	TCHAR		fullMty[EEV2_STRING_MAX_CNTR_FULL_MTY]; 
	TCHAR		cntrGrade[EEV2_STRING_MAX_CNTR_GRADE];
	EEv2_TC_Container()
	{
		ZeroMemory(this,sizeof(EEv2_TC_Container));
	}
};

//- 상차(pick/laden)관련 간략 정보 
struct		EEv2_Load_Container
{
	/*
	"12-character container ID.  
	e.g.) AWSU1234567"
	*/
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];
	
	/*
	e.g.) 22G0
	*/
	TCHAR		cntrIso[8];
	
	/*
	e.g.) 20,40,45,48,53
	*/
	TCHAR		cntrLen[3];//
	
	/*
	"Container Type
	GE : General
	RF :  Reefer
	TK : Tank
	FR : Flat Rack
	OT : Open Top
	BK : Dry Bulk
	AS : Air Surface"
	// SP : SpatialType(  8)
	*/
	BYTE		cntrTp;
	
	/*
	e.g) HJS
	*/
	TCHAR		opr[16];
		
};

struct		EEv2_Summery_Container
{
	TCHAR		cntrNo[EEV2_STRING_MAX_CNTR_NO];		
	TCHAR		cntrIso[EEV2_STRING_MAX_CNTR_ISO]; 
};

struct		EEv3_Job_Container		: public EEv2_Job_Container
{
    BOOL		isHold;
    BOOL		isSeal;
    BOOL		isDamage;
};




#pragma pack()