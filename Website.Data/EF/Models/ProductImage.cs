using System;
using System.Collections.Generic;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class ProductImage : IProductImage
    {
        public virtual int Id { get; set; }
        public virtual int? ProductId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual byte[] Data { get; set; }

        public virtual Product Product { get; set; }
    }
}
