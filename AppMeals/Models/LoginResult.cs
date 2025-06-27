using System.Text.Json.Serialization;

public class LoginResult
{
    [JsonPropertyName("accesstoken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("userid")]
    public int? UserId { get; set; }

    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    [JsonPropertyName("contact")]
    public string? Contact { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}