using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Redis
{
    public class RedisCacheRepository : IRedisCacheRepository
    {
        /***
         * 文档地址
         * https://www.bookstack.cn/read/StackExchange.Redis-docs-cn/Basics.md
         * https://weihanli.github.io/StackExchange.Redis-docs-zh-cn/
         * https://stackexchange.github.io/StackExchange.Redis/Configuration
         * **/
        private readonly ILogger<RedisCacheRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        private readonly int _dbNumber;
        private readonly string _keyPrefix;

        public RedisCacheRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisCacheRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }
        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }


        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString());
        }

        #region

        //public IDatabase Database
        //{
        //    get
        //    {
        //        var db = _redis.GetDatabase(_dbNumber);
        //        if (_keyPrefix?.Length > 0)
        //            return db.WithKeyPrefix(_keyPrefix);
        //        return db;
        //    }
        //}
        ///// <summary>
        ///// 创建事务
        ///// </summary>
        ///// <returns></returns>
        //public ITransaction CreateTransaction() => Database.CreateTransaction();
        //public IServer GetServer(string hostAndPort)
        //{
        //    return _redis.GetServer(hostAndPort);
        //}

        #endregion

        #region 辅助方法

        private string ConvertToJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            //string result = JsonConvert.SerializeObject(value);
            return result;
        }

        private T ConvertToObject<T>(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default(T);
            if (typeof(T) == typeof(string))
            {
                //typeof(T).IsPrimitive || 
                //原生类型
                //return (T)value;
                //return value as T;
                return (T)Convert.ChangeType(value, typeof(T));
            }
            //有个坑DeserializeObject的值一定要SerializeObject才行
            return JsonConvert.DeserializeObject<T>(value);
        }

      

        private List<T> ConvetToList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertToObject<T>(item);
                result.Add(model);
            }
            return result;
        }

        #endregion


        #region Key

        public bool Exists(string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.KeyExists(key, flags);
        }
        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.KeyExistsAsync(key, flags);
        }

        public long Exists(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return _database.KeyExists(redisKeys, flags);
        }
        public Task<long> ExistsAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return _database.KeyExistsAsync(redisKeys, flags);
        }

        public bool Remove(string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.KeyDelete(key, flags);
        }
        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.KeyDeleteAsync(key, flags);
        }

        public long RemoveAll(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return _database.KeyDelete(redisKeys, flags);
        }
        public Task<long> RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return _database.KeyDeleteAsync(redisKeys, flags);
        }

        #endregion

        #region String

        public bool Add(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return _database.StringSet(key, value, null, when, flag);
        }

        public Task<bool> AddAsync(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return _database.StringSetAsync(key, value, null, when, flag);
        }

        public bool Add(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return _database.StringSet(key, value, expiry, when, flag);
        }

        public Task<bool> AddAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return _database.StringSetAsync(key, value, expiry, when, flag);
        }

        public bool Add(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var expiration = expiresAt.UtcDateTime.Subtract(DateTime.UtcNow);

            return _database.StringSet(key, value, expiration, when, flag);
        }

        public Task<bool> AddAsync(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var expiration = expiresAt.UtcDateTime.Subtract(DateTime.UtcNow);

            return _database.StringSetAsync(key, value, expiration, when, flag);
        }

        public bool Replace(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Add(key, value, When.Always, CommandFlags.None);
        }

        public Task<bool> ReplaceAsync(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return AddAsync(key, value, When.Always, CommandFlags.None);
        }

        public bool Replace(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Add(key, value, expiry, when, flag);
        }

        public Task<bool> ReplaceAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return AddAsync(key, value, expiry, when, flag);
        }

        public bool Replace(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Add(key, value, expiresAt, when, flag);
        }
       
        public Task<bool> ReplaceAsync(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return AddAsync(key, value, expiresAt, when, flag);
        }



        #endregion

        #region Hash

        /// <summary>
        /// 是否存在hashKey下的子key的数据
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public bool HashExists(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashExists(hashKey, key, flags);
        }
        public Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashExistsAsync(hashKey, key, flags);
        }

        public bool HashDelete(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDelete(hashKey, key, flags);
        }
      
        public Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDeleteAsync(hashKey, key, flags);
        }

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDelete(hashKey, keys.Select(x => (RedisValue)x).ToArray(), flags);
        }
        public Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), flags);
        }

        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson(value);
            return _database.HashSet(hashKey, key, json, nx ? When.NotExists : When.Always, flags);
        }

        public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson(value);
            return _database.HashSetAsync(hashKey, key, json, nx ? When.NotExists : When.Always, flags);
        }
        public void HashSet<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, ConvertToJson(kv.Value)));
            _database.HashSet(hashKey, entries.ToArray(), flags);
        }
        public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None)
        {
            //var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            var entries = values.Select(kv => new HashEntry(kv.Key, ConvertToJson(kv.Value)));
            return _database.HashSetAsync(hashKey, entries.ToArray(), flags);
        }

        public T HashGet<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = _database.HashGet(hashKey, key, flags);
            //return ConvertObj<T>(redisValue);
            return redisValue.HasValue ? ConvertToObject<T>(redisValue) : default;
        }

        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = await _database.HashGetAsync(hashKey, key, flags).ConfigureAwait(false);
            //return ConvertObj<T>(redisValue);
            return redisValue.HasValue ? ConvertToObject<T>(redisValue) : default;
        }

        public object HashGet(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashGet(hashKey, key, flags);
        }

        public async Task<object> HashGetAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var value = await _database.HashGetAsync(hashKey, key, flags);
            return value.IsNullOrEmpty ? null : JsonConvert.DeserializeObject(value);
        }

        public string HashGetString(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = _database.HashGet(hashKey, key, flags);
            return redisValue.IsNullOrEmpty ? null : redisValue.ToString();
        }

        public async Task<string> HashGetStringAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = await _database.HashGetAsync(hashKey, key, flags);
            return redisValue.IsNullOrEmpty ? null : redisValue.ToString();
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IList<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var result = new Dictionary<string, T>();
            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], HashGet<T>(hashKey, keys[i], flags));
            }

            return result;
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var tasks = new Task<T>[keys.Count];
            for (var i = 0; i < keys.Count; i++)
                tasks[i] = HashGetAsync<T>(hashKey, keys[i], commandFlags);
            await Task.WhenAll(tasks).ConfigureAwait(false);
            var result = new Dictionary<string, T>();
            for (var i = 0; i < tasks.Length; i++)
                result.Add(keys[i], tasks[i].Result);
            return result;
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashGetAll(hashKey, flags)
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => ConvertToObject<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await _database.HashGetAllAsync(hashKey, commandFlags).ConfigureAwait(false))
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => ConvertToObject<T>(x.Value),
                    StringComparer.Ordinal);
        }

        /// <summary>
        /// 为数字增长value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashIncrement(key, key, value, flags);
        }

        /// <summary>
        /// 为数字增长value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>增长后的值</returns>
        public Task<double> HashIncrementAsync(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashIncrementAsync(key, key, value, flags);
        }

        public long HashIncrement(string hashKey, string key, long value, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashIncrement(key, key, value, flags);
        }

        public Task<long> HashIncrementAsync(string hashKey, string key, long value, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashIncrementAsync(key, key, value, flags);
        }


        /// <summary>
        /// 为数字减少value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDecrement(key, key, value, flags);
        }

        /// <summary>
        /// 为数字减少value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>减少后的值</returns>
        public Task<double> HashDecrementAsync(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDecrementAsync(key, key, value, flags);
        }

        public long HashDecrement(string hashKey, string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDecrement(key, key, value, flags);
        }
        public Task<long> HashDecrementAsync(string hashKey, string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashDecrementAsync(key, key, value, flags);
        }

        public long HashLength(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashLength(hashKey, flags);
        }
        public Task<long> HashLengthAsync(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashLengthAsync(hashKey, flags);
        }


        public IEnumerable<string> HashKeys(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashKeys(hashKey, flags).Select(x => x.ToString());
            // RedisValue[] values = _database.HashKeys(hashKey, flags);
            //return values.Select(x => x.ToString()).ToList();
        }
        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return (await _database.HashKeysAsync(hashKey, flags).ConfigureAwait(false)).Select(x => x.ToString());
            // RedisValue[] values = _database.HashKeys(hashKey, flags);
            //return values.Select(x => x.ToString()).ToList();
        }
        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashValues(hashKey, flags).Select(x => ConvertToObject<T>(x));
        }
        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return (await _database.HashValuesAsync(hashKey, flags).ConfigureAwait(false)).Select(x => ConvertToObject<T>(x));
        }


        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags flags = CommandFlags.None)
        {
            return _database.HashScan(hashKey, pattern, pageSize, flags).ToDictionary(x => x.Name.ToString(), x => ConvertToObject<T>(x.Value), StringComparer.Ordinal);
        }

        #endregion

        #region SortedSet 有序集合

        public bool SortedSetAdd<T>(string key, T value, double score, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson<T>(value);
            return _database.SortedSetAdd(key, json, score, flags);
        }

        public Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson<T>(value);
            return _database.SortedSetAddAsync(key, json, score, flags);
        }
        public bool SortedSetRemove<T>(string key, T value, CommandFlags flags = CommandFlags.None)
        {
            return _database.SortedSetRemove(key, ConvertToJson(value), flags);
        }

        public Task<bool> SortedSetRemoveAsync<T>(string key, T value, CommandFlags flags = CommandFlags.None)
        {
            return _database.SortedSetRemoveAsync(key, ConvertToJson(value), flags);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetRangeByRank<T>(
                                    string key,
                                    long start = 0,
                                    long stop = -1,
                                    Order order = Order.Ascending,
                                    CommandFlags flags = CommandFlags.None)
        {

            var result = _database.SortedSetRangeByRank(key, start, stop, order, flags);
            //return ConvetToList<T>(result);
            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(
                                    string key,
                                    long start = 0,
                                    long stop = -1,
                                    Order order = Order.Ascending,
                                    CommandFlags flags = CommandFlags.None)
        {

            var result = await _database.SortedSetRangeByRankAsync(key, start, stop, order, flags).ConfigureAwait(false);
            //return ConvetToList<T>(result);
            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }


        public IEnumerable<T> SortedSetRangeByScore<T>(
                                   string key,
                                   double start = double.NegativeInfinity,
                                   double stop = double.PositiveInfinity,
                                   Exclude exclude = Exclude.None,
                                   Order order = Order.Ascending,
                                   long skip = 0L,
                                   long take = -1L,
                                   CommandFlags flags = CommandFlags.None)
        {
            var result = _database.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);

            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }

        public async Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(
                                  string key,
                                  double start = double.NegativeInfinity,
                                  double stop = double.PositiveInfinity,
                                  Exclude exclude = Exclude.None,
                                  Order order = Order.Ascending,
                                  long skip = 0L,
                                  long take = -1L,
                                  CommandFlags flags = CommandFlags.None)
        {
            var result = await _database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags).ConfigureAwait(false);

            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }


        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return _database.SortedSetLength(key, min, max, exclude, flags);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> SortedSetLengthAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return _database.SortedSetLengthAsync(key, min, max, exclude, flags);
        }

        #endregion

        #region 发布订阅/PubSub


        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _redis.GetSubscriber();
            return sub.Publish(channel, ConvertToJson(message), flags);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _redis.GetSubscriber();
            return sub.PublishAsync(channel, ConvertToJson(message), flags);
        }

        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _redis.GetSubscriber();

            sub.Subscribe(channel, (redisChannel, value) => handler(ConvertToObject<T>(value)), flags);
        }

        public Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _redis.GetSubscriber();
            return sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(ConvertToObject<T>(value)).ConfigureAwait(false), flags);
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            ISubscriber sub = _redis.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            }, flags);
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public Task SubscribeAsync(string subChannel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            ISubscriber sub = _redis.GetSubscriber();
            return sub.SubscribeAsync(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            }, flags);
        }
       
        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _redis.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(ConvertToObject<T>(value)), flags);
        }

        public Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _redis.GetSubscriber();
            return sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(ConvertToObject<T>(value)), flags);
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = _redis.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public Task UnsubscribeAsync(string channel)
        {
            ISubscriber sub = _redis.GetSubscriber();
            return sub.UnsubscribeAsync(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = _redis.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = _redis.GetSubscriber();
            return sub.UnsubscribeAllAsync(flags);
        }

      
        public bool UpdateExpiry(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (_database.KeyExists(key))
                return _database.KeyExpire(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow), flags);

            return false;
        }

        public async Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
                return await _database.KeyExpireAsync(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow), flags).ConfigureAwait(false);

            return false;
        }

        public bool UpdateExpiry(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (_database.KeyExists(key))
                return _database.KeyExpire(key, expiresIn, flags);

            return false;
        }

        public async Task<bool> UpdateExpiryAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
                return await _database.KeyExpireAsync(key, expiresIn, flags).ConfigureAwait(false);

            return false;
        }

        public IDictionary<string, bool> UpdateExpiryAll(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);
            for (var i = 0; i < keys.Length; i++)
            {
                results.Add(keys[i], UpdateExpiry(keys[i], expiresAt.UtcDateTime, flags));
            }
            return results;
        }

        public async Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            var tasks = new Task<bool>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = UpdateExpiryAsync(keys[i], expiresAt.UtcDateTime, flags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);

            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], tasks[i].Result);

            return results;
        }

        public IDictionary<string, bool> UpdateExpiryAll(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);
            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], UpdateExpiry(keys[i], expiresIn, flags));

            return results;
        }

        public async Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            var tasks = new Task<bool>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = UpdateExpiryAsync(keys[i], expiresIn, flags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);

            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], tasks[i].Result);

            return results;
        }


        #endregion
    }
}
