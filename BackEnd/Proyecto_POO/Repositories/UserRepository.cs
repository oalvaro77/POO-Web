using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;

namespace Proyecto_POO.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ProjectDbContext _dbContext;

    public UserRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Login == login);
    }

    public async Task<User?> GetUserByApiKeyAsync(string apiKey)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey);
    }
}
