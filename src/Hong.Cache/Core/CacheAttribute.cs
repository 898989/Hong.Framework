using System;

namespace Hong.Cache.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CacheAttribute : Attribute
    {
        /// <summary>初始化<see cref="CacheAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存标识</param>
        public CacheAttribute(string cacheIdentity) : this(cacheIdentity, 0, 0)
        {

        }

        /// <summary>初始化<see cref="CacheAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存标识</param>
        /// <param name="localCacheTime">本地缓存时间</param>
        public CacheAttribute(string cacheIdentity, int localCacheTime) : this(cacheIdentity, localCacheTime, 0)
        {

        }

        /// <summary>初始化<see cref="CacheAttribute"/>注释
        /// </summary>
        /// <param name="cacheIdentity">缓存标识</param>
        /// <param name="localCacheTime">本地缓存时间</param>
        /// <param name="remoteCacheTime">远程缓存时间</param>
        public CacheAttribute(string cacheIdentity, int localCacheTime, int remoteCacheTime)
        {

        }


        /// <summary>缓存标识
        /// </summary>
        public string CacheIdentity = string.Empty;

        /// <summary>本地缓存时间
        /// </summary>
        public int LocalCacheTime = 0;

        /// <summary>远程缓存时间
        /// </summary>
        public int RemoteCacheTime = 0;
    }
}
