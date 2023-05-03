//using Microsoft.Extensions.Logging;
//using SuperSocket.SocketBase;
//using SuperSocket.SocketBase.Config;
//using SuperSocket.SocketBase.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using YzProject.Common;

//namespace YzProject.SocketService.SuperSocketHelper
//{
//    public class AppSocketServer : IDisposable
//    {
//        private SocketServer server;
//        private readonly ILogger _console;
//        private ILogger<AppSocketServer> logger;
//        //private Dictionary<string, int> ClientList = new Dictionary<string, int>();
//        //private Dictionary<int, string> ClientIds = new Dictionary<int, string>();
//        //private Crypto crypto = new Crypto();
//        //private FromCenterServiceImpl fromCenterService;
//        //private ToCenterServiceImpl tocenterService;
//        private Retry retry;

//        public AppSocketServer(ILogger consoleLog, ILoggerFactory loggerFactory)
//        {
//            _console = consoleLog;
//            logger = loggerFactory.CreateLogger<AppSocketServer>();
//            //server = new TcpPullServer(65000, 2048, 30);
//            //server.OnAccept += Server_OnAccept;
//            //server.OnReceive += Server_OnReceive;
//            //server.OnClose += Server_OnClose;
//            server = new SocketServer(consoleLog);
//            var serverConfig = new ServerConfig();
//            serverConfig.Port = 8501;
//            serverConfig.TextEncoding = "GB2312";
//            serverConfig.MaxConnectionNumber = 100000;   //最大允许的客户端连接数目
//            serverConfig.ListenBacklog = 100000; //最大请求队列，如果客户请求连接数超过这个值，其他的请求将被忽略或者将不会被处理
//            server.Setup(serverConfig);
//            server.SessionClosed += Server_SessionClosed;
//            server.NewRequestReceived += Server_NewRequestReceived;
//            //tocenterService = new ToCenterServiceImpl(consoleLog);
//            //fromCenterService = new FromCenterServiceImpl(logger, this, cache);

//            retry = Retry.Instance();
//        }

//        private void Server_NewRequestReceived(SocketSession session, StringRequestInfo requestInfo)
//        {
//            //throw new NotImplementedException();
//            string payload = string.Empty;
//            try
//            {
//                session.ReceivePackage(requestInfo);
//                string clientId = requestInfo.Key.ToUpper();
//                if (string.IsNullOrEmpty(session.ClientId))
//                {
//                    session.ClientId = clientId;
//                    server.SetSession(clientId, session);
//                }

//                #region
//                //2019-12-09数据自旋
//                //var stop = cacheProvider.GetString(CachingKeys.DATA_BLOCK_STOP);
//                //if (stop == "true")
//                //{
//                //    return;
//                //}
//                //if (string.IsNullOrEmpty(requestInfo.Body)) return;
//                //payload = FixBody(requestInfo.Body);

//                //string msg = string.Empty;
//                //if (Utils.ValidPayLoadWithCRC16WithClientId(session.ClientId, payload, out msg) == false)
//                //{
//                //    Utils.WriteMsg($"校验未通过:{msg}", "SocketServer", $"CRC_{session.ClientId}");
//                //    cacheProvider.AddAsyn(CachingKeys.DEVICE_ONLINE_KEY, $"{session.ClientId}0", session.ClientId);
//                //    var strArr = msg.Split(",");
//                //    var chss = "0";
//                //    foreach (var item in strArr)
//                //    {
//                //        if (item.IndexOf("CH") > -1)
//                //        {
//                //            chss = item.Replace("CH", "").Replace(":", "").Replace("\"", "");
//                //            if (!Utils.rgx_Num.IsMatch(chss))
//                //            {
//                //                chss = "0";
//                //            }
//                //        }
//                //        else if (item.IndexOf("MsgId") > -1)
//                //        {
//                //            var mId = item.Replace("MsgId", "").Replace(":", "").Replace("\"", "");
//                //            Guid guid;
//                //            if (Guid.TryParse(mId, out guid) == true)
//                //            {
//                //                ReplyMsg(mId, chss, clientId);
//                //                Utils.WriteMsg($"{mId}_{payload}", $"aa_{clientId}_{chss}", $"aa_{clientId}");
//                //                return;
//                //            }
//                //        }
//                //    }
//                //    return;
//                //}

//                //var macs = cacheProvider.GetHashKeys(CachingKeys.MAC_MONITOR_KEY);
//                //if (macs != null && macs.Count > 0 && macs.Contains(clientId))
//                //{
//                //    Console.WriteLine(payload);
//                //}
               

//                ////PrintToConsole($"Received Client:'{session.ClientId}' Data: >[{payload}]");
//                ////payload = payload.Replace("\"床", "床");

//                //var temp = (JObject)JsonConvert.DeserializeObject(payload);
//                //var dt = temp.GetValue("Dtype");
//                //var ch = temp.GetValue("CH");
//                //var msgId = temp.GetValue("MsgId");
//                //var key = temp.GetValue("Key");
//                //if (dt != null && dt.ToString() == "10" && ch != null)
//                //{
//                //    ReplyMsg(msgId.ToString(), ch.ToString(), clientId);
//                //}
//                //if (dt != null && dt.ToString() == "aa" && msgId != null)
//                //{
//                //    cacheProvider.RemoveAsyn(CachingKeys.RESEND_SCAN, msgId.ToString());
//                //}
//                //if (dt != null && dt.ToString() == "24" && msgId != null)
//                //{
//                //    cacheProvider.RemoveAsyn(CachingKeys.RESEND_SCAN, msgId.ToString());
//                //}
//                //发送消息到消息队列中
//                //var dto = new R_SMessageEvent
//                //{
//                //    ClientId = clientId,
//                //    DeviceKey = clientId,
//                //    DType = dt.ToString(),
//                //    Content = crypto.Base64Code(payload),
//                //    IP = session.RemoteEndPoint != null ? session.RemoteEndPoint.Address.ToString() : ""
//                //};
//                //tocenterService.Send(dto);
//                #endregion
//            }
//            catch (Exception ex)
//            {
//                var msg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
//                //Utils.WriteServiceLog("SocketServer", "Server_NewRequestReceived", $"payload:{payload},error:{msg}", true, "");
//                //_console.LogError($"Receive Message payload:{payload}");
//                //_console.LogError($"Receive Message progress error:{ex.ToString()}");
//            }
//        }

//        private void Server_SessionClosed(SocketSession session, CloseReason value)
//        {
//            //throw new NotImplementedException();
//            try
//            {
//                //Utils.WriteMsg($"Server_SessionClosed ClientId:'{session.ClientId}'  CloseReason: >[{value.ToString()}]", "SocketServer", $"SocketServer_{session.ClientId}");

//                if (string.IsNullOrEmpty(session.ClientId)) return;
//                string clientId = session.ClientId;
//                server.DelSession(session);  //移除会话          
//                PrintToConsole($"ClientId:{clientId} Disconnect");
//                //var dto = new DeviceDisConnectEvent
//                //{
//                //    ClientId = clientId
//                //};
//                //tocenterService.Send(dto);  //向处理中心发断开连接消息
//            }
//            catch (Exception ex)
//            {
//                var msg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
//                //Utils.WriteServiceLog("SocketServer", "Server_SessionClosed", $"ClientId:{session.ClientId},error:{msg}", true, "");
//            }
//        }

//        #region Server_OnClose
//        //private void Server_OnClose(int obj)
//        //{
//        //    PrintToConsole($"ClientId:{obj} Disconnect");
//        //    string clientId;
//        //    if(ClientIds.TryGetValue(obj,out clientId))
//        //    {
//        //        var dto = new DeviceDisConnectEvent
//        //        {
//        //            ClientId = clientId
//        //        };
//        //        //tocenterService.Send(dto);
//        //    }
//        //} 
//        #endregion

//        #region  Server_OnReceive
//        //private void Server_OnReceive(int arg1, int arg2)
//        //{
//        //    byte[] data = server.Fetch(arg1, server.GetLength(arg1));
//        //    string payload = GB2312_UTF8(data);
//        //    PrintToConsole($"Received Client:'{arg1}' Data: >[{payload}]");
//        //    try
//        //    {
//        //        var temp = (JObject)JsonConvert.DeserializeObject(payload);
//        //        var dt = temp.GetValue("Dtype");
//        //        var key = temp.GetValue("Key");
//        //        if (dt.ToString() == "10" && key.ToString() == "DeviceKey")
//        //        {
//        //            string deviceKey = temp.GetValue("Value").ToString().ToUpper();
//        //            string ch = temp.GetValue("CH").ToString();
//        //            string dkey = $"{deviceKey}{ch}";
//        //            ClientIds[arg1] = dkey;
//        //            ClientList[dkey] = arg1;
//        //            var msgId = temp.GetValue("MsgId");
//        //            ReplyMsg(msgId.ToString(), ch, arg1);
//        //        }
//        //        else
//        //        {
//        //            string clientId;
//        //            if(ClientIds.TryGetValue(arg1,out clientId))
//        //            {
//        //                var ss = clientId.Length>12?clientId.Substring(0, 12):clientId;
//        //                var dto = new R_SMessageEvent
//        //                {
//        //                    ClientId=clientId,
//        //                    DeviceKey = ss,
//        //                    DType = dt.ToString(),
//        //                    Content = crypto.Base64Code(payload)
//        //                };
//        //                //tocenterService.Send(dto);
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _console.LogError($"Receive Message progress error:{ex.ToString()}");
//        //    }
//        //} 
//        #endregion

//        #region  Server_OnAccept
//        //private void Server_OnAccept(int obj)
//        //{
//        //    PrintToConsole($"ClientId:{obj} Connected");
//        //}
//        //private R_SMessageEvent GetR_SMessage(string json, string deviceKey)
//        //{
//        //    R_SMessageEvent dto = null;
//        //    try
//        //    {
//        //        if (string.IsNullOrEmpty(json)) return null;
//        //        JObject temp = (JObject)JsonConvert.DeserializeObject(json);
//        //        var dt = temp.GetValue("Dtype");
//        //        dto = new R_SMessageEvent
//        //        {
//        //            DeviceKey = deviceKey,
//        //            DType = dt.ToString(),
//        //            Content = crypto.Base64Code(json)
//        //        };
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _console.LogError(ex.Message);
//        //    }
//        //    return dto;
//        //} 
//        #endregion

//        #region private method
//        private string GB2312_UTF8(string msg)
//        {
//            var gb2312 = Encoding.GetEncoding("GB2312");
//            var bytes = gb2312.GetBytes(msg);
//            var bts = Encoding.Convert(gb2312, Encoding.UTF8, bytes);
//            return Encoding.UTF8.GetString(bts);
//        }
//        private void PrintToConsole(string message)
//        {
//            //#if DEBUG
//            //_console.LogInformation(message);
//            //#endif
//        }
//        private string FixBody(string payload)
//        {
//            if (string.IsNullOrEmpty(payload)) return "{}";
//            string result = payload;
//            if (payload.IndexOf('{') == -1)
//            {
//                result = "{" + payload;
//            }
//            if (payload.IndexOf('}') == -1)
//            {
//                result += "}";
//            }
//            return result;
//        }

//        /// <summary>
//        /// 发送扫码
//        /// </summary>
//        /// <param name="clientId"></param>
//        /// <param name="msg"></param>
//        /// <param name="retryCount"></param>
//        /// <param name="sendSession"></param>
//        /// <param name="msgId"></param>
//        /// <returns></returns>
//        private bool TrySendScan(string clientId, string msg, ref int retryCount, SocketSession sendSession, string msgId)
//        {
//            if (string.IsNullOrWhiteSpace(msgId)) return false;
//            retryCount++;
//            sendSession = server.GetSocketSession(clientId);
//            if (sendSession != null)
//            {
//                sendSession.Send(msg);
//                //Thread.Sleep(800); //间隔500毫秒发送一次
//                //var record = cacheProvider.GetHashString(CachingKeys.RESEND_SCAN, msgId);
//                //return record == null;
//            }
//            Thread.Sleep(200); //间隔200毫秒取一次会话
//            return false;
//        }

//        /// <summary>
//        /// 发送aa回复
//        /// </summary>
//        /// <param name="clientId"></param>
//        /// <param name="msg"></param>
//        /// <param name="sendSession"></param>
//        /// <param name="msgId"></param>
//        /// <returns></returns>
//        private bool TrySendMsg(string clientId, string msg, SocketSession sendSession, string msgId)
//        {
//            sendSession = server.GetSocketSession(clientId);
//            if (sendSession != null)
//            {
//                sendSession.Send(msg);
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// 其他一般性下行消息发送
//        /// </summary>
//        /// <param name="clientId"></param>
//        /// <param name="msg"></param>
//        /// <param name="retryCount"></param>
//        /// <param name="sendSession"></param>
//        /// <param name="msgId"></param>
//        /// <returns></returns>
//        private bool TrySendCommon(string clientId, string msg, ref int retryCount, SocketSession sendSession, string msgId)
//        {
//            if (string.IsNullOrWhiteSpace(msgId)) return false;
//            retryCount++;
//            sendSession = server.GetSocketSession(clientId);
//            if (sendSession != null)
//            {
//                sendSession.Send(msg);
//                Thread.Sleep(500); //间隔500毫秒发送一次
//                //var record = cacheProvider.GetHashString(CachingKeys.RESEND_SCAN, msgId);
//                //return record == null;
//            }
//            Thread.Sleep(200); //间隔200毫秒取一次会话
//            return false;
//        }
//        #endregion


//        /// <summary>
//        /// 普通应答
//        /// </summary>
//        /// <param name="msgId"></param>
//        /// <param name="ch"></param>
//        /// <param name="clientId"></param>
//        private void ReplyMsg(string msgId, string ch, string clientId)
//        {
//            string content = "{" + $"\"Dtype\":\"aa\",\"CH\":\"{ch}\",\"MsgId\":\"{msgId}\"";
//            var crcCode = CRC.ToCRC16(content, Encoding.GetEncoding("GB2312"), false);
//            //JObject dto = new JObject
//            //{
//            //    { "Dtype", "aa" },
//            //    { "CH", ch },
//            //    { "MsgId", msgId },
//            //    { "CRC", crcCode }
//            //};
//            //var json = JsonConvert.SerializeObject(dto);
//            //SendMsg(clientId, json, "aa", msgId);
//        }

//        /// <summary>
//        /// 消息下发
//        /// </summary>
//        /// <param name="clientId">MAC</param>
//        /// <param name="msg">消息</param>
//        /// <param name="isscan">是否扫码下发</param>
//        /// <param name="msgId">GUID</param>
//        public void SendMsg(string clientId, string msg, string dtype, string msgId = "")
//        {
//            //try
//            //{
//            //    if (string.IsNullOrWhiteSpace(msgId) || string.IsNullOrWhiteSpace(dtype) || string.IsNullOrWhiteSpace(clientId)) return;
//            //    SocketSession sendSession = null;
//            //    int retryCount = 0;
//            //    bool result = false;
//            //    bool isscan = dtype == "20";

//            //    if (dtype == "20")
//            //    {
//            //        cacheProvider.Add(CachingKeys.RESEND_SCAN, msgId, msg); //新增重发缓存
//            //        //外部不做等待，TrySendScan里面有延时
//            //        retry = retry == null ? Retry.Instance() : retry;
//            //        result = retry.ExecuteFromMilliseconds<bool>(() => TrySendScan(clientId, msg, ref retryCount, sendSession, msgId), 0, 60, true);
//            //        cacheProvider.Remove(CachingKeys.RESEND_SCAN, msgId);  //删除重发缓存
//            //    }
//            //    else if (dtype == "aa")
//            //    {
//            //        result = TrySendMsg(clientId, msg, sendSession, msgId);
//            //    }
//            //    else  //其他类型重发,只做20次
//            //    {
//            //        cacheProvider.Add(CachingKeys.RESEND_SCAN, msgId, msg);
//            //        retry = retry == null ? Retry.Instance() : retry;
//            //        result = retry.ExecuteFromMilliseconds<bool>(() => TrySendCommon(clientId, msg, ref retryCount, sendSession, msgId), 0, 20, true);
//            //        cacheProvider.Remove(CachingKeys.RESEND_SCAN, msgId);
//            //    }

//            //    var flag = cacheProvider.HashKeyExist(CachingKeys.MAC_MONITOR_KEY, Utils.ParseDeviceKey(clientId));
//            //    if (flag || isscan || retryCount > 1 || EnvironmentVariables.random.Next(0, 300) == 4)
//            //    {
//            //        if (result)
//            //        {
//            //            Utils.WriteMsg($"获取会话等待次数:{retryCount},下发成功:{msg}", "SocketServer", $"SendMsg_{clientId}");
//            //        }
//            //        else
//            //        {
//            //            Utils.WriteMsg($"获取会话等待次数:{retryCount},下发失败:{msg}", "SocketServer", $"SendMsg_{clientId}");
//            //        }
//            //    }
//            //}
//            //catch (Exception ex)
//            //{
//            //    //_console.LogError($"Send Message error:{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}");
//            //    var errmsg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
//            //    Utils.WriteServiceLog("SocketServer", "SendMsg", $"消息:{msg},error:{errmsg}", true, "");
//            //}
//            //finally
//            //{
//            //    retry = null;
//            //}
//        }

//        /// <summary>
//        /// 启动函数
//        /// </summary>
//        public void Start()
//        {
//            //fromCenterService.Start();
//            server.Start();
//        }
//        /// <summary>
//        /// 终结器
//        /// </summary>
//        public void Dispose()
//        {
//            //tocenterService.Dispose();
//            //fromCenterService.Dispose();
//            server.Stop();
//            server.Dispose();
//        }





//    }
//}
