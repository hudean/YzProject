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
    public class MenuService : ResultService, IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int res = await _menuRepository.DeleteByIdAsync(id);
            return res > 0;
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            return await _menuRepository.DeleteManyAsync(ids);
        }

        public async Task<MenuViewModel> DetailAsync(int id)
        {
            return await _menuRepository.DetailAsync(id);
        }

        public async Task<bool> ExistAsync(ParamMenu param)
        {
            if (param.Id > 0)
            {
                return await _menuRepository.ExistsAsync(r => (r.MenuCode == param.MenuCode || r.MenuName == param.MenuName) && r.Id != param.Id);
            }
            else
            {
                return await _menuRepository.ExistsAsync(r => r.MenuCode == param.MenuCode || r.MenuName == param.MenuName);
            }
        }

        public async Task<List<MenuViewModel>> GetAllListAsync()
        {
            return await _menuRepository.GetAllListAsync();
        }

        public async Task<List<MenuTreeNodeViewModel>> GetMenuListAsync()
        {
            return await _menuRepository.GetMenuListAsync();
        }

        public async Task<PaginatedList<MenuViewModel>> GetPaginatedListAsync(string menuName, int? menuType, int pageIndex, int pageSize)
        {
            return await _menuRepository.GetPaginatedListAsync(menuName, menuType, pageIndex, pageSize);
        }

        public async Task<List<MenuViewModel>> GetOneLevelMenuListAsync()
        {
            return await _menuRepository.GetOneLevelMenuListAsync();
        }

        public async Task<bool> InsertOrUpdateAsync(ParamMenu param)
        {
            return await _menuRepository.InsertOrUpdateAsync(param);
        }

        public async Task<List<MenuViewModel>> GetPermissionListByRoleIdAsync(int roleId)
        {
            return await _menuRepository.GetPermissionListByRoleIdAsync(roleId);
        }

        public async Task<bool> PermissionAuthorizeByRoleIdAsync(int roleId, List<int> permissionIds)
        {
            return await _menuRepository.PermissionAuthorizeByRoleIdAsync(roleId, permissionIds);
        }

        public async Task<List<ConfigurationMenuTreeNodeViewModel>> GetConfigurationPermissionListAsync(int roleId)
        {
            return await _menuRepository.GetConfigurationPermissionListAsync(roleId);
        }

        public async Task<List<MenuTreeNodeViewModel>> GetMenuListByUserIdAsync(int userId)
        {
            return await _menuRepository.GetMenuListByUserIdAsync(userId);
        }
    }
}
