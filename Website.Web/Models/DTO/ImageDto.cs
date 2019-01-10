using System.Drawing;
using Website.Core.Enums;

namespace Website.Web.Models.DTO
{
    public class ImageDto
    {
        public int? Id { get; set; }
        public bool Primary { get; set; }
        public string DataUrl { get; set; }
        public DtoState DtoState { get; set; }
    }
}
