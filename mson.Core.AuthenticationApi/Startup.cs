using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using mson.Core.AuthenticationApi.Config;
using mson.Core.AuthenticationApi.MiddleWare;
using mson.Core.AuthenticationApi.Common;
namespace mson.Core.AuthenticationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 加载配置文件信息
            services.Configure<TokenConfig>(this.Configuration.GetSection("TokenConfig"));//加载配置文件信息
            #endregion
            #region JwtRegisteredClaimNames 方式 直接读取配置文件信息，初始化Token 需要验证的信息,如果不同在一台服务，则产生的Token与验证的Token的服务器验证信息与产生的信息要一致
            var audienceConfig = Configuration.GetSection("TokenConfig");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                #region 下面三个参数是必须
                // 签名秘钥
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // 发行者(颁发机构)
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],
                // 令牌的观众(颁发给谁)
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],
                #endregion
                // 是否验证Token有效期
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
                /***********************************TokenValidationParameters的参数默认值***********************************/
                // RequireSignedTokens = true,
                // SaveSigninToken = false,
                // ValidateActor = false,
                // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                // ValidateAudience = true,
                // ValidateIssuer = true, 
                // ValidateIssuerSigningKey = false,
                // 是否要求Token的Claims中必须包含Expires
                // RequireExpirationTime = true,
                // 允许的服务器时间偏移量
                // ClockSkew = TimeSpan.FromSeconds(300),//TimeSpan.Zero
                // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                // ValidateLifetime = true
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //不使用https
                //o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = tokenValidationParameters;
            });
            #endregion
            services.AddMvc();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            #region  JwtRegisteredClaimNames 方式 注册中间件 TokenMiddleware 
            var audienceConfig = Configuration.GetSection("TokenConfig");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            app.UseAuthentication(new TokenConfig
            {
                #region 初始化注入TokenConfig 到中间件
                Secret = audienceConfig["Secret"], //密钥
                Issuer = audienceConfig["Issuer"], //发行者
                Audience = audienceConfig["Audience"], //令牌的观众
                TokenType = audienceConfig["TokenType"], //表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
                Scope = audienceConfig["Scope"], //表示权限范围，如果与客户端申请的范围一致，此项可省略
                Subject = audienceConfig["Subject"], //主题
                ExpiresIn =Convert.ToInt32(audienceConfig["ExpiresIn"]), //表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
                ClientId = audienceConfig["ClientId"], //表示客户端的ID，必选项
                ResponseType = audienceConfig["ResponseType"], //表示授权类型，必选项，此处的值固定为"code"
                RedirectUri = audienceConfig["RedirectUri"],
                State = audienceConfig["State"], //表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值。
                SigningCredentials = SigningCredentials
                
                #endregion
            });
            #endregion
            app.UseMvc();
        }
    }
}
