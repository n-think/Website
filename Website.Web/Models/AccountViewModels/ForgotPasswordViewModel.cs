using System.ComponentModel.DataAnnotations;

namespace Website.Web.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "RequiredError")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
