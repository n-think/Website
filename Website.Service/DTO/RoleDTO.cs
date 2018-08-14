using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Website.Service.DTO
{
    public class RoleDTO
    {
        public RoleDTO()
        {
            Id = Guid.NewGuid().ToString();
        }

        public RoleDTO(string roleName) : this()
        {
            Name = roleName;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
