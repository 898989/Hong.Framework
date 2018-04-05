using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hong.DAO.Core
{
    public class DBFieldHelper<Model>
    {
        public DataModelInfo<Model> GetModelInfo()
        {
            Type type = typeof(Model);

            DataTableAttribute dataTableAttribute = type.GetTypeInfo().GetCustomAttribute<DataTableAttribute>();
            if (dataTableAttribute == null)
            {
                throw new Exception("没有定义数据表名");
            }

            int index = 0;
            var propertys = type.GetTypeInfo().GetFields();
            var dbFields = new DBFieldAttribute[propertys.Length - 1];
            var proInfos = new FieldInfo[propertys.Length - 1];
            FieldInfo propertyInfo = null;

            foreach (var item in propertys)
            {
                if (item.Name == "ID")
                {
                    propertyInfo = item;
                    continue;
                }

                var attribe = item.GetCustomAttribute<DBFieldAttribute>();
                if (attribe == null)
                {
                    continue;
                }

                if (!attribe.DeclareDBType)
                {
                    attribe.DbType = ToDbType(item.FieldType);
                }

                proInfos[index] = item;
                dbFields[index++] = attribe;
            }

            return new DataModelInfo<Model>(
                dataTableAttribute.TableName,
                Array.FindAll(proInfos, (item) => { return item != null; }),
                Array.FindAll(dbFields, (item) => { return item != null; }),
                propertyInfo);
        }

        public System.Data.DbType ToDbType(Type type)
        {
            return (System.Data.DbType)Enum.Parse(typeof(System.Data.DbType), type.Name);
        }
    }
}
