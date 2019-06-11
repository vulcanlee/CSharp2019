using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SameConcreteClass
{
    public interface IMessage
    {
        string Write(string message);
    }
    public interface IMessageScope1 : IMessage
    {
    }
    public interface IMessageScope2 : IMessage
    {
    }
    public interface IMessageSingleton1 : IMessage
    {
    }
    public interface IMessageSingleton2 : IMessage
    {
    }
    public class MessageClass : IMessageScope1, IMessageScope2, IMessageSingleton1, IMessageSingleton2
    {
        int HashCode;
        public MessageClass()
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
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMessageScope1, MessageClass>();
            services.AddScoped<IMessageScope2, MessageClass>();
            services.AddSingleton<IMessageSingleton1, MessageClass>();
            services.AddSingleton<IMessageSingleton2, MessageClass>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
