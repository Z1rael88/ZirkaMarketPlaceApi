using Newtonsoft.Json;

namespace Application.Dtos;

public class GoogleTokenResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }

    [JsonProperty("id_token")] public string IdToken { get; set; }

    [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
}