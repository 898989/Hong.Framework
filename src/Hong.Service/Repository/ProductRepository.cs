using System.Collections.Generic;
using Hong.Model;
using Hong.Service.Objects;
using Hong.Service.Internal;
using System.Threading.Tasks;
using Hong.Repository;

namespace Hong.Service.Repository
{
    public class ProductRepository : BaseRepository<ProductService, Product>, IRepository<ProductService>
    {
        public ProductRepository() : base()
        {
        }

        public override async Task Add(List<ProductService> service)
        {
            foreach (var item in service)
            {
                item.ValidationAdd();
            }

            using (var tran = DAO.CreateTransactionScope())
            {
                foreach (var item in service)
                {
                    await DAO.Model.Insert(item.DataEntity);
                }

                tran.Complete();
            }
        }

        public override async Task Add(ProductService service)
        {
            service.ValidationAdd();
            await DAO.Model.Insert(service.DataEntity);
        }

        public override async Task Remove(List<ProductService> service, bool activeNotifyEvent = true)
        {
            var models = new List<Product>();
            foreach (var item in service)
            {
                item.ValidationDelete();
                models.Add(item.DataEntity);
            }

            await DAO.Model.Delete(models);
        }

        public override async Task Remove(ProductService service)
        {
            service.ValidationDelete();
            await DAO.Model.Delete(service.DataEntity);
        }

        public override async Task Update(List<ProductService> service)
        {
            foreach (var item in service)
            {
                item.ValidationUpdate();
            }

            using (var tran = DAO.CreateTransactionScope())
            {
                foreach (var item in service)
                {
                    await DAO.Model.Update(item.DataEntity);
                }

                tran.Complete();
            }
        }

        public override async Task Update(ProductService service)
        {
            service.ValidationUpdate();
            await DAO.Model.Update(service.DataEntity);
        }
    }
}
