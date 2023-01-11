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
    public class ExcelExampleService : ResultService, IExcelExampleService
    {
        private readonly IExcelExampleRepository _excelExampleRepository;

        public ExcelExampleService(IExcelExampleRepository excelExampleRepository)
        {
            _excelExampleRepository = excelExampleRepository;
        }


        public async Task<bool> DeleteAsync(int id)
        {
           int res = await  _excelExampleRepository.DeleteByIdAsync(id);
            return res > 0;
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            return await _excelExampleRepository.DeleteManyAsync(ids);
        }

        public async Task<ExcelExample> DetailAsync(int id)
        {
            return await _excelExampleRepository.DetailAsync(id);
        }

        public async Task<bool> ExistAsync(ExcelExample param)
        {
            if (param.Id > 0)
            {
                return await _excelExampleRepository.ExistsAsync(r => r.IdentityCardCode == param.IdentityCardCode && r.Id != param.Id);
            }
            else
            {
                return await _excelExampleRepository.ExistsAsync(r => r.IdentityCardCode == param.IdentityCardCode);
            }


        }

        public async Task<List<ExcelExample>> GetAllListAsync()
        {
            return await _excelExampleRepository.GetAllListAsync();
        }

        public async Task<PaginatedList<ExcelExample>> GetPaginatedListAsync(string searchKeyword, int pageIndex, int pageSize)
        {
            return await _excelExampleRepository.GetPaginatedListAsync(searchKeyword, pageIndex, pageSize);
        }

        public async Task<bool> InsertOrUpdateAsync(ExcelExample param)
        {
            return await _excelExampleRepository.InsertOrUpdateAsync(param);
        }
    }
}
