using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain.ResultModel
{
    public class MenuViewModel
    {
        public int Id { get; set; }

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
        public EnumMenuType MenuType { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 菜单备注
        /// </summary>
        public string Remarks { get; set; }


        public bool IsChecked { get; set; }

    }

    public enum EnumMenuType
    {
        /// <summary>
        /// 导航菜单
        /// </summary>
        [Description("导航菜单")]
        Navigation = 0,
        /// <summary>
        /// 操作按钮
        /// </summary>
        [Description("操作按钮")]
        Button = 1
    }

    /// <summary>
    /// 菜单树模型树（推荐两级）
    /// </summary>
    public class MenuTreeNodeViewModel
    { 
        /// <summary>
        /// 一级菜单
        /// </summary>
        public MenuViewModel FirstMenu { get; set; }

        ///// <summary>
        ///// 二级子菜单
        ///// </summary>
        //public List<MenuViewModel> ChildMenus { get; set; }

        /// <summary>
        /// 多级的子菜单
        /// </summary>
        public List<MenuTreeNodeViewModel> ChildMenus { get; set; }
    }

    /// <summary>
    /// 配置权限页面的菜单树模型树（推荐两级）
    /// </summary>
    public class ConfigurationMenuTreeNodeViewModel
    {
        /// <summary>
        /// 节点标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 节点字段名
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 节点唯一索引值，用于对指定节点进行各类操作
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 节点是否初始展开，默认 false
        /// </summary>
        public bool Spread { get; set; }

        /// <summary>
        /// 节点是否初始为选中状态（如果开启复选框的话），默认 false
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 节点是否为禁用状态。默认 false
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 子节点。支持设定选项同父节点
        /// </summary>
        public List<ConfigurationMenuTreeNodeViewModel> Children { get; set; }
    }
}
