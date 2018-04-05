using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using static Hong.Common.Extendsion.Guard;
using static Hong.MQ.RabbitMQ.Enums;

namespace Hong.MQ.RabbitMQ
{
    /*
    exchange常用有三种类型：
　　  Direct ：处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。
　　  Fanout ：不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都会被转发到与该交换机绑定的所有队列上。
　　  Topic : 将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。

    Queue属性　　
      Durability 和exchange相同，未持久化的队列，服务重启后销毁。
      Auto delete 当没有消费者连接到该队列的时候，队列自动销毁。
      Exclusive 使队列成为私有队列，只有当前应用程序可用，当你需要限制队列只有一个消费者，这是很有用的。
        扩展属性如下对应源程序 RabbitMQ.Client.IModel.QueueDeclare(string, bool, bool, bool, System.Collections.Generic.IDictionary<string,object>)最后的参数
      Message TTL 当一个消息被推送在该队列的时候 可以存在的时间 单位为ms，(对应扩展参数argument "x-message-ttl" )
      Auto expire 在队列自动删除之前可以保留多长时间（对应扩展参数argument "x-expires"）
      Max length 一个队列可以容纳的已准备消息的数量（对应扩展参数argument "x-max-length"）

    注意:
      一旦创建了队列和交换机，就不能修改其标志了。例如，如果创建了一个non-durable的队列，然后想把它改变成durable的，唯一的办法就是删除这个队列然后重现创建。
    */

    /// <summary>发布
    /// </summary>
    public class Publish : IPublish
    {
        internal string Exchange { get; set; }

        string _RouteKey = string.Empty;
        internal string RouteKey
        {
            get
            {
                return _RouteKey;
            }
            set
            {
                if (value == null)
                {
                    _RouteKey = string.Empty;
                }
                else
                {
                    _RouteKey = value;
                }
            }
        }

        internal string QueueKey { get; set; }

        internal SendWay SendByWay { get; set; } = SendWay.Queue;

        readonly ILogger Log;

        RabbitConfiguration Config;

        public Publish(RabbitConfiguration config, ILoggerFactory loggerFactory = null)
        {
            Log = loggerFactory?.CreateLogger("RabbitMQ.Publish");
            Config = config;
        }

        private RabbitMQBase CreateRabbitMQ()
        {
            var rabbitMQBase = new RabbitMQBase(Config);

            rabbitMQBase.CreatePublishPropertys();
            rabbitMQBase.Channel.ConfirmSelect();

            rabbitMQBase.Channel.BasicReturn += (o, e) =>
            {
                string s = e.RoutingKey;
            };

            rabbitMQBase.Channel.CallbackException += (o, e) =>
            {
                Log?.LogError("#From =>CallbackException", e.Exception);
            };
            rabbitMQBase.Channel.ModelShutdown += (o, e) =>
            {
                Log?.LogInformation("#From =>ModelShutdown", e.ReplyText);
            };

            return rabbitMQBase;
        }

        public void SendMsg(string msg)
        {
            if (SendByWay == SendWay.Queue)
            {
                SendMsg(QueueKey, msg);
            }
            else if (SendByWay == SendWay.Exchange)
            {
                SendMsg(Exchange, RouteKey, msg);
            }
        }

        /// <summary>发布消息(未配置交换机使用)
        /// </summary>
        /// <param name="queueKey">队列标识</param>
        /// <param name="msg">消息</param>
        void SendMsg(string queueKey, string msg)
        {
            NotNull(queueKey, nameof(queueKey));
            NotNull(msg, nameof(msg));

            var mq = CreateRabbitMQ();

            mq.Channel.BasicPublish(string.Empty, queueKey, mq.PublishProperties, Encoding.UTF8.GetBytes(msg));
            //this.channel.BasicPublish(string.Empty, queueKey,true, _properties, Encoding.UTF8.GetBytes(msg));
            mq.Close();
        }

        /// <summary>发送消息(配置了交换机使用)
        /// </summary>
        /// <param name="exchange">交换机</param>
        /// <param name="routingKey">路由标识,为空时发过给交换机,非空时发送到指定的路由</param>
        /// <param name="msg">消息</param>
        void SendMsg(string exchange, string routingKey, string msg)
        {
            NotNull(exchange, nameof(exchange));

            var mq = CreateRabbitMQ();
            mq.Channel.BasicPublish(exchange, routingKey, mq.PublishProperties, Encoding.UTF8.GetBytes(msg));
            //this.channel.BasicPublish(exchange, routingKey,true, _properties, Encoding.UTF8.GetBytes(msg));
            mq.Close();
        }
    }
}
