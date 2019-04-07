using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitWait
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"主執行緒開始執行，" +
                $"ID={Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("呼叫 DoWait 方法");
            DoWait();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                Console.WriteLine($"主執行緒正在處理其他事情({(i + 1) * 500}ms) " +
                $"(執行緒:{Thread.CurrentThread.ManagedThreadId})");
            }
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
        static void DoWait()
        {
            Before();
            MyMethodAsync().Wait();
            After();
        }
        private static void Before()
        {
            Console.WriteLine($"  [Before] 呼叫 MyMethodAsync().Wait(); 前的" +
                $"執行緒:{Thread.CurrentThread.ManagedThreadId}");
        }
        private static void After()
        {
            Console.WriteLine($"  [After] 呼叫 MyMethodAsync().Wait(); 後的" +
                $"執行緒:{Thread.CurrentThread.ManagedThreadId}");
        }
        static Task MyMethodAsync()
        {
            Console.WriteLine($"進入到 MyMethodAsync 前，" +
                $"所使用的執行緒 :{Thread.CurrentThread.ManagedThreadId}");
            return Task.Run(() =>
            {
                Console.WriteLine($"MyMethodAsync 開始進行非同步工作，" +
                    $"所使用的執行緒 :{Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("需要花費 7 秒鐘");
                Thread.Sleep(7000);
            });
        }
    }
}
