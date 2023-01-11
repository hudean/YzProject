using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Redis
{
    /// <summary>
    /// redis仓储接口(推荐使用异步的方式)
    /// </summary>
    public interface IRedisCacheRepository
    {
        IEnumerable<string> GetUsers();

        #region Key
    
        public bool Exists(string key, CommandFlags flags = CommandFlags.None);
        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None);

        public long Exists(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);
        public Task<long> ExistsAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        public bool Remove(string key, CommandFlags flags = CommandFlags.None);
        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None);

        public long RemoveAll(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);
        public Task<long> RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        #endregion

        #region String

        public bool Add(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None);
        public Task<bool> AddAsync(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None);
        public bool Add(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None);
        public Task<bool> AddAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None);
        public bool Add(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None);
        public Task<bool> AddAsync(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None);
        public bool Replace(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None);
        public Task<bool> ReplaceAsync(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None);
        public bool Replace(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None);
        public Task<bool> ReplaceAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None);
        public bool Replace(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None);

        public Task<bool> ReplaceAsync(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None);

        #endregion

        #region Hash

        /// <summary>
        /// 是否存在hashKey下的子key的数据
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public bool HashExists(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public bool HashDelete(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        public Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags flags = CommandFlags.None);

        public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags flags = CommandFlags.None);

        public void HashSet<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None);
        
        public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None);

        public T HashGet<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public object HashGet(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public Task<object> HashGetAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public string HashGetString(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public Task<string> HashGetStringAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        public Dictionary<string, T> HashGet<T>(string hashKey, IList<string> keys, CommandFlags flags = CommandFlags.None);

        public Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None);

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags flags = CommandFlags.None);
        public Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 为数字增长value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 为数字增长value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>增长后的值</returns>
        public Task<double> HashIncrementAsync(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None);

        public long HashIncrement(string hashKey, string key, long value, CommandFlags flags = CommandFlags.None);
        public Task<long> HashIncrementAsync(string hashKey, string key, long value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 为数字减少value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 为数字减少value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>减少后的值</returns>
        public Task<double> HashDecrementAsync(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None);

        public long HashDecrement(string hashKey, string key, long value = 1, CommandFlags flags = CommandFlags.None);
        public Task<long> HashDecrementAsync(string hashKey, string key, long value = 1, CommandFlags flags = CommandFlags.None);


        public long HashLength(string hashKey, CommandFlags flags = CommandFlags.None);
        public Task<long> HashLengthAsync(string hashKey, CommandFlags flags = CommandFlags.None);


        public IEnumerable<string> HashKeys(string hashKey, CommandFlags flags = CommandFlags.None);
        public Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags flags = CommandFlags.None);


        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags flags = CommandFlags.None);

        public Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None);
        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags flags = CommandFlags.None);



        #endregion

        #region SortedSet 有序集合

        /// <summary>
        /// 有序集合添加
        /// </summary>
        /// <typeparam name="T">值的实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="score"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(string key, T value, double score, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步有序集合添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CommandFlags flags = CommandFlags.None);


        /// <summary>
        /// 有序集合删除
        /// </summary>
        /// <typeparam name="T">值的实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public bool SortedSetRemove<T>(string key, T value, CommandFlags flags = CommandFlags.None);



        /// <summary>
        /// 异步有序集合删除
        /// </summary>
        /// <typeparam name="T">值的实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task<bool> SortedSetRemoveAsync<T>(string key, T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 根据排序获取指定范围
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetRangeByRank<T>(
                                    string key,
                                    long start = 0,
                                    long stop = -1,
                                    Order order = Order.Ascending,
                                    CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步根据排序获取指定范围
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(
                                    string key,
                                    long start = 0,
                                    long stop = -1,
                                    Order order = Order.Ascending,
                                    CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 根据分数获取指定范围
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="exclude"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetRangeByScore<T>(
                                   string key,
                                   double start = double.NegativeInfinity,
                                   double stop = double.PositiveInfinity,
                                   Exclude exclude = Exclude.None,
                                   Order order = Order.Ascending,
                                   long skip = 0L,
                                   long take = -1L,
                                   CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步根据分数获取指定范围
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="exclude"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(
                                   string key,
                                   double start = double.NegativeInfinity,
                                   double stop = double.PositiveInfinity,
                                   Exclude exclude = Exclude.None,
                                   Order order = Order.Ascending,
                                   long skip = 0L,
                                   long take = -1L,
                                   CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="exclude"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long SortedSetLength(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="exclude"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<long> SortedSetLengthAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        #endregion

        #region 发布订阅/PubSub

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T">消息实体类型</typeparam>
        /// <param name="channel">通道</param>
        /// <param name="message">消息</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步发布
        /// </summary>
        /// <typeparam name="T">消息实体类型</typeparam>
        /// <param name="channel">通道</param>
        /// <param name="message">消息实体</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T">消息实体类型</typeparam>
        /// <param name="channel">通道</param>
        /// <param name="handler">处理委托</param>
        /// <param name="flags">命令标记</param>
        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步订阅
        /// </summary>
        /// <typeparam name="T">消息实体类型</typeparam>
        /// <param name="channel">通道</param>
        /// <param name="handler">消息实体</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel">订阅通道</param>
        /// <param name="handler">处理委托</param>
        /// <param name="flags">命令标记</param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis发布订阅  异步订阅
        /// </summary>
        /// <param name="subChannel">订阅通道</param>
        /// <param name="handler">委托处理</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task SubscribeAsync(string subChannel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T">消息实体类型</typeparam>
        /// <param name="channel">通道</param>
        /// <param name="handler">委托处理</param>
        /// <param name="flags">命令标记</param>
        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步取消订阅
        /// </summary>
        /// <typeparam name="T">消息实体类型</typeparam>
        /// <param name="channel">通道</param>
        /// <param name="handler">处理委托</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel">通道</param>
        public void Unsubscribe(string channel);

        /// <summary>
        /// Redis发布订阅  异步取消订阅
        /// </summary>
        /// <param name="channel">通道</param>
        public Task UnsubscribeAsync(string channel);

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        /// <param name="flags">命令标记</param>
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis发布订阅  异步取消全部订阅
        /// </summary>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);


        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresAt"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public bool UpdateExpiry(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步修改过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresAt"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresIn"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public bool UpdateExpiry(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步修改过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresIn"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task<bool> UpdateExpiryAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="expiresAt"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public IDictionary<string, bool> UpdateExpiryAll(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步修改所有过期时间
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="expiresAt"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="expiresIn"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public IDictionary<string, bool> UpdateExpiryAll(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步修改所有过期时间
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="expiresIn"></param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        #endregion
    }
}
