using System.Collections.Generic;
using Website.Core.Interfaces.Models;

namespace Website.Core.Models.Domain
{
    public class DescriptionGroup : IDescriptionGroup
    {
        public DescriptionGroup()
        {
            DescriptionGroupItems = new HashSet<DescriptionGroupItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DescriptionGroupItem> DescriptionGroupItems { get; set; }
    }
}
