using System.Collections.Generic;

namespace Hong.DAO.QueryCache
{
    public class SQLInfo
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQL;

        /// <summary>
        /// 语句KEY
        /// </summary>
        public string Key;

        public string[] Tables;

        /// <summary>
        /// 参数版本关联缓存KEY
        /// </summary>
        public string ParamsCacheKeys;
    }
}
