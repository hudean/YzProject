using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YzProject.Domain.Entites;
using YzProject.Domain.EntityTypeConfigurations;
using YzProject.EventBus.Abstractions;
using static YzProject.Domain.ModelBuilderExtensions;

namespace YzProject.Domain
{
    public class YzProjectContext : DbContext
    {
        /*
         * 官方文档地址： https://docs.microsoft.com/zh-cn/ef/core/
         * 1、迁移命令 把当前项目设置为程序包控制台启动项目
         * 2、Add-Migration InitialCreate
         * 3、Update-Database
         */
        //private readonly IEventBus _events;
        public YzProjectContext()
        {
        }

        public YzProjectContext(DbContextOptions<YzProjectContext> options
            //, IEventBus eventBus//IEventPublisher events
            )
            : base(options)
        {
            //_events = eventBus;
        }

        // Used by Dapper
        public IDbConnection Connection => Database.GetDbConnection();

        #region DbSet

        public DbSet<Department> Departments { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }

        public DbSet<ExcelExample> ExcelExamples { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion

        /**
       * 打印生成的sql 语句到控制台上  
       * 注意： Program.cs或者appsettings里把Microsoft.EntityFrameworkCore的日志级别设置为warning了要设置为Information
       * **/

        public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(b => b.AddConsole().AddFilter("", LogLevel.Information));
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer("Integrated Security=False;Data Source=.;Initial Catalog=BreastHealth;User Id=sa;pwd=sasino;");
            //}
            //optionsBuilder.UseMySql(EnvironmentVariables.DBConnection2);
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //不需要主键时，要这样配置，但是不让进行更新和插入
            //modelBuilder.Entity<Menu>(entity =>
            //{
            //    entity.HasNoKey();
            //});

            #region 分组配置
            // modelBuilder.Entity<User>().Property(u => u.UserName).IsRequired();
            /*  分组配置
             *  为了减小 OnModelCreating 方法的大小，可以将实体类型的所有配置提取到实现 IEntityTypeConfiguration<TEntity> 的单独类中。
             *  然后，只需从 OnModelCreating 调用 Configure 方法。
             *  可以在给定程序集中应用实现 IEntityTypeConfiguration 的类型中指定的所有配置。
             *  modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoleEntityTypeConfiguration).Assembly);
             */
            //new DepartmentEntityTypeConfiguration().Configure(modelBuilder.Entity<Department>());
            new MenuEntityTypeConfiguration().Configure(modelBuilder.Entity<Menu>());
            new RoleEntityTypeConfiguration().Configure(modelBuilder.Entity<Role>());
            new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
            #endregion

            // 也可以使用modelBuilder扩展方法进行每个类的实现
            modelBuilder.UserRoleConfig().RoleMenuConfig();

            #region 全局查询筛选器
            //使用数据注释来配置模型
            //全局查询筛选器
            //全局查询筛选器是应用于元数据模型（通常为 OnModelCreating）中的实体类型的 LINQ 查询谓词。
            //查询谓词即通常传递给 LINQ Where 查询运算符的布尔表达式。 EF Core 会自动将此类筛选器应用于涉及这些实体类型的任何 LINQ 查询。
            //EF Core 还将其应用于使用 Include 或导航属性进行间接引用的实体类型。 此功能的一些常见应用如下：
            //软删除 - 实体类型定义 IsDeleted 属性。
            //多租户 - 实体类型定义 TenantId 属性。
            //https://docs.microsoft.com/zh-cn/ef/core/querying/filters
            //modelBuilder.Entity<Blog>().HasQueryFilter(b => EF.Property<string>(b, "_tenantId") == _tenantId);
            //modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            //查询过滤器需要在 base.OnModelCreating 之前应用
            //https://www.cnblogs.com/boxrice/p/11670635.html
            //https://www.cnblogs.com/willick/p/13358580.html
            //https://onebyone.icu/archives/2071
            modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.IsDeleted == false);

            #region
            //foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(p => typeof(ISoftDelete).IsAssignableFrom(p.ClrType)))
            //{
            //    entityType.AddSoftDeleteQueryFilter();
            //}

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    // 省略其它无关的代码
            //    if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            //    {
            //        entityType.AddSoftDeleteQueryFilter();
            //    }
            //}
            #endregion
            //modelBuilder.AddSoftDelete();
            #endregion

            base.OnModelCreating(modelBuilder);
            //从装配应用配置
            //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public override int SaveChanges()
        {
            HandleBeforeSaveChanges();
            //int result = base.SaveChanges();
            //SendDomainEventsAsync();
            //return result;
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleBeforeSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
            //await SendDomainEventsAsync();
        }

        /// <summary>
        /// 在保存更改之前处理
        /// </summary>
        private void HandleBeforeSaveChanges()
        {

            foreach (var entry in ChangeTracker.Entries<IEntity>().ToList())
            {
                DateTime dateTime = DateTime.Now;
                switch (entry.State)
                {
                    case EntityState.Added:
                        //entry.Entity.CreatedBy = userId;
                        //entry.Entity.LastModifiedBy = userId;
                        if (entry.Entity is IHasCreationTime hasCreationTime)
                        {
                            hasCreationTime.CreationTime = dateTime;
                        }
                        CheckAndSetId(entry.Entity);
                        break;

                    case EntityState.Modified:
                        //entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        //entry.Entity.LastModifiedBy = userId;
                        if (entry.Entity is IHasModificationTime hasModificationTime)
                        {
                            hasModificationTime.LastModificationTime = dateTime;
                        }
                        break;

                    case EntityState.Deleted:
                        if (entry.Entity is ISoftDelete softDelete)
                        {
                            //softDelete.DeletedBy = userId;
                            //softDelete.DeletedOn = DateTime.UtcNow;
                            softDelete.IsDeleted = true;
                            if (softDelete is IHasDeletionTime hasDeletionTime)
                            {
                                hasDeletionTime.DeletionTime = dateTime;
                            }
                            entry.State = EntityState.Modified;
                        }

                        break;
                }
            }

            ChangeTracker.DetectChanges();
        }

        /// <summary>
        /// 检查和设置Id
        /// </summary>
        /// <param name="entity"></param>
        private void CheckAndSetId(IEntity entity)
        {
            if (entity is IEntity<Guid> entityWithGuidId)
            {
                if (entityWithGuidId.Id != default)
                {
                    return;
                }
                //entityWithGuidId.Id = GuidGenerator.Create();
                entityWithGuidId.Id = Guid.NewGuid();
            }
        }


        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entry.Entity.GetType()))
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.CurrentValues["IsDelete"] = true;
                            break;
                        case EntityState.Added:
                            entry.CurrentValues["IsDelete"] = false;
                            break;
                    }
                }
            }
        }

        //private async Task SendDomainEventsAsync()
        //{
        //    var entitiesWithEvents = ChangeTracker.Entries<IAggregateRoot>()//ChangeTracker.Entries<IEntity>()
        //        .Select(e => e.Entity)
        //        .Where(e => e.DomainEvents.Count > 0)
        //        .ToArray();

        //    foreach (var entity in entitiesWithEvents)
        //    {
        //        var domainEvents = entity.DomainEvents.ToArray();
        //        entity.DomainEvents.Clear();
        //        foreach (var domainEvent in domainEvents)
        //        {
        //            await _events.PublishAsync(domainEvent);
        //        }
        //    }
        //}

        //private void SendDomainEvents()
        //{
        //    var entitiesWithEvents = ChangeTracker.Entries<IAggregateRoot>()//ChangeTracker.Entries<IEntity>()
        //        .Select(e => e.Entity)
        //        .Where(e => e.DomainEvents.Count > 0)
        //        .ToArray();

        //    foreach (var entity in entitiesWithEvents)
        //    {
        //        var domainEvents = entity.DomainEvents.ToArray();
        //        entity.DomainEvents.Clear();
        //        foreach (var domainEvent in domainEvents)
        //        {
        //             _events.Publish(domainEvent);
        //        }
        //    }
        //}
    }
}
