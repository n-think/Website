using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Website.Service.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Артикул")]
        public int Code { get; set; }
        [Display(Name = "Цена")]
        public double Price { get; set; }
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

        //category
        [Display(Name = "Категория")]
        public string CategoryName { get; set; }
        //images
        public byte[][] Images { get; set; }
    }
}
