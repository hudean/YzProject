using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.EventBusRabbitMQ
{
    /// <summary>
    /// RabbitMQ持续连接
    /// </summary>
    public interface IRabbitMQPersistentConnection
         : IDisposable
    {
        /// <summary>
        /// 已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        IModel CreateModel();
    }
}
