using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Web.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required(ErrorMessage = "RequiredError")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
