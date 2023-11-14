using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.WLan
{
    /*  Use
     *  Utilities.WLan.Wifi wifi = new Utilities.WLan.Wifi();
        Utilities.WLan.WLanInterfaceInfo[] wLanInterfaceInfoList = wifi.EnumerateNICs();

        foreach (Utilities.WLan.WLanInterfaceInfo wLanInterfaceInfo in wLanInterfaceInfoList)
        {
            string strInterfaceDescription = wLanInterfaceInfo.InterfaceDescription;
            string strState = wLanInterfaceInfo.State.ToString();

            SaveLog("Wi-Fi State",
                "Description : " + strInterfaceDescription + " State : " + strState);

            string strLog = "";
            strLog += "---------------------------------------------\r\n";
            strLog += "Message Name : WLanInterfaceInfo\r\n";
            strLog += string.Format("InterfaceDescription : {0}\r\n", strInterfaceDescription);
            strLog += string.Format("State : {0}\r\n", strState);
            strLog += "---------------------------------------------\r\n";

            MainWindow.LogWin.WriteLog(strLog);
        }
     */
    /// <summary>
    /// Interface state enums
    /// </summary>
    public enum WLanInterfaceState : int
    {
        NotReady = 0,
        Connected,
        AdHocNetworkFormed,
        Disconnecting,
        Disconnected,
        Associating,
        Discovering,
        Authenticating
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WLanInterfaceInfo
    {
        /// GUID->_GUID
        public Guid InterfaceGuid;

        /// WCHAR[256]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string InterfaceDescription;

        /// WLAN_INTERFACE_STATE->_WLAN_INTERFACE_STATE
        public WLanInterfaceState State;
    }

    /// <summary>
    /// This structure contains an array of NIC information
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WLanInterfaceInfoList
    {
        public Int32 dwNumberofItems;
        public Int32 dwIndex;
        public WLanInterfaceInfo[] InterfaceInfo;

        public WLanInterfaceInfoList(IntPtr pList)
        {
            // The first 4 bytes are the number of WLAN_INTERFACE_INFO structures.
            dwNumberofItems = Marshal.ReadInt32(pList, 0);

            // The next 4 bytes are the index of the current item in the unmanaged API.
            dwIndex = Marshal.ReadInt32(pList, 4);

            // Construct the array of WLAN_INTERFACE_INFO structures.
            InterfaceInfo = new WLanInterfaceInfo[dwNumberofItems];

            for (int i = 0; i < dwNumberofItems; i++)
            {
                // The offset of the array of structures is 8 bytes past the beginning. Then, take the index and multiply it by the number of bytes in the structure.
                // the length of the WLAN_INTERFACE_INFO structure is 532 bytes - this was determined by doing a sizeof(WLAN_INTERFACE_INFO) in an unmanaged C++ app.
                IntPtr pItemList = new IntPtr(pList.ToInt32() + (i * 532) + 8);

                // Construct the WLAN_INTERFACE_INFO structure, marshal the unmanaged structure into it, then copy it to the array of structures.
                WLanInterfaceInfo wii = new WLanInterfaceInfo();
                wii = (WLanInterfaceInfo)Marshal.PtrToStructure(pItemList, typeof(WLanInterfaceInfo));
                InterfaceInfo[i] = wii;
            }
        }
    }

    public class Wifi
    {
        #region API

        private const int WLAN_API_VERSION_2_0 = 2; //Windows Vista WiFi API Version
        private const int WLAN_API_XP_VERSION = 1;
        private const int ERROR_SUCCESS = 0;

        /// <summary>
        /// Opens a connection to the server
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanOpenHandle(UInt32 dwClientVersion, IntPtr pReserved, out UInt32 pdwNegotiatedVersion, ref IntPtr phClientHandle);

        /// <summary>
        /// Closes a connection to the server
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        /// <summary>
        /// Enumerates all wireless interfaces in the laptop
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved, ref IntPtr ppInterfaceList);

        /// <summary>
        /// Frees memory returned by native WiFi functions
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern void WlanFreeMemory(IntPtr pmemory);

        #endregion
        #region Functions

        /// <summary>
        ///get NIC state  
        /// </summary>
        public static string GetStateDescription(WLanInterfaceState state)
        {
            string stateDescription = string.Empty;
            switch (state)
            {
                case WLanInterfaceState.NotReady:
                    stateDescription = "Not Ready";
                    break;
                case WLanInterfaceState.Connected:
                    stateDescription = "Connected";
                    break;
                case WLanInterfaceState.AdHocNetworkFormed:
                    stateDescription = "First node in an adhoc network";
                    break;
                case WLanInterfaceState.Disconnecting:
                    stateDescription = "Disconnecting";
                    break;
                case WLanInterfaceState.Disconnected:
                    stateDescription = "Disconnected";
                    break;
                case WLanInterfaceState.Associating:
                    stateDescription = "Associating";
                    break;
                case WLanInterfaceState.Discovering:
                    stateDescription = "Discovering";
                    break;
                case WLanInterfaceState.Authenticating:
                    stateDescription = "Authenticating";
                    break;
            }

            return stateDescription;
        }

        /// <summary>
        /// enumerate wireless network adapters using wifi api
        /// </summary>
        public WLanInterfaceInfo[] EnumerateNICs()
        {
            List<WLanInterfaceInfo> ret = new List<WLanInterfaceInfo>();
            uint serviceVersion = 0;
            IntPtr handle = IntPtr.Zero;
            if (WlanOpenHandle(2, IntPtr.Zero, out serviceVersion, ref handle) == ERROR_SUCCESS)
            {
                IntPtr ppInterfaceList = IntPtr.Zero;
                WLanInterfaceInfoList interfaceList;

                if (WlanEnumInterfaces(handle, IntPtr.Zero, ref ppInterfaceList) == ERROR_SUCCESS)
                {
                    //Tranfer all values from IntPtr to WLAN_INTERFACE_INFO_LIST structure 
                    interfaceList = new WLanInterfaceInfoList(ppInterfaceList);

                    for (int i = 0; i < interfaceList.dwNumberofItems; i++)
                        ret.Add(interfaceList.InterfaceInfo[i]);
                    //ret += interfaceList.InterfaceInfo[i].strInterfaceDescription + " --> " + GetStateDescription(interfaceList.InterfaceInfo[i].isState) + "\n";

                    //frees memory
                    if (ppInterfaceList != IntPtr.Zero)
                        WlanFreeMemory(ppInterfaceList);
                }
                //close handle
                WlanCloseHandle(handle, IntPtr.Zero);
            }
            return ret.ToArray();
        }

        #endregion
    }
}