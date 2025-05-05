using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Services.Interfaces;

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
        public async Task<IActionResult> CrearPersona([FromBody] PersonDTO person)
        {
            var (user, password) =await  _personService.CrearPersona(person);
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
        public async Task<IActionResult> ObtenerPersonas()
        {
            var persons = await _personService.ObtenerTodasLasPersonas();
            return Ok(persons);
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> ObtenerPersona(int id)
        {
            var person = await _personService.PersonaPorID(id);
            if (person == null)
            {
                return NotFound("No encontrado");
            }
            return Ok(person);  
        }

        [HttpGet("by-identication/{identifacion}")]
        public async Task<IActionResult> ObtenerPorIdentificacion(string identifacion)
        {
            var person = await _personService.PersonaPorIdentificacion(identifacion);
            if (person == null)
            {
                return NotFound("Persona no encontrada");

            }

            return Ok(person);
        }

        [HttpGet("by-age/{edad}")]
        public async Task<IActionResult> ObtenerPorEdad(int edad)
        {
            var persons = await _personService.PersonaPorEdad(edad);
            if (persons == null) return NotFound("Persona no encontrada");
            return Ok(persons);
        }

        [HttpGet("by-FName/{nombre}")]
        public async Task<IActionResult> ObtenerPorNombre(string nombre)
        {
            var persons = await _personService.PersonaPorPNombre(nombre);
            if (persons == null || !persons.Any()) return NotFound("Persona no encontrada");
            return Ok(persons);
        }

        [HttpGet("by-FLast/{apellido}")]
        public async Task<IActionResult> obtenerPorApellido(string apellido)
        {
            var persons = await _personService.PersonaPorApellido(apellido);
            if (persons == null || !persons.Any()) return NotFound("Persona no encontrada");
            return Ok(persons);
        }

        [HttpPut("{id}")]
        public async Task< IActionResult> ActualizarPersona(int id, [FromBody] Person person)
        {
            if (id != person.Id) return BadRequest();

            var result = await _personService.ActualizarPersona(person);
            return result ? Ok(person) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> BorrarPersona(int id)
        {
            var result = await _personService.EliminarPersona(id);
            return result ? Ok("Eliminada exitosamente") : NotFound("Persona no encontrada");
        }

        [HttpPost("change-password/{id}")]
        public  async Task<IActionResult> CambiarPassword(int id, [FromBody] string newPassword)
        {
            var result = await _personService.CambiarPassword(id, newPassword);
            return result? Ok("Password cambiado"): NotFound();
        }
        
        [HttpGet("User-details/{id}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _personService.GetUserDetails(id);
          
            return user == null ? NotFound("Usuario no encontrado") : Ok(user);
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _personService.GetAllUsers();

            return Ok(users);
        }

    }
}
