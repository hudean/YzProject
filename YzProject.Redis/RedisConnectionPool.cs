using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Redis
{
    /// <summary>
    /// 不推荐使用自己写的RedisConnectionPool 建议使用扩展包自带的RedisConnectionPoolManager
    /// 低版本使用RedisCacheConnectionPoolManager
    /// </summary>
    public class RedisConnectionPool : IRedisCacheConnectionPoolManager
    {
        private static ConcurrentBag<Lazy<ConnectionMultiplexer>> connections;
        private readonly RedisConfiguration redisConfiguration;

        //private readonly static object @lock = new();
        //private readonly IStateAwareConnection[] connections;
        //private readonly RedisConfiguration redisConfiguration;
        //private readonly ILogger<RedisConnectionPool> logger;
        //private readonly Random random = new();
        private bool isDisposed;

        public RedisConnectionPool(RedisConfiguration redisConfiguration)
        {
            this.redisConfiguration = redisConfiguration;
            Initialize();

        }

        //public RedisConnectionPool(int dbNum, RedisConfig config)
        //{
        //    RedisConfiguration redisConfiguration = new RedisConfiguration()
        //    {
        //        AbortOnConnectFail = config.AbortOnConnectFail,
        //        Hosts = new RedisHost[] {
        //                              new RedisHost()
        //                              {
        //                                  Host = config.Url,
        //                                  Port = config.Port
        //                              },
        //                            },
        //        ConnectTimeout = config.ConnectTimeout,
        //        Database = dbNum,
        //        Ssl = false,
        //        Password = config.Password,
        //        ServerEnumerationStrategy = new ServerEnumerationStrategy()
        //        {
        //            Mode = ServerEnumerationStrategy.ModeOptions.All,
        //            TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
        //            UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
        //        },
        //        PoolSize = 1000
        //    };
        //    this.redisConfiguration = redisConfiguration;
        //    Initialize();
        //}

        public void Dispose()
        {
            var activeConnections = connections.Where(lazy => lazy.IsValueCreated).ToList();
            activeConnections.ForEach(connection => connection.Value.Dispose());
            Initialize();
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }


        //private void Dispose(bool disposing)
        //{
        //    if (isDisposed)
        //        return;

        //    if (disposing)
        //    {
        //        // free managed resources
        //        foreach (var connection in connections)
        //            connection.Dispose();
        //    }

        //    isDisposed = true;
        //}


        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public IConnectionMultiplexer GetConnection()
        {
            Lazy<ConnectionMultiplexer> response;
            var loadedLazys = connections.Where(lazy => lazy.IsValueCreated);

            if (loadedLazys.Count() == connections.Count)
            {
                response = connections.OrderBy(x => x.Value.GetCounters().TotalOutstanding).First();
            }
            else
            {
                response = connections.First(lazy => !lazy.IsValueCreated);
            }

            return response.Value;
        }

       

        /// <summary>
        /// 连接池信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ConnectionPoolInformation GetConnectionInformations()
        {
            //throw new NotImplementedException();
            var activeConnections = 0;
            var invalidConnections = 0;

            foreach (var connection in connections)
            {
                if (!(connection?.Value?.IsConnected ?? false))
                {
                    invalidConnections++;
                    continue;
                }

                activeConnections++;
            }

            return new()
            {
                RequiredPoolSize = redisConfiguration.PoolSize,
                ActiveConnections = activeConnections,
                InvalidConnections = invalidConnections
            };
        }



        private void Initialize()
        {
            connections = new ConcurrentBag<Lazy<ConnectionMultiplexer>>();

            for (int i = 0; i < redisConfiguration.PoolSize; i++)
            {
                connections.Add(new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConfiguration.ConfigurationOptions)));
            }
        }
    }
}
