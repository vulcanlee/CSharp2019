using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            //使用同步方式呼叫();
            //建立執行緒_使用Thread類別來建立一個執行緒物件與傳遞參數用法();
            //透過執行緒集區ThreadPool取得一個執行緒與傳遞參數用法();
            //當使用執行緒類別建立執行個體_如何等待該執行緒執行完成();
            //透過執行緒集區取得的執行緒_如何得知該執行緒已經結束執行了();
            //如何取得執行緒執行完成之後的執行結果回傳值();
            //當使用執行緒類別建立執行個體_如何取消正在執行的執行緒();
            當執行緒內的程式碼拋出例外異常_會有甚麼問題產生();
        }

        private static void 當執行緒內的程式碼拋出例外異常_會有甚麼問題產生()
        {
            ThreadPool.QueueUserWorkItem(MyMethod27, "@");
            Thread.Sleep(5000);
            Console.WriteLine("因為執行緒拋出例外異常，這行文字永遠無法顯示在螢幕上");
        }

        private static void 當使用執行緒類別建立執行個體_如何取消正在執行的執行緒()
        {
            Thread thread1 = new Thread(MyMethod25);
            Thread thread2 = new Thread(MyMethod26);
            thread1.Start("*");
            thread2.Start("-");

            Thread.Sleep(1000);
            thread2.Abort();
            Thread.Sleep(3000);
            thread1.Abort();
        }

        static int Result1, Result2;
        private static void 如何取得執行緒執行完成之後的執行結果回傳值()
        {
            ThreadPool.QueueUserWorkItem(MyMethod23, AllWaitHandles[0]);
            ThreadPool.QueueUserWorkItem(MyMethod24, AllWaitHandles[1]);
            WaitHandle.WaitAll(AllWaitHandles);
            Console.WriteLine();
            Console.WriteLine($"MyMethod23 的執行結果為 {Result1}");
            Console.WriteLine($"MyMethod24 的執行結果為 {Result2}");
        }

        static WaitHandle[] AllWaitHandles = new WaitHandle[]
        {
            new AutoResetEvent(false),new AutoResetEvent(false)
        };
        private static void 透過執行緒集區取得的執行緒_如何得知該執行緒已經結束執行了()
        {
            ThreadPool.QueueUserWorkItem(MyMethod21, AllWaitHandles[0]);
            ThreadPool.QueueUserWorkItem(MyMethod22, AllWaitHandles[1]);
            WaitHandle.WaitAll(AllWaitHandles);
        }

        private static void 當使用執行緒類別建立執行個體_如何等待該執行緒執行完成()
        {
            Thread thread1 = new Thread(MyMethod1);
            Thread thread2 = new Thread(MyMethod2);
            thread1.Start();
            thread2.Start("-");

            thread1.Join();
            thread2.Join();
        }

        private static void 透過執行緒集區ThreadPool取得一個執行緒與傳遞參數用法()
        {
            // 要從執行緒集區內取得一個執行緒，需要提供一個
            //   public delegate void WaitCallback(object state);
            // 委派函式簽章，也就是該委派方法必須要有一個參數
            // 因此，在這裡透過 Lambda 匿名委派方法來呼叫 MyMethod1() 方法
            ThreadPool.QueueUserWorkItem(_ => MyMethod1());
            // 使用執行緒集區取得的執行緒，若要傳遞引數，請接著委派方法之後傳入
            ThreadPool.QueueUserWorkItem(MyMethod2, "-");
            Thread.Sleep(2000);
        }

        private static void 建立執行緒_使用Thread類別來建立一個執行緒物件與傳遞參數用法()
        {
            Thread thread1 = new Thread(MyMethod1);
            Thread thread2 = new Thread(MyMethod2);
            thread1.Start();
            thread2.Start("-");

            Thread.Sleep(2000);
        }

        static void 使用同步方式呼叫()
        {
            MyMethod1();
            MyMethod2("-");
        }
        static void MyMethod1()
        {
            for (int i = 0; i < 500; i++)
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
        static void MyMethod21(object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            for (int i = 0; i < 800; i++)
            {
                Console.Write("*");
            }
            are.Set();
        }
        static void MyMethod22(object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            for (int i = 0; i < 500; i++)
            {
                Console.Write("-");
            }
            are.Set();
        }
        static void MyMethod23(object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            for (int i = 0; i < 800; i++)
            {
                Console.Write("*");
            }
            Result1 = 800;
            are.Set();
        }
        static void MyMethod24(object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            for (int i = 0; i < 500; i++)
            {
                Console.Write("-");
            }
            Result2 = 500;
            are.Set();
        }
        static void MyMethod25(object message)
        {
            while (true)
            {
                Console.Write("*");
                Thread.Sleep(30);
            }
        }
        static void MyMethod26(object message)
        {
            while (true)
            {
                Console.Write("-");
                Thread.Sleep(30);
            }
        }
        static void MyMethod27(object message)
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.Write(message.ToString());
                if (i == 300)
                {
                    throw new Exception("喔喔，系統拋出例外異常");
                }
            }
        }
    }
}

