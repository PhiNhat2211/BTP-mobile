using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using VMT_Data_JAT2;
using System.IO;
using System.Diagnostics;
using static Common.Util.Registry64;
using Microsoft.Win32;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string strAppConstName = "CLT_VMT_ITV";
        private static Mutex appMutex;

        public static string _lastExceptionMessage = "";

        public App()
        {
            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, VMT_ITV.MainWindow.KeyCLTVMT_ITV);
            if (uIntPtr.ToUInt32() > 0)
            {
                if (GetRegValue(uIntPtr, "WaitingUpdate").Contains("1"))
                {
                    TrySetRegValue(uIntPtr, "WaitingUpdate", "0");
                }
            }
            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(VMT_ITV.MainWindow.KeyCLTVMT_ITV, true);
            //string strWaitingUpdate = "";
            //if (keyDir != null)
            //    strWaitingUpdate = (String)keyDir.GetValue("WaitingUpdate", @"Unknown");
            //if (strWaitingUpdate.Contains("1"))            
            //{
            //    //Environment.Exit(0);
            //    keyDir.SetValue("WaitingUpdate", "0");
            //}

            //ForeignInfo.GetUserLanguageType(ref ForeignInfo.languageUser);
            ForeignInfo.GetUserLanguagePath(ref ForeignInfo.languagePathUser);

            //Count the number of proccess
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                p.ProcessName == proc.ProcessName).Count();

            if (count > 3)
            {
                MessageBox.Show("Maximum instances reached...");
                App.Current.Shutdown();
            }

            // Check another Music Player instance was exist.
            bool mutextIsNew;
            appMutex = new System.Threading.Mutex(true, strAppConstName, out mutextIsNew);

            //if (!mutextIsNew)
            //    Application.Current.Shutdown();

            // Event handler to process unhandled exception from current application
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        // Unhanded Exception Handler
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception _exLast = e.Exception;

            while (_exLast.InnerException != null)
            {
                _exLast = _exLast.InnerException;
            }

            _lastExceptionMessage = _exLast.Message;

            string exceptionLog = string.Format("Message : {0}\r\n", _exLast.Message);
            exceptionLog += string.Format("Source : {0}\r\n", _exLast.Source);
            exceptionLog += string.Format("[StackTrace]\r\n{0}\r\n", _exLast.StackTrace);

            DateTime time = DateTime.Now;             // Use current time
            string format = "yyyy-MM-dd_HH-mm-ss";      // Use this format
            string timetag = time.ToString(format);   // Write to console


            string exceptionLogFile = string.Format(@"{0}\Log\ExceptionLog_{1}.txt", AppDomain.CurrentDomain.BaseDirectory, timetag);

            try
            {
                System.IO.File.WriteAllText(exceptionLogFile, exceptionLog);

                MessageBox.Show(_exLast.Message);

                e.Handled = true;
            }
            catch
            {
                Directory.CreateDirectory(exceptionLogFile);
            }

        }
    }
}
