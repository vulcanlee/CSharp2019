using System;
using System.Threading;

namespace WhatsThreadPool
{
    class Program
    {
        public static int AvailableWorkerThreads = 0;
        public static int MaxRunningWorkThreads = 0;
        static void Main(string[] args)
        {
            #region 測試執行緒集區的各種實驗性參數
            // 測試參數1 : 同時要求進行得並行工作
            int testLoop = 12;
            // 測試參數2 : ThreadPool 的最大可以容許的執行緒數量。
            //ThreadPool.SetMaxThreads(10, 10);
            // 測試參數3 : ThreadPool 預設建立的執行緒數量。
            //ThreadPool.SetMinThreads(10, 10);
            #endregion

            ThreadPoolInformation threadPoolInformation = new ThreadPoolInformation();
            GetThreadPoolInformation(threadPoolInformation);
            AvailableWorkerThreads = threadPoolInformation.AvailableWorkerThreads;
            ShowAllThreadPoolInformation(threadPoolInformation);

            // 代表當計數到達零時，將會收到訊號，否則，就會繼續等待
            // 這將會用來等候所有的執行緒執行完畢的一個同步化技術
            CountdownEvent done = new CountdownEvent(testLoop);
            for (int i = 0; i < testLoop; i++)
            {
                int idx = i;
                // 從執行緒集區內取得一個執行緒來執行工作
                // 沒有可用執行緒，執行緒集區將會自動建立一個
                // 若執行緒集區無法再建立新的執行緒，將會等到有執行緒被回收之後，才能繼續執行
                ThreadPool.QueueUserWorkItem(x =>
                {
                    int currentThreadID = Thread.CurrentThread.ManagedThreadId;
                    // 顯示該執行已經開始執行了(已經從執行緒集區內取得到新的執行緒)
                    Console.WriteLine($"執行緒開始[{idx}]: ID={currentThreadID}, time={DateTime.Now.TimeOfDay}");

                    ThreadPoolInformation newThreadPoolInformation = new ThreadPoolInformation();
                    GetThreadPoolInformation(threadPoolInformation);
                    ShowAvailableThreadPoolInformation(threadPoolInformation);
                    // 模擬使用同步方式來等候一個非同步的作業完成
                    Thread.Sleep(1000 * testLoop);
                    // 顯示該執行緒已經完成執行了
                    Console.WriteLine($"執行緒結束[{idx}]: ID={currentThreadID}, time={DateTime.Now.TimeOfDay}");
                    done.Signal();
                });
            }

            done.Wait();
        }

        // 取得執行緒集區內的相關設定參數
        static void GetThreadPoolInformation(ThreadPoolInformation threadPoolInformation)
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
        }
        // 顯示執行緒集區內的所有運作參數
        static void ShowAllThreadPoolInformation(ThreadPoolInformation threadPoolInformation)
        {
            Console.WriteLine($"   可用背景工作執行緒的數目 : {threadPoolInformation.AvailableWorkerThreads} / " +
                $"可用非同步 I/O 執行緒的數目 : { threadPoolInformation.AvailableCompletionPortThreads}");

            Console.WriteLine($"   執行緒集區中的背景工作執行緒最大數目 : {threadPoolInformation.MaxWorkerThreads} " +
                $"/ 執行緒集區中的非同步 I/O 執行緒最大數目 : { threadPoolInformation.MaxCompletionPortThreads}");

            Console.WriteLine($"   需要建立的背景工作執行緒最小數目 : {threadPoolInformation.MinWorkerThreads}" +
                $" / 需要建立的非同步 I/O 執行緒最小數目 : { threadPoolInformation.MinCompletionPortThreads}");
            Console.WriteLine($"");
        }
        // 顯示執行緒集區內上有多少空間，可以用來增加新執行緒的數量
        static void ShowAvailableThreadPoolInformation(ThreadPoolInformation threadPoolInformation)
        {
            Console.WriteLine($"   可用背景工作執行緒的數目 : {threadPoolInformation.AvailableWorkerThreads} / " +
                $"可用非同步 I/O 執行緒的數目 : { threadPoolInformation.AvailableCompletionPortThreads}");
            Console.WriteLine($"");
        }
    }
    // 儲存執行緒集區相關運作參數的類別
    public class ThreadPoolInformation
    {
        public int AvailableWorkerThreads { get; set; }
        public int AvailableCompletionPortThreads { get; set; }
        public int MaxWorkerThreads { get; set; }
        public int MaxCompletionPortThreads { get; set; }
        public int MinWorkerThreads { get; set; }
        public int MinCompletionPortThreads { get; set; }
    }
}
