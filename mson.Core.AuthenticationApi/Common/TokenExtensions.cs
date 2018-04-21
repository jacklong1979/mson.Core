using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using mson.Core.AuthenticationApi.Config;
using mson.Core.AuthenticationApi.MiddleWare;

namespace mson.Core.AuthenticationApi.Common
{
    /// <summary>
    /// 扩展帮助类
    /// </summary>
    public static class TokenExtensions
    {
        public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, TokenConfig options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<TokenMiddleware>(Options.Create(options));//注册中间件
        }
    }
}
