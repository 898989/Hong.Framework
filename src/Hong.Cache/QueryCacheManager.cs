using Hong.Cache.Configuration;
using Hong.Cache.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Hong.Common.Extendsion;
using Hong.Common.Tools;
using static Hong.Common.Extendsion.Guard;
using Microsoft.Extensions.Logging;

namespace Hong.Cache
{
    /*
     * 每次获取返回的对象版本加1
     * 添加或更新时缓存时使用当前版本, 操作成功后当前对象的版本加1
     */
    public class QueryCacheManager<TValue> : Manager<TValue>, IQueryCacheManager<TValue>, ISyncEvent
    {
        internal QueryCacheManager(CacheConfiguration configuration, string localCacheConfigKey = null, string remoteCacheConfigKey = null, ILoggerFactory loggerFactory = null)
        {
            LocalCache = configuration.CreateLocalHandle<byte[]>(localCacheConfigKey);
            RemoteCaches = configuration.CreateRemoteHandle<byte[]>(remoteCacheConfigKey);

            MessageManager = configuration.MessageManager;
            MessageManager?.RegisteEvents<TValue>(this);

            var type = typeof(TValue);
            CacheSet = type.GetTypeInfo().GetCustomAttribute<CacheSetAttribute>();
            if (CacheSet == null)
            {
                throw new Exception(type.FullName + "必须添加CacheSet属性设置");
            }

            //效验设置
            if (CacheSet.LocalCacheTime <= CacheSet.RemoteCacheTime)
            {
                throw new Exception(type.FullName + "远程缓存时间必须大于等于本地时间");
            }

            var fInfo = type.GetTypeInfo().GetField("Version");
            if (fInfo != null)
            {
                GetVersion = Reflection.GetField<TValue, int>(fInfo);
            }
            else
            {
                GetVersion = Reflection.GetProperty<TValue, int>("Version");
            }

            Log = loggerFactory?.CreateLogger("CacheManager");
        }

        Func<TValue, int> GetVersion;

        CacheManager<string> TableVersionManager;

        /// <summary>
        /// 已删除的版本
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns></returns>
        public IEnumerable<string> RemovedVersions()
        {
            return null;
        }

        /// <summary>
        /// 获取版本的所有Key
        /// </summary>
        /// <param name="version">缓存域</param>
        /// <returns></returns>
        public IEnumerable<string> RemovedVersionKeys(int version)
        {
            return null;
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="keys">缓存键</param>
        /// <returns></returns>
        public IEnumerable<string> RemovedKeys(string[] keys)
        {
            return null;
        }

        string BuildKey(string key, string[] tables = null)
        {
            NotNullOrEmpty(key, nameof(key));

            if (tables != null)
            {
                foreach (var t in tables)
                {
                    var k = TableVersionManager.TryGet(t);
                    if (string.IsNullOrEmpty(k))
                    {
                        k = "0";
                    }

                    key += "/" + k;
                }
            }

            return key;
        }

        public TValue TryGet(string key, string[] tables = null)
        {
            key = BuildKey(key, tables);

            TValue obj = default(TValue);
            byte[] bytes = null;
            var sessionCache = SessionCache;

            #region Session缓存

            if (sessionCache != null)
            {
                obj = sessionCache.Get<TValue>(key);

                if (obj != null)
                {
                    return obj;
                }
            }

            #endregion

            #region 本地缓存

            if (LocalCache != null)
            {
                bytes = LocalCache.TryGet(key);
                if (bytes != null)
                {
                    obj = bytes.Deserialize<TValue>();
                    if (obj == null)
                    {
                        Log?.LogError("#Method =>TryGet", "反序化失败, 类型:" + typeof(TValue).FullName);
                        throw new Exception("反序化失败");
                    }

                    sessionCache?.Set(key, obj);

                    return obj;
                }
            }

            #endregion

            #region 远程缓存

            else if (RemoteCaches != null)
            {
                bytes = Try(() => RemoteCaches.TryGet(key), RemoteCaches.TryCount);
                if (bytes != null)
                {
                    obj = bytes.Deserialize<TValue>();

                    if (obj == null)
                    {
                        Log?.LogError("#Method =>TryGet", "反序化失败, 类型:" + typeof(TValue).FullName);
                        throw new Exception("反序化失败");
                    }

                    sessionCache?.Set(key, obj);
                }
            }

            #endregion

            if (obj == null)
            {
                return default(TValue);
            }

            return obj;
        }

        public short TrySet(string key, string[] tables, TValue value)
        {
            key = BuildKey(key, tables);

            var randomTime = MyRandom.Next(MaxRandom);
            var version = GetVersion(value);
            short result = 0;
            var serializedValue = value.Serialize();
            bool localSeted = false;

            if (LocalCache != null)
            {
                result = LocalCache.TrySet(key, serializedValue, version, CacheSet.ExpirationMode, ComputeCacheTime(CacheSet.LocalCacheTime, randomTime));
                if (result != 1)
                {
                    return result;
                }

                localSeted = true;
            }

            if (RemoteCaches != null)
            {
                result = Try(() => RemoteCaches.TrySet(key, serializedValue, version, CacheSet.ExpirationMode, ComputeCacheTime(CacheSet.RemoteCacheTime, randomTime)), RemoteCaches.TryCount);
            }

            if (localSeted && result == 1)
            {
                MessageManager?.NotifyUpdate<TValue>(key, null);
            }

            return result;
        }

        public bool TryRemove(string key, string[] tables = null)
        {
            bool localRemoved = false;
            key = BuildKey(key, tables);

            SessionCache?.Remove(key);

            if (LocalCache != null)
            {
                if (!LocalCache.TryRemove(key))
                {
                    return false;
                }

                localRemoved = true;
            }

            if (RemoteCaches == null)
            {
                return true;
            }

            if (!Try(() => RemoteCaches.TryRemove(key), RemoteCaches.TryCount))
            {
                return false;
            }

            if (localRemoved)
            {
                MessageManager?.NotifyRemove<TValue>(key, null);
            }

            return true;
        }
    }
}
