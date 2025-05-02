using Proyecto_POO.Models;

namespace Proyecto_POO.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        string? Login(string login, string password, string apiKey);
    }
}
