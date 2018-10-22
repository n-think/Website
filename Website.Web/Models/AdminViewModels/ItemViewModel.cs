using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Service.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class ItemViewModel
    {
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
        [Display(Name = "В магазине")]
        public bool Enabled { get; set; }
        [Display(Name = "Добавлен")]
        public DateTimeOffset Added { get; set; }
        [Display(Name = "Редактирован")]
        public DateTimeOffset Changed { get; set; }
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
