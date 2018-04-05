using System;
using System.Collections.Generic;
using System.Text;

namespace Hong.Cache.Core
{
    public interface ISyncEvent
    {
        /// <summary>
        /// 缓存更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        void OnUpdate(object sender, SyncMessage msg);

        /// <summary>
        /// 缓存删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        void OnRemove(object sender, SyncMessage msg);

        /// <summary>
        /// 缓存清空事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        void OnClear(object sender, SyncMessage msg);
    }
}
