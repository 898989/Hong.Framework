using static Hong.Common.Extendsion.Guard;

namespace Hong.Cache.Redis
{
    /// <summary>redis服务器终节点
    /// </summary>
    public sealed class ServerEndPoint
    {
        /// <summary>
        /// 初始化 <see cref="ServerEndPoint"/> 类.
        /// </summary>
        public ServerEndPoint()
        {
        }

        /// <summary>
        /// 初始化 <see cref="ServerEndPoint"/> 类.
        /// </summary>
        /// <param name="host">redis主机</param>
        /// <param name="port">redis端口</param>
        /// <exception cref="System.ArgumentNullException">If host is null.</exception>
        public ServerEndPoint(string host, int port)
        {
            NotNullOrWhiteSpace(host, nameof(host));

            this.Host = host;
            this.Port = port;
        }

        /// <summary>获取设置端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>获取设置主机
        /// </summary>
        public string Host { get; set; }

    }
}
