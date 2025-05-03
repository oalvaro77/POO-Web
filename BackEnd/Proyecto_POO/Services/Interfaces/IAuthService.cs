using Proyecto_POO.Models;

namespace Proyecto_POO.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        Task<string?> Login(string login, string password, string apiKey);
    }
}
