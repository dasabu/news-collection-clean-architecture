namespace NewsCollection.Application.Dtos;

public record class RegisterRequestDto(
    string Name,
    string Email,
    string Password
);

public record class LoginRequestDto(
    string Email,
    string Password
);

public record class TokenResponseDto(
    string AccessToken
);