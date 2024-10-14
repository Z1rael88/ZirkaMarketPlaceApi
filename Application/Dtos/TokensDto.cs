namespace Application.Dtos;

public class TokensDto
{
    public required string AccessToken { get; set; }

    public required string RefreshToken { get; set; }

    public required DateTime AccessTokenExpirationDate { get; set; }

    public required DateTime RefreshTokenExpirationDate { get; set; }
}