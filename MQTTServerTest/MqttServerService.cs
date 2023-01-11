using MQTTnet.Server;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Protocol;
using MQTTnet.Client;
using MQTTnet.Packets;
using System.Threading;

namespace MQTTServerTest
{
    public class MqttServerService
    {
       
        public MqttServerService()
        { 
        
        }
        public MqttServer mqttServer;

        const string ServerClientId = "Server_01";
        //https://github.com/dotnet/MQTTnet/wiki/Server
        public async Task StartServer()
        {
            var mqttFactory = new MqttFactory();
            //MqttServerOptions mqttServerOptions = new MqttServerOptions();
            var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
            #region 另一种写法
            //MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder();
            //optionsBuilder.WithDefaultEndpoint();
            //optionsBuilder.WithDefaultEndpointPort(10086);//设置服务端 端口号
            //optionsBuilder.WithConnectionBacklog(1000);//最大连接数
            //MqttServerOptions options = optionsBuilder.Build();
            #endregion
            mqttServerOptions.DefaultCommunicationTimeout = new TimeSpan(0, 0, 15);
            // 设置服务器端地址和端口号
            mqttServerOptions.DefaultEndpointOptions.Port = 10086;
            mqttServerOptions.DefaultEndpointOptions.ConnectionBacklog = 1000;
            mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
            #region 一些事件
            // 设置客户端连接成功后的处理程序
            mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;//客户端连接事件
            // 设置客户端断开后的处理程序
            mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;//客户端关闭事件
            //消息接受事件
            mqttServer.ApplicationMessageNotConsumedAsync += MqttServer_ApplicationMessageNotConsumedAsync;
            // 设置消息订阅通知
            mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;//客户端订阅主题事件
            // 设置消息退订通知
            mqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;//客户端取消订阅主题事件
            //启动后事件
            mqttServer.StartedAsync += MqttServer_StartedAsync;
            //停止后事件
            mqttServer.StoppedAsync += MqttServer_StoppedAsync;
            // 消息接收事件
            mqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;
            //用户名和密码验证相关
            mqttServer.ValidatingConnectionAsync += e =>
            {
                //if (e.ClientId != "ValidClientId")
                //{
                //    e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                //}
                //ValidUser
                if (e.UserName != "admin") 
                {
                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                }
                //SecretPassword
                if (e.Password != "123456")
                {
                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                }
                //e.ReasonCode = MqttConnectReasonCode.Success;
                return Task.CompletedTask;
            };
           
            // 设置消息处理程序
            //mqttServer.LoadingRetainedMessageAsync += MqttServer_LoadingRetainedMessageAsync;
            #endregion
            await mqttServer.StartAsync();

            //while (true)
            //{
            //    Thread.Sleep(5000);
            //    var message = new MqttApplicationMessageBuilder()
            //    .WithTopic("my/topic")
            //    .WithPayload("我是服务端发送的数据")
            //    .WithRetainFlag()
            //    .Build();
            //    Console.WriteLine("PublishAsync");
            //    //await mqttServer.PublishAsync(message, CancellationToken.None);
            //    await mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message)
            //    {
            //        SenderClientId = "ValidClientId"
            //    });

            //}


        }


        #region 事件

        /// <summary>
        /// 消息接收事件，接受所有客户端包括服务端发出得消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            //如果是ClientId本服务器id就不接受
            if (string.Equals(arg.ClientId, ServerClientId))
            {
                return Task.CompletedTask;
            }

            Console.WriteLine($"InterceptingPublishAsync：客户端ID=【{arg.ClientId}】 Topic主题=【{arg.ApplicationMessage.Topic}】 消息=【{Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}】 qos等级=【{arg.ApplicationMessage.QualityOfServiceLevel}】");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 关闭后事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttServer_StoppedAsync(EventArgs arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine("StoppedAsync:MQTT服务已关闭...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动后事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttServer_StartedAsync(EventArgs arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine("StartedAsync:MQTT服务已启动...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 消息接收事件，只接受主题没有被订阅的消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task MqttServer_ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
        {
            //throw new NotImplementedException();
            string payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine($"ApplicationMessageNotConsumedAsync: 发送端ID=[{arg.SenderId}],Topic主题=[{arg.ApplicationMessage.Topic}],消息=[{payload}],qos等级=[{arg.ApplicationMessage.QualityOfServiceLevel}]");
            return Task.CompletedTask;
        }

        private async Task MqttServer_LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs arg)
        {
            //throw new NotImplementedException();
            //Console.WriteLine(arg.LoadedRetainedMessages)
            // 获取消息的客户端识别码
            //var megs = arg.LoadedRetainedMessages;
            //string clientId = arg.ClientId;
            //// 获取消息的主题
            //string topic = args.ApplicationMessage.Topic;
            //// 获取发送的消息内容
            //string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            //// 获取消息的发送级别(Qos)
            //var qos = args.ApplicationMessage.QualityOfServiceLevel;
            //// 获取消息的保持形式
            //bool retain = args.ApplicationMessage.Retain;
            var list = arg.LoadedRetainedMessages;
            foreach (var item in list)
            {
                // 获取消息的主题
                string topic = item.Topic.ToLower();
                Console.WriteLine("接受消息");
                Console.WriteLine(topic);
                //var gb2312 = Encoding.GetEncoding("GB2312");
                //var bts = Encoding.Convert(gb2312, Encoding.UTF8, bytes);
                //string payload = Encoding.UTF8.GetString(bts);
                // 获取发送的消息内容
                string payload = Encoding.UTF8.GetString(item.Payload);
                Console.WriteLine(payload);
            }



            //Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 客户端取消订阅主题事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
        {
            //throw new NotImplementedException();
            // 获取客户端识别码
            string clientId = arg.ClientId;
            // 获取客户端发起的订阅主题
            string topic = arg.TopicFilter;

            Console.WriteLine($"ClientUnsubscribedTopicAsync：客户端ID=[{clientId}]已退订主题:[{topic}]");
            //Console.WriteLine($"ClientUnsubscribedTopicAsync：客户端ID=【{arg.ClientId}】已取消订阅的主题=【{arg.TopicFilter}】  ");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 客户端订阅主题事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            //throw new NotImplementedException();
            // 获取客户端识别码
            string clientId = arg.ClientId;
            // 获取客户端发起的订阅主题
            string topic = arg.TopicFilter.Topic;

            Console.WriteLine($"ClientSubscribedTopicAsync: 客户端ID=[{clientId}]已订阅主题=[{topic}]");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 客户端连接成功事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            await Task.CompletedTask;
            // 获取客户端识别码
            string clientId = arg.ClientId;
            //ReplyMsg("客户端连接成功服务端通知", clientId);
            //Console.WriteLine($"ClientConnectedAsync：客户端ID=[{clientId}]");
            //ICollection<MqttTopicFilter> list = new List<MqttTopicFilter>() { new MqttTopicFilterBuilder().WithTopic("MyTopic").Build() };
            //await mqttServer.SubscribeAsync(clientId, list);

            Console.WriteLine($"ClientConnectedAsync：客户端ID=【{arg.ClientId}】已连接, 用户名=【{arg.UserName}】地址=【{arg.Endpoint}】  ");
        }

        /// <summary>
        /// 客户端断开连接事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            //throw new NotImplementedException();
            await Task.CompletedTask;
            // 获取客户端识别码
            string clientId = arg.ClientId;
            //Console.WriteLine($"ClientDisconnectedAsync：客户端ID=[{clientId}]");
            Console.WriteLine($"ClientDisconnectedAsync：客户端ID=【{arg.ClientId}】已断开, 地址=【{arg.Endpoint}】  ");

        }


        /// <summary>
        /// 用户名和密码验证有关
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            arg.ReasonCode = MqttConnectReasonCode.Success;
            if ((arg.UserName ?? string.Empty) != "admin" || (arg.Password ?? String.Empty) != "123456")
            {
                arg.ReasonCode = MqttConnectReasonCode.Banned;
                Console.WriteLine($"ValidatingConnectionAsync：客户端ID=【{arg.ClientId}】用户名或密码验证错误 ");

            }
            return Task.CompletedTask;
        }

        private void ReplyMsg(string msg, string clientId)
        {
            string topic = $"mcu/{clientId}";
            var bytes = Encoding.UTF8.GetBytes(msg);
            var message = new MqttApplicationMessageBuilder()
                  .WithTopic(topic)
                  .WithPayload(bytes)
                  .Build();
            mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message)
            {
                SenderClientId = clientId
            }).Wait();
        }

        #endregion
    }
}
