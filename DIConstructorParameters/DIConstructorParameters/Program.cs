using Microsoft.Extensions.DependencyInjection;
using System;

namespace DIConstructorParameters
{
    interface IMyInterface1 { }
    class MyClass1 : IMyInterface1 { }
    interface IMyInterface2 { }
    class MyClass2 : IMyInterface2 { }
    interface IMyInterface3 { }
    class MyClass3 : IMyInterface3 { }
    interface IYourInterface { }
    class YourClass : IYourInterface
    {
        // 建構函式 1
        public YourClass()
        {
            Console.WriteLine("YourClass 預設建構式1被呼叫");
        }
        // 建構函式 2
        public YourClass(IMyInterface1 myInterface1, IMyInterface2 myInterface2)
        {
            Console.WriteLine("YourClass 建構式2(IMyInterface1, IMyInterface2) 被呼叫");
        }
        //// 建構函式 3
        //public YourClass(IMyInterface1 myInterface1, IMyInterface3 myInterface3)
        //{
        //    Console.WriteLine("YourClass 建構式3(IMyInterface1, IMyInterface3) 被呼叫");
        //}
        // 建構函式 4
        public YourClass(IMyInterface1 myInterface1, string myString)
        {
            Console.WriteLine("YourClass 建構式4(IMyInterface1, string) 被呼叫");
        }
        // 建構函式 5
        public YourClass(IMyInterface1 myInterface1, string myString, int myInt)
        {
            Console.WriteLine("YourClass 建構式5(IMyInterface1, string, int) 被呼叫");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddTransient<IMyInterface1, MyClass1>();
            services.AddTransient<IMyInterface2, MyClass2>();
            services.AddTransient<IMyInterface3, MyClass3>();
            services.AddTransient<IYourInterface, YourClass>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IYourInterface yourInterface = serviceProvider.GetService<IYourInterface>();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
