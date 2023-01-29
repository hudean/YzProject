using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest
{
    internal class SocketClientService
    {
        public class StateObject
        {
            // 当前客户端的Socket
            public Socket workSocket = null;
            // 可接收的最大字节数
            public const int BufferSize = 20200;
            // 接收的数据存储
            public byte[] buffer = new byte[BufferSize];
        }
        public static List<byte> RevBuf;
        public static bool _BoolRevContent = false;
        public static bool BoolRevContent
        {
            get { return _BoolRevContent; }
            set { _BoolRevContent = value; }
        }
        public static Socket clientT;
        public static bool ConnectServercer(string ip, string port)
        {
            try
            {
                IPAddress IP = IPAddress.Parse(ip);
                IPEndPoint Point = new IPEndPoint(IP, int.Parse(port));
                clientT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientT.BeginConnect(Point, new AsyncCallback(ConnectCallback), clientT);
                //connectDone.WaitOne();
                //byte[] A = new byte[] { 0x00, 0x02, 0x06 };
                //Send(client, A);
                //sendDone.WaitOne();
                Receive(clientT);
                //receiveDone.WaitOne();                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            //connectDone.Set();
        }
        private static void Receive(Socket client)
        {
            StateObject state = new StateObject();
            state.workSocket = client;
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;
            int bytesRead = client.EndReceive(ar);
            //byte[] Conn = state.buffer;
            if (bytesRead > 0)
            {
                BoolRevContent = true;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                RevBuf = new List<byte>();
                byte[] ActConn = new byte[bytesRead];
                Buffer.BlockCopy(state.buffer, 0, ActConn, 0, bytesRead);
                RevBuf.AddRange(ActConn);
                BoolRevContent = false;
            }
            else
            {

            }
        }

        public static bool Send(byte[] data)
        {
            try
            {
                clientT.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), clientT);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {

            }
        }
    }
}



