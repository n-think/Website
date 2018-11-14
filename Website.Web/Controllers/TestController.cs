using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Service.Enums;

namespace Website.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var claimList = HttpContext.User.Claims.ToList();
            return View(claimList);
        }

        public IActionResult Form(TestData asdddd)
        {
            var json = JsonConvert.SerializeObject(asdddd, Formatting.Indented);
            return View("Form",json);
        }

        public class TestData
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public List<Data> Datas { get; set; }
        }
        public class Data
        {
            public string DataName { get; set; }
            public List<IFormFile> Files { get; set; }
            public Data Nested { get; set; }
        }

        public IActionResult Calc()
        {
            return View();
        }
    }
}