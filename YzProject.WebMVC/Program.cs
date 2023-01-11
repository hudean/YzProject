using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YzProject.Domain;

namespace YzProject.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<YzProjectContext>();
                    //正式开发不用这么做推荐使用迁移命令
                    var isTrue = context.Database.EnsureCreated();
                    if (isTrue)
                    {
                       DbInitializer.Initialize(context);
                    }

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning))//如果未在配置中设置默认日志级别，以下代码会设置默认日志级别：
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        //注入log4net日志其中一种方式
        //.ConfigureLogging((hostingContext, logging) =>
        //{
        //    // The ILoggingBuilder minimum level determines the
        //    // the lowest possible level for logging. The log4net
        //    // level then sets the level that we actually log at.
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
        //    logging.SetMinimumLevel(LogLevel.Debug);
        //});
    }
}
