using Hong.Cache.Core;
using Hong.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Cache.Configuration
{
    public class CacheConfiguration
    {
        const string _CacheManagerSection = "cache";
        const string _LocalCacheSection = "local";
        const string _RemoteCacheSection = "remote";
        const string _CacheHandleSetion = "handle";
        const string _mqSection = "rabbitMQ";

        List<IHandleConfiguration> _localConfig = null;
        List<IHandleConfiguration> _remoteConfig = null;

        public SyncMessageManager MessageManager { get; private set; }

        public CacheConfiguration(IConfiguration config)
        {
            var cacheManagerConfig = config.GetSection(_CacheManagerSection);
            NotNull(cacheManagerConfig, nameof(cacheManagerConfig));

            var mq = cacheManagerConfig.GetSection(_mqSection);
            if (mq != null)
            {
                MessageManager = new SyncMessageManager(mq);
            }

            _localConfig = LoadConfiguration(cacheManagerConfig.GetSection(_LocalCacheSection));
            _remoteConfig = LoadConfiguration(cacheManagerConfig.GetSection(_RemoteCacheSection));
        }

        List<IHandleConfiguration> LoadConfiguration(IConfiguration config)
        {
            var handleConfigs = new List<IHandleConfiguration>();

            if (handleConfigs == null)
            {
                return handleConfigs;
            }

            ConfigurationSection cacheSetting = new ConfigurationSection();
            config.Bind(cacheSetting);

            if (!cacheSetting.Enabled)
            {
                return handleConfigs;
            }

            var handle = config.GetSection(_CacheHandleSetion);
            if (handle == null)
            {
                return null;
            }

            string type = null;

            foreach (var item in handle.GetChildren())
            {
                type = item.GetValue<string>("type");
                var configuration = Activator.CreateInstance(Type.GetType(HandleNameToConfigName(type)));
                config.Bind(configuration);

                handleConfigs.Add((IHandleConfiguration)configuration);
            }

            return handleConfigs;
        }

        string HandleNameToConfigName(string handleName)
        {
            return handleName.Replace("`1", "Configuration");
        }

        ICache<TValue> GetHandles<TValue>(List<IHandleConfiguration> handleConfigs, string localCacheKey = null, ILoggerFactory loggerFactory = null)
        {
            foreach (var item in handleConfigs)
            {
                if (string.IsNullOrEmpty(localCacheKey) || localCacheKey == item.Key)
                {
                    return item.CreateHandle<TValue>(loggerFactory);
                }
            }

            return null;
        }

        /// <summary>
        /// 创建本地处理
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="localCacheKey"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public ICache<TValue> CreateLocalHandle<TValue>(string localCacheKey = null, ILoggerFactory loggerFactory = null)
        {
            return GetHandles<TValue>(_localConfig, localCacheKey);
        }

        /// <summary>
        /// 创建远程处理
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="remoteCacheKey"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public ICache<TValue> CreateRemoteHandle<TValue>(string remoteCacheKey = null, ILoggerFactory loggerFactory = null)
        {
            return GetHandles<TValue>(_remoteConfig, remoteCacheKey);
        }
    }
}
