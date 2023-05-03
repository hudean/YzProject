//using SuperSocket.SocketBase;
//using SuperSocket.SocketBase.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace YzProject.SocketService.SuperSocketHelper
//{
//    /// <summary>
//    /// （4）、建立一个AppSession,用来发送和接收客户端信息，一个客户端相当于一个session，这一点很重要，
//    /// 应为每一个RUT设备都有一个固定的编号，需要给session的item中添加Key值用来区分不同的客户端。
//    /// </summary>
//    public class SocketSession : AppSession<SocketSession> // AppSession<SocketSession, DTRequestInfo>
//    {
//        /// <summary>
//        /// MAC值
//        /// </summary>
//        public string ClientId { get; set; }

//        public SocketSession()
//        {
//            //var gb2312 = Encoding.GetEncoding("GB2312");
//            //this.Charset = gb2312;
//            KeepAliveMonitor = new SocketClientKeepAliveMonitor("", StopDueToKeepAliveTimeoutAsync);
//        }

//        public void ReceivePackage(StringRequestInfo requestInfo)
//        {
//            KeepAliveMonitor.ReMonitor();
//        }

//        /// <summary>
//        /// 会话启动时
//        /// </summary>
//        protected override void OnSessionStarted()
//        {
//            KeepAliveMonitor.Start(120, new CancellationTokenSource().Token);//活动监听保持 20*1.5D 提高到 120
//            base.OnSessionStarted();
//        }

//        /// <summary>
//        /// 会话关闭时
//        /// </summary>
//        /// <param name="reason"></param>
//        protected override void OnSessionClosed(CloseReason reason)
//        {
//            //base.OnSessionClosed(reason);
//            KeepAliveMonitor.Dispose();
//            _Close(reason);
//        }

//        protected override int GetMaxRequestLength()
//        {
//            return base.GetMaxRequestLength();
//        }

//        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
//        {
//            //base.HandleUnknownRequest(requestInfo);
//            _Close(CloseReason.ProtocolError);
//        }

//        //protected override void HandleUnknownRequest(DTRequestInfo requestInfo)
//        //{
//        //    base.HandleUnknownRequest(requestInfo);
//        //}

//        protected override void HandleException(Exception e)
//        {
//            //base.HandleException(e);
//            //Utils.WriteMsg($"SocketError:{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}", "SinoSocketSession", $"SocketError_{this.ClientId}");
//            _Close(CloseReason.SocketError);
//        }

//        private Task StopDueToKeepAliveTimeoutAsync()
//        {
//            return Task.Run(() =>
//            {
//                _Close(CloseReason.TimeOut);
//            });
//        }

//        public SocketClientKeepAliveMonitor KeepAliveMonitor { get; }

//        private void NotifyClosed()
//        {
//            if (string.IsNullOrEmpty(this.ClientId)) return;
//            try
//            {
//                //断开连接 发送关机消息到事件总线上
//                //var dto = new DeviceDisConnectEvent
//                //{
//                //    ClientId = this.ClientId
//                //};
//                //var impl = new DisconnectedServerImpl();
//                //impl.Send(dto);
//                //impl.Dispose();
//            }
//            catch { }
//        }
//        private void _Close(CloseReason reason)
//        {
//            var js = "";
//            if (this.RemoteEndPoint != null)
//            {
//                js = this.RemoteEndPoint.Address + ":" + this.RemoteEndPoint.Port;
//            }
//            //Utils.WriteMsg($"Close:{js},{reason.ToString()}", "SinoSocketSession", $"SessionClose_{this.ClientId}");

//            NotifyClosed();
//            base.OnSessionClosed(reason);
//            Close();
//        }
//    }
//}
