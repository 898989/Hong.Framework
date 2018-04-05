using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Cache.Core
{
    /// <summary>同步消息命令
    /// </summary>
    public enum SyncMessageAction
    {
        /// <summary>更新缓存项命令
        /// </summary>
        UPDATED,

        /// <summary>移除缓存项命令
        /// </summary>
        REMOVE,

        /// <summary>清空缓存命令
        /// </summary>
        CLEAR
    }
}
