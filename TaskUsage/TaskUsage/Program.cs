using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            //使用同步方式呼叫();
            //建立工作_用Task類別來建立一個非同步工作與傳遞參數用法();
            //透過Task_Factory工廠方法建立一個非同步工作與傳遞參數的用法();
            //透過Task_Run方法建立一個非同步工作與傳遞參數的用法();
            //當使用工作執行個體_如何等待該執行完成();
            //如何取得非同步工作執行完成之後的執行結果回傳值();
            //當使用工作執行個體_如何取消正在執行的非同步工作();
            當工作的委派方法拋出例外異常_會有甚麼問題產生();
        }

        private static void 當工作的委派方法拋出例外異常_會有甚麼問題產生()
        {
            Task task1 = Task.Factory.StartNew(MyMethod13);
            Task task2 = Task.Factory.StartNew(MyMethod23, "-");

            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine($"task1 的狀態值為 IsCompleted={task1.IsCompleted}," +
                $"IsCanceled={task1.IsCanceled}, IsFaulted={task1.IsFaulted}");
            Console.WriteLine($"task2 的執行結果為  IsCompleted={task2.IsCompleted}," +
                $"IsCanceled={task2.IsCanceled}, IsFaulted={task2.IsFaulted}");
        }

        private static void 當使用工作執行個體_如何取消正在執行的非同步工作()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            Task task1 = Task.Run(()=> MyMethod12(token), token);
            Task task2 = Task.Run(() => MyMethod22("-", token), token);

            Thread.Sleep(1000);

            cts.Cancel();

            Thread.Sleep(100);

            Console.WriteLine();
            Console.WriteLine($"task1 的狀態值為 IsCompleted={task1.IsCompleted}," +
                $"IsCanceled={task1.IsCanceled}, IsFaulted={task1.IsFaulted}");
            Console.WriteLine($"task2 的執行結果為  IsCompleted={task2.IsCompleted}," +
                $"IsCanceled={task2.IsCanceled}, IsFaulted={task2.IsFaulted}");
        }

        private static void 如何取得非同步工作執行完成之後的執行結果回傳值()
        {
            Task<string> task1 = Task.Run<string>(MyMethod11);
            Task<int> task2 = Task.Run<int>(() => MyMethod21("-"));
            // 也可以使用底下寫法，讓編譯器自動推斷回傳型別
            //var task1 = Task.Run(MyMethod11);
            //var task2 = Task.Run(() => MyMethod21("-"));

            Task.WhenAll(task1, task2).Wait();

            Console.WriteLine();
            Console.WriteLine($"task1 的執行結果為 {task1.Result}");
            Console.WriteLine($"task2 的執行結果為 {task2.Result}");
        }

        private static void 當使用工作執行個體_如何等待該執行完成()
        {
            Task task1 = Task.Run(MyMethod1);
            Task task2 = Task.Run(() => MyMethod2("-"));

            Task.WhenAll(task1, task2).Wait();
        }

        private static void 透過Task_Run方法建立一個非同步工作與傳遞參數的用法()
        {
            Task task1 = Task.Run(MyMethod1);
            Task task2 = Task.Run(() => MyMethod2("-"));

            Thread.Sleep(2000);
        }

        private static void 透過Task_Factory工廠方法建立一個非同步工作與傳遞參數的用法()
        {
            Task task1 = Task.Factory.StartNew(MyMethod1);
            Task task2 = Task.Factory.StartNew(MyMethod2, "-");

            Thread.Sleep(2000);
        }

        private static void 建立工作_用Task類別來建立一個非同步工作與傳遞參數用法()
        {
            Task task1 = new Task(MyMethod1);
            Task task2 = new Task(MyMethod2, "-");
            task1.Start();
            task2.Start();

            Thread.Sleep(2000);
        }

        static void 使用同步方式呼叫()
        {
            MyMethod1();
            MyMethod2("-");
        }
        static void MyMethod1()
        {
            for (int i = 0; i < 800; i++)
            {
                Console.Write("*");
            }
        }
        static void MyMethod2(object message)
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write(message.ToString());
            }
        }
        static string MyMethod11()
        {
            for (int i = 0; i < 800; i++)
            {
                Console.Write("*");
            }
            return "loop is 800";
        }
        static int MyMethod21(object message)
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write(message.ToString());
            }
            return 500;
        }
        static void MyMethod12(CancellationToken token)
        {
            while (true)
            {
                Console.Write("*");
                Thread.Sleep(30);
                // 這裡的做法將會是正常結束此非同步工作
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        static void MyMethod22(object message, CancellationToken token)
        {
            while (true)
            {
                Console.Write(message.ToString());
                Thread.Sleep(30);
                // 這裡的做法將會是立即結束此非同步工作
                token.ThrowIfCancellationRequested();
            }
        }
        static void MyMethod13()
        {
            for (int i = 0; i < 800; i++)
            {
                if(i==10)
                {
                    throw new Exception("喔喔，有例外異常拋出了");
                }
                Console.Write("*");
            }
        }
        static void MyMethod23(object message)
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write(message.ToString());
            }
        }
    }
}
