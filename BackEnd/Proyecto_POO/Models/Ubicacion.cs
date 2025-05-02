namespace Proyecto_POO.Models;

    public class Ubicacion
    {
        public int Id { get; set; }
        public int Idpersona { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime Fecha { get; set; }


        public Person Person { get; set; }
    }

