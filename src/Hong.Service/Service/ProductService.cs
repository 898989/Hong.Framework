using Hong.Service.Internal;
using System;
using Hong.Model;
using ProtoBuf;

namespace Hong.Service.Objects
{
    [Cache.CacheSet("ProductService", "Product")]
    [ProtoContract]
    public class ProductService : BaseService<Product>
    {
        [ProtoMember(1)]
        internal Model.Product DataEntity;

        internal ProductService()
        {
        }
        public ProductService(Product dataEntity)
        {
            DataEntity = dataEntity;
        }

        public int ID
        {
            get
            {
                throw new NotImplementedException();
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

        public string Name
        {
            get { return DataEntity.Name; }
            set { DataEntity.Name = value; }
        }

        public DateTime AddTime
        {
            get { return DataEntity.AddTime; }
            set { DataEntity.AddTime = value; }
        }

        public string Unit
        {
            get { return DataEntity.Unit; }
            set { DataEntity.Unit = value; }
        }

        public string SmallImage
        {
            get { return DataEntity.SmallImage; }
            set { DataEntity.SmallImage = value; }
        }

        public string BigImage
        {
            get { return DataEntity.BigImage; }
            set { DataEntity.BigImage = value; }
        }

        public string Introduce
        {
            get { return DataEntity.Introduce; }
            set { DataEntity.Introduce = value; }
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
