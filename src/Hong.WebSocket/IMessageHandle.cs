using System;

namespace Hong.WebSocket
{
    public interface IMessageHandle
    {
        /// <summary>关闭事件
        /// 参数:object<see cref="WebSocketSession"/>
        /// </summary>
        EventHandler<string> OnClose { get; }

        /// <summary>收到文本消息事件
        /// 参数:object<see cref="WebSocketSession"/>
        /// </summary>
        EventHandler<string> OnMessage { get;}

        /// <summary>收到二进制消息事件
        /// 参数:object<see cref="WebSocketSession"/>
        /// </summary>
        EventHandler<byte[]> OnBinary { get;}
    }
}
