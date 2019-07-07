using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCancellationToken
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            CancellationToken token = cancellationToken.Token;
            var MyTask = Task.Run(() =>
            {
                Console.WriteLine("正在啟動非同步工作");
                Thread.Sleep(5000);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("非同步工作已經取消了");
                }
                Console.WriteLine("非同步工作結束了");
            }, token);

            //Thread.Sleep(1000);
            cancellationToken.Cancel();

            Console.WriteLine("按下任一按鍵，檢查工作狀態");
            Console.ReadKey();

            Console.WriteLine($"工作狀態 {MyTask.Status}");

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
