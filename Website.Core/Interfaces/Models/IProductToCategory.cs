namespace Website.Core.Interfaces.Models
{
    public interface IProductToCategory
    {
        int ProductId { get; set; }
        int CategoryId { get; set; }
    }
}