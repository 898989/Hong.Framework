using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hong.Cache
{
    /// <summary>
    /// 查询缓存管理接口,通常适用于缓存之间存在关联,批量删除缓存
    /// 缓存由某一缓存配合,同时也必须划分区域,根据版本号一次性快速整个区域的缓存
    /// 其不具体删除缓存,由后台任务去删除
    /// </summary>
    /// <typeparam name="TValue">缓存数据类型</typeparam>
    public interface IQueryCacheManager<TValue>
    {
        /// <summary>
        /// 已删除的版本
        /// </summary>
        /// <param name="region">缓存域</param>
        /// <returns></returns>
        IEnumerable<string> RemovedVersions();

        /// <summary>
        /// 获取版本的所有Key
        /// </summary>
        /// <param name="version">缓存域</param>
        /// <returns></returns>
        IEnumerable<string> RemovedVersionKeys(int version);

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="keys">缓存键</param>
        /// <returns></returns>
        IEnumerable<string> RemovedKeys(string[] keys);

        /// <summary>取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns>缓存键'<paramref name="key"/> 或 <paramref name="region"/>'存在返回<c>CacheItem</c>,否则返回null</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        TValue TryGet(string key, string[] table = null);

        /// <summary>移除缓存,将旧的版本号加入清理站
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <returns>成功返回 true,失败返回 false</returns>
        /// <exception cref="ArgumentNullException">如果'<paramref name="key"/>或<paramref name="region"/>'是空或null</exception>
        bool TryRemove(string key, string[] table = null);

        /// <summary>设置缓存 --存在则更新,不存在添加, 返回成功:1, 0失败, 缓存同步过期:-1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="region">缓存域</param>
        /// <param name="value">缓存值</param>
        /// <param name="version">新值的版本</param>
        /// <param name="expirationMode">过期方式</param>
        /// <param name="cacheTimeSpan">缓存时长,单位秒, 0无限期</param>
        /// <returns>如果键'<paramref name="key"/> 或 <paramref name="region"/>' 添加或更新成功:1, 0失败, 缓存同步过期:-1</returns>
        short TrySet(string key, string[] table, TValue value);
    }
}
