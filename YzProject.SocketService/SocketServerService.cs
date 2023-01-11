using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.SocketService
{
    public class SocketServerService
    {
        private static Socket socketWatch;
        public void StartSocketService()
        {
            try
            {
                //string ipAddress = "127.0.0.1";
                string ipPort = "10086";
                //实例化一个Socket对象，确定网络类型、Socket类型、协议类型
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Any;//IPAddress.Parse(ipAddress);
                //创建端口号对象
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(ipPort));
                //绑定ip和端口
                socketWatch.Bind(point);
                //监听
                socketWatch.Listen(500);
                //开启线程
                Thread thread = new Thread(SocketListen);
                thread.IsBackground = true;
                //thread.Start(socketWatch);
                thread.Start();
            }
            catch (Exception ex)
            { 
            
            }
        }

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
                        //threadSocketServerRecive.Start(socketSend);
                        thread.Start();
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
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}
