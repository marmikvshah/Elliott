using API.Trade.Shared.Logger;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace API.Trade.Shared.Logger
{
    public class TLogger : ILogger
    {
        private string _originalLogFileName;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILogHelpers _logHelpers = new LogHelpers();

        public TLogger()
        {
            GlobalContext.Properties["LogPath"] = _logHelpers.ReturnLogsFolder();
        }

        public void InitConsoleApp()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            var baseDirectory = Path.GetDirectoryName(assembly.Location);
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            string logName = assembly.GetName().Name;
            GlobalContext.Properties["LogName"] = logName;
            GlobalContext.Properties["Version"] = version;

            // Add logging
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var file = new FileInfo(Path.Combine(baseDirectory, "log4net.config"));
            XmlConfigurator.Configure(logRepository, file);
        }

        public void ChangeLogFileName(string logFileName)
        {
            if (string.IsNullOrEmpty(_originalLogFileName))
            {
                if (GlobalContext.Properties["LogName"] != null)
                {
                    _originalLogFileName = GlobalContext.Properties["LogName"].ToString();
                }
            }

            var entryAssembly = Assembly.GetEntryAssembly();
            GlobalContext.Properties["LogName"] = logFileName;
            var logRepository = LogManager.GetRepository(entryAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        public void RevertToOriginalLogFileName()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            GlobalContext.Properties["LogName"] = _originalLogFileName;
            var logRepository = LogManager.GetRepository(entryAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        public string GetLogProperty(string logKey)
        {
            return GlobalContext.Properties[logKey].ToString();
        }

        public void Debug(object message)
        {
            log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            log.Debug(message, exception);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.DebugFormat(provider, format, args);
        }

        public void DebugFormat(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            log.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            log.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            log.DebugFormat(format, arg0, arg1);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            log.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.ErrorFormat(provider, format, args);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            log.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0)
        {
            log.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            log.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(string format, object arg0)
        {
            log.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.FatalFormat(provider, format, args);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            log.FatalFormat(format, arg0, arg1);
        }

        public void Info(object message, Exception exception)
        {
            log.Info(message, exception);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            log.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            log.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0)
        {
            log.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            log.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0)
        {
            log.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.WarnFormat(provider, format, args);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            log.WarnFormat(format, arg0, arg1, arg2);
        }
    }
}
