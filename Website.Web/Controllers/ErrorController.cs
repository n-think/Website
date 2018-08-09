using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Website.Web.Controllers
{
    public class ErrorController : Controller
    {
        //если напрямую вызвали страницы ошибки переадресуем на 404
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (Response.StatusCode == 200)
            {
                Response.StatusCode = 404;
                context.Result = View("Error404");
            }
        }


        //если страница ошибки не определена то сюда
        [Route("/error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            return Content($"errorrrrrr {statusCode}");
            //return View(statusCode);
        }

        //если определена то ее

        [Route("/error/exception")]
        public IActionResult ErrorException()
        {
            //TODO LOG !
            return View("Error500");
        }

        [Route("/error/500")]
        public IActionResult Error500()
        {
            //log?
            return View();
        }


        [Route("/error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("/error/401")]
        public IActionResult Error401()
        {
            return View();
        }
    }
}