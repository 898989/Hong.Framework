using System;

namespace Hong.DAO.Core
{
    /// <summary>
    /// 数据表属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DataTableAttribute : Attribute
    {
        /// <summary>DataTable构建
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        public DataTableAttribute(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>DataTable构建
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="description">描述</param>
        public DataTableAttribute(string tableName, string description)
        {
            this.TableName = tableName;
            this.Description = description;
        }

        /// <summary>数据表名称
        /// </summary>
        public string TableName = string.Empty;

        /// <summary>描述
        /// </summary>
        public string Description = string.Empty;
    }
}
