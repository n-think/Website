using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Website.Web.Models.DTO;

namespace Website.Web.Models.HomeViewModels
{
    public class ViewItemModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Артикул")]

        public int Code { get; set; }

        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Display(Name = "Склад")]
        public int Stock { get; set; }

        [ScaffoldColumn(false)]
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        [ScaffoldColumn(false)]
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        [ScaffoldColumn(false)]
        public List<DescriptionGroupDto> DescriptionGroups { get; set; } = new List<DescriptionGroupDto>();
    }
}