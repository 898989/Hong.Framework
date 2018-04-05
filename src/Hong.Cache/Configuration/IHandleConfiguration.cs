using Hong.Cache.Core;
using Microsoft.Extensions.Logging;

namespace Hong.Cache.Configuration
{
    public interface IHandleConfiguration
    {
        string Key { get; set; }

        /// <summary>创建缓存操作对象
        /// </summary>
        /// <returns></returns>
        ICache<TValue> CreateHandle<TValue>(ILoggerFactory loggerFactory = null);
    }
}
