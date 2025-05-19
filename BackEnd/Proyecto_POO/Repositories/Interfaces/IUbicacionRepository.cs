using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using System.Threading.Tasks;

namespace Proyecto_POO.Repositories.Interfaces;

public interface IUbicacionRepository
{
    Task<Ubicacion> GetLatestByPersonIdAsync(int personId);
    Task<List<Ubicacion>> GetHistoryByPersonAsync(int personId);
    Task AddAsync(Ubicacion ubicacion);
    Task SaveChangesAsync();
}
