using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.ResultModel;

namespace YzProject.Domain.RequestModel
{
    public class ParamMenu
    {
        public int Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; } = 0;

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
    }


    public class ParamQueryPageMenu : ParamPage
    {
       public string MenuName { get; set; }

        public int? MenuType { get; set; }
    }
}
