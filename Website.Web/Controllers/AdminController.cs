using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Service.Interfaces;

namespace Website.Web.Controllers
{
    [Authorize(Roles = "admin, manager")]
    public class AdminController : Controller
    {
        private IClientService _clientService;

        public AdminController(IClientService clientService)
        {
            _clientService = clientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _clientService.GetUsersAsync();
            return View(users);
        }

        public IActionResult Items()
        {
            return View();
        }
    }
}