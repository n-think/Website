using System.Drawing;
using Castle.Core.Internal;
using Website.Service.Enums;

namespace Website.Service.DTO
{
    public class ProductImageDto
    {
        public int? Id { get; set; }
        public string Path { get; set; }
        public string PathSrc { get => Path.IsNullOrEmpty() ? DataUrl : Path; }
        public string ThumbPath { get; set; }
        public string ThumbPathSrc { get => ThumbPath.IsNullOrEmpty() ? DataUrl : ThumbPath; }
        public bool Primary { get; set; }
        public string DataUrl { get; set; }
        public DtoState DtoState { get; set; }
        internal Bitmap Bitmap { get; set; }
    }
}
