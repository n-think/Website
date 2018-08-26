using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public partial class Category : ICategory
    {
        public Category()
        {
            ProductCategory = new HashSet<ProductToCategory>();
        }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual ICollection<ProductToCategory> ProductCategory { get; set; }
    }
}
