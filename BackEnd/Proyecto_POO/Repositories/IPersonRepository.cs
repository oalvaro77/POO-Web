using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories;

public interface IPersonRepository
{
    Task<Person> GetByIdAsync(int id);
    Task<List<Person>> GetAllAsync();
    Task addAsync(Person person);

}
