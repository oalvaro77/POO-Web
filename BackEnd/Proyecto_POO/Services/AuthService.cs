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
    private readonly IConfiguration _config;
    private readonly IPersonService _personService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration, IPersonService personService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _config = configuration;
        _personService = personService;
    }

    public async Task<string?> Login(string login, string password, string apiKey)
    {
        var user = await _userRepository.GetUserByLoginAsync(login);
        if (user == null || user.ApiKey != apiKey)
        {
            Console.WriteLine("Login incorrecto");
            return null;
        }
        Console.WriteLine("Login correcto");
        Console.WriteLine("Apikey esperado:" + user.ApiKey);
        Console.WriteLine("ApiKey recibido: " + apiKey);
        var validPassword = _passwordHasher.VerifyPassword(user.Password, password);

        if (!validPassword)
        {
            Console.WriteLine("Contrasena incorrecta");
            return null;
        }
        Console.WriteLine("Login exitoso");
        return GenerateJwtToken(user);
    }

    public string GenerateJwtToken(User user)
    {


        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("id", user.Idpersona.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
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
}
