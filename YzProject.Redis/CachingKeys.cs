using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Redis
{
    /// <summary>
    /// 缓存键值类
    /// </summary>
    public static class CachingKeys
    {
        /// <summary>
        /// ss
        /// </summary>
        public const string BASEDATA_ALL_KEY = "BASEDATA_ALL_KEY";

        //public  static  string BASEDATA_ALL_KEY { get; set; } = "BASEDATA_ALL_KEY";

        public static string AccessTokenHashKey { get; set; } = "AccessTokenHashKey";
    }
}
