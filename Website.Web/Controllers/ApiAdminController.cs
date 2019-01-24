using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Core.Interfaces.Services;

namespace Website.Web.Controllers
{
    [ApiController]
    [Authorize(Policy = "Administrators")]
    [Route("api/admin/[action]")]
    public class ApiAdminController : ControllerBase
    {
        private IShopManager _shopManager;

        public ApiAdminController(IShopManager sManager)
        {
            _shopManager = sManager;
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> Categories()
        {
            var categories = (await _shopManager.GetAllCategoriesAsync())
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description
                });
            return Ok(categories);
        }

        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> DescriptionGroups()
        {
            var descGroups = (await _shopManager.GetAllDescriptionGroupsAsync())
                .Select(x => new {Id = x.Id, Name = x.Name, Description = x.Description});
            return Ok(descGroups);
        }

        [HttpGet("{groupId:int:min(0)}")]
        [Authorize(Policy = "ViewItems")]
        public async Task<IActionResult> DescriptionItems(int groupId)
        {
            var descItems = (await _shopManager.GetDescriptionItemsAsync(groupId))
                .Select(x => new {x.Id, x.Name, groupId});
            return Ok(descItems);
        }
    }
}