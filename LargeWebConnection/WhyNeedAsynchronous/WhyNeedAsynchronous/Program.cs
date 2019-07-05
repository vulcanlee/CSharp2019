using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WhyNeedAsynchronous
{
    class Program
    {
        static int MaxTasks = 100;
        // 此 URL 是要連上 本地端主機 上的 Web API 測試端點
        static string APIEndPoint = "https://localhost:44382/api/values/AddSync/8/9/1200";
        static string APIHost = "https://localhost:44382/";

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            //ConnectWebAPI();
            //ConnectWebAPIAsync().Wait();
            ConnectAsyncWebAPIAsync().Wait();
            sw.Stop();

            Console.WriteLine($"花費時間: {sw.ElapsedMilliseconds} ms");
        }
        public static void ConnectWebAPI()
        {
            APIEndPoint = "https://localhost:44382/api/values/AddSync/8/9/1200";
            for (int i = 1; i <= MaxTasks; i++)
            {
                int idx = i;
                DateTime begin = DateTime.Now;
                Console.WriteLine($"Task{idx} Begin");
                HttpClient client = new HttpClient();
                string result = client.GetStringAsync(
                    APIEndPoint).Result;
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms)");
            }
        }

        public static async Task ConnectWebAPIAsync()
        {
            APIEndPoint = "https://localhost:44382/api/values/AddSync/8/9/1200";
            APIEndPoint = "https://lobworkshop.azurewebsites.net/api/RemoteSource/AddSync/8/9/1200";
            List<Task> tasks = new List<Task>();

            for (int i = 1; i <= MaxTasks; i++)
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

        public static async Task ConnectAsyncWebAPIAsync()
        {
            APIEndPoint = "https://localhost:44382/api/values/AddAsync/8/9/1200";
            APIEndPoint = "https://lobworkshop.azurewebsites.net/api/RemoteSource/AddAsync/8/9/1200";
            List<Task> tasks = new List<Task>();

            for (int i = 1; i <= MaxTasks; i++)
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
                    Console.WriteLine($"Task{idx} Completed ({total.TotalMilliseconds} ms) ");
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
