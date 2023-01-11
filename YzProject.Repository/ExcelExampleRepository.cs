using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzGraduationProject.Common;
using YzProject.Domain;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Domain.ResultModel;
using YzProject.Domain.Repositories;
using YzProject.Repository.Contract;

namespace YzProject.Repository
{
    public class ExcelExampleRepository : Repository<ExcelExample, int>, IExcelExampleRepository
    {
        public ExcelExampleRepository(YzProjectContext context) : base(context)
        {
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            var entites = await _dbContext.ExcelExamples.Where(x => ids.Contains(x.Id)).ToListAsync();
            _dbContext.ExcelExamples.RemoveRange(entites);
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<ExcelExample> DetailAsync(int id)
        {
            //var data = await (from r in _dbContext.ExcelExamples.Where(x => x.Id == id)
            //                  select r).FirstOrDefaultAsync();
            var data = await _dbContext.ExcelExamples.Where(x => x.Id == id) .FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<ExcelExample>> GetAllListAsync()
        {
            //var query = from r in _dbContext.ExcelExamples
            //            orderby r.Id
            //            select r;
            //return await query.ToListAsync();
            var list = await  _dbContext.ExcelExamples.OrderBy(x => x.Id).ToListAsync();
                       
            return list;
        }

        public async Task<PaginatedList<ExcelExample>> GetPaginatedListAsync(string searchKeyword, int pageIndex, int pageSize)
        {
            var query = from r in _dbContext.ExcelExamples.WhereIf(!string.IsNullOrEmpty(searchKeyword), r => r.Name.Contains(searchKeyword) || r.IdentityCardCode.Contains(searchKeyword))
                        orderby r.Id
                        select r;
            int count = await query.CountAsync();
            var list = new List<ExcelExample>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<ExcelExample>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertOrUpdateAsync(ExcelExample param)
        {
            if (param.Id > 0)
            {
                var excelExample = await _dbContext.ExcelExamples.Where(r => r.Id == param.Id).FirstOrDefaultAsync();
                if (excelExample == null)
                {
                    return false;
                }
                excelExample.Name = param.Name;
                excelExample.Description = param.Description;
                excelExample.IdentityCardCode = param.IdentityCardCode;
                excelExample.Age = param.Age;
                _dbContext.ExcelExamples.Update(excelExample);
            }
            else
            {
                var excelExample = new ExcelExample()
                {
                    Name = param.Name,
                    Description = param.Description,
                    IdentityCardCode = param.IdentityCardCode,
                    Age = param.Age,
                    CreateTime = DateTime.Now
            }; 
                await _dbContext.ExcelExamples.AddAsync(excelExample);
            }
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;

        }
    }
}
