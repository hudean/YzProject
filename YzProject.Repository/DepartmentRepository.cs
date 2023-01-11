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
    public class DepartmentRepository : Repository<Department, int>, IDepartmentRepository
    {
        public DepartmentRepository(YzProjectContext context) : base(context)
        {
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            var entites = await _dbContext.Departments.Where(x => ids.Contains(x.Id)).ToListAsync();
            _dbContext.Departments.RemoveRange(entites);
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<DepartmentViewModel> DetailAsync(int id)
        {
            var data = await (from r in _dbContext.Departments.Where(x => x.Id == id)
                              select new DepartmentViewModel
                              {
                                  Id = r.Id,
                                  CreateTime = r.CreationTime,
                                  CreateUserId = r.CreateUserId,
                                  ContactNumber = r.ContactNumber,
                                  DepartmentCode = r.DepartmentCode,
                                  DepartmentManager = r.DepartmentManager,
                                  DepartmentName = r.DepartmentName,
                                  ParentId = r.ParentId,
                                  Introduction = r.Introduction,
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<DepartmentViewModel>> GetAllListAsync()
        {
            var query = from r in _dbContext.Departments
                        orderby r.Id
                        select new DepartmentViewModel
                        {
                            Id = r.Id,
                            CreateTime = r.CreationTime,
                            CreateUserId = r.CreateUserId,
                            ContactNumber = r.ContactNumber,
                            DepartmentCode = r.DepartmentCode,
                            DepartmentManager = r.DepartmentManager,
                            DepartmentName = r.DepartmentName,
                            ParentId = r.ParentId,
                            Introduction = r.Introduction,
                        };
            return await query.ToListAsync();
        }

        public async Task<PaginatedList<DepartmentViewModel>> GetPaginatedListAsync(string departmentName, int pageIndex, int pageSize)
        {
            var query = from r in _dbContext.Departments.WhereIf(!string.IsNullOrEmpty(departmentName),r=>r.DepartmentName.Contains(departmentName))
                        orderby r.Id
                        select new DepartmentViewModel()
                        {
                            Id = r.Id,
                            CreateTime = r.CreationTime,
                            CreateUserId = r.CreateUserId,
                            ContactNumber = r.ContactNumber,
                            DepartmentCode = r.DepartmentCode,
                            DepartmentManager = r.DepartmentManager,
                            DepartmentName = r.DepartmentName,
                            ParentId = r.ParentId,
                            Introduction = r.Introduction,
                        };
            int count = await query.CountAsync();
            var list = new List<DepartmentViewModel>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<DepartmentViewModel>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertOrUpdateAsync(ParamDepartment param)
        {
            if (param.Id > 0)
            {
                var department = await _dbContext.Departments.Where(r => r.Id == param.Id).FirstOrDefaultAsync();
                if (department == null)
                {
                    return false;
                }
                department.ParentId = param.ParentId;
                department.DepartmentCode = param.DepartmentCode;
                department.DepartmentName = param.DepartmentName;
                department.DepartmentManager = param.DepartmentManager;
                department.CreateUserId = param.CreateUserId;
                department.ContactNumber = param.ContactNumber;
                department.Introduction = param.Introduction;
                _dbContext.Departments.Update(department);
            }
            else
            {
                var department = new Department()
                {
                    ParentId = param.ParentId,
                    DepartmentCode = param.DepartmentCode,
                    DepartmentName = param.DepartmentName,
                    DepartmentManager = param.DepartmentManager,
                    CreateUserId = param.CreateUserId,
                    ContactNumber = param.ContactNumber,
                    //CreateTime = DateTime.Now,
                    IsDeleted = false,
                    Introduction = param.Introduction,
                };
                await _dbContext.Departments.AddAsync(department);
            }
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }
    }
}
