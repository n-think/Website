using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Website.Web.Models.AdminViewModels
{
    public class DeleteUserViewModel
    {
        public int? Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Имя")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        [DisplayFormat(NullDisplayText = "*Не указано*")]
        public string PatrName { get; set; }
    }
}