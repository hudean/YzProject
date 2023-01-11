using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.RequestModel
{
    public class ParamDepartment
    {
        public int Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public string DepartmentManager { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactNumber { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 父级部门ID
        /// </summary>
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateUserId { get; set; }
    }

    public class ParamQueryPageDepartment : ParamPage
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [MaxLength(32, ErrorMessage = "部门名称长度不能大于32")]
        public string DepartmentName { get; set; }
    }
}
