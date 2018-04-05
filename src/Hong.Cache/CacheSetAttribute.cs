using System;

namespace Hong.Cache
{
    public class CacheSetAttribute : Attribute
    {
        /// <summary>初始化<see cref="CacheSetAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存Key前缀</param>
        public CacheSetAttribute(string cacheIdentity)
            : this(cacheIdentity, null, ExpirationMode.None, 0, 0)
        {

        }

        /// <summary>初始化<see cref="CacheSetAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存Key前缀</param>
        /// <param name="defaultRegion">默认缓存区域</param>
        public CacheSetAttribute(string cacheIdentity, string defaultRegion)
            : this(cacheIdentity, defaultRegion, ExpirationMode.None, 0, 0)
        {

        }

        /// <summary>初始化<see cref="CacheSetAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存Key前缀</param>
        /// <param name="defaultRegion">默认缓存区域</param>
        /// <param name="expirationMode">过期模式</param>
        /// <param name="localCacheTime">本地缓存时间</param>
        public CacheSetAttribute(string cacheIdentity, string defaultRegion, ExpirationMode expirationMode, int localCacheTime)
            : this(cacheIdentity, defaultRegion, expirationMode, localCacheTime, 0)
        {

        }

        /// <summary>初始化<see cref="CacheSetAttribute"/>注释
        /// </summary>
        /// <param name="expirationMode">过期模式</param>
        /// <param name="localCacheTime">本地缓存时间</param>
        /// <param name="remoteCacheTime">远程缓存时间</param>
        public CacheSetAttribute(ExpirationMode expirationMode, int localCacheTime, int remoteCacheTime)
            : this(null, null, expirationMode, localCacheTime, remoteCacheTime)
        {
        }

        /// <summary>初始化<see cref="CacheSetAttribute"/>注释
        /// </summary>
        /// <param name="expirationMode">过期模式</param>
        /// <param name="localCacheTime">本地缓存时间</param>
        /// <param name="remoteCacheTime">远程缓存时间</param>
        public CacheSetAttribute(ExpirationMode expirationMode, int localCacheTime)
            : this(null, null, expirationMode, localCacheTime, 0)
        {
        }

        /// <summary>初始化<see cref="CacheSetAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存Key前缀</param>
        /// <param name="defaultRegion">默认缓存区域</param>
        /// <param name="expirationMode">过期模式</param>
        /// <param name="localCacheTime">本地缓存时间</param>
        /// <param name="remoteCacheTime">远程缓存时间</param>
        public CacheSetAttribute(string cacheIdentity, string defaultRegion, ExpirationMode expirationMode, int localCacheTime, int remoteCacheTime)
        {
            CacheIdentity = cacheIdentity;
            ExpirationMode = expirationMode;
            LocalCacheTime = localCacheTime;
            RemoteCacheTime = remoteCacheTime;
            DefaultRegion = defaultRegion;
        }


        /// <summary>缓存标识
        /// </summary>
        public string CacheIdentity = string.Empty;

        /// <summary>过期模式
        /// </summary>
        public ExpirationMode ExpirationMode = ExpirationMode.None;

        /// <summary>本地缓存时间
        /// 分步式情况下,如果使用了远程缓存(二级缓存),会存在以下问题
        /// 当远程一直运行,本地服务刚启动这种情况,会造成本地过期和远程过期存在不至一情况的影响
        /// </summary>
        public int LocalCacheTime = 0;

        /// <summary>远程缓存时间,必须大于等于本地缓存时间
        /// </summary>
        public int RemoteCacheTime = 0;

        /// <summary>默认缓存区域
        /// </summary>
        public string DefaultRegion;
    }
}
