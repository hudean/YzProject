using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.MQTTService
{
    public class MQTTHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<MQTTHostedService> _logger;
        //private readonly IApplicationLifetime _applicationLifetime;
        private MqttServer mqttServer;
        //private readonly MqttServerOptions mqttServerOptions;
        private bool isDispose = false;

        private readonly string ServerClientId = "Server_01";

        public MQTTHostedService(ILogger<MQTTHostedService> logger)
            //, IApplicationLifetime applicationLifetime)
        {
            _logger = logger;

            #region
            //var mqttFactory = new MqttFactory();
            //mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
            //mqttServerOptions.DefaultCommunicationTimeout = new TimeSpan(0, 0, 15);
            //// 设置服务器端地址和端口号
            //mqttServerOptions.DefaultEndpointOptions.Port = 8001;
            //mqttServer  = mqttFactory.CreateMqttServer(mqttServerOptions);

            //mqttServer.ValidatingConnectionAsync += e =>
            //{
            //    //if (e.ClientId != "ValidClientId")
            //    //{
            //    //    e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            //    //}

            //    //if (e.UserName != "ValidUser")
            //    //{
            //    //    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            //    //}

            //    //if (e.Password != "SecretPassword")
            //    //{
            //    //    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            //    //}
            //     e.ReasonCode = MqttConnectReasonCode.Success;
            //    return Task.CompletedTask;
            //};
            //// 设置消息订阅通知
            //mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
            //// 设置消息退订通知
            //mqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
            //// 设置消息处理程序
            //mqttServer.LoadingRetainedMessageAsync += MqttServer_LoadingRetainedMessageAsync;
            //// 设置客户端连接成功后的处理程序
            //mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
            //// 设置客户端断开后的处理程序
            //mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            #endregion
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                //await mqttServer.StartAsync();
                //isDispose = false;
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
              
                await mqttServer.StartAsync();
                isDispose = false;
                // await Utils.WriteServiceLog("MQTTHostedService", "StartAsync", "启动成功", false, "");


                #region
                //// 1. 创建 MqttFactory
                //var mqttFactory = new MqttFactory();
                //#region 使用日志记录运行服务器
                ////此示例启动一个简单的 MQTT 服务器并将日志打印到输出。
                ////重要！不要在实时环境中启用日志记录。它会降低性能。
                ////有关更多详细信息，请参阅示例“运行最小服务器”。

                ////var mqttFactory = new MqttFactory(new ConsoleLogger());
                //#endregion

                //// 出于安全原因，默认情况下不启用“默认”端点（未加密）！
                ////2.创建 MqttServerOptions 的实例，用来定制 MQTT 的各种参数
                //var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
                //// 3. 设置各种选项
                //// 设置服务器端地址和端口号
                //mqttServerOptions.DefaultEndpointOptions.Port = 8001;

                //// 4. 创建 MqttServer 实例
                //var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

                //#region 验证连接/客户端鉴权
                ////在启动服务器之前设置连接验证，以便在没有有效凭据的情况下不会更改连接。
                //mqttServer.ValidatingConnectionAsync += e =>
                //{
                //    if (e.ClientId != "ValidClientId")
                //    {
                //        e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                //    }

                //    if (e.UserName != "ValidUser")
                //    {
                //        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                //    }

                //    if (e.Password != "SecretPassword")
                //    {
                //        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                //    }

                //    return Task.CompletedTask;
                //};

                //#endregion

                //// 5. 设置 MqttServer 的属性
                //// 设置消息订阅通知
                //mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
                //// 设置消息退订通知
                //mqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
                //// 设置消息处理程序
                ////mqttServer.UseApplicationMessageReceivedHandler(MessageReceived);
                //mqttServer.LoadingRetainedMessageAsync += MqttServer_LoadingRetainedMessageAsync;
                //// 设置客户端连接成功后的处理程序
                //mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
                //// 设置客户端断开后的处理程序
                //mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;

                //// 启动服务器
                //await mqttServer.StartAsync();
                #endregion
            }
            catch (Exception ex)
            {
                //await Utils.WriteServiceLog("MQTTHostedService", "StartAsync", ex.InnerException == null ? ex.Message : ex.InnerException.Message, true, "");
            }

            #region 3.1.0
            //    return Task.Run(async () =>
            //    {
            //        try
            //        {

            //            // 1. 创建 MQTT 连接验证，用于连接鉴权
            //            MqttServerConnectionValidatorDelegate connectionValidatorDelegate = new MqttServerConnectionValidatorDelegate(
            //    p =>
            //    {
            //        // p 表示正在发起的一个链接的上下文

            //        // 客户端 id 验证
            //        // 大部分情况下，我们应该使用设备识别号来验证
            //        if (p.ClientId == "twle_client")
            //        {
            //            // 用户名和密码验证
            //            // 大部分情况下，我们应该使用客户端加密 token 验证，也就是可客户端 ID 对应的密钥加密后的 token
            //            if (p.Username != "yufei" && p.Password != "123456")
            //            {
            //                // 验证失败，告诉客户端，鉴权失败
            //                p.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
            //            }
            //        }
            //    }
            //);


            //            // 2. 创建 MqttServerOptions 的实例，用来定制 MQTT 的各种参数
            //            MqttServerOptions options = new MqttServerOptions();

            //            // 3. 设置各种选项
            //            // 客户端鉴权
            //            options.ConnectionValidator = connectionValidatorDelegate;

            //            // 设置服务器端地址和端口号
            //            options.DefaultEndpointOptions.Port = 8001;

            //            // 4. 创建 MqttServer 实例
            //            var mqttServer = new MqttFactory().CreateMqttServer();

            //            // 5. 设置 MqttServer 的属性
            //            // 设置消息订阅通知
            //            mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(SubScribedTopic);
            //            // 设置消息退订通知
            //            mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(UnScribedTopic);
            //            // 设置消息处理程序
            //            mqttServer.UseApplicationMessageReceivedHandler(MessageReceived);
            //            // 设置客户端连接成功后的处理程序
            //            mqttServer.UseClientConnectedHandler(ClientConnected);
            //            // 设置客户端断开后的处理程序
            //            mqttServer.UseClientDisconnectedHandler(ClientDisConnected);

            //            // 启动服务器
            //            await mqttServer.StartAsync(options);


            //            Console.WriteLine("服务器启动成功！直接按回车停止服务");
            //            Console.ReadLine();

            //            //await Utils.PostProcessInfo(EnumDeviceServices.DeviceManager.ToString());
            //            //await Utils.WriteServiceLog("MQTTHostedService", "StartAsync", "启动成功", false, "");
            //        }
            //        catch (Exception ex)
            //        {
            //            //Utils.WriteMsg($"【异常】：{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}", "StartAsync", $"MQTTHostedService");
            //            //await Utils.WriteServiceLog("MQTTHostedService", "StartAsync", ex.InnerException == null ? ex.Message : ex.InnerException.Message, true, "");
            //        }
            //    });
            #endregion
        }

        private Task MqttServer_LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs arg)
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

            //Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");
            return  Task.CompletedTask;
        }

        #region


        ///// <summary>
        ///// 客户端取消主题订阅通知
        ///// </summary>
        ///// <param name="arg"></param>
        ///// <returns></returns>
        //private Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
        //{
        //    //throw new NotImplementedException();
        //    // 获取客户端识别码
        //    string clientId = arg.ClientId;
        //    // 获取客户端发起的订阅主题
        //    string topic = arg.TopicFilter;

        //    Console.WriteLine($"客户端[{clientId}]已退订主题:{topic}");
        //    return Task.CompletedTask;
        //}

        ///// <summary>
        ///// 客户端发起订阅主题通知
        ///// </summary>
        ///// <param name="arg"></param>
        ///// <returns></returns>
        //private Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        //{
        //    //throw new NotImplementedException();
        //    // 获取客户端识别码
        //    string clientId = arg.ClientId;
        //    // 获取客户端发起的订阅主题
        //    string topic = arg.TopicFilter.Topic;

        //    Console.WriteLine($"客户端[{clientId}]已订阅主题:{topic}");
        //    return Task.CompletedTask;
        //}

        ///// <summary>
        ///// 客户端连接成功后的的处理通知
        ///// </summary>
        ///// <param name="arg"></param>
        ///// <returns></returns>
        //private Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        //{
        //    //throw new NotImplementedException();
        //    // 获取客户端识别码
        //    string clientId = arg.ClientId;

        //    Console.WriteLine($"新客户端[{clientId}] 加入");
        //    return Task.CompletedTask;
        //}

        ///// <summary>
        ///// 客户端断开连接通知
        ///// </summary>
        ///// <param name="arg"></param>
        ///// <returns></returns>
        //private Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        //{
        //    //throw new NotImplementedException();
        //    // 获取客户端识别码
        //    string clientId = arg.ClientId;

        //    Console.WriteLine($"新客户端[{clientId}] 已经离开");
        //    return Task.CompletedTask;
        //}

        #endregion

        #region 3.1.0

        //// 客户端发起订阅主题通知
        //private static void SubScribedTopic(MqttServerClientSubscribedTopicEventArgs args)
        //{
        //    // 获取客户端识别码
        //    string clientId = args.ClientId;
        //    // 获取客户端发起的订阅主题
        //    string topic = args.TopicFilter.Topic;

        //    Console.WriteLine($"客户端[{clientId}]已订阅主题:{topic}");
        //}

        //// 客户端取消主题订阅通知
        //private static void UnScribedTopic(MqttServerClientUnsubscribedTopicEventArgs args)
        //{
        //    // 获取客户端识别码
        //    string clientId = args.ClientId;
        //    // 获取客户端发起的订阅主题
        //    string topic = args.TopicFilter;

        //    Console.WriteLine($"客户端[{clientId}]已退订主题:{topic}");
        //}

        //// 接收客户端发送的消息
        //private static void MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        //{
        //    // 获取消息的客户端识别码
        //    string clientId = args.ClientId;
        //    // 获取消息的主题
        //    string topic = args.ApplicationMessage.Topic;
        //    // 获取发送的消息内容
        //    string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
        //    // 获取消息的发送级别(Qos)
        //    var qos = args.ApplicationMessage.QualityOfServiceLevel;
        //    // 获取消息的保持形式
        //    bool retain = args.ApplicationMessage.Retain;

        //    Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");

        //}

        //// 客户端连接成功后的的处理通知
        //private static void ClientConnected(MqttServerClientConnectedEventArgs args)
        //{
        //    // 获取客户端识别码
        //    string clientId = args.ClientId;

        //    Console.WriteLine($"新客户端[{clientId}] 加入");
        //}

        //// 客户端断开连接通知
        //private static void ClientDisConnected(MqttServerClientDisconnectedEventArgs args)
        //{
        //    // 获取客户端识别码
        //    string clientId = args.ClientId;

        //    Console.WriteLine($"新客户端[{clientId}] 已经离开");
        //}



        #endregion

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
        private Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            //await Task.CompletedTask;
            // 获取客户端识别码
            string clientId = arg.ClientId;
            //ReplyMsg("客户端连接成功服务端通知", clientId);
            //Console.WriteLine($"ClientConnectedAsync：客户端ID=[{clientId}]");
            //ICollection<MqttTopicFilter> list = new List<MqttTopicFilter>() { new MqttTopicFilterBuilder().WithTopic("MyTopic").Build() };
            //await mqttServer.SubscribeAsync(clientId, list);

            Console.WriteLine($"ClientConnectedAsync：客户端ID=【{arg.ClientId}】已连接, 用户名=【{arg.UserName}】地址=【{arg.Endpoint}】  ");
            return Task.CompletedTask;
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


        #endregion


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
               Dispose();
            });
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
            if (!isDispose)
            {
                mqttServer.StopAsync().Wait();
                isDispose = true;
                //_applicationLifetime.StopApplication();

                //Task.FromResult(Utils.WriteServiceLog("MQTTHostedService", "StopAsync", "服务停止", false, ""));
            }
        }
    }
}
