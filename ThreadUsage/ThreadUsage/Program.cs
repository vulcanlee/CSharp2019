using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            使用同步方式呼叫();
        }
        static void 使用同步方式呼叫()
        {
            MyMethod1();
            MyMethod2("-");
        }
        static void MyMethod1()
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write("*");
            }
        }
        static void MyMethod2(object message)
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write(message.ToString());
            }
        }
    }
}
