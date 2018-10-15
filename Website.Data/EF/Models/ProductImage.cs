using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class ProductImage : IProductImage
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string ThumbName { get; set; }
        public string Format { get; set; }
        public bool Primary { get; set; }

        public virtual Product Product { get; set; }
    }
}
