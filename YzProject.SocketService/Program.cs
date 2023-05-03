using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YzProject.EventBus.Abstractions;
using YzProject.SocketService.IntegrationEvents.EventHandling;
using YzProject.SocketService.IntegrationEvents.Events;

namespace YzProject.SocketService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });



        ///// <summary>
        ///// �����¼�����
        ///// </summary>
        ///// <param name="app"></param>
        //private void ConfigureEventBus(IApplicationBuilder app)
        //{
        //    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        //    //���ö����¼�
        //    eventBus.Subscribe<R_SMessageEvent, R_SMessageEventHandle>();
        //}
    }
}
