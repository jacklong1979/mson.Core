using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//添加dll的引用 Nuget Microsoft.AspNetCore.Authentication.JwtBearer;

using mson.Core.Common;
using mson.Core.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mson.Core.AuthorizationServerApi.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("gettoken/{uid}/{pwd}")]      
        public IActionResult GetToken(string uid,string pwd)
        {
            UserInfo user = new UserInfo() { UserId = uid,UserName="龙康才", Password = pwd };           
            if (user == null) return Unauthorized();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Consts.Secret);//签名秘钥
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddSeconds(60);

            //var tokenObj = new SecurityTokenDescriptor();
            //tokenObj.Subject = new ClaimsIdentity();
            //tokenObj.Expires = expiresAt;
            //tokenObj.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            //tokenObj.Subject.AddClaim(new Claim(JwtClaimTypes.Audience, "api"));
            //tokenObj.Subject.AddClaim(new Claim(JwtClaimTypes.Issuer, "http://localhost:5200"));
            //tokenObj.Subject.AddClaim(new Claim(JwtClaimTypes.Id, user.UserId.ToString()));
            //tokenObj.Subject.AddClaim(new Claim(JwtClaimTypes.Name, user.UserName));
            ////tokenObj.Subject.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));
            ////tokenObj.Subject.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtClaimTypes.Audience,"api"),
                    new Claim(JwtClaimTypes.Issuer,"http://localhost:2000"),
                    new Claim(JwtClaimTypes.Id, user.UserId.ToString()),
                    new Claim(JwtClaimTypes.Name, user.UserName)
                    //new Claim(JwtClaimTypes.Email, user.Email),
                    //new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber)
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
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
       [Authorize]
        [HttpGet("getuser/{uid}")]
        public IActionResult GetUser(string uid)
        {
            UserInfo user = new UserInfo() { UserId = uid, UserName = "龙康才明天会更好"};          
            return Ok(user);
        }
    }
}
