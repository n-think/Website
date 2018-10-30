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
            Images = new HashSet<ProductImage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Reserved { get; set; }
        public bool Enabled { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Changed { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual ICollection<Description> Descriptions { get; set; }
        public virtual ICollection<ProductToCategory> ProductCategory { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
    }
}
