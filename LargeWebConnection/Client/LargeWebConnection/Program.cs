using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace LargeWebConnection
{
    class Program
    {
        static IServiceCollection serviceCollection;
        static IServiceProvider serviceProvider1;
        static int MaxTasks = 10;
        static string APIServiceName = "lobworkshop";

        // 此 URL 是要連上 Azure 上的 Web API 測試端點
        static string APIEndPoint = "https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/1200";
        static string APIHost = "http://lobworkshop.azurewebsites.net/";

        // 此 URL 是要連上 本地端主機 上的 Web API 測試端點
        //static string APIEndPoint = "https://localhost:5001/api/values/AddSync/8/9/5000";
        //static string APIHost = "https://localhost:5001/";

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            //serviceCollection.AddTransient<IMessage, ConsoleMessage>();
            serviceCollection.AddHttpClient(APIServiceName, client =>
            {
                client.BaseAddress = new Uri(APIHost);
            });
            serviceProvider1 = serviceCollection.BuildServiceProvider();

            //Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)3;
            //ThreadPool.SetMinThreads(50, 50);
            //ThreadPool.SetMinThreads(16, 16);
            new Thread(MonitorThreadPool.BeginMonitor).Start();
            Thread.Sleep(200);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            ConnectWebAPIAsync().Wait();
            sw.Stop();

            Console.WriteLine($"花費時間: {sw.ElapsedMilliseconds} ms");
        }
        public static async Task ConnectWebAPIAsync()
        {
            var factory = serviceProvider1.GetService<IHttpClientFactory>();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(async () =>
                {
                    DateTime begin = DateTime.Now;
                    Console.WriteLine($"Task{idx} Begin");
                    HttpClient client = factory.CreateClient("lobworkshop");
                    string result = await client.GetStringAsync(
                        APIEndPoint);
                    DateTime complete = DateTime.Now;
                    TimeSpan total = complete - begin;
                    Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms) --> {result}");
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
    public class MonitorThreadPool
    {
        public static int LastBusyWorkerThreads;

        public static int LastBusyCompletionPortThreads;

        public static ThreadPoolInformation threadPoolInformation;
        public static ThreadPoolInformation threadPoolCurrentInformation;

        public static void BeginMonitor()
        {
            threadPoolInformation = new ThreadPoolInformation();
            GetThreadPoolInformation(threadPoolInformation);
            //LastBusyWorkerThreads = threadPoolInformation.BusyWorkerThreads;
            //LastBusyCompletionPortThreads = threadPoolInformation.BusyCompletionPortThreads;

            ShowAllThreadPoolInformation(threadPoolInformation);
            threadPoolCurrentInformation = threadPoolInformation.Clone();

            while (true)
            {
                ShowCurrentThreadUsage(threadPoolInformation, threadPoolCurrentInformation);
                Thread.Sleep(0);
            }
        }

        public static void ShowCurrentThreadUsage(ThreadPoolInformation threadPoolInformation, ThreadPoolInformation threadPoolCurrentInformation)
        {
            int workerThreads;
            int completionPortThreads;
            // 傳回之執行緒集區的現在還可以容許使用多少的執行緒數量大小
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            threadPoolCurrentInformation.AvailableWorkerThreads = workerThreads;
            threadPoolCurrentInformation.AvailableCompletionPortThreads = completionPortThreads;
            threadPoolCurrentInformation.BusyWorkerThreads = threadPoolInformation.AvailableWorkerThreads - workerThreads;
            threadPoolCurrentInformation.BusyCompletionPortThreads = threadPoolInformation.AvailableCompletionPortThreads - completionPortThreads;
            ShowAvailableThreadPoolInformation(threadPoolCurrentInformation);
        }

        // 取得執行緒集區內的相關設定參數
        public static void GetThreadPoolInformation(ThreadPoolInformation threadPoolInformation)
        {
            int workerThreads;
            int completionPortThreads;

            // 傳回之執行緒集區的現在還可以容許使用多少的執行緒數量大小
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            threadPoolInformation.AvailableWorkerThreads = workerThreads;
            threadPoolInformation.AvailableCompletionPortThreads = completionPortThreads;

            // 擷取可並行使用之執行緒集區的要求數目。 超過該數目的所有要求會繼續佇列，直到可以使用執行緒集區執行緒為止
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            threadPoolInformation.MaxWorkerThreads = workerThreads;
            threadPoolInformation.MaxCompletionPortThreads = completionPortThreads;

            // 在切換至管理執行緒建立和解構的演算法之前，擷取執行緒集區隨著提出新要求，視需要建立的執行緒最小數目。
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            threadPoolInformation.MinWorkerThreads = workerThreads;
            threadPoolInformation.MinCompletionPortThreads = completionPortThreads;

            // 如果目前電腦包含多個處理器群組，則這個屬性會傳回可供 Common Language Runtime (CLR) 使用的邏輯處理器數目
            threadPoolInformation.ProcessorCount = System.Environment.ProcessorCount;
        }

        // 顯示執行緒集區內的所有運作參數
        public static void ShowAllThreadPoolInformation(ThreadPoolInformation threadPoolInformation)
        {
            Console.WriteLine($"執行緒集區的相關設定資訊");
            Console.WriteLine($"邏輯處理器數目 : {threadPoolInformation.ProcessorCount} ");
            Console.WriteLine($"WorkItem Thread :" +
                $" (Busy:{threadPoolInformation.BusyWorkerThreads}, Free:{threadPoolInformation.AvailableWorkerThreads}, Min:{threadPoolInformation.MinWorkerThreads}, Max:{threadPoolInformation.MaxWorkerThreads})");
            Console.WriteLine($"IOPC Thread :" +
                $" (Busy:{threadPoolInformation.BusyCompletionPortThreads}, Free:{threadPoolInformation.AvailableCompletionPortThreads}, Min:{threadPoolInformation.MinCompletionPortThreads}, Max:{threadPoolInformation.MaxCompletionPortThreads})");
            Console.WriteLine($"");
        }

        // 顯示執行緒集區內上有多少空間，可以用來增加新執行緒的數量
        public static void ShowAvailableThreadPoolInformation(ThreadPoolInformation threadPoolInformation)
        {
            if (LastBusyWorkerThreads != threadPoolInformation.BusyWorkerThreads ||
                LastBusyCompletionPortThreads != threadPoolInformation.BusyCompletionPortThreads)
            {
                LastBusyWorkerThreads = threadPoolInformation.BusyWorkerThreads;
                LastBusyCompletionPortThreads = threadPoolInformation.BusyCompletionPortThreads;
                //Console.WriteLine($"   WorkItem Thread :" +
                //    $" (Busy:{threadPoolInformation.BusyWorkerThreads}, Free:{threadPoolInformation.AvailableWorkerThreads}, Min:{threadPoolInformation.MinWorkerThreads}, Max:{threadPoolInformation.MaxWorkerThreads})");
                //Console.WriteLine($"   IOPC Thread :" +
                //    $" (Busy:{threadPoolInformation.BusyCompletionPortThreads}, Free:{threadPoolInformation.AvailableCompletionPortThreads}, Min:{threadPoolInformation.MinCompletionPortThreads}, Max:{threadPoolInformation.MaxCompletionPortThreads})");
            }
        }

    }
    // 儲存執行緒集區相關運作參數的類別
    public class ThreadPoolInformation : ICloneable
    {
        public int ProcessorCount { get; set; }
        public int AvailableWorkerThreads { get; set; }
        public int AvailableCompletionPortThreads { get; set; }
        public int BusyWorkerThreads { get; set; }
        public int BusyCompletionPortThreads { get; set; }
        public int MaxWorkerThreads { get; set; }
        public int MaxCompletionPortThreads { get; set; }
        public int MinWorkerThreads { get; set; }
        public int MinCompletionPortThreads { get; set; }

        public void ComputeBusyThreads(ThreadPoolInformation threadPoolInformation)
        {
            this.BusyWorkerThreads = threadPoolInformation.AvailableWorkerThreads - this.AvailableWorkerThreads;
            this.BusyCompletionPortThreads = threadPoolInformation.BusyCompletionPortThreads - this.BusyCompletionPortThreads;
        }
        public ThreadPoolInformation Clone()
        {
            ICloneable cloneable = this;
            return cloneable.Clone() as ThreadPoolInformation;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
