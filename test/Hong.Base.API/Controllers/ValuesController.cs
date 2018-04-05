using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Hong.Common.Extendsion;
using Hong.Base.API.TestModel;

namespace Hong.Base.API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        ILogger Log { get; set; }

        public ValuesController(ILoggerFactory loggerFactory)
        {
            Log = loggerFactory.CreateLogger("ValuesController");
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
