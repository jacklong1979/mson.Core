//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using mson.Core.Authentication.Config;

//namespace mson.Core.AuthenticationApi.Controllers
//{
//    [Route("api/[controller]")]
//    public class ValuesController : Controller
//    {

//        #region 加载配置文件信息
//        private TokenConfig _TokenConfig { get; set; }
//        public ValuesController(IOptions<TokenConfig> settings)
//        {
//            _TokenConfig = settings.Value;
//        }
//        #endregion
//        // GET api/values
//        [HttpGet]
//        public IEnumerable<string> Get()
//        {
//            var value= new string[] { "value1", "value2" };
//            Console.WriteLine(value);
//            return value;
//        }

//        // GET api/values/5
//        [HttpGet("{id}")]
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST api/values
//        [HttpPost]
//        public void Post([FromBody]string value)
//        {
//        }

//        // PUT api/values/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody]string value)
//        {
//        }

//        // DELETE api/values/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}
