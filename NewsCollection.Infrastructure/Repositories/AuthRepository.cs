using System;
using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class AuthRepository(NewsCollectionContext context) : IAuthRepository
{
    public async Task<User?> GetUserByEmailAsync(string email) => await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    public async Task<bool> UserExistsByEmailAsync(string email) => await context.Users.AnyAsync(u => u.Email == email);
    public async Task AddUserAsync(User user) => await context.Users.AddAsync(user);
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}