using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.RequestModel
{
    internal class ParamAccountModel
    {
    }

    /// <summary>
    /// 注册模型
    /// </summary>
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class TokenRequestModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "The UserName field is required.")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "The Password field is required.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }
}
