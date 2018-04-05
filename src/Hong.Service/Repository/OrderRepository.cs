using System.Collections.Generic;
using Hong.Model;
using Hong.Service.Objects;
using Hong.Service.Internal;
using Hong.DAO;
using System.Threading.Tasks;
using Hong.Repository;

namespace Hong.Service.Repository
{
    public class OrderRepository : BaseRepository<OrderService, Order>, IRepository<OrderService>
    {
        OrderDetailRepository OrderDetailRepository;

        public OrderRepository(OrderDetailRepository orderDetailRepository) : base()
        {
            OrderDetailRepository = orderDetailRepository;
        }

        public override async Task Add(List<OrderService> service)
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
                    await OrderDetailRepository.Add(await item.GetDetails());
                }

                tran.Complete();
            }
        }

        public override async Task Add(OrderService service)
        {
            service.ValidationAdd();

            using (var tran = DAO.CreateTransactionScope())
            {
                await DAO.Model.Insert(service.DataEntity);
                await OrderDetailRepository.Add(await service.GetDetails());

                tran.Complete();
            }
        }

        public override async Task Remove(List<OrderService> service, bool activeNotifyEvent = true)
        {
            var models = new List<Order>();
            foreach (var item in service)
            {
                item.ValidationDelete();
                models.Add(item.DataEntity);
            }

            await DAO.Model.Delete(models);
        }

        public override async Task Remove(OrderService service)
        {
            service.ValidationDelete();

            using (var tran = DAO.CreateTransactionScope())
            {
                await DAO.Model.Delete(service.DataEntity);
                await OrderDetailRepository.Remove(await service.GetDetails());

                tran.Complete();
            }
        }

        public override async Task Update(List<OrderService> service)
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
                    await OrderDetailRepository.Update(await item.GetDetails());
                }

                tran.Complete();
            }
        }

        public override async Task Update(OrderService service)
        {
            service.ValidationUpdate();

            using (var tran = DAO.CreateTransactionScope())
            {
                await DAO.Model.Update(service.DataEntity);
                await OrderDetailRepository.Update(await service.GetDetails());

                tran.Complete();
            }
        }
    }
}
