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
            //ASP.NET Core �е� Razor �ļ�����
            //https://docs.microsoft.com/zh-cn/aspnet/core/mvc/views/view-compilation?view=aspnetcore-6.0&tabs=visual-studio
            services.AddControllersWithViews();
            //services.AddControllersWithViews().AddRazorRuntimeCompilation();


            #region ע��Session����
            //services.AddSession();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
            });
            #endregion

            #region ����
            //���� https://docs.microsoft.com/zh-cn/aspnet/core/security/cors?view=aspnetcore-5.0
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

            //ȡ���Զ�ȥ��Async��׺�Ĺ��� .net core 3.0֮��Ĭ�ϻ���˵����������Async��׺
            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            #region ע����־

            //ע��log4net��־����һ�ַ�ʽ
            //services.AddLogging(logging => {
            //    logging.AddLog4Net();
            //    //Ĭ�ϵ������ļ�·�����ڸ�Ŀ¼�����ļ���Ϊlog4net.config
            //    //����ļ�·���������б仯����Ҫ����������·��������
            //    //��������Ŀ��Ŀ¼�´���һ����Ϊcfg���ļ��У���log4net.config�ļ��������У�������Ϊlog.config
            //    //����Ҫʹ������Ĵ�������������
            //    logging.AddLog4Net(new Log4NetProviderOptions()
            //    {
            //        Log4NetConfigFileName = "cfg/log.config",
            //        Watch = true
            //    });
            //});

            #endregion

            #region ע�����ݿ����
            //����ʹ��ѡ��ģʽ
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
            // ������ݿ��쳣ɸѡ��
            // ��װ�� Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
            // AddDatabaseDeveloperPageExceptionFilter �ڿ���������Ϊ EF Ǩ�ƴ����ṩ���õĴ�����Ϣ��
            services.AddDatabaseDeveloperPageExceptionFilter();
            #endregion


            #region ģ�Ͱ���֤(ֻ����api������)

            //ASP.NET Core WebAPI ģ����֤ ��֤�����Զ��巵�� �ο�����:https://www.cnblogs.com/Jackie-sky/p/14167332.html
            //ģ�Ͱ� ������֤���Զ��巵�ظ�ʽ
            //services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = actionContext =>
            //    {
            //        //��ȡ��֤ʧ�ܵ�ģ���ֶ� 
            //        var errors = actionContext.ModelState
            //        .Where(e => e.Value.Errors.Count > 0)
            //        .Select(e => e.Value.Errors.First().ErrorMessage)
            //        .ToList();
            //        var strErrorMsg = string.Join("|", errors);
            //        //���÷�������
            //        var result = new JsonResponse
            //        {
            //            code = ResponseCode.Fail,
            //            msg = strErrorMsg
            //        };
            //        return new BadRequestObjectResult(result);
            //    };
            //});

            #endregion


            #region ʹ�÷������ʽע�����

            //services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddInjectionRepositorys(ServiceLifetime.Scoped);
            services.AddInjectionServices(ServiceLifetime.Scoped);

            #endregion

            #region Quartz��ʱ����

            //ע��ISchedulerFactory��ʵ��
            //services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            //StartQuartzAsync();
            #endregion

            #region ע��ȫ�ֹ�����

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
            //ע��log4net��־����һ�ַ�ʽ
            loggerFactory.AddLog4Net();
            app.UseStaticFiles();
            app.UseRouting();
            //�������
            app.UseCors(MyAllowSpecificOrigins);
            //����Session
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
        /// Quartz�첽ִ�ж�ʱ����
        /// </summary>
        /// <returns></returns>
        private async Task StartQuartzAsync()
        {
            // �ο����� https://www.cnblogs.com/MicroHeart/p/9402731.html
            //https://www.cnblogs.com/dangzhensheng/p/10496278.html
            StdSchedulerFactory _schedulerFactory = new StdSchedulerFactory();
            //1.ͨ���������õ�����
            IScheduler _scheduler = await _schedulerFactory.GetScheduler();
            //2.����������
            await _scheduler.Start();
            //3.����������(Ҳ��ʱ�����)
            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever())//ÿ10��ִ��һ��
                            .Build();
            //4.������ҵʵ��
            //Jobs��������Ҫִ�е���ҵ
            var jobDetail = JobBuilder.Create<Job.DemoJob>()
                            .WithIdentity("Myjob", "group")//���Ǹ������ҵȡ�˸���Myjob�������֣���ȡ�˸�����Ϊ��group��
                            .Build();
            //5.������������ҵ����󶨵���������
            await _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
