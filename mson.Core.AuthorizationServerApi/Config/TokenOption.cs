using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace mson.Core.AuthorizationServerApi.Config
{
    /// <summary>
    /// Token的配置，生成Token所需要的信息
    /// </summary>
    public class TokenConfig
    {
        //access_token：表示访问令牌，必选项。
        //token_type：表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
        //expires_in：表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
        //refresh_token：表示更新令牌，用来获取下一次的访问令牌，可选项。
        //scope：表示权限范围，如果与客户端申请的范围一致，此项可省略
        //response_type：表示授权类型，必选项，此处的值固定为"code"
        //client_id：表示客户端的ID，必选项
        //redirect_uri：表示重定向URI，可选项
        //scope：表示申请的权限范围，可选项
        //state：表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值。
        //iss：Issuer，发行者
        //sub：Subject，主题
        //aud：Audience，观众
        //exp：Expiration time，过期时间
        //nbf：Not before
        //iat：Issued at，发行时间
        //jti：JWT ID
        /// <summary>
        /// 定义请求的路径，如：http://localhost:2000/token
        /// </summary>
        public string Path { get; set; } = "/token";
        /// <summary>
        /// 发行者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 令牌的观众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// 表示用于生成数字签名的加密密钥和安全算法。
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
