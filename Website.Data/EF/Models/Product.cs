using System;
using System.Collections.Generic;

namespace Website.Data.EF.Models
{
    public partial class Product
    {
        public Product()
        {
            Descriptions = new HashSet<Description>();
            ProductImages = new HashSet<ProductImage>();
            ProductCategory = new HashSet<ProductToCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Description> Descriptions { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ProductToCategory> ProductCategory { get; set; }
    }
}
