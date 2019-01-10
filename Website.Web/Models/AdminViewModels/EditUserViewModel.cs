using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Website.Web.Models.AdminViewModels
{
    public class EditUserViewModel
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Required(ErrorMessage = "RequiredError")]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный email адрес.")]
        [Required(ErrorMessage = "RequiredError")]
        public string Email { get; set; }

        [Display(Name = "Email подтвержден")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный номер телефона.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Телефон подтвержден")]
        public bool PhoneNumberConfirmed { get; set; }

        [ScaffoldColumn(false)]
        public string ConcurrencyStamp { get; set; }

        [Display(Name = "Возможна ли блокировка")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Дата конца блокировки")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:G}")]
        [DataType(DataType.DateTime)]
        //[RegularExpression("\\d{2}[.]\\d{2}[.]\\d{4}[ ]\\d{2}:\\d{2}:\\d{2}", ErrorMessage = "Формат даты ДД.ММ.ГГГГ ЧЧ:ММ:СС")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = "Двухфакторный вход")]
        public bool TwoFactorEnabled { get; set; }
        
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string PatrName { get; set; }
        [Display(Name = "Адрес")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Display(Name = "Город")]
        public string City { get; set; }

        [ScaffoldColumn(false)]
        public string Role { get; set; }

        [ScaffoldColumn(false)]
        public string[] NewClaims { get; set; }

        [ScaffoldColumn(false)]
        public IList<Claim> CurrentClaims { get; set; }
    }
}
