using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AOP
{
    class Program
    {
        static void Main(string[] args)
        {
            AOPTest.Test();
            DynamicProxyTest.Test();
            Console.ReadKey();
        }
    }
}
