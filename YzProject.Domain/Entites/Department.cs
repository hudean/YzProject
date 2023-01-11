using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// 部门实体
    /// </summary>
    [Table("Department")]
    public class Department: Entity<int>, ISoftDelete, IHasCreationTime
    {
        // [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public override int Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Required]
        [MaxLength(16)]
        [Description("部门名称")]
        [Comment("部门名称")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        [Required]
        [MaxLength(16)]
        [Description("部门编号")]
        [Comment("部门编号")]
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        [MaxLength(16)]
        [Description("部门负责人")]
        [Comment("部门负责人")]
        public string DepartmentManager { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(16)]
        [Description("联系电话")]
        [Comment("联系电话")]
        public string ContactNumber { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [MaxLength(255)]
        [Description("简介")]
        [Comment("简介")]
        public string Introduction { get; set; }

        /// <summary>
        /// 父级部门ID
        /// </summary>
        [Comment("父级部门ID")]
        public int ParentId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Comment("创建人ID")]
        public int CreateUserId { get; set; }

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        [Comment("是否已删除")]
        public bool IsDeleted { get; set; } = false;


        // 下面的会在数据库建立外键，现在不推荐数据库外键，推荐使用逻辑外键

        ///// <summary>
        ///// 包含用户
        ///// </summary>
        //public virtual ICollection<User> Users { get; set; }

        ///// <summary>
        ///// 创建人信息
        ///// </summary>
        //public virtual User CreateUser { get; set; }
    }
}
