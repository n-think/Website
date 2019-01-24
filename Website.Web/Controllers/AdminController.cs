using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IAuthenticationService _authService;
        private readonly IDatabaseInitializer _dbInitializer;
        private readonly IUserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly SignInManager _signInManager;
        private readonly IShopManager _shopManager;

        public AdminController(IUserManager userManager, IShopManager shopManager, RoleManager roleManager,
            SignInManager signInManager, IMapper mapper, IDatabaseInitializer dbInitializer,
            IAuthenticationService authService)
        {
            _authService = authService;
            _dbInitializer = dbInitializer;
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
        [Authorize(Policy = "DeleteUsers")]
        public async Task<IActionResult> DeleteUser(int id, string deleteToken)
        {
            if (deleteToken == null || deleteToken != TempData["DeleteToken"]?.ToString())
            {
                return View("Error",
                    new ErrorViewModel
                        {Message = $"Переход на страницу удаления пользователя возможен только после его просмотра."});
            }

            if (id <= 0)
            {
                return View("Error", new ErrorViewModel {Message = $"Получены некорректные данные."});
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Пользователь с id {id} не найден."});
            }

            return View(_mapper.Map<DeleteUserViewModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "DeleteUsers")]
        public async Task<IActionResult> DeleteUserConfirm(int id)
        {
            if (id <= 0)
            {
                return View("Error", new ErrorViewModel {Message = $"Получены некорректные данные."});
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Пользователь с id {id} не найден."});
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ViewData["message"] = result.Errors?.FirstOrDefault()?.Description;
            }
            else
            {
                ViewData["message"] = "Пользователь успешно удален.";
            }

            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Items(
            [FromQuery(Name = "s")] string search,
            [FromQuery(Name = "st")] ItemTypeSelector types,
            [FromQuery(Name = "o")] string sortOrder,
            [FromQuery(Name = "p")] int? page,
            [FromQuery(Name = "c")] int? pageCount,
            [FromQuery(Name = "cat")] int[] categoryIds,
            [FromQuery(Name = "desc")] int[] descGroupIds)
        {
            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(ProductDto.Name);
            }

            var currPage = page ?? 1;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;

            SortPageResult<Product> result =
                await _shopManager.GetSortFilterPageAsync(types, currPage, countPerPage, search, sortOrder,
                    categoryIds, descGroupIds);

            var allCategories =
                _mapper.Map<IEnumerable<CategoryDto>>(await _shopManager.GetAllCategoriesAsync());

            ViewBag.itemCount = result.TotalN;

            var model = new ItemsViewModel()
            {
                CurrentSearch = search,
                CurrentSortOrder = sortOrder,
                Descending = sortOrder.EndsWith("_desc"),
                CurrentPage = currPage,
                CountPerPage = countPerPage,
                Types = (int) types,
                ItemCount = result.TotalN,
                CategoryIds = categoryIds,
                DescGroupIds = descGroupIds,
                Items = _mapper.Map<IEnumerable<ProductDto>>(result.FilteredData),
                Categories = allCategories
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
        [Authorize(Policy = "AddEditItems")]
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
        [Authorize(Policy = "AddEditItems")]
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
                        .Where(x => x.DtoState != DtoState.Deleted)
                        .Select(x => x.Id);
                }

                var descs = Enumerable.Empty<Description>();
                if (viewModel.DescriptionGroups != null)
                {
                    descs = _mapper.Map<IEnumerable<Description>>(viewModel.DescriptionGroups
                        .SelectMany(x => x.DescriptionItems?.Where(i => i.DtoState != DtoState.Deleted)));
                }

                var images = Enumerable.Empty<Image>();
                if (viewModel.Images != null)
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
        [Authorize(Policy = "AddEditItems")]
        public IActionResult AddItem()
        {
            return View("EditItem", new EditItemViewModel {CreateItem = true});
        }

        [HttpGet]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteItem(int id, string deleteToken) //token = 1 use token
        {
            if (deleteToken == null || deleteToken != TempData["DeleteToken"]?.ToString())
            {
                return View("Error",
                    new ErrorViewModel
                        {Message = $"Переход на страницу удаления товара возможен только после его просмотра."});
            }

            if (id <= 0)
            {
                return View("Error", new ErrorViewModel {Message = $"Получены некорректные данные."});
            }

            var item = await _shopManager.GetProductByIdAsync(id, false, false, false);
            if (item == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Товар с id {id} не найден."});
            }

            return View(_mapper.Map<DeleteItemViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteItemConfirm(int id)
        {
            if (id <= 0)
            {
                return View("Error", new ErrorViewModel {Message = $"Получены некорректные данные."});
            }

            var item = await _shopManager.GetProductByIdAsync(id, false, false, false);
            if (item == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Товар с id {id} не найден."});
            }

            var result = await _shopManager.DeleteProductAsync(id);
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

            return View(cats.ToTree().OrderBy(x=>x.Name));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return RedirectToAction("Categories");
            }

            var category = _mapper.Map<Category>(categoryDto);

            OperationResult result = await _shopManager.CreateCategoryAsync(category);
            if (!result.Succeeded)
            {
                TempData["Message"] = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
            }
            else
            {
                TempData["Message"] = $"Категория \"{category.Name}\" успешно добавлена.";
            }

            return RedirectToAction("Categories");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> EditCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return RedirectToAction("Categories");
            }

            var category = _mapper.Map<Category>(categoryDto);

            OperationResult result = await _shopManager.UpdateCategoryAsync(category);
            if (!result.Succeeded)
            {
                TempData["Message"] = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
            }
            else
            {
                TempData["Message"] = $"Категория \"{category.Name}\" успешно изменена.";
            }

            return RedirectToAction("Categories");
        }


        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _shopManager.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Категория с id {id} не найдена."});
            }

            var catDto = _mapper.Map<CategoryDto>(category);
            return View(catDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteCategoryConfirm(CategoryDto category)
        {
            if (category == null)
            {
                return RedirectToAction("Categories");
            }

            var result = await _shopManager.DeleteCategoryAsync(category.Id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return View("DeleteCategory", category);
            }

            TempData["Message"] = $"Категория \"{category.Name}\" успешно удалена.";
            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> DescriptionGroups()
        {
            var descGroups = (await _shopManager.GetAllDescriptionGroupsWithProductCountAsync())
                .Select(x => (
                    _mapper.Map<DescriptionGroupDto>(x.Item1),
                    x.Item2));
            return View(descGroups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> AddDescriptionGroup(DescriptionGroupDto descriptionGroupDto)
        {
            if (descriptionGroupDto == null)
            {
                return RedirectToAction("DescriptionGroups");
            }

            var descriptionGroup = _mapper.Map<DescriptionGroup>(descriptionGroupDto);

            OperationResult result = await _shopManager.CreateDescriptionGroupAsync(descriptionGroup);
            if (!result.Succeeded)
            {
                TempData["Message"] = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
            }
            else
            {
                TempData["Message"] = $"Группа \"{descriptionGroup.Name}\" успешно добавлена.";
            }

            return RedirectToAction("DescriptionGroups");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> EditDescriptionGroup(DescriptionGroupDto descriptionGroupDto)
        {
            if (descriptionGroupDto == null)
            {
                return RedirectToAction("DescriptionGroups");
            }

            var descriptionGroup = _mapper.Map<DescriptionGroup>(descriptionGroupDto);

            OperationResult result = await _shopManager.UpdateDescriptionGroupAsync(descriptionGroup);
            if (!result.Succeeded)
            {
                TempData["Message"] = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
            }
            else
            {
                TempData["Message"] = $"Группа \"{descriptionGroupDto.Name}\" успешно изменена.";
            }

            return RedirectToAction("DescriptionGroups");
        }


        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteDescriptionGroup(int id)
        {
            var descGroup = await _shopManager.GetDescriptionGroupByIdAsync(id);
            if (descGroup == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Категория с id {id} не найдена."});
            }

            var descGroupDto = _mapper.Map<DescriptionGroupDto>(descGroup);
            return View(descGroupDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteDescriptionGroupConfirm(DescriptionGroupDto descriptionGroupDto)
        {
            if (descriptionGroupDto == null)
            {
                return RedirectToAction("DescriptionGroups");
            }

            var result = await _shopManager.DeleteDescriptionGroupAsync(descriptionGroupDto.Id.GetValueOrDefault());
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return View("DeleteDescriptionGroup", descriptionGroupDto);
            }

            TempData["Message"] = $"Категория \"{descriptionGroupDto.Name}\" успешно удалена.";
            return RedirectToAction("DescriptionGroups");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> AddDescriptionItem(DescriptionGroupItemDto descriptionGroupItemDto)
        {
            if (descriptionGroupItemDto == null || descriptionGroupItemDto.DescriptionGroupId == null)
            {
                return BadRequest("Получены некорректные данные");
            }

            var item = _mapper.Map<DescriptionGroupItem>(descriptionGroupItemDto);

            OperationResult result = await _shopManager.CreateDescriptionItemAsync(item);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
            }
            else
            {
                return Created("", item.Id);
            }
        }


        [HttpPost]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> EditDescriptionItem(DescriptionGroupItemDto descriptionGroupItemDto)
        {
            if (descriptionGroupItemDto == null)
            {
                return BadRequest("Получены некорректные данные");
            }

            var item = _mapper.Map<DescriptionGroupItem>(descriptionGroupItemDto);

            OperationResult result = await _shopManager.UpdateDescriptionItemAsync(item);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
            }
            else
            {
                return Ok(item.Id);
            }
        }

        [HttpGet("{id:required:int:min(0)}")]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteDescriptionItem(int id)
        {
            DescriptionGroupItem item = await _shopManager.GetDescriptionItemByIdAsync(id);
            if (item == null)
            {
                return View("Error", new ErrorViewModel {Message = $"Описание с id {id} не найдено."});
            }

            var itemDto = _mapper.Map<DescriptionGroupItemDto>(item);
            return View(itemDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "DeleteItems")]
        public async Task<IActionResult> DeleteDescriptionItemConfirm(DescriptionGroupItemDto descriptionGroupItemDto)
        {
            if (descriptionGroupItemDto == null || descriptionGroupItemDto.Id == null)
            {
                return RedirectToAction("DescriptionGroups");
            }

            var result = await _shopManager.DeleteDescriptionItemAsync(descriptionGroupItemDto.Id.GetValueOrDefault());
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return View("DeleteDescriptionItem", descriptionGroupItemDto);
            }

            TempData["Message"] = $"Описание \"{descriptionGroupItemDto.Name}\" успешно удалено.";
            return RedirectToAction("DescriptionGroups");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditUsers")]
        public async Task<IActionResult> GenerateUsers(int count)
        {
            if (count < 1 || count >= 100)
            {
                TempData["Message"] = $"Количество должно быть от 1 до 100.";
                return RedirectToAction("Index");
            }

            int generated = await _dbInitializer.GenerateUsersAsync(count);
            TempData["Message"] = $"Cгенерировано {generated} пользователей.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AddEditItems")]
        public async Task<IActionResult> GenerateItems(int count)
        {
            if (count < 1 || count >= 100)
            {
                TempData["Message"] = $"Количество должно быть от 1 до 100.";
                return RedirectToAction("Index");
            }

            int generated = await _dbInitializer.GenerateItemsAsync(count);
            TempData["Message"] = $"Cгенерировано {generated} товаров.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin_generated")]
        public async Task<IActionResult> DropCreateDatabase()
        {
            OperationResult result = await _dbInitializer.DropCreateDatabase();

            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                TempData["Message"] = $"Ошибка при пересоздании базы данных. {errors}";
            }
            else
            {
                TempData["Message"] = $"База данных была успешно пересоздана.";
            }

            var newAdminUser = await _userManager.FindByNameAsync("admin");
            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(newAdminUser);
            await _signInManager.SignOutAsync();
            await _authService.SignInAsync(HttpContext, IdentityConstants.ApplicationScheme, userPrincipal,
                new AuthenticationProperties());

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Policy = "ViewOrders")]
        public IActionResult Orders()
        {
            return View();
        }
    }
}