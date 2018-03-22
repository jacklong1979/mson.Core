﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using mson.Core.Common;
using NLog.Extensions.Logging;
using NLog.Web;
namespace mson.Core.AuthorizationServerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            env.ConfigureNLog("nlog.config");//增加日志配置文件
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 注册JwtBearer认证
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//默认的认证方案
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//默认的握手方案
            })
          .AddJwtBearer(o =>
          {
              o.TokenValidationParameters = new TokenValidationParameters
              {
                  NameClaimType = JwtClaimTypes.Name,
                  RoleClaimType = JwtClaimTypes.Role,
                  //下面三个参数是必须
                  ValidIssuer = "http://localhost:2000",//Token颁发机构
                  ValidAudience = "api",//颁发给谁
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Consts.Secret))//签名秘钥

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
                  // ClockSkew = TimeSpan.FromSeconds(300),
                  // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                  // ValidateLifetime = true
              };
          });
            #endregion
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            loggerFactory.AddNLog();//
            app.AddNLogWeb();//增加  NLog to ASP.NET Core
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
