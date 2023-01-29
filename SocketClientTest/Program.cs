using System;

namespace SocketClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SocketClientService.ConnectServercer("127.0.0.1", "10086");
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
