using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LogerProxy : ILog
    {
        private static List<ILog> logerList;
        private static LogerProxy _instance = new LogerProxy();
        static LogerProxy()
        {
            logerList = new List<ILog>
            {
                new Log4NetLoger()
            };
            if (Environment.UserInteractive)
            {
                logerList.Add(new ConsoleLoger());
            }
        }

        public static LogerProxy Instance { get; } = _instance;

        public void WriteDebugLog(string info)
        {
            logerList.ForEach(lg => lg.WriteDebugLog(info));
        }

        public void WriteErrorLog(string info)
        {
            logerList.ForEach(lg => lg.WriteErrorLog(info));
        }

        public void WriteInfoLog(string info)
        {
            logerList.ForEach(lg => lg.WriteInfoLog(info));
        }

        public void WriteLog(string info)
        {
            logerList.ForEach(lg => lg.WriteLog(info));
        }

        public void WriteLog(string info, Exception se)
        {
            logerList.ForEach(lg => lg.WriteLog(info, se));
        }

        public void WriteLog(Exception se)
        {
            logerList.ForEach(lg => lg.WriteLog(se));
        }
    }
}
