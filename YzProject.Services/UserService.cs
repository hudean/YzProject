using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzGraduationProject.Common;
using YzProject.Domain;
using YzProject.Domain.Configs;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Domain.ResultModel;
using YzProject.Repository.Contract;
using YzProject.Services.Contract;

namespace YzProject.Services
{
    public class UserService : ResultService, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            return await _userRepository.DeleteManyAsync(ids);
        }

        public async Task<UserViewModel> DetailAsync(int id)
        {
            return await _userRepository.DetailAsync(id);
        }
        public async Task<bool> ExistAsync(ParamUser param)
        {
            if (param.Id > 0)
            {
                return await _userRepository.ExistsAsync(r => r.UserName == param.UserName && r.Id != param.Id);
            }
            else
            {
                return await _userRepository.ExistsAsync(r => r.UserName == param.UserName);
            }

        }

        public async Task<List<UserViewModel>> GetAllListAsync()
        {
            return await _userRepository.GetAllListAsync();
        }

        public async Task<PaginatedList<UserViewModel>> GetPaginatedListAsync(string userName, int pageIndex, int pageSize)
        {
            return await _userRepository.GetPaginatedListAsync(userName,pageIndex, pageSize);
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _userRepository.GetUserByUserNameAsync(userName);
            //_userRepository.Get(x=>x.UserName==userName).FirstOrDefault();

        }

        

        public async Task<User> InsertOrUpdateAsync(ParamUser param)
        {
            return await _userRepository.InsertOrUpdateAsync(param);
        }

        public async Task<bool> EditPassWordAsync(ParamUserPassword param)
        {
            var model = await _userRepository.FindAsync(param.Id);
            if (model != null)
            {
                string password = Encrypt.Md5Encrypt(param.Password);
                model.Password = password;
                await _userRepository.UpdateAsync(model);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int res = await _userRepository.DeleteByIdAsync(id);
            return res > 0;
        }

        //public async Task<bool> AddUserAsync(string userName)
        //{
        //    return await _userRepository.AddUserAsync(userName);
        //}

        #region jwt

        /// <summary>
        /// 异步获取token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
        {
            return await _userRepository.GetTokenAsync(model);
        }


        /// <summary>
        /// 异步刷新token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AuthenticationModel> RefreshTokenAsync(string token)
        {
            return await _userRepository.RefreshTokenAsync(token);
        }

        /// <summary>
        /// 异步撤销令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> RevokeTokenAsync(string token)
        {
            return await _userRepository.RevokeTokenAsync(token);
        }

        #endregion

        public async Task<User> GetUserAsync(int id)
        {
            return await _userRepository.FindAsync(id);
        }

        public async Task<bool> EditUserAsync(User user)
        {
             await _userRepository.UpdateAsync(user, true);
            return true;
        }

    }
}
