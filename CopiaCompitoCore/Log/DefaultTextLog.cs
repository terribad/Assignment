using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace AssignmentCore
{
    public class DefaultTextLog : ILog
    {
        private StreamWriter sw;
        private LogLevel logLevel;

        public DefaultTextLog(LogLevel level, string fileName = null, bool deletePre = false)
        {
            if (fileName == null)
                fileName = MakeLogName();
            if (File.Exists(fileName) && deletePre)
                File.Delete(fileName);
            sw = new StreamWriter(fileName, true);
            sw.AutoFlush = true;
            this.logLevel = level;
            InitSession();
        }

        public void InitSession()
        {
            sw.WriteLine("#Nuova sessione: {0:t}", Global.Time.Now());
        }

        public void Write(LogLevel level, string msg, params object[] args)
        {
            if (level > logLevel)
                return;
            string prefix = level == LogLevel.Error ? "[ERROR] " : "[INFO] ";
            string text = prefix + string.Format(msg, args);
            sw.WriteLine(ToError(text));
        }
        public void Close()
        {
            if (sw != null)
                sw.Close();
        }

        public static string ToError(string target, string fix = "\r\n\t")
        {
            return target.Replace("\n", fix);
        }

        private static string MakeLogName()
        {
            var logName = Path.Combine(Path.GetTempPath(), Assembly.GetEntryAssembly().GetName().Name);
            return string.Format("{0} {1:dd-MM-yyyy}.{2}", logName, Global.Time.Now(), "log");
        }
    }
}
