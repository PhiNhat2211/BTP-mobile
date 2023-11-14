using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
//using VMT_RMG_800by600;

using VMT_Data_JAT2;
using System.IO;
using Microsoft.Win32;
using static Common.Util.Registry64;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum ResolutionType
        {
            TYPE_1024by768 = 0,
            TYPE_800by600,
        }

        private static readonly string strAppConstName = "CLT_VMT_RMG";
        private static Mutex appMutex;

        public static string _lastExceptionMessage = "";

        //static public ResolutionType CurrentResolution = ResolutionType.TYPE_800by600;

        // Common for All Site
        static public String KeyCLTAgent = @"SOFTWARE\CyberLogitec\CLT Agent for Windows";
        static public Boolean TEST_MODE = false; // true - test mode, false - real mode
        static public Boolean TEST_WRITE_MODE = false; // TRUE - Write log to File
        static public Boolean STANDALONE_MODE = false; // true - stand alone mode, false - real mode
        static public Boolean MESSAGE_CAPTURE_MODE = false; // true - capture mode, false - no capture

        public App()
        {
            deleteLog();
            // Sep 22 2021 Change registry from CURRENT_USER to LOCAL_MACHINE
            UIntPtr uIntPtr = GetRegUInt(HKEY_LOCAL_MACHINE, VMT_RMG.MainWindow.KeyCLTVMT_RMG);
            if (uIntPtr.ToUInt32() > 0)
            {
                if (GetRegValue(uIntPtr, "WaitingUpdate").Contains("1"))
                {
                    TrySetRegValue(uIntPtr, "WaitingUpdate", "0");
                }
            }
            //Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(VMT_RMG.MainWindow.KeyCLTVMT_RMG, true);
            //string strWaitingUpdate = "";
            //if (keyDir != null)
            //    strWaitingUpdate = (String)keyDir.GetValue("WaitingUpdate", @"Unknown");
            //if (strWaitingUpdate.Contains("1"))
            //{
            //    //Environment.Exit(0);
            //    keyDir.SetValue("WaitingUpdate", "0");
            //}
            ForeignInfo.GetUserLanguagePath(ref ForeignInfo.languagePathUser);
            ForeignInfo.GetExternalTruck(ref ForeignInfo.externalTruck);
            ForeignInfo.GetYTAssigned(ref ForeignInfo.ytAssigned);
            ForeignInfo.GetJobTypeSortOrder(ref ForeignInfo.jobType);

            // Check another Music Player instance was exist.
            bool mutextIsNew;
            appMutex = new System.Threading.Mutex(true, strAppConstName, out mutextIsNew);

            if (!mutextIsNew)
                Application.Current.Shutdown();

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

            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Log");
            string exceptionLogFile = string.Format(@"{0}\Log\ExceptionLog_{1}.txt", AppDomain.CurrentDomain.BaseDirectory, timetag);

            System.IO.File.WriteAllText(exceptionLogFile, exceptionLog);

            MessageBox.Show("[Message]" + _exLast.Message + Environment.NewLine + 
                "[Stack Trace]" + _exLast.StackTrace);

            e.Handled = true;
        }

        //private ResolutionType GetResolutionType()
        //{
        //    var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        //    if (screen.Width >= 1024 && screen.Height >= 768)
        //        App.CurrentResolution = ResolutionType.TYPE_1024by768;
        //    else
        //        App.CurrentResolution = ResolutionType.TYPE_800by600;

        //    return App.CurrentResolution;
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashForm.ShowSplash();
            
            //String uriString = String.Empty;
            ////switch (App.CurrentResolution) 
            //switch (GetResolutionType())
            //{
            //    case ResolutionType.TYPE_800by600:
            //        uriString = "AppCodes(800_600)/MainWindow.xaml";
            //        break;
            //    case ResolutionType.TYPE_1024by768:
            //    default:
            //        uriString = "AppCodes/MainWindow.xaml";
            //        break;
            //}

            //var uri = new Uri(uriString, System.UriKind.Relative);
            //this.StartupUri = uri;
        }

        public void deleteLog()  // nDataType 0 EEv2JobOrder, 
        {
            try
            {
                string sRootPath = AppCfgMgr.GetAppDirectory();
                string sDirPath = sRootPath + @"JOBCLICK_log";
                if (Directory.Exists(sDirPath) == true)
                {
                    var dir = new DirectoryInfo(sDirPath);
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static string GetAppDirectory()
        {
            string path = null;

            path = AppDomain.CurrentDomain.BaseDirectory;

            return path;
        }
    }
}
