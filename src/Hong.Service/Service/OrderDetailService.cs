using Hong.Service.Internal;
using System;
using Hong.Model;
using Hong.Common.Extendsion;
using Hong.Service.Repository;
using ProtoBuf;
using System.Threading.Tasks;

namespace Hong.Service.Objects
{
    [Cache.CacheSet("OrderDetailService", "OrderDetail")]
    [ProtoContract]
    public class OrderDetailService : BaseService<OrderDetail>
    {
        [ProtoMember(1)]
        internal Model.OrderDetail DataEntity;

        internal OrderDetailService()
        {
        }

        public OrderDetailService(OrderDetail dataEntity)
        {
            DataEntity = dataEntity;
        }

        public int ID
        {
            get
            {
                return DataEntity.ID;
            }
            internal set
            {
                DataEntity.ID = value;
            }
        }

        public int Version
        {
            get { return DataEntity.ID; }
            set { DataEntity.Version = value; }
        }

        public int OrderID
        {
            get { return DataEntity.OrderID; }
            set { DataEntity.OrderID = value; }
        }

        public int ProductID
        {
            get { return DataEntity.ProductID; }
            set { DataEntity.ProductID=value; }
        }

        public int Quatity
        {
            get { return DataEntity.Quatity; }
            set { DataEntity.Quatity = value; }
        }

        public decimal Price
        {
            get { return DataEntity.Price; }
            set { DataEntity.Price = value; }
        }

        public decimal PayPrice
        {
            get { return DataEntity.PayPrice; }
            set { DataEntity.PayPrice = value; }
        }

        public decimal Money
        {
            get { return DataEntity.Money; }
            set { DataEntity.Money = value; }
        }

        public decimal PayMoney
        {
            get { return DataEntity.PayMoney; }
            set { DataEntity.PayMoney = value; }
        }

        public async Task<ProductService> GetProduct()
        {
            return await ServiceProvider.GetService<ProductRepository>().Get(ProductID);
        }

        public override void ValidationAdd()
        {
            throw new NotImplementedException();
        }

        public override void ValidationDelete()
        {
            throw new NotImplementedException();
        }

        public override void ValidationUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
