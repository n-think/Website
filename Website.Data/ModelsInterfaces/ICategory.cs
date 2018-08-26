namespace Website.Data.ModelsInterfaces
{
    public interface ICategory
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}