using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Core.Interfaces.Services;

namespace Website.Web.Controllers
{
    public class ImagesController : Controller
    {
        public IShopManager ShopManager { get; }

        public ImagesController(IShopManager shopManager)
        {
            ShopManager = shopManager;
        }

        [HttpGet("[controller]/{id:required:int:min(0)}")]
        public async Task<IActionResult> FullSize(int id)
        {
            var (bytes, mime) = await ShopManager.GetImageDataMimeAsync(id, false);
                        
            if (bytes == null)
            {
                return NotFound();
            }
            
            MemoryStream ms = new MemoryStream(bytes);
            return new FileStreamResult(ms, mime);
        }

        [HttpGet("[controller]/[action]/{id:required:int:min(0)}")]
        public async Task<IActionResult> Thumb(int id)
        {
            var (bytes, mime) = await ShopManager.GetImageDataMimeAsync(id, true);
                        
            if (bytes == null)
            {
                return NotFound();
            }
            
            MemoryStream ms = new MemoryStream(bytes);
            return new FileStreamResult(ms, mime);
        }
    }
}