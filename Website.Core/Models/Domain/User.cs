using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    public class User : IdentityUser<int>
    {
        public User() 
        {
            UserRoles = new HashSet<UserRole>();
            Claims = new HashSet<UserClaim>();
            Logins = new HashSet<UserLogin>();
            Tokens = new HashSet<UserToken>();
        }
        
        public User(string userName) : this()
        {
            this.UserName = userName;
        }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PatrName { get; set; }
        public string FullName => $"{LastName} {FirstName} {PatrName}";
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual DateTimeOffset? RegistrationDate { get; set; }
        public virtual DateTimeOffset? LastActivityDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; }

        public virtual ICollection<UserClaim> Claims { get; }

        public virtual ICollection<UserLogin> Logins { get; }

        public virtual ICollection<UserToken> Tokens { get; }

        public override string ToString()
        {
            return this.UserName;
        }
    }
}
