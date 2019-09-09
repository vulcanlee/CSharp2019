using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MultiAsyncCombine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("這裡將會展示當要呼叫多個非同步作業的各種不同設計方式");

            // 這裡寫法的特色：使用一行敘述 (Task 支援 fluent API 寫法)，就可以完成所有設計
            //使用工作ContinueWith();

            // 這裡寫法的特色：需要使用封鎖式等待非同步作業(可能是非同步工作或者非同步方法)的完成
            同步封鎖等待非同步作業完成();

            // 這裡寫法的特色：使用 async 修飾詞 與 await 運算子 ，採用不會直接封鎖的方式來呼叫非同步作業
            // 重點是可以使用同步設計的風格和方式，設計出具有非同步運作能力的程式碼
            //await呼叫非同步方法Async().Wait();
        }

        private static void 同步封鎖等待非同步作業完成()
        {
            string url = "https://lobworkshop.azurewebsites.net/api/RemoteSource/AddASync/15/43/3000";
            string content = new HttpClient().GetStringAsync(url).Result;
            var task2 = File.WriteAllTextAsync("同步封鎖等待完成.txt", content);
            task2.Wait();
            if (task2.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine("已經成功下載內容並且寫入到檔案內");
            }
            else
            {
                Console.WriteLine("寫入檔案發生了問題");
            }
        }

        private static void 使用工作ContinueWith()
        {
            string url = "https://lobworkshop.azurewebsites.net/api/RemoteSource/AddASync/15/43/3000";
            var myTask = new HttpClient().
                GetStringAsync(url).ContinueWith(task1 =>
                {
                    string content = task1.Result;
                    File.WriteAllTextAsync("MyFileContinueWith.txt", content).
                    ContinueWith(task2 =>
                    {
                        if (task2.Status == TaskStatus.RanToCompletion)
                        {
                            Console.WriteLine("已經成功下載內容並且寫入到檔案內");
                        }
                        else
                        {
                            Console.WriteLine("寫入檔案發生了問題");
                        }
                    });
                });

            Thread.Sleep(4000);
        }

        private static async Task await呼叫非同步方法Async()
        {
            string content = await GetRemoteStringAsync();
            await WriteRemoteStringAsync("MyFile.txt", content);
        }

        static async Task<string> GetRemoteStringAsync()
        {
            string url = "https://lobworkshop.azurewebsites.net/api/RemoteSource/AddASync/15/43/3000";
            string result = await new HttpClient().GetStringAsync(url);
            return result;
        }
        static async Task WriteRemoteStringAsync(string filename, string content)
        {
            await File.WriteAllTextAsync(filename, content);
        }
    }
}
