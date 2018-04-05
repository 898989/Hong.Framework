using Hong.Repository.Internal;
using Hong.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hong.Model;
using Hong.Service.Objects;

namespace Hong.Repository
{
    public class OrderRepository : BaseRepository<OrderService, Order>
    {
        public List<OrderService> Find(string condition, string order, int pageIndex, int pageSize)
        {
            return null;
        }
    }
}
