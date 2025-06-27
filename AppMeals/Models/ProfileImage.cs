using System.Text.Json.Serialization;

namespace AppMeals.Models
{
    public class ProfileImage
    {
        [JsonPropertyName("image")]
        public string? ImageUrl { get; set; }

        public string? ImagePath => AppConfig.BaseUrl + ImageUrl;
    }
}
