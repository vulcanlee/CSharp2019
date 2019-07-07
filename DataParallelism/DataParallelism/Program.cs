using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataParallelism
{
    class Program
    {
        static int MaxLoop = 30;
        static int SimulateBusy = 5000;
        static void Main(string[] args)
        {
            //Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)2;
            //ThreadPool.SetMinThreads(4, 4);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            ParallelFor();
            //ParallelForEach();
            sw.Stop();

            Console.WriteLine($"花費時間: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void ParallelFor()
        {
            Parallel.For(0, MaxLoop, x =>
            {
                Console.WriteLine($"開始非同步工作{x}, Thread ID={Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(SimulateBusy);
                Console.WriteLine($"結束非同步工作{x}, Thread ID={Thread.CurrentThread.ManagedThreadId}");
            });
        }

        private static void ParallelForEach()
        {
            var numbers = Enumerable.Range(0, MaxLoop);
            Parallel.ForEach(numbers, x =>
            {
                Console.WriteLine($"開始非同步工作{x}, Thread ID={Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(SimulateBusy);
                Console.WriteLine($"結束非同步工作{x}, Thread ID={Thread.CurrentThread.ManagedThreadId}");
            });
        }
    }
}
