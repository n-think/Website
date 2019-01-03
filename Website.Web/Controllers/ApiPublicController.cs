using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Core.Interfaces.Services;

namespace Website.Web.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/public/[action]")]
    public class ApiPublicController : ControllerBase
    {
        private IShopManager _shopManager;

        public ApiPublicController(IShopManager sManager)
        {
            _shopManager = sManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> InstantSearch(string searchString)
        {
            var categories = await _shopManager.GetAllCategoriesAsync();
            return Ok(categories);
        }

        
    }
}
