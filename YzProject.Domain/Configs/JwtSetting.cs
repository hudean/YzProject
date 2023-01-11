using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Configs
{
    public class JwtSetting
    {
        /// <summary>
        /// Key – 将用于加密的超级密钥。您可以将其移动到其他地方以获得额外的安全性。
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Issuer  – 标识发布 JWT的委托人。
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 受众 – 识别 JWT 的目标接收者。
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// DurationInMinutes – 定义生成的 JWT 保持有效的分钟数。
        /// </summary>
        public double DurationInMinutes { get; set; }
    }
}
