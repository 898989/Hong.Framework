using Hong.Test.Model;
using Xunit;
using Hong.Common.Extendsion;
using System.Collections.Generic;
using Hong.Common.Tools;

namespace Hong.Test.Function
{
    public class TestProtoBuf
    {
        [Fact]
        public void Standard()
        {
            User u = new User();
            u.Mobile = "15999555555";
            u.Dic = new Dictionary<string, string>() {
                { "a","bbbb"},
                {"b","ccccccccccc" }
            };
            u.Type.Name = "初始化";

            var su = u.Serialize();
            var du = su.Deserialize(u.GetType()).TryToType(new User());

            Assert.Equal(u.Mobile, du.Mobile);
            Assert.Equal(u.Type.Name, du.Type.Name);

            du.Dic["a"] = "111111";
            du.Dic.Add("c", "ddddd");
            du.Type.Name = "测试";

            Assert.NotEqual(u.Dic["a"], du.Dic["a"]);
            Assert.NotEqual(u.Dic.Count, du.Dic.Count);
            Assert.NotEqual(u.Type.Name, du.Type.Name);
        }
    }
}
