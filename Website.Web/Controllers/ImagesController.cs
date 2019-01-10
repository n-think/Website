using Microsoft.AspNetCore.Mvc;

namespace Website.Web.Controllers
{
    public class ImagesController : Controller
    {
        [HttpGet("{controller}/{id:required:int:min(0)}")]
        public IActionResult FullSize(int id)
        {
            return Content("full");
        }
        [HttpGet("{controller}/{action}/{id:required:int:min(0)}")]
        public IActionResult Thumb(int id)
        {
            return Content("thumb");
        }
    }
}