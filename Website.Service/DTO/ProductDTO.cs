using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public byte[] ThumbImage { get; set; }
        public byte[] Timestamp { get; set; }

        //category
        public string CategoryName { get; set; }
        //images
        public byte[][] Images { get; set; }
    }
}
