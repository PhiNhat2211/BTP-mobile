using log4net.Config;
using WPCommon.Util;
using LogExtensions;
using System;

namespace Common.Util
{
    public class Log4netMgr : Ext.Singleton<Log4netMgr>
    {
        public enum Log4netLevel
        {
            Debug,
            Info,
            Warn,
            Error
        }

        private const string fileName = @"log4net.config.xml";

        public Log4netMgr()
        {
            LoadLogConfig();
        }

        private void LoadLogConfig()
        {
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!System.IO.File.Exists(configPath))
                return;

            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configPath));
            LoggingExtensions.Core.Log.InitializeWith<LogExtensions.Log4NetLog>();
        }

        public void SetLogPolicy(int maxSizeRollBackups, string maximumFileSize)
        {
            LoggingExtensions.Core.Log._maxSizeRollBackups = maxSizeRollBackups;
            LoggingExtensions.Core.Log._maximumFileSize = maximumFileSize;
        }

        public void Log(string fileName, Log4netLevel level, string msg)
        {
            if (fileName == null)
                fileName = string.Empty;

            if(level == Log4netLevel.Debug)
                fileName.Log().Debug(() => msg);
            else if (level == Log4netLevel.Info)
                fileName.Log().Info(() => msg);
            else if (level == Log4netLevel.Warn)
                fileName.Log().Warn(() => msg);
            else if (level == Log4netLevel.Error)
                fileName.Log().Error(() => msg);
        }

        public void Log(string fileName, string msg, Exception ex)
        {
            if (fileName == null)
                fileName = string.Empty;

            fileName.Log().Error(() => msg, ex);
        }
    }
}
