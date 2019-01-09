using System;
using System.Collections.Generic;

namespace Website.Core.Models.Domain
{
    public class Product 
    {
        public Product()
        {
            Descriptions = new HashSet<Description>();
            ProductToCategory = new HashSet<ProductToCategory>();
            Images = new HashSet<Image>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Code { get; set; }
        public virtual decimal Price { get; set; }
        public virtual int Stock { get; set; }
        public virtual int Reserved { get; set; }
        public virtual bool Available { get; set; }
        public virtual DateTimeOffset Created { get; set; }
        public virtual DateTimeOffset Changed { get; set; }
        public virtual byte[] Timestamp { get; set; }

        public virtual ICollection<Description> Descriptions { get; set; }
        public virtual ICollection<ProductToCategory> ProductToCategory { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
