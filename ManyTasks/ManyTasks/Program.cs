using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManyTasks
{
    class Program
    {
        public static int MaxThreads;
        static int MaxLoop = 200;
        static int SimulateTaskTime = 2000;
        static CountdownEvent  countdownEvent = new CountdownEvent(MaxLoop);
        static void Main(string[] args)
        {
            //Case1();
            //Case2();
            //Case3();
            //Case4();
            Case5();
        }

        private static void Case1()
        {
            int CurrentAvailableThreads = GetAvailableThreads();
            List<Task> allTask = new List<Task>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < MaxLoop; i++)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    int tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(SimulateTaskTime);
                    tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    countdownEvent.Signal();
                });
            }

            countdownEvent.Wait();
            stopwatch.Stop();

            PrintThreadPoolInformation();
            Console.WriteLine($"此次使用到 {MaxThreads} 個背景執行緒");
            Console.WriteLine($"此次共花費 {stopwatch.Elapsed} 時間");
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void Case2()
        {
            int CurrentAvailableThreads = GetAvailableThreads();
            List<Task> allTask = new List<Task>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < MaxLoop; i++)
            {
                allTask.Add(Task.Run(() =>
                {
                    int tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(SimulateTaskTime);
                    tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    countdownEvent.Signal();
                }));
            }

            countdownEvent.Wait();
            stopwatch.Stop();

            PrintThreadPoolInformation();
            Console.WriteLine($"此次使用到 {MaxThreads} 個背景執行緒");
            Console.WriteLine($"此次共花費 {stopwatch.Elapsed} 時間");
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void Case3()
        {
            ThreadPool.SetMinThreads(16, 16);
            int CurrentAvailableThreads = GetAvailableThreads();
            List<Task> allTask = new List<Task>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < MaxLoop; i++)
            {
                allTask.Add(Task.Run(() =>
                {
                    int tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(SimulateTaskTime);
                    tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    countdownEvent.Signal();
                }));
            }

            countdownEvent.Wait();
            stopwatch.Stop();

            PrintThreadPoolInformation();
            Console.WriteLine($"此次使用到 {MaxThreads} 個背景執行緒");
            Console.WriteLine($"此次共花費 {stopwatch.Elapsed} 時間");
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void Case4()
        {
            int CurrentAvailableThreads = GetAvailableThreads();
            List<Task> allTask = new List<Task>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < MaxLoop; i++)
            {
                allTask.Add(Task.Factory.StartNew(() =>
                {
                    int tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(SimulateTaskTime);
                    tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    countdownEvent.Signal();
                }, TaskCreationOptions.LongRunning));
            }

            countdownEvent.Wait();
            stopwatch.Stop();

            PrintThreadPoolInformation();
            Console.WriteLine($"此次使用到 {MaxThreads} 個背景執行緒");
            Console.WriteLine($"此次共花費 {stopwatch.Elapsed} 時間");
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void Case5()
        {
            int CurrentAvailableThreads = GetAvailableThreads();
            List<Task> allTask = new List<Task>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < MaxLoop; i++)
            {
                allTask.Add(Task.Run(async () =>
                {
                    int tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                    await Task.Delay(SimulateTaskTime);
                    tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    countdownEvent.Signal();
                }));
            }

            countdownEvent.Wait();
            stopwatch.Stop();

            PrintThreadPoolInformation();
            Console.WriteLine($"此次使用到 {MaxThreads} 個背景執行緒");
            Console.WriteLine($"此次共花費 {stopwatch.Elapsed} 時間");
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        public static int GetAvailableThreads()
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            return workerThreads;
        }
        public static void PrintThreadPoolInformation()
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"執行緒集區中的背景工作執行緒最大數目 : {workerThreads} / 執行緒集區中的非同步 I/O 執行緒最大數目 : { completionPortThreads}");
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"需要建立的背景工作執行緒最小數目 : {workerThreads} / 需要建立的非同步 I/O 執行緒最小數目 : { completionPortThreads}");
            Console.WriteLine($"");
        }
    }
}
