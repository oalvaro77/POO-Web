using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;
using System.Text.Json;

namespace Proyecto_POO.Repositories;

public class RefreshTokenRepository : IREfreshTokenRepository
{
    private readonly ProjectDbContext _projectDbContext;
    private readonly IDistributedCache _cache;

    public RefreshTokenRepository(ProjectDbContext projectDbContext, IDistributedCache distributedCache)
    {
        _projectDbContext = projectDbContext;
        _cache = distributedCache;
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        _projectDbContext.RefreshTokens.Add(refreshToken);
        await _projectDbContext.SaveChangesAsync();

        //Save in cache
        var cacheDTO = new RefreshTokenCacheDTO
        {
            Id = refreshToken.Id,
            UserId = refreshToken.UserId,
            Token = refreshToken.Token,
            Expires = refreshToken.Expires,
            IsRevoked = refreshToken.IsRevoked
        };
        var cacheKey = $"RefreshToken_{refreshToken.Token}";
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = refreshToken.Expires
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cacheDTO), cacheOptions);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        // Try get to cache
        try
        {
            var cacheKey = $"RefreshToken_{token}";
            string cachedToken;
            try
            {
                cachedToken = await _cache.GetStringAsync(cacheKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al acceder al cache, {ex}");
                cachedToken = null; // Fallback to database
            }

            if (!string.IsNullOrEmpty(cachedToken))
            {
                try
                {
                    var cacheDto = JsonSerializer.Deserialize<RefreshToken>(cachedToken);
                    if (cacheDto != null)
                    {
                        return new RefreshToken
                        {
                            Id = cacheDto.Id,
                            UserId = cacheDto.UserId,
                            Token = cacheDto.Token,
                            Expires = cacheDto.Expires,
                            IsRevoked = cacheDto.IsRevoked
                        };
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error al deserialzar el token del cache. {ex.Message}");
                }
            }

            try
            {
                return await _projectDbContext.RefreshTokens
                    .Where(rt => rt.Token == token && !rt.IsRevoked && rt.Expires > DateTime.UtcNow)
                    .Select(rt => new RefreshToken
                    {
                        Id = rt.Id,
                        UserId = rt.UserId,
                        Token = rt.Token,
                        Expires = rt.Expires,
                        IsRevoked = rt.IsRevoked,
                        User = new User { Idpersona = rt.User.Idpersona, Login = rt.User.Login }
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consulta el refresh Token en la base de datos.{ex.Message}");
                return null;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado en GetRefreshTokenAsync, {ex.Message}");
            return null ;
        }
        

        //if (!string.IsNullOrEmpty(cachedToken))
        //{
        //    var cacheDto = JsonSerializer.Deserialize<RefreshToken>(cachedToken);
        //    if (cacheDto != null)
        //    {
        //        return new RefreshToken
        //        {
        //            Id = cacheDto.Id,
        //            UserId = cacheDto.UserId,
        //            Token = cacheDto.Token,
        //            Expires = cacheDto.Expires,
        //            IsRevoked = cacheDto.IsRevoked
        //        };
        //    }
        //}


        
        //This ovoid loading unnecesary data
           
            //.Include(rt => rt.User)
            //.FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.Expires >  DateTime.UtcNow);
        //Check Possible Null reference return *****************************
    }

    public async Task RevokedRefreshTokenAsync(string token)
    {
        var refreshToken = await _projectDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await _projectDbContext.SaveChangesAsync();

            //Invalite cache

            var cacheKey = $"RefreshToken_{token}";
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
