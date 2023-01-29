using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerTest
{
    public class SocketServerService
    {
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 22000;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        public static string ErrorMsg = string.Empty;
       
      
        /// </summary>
        public static byte[] SendBuf = new byte[] { };

        /// <summary>
        /// 接收的字节
        /// </summary>
        public static List<byte> RevBuf { get; set; }
      
        public static IPEndPoint CurrentClient{ get; set; }

        /// <summary>
        /// 触发接收消息的委托
        /// </summary>
        public static bool _RevBool = false;
        public static event EventHandler RevBoolChanged = null;
        public static bool RevBool
        {
            get { return _RevBool; }
            set
            {
                if (_RevBool != value)
                {
                    _RevBool = value;
                    if (_RevBool)
                    {
                        RevBoolChanged?.Invoke(0, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 存储客户端连接Socket
        /// </summary>
        public static Dictionary<string, Socket> clientConnectionItems = new Dictionary<string, Socket> { };
        /// <summary>
        /// 打开服务器
        /// </summary>
        /// <returns></returns>
        public static bool OpenServer(string Ip, string Port)
        {
            try
            {
                IPAddress IP = IPAddress.Parse(Ip);
                IPEndPoint Point = new IPEndPoint(IP, int.Parse(Port));
                Socket ServerClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerClient.Bind(Point);
                ServerClient.Listen(10);
                ServerClient.BeginAccept(new AsyncCallback(AcceptCallback), ServerClient);
                //thServer = new Thread(new ThreadStart(RevState));
                //thServer.IsBackground = true;
                //thServer.Start();
                Console.WriteLine("服务器打开成功");
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                Console.WriteLine("服务器打开失败:" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="ar"></param>
        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = ar.AsyncState as Socket;
                if (listener != null)
                {
                    Socket handler = listener.EndAccept(ar);
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    IPEndPoint clientipe = (IPEndPoint)handler.RemoteEndPoint;
                    //txt_State.AppendText(clientipe.ToString() + "连上咯" + "\r\n");
                    clientConnectionItems.Add(clientipe.ToString(), handler);
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RevCallback), state);
                    Console.WriteLine(clientipe.ToString() + "----已连上服务器");
                }
                if (listener != null)
                {
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                //MyEvent(ErrorMsg);
            }
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="ar"></param>
        public static void RevCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            //Socket socketClient= ar.AsyncState as Socket;
            Socket handler = state.workSocket;
            if (handler != null)
            {
                IPEndPoint clientipe = (IPEndPoint)handler.RemoteEndPoint;
                try
                {
                    // Read data from the client socket.
                    int bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        byte[] a = new byte[bytesRead];
                        RevBuf = new List<byte>();
                        Buffer.BlockCopy(state.buffer, 0, a, 0, bytesRead);
                        RevBuf.AddRange(a);
                        //txt_Rev.AppendText(RevBuf[0].ToString() + "\r\n");
                        CurrentClient = clientipe;
                        //Send(handler, new byte[] { 0x00, 0x01, 0x02 });
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RevCallback), state);
                        RevBool = true;
                        RevBool = false;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = clientipe.ToString() + "退出";
                    //MyEvent(clientipe.ToString() + "----退出" + ex.Message);
                    //txt_State.AppendText(clientipe.ToString() + "退出" + ex.Message);                   
                }
            }

        }

        /// <summary>
        /// 发送回复客户端
        /// </summary>
        /// <param name="handle">客户端的Socket</param>
        public static void Send(Socket handle)
        {
            // Convert the string data to byte data using ASCII encoding.           
            // Begin sending the data to the remote device.
            if (SendBuf.Length != 0)//确保发送的字节长度不为0
            {
                handle.BeginSend(SendBuf, 0, SendBuf.Length, 0, new AsyncCallback(SendCallback), handle);
            }
            else
            {

            }
        }
        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;
            int bytesSent = handler.EndSend(ar);
            //handler.Shutdown(SocketShutdown.Both);
            //handler.Close();
        }

    }
}
