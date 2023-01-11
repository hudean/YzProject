using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Model
{
    public class JwtTokenConfig
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 发放人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// token过期时间
        /// </summary>
        public int AccessExpiration { get; set; }
        /// <summary>
        /// 刷新过期时间
        /// </summary>
        public int RefreshExpiration { get; set; }
    }
}
