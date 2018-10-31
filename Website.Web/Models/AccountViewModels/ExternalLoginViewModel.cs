using System.ComponentModel.DataAnnotations;

namespace Website.Web.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required(ErrorMessage = "RequiredError")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
