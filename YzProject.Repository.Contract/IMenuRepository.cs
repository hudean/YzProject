using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Domain.Repositories;
using YzProject.Domain.ResultModel;


namespace YzProject.Repository.Contract
{
    public interface IMenuRepository : IRepository<Menu, int>
    {
        Task<bool> DeleteManyAsync(List<int> ids);
        Task<MenuViewModel> DetailAsync(int id);
        Task<List<MenuViewModel>> GetAllListAsync();
        Task<PaginatedList<MenuViewModel>> GetPaginatedListAsync(string menuName, int? menuType, int pageIndex, int pageSize);
        Task<bool> InsertOrUpdateAsync(ParamMenu param);
        Task<List<MenuTreeNodeViewModel>> GetMenuListAsync();
        Task<List<MenuViewModel>> GetOneLevelMenuListAsync();
        Task<List<MenuViewModel>> GetPermissionListByRoleIdAsync(int roleId);
        Task<bool> PermissionAuthorizeByRoleIdAsync(int roleId, List<int> permissionIds);
        Task<List<ConfigurationMenuTreeNodeViewModel>> GetConfigurationPermissionListAsync(int roleId);
        Task<List<MenuTreeNodeViewModel>> GetMenuListByUserIdAsync(int userId);
    }
}
