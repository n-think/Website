using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account.Manage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Data.EF.Models;
using Website.Service.Enums;
using Website.Service.Interfaces;

namespace Website.Web.Controllers
{
    [Authorize(Roles = "admin, manager")]
    public class AdminController : Controller
    {
        private readonly IClientService _clientService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IClientService clientService, UserManager<ApplicationUser> userManager)
        {
            _clientService = clientService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users(string sortOrder, string currentFilter, string searchString, int? page, int? count)
        {
            var users = await _clientService.GetUsersAsync(RoleSelector.Clients,0,10);

            return View(users);
        }

        public async Task<IActionResult> Items()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}