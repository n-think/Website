using System.Collections.Generic;

namespace Website.Core.Models.Domain
{
    public class DescriptionGroup 
    {
        public DescriptionGroup()
        {
            Descriptions = new HashSet<Description>();
            Children = new HashSet<DescriptionGroup>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual byte[] Timestamp { get; set; }

        public virtual DescriptionGroup Parent { get; set; }
        public virtual ICollection<DescriptionGroup> Children { get; set; }
        public virtual ICollection<Description> Descriptions { get; set; }
    }
}
