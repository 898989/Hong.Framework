using Hong.Test.Public;
using System.Text.RegularExpressions;
using Xunit;

namespace Hong.Test.Performance
{
    public class TestRegex
    {
        static Regex regexTableName = new Regex(@"(from|join)\s+([^\(^\s]+)", RegexOptions.IgnoreCase);
        static Regex regexNumber = new Regex(@"^[a-zA-Z0-9\s,\./]*$");

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

        [Fact]
        public void RegexTableName()
        {
            int count = 0;
            var time = Time.WhileUseTime(() =>
             {
                 count = regexTableName.Matches(sql).Count;
             }, 10000);

            var time1 = Time.WhileUseTime(() =>
            {
                count = regexNumber.Matches(sql).Count;
            }, 10000);
        }
    }
}
