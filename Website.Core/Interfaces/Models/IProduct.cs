﻿using System;

namespace Website.Core.Interfaces.Models
{
    public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        int Code { get; set; }
        decimal Price { get; set; }
        int Stock { get; set; }
        int Reserved { get; set; }
        bool Available { get; set; }
        DateTimeOffset Created { get; set; }
        DateTimeOffset Changed { get; set; }
        byte[] Timestamp { get; set; }
    }
}