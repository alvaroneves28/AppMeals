using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppMeals.Models
{
    internal class Token
    {
        [JsonPropertyName("accesstoken")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("tokentype")]
        public string? TokenType { get; set; }
        [JsonPropertyName("userid")]
        public int? UserId { get; set; }
        [JsonPropertyName("username")]
        public string? UserName { get; set; }
    }
}
