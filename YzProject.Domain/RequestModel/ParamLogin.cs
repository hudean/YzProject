using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.RequestModel
{
    public class ParamLogin
    {
        [Required(ErrorMessage = "UserName必填")]
        [MaxLength(32, ErrorMessage = "UserName长度不能大于32")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password必填")]
        [MaxLength(32, ErrorMessage = "Password长度不能大于32")]
        public string Password { get; set; }

        [Required(ErrorMessage = "ValidateCode必填")]
        [MaxLength(6, ErrorMessage = "ValidateCode长度不能大于6")]
        public string ValidateCode { get; set; }
    }
}
