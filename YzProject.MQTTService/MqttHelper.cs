//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
//using MQTTnet.Client.Subscribing;
//using MQTTnet.Client.Unsubscribing;
//using MQTTnet.Server;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace YzProject.MQTTService
//{
//    //MQTTnet 3.1.0版本

//    internal class MqttHelper
//    {
//        // MQTT 服务器端
//        public static IMqttServer mqttServer;

//        // MQTT 客户端
//        public static IMqttClient mqttClient;


//       private static int Start()//Main(string[] args)
//        {
//            string[] args = new string [1]{ ""};
//            if (args.Length != 1)
//            {
//                Console.WriteLine("usage [dotnet run server] start server daemon");
//                Console.WriteLine("usage [dotnet run client] start client daemon");
//            }

//            if (args[0] == "server")
//            {
//                StartServer();
//            }

//            if (args[0] == "client")
//            {
//                StartClient();
//            }

//            return 0;
//        }

//        private static async void StartServer()
//        {
//            try
//            {
//                // 1. 创建 MQTT 连接验证，用于连接鉴权
//                MqttServerConnectionValidatorDelegate connectionValidatorDelegate = new MqttServerConnectionValidatorDelegate(
//                    p =>
//                    {
//                        // p 表示正在发起的一个链接的上下文

//                        // 客户端 id 验证
//                        // 大部分情况下，我们应该使用设备识别号来验证
//                        if (p.ClientId == "twle_client")
//                        {
//                            // 用户名和密码验证
//                            // 大部分情况下，我们应该使用客户端加密 token 验证，也就是可客户端 ID 对应的密钥加密后的 token
//                            if (p.Username != "yufei" && p.Password != "123456")
//                            {
//                                // 验证失败，告诉客户端，鉴权失败
//                                p.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
//                            }
//                        }
//                    }
//                );


//                // 2. 创建 MqttServerOptions 的实例，用来定制 MQTT 的各种参数
//                MqttServerOptions options = new MqttServerOptions();

//                // 3. 设置各种选项
//                // 客户端鉴权
//                options.ConnectionValidator = connectionValidatorDelegate;

//                // 设置服务器端地址和端口号
//                options.DefaultEndpointOptions.Port = 8001;

//                // 4. 创建 MqttServer 实例
//                mqttServer = new MqttFactory().CreateMqttServer();

//                // 5. 设置 MqttServer 的属性
//                // 设置消息订阅通知
//                mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(SubScribedTopic);
//                // 设置消息退订通知
//                mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(UnScribedTopic);
//                // 设置消息处理程序
//                mqttServer.UseApplicationMessageReceivedHandler(MessageReceived);
//                // 设置客户端连接成功后的处理程序
//                mqttServer.UseClientConnectedHandler(ClientConnected);
//                // 设置客户端断开后的处理程序
//                mqttServer.UseClientDisconnectedHandler(ClientDisConnected);

//                // 启动服务器
//                await mqttServer.StartAsync(options);


//                Console.WriteLine("服务器启动成功！直接按回车停止服务");
//                Console.ReadLine();

//            }
//            catch (Exception ex)
//            {
//                Console.Write($"服务器启动失败:{ex}");
//            }
//        }


//        // 客户端发起订阅主题通知
//        private static void SubScribedTopic(MqttServerClientSubscribedTopicEventArgs args)
//        {
//            // 获取客户端识别码
//            string clientId = args.ClientId;
//            // 获取客户端发起的订阅主题
//            string topic = args.TopicFilter.Topic;

//            Console.WriteLine($"客户端[{clientId}]已订阅主题:{topic}");
//        }

//        // 客户端取消主题订阅通知
//        private static void UnScribedTopic(MqttServerClientUnsubscribedTopicEventArgs args)
//        {
//            // 获取客户端识别码
//            string clientId = args.ClientId;
//            // 获取客户端发起的订阅主题
//            string topic = args.TopicFilter;

//            Console.WriteLine($"客户端[{clientId}]已退订主题:{topic}");
//        }

//        // 接收客户端发送的消息
//        private static void MessageReceived(MqttApplicationMessageReceivedEventArgs args)
//        {
//            // 获取消息的客户端识别码
//            string clientId = args.ClientId;
//            // 获取消息的主题
//            string topic = args.ApplicationMessage.Topic;
//            // 获取发送的消息内容
//            string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
//            // 获取消息的发送级别(Qos)
//            var qos = args.ApplicationMessage.QualityOfServiceLevel;
//            // 获取消息的保持形式
//            bool retain = args.ApplicationMessage.Retain;

//            Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");

//        }

//        // 客户端连接成功后的的处理通知
//        private static void ClientConnected(MqttServerClientConnectedEventArgs args)
//        {
//            // 获取客户端识别码
//            string clientId = args.ClientId;

//            Console.WriteLine($"新客户端[{clientId}] 加入");
//        }

//        // 客户端断开连接通知
//        private static void ClientDisConnected(MqttServerClientDisconnectedEventArgs args)
//        {
//            // 获取客户端识别码
//            string clientId = args.ClientId;

//            Console.WriteLine($"新客户端[{clientId}] 已经离开");
//        }


//        #region Client

//        private static void StartClient()
//        {
//            ConnectToServer();
//            SendMessage();
//        }


//        private static void ConnectToServer()
//        {
//            try
//            {
//                // 1. 创建 MQTT 客户端
//                mqttClient = new MqttFactory().CreateMqttClient();


//                // 2 . 设置 MQTT 客户端选项
//                MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder();

//                // 设置服务器端地址
//                optionsBuilder.WithTcpServer("127.0.0.1", 8001);

//                // 设置鉴权参数
//                optionsBuilder.WithCredentials("yufei", "123456");

//                // 设置客户端序列号
//                optionsBuilder.WithClientId(Guid.NewGuid().ToString());

//                // 创建选项
//                IMqttClientOptions options = optionsBuilder.Build();


//                // 设置消息接收处理程序
//                mqttClient.UseApplicationMessageReceivedHandler(args => {
//                    Console.WriteLine("### 收到来自服务器端的消息 ###");
//                    // 收到的消息主题
//                    string topic = args.ApplicationMessage.Topic;
//                    // 收到的的消息内容
//                    string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
//                    // 收到的发送级别(Qos)
//                    var qos = args.ApplicationMessage.QualityOfServiceLevel;
//                    // 收到的消息保持形式
//                    bool retain = args.ApplicationMessage.Retain;

//                    Console.WriteLine($"主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");
//                });

//                // 重连机制
//                mqttClient.UseDisconnectedHandler(async e =>
//                {
//                    Console.WriteLine("与服务器之间的连接断开了，正在尝试重新连接");
//                    // 等待 5s 时间
//                    await Task.Delay(TimeSpan.FromSeconds(5));
//                    try
//                    {
//                        // 重新连接
//                        await mqttClient.ConnectAsync(options);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"重新连接服务器失败:{ex}");
//                    }
//                });

//                // 连接到服务器
//                mqttClient.ConnectAsync(options);
//                Console.WriteLine("连接服务器成功！请输入任意内容并回车进入菜单界面");


//            }
//            catch (Exception ex)
//            {
//                Console.Write($"连接服务器失败: {ex}");
//            }
//        }

//        private static void SendMessage()
//        {
//            Console.ReadLine();

//            // 是否退出
//            bool isExit = false;

//            while (!isExit)
//            {
//                Console.WriteLine(@"请输入
//                    1. 订阅主题
//                    2. 取消订阅
//                    3. 发送消息
//                    4. 退出");

//                string input = Console.ReadLine();
//                string topic = "";

//                switch (input)
//                {
//                    case "1":
//                        Console.WriteLine(@"请输入主题名称：");
//                        topic = Console.ReadLine();
//                        ClientSubscribeTopic(topic);
//                        break;
//                    case "2":
//                        Console.WriteLine(@"请输入需要退订的主题名称：");
//                        topic = Console.ReadLine();
//                        ClientUnsubscribeTopic(topic);
//                        break;
//                    case "3":
//                        Console.WriteLine(@"请输入需要发送的主题名称: ");
//                        topic = Console.ReadLine();
//                        Console.WriteLine(@"请输入需要发送的消息内容：");
//                        string message = Console.ReadLine();
//                        ClientPublish(topic, message);
//                        break;
//                    case "4":
//                        isExit = true;
//                        break;
//                    default:
//                        Console.WriteLine("请输入正确的指令");
//                        break;
//                }
//            }
//        }

//        private static async void ClientSubscribeTopic(string topic)
//        {
//            topic = topic.Trim();
//            if (string.IsNullOrEmpty(topic))
//            {
//                Console.Write("订阅主题不能为空！");
//                return;
//            }

//            // 判断客户端是否连接
//            if (!mqttClient.IsConnected)
//            {
//                Console.WriteLine("MQTT 客户端尚未连接!");
//                return;
//            }

//            // 设置订阅参数
//            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
//                .WithTopicFilter(topic)
//                .Build();

//            // 订阅
//            await mqttClient.SubscribeAsync(
//                    subscribeOptions,
//                    System.Threading.CancellationToken.None);
//        }

//        private static async void ClientUnsubscribeTopic(string topic)
//        {
//            topic = topic.Trim();
//            if (string.IsNullOrEmpty(topic))
//            {
//                Console.Write("退订主题不能为空！");
//                return;
//            }

//            // 判断客户端是否连接
//            if (!mqttClient.IsConnected)
//            {
//                Console.WriteLine("MQTT 客户端尚未连接!");
//                return;
//            }

//            // 设置订阅参数
//            var subscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
//                .WithTopicFilter(topic)
//                .Build();


//            // 退订
//            await mqttClient.UnsubscribeAsync(
//                    subscribeOptions,
//                    System.Threading.CancellationToken.None);
//        }


//        private async static void ClientPublish(string topic, string message)
//        {
//            topic = topic.Trim();
//            message = message.Trim();

//            if (string.IsNullOrEmpty(topic))
//            {
//                Console.Write("退订主题不能为空！");
//                return;
//            }

//            // 判断客户端是否连接
//            if (!mqttClient.IsConnected)
//            {
//                Console.WriteLine("MQTT 客户端尚未连接!");
//                return;
//            }

//            // 填充消息
//            var applicationMessage = new MqttApplicationMessageBuilder()
//                .WithTopic(topic)       // 主题
//                .WithPayload(message)   // 消息
//                .WithExactlyOnceQoS()   // qos
//                .WithRetainFlag()       // retain
//                .Build();

//            await mqttClient.PublishAsync(applicationMessage);
//        }

//        #endregion Client
//    }
//}
