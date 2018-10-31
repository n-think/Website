using System.ComponentModel.DataAnnotations;

namespace Website.Web.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "RequiredError")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "RequiredError")]
        [StringLength(100, ErrorMessage = "{0} должен быть от {2} до {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
