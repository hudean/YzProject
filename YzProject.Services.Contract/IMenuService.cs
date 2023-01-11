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
    public interface IMenuService : IResultService
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        Task<bool> DeleteManyAsync(List<int> ids);

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MenuViewModel> DetailAsync(int id);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(ParamMenu param);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        Task<List<MenuViewModel>> GetAllListAsync();

        /// <summary>
        /// 获取所有的菜单
        /// </summary>
        /// <returns></returns>
        Task<List<MenuTreeNodeViewModel>> GetMenuListAsync();
        
        /// <summary>
        /// 获取一级菜单列表
        /// </summary>
        /// <returns></returns>
        Task<List<MenuViewModel>> GetOneLevelMenuListAsync();

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="menuType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginatedList<MenuViewModel>> GetPaginatedListAsync(string menuName, int? menuType, int pageIndex, int pageSize);

       /// <summary>
       /// 添加/修改
       /// </summary>
       /// <param name="param"></param>
       /// <returns></returns>
        Task<bool> InsertOrUpdateAsync(ParamMenu param);

        /// <summary>
        /// 获取角色对应的所有权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<MenuViewModel>> GetPermissionListByRoleIdAsync(int roleId);

        /// <summary>
        /// 设置角色id对应的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        Task<bool> PermissionAuthorizeByRoleIdAsync(int roleId, List<int> permissionIds);

        /// <summary>
        /// 获取所有权限及角色对应的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<ConfigurationMenuTreeNodeViewModel>> GetConfigurationPermissionListAsync(int roleId);
        Task<List<MenuTreeNodeViewModel>> GetMenuListByUserIdAsync(int userId);
    }
}
