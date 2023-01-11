using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    /// <summary>
    /// 功能菜单实体
    /// </summary>
    public class Menu : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单编码
        /// </summary>
        public string MenuCode { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 类型：0导航菜单；1操作按钮。
        /// </summary>
        //public EnumMenuType MenuType { get; set; }
        public int MenuType { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 菜单备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    //public enum EnumMenuType : int
    //{
    //    /// <summary>
    //    /// 导航菜单
    //    /// </summary>
    //    [Description("导航菜单")]
    //    Navigation = 0,
    //    /// <summary>
    //    /// 操作按钮
    //    /// </summary>
    //    [Description("操作按钮")]
    //    Button = 1
    //}

}
