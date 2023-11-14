// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the VMT_DATAMGR_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// VMT_DATAMGR_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#pragma once

#include "stdafx.h"
#include "stddef.h"

class CBayPow;

// marked as extern "C" to avoid name mangling issue
#ifdef __cplusplus
extern "C"
{
	VMT_DATAMGR_API bool InitGeometry();

	VMT_DATAMGR_API void ReleaseGeometry();

	VMT_DATAMGR_API CBayPow* GetCurrentPos(double* plo, double* pla);

	VMT_DATAMGR_API CBayPow* SetCurrentPos(double lo, double la);

	VMT_DATAMGR_API int GetTotalBlockCount();

	VMT_DATAMGR_API int GetTotalBayCount();

	VMT_DATAMGR_API int GetBayCount(LPTSTR szBlock);

	VMT_DATAMGR_API int GetRowCount(LPTSTR szBay);

	VMT_DATAMGR_API int GetTierCount(LPTSTR szRow);

	//VMT_DATAMGR_API void SetViewPortSize(int width, int height);

	//VMT_DATAMGR_API PixelPoint* TranslateTerminal_GtoP(GeoPoint* pAP);

	//VMT_DATAMGR_API GeoPoint* TranslateTerminal_PtoG(PixelPoint* pPP);

	//VMT_DATAMGR_API PixelPoint* TranslateBlock_GtoP(GeoPoint* pAP);

	//VMT_DATAMGR_API GeoPoint* TranslateBlock_PtoG(PixelPoint* pPP);



};


//===========================================================
//=
//= Basic Implementation by Project Wizard
//=
///==========================================================

// This class is exported from the VMT_DataMgr.dll
class VMT_DATAMGR_API CVMT_DataMgr
{
public:
	CVMT_DataMgr(void);
	// TODO: add your methods here.
};



#endif // If defined __cplusplus
