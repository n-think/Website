using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Website.Web.Models;

namespace Website.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Test");
            //return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Страница описания.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Страница контактов.";

            return View();
        }
    }
}
