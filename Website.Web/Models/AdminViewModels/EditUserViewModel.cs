using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Service.DTO;

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
        public string PhoneNumber { get; set; }

        [Display(Name = "Телефон подтвержден")]
        public bool PhoneNumberConfirmed { get; set; }

        [ScaffoldColumn(false)]
        public string ConcurrencyStamp { get; set; }

        [Display(Name = "Возможна ли блокировка")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Дата конца блокировки")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:G}")]
        //[RegularExpression("\\d{4}[-]\\d{2}[-]\\d{2}[T]\\d{2}:\\d{2}:\\d{2}[.]\\d{3}[+,-]\\d{2}:\\d{2}", ErrorMessage = "Формат даты ДД.ММ.ГГГГ ЧЧ:ММ:СС")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = "Двухфакторный вход")]
        public bool TwoFactorEnabled { get; set; }

        [ScaffoldColumn(false)]
        public string Role { get; set; }

        [ScaffoldColumn(false)]
        public string[] NewClaims { get; set; }

        [ScaffoldColumn(false)]
        public IList<Claim> CurrentClaims { get; set; }

        // Профиль
        [Display(Name = "Имя")]
        public string UserProfileFirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string UserProfileLastName { get; set; }
        [Display(Name = "Отчество")]
        public string UserProfilePatrName { get; set; }
        [Display(Name = "Адрес")]
        [DataType(DataType.MultilineText)]
        public string UserProfileAddress { get; set; }
        [Display(Name = "Город")]
        public string UserProfileCity { get; set; }
    }
}
