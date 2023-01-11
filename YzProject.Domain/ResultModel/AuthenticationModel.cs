using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YzProject.Domain.ResultModel
{
    /// <summary>
    /// 身份验证模型
    /// </summary>
    public class AuthenticationModel
    {
        /// <summary>
        /// 返回状态消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否通过身份验证
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        //public List<string> Roles { get; set; }

        /// <summary>
        /// 访问令牌access token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [JsonIgnore]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
