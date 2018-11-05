namespace Website.Data.ModelsInterfaces
{
    public interface IDescription
    {
        int Id { get; set; }

        int? ProductId { get; set; }
        int? DescriptionGroupItemId { get; set; }
        string Value { get; set; }
    }
}