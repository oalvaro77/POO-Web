using Proyecto_POO.DTOs;
using Proyecto_POO.Models;

namespace Proyecto_POO.Services
{
    public interface IPersonService
    {
        (User user, string password) CrearPersona(PersonDTO usuarioRegisterDTO);
        IEnumerable<PersonDTO> ObtenerTodasLasPersonas();

        PersonDTO? PersonaPorID(int id);
        PersonDTO? PersonaPorIdentificacion(string identificacion);
        IEnumerable<PersonDTO> PersonaPorEdad(int edad);
        IEnumerable<PersonDTO> PersonaPorPNombre(string Pnombre);
        IEnumerable<PersonDTO> PersonaPorApellido(string PApellido);


        bool ActualizarPersona(Person person);
        bool EliminarPersona(int id);
        bool CambiarPassword(int personaid, string newPasswrod);
        UserDTO GetUserDetails(int personid);


        string GenerarLogin(Person person);
        string GenerarPassword();
        (User user, string plainPassword) GenerarUsuario(Person person);
        string GenerarApiKey();
    }
}
