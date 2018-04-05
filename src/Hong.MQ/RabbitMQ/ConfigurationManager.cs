using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Hong.Common.Extendsion.Guard;

namespace Hong.MQ.RabbitMQ
{
    public class ConfigurationManager
    {
        private IConfiguration mqConfig = null;
        const string _publishSection = "publish";
        const string _subscribeSection = "subscribe";
        const string _exchange = "exchange";


        public ConfigurationManager(IConfiguration config)
        {
            NotNull(config, nameof(config));

            mqConfig = config;
        }

        public IPublish CreatePublish(ILoggerFactory loggerFactory = null)
        {
            var config = mqConfig.GetSection(_publishSection);
            if (config == null)
            {
                throw new System.Exception("未设置'publish'配置区");
            }

            var exchange = config.GetValue<string>(_exchange);
            if (!string.IsNullOrEmpty(exchange))
            {
                var exchangeConfig = new PlushExchangeConfiguration();
                config.Bind(exchangeConfig);

                return exchangeConfig.CreateHandle(loggerFactory);
            }
            else
            {
                var queueConfig = new PlushQueueConfiguration();
                config.Bind(queueConfig);

                return queueConfig.CreateHandle(loggerFactory);
            }
        }

        public IPublish CreateSingletonPublish(ILoggerFactory loggerFactory = null)
        {
            var config = mqConfig.GetSection(_publishSection);
            if (config == null)
            {
                throw new System.Exception("未设置'publish'配置区");
            }

            var exchange = config.GetValue<string>(_exchange);
            if (!string.IsNullOrEmpty(exchange))
            {
                var exchangeConfig = new PlushExchangeConfiguration();
                config.Bind(exchangeConfig);

                return exchangeConfig.CreateHandle(loggerFactory);
            }
            else
            {
                var queueConfig = new PlushQueueConfiguration();
                config.Bind(queueConfig);

                return queueConfig.CreateHandle(loggerFactory);
            }
        }


        public ISubscribe CreateSubscribe(ILoggerFactory loggerFactory = null)
        {
            var config = mqConfig.GetSection(_subscribeSection);
            if (config == null)
            {
                throw new System.Exception("没有找到'subscribe'配置区");
            }

            var subscribeConfig = new SubscribeConfiguration();
            config.Bind(subscribeConfig);

            return subscribeConfig.CreateHandle(loggerFactory);
        }

        public Manager CreateManager(ILoggerFactory loggerFactory = null)
        {
            return null;
        }
    }
}
