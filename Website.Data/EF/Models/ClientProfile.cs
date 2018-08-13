using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Website.Data.EF.Models
{
    public class ClientProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatrName { get; set; }
        public string FullName => $"{LastName} {FirstName} {PatrName}";
        public string Address { get; set; }
        public string City { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
