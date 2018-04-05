using System;

namespace Hong.DAO.Core
{
    /// <summary>
    /// 数据库字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DBFieldAttribute : Attribute
    {
        /// <summary>初始化<see cref="DBFieldAttribute"/> 注释,,此方式自动判断数据类型,长度由数据库自动判断
        /// </summary>
        /// <param name="fileName">数据库字段名称</param>
        public DBFieldAttribute(string fileName)
        {
            this.FieldName = fileName;
        }

        /// <summary>初始化<see cref="DBFieldAttribute"/> 注释,此方式自动判断数据类型
        /// </summary>
        /// <param name="fileName">数据库字段名称</param>
        /// <param name="length">字段长度, =0时由数据库自动判断</param>
        public DBFieldAttribute(string fileName, int length)
        {
            this.FieldName = fileName;
            this.Length = length;
        }

        /// <summary>初始化<see cref="DBFieldAttribute"/> 注释,,此方式长度由数据库自动判断
        /// </summary>
        /// <param name="fileName">数据库字段名称</param>
        /// <param name="dbType">数据类型</param>
        public DBFieldAttribute(string fileName, System.Data.DbType dbType)
        {
            this.FieldName = fileName;
            this.DbType = dbType;
            this.DeclareDBType = true;
        }

        /// <summary>初始化<see cref="DBFieldAttribute"/> 注释
        /// </summary>
        /// <param name="fileName">数据库字段名称</param>
        /// <param name="length">字段长度, 为零时由数据库自动判断</param>
        /// <param name="dbType">数据类型</param>
        public DBFieldAttribute(string fileName, int length, System.Data.DbType dbType)
        {
            this.FieldName = fileName;
            this.Length = length;
            this.DbType = dbType;
            this.DeclareDBType = true;
        }

        /// <summary>数据表字段名称
        /// </summary>
        public string FieldName = string.Empty;

        /// <summary>字段长度,默认长度为零
        /// </summary>
        public int Length = 0;

        /// <summary>是否定义了数据库字段类型
        /// </summary>
        internal bool DeclareDBType = false;

        /// <summary>数据库字段类型,默认字符串
        /// </summary>
        public System.Data.DbType DbType = System.Data.DbType.String;
    }
}
