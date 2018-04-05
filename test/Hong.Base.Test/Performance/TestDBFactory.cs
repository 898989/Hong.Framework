using Hong.DAO;
using Hong.DAO.QueryCache;
using Hong.Test.Public;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace Hong.Test.Performance
{
    public class TestDBFactory
    {
        [Fact]
        public void InsertPerformance()
        {
            Hong.Model.Order order = new Hong.Model.Order()
            {
                ID = 1
            };
            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;
            dbFactory.AddedEvent += AddEvent;
            dbFactory.UpdatedEvent += UpdateEvent;
            dbFactory.DeletedEvent += DeleteEvent;
            dbFactory.Load(order);

            var time = Time.UserTime(() =>
            {
                for (var index = 0; index < 10000; index++)
                {
                    dbFactory.Insert(order);
                }
            });

            Debug.WriteLine("InsertPerformance =>time:" + time);
        }

        [Fact]
        public void UpdatePerformance()
        {
            Hong.Model.Order order = new Hong.Model.Order()
            {
                ID = 1
            };
            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;
            dbFactory.Load(order);

            var time = Time.UserTime(() =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    order.UserID = i;
                    dbFactory.Update(order);
                }
            });

            Debug.WriteLine("UpdatePerformance =>time:" + time);
        }

        [Fact]
        public async void SingleThreadQueryPerformance()
        {
            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;
            string sql = "select id from t_orders where id=@v0";
            List<int> ids = await dbFactory.QueryID(sql, 1);

            var time = Time.UserTime(async () =>
            {
                for (var i = 0; i < 10000; i++)
                {
                    ids = await dbFactory.QueryID(sql, 1);
                    Assert.Equal(ids[0], 1);
                }
            });

            Debug.WriteLine("QueryPerformance =>time:" + time);
        }

        [Fact]
        public async void ConcurrentQueryPerformance()
        {
            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;
            string sql = "select id from t_orders where id=@v0";
            List<int> ids = await dbFactory.QueryID(sql, 1);

            Action action = () =>
            {
                var ids0 = dbFactory.QueryID(sql, 1);
                Assert.Equal(ids.Count, 1);
            };

            RunInThread.Start(action, 1000);

            Thread.Sleep(80);
            RunInThread.Start(action, 652);

            Thread.Sleep(200);
            RunInThread.Start(action, 18250);

            Thread.Sleep(5);
            RunInThread.Start(action, 8945);

            Thread.Sleep(1000);
            RunInThread.Start(action, 100000);
        }

        [Fact]
        public void ConcurrentMuiltQueryPerformance()
        {
            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;
            string sql = "select id from t_orders where id=@v0";

            var actions = new Action[] { async() =>
            {
                var ids = await dbFactory.QueryID(sql, 1);
                Assert.Equal(ids[0], 1);
            },  async()=> {
                var ids = await dbFactory.QueryID(sql, 2);
                Assert.Equal(ids[0], 2);
            } ,async()=>  {
                var ids = await dbFactory.QueryID(sql, 3);
                Assert.Equal(ids[0], 3); }
            };

            RunInThread.Start(actions, 10);

            Thread.Sleep(80);
            RunInThread.Start(actions, 652);

            Thread.Sleep(200);
            RunInThread.Start(actions, 18250);

            Thread.Sleep(5);
            RunInThread.Start(actions, 8945);

            Thread.Sleep(1000);
            RunInThread.Start(actions, 100000);
        }


        [Fact]
        public void CharPerformance()
        {
            string dbField = "34lfm33453gsdbv";
            var ok = false;

            var time1 = Time.UserTime(() =>
             {
                 for (var i = 0; i < 10000; i++)
                 {
                     foreach (var c in dbField)
                     {
                         ok = c == '\'' || c == ' ' || c == ';';
                     }
                 }
             });
            Debug.WriteLine("CharPerformance =>time1:" + time1);

            var time2 = Time.UserTime(() =>
              {
                  for (var i = 0; i < 10000; i++)
                  {
                      ok = dbField.Contains("'") || dbField.Contains(" ") || dbField.Contains(";");
                  }
              });
            Debug.WriteLine("CharPerformance =>time2:" + time2);
        }

        [Fact]
        public void ParseSQLTablePerformance()
        {
            Regex regexTableName = new Regex(@"(from|join)\s+([^\(^\s]+)", RegexOptions.IgnoreCase);
            const string sql = @"select * from (
	            select 
                    t_orders.id,
                    t_orders.version,
                    t_orders.add_time,
                    t_orders.user_id,
                    t_orders.identity,
                    t_orders.sum_money,
                    t_orders.pay_money,
                    t_orders.`status`,
                    t_orders.recipient,
                    t_orders.re_address,
                    t_orders.re_mobile,
                    t_orders.re_addr_country_id,
                    t_orders.re_addr_province_id,
                    t_orders.re_addr_city_id,
                    t_orders.re_addr_area_id,
                    t_orders.receiving_time,
                    t_orders.pay_time,
                    t_orders.pay_id,
                    t_orders.express_number
                from t_orders
	            union all 
	            select 
                    t_orders.id,
                    t_orders.version,
                    t_orders.add_time,
                    t_orders.user_id,
                    t_orders.identity,
                    t_orders.sum_money,
                    t_orders.pay_money,
                    t_orders.`status`,
                    t_orders.recipient,
                    t_orders.re_address,
                    t_orders.re_mobile,
                    t_orders.re_addr_country_id,
                    t_orders.re_addr_province_id,
                    t_orders.re_addr_city_id,
                    t_orders.re_addr_area_id,
                    t_orders.receiving_time,
                    t_orders.pay_time,
                    t_orders.pay_id,
                    t_orders.express_number
                from t_orders
            )";

            var time = Time.WhileUseTime(() =>
            {
                string v = null;
                var mc = regexTableName.Matches(sql);
                for (var i = 0; i < mc.Count; i++)
                {
                    v = mc[i].Value;
                }
            }, 10000);

            Debug.WriteLine("CharPerformance =>time:" + time);
        }

        [Fact]
        public void GetQueryKeyPerformance()
        {
            string sql = @"select 
                    t_orders.id,
                    t_orders.version,
                    t_orders.add_time,
                    t_orders.user_id,
                    t_orders.identity,
                    t_orders.sum_money,
                    t_orders.pay_money,
                    t_orders.`status`,
                    t_orders.recipient,
                    t_orders.re_address,
                    t_orders.re_mobile,
                    t_orders.re_addr_country_id,
                    t_orders.re_addr_province_id,
                    t_orders.re_addr_city_id,
                    t_orders.re_addr_area_id,
                    t_orders.receiving_time,
                    t_orders.pay_time,
                    t_orders.pay_id,
                    t_orders.express_number
                from t_orders where user_id=@user_id and `status`=@status and pay_id=@pay_id";

            var queryCacheManager = new QueryKeyManager();
            var sqlKey = queryCacheManager.GetSQLInfo(sql);
            var queryKey = queryCacheManager.GetQueryKey(sql, sqlKey.Key, new object[] { 1, 2, 3 });

            var time = Time.WhileUseTime(() =>
            {
                queryCacheManager.GetQueryKey(sql, queryCacheManager.GetSQLInfo(sql).Key, new object[] { 1, 2, 3 });
            }, 10000);

            Debug.WriteLine("GetQueryKeyPerformance =>time:" + time);
        }


        public void AddEvent(object sender, int id)
        {

        }

        public void UpdateEvent(object sender, int id)
        {

        }

        public void DeleteEvent(object sender, int id)
        {

        }
    }
}
