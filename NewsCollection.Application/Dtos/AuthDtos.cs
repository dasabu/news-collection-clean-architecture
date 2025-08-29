using System.ComponentModel.DataAnnotations;

namespace NewsCollection.Application.Dtos;

public record class RegisterRequestDto(
    [Required]
    string Name,
    [Required][EmailAddress]
    string Email,
    [Required][MinLength(6)]
    string Password
);

public record class LoginRequestDto(
    [Required][EmailAddress]
    string Email,
    [Required][MinLength(6)]
    string Password
);

public record class TokenResponseDto(
    string AccessToken
);