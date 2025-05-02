using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories;

public class UbicacionRepository : IUbicacionRepository
{
    private readonly ProjectDbContext _dbContext;
    
    public UbicacionRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Ubicacion> GetLatestByPersonIdAsync(int personId)
    {
        return await _dbContext.Ubicaciones
            .Where(u => u.Idpersona == personId)
            .OrderByDescending(u => u.Fecha)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Ubicacion>> GetHistoryByPersonAsync(int personId)
    {
        return await _dbContext.Ubicaciones
            .Where(u => u.Idpersona == personId)
            .OrderByDescending(u => u.Fecha)
            .ToListAsync();

    }

    public async Task AddAsync(Ubicacion newUbicacion)
    {
        _dbContext.Ubicaciones.Add(newUbicacion);
        await _dbContext.SaveChangesAsync();
    }
}
