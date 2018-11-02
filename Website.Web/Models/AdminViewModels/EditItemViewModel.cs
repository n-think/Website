using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Website.Service.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class EditItemViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Артикул")]
        public int Code { get; set; }
        [Display(Name = "Цена")]
        [Range(0, int.MaxValue, ErrorMessage = "Цена не может быть меньше нуля.")]
        [DataType(DataType.Currency)]
        [RegularExpression("\\d+([,.])?\\d+", ErrorMessage = "Поле должно содержать цифры, разделенные запятой или точкой.")] // custom binder converts , -> .
        public decimal Price { get; set; }
        [Display(Name = "Склад")]
        [Range(0, int.MaxValue, ErrorMessage = "Кол-во на складе не может быть меньше нуля.")]
        public int Stock { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Резерв не может быть меньше нуля.")]
        [Display(Name = "Резерв")]
        public int Reserved { get; set; }
        [Display(Name = "Доступен")]
        [ScaffoldColumn(false)]
        public bool Available { get; set; }
        [ScaffoldColumn(false)]
        public DateTimeOffset Created { get; set; }
        [ScaffoldColumn(false)]
        public DateTimeOffset Changed { get; set; }
        [ScaffoldColumn(false)]
        public byte[] Timestamp { get; set; }

        //categories
        [ScaffoldColumn(false)]
        public List<CategoryDto> Categories { get; set; }
        //images
        [ScaffoldColumn(false)]
        public List<ProductImageDto> Images { get; set; }
        //descriptions
        [ScaffoldColumn(false)]
        public List<DescriptionGroupDto> Descriptions { get; set; }
    }
}
