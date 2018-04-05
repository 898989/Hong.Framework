using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Hong.Common.Extendsion;
using System;
using System.Collections.Concurrent;
using Hong.DAO.Core;

namespace Hong.DAO.QueryCache
{
    /// <summary>查询缓存管理
    /// </summary>
    public class QueryKeyManager
    {
        /// <summary>记录是否已分析SQL
        /// </summary>
        public readonly ConcurrentDictionary<string, SQLInfo> querySQLKeys = new ConcurrentDictionary<string, SQLInfo>();

        public static ConcurrentDictionary<string, string> tableModels = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 语句关联的表字段
        /// </summary>
        public static Dictionary<string, List<SQLParse.Expression>> TableFileds = new Dictionary<string, List<SQLParse.Expression>>();

        /// <summary>截取表名
        /// </summary>
        readonly Regex regexTableName = new Regex(@"from(.*?)where|join(.*?)on", RegexOptions.IgnoreCase);

        /// <summary>
        /// 获取 SQL 语句信息
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns></returns>
        public SQLInfo GetSQLInfo(string sql)
        {
            if (querySQLKeys.TryGetValue(sql, out SQLInfo sqlInfo))
            {
                return sqlInfo;
            }

            lock (querySQLKeys)
            {
                if (querySQLKeys.TryGetValue(sql, out sqlInfo))
                {
                    return sqlInfo;
                }

                sqlInfo = new SQLInfo()
                {
                    Key = Security.GetMD532(sql)
                };

                var parse = new SQLParse(sql);
                var tables = new List<string>();
                /*
                var mc = regexTableName.Matches(sql);
                for (var index = 0; index < mc.Count; index++)
                {
                    var t = mc[index].Groups[1].Value;
                    foreach (var str in t.Split(','))
                    {
                        var point = str.IndexOf(".");
                        if (point == -1)
                        {
                            tables.Add(str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        }
                        else
                        {
                            tables.Add(str.Substring(point + 1).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        }
                    }
                }

                sqlInfo.Tables = tables.ToArray();
                querySQLKeys.TryAdd(sql, sqlInfo);
                */

                var tableConditions = parse.GetTableCondition();
                foreach (var item in tableConditions)
                {
                    tables.Add(item.Key);

                    if (TableFileds.TryGetValue(item.Key, out List<SQLParse.Expression> expressions))
                    {
                        expressions.AddRange(item.Value);
                    }
                    else
                    {
                        TableFileds.Add(item.Key, item.Value);
                    }
                }

                sqlInfo.Tables = tables.ToArray();
            }

            return sqlInfo;
        }

        /*
         * 查询缓存方案，以最优精度刷新缓存，无法确定精确度的查询以表变动刷新缓存
         * 查询KEY，用关联数据缓存版本决定，其值为动态值（从关联数获取获取版本号）
         * 
         */

        /// <summary>获取查询 Key
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="sqlKey">返回 SQL 语句 Key</param>
        /// <param name="ps">查询参数</param>
        /// <returns>返回查询 Key</returns>
        public string GetQueryKey(string sql, string sqlKey, params object[] ps)
        {
            if (ps == null)
            {
                return sqlKey + "/0";
            }

            var key = sqlKey;
            var sb = new StringBuilder();
            foreach (var item in ps)
            {
                sb.Append(item.ToString()).Append("/");
            }

            key = sb.ToString();
            sb = null;

            key = sqlKey + "/" + key;

            //长度超过150时MD5压缩
            if (key.Length > 150)
            {
                key = Security.GetMD532(key);
            }

            return key;
        }

        static QueryKeyManager()
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName != "Hong.Model")
                {
                    continue;
                }

                foreach (var type in assembly.GetTypes())
                {
                    foreach (var a in type.GetCustomAttributes(false))
                    {
                        if (a is DataTableAttribute)
                        {
                            tableModels.TryAdd(((DataTableAttribute)a).TableName, type.Name);
                        }
                    }
                }
            }
        }
    }
}
