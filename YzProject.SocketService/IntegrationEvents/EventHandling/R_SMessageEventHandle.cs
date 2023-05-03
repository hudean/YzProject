using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Common;
using YzProject.EventBus.Abstractions;
using YzProject.EventBus.Events;
using YzProject.SocketService.IntegrationEvents.Events;
using YzProject.SocketService.SuperSocketHelper;

namespace YzProject.SocketService.IntegrationEvents.EventHandling
{
    public class R_SMessageEventHandle : IIntegrationEventHandler<R_SMessageEvent>
    {

        private readonly AppSocketServer socketServer;

        public R_SMessageEventHandle(AppSocketServer server)
        {
            this.socketServer = server;
        }

        public async Task Handle(R_SMessageEvent @event)
        {
            //throw new NotImplementedException();
            Crypto crypto = new Crypto();
            var content = crypto.Base64Decode(@event.Content);
            string msg = string.Empty;

            //var flag = await cacheProvider.HashKeyExistAsyn(CachingKeys.MAC_MONITOR_KEY, message.DeviceKey);
            //if (flag)
            //{
            //    var js = JsonConvert.SerializeObject(message);
            //    Utils.WriteMsg($"下发成功:{js}", "Socket", $"SendMsg_{message.DeviceKey}");
            //}

            //if (message.DType != "aa")
            //    logger.LogDebug($"[SocketManager]Send Message To Device{message.DeviceKey} Channel:{message.CH} Content:{content}");
            var bytes = Encoding.UTF8.GetBytes(content);
            var gb2312 = Encoding.GetEncoding("GB2312");
            var bts = Encoding.Convert(Encoding.UTF8, gb2312, bytes);

            socketServer.SendMsg(@event.DeviceKey.ToUpper(), gb2312.GetString(bts), @event.DType, @event.MsgId);
            await Task.FromResult(true);
            //return await Task.FromResult(true);
        }
    }
}
