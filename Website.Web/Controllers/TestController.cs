using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Website.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var claimList = HttpContext.User.Claims.ToList();

            return View(claimList);
        }
    }

    public class TestModel
    {

    }
}