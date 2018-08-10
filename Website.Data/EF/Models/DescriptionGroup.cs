using System;
using System.Collections.Generic;

namespace Website.Data.EF.Models
{
    public partial class DescriptionGroup
    {
        public DescriptionGroup()
        {
            Descriptions = new HashSet<Description>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Description> Descriptions { get; set; }
    }
}
