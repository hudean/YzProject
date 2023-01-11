using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Common
{
    /// <summary>
    /// RabbitMQ 发送和接受帮助类
    /// </summary>
    public static class RabbitMQHelper
    {
        //mq地址
        private readonly static string RabbitMQ_Host = "localhost";
        //用户名
        private readonly static string RabbitMQ_User = "guest";
        //密码
        private readonly static string RabbitMQ_Pwd = "guest";

        /// <summary>
        /// RabbitMQ发送消息
        /// </summary>
        /// <param name="jsonstr">具体json格式的字符串</param>
        /// <param name="queuqname">具体入队的队列名称</param>
        /// <returns></returns>
        public static bool SendMsg(string jsonstr, string queuqname)
        {
            try
            {
                //1.实例化连接工厂
                var factory = new ConnectionFactory();
                //主机名，Rabbit会拿这个IP生成一个endpoint，这个很熟悉吧，就是socket绑定的那个终结点。
                factory.HostName = "localhost";
                //默认用户名,用户可以在服务端自定义创建，有相关命令行
                factory.UserName = "guest";
                //默认密码
                factory.Password = "guest";
                //设置端口后自动恢复连接属性
                factory.AutomaticRecoveryEnabled = true;
                //2. 建立连接
                var connection = factory.CreateConnection();
                //3. 创建信道
                var channel = connection.CreateModel();
                try
                {
                    var queue_name = queuqname;//具体入队的队列名称
                    bool durable = true;//队列是否持久化
                    bool exclusive = false;
                    //设置 autoDeleted=true 的队列，当没有消费者之后，队列会自动被删除
                    bool autoDelete = false;
                    //4. 申明队列
                    channel.QueueDeclare(queue_name, durable, exclusive, autoDelete, null);

                    //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; //持久化的消息

                    string message = jsonstr; //传递的消息内容
                    var body = Encoding.UTF8.GetBytes(message);

                    var exchange_name = "";
                    var routingKey = queue_name;//routingKey=queue_name，则为对应队列接收=queue_name

                    channel.BasicPublish(exchange_name, routingKey, properties, body); //开始传递(指定basicProperties) 

                    return true;

                }
                catch (Exception ex)
                {
                    //PubTool.ConnError("RabbitMQ", "RunLog", "发送数据异常：" + ex.Message);
                }
                finally
                {
                    connection.Close();
                    channel.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生异常：" + ex.Message);
                return false;
            }
            return false;

        }

        /// <summary>
        /// RabbitMQ接受消息
        /// </summary>
        /// <param name="queuqname"></param>
        /// <param name="limitnum">具体入队的队列名称</param>
        public static void ReceiveMsg(string queuqname, ushort limitnum = 3)
        {
            #region 处理方法
            try
            {
                #region 构建消息队列
                //1.实例化连接工厂
                var factory = new RabbitMQ.Client.ConnectionFactory();
                factory.HostName = RabbitMQ_Host;
                factory.UserName = RabbitMQ_User;
                factory.Password = RabbitMQ_Pwd;
                factory.AutomaticRecoveryEnabled = true;
                //2. 建立连接 
                var connection = factory.CreateConnection();
                //3. 创建信道
                var channel = connection.CreateModel();

                var queue_name = queuqname;//项目下游上传的队列信息
                bool durable = true;//队列是否持久化
                bool exclusive = false;
                bool autoDelete = false;//设置 autoDeleted=true 的队列，当没有消费者之后，队列会自动被删除
                                        //4. 申明队列
                channel.QueueDeclare(queue_name, durable, exclusive, autoDelete, null);
                //5. 构造消费者实例
                var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
                bool autoAck = false;
                //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认 
                //6. 绑定消息接收后的事件委托

                //8. 启动消费者
                //设置prefetchCount : 3 来告知RabbitMQ，在未收到消费端的N条消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                channel.BasicQos(0, limitnum, false);

                channel.BasicConsume(queue_name, autoAck, consumer);

                #endregion

                #region 队列-接收消息的处理方法
                //以这种形式传递方法    
                //consumer.Received += (model, ea) => { Methed(model, ea, channel);};
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        //var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        //获取消息后进行操作，do something
                        bool flag = false;
                        if (!string.IsNullOrEmpty(message))
                        {
                            try
                            {
                                //做其他存储或处理操作

                            }
                            catch (Exception ex)
                            {
                                //    //pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-处理Message数据时，发生异常----" + ex.Message);
                                //    //pub.ConnError_TestSysShow("RabbitMq-RST", "Item-Attence", "项目下游请求数据-处理Message数据时，发生异常原数据message=" + message);
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            //操作完毕，则手动确认消息可删除
                            // 7. 发送消息确认信号（手动消息确认）
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-接收数据后-发生异常：" + ex.Message);
                    }
                };
                #endregion

                //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-RabbitMQ监听消息启动成功");
            }
            catch (Exception ex)
            {
                //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-构建rabbitmq的queue异常：" + ex.Message);
                //RabbitMQ_Event1_IsRuning = false;
                //RabbitMQ_Event1_checkDate = DateTime.Now;
                //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-RabbitMQ监听消息——已关闭");
            }
            //finally
            //{
            //    connection.Close();//不能关，关了就停止接收消息了
            //    channel.Close();
            //}
            #endregion
        }

        public static void ReceiveMsg(string queuqname, Action<object, BasicDeliverEventArgs, IModel> action, ushort limitnum = 3)
        {
            #region 处理方法
            try
            {
                #region 构建消息队列
                //1.实例化连接工厂
                var factory = new RabbitMQ.Client.ConnectionFactory();
                factory.HostName = RabbitMQ_Host;
                factory.UserName = RabbitMQ_User;
                factory.Password = RabbitMQ_Pwd;
                factory.AutomaticRecoveryEnabled = true;
                //2. 建立连接 
                var connection = factory.CreateConnection();
                //3. 创建信道
                var channel = connection.CreateModel();

                var queue_name = queuqname;//项目下游上传的队列信息
                bool durable = true;//队列是否持久化
                bool exclusive = false;
                bool autoDelete = false;//设置 autoDeleted=true 的队列，当没有消费者之后，队列会自动被删除
                                        //4. 申明队列
                channel.QueueDeclare(queue_name, durable, exclusive, autoDelete, null);
                //5. 构造消费者实例
                var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
                bool autoAck = false;
                //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认 
                //6. 绑定消息接收后的事件委托

                //8. 启动消费者
                //设置prefetchCount : 3 来告知RabbitMQ，在未收到消费端的N条消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                channel.BasicQos(0, limitnum, false);

                channel.BasicConsume(queue_name, autoAck, consumer);

                #endregion

                #region 队列-接收消息的处理方法
                //以这种形式传递方法    
                //consumer.Received += (model, ea) => { Methed(model, ea, channel);};
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        //var body = ea.Body.ToArray();
                        //string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        action(model, ea, channel);
                    }
                    catch (Exception ex)
                    {
                        //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-接收数据后-发生异常：" + ex.Message);
                    }
                };
                #endregion

                //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-RabbitMQ监听消息启动成功");
            }
            catch (Exception ex)
            {
                //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-构建rabbitmq的queue异常：" + ex.Message);
                //RabbitMQ_Event1_IsRuning = false;
                //RabbitMQ_Event1_checkDate = DateTime.Now;
                //JMB_ServiceClass.Common.pub.ConnError("RabbitMQ", "Event_RabbitMQ_Method1", "项目下游请求数据-RabbitMQ监听消息——已关闭");
            }
            //finally
            //{
            //    connection.Close();//不能关，关了就停止接收消息了
            //    channel.Close();
            //}
            #endregion
        }
        static void Methed(object model, BasicDeliverEventArgs ea, IModel channel)
        {
            try
            {
                //var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                //获取消息后进行操作，do something
                bool flag = false;
                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        //做其他存储或处理操作
                        //File.WriteAllText(@"C:\Users\Administrator\Desktop\333.txt", message, Encoding.UTF8);
                        Console.WriteLine("ok :" + message);
                        flag = true;


                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    //操作完毕，则手动确认消息可删除
                    // 7. 发送消息确认信号（手动消息确认）
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
