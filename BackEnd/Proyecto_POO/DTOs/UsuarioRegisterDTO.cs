namespace Proyecto_POO.DTOs
{
    public class UsuarioRegisterDTO
    {
        public string Identificacion { get; set; }
        public string Pnombre { get; set; }
        public string Snombre { get; set; }
        public string Papellido { get; set; }
        public string Sapellido { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Password { get; set; }
    }
}
