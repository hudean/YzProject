using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.SocketService.SuperSocketHelper
{
    /// <summary>
    /// 套接字客户端保持活动监视器
    /// </summary>
    public sealed class SocketClientKeepAliveMonitor : IDisposable
    {
        /// <summary>
        /// 上次数据包接收跟踪器
        /// </summary>
        private readonly Stopwatch _lastPacketReceivedTracker = new Stopwatch();
        /// <summary>
        /// 最后一个非保持活动状态数据包接收跟踪器
        /// </summary>
        private readonly Stopwatch _lastNonKeepAlivePacketReceivedTracker = new Stopwatch();

        private readonly string _clientId;
        /// <summary>
        /// 超时回调
        /// </summary>
        private readonly Func<Task> _timeoutCallback;
        public SocketClientKeepAliveMonitor(string clientId, Func<Task> timeoutCallback)
        {
            _clientId = clientId;
            _timeoutCallback = timeoutCallback;
        }
        public TimeSpan LastPacketReceived => _lastPacketReceivedTracker.Elapsed;

        public TimeSpan LastNonKeepAlivePacketReceived => _lastNonKeepAlivePacketReceivedTracker.Elapsed;

        public void Start(int keepAlivePeriod, CancellationToken cancellationToken)
        {
            if (keepAlivePeriod == 0)
            {
                return;
            }

            Task.Run(async () => await RunAsync(keepAlivePeriod, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
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
                //Utils.WriteMsg($"异常:{ex.Message}", "SocketClientKeepAliveMonitor", $"Socket.SocketClientKeepAliveMonitor");
            }
            catch (Exception ex)
            {
                //Utils.WriteMsg($"异常:{ex.Message}", "SocketClientKeepAliveMonitor", $"Socket.SocketClientKeepAliveMonitor");
            }
            finally
            {
            }
        }
        public void ReMonitor()
        {
            _lastPacketReceivedTracker.Restart();
            _lastNonKeepAlivePacketReceivedTracker.Restart();

        }

        public void Dispose()
        {
            if (_lastPacketReceivedTracker.IsRunning)
                _lastPacketReceivedTracker.Stop();
            if (_lastNonKeepAlivePacketReceivedTracker.IsRunning)
                _lastNonKeepAlivePacketReceivedTracker.Stop();
        }
    }
}
