namespace Website.Data.ModelsInterfaces
{
    public interface ICategory<T>
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        byte[] Timestamp { get; set; }
        T Parent { get; set; }
    }
}