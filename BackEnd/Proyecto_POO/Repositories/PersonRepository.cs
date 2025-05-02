using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.Models;

namespace Proyecto_POO.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly ProjectDbContext _projectDbContext;

    public PersonRepository(ProjectDbContext projectDbContext)
    {
        _projectDbContext = projectDbContext;
    }

    public async Task<Person> GetByIdAsync(int id)
    {
        return await _projectDbContext.Persons.FindAsync(id);

    }

    public async Task<List<Person>> GetAllAsync()
    {
        return await _projectDbContext.Persons.ToListAsync();
    }

    public async Task addAsync(Person person)
    {
        _projectDbContext.Persons.Add(person);
        await _projectDbContext.SaveChangesAsync();
    }
}
