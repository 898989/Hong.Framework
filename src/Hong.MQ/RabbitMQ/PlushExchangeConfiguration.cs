
using Microsoft.Extensions.Logging;
using static Hong.MQ.RabbitMQ.Enums;

namespace Hong.MQ.RabbitMQ
{
    /// <summary>通过交换机发布消息
    /// </summary>
    public class PlushExchangeConfiguration : RabbitConfiguration
    {
        /// <summary>交换机
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>路由,为空时通过交换机配置路由转发消息,不为空时发送到指向路由
        /// </summary>
        public string RouteKey { get; set; } = string.Empty;

        public IPublish CreateHandle(ILoggerFactory loggerFactory = null)
        {
            var publish = new Publish(this, loggerFactory)
            {
                Exchange = Exchange,
                RouteKey = RouteKey,
                SendByWay = SendWay.Exchange
            };

            return publish;
        }

        public ISingletonPublish CreateSingletonHandle(ILoggerFactory loggerFactory = null)
        {
            var publish = new SingletonPublish(this, loggerFactory)
            {
                Exchange = Exchange,
                RouteKey = RouteKey,
                SendByWay = SendWay.Exchange
            };

            return publish;
        }
    }
}
