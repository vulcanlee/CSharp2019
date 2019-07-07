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
            string result = await MyMethodAsync();
            Console.WriteLine($"2 ({Thread.CurrentThread.ManagedThreadId})");

            Console.WriteLine($"呼叫非同步方法結果 {result}");

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
        static async Task<string> MyMethodAsync()
        {
            Console.WriteLine($"進入到非同步方法");
            Console.WriteLine($"3 ({Thread.CurrentThread.ManagedThreadId})");

            //Thread.Sleep(3000);
            await Task.Delay(3000);

            Console.WriteLine($"準備離開到非同步方法");
            Console.WriteLine($"4 ({Thread.CurrentThread.ManagedThreadId})");

            return "My Result";
        }
    }
}
