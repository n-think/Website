using System;
using System.Collections.Generic;

namespace Website.Data.EF.Models
{
    public partial class ProductImage
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }

        public Product Product { get; set; }
    }
}
