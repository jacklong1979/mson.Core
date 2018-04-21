using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Options;
using mson.Core.Models;

namespace mson.Core.AuthenticationApi.Config
{
    /// <summary>
    /// IdentityServer + API+Client演示客户端模式
    /// </summary>
    public class TokenClient
    {
        static string secretString = "IW2FMMOSFMSDLIEPZ1525Z$%^&*(_)?><nbcxdsgcgSLKFDS!!#$^&+_)(*DFS;HDRT755825DBGHGGVBNVK;/;ZWZ4E546456";
        #region IdentityServer + API+Client演示客户端模式
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
        #region 加载配置文件信息
        private TokenConfig _tokenConfig { get; set; }
        public TokenClient(IOptions<TokenConfig> settings)
        {
            _tokenConfig = settings.Value;
        }
        #endregion
        /// <summary>
        /// 客户端注册，客户端能够访问的资源（通过：AllowedScopes）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClient()
        {
            return new List<Client>
            {
                #region 授权中心配置,可以增加多个不同的 Client
                new Client
                {
                    ClientId="client",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    ClientSecrets={new Secret(secretString.Sha256())},
                    AllowedScopes={"api"}
                },
                new Client
                {
                    ClientId = "Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"UserApi"}
                },
                new Client
                {
                    ClientId = "ro.Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"UserApi"}
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
        public static List<UserInfo> GeTestUsers()
        {
            return new List<UserInfo>
            {
                new UserInfo
                {
                    UserId = "test1",
                    UserName = "用户1",
                    Password = "a123"
                },
                new UserInfo
                {
                    UserId = "test2",
                    UserName = "用户2",
                    Password = "b123"
                }
            };
        }
        #endregion
    }
}
