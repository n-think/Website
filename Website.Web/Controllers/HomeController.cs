using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Website.Service.DTO;
using Website.Service.Interfaces;
using Website.Service.Services;
using Website.Web.Models;
using Website.Web.Models.AccountViewModels;

namespace Website.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
