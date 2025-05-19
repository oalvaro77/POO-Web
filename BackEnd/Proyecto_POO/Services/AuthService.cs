using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proyecto_POO.Data;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;
using Proyecto_POO.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Proyecto_POO.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    private readonly IREfreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _config;
    private readonly IPersonService _personService;

    public AuthService(IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        IConfiguration configuration, 
        IPersonService personService,
        ILogger<AuthService> logger,
        IREfreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _config = configuration;
        _personService = personService;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<(string AccessToken, string RefreshToken)> Login(string login, string password, string apiKey)
    {
        try
        {

            var user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null || user.ApiKey != apiKey)
            {
                Console.WriteLine("Login incorrecto");
                return (null, null);
            }
            //Console.WriteLine("Login correcto");
            //Console.WriteLine("Apikey esperado:" + user.ApiKey);
            //Console.WriteLine("ApiKey recibido: " + apiKey);
            var validPassword = _passwordHasher.VerifyPassword(user.Password, password);

            if (!validPassword)
            {
                Console.WriteLine("Contrasena incorrecta");
                return (null, null);
            }
            Console.WriteLine("Login exitoso");
            var accessToken = GenerateJwtToken(user);
            var refreshToken = Guid.NewGuid().ToString();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Idpersona,
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);
            return (accessToken, refreshToken);
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine($"Error en la base de datos durante el login: {e.Message}");
            throw new ApplicationException("Internal server error");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error Inesperado durante el login, {e.Message}, StackTrace: {e.StackTrace}");
            throw new ApplicationException("Internal error server", e);
        }

    }

    public string GenerateJwtToken(User user)
    {


        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("id", user.Idpersona.ToString())
        };

        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey)) throw new InvalidOperationException("Jwt Key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(string AccesToken, string RefreshToken)> RefreshToken(string refreshToken)
    {
        try
        {
            var tokenEntity = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (tokenEntity == null)
            {
                return (null, null);
            }

            var user = tokenEntity.User;
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();

            //Revoked before token
            await _refreshTokenRepository.RevokedRefreshTokenAsync(refreshToken);

            //Generate a new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Idpersona,
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddRefreshTokenAsync(newRefreshTokenEntity);
            return (newAccessToken, newRefreshToken);

        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error en la base de datos durante el refresh. {ex.Message}");
            throw new ApplicationException("Error al procesar el refresh Token intente de nuevo");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado durante el refrest. {ex.Message}");
            throw new ApplicationException("Internal server Error");
        }
   }
}
