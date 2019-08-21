using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentCore
{
    public static class Log
    {
        private static ILog LogSystem { get; set; }       

        public static void Open(ILog logSystem = null)
        {
            Close();
            
            if (logSystem == null)
                logSystem = new DefaultTextLog(LogLevel.Error);
            LogSystem = logSystem;
        }

        public static void Write(LogLevel level, string msg, params object[] args)
        {
            LogSystem.Write(level, msg, args);
        }
        public static void Close()
        {
            if (LogSystem != null)
                LogSystem.Close();
            LogSystem = null;
        }
    }

    public interface ILog
    {
        void Write(LogLevel level, string msg, params object[] args);
        void Close();
    }
    public class NoLog : ILog
    {
        public void Write(LogLevel level, string msg, params object[] args)
        {
        }

        public void Close()
        {
        }
    }

    public enum LogLevel
    {
        Error = 0,
        Normal = 1,
        Verbose = 2
    }
}
