using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class Category : ICategory
    {
        public Category()
        {
            ProductCategory = new HashSet<ProductToCategory>();
            Children = new HashSet<Category>();
        }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        [Unlike(nameof(Id))]
        public int? ParentId { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; }
        public virtual ICollection<ProductToCategory> ProductCategory { get; set; }
    }
}
