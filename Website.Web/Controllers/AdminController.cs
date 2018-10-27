using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Services;
using Website.Web.Models;
using Website.Web.Models.AdminViewModels;

namespace Website.Web.Controllers
{
    [Authorize(Policy = "Administrators")]
    public class AdminController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly IdentityErrorDescriber _errorDescriber;
        private readonly SignInManager _signInManager;
        private readonly IShopManager _shopManager;

        public AdminController(IUserManager userManager, IShopManager shopManager, RoleManager roleManager, SignInManager signInManager, IMapper mapper, IdentityErrorDescriber describer = null)
        {
            _errorDescriber = describer ?? new IdentityErrorDescriber();
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _shopManager = shopManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ViewUsers")]
        public async Task<IActionResult> Users(
            [FromQuery(Name = "s")]string search,
            [FromQuery(Name = "st")]RoleSelector roles,
            [FromQuery(Name = "o")]string sortOrder,
            [FromQuery(Name = "p")]int? page,
            [FromQuery(Name = "c")]int? pageCount)
        {

            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(UserDto.Email);
            }

            var currPage = page == null || page < 0 ? 1 : page.Value;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;

            SortPageResult<UserDto> result = await _userManager.GetSortFilterPageAsync(roles, search, sortOrder, currPage, countPerPage);

            ViewBag.itemCount = result.TotalN;

            var model = new UsersViewModel()
            {
                CurrentSearch = search,
                CurrentSortOrder = sortOrder,
                Descending = sortOrder.EndsWith("_desc"),
                CurrentPage = currPage,
                CountPerPage = countPerPage,
                Roles = (int)roles,
                ItemCount = result.TotalN,
                Users = result.FilteredData
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "ViewUsers")]
        public async Task<IActionResult> ViewUser(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return RedirectToAction("Users");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Пользователь с id {id} не найден." });
            }
            var modelUser = _mapper.Map<UserViewModel>(user);
            modelUser.CurrentClaims = await _userManager.GetClaimsAsync(user);
            modelUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "без роли";
            return View(modelUser);
        }

        [HttpGet]
        [Authorize(Policy = "EditUsers")]
        public async Task<IActionResult> EditUser(string id)
        {
            //TODO get ViewUser EditUser на 90% одинаковые
            if (id.IsNullOrEmpty())
            {
                return RedirectToAction("Users");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Пользователь с id {id} не найден." });
            }
            var modelUser = _mapper.Map<EditUserViewModel>(user);
            modelUser.CurrentClaims = await _userManager.GetClaimsAsync(user);
            modelUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "без роли";
            return View(modelUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditUsers")]
        public async Task<IActionResult> EditUser(EditUserViewModel user)
        {
            if (user == null || user.Id == null || user.ConcurrencyStamp == null)
            {
                return RedirectToAction("Users");
            }

            if (ModelState.IsValid)
            {
                var dtoUser = await _userManager.FindByIdAsync(user.Id);
                if (dtoUser == null)
                {
                    return View("Error", new ErrorViewModel { Message = $"Пользователь с id {user.Id} не найден." });
                }
                var dbConcStamp = dtoUser.ConcurrencyStamp;
                _mapper.Map(user, dtoUser);

                var newClaims = new List<Claim>();
                if (!user.NewClaims.IsNullOrEmpty() && user?.Role == "admin")
                {
                    newClaims = user.NewClaims.Select(x => new Claim(x, "")).ToList();
                }
                if (!user.Role.IsNullOrEmpty())
                {
                    newClaims.Add(new Claim(ClaimTypes.Role, user.Role));
                }
                //try update
                var result = await _userManager.UpdateUserPasswordClaims(dtoUser, user.Password, newClaims);

                if (result.Succeeded) // update successful
                {
                    if (HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value == dtoUser.Id)
                    {
                        await _signInManager.RefreshSignInAsync(dtoUser);
                    }
                    else
                    {
                        await _userManager.UpdateSecurityStampAsync(dtoUser);
                    }
                    TempData["Message"] = "Изменения успешно сохранены";
                    return RedirectToAction("ViewUser", new { id = user.Id });
                }

                //add new errors
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError(identityError.Code, identityError.Description);
                    if (identityError.Code == "ConcurrencyFailure")
                    {
                        user.ConcurrencyStamp = dbConcStamp; // to enable save after concurrency error
                        ModelState.Remove("ConcurrencyStamp"); // remove from the model state or HTML helpers will use the original value
                    }
                }
            }
            return View(user);
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Items(
            [FromQuery(Name = "s")]string search,
            [FromQuery(Name = "st")]ItemTypeSelector types,
            [FromQuery(Name = "o")]string sortOrder,
            [FromQuery(Name = "p")]int? page,
            [FromQuery(Name = "c")]int? pageCount)
        {
            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(ProductDto.Name);
            }

            var currPage = page ?? 1;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;

            SortPageResult<ProductDto> result = await _shopManager.GetSortFilterPageAsync(types, search, sortOrder, currPage, countPerPage);
            //TODO categories filter List<CategoryDTO> allCategories = await _shopManager.GetAllCategories();
            ViewBag.itemCount = result.TotalN;

            var model = new ItemsViewModel()
            {
                CurrentSearch = search,
                CurrentSortOrder = sortOrder,
                Descending = sortOrder.EndsWith("_desc"),
                CurrentPage = currPage,
                CountPerPage = countPerPage,
                Types = (int)types,
                ItemCount = result.TotalN,
                Items = result.FilteredData
                //,Categories = allCategories
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> ViewItem(int id)
        {
            var prod = await _shopManager.GetProductById(id);
            if (prod == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Товар с id {id} не найден." });
            }
            var viewModel = _mapper.Map<ItemViewModel>(prod);
            viewModel.Descriptions = await _shopManager.GetProductDescriptions(id);
            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = "EditItems")]
        public async Task<IActionResult> EditItem(int id)
        {
            var prod = await _shopManager.GetProductById(id);
            if (prod == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Товар с id {id} не найден." });
            }

            var viewModel = _mapper.Map<EditItemViewModel>(prod);
            viewModel.Descriptions = await _shopManager.GetProductDescriptions(id);
            return View(viewModel);
        }

        [HttpPost("Admin/EditItem")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditUsers")]
        public async Task<IActionResult> EditItem([FromBody]EditItemViewModel item)
        {
            if (item == null || item.Id == 0 || item.Timestamp == null)
            {
                return BadRequest("Введены некорректные данные.");
            }

            if (ModelState.IsValid)
            {
                //var dtoUser = await _userManager.FindByIdAsync(user.Id);
                //if (dtoUser == null)
                //{
                //    return View("Error", new ErrorViewModel { Message = $"Пользователь с id {user.Id} не найден." });
                //}
                //var dbConcStamp = dtoUser.ConcurrencyStamp;
                //_mapper.Map(user, dtoUser);

                //var newClaims = new List<Claim>();
                //if (!user.NewClaims.IsNullOrEmpty() && user?.Role == "admin")
                //{
                //    newClaims = user.NewClaims.Select(x => new Claim(x, "")).ToList();
                //}
                //if (!user.Role.IsNullOrEmpty())
                //{
                //    newClaims.Add(new Claim(ClaimTypes.Role, user.Role));
                //}
                ////try update
                //var result = await _userManager.UpdateUserPasswordClaims(dtoUser, user.Password, newClaims);

                //if (result.Succeeded) // update successful
                //{
                //    if (HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value == dtoUser.Id)
                //    {
                //        await _signInManager.RefreshSignInAsync(dtoUser);
                //    }
                //    TempData["Message"] = "Изменения успешно сохранены";
                //    return RedirectToAction("ViewUser", new { id = user.Id });
                //}

                ////add new errors
                //foreach (var identityError in result.Errors)
                //{
                //    ModelState.AddModelError(identityError.Code, identityError.Description);
                //    if (identityError.Code == "ConcurrencyFailure")
                //    {
                //        user.ConcurrencyStamp = dbConcStamp; // to enable save after conc error
                //        ModelState.Remove("ConcurrencyStamp"); // remove from the model state or HTML helpers will use the original value
                //    }
                //}
            }
            return PartialView(item);
        }

        [HttpGet]
        [Authorize(Policy = "CreateItems")]
        public async Task<IActionResult> AddItem()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ViewOrders")]
        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult Categories()
        {
            return View();
        }
    }
}