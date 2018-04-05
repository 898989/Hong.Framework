using Hong.Test.Public;
using System;
using System.Collections.Generic;
using Xunit;

namespace Hong.Test.Performance
{
    public class TestDictionary
    {
        const string key = "type.1/product.";
        const string key1 = @"select * from (
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

        Dictionary<string, string> BuildData(string key)
        {
            var data = new Dictionary<string, string>();

            for (var index = 0; index < 10000; index++)
            {
                data.Add(key + index, index.ToString());
            }

            return data;
        }

        [Fact]
        public void Standard()
        {
            var data = BuildData(key);
            data.Add(key, key);
            string value = null;

            var time = Time.UserTime(() =>
            {
                for (var index = 0; index < 10000; index++)
                {
                    data.TryGetValue(key, out value);
                }
            });

            data = BuildData(key1);
            data.Add(key1, key1);

            var time1 = Time.UserTime(() =>
            {
                for (var index = 0; index < 10000; index++)
                {
                    data.TryGetValue(key1, out value);
                }
            }); 
        }
    }
}
