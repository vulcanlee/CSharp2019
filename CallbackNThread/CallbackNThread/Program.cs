using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CallbackNThread
{
    class MyAsyncClass
    {
        public EventHandler OnCompletion;
        public void DoRun()
        {
            Console.WriteLine($"執行 DoRun 前的執行緒ID={Thread.CurrentThread.ManagedThreadId}");
            ThreadPool.QueueUserWorkItem(x =>
            {
                Console.WriteLine($"進入到非同步執行緒內的執行緒ID={Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("模擬需要3秒鐘的非同步工作");
                Thread.Sleep(3000);

                Console.WriteLine($"準備要呼叫 callback，現在的執行緒ID={Thread.CurrentThread.ManagedThreadId}");
                OnCompletion?.Invoke(this, EventArgs.Empty);
            });
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Main方法內的執行緒ID={Thread.CurrentThread.ManagedThreadId}");
            MyAsyncClass myAsyncObject = new MyAsyncClass();
            myAsyncObject.OnCompletion += (s, e) =>
            {
                Console.WriteLine($"在 Main 方法內的委派 callback ，現在的執行緒ID={Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Console.WriteLine("callback 執行結束了");
            };
            Console.WriteLine($"Main方法內的開始呼叫 DoRun 方法的執行緒ID={Thread.CurrentThread.ManagedThreadId}");
            myAsyncObject.DoRun();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
