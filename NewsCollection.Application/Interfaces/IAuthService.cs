using NewsCollection.Application.Dtos;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterRequestDto request);
    Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
}
