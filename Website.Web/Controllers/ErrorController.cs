﻿using Microsoft.AspNetCore.Mvc;
using Website.Web.Models;

namespace Website.Web.Controllers
{
    public class ErrorController : Controller
    {
        //public ErrorController(ILogger logger)
        //{
        //    _logger = logger;
        //}
        //
        //private ILogger _logger;
        
        [Route("/Error")]
        public IActionResult ErrorDefault()
        {
            return View("Error", new ErrorViewModel() { Message = "Произошла непредвиденная ошибка." });
        }
        
        [Route("/Error/500")]
        public IActionResult Error500()
        {
            return View("Error", new ErrorViewModel()
            {
                ErrorId = 500,
                Message = "Ошибка при выполнении запроса."
            });
        }

        [Route("/Error/404")]
        public IActionResult Error404()
        {
            return View("Error", new ErrorViewModel()
            {
                Header = "404 - Страница не найдена",
                ErrorId = 404,
                Message = "Запрашиваемая страница не найдена."
            });
        }

        [Route("/Error/403")]
        public IActionResult Error403()
        {
            Response.StatusCode = 403;
            return View("Error", new ErrorViewModel()
            {
                Header = "403 - Доступ запрещен",
                ErrorId = 403,
                Message = "У вас нет необходимых прав для выполнения этой операции."
            });
        }

        [Route("/Error/401")]
        public IActionResult Error401()
        {
            return View("Error", new ErrorViewModel()
            {
                ErrorId = 401,
                Message = "Необходимо авторизоваться."
            });
        }

        [Route("/Error/400")]
        public IActionResult Error400()
        {
            return View("Error", new ErrorViewModel()
            {
                ErrorId = 400,
                Message = "Получены некорректные данные."
            });
        }

        [HttpGet]
        public IActionResult ErrorLockout()
        {
            return View("Error", new ErrorViewModel()
            {
                Message = $"Аккаунт временно заблокирован в целях безопасности. Попробуйте снова через 5 минут."
            });
        }
    }
}