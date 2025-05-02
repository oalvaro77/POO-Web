using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Services;

namespace Proyecto_POO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonaController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonaController(IPersonService personService)
        {
            _personService = personService;
        }

        
        [HttpPost]
        public IActionResult CrearPersona([FromBody] PersonDTO person)
        {
            var (user, password) = _personService.CrearPersona(person);
            return Ok(new
            {
                mensaje = "Persona creada exitosamente",
                usuario = new
                {
                    idpersona = user.Idpersona,
                    login = user.Login,
                    apiKey = user.ApiKey,
                    password = password
                },
                
            });

        }
        
        [HttpGet]
        public IActionResult ObtenerPersonas()
        {
            var persons = _personService.ObtenerTodasLasPersonas();
            return Ok(persons);
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult ObtenerPersona(int id)
        {
            var person = _personService.PersonaPorID(id);
            if (person == null)
            {
                return NotFound("No encontrado");
            }
            return Ok(person);  
        }

        [HttpGet("by-identication/{identifacion}")]
        public IActionResult ObtenerPorIdentificacion(string identifacion)
        {
            var person = _personService.PersonaPorIdentificacion(identifacion);
            if (person == null)
            {
                return NotFound("Persona no encontrada");

            }

            return Ok(person);
        }

        [HttpGet("by-age/{edad}")]
        public IActionResult ObtenerPorEdad(int edad)
        {
            var persons = _personService.PersonaPorEdad(edad);
            if (persons == null) return NotFound("Persona no encontrada");
            return Ok(persons);
        }

        [HttpGet("by-FName/{nombre}")]
        public IActionResult ObtenerPorNombre(string nombre)
        {
            var persons = _personService.PersonaPorPNombre(nombre);
            if (persons == null) return NotFound("Persona no encontrada");
            return Ok(persons);
        }

        [HttpGet("by-FLast/{apellido}")]
        public IActionResult obtenerPorApellido(string apellido)
        {
            var persons = _personService.PersonaPorApellido(apellido);
            if (persons == null) return NotFound("Persona no encontrada");
            return Ok(persons);
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarPersona(int id, [FromBody] Person person)
        {
            if (id != person.Id) return BadRequest();

            var result = _personService.ActualizarPersona(person);
            return result ? Ok(person) : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult BorrarPersona(int id)
        {
            var result = _personService.EliminarPersona(id);
            return result ? Ok("Eliminada exitosamente") : NotFound("Persona no encontrada");
        }

        [HttpPost("change-password/{id}")]
        public  IActionResult CambiarPassword(int id, [FromBody] string newPassword)
        {
            var result =  _personService.CambiarPassword(id, newPassword);
            return result? Ok("Pasaporte cambiado"): NotFound();
        }
        
        [HttpGet("User-details/{id}"), Authorize]
        public IActionResult GetUserDetails(int id)
        {
            var user = _personService.GetUserDetails(id);
          
            return user == null ? NotFound("Usuario no encontrado") : Ok(user);
        }


    }
}
