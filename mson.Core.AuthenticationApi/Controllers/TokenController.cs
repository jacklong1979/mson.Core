using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using mson.Core.AuthenticationApi.Config;
using System.Linq;
//添加dll的引用 Nuget Microsoft.AspNetCore.Authentication.JwtBearer;

using mson.Core.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mson.Core.AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {

        #region 加载配置文件信息
        private TokenConfig _tokenConfig { get; set; }
        public TokenController(IOptions<TokenConfig> settings)
        {
            _tokenConfig = settings.Value;
        }
        #endregion       
        [Authorize]
        [HttpGet("getuser/{uid}")]
        public IActionResult GetUser(string uid)
        {           
            var ss=((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault();
            var usera = User.FindFirst(ClaimTypes.Sid);
            UserInfo user = new UserInfo() { UserId = uid, UserName = "明天会更好" };
            return Ok(user);
        }
    }
}
