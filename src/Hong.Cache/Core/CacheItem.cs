using System;

namespace Hong.Cache.Core
{
    /// <summary>缓存项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheItem<TValue>
    {
        /// <summary>构建CacheItem
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">键值</param>
        /// <param name="version">键值版本</param>
        /// <param name="expirationMode">缓存过期方式</param>
        public CacheItem(string key, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0) :
            this(key, null, value, version, expirationMode, cacheTimeSpan)
        {
        }

        /// <summary>构建CacheItem
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">键区域</param>
        /// <param name="value">键值</param>
        /// <param name="version">键值版本</param>
        /// <param name="expirationMode">缓存过期方式</param>
        public CacheItem(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0)
        {
            Key = key;
            Region = region;
            CreatedUtc = DateTime.Now;

            Reset(value, version, expirationMode, cacheTimeSpan);
        }

        public void Reset(TValue value, long version, ExpirationMode expirationMode, int cacheTimeSpan)
        {
            Value = value;
            Version = version;

            ReSetExpiration(expirationMode, cacheTimeSpan);
        }

        public void ReSetExpiration(ExpirationMode expirationMode, int cacheTimeSpan)
        {
            ExpirationMode = expirationMode;
            ExpirationTimeout = new TimeSpan(cacheTimeSpan * 1000);

            if (expirationMode == ExpirationMode.Absolute)
            {
                AbsoluteExpirationTime = DateTime.Now.AddSeconds(cacheTimeSpan);
            }
            else
            {
                AbsoluteExpirationTime = DateTime.Now;
            }
        }

        /// <summary>创建时间
        /// </summary>
        public DateTime CreatedUtc = DateTime.Now;

        /// <summary>过期方式
        /// </summary>
        public ExpirationMode ExpirationMode = ExpirationMode.None;

        /// <summary>过期时间, 单位秒
        /// </summary>
        public TimeSpan _ExpirationTimeout = new TimeSpan(0);
        /// <summary>过期时间,单位秒
        /// </summary>
        public TimeSpan ExpirationTimeout
        {
            get
            {
                return _ExpirationTimeout;
            }
            set
            {
                _ExpirationTimeout = value;
                AbsoluteExpirationTime = CreatedUtc.AddSeconds(value.Seconds);
            }
        }

        /// <summary>绝对过期时间
        /// </summary>
        private DateTime AbsoluteExpirationTime = new DateTime(0);
        /// <summary>滑动过期时间
        /// </summary>
        private DateTime SlidingExpirationTime
        {
            get
            {
                return LastAccessedUtc.AddSeconds(this.ExpirationTimeout.Seconds);
            }
        }

        /// <summary>是否过期
        /// </summary>
        public bool Expired
        {
            get
            {
                if (ExpirationMode == ExpirationMode.Absolute)
                {
                    if (AbsoluteExpirationTime < DateTime.Now)
                    {
                        return true;
                    }
                }
                else if (ExpirationMode == ExpirationMode.Sliding)
                {
                    if (SlidingExpirationTime < DateTime.Now)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>缓存键
        /// </summary>
        public string Key { get; }

        /// <summary>缓存值
        /// </summary>
        public TValue Value;

        /// <summary>最后访问时间
        /// </summary>
        public DateTime LastAccessedUtc = DateTime.Now;

        /// <summary>区域
        /// </summary>
        public string Region { get; }

        /// <summary>版本号
        /// </summary>
        public long Version = 0;
    }
}
