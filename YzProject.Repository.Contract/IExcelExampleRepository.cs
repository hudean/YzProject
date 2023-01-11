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
    public interface IExcelExampleRepository : IRepository<ExcelExample, int>
    {
        Task<bool> DeleteManyAsync(List<int> ids);
        Task<ExcelExample> DetailAsync(int id);
        Task<List<ExcelExample>> GetAllListAsync();
        Task<PaginatedList<ExcelExample>> GetPaginatedListAsync(string searchKeyword, int pageIndex, int pageSize);
        Task<bool> InsertOrUpdateAsync(ExcelExample param);
    }
}
