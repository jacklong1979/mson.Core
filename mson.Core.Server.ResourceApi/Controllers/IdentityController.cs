using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mson.Core.Server.ResourceApi.Controllers
{
   // [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        #region 【方式1】IdentityServer + API+Client演示客户端模式
        [Route("identity")]
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
        #endregion
    }
}
