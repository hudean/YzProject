using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// 角色菜单关系实体
    /// 角色和菜单 多对多关系
    /// </summary>
    public class RoleMenu : Entity<Guid>
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 菜单id
        /// </summary>
        public int MenuId { get; set; }
    }
}
