using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Website.Web.Models.DTO
{
    public class UserDto
    {
        public UserDto(string userName)
        {
            this.UserName = userName;
        }

        public int Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        [Display(Name = "Email подтвержден")]
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Телефон подтвержден")]
        public bool PhoneNumberConfirmed { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string SecurityStamp { get; set; }
        [Display(Name = "Возможна ли блокировка")]
        public bool LockoutEnabled { get; set; }
        [Display(Name = "Дата конца блокировки")]
        public DateTimeOffset? LockoutEnd { get; set; }
        [Display(Name = "Двухфакторный вход")]
        public bool TwoFactorEnabled { get; set; }
        [Display(Name = "Неудачных попыток входа")]
        public int AccessFailedCount { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string PatrName { get; set; }
        [Display(Name = "ФИО")]
        public string FullName => $"{LastName} {FirstName} {PatrName}";
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Город")]
        public string City { get; set; }
        [Display(Name = "Дата регистрации")]
        public DateTimeOffset? RegistrationDate { get; set; }
        [Display(Name = "Дата последней активности")]
        public DateTimeOffset? LastActivityDate { get; set; }

        public override string ToString()
        {
            return this.UserName;
        }
    }
}
