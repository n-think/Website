using System.Collections.Generic;

namespace Website.Core.Models.Domain
{
    public class DescriptionGroupItem
    {
        public DescriptionGroupItem()
        {
            Descriptions = new HashSet<Description>();
        }
        public int Id { get; set; }
        public int? DescriptionGroupId { get; set; }
        public string Name { get; set; }

        public virtual DescriptionGroup DescriptionGroup { get; set; }
        public virtual ICollection<Description> Descriptions { get; set; }
    }
}