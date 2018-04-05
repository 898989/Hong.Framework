using System;
using System.Collections.Generic;
using System.Threading;

namespace Hong.DAO.QueryCache
{
    public class ConcurrentLimitInfo
    {
        /// <summary>
        /// 当前执行的线程ID
        /// </summary>
        public int FirstThreadHashCode;

        /// <summary>临时缓存查询结果
        /// </summary>
        public List<int> TempCacheQueryResult;

        /// <summary>
        /// 开始执行时间,防止执行查询线程中断造成永远无法查询问题
        /// </summary>
        public DateTime StartTime;

        /// <summary>异常信息
        /// </summary>
        public Exception Exception;
    }
}
