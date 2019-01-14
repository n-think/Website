using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Website.Web.Models.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class EditItemViewModel
    {
        [ScaffoldColumn(false)] public int? Id { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Необходимо ввести наименование товара.")]
        public string Name { get; set; }

        [Display(Name = "Описание")] public string Description { get; set; }

        [Display(Name = "Артикул")]
        [Required(ErrorMessage = "Необходимо ввести артикул товара.")]
        public int? Code { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Необходимо ввести цену товара.")] //regex, required в разметке вьюшки, номера через запятую без ошибок валидации подругому не смог
        [RegularExpression("\\d+([,.])?\\d+", ErrorMessage =
            "Поле должно быть положительным и содержать цифры, разделенные запятой или точкой.")] // custom mvc binder converts , -> .
        public decimal Price { get; set; }

        [Display(Name = "Склад")]
        [Required(ErrorMessage = "Необходимо ввести кол-во товара на складе.")]
        [Range(1, int.MaxValue, ErrorMessage = "Кол-во на складе не может быть меньше единицы.")]
        public int? Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Резерв не может быть меньше нуля.")]
        [Display(Name = "Резерв")]
        public int? Reserved { get; set; }

        [Display(Name = "Доступен")]
        [ScaffoldColumn(false)]
        public bool Available { get; set; }

        [ScaffoldColumn(false)] public DateTimeOffset Created { get; set; }
        [ScaffoldColumn(false)] public DateTimeOffset Changed { get; set; }
        [ScaffoldColumn(false)] public byte[] Timestamp { get; set; }
        [ScaffoldColumn(false)] public bool CreateItem { get; set; }

        [ScaffoldColumn(false)] public List<CategoryDto> Categories { get; set; }
        [ScaffoldColumn(false)] public List<ImageDto> Images { get; set; }
        [ScaffoldColumn(false)] public List<DescriptionGroupDto> DescriptionGroups { get; set; }
    }
}