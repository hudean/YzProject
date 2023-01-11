using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Packets;

namespace MQTTClientTest
{
    public class MqttClientService
    {
        private readonly string _clientId;
        private readonly string _ip;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _pwd;
        public MqttClientService(string clientId, string ip, int port, string userName, string pwd)
        {
            _clientId = clientId;
            _ip = ip;
            _port = port;
            _userName = userName;
            _pwd = pwd;
        }

        private IMqttClient mqttClient = null;

        public async Task CreateMqttClient()
        {
            //实例化 创建客户端对象
            //客户端支持 Connected、Disconnected 和 ApplicationMessageReceived 事件，
            //用来处理客户端与服务端连接、客户端从服务端断开以及客户端收到消息的事情。
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            //mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            //mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            //mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            //连接到服务器前，获取所需要的MqttClientTcpOptions 对象的信息
            var options = new MqttClientOptionsBuilder()
            .WithClientId(_clientId)                    // clientid是设备id
            .WithTcpServer(_ip, _port)              //onenet ip：183.230.40.39    port:6002
            .WithCredentials(_userName, _pwd)      //username为产品id       密码为鉴权信息或者APIkey
            //.WithTls()//服务器端没有启用加密协议，这里用tls的会提示协议异常
            .WithCleanSession(false)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(2000))
            .Build();

            //应用程序消息接收
            mqttClient.ApplicationMessageReceivedAsync +=  async (e) =>
            {
                //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###"); 
                Console.WriteLine("### 收到的应用程序消息 ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
                await Task.CompletedTask;
            };

            mqttClient.ConnectedAsync += async (e) =>
            {
                await Task.CompletedTask;
                //Console.WriteLine("### CONNECTED WITH SERVER, SUBSCRIBING ###");
                Console.WriteLine("### 已连接服务器，订阅 ###");
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("my/topic").Build());
            };

            mqttClient.DisconnectedAsync += async (e) =>
            {
                //Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                Console.WriteLine("### 与服务器断开连接 ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                    Console.WriteLine("### 重新连接失败 ###");
                }
            };

            try
            {
                await mqttClient.ConnectAsync(options);
            }
            catch
            {
                Console.WriteLine("### CONNECTING FAILED ###");
                Console.WriteLine("### 连接失败 ###");
            }

            //Console.WriteLine("### WAITING FOR APPLICATION MESSAGES ###");
            Console.WriteLine("### 等待应用程序消息 ###");
            var messageFactory = new MqttApplicationMessageFactory();
            while (true)
            {
                //Console.ReadLine();
                Thread.Sleep(3000);
                //var applicationMessage = messageFactory.CreateApplicationMessage("myTopic", "Hello World", MqttQualityOfServiceLevel.AtLeastOnce);
                //await mqttClient.PublishAsync(applicationMessage);
                MqttPublishPacket mqttPublishPacket = new MqttPublishPacket()
                {
                    Topic = "MyTopic",
                    Payload = Encoding.UTF8.GetBytes("我是客户端发送的信息"),
                    ContentType= "UTF8",
                    Retain = true,
                };
                var message = messageFactory.Create(mqttPublishPacket);
                //var message = new MqttApplicationMessageBuilder()
                //.WithTopic("MyTopic")
                //.WithPayload("Hello World")
                //.WithRetainFlag()
                //.Build();
                Console.WriteLine("PublishAsync");
                await mqttClient.PublishAsync(message, CancellationToken.None);
            }

            //调用异步方法连接到服务端
           // await mqttClient.ConnectAsync(options);
            //await PublishAsync();
        }


        #region

        /// <summary>
        /// Mqtt客户端收到消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            //Console.WriteLine($"Mqtt客户端消息接收.");
            //string topic = arg.ApplicationMessage.Topic.ToLower();
            //Console.WriteLine("接受消息");
            //Console.WriteLine(topic);
            //string payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            //Console.WriteLine(payload);
            //return Task.CompletedTask;
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {arg.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {arg.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {arg.ApplicationMessage.Retain}");
            Console.WriteLine();

            await PublishAsync();
        }

        /// <summary>
        /// Mqtt客户端连接成功
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine($"Mqtt客户端连接成功.");
            Console.WriteLine("### CONNECTED WITH SERVER ###");
            // Subscribe to a topic
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("my/topic").Build());
            Console.WriteLine("### SUBSCRIBED ###");
        }

        /// <summary>
        /// Mqtt客户端连接断开
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine($"Mqtt客户端连接断开");
            return Task.CompletedTask;
        }

        #endregion
        public async Task  PublishAsync()
        {
            Console.WriteLine($"Mqtt客户端发送数据.");
            //MqttApplicationMessage mqttApplicationMessage = new MqttApplicationMessage()
            //{
            //     ContentType= "UTF-8",
            //     Payload = Encoding.UTF8.GetBytes("我是一个客户端1发送的数据"),
            //     Topic= "send/server"

            //};
            //await mqttClient.PublishAsync(mqttApplicationMessage);
            var message = new MqttApplicationMessageBuilder()
            .WithTopic("MyTopic")
            .WithPayload("Hello World")
            .WithRetainFlag()
            .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }

    }
}
