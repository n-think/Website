namespace Website.Data.ModelsInterfaces
{
    public interface IDescription
    {
        int Id { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        int? ProductId { get; set; }
        int? DescriptionGroupId { get; set; }
    }
}