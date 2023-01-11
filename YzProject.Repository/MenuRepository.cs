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
using System.ComponentModel;

namespace YzProject.Repository
{
    public class MenuRepository : Repository<Menu, int>, IMenuRepository
    {
        public MenuRepository(YzProjectContext context) : base(context)
        {
        }

        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            var entites = await _dbContext.Menus.Where(x => ids.Contains(x.Id)).ToListAsync();
            _dbContext.Menus.RemoveRange(entites);
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<MenuViewModel> DetailAsync(int id)
        {
            var data = await (from r in _dbContext.Menus.Where(x => x.Id == id)
                              select new MenuViewModel
                              {
                                  Id = r.Id,
                                  ParentId = r.ParentId,
                                  SerialNumber = r.SerialNumber,
                                  MenuName = r.MenuName,
                                  MenuCode = r.MenuCode,
                                  Url = r.Url,
                                  MenuType = (EnumMenuType)r.MenuType,
                                  Icon = r.Icon,
                                  Remarks = r.Remarks,
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<MenuViewModel>> GetAllListAsync()
        {
            var query = from r in _dbContext.Menus
                        orderby r.SerialNumber
                        select new MenuViewModel
                        {
                            Id = r.Id,
                            ParentId = r.ParentId,
                            SerialNumber = r.SerialNumber,
                            MenuName = r.MenuName,
                            MenuCode = r.MenuCode,
                            Url = r.Url,
                            MenuType = (EnumMenuType)r.MenuType,
                            Icon = r.Icon,
                            Remarks = r.Remarks,
                        };
            return await query.ToListAsync();
        }

        public async Task<List<MenuTreeNodeViewModel>> GetMenuListAsync()
        {
            var data = await (from r in _dbContext.Menus.Where(r => r.MenuType == 0)
                              orderby r.SerialNumber
                              select new MenuViewModel
                              {
                                  Id = r.Id,
                                  ParentId = r.ParentId,
                                  SerialNumber = r.SerialNumber,
                                  MenuName = r.MenuName,
                                  MenuCode = r.MenuCode,
                                  Url = r.Url,
                                  MenuType = (EnumMenuType)r.MenuType,
                                  Icon = r.Icon,
                                  Remarks = r.Remarks,
                              }).ToListAsync();
            //获取所有1级菜单
            //var firstMenus = data.Where(i => i.ParentId == 0).ToList();
            //List<MenuTreeViewModel> menuList = new List<MenuTreeViewModel>();
            //foreach (var item in firstMenus)
            //{
            //    menuList.Add(new MenuTreeViewModel()
            //    {
            //        FirstMenu = item,
            //        ChildMenus = data.Where(i => i.ParentId == item.Id).ToList()
            //    }) ;
            //}
            var menuList = GetMenuTreeByMenuList(data);
            return menuList;
        }

        /// <summary>
        /// 集合根据父节点id递归获取一个树
        /// </summary>
        /// <param name="list">节点集合</param>
        /// <param name="parentId">父节点id</param>
        /// <returns></returns>
        private List<MenuTreeNodeViewModel> GetMenuTreeByMenuList(List<MenuViewModel> list, int parentId = 0)
        {
            List<MenuTreeNodeViewModel> menuList = new List<MenuTreeNodeViewModel>();
            var menus = list.Where(i => i.ParentId == parentId).ToList();
            //list集合移除menus集合
            menus.ForEach(r => { list.Remove(r); });
            foreach (var item in menus)
            {
                //可以把FirstMenu里所有属性放到基类里
                menuList.Add(new MenuTreeNodeViewModel()
                {
                    FirstMenu = item,
                    ChildMenus = GetMenuTreeByMenuList(list, item.Id)
                });
            }
            return menuList;
        }

        public async Task<List<MenuViewModel>> GetOneLevelMenuListAsync()
        {
            var data = await (from r in _dbContext.Menus.Where(r => r.MenuType == 0 && r.ParentId == 0)
                              orderby r.SerialNumber
                              select new MenuViewModel
                              {
                                  Id = r.Id,
                                  ParentId = r.ParentId,
                                  SerialNumber = r.SerialNumber,
                                  MenuName = r.MenuName,
                                  MenuCode = r.MenuCode,
                                  Url = r.Url,
                                  MenuType = (EnumMenuType)r.MenuType,
                                  Icon = r.Icon,
                                  Remarks = r.Remarks,
                              }).ToListAsync();
            return data;
        }

        public async Task<PaginatedList<MenuViewModel>> GetPaginatedListAsync(string menuName, int? menuType, int pageIndex, int pageSize)
        {
            var query = from r in _dbContext.Menus.WhereIf(!string.IsNullOrEmpty(menuName), r => r.MenuName.Contains(menuName))
                        .WhereIf(menuType != null, r => r.MenuType == menuType)
                        orderby r.SerialNumber
                        select new MenuViewModel()
                        {
                            Id = r.Id,
                            ParentId = r.ParentId,
                            SerialNumber = r.SerialNumber,
                            MenuName = r.MenuName,
                            MenuCode = r.MenuCode,
                            Url = r.Url,
                            MenuType = (EnumMenuType)r.MenuType,
                            Icon = r.Icon,
                            Remarks = r.Remarks,
                        };
            int count = await query.CountAsync();
            var list = new List<MenuViewModel>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<MenuViewModel>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertOrUpdateAsync(ParamMenu param)
        {
            if (param.Id > 0)
            {
                var model = await _dbContext.Menus.Where(r => r.Id == param.Id).FirstOrDefaultAsync();
                if (model == null)
                {
                    return false;
                }
                model.ParentId = param.ParentId;
                model.SerialNumber = param.SerialNumber;
                model.MenuName = param.MenuName;
                model.MenuCode = param.MenuCode;
                model.Url = param.Url;
                model.MenuType = (int)param.MenuType;
                model.Icon = param.Icon;
                model.Remarks = param.Remarks;
                _dbContext.Menus.Update(model);
            }
            else
            {
                var model = new Menu()
                {
                    ParentId = param.ParentId,
                    SerialNumber = param.SerialNumber,
                    MenuName = param.MenuName,
                    MenuCode = param.MenuCode,
                    Url = param.Url,
                    MenuType = (int)param.MenuType,
                    Icon = param.Icon,
                    Remarks = param.Remarks,
                };
                await _dbContext.Menus.AddAsync(model);
            }
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<List<MenuViewModel>> GetPermissionListByRoleIdAsync(int roleId)
        {
            var query = from m in _dbContext.Menus
                        join r in _dbContext.RoleMenus.Where(r => r.RoleId == roleId) on m.Id equals r.MenuId
                        orderby m.SerialNumber
                        select new MenuViewModel
                        {
                            Id = m.Id,
                            ParentId = m.ParentId,
                            SerialNumber = m.SerialNumber,
                            MenuName = m.MenuName,
                            MenuCode = m.MenuCode,
                            Url = m.Url,
                            MenuType = (EnumMenuType)m.MenuType,
                            Icon = m.Icon,
                            Remarks = m.Remarks,
                        };
            return await query.ToListAsync();
        }

        public async Task<bool> PermissionAuthorizeByRoleIdAsync(int roleId, List<int> permissionIds)
        {
            var list = await _dbContext.RoleMenus.Where(r => r.RoleId == roleId).ToListAsync();
            _dbContext.RoleMenus.RemoveRange(list);
            await _dbContext.SaveChangesAsync();
            //批量插入
            if (permissionIds?.Any() ?? false)
            {
                List<RoleMenu> roleMenus = new List<RoleMenu>();
                foreach (var id in permissionIds)
                {
                    var model = new RoleMenu()
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        MenuId = id
                    };
                    roleMenus.Add(model);
                }
                await _dbContext.RoleMenus.AddRangeAsync(roleMenus);
                int res = await _dbContext.SaveChangesAsync();
                return res > 0;
            }
            return false;
        }

        public async Task<List<ConfigurationMenuTreeNodeViewModel>> GetConfigurationPermissionListAsync(int roleId)
        {
            //throw new NotImplementedException();
            var data = await (from r in _dbContext.Menus//.Where(r => r.MenuType == 0)
                              orderby r.SerialNumber
                              select new MenuViewModel
                              {
                                  Id = r.Id,
                                  ParentId = r.ParentId,
                                  SerialNumber = r.SerialNumber,
                                  MenuName = r.MenuName,
                                  MenuCode = r.MenuCode,
                                  Url = r.Url,
                                  MenuType = (EnumMenuType)r.MenuType,
                                  Icon = r.Icon,
                                  Remarks = r.Remarks,
                              }).ToListAsync();
            var ids = await (from m in _dbContext.Menus
                             join rm in _dbContext.RoleMenus.Where(r => r.RoleId == roleId) on m.Id equals rm.MenuId
                             select new
                             {
                                 Id = m.Id
                             }).ToListAsync();
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

            var menuList = GetMenuTreeByConfigurationMenuList(data);
            return menuList;
        }


        /// <summary>
        /// 集合根据父节点id递归获取一个树
        /// </summary>
        /// <param name="list">节点集合</param>
        /// <param name="parentId">父节点id</param>
        /// <returns></returns>
        private List<ConfigurationMenuTreeNodeViewModel> GetMenuTreeByConfigurationMenuList(List<MenuViewModel> list, int parentId = 0)
        {
            List<ConfigurationMenuTreeNodeViewModel> menuList = new List<ConfigurationMenuTreeNodeViewModel>();
            var menus = list.Where(i => i.ParentId == parentId).ToList();
            //list集合移除menus集合
            menus.ForEach(r => { list.Remove(r); });
            foreach (var item in menus)
            {
                //可以把FirstMenu里所有属性放到基类里
                menuList.Add(new ConfigurationMenuTreeNodeViewModel()
                {
                    Title = item.MenuName,
                    Field = "",
                    Checked = item.IsChecked,
                    Disabled = false,
                    Id = item.Id,
                    Spread = true,
                    Children = GetMenuTreeByConfigurationMenuList(list, item.Id)
                });

            }
            return menuList;
        }

        public async Task<List<MenuTreeNodeViewModel>> GetMenuListByUserIdAsync(int userId)
        {
            var data = await (from m in _dbContext.Menus.Where(r => r.MenuType == 0)
                              join rm in _dbContext.RoleMenus on m.Id equals rm.MenuId
                              join r in _dbContext.Roles on rm.RoleId equals r.Id
                              join ur in _dbContext.UserRoles on r.Id equals ur.RoleId
                              join u in _dbContext.Users.Where(r=>r.Id == userId) on ur.UserId equals u.Id
                              orderby m.SerialNumber
                              select new MenuViewModel
                              {
                                  Id = m.Id,
                                  ParentId = m.ParentId,
                                  SerialNumber = m.SerialNumber,
                                  MenuName = m.MenuName,
                                  MenuCode = m.MenuCode,
                                  Url = m.Url,
                                  MenuType = (EnumMenuType)m.MenuType,
                                  Icon = m.Icon,
                                  Remarks = m.Remarks,
                              }).ToListAsync();
            var menuList = GetMenuTreeByMenuList(data);
            return menuList;
        }
    }
}
