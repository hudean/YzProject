using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.ZigbeeService
{
    /// <summary>
    /// 客户端保持活动监视器
    /// </summary>
    public class ClientKeepAliveMonitor
    {
        /// <summary>
        /// 最后收到的数据包跟踪器
        /// </summary>
        private readonly Stopwatch _lastPacketReceivedTracker = new Stopwatch();
        /// <summary>
        /// 最后一个非保持活动数据包接收跟踪器
        /// </summary>
        private readonly Stopwatch _lastNonKeepAlivePacketReceivedTracker = new Stopwatch();
        /// <summary>
        /// 客户端id
        /// </summary>
        private readonly string _clientId;
        /// <summary>
        /// 超时回调委托
        /// </summary>
        private readonly Func<Task> _timeoutCallback;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientId">客户端id</param>
        /// <param name="timeoutCallback">超时回调委托</param>
        public ClientKeepAliveMonitor(string clientId, Func<Task> timeoutCallback)
        {
            _clientId = clientId;
            _timeoutCallback = timeoutCallback;
        }
        public TimeSpan LastPacketReceived => _lastPacketReceivedTracker.Elapsed;

        public TimeSpan LastNonKeepAlivePacketReceived => _lastNonKeepAlivePacketReceivedTracker.Elapsed;

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="keepAlivePeriod">保活期</param>
        /// <param name="cancellationToken"></param>
        public void Start(int keepAlivePeriod, CancellationToken cancellationToken)
        {
            if (keepAlivePeriod == 0)
            {
                return;
            }

            Task.Run(async () => await RunAsync(keepAlivePeriod, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="keepAlivePeriod">保活期</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task RunAsync(int keepAlivePeriod, CancellationToken cancellationToken)
        {
            try
            {
                _lastPacketReceivedTracker.Restart();
                _lastNonKeepAlivePacketReceivedTracker.Restart();

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_lastPacketReceivedTracker.Elapsed.TotalSeconds > keepAlivePeriod * 1.5D)
                    {

                        if (_timeoutCallback != null)
                        {
                            await _timeoutCallback().ConfigureAwait(false);
                        }

                        return;
                    }

                    await Task.Delay(keepAlivePeriod, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex)
            {
                //Utils.WriteMsg($"异常:{ex.Message}", "ClientKeepAliveMonitor", $"SNDataServer.ClientKeepAliveMonitor");
            }
            catch (Exception ex)
            {
                //Utils.WriteMsg($"异常:{ex.Message}", "ClientKeepAliveMonitor", $"SNDataServer.ClientKeepAliveMonitor");
            }
            finally
            {
            }
        }

        /// <summary>
        /// 重新监控
        /// </summary>
        public void ReMonitor()
        {
            _lastPacketReceivedTracker.Restart();
            _lastNonKeepAlivePacketReceivedTracker.Restart();

        }
    }
}
