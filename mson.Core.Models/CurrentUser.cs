using System;

namespace mson.Core.Models
{
    /// <summary>
    /// 当前用户
    /// </summary>
    public class UserInfo
    {
        public string UserId { get; set; }
        public string  Password { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
