using Xunit;
using Hong.Test.Public;

namespace Hong.Test.Performance
{
    public class TestSecurity
    {
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
        public void MD5_16()
        {
            var time = Time.WhileUseTime(() =>
            {
                Hong.Common.Extendsion.Security.GetMD516(sql);
            }, 10000);

            var time1 = Time.WhileUseTime(() =>
            {
                Hong.Common.Extendsion.Security.GetMD532(sql);
            }, 10000);
        }
    }
}
