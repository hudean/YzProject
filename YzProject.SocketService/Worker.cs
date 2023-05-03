using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YzProject.Common;
using YzProject.SocketService.SuperSocketHelper;

namespace YzProject.SocketService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        //private AppSocketServer server;
        private readonly IApplicationLifetime _applicationLifetime;
        private bool isdispose = false;
        private Crypto crypto = new Crypto();
        //private Dictionary<string, int> ClientList = new Dictionary<string, int>();
        //private Dictionary<int, string> ClientIds = new Dictionary<int,string>();
        private readonly ILoggerFactory _loggerFactory;
        public Worker(ILogger<Worker> logger, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _loggerFactory = loggerFactory;
           // server = new AppSocketServer(logger, loggerFactory);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);

            //}
            await Task.Run(() =>
            {
                try
                {
                    StartServer();
                    //while (true)
                    //{
                    //    server = new SocketServer(_console, _loggerFactory);
                    //    server.Start();
                    //    Task.Delay(120000).Wait();
                    //    server.Dispose();
                    //    Task.Delay(10000).Wait();
                    //}

                }
                catch (Exception ex)
                {
                    //_console.LogError(ex.ToString());
                    //Utils.WriteServiceLog("SocketHostedService", "StartAsync", ex.ToString(), true, "");
                }
            });
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Dispose();
            });
        }


        public override void Dispose()
        {
            //if (!isdispose)
            //{
            //    server.Dispose();
            //    isdispose = true;
            //    _applicationLifetime.StopApplication();
            //}
            base.Dispose();
        }

        private void StartServer()
        {
            try
            {
                //server.Start();
                //isdispose = false;
            }
            catch (Exception ex)
            {
                //_console.LogError(ex.ToString());
                //Utils.WriteServiceLog("SocketHostedService", "StartServer", ex.ToString(), true, "");
                StartServer();
            }
            //finally
            //{
            //    StartServer();
            //}
        }
    }
}
