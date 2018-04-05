
using Microsoft.Extensions.Logging;
using static Hong.MQ.RabbitMQ.Enums;

namespace Hong.MQ.RabbitMQ
{
    /// <summary>通过消息队列发布消息
    /// </summary>
    public class PlushQueueConfiguration : RabbitConfiguration
    {
        /// <summary>消息队列标识
        /// </summary>
        public string QueueKey { get; set; }

        public IPublish CreateHandle(ILoggerFactory loggerFactory = null)
        {
            var publish = new Publish(this, loggerFactory);

            publish.QueueKey = QueueKey;
            publish.SendByWay = SendWay.Queue;

            return publish;
        }
    }
}
