using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Website.Web.Models.AdminViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
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
        [DisplayFormat(NullDisplayText = "Блокировка не активна")]
        public DateTimeOffset? LockoutEnd { get; set; }
        [Display(Name = "Двухфакторный вход")]
        public bool TwoFactorEnabled { get; set; }
        [Display(Name = "Неудачных попыток входа")]
        public int AccessFailedCount { get; set; }
        [Display(Name = "Дата последней активности")]
        public DateTimeOffset? LastActivityDate { get; set; }
        
        [Display(Name = "Имя")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string PatrName { get; set; }
        [Display(Name = "Адрес")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string Address { get; set; }
        [Display(Name = "Город")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string City { get; set; }
        [Display(Name = "Дата регистрации")]
        public DateTimeOffset? RegistrationDate { get; set; }

        [ScaffoldColumn(false)]
        public string Role { get; set; }

        [ScaffoldColumn(false)]
        public IList<Claim> CurrentClaims { get; set; }
    }
}
