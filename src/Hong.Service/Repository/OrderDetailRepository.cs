using System.Collections.Generic;
using Hong.Model;
using Hong.Service.Objects;
using Hong.Service.Internal;
using Hong.Repository;
using System.Threading.Tasks;

namespace Hong.Service.Repository
{
    public class OrderDetailRepository : BaseRepository<OrderDetailService, OrderDetail>, IRepository<OrderDetailService>
    {
        #region SQL命令

        /// <summary>通过订单ID获取订单明细
        /// </summary>
        internal const string QUERY_SQL_ByOrderID = "select id from t_order_details where order_id=@v0";

        #endregion

        public OrderDetailRepository() : base()
        {
        }

        public override async Task Add(List<OrderDetailService> service)
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

        public override async Task Add(OrderDetailService service)
        {
            service.ValidationAdd();
            await DAO.Model.Insert(service.DataEntity);
        }

        public override async Task Remove(List<OrderDetailService> service, bool activeNotifyEvent = true)
        {
            var models = new List<OrderDetail>();
            foreach (var item in service)
            {
                item.ValidationDelete();
                models.Add(item.DataEntity);
            }

            await DAO.Model.Delete(models);
        }

        public override async Task Remove(OrderDetailService service)
        {
            service.ValidationDelete();
            await DAO.Model.Delete(service.DataEntity);
        }

        public override async Task Update(List<OrderDetailService> service)
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

        public override async Task Update(OrderDetailService service)
        {
            service.ValidationUpdate();
            await DAO.Model.Update(service.DataEntity);
        }
    }
}
