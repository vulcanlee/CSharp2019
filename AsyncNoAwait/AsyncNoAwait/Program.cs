using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncNoAwait
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"1 ({Thread.CurrentThread.ManagedThreadId})");
            var task= MyMethodAsync();
            Console.WriteLine($"2 ({Thread.CurrentThread.ManagedThreadId})");

            string result = await task;
            Console.WriteLine($"呼叫非同步方法結果 {result}");

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
        static async Task<string> MyMethodAsync()
        {
            Console.WriteLine($"進入到非同步方法");
            Console.WriteLine($"3 ({Thread.CurrentThread.ManagedThreadId})");

            // 測試條件1: 在 async 方法內，沒有使用到任何的 await 運算子
            Thread.Sleep(3000);
            // 測試條件2: 在 async 方法內，使用到 await 運算子
            //await Task.Delay(3000);

            Console.WriteLine($"準備離開到非同步方法");
            Console.WriteLine($"4 ({Thread.CurrentThread.ManagedThreadId})");

            return "My Result";
        }
    }
}
