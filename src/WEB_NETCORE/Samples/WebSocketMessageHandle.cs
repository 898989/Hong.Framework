using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB_NETCORE.Samples
{
    public class WebSocketMessageHandle : Hong.WebSocket.IMessageHandle
    {
        public EventHandler<string> OnClose { get; } = (o, e) =>
        {
        };
        public EventHandler<string> OnMessage { get; } = (o, e) =>
        {
            var session = (Hong.WebSocket.WebSocketSession)o;

            try
            {
                session.SendAsync("{\"msg\":\"我收到了消息:" + e + "\"}").ConfigureAwait(false).GetAwaiter();
            }
            catch { }

        };
        public EventHandler<byte[]> OnBinary { get; } = (o, e) =>
        {

        };
    }
}
