using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Service.DTO;
using Website.Service.Interfaces;

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
