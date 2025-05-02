using System.Text.Json.Serialization;

namespace Proyecto_POO.Models;

public class Person
{
   
    public int Id { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string Pnombre { get; set; } = string.Empty;
    public string Snombre { get; set; } = string.Empty;
    public string Papellido { get; set; } = string.Empty;
    public string Sapellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public DateTime? Fechanacimiento { get; set; }
    public int Edad {  get; set; }
    public string Edadclinica {  get; set; } = string.Empty;

    [JsonIgnore]
    public virtual User? User { get; set; }

    public void CalcularEdades()
    {
        if (Fechanacimiento.HasValue) 
        {
            Edad = CalcularEdad(Fechanacimiento.Value);
            Edadclinica = CalcularEdadClinica(Fechanacimiento.Value);
        }
    }

    private int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        var edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        return edad;
    }



    private string CalcularEdadClinica(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        var anio = hoy.Year - fechaNacimiento.Year;
        var mes = hoy.Month - fechaNacimiento.Month;
        var dias = hoy.Day - fechaNacimiento.Day;

        if (dias < 0)
        {
            mes--;
            dias += DateTime.DaysInMonth(hoy.Year, hoy.Month);
        }
        if (mes < 0)
        {
            anio--;
            mes += 12;
        }
        return $"{anio} años {mes} meses {dias} días";
    }

    public ICollection<Ubicacion>Ubicacions { get;set; } = new List<Ubicacion>();


}


