using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace HowConfigureAwait
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

        private async void BtnWillBlock_Click(object sender, RoutedEventArgs e)
        {
            string result = Method1Async().Result;
            Debug.WriteLine(result);
        }
        async Task<string> Method1Async()
        {
            Console.WriteLine($"呼叫 Method1Async 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            Task<string> task = new HttpClient().GetStringAsync(url);
            await task.ConfigureAwait(true);
            string result = task.Result;
            Console.WriteLine($"呼叫 Method1Async 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            return " @@恭喜你 - Method1Async({result}) 已經執行完畢了@@ ";
        }

        private void BtnWillNotBlock_Click(object sender, RoutedEventArgs e)
        {
            string result = Method2Async().Result;
            Debug.WriteLine(result);
            tbMessage.Text = result;
        }
        async Task<string> Method2Async()
        {
            Console.WriteLine($"呼叫 Method2Async 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            Task<string> task = new HttpClient().GetStringAsync(url);
            await task.ConfigureAwait(false);
            string result = task.Result;
            Console.WriteLine($"呼叫 Method2Async 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            return $" @@恭喜你 - Method2Async({result}) 已經執行完畢了@@ ";
        }

        private void BtnWillNotBlockByAsyncMethod_Click(object sender, RoutedEventArgs e)
        {
            Task<string> task = Method3Async();
            task.ConfigureAwait(false);
            string result = task.Result;
            Debug.WriteLine(result);
        }
        async Task<string> Method3Async()
        {
            Console.WriteLine($"呼叫 Method1Async 之前，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            Task<string> task = new HttpClient().GetStringAsync(url);
            await task.ConfigureAwait(true);
            string result = task.Result;
            Console.WriteLine($"呼叫 Method1Async 之後，執行緒 ID {Thread.CurrentThread.ManagedThreadId}");
            return " @@恭喜你 - Method1Async({result}) 已經執行完畢了@@ ";
        }
    }
}
