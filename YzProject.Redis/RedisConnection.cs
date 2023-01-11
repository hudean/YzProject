using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Redis
{
    /// <summary>
    /// 获取Redis连接池ConnectionMultiplexer（如果使用asp.net core依赖注入使用另一种方式）
    /// </summary>
    public static class RedisConnection
    {
        //private static readonly ILogger<RedisConnection> logger;
        private static readonly object @lock = new object();
        private static ConnectionMultiplexer _connectionMultiplexer;
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private static ConfigurationOptions configurationOptions = ConfigurationOptions.Parse("127.0.0.1:6379,password=123456,connectTimeout=2000");

        /// <summary>
        /// 键头字符串
        /// </summary>
        public static readonly string KeyPrefix = "HDA_";
        // https://developer.aliyun.com/article/272212
        //https://www.coder.work/article/1578168 对象池
        ///// <summary>
        ///// 单例获取
        ///// </summary>
        //public static ConnectionMultiplexer GetConnectionMultiplexer
        //{
        //    get
        //    {
        //        if (_connectionMultiplexer == null)
        //        {
        //            lock (@lock)
        //            {
        //                if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
        //                {
        //                    _connectionMultiplexer = GetManager();
        //                    //_connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        //                }
        //            }
        //        }
        //        return _connectionMultiplexer;
        //    }
        //}

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer GetConnectionMultiplexer()
        {
            if (_connectionMultiplexer == null)
            {
                lock (@lock)
                {
                    if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
                    {
                        _connectionMultiplexer = GetManager(configurationOptions);
                        //_connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
                    }
                }
            }
            return _connectionMultiplexer;
        }


        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = GetManager(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        //public static ConnectionMultiplexer GetConnection(RedisConfigOptions config)
        //{
        //    string key = config.Url + config.Port;
        //    if (!ConnectionCache.ContainsKey(key))
        //    {
        //        var options = new ConfigurationOptions()
        //        {
        //            SyncTimeout = config.Timeout,
        //            EndPoints =
        //            {
        //                {config.Url,config.Port }
        //            },
        //            Password = config.Password
        //        };
        //        var multiplexer = ConnectionMultiplexer.Connect(options);
        //        ConnectionCache[key] = GetConnectionMultiplexer(multiplexer);
        //    }
        //    return ConnectionCache[key];
        //}

        private static ConnectionMultiplexer GetManager(string connectionString)
        {
            //connectionString = connectionString ?? "localhost";
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString is not null or not empty");
            }
            var connect = ConnectionMultiplexer.Connect(connectionString);

            //注册如下事件
            connect.ConnectionFailed += ConnectionFailed;
            connect.ConnectionRestored += ConnectionRestored;
            connect.ErrorMessage += ErrorMessage;
            connect.ConfigurationChanged += ConfigurationChanged;
            connect.HashSlotMoved += HashSlotMoved;
            connect.InternalError += InternalError;
            return connect;
        }

        private static ConnectionMultiplexer GetManager(ConfigurationOptions config)
        {
            var connect = ConnectionMultiplexer.Connect(config);

            //注册如下事件
            connect.ConnectionFailed += ConnectionFailed;
            connect.ConnectionRestored += ConnectionRestored;
            connect.ErrorMessage += ErrorMessage;
            connect.ConfigurationChanged += ConfigurationChanged;
            connect.HashSlotMoved += HashSlotMoved;
            connect.InternalError += InternalError;

            return connect;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConfigurationChanged(object sender, EndPointEventArgs e)
        {

        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            //logger.LogError("Redis error: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            //logger.LogError("Redis connection error restored.");
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            //logger.LogError(e.Exception, "Redis connection error {0}.", e.FailureType);
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {

        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void InternalError(object sender, InternalErrorEventArgs e)
        {
            //logger.LogError(e.Exception, "Redis internal error {0}.", e.Origin);
        }

        #endregion

    }
}
