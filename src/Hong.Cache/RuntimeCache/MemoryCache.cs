using System;
using System.Collections.Concurrent;
using System.Linq;
using Hong.Cache.Core;
using static Hong.Common.Extendsion.Guard;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Hong.Cache.RuntimeCache
{
    public class MemoryCache<TValue> : Cache<TValue>
    {
        ConcurrentDictionary<string, CacheItem<TValue>> _datas = new ConcurrentDictionary<string, CacheItem<TValue>>();

        /// <summary>
        /// 最后一次释放过期扫描时间
        /// </summary>
        DateTime _lastScanTime = DateTime.Now;

        /// <summary>
        /// 并发控制描版本
        /// </summary>
        int _scanVersion = 0;

        /// <summary>
        /// 是否已释放
        /// </summary>
        bool _disposed = false;

        /// <summary>
        /// 是否可写
        /// </summary>
        bool _canWrite = true;

        /// <summary>
        /// 扫描任务是否在进行中
        /// </summary>
        bool _isRuningScan = false;

        /// <summary>
        /// 日志
        /// </summary>
        readonly ILogger log = null;

        internal MemoryCache(MemoryCacheConfiguration configuration, ILoggerFactory loggerFactory = null)
        {
            log = loggerFactory?.CreateLogger("MemoryCache");
            MaxDataSize = configuration.MaxSize;
        }

        /// <summary>
        /// 最大数据量
        /// </summary>
        int MaxDataSize { get; set; } = 10000;

        public override IEnumerable<string> RegionHasKeys(string region)
        {
            var key = region + RegionKeyDecollator;

            foreach (var item in _datas.Where(p => p.Key.StartsWith(key, StringComparison.Ordinal)))
            {
                yield return item.Key;
            }
        }

        public override async Task<bool> TryClear() =>
            await new Task<bool>(() =>
            {
                _datas.Clear();
                return true;
            });

        public override async Task<bool> TryClear(string region) =>
            await new Task<bool>(() =>
            {
                foreach (var key in RegionHasKeys(region))
                {
                    _datas.TryRemove(key, out CacheItem<TValue> cacheItem);
                }

                return true;
            });

        public override TValue TryGet(string key, string region)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (_datas.TryGetValue(key, out CacheItem<TValue> cacheItem))
            {
                if (cacheItem.Expired)
                {
                    TryRemove(key, region);
                    return default(TValue);
                }

                cacheItem.LastAccessedUtc = DateTime.Now;

                StartScanForExpiredItems();

                return cacheItem.Value;
            }

            return default(TValue);
        }

        public override bool TryRemove(string key, string region)
        {
            NotNullOrEmpty(key, nameof(key));

            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            return _datas.TryRemove(key, out CacheItem<TValue> cacheItem);
        }

        public override short TrySet(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            var cacheItem = Get(key);
            if (cacheItem == null)
            {
                if (!_canWrite)
                {
                    return 0;
                }

                cacheItem = new CacheItem<TValue>(key, region, value, version, expirationMode, cacheTimeSpan);
                try
                {
                    if (_datas.TryAdd(key, cacheItem))
                    {
                        return 1;
                    }
                }
                catch (OverflowException ex)
                {
                    log?.LogError("#Method =>TrySet =>TryAdd", ex);
                    return 0;
                }
                finally
                {
                    StartScanForExpiredItems();
                }
            }
            else if (Monitor.TryEnter(cacheItem, 10))
            {
                if (version <= cacheItem.Version)
                {
                    Monitor.Exit(cacheItem);
                    return -1;
                }

                cacheItem.Reset(value, version, expirationMode, cacheTimeSpan);
                Monitor.Exit(cacheItem);

                StartScanForExpiredItems();

                return 1;
            }
            else
            {
                log?.LogError("#Method =>TrySet =>Monitor.TryEnter", "等待超时");
            }

            return 0;
        }

        public override bool TryAdd(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (!_canWrite)
            {
                return false;
            }

            var item = new CacheItem<TValue>(key, region, value, version, expirationMode, cacheTimeSpan);

            try
            {
                return _datas.TryAdd(key, item);
            }
            catch (OverflowException ex)
            {
                log?.LogError("#Method =>TryAdd =>TryAdd", ex);
                return false;
            }
            finally
            {
                StartScanForExpiredItems();
            }
        }

        public override bool TryExpire(string key, string region, ExpirationMode expirationMode, int cacheTimeSpan)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            var cacheItem = Get(key);
            if (cacheItem == null) return false;

            cacheItem.ReSetExpiration(expirationMode, cacheTimeSpan);

            StartScanForExpiredItems();

            return true;
        }

        public override short TryUpdate(string key, string region, TValue value, long version)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            var cacheItem = Get(key);
            if (cacheItem == null) return 0;

            if (Monitor.TryEnter(cacheItem, 10))
            {
                if (version <= cacheItem.Version)
                {
                    Monitor.Exit(cacheItem);
                    return -1;
                }

                cacheItem.Value = value;
                cacheItem.LastAccessedUtc = DateTime.Now;
                cacheItem.Version = version;
                Monitor.Exit(cacheItem);

                StartScanForExpiredItems();

                return 1;
            }
            else
            {
                log?.LogError("#Method =>TryUpdate =>Monitor.TryEnter", "等待超时");
            }

            StartScanForExpiredItems();
            return 0;
        }

        CacheItem<TValue> Get(string key)
        {
            if (_datas.TryGetValue(key, out CacheItem<TValue> cacheItem))
            {
                return cacheItem;
            }

            return null;
        }

        /// <summary>清理已过期项目
        /// </summary>
        void ClearExpiredItem()
        {
            var removed = 0;

            foreach (var item in _datas.Values)
            {
                if (item.Expired)
                {
                    _datas.TryRemove(item.Key, out CacheItem<TValue> obj);
                    removed++;
                }
            }
        }

        private void StartScanForExpiredItems()
        {
            var v = _scanVersion;
            if (!_isRuningScan &&
                5 < (DateTime.Now - _lastScanTime).TotalMinutes &&
                Interlocked.CompareExchange(ref _scanVersion, v + 1, v) == v)
            {
                _isRuningScan = true;
                _canWrite = _datas.Count < MaxDataSize;
                _lastScanTime = DateTime.Now;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ClearExpiredItem();
                    }
                    catch
                    {
                        log.LogWarning("清理本地缓存时失败");
                    }
                    finally
                    {
                        _isRuningScan = false;
                    }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        ~MemoryCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }

                _disposed = true;
            }
        }
    }
}
