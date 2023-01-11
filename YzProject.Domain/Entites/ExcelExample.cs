using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// excel导入导出示例实体类
    /// </summary>
    [Table("ExcelExample")]
    public class ExcelExample : Entity<int>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(16)]
        //[Comment("状态（1正常 2停用 3删除）")] //using Microsoft.EntityFrameworkCore;
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        //[StringLength(maximumLength = 50, MinimumLength = 3)] // 名称的长度不能超过 50，不能小于 3 这个不行
        [MaxLength(18)]
        [MinLength(15)]
        [Required]
        public string IdentityCardCode { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Range(0,150)]

        public int Age { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
