using Hong.Cache.Configuration;
using Hong.Cache.Core;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Hong.Cache.Redis
{
    public class RedisCacheConfiguration : IHandleConfiguration
    {
        /// <summary>缓存配置项标识
        /// </summary>
        public string Key { get; set; }

        string _ConnectionString = string.Empty;
        /// <summary>连接串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                var ips = System.Net.Dns.GetHostAddressesAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
                _ConnectionString = ips.First().ToString();

            }
        }
        /// <summary>用户
        /// </summary>
        public string User { get; set; }

        /// <summary>密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>连接超时等待时间
        /// </summary>
        public int ConnectionTimeout { get; set; } = 1000;

        /// <summary>重试次数
        /// </summary>
        public int TryCount { get; set; } = 1;

        /// <summary>开启管理
        /// </summary>
        public bool AllowAdmin { get; set; } = true;

        /// <summary>是否启用SSL
        /// </summary>
        public bool IsSsl { get; set; }

        /// <summary>SSL主机
        /// </summary>
        public string SslHost { get; set; }

        /// <summary>连接重试次数
        /// </summary>
        public int ConnectRetry { get; set; } = 1;

        /// <summary>主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>端口
        /// </summary>
        public int Port { get; set; } = 6379;

        /// <summary>数据库
        /// </summary>
        public int DataBase { get; set; } = 0;

        /// <summary>创建redis缓存操作对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="loggerFactory">日志创建接口</param>
        /// <returns></returns>
        public ICache<TValue> CreateHandle<TValue>(ILoggerFactory loggerFactory = null)
        {
            return new RedisCache<TValue>(this, loggerFactory);
        }
    }

}
