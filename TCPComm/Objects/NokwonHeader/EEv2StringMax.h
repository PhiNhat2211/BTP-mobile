﻿#pragma once
//===================================================
// lion - version	2014.01.24
// lion - 스트링 갯수 - 중앙 통제 -( 절대 바이트 사이즈 아님)
//===================================================
#define		EEV2_STRING_MAX_ROOT							20

#define		EEV2_STRING_MAX_MACHINE_ID				20
#define		EEV2_STRING_MAX_MACHINE_TP				EEV2_STRING_MAX_MACHINE_ID
#define		EEV2_STRING_MAX_MACHINE_STATUS		3
#define		EEV2_STRING_MAX_MACHINE_VRTFLG		3

#define		EEV2_STRING_MAX_MACHINE_MOVE_TYPE	3 //EE_YtTask moveTp
#define		EEV2_STRING_MAX_MACHINE_NOTICE_TYPE	5	//EE_YtTask noticeTp

#define		EEV2_STRING_MAX_MACHINE_MATCH_TO_DESTINATION	5	//EE_YtTask matchToDestination

#define		EEV2_STRING_MAX_CHASSIS_NO				20

#define		EEV2_STRING_MAX_LOCATION_TYPE		10
#define		EEV2_STRING_MAX_LOCATION_BLOCK			9+1 //FENCE

#define		EEV2_STRING_MAX_LOCATION_BAY				3+1
#define		EEV2_STRING_MAX_LOCATION_ROW				3+1
#define		EEV2_STRING_MAX_LOCATION_TIER				2+1
#define		EEV2_STRING_MAX_LOCATION_LANE				2+1
#define		EEV2_STRING_MAX_LOCATION_LOCATION		30
#define		EEV2_STRING_MAX_LOCATION_BAY_NAME			10

#define		EEV2_STRING_MAX_BOUND_ID		20


#define		EEV2_STRING_MAX_VESSEL_DECKHOLD			5
#define		EEV2_STRING_MAX_VESSEL_BLOCK				20
#define		EEV2_STRING_MAX_VESSEL_BAY					10
#define		EEV2_STRING_MAX_VESSEL_ROW					10
#define		EEV2_STRING_MAX_VESSEL_TIER					10
#define		EEV2_STRING_MAX_VESSEL_CNTR_NO			16
#define		EEV2_STRING_MAX_VESSEL_MACHINE_ID		10


#define		EEV2_STRING_MAX_CNTR_NO						13
#define		EEV2_STRING_MAX_CNTR_ISO						10
#define		EEV2_STRING_MAX_CNTR_TYPE						4
#define		EEV2_STRING_MAX_CNTR_STYPE_DESC			8
#define		EEV2_STRING_MAX_CNTR_OPRCODE				10
#define		EEV2_STRING_MAX_CNTR_CLASS					4
#define		EEV2_STRING_MAX_CNTR_CARGO_TYPE			2+1
#define		EEV2_STRING_MAX_CNTR_FULL_MTY			2+1
#define		EEV2_STRING_MAX_CNTR_LENGTH			10
#define		EEV2_STRING_MAX_CNTR_HEIGHT				10
#define		EEV2_STRING_MAX_CNTR_WEIGHT				10
#define		EEV2_STRING_MAX_CNTR_PORT_OF_DISCHARGE				16
#define		EEV2_STRING_MAX_CNTR_NEXT_PORT				16
#define		EEV2_STRING_MAX_CNTR_PORT_OF_LOAD				16
#define		EEV2_STRING_MAX_CNTR_FINAL_PORT				16
#define		EEV2_STRING_MAX_CNTR_GRADE					8
#define		EEV2_STRING_MAX_CNTR_DOOR_DIRECTION			3
#define		EEV2_STRING_MAX_JOB_KEY						100
#define		EEV2_STRING_MAX_TIMESTAMP					30
#define		EEV2_STRING_MAX_ETW							20

#define		EEV2_STRING_MAX_JOBTYPE_TYPE							3
#define		EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_FLAG			3
// TandemKey OverLength감지하여 Warning으로 대체
#define		EEV2_STRING_MAX_JOBTYPE_TWIN_TANDEM_KEY			40 
#define		EEV2_STRING_MAX_JOBTYPE_STATUS						2
#define		EEV2_STRING_MAX_JOBTYPE_VESSEL_CODE							10
#define		EEV2_STRING_MAX_JOBTYPE_VOY_NO					16
#define		EEV2_STRING_MAX_JOBTYPE_QUEUE						20 //
#define		EEV2_STRING_MAX_JOBTYPE_PLAN_SQ					20 //
#define		EEV2_STRING_MAX_JOBTYPE_JOB_FLAG_INFO			3
#define		EEV2_STRING_MAX_JOBTYPE_CONE_PLAN			50
#define		EEV2_STRING_MAX_JOBTYPE_ETW						EEV2_STRING_MAX_ETW
#define		EEV2_STRING_MAX_JOBTYPE_EVENT_ID				10

#define		EEV2_STRING_MAX_TERMINAL_INOUT_TYPE		5
// STS관련
#define		EEV2_STRING_MAX_SPREADER_MODE			20
#define		EEV2_STRING_MAX_SPREADER_TP				25
#define		EEV2_STRING_MAX_SPREADER_STATE			25


#define		EEV2_STRING_MAX_DMG_CD							10
#define		EEV2_STRING_MAX_DMG_INOUT						3
#define		EEV2_STRING_MAX_DMG_PART							3
#define		EEV2_STRING_MAX_DMG_RANGE						20
#define		EEV2_STRING_MAX_DMG_DESC							128

#define		EEV2_STRING_MAX_IMDG_IMDG		10
#define		EEV2_STRING_MAX_IMDG_UNNO		10
#define		EEV2_STRING_MAX_IMDG_FIRECODE		10

#define		EEV2_STRING_MAX_SEAL_SEALNO						20
#define		EEV2_STRING_MAX_SEAL_TYPE						3

#define		EEV2_STRING_MAX_GANG_POOL					30
#define		EEV2_STRING_MAX_GANG_POOL_NAME			30
#define		EEV2_STRING_MAX_LD_SEQ_CNT					5
#define		EEV2_STRING_MAX_MMF_GANG_POOL				50

#define		EEV2_STRING_MAX_COLORCODE_CODE			10
#define		EEV2_STRING_MAX_COLORCODE_FORE			10
#define		EEV2_STRING_MAX_COLORCODE_BACK			10

#define		EEV2_STRING_MAX_VESSEL_CODE				10
#define		EEV2_STRING_MAX_VESSEL_VESSEL				50

#define		EEV2_STRING_MAX_REASON_CODE		20
#define		EEV2_STRING_MAX_REASON_NAME		30

//sy
#define		EEV2_STRING_MAX_STSTASK_NEIGHBOR		10
#define		EEV2_STRING_MAX_STSTASK_FLAG			10
#define		EEV2_STRING_MAX_STSTASK_ARRIVES			20
//

#define		EEV2_STRING_MAX_DATE_TIME				30

#define		EEV2_STRING_MAX_REEFER_PLUGCD				3+1

#define		EEV2_BLOCKMAP_LIST_MAX		100 //
#define		EEV2_BLOCK_BAY_MAX		150
#define		EEV2_BLOCK_ROW_MAX		10
#define		EEV2_BLOCK_TIER_MAX		10

#define		EEV2_STRING_MAX_USER_ID20		20
#define		EEV2_STRING_MAX_USER_PW20		20

#define		EEV2_STRING_MAX_USER_ID		7
#define		EEV2_STRING_MAX_USER_PW		7
#define		EEV2_STRING_MAX_USER_NAME					20
#define		EEV2_STRING_MAX_USER_GROUP					50
#define		EEV2_STRING_MAX_USER_STATE					10
#define		EEV2_STRING_MAX_CHASSIS_NAME				30

#define		EEV2_STRING_MAX_USER_RELIGION				100
#define		EEV2_STRING_MAX_USER_MOBILE					50

#define		EEV2_STRING_MAX_CONFIGURE_DESC				10
#define		EEV2_STRING_MAX_CONFIGURE_GROUP_NAME	50

#define		EEV2_STRING_MAX_INVEN_INOUT					4
#define		EEV2_STRING_MAX_INVEN_REASON					10

#define		EEV2_STRING_MAX_ORPHAN_REASON_DESC				50
#define		EEV2_STRING_MAX_ORPHAN_ZONE					10



#define		EEV2_STRING_MAX_ATTR_NAME		20
#define		EEV2_STRING_MAX_ATTR_ENTERANCE	10
#define		EEV2_STRING_MAX_ATTR_LANE_TYPE		10
#define		EEV2_STRING_MAX_ATTR_LANE_NAME	10



#define		EEV2_STRING_MAX_PORT_CODE		10
#define		EEV2_STRING_MAX_RFID_READER_ID		EEV2_STRING_MAX_MACHINE_ID
#define		EEV2_STRING_MAX_RFID_READER_LOC		EEV2_STRING_MAX_MACHINE_ID
#define		EEV2_STRING_MAX_RFID_READER_TP		10

#define		EEV2_STRING_MAX_ZONE_ID				10
// YYYYMMDDhhmmss
#define		EEV2_STRING_MAX_TIME					15

#define		EEV2_STRING_MAX_ALERT_TP		10
#define		EEV2_STRING_MAX_ALERT_DESC		100
#define		EEV2_STRING_MAX_ALERT_ARG		30

#define		EEV2_STRING_MAX_ALRAM_DESC		20

#define		EEV2_STRING_MAX_NOTICE_MSG		300

#define		EEV2_STRING_MAX_AREA_TYPE		10
#define		EEV2_STRING_MAX_AREA_BLOCK	10
#define		EEV2_STRING_MAX_AREA_FROM		10
#define		EEV2_STRING_MAX_AREA_TO			10


#define		EEV2_STRING_MAX_TIER_ROW_COUNT		49
#define		EEV2_STRING_MAX_SERVER_MESSAGE		30

#define		EEV2_STRING_MAX_ITV_JOB_STAT			5
#define		EEV2_STRING_MAX_ITV_JOB_ETA				5
#define		EEV2_STRING_MAX_ITV_JOB_KPI_RESULT	10


#define		OCR_STRING_MAX_PLAN_KEY		20


#define		EE_SERVER_MODULE				20

#define		EEV3_STRING_MAX_SPACE_RESERVED		128
#define     EEV2_STRING_MAX_EXECUTE_SITE        10