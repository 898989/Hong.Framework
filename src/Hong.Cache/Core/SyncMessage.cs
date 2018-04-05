using ProtoBuf;
using System;

namespace Hong.Cache.Core
{
    /// <summary>同步消息
    /// </summary>
    [ProtoContract]
    public class SyncMessage : EventArgs
    {
        /// <summary>
        /// 消息命令
        /// </summary>
        [ProtoMember(1)]
        public SyncMessageAction Action;

        /// <summary>
        /// 消息关联缓存键
        /// </summary>
        [ProtoMember(2)]
        public string Key = string.Empty;

        /// <summary>
        /// 消息关联缓存键区域
        /// </summary>
        [ProtoMember(3)]
        public string Region = string.Empty;

        /// <summary>
        /// 来源
        /// </summary>
        [ProtoMember(4)]
        public string OwnerIdentity = string.Empty;

        /// <summary>
        /// 消息来源
        /// </summary>
        [ProtoMember(5)]
        public string FromFullName = string.Empty;

        /// <summary>
        /// 参数
        /// </summary>
        [ProtoMember(6)]
        public string Params = string.Empty;
    }
}
