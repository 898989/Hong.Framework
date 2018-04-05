using Hong.DAO.Core;
using ProtoBuf;
using System;

namespace Hong.Model
{
    [DataTable("t_users", "会员")]
    public class User
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

        [DBField("account", 0)]
        [ProtoMember(10)]
        public string Account = string.Empty;

        [DBField("name", 20)]
        [ProtoMember(4)]
        public string Name = string.Empty;

        [DBField("add_time", 0)]
        [ProtoMember(5)]
        public DateTime AddTime = DateTime.Now;

        [DBField("mobile", 20)]
        [ProtoMember(6)]
        public string Mobile = string.Empty;

        [DBField("email", 20)]
        [ProtoMember(7)]
        public string Email = string.Empty;
    }
}
