using Hong.Service.Internal;
using System;
using System.Collections.Generic;
using Hong.Service.Repository;
using Hong.Common.Extendsion;
using System.ComponentModel;
using ProtoBuf;
using System.Threading.Tasks;

namespace Hong.Service.Objects
{
    [Cache.CacheSet("OrderService", "Order")]
    [ProtoContract]
    public class OrderService : BaseService<Model.Order>
    {
        [ProtoMember(1)]
        internal Model.Order DataEntity;

        public int ID
        {
            get { return DataEntity.ID; }
            internal set { DataEntity.ID = value; }
        }

        public int Version
        {
            get { return DataEntity.Version; }
            set { DataEntity.Version = value; }
        }

        public DateTime AddTime
        {
            get { return DataEntity.AddTime; }
            set { DataEntity.AddTime = value; }
        }

        public int UserID
        {
            get { return DataEntity.UserID; }
            set { DataEntity.UserID = value; }
        }

        public string Identity
        {
            get { return DataEntity.Identity; }
            set { DataEntity.Identity = value; }
        }

        public decimal SumMoney
        {
            get { return DataEntity.SumMoney; }
            set { DataEntity.SumMoney = value; }
        }

        public decimal PayMoney
        {
            get { return DataEntity.PayMoney; }
            set { DataEntity.PayMoney = value; }
        }

        public OrderStatus Status
        {
            get { return (OrderStatus)DataEntity.Status; }
            set { DataEntity.Status = (short)value; }
        }

        public string Recipient
        {
            get { return DataEntity.Recipient; }
            set { DataEntity.Recipient = value; }
        }

        public string RecipientAddress
        {
            get { return DataEntity.RecipientAddress; }
            set { DataEntity.RecipientAddress = value; }
        }

        public string RecipientMobile
        {
            get { return DataEntity.RecipientMobile; }
            set { DataEntity.RecipientMobile = value; }
        }

        public int RecipientAddrCountryID
        {
            get { return DataEntity.RecipientAddrCountryID; }
            set { DataEntity.RecipientAddrCountryID = value; }
        }

        public int RecipientAddrProvinceID
        {
            get { return DataEntity.RecipientAddrProvinceID; }
            set { DataEntity.RecipientAddrProvinceID = value; }
        }

        public int RecipientAddrCityID
        {
            get { return DataEntity.RecipientAddrCityID; }
            set { DataEntity.RecipientAddrCityID = value; }
        }

        public int RecipientAddrAreaID
        {
            get { return DataEntity.RecipientAddrAreaID; }
            set { DataEntity.RecipientAddrAreaID = value; }
        }

        public string ReceivingTime
        {
            get { return DataEntity.ReceivingTime; }
            set { DataEntity.ReceivingTime = value; }
        }

        public DateTime PayTime
        {
            get { return DataEntity.PayTime; }
            set { DataEntity.PayTime = value; }
        }

        public int PayID
        {
            get { return DataEntity.PayID; }
            set { DataEntity.PayID = value; }
        }

        public string ExpressNumber
        {
            get { return DataEntity.ExpressNumber; }
            set { DataEntity.ExpressNumber = value; }
        }

        public async Task<UserService> GetUser()
        {
            return await ServiceProvider.GetService<UserRepository>().Get(UserID);
        }

        public List<OrderDetailService> details = null;
        public async Task<List<OrderDetailService>> GetDetails()
        {
            if (details == null)
            {
                if (ID < 1)
                    details = new List<OrderDetailService>();
                else
                    details = await ServiceProvider.GetService<OrderDetailRepository>().Query(OrderDetailRepository.QUERY_SQL_ByOrderID, new object[] { ID });
            }

            return details;
        }

        internal OrderService()
        {
        }

        public OrderService(Model.Order dataEntity)
        {
            DataEntity = dataEntity;
        }

        public async Task AddDetails(OrderDetailService newDetail)
        {
            var details = await GetDetails();
            details.Add(newDetail);
        }
        public async Task AddDetails(List<OrderDetailService> newDetails)
        {
            var details = await GetDetails();
            foreach (var item in newDetails) details.Add(item);
        }

        public override void ValidationAdd()
        {
        }

        public override void ValidationUpdate()
        {
        }

        public override void ValidationDelete()
        {
        }
    }

    public enum OrderStatus
    {
        [Description("待付款")]
        WaitPay = 0,
        [Description("已付款")]
        Payed = 1,
        [Description("待发货")]
        WaitDeliverGoods = 2,
        [Description("待收货")]
        WaitingForTheGoods = 3
    }

}
