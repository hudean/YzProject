using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace YzProject.WebAPI.Globalizations
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// this[string name]这是我们将在控制器中使用的入口方法。
        /// 它接受一个键并尝试使用前面解释的方法从 JSON 文件中找到相应的值。
        /// 需要注意的是，如果在 JSON 文件中找不到值，该方法将返回相同的键。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                string value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                    : actualValue;
            }
        }

        /// <summary>
        /// 根据 CurrentCulture 读取 JSON 文件名，并返回一个 LocalizedString 对象列表。
        /// 此列表将包含找到的 JSON 文件中所有条目的键和值。每个读取的 JSON 值都被反序列化。
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            string filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sReader = new StreamReader(str))
            using (var reader = new JsonTextReader(sReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                        continue;
                    string key = (string)reader.Value;
                    reader.Read();
                    string value = _serializer.Deserialize<string>(reader);
                    yield return new LocalizedString(key, value, false);
                }
            }
        }

        /// <summary>
        /// GetString() 这是负责本地化字符串的函数。在这里，文件路径也是根据请求的当前文化确定的。
        /// 如果该文件存在，则使用一个非常独特的名称创建一个缓存键。理想情况下，系统会尝试检查缓存内存中是否存在对应键的任何值。
        /// 如果在缓存中找到值，则将其返回。否则，应用程序会访问 JSON 文件并尝试获取并返回找到的字符串。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetString(string key)
        {
            string relativeFilePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            string fullFilePath = Path.GetFullPath(relativeFilePath);
            if (File.Exists(fullFilePath))
            {
                string cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                string cacheValue = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;
                string result = GetValueFromJSON(key, Path.GetFullPath(relativeFilePath));
                if (!string.IsNullOrEmpty(result)) _cache.SetString(cacheKey, result);
                return result;
            }
            return default(string);
        }

        /// <summary>
        /// GetValueFromJSON() 顾名思义，此方法接受 JSON 文件的属性名称和文件路径，然后在读取模式下打开该文件。
        /// 如果在 JSON 文件中找到相应的属性，则返回该属性。否则将返回空值。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetValueFromJSON(string propertyName, string filePath)
        {
            if (propertyName == null) return default;
            if (filePath == null) return default;
            using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sReader = new StreamReader(str))
            using (var reader = new JsonTextReader(sReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == propertyName)
                    {
                        reader.Read();
                        return _serializer.Deserialize<string>(reader);
                    }
                }
                return default;
            }
        }

    }
}
