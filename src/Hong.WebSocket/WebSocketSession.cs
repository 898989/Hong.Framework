using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;

namespace Hong.WebSocket
{
    public class WebSocketSession
    {
        /// <summary>唯一标识
        /// </summary>
        public string Identity = string.Empty;

        /// <summary>开始时间
        /// </summary>
        public DateTime StartTime = DateTime.Now;

        /// <summary>最后消息时间
        /// </summary>
        public DateTime LastMessageTime = DateTime.Now;

        /// <summary>关联标识
        /// </summary>
        public string AssociatedIdentity = string.Empty;

        /// <summary>WebSocket
        /// </summary>
        public System.Net.WebSockets.WebSocket WebSocket = null;

        private ILogger Log;

        public WebSocketSession(ILogger log)
        {
            Log = log;
        }

        /// <summary>发送消息给某客户端
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync(string message)
        {
            if (WebSocket == null)
            {
                return;
            }

            NotNullOrEmpty(message, nameof(message));

            if (LastMessageTime < DateTime.Now.AddSeconds(-6))
            {
                Abort();//释放连接
                throw new Exception("连接超时");
            }

            LastMessageTime = DateTime.Now;
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

            try
            {
                await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Log?.LogError("发送消息失败, 内容:" + message, ex);
                Abort();

                throw;
            }
        }

        /// <summary>中止连接
        /// </summary>
        public void Abort()
        {
            if (WebSocket == null)
            {
                return;
            }

            try
            {
                WebSocket.Abort();
            }
            catch (Exception ex)
            {
                Log?.LogError("中止连接失败", ex);
                throw;
            }
            finally
            {
                try
                {
                    WebSocket.Dispose();
                }
                catch { }
                finally
                {
                    WebSocket = null;
                }
            }
        }
    }
}
