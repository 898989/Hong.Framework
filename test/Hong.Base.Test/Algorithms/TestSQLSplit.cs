using Hong.DAO.QueryCache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hong.Base.Test.Algorithms
{
    
    public class TestSQLSplit
    {
        [Fact]
        public void TestSplit()
        {
            //string sql = "select * from t_orders where id in (select orderId from t_orderDetails where ProductId={0})";
            //string sql = "select * from t_orders where id={0} UNINSTALL select * from t_orders where id={1} ";
            //string sql = "select * from t_orders WHeRE id in (select orderId from t_orderDetails where ProductId={0}) UNINSTALL select * from t_orders where id={1}";
            string sql = "select a.typeid,b.name from t_products as a join t_productmaster as b on a.masterid=b.id where a.datetime between '2017/01/01' and '2017/12/31';";
            var aaa = new SQLParse(sql);
            var result =  aaa.Split();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < 200; i++)
            {
                var t = aaa.GetTableCondition();
            }

            watch.Stop();

        }
    }
}
