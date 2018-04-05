using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;

namespace Hong.WebSocket
{
    public class WebSocketHandle
    {
        readonly WebSocketSessionManger SessionManager;
        readonly ILogger Log;
        readonly IMessageHandle IMessageHandle;
        const int MaxBufferSize = 2048;

        public WebSocketHandle(ILoggerFactory loggerFactory, WebSocketSessionManger sessionManager, IMessageHandle iMessageHandle)
        {
            Log = loggerFactory.CreateLogger("WebSocket");
            SessionManager = sessionManager;
            IMessageHandle = iMessageHandle;
        }

        public async Task Process(HttpContext context)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            if (socket == null || socket.State != WebSocketState.Open)
            {
                return;
            }

            var cookie = context.Request.Cookies[WebSocketSessionManger.IDENTITY_COOKIE_NAME];
            if (cookie == null || string.IsNullOrEmpty(cookie))
            {
                throw new Exception("沒有Socket标识");
            }

            var session = SessionManager.AddSession(cookie, socket);

            try
            {
                await SessionHandle(session);
            }
            catch { }
        }

        /// <summary>处理请求
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        async Task SessionHandle(WebSocketSession session)
        {
            if (session == null)
            {
                return;
            }

            byte[] reciveData = null;
            ArraySegment<byte> buffer;
            WebSocketReceiveResult received = null;
            var webSocket = session.WebSocket;
            var offset = 0;

            while (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.Connecting)
            {
                reciveData = new byte[MaxBufferSize];
                buffer = new ArraySegment<byte>(reciveData);
                offset = 0;

                received = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (received.MessageType == WebSocketMessageType.Close)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("关闭");
#endif
                    try
                    {
                        IMessageHandle.OnClose?.Invoke(session, received.CloseStatusDescription);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("关闭事件执行失败", ex);
                    }

                    break;
                }

                while (!received.EndOfMessage)
                {
                    offset += received.Count;
                    received = await webSocket.ReceiveAsync(new ArraySegment<byte>(reciveData, offset, MaxBufferSize - offset), CancellationToken.None);
                }

                session.LastMessageTime = DateTime.Now;

                if (received.MessageType == WebSocketMessageType.Text)
                {
                    //心跳
                    var data = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0');
                    if (data == "1")
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, CancellationToken.None);

                        continue;
                    }
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("收到消息:" + data);
#endif
                    try
                    {
                        IMessageHandle.OnMessage?.Invoke(session, data);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("接收文件消息事件执行失败", ex);
                    }
                }
                else if (received.MessageType == WebSocketMessageType.Binary)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("收到二进制数据,长度:" + buffer.Array.Length);
#endif
                    try
                    {
                        IMessageHandle.OnBinary?.Invoke(session, buffer.Array);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("接收二进制消息事件执行失败", ex);
                    }
                }
            }
        }
    }
}
