using System;
using System.Collections.Generic;

namespace Website.Data.EF.Models
{
    public partial class Description
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ProductId { get; set; }
        public int? DescriptionGroup { get; set; }
        public int? SubDescrGroup { get; set; }

        public virtual DescriptionGroup DescriptionGroupNavigation { get; set; }
        public virtual Product Product { get; set; }
    }
}
