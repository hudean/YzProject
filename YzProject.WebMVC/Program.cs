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
                    //��ʽ����������ô���Ƽ�ʹ��Ǩ������
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
                // .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning))//���δ������������Ĭ����־�������´��������Ĭ����־����
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        //ע��log4net��־����һ�ַ�ʽ
        //.ConfigureLogging((hostingContext, logging) =>
        //{
        //    // The ILoggingBuilder minimum level determines the
        //    // the lowest possible level for logging. The log4net
        //    // level then sets the level that we actually log at.
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
        //    logging.SetMinimumLevel(LogLevel.Debug);
        //});
    }
}
