using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Website.Web.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
            [Required]
            [StringLength(7, ErrorMessage = "{0} должен быть от {2} до {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Код подтверждения")]
            public string Code { get; set; }

            [BindNever]
            public string SharedKey { get; set; }

            [BindNever]
            public string AuthenticatorUri { get; set; }
    }
}
