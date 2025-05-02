using Proyecto_POO.Models;

namespace Proyecto_POO.Services;

public interface IUbicacionServices
{
    Task<(bool success, string messages, Ubicacion Ubicacion)> AddUbicacionAsync(int personaId, string direccion);
}
