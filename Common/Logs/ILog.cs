using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ILog
    {
        void WriteDebugLog(string info);

        void WriteInfoLog(string info);

        void WriteErrorLog(string info);

        void WriteLog(string info);

        void WriteLog(string info, Exception se);

        void WriteLog(Exception se);
    }
}
