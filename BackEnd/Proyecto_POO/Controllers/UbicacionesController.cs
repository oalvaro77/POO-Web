using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Services;

namespace Proyecto_POO.Controllers;


[ApiController]
[Route("api/[controller]")]

public class UbicacionesController : ControllerBase
{
    private readonly ProjectDbContext _projectDbContext;
    private readonly IGeocodingService _geocodingServices;
    private readonly IUbicacionServices _ubicacionServices;

    public UbicacionesController(ProjectDbContext projectDbContext, IGeocodingService geocodingService, IUbicacionServices ubicacionServices)
    {
        _projectDbContext = projectDbContext;
        _geocodingServices = geocodingService;
        _ubicacionServices = ubicacionServices;
    }

    [HttpGet("Actuales")]
    public async Task<IActionResult> GetUbicacionesActuales()
    {
        var actuales = await _projectDbContext.Persons
            .Include(p => p.Ubicacions)
            .Select(p => new
            {
                PersonaId = p.Id,
                Nombre = p.Pnombre,
                UltimaUbicacion = p.Ubicacions
                .OrderByDescending(u => u.Fecha)
                .Select(u => new
                {
                    u.Direccion,
                    u.Latitud,
                    u.Longitud,
                    u.Fecha,
                }).FirstOrDefault()
            }).ToListAsync();

        return Ok(actuales);
    }

    [HttpGet("historial/{personaID}")]
    public async Task<IActionResult> HistorialPersona(int personaID)
    {
        var historial = await _projectDbContext.Ubicaciones
        .Where(u => u.Idpersona == personaID)
        .OrderByDescending(u => u.Fecha)
        .Select(u => new
        {
            u.Direccion,
            u.Latitud,
            u.Longitud,
            u.Fecha
        }).ToListAsync();

        if (historial == null || !historial.Any())
        {
            return NotFound("No se encontró historial para esta persona.");
        }

        return Ok(historial);
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
