using System;
using System.Drawing;
using System.Security.Policy;
using Website.Core.Enums;
using Website.Web.Controllers;

namespace Website.Web.Models.DTO
{
    public class ImageDto
    {
        private static readonly string FullImageServerPath;
        private static readonly string ThumbImageServerPath;
        static ImageDto()
        {
            string fullCtrName = nameof(ImagesController);
            string controllerName = fullCtrName.Substring(0, fullCtrName.LastIndexOf("Controller", StringComparison.Ordinal));
            FullImageServerPath =  $"/{controllerName}/";
            ThumbImageServerPath = $"/{controllerName}/{nameof(ImagesController.Thumb)}/";
        }

        public int? Id { get; set; }
        public bool Primary { get; set; }
        public string DataUrl { get; set; }
        public string DataUrlOrPath => DataUrl ?? FullImageServerPath + Id;
        public string DataUrlOrThumbPath => DataUrl ?? ThumbImageServerPath + Id;
        public DtoState DtoState { get; set; }
    }
}
