using Hong.DAO;
using Hong.DAO.Core;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Hong.Test.Function
{
    public class TestDAO
    {
        [Fact]
        public void Insert()
        {
            Hong.Model.Order order = new Hong.Model.Order();
            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;
            order.UserID = 10;
            dbFactory.Insert(order);
            order.UserID = 11;
            dbFactory.Load(order);
            Assert.Equal(order.UserID, 10);
        }

        [Fact]
        public void Update()
        {
            Hong.Model.Order order = new Hong.Model.Order()
            {
                ID = 1
            };

            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance().Model;

            dbFactory.Load(order);
            order.AddTime = DateTime.Now;
            var version = order.Version;

            dbFactory.Update(order);

            Assert.Equal(order.Version, version + 1);

            order.Version--;
            try
            {
                dbFactory.Update(order);
            }
            catch (VersionException)
            {
            }
            catch
            {
                throw;
            }
        }

        [Fact]
        public async void Transaction1()
        {
            int orderId = 1;
            Hong.Model.Order order = new Hong.Model.Order()
            {
                ID = orderId
            };

            var dbFactory = DBFactory<Hong.Model.Order>.CreateInstance();
            await dbFactory.Model.Load(order);
            int userId = order.UserID;

            try
            {
                await TranUpdateError(dbFactory, orderId);
            }
            catch
            {
            }

            await dbFactory.Model.Load(order);
            Assert.Equal(userId, order.UserID);

            await TranUpdateOK(dbFactory, orderId);

            Assert.NotEqual(userId, order.UserID);

            try
            {
                using (var tran = dbFactory.CreateTransactionScope())
                {
                    await TranUpdateOK(dbFactory, orderId);
                    await TranUpdateError(dbFactory, orderId);
                }
            }
            catch { }

            Assert.Equal(userId, order.UserID);
        }

        async Task TranUpdateOK(DBFactory<Hong.Model.Order> dbFactory, int orderId)
        {
            Hong.Model.Order order = new Hong.Model.Order()
            {
                ID = orderId
            };

            using (var tran = dbFactory.CreateTransactionScope())
            {
                await dbFactory.Model.Load(order);
                order.UserID = 100084;
                await dbFactory.Model.Update(order);
                tran.Complete();
            }
        }

        async Task TranUpdateError(DBFactory<Hong.Model.Order> dbFactory, int orderId)
        {
            Hong.Model.Order order = new Hong.Model.Order()
            {
                ID = orderId
            };

            using (var tran = dbFactory.CreateTransactionScope())
            {
                await dbFactory.Model.Load(order);
                order.UserID = 100083;
                await dbFactory.Model.Update(order);
                throw new Exception("异常测试");
            }
        }
    }
}
