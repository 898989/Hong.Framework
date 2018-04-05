using Hong.Test.Function.Public;
using Xunit;

namespace Hong.Test.Function
{
    public class TestRedis
    {
        [Fact]
        public void Standard()
        {
            var redisConfig = new Cache.Redis.RedisCacheConfiguration()
            {
                ConnectionTimeout = 1000,
                AllowAdmin = true,
                Host = "192.168.1.106",
                Port = 6379
            };

            var redisHandle1 = redisConfig.CreateHandle<byte[]>();
            var redisHandle2 = redisConfig.CreateHandle<string>();

            new TestCache().Standard(redisHandle1, redisHandle2);
        }
    }
}
