using Hong.MQ.RabbitMQ;

namespace Hong.MQ.RabbitMQ
{
    public class RabbitConfiguration
    {
        /// <summary>心跳超时时间,集群、高可用部署时至关重要的设置
        /// </summary>
        public ushort HearBeat { get; set; } = 60;

        /// <summary>连接中断是否自动重连,true不需要手动处理自动连接
        /// </summary>
        public bool AutoReConnect { get; set; } = true;

        /// <summary>连接中断自动重连重试间隔时间,单位秒
        /// </summary>
        public int AutoRetryConnectInterval { get; set; } = 5;

        /// <summary>自动关闭连接
        /// </summary>
        public bool AutoClose { get; set; } = true;

        /// <summary>主机
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>端口
        /// </summary>
        public int Port { get; set; } = 15672;

        /// <summary>用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
