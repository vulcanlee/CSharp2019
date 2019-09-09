using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncMethodTaskCancellation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task fooAsyncTask = Task.Run(() =>
            {
                MyMethod(token);
            }, token);

            // 這裡在呼叫 Task.Run 方法的時候，第二個參數沒有傳遞 CancellationToken 物件
            Task fooAsyncTaskWithoutToken = Task.Run(() =>
            {
                MyMethod(token);
            });

            Task fooAsyncMethod = MyMethodAsync(token);

            await Task.Delay(500);

            cts.Cancel();

            await Task.Delay(1000);

            Console.WriteLine($"非同步工作的狀態: IsCompleted={fooAsyncTask.IsCompleted} , " +
                $"IsCanceled={fooAsyncTask.IsCanceled} , IsFaulted={fooAsyncTask.IsFaulted}");
            Console.WriteLine($"非同步工作(沒有傳送取消權杖)的狀態: IsCompleted={fooAsyncTaskWithoutToken.IsCompleted} , " +
                $"IsCanceled={fooAsyncTaskWithoutToken.IsCanceled} , IsFaulted={fooAsyncTaskWithoutToken.IsFaulted}");
            Console.WriteLine($"非同步方法的狀態: IsCompleted={fooAsyncMethod.IsCompleted} , " +
                $"IsCanceled={fooAsyncMethod.IsCanceled} , IsFaulted={fooAsyncMethod.IsFaulted}");

        }
        static async Task MyMethodAsync(CancellationToken token)
        {
            await Task.Delay(1000);
            token.ThrowIfCancellationRequested();
        }
        static void MyMethod(CancellationToken token)
        {
            Thread.Sleep(1000);
            token.ThrowIfCancellationRequested();
        }
    }
}
