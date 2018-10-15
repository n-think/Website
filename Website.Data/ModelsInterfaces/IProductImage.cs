namespace Website.Data.ModelsInterfaces
{
    public interface IProductImage
    {
        int Id { get; set; }
        int? ProductId { get; set; }
        string Path { get; set; }
    }
}