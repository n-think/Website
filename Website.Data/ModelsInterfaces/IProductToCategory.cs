﻿using Website.Data.EF.Models;

namespace Website.Data.ModelsInterfaces
{
    public interface IProductToCategory
    {
        int ProductId { get; set; }
        int CategoryId { get; set; }
    }
}