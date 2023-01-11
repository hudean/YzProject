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
    public class RoleRepository : Repository<Role, int>, IRoleRepository
    {
        public RoleRepository(YzProjectContext context) : base(context)
        {
        }


        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            var entites = await _dbContext.Roles.Where(x => ids.Contains(x.Id)).ToListAsync();
            _dbContext.Roles.RemoveRange(entites);
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<RoleViewModel> DetailAsync(int id)
        {
            var data = await (from r in _dbContext.Roles.Where(x => x.Id == id)
                              select new RoleViewModel
                              {
                                  Id = r.Id,
                                  RoleName = r.RoleName,
                                  RoleDescription = r.RoleDescription,
                                  //RoleCode = r.RoleCode,
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<RoleViewModel>> GetAllListAsync()
        {
            var query = from r in _dbContext.Roles
                        select new RoleViewModel
                        {
                            Id = r.Id,
                            RoleName = r.RoleName,
                            RoleDescription = r.RoleDescription,
                            //RoleCode = r.RoleCode,
                        };
            return await query.ToListAsync();
        }

        public async Task<List<RoleSetViewModel>> GetConfigurationRoleListAsync(int userId)
        {
            var data = await(from r in _dbContext.Roles
                             select new RoleSetViewModel
                             {
                                 Id = r.Id,
                                 RoleName = r.RoleName,
                                 RoleDescription = r.RoleDescription,
                             }).ToListAsync();
            var ids = (await(from r in _dbContext.Roles
                            join ur in _dbContext.UserRoles.Where(r => r.UserId == userId) on r.Id equals ur.RoleId
                            select new
                            {
                                Id = r.Id
                            }).ToListAsync()).Distinct();
            if (ids != null)
            {
                foreach (var item in data)
                {
                    if (ids.Any(r => r.Id == item.Id))
                    {
                        item.IsChecked = true;
                    }
                }
            }
            return data;
        }

        public async Task<PaginatedList<RoleViewModel>> GetPaginatedListAsync(string roleName, int pageIndex, int pageSize)
        {
            var query = from r in _dbContext.Roles.WhereIf(!string.IsNullOrEmpty(roleName), r => r.RoleName.Contains(roleName))
                        select new RoleViewModel()
                        {
                            Id = r.Id,
                            RoleName = r.RoleName,
                            RoleDescription = r.RoleDescription,
                            //RoleCode = r.RoleCode,
                        };
            int count = await query.CountAsync();
            var list = new List<RoleViewModel>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<RoleViewModel>(list, count, pageIndex, pageSize);
        }

        public async Task<List<RoleViewModel>> GetRoleListByUserIdAsync(int userId)
        {
            var query = from r in _dbContext.Roles
                        join ur in _dbContext.UserRoles.Where(r => r.UserId == userId) on r.Id equals ur.RoleId
                        select new RoleViewModel
                        {
                            Id = r.Id,
                            RoleName = r.RoleName,
                            RoleDescription = r.RoleDescription,
                        };
            return await query.ToListAsync();
        }

        public async Task<bool> InsertOrUpdateAsync(ParamRole param)
        {
            if (param.Id > 0)
            {
                var role = await _dbContext.Roles.Where(r => r.Id == param.Id).FirstOrDefaultAsync();
                if (role == null)
                {
                    return false;
                }
                role.RoleDescription = param.RoleDescription;
                //role.RoleCode = param.RoleCode;
                role.RoleName = param.RoleName;
                _dbContext.Roles.Update(role);
            }
            else
            {
                var role = new Role()
                {
                    RoleName = param.RoleName,
                    //RoleCode = param.RoleCode,
                    RoleDescription = param.RoleDescription,
                };
                await _dbContext.Roles.AddAsync(role);
            }
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<bool> SetRolesByUserIdAsync(int userId, List<int> roleIds)
        {
            var list = await _dbContext.UserRoles.Where(r => r.UserId == userId).ToListAsync();
            _dbContext.UserRoles.RemoveRange(list);
            await _dbContext.SaveChangesAsync();
            //批量插入
            if (roleIds?.Any() ?? false)
            {
                List<UserRole> userRoles = new List<UserRole>();
                foreach (var id in roleIds)
                {
                    var model = new UserRole()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        RoleId = id
                    };
                    userRoles.Add(model);
                }
                await _dbContext.UserRoles.AddRangeAsync(userRoles);
                int res = await _dbContext.SaveChangesAsync();
                return res > 0;
            }
            return false;
        }

        //public async Task<bool> UpdateRoleMenuAsync(int roleId, List<RoleMenu> roleMenus)
        //{
        //    var oldDatas = await _dbContext.RoleMenus.Where(it => it.RoleId == roleId).ToListAsync();
        //    oldDatas.ForEach(it => _dbContext.RoleMenus.Remove(it));
        //    await _dbContext.SaveChangesAsync();
        //    await _dbContext.RoleMenus.AddRangeAsync(roleMenus);
        //    int res = await _dbContext.SaveChangesAsync();
        //    return res > 0;
        //}
    }
}
