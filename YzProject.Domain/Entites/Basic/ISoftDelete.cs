using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// 软删除
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// 用于将实体标记为“已删除” true 删除
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
