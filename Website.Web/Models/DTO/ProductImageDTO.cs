using System.Drawing;
using Website.Core.Enums;

namespace Website.Web.Models.DTO
{
    public class ProductImageDto
    {
        public int? Id { get; set; }
        public string Path { get; set; }
        public string PathSrc { get => string.IsNullOrEmpty(Path) ? DataUrl : Path; }
        public string ThumbPath { get; set; }
        public string ThumbPathSrc { get => string.IsNullOrEmpty(ThumbPath) ? DataUrl : ThumbPath; }
        public bool Primary { get; set; }
        public string DataUrl { get; set; }
        public DtoState DtoState { get; set; }
        public Bitmap Bitmap { get; set; }
    }
}
