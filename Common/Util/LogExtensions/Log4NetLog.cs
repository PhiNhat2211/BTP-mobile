using System.Globalization;
using log4net.Config;
using log4net.Core;
using log4net.Util;
using log4net.Appender;
using log4net.Layout;
using System;
using System.Runtime;
using global::log4net;
using global::log4net.Repository;

[assembly: XmlConfigurator(Watch = true)]
namespace LogExtensions
{
    /// <summary>
    /// Log4net logger implementing special ILog class
    /// </summary>
    public sealed class Log4NetLog : global::LoggingExtensions.Core.ILog, global::LoggingExtensions.Core.ILog<Log4NetLog>
    {
        private global::log4net.ILog _logger;

        // ignore Log4NetLog in the call stack
        private static readonly Type _declaringType = typeof(Log4NetLog);

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void InitializeFor(string loggerName, int maxSizeRollBackups, string maximumFileSize)
        {
            string pathFormat = string.Empty;
            if (string.IsNullOrEmpty(loggerName))
            {
                loggerName = "NoNamed";
                //pathFormat = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", loggerName + ".Log");
                pathFormat = System.IO.Path.Combine(
                             System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"), loggerName + ".Log");
            }
            else
            {
                pathFormat = System.IO.Path.Combine(
                             System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"), loggerName + ".Log");
            }

            RollingFileAppender configAppender = (RollingFileAppender)GetAppender(loggerName);

            RollingFileAppender newAppender = new RollingFileAppender();
            newAppender.StaticLogFileName = false;
            newAppender.File = pathFormat;
            newAppender.PreserveLogFileNameExtension = true;
            newAppender.LockingModel = new FileAppender.MinimalLock();

            if (configAppender == null)
            {   
                newAppender.AppendToFile = true;
                newAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;
                newAppender.DatePattern = "_yyyy-MM-dd";
                newAppender.MaxSizeRollBackups = maxSizeRollBackups; // About 1 Month
                newAppender.MaximumFileSize = maximumFileSize; // About 1 Day // ex) 1h 30m -> 1MB
                //newAppender.Layout = new PatternLayout("%date [%-5level] - %message%newline");
                newAppender.Layout = new PatternLayout("%d{yyyy-MM-dd HH:mm:ss.fff} [%-5level] - %message%newline");
                //newAppender.Layout = new PatternLayout("%d{yyyy-MM-dd HH:mm:ss.fff} %-5p %c> %m%n");
            }
            else
            {   
                newAppender.AppendToFile = configAppender.AppendToFile;
                newAppender.RollingStyle = configAppender.RollingStyle;
                newAppender.DatePattern = configAppender.DatePattern;
                newAppender.MaxSizeRollBackups = configAppender.MaxSizeRollBackups;
                newAppender.MaximumFileSize = configAppender.MaximumFileSize;
                newAppender.Layout = configAppender.Layout;
            }

            newAppender.ActivateOptions();

            DummyLogger dummyLogger = new DummyLogger(loggerName);
            dummyLogger.Level = Level.All;
            dummyLogger.AddAppender(newAppender);
            ILog ilog = new LogImpl(dummyLogger);
            _logger = ilog;

            //_logger = LogManager.GetLogger(loggerName);
        }

        private IAppender GetAppender(string appenderName)
        {
            ILoggerRepository repository = LogManager.GetRepository();
            foreach (IAppender appender in repository.GetAppenders())
            {
                if (appender.Name.CompareTo(appenderName) == 0)
                    return appender;
            }
            return null;
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Debug(string message, params object[] formatting)
        {
            if (_logger.IsDebugEnabled)
                Log(Level.Debug, message, formatting);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Debug(Func<string> message)
        {
            if (_logger.IsDebugEnabled)
                Log(Level.Debug, message);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Info(string message, params object[] formatting)
        {
            if (_logger.IsInfoEnabled)
                Log(Level.Info, message, formatting);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Info(Func<string> message)
        {
            if (_logger.IsInfoEnabled)
                Log(Level.Info, message);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Warn(string message, params object[] formatting)
        {
            if (_logger.IsWarnEnabled)
                Log(Level.Warn, message, formatting);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Warn(Func<string> message)
        {
            if (_logger.IsWarnEnabled)
                Log(Level.Warn, message);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Error(string message, params object[] formatting)
        {
            // don't check for enabled at this level
            //if (_logger.IsErrorEnabled)
            Log(Level.Error, message, formatting);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Error(Func<string> message)
        {
            // don't check for enabled at this level
            //if (_logger.IsErrorEnabled)
            Log(Level.Error, message);
        }

        public void Error(Func<string> message, Exception exception)
        {
            // don't check for enabled at this level
            //if (_logger.IsErrorEnabled)
            Log(Level.Error, message, exception);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Fatal(string message, params object[] formatting)
        {
            // don't check for enabled at this level
            //if(_logger.IsFatalEnabled)
            Log(Level.Fatal, message, formatting);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Fatal(Func<string> message)
        {
            // don't check for enabled at this level
            //if(_logger.IsFatalEnabled)
            Log(Level.Fatal, message);
        }

        public void Fatal(Func<string> message, Exception exception)
        {
            // don't check for enabled at this level
            //if(_logger.IsFatalEnabled)
            Log(Level.Fatal, message, exception);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private void Log(Level level, Func<string> message)
        {
            Log(level, message(), null);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private void Log(Level level, Func<string> message, Exception exception)
        {
            _logger.Logger.Log(_declaringType, level, message(), exception);
        }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private void Log(Level level, string message, params object[] formatting)
        {
            // SystemStringFormat is used to evaluate the message as late as possible. A filter may discard this message.
            _logger.Logger.Log(_declaringType, level, new SystemStringFormat(CultureInfo.CurrentCulture, message, formatting), null);
        }
    }
}