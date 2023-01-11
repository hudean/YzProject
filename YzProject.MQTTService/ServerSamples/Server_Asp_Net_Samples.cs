//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using MQTTnet.AspNetCore;
//using MQTTnet.Server;

//namespace YzProject.MQTTService.ServerSamples
//{
//    public static class Server_Asp_Net_Samples
//    {
//        public static Task Start_Server_With_WebSockets_Support()
//        {
//            /*
//             * This sample starts a minimal ASP.NET Webserver including a hosted MQTT server.
//             */
//            var host = Host.CreateDefaultBuilder(Array.Empty<string>())
//                .ConfigureWebHostDefaults(
//                    webBuilder =>
//                    {
//                        webBuilder.UseKestrel(
//                            o =>
//                            {
//                                // This will allow MQTT connections based on TCP port 1883.
//                                o.ListenAnyIP(1883, l => l.UseMqtt());

//                                // This will allow MQTT connections based on HTTP WebSockets with URI "localhost:5000/mqtt"
//                                // See code below for URI configuration.
//                                o.ListenAnyIP(5000); // Default HTTP pipeline
//                            });

//                        webBuilder.UseStartup<Startup>();
//                    });

//            return host.RunConsoleAsync();
//        }

//        sealed class MqttController
//        {
//            public MqttController()
//            {
//                // Inject other services via constructor.
//            }

//            public Task OnClientConnected(ClientConnectedEventArgs eventArgs)
//            {
//                Console.WriteLine($"Client '{eventArgs.ClientId}' connected.");
//                return Task.CompletedTask;
//            }


//            public Task ValidateConnection(ValidatingConnectionEventArgs eventArgs)
//            {
//                Console.WriteLine($"Client '{eventArgs.ClientId}' wants to connect. Accepting!");
//                return Task.CompletedTask;
//            }
//        }

//        sealed class Startup
//        {
//            public void Configure(IApplicationBuilder app, IWebHostEnvironment environment, MqttController mqttController)
//            {
//                app.UseRouting();

//                app.UseEndpoints(
//                    endpoints =>
//                    {
//                        endpoints.MapConnectionHandler<MqttConnectionHandler>(
//                            "/mqtt",
//                            httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
//                                protocolList => protocolList.FirstOrDefault() ?? string.Empty);
//                    });

//                app.UseMqttServer(
//                    server =>
//                    {
//                        /*
//                         * Attach event handlers etc. if required.
//                         */

//                        server.ValidatingConnectionAsync += mqttController.ValidateConnection;
//                        server.ClientConnectedAsync += mqttController.OnClientConnected;
//                    });
//            }

//            public void ConfigureServices(IServiceCollection services)
//            {
//                services.AddHostedMqttServer(
//                    optionsBuilder =>
//                    {
//                        optionsBuilder.WithDefaultEndpoint();
//                    });

//                services.AddMqttConnectionHandler();
//                services.AddConnections();

//                services.AddSingleton<MqttController>();
//            }
//        }
//    }
//}
