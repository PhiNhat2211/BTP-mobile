/////////////////////////////////////////////////
// 서버에서 받아서 UI로 올려주는 구조체 
/////////////////////////////////////////////////
#pragma pack(1)

// define
#define TIER_ROW_MAX 200

////////////////////////////////////////////////////////////////////////
// sBlockBayInfoSimple
//////////////////////////////////////////////////////////////////////////
struct sBlockBayInfoSimple
{
    int    m_iBay;
    int    m_iMaxRow;
    int    m_iMaxTier;
	char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
	byte cntrCount;      
	EEv2_Simple_Conatainer cntr[TIER_ROW_MAX];  

	sBlockBayInfoSimple()
	{
		Default();
	}

	void Default()
	{
        m_iBay = 0;
        m_iMaxRow = 0;
        m_iMaxTier =0;
		ZeroMemory(BlockBay, TIER_ROW_MAX);
		cntrCount = 0;
		ZeroMemory(cntr, sizeof(EEv2_Simple_Conatainer)*TIER_ROW_MAX);
	}
};

/*
struct sBlockBayInfoSimple2
{
    int      m_iBay;
    byte    m_iRowMax;
    byte    m_iTierMax;
    char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
    byte cntrCount;      
    EEv2_Simple_Conatainer cntr[TIER_ROW_MAX];  

    sBlockBayInfoSimple2()
    {
        Default();
    }

    void Default()
    {
        ZeroMemory(BlockBay, TIER_ROW_MAX);
        cntrCount = 0;
        ZeroMemory(cntr, sizeof(EEv2_Simple_Conatainer)*TIER_ROW_MAX);
    }
};
*/
////////////////////////////////////////////////////////////////////////
// sBlockBayInfo
////////////////////////////////////////////////////////////////////////
struct sBlockBayInfo
{
    int    m_iBay;
    int    m_iMaxRow;
    int    m_iMaxTier;
	char BlockBay[TIER_ROW_MAX]; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
	byte cntrCount;
	EEv2_Def_Inventory cntr[TIER_ROW_MAX];

	sBlockBayInfo()
	{
		Default();
	}

	void Default()
	{
        m_iBay = 0;
        m_iMaxRow = 0;
        m_iMaxTier =0;
		ZeroMemory(BlockBay, TIER_ROW_MAX);
		cntrCount = 0;
		ZeroMemory(cntr, sizeof(EEv2_Def_Inventory)*TIER_ROW_MAX);
	}
};

///////////////////////////////////////////////////////////////////////
// EEv2JobOrderForITV 잡오더로 모든걸 처리한다.
///////////////////////////////////////////////////////////////////////
struct EEv2JobOrderForITV
{
	EEv2JobOrder firstJob;
	TCHAR        firstJobStatus[5];
	int          firstJobETA;
	EEv2JobOrder secondJob;
	TCHAR        secondJobStatus[5];
	int          secondJobETA;
	int          kpiInfo1;
	int          kpiInfo2;
	int          kpiInfo3;
	int          kpiInfo4;

	EEv2JobOrderForITV()
	{
		Default();
	}

	void Default()
	{
		ZeroMemory(&firstJob, sizeof(EEv2JobOrder));
		ZeroMemory(firstJobStatus, sizeof(TCHAR)*5);
		firstJobETA = 0;
		ZeroMemory(&secondJob, sizeof(EEv2JobOrder));
		ZeroMemory(secondJobStatus, sizeof(TCHAR)*5);
		secondJobETA = 0;
		kpiInfo1 = 0;
		kpiInfo2 = 0;
		kpiInfo3 = 0;
		kpiInfo4 = 0;
	}
};

//////////////////////////////////////////////////
//
//////////////////////////////////////////////////
// struct EEv2JobOrderForITVKAP
// {
// 	EEv2JobOrderKAP firstJob;
// 	TCHAR           firstJobStatus[5];
// 	int             firstJobETA;
// 	EEv2JobOrderKAP secondJob;
// 	TCHAR           secondJobStatus[5];
// 	int             secondJobETA;
// 	int             kpiInfo1;
// 	int             kpiInfo2;
// 	int             kpiInfo3;
// 	int             kpiInfo4;
// 
// 	EEv2JobOrderForITVKAP()
// 	{
// 		Default();
// 	}
// 
// 	void Default()
// 	{
// 		ZeroMemory(&firstJob, sizeof(EEv2JobOrderKAP));
// 		ZeroMemory(firstJobStatus, sizeof(TCHAR)*5);
// 		firstJobETA = 0;
// 		ZeroMemory(&secondJob, sizeof(EEv2JobOrderKAP));
// 		ZeroMemory(secondJobStatus, sizeof(TCHAR)*5);
// 		secondJobETA = 0;
// 		kpiInfo1 = 0;
// 		kpiInfo2 = 0;
// 		kpiInfo3 = 0;
// 		kpiInfo4 = 0;
// 	}
// };
// 



////////////////////////////////////////////////////////////////////////
// 
////////////////////////////////////////////////////////////////////////
struct sBBwgs
{
	TCHAR  m_szId[20];
	TCHAR  m_szType[20];

	double TL_LO;
	double TL_LA;
	double BR_LO;
	double BR_LA;

	sBBwgs()
		:TL_LO(0.0)
		,TL_LA(0.0)
		,BR_LO(0.0)
		,BR_LA(0.0)
	{
		memset(m_szId, 0, sizeof(TCHAR)*20);
		memset(m_szType, 0, sizeof(TCHAR)*20);
	}
};

//---------------------------------------------------
struct sBBay
{
	TCHAR  m_szId[20];
    TCHAR  m_szBlock[20];
	TCHAR  m_szType[20];
    int         m_iBayNum;

	double TL_LO;
	double TL_LA;
	double TR_LO;
	double TR_LA;
	double BR_LO;
	double BR_LA;
	double BL_LO;
	double BL_LA;

	Vector3 m_Area[4];


	sBBay()
		:TL_LO(0.0)
		,TL_LA(0.0)
		,TR_LO(0.0)
		,TR_LA(0.0)
		,BR_LO(0.0)
		,BR_LA(0.0)
		,BL_LO(0.0)
		,BL_LA(0.0)
	{
		memset(m_szId, 0, sizeof(TCHAR)*20);
		memset(m_szType, 0, sizeof(TCHAR)*20);
        m_iBayNum = 0;
	}
};


//---------------------------------------------------
typedef vector<wstring> InBlockName;

//---------------------------------------------------
struct sBRoot : public sBBay
{
	int				   m_BlockCount;    // 포함하고 있는 Block 의 갯수
	InBlockName        m_BlockNames;    // 포함하고 있는 Block 의 이름들(이름으로 자식노드를 검색한다.)

	sBRoot()
		:m_BlockCount(0)
	{
	}

	~sBRoot()
	{
		vector<wstring> emptyData;
		m_BlockNames.swap(emptyData);
	}
};

//---------------------------------------------------
struct sBSub : public sBBay
{
	int				   m_BlockCount;    // 포함하고 있는 Block 의 갯수
	InBlockName        m_BlockNames;    // 포함하고 있는 Block 의 이름들(이름으로 자식노드를 검색한다.)

	sBSub()
		:m_BlockCount(0)
	{
	}

	~sBSub()
	{
		vector<wstring> emptyData;
		m_BlockNames.swap(emptyData);
	}
};

//---------------------------------------------------
struct sBBlock : public sBBay
{
	int m_RowType;               // 로우숫자표시방향 0=regular:정방향, 1=reverse:역방향 
	int m_BayStartNum;           // Block이 포함하고 있는 Bay의 시작번호 
	int m_BayCount;              // Block이 포함하고 있는 Bay의 갯수 (1, 3, 5, 7 = 40ft씩 증가)
	int m_IsSubBlock;            // Sub Block 이냐? (0=아니다. 메인블락이다., 1=서브블락이다.)
	int m_SubBlockCount;         // 메인블락일때 Sub Block을 몇개 가지고 있는가. 
	wstring m_ParentBlockName;   // 부모 블락의 이름은? 하나만 가진다. (주로 Reefeer 지역)
	InBlockName m_BayNames;
};


//---------------------------------------------------
typedef map<wstring, sBRoot>     BRootMap;
typedef map<wstring, sBSub>      BSubMap;
typedef map<wstring, sBBlock>    BBlockMap;
typedef map<wstring, sBBay>      BBayMap;

//---------------------------------------------------
struct BBResult
{
	TCHAR BlockName[20];
	int       BayName;

	BBResult()
	{
	    Default();
	}
    void Default()
    {
	    ZeroMemory(BlockName, sizeof(TCHAR)*20);
		BayName=0;
    }
};



#pragma pack()