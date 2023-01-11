using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// 将最后修改时间属性添加到类的标准接口。
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        /// 该实体的最后修改时间。
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}
