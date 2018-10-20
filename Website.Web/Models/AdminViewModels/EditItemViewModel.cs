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
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Display(Name = "Склад")]
        public int Stock { get; set; }
        [Display(Name = "Резерв")]
        public int Reserved { get; set; }
        [Display(Name = "В магазине")]
        [ScaffoldColumn(false)]
        public bool Enabled { get; set; }
        [ScaffoldColumn(false)]
        public DateTimeOffset Added { get; set; }
        [ScaffoldColumn(false)]
        public DateTimeOffset Changed { get; set; }
        [ScaffoldColumn(false)]
        public byte[] Timestamp { get; set; }

        //categories
        [ScaffoldColumn(false)]
        public List<CategoryDTO> Categories { get; set; }
        //images
        [ScaffoldColumn(false)]
        public List<ProductImageDTO> Images { get; set; }
        //descriptions
        [ScaffoldColumn(false)]
        public List<DescriptionGroupDTO> Descriptions { get; set; }

        [ScaffoldColumn(false)]
        public string JsonData { get; set; }
        [ScaffoldColumn(false)]
        public List<DescriptionGroupDTO> AllDescriptionGroups { get; set; }
        [ScaffoldColumn(false)]
        public List<CategoryDTO> AllCategories { get; set; }
    }
}
