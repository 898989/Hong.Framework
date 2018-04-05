using Hong.Service.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WEB_NETCORE.Controllers
{
    [Route("/[action].html", Order = 0)]
    public class HomeController : Controller
    {
        OrderRepository _OrderRepository;
        public HomeController(OrderRepository orderRepository)
        {
            _OrderRepository = orderRepository;
        }

        public async Task<string> OrderID()
        {
            var order = await _OrderRepository.Get(1);
            return order.ID.ToString();
        }

        public async Task<string> Query()
        {
            var ids = await _OrderRepository.Query(@"select id from t_orders where user_id=@v0", new object[] { 1 });
            return ids.Count.ToString();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        ////public JsonResult Speed()
        ////{

        ////}

        ////public JsonResult Speed1()
        ////{

        ////}
    }
}
