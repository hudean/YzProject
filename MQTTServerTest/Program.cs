using MQTTnet.Server;
using System;
using System.Threading.Tasks;

namespace MQTTServerTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MqttServerService mqttServerService = new MqttServerService();
            await  mqttServerService.StartServer();
            MqttService._mqttServer = mqttServerService.mqttServer;
            Console.ReadLine();
            MqttService.PublishData("I'm Server");
            Console.ReadKey();
        } 
    }
}
