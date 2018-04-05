using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Hong.Test
{
    public class PropertyAttributeTest
    {
        [Fact]
        public void Standard()
        {
            Hong.Model.Order order = new Hong.Model.Order();
            MustPropertyTest(order.GetType());
        }

        /// <summary>必须属性测试
        /// </summary>
        /// <param name="type"></param>
        void MustPropertyTest(Type type)
        {
            var propertyInfo = type.GetProperty("ID");
            Assert.Null(propertyInfo);
            MustAttributeTest(propertyInfo);

            propertyInfo = type.GetProperty("Version");
            Assert.Null(propertyInfo);
            MustAttributeTest(propertyInfo);
        }

        /// <summary>必须属性注释测试
        /// </summary>
        /// <param name="propertyInfo"></param>
        void MustAttributeTest(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(true);
            Assert.True(attributes.Length > 1);

            var dbfiedAttribute = attributes.Where((object item) => { return item is DAO.Core.DBFieldAttribute; });
            Assert.True(dbfiedAttribute.Count() == 1);

            var protoBufAttribute = attributes.Where((object item) => { return item is ProtoBuf.ProtoMemberAttribute; });
            Assert.True(protoBufAttribute.Count() == 1);
        }
    }
}
