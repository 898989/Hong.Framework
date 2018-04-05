using RabbitMQ.Client;

namespace Hong.MQ.RabbitMQ
{
    public class Manager: RabbitMQBase
    {
        public Manager(RabbitConfiguration config) :
            base(config)
        {
        }

        /// <summary>创建交换机
        /// </summary>
        /// <param name="exchangeKey">交换机标识</param>
        /// <param name="exchangeType">交换机类型</param>
        /// <param name="durable">消息持久</param>
        public void CreateExchange(string exchangeKey, string exchangeType, bool durable)
        {
            this.Channel.ExchangeDeclare(exchangeKey, exchangeType, durable);
        }

        /// <summary>交换机邦定队列
        /// </summary>
        /// <param name="exchangeKey">交换机标识</param>
        /// <param name="channelKey">队列标识</param>
        /// <param name="routeKey">路由标识</param>
        public void ExchangeBindChannel(string exchangeKey, string channelKey, string routeKey)
        {
            this.Channel.ExchangeBind(channelKey, exchangeKey, routeKey);
        }

        /// <summary>创建队列
        /// </summary>
        /// <param name="channelKey">队列标识</param>
        /// <param name="durable">消息持久</param>
        public void CreateQueue(string channelKey, bool durable)
        {
            this.Channel.QueueDeclare(channelKey, durable, false, false, null);
        }

    }
}
