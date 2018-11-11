using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Website.Data.EF.Migrations;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Services;
using Website.Web.Models;
using Website.Web.Models.AdminViewModels;

namespace Website.Web.Controllers
{
    //[RequireHttps]
    [Route("[controller]/[action]")]
    [Authorize(Policy = "Administrators")]
    public class AdminController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly SignInManager _signInManager;
        private readonly IShopManager _shopManager;

        public AdminController(IUserManager userManager, IShopManager shopManager, RoleManager roleManager, SignInManager signInManager,
            IMapper mapper, IAntiforgery antiForgery)
        {
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

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ViewUsers")]
        public async Task<IActionResult> ViewUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Пользователь с id {id} не найден." });
            }
            var modelUser = _mapper.Map<UserViewModel>(user);
            modelUser.CurrentClaims = await _userManager.GetClaimsAsync(user);
            modelUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "без роли";
            return View(modelUser);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "EditUsers")]
        public async Task<IActionResult> EditUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
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
        public async Task<IActionResult> EditUser(EditUserViewModel editModel)
        {
            if (editModel?.Id == null || editModel.ConcurrencyStamp == null)
            {
                return RedirectToAction("Users");
            }

            if (ModelState.IsValid)
            {
                var dtoUser = await _userManager.FindByIdAsync(editModel.Id);
                if (dtoUser == null)
                {
                    return View("Error", new ErrorViewModel { Message = $"Пользователь с id {editModel.Id} не найден." });
                }
                var dbConcStamp = dtoUser.ConcurrencyStamp;
                _mapper.Map(editModel, dtoUser);

                var newClaims = new List<Claim>();
                if (!editModel.NewClaims.IsNullOrEmpty() && editModel.Role == "admin")
                {
                    newClaims = editModel.NewClaims.Select(x => new Claim(x, "")).ToList();
                }
                if (!editModel.Role.IsNullOrEmpty())
                {
                    newClaims.Add(new Claim(ClaimTypes.Role, editModel.Role));
                }
                //try update
                var result = await _userManager.UpdateUserPasswordClaims(dtoUser, editModel.Password, newClaims);
                if (result.Succeeded)
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
                    return RedirectToAction("ViewUser", new { id = editModel.Id });
                }
                //add new errors
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError(identityError.Code, identityError.Description);
                    if (identityError.Code == "ConcurrencyFailure")
                    {
                        editModel.ConcurrencyStamp = dbConcStamp; // to enable save after concurrency error
                        ModelState.Remove("ConcurrencyStamp"); // remove from the model state or HTML helpers will use the original value
                    }
                }
            }
            return View(editModel);
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
            //TODO categories filter // List<CategoryDTO> allCategories = await _shopManager.GetAllCategoriesAsync();
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

        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> ViewItem(int id)
        {
            var prod = await _shopManager.GetProductByIdAsync(id, true, true, true);
            if (prod == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Товар с id {id} не найден." });
            }
            var viewModel = _mapper.Map<ItemViewModel>(prod);
            return View("ViewItem", viewModel);
        }

        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "EditItems")]
        public async Task<IActionResult> EditItem(int id)
        {
            var prod = await _shopManager.GetProductByIdAsync(id, true, true, true);
            if (prod == null)
            {
                return View("Error", new ErrorViewModel { Message = $"Товар с id {id} не найден." });
            }

            var viewModel = _mapper.Map<EditItemViewModel>(prod);
            viewModel.Descriptions = prod.Descriptions;
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditItems")]
        public async Task<IActionResult> EditItem([FromBody]EditItemViewModel item) //json input
        {
            if (item?.Id == null || item?.Name == null || item.Timestamp == null)
            {
                return BadRequest("Введены некорректные данные.");
            }
            if (ModelState.IsValid)
            {
                OperationResult result;
                ProductDto updatedProductDto;

                var itemDto = _mapper.Map<ProductDto>(item);
                if (item.Id.Value == -1)
                {
                    result = await _shopManager.CreateProductAsync(itemDto);
                    updatedProductDto = await _shopManager.GetProductByNameAsync(itemDto.Name, true, true, true);
                }
                else
                {
                    result = await _shopManager.UpdateProductAsync(itemDto);
                    updatedProductDto = await _shopManager.GetProductByIdAsync(itemDto.Id.GetValueOrDefault(), true, true, true);
                }
                if (result.Succeeded)
                {
                    TempData["Message"] = "Изменения успешно сохранены";
                    var itemViewModel = _mapper.Map<ItemViewModel>(updatedProductDto);
                    return PartialView("ViewItem", itemViewModel);

                }
                //add new errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    if (error.Code == nameof(OperationErrorDescriber.ConcurrencyFailure) || error.Code == nameof(OperationErrorDescriber.InvalidImageFormat))
                    {
                        item.Timestamp = updatedProductDto?.Timestamp ?? item.Timestamp; ; // to enable save after conc error or incomplete updates
                        ModelState.Remove("Timestamp"); // remove from the model state or HTML helpers will use the original value
                    }
                }
            }
            return PartialView(item);
        }

        [HttpGet]
        [Authorize(Policy = "EditItems")]
        public IActionResult AddItem()
        {
            return View("EditItem", new EditItemViewModel { CreateItem = true });
        }

        [HttpGet]
        [Authorize(Policy = "EditItems")]
        public IActionResult DeleteItem(DeleteItemModel model, string deleteToken) //token = 1 use token
        {
            if (model?.Id == null || deleteToken == null || deleteToken != TempData["DeleteToken"]?.ToString())
            {
                return View("Error", new ErrorViewModel { Message = $"Переход на страницу удаления товара возможен только после его просмотра." });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditItems")]
        public async Task<IActionResult> DeleteItemConfirm(int? id, string token) //token = 1 use token
        {
            if (id == null || token == null || token != TempData["DeleteToken"]?.ToString())
            {
                return View("Error", new ErrorViewModel { Message = $"Получены некорректные данные." });
            }
            var itemToDelete = await _shopManager.GetProductByIdAsync(id.Value);
            if (itemToDelete == null)
            {
                ViewData["message"] = "Товар с таким ID не найден или уже удален.";
            }
            var result = await _shopManager.DeleteProductAsync(itemToDelete);
            if (!result.Succeeded)
            {
                ViewData["message"] = result.Errors?.FirstOrDefault()?.Description;
            }
            else
            {
                ViewData["message"] = "Товар успешно удален.";
            }
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ViewOrders")]
        public IActionResult Orders()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Categories()
        {
            var cats = await _shopManager.GetAllCategoriesAsync(true);
            return View(cats.ToTree());
        }

        [HttpPost]
        [Authorize(Policy = "EditItems")]
        public IActionResult EditCategories()
        {
            return Ok();
        }

        public IActionResult DescriptionGroups()
        {
            return View();
        }
    }
}