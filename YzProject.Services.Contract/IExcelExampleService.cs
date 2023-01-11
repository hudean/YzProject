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
    public interface IExcelExampleService : IResultService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        Task<List<ExcelExample>> GetAllListAsync();

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginatedList<ExcelExample>> GetPaginatedListAsync(string searchKeyword, int pageIndex, int pageSize);

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="param">实体</param>
        /// <returns></returns>
        Task<bool> InsertOrUpdateAsync(ExcelExample param);

        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        Task<bool> DeleteManyAsync(List<int> ids);

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
        Task<ExcelExample> DetailAsync(int id);

        Task<bool> ExistAsync(ExcelExample param);
    }
}
