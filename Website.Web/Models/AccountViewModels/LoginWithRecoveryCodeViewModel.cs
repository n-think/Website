using System.ComponentModel.DataAnnotations;

namespace Website.Web.Models.AccountViewModels
{
    public class LoginWithRecoveryCodeViewModel
    {
        [Required(ErrorMessage = "RequiredError")]
        [DataType(DataType.Text)]
        [Display(Name = "Код восстановления")]
        public string RecoveryCode { get; set; }
    }
}
