using Microsoft.AspNetCore.Identity;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Services;

public class AuthService(IAuthRepository repository, ITokenProvider tokenProvider) : IAuthService
{
    public async Task<User?> RegisterAsync(RegisterRequestDto request)
    {
        if (await repository.UserExistsByEmailAsync(request.Email))
        {
            return null;
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };
        user.Password = new PasswordHasher<User>().HashPassword(user, request.Password);

        await repository.AddUserAsync(user);
        await repository.SaveChangesAsync();
        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await repository.GetUserByEmailAsync(request.Email);
        if (user == null
            || new PasswordHasher<User>().VerifyHashedPassword(
                user, user.Password, request.Password
            ) == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new TokenResponseDto(tokenProvider.GenerateToken(user));
    }
}
