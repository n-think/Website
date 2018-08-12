using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Website.Web.Controllers
{
    public class ErrorController : Controller
    {
        //private ILogger _logger;

        ////если напрямую вызвали страницы ошибки переадресуем на 404, upd. не работает с 403
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    if (Response.StatusCode == 200)
        //    {
        //        Response.StatusCode = 404;
        //        context.Result = View("Error404");
        //    }
        //}

        //public ErrorController(ILogger logger)
        //{
        //    _logger = logger;
        //}

        //если страница ошибки не определена то сюда
        [HttpGet]
        [Route("/Error/{statusCode:int}")]
        public IActionResult ErrorDefault(int statusCode)
        {
            //return Content($"errorrrrrr {statusCode}");
            return View(statusCode);
        }

        //если определена то ее

        [HttpGet]
        [Route("/Error/Exception")]
        public IActionResult ErrorException()
        {
            //TODO LOG ?
            return View("Error");
        }

        [HttpGet]
        [Route("/Error/500")]
        public IActionResult Error500()
        {
            //log?
            return View();
        }

        [HttpGet]
        [Route("/Error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [HttpGet]
        [Route("/Error/403")]
        public IActionResult Error403()
        {
            Response.StatusCode = 403;
            return View();
        }

        [HttpGet]
        [Route("/Error/401")]
        public IActionResult Error401()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }
    }
}