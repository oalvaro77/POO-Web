using AutoMapper;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories;

namespace Proyecto_POO.Services;

public class UbicacionServices : IUbicacionServices
{
    private readonly IUbicacionRepository _ubicacionRepository;
    private readonly IPersonRepository _personRepository;
    //private readonly ProjectDbContext _projectDbContext;
    private readonly IGeocodingService _geocodingService;
    private readonly IMapper _mapper;

    public UbicacionServices(IUbicacionRepository ubicacionRepository, IPersonRepository personRepository,  IGeocodingService geocodingService, IMapper mapper)
    {
        _ubicacionRepository = ubicacionRepository;
        _personRepository = personRepository;
        _geocodingService = geocodingService;
        _mapper = mapper;
    }

    public async Task<(bool success, string messages, Ubicacion Ubicacion)> AddUbicacionAsync(int personaId, string direccion)
    {
        try
        {
            var persona = await _personRepository.GetByIdAsync(personaId);
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
}
