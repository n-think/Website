using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Web.Models.AdminViewModels;

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

        [HttpGet]
        public async Task<IActionResult> Users(
            [FromQuery(Name = "s")]string search,
            [FromQuery(Name = "st")]RoleSelector roles,
            [FromQuery(Name = "o")]string sortOrder,
            [FromQuery(Name = "p")]int? page,
            [FromQuery(Name = "c")]int? pageCount)
        {

            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(UserDTO.UserName);
            }

            ViewBag.currSearch = search;
            ViewBag.currSort = sortOrder;
            ViewBag.desc = sortOrder.EndsWith("_desc");
            ViewBag.currPage = page.HasValue ? page.Value : 1; // int not nullable
            ViewBag.countPerPage = pageCount ?? 30; // get from client js?
            ViewBag.roles = (int)roles;

            int pageN = ViewBag.currPage;
            var pageSize = ViewBag.countPerPage;
            SortPageResult<UserDTO> result = await _userManager.GetSortFilterPageAsync(roles, search, sortOrder, pageN, pageSize);
            
            ViewBag.itemCount = result.TotalN;

            return View(result.FilteredData);
        }

        public async Task<IActionResult> Items()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}