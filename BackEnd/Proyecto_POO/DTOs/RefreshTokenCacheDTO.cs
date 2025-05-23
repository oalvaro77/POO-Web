﻿namespace Proyecto_POO.DTOs;

public class RefreshTokenCacheDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
}
