using System.Linq;
using System;
using YzProject.Domain;
using YzProject.Domain.Entites;
using YzGraduationProject.Common;

namespace YzProject.WebAPI
{
    /// <summary>
    /// 设定数据库种子
    /// </summary>
    public static class DbInitializer
    {
        // https://docs.microsoft.com/zh-cn/ef/core/modeling/data-seeding
        // https://docs.microsoft.com/zh-cn/aspnet/core/data/ef-rp/intro?view=aspnetcore-5.0&tabs=visual-studio
        // 建议第一次初始化数据种子，后面都采用迁移命令

        public static void Initialize(YzProjectContext context)
        {
            if (!context.Departments.Any())
            {
                var departments = new Department[]
              {
                new Department{ Id=1,DepartmentName="技术部", ContactNumber="15100005879", CreateUserId=0, DepartmentCode="001", DepartmentManager="张三", ParentId=0,Introduction="这是技术研发部门"},
                new Department{ Id=2,DepartmentName="综合部", ContactNumber="15189705879", CreateUserId=0, DepartmentCode="002", DepartmentManager="李四", ParentId=0,Introduction="这是人事后勤等综合服务部门"},
                new Department{ Id=3,DepartmentName="财务部", ContactNumber="15889705879", CreateUserId=0, DepartmentCode="003", DepartmentManager="王五", ParentId=0,Introduction="这是财务部门" }
              };
                context.Departments.AddRange(departments);
                context.SaveChanges();
            }

            if (!context.Menus.Any())
            {
                var menus = new Menu[]
              {
                  new Menu{ Id=1, MenuName="首页", ParentId=0, Icon="", MenuCode="home.index", MenuType=0,  SerialNumber=100, Url="/Home/Index" },
                  new Menu{ Id=2, MenuName="系统管理", ParentId=0, Icon="", MenuCode="系统管理", MenuType=0,  SerialNumber=200, Url="" },
                  new Menu{ Id=3, MenuName="部门管理", ParentId=2, Icon="", MenuCode="部门管理", MenuType=0,  SerialNumber=201, Url="/Department/Index" },
                  new Menu{ Id=4, MenuName="菜单管理", ParentId=2, Icon="", MenuCode="menu.", MenuType=0,  SerialNumber=202, Url="/Menu/Index" },
                  new Menu{ Id=5, MenuName="角色管理", ParentId=2, Icon="", MenuCode="角色管理", MenuType=0,  SerialNumber=203, Url="/Role/Index" },
                  new Menu{ Id=6, MenuName="用户管理", ParentId=2, Icon="", MenuCode="用户管理", MenuType=0,  SerialNumber=204, Url="/User/Index" },
              };
                context.Menus.AddRange(menus);
                context.SaveChanges();
            }

            if (!context.Roles.Any())
            {
                var roles = new Role[]
              {
                   new Role{ Id=1,RoleName="管理员", RoleDescription = "系统管理员"},
                   new Role{ Id=2,RoleName="普通用户", RoleDescription ="普通用户"},
              };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            if (!context.RoleMenus.Any())
            {
                var roleMenus = new RoleMenu[]
                {
                   new RoleMenu{ RoleId=1, MenuId = 1},
                   new RoleMenu{ RoleId=1, MenuId = 2},
                   new RoleMenu{ RoleId=1, MenuId = 3},
                   new RoleMenu{ RoleId=1, MenuId = 4},
                   new RoleMenu{ RoleId=1, MenuId = 5},
                   new RoleMenu{ RoleId=1, MenuId = 6},
                   new RoleMenu{ RoleId=2, MenuId =1},
                };
                context.RoleMenus.AddRange(roleMenus);
                context.SaveChanges();
            }

            // Look for any users.
            if (!context.Users.Any())
            {
                var users = new User[]
              {
                new User{ UserName="admin", Password=Encrypt.Md5Encrypt("@123qwe") , EMail="133@qq.com", DepartmentId=1,  Mobile="13788886939", Name="yz", CreateTime=DateTime.Now, CreateUserId=0, },

              };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.UserRoles.Any())
            {
                var userRoles = new UserRole[]
                {
                   new UserRole{ RoleId=1, UserId = 1},
                };
                context.UserRoles.AddRange(userRoles);
                context.SaveChanges();
            }

        }
    }
}
