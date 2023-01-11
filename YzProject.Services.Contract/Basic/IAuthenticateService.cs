using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Services.Contract
{
    /// <summary>
    /// 认证服务
    /// </summary>
    public interface IAuthenticateService
    {
        Task<string> GenerateJwtToken(string userName);
    }
}
