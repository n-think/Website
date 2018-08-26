namespace Website.Data.ModelsInterfaces
{
    public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        int Code { get; set; }
        int Price { get; set; }
        int Quantity { get; set; }
        byte[] ThumbImage { get; set; }
        byte[] Timestamp { get; set; }
    }
}