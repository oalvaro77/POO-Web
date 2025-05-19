using Proyecto_POO.DTOs;
using Proyecto_POO.Models;

namespace Proyecto_POO.Services.Interfaces;

public interface IUbicacionServices
{
    Task<(bool success, string messages, Ubicacion Ubicacion)> AddUbicacionAsync(int personaId, string direccion);
    Task<List<UbicacionActualDTO>> GetUbicacionesActualesAsync();
    Task<List<UbicacionDTO>> GetHistorialUbicacionesAsync(int personaId);

    Task<UbicacionActualDTO> GetUbicacionActualById(int id);
}
