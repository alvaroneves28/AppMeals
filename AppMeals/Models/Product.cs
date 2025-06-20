﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppMeals.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? Detail { get; set; }

        [JsonPropertyName("urlImage")]
        public string? ImageUrl { get; set; }
    }
}
