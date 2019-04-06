using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WaitAllWhenAll
{
    class Program
    {
        static List<int> sleepSeconds = new List<int>() { 3, 2, 5, 7, 2, 3, 4, 5, 5, 1, 7, 2, 4, 4, 5 };
        static void Main(string[] args)
        {
            WaitAll();
            WhenAll();


            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static async void WhenAll()
        {
            string host = "https://lobworkshop.azurewebsites.net";
            string path = "/api/RemoteSource/Add/15/43/@";
            string url = $"{host}{path}";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Task<string>> allTasks = new List<Task<string>>();
            foreach (var item in sleepSeconds)
            {
                var fooUrl = url.Replace("@", item.ToString());
                var task = new HttpClient().GetStringAsync(fooUrl);
                allTasks.Add(task);
            }
            await Task.WhenAll(allTasks.ToArray());
            sw.Stop();
            Console.WriteLine($"Wait total {sw.ElapsedMilliseconds} ms");
        }

        private static  void WaitAll()
        {
            string host = "https://lobworkshop.azurewebsites.net";
            string path = "/api/RemoteSource/Add/15/43/@";
            string url = $"{host}{path}";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Task<string>> allTasks = new List<Task<string>>();
            foreach (var item in sleepSeconds)
            {
                var fooUrl = url.Replace("@", item.ToString());
                var task = new HttpClient().GetStringAsync(fooUrl);
                allTasks.Add(task);
            }
            Task.WaitAll(allTasks.ToArray());
            sw.Stop();
            Console.WriteLine($"Wait total {sw.ElapsedMilliseconds} ms");
        }
    }
}
