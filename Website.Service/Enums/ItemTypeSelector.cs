using System.ComponentModel.DataAnnotations;

namespace Website.Service.Enums
{
    public enum ItemTypeSelector
    {
        [Display(Name = "Все")]
        All,
        [Display(Name = "В магазине")]
        Enabled,
        [Display(Name = "Не в магазине")]
        Disabled
    }
}
