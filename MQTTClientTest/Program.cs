using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System;
using System.Threading.Tasks;

namespace MQTTClientTest
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //MqttClientService mqttClientService = new MqttClientService("ValidClientId", "127.0.0.1", 8001, "ValidUser", "SecretPassword");
            //await mqttClientService.CreateMqttClient();

            MqttClientService2 mqttClientService2 = new MqttClientService2();
            mqttClientService2.MqttClientStart();
            Console.ReadLine();
            mqttClientService2.Publish("hey,how are you!");
            Console.ReadKey();
        }


    }
}
