using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Service.Enums;
using Website.Service.Interfaces;
using Website.Service.Services;

namespace Website.Web.Controllers
{
    [Authorize(Roles = "admin, manager")]
    public class AdminController : Controller
    {
        private readonly IUserManager _userManager;

        public AdminController(IUserManager userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users(string sortOrder, string currentFilter, string searchString, int? page, int? count)
        {
            var users = await _userManager.GetUsersAsync(RoleSelector.Clients,0,10);

            return View(users);
        }

        public async Task<IActionResult> Items()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}