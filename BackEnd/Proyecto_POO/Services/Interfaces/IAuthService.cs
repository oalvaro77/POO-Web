using Proyecto_POO.Models;

namespace Proyecto_POO.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        Task<(string AccessToken, string RefreshToken)> Login(string login, string password, string apiKey);
        Task<(string AccesToken, String RefreshToken)> RefreshToken(string refreshToken);
    }
}
