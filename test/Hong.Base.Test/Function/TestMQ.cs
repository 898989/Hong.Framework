using Hong.MQ.RabbitMQ;
using System.Collections.Generic;
using Xunit;

namespace Hong.Test.Function
{
    public class TestMQ
    {
        [Fact]
        public void Standard()
        {
            const string plushExchange = "cachePlushExchange";
            const string subcribeQueue = "cachesubscribeQueue";

            var plusherConfig = new PlushExchangeConfiguration()
            {
                Host = "192.168.1.109",
                Port = 5672,
                UserName = "zhanghong",
                Password = "zhanghong@2016",
                Exchange = plushExchange
            };

            var subscriteConfig1 = new SubscribeConfiguration()
            {
                Host = "192.168.1.109",
                Port = 5672,
                UserName = "zhanghong",
                Password = "zhanghong@2016",
                QueueKey = subcribeQueue + "_1"
            };

            var subscriteConfig2 = new SubscribeConfiguration()
            {
                Host = "192.168.1.109",
                Port = 5672,
                UserName = "zhanghong",
                Password = "zhanghong@2016",
                QueueKey = subcribeQueue + "_2"
            };

            var plusher = plusherConfig.CreateHandle();

            var subcribeResult = new List<string>();

            var subscribe1 = subscriteConfig1.CreateHandle();
            subscribe1.Start(true, (exchange, routeKey, msg) =>
             {
                 subcribeResult.Add(exchange + "." + routeKey + "=>say:" + msg);
             });

            var subscribe2 = subscriteConfig2.CreateHandle();
            subscribe2.Start(true, (exchange, routeKey, msg) =>
            {
                subcribeResult.Add(exchange + "." + routeKey + "=>say:" + msg);
            });

            plusher.SendMsg("测试消息");

            System.Threading.SpinWait.SpinUntil(() =>
            {
                return subcribeResult.Count == 2;
            }, 10000);

            plusher.Close();
            subscribe1.Close();
            subscribe2.Close();

            Assert.True(subcribeResult.Count == 2);
        }
    }
}
