using System;

namespace SocketServerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SocketServerService.OpenServer("127.0.0.1", "10086");
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
