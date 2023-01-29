using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.SocketService
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

    public class SocketServerService
    {
        private static Socket socketWatch;

        //public static Dictionary<string, Socket> ClientConnectionItems = new Dictionary<string, Socket>();

        /// <summary>
        /// 存储客户端连接socket
        /// </summary>
        public static ConcurrentDictionary<string, Socket> ClientConnectionItems { get; set; } = new ConcurrentDictionary<string, Socket>();

        public void StartSocketService()
        {
            RunSocketServer("127.0.0.1", 10086);
            #region
            //try
            //{
            //    //string ipAddress = "127.0.0.1";
            //    string ipPort = "10086";
            //    //实例化一个Socket对象，确定网络类型、Socket类型、协议类型
            //    socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    IPAddress ip = IPAddress.Any;//IPAddress.Parse(ipAddress);
            //    //创建端口号对象
            //    IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(ipPort));
            //    //绑定ip和端口
            //    socketWatch.Bind(point);
            //    //监听
            //    socketWatch.Listen(500);
            //    //开启线程
            //    Thread thread = new Thread(SocketListen);
            //    thread.IsBackground = true;
            //    //thread.Start(socketWatch);
            //    thread.Start();
            //}
            //catch (Exception ex)
            //{

            //}
            #endregion
        }



        #region 方式一

        private void SocketListen()//object o
        {
            //Socket socketWatch = o as Socket;
            while (true)
            {
                try
                {
                    if (socketWatch != null)
                    {
                        //负责跟客户端通信的socket，等待客户端的连接,并且创建一个负责通信的socket
                        var socket = socketWatch.Accept();
                        //获取客户端ip和端口号
                        var str = socket.RemoteEndPoint.ToString();
                        //开启一个新线程不停的接收来自客户端的消息.
                        Thread thread = new Thread(SocketRecive);
                        thread.IsBackground = true;
                        thread.Start(socket);
                        //thread.Start();
                    }
                }
                catch (Exception ex)
                {
                    //m_log.Error("Socket服务监听失败，报错信息：" + ex.Message);
                }
            }
        }

        private void SocketRecive(object obj)
        {
            Socket recviceSocket = obj as Socket;
            while (true)
            {
                try
                {
                    if (recviceSocket != null && recviceSocket.Connected)
                    {
                        //客户端连接成功后，服务器应该接收来自客户端的消息
                        byte[] buffer = new byte[1024 * 1024];

                        //实际接收到的有效字节数
                        int r = recviceSocket.Receive(buffer);
                        //当结束
                        if (r == 0)
                        {
                            break;
                        }
                        string str = Encoding.GetEncoding("utf-8").GetString(buffer, 0, r);
                        string socketMsg = str.Substring(str.IndexOf("{"));
                        //WorkstationStateModel stationState = JsonConvert.DeserializeObject<WorkstationStateModel>(socketMsg);
                        //if (stationState != null)
                        //{

                        //}
                        //回复
                        //recviceSocket.Send();
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        #endregion

        #region 方式二


        /// <summary>
        /// 打开Socket服务端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void RunSocketServer(string ip, int port)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                //创建端口号对象
                IPEndPoint point = new IPEndPoint(ipAddress, port);
                //实例化一个Socket对象，确定网络类型、Socket类型、协议类型
                Socket serverClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                serverClient.Bind(point);
                serverClient.Listen(500);
                serverClient.BeginAccept(new AsyncCallback(AcceptCallback), serverClient);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = ar.AsyncState as Socket;
                if (listener != null)
                {
                    Socket handler = listener.EndAccept(ar);
                    IPEndPoint point = (IPEndPoint)handler.RemoteEndPoint;
                    ClientConnectionItems.TryAdd(point.ToString(), handler);
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    handler.BeginAccept(new AsyncCallback(AcceptCallback), handler);
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReciveCallback), state);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReciveCallback(IAsyncResult ar)
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
                        //RevBuf = new List<byte>();
                        Buffer.BlockCopy(state.buffer, 0, a, 0, bytesRead);
                        //RevBuf.AddRange(a);
                        ////txt_Rev.AppendText(RevBuf[0].ToString() + "\r\n");
                        //CurrentClient = clientipe;
                        //Send(handler, new byte[] { 0x00, 0x01, 0x02 });
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReciveCallback), state);
                        //RevBool = true;
                        //RevBool = false;
                    }
                }
                catch (Exception ex)
                {
                    //txt_State.AppendText(clientipe.ToString() + "退出" + ex.Message);                   
                }
            }
        }


        /// <summary>
        /// 发送回复客户端
        /// </summary>
        /// <param name="handle">客户端的Socket</param>
        public static void Send(Socket handle, byte[] sendBuffer)
        {
            // Convert the string data to byte data using ASCII encoding.           
            // Begin sending the data to the remote device.
            if (sendBuffer.Length != 0)//确保发送的字节长度不为0
            {
                handle.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SendCallback), handle);
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

        #endregion
    }
}
