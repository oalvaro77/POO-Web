using Proyecto_POO.DTOs;
using Proyecto_POO.Models;

namespace Proyecto_POO.Services.Interfaces
{
    public interface IPersonService
    {
        Task<(User user, string password)> CrearPersona(PersonDTO usuarioRegisterDTO);
        Task<IEnumerable<PersonDTO>> ObtenerTodasLasPersonas();

        Task<PersonDTO?> PersonaPorID(int id);
        Task<PersonDTO?> PersonaPorIdentificacion(string identificacion);
        Task<List<PersonDTO>> PersonaPorEdad(int edad);
        Task<IEnumerable<PersonDTO>> PersonaPorPNombre(string Pnombre);
        Task<IEnumerable<PersonDTO>> PersonaPorApellido(string PApellido);

        Task<IEnumerable<UserDTO>> GetAllUsers();


        Task<bool> ActualizarPersona(Person person);
        Task<bool> EliminarPersona(int id);
        Task<bool> CambiarPassword(int personaid, string newPasswrod);
        Task<UserDTO> GetUserDetails(int personid);


        string GenerarLogin(Person person);
        string GenerarPassword();
        (User user, string plainPassword) GenerarUsuario(Person person);
        string GenerarApiKey();
    }
}
