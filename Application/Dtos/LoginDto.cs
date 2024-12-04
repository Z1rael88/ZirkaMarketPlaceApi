namespace Application.Dtos;

public class LoginDto
{
    public required string UserName { get; set; }
    

    public required string Password { get; set; }
    public required bool IsGoogleLogin { get; set; }
    public required string GoogleToken { get; set; }
}
