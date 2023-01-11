using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YzProject.Domain;
using YzProject.Domain.Configs;
using YzProject.Domain.ResultModel;
using YzProject.EventBus.Abstractions;
using YzProject.Redis;
using YzProject.WebAPI.Extensions;
using YzProject.WebAPI.Filters;
using YzProject.WebAPI.Globalizations;
using YzProject.WebAPI.IntegrationEvents.EventHandling;
using YzProject.WebAPI.IntegrationEvents.Events;
using YzProject.WebAPI.Middlewares;
using static YzProject.WebAPI.SwaggerHelper.CustomApiVersion;

namespace YzProject.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //������С�߳���
            //https://learn.microsoft.com/zh-cn/dotnet/core/runtime-config/threading
            ThreadPool.SetMinThreads(200, 200);
            #region Ϊ�м���� JSONLocalizer ���з���ע��
            //https://codewithmukesh.com/blog/json-based-localization-in-aspnet-core/
            //https://codewithmukesh.com/blog/globalization-and-localization-in-aspnet-core/
            services.AddLocalization();
            services.AddSingleton<LocalizationMiddleware>();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            #endregion

            #region �����֤ - JWT
            //�� appsettings �е� JWT ������ӵ����ǵ� JWT ���С�
            services.Configure<JwtSetting>(Configuration.GetSection("JWT"));
            //ASP.NET Core �е�Ĭ����չ�����ڽ������֤������ӵ�Ӧ�ó���
            services.AddAuthentication(options =>
             {
                 //������������Ҫ��Ĭ�������֤���ͣ��� JWT ���������֤
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             })
                 //���ǹ������� JWT ���صġ�
                 .AddJwtBearer(o =>
                 {
                     o.RequireHttpsMetadata = false; //�����������Ƿ���Ҫ HTTPS ���������ǽ�������Ϊ false��
                     o.SaveToken = false;
                     //�� appsettings.json ��ȡ���ò���ӵ� JWT ����
                     o.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,//�Ƿ���ö�ǩ��securityToken��SecurityKey������֤
                         ValidateIssuer = true,//�Ƿ���֤�䷢��
                         ValidateAudience = true,//�Ƿ���֤������
                         ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                         ClockSkew = TimeSpan.Zero,
                         ValidIssuer = Configuration["JWT:Issuer"],//�䷢��
                         ValidAudience = Configuration["JWT:Audience"],//������
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))//ǩ����Կ
                     };
                 });
            #endregion

            #region ע����־

            //ע��Serilog��־����һ�ַ�ʽ
            //services.AddLogging(logging =>
            //{
            //    Log.Logger = new LoggerConfiguration().MinimumLevel.Error().Enrich.FromLogContext().WriteTo.Seq("http://seq").CreateLogger();
            //    logging.AddSerilog(Log.Logger, dispose: true);
            //});

            #endregion

            services.AddOptions();

            //ע�ὡ��������
            services.AddCustomHealthCheck(Configuration);
            //ע����Ϣ�������ӷ���
            services.AddIntegrationServices(Configuration);
            //ע���¼����߷���
            services.AddEventBus(Configuration);

            #region ����ע��redis����

            services.Configure<Settings>(Configuration);
            //https://www.coder.work/article/1578168 �����
            //ͨ�����������ӣ�����ȷ�����ǵķ�����redis׼����֮ǰ�޷������� 
            //����ܻ���������ٶȣ������ڽ��� IP ��ַ���ӳ�Ȼ�󴴽����ӣ��ƶ��ƺ��Ǻ����
            //�����ɱ��������õ�һ������֧���ͷ���
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<Settings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.RedisConnectionString, true);
                configuration.ResolveDns = true;
                return ConnectionMultiplexer.Connect(configuration);
            });

            //services.AddSingleton<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(""));
            #endregion

            #region ע��reids����
            services.AddTransient<IRedisCacheRepository, RedisCacheRepository>();
            #endregion

            #region ע�����ݿ����
            services.AddDbContext<YzProjectContext>(options =>
            {
                //options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("5.5.28"));
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                     //new MySqlServerVersion(new Version(5, 5, 28)),
                     //ServerVersion.Parse("8.0.22-mysql"),
                     new MySqlServerVersion(new Version(5, 7, 37)),
                     mySqlOptions =>
                     {
                         mySqlOptions.MigrationsAssembly("YzProject.Domain");
                     }); ;
            });
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //{
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            //});
            // ������ݿ��쳣ɸѡ��
            // ��װ�� Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
            // AddDatabaseDeveloperPageExceptionFilter �ڿ���������Ϊ EF Ǩ�ƴ����ṩ���õĴ�����Ϣ��
            //services.AddDatabaseDeveloperPageExceptionFilter();
            #endregion

            #region ע��ִ���ҵ���
            services.AddInjectionRepositorys(ServiceLifetime.Scoped);
            services.AddInjectionServices(ServiceLifetime.Scoped);
            #endregion

            #region ģ�Ͱ� ������֤���Զ��巵�ظ�ʽ(ֻ����api�����ã�mvc����ActionFilter����֤)

            //ASP.NET Core WebAPI ģ����֤ ��֤�����Զ��巵��
            //�ο�����:https://www.cnblogs.com/Jackie-sky/p/14167332.html
            //https://www.cnblogs.com/zoe-zyq/p/12627630.html
            //https://www.cnblogs.com/lex-wu/p/11265458.html
            //https://www.cnblogs.com/xiaoxiaotank/p/15657240.html
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //��ȡ��֤ʧ�ܵ�ģ���ֶ� 
                    var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => e.Value.Errors.First().ErrorMessage)
                    .ToList();
                    var strErrorMsg = string.Join("|", errors);
                    //���÷�������
                    var result = new ResultData
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = strErrorMsg
                    };
                    return new BadRequestObjectResult(result);
                };
            });
            #endregion

            //services.AddAuthorization(options => {
            //    //options.
            //});

            #region ע��ȫ�ֹ�����

            services.AddMvc(options =>
            {
                options.Filters.Add<AuthorizationFilter>();
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ActionFilter>();
            });
            //ֻ�ڿ�������action��[ServiceFilter(typeof(AuthorizationFilter))]������
            //services.AddScoped<AuthorizationFilter>();
            #endregion

            #region ���÷����������ϴ�������ļ�����

            //services.Configure<FormOptions>(options =>
            //{
            //    options.ValueLengthLimit = int.MaxValue;
            //    options.MultipartBodyLengthLimit = int.MaxValue;
            //    options.MultipartHeadersLengthLimit = int.MaxValue;
            //});
            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestBodySize = int.MaxValue;
            //});

            #endregion

            #region api�汾����
            //https://github.com/dotnet/aspnet-api-versioning
            // https://github.com/dotnet/aspnet-api-versioning/wiki/API-Version-Reader
            //https://zhuanlan.zhihu.com/p/463951128
            //https://zhuanlan.zhihu.com/p/566558065
            //https://learn.microsoft.com/zh-cn/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
            //https://github.com/dotnet/aspnet-api-versioning/issues/107
            //services.AddApiVersioning(opt =>
            //{
            //    opt.ReportApiVersions = true;
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //    opt.DefaultApiVersion = new ApiVersion(1, 0);
            //    opt.ApiVersionReader = new HeaderApiVersionReader("version");
            //}).AddVersionedApiExplorer(opt =>
            //{
            //    opt.GroupNameFormat = "'v'V";
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //});
            #endregion

            services.AddControllers().AddJsonOptions(config =>
            {
                //����ĵ�����������˵���в�������ĸ��Сд������
                config.JsonSerializerOptions.PropertyNamingPolicy = null;
            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            #region swagger���ã���ȡxml��Ϣ

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YzProject.WebAPI", Version = "v1", Description = "v1�ĵ�����" });
                //https://learn.microsoft.com/zh-cn/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
                //typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                //{
                //    c.SwaggerDoc(version, new OpenApiInfo
                //    {
                //        Version = version,
                //        Title = $"�ӿ��ĵ�{version}"
                //    });
                //});
                //https://qastack.cn/programming/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
                //https://blog.csdn.net/zhang_adrian/article/details/90241878
                #region 1����������Bearer��Swagger��aka Swashbuckle�������֤�Ի���������ؼ��֣����磺"bearer xT1..."������ʹ���������ã�
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     { jwtSecurityScheme, Array.Empty<string>() }
                 });

                #endregion

                #region 2��swagger token �����ʽ��Bearer {token}

                //��swagger�����֤��api��������authorize��ť
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "Authorization format : Bearer {token}",
                //    //Description = "jwt ��Ȩ�����ݽ�������ͷ�н��д��䣩ֱ�����¿�������Bearer {token}(ע������ֱ����һ���ո�) \",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //  {
                //    new OpenApiSecurityScheme
                //    {
                //      Reference = new OpenApiReference
                //        {
                //          Type = ReferenceType.SecurityScheme,
                //          Id = "Bearer"
                //        },
                //        Scheme = "oauth2",
                //        Name = "Bearer",
                //        In = ParameterLocation.Header,

                //    },
                //      new List<string>()
                //  }
                //});

                #endregion

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                #region
                //c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                // Set the comments path for the Swagger JSON and UI.
                //��ȡxmlע���ļ���Ŀ¼
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //xml�ĵ�����·��
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //TRUE ��ʾ��������ע�� Ĭ�ϵĵڶ���������false
                //c.IncludeXmlComments(xmlPath,true);
                //c.OrderActionsBy(o => o.RelativePath);//��action�����ƽ�������

                // ���������Swagger ������ʾ����  �ǾͿ����ں��������һ������ [ApiExplorerSettings(IgnoreApi = true)]
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "Value: Bearer {token}",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //  {
                //    new OpenApiSecurityScheme
                //    {
                //      Reference = new OpenApiReference
                //      {
                //        Type = ReferenceType.SecurityScheme,
                //        Id = "Bearer"
                //      },Scheme = "oauth2",Name = "Bearer",In = ParameterLocation.Header,
                //    },new List<string>()
                //  }
                //});
                #endregion
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YzProject.WebAPI v1"));
                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                //    c.RoutePrefix = string.Empty;
                //    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                //    c.DefaultModelsExpandDepth(-1);
                //});
            }

            #region ���뽡������м��
            //app.UseHealthChecks("/healthChecks", new HealthCheckOptions
            //{
            //    ResponseWriter = async (context, report) =>
            //    {
            //        var result = JsonConvert.SerializeObject(
            //            new
            //            {
            //                status = report.Status.ToString(),
            //                errors = report.Entries.Select(e => new { key = e.Key, value = System.Enum.GetName(typeof(HealthStatus), e.Value.Status) })
            //            });
            //        context.Response.ContentType = MediaTypeNames.Application.Json;
            //        await context.Response.WriteAsync(result);
            //    },
            //    AllowCachingResponses = false,
            //    ResultStatusCodes =
            //    {
            //        [HealthStatus.Healthy] = StatusCodes.Status200OK,
            //        [HealthStatus.Degraded] = StatusCodes.Status200OK,
            //        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            //    }
            //});

            //app.UseHealthChecks("/health", new HealthCheckOptions
            //{
            //    ResponseWriter = async (context, report) =>
            //    {
            //        context.Response.ContentType = "application/json";
            //        var response = new HealthCheckReponse
            //        {
            //            Status = report.Status.ToString(),
            //            HealthChecks = report.Entries.Select(x => new IndividualHealthCheckResponse
            //            {
            //                Components = x.Key,
            //                Status = x.Value.Status.ToString(),
            //                Description = x.Value.Description

            //            }),
            //            HealthCheckDuration = report.TotalDuration
            //        };
            //        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            //    }
            //});
            #endregion

            #region ע��Serilog��־����һ�ַ�ʽ
            //Log.Logger = new LoggerConfiguration().MinimumLevel.Error().Enrich.FromLogContext().WriteTo.Seq("http://seq").CreateLogger();
            //loggerFactory.AddSerilog(dispose: true);
            #endregion

            #region �������м��/ȫ��

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US"))
            };
            app.UseRequestLocalization(options);
            app.UseStaticFiles();
            app.UseMiddleware<LocalizationMiddleware>();
            #endregion

            app.UseRouting();

            //ȷ�� app.UseAuthentication(); ���ǳ����� app.UseAuthorization(); ֮ǰ����Ϊ�Ӽ����Ͻ����ȶ��û����������֤��Ȼ���ٽ�����Ȩ��
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });

            ConfigureEventBus(app);

            //app.UseSwagger().UseSwaggerUI(c =>
            //{
            //    //���հ汾������ʾ
            //    //typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
            //    //{
            //    //    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"��������С����ӿ��ĵ�{version}");
            //    //});

            //    typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
            //    {
            //        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"�ӿ��ĵ�{version}");
            //    });
            //});
        }



        /// <summary>
        /// �����¼�����
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //���ö����¼�
            //eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}
