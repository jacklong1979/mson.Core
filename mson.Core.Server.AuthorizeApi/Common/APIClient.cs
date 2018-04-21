using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using mson.Core.Server.Models;
using static IdentityServer4.IdentityServerConstants;

namespace mson.Core.Server.AuthorizeApi.Common
{
    #region 用户Resource Owner Password模式需要对账号密码进行验证
    /// <summary>
    /// 用户Resource Owner Password模式需要对账号密码进行验证
    /// </summary>
    public class MyUserValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context.UserName == "admin" && context.Password == "123")
            {
                //验证成功
                //使用subject可用于在资源服务器区分用户身份等等
                //获取：资源服务器通过User.Claims.Where(l => l.Type == "sub").FirstOrDefault();获取
                context.Result = new GrantValidationResult(subject: "admin", authenticationMethod: "custom");
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
            return Task.FromResult(0);
        }
    }
    #endregion
    /// <summary>
    /// IdentityServer + API+Client演示客户端模式
    /// </summary>
    public class APIClient
    {

        #region 【方式1】IdentityServer + API+Client演示客户端模式
        static string secretString = "25Z$%^&*(_)?><nbc6";
        /// <summary>
        /// 定义授权范围（通过API可以访问的资源）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                //给api资源定义一个scopes
                new ApiResource("api1","my api")
            };
        }      
        /// <summary>
        /// 客户端注册，客户端能够访问的资源（通过：AllowedScopes）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                #region 授权中心配置,可以增加多个不同的 Client
                new Client
                {
                    ClientId="client1",
                    AllowedGrantTypes=GrantTypes.ClientCredentials, // 没有交互性用户，使用 clientid/secret 实现认证。client credentials模式则不需要对账号密码验证
                    ClientSecrets={new Secret(secretString.Sha256())},
                    AllowedScopes={ "api1" }//  // 客户端有权访问的范围（Scopes）
                },
               new Client
                {
                    ClientId = "client",
	                // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
	                // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret(secretString.Sha256())
                    },
	                // 客户端有权访问的范围（Scopes）
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                     ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret(secretString.Sha256())
                    },
                    AllowedScopes = { "api1" }
                    //ClientId = "pwdClient",
                    //AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,//Resource Owner Password模式需要对账号密码进行验证（如果是client credentials模式则不需要对账号密码验证了）：
                    //ClientSecrets ={new Secret(secretString.Sha256())},                  
                    //AllowedScopes =
                    //{
                    //    "UserApi"
                    //    //如果想带有RefreshToken，那么必须设置：StandardScopes.OfflineAccess
                    //    //如果是Client Credentials模式不支持RefreshToken的，就不需要设置OfflineAccess
                    //    //StandardScopes.OfflineAccess
                    //}
                    // //AccessTokenLifetime = 3600, //AccessToken的过期时间， in seconds (defaults to 3600 seconds / 1 hour)
                    ////AbsoluteRefreshTokenLifetime = 60, //RefreshToken的最大过期时间，in seconds. Defaults to 2592000 seconds / 30 day
                    ////RefreshTokenUsage = TokenUsage.OneTimeOnly,   //默认状态，RefreshToken只能使用一次，使用一次之后旧的就不能使用了，只能使用新的RefreshToken
                    ////RefreshTokenUsage = TokenUsage.ReUse,   //可重复使用RefreshToken，RefreshToken，当然过期了就不能使用了
                },
                   // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "MVC",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                     ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "UserApi"
                    },
                    AllowOfflineAccess = true
                },
                   // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5003" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "UserApi"
                    },
                }
                #endregion
            };
        }
        /// <summary>
        /// 测试用户
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GeTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "abc",
                    Password = "a123"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bcd",
                    Password = "b123"
                }
            };
        }
        #endregion
    }
}
