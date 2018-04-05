using System.Collections.Generic;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Cache.Core
{
    /// <summary>缓存基类
    /// </summary>
    public abstract class Cache<TValue> : ICache<TValue>
    {
        /// <summary>重试次数
        /// </summary>
        public int TryCount { get; set; }

        /// <summary>
        /// 缓存域里面的所有key
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns></returns>
        public abstract IEnumerable<string> RegionHasKeys(string region);

        /// <summary>取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存键'<paramref name="key"/>'存在返回<c>CacheItem</c>,否则返回null</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        public TValue TryGet(string key) => TryGet(key, null);

        /// <summary>取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns>缓存键'<paramref name="key"/> 或 <paramref name="region"/>'存在返回<c>CacheItem</c>,否则返回null</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        public abstract TValue TryGet(string key, string region);

        /// <summary>新增缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <returns>如果键'<paramref name="key"/>'已存在返回 false,不存在返回true</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        public bool TryAdd(string key, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0) =>
            TryAdd(key, null, value, version, expirationMode, cacheTimeSpan);

        /// <summary>新增缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <param name="expirationMode">过期方式, 默认不过期</param>
        /// <param name="cacheTimeSpan">缓存时长,单位秒, 0无限期</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>'已存在返回 false,不存在返回true</returns>
        public abstract bool TryAdd(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0);

        /// <summary>更新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <returns>如果键'<paramref name="key"/>' 更新成功:1, 不存在:0, 缓存同步过期:-1</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        public short TryUpdate(string key, TValue value, long version) => TryUpdate(key, null, value, version);

        /// <summary>更新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>' 更新成功:1, 不存在:0, 缓存同步过期:-1</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        public abstract short TryUpdate(string key, string region, TValue value, long version);

        /// <summary>设置缓存 --存在则更新,不存在添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>如果键'<paramref name="key"/>' 添加或更新成功:1, 0失败, 缓存同步过期:-1</returns>
        public short TrySet(string key, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0) =>
            TrySet(key, null, value, version, expirationMode, cacheTimeSpan);

        /// <summary>设置缓存 --存在则更新,不存在添加
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时长,单位秒, 0无限期</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>' 添加或更新成功:1, 0失败, 缓存同步过期:-1</returns>
        public abstract short TrySet(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0);

        /// <summary>移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        public bool TryRemove(string key) => TryRemove(key, null);

        /// <summary>移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        public abstract bool TryRemove(string key, string region);

        /// <summary>重置设置过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时间</param>
        /// <returns>缓存键'<paramref name="key"/>' 设置成功:true, 失败:false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        public bool TryExpire(string key, ExpirationMode expirationMode, int cacheTimeSpan) =>
            TryExpire(key, null, expirationMode, cacheTimeSpan);

        /// <summary>重置设置过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时间,单位秒, 0无限期</param>
        /// <returns>缓存键'<paramref name="key"/> 或 <paramref name="region"/>'设置成功:true, 失败:false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        public abstract bool TryExpire(string key, string region, ExpirationMode expirationMode, int cacheTimeSpan);

        /// <summary>清除缓存
        /// </summary>
        /// <returns>成功返回 true,失败返回 false</returns>
        public abstract Task<bool> TryClear();

        /// <summary>清除缓存
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="region"/>是空或null</exception>
        public abstract Task<bool> TryClear(string region);

        /// <summary>Region和Key的分割符
        /// </summary>
        protected const string RegionKeyDecollator = "/";

        /// <summary>生成缓存KEY
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns></returns>
        protected string BuildKey(string key, string region)
        {
            NotNull(region, nameof(region));
            NotNull(key, nameof(key));

            return string.Concat(region, RegionKeyDecollator, key);
        }
    }
}
