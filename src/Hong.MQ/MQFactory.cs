using Hong.MQ.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Hong.MQ
{
    public class MQFactory
    {
        ConfigurationManager configurationManager = null;

        public MQFactory(IConfiguration config = null)
        {
            if (config == null)
            {
                if (!File.Exists("web.json"))
                {
                    throw new FileNotFoundException("没有找到配置文件web.json");
                }

                var c = new ConfigurationBuilder()
                .AddJsonFile("web.json")
                .Build();

                const string _rabbitMQSection = "rabbitMQ";

                config = c.GetSection(_rabbitMQSection);
                if (config == null)
                {
                    throw new System.Exception("请在配置文件添加'rabbitMQ'区域配置");
                }
            }

            configurationManager = new ConfigurationManager(config);
        }

        /// <summary>
        /// 创建发布者,每次发送使用不同的通道
        /// </summary>
        /// <param name="loggerFactory">日志</param>
        /// <returns></returns>
        public IPublish CreatePublish(ILoggerFactory loggerFactory = null)
        {
            return configurationManager.CreatePublish(loggerFactory);
        }

        /// <summary>
        /// 创建发布者,每次发送使用同一通道
        /// </summary>
        /// <param name="loggerFactory">日志</param>
        /// <returns></returns>
        public IPublish CreateSingletonPublish(ILoggerFactory loggerFactory = null)
        {
            return configurationManager.CreateSingletonPublish(loggerFactory);
        }

        public ISubscribe CreateISubscribe(ILoggerFactory loggerFactory = null)
        {
            return configurationManager.CreateSubscribe(loggerFactory);
        }
    }
}
