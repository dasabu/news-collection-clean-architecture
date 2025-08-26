using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
}
