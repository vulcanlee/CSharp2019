using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunContinueWith
{
    class Program
    {
        static async Task Main(string[] args)
        {
            PrintStatus("開始執行 Main 方法");
            Console.WriteLine();

            await UsingContinueWithAsync();
            PrintStatus("await UsingContinueWithAsync() 執行完成");
            Console.WriteLine();

            await UsingContinueWithByExecuteSynchronouslyAsync();
            PrintStatus("await UsingContinueWithByExecuteSynchronouslyAsync() 執行完成");
            Console.WriteLine();

            await UsingContinueWithByLongRunningAsync();
            PrintStatus("await UsingContinueWithByLongRunningAsync() 執行完成");
            Console.WriteLine();

            await UsingTaskRunAsync();
            PrintStatus("await UsingTaskRunAsync() 執行完成");
            Console.WriteLine();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
        static async Task UsingTaskRunAsync()
        {
            PrintStatus("開始執行僅使用 Task.Run 方法 - UsingTaskRunAsync");
            var task = Task.Run(() =>
            {
                Thread.Sleep(2000);
                PrintStatus("正在 Task.Run 內執行中...");
            });
            await task;
            PrintStatus("使用 Task.Run 方法結束之後 - UsingTaskRunAsync");
        }
        static Task UsingContinueWithAsync()
        {
            PrintStatus("開始執行 ContinueWith 方法 - UsingContinueWithAsync");
            var task = Task.Run(() =>
            {
                Thread.Sleep(2000);
                PrintStatus("正在 Task.Run 內執行中...");
            });
            var taskContinue = task.ContinueWith(t =>
            {
                PrintStatus("正在 ContinueWith 內執行中...");
            });
            return taskContinue;
        }

        static Task UsingContinueWithByExecuteSynchronouslyAsync()
        {
            PrintStatus("開始執行 ContinueWith 方法 - UsingContinueWithByExecuteSynchronouslyAsync");
            var task = Task.Run(() =>
            {
                Thread.Sleep(2000);
                PrintStatus("正在 Task.Run 內執行中...");
            });
            var taskContinue = task.ContinueWith(t =>
            {
                PrintStatus("正在 ContinueWith 內執行中...");
            }, TaskContinuationOptions.ExecuteSynchronously);
            return taskContinue;
        }


        static Task UsingContinueWithByLongRunningAsync()
        {
            PrintStatus("開始執行 ContinueWith 方法 - UsingContinueWithByLongRunningAsync");
            var task = Task.Run(() =>
            {
                Thread.Sleep(2000);
                PrintStatus("正在 Task.Run 內執行中...");
            });
            var taskContinue = task.ContinueWith(t =>
            {
                PrintStatus("正在 ContinueWith 內執行中...");
            }, TaskContinuationOptions.LongRunning);
            return taskContinue;
        }
        private static void PrintStatus(string message)
        {
            Console.WriteLine($"{message} : {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
