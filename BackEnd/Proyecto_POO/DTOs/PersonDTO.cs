namespace Proyecto_POO.DTOs;

public class PersonDTO
{
    
    public string Identificacion { get; set; } = string.Empty;
    public string Pnombre { get; set; } = string.Empty;
    public string Snombre { get; set; } = string.Empty;
    public string Papellido { get; set; } = string.Empty;
    public string Sapellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? Fechanacimiento { get; set; }
}
