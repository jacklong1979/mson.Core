using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using mson.Core.Common;
using mson.Core.Models;

namespace mson.Core.AuthorizationServerApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly ILogger<AuthController> _logger;
        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }
        [HttpGet("getuser")]
        public IActionResult GetUser()
        {
            _logger.LogInformation("中国人民【LogInformation】");
            _logger.LogDebug("中国人民【LogDebug】");
            _logger.LogWarning("中国人民【LogWarning】");
            _logger.LogError("中国人民【LogError】");
            _logger.LogTrace("中国人民【LogTrace】");
            CurrentUser user = new CurrentUser();
            user.UserId = "longkc";
            user.UserName = "中国人";
            user.PhoneNumber = "1355689220";
            user.Address = "中国深圳市场龙华新区";
            return Json(user);
        }
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]CurrentUser userDto)
        {
            var user = userDto;// _store.FindUser(userDto.UserName, userDto.Password);
            if (user == null) return Unauthorized();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Consts.Secret);//签名秘钥
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(JwtClaimTypes.Audience,"api"),
            new Claim(JwtClaimTypes.Issuer,"http://localhost:5200"),
            new Claim(JwtClaimTypes.Id, user.UserId.ToString()),
            new Claim(JwtClaimTypes.Name, user.UserName),
            new Claim(JwtClaimTypes.Email, user.Email),
            new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber)
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    sid = user.UserId,
                    name = user.UserName,
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }
    }
}
