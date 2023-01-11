using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using YzProject.Common;
using YzProject.EventBus;
using YzProject.EventBus.Abstractions;
using YzProject.EventBusRabbitMQ;
using YzProject.WebAPI.HealthChecks;
using YzProject.WebAPI.IntegrationEvents.EventHandling;

namespace YzProject.WebAPI.Extensions
{
    /// <summary>
    /// ServiceProvider的扩展方法
    /// </summary>
    public static class ServiceProviderExtension
    {
        /// <summary>
        /// 注册集成服务（RabbitMQ集成服务）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region 注册事件总线日志

            //services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            //    sp => (DbConnection c) => new IntegrationEventLogService(c));

            //services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

            #endregion

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }


        /// <summary>
        /// 注册事件总线服务(也可以使用MediatR实现)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<Autofac.ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //注册要订阅事件的处理器
            //services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
            services.AddTransient<OrderStartedIntegrationEventHandler>();

            return services;
        }

        /// <summary>
        /// 注册健康检测服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            //文档地址：https://codewithmukesh.com/blog/healthchecks-in-aspnet-core-explained/
            //Install-Package Microsoft.Extensions.Diagnostics.HealthChecks
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
            //hcBuilder.AddSqlServer(
            //    //configuration["ConnectionString"],
            //    configuration.GetConnectionString("DefaultConnection"),
            //    name: "YzProjectDB-check",
            //    tags: new string[] { "YzProjectDB" });
            hcBuilder.AddMySql(
               configuration.GetConnectionString("DefaultConnection"),
               name: "YzProjectDB-check",
               tags: new string[] { "YzProjectDB" });

            hcBuilder
                .AddRedis(
                    configuration["RedisConnectionString"],
                    name: "redis-check",
                    tags: new string[] { "redis" });

            hcBuilder
                    .AddRabbitMQ(
                        $"amqp://{configuration["EventBusConnection"]}",
                        name: "basket-rabbitmqbus-check",
                        tags: new string[] { "rabbitmqbus" });

            #region

            //健康检查服务
            //services.AddHealthChecks()
            //    .AddMemoryHealthCheck("memory_check")  //.AddCheck<MemoryHealthCheck>("memory_check")
            //    .AddPrivateMemoryHealthCheck(1000_000_000L)   //最大私有内存不超过1GB
            //                                                  //.AddVirtualMemorySizeHealthCheck(1000_000_000L)   //最大虚拟内存不超过1GB
            //    .AddWorkingSetHealthCheck(1000_000_000L)   //最大工作内存不超过1GB
            //                                               //.AddDiskStorageHealthCheck(x => x.AddDrive("c", 1000_000_000L)) //C盘需要超过1GB自由空间;
            //    .AddCheck("disk", () =>
            //    {
            //        var sp = Utils.GetHardDiskSpace("C");
            //        if (sp > 1024 * 10)
            //        {
            //            return HealthCheckResult.Healthy();
            //        }
            //        return HealthCheckResult.Unhealthy();
            //    })
            //    //.AddAsyncCheck("Http", async () =>
            //    //{
            //    //    using (HttpClient client = new HttpClient())
            //    //    {
            //    //        try
            //    //        {
            //    //            var response = await client.GetAsync("http://localhost:9100/api/values");
            //    //            if (!response.IsSuccessStatusCode)
            //    //            {
            //    //                throw new Exception("Url not responding with 200 OK");
            //    //            }
            //    //        }
            //    //        catch (Exception)
            //    //        {
            //    //            return await Task.FromResult(HealthCheckResult.Unhealthy());
            //    //        }
            //    //    }
            //    //    return await Task.FromResult(HealthCheckResult.Healthy());
            //    //})
            //    //.AddMySql(EnvironmentVariables.DBConnection)
            //    .AddRedis("127.0.0.1:6379,defaultDatabase=0,password=sino#2019")
            //    .AddRabbitMQ("host=127.0.0.1:5672");

            #endregion

            return services;
        }
    }
}
