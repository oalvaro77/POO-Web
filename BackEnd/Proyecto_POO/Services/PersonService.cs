using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository personRepository, IPasswordHasher passwordHasher, IMapper mapper)
        {   
            _personRepository = personRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<(User user, string password)> CrearPersona(PersonDTO personDTO)
        {
            var person = _mapper.Map<Person>(personDTO);
            person.CalcularEdades();
            await _personRepository.AddPersonAsync(person);
            

            var (user, password) = GenerarUsuario(person);
            person.User = user;
            await _personRepository.SaveChangesAsync();

            return (user, password);
        }

        public async Task<IEnumerable<PersonDTO>> ObtenerTodasLasPersonas()
        {
            var persons = await _personRepository.GetAllPersonsAsync();
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
        }

        public async Task<PersonDTO?> PersonaPorID(int id)
        {
            var person = await _personRepository.GetPersonByIdAsync(id);
            return person != null ? _mapper.Map<PersonDTO>(person) : null;
        }

        public async Task<PersonDTO?> PersonaPorIdentificacion(string identificacion)
        {
            var person = await _personRepository.GetPersonByIdentification(identificacion);
            return person != null ? _mapper.Map<PersonDTO?>(person) : null;
            //return _context.Persons.Include(p => p.User)
            //    .FirstOrDefault(p => p.Identificacion == identificacion);
        }

        public async Task<List<PersonDTO>> PersonaPorEdad(int edad)
        {
            var persons = await _personRepository.GetPersonByAge(edad);
            return _mapper.Map<List<PersonDTO>>(persons);
            //return _context.Persons.Include (_p => _p.User).Where(p => p.Fechanacimiento.HasValue && (DateTime.Now.Year - p.Fechanacimiento.Value.Year) == edad)
            //    .ToList();
        }

        public async Task<IEnumerable<PersonDTO>> PersonaPorPNombre(string Pnombre)
        {
            var persons = await _personRepository.GetPersonByPname(Pnombre);
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
        }

        public async Task<IEnumerable<PersonDTO>> PersonaPorApellido(string PApellido)
        {
            var persons = await _personRepository.GetPersonByPApellido(PApellido);
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
            //return _context.Persons.Include(p => p.User).Where(p => p.Papellido.Contains(PApellido))
            //    .ToList();
        }

        public async Task<bool> ActualizarPersona(Person person)
        {
            return await _personRepository.UpdatePersonAsync(person);
            
        }

        public async Task<bool> EliminarPersona(int id)
        {
            return await _personRepository.DeletePersonAsync(id);
        }

        public async Task<bool> CambiarPassword(int personaid, string newPasswrod)
        {
            var user = await _personRepository.GetUserByPerson(personaid);
            if (user == null) return false;
            user.Password = _passwordHasher.HashPassword(newPasswrod);
            return await _personRepository.UpdatUserAsync(user);
        }

        public async Task<UserDTO> GetUserDetails(int personid)
        {
            var user = await _personRepository.GetUserByPerson(personid);
            return _mapper.Map<UserDTO>(user);

        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var user = await _personRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(user);

        }


        public string GenerarLogin(Person person)
        {
            if(person == null)
                throw new ArgumentNullException(nameof(person));
            if (string.IsNullOrEmpty(person.Pnombre)) throw new ArgumentException("El nombre no puede estar vacio", nameof(person.Pnombre));
            if (string.IsNullOrEmpty(person.Papellido)) throw new ArgumentException("El apellido no puede estar vacio", nameof(person.Papellido));
            string login = $"{person.Pnombre}{person.Papellido[0]}{person.Id}";
            return (login);
        }

        public string GenerarPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public string GenerarApiKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public (User user, string plainPassword) GenerarUsuario(Person person)
        {
            var password = GenerarPassword();
            var hashedPassword = _passwordHasher.HashPassword(password);
            var apiKey = GenerarApiKey();

            var user = new User()
            {
                Idpersona = person.Id,
                Login = GenerarLogin(person),
                Password = hashedPassword,
                ApiKey = apiKey
            };

            return (user, password);
        }

    }
}
