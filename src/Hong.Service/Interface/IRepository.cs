using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hong.Repository
{
    public interface IRepository<Service>
    {
        Task<Service> Get(int id);

        Task<List<Service>> Get(List<int> ids);

        Task Add(Service service);
        Task Add(List<Service> service);

        Task Update(Service service);
        Task Update(List<Service> service);

        Task Remove(Service service);
        Task Remove(List<Service> service, bool activeNotifyEvent = true);

        Task<List<Service>> Query(string sql, params object[] arguments);
    }
}
