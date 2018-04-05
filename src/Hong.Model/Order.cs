using Hong.DAO.Core;
using ProtoBuf;
using System;

namespace Hong.Model
{
    [DataTable("t_orders", "订单")]
    [ProtoContract]
    public class Order
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

        [DBField("add_time", 0)]
        [ProtoMember(3)]
        public DateTime AddTime = DateTime.Now;

        [DBField("user_id", 0)]
        [ProtoMember(4)]
        public int UserID;

        [DBField("identity", 20)]
        [ProtoMember(5)]
        public string Identity = string.Empty;

        [DBField("sum_money", System.Data.DbType.Decimal)]
        [ProtoMember(6)]
        public decimal SumMoney;

        [DBField("pay_money", System.Data.DbType.Decimal)]
        [ProtoMember(7)]
        public decimal PayMoney;

        [DBField("status", System.Data.DbType.Int16)]
        [ProtoMember(8)]
        public short Status;

        [DBField("recipient", 50)]
        [ProtoMember(9)]
        public string Recipient = string.Empty;

        [DBField("re_address", 200)]
        [ProtoMember(10)]
        public string RecipientAddress = string.Empty;

        [DBField("re_mobile", 20)]
        [ProtoMember(11)]
        public string RecipientMobile = string.Empty;

        [DBField("re_addr_country_id", 0)]
        [ProtoMember(12)]
        public int RecipientAddrCountryID;

        [DBField("re_addr_province_id", 0)]
        [ProtoMember(13)]
        public int RecipientAddrProvinceID;

        [DBField("re_addr_city_id", 0)]
        [ProtoMember(14)]
        public int RecipientAddrCityID;

        [DBField("re_addr_area_id", 0)]
        [ProtoMember(15)]
        public int RecipientAddrAreaID;

        [DBField("receiving_time", 30)]
        [ProtoMember(16)]
        public string ReceivingTime = string.Empty;

        [DBField("pay_time", System.Data.DbType.DateTime)]
        [ProtoMember(17)]
        public DateTime PayTime;

        [DBField("pay_id")]
        [ProtoMember(18)]
        public int PayID;

        [DBField("express_number", 25)]
        [ProtoMember(19)]
        public string ExpressNumber = string.Empty;
    }
}
