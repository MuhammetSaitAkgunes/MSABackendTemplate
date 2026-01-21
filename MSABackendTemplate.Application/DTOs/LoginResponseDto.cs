namespace MSABackendTemplate.Application.DTOs;

public class LoginResponseDto
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
