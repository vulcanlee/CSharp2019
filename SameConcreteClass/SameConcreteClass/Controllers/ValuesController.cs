using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SameConcreteClass.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMessageScope1 messageScope1;
        private readonly IMessageScope2 messageScope2;
        private readonly IMessageSingleton1 messageSingleton1;
        private readonly IMessageSingleton2 messageSingleton2;

        public ValuesController(IMessageScope1 messageScope1, IMessageScope2 messageScope2,
            IMessageSingleton1 messageSingleton1, IMessageSingleton2 messageSingleton2)
        {
            this.messageScope1 = messageScope1;
            this.messageScope2 = messageScope2;
            this.messageSingleton1 = messageSingleton1;
            this.messageSingleton2 = messageSingleton2;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2",
                messageScope1.Write("messageScope1"), messageScope2.Write("messageScope2"),
                messageSingleton1.Write("messageSingleton1"), messageSingleton2.Write("messageSingleton2")
            };
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
