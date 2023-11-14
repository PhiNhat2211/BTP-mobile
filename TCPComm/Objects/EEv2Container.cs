using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm.EEStruct
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Def_Damage : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_DMG_CD)]
        public String dmgCd;//
        /*
        "Damage Code
    e.g) AS :  ALL SNOW
          PU : PUSHED
          BM : Band Missing"

        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_DMG_INOUT)]
        public String dmgInOut; //Inside / Outside
        /*
        "Inside / Outside
    e.g) I : Inside, O : Outside"

        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_DMG_PART)]
        public String dmgPart;//Damage part
        /*
        "Damage part
    e.g) F : Fore, R : Rear, L : Left, R : Right, T : Top, B : BottomR)"

        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_DMG_RANGE)]
        public String dmgRange;//Damage Range
        /*
        "Damage Range
    e.g) 10*10*10"

        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_DMG_DESC)]
        public String dmgDesc;//ISO Code
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Def_Seal : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_SEAL_SEALNO)]
        public String sealNo;//IMDG
        /*
        "Seal No
    e.g.) 12311231231"

        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_SEAL_TYPE)]
        public String sealTp;
        /*
        "C : Custom
        O : Operator"

        */
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Def_Reefer : EEParentClass
    {
        public float reeferTemp;		//온도
        public float WorkTemp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_REEFER_PLUGCD)]
        public String plugCd;
        /*
        "status of Plug
        PIW : Wait for Plug-In
        PIM : Monitoring of Plug-In
        POW : Wait for Plug-Out
        POC :  Completed of Plug-Out"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
        public String unit;
        /*
        "unit of temperature
        F : Fahrenheit
        C : Celsius "
        */
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Def_Imdg : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_IMDG_IMDG)]
        public String imdg;//IMDG
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_IMDG_UNNO)]
        public String unNo;//UNNO
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_IMDG_FIRECODE)]
        public String fireCd;//Fire Code
    };

    //===================================================
    // Job관련 정보를 받을때 포함되는 컨테이너 정보구조체
    //===================================================
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Job_Container : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_ISO)]
        public String cntrIso;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_TYPE)]
        public String cntrTp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_CLASS)]
        public String cls;//수출 수입
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_OPRCODE)]
        public String opr; //e.g) HJS
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_CARGO_TYPE)]
        public String cntrCgoTp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_FULL_MTY)]
        public String fullMty;

    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Visual_Container : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        /*
        "12-character container ID.  
        e.g.) AWSU1234567"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_ISO)]
        public String cntrIso;
        /*
        e.g.) 22G0
        */
        public Byte cntrTp;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_STYPE_DESC)]
        public String cntrSpTp;
        /*
        "Container Special Type
     e.g) AKD,OOG (AKD-Awkward,OOG -Out of Gauge)"
        */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_OPRCODE)]
        public String opr;
        /*
        e.g) HJS
        */

        public Byte cls;
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
        public Byte cntrCgoTp;
        /*
        "Cargo Type
     G : General
     M: Empty
     MH : Empty Hazardous
     R : Reefer
     RH : Reefer Hazardous
     H : Hazardous"
        */
        public Byte fullMty; // 
        /*
        "F: Full
         M: Empty"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_LOCATION)]
        public String location;
        /*
        61A-34-A-2
        VIRTUAL BLOCK
        1A-17-C-5
        ZONE 47
        */
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_CNTR : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        /*
        "12-character container ID.  
        e.g.) AWSU1234567"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public String cntrIso;
        /*
        e.g.) 22G0
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public String cntrLen;//
        /*
        e.g.) 20,40,45,48,53
        */
        public Byte cntrTp;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String opr;
        /*
        e.g) HJS
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public String cntrSpTp;
        /*
        "Container Special Type
     e.g) AKD,OOG (AKD-Awkward,OOG -Out of Gauge)"
        */
        public Byte cls;
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
        public Byte cntrCgoTp;
        /*
        "Cargo Type
     G : General
     M: Empty
     MH : Empty Hazardous
     R : Reefer
     RH : Reefer Hazardous
     H : Hazardous"
        */
        public Byte fullMty; // 
        /*
        "F: Full
         M: Empty"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_LOCATION)]
        public String location;
        /*
	
	
        */
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_CNTR_LOAD : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        /*
        "12-character container ID.  
        e.g.) AWSU1234567"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public String cntrIso;
        /*
        e.g.) 22G0
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public String cntrLen;//
        /*
        e.g.) 20,40,45,48,53
        */
        public Byte cntrTp;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String opr;
        /*
        e.g) HJS
        */
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Def_Container : EEParentClass
    {   
        public EEv2_Def_Damage[] dmgArray
        {
            #region [dmgArray get method]
            get
            {
                EEv2_Def_Damage[] _dmgArray = new EEv2_Def_Damage[EEv2StringMax.COUNT_MAX_DAMAGE];

                int offset = 0;
                int dmgSize = Marshal.SizeOf(typeof(EEv2_Def_Damage));
                IntPtr buff = Marshal.AllocHGlobal(dmgSize);

                for (int n = 0; n < EEv2StringMax.COUNT_MAX_DAMAGE; n++)
                {
                    Marshal.Copy(b_dmgArray, offset, buff, dmgSize);

                    EEv2_Def_Damage da = (EEv2_Def_Damage)Marshal.PtrToStructure(buff, typeof(EEv2_Def_Damage));
                    _dmgArray[n] = da;

                    offset += dmgSize;
                }

                Marshal.FreeHGlobal(buff);

                return _dmgArray;
            }
            #endregion [dmgArray get method]
        }

        public EEv2_Def_Seal[] sealArray
        {
            #region [sealArray get method]
            get
            {
                EEv2_Def_Seal[] _sealArray = new EEv2_Def_Seal[EEv2StringMax.COUNT_MAX_SEAL];

                int offset = 0;
                int SealSize = Marshal.SizeOf(typeof(EEv2_Def_Seal));
                IntPtr buff = Marshal.AllocHGlobal(SealSize);

                for (int n = 0; n < EEv2StringMax.COUNT_MAX_SEAL; n++)
                {
                    Marshal.Copy(b_sealArray, offset, buff, SealSize);

                    EEv2_Def_Seal seal = (EEv2_Def_Seal)Marshal.PtrToStructure(buff, typeof(EEv2_Def_Seal));
                    _sealArray[n] = seal;

                    offset += SealSize;
                }

                Marshal.FreeHGlobal(buff);

                return _sealArray;
            }
            #endregion [sealArray get method]
        }

        public EEv2_Def_Imdg[] imdgArray
        {
            #region [imdgArray get method]
            get
            {
                EEv2_Def_Imdg[] _imdgArray = new EEv2_Def_Imdg[EEv2StringMax.COUNT_MAX_IMDG];

                int offset = 0;
                int imdgSize = Marshal.SizeOf(typeof(EEv2_Def_Imdg));
                IntPtr buff = Marshal.AllocHGlobal(imdgSize);

                for (int n = 0; n < EEv2StringMax.COUNT_MAX_IMDG; n++)
                {
                    Marshal.Copy(b_imdgArray, offset, buff, imdgSize);

                    EEv2_Def_Imdg imdg = (EEv2_Def_Imdg)Marshal.PtrToStructure(buff, typeof(EEv2_Def_Imdg));
                    _imdgArray[n] = imdg;

                    offset += imdgSize;
                }

                Marshal.FreeHGlobal(buff);

                return _imdgArray;
            }
            #endregion [imdgArray get method]
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_ISO)]
        public String cntrIso;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_TYPE)]
        public String cntrTp;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_LENGTH)]
        public String cntrLen;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_HEIGHT)]
        public String cntrHgt;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_OPRCODE)]
        public String opr; //e.g) HJS
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_WEIGHT)]
        public String cntrWgt;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_CLASS)]
        public String cls;//수출 수입
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_STYPE_DESC)]
        public String cntrSpTp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_CARGO_TYPE)]
        public String cntrCgoTp;
        /*
        "Cargo Type
         G : General
         M: Empty
         MH : Empty Hazardous
         R : Reefer
         RH : Reefer Hazardous
         H : Hazardous"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_FULL_MTY)]
        public String fullMty;
        /*
        "F: Full
        M: Empty"
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_PORT_OF_DISCHARGE)]
        public String pod;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NEXT_PORT)]
        public String nPod;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_PORT_OF_LOAD)]
        public String pol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_FINAL_PORT)]
        public String fPol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_GRADE)]
        public String cntrGrade;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_DOOR_DIRECTION)]
        public String doorDirect;
        /*
        Fore:F
        After:A
        */
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean isDmg;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean isSeal;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean isHold;
        public EEv2_Def_Reefer reefer;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.SIZE_DAMAGE_ST * EEv2StringMax.COUNT_MAX_DAMAGE)]
        public byte[] b_dmgArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.SIZE_SEAL_ST * EEv2StringMax.COUNT_MAX_SEAL)]
        public byte[] b_sealArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.SIZE_IMDG_ST * EEv2StringMax.COUNT_MAX_IMDG)]
        public byte[] b_imdgArray;

    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_TC_Container : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_ISO)]
        public String cntrIso;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_TYPE)]
        public String cntrTp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_CLASS)]
        public String cls;//수출 수입
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_OPRCODE)]
        public String opr; //e.g) HJS
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_CARGO_TYPE)]
        public String cntrCgoTp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_FULL_MTY)]
        public String fullMty;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_GRADE)]
        public String cntrGrade;
    };

    //- 상차(pick/laden)관련 간략 정보 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Load_Container : EEParentClass
    {
        /*
        "12-character container ID.  
        e.g.) AWSU1234567"
        */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;

        /*
        e.g.) 22G0
        */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public String cntrIso;

        /*
        e.g.) 20,40,45,48,53
        */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public String cntrLen;//

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
        public Byte cntrTp;

        /*
        e.g) HJS
        */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public String opr;

    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Summery_Container : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_NO)]
        public String cntrNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EEv2StringMax.EEV2_STRING_MAX_CNTR_ISO)]
        public String cntrIso;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3_Job_Container : EEv2_Job_Container
    {
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean isHold;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean isSeal;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean isDamage;
    };
}