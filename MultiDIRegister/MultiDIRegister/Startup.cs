using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MultiDIRegister
{
    public interface IMessage
    {
        string Output(string msg);
    }
    public class ConsoleMessage : IMessage
    {
        public string Output(string msg)
        {
            return $"Console : {msg}</br>";
        }
    }
    public class FileMessage : IMessage
    {
        public string Output(string msg)
        {
            return $"File : {msg}</br>";
        }
    }
    public class Startup
    {
        IServiceCollection Services;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;
            services.AddTransient<IMessage, ConsoleMessage>();
            services.AddTransient<IMessage, FileMessage>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IMessage message)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Services)
            {
                if(item.ServiceType.Name.Contains("IMessage"))
                {
                    sb.Append($"{item.ServiceType.Name} => {item.ImplementationType.Name}");
                    sb.Append("</br>");
                }
            }
                if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(message.Output("Hello World!"));
                    await context.Response.WriteAsync(sb.ToString());
                });
            });
        }
    }
}
