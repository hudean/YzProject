using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.ResultModel
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        ///// <summary>
        ///// 角色编码
        ///// </summary>
        //public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string RoleDescription { get; set; }
    }
}
