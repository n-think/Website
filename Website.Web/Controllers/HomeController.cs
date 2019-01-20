using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Web.Models.AdminViewModels;
using Website.Web.Models.DTO;
using Website.Web.Models.HomeViewModels;

namespace Website.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IShopManager _shopManager;

        public HomeController(IShopManager shopManager, IMapper mapper)
        {
            _mapper = mapper;
            _shopManager = shopManager;
        }
        
        public async Task<ActionResult> Index(
            [FromQuery(Name = "p")] int? page,
            [FromQuery(Name = "c")] int? pageCount)
        {
            var currPage = page ?? 1;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;
            
            SortPageResult<Product> result =
                await _shopManager.GetSortFilterPageAsync(ItemTypeSelector.Enabled, currPage, countPerPage);

            var recentProductNumber = 5;
            IEnumerable<Product> recentProducts = await _shopManager.GetNewProducts(recentProductNumber);
            
            var model = new IndexViewModel()
            {
                RecentItems = _mapper.Map<IEnumerable<ProductDto>>(recentProducts),
                Items = _mapper.Map<IEnumerable<ProductDto>>(result.FilteredData),
                ItemCount = result.TotalN,
                CurrentPage = currPage,
                CountPerPage = countPerPage
            };
            
            return View(model);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Search(
            [FromQuery(Name = "s")] string search,
            [FromQuery(Name = "o")] string sortOrder,
            [FromQuery(Name = "p")] int? page,
            [FromQuery(Name = "c")] int? pageCount,
            [FromQuery(Name = "cat")] int[] categoryIds)
            
        {
            if (sortOrder.IsNullOrEmpty())
            {
                sortOrder = nameof(ProductDto.Name);
            }

            var currPage = page ?? 1;
            var countPerPage = pageCount == null || pageCount <= 0 ? 15 : pageCount.Value;

            SortPageResult<Product> result =
                await _shopManager.GetSortFilterPageAsync(ItemTypeSelector.Enabled, currPage, countPerPage,
                    search, sortOrder, categoryIds);

            var allCategories =
                _mapper.Map<IEnumerable<CategoryDto>>(await _shopManager.GetAllCategoriesAsync());

            ViewBag.itemCount = result.TotalN;

            //TODO mapper
            var model = new SearchViewModel()
            {
                CurrentSearch = search,
                CurrentSortOrder = sortOrder,
                Descending = sortOrder.EndsWith("_desc"),
                CurrentPage = currPage,
                CountPerPage = countPerPage,
                ItemCount = result.TotalN,
                CategoryIds = categoryIds,
                FilteredItems = _mapper.Map<IEnumerable<ProductDto>>(result.FilteredData),
                AllCategories = allCategories
            };

            if (!search.IsNullOrEmpty())
            {
                IEnumerable<Category> filteredCategories = await _shopManager.SearchCategoriesByName(search);
                model.FilteredCategories = _mapper.Map<IEnumerable<CategoryDto>>(filteredCategories);
            }

            return View(model);
        }
        
        [HttpGet("[action]/{id:int:required:min(0)}")]
        public async Task<IActionResult> ViewItem(int id)
        {
            var item = await _shopManager.GetProductByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<ViewItemModel>(item));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Страница описания.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Страница контактов.";

            return View();
        }
    }
}
