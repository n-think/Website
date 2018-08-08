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
        [Key]
        [ForeignKey("IdentityUser")]
        public string Id { get; set; }

        public int Age { get; set; }

        public virtual IdentityUser IdentityUser { get; set; }
    }
}
