using System.Collections.Concurrent;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;
using Hong.Common.Extendsion;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Hong.WebSocket
{
    public class WebSocketSessionManger
    {
        public const string IDENTITY_COOKIE_NAME = "_guid";

        static ConcurrentDictionary<string, WebSocketSession> _session = new ConcurrentDictionary<string, WebSocketSession>();
        readonly ILogger Log;

        public WebSocketSessionManger(ILoggerFactory loggerFactory)
        {
            Log = loggerFactory.CreateLogger("WebSocket");
        }

        /// <summary>根据标识获取session
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public WebSocketSession GetSession(string identity)
        {
            NotNullOrEmpty(identity, nameof(identity));

            WebSocketSession session = null;
            if (_session.TryGetValue(identity, out session))
            {
                return session;
            }

            return null;
        }

        /// <summary>删除session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool RemoveSession(WebSocketSession session)
        {
            NotNull(session, nameof(session));

            if (!_session.TryRemove(session.Identity, out session))
            {
                return false;
            }

            session.Abort();

            return true;
        }

        /// <summary>添加session
        /// 同一浏览器只能创建一个
        /// 如果是多页面,建议方案使用页面消息相互转告
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        internal WebSocketSession AddSession(string identity, System.Net.WebSockets.WebSocket webSocket)
        {
            NotNullOrEmpty(identity, nameof(identity));
            NotNull(webSocket, nameof(webSocket));

            WebSocketSession session;
            if (_session.TryGetValue(identity, out session))
            {
                session.WebSocket = webSocket;
                session.StartTime = DateTime.Now;
                session.LastMessageTime = DateTime.Now;

                return session;
            }

            session = new WebSocketSession(Log)
            {
                WebSocket = webSocket,
                StartTime = DateTime.Now,
                LastMessageTime = DateTime.Now
            };

            if (_session.TryAdd(identity, session))
            {
                return session;
            }

            return null;
        }

        /// <summary>所有session
        /// </summary>
        public ICollection<WebSocketSession> AllSessions
        {
            get { return _session.Values; }
        }

        /// <summary>当前请求对应的session
        /// </summary>
        public WebSocketSession CurrentRequestSession
        {
            get
            {
                var guid = Cookie.Get(IDENTITY_COOKIE_NAME);

                if (string.IsNullOrEmpty(guid))
                {
                    return null;
                }

                return GetSession(guid);
            }
        }

        /// <summary>创建新的session
        /// </summary>
        public void CreateNewSession()
        {
            var guid = Guid.NewGuid().ToString();
            Cookie.Set(IDENTITY_COOKIE_NAME, guid);
        }

        /// <summary>消息广播
        /// </summary>
        /// <param name="message">消息</param>
        public void BroadcastAsync(string message)
        {
            var task = new Task(async () =>
            {
                foreach (var session in AllSessions)
                {
                    try
                    {
                        await session.SendAsync(message);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Log.LogDebug("广播消息失败,identity:" + session.Identity, ex);
#endif
                    }
                }
            });

            task.Start();
            task.Wait();
        }

        /// <summary>中止所有连接
        /// </summary>
        public void AbortAllSessionAsync()
        {
            var task = new Task(() =>
            {
                foreach (var session in AllSessions)
                {
                    try
                    {
                        session.Abort();
                    }
                    catch { }
                }
            });

            task.Start();
            task.Wait();
        }
    }
}
