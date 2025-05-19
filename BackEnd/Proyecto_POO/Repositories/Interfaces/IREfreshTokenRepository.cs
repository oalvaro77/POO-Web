using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories.Interfaces;

public interface IREfreshTokenRepository
{
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    Task RevokedRefreshTokenAsync(string token);
}
