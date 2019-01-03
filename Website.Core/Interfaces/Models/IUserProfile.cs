using System;

namespace Website.Core.Interfaces.Models
{
    public interface IUserProfile
    {
        string Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PatrName { get; set; }
        string FullName { get; }
        string Address { get; set; }
        string City { get; set; }
        DateTimeOffset? RegistrationDate { get; set; }
        byte[] Timestamp { get; set; }
    }
}