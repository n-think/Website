namespace Website.Data.ModelsInterfaces
{
    public interface IProductImage
    {
        int Id { get; set; }
        int? ProductId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        byte[] Data { get; set; }
    }
}