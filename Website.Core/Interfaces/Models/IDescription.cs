namespace Website.Core.Interfaces.Models
{
    public interface IDescription
    {
        int Id { get; set; }

        int? ProductId { get; set; }
        int? DescriptionGroupItemId { get; set; }
        string Value { get; set; }
    }
}