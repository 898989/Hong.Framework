using Hong.Cache.Configuration;
using Hong.Cache.Core;
using Microsoft.Extensions.Logging;

namespace Hong.Cache.RuntimeCache
{
    public class MemoryCacheConfiguration : IHandleConfiguration
    {
        /// <summary>缓存配置项标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 最大空间
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>创建缓存处理对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="loggerFactory">日志创建接口</param>
        /// <returns></returns>
        public ICache<TValue> CreateHandle<TValue>(ILoggerFactory loggerFactory = null)
        {
            return new MemoryCache<TValue>(this, loggerFactory);
        }
    }
}
