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
    public interface IRoleService : IResultService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        Task<List<RoleViewModel>> GetAllListAsync();

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginatedList<RoleViewModel>> GetPaginatedListAsync(string roleName,int pageIndex, int pageSize);

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="param">实体</param>
        /// <returns></returns>
        Task<bool> InsertOrUpdateAsync(ParamRole param);

        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        Task<bool> DeleteManyAsync(List<int> ids);
        Task<bool> ExistAsync(ParamRole param);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">Id</param>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        Task<RoleViewModel> DetailAsync(int id);

        Task<List<RoleViewModel>> GetRoleListByUserIdAsync(int userId);
        Task<bool> SetRolesByUserIdAsync(int userId, List<int> roleIds);
        Task<List<RoleSetViewModel>> GetConfigurationRoleListAsync(int userId);
    }
}
