using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.RequestModel
{
    public class ParamRole
    {
        public int Id { get; set; }

        //public string RoleCode { get; set; }

        [Required(ErrorMessage = "角色名称不能为空。")]
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

    }
    public class ParamQueryPageRole : ParamPage
    {
        public string RoleName { get; set; }
    }
}
