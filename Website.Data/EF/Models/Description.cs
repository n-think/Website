using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class Description : IDescription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? ProductId { get; set; }
        public int? DescriptionGroupId { get; set; }

        public virtual DescriptionGroup DescriptionGroupNavigation { get; set; }
        public virtual Product Product { get; set; }
    }
}
