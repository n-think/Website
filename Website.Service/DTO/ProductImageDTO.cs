using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Website.Service.DTO
{
    public class ProductImageDto
    {
        public int? Id { get; set; }
        public string Path { get; set; }
        public string ThumbPath { get; set; }
        public bool Primary { get; set; }
        public bool PendingDel { get; set; }
        public bool PendingAdd { get; set; }
        public string UriBase64Data { get; set; }
    }
}
