using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;

namespace Proyecto_POO.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly ProjectDbContext _projectDbContext;

    public PersonRepository(ProjectDbContext projectDbContext)
    {
        _projectDbContext = projectDbContext;
    }

    

    public async Task AddPersonAsync(Person person)
    {
        _projectDbContext.Persons.Add(person);
    }

    public async Task<User> AddUserAsync(User user)
    {
      _projectDbContext.Users.Add(user);
      return user;
    }

    public async Task<Person> GetPersonByIdAsync(int id)
    {
        return await _projectDbContext.Persons
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Person>> GetAllPersonsAsync()
    {
        return await _projectDbContext.Persons
              .Include(p => p.User)
              .ToListAsync();
    }

    public async Task<Person?> GetPersonByIdentification(string identification)
    {
        return await _projectDbContext.Persons
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Identificacion == identification);
    }

    public async Task<List<Person>> GetPersonByAge(int age)
    {
        return await _projectDbContext.Persons
            
            .Where(p => p.Fechanacimiento.HasValue && (DateTime.UtcNow.Year - p.Fechanacimiento.Value.Year) == age)
            .ToListAsync();
    }

    public async Task<List<Person>> GetPersonByPname(string pNombre)
    {
        return await _projectDbContext.Persons
            
            .Where(p => p.Pnombre.Contains(pNombre))
            .ToListAsync();
        
    }

    public async Task<List<Person>> GetPersonByPApellido(string pApellido)
    {
        return await _projectDbContext.Persons
            
            .Where(p => p.Papellido.Contains(pApellido))
            .ToListAsync();
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
       var person = await _projectDbContext.Persons.FindAsync(id);
        if (person == null) { return false; }
        _projectDbContext.Persons.Remove(person);
        return await _projectDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdatePersonAsync(Person person)
    {
        _projectDbContext.Persons.Update(person);
        return await _projectDbContext.SaveChangesAsync() > 0;
    }

    public async Task<User?> GetUserByPerson(int personId)
    {
        return await _projectDbContext.Users
            .FirstOrDefaultAsync(u => u.Idpersona == personId);
    }

    public async Task<bool> UpdatUserAsync(User user)
    {
        _projectDbContext.Users.Update(user);
        return await _projectDbContext.SaveChangesAsync() > 0;
    }

    public async Task SaveChangesAsync()
    {
         await _projectDbContext.SaveChangesAsync();
    }

    public async Task<List<Person>> GetPersonsWithLocationsAsync()
    {
        return await _projectDbContext.Persons
            .Include(p => p.Ubicacions)
            .Where(p => p.Ubicacions.Any())
            .ToListAsync();
    }

    public async Task<Person> GetPersonWithLocation(int id)
    {
        return await _projectDbContext.Persons
            .Include(p => p.Ubicacions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _projectDbContext.Users
              .ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _projectDbContext.Users
            .FirstOrDefaultAsync(p => p.Idpersona == id);
    }

  
}
