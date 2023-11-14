using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm.EEStruct
{
    //===================================================
    // Job관련 정보를 받을때 포함되는 목적위치의 정보 구조체
    //===================================================
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Job_Location : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TYPE)]
        public String locTp;
        /*
        enum enType- 다 대문자임
        {
            "Vessel",
            "Yard",
            "Rail",
            "TP",
            "IP",
            "Lane"
        };
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
        public String blck;
        /*
        "Block Name 
        e.g) 1A, 2A, 1B, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BAY)]
        public String bay;
        /*
        "Bay Name
        e.g) 1, 2, 3, 4, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_ROW)]
        public String row;
        /*
        "Row Name
        e.g) A, B, C, D, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TIER)]
        public String tier;
        /*
        "Tier Name
        e.g) 1, 2, 3, 4, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_LANE)]
        public String lane;
        //W : Water-side //L :Land-side
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_LOCATION)]
        public String location;
        /*
        "Location                             if Lane Type
        e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
        */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Yard_Location : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
        public String blck;
        /*
        "Block Name 
        e.g) 1A, 2A, 1B, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BAY)]
        public String bay;
        /*
        "Bay Name
        e.g) 1, 2, 3, 4, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_ROW)]
        public String row;
        /*
        "Row Name
        e.g) A, B, C, D, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TIER)]
        public String tier;
        /*
        "Tier Name
        e.g) 1, 2, 3, 4, …."
        */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Def_Location : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TYPE)]
        public String locTp;
        /*
        enum enType- 다 대문자임
        {
            "Vessel",
            "Yard",
            "Rail",
            "TP",
            "IP",
            "Lane"
        };
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
        public String blck;
        /*
        "Block Name 
        e.g) 1A, 2A, 1B, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BAY)]
        public String bay;
        /*
        "Bay Name
        e.g) 1, 2, 3, 4, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_ROW)]
        public String row;
        /*
        "Row Name
        e.g) A, B, C, D, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_TIER)]
        public String tier;
        /*
        "Tier Name
        e.g) 1, 2, 3, 4, …."
        */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_LANE)]
        public String lane;
        //W : Water-side //L :Land-side
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_LOCATION)]
        public String location;
        /*
        "Location                             if Lane Type
        e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
        */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_Area : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_AREA_FROM)]
        public String from;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_AREA_TO)]
        public String to;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv2_NoWorkLocation : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_AREA_BLOCK)]
        public String blck;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_AREA_TYPE)]
        public String noWorkTp;
        EEv2_Area bay;
        EEv2_Area row;
        EEv2_Area tier;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3_Spreader
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_SPREADER_MODE)]
        public String sprdMd; //sy
        //    "SINGLE"
        //    "TWIN"
        //    "TANDEM"
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_SPREADER_TP)]
        public String sprdTp; //sy
        //    "SINGLE_SPREADER20"
        //    "SINGLE_SPREADER40"
        //    "SINGLE_SPREADER45"
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_SPREADER_STATE)]
        public String sprdSts; //sy
        //   "LS_SPREADER_LOCKED"
        //   "LS_SPREADER_UNLOCKED"
    }
}
