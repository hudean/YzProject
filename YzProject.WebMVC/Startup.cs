using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using YzProject.Domain;
using YzProject.Model;
using YzProject.WebMVC.Extensions;
using YzProject.WebMVC.Filters;

namespace YzProject.WebMVC
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //ASP.NET Core 中的 Razor 文件编译
            //https://docs.microsoft.com/zh-cn/aspnet/core/mvc/views/view-compilation?view=aspnetcore-6.0&tabs=visual-studio
            services.AddControllersWithViews();
            //services.AddControllersWithViews().AddRazorRuntimeCompilation();


            #region 注册Session服务
            //services.AddSession();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
            });
            #endregion

            #region 跨域
            //跨域 https://docs.microsoft.com/zh-cn/aspnet/core/security/cors?view=aspnetcore-5.0
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
                {
                    //builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials();
                    builder.AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
            #endregion

            //取消自动去除Async后缀的功能 .net core 3.0之后默认会过滤掉方法名里的Async后缀
            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            #region 注入日志

            //注入log4net日志其中一种方式
            //services.AddLogging(logging => {
            //    logging.AddLog4Net();
            //    //默认的配置文件路径是在根目录，且文件名为log4net.config
            //    //如果文件路径或名称有变化，需要重新设置其路径或名称
            //    //比如在项目根目录下创建一个名为cfg的文件夹，将log4net.config文件移入其中，并改名为log.config
            //    //则需要使用下面的代码来进行配置
            //    logging.AddLog4Net(new Log4NetProviderOptions()
            //    {
            //        Log4NetConfigFileName = "cfg/log.config",
            //        Watch = true
            //    });
            //});

            #endregion

            #region 注册数据库服务
            //可以使用选项模式
            string dbType = Configuration["DbType"];

            if (dbType.ToUpper().Equals("MYSQL"))
            {
                services.AddDbContext<YzProjectContext>(options =>
                {
                    //options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("5.5.28"));
                    options.UseMySql(
                         Configuration.GetConnectionString("DefaultConnection"),
                         //new MySqlServerVersion(new Version(5, 5, 28)),
                         new MySqlServerVersion(new Version(5, 7, 37)),
                         mySqlOptions =>
                         {
                             mySqlOptions.MigrationsAssembly("YzProject.EntityFrameworkCore");
                         });
                });
            }
            else if (dbType.ToUpper().Equals("SQLSERVER"))
            {
                services.AddDbContext<YzProjectContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
            }
            // 添加数据库异常筛选器
            // 安装包 Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
            // AddDatabaseDeveloperPageExceptionFilter 在开发环境中为 EF 迁移错误提供有用的错误信息。
            services.AddDatabaseDeveloperPageExceptionFilter();
            #endregion


            #region 模型绑定验证(只能在api上有用)

            //ASP.NET Core WebAPI 模型验证 验证特性自定义返回 参考文章:https://www.cnblogs.com/Jackie-sky/p/14167332.html
            //模型绑定 特性验证，自定义返回格式
            //services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = actionContext =>
            //    {
            //        //获取验证失败的模型字段 
            //        var errors = actionContext.ModelState
            //        .Where(e => e.Value.Errors.Count > 0)
            //        .Select(e => e.Value.Errors.First().ErrorMessage)
            //        .ToList();
            //        var strErrorMsg = string.Join("|", errors);
            //        //设置返回内容
            //        var result = new JsonResponse
            //        {
            //            code = ResponseCode.Fail,
            //            msg = strErrorMsg
            //        };
            //        return new BadRequestObjectResult(result);
            //    };
            //});

            #endregion


            #region 使用反射的形式注入服务

            //services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddInjectionRepositorys(ServiceLifetime.Scoped);
            services.AddInjectionServices(ServiceLifetime.Scoped);

            #endregion

            #region Quartz定时任务

            //注册ISchedulerFactory的实例
            //services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            //StartQuartzAsync();
            #endregion

            #region 注册全局过滤器

            services.AddMvc(options =>
            {
                options.Filters.Add<AuthorizationFilter>();
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ActionFilter>();
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //注入log4net日志其中一种方式
            loggerFactory.AddLog4Net();
            app.UseStaticFiles();
            app.UseRouting();
            //允许跨域
            app.UseCors(MyAllowSpecificOrigins);
            //启用Session
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    //pattern: "{controller=Home}/{action=Index}/{id?}");
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }


        /// <summary>
        /// Quartz异步执行定时任务
        /// </summary>
        /// <returns></returns>
        private async Task StartQuartzAsync()
        {
            // 参考文章 https://www.cnblogs.com/MicroHeart/p/9402731.html
            //https://www.cnblogs.com/dangzhensheng/p/10496278.html
            StdSchedulerFactory _schedulerFactory = new StdSchedulerFactory();
            //1.通过工场类获得调度器
            IScheduler _scheduler = await _schedulerFactory.GetScheduler();
            //2.开启调度器
            await _scheduler.Start();
            //3.创建触发器(也叫时间策略)
            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever())//每10秒执行一次
                            .Build();
            //4.创建作业实例
            //Jobs即我们需要执行的作业
            var jobDetail = JobBuilder.Create<Job.DemoJob>()
                            .WithIdentity("Myjob", "group")//我们给这个作业取了个“Myjob”的名字，并取了个组名为“group”
                            .Build();
            //5.将触发器和作业任务绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
