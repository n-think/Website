using System;
using System.Collections.Generic;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class Product : IProduct
    {
        public Product()
        {
            Descriptions = new HashSet<Description>();
            ProductCategory = new HashSet<ProductToCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public byte[] ThumbImage { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual ICollection<Description> Descriptions { get; set; }
        public virtual ICollection<ProductToCategory> ProductCategory { get; set; }
    }
}
