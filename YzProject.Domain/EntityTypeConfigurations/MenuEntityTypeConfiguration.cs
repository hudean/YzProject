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
    internal class MenuEntityTypeConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menu").HasKey(c => c.Id);
            builder.HasComment("菜单表");
            builder.Property(b => b.Id).HasComment("菜单表主键id");
            builder.Property(b => b.ParentId).IsRequired().HasDefaultValue(0).HasComment("父级菜单Id");
            builder.Property(b => b.SerialNumber).IsRequired().HasComment("序号");
            builder.Property(b => b.MenuName).IsRequired().HasMaxLength(16).HasComment("菜单名称");
            builder.Property(b => b.MenuCode).IsRequired().HasMaxLength(16).HasComment("菜单编码");
            builder.Property(b => b.Url).HasMaxLength(255).HasComment("菜单地址");
            builder.Property(b => b.MenuType).IsRequired().HasDefaultValue(0).HasComment("类型：0导航菜单；1操作按钮");
            builder.Property(b => b.Icon).HasMaxLength(64).HasComment("菜单图标");
            builder.Property(b => b.Remarks).HasMaxLength(255).HasComment("备注");
            builder.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(0).HasComment("是否删除");
        }
    }
}
