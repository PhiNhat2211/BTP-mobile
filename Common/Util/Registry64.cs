using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Util
{
    public class Registry64
    {
        #region [Check 64/32bit OS]
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        public static bool Is64Bit()
        {
            if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool Is32BitProcessOn64BitProcessor()
        {
            bool retVal;

            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);

            return retVal;
        }
        #endregion [Check 64/32bit OS]

        #region [Open Reg]
        public enum RegWow64Options
        {
            None = 0,
            KEY_WOW64_64KEY = 0x0100,
            KEY_WOW64_32KEY = 0x0200
        }

        public enum RegistryRights
        {
            ReadKey = 131097,
            WriteKey = 131078
        }

        /// <summary>
        /// Open a registry key using the Wow64 node instead of the default 32-bit node.
        /// </summary>
        /// <param name="parentKey">Parent key to the key to be opened.</param>
        /// <param name="subKeyName">Name of the key to be opened</param>
        /// <param name="writable">Whether or not this key is writable</param>
        /// <param name="options">32-bit node or 64-bit node</param>
        /// <returns></returns>
        public static RegistryKey _openSubKey(RegistryKey parentKey, string subKeyName, bool writable, RegWow64Options options)
        {
            //Sanity check
            if (parentKey == null || _getRegistryKeyHandle(parentKey) == IntPtr.Zero)
            {
                return null;
            }

            //Set rights
            int rights = (int)RegistryRights.ReadKey;
            if (writable)
                rights = (int)RegistryRights.WriteKey;

            //Call the native function >.<
            int subKeyHandle, result = RegOpenKeyEx(_getRegistryKeyHandle(parentKey), subKeyName, 0, rights | (int)options, out subKeyHandle);

            //If we errored, return null
            if (result != 0)
            {
                return null;
            }

            //Get the key represented by the pointer returned by RegOpenKeyEx
            RegistryKey subKey = _pointerToRegistryKey((IntPtr)subKeyHandle, writable, false);
            return subKey;
        }

        /// <summary>
        /// Get a pointer to a registry key.
        /// </summary>
        /// <param name="registryKey">Registry key to obtain the pointer of.</param>
        /// <returns>Pointer to the given registry key.</returns>
        public static IntPtr _getRegistryKeyHandle(RegistryKey registryKey)
        {
            //Get the type of the RegistryKey
            Type registryKeyType = typeof(RegistryKey);
            //Get the FieldInfo of the 'hkey' member of RegistryKey
            System.Reflection.FieldInfo fieldInfo =
            registryKeyType.GetField("hkey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            //Get the handle held by hkey
            SafeHandle handle = (SafeHandle)fieldInfo.GetValue(registryKey);
            //Get the unsafe handle
            IntPtr dangerousHandle = handle.DangerousGetHandle();
            return dangerousHandle;
        }

        /// <summary>
        /// Get a registry key from a pointer.
        /// </summary>
        /// <param name="hKey">Pointer to the registry key</param>
        /// <param name="writable">Whether or not the key is writable.</param>
        /// <param name="ownsHandle">Whether or not we own the handle.</param>
        /// <returns>Registry key pointed to by the given pointer.</returns>
        public static RegistryKey _pointerToRegistryKey(IntPtr hKey, bool writable, bool ownsHandle)
        {
            //Get the BindingFlags for private contructors
            System.Reflection.BindingFlags privateConstructors = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            //Get the Type for the SafeRegistryHandle
            Type safeRegistryHandleType = typeof(Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid).Assembly.GetType("Microsoft.Win32.SafeHandles.SafeRegistryHandle");
            //Get the array of types matching the args of the ctor we want
            Type[] safeRegistryHandleCtorTypes = new Type[] { typeof(IntPtr), typeof(bool) };
            //Get the constructorinfo for our object
            System.Reflection.ConstructorInfo safeRegistryHandleCtorInfo = safeRegistryHandleType.GetConstructor(
            privateConstructors, null, safeRegistryHandleCtorTypes, null);
            //Invoke the constructor, getting us a SafeRegistryHandle
            Object safeHandle = safeRegistryHandleCtorInfo.Invoke(new Object[] { hKey, ownsHandle });

            //Get the type of a RegistryKey
            Type registryKeyType = typeof(RegistryKey);
            //Get the array of types matching the args of the ctor we want
            Type[] registryKeyConstructorTypes = new Type[] { safeRegistryHandleType, typeof(bool) };
            //Get the constructorinfo for our object
            System.Reflection.ConstructorInfo registryKeyCtorInfo = registryKeyType.GetConstructor(
            privateConstructors, null, registryKeyConstructorTypes, null);
            //Invoke the constructor, getting us a RegistryKey
            RegistryKey resultKey = (RegistryKey)registryKeyCtorInfo.Invoke(new Object[] { safeHandle, writable });
            //return the resulting key
            return resultKey;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, int ulOptions, int samDesired, out int phkResult);
        #endregion [Open Reg]

        #region [Read Write Delete Reg]
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW", SetLastError = true)]
        public static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, uint options, int sam, out UIntPtr phkResult);
        public static UIntPtr HKEY_CURRENT_USER = (UIntPtr)0x80000001;
        public static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
        public static int KEY_QUERY_VALUE = 0x0001;
        public static int KEY_SET_VALUE = 0x0002;
        public static int KEY_CREATE_SUB_KEY = 0x0004;
        public static int KEY_ENUMERATE_SUB_KEYS = 0x0008;
        public static int KEY_WOW64_64KEY = 0x0100;
        public static int KEY_WOW64_32KEY = 0x0200;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        public static extern int RegQueryValueEx(UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType,
          StringBuilder lpData, ref int lpcbData);

        [DllImport("advapi32.dll")]
        static extern uint RegSetValueEx(UIntPtr hKey, [MarshalAs(UnmanagedType.LPStr)] string lpValueName, int Reserved, RegistryValueKind dwType, string lpData, int cbData);


        [DllImport("advapi32.dll")]
        static extern uint RegDeleteValue(UIntPtr hKey, [MarshalAs(UnmanagedType.LPStr)] string lpValueName);

        public static UIntPtr GetRegUInt(UIntPtr hKey, String dir)
        {
            UIntPtr regKeyHandle;
            RegOpenKeyEx(hKey, dir, 0, KEY_SET_VALUE | KEY_QUERY_VALUE | (Is64Bit() ? KEY_WOW64_64KEY : KEY_WOW64_32KEY), out regKeyHandle);          
            return regKeyHandle;
        }
        public static String GetRegValue(UIntPtr regKeyHandle, String key)
        {
            uint type;
            StringBuilder stringBuilder = new StringBuilder(2048);
            int cbData = stringBuilder.Capacity;
            if (RegQueryValueEx(regKeyHandle, key, 0, out type, stringBuilder, ref cbData) == 0)
            {
                return Convert.ToString(stringBuilder);
            }
            return String.Empty;
        }
        public static void TrySetRegValue(UIntPtr regKeyHandle, String key, String value)
        {
            RegSetValueEx(regKeyHandle, key, 0, RegistryValueKind.String, value, ASCIIEncoding.ASCII.GetByteCount(value));
        }
        public static void TryDeleteRegValue(UIntPtr regKeyHandle, String key)
        {
            RegDeleteValue(regKeyHandle, key);
        }
        #endregion [Read Write Delete Reg]
    }
}
