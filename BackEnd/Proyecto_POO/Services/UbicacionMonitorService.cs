using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Services;

public class UbicacionMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UbicacionMonitorService> _logger;

    public UbicacionMonitorService(IServiceProvider serviceProvider, ILogger<UbicacionMonitorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var ubicacionRepository = scope.ServiceProvider.GetRequiredService<IUbicacionRepository>();
            var personRepository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
            var geocoding = scope.ServiceProvider.GetRequiredService<IGeocodingService>();

            var personas = await personRepository.GetAllPersonsAsync();

            foreach (var person in personas)
            {
                var ultimaUbicacion = await ubicacionRepository.GetLatestByPersonIdAsync(person.Id);

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

                    await ubicacionRepository.AddAsync(nuevaUbicacion);

                }
                catch (Exception ex)
                {
                    {
                        _logger.LogError(ex, $"No se actualizo la ubicacion de {person.Pnombre} (ID:{person.Id}), coordenadas sin cambios");
                    }

                }

                
            }
            await ubicacionRepository.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
        }
    }
}
