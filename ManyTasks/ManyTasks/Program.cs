using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyTasks
{
    class Program
    {
        public static int MaxThreads;
        static void Main(string[] args)
        {
            //ThreadPool.SetMinThreads(50, 50);
            int CurrentAvailableThreads;
            CurrentAvailableThreads = GetAvailableThreads();
            List<Task> allTask = new List<Task>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                allTask.Add(Task.Run(() =>
                {
                    int tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(2000);
                    tmpThreadCC = CurrentAvailableThreads - GetAvailableThreads();
                    if (MaxThreads < tmpThreadCC) MaxThreads = tmpThreadCC;
                }));
            }

            Task.WhenAll(allTask).Wait();
            stopwatch.Stop();
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
    }
}
