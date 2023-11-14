/////////////////////////////////////////////////
// �������� �޾Ƽ� UI�� �÷��ִ� ����ü 
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
// EEv2JobOrderForITV ������� ���� ó���Ѵ�.
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
	int				   m_BlockCount;    // �����ϰ� �ִ� Block �� ����
	InBlockName        m_BlockNames;    // �����ϰ� �ִ� Block �� �̸���(�̸����� �ڽĳ�带 �˻��Ѵ�.)

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
	int				   m_BlockCount;    // �����ϰ� �ִ� Block �� ����
	InBlockName        m_BlockNames;    // �����ϰ� �ִ� Block �� �̸���(�̸����� �ڽĳ�带 �˻��Ѵ�.)

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
	int m_RowType;               // �ο����ǥ�ù��� 0=regular:������, 1=reverse:������ 
	int m_BayStartNum;           // Block�� �����ϰ� �ִ� Bay�� ���۹�ȣ 
	int m_BayCount;              // Block�� �����ϰ� �ִ� Bay�� ���� (1, 3, 5, 7 = 40ft�� ����)
	int m_IsSubBlock;            // Sub Block �̳�? (0=�ƴϴ�. ���κ���̴�., 1=�������̴�.)
	int m_SubBlockCount;         // ���κ���϶� Sub Block�� � ������ �ִ°�. 
	wstring m_ParentBlockName;   // �θ� ����� �̸���? �ϳ��� ������. (�ַ� Reefeer ����)
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