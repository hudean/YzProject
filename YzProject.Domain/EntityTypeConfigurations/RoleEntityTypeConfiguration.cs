using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain.EntityTypeConfigurations
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role").HasKey(c => c.Id);
            builder.HasComment("角色表");
            builder.Property(b => b.Id).HasComment("角色表主键id");
            //builder.Property(b => b.RoleCode).IsRequired().HasMaxLength(32).HasComment("角色编码");
            builder.Property(b => b.RoleName).IsRequired().HasMaxLength(32).HasComment("角色名称");
            builder.Property(b => b.RoleDescription).HasMaxLength(255).HasComment("备注");
        }
    }
}
