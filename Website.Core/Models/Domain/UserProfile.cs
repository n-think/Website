using System;
using Website.Core.Interfaces.Models;

namespace Website.Core.Models.Domain
{
    public class UserProfile : IUserProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatrName { get; set; }
        public string FullName => $"{LastName} {FirstName} {PatrName}";
        public string Address { get; set; }
        public string City { get; set; }
        public DateTimeOffset? RegistrationDate { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual User User { get; set; }
    }
}
