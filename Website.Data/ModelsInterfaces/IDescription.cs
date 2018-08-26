namespace Website.Data.ModelsInterfaces
{
    public interface IDescription
    {
        int Id { get; set; }
        string Name { get; set; }
        int? ProductId { get; set; }
        int? DescriptionGroup { get; set; }
        int? SubDescrGroup { get; set; }
    }
}