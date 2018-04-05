using Hong.DAO.Core;
using ProtoBuf;

namespace Hong.Model
{
    [DataTable("t_order_details", "订单明细")]
    [ProtoContract]
    public class OrderDetail
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

        [DBField("order_id", 0)]
        [ProtoMember(3)]
        public int OrderID;

        [DBField("product_id", 0)]
        [ProtoMember(4)]
        public int ProductID;

        [DBField("quatity", 0)]
        [ProtoMember(5)]
        public int Quatity = 0;

        [DBField("price", 0)]
        [ProtoMember(6)]
        public decimal Price = 0;

        [DBField("pay_price", 0)]
        [ProtoMember(7)]
        public decimal PayPrice = 0;

        [DBField("totall_money", 0)]
        [ProtoMember(8)]
        public decimal Money = 0;

        [DBField("pay_money", 0)]
        [ProtoMember(9)]
        public decimal PayMoney = 0;
    }
}
