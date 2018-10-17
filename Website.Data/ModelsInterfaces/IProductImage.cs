namespace Website.Data.ModelsInterfaces
{
    public interface IProductImage
    {
        int Id { get; set; }
        int? ProductId { get; set; }
        string Path { get; set; }
        string Name { get; set; }
        string ThumbName { get; set; }
        string Format { get; set; }
        bool Primary { get; set; }
    }
}