using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Service.DTO
{
    public class ProductDto
    {
        public ProductDto()
        {
            Categories = new List<CategoryDto>();
            Images = new List<ProductImageDto>();
            Descriptions = new List<DescriptionGroupDto>();
        }
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Артикул")]
        public int Code { get; set; }
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
        [Display(Name = "Склад")]
        public int Stock { get; set; }
        [Display(Name = "Резерв")]
        public int Reserved { get; set; }
        [Display(Name = "Доступен")]
        public bool Available { get; set; }
        [Display(Name = "Добавлен")]
        public DateTimeOffset Created { get; set; }
        [Display(Name = "Редактирован")]
        public DateTimeOffset Changed { get; set; }
        public byte[] Timestamp { get; set; }

        //categories
        public List<CategoryDto> Categories { get; set; }
        //images
        public List<ProductImageDto> Images { get; set; }
        //descriptions
        public List<DescriptionGroupDto> Descriptions { get; set; }
    }
}
