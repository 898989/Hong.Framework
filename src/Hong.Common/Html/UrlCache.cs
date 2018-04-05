using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Hong.Common.Html
{
    public class UrlCache
    {
        private ConcurrentDictionary<string, UrlInfo> _caches = new ConcurrentDictionary<string, UrlInfo>();
        private const int _maxCacheCount = 100000;

        public UrlInfo Get(string urlKey)
        {
            UrlInfo obj;

            if (_caches.TryGetValue(urlKey, out obj))
            {
                return obj;
            }

            return null;
        }

        public void Remove(string urlKey)
        {
            UrlInfo obj;

            _caches.TryRemove(urlKey, out obj);
        }

        public void Set(string urlKey, UrlInfo value)
        {
            if (!CanAddCache) return;

            _caches.TryAdd(urlKey, value);
        }

        /// <summary>是否可以增加缓存
        /// </summary>
        private bool CanAddCache
        {
            get
            {
                return CachedCount < _maxCacheCount;
            }
        }

        /// <summary>已缓存数量
        /// </summary>
        private int CachedCount { get; set; }

        /// <summary>清理过期缓存
        /// </summary>
        public void ClearOverdue()
        {
            CachedCount = _caches.Count;

            var afterTime = 5;

            //未超限制的情况下以当前时间计算已缓存时间,如果缓存数量超限时计算10分钟后过期的缓存
            var ts = CanAddCache ? new TimeSpan(DateTime.Now.Ticks) : new TimeSpan(DateTime.Now.AddMinutes(afterTime).Ticks);

            ReClear:
            var overdueKeys = _caches.Where(item =>
            {
                var urlInfo = item.Value;
                if (urlInfo.Overdue)
                {
                    return true;
                }

                return urlInfo.Setting.ServerCacheTime < ts.Subtract(urlInfo.CacheCreateTime).Seconds;
            });

            var index = 0;
            foreach (var item in overdueKeys)
            {
                Remove(item.Key);

                if (index++ % 50 == 0)
                {
                    System.Threading.SpinWait.SpinUntil(null, 20);
                }
            }

            CachedCount = _caches.Count;

            //清除后还是超限,继续清理
            if (!CanAddCache)
            {
                //增加清除阀值
                afterTime += 5;
                ts = new TimeSpan(DateTime.Now.AddMinutes(afterTime).Ticks);

                goto ReClear;
            }
        }
    }
}
