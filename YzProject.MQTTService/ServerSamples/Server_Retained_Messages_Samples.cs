﻿using MQTTnet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YzProject.MQTTService.ServerSamples
{
    /// <summary>
    /// 服务端保留消息样本
    /// </summary>
    public static class Server_Retained_Messages_Samples
    {
        /// <summary>
        /// 持久化保留消息
        /// </summary>
        /// <returns></returns>
        public static async Task Persist_Retained_Messages()
        {
            /*
             * This sample starts a simple MQTT server which will store all retained messages in a file.
             */

            var storePath = Path.Combine(Path.GetTempPath(), "RetainedMessages.json");

            var mqttFactory = new MqttFactory();

            // Due to security reasons the "default" endpoint (which is unencrypted) is not enabled by default!
            var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();

            using (var server = mqttFactory.CreateMqttServer(mqttServerOptions))
            {
                // Make sure that the server will load the retained messages.
                server.LoadingRetainedMessageAsync += async eventArgs =>
                {
                    try
                    {
                        eventArgs.LoadedRetainedMessages = await JsonSerializer.DeserializeAsync<List<MqttApplicationMessage>>(File.OpenRead(storePath));
                        Console.WriteLine("Retained messages loaded.");
                    }
                    catch (FileNotFoundException)
                    {
                        // Ignore because nothing is stored yet.
                        Console.WriteLine("No retained messages stored yet.");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                };

                // Make sure to persist the changed retained messages.
                server.RetainedMessageChangedAsync += async eventArgs =>
                {
                    try
                    {
                        // This sample uses the property _StoredRetainedMessages_ which will contain all(!) retained messages.
                        // The event args also contain the affected retained message (property ChangedRetainedMessage). This can be
                        // used to write all retained messages to dedicated files etc. Then all files must be loaded and a full list
                        // of retained messages must be provided in the loaded event.

                        var buffer = JsonSerializer.SerializeToUtf8Bytes(eventArgs.StoredRetainedMessages);
                        await File.WriteAllBytesAsync(storePath, buffer);
                        Console.WriteLine("Retained messages saved.");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                };

                // Make sure to clear the retained messages when they are all deleted via API.
                server.RetainedMessagesClearedAsync += _ =>
                {
                    File.Delete(storePath);
                    return Task.CompletedTask;
                };

                await server.StartAsync();

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
