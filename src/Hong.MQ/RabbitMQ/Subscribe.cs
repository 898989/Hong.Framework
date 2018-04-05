using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Hong.MQ.RabbitMQ
{
    /// <summary>订阅
    /// </summary>
    public class Subscribe : RabbitMQBase, ISubscribe
    {
        readonly ILogger Log;

        public Subscribe(RabbitConfiguration config, ILoggerFactory loggerFactory = null) :
            base(config)
        {
            Log = loggerFactory?.CreateLogger("RabbitMQ.Subscribe");
        }

        /// <summary>收接消息的队列列表
        /// </summary>
        internal string QueueKey { get; set; }

        string ConsumeTag { get; set; }

        /// <summary>开始接收消息
        /// </summary>
        /// <param name="queueKey">队列标识</param>
        /// <param name="reject">处理消息失败时,是否重新将消息放入队列</param>
        /// <param name="callBack">接收到消息时回调</param>
        public void Start(bool reject, Action<string, string, string> callBack)
        {
            //当同时消息多个队列时可能产生并发,可以使用自定义任务调度器, 限制与定制TaskScheduler并发度,否则当预取大于1时可能要注意并发
            //this.connectionFactory.TaskScheduler = new MyTaskScheduler(1);

            //此设置很重要,根据实现情况.预取量,一次多个减少网络带宽
            //此处只当参考
            Channel.BasicQos(0, 10, true);

            //测试使用增加处理延时
            var sleep = new Random().Next(100);

            var consumer = new EventingBasicConsumer(this.Channel);

            consumer.Received += (ch, ea) =>
            {
                System.Threading.Thread.Sleep(sleep);
                var body = Encoding.UTF8.GetString(ea.Body);

                IModel channel = ((EventingBasicConsumer)ch).Model;
                if (channel.IsClosed)
                {
                    return;
                }

                try
                {
                    channel.BasicAck(ea.DeliveryTag, false);

                    callBack(ea.Exchange, ea.RoutingKey, body);
                }
                catch (Exception ex)
                {
                    Log?.LogError("#From =>Received", ex);

                    if (reject && channel.IsOpen)
                        channel.BasicReject(ea.DeliveryTag, true);
                }
            };

            ConsumeTag = Channel.BasicConsume(QueueKey, false, consumer);
        }

        /// <summary>停止接收消息
        /// </summary>
        public void Stop()
        {
            Channel.BasicCancel(ConsumeTag);
        }

        public override void Close()
        {
            Stop();
            base.Close();
        }
    }
}
