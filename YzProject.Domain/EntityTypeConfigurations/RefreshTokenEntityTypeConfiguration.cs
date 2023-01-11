using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain.EntityTypeConfigurations
{
    internal class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasComment("刷新令牌表").ToTable("RefreshToken").HasKey(c => c.Id);
            builder.Property(b => b.Id).HasComment("刷新令牌表主键Id");
            builder.Property(b => b.UserId).IsRequired().HasComment("用户id");
            builder.Property(b => b.Token).IsRequired().HasComment("刷新令牌");
            builder.Property(b => b.Expires).IsRequired().HasComment("过期时间");
            builder.Property(b => b.Created).IsRequired().HasComment("创建时间");
            builder.Property(b => b.Revoked).HasComment("撤销时间");
            builder.Ignore(b => b.IsActive);
            builder.Ignore(b => b.IsExpired);
            //modelBuilder.Entity<Blog>().Ignore(b => b.LoadedFromDatabase);
        }
    }
}
