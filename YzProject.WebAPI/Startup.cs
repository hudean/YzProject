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
            //配置最小线程数
            //https://learn.microsoft.com/zh-cn/dotnet/core/runtime-config/threading
            ThreadPool.SetMinThreads(200, 200);
            #region 为中间件和 JSONLocalizer 进行服务注册
            //https://codewithmukesh.com/blog/json-based-localization-in-aspnet-core/
            //https://codewithmukesh.com/blog/globalization-and-localization-in-aspnet-core/
            services.AddLocalization();
            services.AddSingleton<LocalizationMiddleware>();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            #endregion

            #region 添加认证 - JWT
            //将 appsettings 中的 JWT 部分添加到我们的 JWT 类中。
            services.Configure<JwtSetting>(Configuration.GetSection("JWT"));
            //ASP.NET Core 中的默认扩展，用于将身份验证服务添加到应用程序
            services.AddAuthentication(options =>
             {
                 //定义了我们需要的默认身份验证类型，即 JWT 承载身份验证
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             })
                 //它是关于配置 JWT 承载的。
                 .AddJwtBearer(o =>
                 {
                     o.RequireHttpsMetadata = false; //定义了我们是否需要 HTTPS 连接让我们将其设置为 false。
                     o.SaveToken = false;
                     //从 appsettings.json 读取设置并添加到 JWT 对象
                     o.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,//是否调用对签名securityToken的SecurityKey进行验证
                         ValidateIssuer = true,//是否验证颁发者
                         ValidateAudience = true,//是否验证接收者
                         ValidateLifetime = true,//是否验证失效时间
                         ClockSkew = TimeSpan.Zero,
                         ValidIssuer = Configuration["JWT:Issuer"],//颁发者
                         ValidAudience = Configuration["JWT:Audience"],//接收者
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))//签名秘钥
                     };
                 });
            #endregion

            #region 注入日志

            //注入Serilog日志其中一种方式
            //services.AddLogging(logging =>
            //{
            //    Log.Logger = new LoggerConfiguration().MinimumLevel.Error().Enrich.FromLogContext().WriteTo.Seq("http://seq").CreateLogger();
            //    logging.AddSerilog(Log.Logger, dispose: true);
            //});

            #endregion

            services.AddOptions();

            //注册健康检查服务
            services.AddCustomHealthCheck(Configuration);
            //注册消息队列连接服务
            services.AddIntegrationServices(Configuration);
            //注册事件总线服务
            services.AddEventBus(Configuration);

            #region 单例注入redis连接

            services.Configure<Settings>(Configuration);
            //https://www.coder.work/article/1578168 对象池
            //通过在这里连接，我们确保我们的服务在redis准备好之前无法启动。 
            //这可能会减慢启动速度，但鉴于解析 IP 地址有延迟然后创建连接，移动似乎是合理的
            //启动成本而不是让第一个请求支付惩罚。
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<Settings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.RedisConnectionString, true);
                configuration.ResolveDns = true;
                return ConnectionMultiplexer.Connect(configuration);
            });

            //services.AddSingleton<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(""));
            #endregion

            #region 注入reids服务
            services.AddTransient<IRedisCacheRepository, RedisCacheRepository>();
            #endregion

            #region 注册数据库服务
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
            // 添加数据库异常筛选器
            // 安装包 Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
            // AddDatabaseDeveloperPageExceptionFilter 在开发环境中为 EF 迁移错误提供有用的错误信息。
            //services.AddDatabaseDeveloperPageExceptionFilter();
            #endregion

            #region 注入仓储和业务层
            services.AddInjectionRepositorys(ServiceLifetime.Scoped);
            services.AddInjectionServices(ServiceLifetime.Scoped);
            #endregion

            #region 模型绑定 特性验证，自定义返回格式(只能在api上有用，mvc请在ActionFilter里验证)

            //ASP.NET Core WebAPI 模型验证 验证特性自定义返回
            //参考文章:https://www.cnblogs.com/Jackie-sky/p/14167332.html
            //https://www.cnblogs.com/zoe-zyq/p/12627630.html
            //https://www.cnblogs.com/lex-wu/p/11265458.html
            //https://www.cnblogs.com/xiaoxiaotank/p/15657240.html
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //获取验证失败的模型字段 
                    var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => e.Value.Errors.First().ErrorMessage)
                    .ToList();
                    var strErrorMsg = string.Join("|", errors);
                    //设置返回内容
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

            #region 注册全局过滤器

            services.AddMvc(options =>
            {
                options.Filters.Add<AuthorizationFilter>();
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ActionFilter>();
            });
            //只在控制器或action上[ServiceFilter(typeof(AuthorizationFilter))]才有用
            //services.AddScoped<AuthorizationFilter>();
            #endregion

            #region 设置服务器允许上传的最大文件限制

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

            #region api版本控制
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
                //解决文档中样例参数说明中参数首字母变小写的问题
                config.JsonSerializerOptions.PropertyNamingPolicy = null;
            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            #region swagger配置，读取xml信息

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YzProject.WebAPI", Version = "v1", Description = "v1文档描述" });
                //https://learn.microsoft.com/zh-cn/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
                //typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                //{
                //    c.SwaggerDoc(version, new OpenApiInfo
                //    {
                //        Version = version,
                //        Title = $"接口文档{version}"
                //    });
                //});
                //https://qastack.cn/programming/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
                //https://blog.csdn.net/zhang_adrian/article/details/90241878
                #region 1、避免总是Bearer在Swagger（aka Swashbuckle）身份验证对话框上输入关键字，例如："bearer xT1..."，可以使用以下配置：
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

                #region 2、swagger token 输入格式：Bearer {token}

                //给swagger添加验证，api界面新增authorize按钮
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "Authorization format : Bearer {token}",
                //    //Description = "jwt 授权（数据将在请求头中进行传输）直接在下框中输入Bearer {token}(注意两者直接是一个空格) \",
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
                //获取xml注释文件的目录
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //xml文档绝对路径
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //TRUE 显示控制器层注释 默认的第二个参数是false
                //c.IncludeXmlComments(xmlPath,true);
                //c.OrderActionsBy(o => o.RelativePath);//对action的名称进行排序

                // 如果不想在Swagger 里面显示出来  那就可以在函数上面加一个特性 [ApiExplorerSettings(IgnoreApi = true)]
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

            #region 加入健康检查中间件
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

            #region 注入Serilog日志其中一种方式
            //Log.Logger = new LoggerConfiguration().MinimumLevel.Error().Enrich.FromLogContext().WriteTo.Seq("http://seq").CreateLogger();
            //loggerFactory.AddSerilog(dispose: true);
            #endregion

            #region 多语言中间件/全球化

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US"))
            };
            app.UseRequestLocalization(options);
            app.UseStaticFiles();
            app.UseMiddleware<LocalizationMiddleware>();
            #endregion

            app.UseRouting();

            //确保 app.UseAuthentication(); 总是出现在 app.UseAuthorization(); 之前，因为从技术上讲，先对用户进行身份验证，然后再进行授权。
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
            //    //按照版本倒叙显示
            //    //typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
            //    //{
            //    //    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"悦美乳腺小程序接口文档{version}");
            //    //});

            //    typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
            //    {
            //        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"接口文档{version}");
            //    });
            //});
        }



        /// <summary>
        /// 配置事件总线
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //启用订阅事件
            //eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}
