using Hong.Cache.Core;
using Hong.Test.Model;
using Xunit;
using Hong.Common.Tools;

namespace Hong.Test.Function.Public
{
    public class TestCache
    {
        public async void Standard(ICache<byte[]> byteCacheHandler, ICache<string> stringCacheHandler1)
        {
            string key = "User.1";
            string region = "test";

            Assert.True(await byteCacheHandler.TryClear(region));
            User u = new User();

            #region 读写测试
            u.Telephone = "abcd";
            u.Email = "abc@aa.com";
            Assert.Equal(byteCacheHandler.TrySet(key, region, u.Serialize(), u.Version), 1);
            var fromRedisUser = (User)byteCacheHandler.TryGet(key, region).Deserialize(u.GetType());
            Assert.Equal(u.Telephone, fromRedisUser.Telephone);
            Assert.Equal(u.Email, fromRedisUser.Email);

            #endregion
            Assert.Equal(byteCacheHandler.TrySet(key, region, fromRedisUser.Serialize(), fromRedisUser.Version), -1);
            fromRedisUser.Version++;
            #region 版本测试

            #endregion

            #region TryClear测试

            Assert.True(await byteCacheHandler.TryClear(region));
            var byteObj = byteCacheHandler.TryGet(key, region);
            Assert.Null(byteObj);

            Assert.True(byteCacheHandler.TrySet(key, region, u.Serialize(), u.Version) == 1);
            Assert.True(await byteCacheHandler.TryClear());
            byteObj = byteCacheHandler.TryGet(key, region);
            Assert.Null(byteObj);

            #endregion

            #region TryUpdate测试
            Assert.Equal(byteCacheHandler.TrySet(key, region, u.Serialize(), u.Version), 1);

            u.Email = "redis@update.com";
            Assert.Equal(byteCacheHandler.TryUpdate(key, region, u.Serialize(), u.Version), -1);

            u.Version++;
            Assert.Equal(byteCacheHandler.TryUpdate(key, region, u.Serialize(), u.Version), 1);

            fromRedisUser = (User)byteCacheHandler.TryGet(key, region).Deserialize(u.GetType());
            fromRedisUser.Version++;
            Assert.Equal(u.Email, fromRedisUser.Email);

            //TrySet版本测试
            Assert.Equal(byteCacheHandler.TrySet(key, region, u.Serialize(), u.Version), -1);
            u.Version++;
            Assert.Equal(byteCacheHandler.TrySet(key, region, u.Serialize(), u.Version), 1);
            #endregion

            #region 测试空值
            byte[] emptyArrary = new byte[0];
            byteCacheHandler.TrySet("byte.test", emptyArrary, 1);
            byte[] getEmptyArrary = byteCacheHandler.TryGet("byte.test");
            Assert.NotNull(getEmptyArrary);
            Assert.True(getEmptyArrary.Length == 0);

            string emptyStr = string.Empty;
            stringCacheHandler1.TrySet("string.test", emptyStr, 1);
            string getEmptyStr = stringCacheHandler1.TryGet("string.test");
            Assert.NotNull(getEmptyStr);
            Assert.True(getEmptyStr.Length == 0);
            #endregion

            Assert.True(await byteCacheHandler.TryClear(region));
        }
    }
}
