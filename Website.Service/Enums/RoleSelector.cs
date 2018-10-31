using System.ComponentModel.DataAnnotations;

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
