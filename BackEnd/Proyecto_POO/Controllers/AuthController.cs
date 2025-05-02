using Microsoft.AspNetCore.Mvc;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Services;

namespace Proyecto_POO.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
   
    public AuthController(IAuthService authService)
    {
        _authService = authService;
   
    }



    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginDTO login)
    {
        var apiKey = Request.Headers["X-Api-Key"].FirstOrDefault();

        Console.WriteLine("Api key recibido: " + apiKey);

        if (string.IsNullOrEmpty(apiKey))
        {
            return Unauthorized();
        }
        var token = _authService.Login(login.Login, login.Password, apiKey);

        if (token == null) return Unauthorized("Credenciales invalidas");

        return Ok(new { token });
    }
}
