using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.MQTTService
{
    internal class MqttHelper4
    {
     
        /// <summary>
        /// 客户端
        /// </summary>
        /// <returns></returns>
        public static async Task Connect_Client()
        {
            await Task.CompletedTask;
            var mqttFactory = new MqttFactory();
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                //使用Build构建
                //var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("broker.hivemq.com").Build();

                //// 如果服务器不可用，这将引发异常。
                //// 此消息的结果返回从服务器发送的其他数据。详情请参考 MQTT 协议规范。
                //var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                //Console.WriteLine("The MQTT client is connected.");

                //response.DumpToConsole();

                //// 通过调用 _DisconnectAsync_ 向服务器发送干净的断开连接。
                //// 否则，TCP 连接将被丢弃，服务器将将其作为非干净断开连接处理（有关详细信息，请参阅 MQTT 规范）。
                //var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

                //await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);


                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("127.0.0.1", 1883)
                    .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                    .WithClientId("clientid_pascalming")
                    .WithCleanSession(false)
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(30))
                    .WithCredentials("user", "password")
                    .Build();

                //与3.1对比，事件订阅名称和接口已经变化
                mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
                mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
                mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
                var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            }
        }

        private static Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private static Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine($"Mqtt客户端连接成功.");
            return Task.CompletedTask;
        }

        private static Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine($"Mqtt客户端连接断开");
            return Task.CompletedTask;
        }
    }
}
