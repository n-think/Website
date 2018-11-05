using System;
using System.Collections.Generic;
using System.Text;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public class DescriptionGroupItem : IDescriptionGroupItem
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
