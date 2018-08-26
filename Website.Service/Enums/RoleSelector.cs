using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Website.Service.Enums
{
    public enum RoleSelector
    {
        [Display(Name = "Пользователи")]
        Users,
        [Display(Name = "Все")]
        Everyone,
        [Display(Name = "Администраторы")]
        Administrators
    }
}
