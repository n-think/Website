using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class ProductToCategory : IProductToCategory
    {
        public virtual int ProductId { get; set; }
        public virtual int CategoryId { get; set; }


        // Nav property for Product
        public virtual Product Product { get; set; }
        // Nav property for Category
        public virtual Category Category { get; set; }
    }
}