using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace HelpClassLibrary
{
    public class ServiceRegistration
    {
        public string Lifetime { get; set; }
        public string ServiceType { get; set; }
        public string ImplementationType { get; set; }
        public string ImplementationInstance { get; set; }
        public string ImplementationFactory { get; set; }
    }
    public static class ServiceDescriptorHelper
    {
        public static void Save(IServiceCollection serviceDescriptors, string filename)
        {
            ServiceCollection services = serviceDescriptors as ServiceCollection;
            List<ServiceRegistration> list = new List<ServiceRegistration>();
            foreach (var item in serviceDescriptors)
            {
                var newObject = new ServiceRegistration()
                {
                    Lifetime = item.Lifetime.ToString(),
                    ServiceType = item.ServiceType.Name,
                    ImplementationType = item.ImplementationType?.Name,
                    ImplementationInstance = item.ImplementationInstance?.GetType().Name,
                    ImplementationFactory = item.ImplementationFactory?.GetType().GenericTypeArguments[1].Name
                    //ImplementationFactory = item.ImplementationFactory != null ? "Factory" : ""
                };
                list.Add(newObject);
            }
            //var foo = JsonConvert.SerializeObject(newObject);
            string content = System.Text.Json.JsonSerializer.Serialize(list, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(filename, content);

            #region 產生出 HTML 檔案內容
            var sb = new StringBuilder();
            sb.Append("<h1>All Services</h1>");
            sb.Append("<table><thead>");
            sb.Append("<tr><th>Index</th><th>Type</th><th>Lifetime</th><th>ImplementationType</th><th>ImplementationInstance</th><th>ImplementationFactory</th></tr>");
            sb.Append("</thead><tbody>");
            int index = 1;
            foreach (var svc in serviceDescriptors)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{index++}</td>");
                sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                sb.Append($"<td>{svc.Lifetime}</td>");
                sb.Append($"<td>{(svc.ImplementationType!=null?svc.ImplementationType.Name:"")}</td>");
                sb.Append($"<td>{(svc.ImplementationInstance != null?svc.ImplementationInstance.GetType().Name : "")}</td>");
                sb.Append($"<td>{(svc.ImplementationFactory != null ? svc.ImplementationFactory.GetType().GenericTypeArguments[1].Name : "")}</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            File.WriteAllText(filename.Replace(".json", ".html"), sb.ToString());

            #endregion
        }
    }
}
