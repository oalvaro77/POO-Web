using System.Text.Json.Serialization;

namespace Proyecto_POO.Models
{
    public class User
    {
        public int Idpersona { get; set; }      
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty ;
        [JsonIgnore]
        public string ApiKey {  get; set; } = string.Empty ;
        [JsonIgnore]
        public  Person Person { get; set; }

    }
}
