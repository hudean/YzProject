//using Microsoft.Extensions.Logging;
//using SuperSocket.SocketBase;
//using SuperSocket.SocketBase.Config;
//using SuperSocket.SocketBase.Protocol;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Intrinsics.Arm;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;

//namespace YzProject.SocketService.SuperSocketHelper
//{
//    /// <summary>
//    /// 5、建立AppServer，自定义适合自己项目的服务。
//    /// 这个项目需要的就是服务端给客户端RTU发送数据请求指令，客户端才能做出响应返回数据十六进制的数据报文。
//    /// </summary>
//    public class SocketServer : AppServer<SocketSession>//AppServer<SocketSession, DTRequestInfo>
//    {
//        private readonly ILogger _console;
//        //private readonly Dictionary<string, SocketSession> _sessions = new Dictionary<string, SocketSession>();
//        private readonly ConcurrentDictionary<string, SocketSession> _sessions = new ConcurrentDictionary<string, SocketSession>();
//        public SocketServer(ILogger consoleLog)
//        {
//            _console = consoleLog;
//        }
//        public void SetSession(string clientId, SocketSession session)
//        {
//            _sessions[clientId] = session;
//        }
//        public void DelSession(SocketSession session)
//        {
//            if (_sessions.ContainsKey(session.ClientId))
//            {
//                //_sessions.Remove(session.ClientId);
//                //SocketSession socketSession;
//                _sessions.Remove(session.ClientId,out SocketSession socketSession);
//            }
            
//        }
//        public SocketSession GetSocketSession(string clientId)
//        {
//            if (_sessions.ContainsKey(clientId))
//                return _sessions[clientId];
//            return null;
//        }
//    }

//}
