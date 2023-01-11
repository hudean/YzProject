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
    public interface IDepartmentRepository : IRepository<Department,int>
    {
        Task<bool> DeleteManyAsync(List<int> ids);
        Task<DepartmentViewModel> DetailAsync(int id);
        Task<List<DepartmentViewModel>> GetAllListAsync();
        Task<PaginatedList<DepartmentViewModel>> GetPaginatedListAsync(string departmentName, int pageIndex, int pageSize);
        Task<bool> InsertOrUpdateAsync(ParamDepartment param);
    }
}
