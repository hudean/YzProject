using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MQTTClientTest
{
    public class MqttClientService2
    {
        public static IMqttClient mqttClient;
        public MqttClientService2()
        { 
        
        }

        public void MqttClientStart()
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1",10086)//要访问的mqtt服务端的ip和端口号
                .WithCredentials("admin","123456")//要访问的mqtt服务端的用户名和密码
                .WithClientId("testClient01")//设置客户端id
                .WithCleanSession()
                .WithTls(new MqttClientOptionsBuilderTlsParameters { 
                    UseTls = false //是否使用tls加密
                });

            var clientOptions = optionsBuilder.Build();
            mqttClient = new MqttFactory().CreateMqttClient();

            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;//客户端连接成功事件
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;//客户端连接关闭事件
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;//收到消息事件
           
            mqttClient.ConnectAsync(clientOptions);

        }

        /// <summary>
        /// 收到消息事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine($"ApplicationMessageReceivedAsync：客户端ID=【{arg.ClientId}】接收到消息。 Topic主题=【{arg.ApplicationMessage.Topic}】 消息=【{Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}】 qos等级=【{arg.ApplicationMessage.QualityOfServiceLevel}】");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine($"客户端已断开与服务端的连接……");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 客户端连接成功事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine($"客户端已连接服务端……");
           
            // 订阅消息主题
            //主题订阅的内容写到连接成功以后的事件里面，不然客户端连接期间，可能就执行了主题订阅，会存在订阅失败的情况。
            //改为写入到连接成功以后的事件里面，可以保证主题订阅肯定是在客户端连接成功以后才执行的。

            // MqttQualityOfServiceLevel: （QoS）:  0 最多一次，接收者不确认收到消息，并且消息不被发送者存储和重新发送提供与底层 TCP 协议相同的保证。
            // 1: 保证一条消息至少有一次会传递给接收方。发送方存储消息，直到它从接收方收到确认收到消息的数据包。一条消息可以多次发送或传递。
            // 2: 保证每条消息仅由预期的收件人接收一次。级别2是最安全和最慢的服务质量级别，保证由发送方和接收方之间的至少两个请求/响应（四次握手）。
            mqttClient.SubscribeAsync("topic_01", MqttQualityOfServiceLevel.AtLeastOnce);
            mqttClient.UnsubscribeAsync("topic_01");//取消订阅
            mqttClient.SubscribeAsync("topic_01", MqttQualityOfServiceLevel.AtLeastOnce);//恢复订阅
            //无论是不是本客户端发送得数据只要是topic_01主题，都会订阅
            return Task.CompletedTask;
        }

        public void Publish(string data)
        {
            var message = new MqttApplicationMessage
            {
                Topic = "topic_01",  //本客户端也会接受该消息
                //Topic = "topic_02",    //本客户端不会接受该消息
                Payload = Encoding.Default.GetBytes(data),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                Retain = true  // 服务端是否保留消息。true为保留，如果有新的订阅者连接，就会立马收到该消息。
            };
            mqttClient.PublishAsync(message);
        }
    }
}
