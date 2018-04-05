using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hong.Cache.Core
{
    public interface ICache<TValue>
    {
        /// <summary>重试次数
        /// </summary>
        int TryCount { get; set; }

        /// <summary>
        /// 缓存域里面的所有key
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns></returns>
        IEnumerable<string> RegionHasKeys(string region);

        /// <summary>清除缓存
        /// </summary>
        /// <returns>成功返回 true,失败返回 false</returns>
        Task<bool> TryClear();
        /// <summary>清除缓存
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="region"/>是空或null</exception>
        Task<bool> TryClear(string region);

        /// <summary>取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存键'<paramref name="key"/>'存在返回<c>CacheItem</c>,否则返回null</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        TValue TryGet(string key);
        /// <summary>取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns>缓存键'<paramref name="key"/> 或 <paramref name="region"/>'存在返回<c>CacheItem</c>,否则返回null</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        TValue TryGet(string key, string region);

        /// <summary>移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        bool TryRemove(string key);
        /// <summary>移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        bool TryRemove(string key, string region);

        /// <summary>设置缓存 --存在则更新,不存在添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>如果键'<paramref name="key"/>' 添加或更新成功:1, 0失败, 缓存同步过期:-1</returns>
        short TrySet(string key, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0);
        /// <summary>设置缓存 --存在则更新,不存在添加
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时长,单位秒, 0无限期</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>' 添加或更新成功:1, 0失败, 缓存同步过期:-1</returns>
        short TrySet(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0);

        /// <summary>新增缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <returns>如果键'<paramref name="key"/>'已存在返回 false,不存在返回true</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        bool TryAdd(string key, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0);
        /// <summary>新增缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <param name="expirationMode">过期方式, 默认不过期</param>
        /// <param name="cacheTimeSpan">缓存时长,单位秒, 0无限期</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>'已存在返回 false,不存在返回true</returns>
        bool TryAdd(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0);

        /// <summary>重置设置过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时间</param>
        /// <returns>缓存键'<paramref name="key"/>' 设置成功:true, 失败:false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        bool TryExpire(string key, ExpirationMode expirationMode, int timeSpan);
        /// <summary>重置设置过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时间,单位秒, 0无限期</param>
        /// <returns>缓存键'<paramref name="key"/> 或 <paramref name="region"/>'设置成功:true, 失败:false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        bool TryExpire(string key, string region, ExpirationMode expirationMode, int timeSpan);

        /// <summary>更新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <returns>如果键'<paramref name="key"/>' 更新成功:1, 不存在:0, 缓存同步过期:-1</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>是空或null</exception>
        short TryUpdate(string key, TValue value, long version);
        /// <summary>更新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>' 更新成功:1, 不存在:0, 缓存同步过期:-1</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        short TryUpdate(string key, string region, TValue value, long version);
    }
}