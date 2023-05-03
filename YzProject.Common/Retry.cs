using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.Common
{
    /// <summary>
    /// 重试类
    /// </summary>
    public class Retry : IDisposable
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static Retry Instance()
        {
            return new Retry();
        }

        public TResult Execute<TResult>(Func<TResult> action, int secondsInterval, int retryCount, TResult expectedResult, bool isSuppressException = true)
        {
            TResult result = default(TResult);
            var exceptions = new List<Exception>();
            for (var retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Thread.Sleep(secondsInterval * 1000);
                    }
                    result = action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                if (result.Equals(expectedResult))
                {
                    return result;
                }
            }

            if (!isSuppressException)
            {
                throw new AggregateException(exceptions);
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 重试函数
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action">委托函数</param>
        /// <param name="interval">间隔时间(毫秒)</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="expectedResult">期望结果</param>
        /// <param name="isSuppressException"></param>
        /// <returns></returns>
        public TResult ExecuteFromMilliseconds<TResult>(Func<TResult> action, int interval, int retryCount, TResult expectedResult, bool isSuppressException = true)
        {
            TResult result = default(TResult);
            var exceptions = new List<Exception>();
            for (var retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Thread.Sleep(interval);
                    }
                    result = action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                if (result.Equals(expectedResult))
                {
                    return result;
                }
            }

            if (!isSuppressException)
            {
                throw new AggregateException(exceptions);
            }
            else
            {
                return result;
            }
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
