using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Website.Service.DTO
{
    public class ClientProfileDTO
    {
        [Required]
        public string Email { get; set; }

        public int Age { get; set; }
        public int FirstName { get; set; }
    }
}
