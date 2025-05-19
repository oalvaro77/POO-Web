using AutoMapper;
using Microsoft.EntityFrameworkCore.Migrations;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Services;

public class UbicacionServices : IUbicacionServices
{
    private readonly IUbicacionRepository _ubicacionRepository;
    private readonly IPersonRepository _personRepository;
    //private readonly ProjectDbContext _projectDbContext;
    private readonly IGeocodingService _geocodingService;
    private readonly IMapper _mapper;
    private readonly ILogger<UbicacionServices> _logger;

    public UbicacionServices(IUbicacionRepository ubicacionRepository,
        IPersonRepository personRepository,
        IGeocodingService geocodingService,
        IMapper mapper,
        ILogger<UbicacionServices> logger)
    {
        _ubicacionRepository = ubicacionRepository;
        _personRepository = personRepository;
        _geocodingService = geocodingService;
        _mapper = mapper;
        _logger = logger;

    }

    public async Task<(bool success, string messages, Ubicacion Ubicacion)> AddUbicacionAsync(int personaId, string direccion)
    {
        try
        {
            var persona = await _personRepository.GetPersonByIdAsync(personaId);
            if (persona == null)
            {
                return (false, "La persona no existe", null);
            }

            (double latitude, double longitude) coordinates;

            try
            {
                coordinates = await _geocodingService.ObtenerCoordenadas(direccion);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener las coordendas {ex.Message}", null);
            }

            var request = new UbicacionesRequesDTO { PersonaId = personaId, Direccion = direccion };
            var ubicacion = _mapper.Map<Ubicacion>(request);
            ubicacion.Latitud = coordinates.latitude;
            ubicacion.Longitud = coordinates.longitude;
            ubicacion.Fecha = DateTime.UtcNow;

            //_projectDbContext.Ubicaciones.Add(ubicacion);
            await _ubicacionRepository.AddAsync(ubicacion);

            return (true, "Ubicacion anadida con exito", ubicacion);
        }
        catch (Exception ex)
        {
            return (false, $"Error al guardar la ubicacion{ex.Message}", null);

        }
    }

    public async Task<List<UbicacionDTO>> GetHistorialUbicacionesAsync(int personaId)
    {
        try
        {

            var historial = await _ubicacionRepository.GetHistoryByPersonAsync(personaId);
            if (historial == null || !historial.Any())
            {
                _logger.LogWarning($"No se encontro historial para PersonaId: {personaId}", personaId);
                return new List<UbicacionDTO>();
            }

            var historialDTO = _mapper.Map<List<UbicacionDTO>>(historial);
            _logger.LogInformation($"Historial obtenido con extio para PersonaId: {personaId}", personaId);
            return historialDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener el historial de ubicaciones para PersonaId: {personaId}", personaId);
            throw;
        }

    }

    public async Task<List<UbicacionActualDTO>> GetUbicacionesActualesAsync()
    {
        _logger.LogInformation("Obteniendo ubicaciones actuales");

        var personas = await _personRepository.GetPersonsWithLocationsAsync();
        var ubicacions = new List<UbicacionActualDTO>();

        foreach (var persona in personas)
        {
            var ultimaUbicacion = persona.Ubicacions
                .OrderByDescending(u => u.Fecha)
                .FirstOrDefault();

            if (ultimaUbicacion != null)
            {
                ubicacions.Add(new UbicacionActualDTO
                {
                    PersonaId = persona.Id,
                    Nombre = persona.Pnombre,
                    UltimaUbicacion = _mapper.Map<UbicacionDTO>(ultimaUbicacion)
                });
            }
        }

        _logger.LogInformation("Ubicaciones actuales obtenidas", ubicacions.Count);
        return ubicacions;

    }

    public async Task<UbicacionActualDTO> GetUbicacionActualById(int id)
    {
        _logger.LogInformation("Obteniendo ubicacion actual");

        var persona = await _personRepository.GetPersonWithLocation(id);
        if(persona == null)
        {
            _logger.LogWarning("No se encontra la persona con el id", id);
            return null;

        }

        var ultimaUbicacion = persona.Ubicacions
            .OrderByDescending(u => u.Fecha)
            .FirstOrDefault();

        if (ultimaUbicacion == null)
        {
            _logger.LogWarning("No se encontró una ubicación actual para la persona con ID {Id}", id);
            return null;
        }

        var ubicacionDTO = new UbicacionActualDTO
        {
            PersonaId = persona.Id,
            Nombre = persona.Pnombre,
            UltimaUbicacion = _mapper.Map<UbicacionDTO>(ultimaUbicacion)
        };
        _logger.LogInformation("Ubicación actual obtenida para la persona con ID {Id}", id);
        return ubicacionDTO;

    }
}
