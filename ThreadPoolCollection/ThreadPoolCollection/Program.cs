using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolCollection
{
    class Program
    {
        static Semaphore semaphore;
        static int MaxEmulateThreads = 11;
        static int MinThreadsOnThreadPool = 0;
        static ConcurrentDictionary<int, int> ThreadsOnThreadPool = new ConcurrentDictionary<int, int>();
        static CountdownEvent Countdown = new CountdownEvent(MaxEmulateThreads);
        static int ThreadPoolCollectionTime1 = 30;
        static int ThreadPoolCollectionTime2 = 30;
        static void Main(string[] args)
        {
            FindDefaultThreadOnThreadPool();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"第 1 次，產生 {MaxEmulateThreads} 執行緒請求");
            EmulateMoreThreads((MaxEmulateThreads - MinThreadsOnThreadPool + 1) * 1000, (MaxEmulateThreads - MinThreadsOnThreadPool + 1) * 1000);
            Thread.Sleep(2000);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"第 2次，產生 {MaxEmulateThreads} 執行緒請求");
            Countdown.Reset();
            EmulateMoreThreads(5 * 1000, 5 * 1000);
            Thread.Sleep(2000);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"第 3次，產生 {MaxEmulateThreads} 執行緒請求，休息 {ThreadPoolCollectionTime1} 秒");
            Thread.Sleep(ThreadPoolCollectionTime1 * 1000);
            Countdown.Reset();
            EmulateMoreThreads((MaxEmulateThreads - MinThreadsOnThreadPool + 1) * 1000, (MaxEmulateThreads - MinThreadsOnThreadPool + 1) * 1000);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"休息 {ThreadPoolCollectionTime2} 秒，等候執行緒集區清空新建立的執行緒");
            Thread.Sleep(ThreadPoolCollectionTime2 * 1000);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"第 4次，產生 {MaxEmulateThreads} 執行緒請求");
            ThreadsOnThreadPool.Clear();
            FindDefaultThreadOnThreadPool();
            Countdown.Reset(); EmulateMoreThreads((MaxEmulateThreads - MinThreadsOnThreadPool + 1) * 1000, (MaxEmulateThreads - MinThreadsOnThreadPool + 1) * 1000);
            Thread.Sleep(2000);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void EmulateMoreThreads(int defaultSleep, int NewSleep)
        {
            Console.WriteLine();
            for (int i = 1; i <= MaxEmulateThreads; i++)
            {
                int idx = i;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    string labelForThreadPool = "";
                    if (ThreadsOnThreadPool.ContainsKey(threadId))
                    {
                        labelForThreadPool = "**";
                        Console.WriteLine($"要求執行緒作業({idx}) {labelForThreadPool} Thread{threadId} 從執行緒集區取得該執行緒 {DateTime.Now.TimeOfDay}");
                        Thread.Sleep(defaultSleep);
                    }
                    else
                    {
                        labelForThreadPool = "";
                        Console.WriteLine($"要求執行緒作業({idx}) Thread{threadId} 是執行緒集區額外新建立的執行緒 {DateTime.Now.TimeOfDay}");
                        Thread.Sleep(NewSleep);
                    }
                    Countdown.Signal();
                    Console.WriteLine($"要求執行緒作業({idx}) {labelForThreadPool} Thread{threadId} 準備結束執行 {DateTime.Now.TimeOfDay}");
                });
            }
            Countdown.Wait();
        }

        private static void FindDefaultThreadOnThreadPool()
        {
            int workerThreads;
            int completionPortThreads;
            // 傳回之執行緒集區的現在還可以容許使用多少的執行緒數量大小
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            MinThreadsOnThreadPool = workerThreads;
            CountdownEvent countdown = new CountdownEvent(MinThreadsOnThreadPool);
            semaphore = new Semaphore(0, MinThreadsOnThreadPool);
            for (int i = 0; i < MinThreadsOnThreadPool; i++)
            {
                int idx = i;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    Thread.CurrentThread.Name = $"預設執行緒 {idx}";
                    Console.WriteLine($"Thread{threadId} 已經從執行緒集區取得該執行緒");
                    ThreadsOnThreadPool.TryAdd(threadId, threadId);
                    countdown.Signal();
                    semaphore.WaitOne();
                    Console.WriteLine($"Thread{threadId} 已經歸還給執行緒集區");
                });
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("等候取得所有執行緒都從執行緒集區取得...");
            countdown.Wait();

            Console.WriteLine("準備把取得的執行緒歸還給執行緒集區...");
            semaphore.Release(workerThreads);
            Thread.Sleep(2000);
        }
    }
}
