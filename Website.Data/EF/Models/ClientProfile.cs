using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Website.Data.EF.Models
{
        //TODO concurrency fields
    public class ClientProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatrName { get; set; }
        public string FullName { get => $"{LastName} {FirstName} {PatrName}"; }
        public int Age { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
