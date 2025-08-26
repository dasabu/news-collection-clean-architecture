using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ITokenProvider
{
    string GenerateToken(User user);
}
