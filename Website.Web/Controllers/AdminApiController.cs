using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Service.DTO;

namespace Website.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminApiController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "ViewItems")]
        public IActionResult Categories(int? id)
        {
            if (!id.HasValue || id < 0)
            {
                return BadRequest("");
            }

            var category = new[] { new CategoryDto() };

            if (category.IsNullOrEmpty())
            {
                return NotFound("");
            }

            var result = new[]
            {
                new CategoryDto()
                {
                    Name = "cat1",
                    Description = "cat1 desc",
                    Id = 1,
                    Timestamp = new byte[] {1,2}
                },
                new CategoryDto()
                {
                    Name = "cat2",
                    Description = "cat2 desc",
                    Id = 2,
                    Timestamp = new byte[] {1,2}
                },
                new CategoryDto()
                {
                    Name = "cat3",
                    Description = "cat3 desc",
                    Id = 3,
                    ParentId = 1,
                    Timestamp = new byte[] {1,2}
                }

            };
            return Ok(result);
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
