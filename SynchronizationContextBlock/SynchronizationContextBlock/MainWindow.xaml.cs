using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SynchronizationContextBlock
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        string url = "https://lobworkshop.azurewebsites.net/api/RemoteSource/Add/8/9/3";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnWillBlock_Click(object sender, RoutedEventArgs e)
        {
            // 請解釋這裡為什麼會產生執行緒封鎖 Block 的狀態
            string result = MethodAsync().Result;
            txtbkMessage.Text = "BtnWillBlock_Click 執行完畢 " + result;
        }

        private void BtnWhyWillNotBlock_Click(object sender, RoutedEventArgs e)
        {
            // 當直接把 Task.Delay 拿來使用，請解釋這裡為什麼不會產生執行緒封鎖 Block 的狀態
            //Task.Delay(1000).Wait();
            string result = new HttpClient().GetStringAsync(url).Result;
            txtbkMessage.Text = $"BtnWhyWillNotBlock_Click 執行完畢，Web API 結果:{result}";
        }

        private void btnUsingTaskRunWillNotBlock_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"呼叫 BtnWillNotBlock_Click 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            string result = Task.Run(async () =>
              {
                  Console.WriteLine($"呼叫 await MethodAsync(); 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
                  string callResult = await MethodAsync();
                  Console.WriteLine($"呼叫 await MethodAsync(); 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
                  return callResult;
              }).Result;
            Console.WriteLine($"呼叫 BtnWillNotBlock_Click 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            txtbkMessage.Text = "BtnWillBlock_Click 執行完畢 " + result;
        }

        private void BtnUsingNewThreadWillNotBlock_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            Thread thread = new Thread(x =>
            {
                Console.WriteLine($"呼叫 await MethodAsync(); 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
                result = MethodAsync().Result;
                Console.WriteLine($"呼叫 await MethodAsync(); 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            })
            { IsBackground = true };
            thread.Start();
            thread.Join();
            Console.WriteLine($"呼叫 BtnUsingNewThreadWillNotBlock_Click 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            txtbkMessage.Text = "BtnUsingNewThreadWillNotBlock_Click 執行完畢 " + result;
        }

        private void BtnResetSynchronizationContextWillNotBlock_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"呼叫 BtnResetSynchronizationContextWillNotBlock_Click 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            string result = MethodAsync().Result;
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            Console.WriteLine($"呼叫 BtnResetSynchronizationContextWillNotBlock_Click 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            txtbkMessage.Text = "BtnResetSynchronizationContextWillNotBlock_Click 執行完畢 " + result;
        }

        private void BtnUsingTaskLibraryWillNotBlock_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"呼叫 BtnUsingTaskLibraryWillNotBlock_Click 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            string result = InvokeAsyncMethod(() => MethodAsync());
            Console.WriteLine($"呼叫 BtnUsingTaskLibraryWillNotBlock_Click 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            txtbkMessage.Text = "BtnUsingTaskLibraryWillNotBlock_Click 執行完畢 " + result;
        }

        async Task<string> MethodAsync()
        {
            Console.WriteLine($"呼叫 MethodAsync 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            //await Task.Delay(1000);
            await new HttpClient().GetStringAsync(url);
            Console.WriteLine($"呼叫 MethodAsync 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            return " @@恭喜你 - MethodAsync 已經執行完畢了@@ ";
        }

        T InvokeAsyncMethod<T>(Func<Task<T>> func)
        {
            return Task.Factory.StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}
