using MQTTnet.Server;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Internal;

namespace YzProject.MQTTService.ServerSamples
{
    /// <summary>
    /// 服务端拦截样本
    /// </summary>
    public static class Server_Intercepting_Samples
    {
        /// <summary>
        /// 拦截应用程序消息
        /// </summary>
        /// <returns></returns>
        public static async Task Intercept_Application_Messages()
        {
            /*
             * This sample starts a simple MQTT server which manipulate all processed application messages.
             * Please see _Server_Simple_Samples_ for more details on how to start a server.
             */

            var mqttFactory = new MqttFactory();
            var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

            using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
            {
                mqttServer.InterceptingPublishAsync += args =>
                {
                    // Here we only change the topic of the received application message.
                    // but also changing the payload etc. is required. Changing the QoS after
                    // transmitting is not supported and makes no sense at all.
                    args.ApplicationMessage.Topic += "/manipulated";

                    return CompletedTask.Instance;
                };

                await mqttServer.StartAsync();

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                await mqttServer.StopAsync();
            }
        }
    }
}
