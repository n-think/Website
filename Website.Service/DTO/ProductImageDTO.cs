using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Website.Service.DTO
{
    public class ProductImageDTO
    {
        public Bitmap Bitmap { get; set; }
        public string Path { get; set; }
        public string ThumbPath { get; set; }
        public bool Primary { get; set; }
    }
}
