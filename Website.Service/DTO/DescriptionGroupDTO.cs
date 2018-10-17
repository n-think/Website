using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.DTO
{
    public class DescriptionGroupDTO
    {
        public DescriptionGroupDTO()
        {
            Items = new List<DescriptionItem>();
        }
        public List<DescriptionItem> Items { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Name:{Name}; Desc:{Description}";
        }
    }

    public class DescriptionItem
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Name:{Name}; Value:{Value}";
        }
    }
}
