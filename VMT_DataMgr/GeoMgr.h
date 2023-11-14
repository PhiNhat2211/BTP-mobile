#pragma once

#ifndef __GEOMGR_HEADER__
#define __GEOMGR_HEADER__

#include "stdafx.h"
#include "stddef.h"
#include <map>
#include <vector>
#include <string>

using namespace std;

#pragma pack(1)


class PixelPoint
{
public:
	int x;
	int y;

	PixelPoint()
	{
		x=0;
		y=0;
	}
};

class GeoPoint
{
public:
	double lo;
	double la;

	GeoPoint()
	{
		lo=0;
		la=0;
	}
};


class CBayPow	// Bay (YardPow)
{
public:
	TCHAR m_szID[20];
	TCHAR m_szBlock[20];
	TCHAR m_szBay[20];
	TCHAR m_szBayEst[20];
	int m_nRow;


	GeoPoint posTL;
	GeoPoint posTR;
	GeoPoint posBL;
	GeoPoint posBR;

	GeoPoint posCT;

	CBayPow()
	{
		memset(m_szID,0x00,sizeof(TCHAR)*20);
		memset(m_szBlock,0x00,sizeof(TCHAR)*20);
		memset(m_szBay,0x00,sizeof(TCHAR)*20);
		memset(m_szBayEst,0x00,sizeof(TCHAR)*20);

		m_nRow = 7; // Currently 7 Fixed
	}

	void SetID(LPTSTR szID)
	{
		if(szID == NULL)
			return;

		_tcscpy(this->m_szID, szID);

		// Parsing ID
		int curCnt = 0;
		TCHAR szTemp[20];
		memset(szTemp, 0x00, sizeof(TCHAR)*20);
		_tcscpy(szTemp, szID);

		TCHAR* szToken = _tcstok(szTemp, _T("-"));

		while(szToken != NULL)
		{
			if(curCnt == 1)
				_tcscpy(this->m_szBlock, szToken);
			else if(curCnt == 2)
				_tcscpy(this->m_szBay, szToken);

			szToken = _tcstok(NULL, _T("-"));
			curCnt++;
		}
	}

	bool PntInRect(double lo, double la)
	{
		bool bRet = false;

		if( posTL.lo < lo && lo < posTR.lo )
		{
			if( posBL.la < la && la <posTL.la )
				bRet = true;
		}

		return bRet;
	}
};

class CBlockArea	// Block (Yard Area)
{
public :
	TCHAR m_szID[20];
	TCHAR m_szBlock[20];

	GeoPoint posTL;			
	GeoPoint posTR;
	GeoPoint posBL;	
	GeoPoint posBR;

	GeoPoint posCT;

	GeoPoint* _pGeoPoint[4];

	std::map<string, CBayPow*> map_Bay;

	CBlockArea()
	{
		memset(m_szID,0x00,sizeof(TCHAR)*20);
		memset(m_szBlock,0x00,sizeof(TCHAR)*20);

		_pGeoPoint[0] = &posTL;
		_pGeoPoint[1] = &posTR;
		_pGeoPoint[2] = &posBR;
		_pGeoPoint[3] = &posBL;
	}

	void SetID(LPTSTR szID)
	{
		if(szID == NULL)
			return;

		_tcscpy(this->m_szID, szID);

		// Parsing ID
		int curCnt = 0;
		TCHAR szTemp[20];
		memset(szTemp, 0x00, sizeof(TCHAR)*20);
		_tcscpy(szTemp, szID);

		TCHAR* szToken = _tcstok(szTemp, _T("-"));

		while(szToken != NULL)
		{
			if(curCnt == 1)
				_tcscpy(this->m_szBlock, szToken);

			szToken = _tcstok(NULL, _T("-"));
			curCnt++;
		}
	}
	
	bool PntInShape(double lo, double la)
	{	
        int polySides = 4;
		bool oddNodes = false;
        int i, j = polySides - 1;
        for (i = 0; i < polySides; i++)
        {
			if (_pGeoPoint[i]->la < la && _pGeoPoint[j]->la >= la ||
                _pGeoPoint[j]->la < la && _pGeoPoint[i]->la >= la)
            {
				if (_pGeoPoint[i]->lo + (la - _pGeoPoint[i]->la) / (_pGeoPoint[j]->la - _pGeoPoint[i]->la) * (_pGeoPoint[j]->lo - _pGeoPoint[i]->lo) < lo)
                {
                    oddNodes = !oddNodes;
                }
            }
            j = i;
        }

        return oddNodes;
	}

	bool PntInRect(double lo, double la)
	{
		bool bRet = false;

		if( posTL.lo < lo && lo < posTR.lo )
		{
			if( posBL.la < la && la <posTL.la )
				bRet = true;
		}

		return bRet;
	}
};


class CTerminalArea
{
public :
	TCHAR m_szID[20];
	GeoPoint posTL;
	GeoPoint posTR;
	GeoPoint posBL;
	GeoPoint posBR;

	std::map<string, CBlockArea*> map_Block;

	CTerminalArea()
	{
		memset(m_szID,0x00,sizeof(TCHAR)*20);
	}

	void SetID(LPTSTR szID)
	{
		if(szID == NULL)
			return;

		_tcscpy(this->m_szID, szID);
	}

	bool PntInRect(double lo, double la)
	{
		bool bRet = false;

		if( posTL.lo < lo && lo < posTR.lo )
		{
			if( posBL.la < la && la <posBR.la )
				bRet = true;
		}

		return bRet;
	}
};



class CGeoMgr
{
//private :
//	static CGeoMgr* pInstance;
//
//	CGeoMgr(void);
//
//public:
//	static CGeoMgr& GetInstance()
//	{
//		if(pInstance == NULL )
//			pInstance = new CGeoMgr;
//
//		return *pInstance;
//	}
//
//	static CGeoMgr* GetInstancePtr()
//	{
//		if(pInstance == NULL )
//			pInstance = new CGeoMgr;
//
//		return pInstance;
//	}
//
//	static void ReleseInstance()
//	{
//		if(pInstance != NULL )
//			delete pInstance;
//	}
public:
	CGeoMgr();
	~CGeoMgr();

public:
	bool m_bInit;

	int m_nViewWidth;
	int m_nViewHeight;

	CTerminalArea* pTA;

	CTerminalArea* m_pTerminal;

	std::vector<CBlockArea*> vector_Block;
	std::vector<CBayPow*> vector_Bay;

	GeoPoint m_curGP;
	PixelPoint m_curPP;

public:

	bool InitGeometry();

    bool ReleaseGeometry();

    CBayPow* GetCurrentPos(double* plo, double* pla);

    CBayPow* SetCurrentPos(double lo, double la);

    int GetTotalBlockCount();

    int GetTotalBayCount();

    int GetBayCount(LPTSTR szBlock);

    int GetRowCount(LPTSTR szBay);

    int GetTierCount(LPTSTR szRow);


	
    void SetViewPortSize(int width, int height);

    PixelPoint* TranslateTerminal_GtoP(GeoPoint* pAP);

    GeoPoint* TranslateTerminal_PtoG(PixelPoint* pPP);

	PixelPoint* TranslateBlock_GtoP(GeoPoint* pAP);

    GeoPoint* TranslateBlock_PtoG(PixelPoint* pPP);

};

#pragma pack()

#endif

