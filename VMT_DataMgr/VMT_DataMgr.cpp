// VMT_DataMgr.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "VMT_DataMgr.h"
#include <ctime>
#include <process.h>
#include <time.h>
#include <stdio.h>
#include <comdef.h>
#include <CRTDBG.H>
#include <atlconv.h>
#include "GeoMgr.h"


CGeoMgr* pGeoMgr = NULL;


bool InitGeometry()
{
	if( !pGeoMgr )
		pGeoMgr = new CGeoMgr();

	bool bRet = pGeoMgr->InitGeometry();
	
	return bRet;
}

void ReleaseGeometry()
{
	if(pGeoMgr)
	{
		pGeoMgr->ReleaseGeometry();
		SAFE_DELETE(pGeoMgr);
	}
}

CBayPow* GetCurrentPos(double* plo, double* pla)
{
	if( !pGeoMgr )
		return NULL;

	CBayPow* pBP = NULL;
	pBP = pGeoMgr->GetCurrentPos(plo, pla);

	return pBP;
}

CBayPow* SetCurrentPos(double lo, double la)
{
	if( !pGeoMgr )
		return NULL;

	CBayPow* pBP = NULL;
	pBP = pGeoMgr->SetCurrentPos(lo, la);

	return pBP; 
}

int GetTotalBlockCount()
{
	if( !pGeoMgr )
		return 0;

	pGeoMgr->GetTotalBlockCount();
}

int GetTotalBayCount()
{
	if( !pGeoMgr )
		return 0;

	pGeoMgr->GetTotalBayCount();
}

int GetBayCount(LPTSTR szBlock)
{
	if( !pGeoMgr )
		return 0;

	pGeoMgr->GetBayCount(szBlock);
}

int GetRowCount(LPTSTR szBay)
{
	if( !pGeoMgr )
		return 0;

	pGeoMgr->GetBayCount(szBay);
}

int GetTierCount(LPTSTR szRow)
{
	if( !pGeoMgr )
		return 0;

	pGeoMgr->GetBayCount(szRow);
}

//-------------------------------------------------------------
//-
//- Class VMT_DataMgr
//-
//-------------------------------------------------------------

// This is the constructor of a class that has been exported.
// see VMT_DataMgr.h for the class definition
CVMT_DataMgr::CVMT_DataMgr()
{
	return;
}
