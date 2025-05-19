using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Tests.IntegrationTest;

internal class FakePersonService : IPersonService
{
    public Task<bool> ActualizarPersona(Person person)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CambiarPassword(int personaid, string newPasswrod)
    {
        throw new NotImplementedException();
    }

    public Task<(User user, string password)> CrearPersona(PersonDTO usuarioRegisterDTO)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EliminarPersona(int id)
    {
        throw new NotImplementedException();
    }

    public string GenerarApiKey()
    {
        throw new NotImplementedException();
    }

    public string GenerarLogin(Person person)
    {
        throw new NotImplementedException();
    }

    public string GenerarPassword()
    {
        throw new NotImplementedException();
    }

    public (User user, string plainPassword) GenerarUsuario(Person person)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDTO>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> GetUserDetails(int personid)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PersonDTO>> ObtenerTodasLasPersonas()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PersonDTO>> PersonaPorApellido(string PApellido)
    {
        throw new NotImplementedException();
    }

    public Task<List<PersonDTO>> PersonaPorEdad(int edad)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDTO?> PersonaPorID(int id)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDTO?> PersonaPorIdentificacion(string identificacion)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PersonDTO>> PersonaPorPNombre(string Pnombre)
    {
        throw new NotImplementedException();
    }
}
