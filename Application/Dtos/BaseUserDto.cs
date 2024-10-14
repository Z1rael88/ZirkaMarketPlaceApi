using Domain.Enums;

namespace Application.Dtos;

public class BaseUserDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int Age { get; set; }
    public string UserName { get; set; }
    public required Role Role { get; set; }
}