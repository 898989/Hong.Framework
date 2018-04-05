using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hong.DAO;
using Hong.Common.Extendsion;
using Hong.Service.Repository;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hong.Base.API.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        OrderRepository _OrderRepository;
        public OrderController(OrderRepository orderRepository)
        {
            _OrderRepository = orderRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        public async Task<string> TestTran()
        {
            var id = await GetOrUpdateId();
            return id.ToString();
        }

        async Task<int> GetOrUpdateId()
        {
            using (var tran = DBFactory<Model.Order>.CreateInstance().CreateTransactionScope())
            {
                var order = await _OrderRepository.Get(1);
                order.UserID = 2;
                await _OrderRepository.Update(order);
                tran.Complete();

                return order.ID;
            }
        }

        [HttpGet]
        public async Task<string> TestSpeed()
        {
            var order = await _OrderRepository.Get(1);
            return order.ID.ToString();
        }
    }
}
