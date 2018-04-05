using Hong.Cache;
using Hong.Test.Model;
using System.Collections.Generic;
using Xunit;

namespace Hong.Test.Function
{
    public class TestCacheManager
    {
        [Fact]
        public void Standard()
        {
            User u = new User();

            u.ID = 45;
            u.Telephone = "123456";
            u.Mobile = "15999663333";
            u.Email = "test@test.com";
            u.Dic = new Dictionary<string, string>() {
                { "a","bbbb"},
                {"b","ccccccccccc" }
            };

            ICacheManager<User> cache = CacheFactory.CreateCacheManager<User>();

            var version = u.Version;
            cache.TryAdd("1.user.1", u);
            Assert.True(version < u.Version);

            version = u.Version;
            var newUser = cache.TryGet("1.user.1");
            Assert.True(version == newUser.Version);

            newUser.Dic["a"] = "111111";
            newUser.Dic.Add("c", "ddddd");

            Assert.Equal(u.Telephone, newUser.Telephone);
            Assert.Equal(u.Mobile, newUser.Mobile);
            Assert.Equal(u.Email, newUser.Email);
            Assert.NotEqual(u.Dic.Count, newUser.Dic.Count);
            Assert.NotEqual(u.Dic["a"], newUser.Dic["a"]);
        }
    }
}
