using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Website.Service.DTO
{
    public class ClientProfileDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatrName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
