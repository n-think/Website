using System.Linq;
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

        public IActionResult Calc()
        {
            return View();
        }
    }
}