using System.Collections.Generic;

namespace Website.Core.Models.Domain
{
    public class Category
    {
        public Category()
        {
            ProductCategory = new HashSet<ProductToCategory>();
            Children = new HashSet<Category>();
        }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual byte[] Timestamp { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; }
        public virtual ICollection<ProductToCategory> ProductCategory { get; set; }
    }
}
