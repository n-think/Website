using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Website.Data.EF.Models
{
    public class Client
    {
        [Key]
        [ForeignKey("IdentityUser")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Не указан возраст")]
        [Range(18, 100, ErrorMessage = "Только от 18")]
        public int Age { get; set; }
    }
}
