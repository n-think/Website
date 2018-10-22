using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Web.Models.AccountViewModels
{
    public class LoginWith2FaViewModel
    {
        [Required(ErrorMessage = "RequiredError")]
        [StringLength(7, ErrorMessage = "{0} должен быть от {2} до {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Код двухэтапной аутентификации")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Запомнить это устройство")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
