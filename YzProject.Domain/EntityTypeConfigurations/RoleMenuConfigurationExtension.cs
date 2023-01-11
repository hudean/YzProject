using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain.EntityTypeConfigurations
{
    /// <summary>
    /// 使用扩展方法的角色菜单映射配置
    /// </summary>
    internal static class RoleMenuConfigurationExtension
    {
        /// <summary>
        /// 角色菜单映射配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns></returns>
        public static ModelBuilder RoleMenuConfig(this ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RoleMenu>()
            //    .ToTable("RoleMenu")
            //    .HasKey(x => x.Id);
            //modelBuilder.Entity<RoleMenu>()
            //    .Property(x => x.RoleId)
            //    .IsRequired();
            //modelBuilder.Entity<RoleMenu>()
            //    .Property(x => x.MenuId)
            //    .IsRequired();

            modelBuilder.Entity<RoleMenu>(entity =>
            {
                entity.ToTable("RoleMenu").HasKey(x => x.Id);

                entity.HasComment("角色菜单关系表");

                entity.Property(e => e.Id).HasComment("主键ID");

                entity.Property(e => e.RoleId).IsRequired().HasComment("角色ID");

                entity.Property(e => e.MenuId).IsRequired().HasComment("菜单ID");
            });
            return modelBuilder;
        }
    }
}
