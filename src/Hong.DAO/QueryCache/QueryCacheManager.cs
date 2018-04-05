using Hong.Cache;
using Hong.DAO.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hong.DAO.QueryCache
{
    public class QueryCacheManager<Model>
    {
        /// <summary>查询结果缓存管理
        /// </summary>
        readonly IQueryCacheManager<List<int>> cache = CacheFactory.CreateQueryCacheManager<List<int>>();

        /// <summary>并发限制信息
        /// </summary>
        readonly ConcurrentDictionary<string, ConcurrentLimitInfo> cctLimit = new ConcurrentDictionary<string, ConcurrentLimitInfo>();

        /// <summary>缓存 KEY 管理
        /// </summary>IOKYU,MN
        readonly QueryKeyManager keyManager = new QueryKeyManager();

        /// <summary>获取缓存数据
        /// </summary>
        /// <param name="queryData">查询数据方法</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="arguments">SQL 参数</param>
        /// <returns>返回查询结果</returns>
        public async Task<List<int>> GetCacheData(Func<Task<List<int>>> queryData, string sql, params object[] arguments)
        {
            var sqlInfo = keyManager.GetSQLInfo(sql);
            var queryKey = keyManager.GetQueryKey(sql, sqlInfo.Key, arguments);

            return await ConcurrentLimit(queryKey, () =>
            {
                var result = cache.TryGet(queryKey, sqlInfo.Tables);
                return result ?? new List<int>();

            }, async () =>
            {
                var data = await queryData();
                switch (cache.TrySet(queryKey, sqlInfo.Tables, data))
                {
                    case 1:
                    case 0:
                        return data;
                    case -1:
                        return null;
                }

                return null;
            });
        }

        /// <summary>相同查询并发限制,此存在问题暂放弃
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getDataFromCache">从缓存获取查询结果</param>
        /// <param name="getDataFromDatabase">从数据库查询</param>
        /// <returns></returns>
        async Task<List<int>> ConcurrentLimit(string key, Func<List<int>> getDataFromCache, Func<Task<List<int>>> getDataFromDatabase)
        {
            //有缓存,直接返回缓存
            var data = getDataFromCache();
            if (data != null)
            {
                return data;
            }

            //无缓存,并发时只执行一个查询
            var hashCode = Thread.CurrentThread.GetHashCode();
            var limitInfo = cctLimit.GetOrAdd(key, (item) => new ConcurrentLimitInfo());

            var oldCode = limitInfo.FirstThreadHashCode;
            if (oldCode == -1 && limitInfo.StartTime.AddSeconds(1) <= DateTime.Now &&
                Interlocked.CompareExchange(ref limitInfo.FirstThreadHashCode, hashCode, oldCode) == oldCode)
            {
                //初始化批次
                limitInfo.TempCacheQueryResult = null;
                limitInfo.Exception = null;
                limitInfo.StartTime = DateTime.Now;
            }
            else
            {
                //等待第一个线程查询数据完成
                while (limitInfo.FirstThreadHashCode != -1)
                {
                    SpinWait.SpinUntil(() => limitInfo.FirstThreadHashCode != -1, 1);
                }

                return limitInfo.TempCacheQueryResult;
            }

            try
            {
                limitInfo.TempCacheQueryResult = await getDataFromDatabase();
            }
            catch (Exception ex)
            {
                limitInfo.Exception = ex;
                limitInfo.TempCacheQueryResult = null;
            }
            finally
            {
                limitInfo.FirstThreadHashCode = -1;
            }

            return limitInfo.TempCacheQueryResult;
        }

        /// <summary>
        /// 删除指定表关联的所有查询缓存
        /// </summary>
        /// <param name="table">表</param>
        public void TryRemoveCache(string table)
        {
            cache.TryRemove(table);
        }

        public void TryRemoveCache(DataModelInfo<Model> model, Option option)
        {
            if (!QueryKeyManager.TableFileds.TryGetValue(model.TableName, out List<SQLParse.Expression> fields))
            {
                return;
            }

            foreach (var f in fields)
            {
                if (option == Option.Delete && f.Left == "id") continue;

                //cache.TryRemove(sqls.Value.Key);
            }
        }

        public enum Option
        {
            Add, Upate, Delete
        }
    }
}
