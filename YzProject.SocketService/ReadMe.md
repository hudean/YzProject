1、日志
log4net日志文档
参考：https://www.cnblogs.com/dongteng/p/7436328.html

github: https://github.com/kerryjiang/SuperSocket/tree/master/src

1、nuget安装SuperSocket 包（版本1.6.6.1）
2、自定义自己服务器中相关的类
建议按照如下顺序建立类： RequestInfo>ReceiveFilter>AppSession>AppServer

***\*（1）、建立一个RequestInfo\****

RequestInfo类

```
 public class DTRequestInfo : IRequestInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="body">接收的数据体</param>
        public DTRequestInfo(string key, byte[] body)
        {
            this.Key = key;
            this.Body = body;
        }
        public string Key
        {
            get; set;
        }
        /// <summary>
        /// 请求信息缓存
        /// </summary>
        public byte[] Body { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID { get; set; }

    }
```

**（2）、建立一个数据接收过滤器帮助 类，作为过滤器的继承的父类，主要的作用是用来接收处理客户端传类的二进制字符，返回有效的数据部分。**

ReceiveFilterHelper类

```
/// <summary>
    /// 处理获取的所有数据
    /// </summary>
    /// <typeparam name="TRequestInfo"></typeparam>
   public abstract class ReceiveFilterHelper<TRequestInfo> : ReceiveFilterBase<TRequestInfo>
        where TRequestInfo : IRequestInfo
    {
        private  SearchMarkState<byte> m_BeginSearchState;
        private  SearchMarkState<byte> m_EndSearchState;
        private bool m_FoundBegin = false;
        protected TRequestInfo NullRequestInfo = default(TRequestInfo);
        /// <summary>
        /// 初始化实例
        /// </summary>
        protected ReceiveFilterHelper()
        {
 
        }
        /// <summary>
        /// 过滤指定的会话
        /// </summary>
        /// <param name="readBuffer">数据缓存</param>
        /// <param name="offset">数据起始位置</param>
        /// <param name="length">缓存长度</param>
        /// <param name="toBeCopied"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public override TRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;
            int searchEndMarkOffset;
            int searchEndMarkLength;
            //在此处做了处理，将接收到的第一个字符作为起始过滤标志，到结束。返回指定长度的数据。
            byte[] startMark = new byte[] { readBuffer[offset] };
            byte[] endMark = new byte[] {0xff };
            m_BeginSearchState = new SearchMarkState<byte>(startMark);
            m_EndSearchState = new SearchMarkState<byte>(endMark);
            //上一个开始标记长度
            int prevMatched = 0;
            int totalParsed = 0;
 
            if (!m_FoundBegin)
            {
                prevMatched = m_BeginSearchState.Matched;
                int pos = readBuffer.SearchMark(offset, length, m_BeginSearchState, out totalParsed);
 
                if (pos < 0)
                {
                    //不要缓存无效数据
                    if (prevMatched > 0 || (m_BeginSearchState.Matched > 0 && length != m_BeginSearchState.Matched))
                    {
                        State = FilterState.Error;
                        return NullRequestInfo;
                    }
 
                    return NullRequestInfo;
                }
                else //找到匹配的开始标记
                {
                    //But not at the beginning
                    if (pos != offset)
                    {
                        State = FilterState.Error;
                        return NullRequestInfo;
                    }
                }
 
                //找到开始标记
                m_FoundBegin = true;
 
                searchEndMarkOffset = pos + m_BeginSearchState.Mark.Length - prevMatched;
 
                //This block only contain (part of)begin mark
                if (offset + length <= searchEndMarkOffset)
                {
                    AddArraySegment(m_BeginSearchState.Mark, 0, m_BeginSearchState.Mark.Length, false);
                    return NullRequestInfo;
                }
 
                searchEndMarkLength = offset + length - searchEndMarkOffset;
            }
            else//Already found begin mark
            {
                searchEndMarkOffset = offset;
                searchEndMarkLength = length;
            }
 
            while (true)
            {
                var prevEndMarkMatched = m_EndSearchState.Matched;
                var parsedLen = 0;
                var endPos = readBuffer.SearchMark(searchEndMarkOffset, searchEndMarkLength, m_EndSearchState, out parsedLen);
 
                //没有找到结束标记
                if (endPos < 0)
                {
                    rest = 0;
                    if (prevMatched > 0)//还缓存先前匹配的开始标记
                        AddArraySegment(m_BeginSearchState.Mark, 0, prevMatched, false);
                    AddArraySegment(readBuffer, offset, length, toBeCopied);
                }
 
                //totalParsed += parsedLen;
                //rest = length - totalParsed;
                totalParsed = 0;
                byte[] commandData = new byte[BufferSegments.Count + prevMatched + totalParsed];
 
                if (BufferSegments.Count > 0)
                    BufferSegments.CopyTo(commandData, 0, 0, BufferSegments.Count);
 
                if (prevMatched > 0)
                    Array.Copy(m_BeginSearchState.Mark, 0, commandData, BufferSegments.Count, prevMatched);
 
                Array.Copy(readBuffer, offset, commandData, BufferSegments.Count + prevMatched, totalParsed);
 
                var requestInfo = ProcessMatchedRequest(commandData, 0, commandData.Length);
 
                
                Reset();
                return requestInfo;
                if (prevMatched > 0)//Also cache the prev matched begin mark
                    AddArraySegment(m_BeginSearchState.Mark, 0, prevMatched, false);
                AddArraySegment(readBuffer, offset, length, toBeCopied);
                //return NullRequestInfo;
            }
        }
        /// <summary>
        /// Processes the matched request.
        /// </summary>
        /// <param name="readBuffer">The read buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        protected abstract TRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length);
 
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public override void Reset()
        {
            m_BeginSearchState.Matched = 0;
            m_EndSearchState.Matched = 0;
            m_FoundBegin = false;
            base.Reset();
        }
    }
```

**（3）、建立一个数据接收过滤器，继承ReceiveFilterHelper类，过来接收并过滤指定的信息内容。**

 ReceiveFilter类

```
 /// <summary>
    /// 数据接收过滤器
    /// </summary>
    public class DTReceiveFilter : ReceiveFilterHelper<DTRequestInfo>
    {
        /// <summary>
        /// 重写方法
        /// </summary>
        /// <param name="readBuffer">过滤之后的数据缓存</param>
        /// <param name="offset">数据起始位置</param>
        /// <param name="length">数据缓存长度</param>
        /// <returns></returns>
        protected override DTRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            //返回构造函数指定的数据格式
            return new DTRequestInfo(Encoding.UTF8.GetString(readBuffer, offset, length), readBuffer);
        }
    }
```

**（4）、建立一个AppSession,用来发送和接收客户端信息，一个客户端相当于一个session，这一点很重要，应为每一个RUT设备都有一个固定的编号，需要给session的item中添加Key值用来区分不同的客户端。**

AppSession类

```
 public class SocketSession:AppSession<SocketSession,DTRequestInfo>
    {
 
        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
        }
 
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }
        protected override int GetMaxRequestLength()
        {
            return base.GetMaxRequestLength();
        }
 
        protected override void HandleUnknownRequest(DTRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }
    }
```

**（5）、建立AppServer，自定义适合自己项目的服务。这个项目需要的就是服务端给客户端RTU发送数据请求指令，客户端才能做出响应返回数据十六进制的数据报文。**

AppServer类

```
 public class SocketServer : AppServer<SocketSession, DTRequestInfo>
    {
        Timer requestTimer = null;

        public SocketServer() : base(new DefaultReceiveFilterFactory<DTReceiveFilter, DTRequestInfo>())
        {
            //定时发送请求压力的报文
            double sendInterval = double.Parse(ConfigurationManager.AppSettings["sendInterval"]);
            requestTimer = new Timer(sendInterval);
            requestTimer.Elapsed += RequestTimer_Elapsed;
            requestTimer.Enabled = true;
            requestTimer.Start();
        }

        private void RequestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //发送请求报文
            var sessionList = GetAllSessions();
            //Logger.Error(sessionList);
            foreach (var session in sessionList)
            {
                Dictionary<string, string> routs = ConfigManager.GetAllConfig();
                try
                {
                    foreach (var item in routs)
                    {
                        if (item.Key.ToString().Contains("rout2_"))
                        {
                            string routeID = item.Key.ToString().Split('_')[1];
                            byte[] rout = ConvertHelper.strToToHexByte(routeID);
                            byte[] address = ConvertHelper.strToToHexByte(item.Value.ToString());
                            /// 合成报文 
                            List<byte> data = new List<byte>();
                            data.Add(rout[0]);
                            data.Add(0x04);//读取数据
                            data.Add(address[0]);
                            data.Add(address[1]);
                            data.Add(address[2]);
                            data.Add(address[3]);
                            byte[] checkcode = CRC16.crc_16(data.ToArray());
                            data.Add(checkcode[1]);
                            data.Add(checkcode[0]);
                            /// 发送报文
                            //使用字节抽屉存储
                            // ArraySegment<byte> sendData = new ArraySegment<byte>(data.ToArray());
                            session.Send(data.ToArray(), 0, data.ToArray().Length);
                            // Console.WriteLine("发送数据：" + ConvertHelper.byteToHexStr(data.ToArray()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    //写入日志
                    /// Logger.Info(ex.Message);
                }
            }
        }
        protected override void OnNewSessionConnected(SocketSession session)
        {
            base.OnNewSessionConnected(session);
            //Logger.Error(session.SessionID);
        }
        protected override void ExecuteCommand(SocketSession session, DTRequestInfo requestInfo)
        {
            base.ExecuteCommand(session, requestInfo);
        }
        protected override void OnStarted()
        {
            base.OnStarted();
        }
        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }
    }
```

1、在程序启动时开启服务，用来监听客户端。

```
appServer = new SocketServer();
            int Port = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());
            if (!appServer.Setup(Port))
            {
                label1.Text = "端口装载失败";
                return;
            }
            if (!appServer.Start())
            {
                label1.Text = "服务启动失败";
                return;
            }
            label1.Text = "压力采集终端服务已开启";
            //监听地址
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    label2.Text = "监听IP：" + IpEntry.AddressList[i];
                    label3.Text = "端口：" + Port.ToString();
                }
            }
            //注册事件
            appServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<DTSession, DTRequestInfo>(appServer_NewRecivede);
```

2、请求接收事件

```
private  void appServer_NewRecivede(SocketSession session, DTRequestInfo requestInfo)
        {
            byte[] bytes = requestInfo.Body;
            if (bytes[0] == 0xfe)
                return;
            //设备地址
            //设备地址
            string devAddress = bytes[0].ToString("X");
            string dataType = bytes[1].ToString("X");
            if (dataType != "4")
            {
                string tt = string.Empty;
                foreach (var item in bytes.Take(4).Reverse())
                {
                    tt += item.ToString("X2");
                }
                //resultInfo.DeviceID = tt;
                session.Items["deviceid"] = tt;
                return;
            }
            //数据长度
            int dataLenth = bytes[2];
            //数据
            byte[] datas = bytes.Skip(3).Take(dataLenth).ToArray();
            //从缓冲区截取有效数据
            string datastrs = ConvertHelper.byteToHexStr(bytes.Take(dataLenth + 5).ToArray());
            //采集值
            int value = datas[1] + datas[0] * 256;
            //数值转换
            //电流
            double f = 3.3 / 1023 * value / 150 * 1000;
            //压力
            double yl = 0.0625 * (f - 4);
            if (session.Items.Count==0)
            {
                return;
            }
            //将结果写入log
            session.Logger.Info("设备编号：" + session.Items["deviceid"].ToString() + "    压力值：" + yl.ToString() + "    时间：" + DateTime.Now.ToString() + "  报文：" + datastrs);
                    }
```

