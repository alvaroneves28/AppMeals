
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppMeals.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [JsonPropertyName("urlImage")]
        public string? ImageUrl { get; set; }

    }
}
