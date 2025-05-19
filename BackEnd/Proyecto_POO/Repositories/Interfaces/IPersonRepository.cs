using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories.Interfaces;

public interface IPersonRepository
{
    Task AddPersonAsync(Person person);
    Task<User> AddUserAsync(User user);
    Task<Person> GetPersonByIdAsync(int id);
    Task<List<Person>> GetAllPersonsAsync();
    Task<Person?> GetPersonByIdentification(string identification);
    Task<List<Person>> GetPersonByAge(int age);
    Task<List<Person>> GetPersonByPname(string pNombre);
    Task<List<Person>> GetPersonByPApellido(string pApellido);
    Task<bool> DeletePersonAsync(int id);
    Task<bool> UpdatePersonAsync(Person person);
    Task<User?> GetUserByPerson(int personId);
    Task<bool> UpdatUserAsync(User user);
    Task SaveChangesAsync();

    Task<List<Person>> GetPersonsWithLocationsAsync();

    Task<Person> GetPersonWithLocation(int id);
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);



}
