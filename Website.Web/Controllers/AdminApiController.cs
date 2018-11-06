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
            var categories = await _shopManager.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> DescriptionGroups()
        {
            var descGroups = await _shopManager.GetDescriptionGroupsAsync();
            return Ok(descGroups);
        }

        [HttpGet("{groupId:int}")]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> DescriptionItems(int groupId)
        {
            if (groupId < 0)
            {
                return BadRequest("");
            }
            var descItems = (await _shopManager.GetDescriptionItemsAsync(groupId))
                .Select(x=>new {x.Id,x.Name});
            return Ok(descItems);
        }
    }
}
