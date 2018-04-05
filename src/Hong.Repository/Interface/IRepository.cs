using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Repository
{
    public interface IRepository<Service>
    {
        Service FindByID(long id);
        List<Service> Find(long[] ids);

        List<Service> Find(string condition, string order, int pageIndex, int pageSize);
    }
}
