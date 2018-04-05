using RabbitMQ.Client;
using System;

namespace Hong.MQ.RabbitMQ
{
    public class RabbitMQBase
    {
        /// <summary>配置
        /// </summary>
        protected RabbitConfiguration EndPoint { get; set; }

        /// <summary>连接工厂
        /// </summary>
        protected ConnectionFactory ConnectionFactory { get; set; }

        /// <summary>连接
        /// </summary>
        protected IConnection Connection { get; set; }

        /// <summary>消息通道
        /// </summary>
        internal IModel Channel { get; set; }

        /// <summary>AMQP基本内容类头配置,用消息发布
        /// </summary>
        internal IBasicProperties PublishProperties = null;

        /// <summary>初始化<see cref="RabbitMQBase"/>类
        /// </summary>
        /// <param name="hostName">域名或IP</param>
        /// <param name="port">端口</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public RabbitMQBase(RabbitConfiguration config)
        {
            EndPoint = config;
            InitChannel();
        }

        void InitChannel()
        {
            ConnectionFactory = new ConnectionFactory();

            ConnectionFactory.HostName = EndPoint.Host;
            ConnectionFactory.UserName = EndPoint.UserName;
            ConnectionFactory.Password = EndPoint.Password;
            ConnectionFactory.Port = EndPoint.Port;

            ConnectionFactory.RequestedHeartbeat = EndPoint.HearBeat;
            ConnectionFactory.AutomaticRecoveryEnabled = EndPoint.AutoReConnect;
            ConnectionFactory.NetworkRecoveryInterval = new TimeSpan(EndPoint.AutoRetryConnectInterval);

            Connection = ConnectionFactory.CreateConnection();
            //connection.AutoClose = point.AutoClose;
            Connection.ConnectionShutdown += (o, e) =>
            {
                //此处添加关闭日志e.ReplyText
            };

            Channel = Connection.CreateModel();
        }

        /// <summary>
        /// 创建发布消息属性
        /// </summary>
        public void CreatePublishPropertys()
        {
            ///AMQP基本内容类头配置
            PublishProperties = Channel.CreateBasicProperties();
            ///消息持久化
            PublishProperties.DeliveryMode = 2;
        }

        /// <summary>关闭
        /// </summary>
        public virtual void Close()
        {
            this.Channel.Close(200, "Goodbye");
            //this.Channel.Close();
        }
    }
}
