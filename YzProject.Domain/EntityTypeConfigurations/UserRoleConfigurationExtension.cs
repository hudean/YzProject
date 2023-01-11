using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YzProject.Domain.Entites;

namespace YzProject.Domain.EntityTypeConfigurations
{
    /// <summary>
    /// 使用扩展方法的用户角色映射配置
    /// </summary>
    internal static class UserRoleConfigurationExtension
    {
        /// <summary>
        /// 用户角色映射配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns></returns>
        public static ModelBuilder UserRoleConfig(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .ToTable("UserRole")
                .HasKey(x => x.Id);
            modelBuilder.Entity<UserRole>()
                .Property(x => x.UserId)
                .IsRequired();
            modelBuilder.Entity<UserRole>()
                .Property(x => x.RoleId)
                .IsRequired();
            return modelBuilder;
        }
    }
}
