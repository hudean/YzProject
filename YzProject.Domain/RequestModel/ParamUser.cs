using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.RequestModel
{
    public class ParamUser
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateUserId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HardImgUrl { get; set; }


        public string Address { get; set; }

    }

    public class ParamQueryPageUser : ParamPage
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [MaxLength(32, ErrorMessage = "用户长度不能大于32")]
        public string UserName { get; set; }
    }

    public class ParamUserPassword
    {
        public int Id { get; set; }

        [MaxLength(32, ErrorMessage = "Password长度不能大于32")]
        public string Password { get; set; }


        [MaxLength(32, ErrorMessage = "ConfirmPassword长度不能大于32")]
        public string ConfirmPassword { get; set; }
    }
}
