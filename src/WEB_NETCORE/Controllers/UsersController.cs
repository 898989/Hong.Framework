using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Hong.Base.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        //static ICacheManager<UserModel> cache = CacheFactory.CreateCacheManager<UserModel>();
        // GET api/values
        [HttpGet]
        public string Test()
        {
            //UserModel user = new UserModel();
            //user.ID = 2;
            //user.Set(nameof(user.Name), "test2");
            //user.Set(nameof(user.Age), 20);
            //user.Set(nameof(user.description), "测试22");

            DateTime t = DateTime.Now;
            ////Hong.DAO.DBFactory.Model.Update(user);
            //var s = (DateTime.Now - t).TotalMilliseconds;
            //DBFactory<UserModel> dbFactory = new DBFactory<UserModel>();
            bool hasCache = false;
            //t = DateTime.Now;
            //for (var i = 0; i < 100; i++)
            //{
            //    var u = cache.TryGet("UserModel.1");
            //    if (u == null)
            //    {
            //        u = new UserModel();
            //        u.ID = 2;
            //        dbFactory.Model.Load(u);
            //    }
            //    else hasCache = true;

            //    cache.TrySet("UserModel.1", u);
            //}

            return "用时:" + (DateTime.Now - t).TotalMilliseconds + "|有缓存:" + hasCache;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
