using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ForNETCore22
{
    class Program
    {
        static IServiceCollection serviceCollection;
        static IServiceProvider serviceProvider1;
        static int RemoteSleepMS = 1200;
        static int MaxTasks = 100;
        static string APIServiceName = "lobworkshop";
        static HttpClient StaticHttpClient = new HttpClient();

        // 此 URL 是要連上 Azure 上的 Web API 測試端點
        static string APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddASync/8/9/{RemoteSleepMS}";
        static string APIHost = "http://lobworkshop.azurewebsites.net/";
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
            #region HttpClient Factory
            // 用戶端使用 HttpCliet 工廠且同步等待結果，呼叫遠端同步 Web API
            //UsingHttpClientFactorySyncConnectSyncWebAPIAsync();

            // 用戶端使用 HttpCliet 工廠且非同步等待結果，呼叫遠端同步 Web API
            //UsingHttpClientFactoryAsyncConnectSyncWebAPIAsync().Wait();

            // 用戶端使用 HttpCliet 工廠且非同步等待結果，呼叫遠端非同步 Web API
            //UsingHttpClientFactoryAsyncConnectAsyncWebAPIAsync().Wait();
            #endregion

            #region HttpClient Static Singleton
            // 用戶端使用 HttpCliet Static Singleton且同步等待結果，呼叫遠端同步 Web API
            //UsingHttpClientStaticSingletonSyncConnectSyncWebAPIAsync();

            // 用戶端使用 HttpCliet Static Singleton且非同步等待結果，呼叫遠端同步 Web API
            //UsingHttpClientStaticSingletonAsyncConnectSyncWebAPIAsync().Wait();

            // 用戶端使用 HttpCliet Static Singleton且非同步等待結果，呼叫遠端非同步 Web API
            //UsingHttpClientStaticSingletonAsyncConnectAsyncWebAPIAsync().Wait();
            #endregion

            #region New HttpClient
            // 用戶端使用 New HttpCliet 且同步等待結果，呼叫遠端同步 Web API
            //UsingNewHttpClientSyncConnectSyncWebAPIAsync();

            // 用戶端使用 New HttpCliet 且非同步等待結果，呼叫遠端同步 Web API
            //UsingNewHttpClientAsyncConnectSyncWebAPIAsync().Wait();

            // 用戶端使用 New HttpCliet 且非同步等待結果，呼叫遠端非同步 Web API
            UsingNewHttpClientAsyncConnectAsyncWebAPIAsync().Wait();
            #endregion

            sw.Stop();

            Console.WriteLine($"花費時間: {sw.ElapsedMilliseconds} ms");
        }

        #region HttpClient Factory
        public static void UsingHttpClientFactorySyncConnectSyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/{RemoteSleepMS}";
            var factory = serviceProvider1.GetService<IHttpClientFactory>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = factory.CreateClient("lobworkshop");
                UsingHttpClientFactorySyncConnectSyncWebAPIRequestAsync(client, idx);
            }
        }
        private static void UsingHttpClientFactorySyncConnectSyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            DateTime begin = DateTime.Now;
            Console.WriteLine($"Task{idx} Begin");
            string result = client.GetStringAsync(
                APIEndPoint).Result;
            DateTime complete = DateTime.Now;
            TimeSpan total = complete - begin;
            Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
        }

        public static async Task UsingHttpClientFactoryAsyncConnectSyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/{RemoteSleepMS}";
            var factory = serviceProvider1.GetService<IHttpClientFactory>();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = factory.CreateClient("lobworkshop");
                tasks.Add(UsingHttpClientFactoryAsyncConnectSyncWebAPIRequestAsync(client, idx));
            }

            await Task.WhenAll(tasks);
        }
        private static Task UsingHttpClientFactoryAsyncConnectSyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            return Task.Run(async () =>
            {
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                string result = await client.GetStringAsync(
                    APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            });
        }

        public static async Task UsingHttpClientFactoryAsyncConnectAsyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddAsync/8/9/{RemoteSleepMS}";
            var factory = serviceProvider1.GetService<IHttpClientFactory>();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = factory.CreateClient("lobworkshop");
                tasks.Add(UsingHttpClientFactoryAsyncConnectAsyncWebAPIRequestAsync(client, idx));
            }

            await Task.WhenAll(tasks);
        }
        private static Task UsingHttpClientFactoryAsyncConnectAsyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            return Task.Run(async () =>
            {
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                string result = await client.GetStringAsync(
                    APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            });
        }
        #endregion

        #region HttpClient Static Singleton
        public static void UsingHttpClientStaticSingletonSyncConnectSyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/{RemoteSleepMS}";
            var factory = serviceProvider1.GetService<IHttpClientFactory>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = StaticHttpClient;
                UsingHttpClientStaticSingletonSyncConnectSyncWebAPIRequestAsync(client, idx);
            }
        }
        private static void UsingHttpClientStaticSingletonSyncConnectSyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            DateTime begin = DateTime.Now;
            Console.WriteLine($"Task{idx} Begin");
            string result = client.GetStringAsync(
                APIEndPoint).Result;
            DateTime complete = DateTime.Now;
            TimeSpan total = complete - begin;
            Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
        }

        public static async Task UsingHttpClientStaticSingletonAsyncConnectSyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/{RemoteSleepMS}";
            var factory = serviceProvider1.GetService<IHttpClientFactory>();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = StaticHttpClient;
                tasks.Add(UsingHttpClientStaticSingletonAsyncConnectSyncWebAPIRequestAsync(client, idx));
            }

            await Task.WhenAll(tasks);
        }
        private static Task UsingHttpClientStaticSingletonAsyncConnectSyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            return Task.Run(async () =>
            {
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                string result = await client.GetStringAsync(
                    APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            });
        }

        public static async Task UsingHttpClientStaticSingletonAsyncConnectAsyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddAsync/8/9/{RemoteSleepMS}";
            var factory = serviceProvider1.GetService<IHttpClientFactory>();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = StaticHttpClient;
                tasks.Add(UsingHttpClientStaticSingletonAsyncConnectAsyncWebAPIRequestAsync(client, idx));
            }

            await Task.WhenAll(tasks);
        }
        private static Task UsingHttpClientStaticSingletonAsyncConnectAsyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            return Task.Run(async () =>
            {
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                string result = await client.GetStringAsync(
                    APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            });
        }
        #endregion

        #region New HttpClient
        public static void UsingNewHttpClientSyncConnectSyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/{RemoteSleepMS}";

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = new HttpClient();
                UsingNewHttpClientSyncConnectSyncWebAPIRequestAsync(client, idx);
            }
        }
        private static void UsingNewHttpClientSyncConnectSyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            DateTime begin = DateTime.Now;
            Console.WriteLine($"Task{idx} Begin");
            string result = client.GetStringAsync(
                APIEndPoint).Result;
            DateTime complete = DateTime.Now;
            TimeSpan total = complete - begin;
            Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
        }

        public static async Task UsingNewHttpClientAsyncConnectSyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/{RemoteSleepMS}";
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = new HttpClient();
                tasks.Add(UsingNewHttpClientAsyncConnectSyncWebAPIRequestAsync(client, idx));
            }

            await Task.WhenAll(tasks);
        }
        private static Task UsingNewHttpClientAsyncConnectSyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            return Task.Run(async () =>
            {
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                string result = await client.GetStringAsync(
                    APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            });
        }

        public static async Task UsingNewHttpClientAsyncConnectAsyncWebAPIAsync()
        {
            APIEndPoint = $"https://lobworkshop.azurewebsites.net/api/RemoteSource/AddAsync/8/9/{RemoteSleepMS}";
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                HttpClient client = new HttpClient();
                tasks.Add(UsingNewHttpClientAsyncConnectAsyncWebAPIRequestAsync(client, idx));
            }

            await Task.WhenAll(tasks);
        }
        private static Task UsingNewHttpClientAsyncConnectAsyncWebAPIRequestAsync(HttpClient client, int idx)
        {
            return Task.Run(async () =>
            {
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                string result = await client.GetStringAsync(
                    APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            });
        }
        #endregion

        public static async Task UsingCreateNewHttpClientAsync()
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
                    HttpClient client = new HttpClient();
                    string result = await client.GetStringAsync(
                        APIEndPoint);
                    DateTime complete = DateTime.Now;
                    TimeSpan total = complete - begin;
                    Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
                }));
            }

            await Task.WhenAll(tasks);
        }
        public static async Task UsingStaticHttpClientAsync()
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
                    HttpClient client = StaticHttpClient;
                    string result = await client.GetStringAsync(
                        APIEndPoint);
                    DateTime complete = DateTime.Now;
                    TimeSpan total = complete - begin;
                    Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
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
