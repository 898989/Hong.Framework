using System.Collections.Generic;
using Hong.Model;
using Hong.Service.Objects;
using Hong.Service.Internal;
using System.Threading.Tasks;
using Hong.Repository;

namespace Hong.Service.Repository
{
    public class UserRepository : BaseRepository<UserService, User>, IRepository<UserService>
    {
        public UserRepository() : base()
        {
        }

        public override async Task Add(List<UserService> service)
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

        public override async Task Add(UserService service)
        {
            service.ValidationAdd();
            await DAO.Model.Insert(service.DataEntity);
        }

        public override async Task Remove(List<UserService> service, bool activeNotifyEvent = true)
        {
            var models = new List<User>();
            foreach (var item in service)
            {
                item.ValidationDelete();
                models.Add(item.DataEntity);
            }

            await DAO.Model.Delete(models);
        }

        public override async Task Remove(UserService service)
        {
            service.ValidationDelete();
            await DAO.Model.Delete(service.DataEntity);
        }

        public override async Task Update(List<UserService> service)
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

        public override async Task Update(UserService service)
        {
            service.ValidationUpdate();
            await DAO.Model.Update(service.DataEntity);
        }
    }
}
