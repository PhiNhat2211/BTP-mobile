#include "StdAfx.h"
#include "GeoMgr.h"
#include "Math.h"

#include <TinyXPath/xpath_static.h>



CGeoMgr::CGeoMgr(void)
{
	m_bInit = false;

	int m_nViewWidth = 10000;
	int m_nViewHeight = 10000;

	m_curGP.lo = 39.08583855555;
	m_curGP.la = 22.52706555555;

	pTA = NULL;
}


CGeoMgr::~CGeoMgr(void)
{
}


bool CGeoMgr::InitGeometry()
{
	bool bRet = false;

	// Create POW Container Object
	this->pTA = new CTerminalArea();

	//-------------------------------------------------
	//- Get Geometry POW XML file
	TCHAR szPath[_MAX_PATH];
	CString strPath;

	::GetModuleFileName(NULL, szPath, _MAX_PATH);
	strPath = szPath;
	int iIndex = strPath.ReverseFind('\\');

	strPath = strPath.Left(iIndex);
	strPath += _T("\\Data\\BlockBay_RTG_NCT.xml");


	//- Loading XML Data
	TiXmlDocument * XDp_doc;
	TiXmlElement * XEp_main;

	TIXML_STRING S_res;
	TiXmlNode* N_res;
	XDp_doc = new TiXmlDocument;

	// Use ALT Conversion
	USES_CONVERSION;

	//---------------------------------------------------
	// Get LiveUpdate Server AddresS
	if (!XDp_doc -> LoadFile (T2A(strPath)))
		return bRet;

	TiXmlNode* N_bound = TinyXPath::XNp_xpath_node(XDp_doc->RootElement(), "/Terminal/BOUND");

	while(true)
	{
		if(N_bound == NULL )
			break;

		CString id = TinyXPath::S_xpath_string(N_bound, "@id]").c_str();

		if( id.Left(2) == "TA")
		{
			CTerminalArea* pTA = new CTerminalArea();
			pTA->SetID(id.GetBuffer());

			pTA->posTL.lo = atof(TinyXPath::S_xpath_string(N_bound, "//TL_LO/text()").c_str());
			pTA->posTL.la = atof(TinyXPath::S_xpath_string(N_bound, "//TL_LA/text()").c_str());
			pTA->posTR.lo = atof(TinyXPath::S_xpath_string(N_bound, "//TR_LO/text()").c_str());
			pTA->posTR.la = atof(TinyXPath::S_xpath_string(N_bound, "//TR_LA/text()").c_str());
			pTA->posBL.lo = atof(TinyXPath::S_xpath_string(N_bound, "//BL_LO/text()").c_str());
			pTA->posBL.la = atof(TinyXPath::S_xpath_string(N_bound, "//BL_LA/text()").c_str());
			pTA->posBR.lo = atof(TinyXPath::S_xpath_string(N_bound, "//BR_LO/text()").c_str());
			pTA->posBR.la = atof(TinyXPath::S_xpath_string(N_bound, "//BR_LA/text()").c_str());

			m_pTerminal = pTA;
		}
		else if (id.Left(2) == "YA")
		{
			CBlockArea* pBA = new CBlockArea();
			pBA->SetID(id.GetBuffer());

			pBA->posTL.lo = atof(TinyXPath::S_xpath_string(N_bound, "//TL_LO/text()").c_str());
			pBA->posTL.la = atof(TinyXPath::S_xpath_string(N_bound, "//TL_LA/text()").c_str());
			pBA->posTR.lo = atof(TinyXPath::S_xpath_string(N_bound, "//TR_LO/text()").c_str());
			pBA->posTR.la = atof(TinyXPath::S_xpath_string(N_bound, "//TR_LA/text()").c_str());
			pBA->posBL.lo = atof(TinyXPath::S_xpath_string(N_bound, "//BL_LO/text()").c_str());
			pBA->posBL.la = atof(TinyXPath::S_xpath_string(N_bound, "//BL_LA/text()").c_str());
			pBA->posBR.lo = atof(TinyXPath::S_xpath_string(N_bound, "//BR_LO/text()").c_str());
			pBA->posBR.la = atof(TinyXPath::S_xpath_string(N_bound, "//BR_LA/text()").c_str());

			pBA->posCT.lo = (pBA->posTL.lo +pBA->posTR.lo +pBA->posBL.lo +pBA->posBL.lo) /4;
			pBA->posCT.la = (pBA->posTL.la +pBA->posTR.la +pBA->posBL.la +pBA->posBL.la) /4;

			this->vector_Block.push_back(pBA);
		}
		else if( id.Left(2) == "YP")
		{
			CBayPow* pBP = new CBayPow();
			pBP->SetID(id.GetBuffer());

			pBP->posTL.lo = atof(TinyXPath::S_xpath_string(N_bound, "//TL_LO/text()").c_str());
			pBP->posTL.la = atof(TinyXPath::S_xpath_string(N_bound, "//TL_LA/text()").c_str());
			pBP->posTR.lo = atof(TinyXPath::S_xpath_string(N_bound, "//TR_LO/text()").c_str());
			pBP->posTR.la = atof(TinyXPath::S_xpath_string(N_bound, "//TR_LA/text()").c_str());
			pBP->posBL.lo = atof(TinyXPath::S_xpath_string(N_bound, "//BL_LO/text()").c_str());
			pBP->posBL.la = atof(TinyXPath::S_xpath_string(N_bound, "//BL_LA/text()").c_str());
			pBP->posBR.lo = atof(TinyXPath::S_xpath_string(N_bound, "//BR_LO/text()").c_str());
			pBP->posBR.la = atof(TinyXPath::S_xpath_string(N_bound, "//BR_LA/text()").c_str());

			pBP->posCT.lo = (pBP->posTL.lo +pBP->posTR.lo +pBP->posBL.lo +pBP->posBL.lo) /4;
			pBP->posCT.la = (pBP->posTL.la +pBP->posTR.la +pBP->posBL.la +pBP->posBL.la) /4;

			this->vector_Bay.push_back(pBP);
		}

		N_bound = N_bound->NextSiblingElement();
	}

	//----------------------------------------------
	// Bind Geomety Data by Hierachy
	//----------------------------------------------
	vector<CBlockArea*>::iterator iBlock;
	for(iBlock = this->vector_Block.begin(); iBlock != this->vector_Block.end(); iBlock++)
	{
		string strKey = T2A( (*iBlock)->m_szBlock );
		CBlockArea* pValue = (*iBlock);
		this->m_pTerminal->map_Block[strKey] = pValue;
	}

	// Bind Bay to Block
	vector<CBayPow*>::iterator iBay;
	for(iBay = this->vector_Bay.begin(); iBay != this->vector_Bay.end(); iBay++)
	{
		string strBlockKey = T2A( (*iBay)->m_szBlock );
		string strBayKey = T2A( (*iBay)->m_szBay );
		CBayPow* pValue = (*iBay);

		m_pTerminal->map_Block[strBlockKey]->map_Bay[strBayKey] = pValue;
	}

	
	SAFE_DELETE(XDp_doc);

	this->m_bInit = true;
	bRet = true;

	return bRet; 
}

bool CGeoMgr::ReleaseGeometry()
{
	bool bRet = false;

	SAFE_DELETE(m_pTerminal);

	vector<CBlockArea*>::iterator iBlock;
	for(iBlock = this->vector_Block.begin(); iBlock != this->vector_Block.end(); iBlock++)
	{
		SAFE_DELETE((*iBlock));
	}
	this->vector_Block.clear();

	// Bind Bay to Block
	vector<CBayPow*>::iterator iBay;
	for(iBay = this->vector_Bay.begin(); iBay != this->vector_Bay.end(); iBay++)
	{
		SAFE_DELETE((*iBay));
	}
	this->vector_Bay.clear();

	bRet = true;
	return bRet;
}

CBayPow* CGeoMgr::GetCurrentPos(double* plo, double* pla)
{
	return NULL;
}

CBayPow* CGeoMgr::SetCurrentPos(double lo, double la)
{
	if( !this->m_bInit )
		return NULL;

	CBlockArea* pBA = NULL;
	CBayPow* pBP = NULL;
	
	{	//---------------------------------
		//- Find Block Area
		map<string, CBlockArea*>& mapBA = this->m_pTerminal->map_Block;
		map<string, CBlockArea*>::iterator iBA;
	
		double delta, dx, dy, dMax = DBL_MAX;

		for( iBA = mapBA.begin(); iBA != mapBA.end(); iBA++ )
		{
			CBlockArea* pBlock = (*iBA).second;
			dx = pBlock->posCT.lo - lo;
			dy = pBlock->posCT.la - la;
			delta = dx*dx + dy*dy;

			if(delta < dMax)
			{
				dMax = delta;
				pBA = pBlock; 
			}
		}

		if( !pBA )
			return NULL;
	}

	{	//---------------------------------
		//- Find Bay
		map<string, CBayPow*>& mapBP = pBA->map_Bay;
		map<string, CBayPow*>::iterator iBP;

		double delta, dx, dy, dMax = DBL_MAX;
	
		for( iBP = mapBP.begin(); iBP != mapBP.end(); iBP++ )
		{
			CBayPow* pBay = (*iBP).second;
			dx = pBay->posCT.lo - lo;
			dy = pBay->posCT.la - la;
			delta = dx*dx + dy*dy;

			if(delta < dMax)
			{
				dMax = delta;
				pBP = pBay; 

				//---------------------------------
				//- Find BayEst
				double deltaQuad_lo = (pBay->posTR.lo - pBay->posTL.lo) / 4.0;
				double deltaQuad_la = (pBay->posTR.la - pBay->posTL.la) / 4.0;

				double delta_L_dx = 0;
				double delta_L_dy = 0;
				double delta_L_delta = 0;

				double delta_R_dx = 0;
				double delta_R_dy = 0;
				double delta_R_delta = 0;
			
				// 1/4
				{
					delta_L_dx = pBay->posTL.lo - lo;
					delta_L_dy = pBay->posTL.la - la;
					delta_L_delta = delta_L_dx*delta_L_dx + delta_L_dy*delta_L_dy;

					delta_R_dx = pBay->posTL.lo + deltaQuad_lo- lo;
					delta_R_dy = pBay->posTL.la + deltaQuad_la - la;
					delta_R_delta = delta_R_dx*delta_R_dx + delta_R_dy*delta_R_dy;

					if(delta_L_delta < delta_R_delta)
					{
						int evenBayL = _ttoi(pBay->m_szBay);
						if( evenBayL > 0 )
							evenBayL--;

						_stprintf(pBay->m_szBayEst, _T("%d"), evenBayL);
						continue;
					}
				}

				// 2/4
				{
					delta_L_dx = pBay->posTL.lo + deltaQuad_lo- lo;
					delta_L_dy = pBay->posTL.la + deltaQuad_la - la;
					delta_L_delta = delta_L_dx*delta_L_dx + delta_L_dy*delta_L_dy;

					delta_R_dx = pBay->posTL.lo + (2.0*deltaQuad_lo)- lo;
					delta_R_dy = pBay->posTL.la + (2.0*deltaQuad_la)- la;
					delta_R_delta = delta_R_dx*delta_R_dx + delta_R_dy*delta_R_dy;

					if(delta_L_delta < delta_R_delta)
					{
						_tcscpy(pBay->m_szBayEst, pBay->m_szBay);
						continue;
					}
				}

				// 3/4
				{
					delta_L_dx = pBay->posTR.lo - (2.0*deltaQuad_lo)- lo;
					delta_L_dy = pBay->posTR.la - (2.0*deltaQuad_la)- la;
					delta_L_delta = delta_L_dx*delta_L_dx + delta_L_dy*delta_L_dy;

					delta_R_dx = pBay->posTR.lo - deltaQuad_lo - lo;
					delta_R_dy = pBay->posTR.la - deltaQuad_la - la;
					delta_R_delta = delta_R_dx*delta_R_dx + delta_R_dy*delta_R_dy;

					if(delta_L_delta < delta_R_delta)
					{
						_tcscpy(pBay->m_szBayEst, pBay->m_szBay);
						continue;
					}
				}

				// 4/4
				{
					delta_L_dx = pBay->posTR.lo - deltaQuad_lo - lo;
					delta_L_dy = pBay->posTR.la - deltaQuad_la - la;
					delta_L_delta = delta_L_dx*delta_L_dx + delta_L_dy*delta_L_dy;

					delta_R_dx = pBay->posTR.lo - lo;
					delta_R_dy = pBay->posTR.la - la;
					delta_R_delta = delta_R_dx*delta_R_dx + delta_R_dy*delta_R_dy;

					if(delta_L_delta < delta_R_delta)
					{
						int evenBayR = _ttoi(pBay->m_szBay);

						if( evenBayR+1 < mapBP.size() )
							evenBayR++;

						_stprintf(pBay->m_szBayEst, _T("%d"), evenBayR);
						continue;
					}
				}
			}
		}

		//if( pBay->PntInRect(lo, la)  )
		//{
		//	double deltaQuad = (pBay->posTR.lo - pBay->posTL.lo)/4.0;

		//	if( lo < (pBay->posTL.lo + deltaQuad) )
		//	{
		//		int evenBayL = _ttoi(pBay->m_szBay);
		//		
		//		if( evenBayL-1 >= 0 )
		//			evenBayL--;
		//		
		//		_stprintf(pBay->m_szBayEst, _T("%d"), evenBayL);
		//	}
		//	else if( lo > (pBay->posTL.lo + (deltaQuad*3)) )
		//	{
		//		int evenBayR = _ttoi(pBay->m_szBay);
		//		
		//		if( evenBayR+1 < mapBP.size() )
		//			evenBayR++;

		//		_stprintf(pBay->m_szBayEst, _T("%d"), evenBayR);
		//	}
		//	else
		//	{
		//		_tcscpy(pBay->m_szBayEst, pBay->m_szBay);
		//	}

		//	pBP = pBay;
		//	break;
		//}
	}

	// Set Current GeoPoint
	if( pBP )
	{
		this->m_curGP.la = la;
		this->m_curGP.lo = lo;
	}

	return pBP;
}

int CGeoMgr::GetTotalBlockCount()
{
	if( !this->m_bInit )
		return 0;

	return this->vector_Block.size();
}

int CGeoMgr::GetTotalBayCount()
{
	if( !this->m_bInit )
		return 0;

	return this->vector_Bay.size();
}

int CGeoMgr::GetBayCount(LPTSTR szBlock)
{
	if( !this->m_bInit )
		return 0;


	map<string, CBlockArea*>& mapBA = this->m_pTerminal->map_Block;

	// Use ALT Conversion
	USES_CONVERSION;

	string blockId = T2A(szBlock);

	if( mapBA.count(blockId) == 0 )
		return 0;
	
	return mapBA[blockId]->map_Bay.size();
}

int CGeoMgr::GetRowCount(LPTSTR szBay)
{
	if( !this->m_bInit )
		return 0;

	return 7;
}

int CGeoMgr::GetTierCount(LPTSTR szRow)
{
	if( !this->m_bInit )
		return 0;

	return 7;
}

void CGeoMgr::SetViewPortSize(int width, int height)
{
	this->m_nViewWidth = width;
	this->m_nViewHeight = height;
}

PixelPoint* CGeoMgr::TranslateTerminal_GtoP(GeoPoint* pAP)
{
	return NULL;
}

GeoPoint* CGeoMgr::TranslateTerminal_PtoG(PixelPoint* pPP)
{
	return NULL;
}

PixelPoint* CGeoMgr::TranslateBlock_GtoP(GeoPoint* pAP)
{
	return NULL;
}

GeoPoint* CGeoMgr::TranslateBlock_PtoG(PixelPoint* pPP)
{
	return NULL;
}
