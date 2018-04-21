using System;
using Microsoft.IdentityModel.Tokens;

namespace mson.Core.AuthenticationApi.Config
{
    /// <summary>
    /// Token的配置，生成Token所需要的信息
    /// </summary>
    public class TokenConfig
    {
       
        /// <summary>
        /// 定义请求的路径，如：http://localhost:2000/token
        /// </summary>
        public string Path { get; set; } = "/token";
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 发行者(颁发机构)
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 令牌的观众(颁发给谁)
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 到期时间(秒)
        /// </summary>
        public int ExpiresIn { get; set; }
        /// <summary>
        /// 到期时间（日期）
        /// </summary>
        public DateTime ExpiresTime { get { return DateTime.Now.AddSeconds(ExpiresIn); } }
        /// <summary>
        /// /表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
        /// </summary>
        public string TokenType { get; set; }
        /// <summary>
        /// 表示权限范围，如果与客户端申请的范围一致，此项可省略
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 表示客户端的ID，必选项
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 表示授权类型，必选项，此处的值固定为"code"
        /// </summary>
        public string ResponseType { get; set; }
        /// <summary>
        /// 重定向地址
        /// </summary>
        public string RedirectUri { get; set; }
        /// <summary>
        /// 表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值。
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 发行时间
        /// </summary>
        public DateTime Issued { get; set; } = DateTime.Now;       
       
        /// <summary>
        /// 表示用于生成数字签名的加密密钥和安全算法。
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
