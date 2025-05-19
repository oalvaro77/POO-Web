using Microsoft.AspNetCore.Mvc;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Services.Interfaces;

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
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var apiKey = Request.Headers["X-Api-Key"].FirstOrDefault();

        Console.WriteLine("Api key recibido: " + apiKey);

        if (string.IsNullOrEmpty(apiKey))
        {
            return Unauthorized("Api key is requiered");
        }
        var (accessToken, refreshToken) = await _authService.Login(login.Login, login.Password, apiKey);

        if (accessToken == null || refreshToken == null) return Unauthorized("Invalid Credentials");

        return Ok(new { accessToken, refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
    {
        if (string.IsNullOrEmpty(refreshTokenDTO.RefreshToken))
        {
            return BadRequest("Requiered Refresh Token");
        }

        var (accessToken, refreshToken) = await _authService.RefreshToken(refreshTokenDTO.RefreshToken);
        if (accessToken == null || refreshToken == null) return Unauthorized("Expired or invalid refresh Token");
        return Ok(new { accessToken, refreshToken });
    }
}
