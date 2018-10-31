using System.ComponentModel.DataAnnotations;

namespace Website.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "RequiredError")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "RequiredError")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
