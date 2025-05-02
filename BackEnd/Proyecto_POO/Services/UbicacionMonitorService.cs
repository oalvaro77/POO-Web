using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.Models;

namespace Proyecto_POO.Services;

public class UbicacionMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public UbicacionMonitorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
            var geocoding = scope.ServiceProvider.GetRequiredService<IGeocodingService>();

            var personas = await context.Persons
                .Include(p => p.Ubicacions)
                .ToListAsync(stoppingToken);

            foreach (var person in personas)
            {
                var ultimaUbicacion = person.Ubicacions
                    .OrderByDescending(u => u.Fecha)
                    .FirstOrDefault();

                if (ultimaUbicacion == null) continue;
                try
                {
                    var (lat, lng) = await geocoding.ObtenerCoordenadas(ultimaUbicacion.Direccion);

                    var nuevaUbicacion = new Ubicacion
                    {
                        Idpersona = person.Id,
                        Direccion = ultimaUbicacion.Direccion,
                        Latitud = lat,
                        Longitud = lng,
                        Fecha = DateTime.UtcNow,

                    };

                    context.Ubicaciones.Add(nuevaUbicacion);
                }
                catch (Exception ex)
                {
                    {
                        Console.WriteLine($"Error obteniendo coordenadas para {person.Pnombre}: {ex.Message}");
                    }

                }

                await context.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
        }
    }
}
    