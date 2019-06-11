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
using Unity;

namespace AspNETCoreUnity
{
    public interface IMessage
    {
        string Send(string message);
    }

    public class ConsoleMessage : IMessage
    {
        public string Send(string message)
        {
            string result = $"ConsoleMessage :{message}";
            Console.WriteLine(result);
            return result;
        }
    }
    public class FileMessage : IMessage
    {
        public string Send(string message)
        {
            string result = $"FileMessage :{message}";
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();
        }
        //public IServiceProvider ConfigureServices(IServiceCollection services)
        //{
        //    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        //        .AddControllersAsServices();
        //    var serviceProvider =
        //    Unity.Microsoft.DependencyInjection.ServiceProvider.ConfigureServices(services)
        //    as Unity.Microsoft.DependencyInjection.ServiceProvider;

        //    ConfigureContainer((UnityContainer)serviceProvider);
        //    return serviceProvider;
        //}
        public void ConfigureContainer(IUnityContainer container)
        {
            // Could be used to register more types
            container.RegisterType<IMessage, ConsoleMessage>();
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
