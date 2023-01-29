using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class User : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        ///// <summary>
        ///// 密码的盐（一个随机的字符串）
        ///// </summary>
        //public string Salt { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 用户昵称
        ///// </summary>

        //public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>

        public string HardImgUrl { get; set; }

        /// <summary>
        /// 头像缩略图
        /// </summary>
        public string ThumbnailHeadImgUrl { get; set; }

        /// <summary>
        /// 生日
        /// </summary>

        public DateTime? Birthday { get; set; }
       
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginTimes { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int DepartmentId { get; set; }

        ///// <summary>
        ///// 是否已删除
        ///// </summary>
        public bool IsDeleted { get; set; }

        ///// <summary>
        ///// 身份证
        ///// </summary>
        //public string IdentityCardCode { get; set; }


        ///// <summary>
        ///// 身份证背面
        ///// </summary>
        //public string IdentityCardBacktUrl { get; set; }

        ///// <summary>
        ///// 身份证正面
        ///// </summary>
        //public string IdentityCardFrontUrl { get; set; }

    }
}
