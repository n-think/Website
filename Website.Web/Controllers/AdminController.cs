using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Services.Services;
using Website.Web.Infrastructure.TreeHelper;
using Website.Web.Models;
using Website.Web.Models.AdminViewModels;
using Website.Web.Models.DTO;

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

        public AdminController(IUserManager userManager, IShopManager shopManager, RoleManager roleManager,
            SignInManager signInManager,
            IMapper mapper)
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
            [FromQuery(Name = "s")] string search,
            [FromQuery(Name = "st")] RoleSelector roles,
            [FromQuery(Name = "o")] string sortOrder,
            [FromQuery(Name = "p")] int? page,
            [FromQuery(Name = "c")] int? pageCount)
        {
            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(UserDto.Email);
            }

            var currPage = page == null || page < 0 ? 1 : page.Value;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;

            SortPageResult<User> result =
                await _userManager.GetSortFilterPageAsync(roles, search, sortOrder, currPage, countPerPage);

            ViewBag.itemCount = result.TotalN;

            //TODO use mapper
            var model = new UsersViewModel()
            {
                CurrentSearch = search,
                CurrentSortOrder = sortOrder,
                Descending = sortOrder.EndsWith("_desc"),
                CurrentPage = currPage,
                CountPerPage = countPerPage,
                Roles = (int) roles,
                ItemCount = result.TotalN,
                Users = _mapper.Map<IEnumerable<UserDto>>(result.FilteredData)
            };
            return View(model);
        }

        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "ViewUsers")]
        public async Task<IActionResult> ViewUser(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Пользователь с id {id} не найден."});
            }

            UserViewModel modelUser = _mapper.Map<UserViewModel>(user);

            modelUser.CurrentClaims = await _userManager.GetClaimsAsync(user);
            modelUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "без роли";
            return View(modelUser);
        }

        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "EditUsers")]
        public async Task<IActionResult> EditUser(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Пользователь с id {id} не найден."});
            }

            EditUserViewModel modelUser = _mapper.Map<EditUserViewModel>(user);
            modelUser.CurrentClaims = await _userManager.GetClaimsAsync(user);
            modelUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "без роли";
            return View(modelUser);
        }

        [HttpPost("{id:required:int:min(0)}")]
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
                var user = await _userManager.FindByIdAsync(editModel.Id);

                if (user == null)
                {
                    return View("Error", new ErrorViewModel {Message = $"Пользователь с id {editModel.Id} не найден."});
                }

                var dbConcStamp = user.ConcurrencyStamp;
                _mapper.Map(editModel, user);

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
                var result = await _userManager.UpdateUserPasswordClaims(user, editModel.Password, newClaims);
                if (result.Succeeded)
                {
                    if (HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value ==
                        user.Id.ToString())
                    {
                        await _signInManager.RefreshSignInAsync(user);
                    }
                    else
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                    }

                    TempData["Message"] = "Изменения успешно сохранены";
                    return RedirectToAction("ViewUser", new {id = editModel.Id});
                }

                //add new errors
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError(identityError.Code, identityError.Description);
                    if (identityError.Code == "ConcurrencyFailure")
                    {
                        editModel.ConcurrencyStamp = dbConcStamp; // to enable save after concurrency error
                        ModelState.Remove(
                            "ConcurrencyStamp"); // remove from the model state or HTML helpers will use the original value
                    }
                }
            }

            return View(editModel);
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Items(
            [FromQuery(Name = "s")] string search,
            [FromQuery(Name = "st")] ItemTypeSelector types,
            [FromQuery(Name = "o")] string sortOrder,
            [FromQuery(Name = "p")] int? page,
            [FromQuery(Name = "c")] int? pageCount)
        {
            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(ProductDto.Name);
            }

            var currPage = page ?? 1;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;

            SortPageResult<Product> result =
                await _shopManager.GetSortFilterPageAsync(types, search, sortOrder, currPage, countPerPage);

            //TODO categories filter // List<CategoryDTO> allCategories = await _shopManager.GetAllCategoriesAsync();

            ViewBag.itemCount = result.TotalN;

            //TODO mapper
            var model = new ItemsViewModel()
            {
                CurrentSearch = search,
                CurrentSortOrder = sortOrder,
                Descending = sortOrder.EndsWith("_desc"),
                CurrentPage = currPage,
                CountPerPage = countPerPage,
                Types = (int) types,
                ItemCount = result.TotalN,
                Items = _mapper.Map<IEnumerable<ProductDto>>(result.FilteredData),
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
                return View("Error", new ErrorViewModel {Message = $"Товар с id {id} не найден."});
            }

            ItemViewModel viewModel = _mapper.Map<ItemViewModel>(prod);
            return View("ViewItem", viewModel);
        }

        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "EditItems")]
        public async Task<IActionResult> EditItem(int id)
        {
            var prod = await _shopManager.GetProductByIdAsync(id, true, true, true);
            if (prod == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Товар с id {id} не найден."});
            }

            var viewModel = _mapper.Map<EditItemViewModel>(prod);
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditItems")]
        public async Task<IActionResult> EditItem([FromBody] EditItemViewModel viewModel) //json input //move to api?
        {
            if (viewModel?.Id == null || viewModel?.Name == null || viewModel.Timestamp == null)
            {
                return BadRequest("Введены некорректные данные.");
            }

            if (ModelState.IsValid)
            {
                OperationResult result;

                var product = _mapper.Map<Product>(viewModel);

                var catsIds = Enumerable.Empty<int>();
                if (viewModel.Categories != null)
                {
                    catsIds = viewModel.Categories
                        .Where(x=>x.DtoState != DtoState.Deleted)
                        .Select(x=> x.Id);
                }
                
                var descs = Enumerable.Empty<Description>();
                if (viewModel.Categories != null)
                {
                    descs = _mapper.Map<IEnumerable<Description>>(viewModel.DescriptionGroups
                        .SelectMany(x => x.DescriptionItems?.Where(i=>i.DtoState != DtoState.Deleted)));
                }
                                
                var images = Enumerable.Empty<Image>();
                if (viewModel.Categories != null)
                {
                    images = _mapper.Map<IEnumerable<Image>>(viewModel.Images?
                        .Where(x => x.DtoState != DtoState.Deleted));
                }
                
                if (viewModel.CreateItem)
                {
                    result = await _shopManager.CreateProductAsync(product, images, catsIds, descs);
                }
                else
                {
                    result = await _shopManager.UpdateProductAsync(product, descs, catsIds, images);
                }

                if (result.Succeeded)
                {
                    TempData["Message"] = "Изменения успешно сохранены";
                    //provide relative address for redirect on ajax
                    return Ok($"/{nameof(AdminController).Substring(0, 5)}/{nameof(ViewItem)}/{product.Id}");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(x => x.ErrorMessage));
            return BadRequest(errors);
        }

        [HttpGet]
        [Authorize(Policy = "EditItems")]
        public IActionResult AddItem()
        {
            return View("EditItem", new EditItemViewModel {CreateItem = true});
        }

        [HttpGet]
        [Authorize(Policy = "EditItems")]
        public IActionResult DeleteItem(DeleteItemModel model, string deleteToken) //token = 1 use token
        {
            if (model?.Id == null || deleteToken == null || deleteToken != TempData["DeleteToken"]?.ToString())
            {
                return View("Error",
                    new ErrorViewModel
                        {Message = $"Переход на страницу удаления товара возможен только после его просмотра."});
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
                return View("Error", new ErrorViewModel {Message = $"Получены некорректные данные."});
            }

            var result = await _shopManager.DeleteProductAsync(id.GetValueOrDefault());
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
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Categories()
        {
            var catTuples = await _shopManager.GetAllCategoriesWithProductCountAsync();
            var cats = new List<CategoryDto>();
            foreach (var catTuple in catTuples)
            {
                var catDto = _mapper.Map<CategoryDto>(catTuple.Item1);
                catDto.ProductCount = catTuple.Item2;
                cats.Add(catDto);
            }

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

        [HttpGet]
        [Authorize(Policy = "ViewOrders")]
        public IActionResult Orders()
        {
            return View();
        }
    }
}