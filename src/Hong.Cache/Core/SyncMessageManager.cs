using Hong.Common.Tools;
using Hong.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Cache.Core
{
    /// <summary>
    /// 同步消息管理
    /// </summary>
    public class SyncMessageManager
    {
        public SyncMessageManager(IConfiguration config, ILoggerFactory loggerFactory = null)
        {
            NotNull(config, nameof(config));

            var factory = new MQFactory(config);
            Publisher = factory.CreatePublish(loggerFactory);
            Subscriber = factory.CreateISubscribe(loggerFactory);

            RegisteSubscriber();
        }

        /// <summary>消息发布者
        /// </summary>
        IPublish Publisher { get; set; }

        /// <summary>消息认阅者
        /// </summary>
        ISubscribe Subscriber { get; set; }

        /// <summary>自已的KEY
        /// </summary>
        string OwnerIdentify { get; set; } = Guid.NewGuid().ToString();

        /// <summary>注册订阅
        /// </summary>
        /// <returns></returns>
        void RegisteSubscriber()
        {
            Subscriber.Start(true, Event);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="TValue">类型</typeparam>
        /// <param name="iEvent">事件消息体</param>
        public void RegisteEvents<TValue>(ISyncEvent iEvent)
        {
            OnEvents.TryAdd(typeof(TValue).FullName, iEvent);
        }

        #region 通知消息

        public void NotifyUpdate<TValue>(string key, string region) => Notify<TValue>(SyncMessageAction.UPDATED, key, region);

        public void NotifyRemove<TValue>(string key, string region) => Notify<TValue>(SyncMessageAction.REMOVE, key, region);

        public void NotifyClear<TValue>(string region) => Notify<TValue>(SyncMessageAction.CLEAR, null, region);

        /// <summary>
        /// 发送同步通知消息
        /// </summary>
        /// <param name="action">消息</param>
        /// <param name="key">缓存key</param>
        /// <param name="region">缓存域</param>
        /// <param name="ps">参数</param>
        void Notify<TValue>(SyncMessageAction action, string key, string region, string ps = null)
        {
            var msg = new SyncMessage()
            {
                Action = action,
                Key = key,
                Region = region,
                OwnerIdentity = OwnerIdentify,
                FromFullName = typeof(TValue).FullName,
                Params = ps
            };

            Publisher.SendMsg(System.Text.Encoding.UTF8.GetString(msg.Serialize()));
        }

        #endregion

        #region 消息事件

        void Event(string exchange, string routeKey, string msg)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msg);
            var syncMessage = bytes.Deserialize<SyncMessage>();

            if (syncMessage.OwnerIdentity == this.OwnerIdentify)
            {
                return;
            }

            switch (syncMessage.Action)
            {
                case SyncMessageAction.UPDATED:
                    OnUpdate(syncMessage);
                    break;

                case SyncMessageAction.REMOVE:
                    OnRemove(syncMessage);
                    break;

                case SyncMessageAction.CLEAR:
                    OnClear(syncMessage);
                    break;
            }
        }

        void OnUpdate(SyncMessage message)
        {
            if (OnEvents.TryGetValue(message.FromFullName, out ISyncEvent iEvents))
            {
                iEvents.OnUpdate(this, message);
            }
        }

        void OnRemove(SyncMessage message)
        {
            if (OnEvents.TryGetValue(message.FromFullName, out ISyncEvent iEvents))
            {
                iEvents.OnRemove(this, message);
            }
        }

        void OnClear(SyncMessage message)
        {
            if (OnEvents.TryGetValue(message.FromFullName, out ISyncEvent iEvents))
            {
                iEvents.OnClear(this, message);
            }
        }

        /// <summary>更新缓存事件
        /// </summary>
        ConcurrentDictionary<string, ISyncEvent> OnEvents = new ConcurrentDictionary<string, ISyncEvent>();
        #endregion
    }
}
