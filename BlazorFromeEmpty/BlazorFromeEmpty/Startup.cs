using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorFromeEmpty
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region 在這裡要宣告所要使用的服務
            services.AddRazorPages();
            services.AddServerSideBlazor();
            // 設著將這行註解起來，並且執行，看看會發生甚麼問題？
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/");
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region 請加入這兩行敘述
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            #endregion

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //await context.Response.WriteAsync("Hello World!");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
