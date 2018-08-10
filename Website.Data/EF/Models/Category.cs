using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Data.EF.Models
{
    public partial class Category
    {
        public Category()
        {
            ProductCategory = new HashSet<ProductToCategory>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ProductToCategory> ProductCategory { get; set; }
    }
}
