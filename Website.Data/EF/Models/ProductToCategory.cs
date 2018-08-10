namespace Website.Data.EF.Models
{
    public class ProductToCategory
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}