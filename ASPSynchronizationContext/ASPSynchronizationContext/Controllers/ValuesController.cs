using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;

namespace ASPSynchronizationContext.Controllers
{
    public class ValuesController : ApiController
    {
        // https://localhost:44320/api/values
        // GET api/values
        public IEnumerable<string> Get()
        {
            SynchronizationContext sc = SynchronizationContext.Current;            

            ShowThreadInformation("HTTP Reques 請求執行緒");

            for (int i = 1; i <= 5; i++)
            {
                int asyncIdx = i;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Console.Write($"非同步工作{asyncIdx}，需要2秒的處理時間");
                    ShowThreadInformation($"非同步工作{asyncIdx}");
                    Thread.Sleep(2000);
                    sc.Send(x =>
                    {
                        Console.Write($"非同步工作{asyncIdx}完成後，要執行接續的工作");
                        ShowThreadInformation($"非同步工作{asyncIdx}完成後，要執行接續的工作");
                    }, null);
                });
            }
            Thread.Sleep(3000);
            return new string[] { "value1", "value2" };
        }

        private static void ShowThreadInformation(string msg)
        {
            Debug.WriteLine($"{msg} >> 執行緒ID = {Thread.CurrentThread.ManagedThreadId}");
            Debug.WriteLine($"{msg} >> HttpContent = {System.Web.HttpContext.Current}");
            Debug.WriteLine($"{msg} >> 目前同步內容 = {SynchronizationContext.Current}");
            Thread.Sleep(300);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
