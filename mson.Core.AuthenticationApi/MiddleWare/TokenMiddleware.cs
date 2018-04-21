using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using mson.Core.AuthenticationApi.Config;
using Newtonsoft.Json;

namespace mson.Core.AuthenticationApi.MiddleWare
{
    /// <summary>
    /// Token中间件类
    /// </summary>
    public class TokenMiddleware : Controller
    {
        private readonly RequestDelegate _next;
        private  TokenConfig _options;

        public TokenMiddleware(RequestDelegate next, IOptions<TokenConfig> options)
        {
            _next = next;
            _options = options.Value;
        }
        #region JwtRegisteredClaimNames 方式
        public Task Invoke(HttpContext context)
        {
            #region 如果请求地址不匹配，则跳过 如：http://localhost:2000/token
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }
            #endregion
            #region 请求方式必须是 POST 并且 Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("请设置post请求和 Content-Type=application/x-www-form-urlencoded");
            }
            #endregion
            return  GenerateToken(context);
        }
        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
              
                new Claim(JwtRegisteredClaimNames.Aud, _options.Audience),
                new Claim(JwtRegisteredClaimNames.Sid, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Exp, _options.ExpiresIn.ToString()),
                new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, _options.Issued.ToString(), ClaimValueTypes.Integer64),//发行时间
                //用户名
                new Claim("UserName","中国人民"),
                //角色
                new Claim("Role","我是角色"),
                new Claim("Country","中国"),
                new Claim("Expired",_options.ExpiresIn.ToString()),
                new Claim("Mobile","13556891160")
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer:  _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: _options.ExpiresTime,
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                token_type = _options.TokenType,
                expires_in = _options.ExpiresIn, //过期时间(秒)
                expires_time = _options.ExpiresTime.ToString("yyyy-MM-dd HH:mm:ss"), //过期时间（日期）
                claims = claims
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            // DON'T do this in production, obviously!
            if (username == "TEST" && password == "TEST123")
            {
                return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
        #endregion
    }
}
