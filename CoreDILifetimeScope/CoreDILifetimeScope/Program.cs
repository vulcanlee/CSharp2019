using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace CoreDILifetimeScope
{
    public interface IMessage
    {
        string Write(string message);
    }
    public class ConsoleMessage : IMessage
    {
        int HashCode;
        public ConsoleMessage()
        {
            HashCode = this.GetHashCode();
            Console.WriteLine($"ConsoleMessage ({HashCode}) 已經被建立了");
        }
        public string Write(string message)
        {
            string result = $"[Console 輸出  ({HashCode})] {message}";
            Console.WriteLine(result);
            return result;
        }
        ~ConsoleMessage()
        {
            Console.WriteLine($"ConsoleMessage ({HashCode}) 已經被釋放了");
        }
    }
    public class Program
    {
        static IServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            IMessage message1;
            IMessage message2;
            IMessage message1_1;
            IMessage message2_1;
            IMessage message9_1;
            IMessage message9;
            IServiceProvider serviceProvider1;
            IServiceProvider serviceProvider2;
            IServiceProvider serviceProvider3;
            IServiceCollection serviceCollection;
            IServiceScope serviceScope2;
            IServiceScope serviceScope3;

            serviceCollection = new ServiceCollection();
            //serviceCollection.AddTransient<IMessage, ConsoleMessage>();
            serviceCollection.AddScoped<IMessage, ConsoleMessage>();
            //serviceCollection.AddSingleton<IMessage, ConsoleMessage>();
            serviceProvider1 = serviceCollection.BuildServiceProvider();

            #region 使用預設 Scope
            //message1 = serviceProvider1.GetService<IMessage>();
            //message1.Write("M1 - Vulcan");
            //message2 = serviceProvider1.GetService<IMessage>();
            //message2.Write("M2 - Lee");
            //message1 = null;
            //message2 = null;
            //GC.Collect(2);
            //Thread.Sleep(1000);
            //message9 = serviceProvider1.GetService<IMessage>();
            //message9.Write("M9 - Vulcan Lee");
            #endregion

            #region 使用兩個 Scope
            serviceScope2 = serviceProvider1.CreateScope();
            serviceProvider2 = serviceScope2.ServiceProvider;
            message1 = serviceProvider2.GetService<IMessage>();
            message1.Write("M1 - Vulcan");
            message2 = serviceProvider2.GetService<IMessage>();
            message2.Write("M2 - Lee");
            message1 = null;
            message2 = null;
            GC.Collect(2);
            Thread.Sleep(1000);
            message9 = serviceProvider2.GetService<IMessage>();
            message9.Write("M9 - Vulcan Lee");

            serviceScope3 = serviceProvider1.CreateScope();
            serviceProvider3 = serviceScope3.ServiceProvider;
            message1_1 = serviceProvider3.GetService<IMessage>();
            message1_1.Write("M1_1 - Ada");
            message2_1 = serviceProvider3.GetService<IMessage>();
            message2_1.Write("M2_1 - Chan");
            message1_1 = null;
            message2_1 = null;
            GC.Collect(2);
            Thread.Sleep(1000);
            message9_1 = serviceProvider3.GetService<IMessage>();
            message9_1.Write("M9_1 - Ada Chan");
            // 若將底下的程式碼註解起來(在 AddScoped 模式)，則 
            // message1, message2 指向到 ConsoleMessage 會被釋放掉
            message9.Write("M9 - Vulcan Lee");
            #endregion

            //Console.WriteLine("Press any key for continuing...");
            //Console.ReadKey();
        }
    }
}
