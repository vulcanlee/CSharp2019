using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowThreadPoolInformation();
        }
        static void ShowThreadPoolInformation()
        {
            int workerThreads = 0;
            int completionPortThreads = 0;
            Console.WriteLine($"現在執行緒集區內的數量資訊");
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"執行緒集區的背景工作執行緒最小數目 : {workerThreads}");
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"可用背景工作執行緒的數目 : {workerThreads}");
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"執行緒集區中的背景工作執行緒最大數目 : {workerThreads}");
            Console.WriteLine($"");
        }
    }
}
