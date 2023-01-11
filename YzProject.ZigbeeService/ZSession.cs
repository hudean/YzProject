using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.ZigbeeService
{
    public class ZSession
    {
        private Queue<ReturnData> queueList;
        private ReturnData lastDto;
        private readonly string DeviceKey;
        private readonly string Channel;
        public string ClientId { get; }
        public ClientKeepAliveMonitor KeepAliveMonitor { get; }
        public ZSession(string clientId)
        {
            ClientId = clientId;
            //DeviceKey = Utils.ParseDeviceKey(clientId);
            //Channel = Utils.ParseCH(clientId);
            KeepAliveMonitor = new ClientKeepAliveMonitor(clientId, StopDueToKeepAliveTimeoutAsync);
            queueList = new Queue<ReturnData>();
        }

        public void SessionStart()
        {
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            KeepAliveMonitor.Start(15, cancellationToken);
            Task.Run(async () => await Save(cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 接收包
        /// </summary>
        /// <param name="dto"></param>
        public void ReceivePackage(ReturnData dto)
        {
            queueList.Enqueue(dto);
            KeepAliveMonitor.ReMonitor();
        }
        private async Task Save(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (queueList.Count > 0)
                    {
                        ReturnData dto = queueList.Dequeue();
                        //出队并处理处理比如数据发送到接口
                        //await FunData(dto);
                    }
                    await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex)
            {
                //Utils.WriteMsg($"异常:{ex.Message}", "ZSession", $"SNDataServerProxyService.ZSession");
            }
            catch (Exception ex)
            {
               // Utils.WriteMsg($"异常:{ex.Message}", "ZSession", $"SNDataServerProxyService.ZSession");
            }
            finally
            {
            }
        }

        /// <summary>
        /// 由于保持活动超时而停止
        /// </summary>
        /// <returns></returns>
        private Task StopDueToKeepAliveTimeoutAsync()
        {
            return Task.Run(async () =>
            {
                int i = 0;
                while (i++ < 3 && !await DisConnected()) ;
            });
        }

        private async Task<bool> DisConnected()
        {
            await Task.CompletedTask;
            return false;
            //Console.WriteLine($"Device:{DeviceKey} Disconnect.");
            //var dto = new SignalDeviceStateDataObject();
            //dto.Timestamp = DateTime.Now.ToString("yyyyMMddhhmmsss");
            //dto.Sign = dto.MakeSign();
            //dto.DeviceKey = DeviceKey;
            //dto.Channel = Channel;
            //return await Post("api/DeviceState/Disconnected", dto);
        }
    }
}
