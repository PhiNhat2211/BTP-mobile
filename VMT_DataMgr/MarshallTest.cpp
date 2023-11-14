
#include "StdAfx.h"
#include "stddef.h"
#include "MarshallTest.h"

#include <ctime>
#include <process.h>
#include <time.h>
#include <stdio.h>
#include <comdef.h>
#include <CRTDBG.H>
#include <atlconv.h>


sNCT_TestData g_TestData;
TCHAR g_szDLLName[24] = _T("VMT_DataManager");
TCHAR g_szCurTime[24] = _T("");

HANDLE g_hTestThread = NULL;
unsigned int g_nTestThreadID = 0;


unsigned __stdcall CallbackTestThread(void* arg)
{
	while(1)
	{
		Sleep(5000);

		time_t   current_time;
		time( &current_time);

		g_TestData.m_ulTime = current_time;

		USES_CONVERSION;
		_stprintf( g_szCurTime, A2T(ctime(&current_time)) );

		g_TestData.m_pfnGetCurrentTime(g_szCurTime);

	}
	return 0;
}


void Callback_GetCurrentTime_test(TCHAR* sztime)
{
	sztime[0] = 'a';
	//return (unsigned long int)sztime;
}


sNCT_TestData* GetCVMT_TestData()
{
	int i = sizeof(double);
	return &g_TestData;
}

bool SetCVMT_TestData(sNCT_TestData* pdata)
{
	g_TestData = *pdata;
	g_TestData.m_szDLLName = g_szDLLName;

	return true;
}

LPTSTR GetTStringPointer()
{
	//return g_TestData.m_tszTrolleyPosition;
	return NULL;
}

LPSTR GetStringPointer()
{
	return g_TestData.m_szMachinName;
}



void StartCallbackThread()
{
	if( g_hTestThread )
		return;

	g_hTestThread  = (HANDLE)_beginthreadex(NULL,
				0,
				CallbackTestThread,
				(void *)&g_TestData,
				0,
				&g_nTestThreadID);
}


void StopCallbackThread()
{
	TerminateThread(g_hTestThread, 0 );

	g_hTestThread = NULL;
}


void StructureArgTest(SaveXML* pst)
{
	TCHAR szMessage[512];
	memset(szMessage, 0x00, 512);

	_stprintf(szMessage, _T("Serial-1: %d, %d, Serial-2 : %d, %d"),pst->AutoSeach1, pst->Port1, pst->AutoSeach2, pst->Port2);

	MessageBox(NULL, szMessage, _T("Test Message"), MB_OK);

}



//===========================================================
//=
//= Basic Implementation by Project Wizard
//=
///==========================================================

// This is an example of an exported variable
VMT_DATAMGR_API int nVMT_DataMgr=0;

// This is an example of an exported function.
VMT_DATAMGR_API int fnVMT_DataMgr(void)
{
	return 42;
}

CMarshallTest::CMarshallTest(void)
{
}


CMarshallTest::~CMarshallTest(void)
{
}
