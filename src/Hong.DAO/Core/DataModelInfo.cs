using System;
using System.Reflection;

namespace Hong.DAO.Core
{
    /// <summary>
    /// 数据实体信息
    /// </summary>
    /// <typeparam name="Model"></typeparam>
    public class DataModelInfo<Model>
    {
        /// <summary>初始化<see cref="DataModelInfo"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="modelFeildInfos"></param>
        /// <param name="dbFeilds"></param>
        /// <param name="idProperty"></param>
        public DataModelInfo(string tableName, FieldInfo[] modelFeildInfos, DBFieldAttribute[] dbFeilds, FieldInfo idProperty)
        {
            TableName = tableName;
            DBFeilds = dbFeilds;

            var config = DataConfigurationManager.Configuration();
            TableNameInSQL = config.KeywordFrontChar + tableName + config.KeywordEndChar;

            InitSelectFields(config);
            InitSelectSQL(config);
            InitInsertSQL(config);
            InitUpdateSQL(config);
            InitDeleteSQL(config);
            InitPropertys(modelFeildInfos, idProperty);
            InitCreateMethod();
        }

        void InitSelectFields(DataConfiguration config)
        {
            var sb = new System.Text.StringBuilder();

            foreach (DBFieldAttribute f in DBFeilds)
            {
                sb.Append(config.KeywordFrontChar).Append(f.FieldName).Append(config.KeywordEndChar).Append(",");
            }

            SelectFields = sb.Remove(sb.Length - 1, 1).ToString();
            sb = null;
        }

        void InitSelectSQL(DataConfiguration config)
        {
            var sb = new System.Text.StringBuilder();

            sb.Append("select ").Append(SelectFields)
                .Append(" from ")
                .Append(TableNameInSQL)
                .Append(" where id=");

            SelectSQL = sb.ToString();
            sb = null;
        }

        void InitInsertSQL(DataConfiguration config)
        {
            var sb = new System.Text.StringBuilder();

            sb.Append("insert into ")
                .Append(TableNameInSQL)
                .Append(" (");

            foreach (DBFieldAttribute f in DBFeilds)
            {
                sb.Append(config.KeywordFrontChar).Append(f.FieldName).Append(config.KeywordEndChar)
                    .Append(",");
            }
            sb.Remove(sb.Length - 1, 1).Append(") values (");

            for (var index = 0; index < DBFeilds.Length; index++)
            {
                sb.Append(config.ParamerNameStr).Append(index)
                    .Append(",");
            }
            sb.Remove(sb.Length - 1, 1).Append(")");

            if (config.DataBase == "mysql")
            {
                sb.Append(";select LAST_INSERT_ID();");
            }
            else if (config.DataBase == "sqlite")
            {
                sb.Append(";select last_insert_rowid();");
            }
            else if (config.DataBase == "mssql")
            {
                sb.Append(";select SCOPE_IDENTITY();");
            }

            InsertSQL = sb.ToString();
            sb = null;
        }

        void InitUpdateSQL(DataConfiguration config)
        {
            var index = 0;
            var sb = new System.Text.StringBuilder();

            sb.Append("update ")
                .Append(TableNameInSQL)
                .Append(" set ");

            foreach (DBFieldAttribute f in DBFeilds)
            {
                sb.Append(config.KeywordFrontChar).Append(f.FieldName).Append(config.KeywordEndChar)
                    .Append("=")
                    .Append(config.ParamerNameStr).Append(index++)
                    .Append(",");
            }

            sb.Remove(sb.Length - 1, 1)
                .Append(" where id=");

            UpdateSQL = sb.ToString();
            sb = null;
        }

        void InitDeleteSQL(DataConfiguration config)
        {
            DeleteSQL = string.Format("delete from {0} where id=", TableNameInSQL);
        }

        void InitPropertys(FieldInfo[] modelFeildInfos, FieldInfo idProperty)
        {
            Func<Model, object>[] getfunc = new Func<Model, object>[modelFeildInfos.Length];
            Action<Model, object>[] setfunc = new Action<Model, object>[modelFeildInfos.Length];
            int index = 0;

            foreach (FieldInfo item in modelFeildInfos)
            {
                if (item.Name == "Version")
                {
                    VersionIncrement = Common.Extendsion.Reflection.Increment<Model>(item);
                    GetVersion = Common.Extendsion.Reflection.GetField<Model, int>(item);
                    //获取版本时自动加1
                    getfunc[index] = Common.Extendsion.Reflection.GetFieldAndIncrement<Model, object>(item);
                }
                else
                {
                    getfunc[index] = Common.Extendsion.Reflection.GetField<Model, object>(item);
                }

                setfunc[index++] = Common.Extendsion.Reflection.SetField<Model, object>(item);
            }

            GetFields = getfunc;
            SetFields = setfunc;
            GetID = Common.Extendsion.Reflection.GetField<Model, int>(idProperty);
            SetID = Common.Extendsion.Reflection.SetField<Model, int>(idProperty);
        }

        void InitCreateMethod()
        {
            CreateInstance = Common.Extendsion.Reflection.CreateInstance<Model>();
        }

        /// <summary>数据表字段
        /// </summary>
        public DBFieldAttribute[] DBFeilds;

        /// <summary>数据表名称
        /// </summary>
        public string TableName;

        /// <summary>数据表名称用组合SQL语句
        /// </summary>
        public string TableNameInSQL;

        /// <summary>获取字段值
        /// </summary>
        public Func<Model, object>[] GetFields;

        /// <summary>设置字段值
        /// </summary>
        public Action<Model, object>[] SetFields;

        /// <summary>查询字段串
        /// </summary>
        public string SelectFields;

        /// <summary>单行查询SQL语句
        /// </summary>
        public string SelectSQL;

        /// <summary>添加SQL语句
        /// </summary>
        public string InsertSQL;

        /// <summary>更新SQL语句
        /// </summary>
        public string UpdateSQL;

        /// <summary>删除语句
        /// </summary>
        public string DeleteSQL;

        /// <summary>获取ID值
        /// </summary>
        public Func<Model, int> GetID;

        /// <summary>设置ID值
        /// </summary>
        public Action<Model, int> SetID;

        /// <summary>获取原始版本号
        /// </summary>
        public Func<Model, int> GetVersion;

        /// <summary>创建实例
        /// </summary>
        public Func<Model> CreateInstance;

        /// <summary>版本自增1
        /// </summary>
        public Action<Model> VersionIncrement;
    }
}
