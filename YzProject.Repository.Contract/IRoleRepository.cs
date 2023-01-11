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
    public interface IRoleRepository : IRepository<Role, int>
    {
        Task<bool> DeleteManyAsync(List<int> ids);
        Task<RoleViewModel> DetailAsync(int id);
        Task<List<RoleViewModel>> GetAllListAsync();
        Task<PaginatedList<RoleViewModel>> GetPaginatedListAsync(string roleName, int pageIndex, int pageSize);

        Task<bool> InsertOrUpdateAsync(ParamRole param);
        Task<List<RoleViewModel>> GetRoleListByUserIdAsync(int userId);
        Task<bool> SetRolesByUserIdAsync(int userId, List<int> roleIds);
        Task<List<RoleSetViewModel>> GetConfigurationRoleListAsync(int userId);
    }
}
