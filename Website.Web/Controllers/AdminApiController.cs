using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Service.DTO;
using Website.Service.Interfaces;

namespace Website.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminApiController : ControllerBase
    {
        private IShopManager _shopManager;

        public AdminApiController(IShopManager sManager)
        {
            _shopManager = sManager;
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Categories()
        {
            var categories = await _shopManager.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public IActionResult Descriptions(int? id)
        {
            if (!id.HasValue || id < 0)
            {
                return BadRequest("");
            }

            var descs = new Object[0];

            if (descs.IsNullOrEmpty())
            {
                return NotFound("");
            }

            return Ok();
        }
    }
}
