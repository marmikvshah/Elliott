using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace API.Trade.Shared.Logger
{
    public class LogHelpers : ILogHelpers
    {
        public string ReturnLogsFolder()
        {
            string logsFolder = "E:\\Logs";
            string drive = Path.GetPathRoot(logsFolder);
            if (Directory.Exists(drive))
            {
                return logsFolder;
            }
            else
            {
                logsFolder = "D:\\Logs";
            }

            drive = Path.GetPathRoot(logsFolder);
            if (Directory.Exists(drive))
            {
                return logsFolder;
            }
            else
            {
                logsFolder = "C:\\Logs";
            }
            return logsFolder;
        }
    }
}
