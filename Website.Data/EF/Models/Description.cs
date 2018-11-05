using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class Description : IDescription
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int? ProductId { get; set; }
        public int? DescriptionGroupItemId { get; set; }

        public virtual DescriptionGroupItem DescriptionGroupItem { get; set; }
        public virtual Product Product { get; set; }
    }
}
