using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.RequestModel
{
    public class ParamMenuAuth
    {
        public int RoleId { get; set; }
        public int[] PermissionIds { get; set; }

    }
}
