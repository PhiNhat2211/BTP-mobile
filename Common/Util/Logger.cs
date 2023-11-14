using System;
using System.Diagnostics;
using System.IO;

namespace Common.Util
{   
    public class Logger
    {
        public enum LogFilters
        {
            Trace = 1, // Extremely detailed and repetitive (many times per frame) debugging information that is likely to pollute logs.
            Debug = 2, // Less detailed debugging information.
            Info = 3, // Status information from important points during execution.
            Warning = 4, // Minor or potential problems found during execution of a task.
            Error = 5, // Major problems found during execution of a task that prevent it from being completed.
            Critical = 6 // Major problems during execution that threathen the stability of the entire application. (This severity cannot be filtered out.)
        }

        private static string _logPath = string.Empty;
        private static LogFilters _logFilter = LogFilters.Error;


        // 특정 환경에서 c#에서 제공하는 writeline 이 안먹히는것이 있어 이걸로 사용
        [System.Runtime.InteropServices.DllImport("kernel32", EntryPoint = "OutputDebugStringA")]
        public static extern int OutputDebugString(string lpOutputString);

        public static void SetLogPath(string path)
        {
            _logPath = path;
        }

        public static void SetLogFilters(LogFilters filter)
        {
            _logFilter = filter;
        }

        private static int _maxSizeRollBackups = 7;
        private static string _maximumFileSize = "20MB";
        public static void SetLogPolicy(int maxSizeRollBackups, string maximumFileSize)
        {
            _maxSizeRollBackups = maxSizeRollBackups;
            _maximumFileSize = maximumFileSize;

            Log4netMgr.Instance.SetLogPolicy(maxSizeRollBackups, maximumFileSize);

            string strLogFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (Directory.Exists(strLogFolder))
            {
                string[] fileArray = Directory.GetFiles(strLogFolder, "*.*", SearchOption.AllDirectories);
                foreach (string file in fileArray)
                {
                    FileInfo fInfo = new FileInfo(file);
                    TimeSpan tSpan = DateTime.UtcNow - fInfo.LastWriteTimeUtc;
                    TimeSpan dDay = new TimeSpan(_maxSizeRollBackups, 0, 0, 0);

                    if (tSpan - dDay > TimeSpan.Zero)
                    {
                        fInfo.Delete();
                    }
                }
            }
        }

        public static void Log(string logMessage, LogFilters filter = LogFilters.Error)
        {
            if (filter < _logFilter)
            {
                return;
            }

            string output = null;

            StackTrace st = new StackTrace(true);
            StackFrame sf = (st != null) ? st.GetFrame(1) : null;

            try
            {
                if (sf == null)
                {                    
                    output = logMessage;
                }
                else
                {
                    string fileName = Path.GetFileName(sf.GetFileName());
                    output = // "[VMT] "
                        //+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " "
                        logMessage + " <----- "
                        + sf.GetMethod().Name
                        + "() Line:" + sf.GetFileLineNumber()
                        + " " + fileName;
                }
            }
            catch (Exception)
            {                
            }

            if (_logPath != string.Empty)
            {
                //SetWriteLog(output, _logPath);
                Log4netMgr.Instance.Log(_logPath, Log4netMgr.Log4netLevel.Info, output);
            }
            else
            {
#if DEBUG
                Trace.WriteLine(output);
#else
                OutputDebugString(output);
#endif
            }
        }

        private static object lockObject = new object();
        private static void SetWriteLog(string logstring, string logFilePath)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\" + logFilePath;
            FileInfo fileinfo = new FileInfo(filepath);

            if (Directory.Exists(fileinfo.DirectoryName) == false)
            {
                Directory.CreateDirectory(fileinfo.DirectoryName);
            }

            try
            {
                lock (lockObject)
                {
                    FileStream filesavestream = File.Open(filepath, FileMode.Append);
                    StreamWriter sw = new StreamWriter(filesavestream);

                    sw.WriteLine(logstring);
                    sw.Close();
                }
            }
            catch (Exception)
            {                
            }
        }
    }
}
