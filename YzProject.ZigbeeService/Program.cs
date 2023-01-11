using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace YzProject.ZigbeeService
{
    public class Program
    {
        // 控制台程序 添加Microsoft.Extensions.Hosting包
        // 也可以直接创建一个 worker service 程序
        // https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0
        // https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio

        public static void Main(string[] args)
        {
            #region 第一种

            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    //var temp = context.Configuration["AllowedHosts"];
                    context.HostingEnvironment.ApplicationName = "YzProject.ZigbeeService";
                    builder.Sources.Clear();
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json");
                    builder.AddCommandLine(args);
                })
                 .ConfigureServices((context, services) =>
                 {
                     //添加后台服务
                     services.AddHostedService<ZigbeeHostedService>();
                 })
                 .ConfigureLogging((logging) =>
                 {
                     //安装包Microsoft.Extensions.Logging
                     //安装包Microsoft.Extensions.Logging.Console
                     //安装包Microsoft.Extensions.Logging.Debug
                     logging.AddConsole();
                     logging.AddDebug();
                     //Log Level
                     logging.SetMinimumLevel(LogLevel.Error);
                 })
                 .UseConsoleLifetime()
                 .Build();
            //using (var serviceScope = host.Services.CreateScope())
            //{
            //    var service = serviceScope.ServiceProvider;
            //    var configuration = service.GetRequiredService<IConfiguration>();
            //}
            host.Run();

            #endregion

            #region 第二种
            //CreateHostBuilder(args).Build().Run();
            #endregion

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) => {
                    configuration.Sources.Clear();
                    IHostEnvironment env = hostingContext.HostingEnvironment;
                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                    IConfigurationRoot configurationRoot = configuration.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ZigbeeHostedService>();
                });
    }
}
