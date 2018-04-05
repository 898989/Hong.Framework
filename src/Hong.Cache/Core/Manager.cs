using Hong.Common.Extendsion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Cache.Core
{
    public class Manager<TValue> : ISyncEvent
    {
        /// <summary>
        /// 日志
        /// </summary>
        protected ILogger Log;

        /// <summary>订阅消息
        /// </summary>
        protected SyncMessageManager MessageManager;

        /// <summary>本地缓存
        /// </summary>
        public ICache<byte[]> LocalCache;

        /// <summary>远程缓存
        /// </summary>
        public ICache<byte[]> RemoteCaches;

        /// <summary>缓存设置
        /// </summary>
        public CacheSetAttribute CacheSet;

        /// <summary>Session缓存
        /// </summary>
        public SessionCache SessionCache
        {
            get
            {
                return ServiceProvider.GetRequestServices<SessionCache>();
            }
        }

        /// <summary>远程缓存失败重试间隔时间(单位毫秒)
        /// </summary>
        public virtual int RemoteTryWait{ get; set; } = 10;

        protected const int MaxRandom = 50;
        protected Random MyRandom = new Random();
        /// <summary>
        /// 计算实际缓存时间,防并发雪蹦
        /// </summary>
        /// <param name="cacheTime">缓存</param>
        /// <param name="randomTime">随机数</param>
        /// <returns></returns>
        protected int ComputeCacheTime(int cacheTime, int randomTime)
        {
            if (cacheTime == 0)
            {
                return 0;
            }

            return cacheTime + randomTime;
        }

        /// <summary>
        /// 缓存域里面的所有KEY,为保让缓存域的一直至性,优先取远程缓存的缓存域
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns></returns>
        public IEnumerable<string> RegionHasKeys(string region)
        {
            if (RemoteCaches != null)
            {
                return RemoteCaches.RegionHasKeys(region);
            }

            if (LocalCache != null)
            {
                return LocalCache.RegionHasKeys(region);
            }

            return new List<string>();
        }

        protected virtual bool TryUpdateLocalCache(SyncMessage msg)
        {
            SessionCache?.Set(msg.Key, msg.Region);

            if (LocalCache != null && !LocalCache.TryRemove(msg.Key, msg.Region))
            {
                return false;
            }

            return true;
        }

        protected virtual bool TryRemoveLocalCache(SyncMessage msg)
        {
            SessionCache?.Remove(msg.Key, msg.Region);

            if (LocalCache != null && !LocalCache.TryRemove(msg.Key, msg.Region))
            {
                return false;
            }

            return true;
        }

        protected virtual async Task<bool> TryClearLocalAsync(SyncMessage msg)
        {
            if (string.IsNullOrEmpty(msg.Region))
            {
                if (LocalCache != null && !await LocalCache.TryClear())
                {
                    return false;
                }
            }
            else if (LocalCache != null && !await LocalCache.TryClear(msg.Region))
            {
                return false;
            }

            return true;
        }

        #region 重试执行

        /// <summary>重试操作
        /// </summary>
        /// <param name="func">操作</param>
        /// <param name="tryCount">重试次数</param>
        /// <returns>true 成功, false 失败</returns>
        protected bool Try(Func<bool> func, int tryCount)
        {
            if (func())
            {
                return true;
            }

            while (tryCount-- > 0)
            {
                Task.Delay(RemoteTryWait).Wait();

                if (func())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>重试操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">操作</param>
        /// <param name="tryCount">重试次数</param>
        /// <returns>返回操作结果</returns>
        protected T Try<T>(Func<T> func, int tryCount)
        {
            var result = func();
            if (result != null)
            {
                return result;
            }

            while (tryCount-- > 0)
            {
                Task.Delay(RemoteTryWait).Wait();

                result = func();
                if (result != null)
                {
                    return result;
                }
            }

            Log?.LogWarning("#Method =>Try", "已超超过重试次数");
            return default(T);
        }

        /// <summary>重试操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">操作</param>
        /// <param name="tryCount">重试次数</param>
        /// <returns>返回操作结果</returns>
        protected async Task<T> Try<T>(Func<Task<T>> func, int tryCount)
        {
            var result = await func();
            if (result != null)
            {
                return result;
            }

            while (tryCount-- > 0)
            {
                Task.Delay(RemoteTryWait).Wait();

                result = await func();
                if (result != null)
                {
                    return result;
                }
            }

            Log?.LogWarning("#Method =>Try", "已超超过重试次数");
            return default(T);
        }

        #endregion

        #region 订阅消息
        void ISyncEvent.OnUpdate(object sender, SyncMessage msg)
        {
#if DEBUG
            Log?.LogInformation("#Method =>OnUpdate", "Key:{" + msg.Key + "}, Region:{" + msg.Region + "}");
#endif
            TryUpdateLocalCache(msg);
        }

        void ISyncEvent.OnRemove(object sender, SyncMessage msg)
        {
#if DEBUG
            Log?.LogInformation("#Method =>OnRemove", "Key:{" + msg.Key + "}, Region:{" + msg.Region + "}");
#endif
            TryRemoveLocalCache(msg);
        }

        async void ISyncEvent.OnClear(object sender, SyncMessage msg)
        {
#if DEBUG
            Log?.LogInformation("#Method =>OnClearRegion", "Region:{" + msg.Region + "}");
#endif
            await TryClearLocalAsync(msg);
        }
        #endregion
    }
}
