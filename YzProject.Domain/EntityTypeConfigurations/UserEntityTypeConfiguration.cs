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
    /// <summary>
    /// 分组配置/为了减小 OnModelCreating 方法的大小，可以将实体类型的所有配置提取到实现 IEntityTypeConfiguration<TEntity> 的单独类中。
    /// https://docs.microsoft.com/zh-cn/ef/core/modeling/
    /// </summary>
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasComment("用户表").ToTable("User").HasKey(c => c.Id);
            builder.Property(b => b.Id).HasComment("用户表主键Id");
            builder.Property(b => b.UserName).IsRequired().HasMaxLength(32).HasComment("用户名");
            builder.Property(b => b.Password).IsRequired().HasMaxLength(32).HasComment("密码");
            builder.Property(b => b.Name).HasMaxLength(32).HasComment("用户姓名");
            //builder.Property(b => b.NickName).HasMaxLength(32).HasComment("用户昵称");
            builder.Property(b => b.HardImgUrl).HasMaxLength(255).HasComment("头像");
            builder.Property(b => b.ThumbnailHeadImgUrl).HasMaxLength(255).HasComment("头像缩略图");
            builder.Property(b => b.Birthday).HasComment("生日");
            builder.Property(b => b.EMail).IsRequired().HasMaxLength(32).HasComment("邮箱");
            builder.Property(b => b.Mobile).IsRequired().HasMaxLength(32).HasComment("手机号码");
            builder.Property(b => b.Address).HasMaxLength(255).HasComment("地址");
            builder.Property(b => b.Introduction).HasMaxLength(int.MaxValue).HasComment("简介");
            builder.Property(b => b.CreateUserId).IsRequired().HasComment("创建用户id");
            builder.Property(b => b.CreateTime).IsRequired().HasColumnType("datetime").HasComment("创建日期");
            builder.Property(b => b.LastLoginTime).HasColumnType("datetime").HasComment("上一次登录时间");
            builder.Property(b => b.LoginTimes).IsRequired().HasDefaultValue(0).HasComment("登录次数");
            builder.Property(b => b.DepartmentId).IsRequired().HasComment("部门id");
            builder.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(0).HasComment("是否删除");
            //builder.Property(b => b.IdentityCardCode).HasMaxLength(18).HasComment("身份证号码");
            //builder.Property(b => b.IdentityCardBacktUrl).HasMaxLength(255).HasComment("身份证背面");
            //builder.Property(b => b.IdentityCardFrontUrl).HasMaxLength(255).HasComment("身份证正面");
        }
    }
}
