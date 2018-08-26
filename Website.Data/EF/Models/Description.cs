using System;
using System.Collections.Generic;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    public partial class Description : IDescription
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int? ProductId { get; set; }
        public virtual int? DescriptionGroup { get; set; }
        public virtual int? SubDescrGroup { get; set; }

        public virtual DescriptionGroup DescriptionGroupNavigation { get; set; }
        public virtual Product Product { get; set; }
    }
}
