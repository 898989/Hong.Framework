using Hong.DAO.Core;
using ProtoBuf;
using System;

namespace Hong.Model
{
    [DataTable("t_products", "商品")]
    [ProtoContract]
    public class Product
    {
        /// <summary>编号
        /// </summary>
        [DBField("id", 0)]
        [ProtoMember(1)]
        public int ID;

        /// <summary>数据版本
        /// </summary>
        [DBField("version", 0)]
        [ProtoMember(2)]
        public int Version = 0;

        [DBField("name", 50)]
        [ProtoMember(3)]
        public string Name = string.Empty;

        [DBField("add_time", 0)]
        [ProtoMember(4)]
        public DateTime AddTime = DateTime.Now;

        [DBField("unit", 5)]
        [ProtoMember(5)]
        public string Unit = string.Empty;

        [DBField("small_image", 20)]
        [ProtoMember(6)]
        public string SmallImage = string.Empty;

        [DBField("big_image", 20)]
        [ProtoMember(7)]
        public string BigImage = string.Empty;

        [DBField("introduce", 0)]
        [ProtoMember(8)]
        public string Introduce = string.Empty;
    }
}
