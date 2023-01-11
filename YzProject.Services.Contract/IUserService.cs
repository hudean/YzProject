using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Domain.ResultModel;

namespace YzProject.Services.Contract
{
    public interface IUserService : IResultService
    {
        //Task<bool> AddUserAsync(string userName);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteManyAsync(List<int> ids);
        Task<UserViewModel> DetailAsync(int id);

        Task<bool> ExistAsync(ParamUser param);
        Task<List<UserViewModel>> GetAllListAsync();
        Task<PaginatedList<UserViewModel>> GetPaginatedListAsync(string userName, int pageIndex, int pageSize);
        Task<bool> InsertOrUpdateAsync(ParamUser param);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<bool> EditPassWordAsync(ParamUserPassword param);

        #region jwt

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<AuthenticationModel> RefreshTokenAsync(string token);

        /// <summary>
        /// 撤销令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> RevokeTokenAsync(string token);
        #endregion

    }
}
