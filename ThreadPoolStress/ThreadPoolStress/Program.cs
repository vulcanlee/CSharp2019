using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vulcan.Courses;

namespace ThreadPoolStress
{
    class Program
    {
        #region 測試用的參數
        static int x最小執行緒數量 = 1;
        static int x最大執行緒數量 = 12;
        static int x迴圈數量 = 20;
        #endregion

        static int ActionKeyChar = -10;
        static int beginNumberOneASCII = 48;

        static void Main(string[] args)
        {
            #region 顯示預設執行緒集區的設定內容
            Console.WriteLine(AsyncCourse.GetThreadPoolHint(true));
            Console.WriteLine(AsyncCourse.GetThreadPoolInfo());
            Console.WriteLine();

            // 準備好要開始執行，按下任一按鍵
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
            #endregion

            #region 重新調整執行緒集區的設定
            //Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0b0000_1000;
            // 設定執行緒集區隨著提出新要求，視需要建立的執行緒最小數目
            ThreadPool.SetMinThreads(x最小執行緒數量, x最小執行緒數量);
            // 設定可並行使用之執行緒集區的要求數目。 超過該數目的所有要求會繼續佇列，直到可以使用執行緒集區執行緒為止
            ThreadPool.SetMaxThreads(x最大執行緒數量, x最大執行緒數量);

            Console.WriteLine(AsyncCourse.GetThreadPoolInfo());
            Console.WriteLine();
            #endregion

            #region  建立一個執行緒，監聽使用者輸入的按鍵
            new Thread(x =>
            {
                while (true)
                {
                    // 輸入 1~9 之間的按鍵，將會結束指定索引的執行緒執行
                    ConsoleKeyInfo key = Console.ReadKey();
                    ActionKeyChar = key.KeyChar - beginNumberOneASCII;
                    if(key.Key == ConsoleKey.C)
                    {
                        for (int i = 1; i <= x迴圈數量; i++)
                        {
                            ActionKeyChar = i;
                            Thread.Sleep(110);
                        }
                    }
                }
            }).Start();
            #endregion

            #region 向執行緒集區要求 20 個執行緒
            for (int i = 1; i <= x迴圈數量; i++)
            {
                int idx = i;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Console.WriteLine($"({idx}) {AsyncCourse.CurrentThreadId} 已經啟動執行了" +
                        $"({DateTime.Now})");
                    while (true)
                    {
                        Thread.Sleep(100);
                        //await Task.Delay(100);
                        if (idx == ActionKeyChar)
                        {
                            ActionKeyChar = -10;
                            break;
                        }
                    }
                    Console.WriteLine($"   ({idx}) {AsyncCourse.CurrentThreadId} 準備結束執行");
                });
            }
            #endregion
        }
    }
}
