namespace Website.Core.Interfaces.Models
{
    public interface IDescriptionGroup
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}