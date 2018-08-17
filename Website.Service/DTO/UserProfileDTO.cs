using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Website.Service.DTO
{
    public class UserProfileDTO
    {
        [Display(Name = "Логин")]
        public string Login { get; set; }
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
    }
}
