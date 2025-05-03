using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByLoginAsync(string login);
    Task<User?> GetUserByApiKeyAsync(string apiKey);
}
