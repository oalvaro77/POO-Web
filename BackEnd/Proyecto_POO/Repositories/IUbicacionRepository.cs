using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories;

public interface IUbicacionRepository
{
    Task<Ubicacion> GetLatestByPersonIdAsync(int  personId);
    Task<List<Ubicacion>> GetHistoryByPersonAsync(int personId);
    Task AddAsync(Ubicacion ubicacion);
}
