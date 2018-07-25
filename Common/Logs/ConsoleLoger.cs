using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ConsoleLoger : ILog
    {
        public void WriteDebugLog(string info)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"debug {DateTime.Now.ToString("HH:mm:sss")}:{info}");
        }

        public void WriteErrorLog(string info)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error {DateTime.Now.ToString("HH:mm:sss")}:{info}");
        }

        public void WriteInfoLog(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"info {DateTime.Now.ToString("HH:mm:sss")}:{info}");
        }

        public void WriteLog(string info)
        {
            WriteInfoLog(info);
        }

        public void WriteLog(string info, Exception se)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error {DateTime.Now.ToString("HH:mm:sss")}:{info}");
            Console.WriteLine("\t" + se);
        }

        public void WriteLog(Exception se)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error {DateTime.Now.ToString("HH:mm:sss")}");
            Console.WriteLine("\t" + se);
        }
    }
}
