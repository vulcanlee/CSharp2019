using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CoreServiceLocator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IServiceProvider serviceProvider;

        public ValuesController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] IServiceProvider actionServiceProvider)
        {
            List<string> result = new List<string>();
            //result.Add($"從 IApplicationBuilder.ApplicationServices 取得的 ServiceProvider {Startup.serviceProvider.GetHashCode().ToString()}");
            //result.Add($"從 IApplicationBuilder.ApplicationServices 解析出 IMessage {Startup.serviceProvider.GetService<IMessage>().GetHashCode().ToString()}");
            result.Add($"建構式注入取得的 ServiceProvider {serviceProvider.GetHashCode().ToString()}");
            result.Add($"從 建構式注入取得的 ServiceProvider 解析出 IMessage {serviceProvider.GetService<IMessage>().GetHashCode().ToString()}");
            result.Add($"使用 FromServices 屬性取得的 ServiceProvider {actionServiceProvider.GetHashCode().ToString()}");
            result.Add($"使用 FromServices 屬性取得的 ServiceProvider 解析出 IMessage {actionServiceProvider.GetService<IMessage>().GetHashCode().ToString()}");
            result.Add($"從 RequestServices 取得的 ServiceProvider {HttpContext.RequestServices.GetHashCode().ToString()}");
            result.Add($"從 RequestServices 取得的 ServiceProvider 解析出 IMessage {HttpContext.RequestServices.GetService<IMessage>().GetHashCode().ToString()}");
            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
