using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace NewsCollection.Infrastructure.Repositories;

public class EmailLogRepository(NewsCollectionContext context) : IEmailLogRepository
{
    public async Task LogEmailAsync(
        int userId, string type, bool success, string? errorMessage = null
    ) {
        await context.EmailLogs.AddAsync(new EmailLog
        {
            UserId = userId,
            Type = type,
            SentAt = DateTime.UtcNow,
            Success = success,
            ErrorMessage = errorMessage
        });
        await context.SaveChangesAsync();
    }
}