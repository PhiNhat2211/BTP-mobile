using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace VMT_Data_JAT2.Marshalling
{
    public class Geometry
    {
        #region [Geometry Manager Import Structure for RTG]

        //--------------------------------------------------------------------------------------
        //
        // For VMT_DataMgr DLL for Geometry Data
        //
        //--------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sPixelPoint
        {
            public int x;
            public int y;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sGeoPoint
        {
            public double lo;
            public double la;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [Serializable]
        public struct sBayPow
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String m_szID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String m_szBlock;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String m_szBay;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public String m_szBayEst;

            public int m_nRow;

            public sGeoPoint posTL;
            public sGeoPoint posTR;
            public sGeoPoint posBL;
            public sGeoPoint posBR;
        }

        public class sPosition
        {
            public String m_cBlock;
            public String m_cBay;
            public String m_cRow;
            public String m_cTier;

            public sPosition()
            {
                m_cBlock = String.Empty;
                m_cBay = String.Empty;
                m_cRow = String.Empty;
                m_cTier = String.Empty;
            }

            public Boolean Equal(sPosition p)
            {
                if (this.m_cBlock.Equals(p.m_cBlock) && this.m_cBay.Equals(p.m_cBay) &&
                    this.m_cRow.Equals(p.m_cRow) && this.m_cTier.Equals(p.m_cTier))
                    return true;
                return false;
            }

            public Boolean IsEmpty()
            {
                if (String.IsNullOrEmpty(this.m_cBlock) && String.IsNullOrEmpty(this.m_cBay) &&
                    String.IsNullOrEmpty(this.m_cRow) && String.IsNullOrEmpty(this.m_cTier))
                    return true;
                return false;
            }

            public void Clear()
            {
                m_cBlock = m_cBay = m_cRow = m_cTier = String.Empty;
            }
        }

        #endregion [Geometry Manager Import Structure for RTG]



        #region [Geometry Manager Import API for RTG]


        //----------------------------------------------------------
        //- Geometry Relation Functions
        //----------------------------------------------------------
        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InitGeometry();

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ReleaseGeometry();

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetCurrentPos([In][Out] ref double lo, [In][Out] ref double la);

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SetCurrentPos(double lo, double la);

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTotalBlockCount();

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTotalBayCount();

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetBayCount([MarshalAs(UnmanagedType.LPTStr)][In][Out] String szBlock);

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetRowCount([MarshalAs(UnmanagedType.LPTStr)][In][Out] String szBay);

        [DllImport("VMT_DataMgr", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTierCount([MarshalAs(UnmanagedType.LPTStr)][In][Out] String szRow);

        [DllImport("advapi32.dll")]
        public static extern void InitiateSystemShutdown(string lpMachineName, string lpMessage, int dwTimeout, bool bForceAppsClosed, bool bRebootAfterShutdown);


        #endregion [Geometry Manager Import API for RTG]

    }
}
