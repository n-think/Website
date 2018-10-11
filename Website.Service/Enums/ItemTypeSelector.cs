using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
