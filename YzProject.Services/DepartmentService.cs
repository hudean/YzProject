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
    public class DepartmentService : ResultService, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int res =  await _departmentRepository.DeleteByIdAsync(id);
            return res > 0;
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            return await _departmentRepository.DeleteManyAsync(ids);
        }

        public async Task<DepartmentViewModel> DetailAsync(int id)
        {
            return await _departmentRepository.DetailAsync(id);
        }

        public async Task<List<DepartmentViewModel>> GetAllListAsync()
        {
            return await _departmentRepository.GetAllListAsync();
        }

        public async Task<PaginatedList<DepartmentViewModel>> GetPaginatedListAsync(string departmentName, int pageIndex, int pageSize)
        {
            return await _departmentRepository.GetPaginatedListAsync(departmentName, pageIndex, pageSize);
        }

        public async Task<bool> InsertOrUpdateAsync(ParamDepartment param)
        {
            return await _departmentRepository.InsertOrUpdateAsync(param);
        }

        public async Task<bool> ExistAsync(ParamDepartment param)
        {
            if (param.Id > 0)
            {
                return await _departmentRepository.ExistsAsync(r => r.DepartmentName == param.DepartmentName && r.Id != param.Id);
            }
            else
            {
                return await _departmentRepository.ExistsAsync(r => r.DepartmentName == param.DepartmentName);
            }

        }
    }
}
