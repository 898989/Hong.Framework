
using Microsoft.Extensions.Logging;

namespace Hong.MQ.RabbitMQ
{
    public class SubscribeConfiguration: RabbitConfiguration
    {
        /// <summary>消息队列标识
        /// </summary>
        public string QueueKey { get; set; }

        public ISubscribe CreateHandle(ILoggerFactory loggerFactory = null)
        {
            var subscribe = new Subscribe(this, loggerFactory);
            subscribe.QueueKey = QueueKey;

            return subscribe;
        }
    }
}
