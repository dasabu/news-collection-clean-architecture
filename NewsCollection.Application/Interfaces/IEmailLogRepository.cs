using System;

namespace NewsCollection.Application.Interfaces;

public interface IEmailLogRepository
{
    Task LogEmailAsync(int userId, string type, bool success, string? errorMessage = null);
}
