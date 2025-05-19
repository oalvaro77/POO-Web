using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Controllers;


[ApiController]
[Route("api/[controller]")]

public class UbicacionesController : ControllerBase
{
    //private readonly ProjectDbContext _projectDbContext;
    private readonly IGeocodingService _geocodingServices;
    private readonly IUbicacionServices _ubicacionServices;
    private readonly ILogger<UbicacionesController> _logger;

    public UbicacionesController(ProjectDbContext projectDbContext, IGeocodingService geocodingService, IUbicacionServices ubicacionServices, ILogger<UbicacionesController> logger)
    {
        //_projectDbContext = projectDbContext;
        _geocodingServices = geocodingService;
        _ubicacionServices = ubicacionServices;
        _logger = logger;
    }

    [HttpGet("Actuales")]
    public async Task<IActionResult> GetUbicacionesActuales()
    {
        try
        {
            _logger.LogInformation("Obteniendo ubicaciones actuales");

            var ubicaciones = await _ubicacionServices.GetUbicacionesActualesAsync();
            if (!ubicaciones.Any())
            {
                _logger.LogWarning("No se encontraron personas con ubicaciones actuales.");
                return NotFound("No se encontraron personas con ubicaciones actuales.");
            }

            _logger.LogInformation("Ubicaciones actuales devueltas con éxito. Total: {Count}", ubicaciones.Count);
            return Ok(ubicaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ubicaciones actuales.");
            return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
        }
    }
    [HttpGet("Actuales/{personaID}")]
    public async Task<IActionResult> GetUbicacionActualById(int personaID)
    {
        try
        {
            if (personaID <= 0)
            {
                _logger.LogWarning("Id de persona invalido: {PersonaId}", personaID);
                return BadRequest("El Id de la persona deber ser mayor de 0");
            }
            _logger.LogInformation("Obteniendo  Ubicacion actual para PersonaId: {PersonaId}", personaID);

            var ubicacion = await _ubicacionServices.GetUbicacionActualById(personaID);
            if (ubicacion == null)
            {
                _logger.LogWarning("No se encontró historial para PersonaId: {PersonaId}", personaID);
                return NotFound("No se encontró historial para esta persona.");
            }

            _logger.LogInformation("Ubicacion actual devuelta con éxito para PersonaId: {PersonaId}", personaID);
            return Ok(ubicacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el historial de ubicaciones para PersonaId: {PersonaId}", personaID);
            return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
        }
    }
        //var actuales = await _projectDbContext.Persons
        //    .Include(p => p.Ubicacions)
        //    .Select(p => new
        //    {
        //        PersonaId = p.Id,
        //        Nombre = p.Pnombre,
        //        UltimaUbicacion = p.Ubicacions
        //        .OrderByDescending(u => u.Fecha)
        //        .Select(u => new
        //        {
        //            u.Direccion,
        //            u.Latitud,
        //            u.Longitud,
        //            u.Fecha,
        //        }).FirstOrDefault()
        //    }).ToListAsync();

        //return Ok(actuales);
    

    [HttpGet("historial/{personaID}")]
    public async Task<IActionResult> HistorialPersona(int personaID)
    {
        try
        {
            if (personaID <= 0)
            {
                _logger.LogWarning("Id de persona invalido: {PersonaId}", personaID);
                return BadRequest("El Id de la persona deber ser mayor de 0");
            }
            _logger.LogInformation("Obteniendo historial de ubicaciones para PersonaId: {PersonaId}", personaID);

            var historial = await _ubicacionServices.GetHistorialUbicacionesAsync(personaID);
            if (!historial.Any())
            {
                _logger.LogWarning("No se encontró historial para PersonaId: {PersonaId}", personaID);
                return NotFound("No se encontró historial para esta persona.");
            }

            _logger.LogInformation("Historial devuelto con éxito para PersonaId: {PersonaId}. Total de ubicaciones: {Count}", personaID, historial.Count);
            return Ok(historial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el historial de ubicaciones para PersonaId: {PersonaId}", personaID);
            return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
        }
        //var historial = await _projectDbContext.Ubicaciones
        //.Where(u => u.Idpersona == personaID)
        //.OrderByDescending(u => u.Fecha)
        //.Select(u => new
        //{
        //    u.Direccion,
        //    u.Latitud,
        //    u.Longitud,
        //    u.Fecha
        //}).ToListAsync();

        //if (historial == null || !historial.Any())
        //{
        //    return NotFound("No se encontró historial para esta persona.");
        //}

        //return Ok(historial);
    }

    //[HttpPost("Actualizar direccion")]
    //public async Task<IActionResult> ActualizarDireccion(int personaID, [FromBody] string nuevaDireccion)
    //{
    //    var persona = await _projectDbContext.Persons.Include(p => p.Ubicacions).FirstOrDefaultAsync(p => p.Id == personaID);

    //    if (persona == null)
    //    {
    //        return NotFound("Persona no encontrada");
    //    }

    //    var nuevaUbicacion = new Ubicacion
    //    {
    //        Idpersona = personaID,
    //        Direccion = nuevaDireccion,
    //        Fecha = DateTime.UtcNow,
    //    };

    //    _projectDbContext.Ubicaciones.Add(nuevaUbicacion);
    //    await _projectDbContext.SaveChangesAsync();

    //    return Ok();

    //}

    [HttpPost]
    public async Task<IActionResult> AddUbicacion([FromBody] UbicacionesRequesDTO request)
    {
        //Validar el modelo recibido
        if (request == null || string.IsNullOrEmpty(request.Direccion))
        {
            return BadRequest("La direccion es requerida");
        }

        //Obtener las coordenadas usando el servicio de geolocalizacion "GeocodingServices"
        var (success, message, ubicacion) = await _ubicacionServices.AddUbicacionAsync(request.PersonaId, request.Direccion);
        
        if (!success) return BadRequest(message);

        var response = new
        {
            Id = ubicacion.Id,
            Direccion = ubicacion.Direccion,
            Latitud = ubicacion.Latitud,
            Longitud = ubicacion.Longitud,
            Fecha = ubicacion.Fecha
        };

        return Ok(response);



    }
}
