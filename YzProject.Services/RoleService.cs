using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Domain.ResultModel;
using YzProject.Repository.Contract;
using YzProject.Services.Contract;

namespace YzProject.Services
{
    public class RoleService : ResultService, IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int res = await _roleRepository.DeleteByIdAsync(id);
            return res > 0;
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            return await _roleRepository.DeleteManyAsync(ids);
        }

        public async Task<RoleViewModel> DetailAsync(int id)
        {
            return await _roleRepository.DetailAsync(id);
        }

        public async Task<List<RoleViewModel>> GetAllListAsync()
        {
            return await _roleRepository.GetAllListAsync();
        }

        public async Task<PaginatedList<RoleViewModel>> GetPaginatedListAsync(string roleName,int pageIndex, int pageSize)
        {
            return await _roleRepository.GetPaginatedListAsync(roleName,pageIndex, pageSize);
        }

        public async Task<bool> InsertOrUpdateAsync(ParamRole param)
        {
            return await _roleRepository.InsertOrUpdateAsync(param);
        }

        public async Task<bool> ExistAsync(ParamRole param)
        {
            if (param.Id > 0)
            {
                //return await _roleRepository.AnyAsync(r => (r.RoleCode == param.RoleCode || r.RoleName == param.RoleName) && r.Id != param.Id);
                return await _roleRepository.ExistsAsync(r =>  r.RoleName == param.RoleName && r.Id != param.Id);
            }
            else
            {
                //return await _roleRepository.AnyAsync(r => r.RoleCode == param.RoleCode || r.RoleName == param.RoleName);
                return await _roleRepository.ExistsAsync(r => r.RoleName == param.RoleName);
            }
        }

        public async Task<List<RoleViewModel>> GetRoleListByUserIdAsync(int userId)
        {
            return await _roleRepository.GetRoleListByUserIdAsync(userId);
        }

        public async Task<bool> SetRolesByUserIdAsync(int userId, List<int> roleIds)
        {
            return await _roleRepository.SetRolesByUserIdAsync(userId, roleIds);
        }

        public async Task<List<RoleSetViewModel>> GetConfigurationRoleListAsync(int userId)
        {
            return await _roleRepository.GetConfigurationRoleListAsync(userId);
        }
    }
}
