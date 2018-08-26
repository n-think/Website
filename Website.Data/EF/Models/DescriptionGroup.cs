using System;
using System.Collections.Generic;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class DescriptionGroup : IDescriptionGroup
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
