using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CoreLogging
{
    class MyClass
    {
        private readonly ILogger<MyClass> logger;

        public MyClass(ILogger<MyClass> logger)
        {
            this.logger = logger;
        }
        public void Method()
        {
            logger.LogInformation("現在正在執行 Method 方法");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var myClass = serviceProvider.GetService<MyClass>();

            myClass.Method();

            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogError("Program 類別內發生了意外異常");

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                      .AddTransient<MyClass>();
        }
    }
}
