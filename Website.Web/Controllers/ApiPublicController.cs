using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

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

        [HttpGet("{searchString:required:minlength(2)}")]
        public async Task<IActionResult> InstantSearch(string searchString)
        {
            IEnumerable<Product> products = await _shopManager.SearchProductsByName(searchString);

            return Ok(products.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.Price,
                    imageThumb = $"/images/thumb/{x.Images.FirstOrDefault(y=>y.Primary)?.Id}"
                }
            ));
        }
    }
}