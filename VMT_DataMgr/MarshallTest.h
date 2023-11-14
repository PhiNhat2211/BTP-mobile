// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the VMT_DATAMGR_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// VMT_DATAMGR_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#pragma once

#include "stdafx.h"


//**********************************************************
//*
//*  Marshalling Test Codes Space [Begine
//*
//**********************************************************

//===========================================================
//= Callback Prototype Declaration
//===========================================================

//Test Callback Function
typedef int (CALLBACK* CLBK_GetCurrentTime)(LPTSTR szTime);

#pragma pack(1)

struct Member
{
	int a;
	TCHAR				mb[12];
	Member()
	{
		a =1;
		_tcscpy(mb, _T("-1200"));
	}
};

typedef struct sNCT_TestDataTag
{
	//----------------Postamble
	bool				boolMember;
	double				doubleMember;
	Member				member[12];
	BYTE				m_btPostamble;
	char				m_szMachinName[12];
	TCHAR				m_tszTrolleyPosition[12];
	int					m_nInstanceID;
	DWORD				m_dwDataSize;
	ULONG				m_ulTime;
	LPTSTR				m_szDLLName;
	CLBK_GetCurrentTime	m_pfnGetCurrentTime;

	sNCT_TestDataTag()
	{
		boolMember = true;
		doubleMember = 1.1;
		m_btPostamble = 0x7E;
		strcpy(m_szMachinName,"MT7876");
		_tcscpy(m_tszTrolleyPosition, _T("-1200"));
		m_nInstanceID = 1004;
		m_dwDataSize = sizeof(sNCT_TestDataTag);
		m_ulTime = 412300000;
		m_szDLLName = NULL;
		m_pfnGetCurrentTime = NULL;
		
	}

	int GetInstanceID() { return m_nInstanceID; };
}sNCT_TestData;


struct SaveXML
{
	bool AutoSeach1;     // xml com port 첫번째 
	int Port1;
	bool AutoSeach2;    // xml com port 두번째
	int Port2;
};

#pragma pack()

#ifdef __cplusplus
extern "C" {
#endif

VMT_DATAMGR_API void  Callback_GetCurrentTime_test(TCHAR* value);
VMT_DATAMGR_API sNCT_TestData* GetCVMT_TestData();
VMT_DATAMGR_API LPTSTR GetTStringPointer();
VMT_DATAMGR_API LPSTR GetStringPointer();

VMT_DATAMGR_API  bool SetCVMT_TestData(sNCT_TestData* data);
VMT_DATAMGR_API void StartCallbackThread();
VMT_DATAMGR_API void StopCallbackThread();

VMT_DATAMGR_API void StructureArgTest(SaveXML* pst);

#ifdef __cplusplus
}
#endif


extern VMT_DATAMGR_API int nVMT_DataMgr;

VMT_DATAMGR_API int fnVMT_DataMgr(void);


//**********************************************************
//*
//*  Marshalling Test Codes Space [End]
//*
//**********************************************************

class VMT_DATAMGR_API CMarshallTest
{
public:
	CMarshallTest(void);
	~CMarshallTest(void);
};

