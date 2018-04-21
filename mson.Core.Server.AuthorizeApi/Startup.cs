using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mson.Core.Server.AuthorizeApi.Common;

namespace mson.Core.Server.AuthorizeApi
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
            #region 【方式1】IdentityServer + API+Client演示客户端模式
            // 使用内存存储，密钥，客户端和资源来配置身份服务器。
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(APIClient.GetApiResource())
                .AddInMemoryClients(APIClient.GetClients())
                .AddResourceOwnerValidator<MyUserValidator>()
                .AddTestUsers(APIClient.GeTestUsers());

            ////RSA：证书长度2048以上，否则抛异常
            ////配置AccessToken的加密证书
            //var rsa = new RSACryptoServiceProvider();
            ////从配置文件获取加密证书
            //rsa.ImportCspBlob(Convert.FromBase64String(Configuration["SigningCredential"]));
            ////IdentityServer4授权服务配置
            //services.AddIdentityServer()
            //    .AddSigningCredential(new RsaSecurityKey(rsa))    //设置加密证书
            //    //.AddTemporarySigningCredential()    //测试的时候可使用临时的证书
            //    .AddInMemoryScopes(TokenClient.GetScopes())
            //    .AddInMemoryClients(TokenClient.GetClients())
            //    //如果是client credentials模式那么就不需要设置验证User了
            //    .AddResourceOwnerValidator<MyUserValidator>() //User验证接口
            //    //.AddInMemoryUsers(OAuth2Config.GetUsers())    //将固定的Users加入到内存中
            //    ;

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
            app.UseIdentityServer();//【方式1】IdentityServer + API + Client演示客户端模式                                   
            app.UseMvc();
        }
    }
}
