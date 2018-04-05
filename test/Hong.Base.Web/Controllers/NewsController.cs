using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Hong.Base.Web.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Detail(long id)
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Update(long id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }
    }
}
