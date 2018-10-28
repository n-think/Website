using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Website.Service.Enums;

namespace Website.Service.DTO
{
    public class ProductImageDto
    {
        public int? Id { get; set; }
        public string Path { get; set; }
        public string ThumbPath { get; set; }
        public bool Primary { get; set; }
        public string UriBase64Data { get; set; }
        public DtoState DtoState { get; set; }
    }
}
