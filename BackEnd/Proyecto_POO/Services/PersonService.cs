using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Data;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;

namespace Proyecto_POO.Services
{
    public class PersonService : IPersonService
    {
        private readonly ProjectDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public PersonService(ProjectDbContext context, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public (User user, string password) CrearPersona(PersonDTO personDTO)
        {
            var person = _mapper.Map<Person>(personDTO);
            person.CalcularEdades();
            _context.Persons.Add(person);
            _context.SaveChanges();

            var (user, password) = GenerarUsuario(person);
            person.User = user;
            _context.SaveChanges();

            return (user, password);
        }

        public IEnumerable<PersonDTO> ObtenerTodasLasPersonas()
        {
            var persons = _context.Persons.Include(p => p.User).ToList();
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
        }

        public PersonDTO? PersonaPorID(int id)
        {
            var person = _context.Persons.Include(p => p.User).FirstOrDefault(p => p.Id == id);
            return person != null ? _mapper.Map<PersonDTO>(person) : null;
        }

        public PersonDTO? PersonaPorIdentificacion(string identificacion)
        {
            var person = _context.Persons.Include(_p => _p.User).FirstOrDefault(p => p.Identificacion == identificacion);
            return person != null ? _mapper.Map<PersonDTO?>(person) : null;
            //return _context.Persons.Include(p => p.User)
            //    .FirstOrDefault(p => p.Identificacion == identificacion);
        }

        public IEnumerable<PersonDTO> PersonaPorEdad(int edad)
        {
            var persons = _context.Persons.Include(p => p.User).Where(p => p.Fechanacimiento.HasValue && (DateTime.Now.Year - p.Fechanacimiento.Value.Year) == edad).ToList();
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
            //return _context.Persons.Include (_p => _p.User).Where(p => p.Fechanacimiento.HasValue && (DateTime.Now.Year - p.Fechanacimiento.Value.Year) == edad)
            //    .ToList();
        }

        public IEnumerable<PersonDTO> PersonaPorPNombre(string Pnombre)
        {
            var persons = _context.Persons.Include(p => p.User).Where(p => p.Pnombre.Contains(Pnombre)).ToList();
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
        }

        public IEnumerable<PersonDTO> PersonaPorApellido(string PApellido)
        {
            var persons = _context.Persons.Include(p => p.User).Where(p => p.Papellido.Contains(PApellido)).ToList();
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
            //return _context.Persons.Include(p => p.User).Where(p => p.Papellido.Contains(PApellido))
            //    .ToList();
        }

        public bool ActualizarPersona(Person person)
        {
            _context.Persons.Update(person);
            return _context.SaveChanges() > 0;
        }

        public bool EliminarPersona(int id)
        {
            var person = _context.Persons.Find(id);
            if (person == null) return false;
            _context.Persons.Remove(person);
            return _context.SaveChanges() > 0;
        }

        public bool CambiarPassword(int personaid, string newPasswrod)
        {
            var user = _context.Users.FirstOrDefault(u => u.Idpersona == personaid);
            if (user == null) return false;
            user.Password = _passwordHasher.HashPassword(newPasswrod);
            return _context.SaveChanges() > 0;
        }

        public UserDTO GetUserDetails(int personid)
        {
            var user = _context.Users.Where(u => u.Idpersona == personid).FirstOrDefault();
            return _mapper.Map<UserDTO>(user);

        }



        public string GenerarLogin(Person person)
        {
            return $"{person.Pnombre}{person.Papellido[0]}{person.Id}";
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
