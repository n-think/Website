using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    public class Role : IdentityRole<int>
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public Role(string roleName) : this()
        {
            Name = roleName;
        }
        
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
